namespace PKHeX.Core
{
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
}
