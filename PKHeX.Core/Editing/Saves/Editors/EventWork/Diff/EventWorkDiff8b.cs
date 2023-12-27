using System.Collections.Generic;
using System.Linq;
using static PKHeX.Core.EventWorkUtil;
using static PKHeX.Core.EventWorkDiffCompatibility;
using static PKHeX.Core.EventWorkDiffCompatibilityExtensions;

namespace PKHeX.Core;

public sealed class EventWorkDiff8b : IEventWorkDiff
{
    private SAV8BS? S1;
    public List<int> SetSystem { get; } = [];
    public List<int> SetFlags { get; } = [];
    public List<int> ClearedSystem { get; } = [];
    public List<int> ClearedFlags { get; } = [];
    public List<int> WorkChanged { get; } = [];
    public List<string> WorkDiff { get; } = [];
    public EventWorkDiffCompatibility Message { get; private set; }

    private const int MAX_SAVEFILE_SIZE = 0x10_0000; // 1 MB

    public EventWorkDiff8b(SAV8BS s1, SAV8BS s2) => Diff(s1, s2);

    public EventWorkDiff8b(string f1, string f2)
    {
        Message = SanityCheckFiles(f1, f2, MAX_SAVEFILE_SIZE);
        if (Message != Valid)
            return;
        var s1 = SaveUtil.GetVariantSAV(f1);
        var s2 = SaveUtil.GetVariantSAV(f2);
        if (s1 is not SAV8BS b1 || s2 is not SAV8BS b2)
        {
            Message = DifferentGameGroup;
            return;
        }
        Diff(b1, b2);
    }

    private void Diff(SAV8BS s1, SAV8BS s2)
    {
        S1 = s1;
        if (s1.Version != s2.Version)
        {
            Message = DifferentVersion;
            return;
        }

        DiffSavesFlag(s1.FlagWork, s2.FlagWork, SetFlags, ClearedFlags);
        DiffSavesSystem(s1.FlagWork, s2.FlagWork, SetSystem, ClearedSystem);
        DiffSavesWork(s1.FlagWork, s2.FlagWork, WorkChanged, WorkDiff);
        S1 = s1;
    }

    public IReadOnlyList<string> Summarize()
    {
        if (S1 == null)
            return [];

        var fOn = SetFlags.Select(z => new FlagSummary(z).ToString());
        var fOff = ClearedFlags.Select(z => new FlagSummary(z).ToString());

        var sOn = SetSystem.Select(z => new FlagSummary(z).ToString());
        var sOff = ClearedSystem.Select(z => new FlagSummary(z).ToString());

        var wt = WorkChanged.Select((z, i) => new WorkSummary(z, WorkDiff[i]).ToString());

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

        list.Add("System: ON");
        list.Add("=========");
        list.AddRange(sOn);
        if (SetFlags.Count == 0)
            list.Add("None.");

        list.Add("");
        list.Add("System: OFF");
        list.Add("==========");
        list.AddRange(sOff);
        if (ClearedSystem.Count == 0)
            list.Add("None.");

        list.Add("");
        list.Add("Work:");
        list.Add("=====");
        if (WorkChanged.Count == 0)
            list.Add("None.");
        list.AddRange(wt);

        return list;
    }

    private readonly record struct FlagSummary(int Raw)
    {
        public override string ToString() => $"{Raw:0000}\t{false}";
    }

    private readonly record struct WorkSummary(int Raw, string Text)
    {
        public override string ToString() => $"{Raw:000}\t{Text}";
    }
}
