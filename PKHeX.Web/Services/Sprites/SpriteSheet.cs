using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace PKHeX.Web.Services.Sprites;

/// <summary>The atlas manifest, <c>sprites/manifest.json</c>, as PKHeX.Web.SpriteAtlas writes it.</summary>
/// <param name="Version">Layout version; see <see cref="SpriteSheet.ManifestVersion"/>.</param>
/// <param name="Image">The atlas file name, relative to <c>sprites/</c>: <c>pokemon.{hash}.png</c>.</param>
/// <param name="Stylesheet">The stylesheet file name, relative to <c>sprites/</c>: <c>sprites.{hash}.css</c>.</param>
/// <param name="CellWidth">Width of a cell.</param>
/// <param name="CellHeight">Height of a cell.</param>
/// <param name="Columns">Cells per row.</param>
/// <param name="Rows">Rows of cells.</param>
/// <param name="Sprites">Each resource name's entry.</param>
public sealed record SpriteManifest(int Version, string Image, string Stylesheet, int CellWidth, int CellHeight, int Columns, int Rows, Dictionary<string, SpriteManifestEntry> Sprites);

/// <summary>One image in the atlas: its cell and its size from the cell's top-left corner.</summary>
public sealed record SpriteManifestEntry(int Cell, int Width, int Height);

/// <summary>Reads the manifest without reflection, so trimming keeps what it needs.</summary>
[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(SpriteManifest))]
internal sealed partial class SpriteManifestJson : JsonSerializerContext;

/// <summary>
/// One slot's sprite, as atlas cells. Each cell is shown with the <c>sprite-c{cell}</c> class of the generated stylesheet.
/// </summary>
/// <param name="Layers">The images chosen, by resource name.</param>
/// <param name="Base">Cell of the species sprite.</param>
/// <param name="Unknown">Cell of the unknown mark, when laid over a default sprite standing in for a form.</param>
/// <param name="Egg">Cell of the egg icon, for an egg.</param>
/// <param name="Star">Cell of the shiny star, for a shiny entity.</param>
public sealed record SpriteCells(SpriteLayers Layers, int Base, int? Unknown, int? Egg, int? Star)
{
    /// <summary>
    /// True when the species image, with any unknown mark on it, is faded as one: an egg holding an item, whose egg icon covers it.
    /// The desktop fades the image it has already composed, so the two are faded together, not each on its own.
    /// </summary>
    public bool SpeciesFaded => Layers.EggMode == EggLayer.OverSpecies;

    /// <summary>The classes of the species layers' <c>img</c>s, bottom first: the species sprite, then any unknown mark.</summary>
    public IEnumerable<string> SpeciesClasses()
    {
        yield return $"sprite-c{Base}";
        if (Unknown is { } unknown)
        {
            yield return $"sprite-c{unknown} sprite-unknown";
        }
    }

    /// <summary>
    /// The classes of the overlays' <c>img</c>s, bottom first: the egg icon, then the shiny star. Each has the cell's class from the generated
    /// stylesheet plus its own class from app.css, which places and fades it as the desktop does.
    /// </summary>
    public IEnumerable<string> OverlayClasses()
    {
        if (Egg is { } egg)
        {
            yield return Layers.EggMode == EggLayer.AsItem ? $"sprite-c{egg} sprite-egg-item" : $"sprite-c{egg} sprite-egg";
        }
        if (Star is { } star)
        {
            yield return $"sprite-c{star} sprite-star";
        }
    }
}

/// <summary>
/// A validated atlas manifest: which images the published atlas holds, and where.
/// </summary>
public sealed partial class SpriteSheet
{
    /// <summary>The manifest layout this build reads.</summary>
    public const int ManifestVersion = 1;

    /// <summary>Folder of the published sprite files, relative to the app's base address.</summary>
    public const string Folder = "sprites/";

    /// <summary>The manifest: the one fixed name, which names the atlas and stylesheet of the same build.</summary>
    public const string ManifestPath = Folder + "manifest.json";

    private readonly Dictionary<string, SpriteManifestEntry> entries;

