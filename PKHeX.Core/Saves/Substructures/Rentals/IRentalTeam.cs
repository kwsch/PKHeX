using System;

namespace PKHeX.Core;

/// <summary>
/// Exposes a rental team interface to fetch team data.
/// </summary>
/// <typeparam name="T">Type of entity stored in the rental team</typeparam>
public interface IRentalTeam<T> where T : PKM
{
    /// <summary>
    /// Gets the entity at a specific slot.
    /// </summary>
    T GetSlot(int slot);

    /// <summary>
    /// Sets the entity at a specific slot.
    /// </summary>
    void SetSlot(int slot, T pk);

    /// <summary>
    /// Gets the entire team.
    /// </summary>
    void GetTeam(Span<T> team);

    /// <summary>
    /// Sets the entire team.
    /// </summary>
    void SetTeam(ReadOnlySpan<T> team);

    /// <summary>
    /// Gets the entire team.
    /// </summary>
    T[] GetTeam();
}
