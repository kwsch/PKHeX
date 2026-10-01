using System.Globalization;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace PKHeX.Web.SpriteAtlas;

/// <summary>Where one image sits in the atlas.</summary>
/// <param name="Cell">Cell index, counted row by row from the top left.</param>
/// <param name="Width">Width of the image, which starts at the cell's top-left corner.</param>
/// <param name="Height">Height of the image.</param>
public readonly record struct AtlasEntry(int Cell, int Width, int Height);

/// <summary>The generated files, in memory.</summary>
/// <param name="Atlas">The atlas PNG.</param>
/// <param name="AtlasFileName">The atlas's published name, which carries a hash of its content.</param>
/// <param name="Stylesheet">One class per cell that shows that cell of the atlas in an <c>img</c> element.</param>
/// <param name="StylesheetFileName">The stylesheet's published name, which carries a hash of its content.</param>
/// <param name="Manifest">What the app reads: the names of the other two, the layout and each resource name's entry.</param>
/// <param name="Sources">Provenance: each resource name's source file and its SHA-256.</param>
/// <param name="Entries">Each resource name's entry, as in <paramref name="Manifest"/>.</param>
public sealed record AtlasFiles(byte[] Atlas, string AtlasFileName, string Stylesheet, string StylesheetFileName, string Manifest, string Sources, IReadOnlyDictionary<string, AtlasEntry> Entries)
{
    /// <summary>
    /// Every file <see cref="AtlasWriter.Write"/> creates: the atlas, and each text file with its Brotli (<c>.br</c>) and gzip (<c>.gz</c>) copies.
    /// </summary>
    public IReadOnlyList<string> FileNames =>
    [
        AtlasFileName,
        .. new[] { StylesheetFileName, AtlasWriter.ManifestFileName, AtlasWriter.SourcesFileName }.SelectMany(n => new[] { n, n + ".br", n + ".gz" }),
    ];
}

/// <summary>
/// Packs the chosen images into one atlas and writes the files PKHeX.Web publishes under <c>sprites/</c>.
/// </summary>
/// <remarks>
/// <para>
/// Images go into fixed cells in ordinal order of their resource names, each at its cell's top-left corner. The output depends only on the
/// source images and the list of names, so two runs give identical bytes; the Unit tier checks that.
/// </para>
/// <para>
/// Adding one image moves every later cell, so an atlas and a stylesheet from different builds must never be combined. Both are therefore
/// named by a hash of their content (<c>pokemon.{hash}.png</c>, <c>sprites.{hash}.css</c>), and only the manifest, which names them, has a fixed name.
/// A browser holding an earlier manifest asks for files that a new deployment no longer has, and falls back to text; it cannot mix the two.
/// </para>
/// </remarks>
public static class AtlasWriter
{
    /// <summary>Cell width: the desktop's sprite width.</summary>
    public const int CellWidth = 68;

    /// <summary>Cell height: the desktop's sprite height.</summary>
    public const int CellHeight = 56;

    /// <summary>Cells per row.</summary>
    public const int Columns = 32;

    /// <summary>Largest atlas side; browsers decode larger images, but some limit canvas and texture sizes to this.</summary>
    public const int MaxSide = 16384;

    /// <summary>Largest atlas file; the per-file limit of the most restrictive host considered (Cloudflare Pages).</summary>
    public const int MaxBytes = 25 * 1024 * 1024;

    /// <summary>Version of the manifest's layout, checked by the app.</summary>
    public const int ManifestVersion = 1;

    /// <summary>The manifest's published name, the one fixed name the app starts from.</summary>
    public const string ManifestFileName = "manifest.json";

    /// <summary>The provenance list's published name; it is never fetched by the app.</summary>
    public const string SourcesFileName = "sources.json";

    /// <summary>Hex digits of the content hash in the atlas and stylesheet names.</summary>
    public const int HashLength = 16;