    private SpriteSheet(Dictionary<string, SpriteManifestEntry> entries, string image, string stylesheet)
    {
        this.entries = entries;
        ImagePath = Folder + image;
        StylesheetPath = Folder + stylesheet;
    }

    /// <summary>The atlas image. Every sprite is drawn from exactly this URL, the one preloaded at startup.</summary>
    public string ImagePath { get; }

    /// <summary>The generated stylesheet with one class per cell.</summary>
    public string StylesheetPath { get; }

    /// <summary>Number of images in the atlas.</summary>
    public int Count => entries.Count;

    /// <summary>Whether the atlas holds the image named <paramref name="name"/>.</summary>
    public bool Contains(string name) => entries.ContainsKey(name);

    /// <summary>
    /// Checks <paramref name="manifest"/> and returns the sheet it describes.
    /// </summary>
    /// <exception cref="InvalidDataException">
    /// The manifest has another version, names files other than a hashed atlas and stylesheet in <see cref="Folder"/>, has a cell outside the atlas or larger than a cell, two images in one cell, or lacks an image
    /// every sprite may need (<see cref="SpriteKeys.Fixed"/>).
    /// </exception>
    public static SpriteSheet From(SpriteManifest manifest)
    {
        if (manifest.Version != ManifestVersion)
        {
            throw new InvalidDataException($"The sprite manifest has version {manifest.Version}; this build reads {ManifestVersion}.");
        }
        // Only the generator's hashed names are accepted, so a manifest cannot point sprites at another path or host.
        if (manifest.Image is null || !ImageName().IsMatch(manifest.Image) || manifest.Stylesheet is null || !StylesheetName().IsMatch(manifest.Stylesheet))
        {
            throw new InvalidDataException("The sprite manifest names unexpected files.");
        }
        if (manifest.CellWidth <= 0 || manifest.CellHeight <= 0 || manifest.Columns <= 0 || manifest.Rows <= 0 || manifest.Sprites is null)
        {
            throw new InvalidDataException("The sprite manifest has an invalid layout.");
        }
        var cells = (long)manifest.Columns * manifest.Rows;
        var used = new HashSet<int>();
        foreach (var (name, entry) in manifest.Sprites)
        {
            if (entry is null || entry.Cell < 0 || entry.Cell >= cells || !used.Add(entry.Cell)
                || entry.Width <= 0 || entry.Height <= 0 || entry.Width > manifest.CellWidth || entry.Height > manifest.CellHeight)
            {
                throw new InvalidDataException($"The sprite manifest entry {name} is outside the atlas or overlaps another.");
            }
        }
        var missing = SpriteKeys.Fixed.Where(k => !manifest.Sprites.ContainsKey(k)).ToArray();
        if (missing.Length != 0)
        {
            throw new InvalidDataException($"The sprite manifest lacks {string.Join(", ", missing)}.");
        }
        return new SpriteSheet(new Dictionary<string, SpriteManifestEntry>(manifest.Sprites, StringComparer.Ordinal), manifest.Image, manifest.Stylesheet);
    }

    /// <summary>
    /// The cells of <paramref name="slot"/>'s sprite, or null when it has none: an empty position or a bad egg, whose contents are not trusted.
    /// </summary>
    public SpriteCells? Resolve(SlotSummary slot)
    {
        if (!slot.CanOpen)
        {
            return null;
        }
        if (SpriteKeys.Resolve(slot.Species, slot.Form, slot.Gender, slot.IsShiny, slot.IsEgg, slot.HoldsItem, Contains) is not { } layers)
        {
            return null;
        }
        return new SpriteCells(
            layers,
            entries[layers.Base].Cell,
            layers.UnknownForm ? entries[SpriteKeys.Unknown].Cell : null,
            layers.Egg is { } egg ? entries[egg].Cell : null,
            layers.Shiny ? entries[SpriteKeys.ShinyStar].Cell : null);
    }

    [GeneratedRegex("^pokemon\\.[0-9a-f]{16}\\.png$")]
    private static partial Regex ImageName();

    [GeneratedRegex("^sprites\\.[0-9a-f]{16}\\.css$")]
    private static partial Regex StylesheetName();
}
