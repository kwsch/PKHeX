using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using static PKHeX.Core.EventWorkDiffCompatibility;
using static PKHeX.Core.EventWorkDiffCompatibilityExtensions;

namespace PKHeX.Core;

/// <summary>
/// Calculates differences in the Event Blocks between two <see cref="SaveFile"/>.
/// </summary>
public sealed class EventBlockDiff<TSave, TWorkValue> : IEventWorkDiff
    where TSave : class, IEventFlagArray, IEventWorkArray<TWorkValue>
    where TWorkValue : unmanaged, IEquatable<TWorkValue>
{
    public List<int> SetFlags { get; } = [];
    public List<int> ClearedFlags { get; } = [];
    public List<int> WorkChanged { get; } = [];
    public List<string> WorkDiff { get; } = [];
    public EventWorkDiffCompatibility Message { get; private set; }

    private const int MAX_SAVEFILE_SIZE = 0x10_0000; // 1 MB

    private static bool TryGetSaveFile(string path, [NotNullWhen(true)] out TSave? sav, out GameVersion version)
    {
        version = default;
        sav = null;
        if (!SaveUtil.TryGetSaveFile(path, out var s) || s is not TSave b)
            return false;
        sav = b;
        version = s.Version;
        return true;
    }

    public EventBlockDiff(TSave s1, TSave s2) => Diff(s1, s2);

    public EventBlockDiff(string f1, string f2)
    {
        Message = SanityCheckFiles(f1, f2, MAX_SAVEFILE_SIZE);
        if (Message != Valid)
            return;

        if (!TryGetSaveFile(f1, out var s1, out var v1) || !TryGetSaveFile(f2, out var s2, out var v2))
        {
            Message = DifferentGameGroup;
            return;
        }
        if (v1 != v2)
        {
            Message = DifferentVersion;
            return;
        }

        Diff(s1, s2);
    }

    private static EventWorkDiffCompatibility SanityCheckSaveInfo(TSave s1, TSave s2)
    {
        if (s1.GetType() != s2.GetType())
            return DifferentGameGroup;

        return Valid;
    }

    private void Diff(TSave s1, TSave s2)
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

        list.Add(string.Empty);
        list.Add("Flags: OFF");
        list.Add("==========");
        list.AddRange(fOff);
        if (ClearedFlags.Count == 0)
            list.Add("None.");

        list.Add(string.Empty);
        list.Add("Work:");
        list.Add("=====");
        if (WorkChanged.Count == 0)
            list.Add("None.");
        list.AddRange(wt);

        return list;
    }
}
