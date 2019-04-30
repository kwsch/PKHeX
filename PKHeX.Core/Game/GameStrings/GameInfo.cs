using System;
using System.Collections.Generic;

namespace PKHeX.Core
{
    public static class GameInfo
    {
        private static readonly string[] ptransp = { "ポケシフター", "Poké Transfer", "Poké Fret", "Pokétrasporto", "Poképorter", "Pokétransfer", "포케시프터", "宝可传送", "寶可傳送" };
        private static readonly string[] lang_val = { "ja", "en", "fr", "it", "de", "es", "ko", "zh", "zh2" };
        private const string DefaultLanguage = "en";
        public static string CurrentLanguage { get; set; } = DefaultLanguage;
        public static int Language(string lang = null) => Array.IndexOf(lang_val, lang ?? CurrentLanguage);
        public static string Language2Char(uint lang) => lang > lang_val.Length ? DefaultLanguage : lang_val[lang];
        private static readonly GameStrings[] Languages = new GameStrings[lang_val.Length];

        public static readonly IReadOnlyList<string> GenderSymbolUnicode = new[] {"♂", "♀", "-"};
        public static readonly IReadOnlyList<string> GenderSymbolASCII = new[] {"M", "F", "-"};

        // Lazy fetch implementation
        private static int DefaultLanguageIndex => Array.IndexOf(lang_val, DefaultLanguage);

        public static int GetLanguageIndex(string lang)
        {
            int l = Array.IndexOf(lang_val, lang);
            return l < 0 ? DefaultLanguageIndex : l;
        }

        public static GameStrings GetStrings(string lang)
        {
            int index = GetLanguageIndex(lang);
            return GetStrings(index);
        }

        public static GameStrings GetStrings(int index)
        {
            return Languages[index] ?? (Languages[index] = new GameStrings(lang_val[index]));
        }

        public static string GetTransporterName(string lang)
        {
            int index = GetLanguageIndex(lang);
            if (index >= ptransp.Length)
                index = DefaultLanguageIndex;
            return ptransp[index];
        }

        public static GameStrings Strings { get; set; } = GetStrings(DefaultLanguage);

        public static string[] GetStrings(string ident, string lang, string type = "text")
        {
            string[] data = Util.GetStringList(ident, lang, type);
            if (data == null || data.Length == 0)
                data = Util.GetStringList(ident, DefaultLanguage, type);

            return data;
        }

        public static string GetVersionName(GameVersion version)
        {
            var list = (List<ComboItem>) VersionDataSource;
            var first = list.Find(z => z.Value == (int) version);
            return first.Equals(default(ComboItem)) ? version.ToString() : first.Text;
        }

        // DataSource providing
        public static IReadOnlyList<ComboItem> ItemDataSource => Strings.ItemDataSource;
        public static IReadOnlyList<ComboItem> SpeciesDataSource => Strings.SpeciesDataSource;
        public static IReadOnlyList<ComboItem> BallDataSource => Strings.BallDataSource;
        public static IReadOnlyList<ComboItem> NatureDataSource => Strings.NatureDataSource;
        public static IReadOnlyList<ComboItem> AbilityDataSource => Strings.AbilityDataSource;
        public static IReadOnlyList<ComboItem> VersionDataSource => Strings.VersionDataSource;
        public static IReadOnlyList<ComboItem> LegalMoveDataSource => Strings.LegalMoveDataSource;
        public static IReadOnlyList<ComboItem> HaXMoveDataSource => Strings.HaXMoveDataSource;
        public static IReadOnlyList<ComboItem> MoveDataSource => Strings.MoveDataSource;
        public static IReadOnlyList<ComboItem> EncounterTypeDataSource => Strings.EncounterTypeDataSource;
        public static IReadOnlyList<ComboItem> Regions => GameStrings.Regions;

        public static IReadOnlyList<ComboItem> LanguageDataSource(int gen) => GameStrings.LanguageDataSource(gen);

        public static IReadOnlyList<ComboItem> GetAbilityList(int[] abils, int format)
        {
            if (format == 3 && abils[1] == abils[0])
                abils = new[] { abils[0] };
            return Strings.GetAbilityDataSource(abils);
        }

