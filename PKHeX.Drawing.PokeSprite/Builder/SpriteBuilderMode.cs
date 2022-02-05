using PKHeX.Core;
using static PKHeX.Drawing.PokeSprite.SpriteBuilderMode;
using static PKHeX.Drawing.PokeSprite.SpriteBuilderPreference;

namespace PKHeX.Drawing.PokeSprite;

public enum SpriteBuilderMode
{
    KeepCurrent,
    SpritesClassic5668,
    CircleMugshot5668,
}

public enum SpriteBuilderPreference
{
    UseSuggested,
    DoNotChange,
    ForceSprites,
    ForceMugshots,
}

public static class SpriteBuilderUtil
{
    public static SpriteBuilderPreference SpriterPreference { get; set; } = UseSuggested;

    public static SpriteBuilderMode GetSuggestedMode(SaveFile sav) => SpriterPreference switch
    {
        ForceMugshots => CircleMugshot5668,
        ForceSprites => SpritesClassic5668,
        DoNotChange => KeepCurrent,
        _ => sav switch // Default, suggest.
        {
            SAV8LA => CircleMugshot5668,
            _ => SpritesClassic5668,
        },
    };
}
