using static PKHeX.Core.Species;

namespace PKHeX.Core;

/// <summary>
/// Logic rules for Pokémon Bank's Transporter functionality.
/// </summary>
public static class TransporterLogic
{
    /// <summary>
    /// Checks if the <see cref="species"/> must have its First ability rather than Hidden ability.
    /// </summary>
    /// <remarks>For Generation 1 games transferring to Generation 7.</remarks>
    public static bool IsHiddenDisallowedVC1(ushort species)
    {
        System.Diagnostics.Debug.Assert(species <= Legal.MaxSpeciesID_1);
        return species is (int)Gastly or (int)Haunter or (int)Gengar
            or (int)Koffing or (int)Weezing
            or (int)Mew;
    }

    /// <summary>
    /// Checks if the <see cref="species"/> must have its First ability rather than Hidden ability.
    /// </summary>
    /// <remarks>For Generation 2 games transferring to Generation 7.</remarks>
    public static bool IsHiddenDisallowedVC2(ushort species)
    {
        System.Diagnostics.Debug.Assert(species <= Legal.MaxSpeciesID_2);
        return species is (int)Gastly or (int)Haunter or (int)Gengar
            or (int)Koffing or (int)Weezing
            or (int)Misdreavus or (int)Unown
            or (int)Mew or (int)Celebi;
    }
}
