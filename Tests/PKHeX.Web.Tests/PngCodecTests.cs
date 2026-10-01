extern alias atlas;

using System.Buffers.Binary;
using System.IO.Compression;
using FluentAssertions;
using atlas::PKHeX.Web.SpriteAtlas;
using Xunit;

namespace PKHeX.Web.Tests;

/// <summary>
/// The atlas generator's PNG codec decodes every format the desktop's sprites use and writes PNGs that decode to the same pixels.
/// </summary>
/// <remarks>
/// Test images are assembled here byte by byte, with row filters worked out by hand, so the decoder is checked against the PNG
/// specification rather than against its own encoder. The E2E tier also compares atlas cells with the browser's decoding of the sources.
/// </remarks>
[Trait(TestCategory.Name, TestCategory.Unit)]
public sealed class PngCodecTests
{
    private const byte Gray = 0;
    private const byte Rgb = 2;
    private const byte Palette = 3;
    private const byte GrayAlpha = 4;
    private const byte Rgba = 6;

    [Fact]
    public void ChunkCrcMatchesTheSpecification()
    {
        // Every PNG ends with this IEND chunk, whose CRC is fixed.
        Png.Crc32.Compute("IEND"u8).Should().Be(0xAE426082);
    }

    /// <summary>
    /// A 3x2 greyscale image: row 0 [10, 20, 30] unfiltered, row 1 [15, 40, 50] stored with <paramref name="filter"/>.
    /// The filtered bytes are worked out by hand from the PNG specification.
    /// </summary>
    [Theory]
    [InlineData(0, new byte[] { 15, 40, 50 })]
    [InlineData(1, new byte[] { 15, 25, 10 })] // Sub: minus the pixel to the left
    [InlineData(2, new byte[] { 5, 20, 20 })] // Up: minus the pixel above
    [InlineData(3, new byte[] { 10, 23, 15 })] // Average: minus floor((left + above) / 2)
    [InlineData(4, new byte[] { 5, 20, 10 })] // Paeth: minus whichever of left, above, upper-left is closest to left + above - upper-left
    public void DecodesEveryRowFilter(byte filter, byte[] filtered)
    {
        byte[] raw = [0, 10, 20, 30, filter, .. filtered];
        var image = Png.Decode(Build(3, 2, Gray, raw));

        Channel(image, 0).Should().Equal(10, 20, 30, 15, 40, 50);
        Channel(image, 3).Should().OnlyContain(a => a == 255);
    }

    /// <summary>
    /// Paeth's tie-break: row 0 [10, 30], row 1 [0, 35] stored with Paeth. For the second pixel left = 0, above = 30, upper-left = 10,
    /// so the estimate 20 is equally far from above and upper-left, and the specification picks above (35 - 30 = 5), not upper-left.
    /// The first pixel has only above (10), so it is stored as 0 - 10 = 246.
    /// </summary>
    [Fact]
    public void PaethPrefersAboveOverUpperLeftOnATie()
    {
        var image = Png.Decode(Build(2, 2, Gray, [0, 10, 30, 4, 246, 5]));

        Channel(image, 0).Should().Equal(10, 30, 0, 35);
    }

    [Fact]
    public void DecodesGreyscaleWithATransparentValue()
    {
        var image = Png.Decode(Build(2, 1, Gray, [0, 7, 200], trns: [0, 200]));

        image.Pixels.Should().Equal(7, 7, 7, 255, 200, 200, 200, 0);
    }

    [Fact]
    public void DecodesRgbWithATransparentColour()
    {
        var image = Png.Decode(Build(2, 1, Rgb, [0, 1, 2, 3, 4, 5, 6], trns: [0, 4, 0, 5, 0, 6]));

        image.Pixels.Should().Equal(1, 2, 3, 255, 4, 5, 6, 0);
    }

    [Fact]
    public void DecodesAPaletteWithPartialTransparency()
    {
        // tRNS covers only entry 0; entry 1 is therefore opaque.
        var image = Png.Decode(Build(2, 1, Palette, [0, 0, 1], plte: [9, 8, 7, 1, 2, 3], trns: [0]));

        image.Pixels.Should().Equal(9, 8, 7, 0, 1, 2, 3, 255);
    }

