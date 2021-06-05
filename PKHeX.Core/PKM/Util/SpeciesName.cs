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
        /// Egg name list indexed by the <see cref="LanguageID"/> value.
        /// </summary>
        /// <remarks>Indexing matches <see cref="SpeciesLang"/>.</remarks>
        private static readonly string[] EggNames =
        {
            "タマゴ",
            "タマゴ",
            "Egg",
            "Œuf",
            "Uovo",
            "Ei",
            "Huevo",
            "Huevo",
            "알",
            "蛋",
            "蛋",
        };

        /// <summary>
        /// <see cref="PKM.Nickname"/> to <see cref="Species"/> table for all <see cref="LanguageID"/> values.
        /// </summary>
        public static readonly IReadOnlyList<Dictionary<string, int>> SpeciesDict = Util.GetMultiDictionary(SpeciesLang);

        /// <summary>
        /// Gets a Pokémon's default name for the desired language ID.
        /// </summary>
        /// <param name="species">National Dex number of the Pokémon. Should be 0 if an egg.</param>
        /// <param name="language">Language ID of the Pokémon</param>
        /// <returns>The Species name if within expected range, else an empty string.</returns>
        /// <remarks>Should only be used externally for message displays; for accurate in-game names use <see cref="GetSpeciesNameGeneration"/>.</remarks>
        public static string GetSpeciesName(int species, int language)
        {
            if ((uint)language >= SpeciesLang.Count)
                return string.Empty;

            if (species == 0)
                return EggNames[language];

            var arr = SpeciesLang[language];
            if ((uint)species >= arr.Count)
                return string.Empty;

            return arr[species];
        }

        /// <summary>
        /// Gets a Pokémon's default name for the desired language ID and generation.
        /// </summary>
        /// <param name="species">National Dex number of the Pokémon. Should be 0 if an egg.</param>
        /// <param name="language">Language ID of the Pokémon</param>
        /// <param name="generation">Generation specific formatting option</param>
        /// <returns>Generation specific default species name</returns>
        public static string GetSpeciesNameGeneration(int species, int language, int generation)
        {
            return generation switch
            {
                <= 4 => GetSpeciesName1234(species, language, generation),
                7 when language == (int) LanguageID.ChineseS => GetSpeciesName7ZH(species, language),
                _ => GetSpeciesName(species, language)
            };
        }

        private static string GetSpeciesName1234(int species, int language, int generation)
        {
            if (species == 0)
                return GetEggName1234(species, language, generation);

            string nick = GetSpeciesName(species, language);
            switch (language)
            {
                case (int)LanguageID.Korean when generation == 2:
                    return StringConverter2KOR.LocalizeKOR2(nick);
                case (int)LanguageID.Korean:
                case (int)LanguageID.Japanese:
                    return nick; // No further processing
            }

            // All names are uppercase.
            var sb = new System.Text.StringBuilder(nick);
            for (int i = 0; i < sb.Length; i++)
                sb[i] = char.ToUpper(sb[i]);
            if (language == (int)LanguageID.French)
                StringConverter4.StripDiacriticsFR4(sb); // strips accents on E and I

            // Gen1/2 species names do not have spaces.
            if (generation < 3)
                sb.Replace(" ", string.Empty);

            return sb.ToString();
        }

        private static string GetEggName1234(int species, int language, int generation)
        {
            if (generation == 3)
                return "タマゴ"; // All Gen3 eggs are treated as JPN eggs.

            // Gen2 & Gen4 don't use Œuf like in future games
            if (language == (int)LanguageID.French)
                return generation == 2 ? "OEUF" : "Oeuf";

            var nick = GetSpeciesName(species, language);

            // All Gen4 egg names are Title cased.
            if (generation == 4)
                return nick;

            // Gen2: All Caps
            return nick.ToUpper();
        }

        /// <summary>
        /// Gets the Generation 7 species name for Chinese games.
        /// </summary>
        /// <remarks>
        /// Species Names for Chinese (Simplified) were revised during Generation 8 Crown Tundra DLC (#2).
        /// For a Gen7 species name request, return the old species name (hardcoded... yay).
        /// In an updated Gen8 game, the species nickname will automatically reset to the correct localization (on save/load ?), fixing existing entries.
        /// We don't differentiate patch revisions, just generation; Gen8 will return the latest localization.
        /// Gen8 did revise CHT species names, but only for Barraskewda, Urshifu, and Zarude. These species are new (Gen8); we can just use the latest.
        /// </remarks>
        private static string GetSpeciesName7ZH(int species, int language) => species switch
        {
            // Revised in DLC1 - Isle of Armor
            // https://cn.portal-pokemon.com/topics/event/200323190120_post_19.html
            (int)Species.Porygon2 => "多边兽Ⅱ",  // Later changed to 多边兽２型
            (int)Species.PorygonZ => "多边兽Ｚ", // Later changed to 多边兽乙型
            (int)Species.Mimikyu => "谜拟Ｑ",    // Later changed to 谜拟丘

            // Revised in DLC2 - Crown Tundra
            // https://cn.portal-pokemon.com/topics/event/201020170000_post_21.html
            (int)Species.Cofagrigus => "死神棺", // Later changed to 迭失棺
            (int)Species.Pangoro => "流氓熊猫",  // Later changed to 霸道熊猫
          //(int)Species.Nickit => "偷儿狐",     // Later changed to 狡小狐
          //(int)Species.Thievul => "狐大盗",    // Later changed to 猾大狐
          //(int)Species.Toxel => "毒电婴",      // Later changed to 电音婴
          //(int)Species.Runerigus => "死神板",  // Later changed to 迭失板

            _ => GetSpeciesName(species, language),
        };

        /// <summary>
        /// Checks if the input <see cref="nickname"/> is not the species name for all languages.
        /// </summary>
        /// <param name="species">National Dex number of the Pokémon. Should be 0 if an egg.</param>
        /// <param name="nickname">Current name</param>
        /// <param name="generation">Generation specific formatting option</param>
        /// <returns>True if it does not match any language name, False if not nicknamed</returns>
        public static bool IsNicknamedAnyLanguage(int species, string nickname, int generation = PKX.Generation)
        {
            var langs = Language.GetAvailableGameLanguages(generation);
            return langs.All(language => IsNicknamed(species, nickname, language, generation));
        }

        /// <summary>
        /// Checks if the input <see cref="nickname"/> is not the species name.
        /// </summary>
        /// <param name="species">National Dex number of the Pokémon. Should be 0 if an egg.</param>
        /// <param name="nickname">Current name</param>
        /// <param name="language">Language ID of the Pokémon</param>
        /// <param name="generation">Generation specific formatting option</param>
        /// <returns>True if it does not match the language name, False if not nicknamed (matches).</returns>
        public static bool IsNicknamed(int species, string nickname, int language, int generation = PKX.Generation)
        {
            return GetSpeciesNameGeneration(species, language, generation) != nickname;
        }

        /// <summary>
        /// Gets the Species name Language ID for the current name and generation.
        /// </summary>
        /// <param name="species">National Dex number of the Pokémon. Should be 0 if an egg.</param>
        /// <param name="priorityLanguage">Language ID with a higher priority</param>
        /// <param name="nickname">Current name</param>
        /// <param name="generation">Generation specific formatting option</param>
        /// <returns>Language ID if it does not match any language name, -1 if no matches</returns>
        public static int GetSpeciesNameLanguage(int species, int priorityLanguage, string nickname, int generation = PKX.Generation)
        {
            var langs = Language.GetAvailableGameLanguages(generation);
            if (langs.Contains(priorityLanguage) && GetSpeciesNameGeneration(species, priorityLanguage, generation) == nickname)
                return priorityLanguage;

            return GetSpeciesNameLanguage(species, nickname, generation, langs);
        }

        /// <summary>
        /// Gets the Species name Language ID for the current name and generation.
        /// </summary>
        /// <param name="species">National Dex number of the Pokémon. Should be 0 if an egg.</param>
        /// <param name="nickname">Current name</param>
        /// <param name="generation">Generation specific formatting option</param>
        /// <returns>Language ID if it does not match any language name, -1 if no matches</returns>
        public static int GetSpeciesNameLanguage(int species, string nickname, int generation = PKX.Generation)
        {
            var langs = Language.GetAvailableGameLanguages(generation);
            return GetSpeciesNameLanguage(species, nickname, generation, langs);
        }

        private static int GetSpeciesNameLanguage(int species, string nickname, int generation, IReadOnlyList<int> langs)
        {
            foreach (var lang in langs)
            {
                if (GetSpeciesNameGeneration(species, lang, generation) == nickname)
                    return lang;
            }
            return -1;
        }

        /// <summary>
        /// Gets the Species ID for the specified <see cref="speciesName"/> and <see cref="language"/>.
        /// </summary>
        /// <param name="speciesName">Species Name</param>
        /// <param name="language">Language the name is from</param>
        /// <returns>Species ID</returns>
        /// <remarks>Only use this for modern era name -> ID fetching.</remarks>
        public static int GetSpeciesID(string speciesName, int language = (int)LanguageID.English)
        {
            if (SpeciesDict[language].TryGetValue(speciesName, out var val))
                return val;

            // stupid ’, ignore language if we match these.
            return speciesName switch
            {
                "Farfetch'd" => (int)Species.Farfetchd,
                "Sirfetch'd" => (int)Species.Sirfetchd,
                _ => -1
            };
        }
    }
}
