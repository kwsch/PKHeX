using System.Collections.Generic;
using System.Linq;
using static PKHeX.Core.LanguageID;

namespace PKHeX.Core
{
    /// <summary>
    /// Logic relating to <see cref="LanguageID"/> values.
    /// </summary>
    public static class Language
    {
        private static readonly int[] Languages_3 =
        {
            (int)Japanese,
            (int)English,
            (int)French,
            (int)German,
            (int)Spanish,
            (int)Italian,
        };

        // check Korean for the VC case, never possible to match string outside of this case
        private static readonly int[] Languages_GB = Languages_3.Concat(new[] {(int)Korean}).ToArray();
        private static readonly int[] Languages_46 = Languages_GB;
        private static readonly int[] Languages_7 = Languages_46.Concat(new[] {(int)ChineseS, (int)ChineseT}).ToArray();
        private const LanguageID SafeLanguage = English;

        public static IReadOnlyList<int> GetAvailableGameLanguages(int generation = PKX.Generation) => generation switch
        {
            < 3 => Languages_GB,
            < 4 => Languages_3,
            < 7 => Languages_46,
              _ => Languages_7,
        };

        public static LanguageID GetSafeLanguage(int generation, LanguageID prefer, GameVersion game = GameVersion.Any) => generation switch
        {
            1 when game == GameVersion.BU => Japanese,
            1 or 2      => Languages_GB.Contains((int)prefer) && (prefer != Korean || game == GameVersion.C) ? prefer : SafeLanguage,
            3           => Languages_3 .Contains((int)prefer) ? prefer : SafeLanguage,
            4 or 5 or 6 => Languages_46.Contains((int)prefer) ? prefer : SafeLanguage,
            _           => Languages_7 .Contains((int)prefer) ? prefer : SafeLanguage,
        };

        public static string GetLanguage2CharName(this LanguageID language) => language switch
        {
            Japanese => "ja",
            French => "fr",
            Italian => "it",
            German => "de",
            Spanish => "es",
            Korean => "ko",
            ChineseS or ChineseT => "zh",
            _ => GameLanguage.DefaultLanguage,
        };

        /// <summary>
        /// Gets the Main Series language ID from a GameCube (C/XD) language ID.
        /// </summary>
        /// <param name="value">GameCube (C/XD) language ID.</param>
        /// <returns>Main Series language ID.</returns>
        public static byte GetMainLangIDfromGC(byte value)
        {
            if (value is <= 2 or > 7)
                return value;
            return (byte) GCtoMainSeries[(LanguageGC)value];
        }

        /// <summary>
        /// Gets the GameCube (C/XD) language ID from a Main Series language ID.
        /// </summary>
        /// <param name="value">Main Series language ID.</param>
        /// <returns>GameCube (C/XD) language ID.</returns>
        public static byte GetGCLangIDfromMain(byte value)
        {
            if (value is <= 2 or > 7)
                return value;
            return (byte) MainSeriesToGC[(LanguageID)value];
        }

        private static readonly Dictionary<LanguageID, LanguageGC> MainSeriesToGC = new()
        {
            {German, LanguageGC.German},
            {French, LanguageGC.French},
            {Italian, LanguageGC.Italian},
            {Spanish, LanguageGC.Spanish},
            {UNUSED_6, LanguageGC.UNUSED_6},
        };

        private static readonly Dictionary<LanguageGC, LanguageID> GCtoMainSeries = new()
        {
            {LanguageGC.German, German},
            {LanguageGC.French, French},
            {LanguageGC.Italian, Italian},
            {LanguageGC.Spanish, Spanish},
            {LanguageGC.UNUSED_6, UNUSED_6},
        };
    }
}
