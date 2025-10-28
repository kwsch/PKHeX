namespace PKHeX.Core;

/// <summary>
/// Exposes miscellaneous metadata about an entity species/form.
/// </summary>
public interface IPersonalMisc
{
    /// <summary>
    /// Evolution Stage value (or equivalent for un-evolved).
    /// </summary>
    int EvoStage { get; set; }

    /// <summary>
    /// Main color ID of the entry. The majority of the Pokémon's color is of this color, usually.
    /// </summary>
    int Color { get; set; }

    /// <summary>
    /// Height of the entry in meters (m).
    /// </summary>
    int Height { get; set; }

    /// <summary>
    /// Mass of the entry in kilograms (kg).
    /// </summary>
    int Weight { get; set; }
}