    /// <summary>
    /// Builds the atlas from the images <paramref name="names"/> refers to in <paramref name="index"/>.
    /// </summary>
    /// <param name="index">The desktop's images.</param>
    /// <param name="names">Resource names to pack.</param>
    /// <param name="sourceRoot">Directory the provenance paths are written relative to, e.g. the PokeSprite project.</param>
    /// <exception cref="InvalidDataException">An image cannot be read, does not fit a cell, or the atlas exceeds a limit.</exception>
    public static AtlasFiles Build(ResxSpriteIndex index, IReadOnlyList<string> names, string sourceRoot)
    {
        var ordered = names.Distinct(StringComparer.Ordinal).Order(StringComparer.Ordinal).ToArray();
        if (ordered.Length == 0)
        {
            throw new InvalidDataException("There are no images to pack.");
        }
        var rows = (ordered.Length + Columns - 1) / Columns;
        var (width, height) = (Columns * CellWidth, rows * CellHeight);
        if (width > MaxSide || height > MaxSide)
        {
            throw new InvalidDataException($"The atlas would be {width}x{height}, over the {MaxSide} pixel limit.");
        }

        var atlas = RgbaImage.Blank(width, height);
        var entries = new Dictionary<string, AtlasEntry>(StringComparer.Ordinal);
        var sources = new List<(string Name, string Path, string Hash)>();
        for (var cell = 0; cell < ordered.Length; cell++)
        {
            var name = ordered[cell];
            var path = index.PathOf(name);
            var bytes = File.ReadAllBytes(path);
            RgbaImage image;
            try
            {
                image = Png.Decode(bytes);
            }
            catch (InvalidDataException e)
            {
                throw new InvalidDataException($"{name} ({path}): {e.Message}", e);
            }
            if (image.Width > CellWidth || image.Height > CellHeight)
            {
                throw new InvalidDataException($"{name} is {image.Width}x{image.Height}, larger than a {CellWidth}x{CellHeight} cell.");
            }
            atlas.Blit(image, (cell % Columns) * CellWidth, (cell / Columns) * CellHeight);
            entries.Add(name, new AtlasEntry(cell, image.Width, image.Height));
            var relative = Path.GetRelativePath(sourceRoot, path).Replace(Path.DirectorySeparatorChar, '/');
            sources.Add((name, relative, Convert.ToHexStringLower(SHA256.HashData(bytes))));
        }

        var png = Png.Encode(atlas);
        if (png.Length > MaxBytes)
        {
            throw new InvalidDataException($"The atlas is {png.Length} bytes, over the {MaxBytes} byte limit.");
        }
        var atlasName = $"pokemon.{ContentHash(png)}.png";
        var stylesheet = Stylesheet(entries);
        var stylesheetName = $"sprites.{ContentHash(Utf8NoBom.GetBytes(stylesheet))}.css";
        return new AtlasFiles(png, atlasName, stylesheet, stylesheetName, Manifest(entries, rows, atlasName, stylesheetName), Sources(sources), entries);
    }

    /// <summary>The first <see cref="HashLength"/> hex digits of the SHA-256 of <paramref name="data"/>.</summary>
    private static string ContentHash(byte[] data) => Convert.ToHexStringLower(SHA256.HashData(data))[..HashLength];

    /// <summary>
    /// Writes <paramref name="files"/> into <paramref name="directory"/>, creating it if needed. Each text file also gets Brotli (<c>.br</c>) and
    /// gzip (<c>.gz</c>) copies, as the publish makes for the app's own assets, for hosts that serve precompressed files. The atlas does not:
    /// PNG data is already compressed.
    /// </summary>
    /// <remarks>
    /// Files an earlier run wrote there, hashed or not, are removed first: their names differ, and the build publishes everything in the directory.
    /// Other files are left alone.
    /// </remarks>
    public static void Write(AtlasFiles files, string directory)
    {
        Directory.CreateDirectory(directory);
        foreach (var stale in new[] { "pokemon*.png", "sprites*.css*", ManifestFileName + "*", SourcesFileName + "*" }.SelectMany(p => Directory.GetFiles(directory, p)))
        {
            File.Delete(stale);
        }
        File.WriteAllBytes(Path.Combine(directory, files.AtlasFileName), files.Atlas);
        WriteText(directory, files.StylesheetFileName, files.Stylesheet);
        WriteText(directory, ManifestFileName, files.Manifest);
        WriteText(directory, SourcesFileName, files.Sources);
    }

    private static readonly UTF8Encoding Utf8NoBom = new(false);

