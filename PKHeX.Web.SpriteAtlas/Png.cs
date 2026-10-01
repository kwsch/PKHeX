using System.Buffers.Binary;
using System.IO.Compression;

namespace PKHeX.Web.SpriteAtlas;

/// <summary>
/// An image as 8-bit RGBA samples, row by row, without padding: 4 bytes per pixel, straight (not premultiplied) alpha.
/// </summary>
/// <param name="Width">Width in pixels.</param>
/// <param name="Height">Height in pixels.</param>
/// <param name="Pixels">The samples, <c>Width * Height * 4</c> bytes.</param>
public sealed record RgbaImage(int Width, int Height, byte[] Pixels)
{
    /// <summary>Bytes per pixel.</summary>
    public const int BytesPerPixel = 4;

    /// <summary>Creates a fully transparent image.</summary>
    public static RgbaImage Blank(int width, int height) => new(width, height, new byte[checked(width * height * BytesPerPixel)]);

    /// <summary>Copies <paramref name="source"/> into this image with its top-left corner at (<paramref name="x"/>, <paramref name="y"/>).</summary>
    /// <exception cref="ArgumentOutOfRangeException">The source does not fit at that position.</exception>
    public void Blit(RgbaImage source, int x, int y)
    {
        if (x < 0 || y < 0 || x + source.Width > Width || y + source.Height > Height)
        {
            throw new ArgumentOutOfRangeException(nameof(source), $"A {source.Width}x{source.Height} image does not fit at ({x}, {y}) in {Width}x{Height}.");
        }
        var rowBytes = source.Width * BytesPerPixel;
        for (var row = 0; row < source.Height; row++)
        {
            source.Pixels.AsSpan(row * rowBytes, rowBytes).CopyTo(Pixels.AsSpan((((y + row) * Width) + x) * BytesPerPixel, rowBytes));
        }
    }

    /// <summary>Returns the <paramref name="width"/> by <paramref name="height"/> region with its top-left corner at (<paramref name="x"/>, <paramref name="y"/>).</summary>
    public RgbaImage Crop(int x, int y, int width, int height)
    {
        if (x < 0 || y < 0 || width < 0 || height < 0 || x + width > Width || y + height > Height)
        {
            throw new ArgumentOutOfRangeException(nameof(width), $"The region {width}x{height} at ({x}, {y}) is outside {Width}x{Height}.");
        }
        var result = Blank(width, height);
        var rowBytes = width * BytesPerPixel;
        for (var row = 0; row < height; row++)
        {
            Pixels.AsSpan((((y + row) * Width) + x) * BytesPerPixel, rowBytes).CopyTo(result.Pixels.AsSpan(row * rowBytes, rowBytes));
        }
        return result;
    }
}

/// <summary>
/// A minimal PNG codec for the sprite atlas: it decodes the formats PKHeX.Drawing.PokeSprite's images use and encodes RGBA.
/// </summary>
/// <remarks>
/// <para>
/// Decoding accepts non-interlaced images with a bit depth of 8 in every colour type (greyscale, RGB, palette, greyscale with alpha, RGBA),
/// including <c>tRNS</c> transparency, all five row filters, and multiple <c>IDAT</c> chunks. Every chunk CRC and the decompressed length
/// are checked. Anything else (other bit depths, interlacing, an unknown critical chunk) is refused rather than guessed at.
/// Ancillary chunks such as colour profiles and gamma are ignored, as browsers do for sprites without them; none of the sources carry one
/// that changes their samples.
/// </para>
/// <para>
/// Encoding writes only <c>IHDR</c>, one <c>IDAT</c> and <c>IEND</c>, choosing each row's filter with the usual minimum-sum-of-absolute-differences
/// heuristic and compressing with <see cref="ZLibStream"/>. There is no timestamp or text chunk, so the same pixels always give the same bytes
/// on the same .NET runtime.
/// </para>
/// </remarks>
public static class Png
{
    private static ReadOnlySpan<byte> Signature => [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A];

    private const byte ColorGray = 0;
    private const byte ColorRgb = 2;
    private const byte ColorPalette = 3;
    private const byte ColorGrayAlpha = 4;
    private const byte ColorRgba = 6;

    /// <summary>Largest width or height accepted, well above any sprite or atlas.</summary>
    private const int MaxDimension = 1 << 15;

