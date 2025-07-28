using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace PKHeX.Core.Bulk;

/// <summary>
/// Analyzes content within a <see cref="SaveFile"/> for overall <see cref="PKM"/> legality analysis.
/// </summary>
public sealed class BulkAnalysis
{
    public readonly IReadOnlyList<SlotCache> AllData;
    public readonly IReadOnlyList<LegalityAnalysis> AllAnalysis;
    public readonly ITrainerInfo Trainer;
    public readonly List<BulkCheckResult> Parse = [];
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
        AllAnalysis = GetIndividualAnalysis(CollectionsMarshal.AsSpan(list));
        CloneFlags = new bool[AllData.Count];

        ScanAll();
        Valid = Parse.Count == 0 || Parse.TrueForAll(static z => z.Result.Valid);
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
    /// <remarks>
    /// Adding a bulk analyzer here as a user of this library? Be sure to register a <see cref="IExternalLegalityChecker"/> to display results in string output.
    /// </remarks>
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
    public void AddLine(SlotCache first, SlotCache second, LegalityCheckResultCode msg, CheckIdentifier i, Severity s = Severity.Invalid)
    {
        var line = GetSummary(first) + Environment.NewLine + GetSummary(second);
        var chk = CheckResult.Get(s, i, msg);
        Parse.Add(new(chk, line));
    }

    /// <summary>
    /// Adds a new entry to the <see cref="Parse"/> list.
    /// </summary>
    public void AddLine(SlotCache first, LegalityCheckResultCode msg, CheckIdentifier i, Severity s = Severity.Invalid)
    {
        var line = GetSummary(first);
        var chk = CheckResult.Get(s, i, msg);
        Parse.Add(new(chk, line));
    }

    private static LegalityAnalysis[] GetIndividualAnalysis(ReadOnlySpan<SlotCache> list)
    {
        var result = new LegalityAnalysis[list.Length];
        for (int i = 0; i < list.Length; i++)
            result[i] = Get(list[i]);
        return result;
    }

    private static LegalityAnalysis Get(SlotCache cache) => new(cache.Entity, cache.SAV.Personal, cache.Source.Type);

    public string Report(LegalityLocalizationSet localization)
    {
        var sb = new StringBuilder(1024);
        foreach (var (chk, comment) in Parse)
        {
            if (sb.Length != 0)
                sb.AppendLine(); // gap for next result

            var code = chk.Result;
            var template = code is LegalityCheckResultCode.External
                ? ExternalLegalityCheck.Localize(chk, localization)
                : code.GetTemplate(localization.Lines);
            var judge = localization.Description(chk.Judgement);
            sb.AppendFormat(localization.Lines.F0_1, judge, template);
            sb.AppendLine();

            sb.AppendLine(comment);
        }
        if (sb.Length == 0)
            sb.AppendLine(localization.Lines.Valid);
        return sb.ToString();
    }
}