    private static void WriteText(string directory, string name, string text)
    {
        var bytes = Utf8NoBom.GetBytes(text);
        File.WriteAllBytes(Path.Combine(directory, name), bytes);
        File.WriteAllBytes(Path.Combine(directory, name + ".br"), Compress(bytes, s => new BrotliStream(s, CompressionLevel.SmallestSize, leaveOpen: true)));
        // GZipStream writes no file name and a zero timestamp, so the copy is as reproducible as the file.
        File.WriteAllBytes(Path.Combine(directory, name + ".gz"), Compress(bytes, s => new GZipStream(s, CompressionLevel.SmallestSize, leaveOpen: true)));
    }

    private static byte[] Compress(byte[] data, Func<Stream, Stream> compressor)
    {
        using var output = new MemoryStream();
        using (var stream = compressor(output))
        {
            stream.Write(data);
        }
        return output.ToArray();
    }

    /// <summary>
    /// One rule per cell. With the <c>img</c> sized to the image and <c>object-fit: none</c> (set in app.css), <c>object-position</c>
    /// shifts the atlas so only that cell shows. Classes are used because the Content Security Policy blocks inline styles.
    /// </summary>
    private static string Stylesheet(Dictionary<string, AtlasEntry> entries)
    {
        var css = new StringBuilder();
        css.Append("/* Generated by PKHeX.Web.SpriteAtlas from PKHeX.Drawing.PokeSprite's images; do not edit. */\n");
        foreach (var entry in entries.Values.OrderBy(e => e.Cell))
        {
            var x = (entry.Cell % Columns) * CellWidth;
            var y = (entry.Cell / Columns) * CellHeight;
            css.Append(CultureInfo.InvariantCulture, $".sprite-c{entry.Cell}{{object-position:{Offset(x)} {Offset(y)};width:{entry.Width}px;height:{entry.Height}px}}\n");
        }
        return css.ToString();
    }

    private static string Offset(int value) => value == 0 ? "0" : $"-{value.ToString(CultureInfo.InvariantCulture)}px";

    private static string Manifest(Dictionary<string, AtlasEntry> entries, int rows, string atlasName, string stylesheetName)
    {
        using var buffer = new MemoryStream();
        using (var json = new Utf8JsonWriter(buffer))
        {
            json.WriteStartObject();
            json.WriteNumber("version", ManifestVersion);
            json.WriteString("image", atlasName);
            json.WriteString("stylesheet", stylesheetName);
            json.WriteNumber("cellWidth", CellWidth);
            json.WriteNumber("cellHeight", CellHeight);
            json.WriteNumber("columns", Columns);
            json.WriteNumber("rows", rows);
            json.WriteStartObject("sprites");
            foreach (var (name, entry) in entries.OrderBy(e => e.Key, StringComparer.Ordinal))
            {
                json.WriteStartObject(name);
                json.WriteNumber("cell", entry.Cell);
                json.WriteNumber("width", entry.Width);
                json.WriteNumber("height", entry.Height);
                json.WriteEndObject();
            }
            json.WriteEndObject();
            json.WriteEndObject();
        }
        return Encoding.UTF8.GetString(buffer.ToArray()) + "\n";
    }

    private static string Sources(List<(string Name, string Path, string Hash)> sources)
    {
        using var buffer = new MemoryStream();
        // A fixed newline, so the file is the same whichever system generates it.
        using (var json = new Utf8JsonWriter(buffer, new JsonWriterOptions { Indented = true, NewLine = "\n" }))
        {
            json.WriteStartObject();
            json.WriteString("generator", "PKHeX.Web.SpriteAtlas");
            json.WriteString("source", "PKHeX.Drawing.PokeSprite (resource name, file relative to that project, SHA-256 of the file)");
            json.WriteStartObject("sprites");
            foreach (var (name, path, hash) in sources.OrderBy(s => s.Name, StringComparer.Ordinal))
            {
                json.WriteStartObject(name);
                json.WriteString("file", path);
                json.WriteString("sha256", hash);
                json.WriteEndObject();
            }
            json.WriteEndObject();
            json.WriteEndObject();
        }
        return Encoding.UTF8.GetString(buffer.ToArray()) + "\n";
    }
}
