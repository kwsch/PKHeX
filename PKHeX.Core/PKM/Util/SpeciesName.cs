using System;
using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core
{
    /// <summary>
    /// Logic related to the name of a <see cref="Species"/>.
    /// </summary>
    public static class SpeciesName
    {
        /// <summary>
        /// Species name lists indexed by the <see cref="LanguageID"/> value.
        /// </summary>
        public static readonly IReadOnlyList<IReadOnlyList<string>> SpeciesLang = new[]
        {
            Util.GetSpeciesList("ja"), // 0 (unused, invalid)
            Util.GetSpeciesList("ja"), // 1
            Util.GetSpeciesList("en"), // 2
            Util.GetSpeciesList("fr"), // 3
            Util.GetSpeciesList("it"), // 4
            Util.GetSpeciesList("de"), // 5
            Util.GetSpeciesList("es"), // 6 (reserved for Gen3 KO?, unused)
            Util.GetSpeciesList("es"), // 7
            Util.GetSpeciesList("ko"), // 8
            Util.GetSpeciesList("zh"), // 9 Simplified
            Util.GetSpeciesList("zh2"), // 10 Traditional
        };

        /// <summary>
        /// <see cref="PKM.Nickname"/> to <see cref="Species"/> table for all <see cref="LanguageID"/> values.
        /// </summary>
        public static readonly IReadOnlyList<Dictionary<string, int>> SpeciesDict = Util.GetMultiDictionary(SpeciesLang);

        /// <summary>
        /// Gets a Pokémon's default name for the desired language ID.
        /// </summary>
        /// <param name="species">National Dex number of the Pokémon. Should be 0 if an egg.</param>
        /// <param name="lang">Language ID of the Pokémon</param>
        /// <returns>The Species name if within expected range, else an empty string.</returns>
        /// <remarks>Should only be used externally for message displays; for accurate in-game names use <see cref="GetSpeciesNameGeneration"/>.</remarks>
        public static string GetSpeciesName(int species, int lang)
        {
            if ((uint)lang >= SpeciesLang.Count)
                return string.Empty;

            var arr = SpeciesLang[lang];
            if ((uint)species >= arr.Count)
                return string.Empty;

            return arr[species];
        }

        /// <summary>
        /// Gets a Pokémon's default name for the desired language ID and generation.
        /// </summary>
        /// <param name="species">National Dex number of the Pokémon. Should be 0 if an egg.</param>
        /// <param name="lang">Language ID of the Pokémon</param>
        /// <param name="generation">Generation specific formatting option</param>
        /// <returns>Generation specific default species name</returns>
        public static string GetSpeciesNameGeneration(int species, int lang, int generation)
        {
            if (generation >= 5)
                return GetSpeciesName(species, lang);

            if (generation == 3 && species == 0)
                return "タマゴ";

            string nick = GetSpeciesName(species, lang);
            if (generation == 2 && lang == (int)LanguageID.Korean)
                return StringConverter2KOR.LocalizeKOR2(nick);

            if (generation < 5 && (generation != 4 || species != 0)) // All caps GenIV and previous, except GenIV eggs.
            {
                nick = nick.ToUpper();
                if (lang == (int)LanguageID.French)
                    nick = StringConverter4.StripDiacriticsFR4(nick); // strips accents on E and I
            }
            if (generation < 3)
                nick = nick.Replace(" ", string.Empty);
            return nick;
        }

        /// <summary>
        /// Checks if a nickname matches the species name of any language.
        /// </summary>
        /// <param name="species">National Dex number of the Pokémon. Should be 0 if an egg.</param>
        /// <param name="nick">Current name</param>
        /// <param name="generation">Generation specific formatting option</param>
        /// <returns>True if it does not match any language name, False if not nicknamed</returns>
        public static bool IsNicknamedAnyLanguage(int species, string nick, int generation = PKX.Generation)
        {
            if (species == (int)Species.Farfetchd && string.Equals(nick, "Farfetch'd", StringComparison.OrdinalIgnoreCase)) // stupid ’
                return false;
            if (species == (int)Species.Sirfetchd && string.Equals(nick, "Sirfetch'd", StringComparison.OrdinalIgnoreCase)) // stupid ’
                return false;

            var langs = Language.GetAvailableGameLanguages(generation);
            return langs.All(lang => GetSpeciesNameGeneration(species, lang, generation) != nick);
        }

        /// <summary>
        /// Gets the Species name Language ID for the current name and generation.
        /// </summary>
        /// <param name="species">National Dex number of the Pokémon. Should be 0 if an egg.</param>
        /// <param name="priorityLanguage">Language ID with a higher priority</param>
        /// <param name="nick">Current name</param>
        /// <param name="generation">Generation specific formatting option</param>
        /// <returns>Language ID if it does not match any language name, -1 if no matches</returns>
        public static int GetSpeciesNameLanguage(int species, int priorityLanguage, string nick, int generation = PKX.Generation)
        {
            var langs = Language.GetAvailableGameLanguages(generation);
            if (langs.Contains(priorityLanguage) && GetSpeciesNameGeneration(species, priorityLanguage, generation) == nick)
                return priorityLanguage;

            return GetSpeciesNameLanguage(species, nick, generation, langs);
        }

        /// <summary>
        /// Gets the Species name Language ID for the current name and generation.
        /// </summary>
        /// <param name="species">National Dex number of the Pokémon. Should be 0 if an egg.</param>
        /// <param name="nick">Current name</param>
        /// <param name="generation">Generation specific formatting option</param>
        /// <returns>Language ID if it does not match any language name, -1 if no matches</returns>
        public static int GetSpeciesNameLanguage(int species, string nick, int generation = PKX.Generation)
        {
            var langs = Language.GetAvailableGameLanguages(generation);
            return GetSpeciesNameLanguage(species, nick, generation, langs);
        }

        private static int GetSpeciesNameLanguage(int species, string nick, int generation, IReadOnlyList<int> langs)
        {
            foreach (var lang in langs)
            {
                if (GetSpeciesNameGeneration(species, lang, generation) == nick)
                    return lang;
            }
            return -1;
        }

        /// <summary>
        /// Gets the Species ID for the specified <see cref="specName"/> and <see cref="language"/>.
        /// </summary>
        /// <param name="specName">Species Name</param>
        /// <param name="language">Language the name is from</param>
        /// <returns>Species ID</returns>
        /// <remarks>Only use this for modern era name -> ID fetching.</remarks>
        public static int GetSpeciesID(string specName, int language = (int)LanguageID.English)
        {
            return SpeciesDict[language].TryGetValue(specName, out var val) ? val : -1;
        }
    }
}