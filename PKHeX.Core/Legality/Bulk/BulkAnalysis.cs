using System;
using System.Collections.Generic;

namespace PKHeX.Core.Bulk;

/// <summary>
/// Analyzes content within a <see cref="SaveFile"/> for overall <see cref="PKM"/> legality analysis.
/// </summary>
public sealed class BulkAnalysis
{
    public readonly IReadOnlyList<SlotCache> AllData;
    public readonly IReadOnlyList<LegalityAnalysis> AllAnalysis;
    public readonly ITrainerInfo Trainer;
    public readonly List<CheckResult> Parse = [];
    public readonly Dictionary<ulong, SlotCache> Trackers = [];
    public readonly bool Valid;

    public readonly BulkAnalysisSettings Settings;
    private readonly bool[] CloneFlags;

    /// <summary>
    /// Checks if the <see cref="AllData"/> entity at <see cref="entryIndex"/> was previously marked as a clone of another index.
    /// </summary>
    public bool GetIsClone(int entryIndex) => CloneFlags[entryIndex];

    /// <summary>
    /// Marks the <see cref="AllData"/> entity at <see cref="entryIndex"/> as a clone of another index.
    /// </summary>
    public bool SetIsClone(int entryIndex, bool value = true) => CloneFlags[entryIndex] = value;

    public BulkAnalysis(SaveFile sav, BulkAnalysisSettings settings)
    {
        Trainer = sav;
        Settings = settings;
        var list = new List<SlotCache>(sav.BoxSlotCount + (sav.HasParty ? 6 : 0) + 5);
        SlotInfoLoader.AddFromSaveFile(sav, list);
        list.RemoveAll(IsEmptyData);
        AllData = list;
        AllAnalysis = GetIndividualAnalysis(list);
        CloneFlags = new bool[AllData.Count];

        ScanAll();
        Valid = Parse.Count == 0 || Parse.TrueForAll(static z => z.Valid);
    }

    // Remove things that aren't actual stored data, or already flagged by legality checks.
    private static bool IsEmptyData(SlotCache obj)
    {
        var pk = obj.Entity;
        if (pk.Species - 1u >= pk.MaxSpeciesID)
            return true;
        if (!pk.ChecksumValid)
            return true;
        return false;
    }

    /// <summary>
    /// Supported <see cref="IBulkAnalyzer"/> checkers that will be iterated through to check all entities.
    /// </summary>
    public static readonly List<IBulkAnalyzer> Analyzers =
    [
        new StandardCloneChecker(),
        new DuplicateTrainerChecker(),
        new DuplicatePIDChecker(),
        new DuplicateEncryptionChecker(),
        new HandlerChecker(),
        new DuplicateGiftChecker(),
    ];

    private void ScanAll()
    {
        foreach (var analyzer in Analyzers)
            analyzer.Analyze(this);
    }

    private static string GetSummary(SlotCache entry) => $"[{entry.Identify()}]";

    /// <summary>
    /// Adds a new entry to the <see cref="Parse"/> list.
    /// </summary>
    public void AddLine(SlotCache first, SlotCache second, string msg, CheckIdentifier i, Severity s = Severity.Invalid)
    {
        var c = $"{msg}{Environment.NewLine}{GetSummary(first)}{Environment.NewLine}{GetSummary(second)}{Environment.NewLine}";
        var chk = new CheckResult(s, i, c);
        Parse.Add(chk);
    }

    /// <summary>
    /// Adds a new entry to the <see cref="Parse"/> list.
    /// </summary>
    public void AddLine(SlotCache first, string msg, CheckIdentifier i, Severity s = Severity.Invalid)
    {
        var c = $"{msg}{Environment.NewLine}{GetSummary(first)}{Environment.NewLine}";
        var chk = new CheckResult(s, i, c);
        Parse.Add(chk);
    }

    private static LegalityAnalysis[] GetIndividualAnalysis(IReadOnlyList<SlotCache> pkms)
    {
        var results = new LegalityAnalysis[pkms.Count];
        for (int i = 0; i < pkms.Count; i++)
            results[i] = Get(pkms[i]);
        return results;
    }

    private static LegalityAnalysis Get(SlotCache cache) => new(cache.Entity, cache.SAV.Personal, cache.Source.Type);
}
