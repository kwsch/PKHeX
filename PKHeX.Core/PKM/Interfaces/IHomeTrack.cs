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

    bool HasTracker => Tracker != 0;
}