        /// <summary>
        /// Gets the location names array for a specified generation.
        /// </summary>
        /// <param name="gen">Generation to get location names for.</param>
        /// <param name="bankID">BankID used to choose the text bank.</param>
        /// <param name="version">Version of origin</param>
        /// <returns>List of location names.</returns>
        private static IReadOnlyList<string> GetLocationNames(int gen, int bankID, GameVersion version)
        {
            switch (gen)
            {
                case 2: return Strings.metGSC_00000;
                case 3:
                    return version == GameVersion.CXD ? Strings.metCXD_00000 : Strings.metRSEFRLG_00000;
                case 4: return GetLocationNames4(bankID);
                case 5: return GetLocationNames5(bankID);
                case 6: return GetLocationNames6(bankID);
                case 7:
                    if (GameVersion.GG.Contains(version))
                        return GetLocationNames7GG(bankID);
                    return GetLocationNames7(bankID);
                default:
                    return Array.Empty<string>();
            }
        }

        private static IReadOnlyList<string> GetLocationNames4(int bankID)
        {
            switch (bankID)
            {
                case 0: return Strings.metHGSS_00000;
                case 2: return Strings.metHGSS_02000;
                case 3: return Strings.metHGSS_03000;
                default: return Array.Empty<string>();
            }
        }

        private static IReadOnlyList<string> GetLocationNames5(int bankID)
        {
            switch (bankID)
            {
                case 0: return Strings.metBW2_00000;
                case 3: return Strings.metBW2_30000;
                case 4: return Strings.metBW2_40000;
                case 6: return Strings.metBW2_60000;
                default: return Array.Empty<string>();
            }
        }

        private static IReadOnlyList<string> GetLocationNames6(int bankID)
        {
            switch (bankID)
            {
                case 0: return Strings.metXY_00000;
                case 3: return Strings.metXY_30000;
                case 4: return Strings.metXY_40000;
                case 6: return Strings.metXY_60000;
                default: return Array.Empty<string>();
            }
        }

        private static IReadOnlyList<string> GetLocationNames7(int bankID)
        {
            switch (bankID)
            {
                case 0: return Strings.metSM_00000;
                case 3: return Strings.metSM_30000;
                case 4: return Strings.metSM_40000;
                case 6: return Strings.metSM_60000;
                default: return Array.Empty<string>();
            }
        }

        private static IReadOnlyList<string> GetLocationNames7GG(int bankID)
        {
            switch (bankID)
            {
                case 0: return Strings.metGG_00000;
                case 3: return Strings.metGG_30000;
                case 4: return Strings.metGG_40000;
                case 6: return Strings.metGG_60000;
                default: return Array.Empty<string>();
            }
        }

        /// <summary>
        /// Gets the location name for the specified parameters.
        /// </summary>
        /// <param name="eggmet">Location is from the <see cref="PKM.Egg_Location"/></param>
        /// <param name="locval">Location value</param>
        /// <param name="format">Current <see cref="PKM.Format"/></param>
        /// <param name="generation"><see cref="PKM.GenNumber"/> of origin</param>
        /// <param name="version">Current GameVersion (only applicable for <see cref="GameVersion.GG"/> differentiation)</param>
        /// <returns>Location name</returns>
        public static string GetLocationName(bool eggmet, int locval, int format, int generation, GameVersion version)
        {
            int gen = -1;
            int bankID = 0;

            if (format == 2)
            {
                gen = 2;
            }
            else if (format == 3)
            {
                gen = 3;
            }
            else if (generation == 4 && (eggmet || format == 4)) // 4
            {
                const int size = 1000;
                bankID = locval / size;
                gen = 4;
                locval %= size;
            }
            else // 5-7+
            {
                const int size = 10000;
                bankID = locval / size;

                int g = generation;
                if (g >= 5)
                    gen = g;
                else if (format >= 5)
                    gen = format;

                locval %= size;
                if (bankID >= 3) // 30000 and onwards don't use 0th index, shift down 1
                    locval--;
            }

            var bank = GetLocationNames(gen, bankID, version);
            if (bank.Count <= locval)
                return string.Empty;
            return bank[locval];
        }

        /// <summary>
        /// Gets the location list for a specific version, which can retrieve either met locations or egg locations.
        /// </summary>
        /// <param name="version">Version to retrieve for</param>
        /// <param name="pkmFormat">Generation to retrieve for</param>
        /// <param name="egg">Egg Locations are to be retrieved instead of regular Met Locations</param>
        /// <returns>Consumable list of met locations</returns>
        public static IReadOnlyList<ComboItem> GetLocationList(GameVersion version, int pkmFormat, bool egg = false)
        {
            return Strings.GetLocationList(version, pkmFormat, egg);
        }
    }
}
