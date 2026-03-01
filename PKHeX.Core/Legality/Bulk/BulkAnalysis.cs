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
    public readonly Dictionary<ulong, (SlotCache Slot, int Index)> Trackers = [];
    public bool Valid => Parse.Count == 0 || Parse.TrueForAll(static z => z.Result.Valid);

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

    public BulkAnalysis(SaveFile sav, BulkAnalysisSettings settings) : this(GetSlots(sav), settings, sav)
    {
        RunSaveSpecificChecks();
    }

    public BulkAnalysis(IEnumerable<SlotCache> source, BulkAnalysisSettings settings, ITrainerInfo tr) : this(GetSlots(source), settings, tr)
    {
    }

    private BulkAnalysis(List<SlotCache> list, BulkAnalysisSettings settings, ITrainerInfo tr)
    {
        Trainer = tr;
        Settings = settings;
        AllData = list;
        AllAnalysis = GetIndividualAnalysis(CollectionsMarshal.AsSpan(list));
        CloneFlags = new bool[AllData.Count];

        ScanAll();
    }

    private static List<SlotCache> GetSlots(SaveFile sav)
    {
        var list = new List<SlotCache>(sav.BoxSlotCount + (sav.HasParty ? 6 : 0) + 5);
        SlotInfoLoader.AddFromSaveFile(sav, list);
        list.RemoveAll(IsEmptyData);
        return list;
    }

    private static List<SlotCache> GetSlots(IEnumerable<SlotCache> source)
    {
        var list = new List<SlotCache>();
        foreach (var slot in source)
        {
            if (IsEmptyData(slot))
                continue;
            list.Add(slot);
        }
        return list;
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
    private static readonly List<IBulkAnalyzer> Analyzers =
    [
        new StandardCloneChecker(),
        new DuplicateTrainerChecker(),
        new DuplicatePIDChecker(),
        new DuplicateEncryptionChecker(),
        new HandlerChecker(),
        new DuplicateGiftChecker(),
    ];

    private static readonly List<IBulkAnalyzer> SaveAnalyzers =
    [
        new DuplicateFusionChecker(),
        new DuplicateUniqueItemChecker(),
    ];

    private void ScanAll()
    {
        foreach (var analyzer in Analyzers)
            analyzer.Analyze(this);
        foreach (var analyzer in ExternalBulkCheck.ExternalCheckers.Values)
            analyzer.Analyze(this);
    }

    private void RunSaveSpecificChecks()
    {
        foreach (var analyzer in SaveAnalyzers)
            analyzer.Analyze(this);
    }

    private static string GetSummary(SlotCache entry) => $"[{entry.Identify()}]";

    /// <summary>
    /// Adds a new entry to the <see cref="Parse"/> list.
    /// </summary>
    public void AddLine(SlotCache first, CheckIdentifier id, int index1,
        LegalityCheckResultCode msg, ushort argument1 = 0, ushort argument2 = 0, Severity s = Severity.Invalid)
    {
        var line = GetSummary(first);
        var chk = new CheckResult
        {
            Judgement = s,
            Identifier = id,
            Result = msg,
            Argument = argument1,
            Argument2 = argument2,
        };
        Parse.Add(new(chk, line, index1));
    }

    /// <summary>
    /// Adds a new entry to the <see cref="Parse"/> list.
    /// </summary>
    public void AddLine(SlotCache first, SlotCache second, CheckIdentifier id, int index1, int index2,
        LegalityCheckResultCode msg, ushort argument1 = 0, ushort argument2 = 0, Severity s = Severity.Invalid)
    {
        var line = GetSummary(first) + Environment.NewLine + GetSummary(second);
        var chk = new CheckResult
        {
            Judgement = s,
            Identifier = id,
            Result = msg,
            Argument = argument1,
            Argument2 = argument2,
        };
        Parse.Add(new(chk, line, index1, index2));
    }

    public void AddMessage(string message, CheckResult chk, int index = BulkCheckResult.NoIndex) => Parse.Add(new(chk, message, index));

    public void AddExternal(SlotCache first, CheckIdentifier id, int index1, ushort identity, ushort argument = 0, Severity s = Severity.Invalid)
        => AddLine(first, id, index1, LegalityCheckResultCode.External, identity, argument, s);

    public void AddExternal(SlotCache first, SlotCache second, CheckIdentifier id, int index1, int index2, ushort identity, ushort argument = 0, Severity s = Severity.Invalid)
        => AddLine(first, second, id, index1, index2, LegalityCheckResultCode.External, identity, argument, s);

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
        var context = LegalityLocalizationContext.Create(AllAnalysis[0], localization);
        foreach (var (chk, comment, index1, index2) in Parse)
        {
            if (sb.Length != 0)
                sb.AppendLine(); // gap for next result

            var code = chk.Result;
            if (code is LegalityCheckResultCode.External)
            {
                var judge = localization.Description(chk.Judgement);
                var template = ExternalBulkCheck.Localize(chk, localization, AllAnalysis, index1, index2);
                sb.AppendFormat(localization.Lines.F0_1, judge, template);
            }
            else
            {
                sb.Append(context.Humanize(chk));
            }
            sb.AppendLine();

            if (comment.Length != 0)
                sb.AppendLine(comment);
        }
        if (sb.Length == 0)
            sb.AppendLine(localization.Lines.Valid);
        return sb.ToString();
    }
}
