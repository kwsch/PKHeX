using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static PKHeX.Core.LegalityCheckStrings;

namespace PKHeX.Core;

/// <summary>
/// Formatting logic for <see cref="LegalityAnalysis"/> to create a human-readable <see cref="T:System.String"/>.
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
            lines.Add(string.Format(L_F0_1, L_XLocation, loc));

        // Version
        if (enc.Generation <= 2)
            lines.Add(string.Format(L_F0_1, nameof(GameVersion), enc.Version));

        // PID/IV
        AddEncounterInfoPIDIV(la, lines);
    }

    public static void AddEncounterInfoPIDIV(LegalityAnalysis la, List<string> lines)
    {
        var info = la.Info;
        if (!info.PIDParsed)
            info.PIDIV = MethodFinder.Analyze(la.Entity);
        AddEncounterInfoPIDIV(lines, info);
    }

    private static void AddEncounterInfoPIDIV(List<string> lines, LegalInfo info)
    {
        var pidiv = info.PIDIV;
        var type = pidiv.Type;
        var msgType = string.Format(L_FPIDType_0, type);
        var enc = info.EncounterOriginal;
        if (enc is IRandomCorrelationEvent3 r3)
        {
            if (type is not PIDType.None)
            {
                var mainType = r3.GetSuggestedCorrelation();
                if (mainType != type)
                    msgType += $" [{mainType}]";
            }
            if (enc is EncounterGift3 { Method: PIDType.BACD_M } && info.PIDIVMatches) // mystry
            {
                var detail = MystryMew.GetSeedIndexes(pidiv.OriginSeed);
                msgType += $" ({detail.Index}-{detail.SubIndex})";
            }
        }
        lines.Add(msgType);
        if (pidiv.NoSeed)
        {
            if (enc is EncounterStatic4Pokewalker)
            {
                if (type is not PIDType.Pokewalker)
                    return;
                var line = GetLinePokewalkerSeed(info);
                lines.Add(line);
            }
            else if (enc is PCD pcd)
            {
                var gift = pcd.Gift;
                if (gift is { HasPID: false }) // tick rand
                {
                    var ticks = ARNG.Prev(info.Entity.EncryptionConstant);
                    var line = string.Format(L_FOriginSeed_0, ticks.ToString("X8"));
                    line += $" [{ticks / 524_288f:F2}]"; // seconds?
                    lines.Add(line);
                }
                if (gift is { HasIVs: false })
                {
                    var pk = info.Entity;
                    Span<int> ivs = stackalloc int[6];
                    pk.GetIVs(ivs);

                    var date = pk.MetDate ?? new DateOnly(2000, 1, 1);
                    var initial = ClassicEraRNG.SeekInitialSeedForIVs(ivs, (uint)date.Year, (uint)date.Month, (uint)date.Day, out var origin);
                    var components = ClassicEraRNG.DecomposeSeed(initial, (uint)date.Year, (uint)date.Month, (uint)date.Day);
                    
                    AppendInitialDateTime(lines, initial, origin, components);
                    if (components.IsInvalid())
                        lines.Add("INVALID");
                }
            }
            else if (enc is EncounterEgg3)
            {
                if (Daycare3.TryGetOriginSeed(info.Entity, out var day3))
                {
                    var line = string.Format(L_FOriginSeed_0, day3.Origin.ToString("X8"));
                    lines.Add(line);

                    lines.Add($"Initial: 0x{day3.Initial:X8}, Frame: {day3.Advances + 1}"); // frames are 1-indexed
                    var sb = new StringBuilder();
                    AppendFrameTimeStamp(day3.Advances, sb);
                    lines.Add($"Time: {sb}");
                }
            }
            return;
        }

        if (pidiv.IsSeed64())
        {
            var line = string.Format(L_FOriginSeed_0, pidiv.Seed64.ToString("X16"));
            lines.Add(line);
            return;
        }
        if (enc is IEncounterSlot34 s)
        {
            var line = GetLineSlot34(info, pidiv, s);
            lines.Add(line);
        }
        else
        {
            var seed = pidiv.OriginSeed;
            var line = string.Format(L_FOriginSeed_0, seed.ToString("X8"));
            if (pidiv.Mutated is not 0 && pidiv.OriginSeed != pidiv.EncounterSeed)
                line += $" [{pidiv.EncounterSeed:X8}]";
            lines.Add(line);
        }
        if (enc is EncounterSlot3 or EncounterStatic3)
            AppendDetailsFrame3(info, lines);
        else if (enc is EncounterSlot4 or EncounterStatic4)
            AppendDetailsDate4(info, lines);
    }

    private static string GetLinePokewalkerSeed(LegalInfo info)
    {
        var pk = info.Entity;
        var result = PokewalkerRNG.GetLeastEffortSeed((uint)pk.IV_HP, (uint)pk.IV_ATK, (uint)pk.IV_DEF, (uint)pk.IV_SPA, (uint)pk.IV_SPD, (uint)pk.IV_SPE);
        var line = string.Format(L_FOriginSeed_0, result.Seed.ToString("X8"));
        line += $" [{result.Type} @ {result.PriorPoke}]";
        return line;
    }

    private static string GetLineSlot34(LegalInfo info, PIDIV pidiv, IEncounterSlot34 s)
    {
        var lead = pidiv.Lead;
        var seed = !info.FrameMatches || lead == LeadRequired.Invalid ? pidiv.OriginSeed : pidiv.EncounterSeed;
        var line = string.Format(L_FOriginSeed_0, seed.ToString("X8"));
        if (lead != LeadRequired.None)
        {
            if (lead is LeadRequired.Static)
                line += $" [{s.StaticIndex}/{s.StaticCount}]";
            else if (lead is LeadRequired.MagnetPull)
                line += $" [{s.MagnetPullIndex}/{s.MagnetPullCount}]";
            else
                line += $" [{s.SlotNumber}]";

            line += $" ({lead.Localize()})";
        }
        else
        {
            line += $" [{s.SlotNumber}]";
        }

        return line;
    }

    private static void AppendDetailsDate4(LegalInfo info, List<string> lines)
    {
        var pidiv = info.PIDIV;
        if (pidiv.Type is not (PIDType.Method_1 or PIDType.ChainShiny))
            return;

        // Try to determine date/time
        var enc = info.EncounterOriginal;
        var seed = enc is EncounterSlot4 && info.FrameMatches ? pidiv.EncounterSeed : pidiv.OriginSeed;

        // Assume the met date is the same as the encounter date.
        var entity = info.Entity;
        var date = entity.MetDate ?? new DateOnly(2000, 1, 1);
        var initialSeed = ClassicEraRNG.SeekInitialSeed((uint)date.Year, (uint)date.Month, (uint)date.Day, seed);
        AppendInitialDateTime(lines, initialSeed, seed, date);
    }

    private static void AppendInitialDateTime(List<string> lines, uint initialSeed, uint origin, DateOnly date)
    {
        var decompose = ClassicEraRNG.DecomposeSeed(initialSeed, (uint)date.Year, (uint)date.Month, (uint)date.Day);
        AppendInitialDateTime(lines, initialSeed, origin, decompose);
    }

    private static void AppendInitialDateTime(List<string> lines, uint initialSeed, uint origin, InitialSeedComponents4 decompose)
    {
        var advances = LCRNG.GetDistance(initialSeed, origin);
        lines.Add($"{decompose.Year+2000:0000}-{decompose.Month:00}-{decompose.Day:00} @ {decompose.Hour:00}:{decompose.Minute:00}:{decompose.Second:00} - {decompose.Delay}");
        lines.Add($"Initial: 0x{initialSeed:X8}, Frame: {advances + 1}"); // frames are 1-indexed
    }

    private static void AppendDetailsFrame3(LegalInfo info, List<string> lines)
    {
        var pidiv = info.PIDIV;
        var pk = info.Entity;
        var enc = info.EncounterOriginal;
        var seed = enc is EncounterSlot3 && info.FrameMatches ? pidiv.EncounterSeed : pidiv.OriginSeed;
        var (initialSeed, advances) = GetInitialSeed3(seed, pk.Version);
        lines.Add($"Initial: 0x{initialSeed:X8}, Frame: {advances + 1}"); // frames are 1-indexed

        var sb = new StringBuilder();
        AppendFrameTimeStamp(advances, sb);
        lines.Add($"Time: {sb}");

        // Try appending the TID frame if it originates from Emerald.
        if (pk.Version is not GameVersion.E)
            return;
        // don't bother ignoring postgame-only. E4 resets the seed, but it's annoying to check.
        var tidSeed = pk.TID16; // start of game
        var tidAdvances = LCRNG.GetDistance(tidSeed, seed);
        if (tidAdvances >= advances)
            return; // only show if it makes sense to
        lines.Add($"New Game: 0x{tidSeed:X8}, Frame: {tidAdvances + 1}"); // frames are 1-indexed
        sb.Clear();
        AppendFrameTimeStamp(tidAdvances, sb);
        lines.Add($"Time: {sb}");
    }

    private static void AppendFrameTimeStamp(uint frame, StringBuilder sb)
    {
        var time = TimeSpan.FromSeconds((double)frame / 60);
        if (time.TotalHours >= 1)
            sb.Append($"{(int)time.TotalHours}:");
        if (time.TotalMinutes >= 1 || sb.Length != 0)
            sb.Append($"{time.Minutes:00}:");
        sb.Append($"{time.Seconds:00}.");
        sb.Append($"{time.Milliseconds / 10:00}");

        if (time.TotalDays >= 1)
            sb.Append($" (days: {(int)time.TotalDays})");
    }

    private static (uint Seed, uint Advances) GetInitialSeed3(uint seed, GameVersion game)
    {
        if (game is GameVersion.E) // Always 0 seed.
            return (0, LCRNG.GetDistance(0, seed));

        var nearest16 = seed;
        uint ctr = 0;
        while (nearest16 > ushort.MaxValue || ctr < 360) // 6 seconds to boot->encounter?
        {
            nearest16 = LCRNG.Prev(nearest16);
            ctr++;
        }
        if (game is GameVersion.R or GameVersion.S)
        {
            const uint drySeed = 0x05A0;
            var advances = LCRNG.GetDistance(drySeed, seed);
            if (advances < ushort.MaxValue << 2)
                return (drySeed, advances);
        }
        return (nearest16, ctr);
    }

    private static string Localize(this LeadRequired lead)
    {
        if (lead is LeadRequired.Invalid)
            return "❌";
        var (ability, isFail, condition) = lead.GetDisplayAbility();
        var abilities = GameInfo.Strings.Ability;
        var name = abilities[(int)ability];
        var result = isFail ? string.Format(L_F0_1, name, "❌") : name;
        if (condition != EncounterTriggerCondition.None)
            result += $"-{condition}";
        return result;
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
        return loc.GetEncounterLocation(enc.Generation, enc.Version);
    }
}
