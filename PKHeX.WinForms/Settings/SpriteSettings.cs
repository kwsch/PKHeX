using PKHeX.Core;
using PKHeX.Drawing.PokeSprite;

namespace PKHeX.WinForms;

public sealed class SpriteSettings : ISpriteSettings
{
    [LocalizedDescription("Choice for which sprite building mode to use.")]
    public SpriteBuilderPreference SpritePreference { get; set; } = SpriteBuilderPreference.UseSuggested;

    [LocalizedDescription("Show fan-made shiny sprites when the PKM is shiny.")]
    public bool ShinySprites { get; set; } = true;

    [LocalizedDescription("Show an Egg Sprite As Held Item rather than hiding the PKM")]
    public bool ShowEggSpriteAsHeldItem { get; set; } = true;

    [LocalizedDescription("Show the required ball for an Encounter Template")]
    public bool ShowEncounterBall { get; set; } = true;

    [LocalizedDescription("Show a background to differentiate an Encounter Template's type")]
    public SpriteBackgroundType ShowEncounterColor { get; set; } = SpriteBackgroundType.FullBackground;

    [LocalizedDescription("Show a background to differentiate the recognized Encounter Template type for PKM slots")]
    public SpriteBackgroundType ShowEncounterColorPKM { get; set; }

    [LocalizedDescription("Opacity for the Encounter Type background layer.")]
    public byte ShowEncounterOpacityBackground { get; set; } = 0x3F; // kinda low

    [LocalizedDescription("Opacity for the Encounter Type stripe layer.")]
    public byte ShowEncounterOpacityStripe { get; set; } = 0x5F; // 0xFF opaque

    [LocalizedDescription("Amount of pixels thick to show when displaying the encounter type color stripe.")]
    public int ShowEncounterThicknessStripe { get; set; } = 4; // pixels

    [LocalizedDescription("Show a thin stripe to indicate the percent of level-up progress")]
    public bool ShowExperiencePercent { get; set; }

    [LocalizedDescription("Show a background to differentiate the Tera Type for PKM slots")]
    public SpriteBackgroundType ShowTeraType { get; set; } = SpriteBackgroundType.BottomStripe;

    [LocalizedDescription("Amount of pixels thick to show when displaying the Tera Type color stripe.")]
    public int ShowTeraThicknessStripe { get; set; } = 4; // pixels

    [LocalizedDescription("Opacity for the Tera Type background layer.")]
    public byte ShowTeraOpacityBackground { get; set; } = 0xFF; // 0xFF opaque

    [LocalizedDescription("Opacity for the Tera Type stripe layer.")]
    public byte ShowTeraOpacityStripe { get; set; } = 0xAF; // 0xFF opaque
}
