namespace PKHeX.Core
{
    /// <summary>
    /// Tracks information about modifications made to a <see cref="SaveFile"/>
    /// </summary>
    public class SaveFileState
    {
        /// <summary>
        /// Mutable value tracking if the save file has been changed. This is set manually by modifications, and not for all modifications.
        /// </summary>
        public bool Edited { get; set; }

        /// <summary>
        /// Toggle determining if the save file can be exported.
        /// </summary>
        /// <remarks>
        /// This is always true, unless the save file is a "fake" save file with blank data. Blank Save Files are essentially zeroed out buffers.
        /// </remarks>
        public readonly bool Exportable;

        /// <summary>
        /// Original Binary Data the save file was loaded with.
        /// </summary>
        public readonly byte[] BAK;

        public SaveFileState(byte[] bak, bool exportable = true)
        {
            BAK = bak;
            Exportable = exportable;
        }
    }
}
