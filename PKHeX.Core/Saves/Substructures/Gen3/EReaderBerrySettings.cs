using static PKHeX.Core.EReaderBerryMatch;

namespace PKHeX.Core
{
    /// <summary>
    /// Stores details about the Gen3 e-Reader Berry values.
    /// </summary>
    public static class EReaderBerrySettings
    {
        /// <summary> e-Reader Berry is Enigma or special berry (from e-Reader data)</summary>
        public static bool IsEnigma { get; set; } = true;

        /// <summary> e-Reader Berry Name </summary>
        private static string Name { get; set; } = string.Empty;

        /// <summary> e-Reader Berry Name formatted in Title Case </summary>
        public static string DisplayName => string.Format(LegalityCheckStrings.L_XEnigmaBerry_0, Util.ToTitleCase(Name));

        private static int Language { get; set; }

        public static EReaderBerryMatch GetStatus()
        {
            if (IsEnigma) // no e-Reader Berry data provided, can't hold berry.
                return NoData;

            var matchUSA = Legal.EReaderBerriesNames_USA.Contains(Name);
            if (matchUSA)
            {
                if (Language <= 0)
                    return ValidAny;
                if (Language != 1)
                    return ValidUSA;
                return InvalidUSA;
            }

            var matchJP = Legal.EReaderBerriesNames_JP.Contains(Name);
            if (matchJP)
            {
                if (Language <= 0)
                    return ValidAny;
                if (Language == 1)
                    return ValidJPN;
                return InvalidJPN;
            }

            return NoMatch;
        }

        public static void LoadFrom(SAV3 sav3)
        {
            IsEnigma = sav3.IsEBerryIsEnigma;
            Name = sav3.EBerryName;
            Language = sav3.Japanese ? (int)LanguageID.Japanese : (int)LanguageID.English;
        }
    }

    /// <summary>
    /// For use in validating the current e-Reader Berry settings.
    /// </summary>
    public enum EReaderBerryMatch : byte
    {
        /// <summary> Invalid: Doesn't match either language </summary>
        NoMatch,
        /// <summary> Invalid: No data provided from a save file </summary>
        NoData,
        /// <summary> Invalid: USA berry name on JPN save file </summary>
        InvalidUSA,
        /// <summary> Invalid: JPN berry name on USA save file</summary>
        InvalidJPN,

        /// <summary> Valid: Berry name matches either USA/JPN (Ambiguous Save File Language) </summary>
        ValidAny,
        /// <summary> Valid: Berry name matches a USA berry name </summary>
        ValidUSA,
        /// <summary> Valid: Berry name matches a JPN berry name </summary>
        ValidJPN,
    }
}
