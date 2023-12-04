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
    public string GetReport(LegalityAnalysis l)
    {
        if (l.Valid)
            return L_ALegal;
        if (!l.Parsed)
            return L_AnalysisUnavailable;

        List<string> lines = [];
        GetLegalityReportLines(l, lines);
        return string.Join(Environment.NewLine, lines);
    }

    /// <summary>
    /// Gets a verbose report string for the analysis.
    /// </summary>
    public string GetReportVerbose(LegalityAnalysis l)
    {
        if (!l.Parsed)
            return L_AnalysisUnavailable;

        var lines = GetVerboseLegalityReportLines(l);
        return string.Join(Environment.NewLine, lines);
    }

    private static void GetLegalityReportLines(LegalityAnalysis l, List<string> lines)
    {
        var info = l.Info;
        var pk = l.Entity;

        var evos = info.EvoChainsAllGens;
        LegalityFormatting.AddMoves(info.Moves, lines, pk.Format, false, pk, evos);
        if (pk.Format >= 6)
            LegalityFormatting.AddRelearn(info.Relearn, lines, false, pk, evos);
        LegalityFormatting.AddSecondaryChecksInvalid(l.Results, lines);
    }

    private static List<string> GetVerboseLegalityReportLines(LegalityAnalysis l)
    {
        var lines = new List<string>();
        if (l.Valid)
            lines.Add(L_ALegal);
        else
            GetLegalityReportLines(l, lines);
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

        LegalityFormatting.AddSecondaryChecksValid(l.Results, lines);

        lines.Add(separator);
        lines.Add(string.Empty);
        LegalityFormatting.AddEncounterInfo(l, lines);

        return lines;
    }
}
