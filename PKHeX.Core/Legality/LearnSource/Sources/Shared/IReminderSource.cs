using System;

namespace PKHeX.Core;

/// <summary>
/// Exposes information about how special moves are learned via the move reminding interface.
/// </summary>
public interface IReminderSource
{
    /// <summary>
    /// Checks if the <see cref="move"/> can be learned.
    /// </summary>
    /// <param name="species">Entity base species</param>
    /// <param name="form">Entity base form</param>
    /// <param name="move">Move to check</param>
    bool GetIsReminderMove(ushort species, byte form, ushort move);

    /// <summary>
    /// Gets the reminder move list that is permitted to be instructed.
    /// </summary>
    /// <param name="species">Entity base species</param>
    /// <param name="form">Entity base form</param>
    ReadOnlySpan<ushort> GetReminderMoves(ushort species, byte form);
}
