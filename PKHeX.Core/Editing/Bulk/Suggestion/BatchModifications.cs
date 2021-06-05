using System;
using System.Globalization;

namespace PKHeX.Core
{
    /// <summary>
    /// Modifications using <see cref="BatchInfo"/> legality.
    /// </summary>
    internal static class BatchModifications
    {
        private static bool IsAll(string p) => p.EndsWith("All", true, CultureInfo.CurrentCulture);
        private static bool IsNone(string p) => p.EndsWith("None", true, CultureInfo.CurrentCulture);

        public static ModifyResult SetSuggestedRelearnData(BatchInfo info, string propValue)
        {
            var pk = info.Entity;
            if (pk.Format >= 8)
            {
                pk.ClearRecordFlags();
                if (IsAll(propValue))
                    pk.SetRecordFlags(); // all
                else if (!IsNone(propValue))
                    pk.SetRecordFlags(pk.Moves); // whatever fit the current moves
            }

            pk.SetRelearnMoves(info.SuggestedRelearn);
            return ModifyResult.Modified;
        }

        public static ModifyResult SetSuggestedRibbons(BatchInfo info, string value)
        {
            var pk = info.Entity;
            if (IsNone(value))
                RibbonApplicator.RemoveAllValidRibbons(pk);
            else // All
                RibbonApplicator.SetAllValidRibbons(pk);
            return ModifyResult.Modified;
        }

        public static ModifyResult SetSuggestedMetData(BatchInfo info)
        {
            var pk = info.Entity;
            var encounter = EncounterSuggestion.GetSuggestedMetInfo(pk);
            if (encounter == null)
                return ModifyResult.Error;

            int level = encounter.LevelMin;
            int location = encounter.Location;
            int minimumLevel = EncounterSuggestion.GetLowestLevel(pk, encounter.LevelMin);

            pk.Met_Level = level;
            pk.Met_Location = location;
            pk.CurrentLevel = Math.Max(minimumLevel, level);

            return ModifyResult.Modified;
        }

        /// <summary>
        /// Sets the provided moves in a random order.
        /// </summary>
        /// <param name="pk">Pokémon to modify.</param>
        /// <param name="moves">Moves to apply.</param>
        public static ModifyResult SetMoves(PKM pk, int[] moves)
        {
            pk.SetMoves(moves);
            pk.HealPP();
            return ModifyResult.Modified;
        }
    }
}
