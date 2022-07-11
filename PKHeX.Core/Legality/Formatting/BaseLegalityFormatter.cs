using System;
using System.Collections.Generic;
using static PKHeX.Core.LegalityCheckStrings;

namespace PKHeX.Core;

/// <summary>
/// Default formatter for Legality Result displays.
/// </summary>
public sealed class BaseLegalityFormatter : ILegalityFormatter
{
    public string GetReport(LegalityAnalysis l)
    {
        if (l.Valid)
            return L_ALegal;
        if (!l.Parsed)
            return L_AnalysisUnavailable;

        var lines = GetLegalityReportLines(l);
        return string.Join(Environment.NewLine, lines);
    }

    public string GetReportVerbose(LegalityAnalysis l)
    {
        if (!l.Parsed)
            return L_AnalysisUnavailable;

        var lines = GetVerboseLegalityReportLines(l);
        return string.Join(Environment.NewLine, lines);
    }

    private static List<string> GetLegalityReportLines(LegalityAnalysis l)
    {
        var lines = new List<string>();
        var info = l.Info;
        var pk = l.Entity;

        LegalityFormatting.AddMoves(info.Moves, lines, pk.Format, false, pk, l.Info.EvoChainsAllGens);
        if (pk.Format >= 6)
            LegalityFormatting.AddRelearn(info.Relearn, lines, false, pk, l.Info.EvoChainsAllGens);
        LegalityFormatting.AddSecondaryChecksInvalid(l.Results, lines);
        return lines;
    }

    private static IReadOnlyList<string> GetVerboseLegalityReportLines(LegalityAnalysis l)
    {
        var lines = l.Valid ? new List<string> {L_ALegal} : GetLegalityReportLines(l);
        var info = l.Info;
        var pk = l.Entity;
        const string separator = "===";
        lines.Add(separator);
        lines.Add(string.Empty);
        int initialCount = lines.Count;

        var format = pk.Format;
        LegalityFormatting.AddMoves(info.Moves, lines, format, true, pk, l.Info.EvoChainsAllGens);

        if (format >= 6)
            LegalityFormatting.AddRelearn(info.Relearn, lines, true, pk, l.Info.EvoChainsAllGens);

        if (lines.Count != initialCount) // move info added, break for next section
            lines.Add(string.Empty);

        LegalityFormatting.AddSecondaryChecksValid(l.Results, lines);

        lines.Add(separator);
        lines.Add(string.Empty);
        LegalityFormatting.AddEncounterInfo(l, lines);

        return lines;
    }
}
