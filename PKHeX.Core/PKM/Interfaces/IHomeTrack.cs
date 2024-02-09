namespace PKHeX.Core;

/// <summary>
/// Interface that exposes a <see cref="Tracker"/> for Pokémon HOME.
/// </summary>
/// <remarks>Internally called BankUniqueID</remarks>
public interface IHomeTrack
{
    /// <summary>
    /// Tracker for the associated <see cref="PKM"/>
    /// </summary>
    ulong Tracker { get; set; }

    /// <summary>
    /// Simple check if a <see cref="Tracker"/> is present.
    /// </summary>
    /// <remarks>Does not ensure that it is a valid tracker, just non-zero.</remarks>
    bool HasTracker => Tracker != 0;
}
