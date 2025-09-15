using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PKHeX.Core;

/// <summary>
/// Formatting logic for <see cref="LegalityAnalysis"/> to create a human-readable <see cref="T:System.String"/>.
/// </summary>
public static class LegalityFormatting
{
    public static ILegalityFormatter Formatter { private get; set; } = new BaseLegalityFormatter();

    /// <summary>
    /// Creates a report message with optional verbosity for in-depth analysis.
    /// </summary>
    /// <param name="la">Legality result to format</param>
    /// <param name="verbose">Include all details in the parse, including valid check messages.</param>
    /// <returns>Single line string</returns>
    public static string Report(this LegalityAnalysis la, bool verbose = false)
    {
        var localizer = LegalityLocalizationContext.Create(la);
        return Report(localizer, verbose);
    }

    /// <inheritdoc cref="Report(LegalityAnalysis, bool)"/>
    public static string Report(this LegalityLocalizationContext localizer, bool verbose) => verbose ? GetVerboseLegalityReport(localizer) : GetLegalityReport(localizer);

    /// <inheritdoc cref="Report(LegalityAnalysis, bool)"/>
    public static string Report(this LegalityAnalysis la, string language, bool verbose = false)
    {
        var localizer = LegalityLocalizationContext.Create(la, language);
        return localizer.Report(verbose);
    }

    public static string GetLegalityReport(LegalityLocalizationContext la) => Formatter.GetReport(la);
    public static string GetVerboseLegalityReport(LegalityLocalizationContext la) => Formatter.GetReportVerbose(la);

    public static void AddSecondaryChecksValid(LegalityLocalizationContext la, IEnumerable<CheckResult> results, List<string> lines)
    {
        var outputLines = results
            .Where(chk => chk.Valid && chk.IsNotGeneric())
            .OrderBy(chk => chk.Judgement); // Fishy sorted to top
        foreach (var chk in outputLines)
            lines.Add(la.Humanize(chk));
    }

    public static void AddSecondaryChecksInvalid(LegalityLocalizationContext la, IReadOnlyList<CheckResult> results, List<string> lines)
    {
        foreach (var chk in results)
        {
            if (chk.Valid)
                continue;
            lines.Add(la.Humanize(chk));
        }
    }

    public static void AddRelearn(LegalityLocalizationContext la, ReadOnlySpan<MoveResult> relearn, List<string> lines, bool state)
    {
        for (int i = 0; i < relearn.Length; i++)
        {
            var move = relearn[i];
            if (move.Valid == state)
                lines.Add(la.FormatRelearn(move, i + 1));
        }
    }

    public static void AddMoves(LegalityLocalizationContext la, ReadOnlySpan<MoveResult> moves, List<string> lines, in byte currentFormat, bool state)
    {
        for (int i = 0; i < moves.Length; i++)
        {
            var move = moves[i];
            if (move.Valid != state)
                continue;
            var msg = la.FormatMove(move, i + 1, currentFormat);
            lines.Add(msg);
        }
    }

    /// <summary>
    /// Adds information about the <see cref="LegalityAnalysis.EncounterMatch"/> to the <see cref="lines"/>.
    /// </summary>
    public static void AddEncounterInfo(LegalityLocalizationContext l, List<string> lines)
    {
        var la = l.Analysis;
        var enc = la.EncounterOriginal;
        var display = l.Settings.Encounter;
        // Name
        lines.Add(string.Format(display.Format, display.EncounterType, enc.GetEncounterName(l.Strings.specieslist)));
        if (enc is MysteryGift g)
            lines.Add(g.CardHeader);

        // Location
        var loc = enc.GetEncounterLocation();
        if (!string.IsNullOrEmpty(loc))
            lines.Add(string.Format(display.Format, display.Location, loc));

        // Version
        if (enc.Generation <= 2)
            lines.Add(string.Format(display.Format, display.Version, enc.Version));

        // PID/IV
        AddEncounterInfoPIDIV(l, lines);
    }

    public static void AddEncounterInfoPIDIV(LegalityLocalizationContext l, List<string> lines)
    {
        var strings = l.Settings;
        var la = l.Analysis;
        var info = la.Info;
        if (!info.PIDParsed)
            info.PIDIV = MethodFinder.Analyze(la.Entity);
        AddEncounterInfoPIDIV(strings, lines, info);
    }

