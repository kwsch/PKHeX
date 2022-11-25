using PKHeX.Core;
using static PKHeX.Drawing.PokeSprite.SpriteBuilderMode;
using static PKHeX.Drawing.PokeSprite.SpriteBuilderPreference;

namespace PKHeX.Drawing.PokeSprite;

public enum SpriteBuilderMode
{
    KeepCurrent,
    SpritesClassic5668,
    CircleMugshot5668,
    SpritesArtwork5668,
}

public enum SpriteBuilderPreference
{
    UseSuggested,
    DoNotChange,
    ForceSprites,
    ForceMugshots,
    ForceArtwork,
}

public static class SpriteBuilderUtil
{
    public static SpriteBuilderPreference SpriterPreference { get; set; } = UseSuggested;

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