    /// <summary>Decodes <paramref name="data"/> into RGBA samples.</summary>
    /// <exception cref="InvalidDataException">The data is not a PNG this codec supports, or is damaged.</exception>
    public static RgbaImage Decode(ReadOnlySpan<byte> data)
    {
        if (!data.StartsWith(Signature))
        {
            throw new InvalidDataException("Not a PNG: the signature is missing.");
        }

        var offset = Signature.Length;
        Header? header = null;
        byte[]? palette = null;
        byte[]? transparency = null;
        using var compressed = new MemoryStream();
        var ended = false;
        while (!ended)
        {
            if (offset + 12 > data.Length)
            {
                throw new InvalidDataException("The PNG ends before its IEND chunk.");
            }
            var length = BinaryPrimitives.ReadUInt32BigEndian(data[offset..]);
            if (length > int.MaxValue || offset + 12 + (long)length > data.Length)
            {
                throw new InvalidDataException("A PNG chunk runs past the end of the data.");
            }
            var type = data.Slice(offset + 4, 4);
            var body = data.Slice(offset + 8, (int)length);
            var storedCrc = BinaryPrimitives.ReadUInt32BigEndian(data[(offset + 8 + (int)length)..]);
            if (Crc32.Compute(data.Slice(offset + 4, 4 + (int)length)) != storedCrc)
            {
                throw new InvalidDataException($"The CRC of the {System.Text.Encoding.ASCII.GetString(type)} chunk does not match.");
            }
            offset += 12 + (int)length;

            var name = System.Text.Encoding.ASCII.GetString(type);
            if (header is null && name != "IHDR")
            {
                throw new InvalidDataException("The first PNG chunk is not IHDR.");
            }
            switch (name)
            {
                case "IHDR":
                    header = header is null ? ReadHeader(body) : throw new InvalidDataException("The PNG has more than one IHDR chunk.");
                    break;
                case "PLTE":
                    if (body.Length == 0 || body.Length % 3 != 0 || body.Length > 256 * 3)
                    {
                        throw new InvalidDataException("The PNG palette has an invalid length.");
                    }
                    palette = body.ToArray();
                    break;
                case "tRNS":
                    transparency = body.ToArray();
                    break;
                case "IDAT":
                    compressed.Write(body);
                    break;
                case "IEND":
                    ended = true;
                    break;
                default:
                    // The case of the first letter marks a chunk as critical: one that cannot be ignored without misreading the image.
                    if (char.IsAsciiLetterUpper(name[0]))
                    {
                        throw new InvalidDataException($"The PNG has an unsupported critical chunk {name}.");
                    }
                    break;
            }
        }

        var h = header!.Value;
        if (h.ColorType == ColorPalette && palette is null)
        {
            throw new InvalidDataException("The palette PNG has no PLTE chunk.");
        }
        var channels = Channels(h.ColorType);
        var stride = checked(h.Width * channels);
        var raw = Inflate(compressed, checked(h.Height * (stride + 1)));
        Unfilter(raw, h.Height, stride, channels);
        return ToRgba(raw, h, stride, palette, transparency);
    }

    /// <summary>Encodes <paramref name="image"/> as an 8-bit RGBA PNG.</summary>
    public static byte[] Encode(RgbaImage image)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(image.Width);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(image.Height);
        if (image.Pixels.Length != image.Width * image.Height * RgbaImage.BytesPerPixel)
        {
            throw new ArgumentException("The pixel buffer does not match the image size.", nameof(image));
        }

        using var output = new MemoryStream();
        output.Write(Signature);

        Span<byte> ihdr = stackalloc byte[13];
        BinaryPrimitives.WriteInt32BigEndian(ihdr, image.Width);
        BinaryPrimitives.WriteInt32BigEndian(ihdr[4..], image.Height);
        ihdr[8] = 8; // bit depth
        ihdr[9] = ColorRgba;
        ihdr[10] = 0; // deflate
        ihdr[11] = 0; // adaptive filtering
        ihdr[12] = 0; // not interlaced
        WriteChunk(output, "IHDR"u8, ihdr);

