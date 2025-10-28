namespace PKHeX.Core;

public sealed class SlotWriteSettings
{
    [LocalizedDescription("Automatically modify the Save File's Pokédex when injecting a PKM.")]
    public bool SetUpdateDex { get; set; } = true;

    [LocalizedDescription("Automatically adapt the PKM Info to the Save File (Handler, Format)")]
    public bool SetUpdatePKM { get; set; } = true;

    [LocalizedDescription("Automatically increment the Save File's counters for obtained Pokémon (eggs/captures) when injecting a PKM.")]
    public bool SetUpdateRecords { get; set; } = true;

    [LocalizedDescription("When enabled and closing/loading a save file, the program will alert if the current save file has been modified without saving.")]
    public bool ModifyUnset { get; set; } = true;
}
