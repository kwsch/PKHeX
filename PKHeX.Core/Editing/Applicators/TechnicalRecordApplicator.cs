using System;

namespace PKHeX.Core;

/// <summary>
/// Logic for modifying the Technical Record flags of a <see cref="PK8"/>.
/// </summary>
public static class TechnicalRecordApplicator
{
    /// <summary>
    /// Sets the Technical Record flags for the <see cref="pk"/>.
    /// </summary>
    /// <param name="pk">Pokémon to modify.</param>
    /// <param name="value">Value to set for the record.</param>
    /// <param name="max">Max record to set.</param>
    public static void SetRecordFlags(this ITechRecord pk, bool value, int max)
    {
        for (int i = 0; i < max; i++)
            pk.SetMoveRecordFlag(i, value);
    }

    /// <summary>
    /// Clears the Technical Record flags for the <see cref="pk"/>.
    /// </summary>
    /// <param name="pk">Pokémon to modify.</param>
    public static void ClearRecordFlags(this ITechRecord pk) => pk.SetRecordFlags(false, pk.Permit.RecordCountTotal);

    /// <summary>
    /// Sets the Technical Record flags for the <see cref="pk"/> based on the current moves.
    /// </summary>
    /// <param name="pk">Pokémon to modify.</param>
    /// <param name="moves">Moves to set flags for. If a move is not a Technical Record, it is skipped.</param>
    public static void SetRecordFlags(this ITechRecord pk, ReadOnlySpan<ushort> moves)
    {
        var permit = pk.Permit;
        var moveIDs = permit.RecordPermitIndexes;

        foreach (var m in moves)
        {
            var index = moveIDs.IndexOf(m);
            if (index == -1)
                continue;
            if (permit.IsRecordPermitted(index))
                pk.SetMoveRecordFlag(index);
        }
    }

    /// <summary>
    /// Sets all the Technical Record flags for the <see cref="pk"/> if they are permitted to be learned in-game.
    /// </summary>
    /// <param name="pk">Pokémon to modify.</param>
    public static void SetRecordFlags(this ITechRecord pk)
    {
        var permit = pk.Permit;
        for (int i = 0; i < permit.RecordCountUsed; i++)
        {
            if (permit.IsRecordPermitted(i))
                pk.SetMoveRecordFlag(i);
        }
    }
}
