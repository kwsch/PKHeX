using System;

namespace PKHeX.Core;

/// <summary>
/// Logic for getting valid movesets.
/// </summary>
public static class MoveSetApplicator
{
    /// <param name="pk">Pokémon to modify.</param>
    extension(PKM pk)
    {
        /// <summary>
        /// Applies a new legal moveset to the <see cref="pk"/>, with option to apply random moves instead.
        /// </summary>
        /// <param name="random">True to apply a random moveset, false to apply a level-up moveset.</param>
        public void SetMoveset(bool random = false)
        {
            Span<ushort> moves = stackalloc ushort[4];
            pk.GetMoveSet(moves, random);
            pk.SetMoves(moves);
        }

        /// <summary>
        /// Applies the suggested Relearn Moves to the <see cref="pk"/>.
        /// </summary>
        /// <param name="la">Legality Analysis to use.</param>
        public void SetRelearnMoves(LegalityAnalysis la)
        {
            Span<ushort> moves = stackalloc ushort[4];
            la.GetSuggestedRelearnMoves(moves);
            pk.SetRelearnMoves(moves);
        }

        /// <summary>
        /// Gets a moveset for the provided <see cref="PKM"/> data.
        /// </summary>
        /// <param name="moves">Result storage</param>
        /// <param name="random">Full movepool &amp; shuffling</param>
        /// <returns>4 moves</returns>
        public void GetMoveSet(Span<ushort> moves, bool random = false)
        {
            var la = new LegalityAnalysis(pk);
            la.GetMoveSet(moves, random);

            if (random)
                return;

            var clone = pk.Clone();
            clone.SetMoves(moves);
            var newLa = new LegalityAnalysis(clone);

            if (newLa.Valid)
                return;

            // ReSharper disable once TailRecursiveCall
            GetMoveSet(pk, moves, true);
        }
    }

    /// <param name="la">Precomputed optional</param>
    extension(LegalityAnalysis la)
    {
        /// <summary>
        /// Gets a moveset for the provided <see cref="PKM"/> data.
        /// </summary>
        /// <param name="moves">Result storage</param>
        /// <param name="random">Full movepool &amp; shuffling</param>
        /// <returns>4 moves</returns>
        public void GetMoveSet(Span<ushort> moves, bool random = false)
        {
            la.GetSuggestedCurrentMoves(moves, random ? MoveSourceType.All : MoveSourceType.Encounter);
            if (random && !la.Entity.IsEgg)
                Util.Rand.Shuffle(moves);
        }

        /// <summary>
        /// Fetches <see cref="PKM.RelearnMoves"/> based on the provided <see cref="LegalityAnalysis"/>.
        /// </summary>
        /// <param name="moves">Result storage</param>
        /// <param name="enc">Encounter the relearn moves should be suggested for. If not provided, will use the original encounter from the analysis. </param>
        /// <returns><see cref="PKM.RelearnMoves"/> best suited for the current <see cref="PKM"/> data.</returns>
        public void GetSuggestedRelearnMoves(Span<ushort> moves, IEncounterTemplate? enc = null)
        {
            enc ??= la.EncounterOriginal;
            la.GetSuggestedRelearnMovesFromEncounter(moves, enc);
            if (moves[0] != 0)
                return;

            if (enc is MysteryGift or IEncounterEgg)
                return;

            if (enc is EncounterSlot6AO {CanDexNav: true} dn)
            {
                var chk = la.Info.Moves;
                for (int i = 0; i < chk.Length; i++)
                {
                    if (!chk[i].ShouldBeInRelearnMoves())
                        continue;

                    var move = la.Entity.GetMove(i);
                    if (!dn.CanBeDexNavMove(move))
                        continue;
                    moves.Clear();
                    moves[0] = move;
                    return;
                }
            }

            if (enc is EncounterSlot8b { IsUnderground: true } ug)
            {
                var chk = la.Info.Moves;
                for (int i = 0; i < chk.Length; i++)
                {
                    if (!chk[i].ShouldBeInRelearnMoves())
                        continue;

                    var move = la.Entity.GetMove(i);
                    if (!ug.CanBeUndergroundMove(move))
                        continue;
                    moves.Clear();
                    moves[0] = move;
                    return;
                }

                if (ug.GetBaseEggMove(out var any))
                {
                    moves.Clear();
                    moves[0] = any;
                    return;
                }
            }

            var encounter = EncounterSuggestion.GetSuggestedMetInfo(la.Entity);
            if (encounter is IRelearn {Relearn: {HasMoves:true} r})
                r.CopyTo(moves);
        }
    }
}
