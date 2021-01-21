using System;
using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core
{
    public static class MoveSetApplicator
    {
        /// <summary>
        /// Gets a moveset for the provided <see cref="PKM"/> data.
        /// </summary>
        /// <param name="pk">PKM to generate for</param>
        /// <param name="random">Full movepool &amp; shuffling</param>
        /// <returns>4 moves</returns>
        public static int[] GetMoveSet(this PKM pk, bool random = false)
        {
            var la = new LegalityAnalysis(pk);
            var moves = la.GetMoveSet(random);

            if (random)
                return moves;

            var clone = pk.Clone();
            clone.SetMoves(moves);
            clone.SetMaximumPPCurrent(moves);
            var newLa = new LegalityAnalysis(clone);

            // ReSharper disable once TailRecursiveCall
            return newLa.Valid ? moves : GetMoveSet(pk, true);
        }

        /// <summary>
        /// Gets a moveset for the provided <see cref="PKM"/> data.
        /// </summary>
        /// <param name="la">Precomputed optional</param>
        /// <param name="random">Full movepool &amp; shuffling</param>
        /// <returns>4 moves</returns>
        public static int[] GetMoveSet(this LegalityAnalysis la, bool random = false)
        {
            int[] m = la.GetSuggestedCurrentMoves(random ? MoveSourceType.All : MoveSourceType.Encounter);

            var learn = la.GetSuggestedMovesAndRelearn();
            if (!m.All(z => learn.Contains(z)))
                m = m.Intersect(learn).ToArray();

            if (random && !la.pkm.IsEgg)
                Util.Shuffle(m);

            const int count = 4;
            if (m.Length > count)
                return m.SliceEnd(m.Length - count);
            Array.Resize(ref m, count);
            return m;
        }

        /// <summary>
        /// Fetches <see cref="PKM.RelearnMoves"/> based on the provided <see cref="LegalityAnalysis"/>.
        /// </summary>
        /// <param name="pk">Pokémon to modify.</param>
        /// <returns><see cref="PKM.RelearnMoves"/> best suited for the current <see cref="PKM"/> data.</returns>
        public static IReadOnlyList<int> GetSuggestedRelearnMoves(this PKM pk) => GetSuggestedRelearnMoves(new LegalityAnalysis(pk));

        /// <summary>
        /// Fetches <see cref="PKM.RelearnMoves"/> based on the provided <see cref="LegalityAnalysis"/>.
        /// </summary>
        /// <param name="legal"><see cref="LegalityAnalysis"/> which contains parsed information pertaining to legality.</param>
        /// <returns><see cref="PKM.RelearnMoves"/> best suited for the current <see cref="PKM"/> data.</returns>
        public static IReadOnlyList<int> GetSuggestedRelearnMoves(this LegalityAnalysis legal)
        {
            var m = legal.GetSuggestedRelearnMovesFromEncounter();
            if (m.Any(z => z != 0))
                return m;

            var enc = legal.EncounterMatch;
            if (enc is MysteryGift || enc is EncounterEgg)
                return m;

            var encounter = EncounterSuggestion.GetSuggestedMetInfo(legal.pkm);
            if (encounter is IRelearn r && r.Relearn.Count > 0)
                return r.Relearn;

            return m;
        }
    }
}