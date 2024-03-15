using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using static PKHeX.Core.GameVersion;

namespace PKHeX.Core.Searching;

/// <summary>
/// <see cref="PKM"/> search settings &amp; searcher
/// </summary>
public sealed class SearchSettings
{
    public byte Format { get; init; }
    public byte Generation { get; init; }
    public required ushort Species { get; init; }
    public int Ability { get; init; } = -1;
    public Nature Nature { get; init; } = Nature.Random;
    public int Item { get; init; } = -1;
    public GameVersion Version { get; init; }
    public int HiddenPowerType { get; init; } = -1;

    public SearchComparison SearchFormat { get; init; }
    public SearchComparison SearchLevel { get; init; }

    public bool? SearchShiny { get; set; }
    public bool? SearchLegal { get; set; }
    public bool? SearchEgg { get; set; }
    public int? ESV { get; set; }
    public int? Level { get; init; }

    public int IVType { get; init; }
    public int EVType { get; init; }

    public CloneDetectionMethod SearchClones { get; set; }
    public string BatchInstructions { get; init; } = string.Empty;
    private IReadOnlyList<StringInstruction> BatchFilters { get; set; } = [];
    private IReadOnlyList<StringInstruction> BatchFiltersMeta { get; set; } = [];

    public readonly List<ushort> Moves = [];

    // ReSharper disable once CollectionNeverUpdated.Global
    /// <summary>
    /// Extra Filters to be checked after all other filters have been checked.
    /// </summary>
    /// <remarks>Collection is iterated right before clones are checked.</remarks>
    public List<Func<PKM, bool>> ExtraFilters { get; } = [];

    /// <summary>
    /// Adds a move to the required move list.
    /// </summary>
    /// <param name="move"></param>
    public void AddMove(ushort move)
    {
        if (move != 0 && !Moves.Contains(move))
            Moves.Add(move);
    }

    /// <summary>
    /// Searches the input list, filtering out entries as specified by the settings.
    /// </summary>
    /// <param name="list">List of entries to search</param>
    /// <returns>Search results that match all criteria</returns>
    public IEnumerable<PKM> Search(IEnumerable<PKM> list)
    {
        InitializeFilters();
        var result = SearchInner(list);

        if (SearchClones != CloneDetectionMethod.None)
        {
            var method = SearchUtil.GetCloneDetectMethod(SearchClones);
            result = SearchUtil.GetExtraClones(result, method);
        }

        return result;
    }

    /// <summary>
    /// Searches the input list, filtering out entries as specified by the settings.
    /// </summary>
    /// <param name="list">List of entries to search</param>
    /// <returns>Search results that match all criteria</returns>
    public IEnumerable<SlotCache> Search(IEnumerable<SlotCache> list)
    {
        InitializeFilters();
        var result = SearchInner(list);

        if (SearchClones != CloneDetectionMethod.None)
        {
            var method = SearchUtil.GetCloneDetectMethod(SearchClones);
            string GetHash(SlotCache z) => method(z.Entity);
            result = SearchUtil.GetExtraClones(result, GetHash);
        }

        return result;
    }

    private void InitializeFilters()
    {
        var filters = StringInstruction.GetFilters(BatchInstructions);
        var meta = filters.Where(z => Core.BatchFilters.FilterMeta.Any(x => x.IsMatch(z.PropertyName))).ToList();
        if (meta.Count != 0)
            filters.RemoveAll(meta.Contains);
        BatchFilters = filters;
        BatchFiltersMeta = meta;
    }

    private IEnumerable<PKM> SearchInner(IEnumerable<PKM> list)
    {
        foreach (var pk in list)
        {
            if (!IsSearchMatch(pk))
                continue;
            yield return pk;
        }
    }

    private IEnumerable<SlotCache> SearchInner(IEnumerable<SlotCache> list)
    {
        foreach (var entry in list)
        {
            var pk = entry.Entity;
            if (BatchFiltersMeta.Count != 0 && !BatchEditing.IsFilterMatchMeta(BatchFiltersMeta, entry))
                continue;
            if (!IsSearchMatch(pk))
                continue;
            yield return entry;
        }
    }

    private bool IsSearchMatch(PKM pk)
    {
        if (!SearchSimple(pk))
            return false;
        if (!SearchIntermediate(pk))
            return false;
        if (!SearchComplex(pk))
            return false;

        foreach (var filter in ExtraFilters)
        {
            if (!filter(pk))
                return false;
        }
        return true;
    }

    private bool SearchSimple(PKM pk)
    {
        if (Format > 0 && !SearchUtil.SatisfiesFilterFormat(pk, Format, SearchFormat))
            return false;
        if (Species != 0 && pk.Species != Species)
            return false;
        if (Ability > -1 && pk.Ability != Ability)
            return false;
        if (Nature.IsFixed() && pk.StatNature != Nature)
            return false;
        if (Item > -1 && pk.HeldItem != Item)
            return false;
        if (Version.IsValidSavedVersion() && pk.Version != Version)
            return false;
        return true;
    }

    private bool SearchIntermediate(PKM pk)
    {
        if (Generation > 0 && !SearchUtil.SatisfiesFilterGeneration(pk, Generation))
            return false;
        if (Moves.Count > 0 && !SearchUtil.SatisfiesFilterMoves(pk, CollectionsMarshal.AsSpan(Moves)))
            return false;
        if (HiddenPowerType > -1 && pk.HPType != HiddenPowerType)
            return false;
        if (SearchShiny != null && pk.IsShiny != SearchShiny)
            return false;

        if (IVType > 0 && !SearchUtil.SatisfiesFilterIVs(pk, IVType))
            return false;
        if (EVType > 0 && !SearchUtil.SatisfiesFilterEVs(pk, EVType))
            return false;

        return true;
    }

    private bool SearchComplex(PKM pk)
    {
        if (SearchEgg != null && !FilterResultEgg(pk))
            return false;
        if (Level is { } x and not 0 && !SearchUtil.SatisfiesFilterLevel(pk, SearchLevel, x))
            return false;
        if (SearchLegal != null && new LegalityAnalysis(pk).Valid != SearchLegal)
            return false;
        if (BatchFilters.Count != 0 && !SearchUtil.SatisfiesFilterBatchInstruction(pk, BatchFilters))
            return false;

        return true;
    }

    private bool FilterResultEgg(PKM pk)
    {
        if (SearchEgg == false)
            return !pk.IsEgg;
        if (ESV != null)
            return pk.IsEgg && pk.PSV == ESV;
        return pk.IsEgg;
    }

    public IReadOnlyList<GameVersion> GetVersions(SaveFile sav) => GetVersions(sav, GetFallbackVersion(sav));

    public IReadOnlyList<GameVersion> GetVersions(SaveFile sav, GameVersion fallback)
    {
        if (Version > 0)
            return [Version];

        return Generation switch
        {
            1 when !ParseSettings.AllowGen1Tradeback => [RD, BU, GN, YW],
            2 when sav is SAV2 {Korean: true} => [GD, SI],
            1 or 2 => [RD, BU, GN, YW, /* */ GD, SI, C],

            _ when fallback.GetGeneration() == Generation => GameUtil.GetVersionsWithinRange(sav, Generation).ToArray(),
            _ => GameUtil.GameVersions,
        };
    }

    private static GameVersion GetFallbackVersion(ITrainerInfo sav)
    {
        var parent = GameUtil.GetMetLocationVersionGroup(sav.Version);
        if (parent == Invalid)
            parent = GameUtil.GetMetLocationVersionGroup(GameUtil.GetVersion(sav.Generation));
        return parent;
    }
}
