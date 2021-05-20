namespace PKHeX.Core.Searching
{
    /// <summary>
    /// Search option to check for clones.
    /// </summary>
    public enum CloneDetectionMethod
    {
        /// <summary>
        /// Skip checking for cloned data.
        /// </summary>
        None,

        /// <summary>
        /// Check for clones by creating a hash of file name data.
        /// </summary>
        HashDetails,

        /// <summary>
        /// Check for clones by referencing the PID (or IVs for Gen1/2).
        /// </summary>
        HashPID,
    }
}
