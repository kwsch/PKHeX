namespace PKHeX.Drawing.PokeSprite;

public interface ISpriteSettings
{
    bool ShowEggSpriteAsHeldItem { get; set; }
    bool ShowEncounterBall { get; set; }

    SpriteBackgroundType ShowEncounterColor { get; set; }
    SpriteBackgroundType ShowEncounterColorPKM { get; set; }
    int ShowEncounterThicknessStripe { get; set; }
    byte ShowEncounterOpacityBackground { get; set; }
    byte ShowEncounterOpacityStripe { get; set; }
    bool ShowExperiencePercent { get; set; }
}
