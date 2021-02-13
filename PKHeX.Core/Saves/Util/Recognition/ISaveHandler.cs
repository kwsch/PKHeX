namespace PKHeX.Core
{
#if !(EXCLUDE_EMULATOR_FORMATS && EXCLUDE_HACKS)
    /// <summary>
    /// Provides handling for recognizing atypical save file formats.
    /// </summary>
    public interface ISaveHandler
    {
        /// <summary>
        /// Checks if the requested file size is one that can be recognized by this handler.
        /// </summary>
        /// <param name="size">File size</param>
        /// <returns>True if recognized, false if not recognized.</returns>
        bool IsRecognized(int size);

        /// <summary>
        /// Tries splitting up the <see cref="input"/> into header/footer/data components. Returns null if not a valid save file for this handler.
        /// </summary>
        /// <param name="input">Combined data</param>
        /// <returns>Null if not a valid save file for this handler's format. Returns an object containing header, footer, and inner data references.</returns>
        SaveHandlerSplitResult? TrySplit(byte[] input);
    }
#endif

#if !EXCLUDE_HACKS
    /// <summary>
    /// Provides handling for recognizing atypical save file formats.
    /// </summary>
    public interface ISaveReader : ISaveHandler
    {
        /// <summary>
        /// Reads a save file from the <see cref="data"/>
        /// </summary>
        /// <param name="data">Raw input data</param>
        /// <param name="path">Optional file path.</param>
        /// <returns>Save File object, or null if invalid. Check <see cref="ISaveHandler"/> if it is compatible first.</returns>
        SaveFile? ReadSaveFile(byte[] data, string? path = null);
    }
#endif
}
