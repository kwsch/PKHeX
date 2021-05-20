namespace PKHeX.Core
{
    /// <summary>
    /// Type of <see cref="PokeListGB{T}"/> that is stored.
    /// </summary>
    /// <remarks>Acts as a magic capacity value; there aren't any other List sizes used by the games.</remarks>
    public enum PokeListType : byte
    {
        /// <summary> A single <see cref="GBPKM"/> is contained in this list. </summary>
        Single = 1,

        /// <summary> A full party of <see cref="GBPKM"/> is contained in this list. </summary>
        Party = 6,

        /// <summary> A full box of <see cref="GBPKM"/> (International format) is contained in this list. </summary>
        Stored = 20,

        /// <summary> A full box of <see cref="GBPKM"/> (Japanese) is contained in this list. </summary>
        StoredJP = 30,
    }
}
