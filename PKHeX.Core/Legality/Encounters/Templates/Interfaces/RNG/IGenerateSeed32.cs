namespace PKHeX.Core;

/// <summary>
/// Interface for <seealso cref="IEncounterTemplate"/> that can generate from a 32-bit seed.
/// </summary>
public interface IGenerateSeed32
{
    /// <summary>
    /// Generates the <see cref="PKM"/> from the seed.
    /// </summary>
    /// <remarks>Be sure the <see cref="pk"/> is un-converted and matches its original type.</remarks>
    bool GenerateSeed32(PKM pk, uint seed);
}
