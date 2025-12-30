using System;
using System.Collections.Generic;

namespace PKHeX.Core;

/// <summary>
/// Default formatter for Legality Result displays.
/// </summary>
public sealed class BaseLegalityFormatter : ILegalityFormatter
{
    /// <summary>
    /// Gets a minimal report string for the analysis.
    /// </summary>
    public string GetReport(in LegalityLocalizationContext la)
    {
        var l = la.Analysis;
        if (l.Valid)
            return la.Settings.Lines.Legal;
        if (!l.Parsed)
            return la.Settings.Lines.AnalysisUnavailable;

        List<string> lines = [];
        GetLegalityReportLines(la, lines);
        return string.Join(Environment.NewLine, lines);
    }

    /// <summary>
    /// Gets a verbose report string for the analysis.
    /// </summary>
    public string GetReportVerbose(in LegalityLocalizationContext la)
    {
        var l = la.Analysis;
        if (!l.Parsed)
            return la.Settings.Lines.AnalysisUnavailable;

        var lines = GetVerboseLegalityReportLines(la);
        return string.Join(Environment.NewLine, lines);
    }

    private static void GetLegalityReportLines(in LegalityLocalizationContext la, List<string> lines)
    {
        var l = la.Analysis;
        var info = l.Info;
        var pk = l.Entity;

        LegalityFormatting.AddMoves(la, info.Moves, lines, pk.Format, false);
        if (pk.Format >= 6)
            LegalityFormatting.AddRelearn(la, info.Relearn, lines, false);
        LegalityFormatting.AddSecondaryChecksInvalid(la, l.Results, lines);
    }

    private static List<string> GetVerboseLegalityReportLines(in LegalityLocalizationContext la)
    {
        var l = la.Analysis;
        var lines = new List<string>();
        if (l.Valid)
        {
            lines.Add(la.Settings.Lines.Legal);
            lines.Add(string.Empty);
        }
        else
        {
            GetLegalityReportLines(la, lines);
        }
        var info = l.Info;
        var pk = l.Entity;
        int initialCount = lines.Count;

        var format = pk.Format;
        LegalityFormatting.AddMoves(la, info.Moves, lines, format, true);

        if (format >= 6)
            LegalityFormatting.AddRelearn(la, info.Relearn, lines, true);

        if (lines.Count != initialCount) // move info added, break for next section
            lines.Add(string.Empty);

        LegalityFormatting.AddSecondaryChecksValid(la, l.Results, lines);

        lines.Add(string.Empty);
        LegalityFormatting.AddEncounterInfo(la, lines);

        return lines;
    }
}
