using System.Collections.Generic;

namespace PKHeX.Core
{
    /// <summary>
    /// Shared instance for fetching <see cref="GameStrings"/> data
    /// </summary>
    public static class GameInfo
    {
        private static readonly GameStrings?[] Languages = new GameStrings[GameLanguage.LanguageCount];

        public static string CurrentLanguage { get; set; } = GameLanguage.DefaultLanguage;
        public static readonly IReadOnlyList<string> GenderSymbolUnicode = new[] {"♂", "♀", "-"};
        public static readonly IReadOnlyList<string> GenderSymbolASCII = new[] {"M", "F", "-"};
        private static GameStrings _strings = GetStrings(CurrentLanguage);

        public static GameStrings GetStrings(string lang)
        {
            int index = GameLanguage.GetLanguageIndex(lang);
            return GetStrings(index);
        }

        public static GameStrings GetStrings(int index)
        {
            return Languages[index] ??= new GameStrings(GameLanguage.Language2Char(index));
        }

        public static GameStrings Strings
        {
            get => _strings;
            set => Sources = new GameDataSource(_strings = value);
        }

        public static GameDataSource Sources { get; private set; } = new GameDataSource(_strings);
        public static FilteredGameDataSource FilteredSources { get; set; } = new FilteredGameDataSource(FakeSaveFile.Default, Sources, false);

        public static string GetVersionName(GameVersion version)
        {
            var list = (List<ComboItem>) VersionDataSource;
            var first = list.Find(z => z.Value == (int) version);
            return first.Equals(default(ComboItem)) ? version.ToString() : first.Text;
        }

        // DataSource providing
        public static IReadOnlyList<ComboItem> ItemDataSource => FilteredSources.Items;
        public static IReadOnlyList<ComboItem> SpeciesDataSource => Sources.SpeciesDataSource;
        public static IReadOnlyList<ComboItem> BallDataSource => Sources.BallDataSource;
        public static IReadOnlyList<ComboItem> NatureDataSource => Sources.NatureDataSource;
        public static IReadOnlyList<ComboItem> AbilityDataSource => Sources.AbilityDataSource;
        public static IReadOnlyList<ComboItem> VersionDataSource => Sources.VersionDataSource;
        public static IReadOnlyList<ComboItem> MoveDataSource => Sources.HaXMoveDataSource;
        public static IReadOnlyList<ComboItem> EncounterTypeDataSource => Sources.EncounterTypeDataSource;
        public static IReadOnlyList<ComboItem> Regions => GameDataSource.Regions;

        public static IReadOnlyList<ComboItem> LanguageDataSource(int gen) => GameDataSource.LanguageDataSource(gen);

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
            return Strings.GetLocationName(eggmet, locval, format, generation, version);
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
            return Sources.GetLocationList(version, pkmFormat, egg);
        }
    }
}
