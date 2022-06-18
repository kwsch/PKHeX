namespace PKHeX.Core;

public readonly record struct MemoryVariableSet(string Handler, byte MemoryID, ushort Variable, byte Intensity, byte Feeling)
{
    public static MemoryVariableSet Read(ITrainerMemories pk, int handler) => handler switch
    {
        0 => new MemoryVariableSet(LegalityCheckStrings.L_XOT, pk.OT_Memory, pk.OT_TextVar, pk.OT_Intensity, pk.OT_Feeling), // OT
        1 => new MemoryVariableSet(LegalityCheckStrings.L_XHT, pk.HT_Memory, pk.HT_TextVar, pk.HT_Intensity, pk.HT_Feeling), // HT
        _ => new MemoryVariableSet(LegalityCheckStrings.L_XOT, 0, 0, 0, 0),
    };
}
