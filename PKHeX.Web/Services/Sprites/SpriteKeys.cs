using PKHeX.Core;
using PKHeX.Drawing.PokeSprite;

namespace PKHeX.Web.Services.Sprites;

/// <summary>How an egg is drawn over its species sprite.</summary>
public enum EggLayer
{
    /// <summary>Not an egg.</summary>
    None,

    /// <summary>The species at full opacity, with the egg icon where a held item would go (the desktop default for an egg holding nothing).</summary>
    AsItem,

    /// <summary>The species faded, with the egg icon over all of it (an egg that holds an item, whose icon would take that place).</summary>
    OverSpecies,
}

/// <summary>
/// The images that make up one slot's sprite, as resource names of PKHeX.Drawing.PokeSprite, bottom layer first.
/// </summary>
/// <param name="Base">The species sprite.</param>
/// <param name="UnknownForm">True when <see cref="Base"/> is the species' default sprite standing in for a form that has none; the unknown mark is laid over it, half transparent.</param>
/// <param name="EggMode">Whether, and how, the egg icon is laid over the species.</param>
/// <param name="Egg">The egg icon, or null when <see cref="EggMode"/> is <see cref="EggLayer.None"/>.</param>
/// <param name="Shiny">True when the shiny star is laid over the top-left corner.</param>
public sealed record SpriteLayers(string Base, bool UnknownForm, EggLayer EggMode, string? Egg, bool Shiny);

/// <summary>
/// Chooses the sprite images for an entity the way the PKHeX desktop does for X/Y and Omega Ruby/Alpha Sapphire saves.
/// </summary>
/// <remarks>
/// <para>
/// The desktop suggests its classic 68x56 sprites for these saves (<c>SpriteBuilderUtil.GetSuggestedMode</c>), drawn by <c>SpriteBuilder5668s</c>.
/// This repeats that builder's choices in the Generation 6 context, without any drawing:
/// the <c>b</c> resource for the species, form, gender and shininess (<see cref="SpriteName.GetResourceStringSprite"/>), then the <c>c</c> resource;
/// for a shiny entity with no shiny image, the same again without shininess; then the species' default sprite with the unknown mark over it;
/// and finally the unknown mark alone. Eggs and the shiny star are laid over as <c>SpriteBuilder.GetSprite</c> does with the desktop's default settings.
/// Totem forms, Deoxys in Generation 3 and Arceus in Generation 4 have special cases there that cannot occur in this context.
/// Held-item icons, which the desktop also draws, are not drawn.
/// </para>
/// <para>
/// This file is compiled into both PKHeX.Web and the atlas generator, so the generator packs exactly the images this chooses.
/// </para>
/// </remarks>
public static class SpriteKeys
{
    /// <summary>The unknown mark, drawn when no sprite matches.</summary>
    public const string Unknown = "b_unknown";

    /// <summary>The egg icon.</summary>
    public const string Egg = "b_egg";

    /// <summary>The egg icon for Manaphy, whose egg looks different.</summary>
    public const string ManaphyEgg = "b_490_e";

    /// <summary>The shiny star; Generation 6 does not tell square shinies apart, so the desktop always uses this one.</summary>
    public const string ShinyStar = "rare_icon_alt";

    /// <summary>The context the keys are chosen for.</summary>
    public const EntityContext Context = EntityContext.Gen6;

    /// <summary>Images that every sprite may be drawn from, whatever the save holds.</summary>
    public static IReadOnlyList<string> Fixed { get; } = [Unknown, Egg, ManaphyEgg, ShinyStar];

    static SpriteKeys()
    {
        // The desktop shows shiny sprites unless the user turns them off (SpriteSettings.ShinySprites defaults to true).
        SpriteName.AllowShinySprite = true;
    }

    /// <summary>
    /// Chooses the images for an entity.
    /// </summary>
    /// <param name="species">Stored species; 0 is an empty position, which has no sprite.</param>
    /// <param name="form">Stored form.</param>
    /// <param name="gender">Stored gender.</param>
    /// <param name="shiny">True when the entity is shiny.</param>
    /// <param name="isEgg">True for an egg.</param>
    /// <param name="holdsItem">True when the entity holds an item; it changes how an egg is drawn.</param>
    /// <param name="exists">Whether a resource of the given name is available.</param>
    /// <returns>The layers, or null for species 0.</returns>
    public static SpriteLayers? Resolve(ushort species, byte form, byte gender, bool shiny, bool isEgg, bool holdsItem, Func<string, bool> exists)
    {
        if (species == 0)
        {
            return null;
        }

        var unknownForm = false;
        var key = Find(species, form, gender, shiny, exists);
        if (key is null && shiny)
        {
            key = Find(species, form, gender, false, exists);
        }
        if (key is null)
        {
            var speciesOnly = $"b_{species}";
            (key, unknownForm) = exists(speciesOnly) ? (speciesOnly, true) : (Unknown, false);
        }

        var eggMode = !isEgg ? EggLayer.None : holdsItem ? EggLayer.OverSpecies : EggLayer.AsItem;
        var egg = isEgg ? (species == (ushort)Species.Manaphy ? ManaphyEgg : Egg) : null;
        return new SpriteLayers(key, unknownForm, eggMode, egg, shiny);
    }

    /// <summary>The <c>b</c> resource, then the <c>c</c> one, as <c>SpriteBuilder5668s</c> tries them; null when neither exists.</summary>
    private static string? Find(ushort species, byte form, byte gender, bool shiny, Func<string, bool> exists)
    {
        var name = SpriteName.GetResourceStringSprite(species, form, gender, 0, Context, shiny);
        var primary = 'b' + name;
        if (exists(primary))
        {
            return primary;
        }
        var secondary = 'c' + name;
        return exists(secondary) ? secondary : null;
    }
}
