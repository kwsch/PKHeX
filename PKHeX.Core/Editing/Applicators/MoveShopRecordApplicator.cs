using System;

namespace PKHeX.Core;

/// <summary>
/// Logic for modifying the Move Shop Record flags of a <see cref="PA8"/>.
/// </summary>
public static class MoveShopRecordApplicator
{
    extension(IMoveShop8 shop)
    {
        /// <summary>
        /// Clears all the "purchased" and "mastered" move shop flags.
        /// </summary>
        public void ClearMoveShopFlags()
        {
            var bits = shop.Permit;
            for (int i = 0; i < bits.RecordCountUsed; i++)
                shop.SetPurchasedRecordFlag(i, false);

            if (shop is IMoveShop8Mastery m)
                m.ClearMoveShopFlagsMastered();
        }
    }

    extension(IMoveShop8Mastery shop)
    {
        /// <summary>
        /// Clears all the "mastered" move shop flags.
        /// </summary>
        public void ClearMoveShopFlagsMastered()
        {
            var bits = shop.Permit;
            for (int i = 0; i < bits.RecordCountUsed; i++)
                shop.SetMasteredRecordFlag(i, false);
        }

        /// <summary>
        /// Sets the required move shop flags for the requested entity.
        /// </summary>
        public void SetMoveShopFlags(PKM pk)
        {
            Span<ushort> moves = stackalloc ushort[4];
            pk.GetMoves(moves);
            shop.SetMoveShopFlags(moves, pk);
        }

        /// <summary>
        /// Sets the required move shop flags for the requested entity.
        /// </summary>
        public void SetMoveShopFlags(ReadOnlySpan<ushort> moves, PKM pk)
        {
            var (learn, mastery) = LearnSource8LA.GetLearnsetAndMastery(pk.Species, pk.Form);
            shop.SetMoveShopFlags(moves, learn, mastery, pk.CurrentLevel);
        }

        /// <summary>
        /// Sets all possible move shop flags for the requested entity.
        /// </summary>
        public void SetMoveShopFlagsAll(PKM pk)
        {
            var (learn, mastery) = LearnSource8LA.GetLearnsetAndMastery(pk.Species, pk.Form);
            shop.SetMoveShopFlagsAll(learn, mastery, pk.CurrentLevel);
        }

        /// <summary>
        /// Sets all possible move shop flags for the requested entity.
        /// </summary>
        public void SetMoveShopFlagsAll(Learnset learn, Learnset mastery, byte level)
        {
            var permit = shop.Permit;
            var possible = permit.RecordPermitIndexes;
            for (int index = 0; index < permit.RecordCountUsed; index++)
            {
                var allowed = permit.IsRecordPermitted(index);
                if (!allowed)
                    continue;

                var move = possible[index];
                shop.SetMasteredFlag(learn, mastery, level, index, move);
            }
        }

        /// <summary>
        /// Sets all move shop flags for the currently known moves.
        /// </summary>
        public void SetMoveShopFlags(ReadOnlySpan<ushort> moves, Learnset learn, Learnset mastery, byte level)
        {
            var permit = shop.Permit;
            var possible = permit.RecordPermitIndexes;
            foreach (var move in moves)
            {
                var index = possible.IndexOf(move);
                if (index == -1)
                    continue;
                if (!permit.IsRecordPermitted(index))
                    continue;
                shop.SetMasteredFlag(learn, mastery, level, index, move);
            }
        }

        /// <summary>
        /// Sets the "mastered" move shop flag for the requested move.
        /// </summary>
        public void SetMasteredFlag(Learnset learn, Learnset mastery, byte level, int index, ushort move)
        {
            if (shop.GetMasteredRecordFlag(index))
                return;

            if (learn.TryGetLevelLearnMove(move, out var learnLevel) && level < learnLevel) // Can't learn it yet; must purchase.
            {
                shop.SetPurchasedRecordFlag(index, true);
                shop.SetMasteredRecordFlag(index, true);
                return;
            }

            if (mastery.TryGetLevelLearnMove(move, out var masterLevel) && level < masterLevel) // Can't master it yet; must Seed of Mastery
                shop.SetMasteredRecordFlag(index, true);
        }

        /// <summary>
        /// Sets the "mastered" move shop flag for the encounter.
        /// </summary>
        public void SetEncounterMasteryFlags(ReadOnlySpan<ushort> moves, Learnset mastery, byte level)
        {
            var permit = shop.Permit;
            var possible = permit.RecordPermitIndexes;
            foreach (var move in moves)
            {
                var index = possible.IndexOf(move);
                if (index == -1)
                    continue;
                if (!permit.IsRecordPermitted(index))
                    continue;

                // If the Pokémon is caught with any move shop move in its learnset,
                // and it is high enough level to master it, the game will automatically
                // give it the "Mastered" flag but not the "Purchased" flag
                // For moves that are not in the learnset, set as mastered.
                if (!mastery.TryGetLevelLearnMove(move, out var masteryLevel) || level >= masteryLevel)
                    shop.SetMasteredRecordFlag(index, true);
            }
        }

        /// <summary>
        /// Sets the "purchased" move shop flag for all possible moves.
        /// </summary>
        public void SetPurchasedFlagsAll()
        {
            var permit = shop.Permit;
            for (int index = 0; index < permit.RecordCountUsed; index++)
            {
                var allowed = permit.IsRecordPermitted(index);
                if (!allowed)
                    continue;
                shop.SetPurchasedRecordFlag(index, true);
            }
        }
    }
}
