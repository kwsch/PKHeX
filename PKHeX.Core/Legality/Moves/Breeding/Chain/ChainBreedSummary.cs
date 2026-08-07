namespace PKHeX.Core;

/// <summary>
/// Simple summary of a successful breeding proof, indicating the immediate parents and the depth of the breeding chain.
/// <paramref name="ChainDepth"/> counts breeding links only; transfer transitions are not included.
/// </summary>
public readonly record struct ChainBreedSummary(
    ushort MotherSpecies, byte MotherForm,
    ushort FatherSpecies, byte FatherForm,
    byte ChainDepth);
