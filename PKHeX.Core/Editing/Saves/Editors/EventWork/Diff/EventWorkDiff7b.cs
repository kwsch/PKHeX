using System.Collections.Generic;
using System.Linq;
using static PKHeX.Core.EventWorkUtil;
using static PKHeX.Core.EventWorkDiffCompatibility;
using static PKHeX.Core.EventWorkDiffCompatibilityExtensions;

namespace PKHeX.Core;

public sealed class EventWorkDiff7b : IEventWorkDiff
{
    private SAV7b? S1;
    public List<int> SetFlags { get; } = [];
    public List<int> ClearedFlags { get; } = [];
    public List<int> WorkChanged { get; } = [];
    public List<string> WorkDiff { get; } = [];
    public EventWorkDiffCompatibility Message { get; private set; }

    private const int MAX_SAVEFILE_SIZE = 0x10_0000; // 1 MB

    public EventWorkDiff7b(SAV7b s1, SAV7b s2) => Diff(s1, s2);

    public EventWorkDiff7b(string f1, string f2)
    {
        Message = SanityCheckFiles(f1, f2, MAX_SAVEFILE_SIZE);
        if (Message != Valid)
            return;

        var s1 = SaveUtil.GetVariantSAV(f1);
        var s2 = SaveUtil.GetVariantSAV(f2);
        if (s1 is not SAV7b b1 || s2 is not SAV7b b2)
        {
            Message = DifferentVersion;
            return;
        }

        Diff(b1, b2);
    }

    private void Diff(SAV7b s1, SAV7b s2)
    {
        S1 = s1;
        if (s1.Version != s2.Version)
        {
            Message = DifferentVersion;
            return;
        }

        DiffSavesFlag(s1.Blocks.EventWork, s2.Blocks.EventWork, SetFlags, ClearedFlags);
        DiffSavesWork(s1.Blocks.EventWork, s2.Blocks.EventWork, WorkChanged, WorkDiff);
    }

    public IReadOnlyList<string> Summarize()
    {
        if (S1 == null)
            return [];
        var ew = S1.Blocks.EventWork;

        var fOn = SetFlags.Select(z => FlagSummary.Get(z, ew).ToString());
        var fOff = ClearedFlags.Select(z => FlagSummary.Get(z, ew).ToString());
        var wt = WorkChanged.Select((z, i) => WorkSummary.Get(z, ew, WorkDiff[i]).ToString());

        var list = new List<string> { "Flags: ON", "=========" };
        list.AddRange(fOn);
        if (SetFlags.Count == 0)
            list.Add("None.");

        list.Add("");
        list.Add("Flags: OFF");
        list.Add("==========");
        list.AddRange(fOff);
        if (ClearedFlags.Count == 0)
            list.Add("None.");

        list.Add("");
        list.Add("Work:");
        list.Add("=====");
        if (WorkChanged.Count == 0)
            list.Add("None.");
        list.AddRange(wt);

        return list;
    }

    private readonly record struct FlagSummary(EventVarType Type, int Index, int Raw)
    {
        public override string ToString() => $"{Raw:0000}\t{false}\t{Index:0000}\t{Type}";

        public static FlagSummary Get(int rawIndex, EventWork7b ew)
        {
            var type = ew.GetFlagType(rawIndex, out var subIndex);
            return new FlagSummary(type, subIndex, rawIndex);
        }
    }

    private readonly record struct WorkSummary(EventVarType Type, int Index, int Raw, string Text)
    {
        public override string ToString() => $"{Raw:000}\t{Text}\t{Index:000}\t{Type}";

        public static WorkSummary Get(int rawIndex, EventWork7b ew, string text)
        {
            var type = ew.GetFlagType(rawIndex, out var subIndex);
            return new WorkSummary(type, subIndex, rawIndex, text);
        }
    }
}
