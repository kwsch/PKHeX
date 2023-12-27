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
    public static void SetRecordFlagsAll(this ITechRecord pk, bool value, int max)
    {
        for (int i = 0; i < max; i++)
            pk.SetMoveRecordFlag(i, value);
    }

    /// <summary>
    /// Clears the Technical Record flags for the <see cref="pk"/>.
    /// </summary>
    /// <param name="pk">Pokémon to modify.</param>
    public static void ClearRecordFlags(this ITechRecord pk) => pk.SetRecordFlagsAll(false, pk.Permit.RecordCountTotal);

    /// <summary>
    /// Sets the Technical Record flags for the <see cref="pk"/> based on the current moves.
    /// </summary>
    /// <param name="pk">Pokémon to modify.</param>
    /// <param name="moves">Moves to set flags for. If a move is not a Technical Record, it is skipped.</param>
    public static void SetRecordFlags(this ITechRecord pk, ReadOnlySpan<ushort> moves)
    {
        var permit = pk.Permit;
        SetRecordFlags(pk, moves, permit);
    }

    /// <inheritdoc cref="SetRecordFlags(ITechRecord, ReadOnlySpan{ushort})"/>
    public static void SetRecordFlags(ITechRecord pk, ReadOnlySpan<ushort> moves, IPermitRecord permit)
    {
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
    public static void SetRecordFlagsAll(this ITechRecord pk)
    {
        var permit = pk.Permit;
        SetRecordFlagsAll(pk, permit);
    }

    /// <inheritdoc cref="SetRecordFlagsAll(PKHeX.Core.ITechRecord)"/>"/>
    public static void SetRecordFlagsAll(ITechRecord pk, IPermitRecord permit)
    {
        for (int i = 0; i < permit.RecordCountUsed; i++)
        {
            if (permit.IsRecordPermitted(i))
                pk.SetMoveRecordFlag(i);
        }
    }

    private static void SetRecordFlags<TTable, TInfo>(this ITechRecord pk, ReadOnlySpan<ushort> moves, ReadOnlySpan<EvoCriteria> evos, TTable pt)
        where TTable : IPersonalTable<TInfo> where TInfo : IPersonalInfo, IPermitRecord
    {
        foreach (var evo in evos)
            SetRecordFlags(pk, moves, pt[evo.Species, evo.Form]);
    }

    /// <inheritdoc cref="SetRecordFlagsAll(ITechRecord)"/>
    private static void SetRecordFlagsAll<TTable, TInfo>(this ITechRecord pk, ReadOnlySpan<EvoCriteria> evos, TTable pt)
        where TTable : IPersonalTable<TInfo> where TInfo : IPersonalInfo, IPermitRecord
    {
        foreach (var evo in evos)
            SetRecordFlagsAll(pk, pt[evo.Species, evo.Form]);
    }

    /// <inheritdoc cref="IPermitRecord.IsRecordPermitted"/>
    private static bool IsRecordPermitted<TTable, TInfo>(ReadOnlySpan<EvoCriteria> evos, TTable pt, int index)
        where TTable : IPersonalTable<TInfo> where TInfo : IPersonalInfo, IPermitRecord
    {
        foreach (var evo in evos)
        {
            if (pt[evo.Species, evo.Form].IsRecordPermitted(index))
                return true;
        }
        return false;
    }

    /// <inheritdoc cref="SetRecordFlags(ITechRecord, ReadOnlySpan{ushort})"/>
    public static void SetRecordFlags(this ITechRecord pk, ReadOnlySpan<ushort> moves, ReadOnlySpan<EvoCriteria> evos)
    {
        if (pk is PK9 pk9)
            SetRecordFlags<PersonalTable9SV, PersonalInfo9SV>(pk9, moves, evos, PersonalTable.SV);
        else if (pk is PK8 pk8)
            SetRecordFlags<PersonalTable8SWSH, PersonalInfo8SWSH>(pk8, moves, evos, PersonalTable.SWSH);
    }

    /// <inheritdoc cref="SetRecordFlagsAll(ITechRecord)"/>
    public static void SetRecordFlagsAll(this ITechRecord pk, ReadOnlySpan<EvoCriteria> evos)
    {
        if (pk is PK9 pk9)
            SetRecordFlagsAll<PersonalTable9SV, PersonalInfo9SV>(pk9, evos, PersonalTable.SV);
        else if (pk is PK8 pk8)
            SetRecordFlagsAll<PersonalTable8SWSH, PersonalInfo8SWSH>(pk8, evos, PersonalTable.SWSH);
    }

    /// <inheritdoc cref="IPermitRecord.IsRecordPermitted"/>
    public static bool IsRecordPermitted(this ITechRecord pk, ReadOnlySpan<EvoCriteria> evos, int index) => pk switch
    {
        PK9 => IsRecordPermitted<PersonalTable9SV, PersonalInfo9SV>(evos, PersonalTable.SV, index),
        PK8 => IsRecordPermitted<PersonalTable8SWSH, PersonalInfo8SWSH>(evos, PersonalTable.SWSH, index),
        _ => false,
    };
}
