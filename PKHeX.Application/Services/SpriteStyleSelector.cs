using PKHeX.Core;

namespace PKHeX.Application.Services;

/// <summary>
/// Chooses the <see cref="SpriteStyle"/> for a loaded save, mirroring upstream PKHeX's
/// <c>SpriteBuilderUtil.GetSuggestedMode</c> save-type switch.
/// </summary>
public static class SpriteStyleSelector
{
    /// <summary>The style upstream suggests for the given save file.</summary>
    public static SpriteStyle GetSuggested(SaveFile sav) => sav switch
    {
        SAV8LA => SpriteStyle.Mugshot,
        SAV9SV or SAV9ZA => SpriteStyle.Artwork,
        _ => SpriteStyle.Classic,
    };

    /// <summary>
    /// Resolves the effective style from the user preference and the loaded save.
    /// Force* values win; <see cref="SpritePreference.UseSuggested"/> defers to <see cref="GetSuggested"/>.
    /// </summary>
    public static SpriteStyle Resolve(SpritePreference preference, SaveFile sav) => preference switch
    {
        SpritePreference.ForceSprites => SpriteStyle.Classic,
        SpritePreference.ForceMugshots => SpriteStyle.Mugshot,
        SpritePreference.ForceArtwork => SpriteStyle.Artwork,
        _ => GetSuggested(sav),
    };
}
