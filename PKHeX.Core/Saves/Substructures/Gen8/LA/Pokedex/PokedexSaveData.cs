using System;
using System.Diagnostics.CodeAnalysis;

namespace PKHeX.Core;

/// <summary>
/// Represents direct manipulation and access for <see cref="GameVersion.PLA"/> Pokédex entries.
/// </summary>
public sealed class PokedexSaveData
{
    private readonly PokedexSaveGlobalData GlobalData;
    private readonly PokedexSaveLocalData[] LocalData;
    private readonly PokedexSaveResearchEntry[] ResearchEntries;
    private readonly PokedexSaveStatisticsEntry[] StatisticsEntries;

    public const int POKEDEX_SAVE_DATA_SIZE = 0x1E460;

    public const int STATISTICS_ENTRIES_MAX = 1480;

    public PokedexSaveData(Memory<byte> data)
    {
        ArgumentOutOfRangeException.ThrowIfNotEqual(data.Length, POKEDEX_SAVE_DATA_SIZE);

        GlobalData = new PokedexSaveGlobalData(data[..PokedexSaveGlobalData.SIZE]);

        LocalData = new PokedexSaveLocalData[5];
        for (var i = 0; i < LocalData.Length; i++)
            LocalData[i] = new PokedexSaveLocalData(data.Slice(0x10 + (PokedexSaveLocalData.SIZE * i), PokedexSaveLocalData.SIZE));

        ResearchEntries = new PokedexSaveResearchEntry[PokedexSave8a.MAX_SPECIES];
        for (var i = 0; i < ResearchEntries.Length; i++)
            ResearchEntries[i] = new PokedexSaveResearchEntry(data.Slice(0x70 + (0x58 * i), PokedexSaveResearchEntry.SIZE));

        StatisticsEntries = new PokedexSaveStatisticsEntry[STATISTICS_ENTRIES_MAX];
        for (var i = 0; i < StatisticsEntries.Length; i++)
            StatisticsEntries[i] = new PokedexSaveStatisticsEntry(data.Slice(0x151A8 + (PokedexSaveStatisticsEntry.SIZE * i), PokedexSaveStatisticsEntry.SIZE));
    }

    public bool IsPokedexCompleted(PokedexType8a which) => (GlobalData.Flags & (which < PokedexType8a.Count ? (1 << (int)which) : 1)) != 0;
    public bool IsPokedexPerfect(PokedexType8a which) => (GlobalData.Flags & ((which < PokedexType8a.Count ? (1 << (int)which) : 1) << 6)) != 0;

    public void SetPokedexCompleted(PokedexType8a which) => GlobalData.Flags |= which < PokedexType8a.Count ? (1u << (int)which) : 1;
    public void SetPokedexPerfect(PokedexType8a which) => GlobalData.Flags |= (which < PokedexType8a.Count ? (1u << (int)which) : 1) << 6;

    public PokedexSaveResearchEntry GetResearchEntry(ushort species) => ResearchEntries[species];

    public bool TryGetStatisticsEntry(ushort species, byte form, [NotNullWhen(true)] out PokedexSaveStatisticsEntry? entry)
    {
        var mash = (ushort)(species | (form << 11));
        var fstIdIndex = PokedexConstants8a.FormStorageIndexIds.BinarySearch(mash);
        if (fstIdIndex >= 0)
        {
            entry = StatisticsEntries[PokedexConstants8a.FormStorageIndexValues[fstIdIndex]];
            return true;
        }

        entry = null;
        return false;
    }

    public bool TryGetStatisticsEntry(PKM pk, [NotNullWhen(true)] out PokedexSaveStatisticsEntry? entry, out int shift)
    {
        shift = 0;
        if (pk.IsShiny)
            shift += 4;
        if (((IAlpha)pk).IsAlpha)
            shift += 2;
        if ((pk.Gender & ~2) != 0)
            shift++;

        return TryGetStatisticsEntry(pk.Species, pk.Form, out entry);
    }

    public int GetPokeGetCount(ushort species) => GetResearchEntry(species).NumObtained;
    public int GetPokeResearchRate(ushort species) => GetResearchEntry(species).ResearchRate;

    public ushort NextUpdateCounter()
    {
        var curCounter = GlobalData.UpdateCounter;
        if (curCounter < PokedexConstants8a.MaxPokedexResearchPoints)
            GlobalData.UpdateCounter = (ushort)(curCounter + 1);

        return curCounter;
    }

    public void IncrementReportCounter()
    {
        var reportCounter = GlobalData.ReportCounter;

        // If we need to re-set the counter, reset all species counters
        if (reportCounter >= PokedexConstants8a.MaxPokedexResearchPoints)
        {
            foreach (var entry in ResearchEntries)
            {
                if (entry.LastUpdatedReportCounter != 0)
                    entry.LastUpdatedReportCounter = 1;
            }

            reportCounter = 1;
        }

        GlobalData.ReportCounter = (ushort)(reportCounter + 1);
    }

    public ushort GetReportCounter() => GlobalData.ReportCounter;

    public int GetTotalResearchPoint() => GlobalData.TotalResearchPoints;

    public void SetTotalResearchPoint(int tp) => GlobalData.TotalResearchPoints = tp;

    public bool HasAnyReport(ushort species) => GetResearchEntry(species).HasAnyReport;
    public bool IsPerfect(ushort species) => GetResearchEntry(species).IsPerfect;

    public void SetGlobalField04(byte v) => GlobalData.Field_04 = v;
    public byte GetGlobalField04() => GlobalData.Field_04;

    public void SetLocalField03(int idx, byte v) => LocalData[idx].Field_03 = v;

    public byte GetLocalField03(int idx) => LocalData[idx].Field_03;

    public void GetLocalParameters(int idx, out byte outField02, out ushort outField00, out uint outField04, out uint outField08, out ushort outField0C)
    {
        var local = LocalData[idx];

        outField02 = local.Field_02;
        outField00 = local.Field_00;
        outField04 = local.Field_04;
        outField08 = local.Field_08;
        outField0C = local.Field_0C;
    }

    public void SetGlobalFormField(byte form) => GlobalData.FormField = form;
    public int GetGlobalFormField() => GlobalData.FormField;
}
