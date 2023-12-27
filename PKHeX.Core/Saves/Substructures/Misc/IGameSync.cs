namespace PKHeX.Core;

/// <summary>
/// Provides details about the save file's Game Sync identifier.
/// </summary>
public interface IGameSync
{
    int GameSyncIDSize { get; }
    string GameSyncID { get; set; }
}