        var filtered = Filter(image);
        using (var idat = new MemoryStream())
        {
            using (var zlib = new ZLibStream(idat, CompressionLevel.SmallestSize, leaveOpen: true))
            {
                zlib.Write(filtered);
            }
            WriteChunk(output, "IDAT"u8, idat.ToArray());
        }
        WriteChunk(output, "IEND"u8, []);
        return output.ToArray();
    }

    private readonly record struct Header(int Width, int Height, byte ColorType);

    private static Header ReadHeader(ReadOnlySpan<byte> body)
    {
        if (body.Length != 13)
        {
            throw new InvalidDataException("The IHDR chunk has an invalid length.");
        }
        var width = BinaryPrimitives.ReadUInt32BigEndian(body);
        var height = BinaryPrimitives.ReadUInt32BigEndian(body[4..]);
        if (width is 0 or > MaxDimension || height is 0 or > MaxDimension)
        {
            throw new InvalidDataException($"The PNG size {width}x{height} is outside 1 to {MaxDimension}.");
        }
        var (depth, color, compression, filter, interlace) = (body[8], body[9], body[10], body[11], body[12]);
        if (depth != 8)
        {
            throw new InvalidDataException($"The PNG bit depth {depth} is not supported; only 8 is.");
        }
        if (color is not (ColorGray or ColorRgb or ColorPalette or ColorGrayAlpha or ColorRgba))
        {
            throw new InvalidDataException($"The PNG colour type {color} is invalid.");
        }
        if (compression != 0 || filter != 0)
        {
            throw new InvalidDataException("The PNG uses an unknown compression or filter method.");
        }
        if (interlace != 0)
        {
            throw new InvalidDataException("Interlaced PNGs are not supported.");
        }
        return new Header((int)width, (int)height, color);
    }

    private static int Channels(byte colorType) => colorType switch
    {
        ColorGray or ColorPalette => 1,
        ColorGrayAlpha => 2,
        ColorRgb => 3,
        _ => 4,
    };

    private static byte[] Inflate(MemoryStream compressed, int expected)
    {
        compressed.Position = 0;
        using var zlib = new ZLibStream(compressed, CompressionMode.Decompress);
        var raw = new byte[expected];
        try
        {
            zlib.ReadExactly(raw);
            if (zlib.ReadByte() != -1)
            {
                throw new InvalidDataException("The PNG image data is longer than its size allows.");
            }
        }
        catch (EndOfStreamException)
        {
            throw new InvalidDataException("The PNG image data is shorter than its size requires.");
        }
        return raw;
    }

    /// <summary>Reverses the row filters in place. Each row keeps its leading filter byte.</summary>
    private static void Unfilter(byte[] raw, int height, int stride, int bpp)
    {
        for (var row = 0; row < height; row++)
        {
            var start = (row * (stride + 1)) + 1;
            var line = raw.AsSpan(start, stride);
            Span<byte> prior = row == 0 ? [] : raw.AsSpan(start - stride - 1, stride);
            var filter = raw[start - 1];
            for (var i = 0; i < stride; i++)
            {
                int a = i >= bpp ? line[i - bpp] : 0;
                int b = prior.IsEmpty ? 0 : prior[i];
                int c = i >= bpp && !prior.IsEmpty ? prior[i - bpp] : 0;
                line[i] += filter switch
                {
                    0 => 0,
                    1 => (byte)a,
                    2 => (byte)b,
                    3 => (byte)((a + b) >> 1),
                    4 => Paeth(a, b, c),
                    _ => throw new InvalidDataException($"The PNG row filter {filter} is invalid."),
                };
            }
        }
    }

    private static byte Paeth(int a, int b, int c)
    {
        var p = a + b - c;
        var pa = Math.Abs(p - a);
        var pb = Math.Abs(p - b);
        var pc = Math.Abs(p - c);
        if (pa <= pb && pa <= pc)
        {
            return (byte)a;
        }
        return pb <= pc ? (byte)b : (byte)c;
    }

    private static RgbaImage ToRgba(byte[] raw, Header h, int stride, byte[]? palette, byte[]? transparency)
    {
        var image = RgbaImage.Blank(h.Width, h.Height);
        var px = image.Pixels;
        // For greyscale and RGB, tRNS names one colour (16-bit samples, the low byte used at depth 8) that is fully transparent.
        int? transparentGray = h.ColorType == ColorGray && transparency is { Length: 2 } ? transparency[1] : null;
        (byte R, byte G, byte B)? transparentRgb = h.ColorType == ColorRgb && transparency is { Length: 6 } ? (transparency[1], transparency[3], transparency[5]) : null;
        for (var y = 0; y < h.Height; y++)
        {
            var line = raw.AsSpan((y * (stride + 1)) + 1, stride);
            for (var x = 0; x < h.Width; x++)
            {
                var o = ((y * h.Width) + x) * RgbaImage.BytesPerPixel;
                switch (h.ColorType)
                {
                    case ColorGray:
                        px[o] = px[o + 1] = px[o + 2] = line[x];
                        px[o + 3] = line[x] == transparentGray ? (byte)0 : (byte)255;
                        break;
                    case ColorGrayAlpha:
                        px[o] = px[o + 1] = px[o + 2] = line[x * 2];
                        px[o + 3] = line[(x * 2) + 1];
                        break;
                    case ColorRgb:
                        (px[o], px[o + 1], px[o + 2]) = (line[x * 3], line[(x * 3) + 1], line[(x * 3) + 2]);
                        px[o + 3] = transparentRgb == (px[o], px[o + 1], px[o + 2]) ? (byte)0 : (byte)255;
                        break;
                    case ColorPalette:
                        var index = line[x];
                        if ((index * 3) + 2 >= palette!.Length)
                        {
                            throw new InvalidDataException($"The PNG uses palette entry {index}, which its palette does not have.");
                        }
                        (px[o], px[o + 1], px[o + 2]) = (palette[index * 3], palette[(index * 3) + 1], palette[(index * 3) + 2]);
                        // tRNS may be shorter than the palette; entries it does not cover are opaque.
                        px[o + 3] = transparency is not null && index < transparency.Length ? transparency[index] : (byte)255;
                        break;
                    default:
                        line.Slice(x * 4, 4).CopyTo(px.AsSpan(o, 4));
                        break;
                }
            }
        }
        return image;
    }

    /// <summary>Filters every row of <paramref name="image"/>, prefixing each with its filter type.</summary>
    private static byte[] Filter(RgbaImage image)
    {
        const int bpp = RgbaImage.BytesPerPixel;
        var stride = image.Width * bpp;
        var output = new byte[image.Height * (stride + 1)];
        var candidate = new byte[stride];
        var best = new byte[stride];
        for (var y = 0; y < image.Height; y++)
        {
            var line = image.Pixels.AsSpan(y * stride, stride);
            ReadOnlySpan<byte> prior = y == 0 ? [] : image.Pixels.AsSpan((y - 1) * stride, stride);
            var bestFilter = 0;
            var bestScore = long.MaxValue;
            for (var filter = 0; filter <= 4; filter++)
            {
                long score = 0;
                for (var i = 0; i < stride; i++)
                {
                    int a = i >= bpp ? line[i - bpp] : 0;
                    int b = prior.IsEmpty ? 0 : prior[i];
                    int c = i >= bpp && !prior.IsEmpty ? prior[i - bpp] : 0;
                    var value = (byte)(line[i] - filter switch
                    {
                        0 => 0,
                        1 => a,
                        2 => b,
                        3 => (a + b) >> 1,
                        _ => Paeth(a, b, c),
                    });
                    candidate[i] = value;
                    score += Math.Abs((int)(sbyte)value);
                }
                // Strictly lower only, so ties always resolve to the lowest filter number and the output stays deterministic.
                if (score < bestScore)
                {
                    bestScore = score;
                    bestFilter = filter;
                    candidate.CopyTo(best, 0);
                }
            }
            output[y * (stride + 1)] = (byte)bestFilter;
            best.CopyTo(output, (y * (stride + 1)) + 1);
        }
        return output;
    }

    private static void WriteChunk(Stream output, ReadOnlySpan<byte> type, ReadOnlySpan<byte> body)
    {
        Span<byte> word = stackalloc byte[4];
        BinaryPrimitives.WriteInt32BigEndian(word, body.Length);
        output.Write(word);
        var typed = new byte[4 + body.Length];
        type.CopyTo(typed);
        body.CopyTo(typed.AsSpan(4));
        output.Write(typed);
        BinaryPrimitives.WriteUInt32BigEndian(word, Crc32.Compute(typed));
        output.Write(word);
    }

    /// <summary>The CRC-32 PNG chunks carry (ISO 3309 polynomial, as in zlib).</summary>
    internal static class Crc32
    {
        private static readonly uint[] Table = BuildTable();

        private static uint[] BuildTable()
        {
            var table = new uint[256];
            for (uint n = 0; n < 256; n++)
            {
                var c = n;
                for (var k = 0; k < 8; k++)
                {
                    c = (c & 1) != 0 ? 0xEDB88320 ^ (c >> 1) : c >> 1;
                }
                table[n] = c;
            }
            return table;
        }

        /// <summary>CRC-32 of <paramref name="data"/>.</summary>
        public static uint Compute(ReadOnlySpan<byte> data)
        {
            var crc = 0xFFFFFFFFu;
            foreach (var b in data)
            {
                crc = Table[(crc ^ b) & 0xFF] ^ (crc >> 8);
            }
            return crc ^ 0xFFFFFFFFu;
        }
    }
}
