namespace PKHeX.Core;

public readonly record struct MemoryVariableSet(string Handler, int MemoryID, int Variable, int Intensity, int Feeling)
{
    public static MemoryVariableSet Read(ITrainerMemories pkm, int handler) => handler switch
    {
        0 => new MemoryVariableSet(LegalityCheckStrings.L_XOT, pkm.OT_Memory, pkm.OT_TextVar, pkm.OT_Intensity, pkm.OT_Feeling), // OT
        1 => new MemoryVariableSet(LegalityCheckStrings.L_XHT, pkm.HT_Memory, pkm.HT_TextVar, pkm.HT_Intensity, pkm.HT_Feeling), // HT
        _ => new MemoryVariableSet(LegalityCheckStrings.L_XOT, 0, 0, 0, 0),
    };
}
