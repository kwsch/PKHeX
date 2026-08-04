using System.Collections.Concurrent;
using System.Reflection;
using System.Text;
using SkiaSharp;
using PKHeX.Core;

namespace PKHeX.Avalonia.Services;

/// <summary>
/// Loads Pokémon sprites from embedded resources using SkiaSharp.
/// Mirrors the naming conventions from PKHeX.Drawing.PokeSprite.
/// </summary>
public sealed class SpriteLoader
{
    private readonly Assembly _assembly;
    private readonly ConcurrentDictionary<string, SKBitmap?> _cache = new();
    private readonly HashSet<string> _availableResources;

    private const string ImagePrefix = "PKHeX.Avalonia.Assets.Images.";
    private const string ItemPrefix = ImagePrefix + "Big_Items.";
    private const string OverlayPrefix = ImagePrefix + "Pokemon_Sprite_Overlays.";

    /// <summary>Active sprite style; set per loaded save by <see cref="AvaloniaSpriteRenderer"/>.</summary>
    public SpriteStyle Style { get; set; } = SpriteStyle.Classic;

    private readonly record struct StyleResources(string NormalFolder, string? ShinyFolder, char Letter, string EggFile);

    private static StyleResources GetStyleResources(SpriteStyle style) => style switch
    {
        SpriteStyle.Mugshot => new StyleResources("Legends_Arceus_Sprites.", "Legends_Arceus_Shiny_Sprites.", 'c', "c_egg.png"),
        SpriteStyle.Artwork => new StyleResources("Artwork_Pokemon_Sprites.", "Artwork_Shiny_Sprites.", 'a', "a_egg.png"),
        _ => new StyleResources("Big_Pokemon_Sprites.", "Big_Shiny_Sprites.", 'b', "b_egg.png"),
    };

    // Species that show default sprite regardless of form
    private static readonly HashSet<ushort> SpeciesDefaultFormSprite =
    [
        (ushort)Species.Mothim,
        (ushort)Species.Scatterbug,
        (ushort)Species.Spewpa,
        (ushort)Species.Rockruff,
        (ushort)Species.Mimikyu,
        (ushort)Species.Sinistea,
        (ushort)Species.Polteageist,
        (ushort)Species.Urshifu,
        (ushort)Species.Dudunsparce,
        (ushort)Species.Poltchageist,
        (ushort)Species.Sinistcha,
    ];

    // Species with gender-specific sprites
    private static readonly HashSet<ushort> SpeciesGenderedSprite =
    [
        (ushort)Species.Hippopotas,
        (ushort)Species.Hippowdon,
        (ushort)Species.Unfezant,
        (ushort)Species.Frillish,
        (ushort)Species.Jellicent,
        (ushort)Species.Pyroar,
    ];

    public SpriteLoader()
    {
        _assembly = Assembly.GetExecutingAssembly();
        _availableResources = new HashSet<string>(_assembly.GetManifestResourceNames());
    }

    public SKBitmap? GetSprite(ushort species, byte form, byte gender, uint formarg, bool shiny, EntityContext context)
    {
        if (species == 0)
            return null;

        var resourceName = GetResourceName(species, form, gender, formarg, shiny, context);

        if (_cache.TryGetValue(resourceName, out var cached))
            return cached;

        var bitmap = LoadSprite(resourceName);

        // If shiny not found, try non-shiny
        if (bitmap is null && shiny)
        {
            var nonShinyName = GetResourceName(species, form, gender, formarg, false, context);
            bitmap = LoadSprite(nonShinyName);
        }

        // If form not found, try base form
        if (bitmap is null && form != 0)
        {
            var baseFormName = GetResourceName(species, 0, gender, 0, shiny, context);
            bitmap = LoadSprite(baseFormName);

            if (bitmap is null && shiny)
            {
                baseFormName = GetResourceName(species, 0, gender, 0, false, context);
                bitmap = LoadSprite(baseFormName);
            }
        }

        // Ultimate fallback: just species, in the active style
        if (bitmap is null)
        {
            var res = GetStyleResources(Style);
            var speciesOnlyName = $"{ImagePrefix}{res.NormalFolder}{res.Letter}_{species}.png";
            bitmap = LoadSprite(speciesOnlyName);
        }

        _cache[resourceName] = bitmap;
        return bitmap;
    }

    public SKBitmap? GetShinyOverlay()
    {
        return LoadFromPrefix(OverlayPrefix, "rare_icon_alt.png");
    }

    public SKBitmap? GetEggSprite(ushort species)
    {
        var res = GetStyleResources(Style);
        var folder = $"{ImagePrefix}{res.NormalFolder}";

        // Manaphy has a special egg sprite, only present in the classic set.
        if (species == (ushort)Species.Manaphy && Style == SpriteStyle.Classic)
            return LoadFromPrefix(folder, "b_490_e.png");

        return LoadFromPrefix(folder, res.EggFile);
    }

    public SKBitmap? GetItemSprite(int itemId)
    {
        if (itemId <= 0)
            return null;

        var resourceName = $"{ItemPrefix}bitem_{itemId}.png";
        return LoadSprite(resourceName);
    }

    private string GetResourceName(ushort species, byte form, byte gender, uint formarg, bool shiny, EntityContext context)
    {
        var res = GetStyleResources(Style);
        var spriteName = GetSpriteName(species, form, gender, formarg, context);
        var useShiny = shiny && res.ShinyFolder is not null;
        var folder = useShiny ? res.ShinyFolder! : res.NormalFolder;
        var shinySuffix = useShiny ? "s" : string.Empty;
        return $"{ImagePrefix}{folder}{res.Letter}{spriteName}{shinySuffix}.png";
    }

    private static string GetSpriteName(ushort species, byte form, byte gender, uint formarg, EntityContext context)
    {
        // Species that always show default form
        if (SpeciesDefaultFormSprite.Contains(species))
            form = 0;

        var sb = new StringBuilder(16);
        sb.Append('_').Append(species);

        if (form != 0)
        {
            sb.Append('-').Append(form);

            // Pikachu special forms
            if (species == (ushort)Species.Pikachu)
            {
                if (context == EntityContext.Gen6)
                    sb.Append('c'); // Cosplay
                else if (form == 8)
                    sb.Append('p'); // Let's Go starter
            }
            // Eevee Let's Go starter
            else if (species == (ushort)Species.Eevee && form == 1)
            {
                sb.Append('p');
            }
        }

        // Gender-specific sprites
        if (gender == 1 && SpeciesGenderedSprite.Contains(species))
            sb.Append('f');

        // Alcremie has both form and formarg
        if (species == (ushort)Species.Alcremie)
        {
            if (form == 0)
                sb.Append('-').Append(form);
            sb.Append('-').Append(formarg);
        }

        return sb.ToString();
    }

    private SKBitmap? LoadFromPrefix(string prefix, string fileName)
    {
        var resourceName = $"{prefix}{fileName}";
        return LoadSprite(resourceName);
    }

    private SKBitmap? LoadSprite(string resourceName)
    {
        if (!_availableResources.Contains(resourceName))
            return null;

        try
        {
            using var stream = _assembly.GetManifestResourceStream(resourceName);
            if (stream is null)
                return null;

            return SKBitmap.Decode(stream);
        }
        catch
        {
            return null;
        }
    }

    public void ClearCache()
    {
        foreach (var bitmap in _cache.Values)
            bitmap?.Dispose();
        _cache.Clear();
    }
}
