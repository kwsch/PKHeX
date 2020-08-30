using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core
{
    /// <summary>
    /// Miscellaneous setup utility for legality checking <see cref="IEncounterable"/> data sources.
    /// </summary>
    internal static class EncounterUtil
    {
        /// <summary>
        /// Gets the relevant <see cref="EncounterStatic"/> objects that appear in the relevant game.
        /// </summary>
        /// <param name="source">Table of valid encounters that appear for the game pairing</param>
        /// <param name="game">Game to filter for</param>
        /// <returns>Array of encounter objects that can be encountered in the input game</returns>
        internal static T[] GetEncounters<T>(IEnumerable<T> source, GameVersion game) where T : IVersion
        {
            return source.Where(s => s.Version.Contains(game)).ToArray();
        }

        internal static T[] ConcatAll<T>(params IEnumerable<T>[] arr) => arr.SelectMany(z => z).ToArray();

        internal static EncounterStatic Clone(this EncounterStatic s, int location)
        {
            var result = s.Clone();
            result.Location = location;
            return result;
        }

        internal static T[] Clone<T>(this T s, int[] locations) where T : EncounterStatic
        {
            var Encounters = new T[locations.Length];
            for (int i = 0; i < locations.Length; i++)
                Encounters[i] = (T)s.Clone(locations[i]);
            return Encounters;
        }

        internal static IEnumerable<EncounterStatic5> DreamRadarClone(this EncounterStatic5 s)
        {
            for (int i = 0; i < 8; i++)
                yield return s.DreamRadarClone((5 * i) + 5);  // Level from 5->40 depends on the number of badges
        }

        private static EncounterStatic5 DreamRadarClone(this EncounterStatic5 s, int level)
        {
            var result = (EncounterStatic5)s.Clone(level);
            result.Level = level;
            result.Location = 30015;// Pokemon Dream Radar
            result.Gift = true;     // Only
            result.Ball = 25;       // Dream Ball
            return result;
        }

        internal static void MarkEncounterTradeStrings<T>(T[] table, string[][] strings) where T : EncounterTrade
        {
            int half = strings[1].Length / 2;
            for (int i = 0; i < half; i++)
            {
                var t = table[i];
                t.Nicknames = getNames(i, strings);
                t.TrainerNames = getNames(i + half, strings);
            }
            string[] getNames(int i, IEnumerable<string[]> names) => names.Select(z => z.Length > i ? z[i] : string.Empty).ToArray();
        }
    }
}
