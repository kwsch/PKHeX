using System;
using System.Collections.Generic;
using System.Linq;
using static PKHeX.Core.LegalityCheckStrings;

namespace PKHeX.Core;

/// <summary>
/// Formatting logic for <see cref="LegalityAnalysis"/> to create a human readable <see cref="T:System.String"/>.
/// </summary>
public static class LegalityFormatting
{
    /// <summary>
    /// Creates a report message with optional verbosity for in-depth analysis.
    /// </summary>
    /// <param name="la">Legality result to format</param>
    /// <param name="verbose">Include all details in the parse, including valid check messages.</param>
    /// <returns>Single line string</returns>
    public static string Report(this LegalityAnalysis la, bool verbose = false) => verbose ? GetVerboseLegalityReport(la) : GetLegalityReport(la);

    public static ILegalityFormatter Formatter { private get; set; } = new BaseLegalityFormatter();

    public static string GetLegalityReport(LegalityAnalysis la) => Formatter.GetReport(la);
    public static string GetVerboseLegalityReport(LegalityAnalysis la) => Formatter.GetReportVerbose(la);

    public static void AddSecondaryChecksValid(IEnumerable<CheckResult> results, List<string> lines)
    {
        var outputLines = results
            .Where(chk => chk.Valid && chk.Comment != L_AValid)
            .OrderBy(chk => chk.Judgement) // Fishy sorted to top
            .Select(chk => chk.Format(L_F0_1));
        lines.AddRange(outputLines);
    }

    public static void AddSecondaryChecksInvalid(IReadOnlyList<CheckResult> results, List<string> lines)
    {
        foreach (var chk in results)
        {
            if (chk.Valid)
                continue;
            lines.Add(chk.Format(L_F0_1));
        }
    }

    public static void AddRelearn(ReadOnlySpan<MoveResult> relearn, List<string> lines, bool state, PKM pk, EvolutionHistory history)
    {
        for (int i = 0; i < relearn.Length; i++)
        {
            var move = relearn[i];
            if (move.Valid == state)
                lines.Add(move.Format(L_F0_RM_1_2, i + 1, pk, history));
        }
    }

    public static void AddMoves(ReadOnlySpan<MoveResult> moves, List<string> lines, in int currentFormat, bool state, PKM pk, EvolutionHistory history)
    {
        for (int i = 0; i < moves.Length; i++)
        {
            var move = moves[i];
            if (move.Valid != state)
                continue;
            var msg = move.Format(L_F0_M_1_2, i + 1, pk, history);
            var gen = move.Generation;
            if (currentFormat != gen && gen != 0)
                msg += $" [Gen{gen}]";
            lines.Add(msg);
        }
    }

    /// <summary>
    /// Adds information about the <see cref="LegalityAnalysis.EncounterMatch"/> to the <see cref="lines"/>.
    /// </summary>
    public static void AddEncounterInfo(LegalityAnalysis la, List<string> lines)
    {
        var enc = la.EncounterOriginal;

        // Name
        lines.Add(string.Format(L_FEncounterType_0, enc.GetEncounterName()));
        if (enc is MysteryGift g)
            lines.Add(g.CardHeader);

        // Location
        var loc = enc.GetEncounterLocation();
        if (!string.IsNullOrEmpty(loc))
            lines.Add(string.Format(L_F0_1, "Location", loc));

        // Version
        if (enc.Generation <= 2)
            lines.Add(string.Format(L_F0_1, nameof(GameVersion), enc.Version));

        // PIDIV
        AddEncounterInfoPIDIV(la, lines);
    }

    public static void AddEncounterInfoPIDIV(LegalityAnalysis la, List<string> lines)
    {
        var info = la.Info;
        if (!info.PIDParsed)
            info.PIDIV = MethodFinder.Analyze(la.Entity);
        AddEncounterInfoPIDIV(lines, info.PIDIV);
    }

    private static void AddEncounterInfoPIDIV(List<string> lines, PIDIV pidiv)
    {
        lines.Add(string.Format(L_FPIDType_0, pidiv.Type));
        if (!pidiv.NoSeed)
            lines.Add(string.Format(L_FOriginSeed_0, pidiv.OriginSeed.ToString("X8")));
    }

    public static string GetEncounterName(this IEncounterable enc)
    {
        var str = ParseSettings.SpeciesStrings;
        var name = (uint) enc.Species < str.Count ? str[enc.Species] : enc.Species.ToString();
        return $"{enc.LongName} ({name})";
    }

    public static string? GetEncounterLocation(this IEncounterTemplate enc)
    {
        if (enc is not ILocation loc)
            return null;
        return loc.GetEncounterLocation(enc.Generation, (int)enc.Version);
    }
}
