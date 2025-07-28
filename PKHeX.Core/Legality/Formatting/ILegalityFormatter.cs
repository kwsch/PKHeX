namespace PKHeX.Core;

/// <summary>
/// Formats legality results into a <see cref="T:System.String"/> for display.
/// </summary>
public interface ILegalityFormatter
{
    /// <summary>
    /// Gets a small summary of the legality analysis.
    /// </summary>
    string GetReport(in LegalityLocalizationContext l);

    /// <summary>
    /// Gets a verbose summary of the legality analysis.
    /// </summary>
    string GetReportVerbose(in LegalityLocalizationContext l);
}
