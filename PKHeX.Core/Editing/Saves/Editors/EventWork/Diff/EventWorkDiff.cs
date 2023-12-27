using System;
using System.Collections.Generic;
using System.Linq;
using static PKHeX.Core.EventWorkDiffCompatibility;
using static PKHeX.Core.EventWorkDiffCompatibilityExtensions;

namespace PKHeX.Core;

/// <summary>
/// Calculates differences in the Event Blocks between two <see cref="SaveFile"/>.
/// </summary>
public sealed class EventBlockDiff<T, T2> : IEventWorkDiff where T : class, IEventFlagArray, IEventWorkArray<T2> where T2 : unmanaged, IEquatable<T2>
{
    public List<int> SetFlags { get; } = [];
    public List<int> ClearedFlags { get; } = [];
    public List<int> WorkChanged { get; } = [];
    public List<string> WorkDiff { get; } = [];
    public EventWorkDiffCompatibility Message { get; private set; }

    private const int MAX_SAVEFILE_SIZE = 0x10_0000; // 1 MB

    public EventBlockDiff(T s1, T s2) => Diff(s1, s2);

    public EventBlockDiff(string f1, string f2)
    {
        Message = SanityCheckFiles(f1, f2, MAX_SAVEFILE_SIZE);
        if (Message != Valid)
            return;
        var s1 = SaveUtil.GetVariantSAV(f1);
        var s2 = SaveUtil.GetVariantSAV(f2);
        if (s1 == null || s2 == null || s1.GetType() != s2.GetType() || s1 is not T t1 || s2 is not T t2)
        {
            Message = DifferentGameGroup;
            return;
        }
        Diff(t1, t2);
    }

    private static EventWorkDiffCompatibility SanityCheckSaveInfo(T s1, T s2)
    {
        if (s1.GetType() != s2.GetType())
            return DifferentGameGroup;

        if (s1 is not IVersion i1 || s2 is not IVersion i2 || i1.Version != i2.Version)
            return DifferentVersion;

        return Valid;
    }

    private void Diff(T s1, T s2)
    {
        Message = SanityCheckSaveInfo(s1, s2);
        if (Message != Valid)
            return;

        bool[] oldBits = s1.GetEventFlags();
        bool[] newBits = s2.GetEventFlags();
        var oldConst = s1.GetAllEventWork();
        var newConst = s2.GetAllEventWork();

        for (int i = 0; i < newBits.Length; i++)
        {
            if (oldBits[i] != newBits[i])
                (newBits[i] ? SetFlags : ClearedFlags).Add(i);
        }
        for (int i = 0; i < newConst.Length; i++)
        {
            if (oldConst[i].Equals(newConst[i]))
                continue;
            WorkChanged.Add(i);
            WorkDiff.Add($"{i}: {oldConst[i]} => {newConst[i]}");
        }
    }

    public IReadOnlyList<string> Summarize()
    {
        var fOn = SetFlags.Select(z => $"{z}");
        var fOff = ClearedFlags.Select(z => $"{z}");
        var wt = WorkChanged.Select((z, _) => $"{z}");

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
}
