using static PKHeX.Core.LegalityCheckStrings;

namespace PKHeX.Core
{
    public enum Severity
    {
        Indeterminate = -2,
        Invalid = -1,
        Fishy = 0,
        Valid = 1,
        NotImplemented = 2,
    }

    public static partial class Extensions
    {

        /// <summary>
        /// Converts a Check result Severity determination (Valid/Invalid/etc) to the localized string.
        /// </summary>
        /// <param name="s"><see cref="Severity"/> value to convert to string.</param>
        /// <returns>Localized <see cref="string"/>.</returns>
        public static string Description(this Severity s)
        {
            switch (s)
            {
                case Severity.Indeterminate: return V500;
                case Severity.Invalid: return V501;
                case Severity.Fishy: return V502;
                case Severity.Valid: return V503;
                default: return V504;
            }
        }
    }
}
