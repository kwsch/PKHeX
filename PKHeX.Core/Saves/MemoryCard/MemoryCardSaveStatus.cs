namespace PKHeX.Core;

/// <summary>
/// Flags for indicating what data is present in the Memory Card
/// </summary>
public enum MemoryCardSaveStatus : byte
{
    Invalid,
    NoPkmSaveGame,
    SaveGameCOLO,
    SaveGameXD,
    SaveGameRSBOX,
    MultipleSaveGame,
    DuplicateCOLO,
    DuplicateXD,
    DuplicateRSBOX,
}