    [Fact]
    public void DecodesGreyscaleWithAlpha()
    {
        var image = Png.Decode(Build(2, 1, GrayAlpha, [0, 50, 128, 60, 255]));

        image.Pixels.Should().Equal(50, 50, 50, 128, 60, 60, 60, 255);
    }

    [Fact]
    public void DecodesRgba()
    {
        var image = Png.Decode(Build(1, 1, Rgba, [0, 1, 2, 3, 4]));

        image.Pixels.Should().Equal(1, 2, 3, 4);
    }

    [Fact]
    public void DecodesImageDataSplitAcrossChunks()
    {
        var data = Compress([0, 10, 20, 30]);
        var png = Assemble(Header(3, 1, Gray), ("IDAT", data[..3]), ("IDAT", data[3..]), ("IEND", []));

        Channel(Png.Decode(png), 0).Should().Equal(10, 20, 30);
    }

    [Fact]
    public void EncodedImagesDecodeToTheSamePixels()
    {
        var random = new Random(1234);
        var image = RgbaImage.Blank(37, 11);
        random.NextBytes(image.Pixels);
        // Flat and repeated areas too, so every filter gets chosen somewhere.
        image.Pixels.AsSpan(0, 37 * 4 * 3).Clear();

        var png = Png.Encode(image);

        Png.Decode(png).Pixels.Should().Equal(image.Pixels);
        Png.Encode(image).Should().Equal(png, "the same pixels always encode to the same bytes");
    }

    [Fact]
    public void EncodesOnlyTheRequiredChunks()
    {
        var png = Png.Encode(RgbaImage.Blank(2, 2));

        ChunkTypes(png).Should().Equal("IHDR", "IDAT", "IEND");
    }

    [Fact]
    public void RejectsADamagedChunk()
    {
        var png = Build(1, 1, Rgba, [0, 1, 2, 3, 4]);
        png[^1] ^= 1; // the last byte of the IEND CRC

        var decode = () => Png.Decode(png);
        decode.Should().Throw<InvalidDataException>().WithMessage("*CRC*");
    }

    [Fact]
    public void RejectsSixteenBitSamples()
    {
        var header = Header(1, 1, Rgba);
        header[8] = 16;
        var png = Assemble(header, ("IDAT", Compress([0, 0, 0, 0, 0, 0, 0, 0, 0])), ("IEND", []));

        var decode = () => Png.Decode(png);
        decode.Should().Throw<InvalidDataException>().WithMessage("*bit depth*");
    }

    [Fact]
    public void RejectsInterlacing()
    {
        var header = Header(1, 1, Rgba);
        header[12] = 1;
        var png = Assemble(header, ("IDAT", Compress([0, 1, 2, 3, 4])), ("IEND", []));

        var decode = () => Png.Decode(png);
        decode.Should().Throw<InvalidDataException>().WithMessage("*Interlaced*");
    }

    [Fact]
    public void RejectsAnUnknownCriticalChunk()
    {
        var png = Assemble(Header(1, 1, Rgba), ("ABCD", [1]), ("IDAT", Compress([0, 1, 2, 3, 4])), ("IEND", []));

        var decode = () => Png.Decode(png);
        decode.Should().Throw<InvalidDataException>().WithMessage("*critical chunk ABCD*");
    }

    [Fact]
    public void IgnoresAncillaryChunks()
    {
        var png = Assemble(Header(1, 1, Rgba), ("tEXt", "Comment\0x"u8.ToArray()), ("IDAT", Compress([0, 1, 2, 3, 4])), ("IEND", []));

        Png.Decode(png).Pixels.Should().Equal(1, 2, 3, 4);
    }

