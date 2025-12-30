using System;
using static PKHeX.Core.TechnicalRecordApplicatorOption;

namespace PKHeX.Core;

/// <summary>
/// Logic for modifying the Technical Record flags of a <see cref="ITechRecord"/>.
/// </summary>
public static class TechnicalRecordApplicator
{
    extension(ITechRecord record)
    {
        /// <summary>
        /// Sets the Technical Record flags for the <see cref="record"/>.
        /// </summary>
        /// <param name="value">Value to set for the record.</param>
        /// <param name="max">Max record to set.</param>
        public void SetRecordFlagsAll(bool value, int max)
        {
            for (int i = 0; i < max; i++)
                record.SetMoveRecordFlag(i, value);
        }

        /// <summary>
        /// Clears the Technical Record flags for the <see cref="record"/>.
        /// </summary>
        public void ClearRecordFlags() => record.SetRecordFlagsAll(false, record.Permit.RecordCountTotal);

        /// <summary>
        /// Sets the Technical Record flags for the <see cref="record"/> based on the current moves.
        /// </summary>
        /// <param name="moves">Moves to set flags for. If a move is not a Technical Record, it is skipped.</param>
        public void SetRecordFlags(ReadOnlySpan<ushort> moves)
        {
            var permit = record.Permit;
            record.SetRecordFlags(moves, permit);
        }

        private void SetRecordFlags<TTable, TInfo>(ReadOnlySpan<ushort> moves, ReadOnlySpan<EvoCriteria> evos, TTable pt)
            where TTable : IPersonalTable<TInfo> where TInfo : IPersonalInfo, IPermitRecord
        {
            foreach (var evo in evos)
                record.SetRecordFlags(moves, pt[evo.Species, evo.Form]);
        }

        /// <inheritdoc cref="SetRecordFlagsAll(ITechRecord)"/>
        private void SetRecordFlagsAll<TTable, TInfo>(ReadOnlySpan<EvoCriteria> evos, TTable pt)
            where TTable : IPersonalTable<TInfo> where TInfo : IPersonalInfo, IPermitRecord
        {
            foreach (var evo in evos)
                record.SetRecordFlagsAll(pt[evo.Species, evo.Form]);
        }

        /// <inheritdoc cref="SetRecordFlags(ITechRecord, ReadOnlySpan{ushort})"/>
        public void SetRecordFlags(ReadOnlySpan<ushort> moves, ReadOnlySpan<EvoCriteria> evos)
        {
            if (record is PK9 pk9)
                pk9.SetRecordFlags<PersonalTable9SV, PersonalInfo9SV>(moves, evos, PersonalTable.SV);
            else if (record is PA9 pa9)
                pa9.SetRecordFlags<PersonalTable9ZA, PersonalInfo9ZA>(moves, evos, PersonalTable.ZA);
            else if (record is PK8 pk8)
                pk8.SetRecordFlags<PersonalTable8SWSH, PersonalInfo8SWSH>(moves, evos, PersonalTable.SWSH);
        }

        /// <inheritdoc cref="SetRecordFlagsAll(ITechRecord)"/>
        public void SetRecordFlagsAll(ReadOnlySpan<EvoCriteria> evos)
        {
            if (record is PK9 pk9)
                pk9.SetRecordFlagsAll<PersonalTable9SV, PersonalInfo9SV>(evos, PersonalTable.SV);
            else if (record is PA9 pa9)
                pa9.SetRecordFlagsAll<PersonalTable9ZA, PersonalInfo9ZA>(evos, PersonalTable.ZA);
            else if (record is PK8 pk8)
                pk8.SetRecordFlagsAll<PersonalTable8SWSH, PersonalInfo8SWSH>(evos, PersonalTable.SWSH);
        }

        /// <inheritdoc cref="IPermitRecord.IsRecordPermitted"/>
        public bool IsRecordPermitted(ReadOnlySpan<EvoCriteria> evos, int index) => record switch
        {
            PK9 => IsRecordPermitted<PersonalTable9SV, PersonalInfo9SV>(evos, PersonalTable.SV, index),
            PA9 => IsRecordPermitted<PersonalTable9ZA, PersonalInfo9ZA>(evos, PersonalTable.ZA, index),
            PK8 => IsRecordPermitted<PersonalTable8SWSH, PersonalInfo8SWSH>(evos, PersonalTable.SWSH, index),
            _ => false,
        };

