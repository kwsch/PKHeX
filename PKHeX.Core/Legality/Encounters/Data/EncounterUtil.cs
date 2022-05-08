using System;
using System.Collections.Generic;

namespace PKHeX.Core
{
    /// <summary>
    /// Miscellaneous setup utility for legality checking <see cref="IEncounterTemplate"/> data sources.
    /// </summary>
    internal static class EncounterUtil
    {
        internal static BinLinkerAccessor Get(string resource, string ident) => BinLinkerAccessor.Get(Util.GetBinaryResource($"encounter_{resource}.pkl"), ident);

        /// <summary>
        /// Gets the relevant <see cref="EncounterStatic"/> objects that appear in the relevant game.
        /// </summary>
        /// <param name="source">Table of valid encounters that appear for the game pairing</param>
        /// <param name="game">Game to filter for</param>
        /// <returns>Array of encounter objects that can be encountered in the input game</returns>
        internal static T[] GetEncounters<T>(T[] source, GameVersion game) where T : EncounterStatic
        {
            return Array.FindAll(source, s => s.Version.Contains(game));
        }

        internal static T? GetMinByLevel<T>(EvoCriteria[] chain, IEnumerable<T> possible) where T : class, IEncounterTemplate
        {
            // MinBy grading: prefer species-form match, select lowest min level encounter.
            // Minimum allocation :)
            T? result = null;
            int min = int.MaxValue;

            foreach (var enc in possible)
            {
                int m = int.MaxValue;
                foreach (var evo in chain)
                {
                    bool specDiff = enc.Species != evo.Species || enc.Form != evo.Form;
                    var val = (Convert.ToInt32(specDiff) << 16) | enc.LevelMin;
                    if (val < m)
                        m = val;
                }

                if (m >= min)
                    continue;
                min = m;
                result = enc;
            }

            return result;
        }

        /// <summary>
        /// Loads the language string lists into the <see cref="T"/> objects.
        /// </summary>
        /// <typeparam name="T">Encounter template type</typeparam>
        /// <param name="table">Trade templates</param>
        /// <param name="strings">Localization strings, grouped by language.</param>
        /// <remarks>
        /// The first half of strings in the language resource array are <see cref="EncounterTrade.Nicknames"/>
        /// The second half of strings in the language resource strings are <see cref="EncounterTrade.TrainerNames"/>
        /// </remarks>
        internal static void MarkEncounterTradeStrings<T>(T[] table, ReadOnlySpan<string[]> strings) where T : EncounterTrade
        {
            uint languageCount = (uint)strings[1].Length / 2;
            for (uint i = 0; i < languageCount; i++)
            {
                var t = table[i];
                t.Nicknames = GetNamesForLanguage(strings, i);
                t.TrainerNames = GetNamesForLanguage(strings, languageCount + i);
            }
        }

        /// <summary>
        /// Loads the language string lists into the <see cref="T"/> objects.
        /// </summary>
        /// <typeparam name="T">Encounter template type</typeparam>
        /// <param name="table">Trade templates</param>
        /// <param name="strings">Localization strings, grouped by language.</param>
        internal static void MarkEncounterTradeNicknames<T>(T[] table, ReadOnlySpan<string[]> strings) where T : EncounterTrade
        {
            for (uint i = 0; i < table.Length; i++)
            {
                var t = table[i];
                t.Nicknames = GetNamesForLanguage(strings, i);
            }
        }

        /// <summary>
        /// Grabs the localized names for individual templates for all languages from the specified <see cref="index"/> of the <see cref="names"/> list.
        /// </summary>
        /// <param name="names">Arrays of strings grouped by language</param>
        /// <param name="index">Index to grab from the language arrays</param>
        /// <returns>Row of localized strings for the template.</returns>
        private static string[] GetNamesForLanguage(ReadOnlySpan<string[]> names, uint index)
        {
            var result = new string[names.Length];
            for (int i = 0; i < result.Length; i++)
            {
                var arr = names[i];
                result[i] = index < arr.Length ? arr[index] : string.Empty;
            }
            return result;
        }
    }
}
