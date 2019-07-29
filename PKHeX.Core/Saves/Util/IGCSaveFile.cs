namespace PKHeX.Core
{
    /// <summary>
    /// GameCube save file interface for memory cards.
    /// </summary>
    public interface IGCSaveFile
    {
        bool IsMemoryCardSave { get; }
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
            return gc.IsMemoryCardSave ? memcard + regular : regular;
        }

        /// <summary>
        /// Gets the export extension for a GameCube file.
        /// </summary>
        public static string GCExtension(this IGCSaveFile gc) => gc.IsMemoryCardSave ? ".raw" : ".gci";
    }
}