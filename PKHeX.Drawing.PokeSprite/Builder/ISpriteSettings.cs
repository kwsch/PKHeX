namespace PKHeX.Drawing.PokeSprite;

/// <summary>
/// Defines settings for customizing the appearance of Pokémon sprites in the UI.
/// </summary>
public interface ISpriteSettings
{
    /// <summary>
    /// Display the egg sprite as a held item overlay.
    /// </summary>
    bool ShowEggSpriteAsHeldItem { get; set; }

    /// <summary>
    /// Display the Poké Ball used for the encounter.
    /// </summary>
    bool ShowEncounterBall { get; set; }

    /// <summary>
    /// Background style for the encounter (e.g., stripe, full background).
    /// </summary>
    SpriteBackgroundType ShowEncounterColor { get; set; }

    /// <summary>
    /// Background style for the encounter, based on the Pokémon's properties.
    /// </summary>
    SpriteBackgroundType ShowEncounterColorPKM { get; set; }

    /// <summary>
    /// Background style for Tera Type display.
    /// </summary>
    SpriteBackgroundType ShowTeraType { get; set; }

    /// <summary>
    /// Thickness of the encounter background stripe.
    /// </summary>
    int ShowEncounterThicknessStripe { get; set; }

    /// <summary>
    /// Opacity of the encounter background.
    /// </summary>
    byte ShowEncounterOpacityBackground { get; set; }

    /// <summary>
    /// Opacity of the encounter stripe overlay.
    /// </summary>
    byte ShowEncounterOpacityStripe { get; set; }

    /// <summary>
    /// Display the experience percent bar.
    /// </summary>
    bool ShowExperiencePercent { get; set; }

    /// <summary>
    /// Thickness of the Tera Type background stripe.
    /// </summary>
    int ShowTeraThicknessStripe { get; set; }

    /// <summary>
    /// Opacity of the Tera Type background.
    /// </summary>
    byte ShowTeraOpacityBackground { get; set; }

    /// <summary>
    /// Opacity of the Tera Type stripe overlay.
    /// </summary>
    byte ShowTeraOpacityStripe { get; set; }
}