        /// <inheritdoc cref="SetRecordFlags(ITechRecord, PKM, TechnicalRecordApplicatorOption, LegalityAnalysis)"/>
        public void SetRecordFlags(PKM pk, TechnicalRecordApplicatorOption option)
        {
            record.ClearRecordFlags();
            if (option is None)
                return;
            if (option is ForceAll)
            {
                record.SetRecordFlagsAll(true, record.Permit.RecordCountUsed);
                return;
            }
            var la = new LegalityAnalysis(pk);
            SetRecordFlagsInternal(record, pk, option, la);
        }

        /// <summary>
        /// Applies the Technical Record flags based on the <see cref="option"/>.
        /// </summary>
        /// <param name="pk">Object to apply to, but base type for other logic.</param>
        /// <param name="option">Option to apply.</param>
        /// <param name="la">Legality analysis to use for the option.</param>
        public void SetRecordFlags(PKM pk, TechnicalRecordApplicatorOption option, LegalityAnalysis la)
        {
            record.ClearRecordFlags();
            if (option is None)
                return;
            if (option is ForceAll)
            {
                record.SetRecordFlagsAll(true, record.Permit.RecordCountUsed);
                return;
            }
            SetRecordFlagsInternal(record, pk, option, la);
        }

        /// <summary>
        /// Sets all the Technical Record flags for the <see cref="record"/> if they are permitted to be learned in-game.
        /// </summary>
        public void SetRecordFlagsAll()
        {
            var permit = record.Permit;
            record.SetRecordFlagsAll(permit);
        }

        /// <inheritdoc cref="SetRecordFlagsAll(PKHeX.Core.ITechRecord)"/>"/>
        public void SetRecordFlagsAll(IPermitRecord permit)
        {
            for (int i = 0; i < permit.RecordCountUsed; i++)
            {
                if (permit.IsRecordPermitted(i))
                    record.SetMoveRecordFlag(i);
            }
        }

        /// <inheritdoc cref="SetRecordFlags(ITechRecord, ReadOnlySpan{ushort})"/>
        public void SetRecordFlags(ReadOnlySpan<ushort> moves, IPermitRecord permit)
        {
            var moveIDs = permit.RecordPermitIndexes;

            foreach (var m in moves)
            {
                var index = moveIDs.IndexOf(m);
                if (index == -1)
                    continue;
                if (permit.IsRecordPermitted(index))
                    record.SetMoveRecordFlag(index);
            }
        }
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

    /// <inheritdoc cref="SetRecordFlags(ITechRecord, PKM, TechnicalRecordApplicatorOption, LegalityAnalysis)"/>
    public static void SetRecordFlags<T>(this T pk, TechnicalRecordApplicatorOption option)
        where T : PKM, ITechRecord
        => pk.SetRecordFlags(pk, option);

    private static void SetRecordFlagsInternal(ITechRecord record, PKM pk, TechnicalRecordApplicatorOption option, LegalityAnalysis la)
    {
        if (option is LegalCurrent)
        {
            Span<ushort> moves = stackalloc ushort[4];
            pk.GetMoves(moves);
            var evos = la.Info.EvoChainsAllGens.Get(pk.Context);
            record.SetRecordFlags(moves, evos);
        }
        else if (option is LegalAll)
        {
            var evos = la.Info.EvoChainsAllGens.Get(pk.Context);
            record.SetRecordFlagsAll(evos);
        }
    }
}

/// <summary>
/// Options for applying Technical Record flags.
/// </summary>
public enum TechnicalRecordApplicatorOption
{
    /// <summary>
    /// Do not apply any flags. Clear all flags.
    /// </summary>
    None,

    /// <summary>
    /// Apply all flags, regardless of legality.
    /// </summary>
    ForceAll,

    /// <summary>
    /// Apply legal flags based on the current moves.
    /// </summary>
    LegalCurrent,

    /// <summary>
    /// Apply legal flags based on all moves able to learn in the game it resides in.
    /// </summary>
    LegalAll,
}
