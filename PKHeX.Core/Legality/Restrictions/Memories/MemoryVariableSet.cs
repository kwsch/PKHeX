namespace PKHeX.Core;

/// <summary>
/// Small wrapper to store details about a memory so that it can be passed to verification logic.
/// </summary>
/// <param name="Handler">Trainer name of who originated the memory</param>
/// <param name="MemoryID">Memory ID</param>
/// <param name="Variable">Argument for the memory</param>
/// <param name="Intensity">How strongly they remember the memory</param>
/// <param name="Feeling">How they feel about the memory</param>
public readonly record struct MemoryVariableSet(string Handler, byte MemoryID, ushort Variable, byte Intensity, byte Feeling)
{
    public static MemoryVariableSet Read(ITrainerMemories pk, int handler) => handler switch
    {
        0 => new MemoryVariableSet(LegalityCheckStrings.L_XOT, pk.OT_Memory, pk.OT_TextVar, pk.OT_Intensity, pk.OT_Feeling), // OT
        1 => new MemoryVariableSet(LegalityCheckStrings.L_XHT, pk.HT_Memory, pk.HT_TextVar, pk.HT_Intensity, pk.HT_Feeling), // HT
        _ => new MemoryVariableSet(LegalityCheckStrings.L_XOT, 0, 0, 0, 0),
    };
}
