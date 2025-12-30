using System;

namespace PKHeX.Core;

/// <summary>
/// Logic for modifying the Plus Record flags of a <see cref="PA9"/>.
/// </summary>
public static class PlusRecordApplicator
{
    extension(IPlusRecord record)
    {
        /// <summary>
        /// Sets all the Plus Record flags for the <see cref="record"/> to the given value.
        /// </summary>
        /// <param name="count">Total count of flags to modify [0,x).</param>
        /// <param name="value">Value to set for each record.</param>
        public void SetPlusFlagsAll(int count, bool value)
        {
            for (int i = 0; i < count; i++)
                record.SetMovePlusFlag(i, value);
        }

        /// <summary>
        /// Clears the Plus Record flags for the <see cref="record"/>.
        /// </summary>
        /// <param name="count">Total count of flags to modify [0,x).</param>
        public void ClearPlusFlags(int count) => record.SetPlusFlagsAll(count, false);

        /// <summary>
        /// Sets the Plus Record flags for the <see cref="record"/> based on the legality of learning moves.
        /// </summary>
        /// <param name="permit">Sanity check to retrieve plus record indexes.</param>
        /// <param name="la">Legality analysis of the Pokémon.</param>
        /// <param name="seedOfMastery">Use a Seed of Mastery to bypass the level requirement of mastering the move.</param>
        /// <param name="tm">Apply TM flags as Plus too.</param>
        public void SetPlusFlags(IPermitPlus permit, LegalityAnalysis la, bool seedOfMastery, bool tm)
        {
            // Hopefully this is only called for Legends: Z-A format entities.
            var entity = la.Entity;
            var context = entity.Context;
            var evos = la.Info.EvoChainsAllGens.Get(context);
            switch (la.Entity)
            {
                case PA9 pa9:
                {
                    var learn = LearnSource9ZA.Instance;
                    record.SetPlusFlagsNatural(permit, evos, learn, seedOfMastery);
                    if (pa9 is { IsAlpha: true, ZA: true })
                    {
                        var table = PersonalTable.ZA;
                        var enc = la.EncounterMatch;
                        var epi = table[enc.Species, enc.Form];
                        pa9.SetPlusFlagsSpecific(epi, epi.AlphaMove);
                    }

                    if (tm)
                    {
                        var table = PersonalTable.ZA;
                        record.SetPlusFlagsTM<PersonalTable9ZA, PersonalInfo9ZA>(permit, evos, table);
                    }
                    break;
                }
                default:
                    throw new Exception("Format not supported.");
            }
        }

        public void SetPlusFlags(PKM pk, IPermitPlus permit, PlusRecordApplicatorOption option)
        {
            record.ClearPlusFlags(permit.PlusCountTotal);
            if (option is PlusRecordApplicatorOption.None)
                return;
            if (option is PlusRecordApplicatorOption.ForceAll)
            {
                record.SetPlusFlagsAll(permit.PlusCountUsed, true);
                return;
            }

            var la = new LegalityAnalysis(pk);
            record.SetPlusFlagsInternal(permit, option, la);
        }

        public void SetPlusFlags(IPermitPlus permit, PlusRecordApplicatorOption option, LegalityAnalysis la)
        {
            record.ClearPlusFlags(permit.PlusCountTotal);
            if (option is PlusRecordApplicatorOption.None)
                return;
            if (option is PlusRecordApplicatorOption.ForceAll)
            {
                record.SetPlusFlagsAll(permit.PlusCountUsed, true);
                return;
            }

            record.SetPlusFlagsInternal(permit, option, la);
        }

        public void SetPlusFlagsNatural<TSource>(IPermitPlus permit, ReadOnlySpan<EvoCriteria> evos, TSource source, bool seedOfMastery) where TSource : ILearnSourceBonus
        {
            var indexes = permit.PlusMoveIndexes;
            foreach (var evo in evos)
            {
                var (levelUp, plus) = source.GetLearnsetAndOther(evo.Species, evo.Form);
                var set = seedOfMastery ? levelUp : plus;
                var levels = set.GetAllLevels();
                var moves = set.GetAllMoves();
                for (int i = 0; i < levels.Length; i++)
                {
                    if (evo.LevelMax < levels[i])
                        break;

                    var move = moves[i];
                    var index = indexes.IndexOf(move);
                    record.SetMovePlusFlag(index);
                }
            }
        }

        /// <summary>
        /// Sets all moves that would be learned and naturally available as Plus based on the given level
        /// </summary>
        /// <param name="permit">Permit to use</param>
        /// <param name="plus">Learnset to use</param>
        /// <param name="level">Current level</param>
        /// <param name="extra">Extra moves to set as Plus</param>
        public void SetPlusFlagsEncounter(IPermitPlus permit, Learnset plus, byte level, params ReadOnlySpan<ushort> extra)
        {
            var indexes = permit.PlusMoveIndexes;
            var levels = plus.GetAllLevels();
            var moves = plus.GetAllMoves();

            for (int i = 0; i < levels.Length; i++)
            {
                if (level < levels[i])
                    break;

                var move = moves[i];
                var index = indexes.IndexOf(move);
                record.SetMovePlusFlag(index);
            }

            if (extra.Length != 0)
                record.SetPlusFlagsSpecific(permit, extra);
        }


        public void SetPlusFlagsSpecific(IPermitPlus permit, ushort move)
        {
            var indexes = permit.PlusMoveIndexes;
            var index = indexes.IndexOf(move);
            record.SetMovePlusFlag(index);
        }

        public void SetPlusFlagsSpecific(IPermitPlus permit, params ReadOnlySpan<ushort> extra)
        {
            var indexes = permit.PlusMoveIndexes;
            foreach (var move in extra)
            {
                var index = indexes.IndexOf(move);
                record.SetMovePlusFlag(index);
            }
        }

        private void SetPlusFlagsInternal(IPermitPlus permit, PlusRecordApplicatorOption option, LegalityAnalysis la)
        {
            if (option is PlusRecordApplicatorOption.LegalCurrent)
                record.SetPlusFlags(permit, la, false, false);
            else if (option is PlusRecordApplicatorOption.LegalCurrentTM)
                record.SetPlusFlags(permit, la, false, true);
            else if (option is PlusRecordApplicatorOption.LegalSeedTM)
                record.SetPlusFlags(permit, la, true, true);
        }

        public void SetPlusFlagsTM<TTable, TInfo>(IPermitPlus permit, ReadOnlySpan<EvoCriteria> evos, TTable table)
            where TTable : IPersonalTable<TInfo>
            where TInfo : IPersonalInfo, IPersonalInfoTM
        {
            var indexes = permit.PlusMoveIndexes;
            foreach (var evo in evos)
            {
                var pi = table[evo.Species, evo.Form];
                for (int index = 0; index < indexes.Length; index++)
                {
                    var move = indexes[index];
                    var tmIndex = permit.RecordPermitIndexes.IndexOf(move);
                    if (tmIndex != -1 && pi.GetIsLearnTM(tmIndex))
                        record.SetMovePlusFlag(index);
                }
            }
        }
    }

    public static void SetPlusFlags<T>(this T pk, IPermitPlus permit, PlusRecordApplicatorOption option)
        where T : PKM, IPlusRecord
        => pk.SetPlusFlags(pk, permit, option);
}

public enum PlusRecordApplicatorOption
{
    None,
    ForceAll,
    LegalCurrent,
    LegalCurrentTM,
    LegalSeedTM,
}
