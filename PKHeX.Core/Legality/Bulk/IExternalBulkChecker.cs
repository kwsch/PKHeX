namespace PKHeX.Core.Bulk;

/// <summary>
/// Defines methods for performing bulk legality checks and localizing check results.
/// </summary>
/// <remarks>This interface extends <see cref="IBulkAnalyzer"/> to provide additional functionality for
/// external bulk legality checks, including obtaining localized strings for check results. Implementers should
/// provide a unique identity and a friendly name for each instance.</remarks>
public interface IExternalBulkChecker : IBulkAnalyzer
{
    /// <summary>
    /// Friendly name of the legality check, used for display/internal purposes.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Unique identity of the legality check, used to identify the check in results.
    /// </summary>
    ushort Identity { get; }

    /// <summary>
    /// Requests a localized string for the given check result.
    /// </summary>
    /// <param name="chk">Check result to localize.</param>
    /// <param name="settings">Localization settings and strings that can be used.</param>
    /// <param name="data1">Analysis the check originated from, which may contain additional context.</param>
    /// <returns>Localized string for the check result.</returns>
    string Localize(CheckResult chk, LegalityLocalizationSet settings, LegalityAnalysis data1);

    /// <summary>
    /// Requests a localized string for the given check result.
    /// </summary>
    /// <param name="chk">Check result to localize.</param>
    /// <param name="settings">Localization settings and strings that can be used.</param>
    /// <param name="data1">Analysis the check originated from, which may contain additional context.</param>
    /// <param name="data2">Analysis the check originated from, which may contain additional context.</param>
    /// <returns>Localized string for the check result.</returns>
    string Localize(CheckResult chk, LegalityLocalizationSet settings, LegalityAnalysis data1, LegalityAnalysis data2);
}
