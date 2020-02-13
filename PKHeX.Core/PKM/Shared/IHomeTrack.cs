namespace PKHeX.Core
{
    /// <summary>
    /// Interface that exposes a <see cref="Tracker"/> for Pokémon HOME.
    /// </summary>
    public interface IHomeTrack
    {
        /// <summary>
        /// Tracker for the associated <see cref="PKM"/>
        /// </summary>
        ulong Tracker { get; set; }
    }
}