using static PKHeX.Core.LegalityCheckStrings;

namespace PKHeX.Core
{
    /// <summary> Severity indication of the associated <see cref="CheckResult"/> </summary>
    /// <remarks>
    /// Severity &gt;= <see cref="Valid"/> is green
    /// Severity == <see cref="Fishy"/> is yellow
    /// Severity &lt;= <see cref="Invalid"/> is red
    /// </remarks>
    public enum Severity
    {
        /// <summary>
        /// Cannot determine validity; not valid.
        /// </summary>
        Indeterminate = -2,

        /// <summary>
        /// Definitively not valid.
        /// </summary>
        Invalid = -1,

        /// <summary>
        /// Suspicious values, but still valid.
        /// </summary>
        Fishy = 0,

        /// <summary>
        /// Values are valid.
        /// </summary>
        Valid = 1,
    }

    public static partial class Extensions
    {
        /// <summary>
        /// Converts a Check result Severity determination (Valid/Invalid/etc) to the localized string.
        /// </summary>
        /// <param name="s"><see cref="Severity"/> value to convert to string.</param>
        /// <returns>Localized <see cref="string"/>.</returns>
        public static string Description(this Severity s) => s switch
        {
            Severity.Indeterminate => L_SIndeterminate,
            Severity.Invalid => L_SInvalid,
            Severity.Fishy => L_SFishy,
            Severity.Valid => L_SValid,
            _ => L_SNotImplemented
        };
    }
}
