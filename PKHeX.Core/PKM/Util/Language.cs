using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core
{
    /// <summary>
    /// Logic relating to <see cref="LanguageID"/> values.
    /// </summary>
    public static class Language
    {
        private static readonly int[] Languages_3 =
        {
            (int)LanguageID.Japanese,
            (int)LanguageID.English,
            (int)LanguageID.French,
            (int)LanguageID.German,
            (int)LanguageID.Spanish,
            (int)LanguageID.Italian,
        };

        // check Korean for the VC case, never possible to match string outside of this case
        private static readonly int[] Languages_GB = Languages_3.Concat(new[] {(int)LanguageID.Korean}).ToArray();
        private static readonly int[] Languages_46 = Languages_GB;
        private static readonly int[] Languages_7 = Languages_46.Concat(new[] {(int)LanguageID.ChineseS, (int)LanguageID.ChineseT}).ToArray();

        public static IReadOnlyList<int> GetAvailableGameLanguages(int generation = PKX.Generation)
        {
            if (generation < 3)
                return Languages_GB;
            if (generation < 4)
                return Languages_3;
            if (generation < 7)
                return Languages_46;
            return Languages_7;
        }

        public static LanguageID GetSafeLanguage(int generation, LanguageID prefer, GameVersion game = GameVersion.Any)
        {
            switch (generation)
            {
                case 1:
                case 2:
                    if (Languages_GB.Contains((int)prefer) && (prefer != LanguageID.Korean || game == GameVersion.C))
                        return prefer;
                    return LanguageID.English;
                case 3:
                    if (Languages_3.Contains((int)prefer))
                        return prefer;
                    return LanguageID.English;
                case 4:
                case 5:
                case 6:
                    if (Languages_46.Contains((int)prefer))
                        return prefer;
                    return LanguageID.English;
                default:
                    if (Languages_7.Contains((int)prefer))
                        return prefer;
                    return LanguageID.English;
            }
        }

        public static string GetLanguage2CharName(this LanguageID lang)
        {
            switch (lang)
            {
                default: return GameLanguage.DefaultLanguage;

                case LanguageID.Japanese: return "ja";
                case LanguageID.French: return "fr";
                case LanguageID.Italian: return "it";
                case LanguageID.German: return "de";
                case LanguageID.Spanish: return "es";
                case LanguageID.Korean: return "ko";
                case LanguageID.ChineseS:
                case LanguageID.ChineseT: return "zh";
            }
        }

        /// <summary>
        /// Gets the Main Series language ID from a GameCube (C/XD) language ID.
        /// </summary>
        /// <param name="value">GameCube (C/XD) language ID.</param>
        /// <returns>Main Series language ID.</returns>
        public static byte GetMainLangIDfromGC(byte value)
        {
            if (value <= 2 || value > 7)
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
            if (value <= 2 || value > 7)
                return value;
            return (byte) MainSeriesToGC[(LanguageID)value];
        }

        private static readonly Dictionary<LanguageID, LanguageGC> MainSeriesToGC = new Dictionary<LanguageID, LanguageGC>
        {
            {LanguageID.German, LanguageGC.German},
            {LanguageID.French, LanguageGC.French},
            {LanguageID.Italian, LanguageGC.Italian},
            {LanguageID.Spanish, LanguageGC.Spanish},
            {LanguageID.UNUSED_6, LanguageGC.UNUSED_6},
        };

        private static readonly Dictionary<LanguageGC, LanguageID> GCtoMainSeries = new Dictionary<LanguageGC, LanguageID>
        {
            {LanguageGC.German, LanguageID.German},
            {LanguageGC.French, LanguageID.French},
            {LanguageGC.Italian, LanguageID.Italian},
            {LanguageGC.Spanish, LanguageID.Spanish},
            {LanguageGC.UNUSED_6, LanguageID.UNUSED_6},
        };
    }
}