    private static void AddEncounterInfoPIDIV(LegalityLocalizationSet strings, List<string> lines, LegalInfo info)
    {
        var pidiv = info.PIDIV;
        var type = pidiv.Type;
        var msgType = string.Format(strings.Encounter.Format, strings.Encounter.PIDType, type);
        var enc = info.EncounterOriginal;
        if (enc is IRandomCorrelationEvent3 r3 && info.PIDIVMatches)
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
                var line = GetLinePokewalkerSeed(info, strings);
                lines.Add(line);
            }
            else if (enc is PCD pcd)
            {
                var gift = pcd.Gift;
                if (gift is { HasPID: false }) // tick rand
                {
                    var ticks = ARNG.Prev(info.Entity.EncryptionConstant);
                    var line = string.Format(strings.Encounter.Format, strings.Encounter.OriginSeed, ticks.ToString("X8"));
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

                    AppendInitialDateTime4(lines, initial, origin, components, strings.Encounter);
                    if (components.IsInvalid())
                        lines.Add("INVALID");
                }
            }
            else if (enc is EncounterEgg3)
            {
                if (Daycare3.TryGetOriginSeed(info.Entity, out var day3))
                {
                    var line = string.Format(strings.Encounter.Format, strings.Encounter.OriginSeed, day3.Origin.ToString("X8"));
                    lines.Add(line);

                    lines.Add(string.Format(strings.Encounter.FrameInitial, day3.Initial.ToString("X8"), day3.Advances + 1)); // frames are 1-indexed
                    var sb = new StringBuilder();
                    AppendFrameTimeStamp3(day3.Advances, sb, strings.Encounter);
                    lines.Add(string.Format(strings.Encounter.Format, strings.Encounter.Time, sb));
                }
            }
            return;
        }

        if (pidiv.IsSeed64())
        {
            var line = string.Format(strings.Encounter.Format, strings.Encounter.OriginSeed, pidiv.Seed64.ToString("X16"));
            lines.Add(line);
            return;
        }
        if (enc is IEncounterSlot34 s)
        {
            var line = GetLineSlot34(info, strings, pidiv, s);
            lines.Add(line);
        }
        else
        {
            var seed = pidiv.OriginSeed;
            var line = string.Format(strings.Encounter.Format, strings.Encounter.OriginSeed, seed.ToString("X8"));
            if (pidiv.Mutated is not 0 && pidiv.OriginSeed != pidiv.EncounterSeed)
                line += $" [{pidiv.EncounterSeed:X8}]";
            lines.Add(line);
        }
        if (enc is EncounterSlot3 or EncounterStatic3)
            AppendDetailsFrame3(info, lines, strings.Encounter);
        else if (enc is EncounterSlot4 or EncounterStatic4)
            AppendDetailsDate4(info, lines, strings.Encounter);
    }

    private static string GetLinePokewalkerSeed(LegalInfo info, LegalityLocalizationSet strings)
    {
        var pk = info.Entity;
        var result = PokewalkerRNG.GetLeastEffortSeed((uint)pk.IV_HP, (uint)pk.IV_ATK, (uint)pk.IV_DEF, (uint)pk.IV_SPA, (uint)pk.IV_SPD, (uint)pk.IV_SPE);
        var line = string.Format(strings.Encounter.Format, strings.Encounter.OriginSeed, result.Seed.ToString("X8"));
        line += $" [{result.Type} @ {result.PriorPoke}]";
        return line;
    }

    private static string GetLineSlot34(LegalInfo info, LegalityLocalizationSet strings, PIDIV pidiv, IEncounterSlot34 s)
    {
        var lead = pidiv.Lead;
        var seed = !info.FrameMatches || lead == LeadRequired.Invalid ? pidiv.OriginSeed : pidiv.EncounterSeed;
        var line = string.Format(strings.Encounter.Format, strings.Encounter.OriginSeed, seed.ToString("X8"));
        if (lead != LeadRequired.None)
        {
            if (lead is LeadRequired.Static)
                line += $" [{s.StaticIndex}/{s.StaticCount}]";
            else if (lead is LeadRequired.MagnetPull)
                line += $" [{s.MagnetPullIndex}/{s.MagnetPullCount}]";
            else
                line += $" [{s.SlotNumber}]";

            line += $" ({lead.Localize(strings.Lines)})";
        }
        else
        {
            line += $" [{s.SlotNumber}]";
        }

        return line;
    }

    private static void AppendDetailsDate4(LegalInfo info, List<string> lines, EncounterDisplayLocalization loc)
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
        AppendInitialDateTime4(lines, initialSeed, seed, date, loc);
    }

    private static void AppendInitialDateTime4(List<string> lines, uint initialSeed, uint origin, DateOnly date, EncounterDisplayLocalization loc)
    {
        var decompose = ClassicEraRNG.DecomposeSeed(initialSeed, (uint)date.Year, (uint)date.Month, (uint)date.Day);
        AppendInitialDateTime4(lines, initialSeed, origin, decompose, loc);
    }

    private static void AppendInitialDateTime4(List<string> lines, uint initialSeed, uint origin, InitialSeedComponents4 decompose, EncounterDisplayLocalization loc)
    {
        var advances = LCRNG.GetDistance(initialSeed, origin);
        lines.Add($"{decompose.Year+2000:0000}-{decompose.Month:00}-{decompose.Day:00} @ {decompose.Hour:00}:{decompose.Minute:00}:{decompose.Second:00} - {decompose.Delay}");
        lines.Add(string.Format(loc.FrameInitial, initialSeed.ToString("X8"), advances + 1)); // frames are 1-indexed
    }

    private static void AppendDetailsFrame3(LegalInfo info, List<string> lines, EncounterDisplayLocalization loc)
    {
        var pidiv = info.PIDIV;
        var pk = info.Entity;
        var enc = info.EncounterOriginal;
        var seed = enc is EncounterSlot3 && info.FrameMatches ? pidiv.EncounterSeed : pidiv.OriginSeed;
        var (initialSeed, advances) = GetInitialSeed3(seed, pk.Version);
        lines.Add(string.Format(loc.FrameInitial, initialSeed.ToString("X8"), advances + 1)); // frames are 1-indexed

        var sb = new StringBuilder();
        AppendFrameTimeStamp3(advances, sb, loc);
        lines.Add(string.Format(loc.Format, loc.Time, sb));

        // Try appending the TID frame if it originates from Emerald.
        if (pk.Version is not GameVersion.E)
            return;
        // don't bother ignoring postgame-only. E4 resets the seed, but it's annoying to check.
        var tidSeed = pk.TID16; // start of game
        var tidAdvances = LCRNG.GetDistance(tidSeed, seed);
        if (tidAdvances >= advances)
            return; // only show if it makes sense to
        lines.Add(string.Format(loc.FrameNewGame, tidSeed.ToString("X8"), tidAdvances + 1)); // frames are 1-indexed
        sb.Clear();
        AppendFrameTimeStamp3(tidAdvances, sb, loc);
        lines.Add(string.Format(loc.Format, loc.Time, sb));
    }

    /// <summary>
    /// Converts a generation 3 RNG advancement frame to a timestamp.
    /// </summary>
    /// <param name="frame">Frames elapsed since the initial seed.</param>
    /// <param name="sb">StringBuilder to append the timestamp to.</param>
    /// <param name="loc">Localization strings for formatting.</param>
    private static void AppendFrameTimeStamp3(uint frame, StringBuilder sb, EncounterDisplayLocalization loc)
    {
        var time = TimeSpan.FromSeconds((double)frame / 60);
        if (time.TotalHours >= 1)
            sb.Append($"{(int)time.TotalHours}:");
        if (time.TotalMinutes >= 1 || sb.Length != 0)
            sb.Append($"{time.Minutes:00}:");
        sb.Append($"{time.Seconds:00}.");
        sb.Append($"{time.Milliseconds / 10:00}");

        if (time.TotalDays >= 1)
            sb.AppendFormat(loc.SuffixDays, (int)time.TotalDays);
    }

    private static (uint Seed, uint Advances) GetInitialSeed3(uint seed, GameVersion version)
    {
        // Emerald is always initial seed of 0 for startups other than E4/New Game.
        if (version is GameVersion.E)
            return (0, LCRNG.GetDistance(0, seed));

        var nearest16 = seed;
        uint ctr = 0;
        while (nearest16 > ushort.MaxValue || ctr < (6 * 60)) // minimum 6 seconds to boot->encounter?
        {
            nearest16 = LCRNG.Prev(nearest16);
            ctr++;
        }
        if (version is GameVersion.R or GameVersion.S)
        {
            const uint drySeed = 0x05A0;
            var advances = LCRNG.GetDistance(drySeed, seed);
            if (advances < ushort.MaxValue << 2)
                return (drySeed, advances);
        }
        return (nearest16, ctr);
    }

    private static string Localize(this LeadRequired lead, LegalityCheckLocalization localization)
    {
        if (lead is LeadRequired.Invalid)
            return "❌";
        var (ability, isFail, condition) = lead.GetDisplayAbility();
        var abilities = GameInfo.Strings.Ability;
        var name = abilities[(int)ability];
        var result = isFail ? string.Format(localization.F0_1, name, "❌") : name;
        if (condition != EncounterTriggerCondition.None)
            result += $"-{condition}";
        return result;
    }

    public static string GetEncounterName(this IEncounterable enc, ReadOnlySpan<string> speciesNames)
    {
        // Shouldn't ever be out of range, but just in case.
        var species = enc.Species;
        var name = (uint)species < speciesNames.Length ? speciesNames[species] : species.ToString();
        return $"{enc.LongName} ({name})";
    }

    public static string? GetEncounterLocation(this IEncounterTemplate enc)
    {
        if (enc is not ILocation loc)
            return null;
        return loc.GetEncounterLocation(enc.Generation, enc.Version);
    }
}
