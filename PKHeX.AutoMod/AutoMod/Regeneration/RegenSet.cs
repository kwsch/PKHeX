using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace PKHeX.Core.AutoMod;

public sealed class RegenSet
{
    public static readonly RegenSet Default = new([], Latest.Generation);

    public RegenSetting Extra { get; }
    public ITrainerInfo? Trainer { get; }
    public StringInstructionSet Batch { get; }
    public IReadOnlyList<StringInstruction> EncounterFilters { get; }
    public IReadOnlyList<StringInstruction> VersionFilters { get; }
    public IReadOnlyList<string> SeedFilters { get; }

    public readonly bool HasExtraSettings;
    public readonly bool HasTrainerSettings;
    public bool HasBatchSettings => Batch.Filters.Count != 0 || Batch.Instructions.Count != 0;

    public RegenSet(PKM pk) : this([], pk.Format)
    {
        Extra.Ball = (Ball)pk.Ball;
        Extra.ShinyType = pk.ShinyXor == 0 ? Shiny.AlwaysSquare : pk.IsShiny ? Shiny.AlwaysStar : Shiny.Never;
        if (pk is IAlphaReadOnly { IsAlpha: true })
            Extra.Alpha = true;
        HasExtraSettings = true;
        var tr = new SimpleTrainerInfo(pk.Version) { OT = pk.OriginalTrainerName, TID16 = pk.TID16, SID16 = pk.SID16, Gender = pk.OriginalTrainerGender };
        Trainer = tr;
        HasTrainerSettings = true;
        _ = StringInstruction.TryParseFilter($"=Version={pk.Version}", out var verFilter);
        VersionFilters = [verFilter!];

        List<string> modified = [];
        var ribbons = RibbonInfo.GetRibbonInfo(pk);
        foreach (var rib in ribbons)
        {
            if (rib.HasRibbon)
                modified.Add($".{rib.Name}=true");
        }

        modified.Add($".MetLocation={pk.MetLocation}");
        modified.Add($".MetDate={pk.MetDate:yyyyMMdd}");
        modified.Add($".MetLevel={pk.MetLevel}");
        if (pk is IFormArgument { FormArgument: not 0 } fa)
            modified.Add($".FormArgument={fa.FormArgument}");
        if (modified.Count > 0)
            Batch = new StringInstructionSet(modified.ToArray().AsSpan());
    }

    public RegenSet(IList<BattleTemplateParseError> lines, byte format, Shiny shiny = Shiny.Never)
    {
        Extra = new RegenSetting { ShinyType = shiny };
        HasExtraSettings = Extra.SetRegenSettings(lines);
        HasTrainerSettings = RegenUtil.GetTrainerInfo(lines, format, out var tr);
        Trainer = tr;

        if (lines.Count == 0)
        {
            Batch = new StringInstructionSet(Array.Empty<string>());
            EncounterFilters = [];
            VersionFilters = [];
            SeedFilters = [];
            return;
        }

        List<StringInstruction> eFilter = [];
        List<StringInstruction> vFilter = [];
        List<StringInstruction> mods = [];
        List<string> sFilter = [];
        for (int i = 0; i < lines.Count;)
        {
            var line = lines[i];
            if (line.Type == BattleTemplateParseErrorType.LineLength && (line.Value is null || line.Value.Length == 0))
            {
                lines.RemoveAt(i);
                continue;
            }
            if (line.Type == BattleTemplateParseErrorType.TokenUnknown)
            {
                var sanitized = line.Value.Replace(">=", "≥").Replace("<=", "≤");
                if (StringInstruction.TryParseInstruction(sanitized, out var mod))
                {
                    mods.Add(mod);
                    lines.RemoveAt(i);
                    continue;
                }

                if (RegenUtil.IsEncounterFilter(sanitized, out var e))
                {
                    eFilter.Add(e);
                    lines.RemoveAt(i);
                    continue;
                }
                if (RegenUtil.IsVersionFilter(sanitized, out var v))
                {
                    vFilter.Add(v);
                    lines.RemoveAt(i);
                    continue;
                }
                if (RegenUtil.IsSeedFilter(sanitized, out var s))
                {
                    sFilter.Add(s);
                    lines.RemoveAt(i);
                    continue;
                }
            }
            i++;
        }
        Batch = new([], mods);
        EncounterFilters = eFilter;
        VersionFilters = vFilter;
        SeedFilters = sFilter;
    }

    public string GetSummary()
    {
        var sb = new StringBuilder();
        if (HasExtraSettings)
            sb.AppendLine(RegenUtil.GetSummary(Extra));

        if (HasTrainerSettings && Trainer != null)
            sb.Append(RegenUtil.GetSummary(Trainer));

        if (HasBatchSettings)
            sb.AppendLine(RegenUtil.GetSummary(Batch));

        if (EncounterFilters.Any())
            sb.AppendLine(RegenUtil.GetSummary(EncounterFilters));

        if (VersionFilters.Any())
            sb.AppendLine(RegenUtil.GetSummary(VersionFilters));

        if (SeedFilters.Any())
            sb.AppendLine(SeedFilters[0]);

        return sb.ToString();
    }

    public bool AnyInstructionStartsWith(string name, string value)
    {
        foreach (var z in Batch.Instructions)
        {
            if (!z.PropertyName.StartsWith(name))
                continue;
            if (!z.PropertyValue.StartsWith(value))
                continue;
            return true;
        }
        return false;
    }

    public bool TryGetBatchValue(ReadOnlySpan<char> key, [NotNullWhen(true)] out string? value)
    {
        foreach (var instruction in Batch.Instructions)
        {
            if (!key.SequenceEqual(instruction.PropertyName))
                continue;

            value = instruction.PropertyValue;
            return true;
        }
        value = null;
        return false;
    }
}