    [Theory]
    [InlineData(new byte[] { 0, 1, 2, 3 })] // one byte short
    [InlineData(new byte[] { 0, 1, 2, 3, 4, 5 })] // one byte long
    public void RejectsImageDataOfTheWrongLength(byte[] raw)
    {
        var decode = () => Png.Decode(Build(1, 1, Rgba, raw));
        decode.Should().Throw<InvalidDataException>();
    }

    [Fact]
    public void RejectsAnInvalidRowFilter()
    {
        var decode = () => Png.Decode(Build(1, 1, Rgba, [5, 1, 2, 3, 4]));
        decode.Should().Throw<InvalidDataException>().WithMessage("*filter 5*");
    }

    [Fact]
    public void RejectsAPaletteIndexOutsideThePalette()
    {
        var decode = () => Png.Decode(Build(1, 1, Palette, [0, 2], plte: [1, 2, 3, 4, 5, 6]));
        decode.Should().Throw<InvalidDataException>().WithMessage("*palette entry 2*");
    }

    [Fact]
    public void RejectsDataThatIsNotAPng()
    {
        var decode = () => Png.Decode("GIF89a"u8);
        decode.Should().Throw<InvalidDataException>().WithMessage("*signature*");
    }

    [Fact]
    public void DecodesEveryImageThatGoesIntoTheAtlas()
    {
        foreach (var name in SpriteSelection.Select(SpriteFixtures.Index))
        {
            var image = Png.Decode(File.ReadAllBytes(SpriteFixtures.Index.PathOf(name)));
            image.Width.Should().BeInRange(1, AtlasWriter.CellWidth, name);
            image.Height.Should().BeInRange(1, AtlasWriter.CellHeight, name);
        }
    }

    private static byte[] Channel(RgbaImage image, int channel) => [.. image.Pixels.Where((_, i) => i % 4 == channel)];

    private static byte[] Header(int width, int height, byte colorType)
    {
        var header = new byte[13];
        BinaryPrimitives.WriteInt32BigEndian(header, width);
        BinaryPrimitives.WriteInt32BigEndian(header.AsSpan(4), height);
        header[8] = 8;
        header[9] = colorType;
        return header;
    }

    /// <summary>A complete PNG of <paramref name="raw"/> scanlines (each starting with its filter byte), with optional PLTE and tRNS.</summary>
    private static byte[] Build(int width, int height, byte colorType, byte[] raw, byte[]? plte = null, byte[]? trns = null)
    {
        var chunks = new List<(string, byte[])>();
        if (plte is not null)
        {
            chunks.Add(("PLTE", plte));
        }
        if (trns is not null)
        {
            chunks.Add(("tRNS", trns));
        }
        chunks.Add(("IDAT", Compress(raw)));
        chunks.Add(("IEND", []));
        return Assemble(Header(width, height, colorType), [.. chunks]);
    }

    private static byte[] Assemble(byte[] header, params (string Type, byte[] Body)[] chunks)
    {
        using var png = new MemoryStream();
        png.Write([0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A]);
        foreach (var (type, body) in chunks.Prepend(("IHDR", header)))
        {
            var length = new byte[4];
            BinaryPrimitives.WriteInt32BigEndian(length, body.Length);
            png.Write(length);
            byte[] typed = [.. System.Text.Encoding.ASCII.GetBytes(type), .. body];
            png.Write(typed);
            var crc = new byte[4];
            BinaryPrimitives.WriteUInt32BigEndian(crc, Png.Crc32.Compute(typed));
            png.Write(crc);
        }
        return png.ToArray();
    }

    private static byte[] Compress(byte[] raw)
    {
        using var output = new MemoryStream();
        using (var zlib = new ZLibStream(output, CompressionLevel.Optimal, leaveOpen: true))
        {
            zlib.Write(raw);
        }
        return output.ToArray();
    }

    private static List<string> ChunkTypes(byte[] png)
    {
        var types = new List<string>();
        for (var offset = 8; offset < png.Length;)
        {
            var length = BinaryPrimitives.ReadInt32BigEndian(png.AsSpan(offset));
            types.Add(System.Text.Encoding.ASCII.GetString(png, offset + 4, 4));
            offset += 12 + length;
        }
        return types;
    }
}
