using System;
using System.Collections.Generic;
using static PKHeX.Core.LegalityCheckStrings;

namespace PKHeX.Core;

/// <summary>
/// Default formatter for Legality Result displays.
/// </summary>
public sealed class BaseLegalityFormatter : ILegalityFormatter
{
    /// <summary>
    /// Gets a minimal report string for the analysis.
    /// </summary>
    public string GetReport(LegalityLocalizationContext la)
    {
        var l = la.Analysis;
        if (l.Valid)
            return L_ALegal;
        if (!l.Parsed)
            return L_AnalysisUnavailable;

        List<string> lines = [];
        GetLegalityReportLines(la, lines);
        return string.Join(Environment.NewLine, lines);
    }

    /// <summary>
    /// Gets a verbose report string for the analysis.
    /// </summary>
    public string GetReportVerbose(LegalityLocalizationContext la)
    {
        var l = la.Analysis;
        if (!l.Parsed)
            return L_AnalysisUnavailable;

        var lines = GetVerboseLegalityReportLines(la);
        return string.Join(Environment.NewLine, lines);
    }

    private static void GetLegalityReportLines(LegalityLocalizationContext la, List<string> lines)
    {
        var l = la.Analysis;
        var info = l.Info;
        var pk = l.Entity;

        var evos = info.EvoChainsAllGens;
        LegalityFormatting.AddMoves(info.Moves, lines, pk.Format, false, pk, evos);
        if (pk.Format >= 6)
            LegalityFormatting.AddRelearn(info.Relearn, lines, false, pk, evos);
        LegalityFormatting.AddSecondaryChecksInvalid(la, l.Results, lines);
    }

    private static List<string> GetVerboseLegalityReportLines(LegalityLocalizationContext la)
    {
        var l = la.Analysis;
        var lines = new List<string>();
        if (l.Valid)
            lines.Add(L_ALegal);
        else
            GetLegalityReportLines(la, lines);
        var info = l.Info;
        var pk = l.Entity;
        const string separator = "===";
        lines.Add(separator);
        lines.Add(string.Empty);
        int initialCount = lines.Count;

        var format = pk.Format;
        var evos = info.EvoChainsAllGens;
        LegalityFormatting.AddMoves(info.Moves, lines, format, true, pk, evos);

        if (format >= 6)
            LegalityFormatting.AddRelearn(info.Relearn, lines, true, pk, evos);

        if (lines.Count != initialCount) // move info added, break for next section
            lines.Add(string.Empty);

        LegalityFormatting.AddSecondaryChecksValid(la, l.Results, lines);

        lines.Add(separator);
        lines.Add(string.Empty);
        LegalityFormatting.AddEncounterInfo(la, lines);

        return lines;
    }
}
