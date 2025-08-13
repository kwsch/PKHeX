using PKHeX.Core;
using static PKHeX.Drawing.PokeSprite.SpriteBuilderMode;
using static PKHeX.Drawing.PokeSprite.SpriteBuilderPreference;

namespace PKHeX.Drawing.PokeSprite;

/// <summary>
/// Specifies the sprite rendering mode for Pokémon display.
/// </summary>
public enum SpriteBuilderMode
{
    /// <summary>
    /// Keep the current sprite rendering mode.
    /// </summary>
    KeepCurrent,
    /// <summary>
    /// Use classic 5668-style sprites.
    /// </summary>
    SpritesClassic5668,
    /// <summary>
    /// Use circular mugshot-style sprites (5668).
    /// </summary>
    CircleMugshot5668,
    /// <summary>
    /// Use artwork-based sprites (5668).
    /// </summary>
    SpritesArtwork5668,
}

/// <summary>
/// Specifies the user's preference for sprite rendering.
/// </summary>
public enum SpriteBuilderPreference
{
    /// <summary>
    /// Use the suggested sprite rendering mode based on the save file.
    /// </summary>
    UseSuggested,
    /// <summary>
    /// Do not change the current sprite rendering mode.
    /// </summary>
    DoNotChange,
    /// <summary>
    /// Force the use of classic sprites.
    /// </summary>
    ForceSprites,
    /// <summary>
    /// Force the use of mugshot sprites.
    /// </summary>
    ForceMugshots,
    /// <summary>
    /// Force the use of artwork sprites.
    /// </summary>
    ForceArtwork,
}

/// <summary>
/// Utility methods for determining sprite rendering mode and preference.
/// </summary>
public static class SpriteBuilderUtil
{
    /// <summary>
    /// Gets or sets the current sprite rendering preference.
    /// </summary>
    public static SpriteBuilderPreference SpriterPreference { get; set; } = UseSuggested;

    /// <summary>
    /// Gets the suggested sprite rendering mode for the provided save file.
    /// </summary>
    /// <param name="sav">The save file to analyze.</param>
    /// <returns>The suggested <see cref="SpriteBuilderMode"/>.</returns>
    public static SpriteBuilderMode GetSuggestedMode(SaveFile sav) => SpriterPreference switch
    {
        ForceMugshots => CircleMugshot5668,
        ForceSprites => SpritesClassic5668,
        ForceArtwork => SpritesArtwork5668,
        DoNotChange => KeepCurrent,
        _ => sav switch // Default, suggest.
        {
            SAV8LA => CircleMugshot5668,
            SAV9SV => SpritesArtwork5668,
            _ => SpritesClassic5668,
        },
    };
}
