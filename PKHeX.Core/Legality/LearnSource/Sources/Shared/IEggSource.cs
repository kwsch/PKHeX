using System;

namespace PKHeX.Core;

/// <summary>
/// Exposes information about how moves are learned via Breeding.
/// </summary>
public interface IEggSource
{
    /// <summary>
    /// Checks if the <see cref="move"/> can be learned.
    /// </summary>
    /// <param name="species">Entity base species</param>
    /// <param name="form">Entity base form</param>
    /// <param name="move">Move to check</param>
    bool GetIsEggMove(ushort species, byte form, ushort move);

    /// <summary>
    /// Gets the egg move list that is permitted to be inherited.
    /// </summary>
    /// <param name="species">Entity base species</param>
    /// <param name="form">Entity base form</param>
    ReadOnlySpan<ushort> GetEggMoves(ushort species, byte form);
}
