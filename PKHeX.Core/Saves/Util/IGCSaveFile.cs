namespace PKHeX.Core;

/// <summary>
/// GameCube save file interface for memory cards.
/// </summary>
public interface IGCSaveFile
{
    /// <summary>
    /// GameCube Memory Card the save file was read from.
    /// </summary>
    SAV3GCMemoryCard? MemoryCard { get; }
}

public static class GCSaveExtensions
{
    /// <summary>
    /// Gets an export filter for a GameCube file.
    /// </summary>
    public static string GCFilter(this IGCSaveFile gc)
    {
        const string regular = "GameCube Save File|*.gci|All Files|*.*";
        const string memcard = "Memory Card Raw File|*.raw|Memory Card Binary File|*.bin|";
        return gc.MemoryCard is not null ? memcard + regular : regular;
    }

    /// <summary>
    /// Gets the export extension for a GameCube file.
    /// </summary>
    public static string GCExtension(this IGCSaveFile gc) => gc.MemoryCard is not null ? ".raw" : ".gci";
}
