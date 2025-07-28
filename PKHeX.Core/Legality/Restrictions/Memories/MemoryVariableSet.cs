namespace PKHeX.Core;

/// <summary>
/// Small wrapper to store details about a memory so that it can be passed to verification logic.
/// </summary>
/// <param name="Handler">Trainer name of who originated the memory</param>
/// <param name="MemoryID">Memory ID</param>
/// <param name="Variable">Argument for the memory</param>
/// <param name="Intensity">How strongly they remember the memory</param>
/// <param name="Feeling">How they feel about the memory</param>
public readonly record struct MemoryVariableSet(byte Handler, byte MemoryID, ushort Variable, byte Intensity, byte Feeling)
{
    public static MemoryVariableSet Read(ITrainerMemories pk, int handler) => handler switch
    {
        0 => new(0, pk.OriginalTrainerMemory, pk.OriginalTrainerMemoryVariable, pk.OriginalTrainerMemoryIntensity, pk.OriginalTrainerMemoryFeeling), // OT
        1 => new(1, pk.HandlingTrainerMemory, pk.HandlingTrainerMemoryVariable, pk.HandlingTrainerMemoryIntensity, pk.HandlingTrainerMemoryFeeling), // HT
        _ => new(0, 0, 0, 0, 0),
    };
}
