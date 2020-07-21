using System;

namespace PKHeX.Core
{
    /// <summary>
    /// Language code &amp; string asset loading.
    /// </summary>
    public static class GameLanguage
    {
        public const string DefaultLanguage = "en"; // English
        public static int DefaultLanguageIndex => Array.IndexOf(LanguageCodes, DefaultLanguage);
        public static string Language2Char(int lang) => lang > LanguageCodes.Length ? DefaultLanguage : LanguageCodes[lang];

        public static int LanguageCount => LanguageCodes.Length;

        /// <summary>
        /// Gets the language from the requested 2-char <see cref="lang"/> code.
        /// </summary>
        /// <param name="lang">2 character language code</param>
        /// <returns>Index of the language code; if not a valid language code, returns the <see cref="DefaultLanguageIndex"/>.</returns>
        public static int GetLanguageIndex(string lang)
        {
            int l = Array.IndexOf(LanguageCodes, lang);
            return l < 0 ? DefaultLanguageIndex : l;
        }

        /// <summary>
        /// Language codes supported for loading string resources
        /// </summary>
        /// <see cref="ProgramLanguage"/>
        private static readonly string[] LanguageCodes = { "ja", "en", "fr", "it", "de", "es", "ko", "zh", "zh2" };

        /// <summary>
        /// Pokétransporter location names, ordered per index of <see cref="LanguageCodes"/>
        /// </summary>
        private static readonly string[] ptransp = { "ポケシフター", "Poké Transfer", "Poké Fret", "Pokétrasporto", "Poképorter", "Pokétransfer", "포케시프터", "宝可传送", "寶可傳送" };

        public static string GetTransporterName(int index)
        {
            if ((uint)index >= ptransp.Length)
                index = 2;
            return ptransp[index];
        }

        public static string GetTransporterName(string lang) => GetTransporterName(GetLanguageIndex(lang));

        public static string[] GetStrings(string ident, string lang, string type = "text")
        {
            string[] data = Util.GetStringList(ident, lang, type);
            if (data.Length == 0)
                data = Util.GetStringList(ident, DefaultLanguage, type);

            return data;
        }
    }

    public enum ProgramLanguage
    {
        /// <summary>
        /// Japanese
        /// </summary>
        日本語,

        /// <summary>
        /// English
        /// </summary>
        English,

        /// <summary>
        /// French
        /// </summary>
        Français,

        /// <summary>
        /// Italian
        /// </summary>
        Italiano,

        /// <summary>
        /// German
        /// </summary>
        Deutsch,

        /// <summary>
        /// Spanish
        /// </summary>
        Español,

        /// <summary>
        /// Korean
        /// </summary>
        한국어,

        /// <summary>
        /// Chinese
        /// </summary>
        中文,
    }
}