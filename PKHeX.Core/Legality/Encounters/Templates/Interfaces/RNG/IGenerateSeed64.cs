namespace PKHeX.Core;

/// <summary>
/// Interface for <seealso cref="IEncounterTemplate"/> that can generate from a 64-bit seed.
/// </summary>
public interface IGenerateSeed64
{
    /// <summary>
    /// Generates the <see cref="PKM"/> from the seed.
    /// </summary>
    /// <remarks>Be sure the <see cref="pk"/> is un-converted and matches its original type.</remarks>
    void GenerateSeed64(PKM pk, ulong seed);
}
