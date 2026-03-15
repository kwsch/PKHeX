using PKHeX.Drawing.PokeSprite.Avalonia;

namespace PKHeX.Avalonia.Settings;

public sealed class SpriteSettings : ISpriteSettings
{
    public SpriteBuilderPreference SpritePreference { get; set; } = SpriteBuilderPreference.UseSuggested;
    public bool ShinySprites { get; set; } = true;
    public bool ShowEggSpriteAsHeldItem { get; set; } = true;
    public bool ShowEncounterBall { get; set; } = true;
    public SpriteBackgroundType ShowEncounterColor { get; set; } = SpriteBackgroundType.FullBackground;
    public SpriteBackgroundType ShowEncounterColorPKM { get; set; }
    public byte ShowEncounterOpacityBackground { get; set; } = 0x3F;
    public byte ShowEncounterOpacityStripe { get; set; } = 0x5F;
    public int ShowEncounterThicknessStripe { get; set; } = 4;
    public bool ShowExperiencePercent { get; set; }
    public SpriteBackgroundType ShowTeraType { get; set; } = SpriteBackgroundType.BottomStripe;
    public int ShowTeraThicknessStripe { get; set; } = 4;
    public byte ShowTeraOpacityBackground { get; set; } = 0xFF;
    public byte ShowTeraOpacityStripe { get; set; } = 0xAF;
    public float FilterMismatchOpacity { get; set; } = 0.40f;
    public float FilterMismatchGrayscale { get; set; } = 0.70f;
}
