using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using static PKHeX.Core.GameVersion;
using static PKHeX.Core.Locations;

namespace PKHeX.Core;

/// <summary>
/// Cached copies of Met Location lists
/// </summary>
public sealed class MetDataSource(GameStrings s)
{
    private readonly List<ComboItem> MetGen2 = CreateGen2(s);
    private readonly List<ComboItem> MetGen3 = CreateGen3(s);
    private readonly List<ComboItem> MetGen3CXD = CreateGen3CXD(s);
    private readonly List<ComboItem> MetGen4 = CreateGen4(s);
    private readonly List<ComboItem> MetGen5 = CreateGen5(s);
    private readonly List<ComboItem> MetGen6 = CreateGen6(s);
    private readonly List<ComboItem> MetGen7 = CreateGen7(s);
    private readonly List<ComboItem> MetGen7GG = CreateGen7GG(s);
    private readonly List<ComboItem> MetGen8 = CreateGen8(s);
    private readonly List<ComboItem> MetGen8a = CreateGen8a(s);
    private readonly List<ComboItem> MetGen8b = CreateGen8b(s);
    private readonly List<ComboItem> MetGen9 = CreateGen9(s);

    private IReadOnlyList<ComboItem>? MetGen4Transfer;
    private IReadOnlyList<ComboItem>? MetGen5Transfer;

    private static List<ComboItem> CreateGen2(GameStrings s)
    {
        var locations = Util.GetCBList(s.Gen2.Met0.AsSpan(0, 0x60));
        Util.AddCBWithOffset(locations, s.Gen2.Met0.AsSpan(0x7E, 2), 0x7E);
        return locations;
    }

    private static List<ComboItem> CreateGen3(GameStrings s)
    {
        var locations = Util.GetCBList(s.Gen3.Met0.AsSpan(0, 213));
        Util.AddCBWithOffset(locations, s.Gen3.Met0.AsSpan(253, 3), 253);
        return locations;
    }

    private static List<ComboItem> CreateGen3CXD(GameStrings s)
    {
        var list = Util.GetCBList(s.CXD.Met0);
        list.RemoveAll(z => z.Text.Length == 0);
        return list;
    }

    private static List<ComboItem> CreateGen4(GameStrings s)
    {
        var locations = Util.GetCBList(s.Gen4.Met0, 0);
        Util.AddCBWithOffset(locations, s.Gen4.Met2, 2000, Locations.Daycare4);
        Util.AddCBWithOffset(locations, s.Gen4.Met2, 2000, Locations.LinkTrade4);
        Util.AddCBWithOffset(locations, s.Gen4.Met3, 3000, Locations.Ranger4);
        Util.AddCBWithOffset(locations, s.Gen4.Met0, 0000, Locations4.Met0);
        Util.AddCBWithOffset(locations, s.Gen4.Met2, 2000, Locations4.Met2);
        Util.AddCBWithOffset(locations, s.Gen4.Met3, 3000, Locations4.Met3);
        return locations;
    }

    private ComboItem[] CreateGen4Transfer()
    {
        // Pal Park to front
        var met = MetGen4.ToArray();
        var index = Array.FindIndex(met, static z => z.Value == Locations.Transfer3);
        var pal = met[index];
        Array.Copy(met, 0, met, 1, index);
        met[0] = pal;
        return met;
    }

    private static List<ComboItem> CreateGen5(GameStrings s)
    {
        var locations = Util.GetCBList(s.Gen5.Met0, 0);
        Util.AddCBWithOffset(locations, s.Gen5.Met6, 60000, Locations.Daycare5);
        Util.AddCBWithOffset(locations, s.Gen5.Met3, 30000, Locations.LinkTrade5);
        Util.AddCBWithOffset(locations, s.Gen5.Met0, 00000, Locations5.Met0);
        Util.AddCBWithOffset(locations, s.Gen5.Met3, 30000, Locations5.Met3);
        Util.AddCBWithOffset(locations, s.Gen5.Met4, 40000, Locations5.Met4);
        Util.AddCBWithOffset(locations, s.Gen5.Met6, 60000, Locations5.Met6);
        return locations;
    }

    private ComboItem[] CreateGen5Transfer()
    {
        // PokéTransfer to front
        var index = MetGen5.FindIndex(static z => z.Value == Locations.Transfer4);
        var xfr = MetGen5[index];
        var result = new ComboItem[MetGen5.Count];
        result[0] = xfr;
        var dest = result.AsSpan(1);
        var span = CollectionsMarshal.AsSpan(MetGen5);
        span[..index].CopyTo(dest);
        span[(index + 1)..].CopyTo(dest[index..]);
        return result;
    }

    private static List<ComboItem> CreateGen6(GameStrings s)
    {
        var locations = Util.GetCBList(s.Gen6.Met0, 0);
        Util.AddCBWithOffset(locations, s.Gen6.Met6, 60000, Locations.Daycare5);
        Util.AddCBWithOffset(locations, s.Gen6.Met3, 30000, Locations.LinkTrade6);
        Util.AddCBWithOffset(locations, s.Gen6.Met0, 00000, Locations6.Met0);
        Util.AddCBWithOffset(locations, s.Gen6.Met3, 30000, Locations6.Met3);
        Util.AddCBWithOffset(locations, s.Gen6.Met4, 40000, Locations6.Met4);
        Util.AddCBWithOffset(locations, s.Gen6.Met6, 60000, Locations6.Met6);
        return locations;
    }

    private static List<ComboItem> CreateGen7(GameStrings s)
    {
        var locations = Util.GetCBList(s.Gen7.Met0, 0);
        Util.AddCBWithOffset(locations, s.Gen7.Met6, 60000, Locations.Daycare5);
        Util.AddCBWithOffset(locations, s.Gen7.Met3, 30000, Locations.LinkTrade6);
        Util.AddCBWithOffset(locations, s.Gen7.Met0, 00000, Locations7.Met0);
        Util.AddCBWithOffset(locations, s.Gen7.Met3, 30000, Locations7.Met3);
        Util.AddCBWithOffset(locations, s.Gen7.Met4, 40000, Locations7.Met4);
        Util.AddCBWithOffset(locations, s.Gen7.Met6, 60000, Locations7.Met6);
        return locations;
    }

    private static List<ComboItem> CreateGen7GG(GameStrings s)
    {
        var locations = Util.GetCBList(s.Gen7b.Met0, 0);
        Util.AddCBWithOffset(locations, s.Gen7b.Met6, 60000, Locations.Daycare5);
        Util.AddCBWithOffset(locations, s.Gen7b.Met3, 30000, Locations.LinkTrade6);
        Util.AddCBWithOffset(locations, s.Gen7b.Met0, 00000, Locations7b.Met0);
        Util.AddCBWithOffset(locations, s.Gen7b.Met3, 30000, Locations7b.Met3);
        Util.AddCBWithOffset(locations, s.Gen7b.Met4, 40000, Locations7b.Met4);
        Util.AddCBWithOffset(locations, s.Gen7b.Met6, 60000, Locations7b.Met6);
        return locations;
    }

    private static List<ComboItem> CreateGen8(GameStrings s)
    {
        var locations = Util.GetCBList(s.Gen8.Met0, 0);
        Util.AddCBWithOffset(locations, s.Gen8.Met6, 60000, Locations.Daycare5);
        Util.AddCBWithOffset(locations, s.Gen8.Met3, 30000, Locations.LinkTrade6);
        Util.AddCBWithOffset(locations, s.Gen8.Met0, 00000, Locations8.Met0);
        Util.AddCBWithOffset(locations, s.Gen8.Met3, 30000, Locations8.Met3);
        Util.AddCBWithOffset(locations, s.Gen8.Met4, 40000, Locations8.Met4);
        Util.AddCBWithOffset(locations, s.Gen8.Met6, 60000, Locations8.Met6);

        // Add in the BDSP+PLA magic met locations.
        locations.Add(new ComboItem($"{s.EggName} (HOME)", LocationsHOME.SWSHEgg));
        locations.Add(new ComboItem(s.gamelist[(int)SL], LocationsHOME.SWSL));
        locations.Add(new ComboItem(s.gamelist[(int)VL], LocationsHOME.SHVL));
        locations.Add(new ComboItem(s.gamelist[(int)BD], LocationsHOME.SWBD));
        locations.Add(new ComboItem(s.gamelist[(int)SP], LocationsHOME.SHSP));
        locations.Add(new ComboItem(s.gamelist[(int)PLA], LocationsHOME.SWLA));

        return locations;
    }

    private static List<ComboItem> CreateGen8a(GameStrings s)
    {
        var locations = Util.GetCBList(s.Gen8a.Met0, 0);
        Util.AddCBWithOffset(locations, s.Gen8a.Met3, 30000, Locations.LinkTrade6);
        Util.AddCBWithOffset(locations, s.Gen8a.Met0, 00000, Locations8a.Met0);
        Util.AddCBWithOffset(locations, s.Gen8a.Met3, 30000, Locations8a.Met3);
        Util.AddCBWithOffset(locations, s.Gen8a.Met4, 40000, Locations8a.Met4);
        Util.AddCBWithOffset(locations, s.Gen8a.Met6, 60000, Locations8a.Met6);

        // Add in the BDSP+PLA magic met locations.
        locations.Add(new ComboItem($"{s.EggName} (HOME)", LocationsHOME.SWSHEgg));
        locations.Add(new ComboItem(s.gamelist[(int)BD], LocationsHOME.SWBD));
        locations.Add(new ComboItem(s.gamelist[(int)SP], LocationsHOME.SHSP));
        locations.Add(new ComboItem(s.gamelist[(int)PLA], LocationsHOME.SWLA));

        return locations;
    }

    private static List<ComboItem> CreateGen8b(GameStrings s)
    {
        // Manually add invalid (-1) location from SW/SH as ID 65535
        var locations = new List<ComboItem> { new(s.Gen8.Met0[0], Locations.Default8bNone) };
        Util.AddCBWithOffset(locations, s.Gen8b.Met6, 60000, Locations.Daycare5);
        Util.AddCBWithOffset(locations, s.Gen8b.Met3, 30000, Locations.LinkTrade6);
        Util.AddCBWithOffset(locations, s.Gen8b.Met0, 00000, Locations8b.Met0);
        Util.AddCBWithOffset(locations, s.Gen8b.Met3, 30000, Locations8b.Met3);
        Util.AddCBWithOffset(locations, s.Gen8b.Met4, 40000, Locations8b.Met4);
        Util.AddCBWithOffset(locations, s.Gen8b.Met6, 60000, Locations8b.Met6);
        return locations;
    }

    private static List<ComboItem> CreateGen9(GameStrings s)
    {
        var locations = Util.GetCBList(s.Gen9.Met0, 0);
        Util.AddCBWithOffset(locations, s.Gen9.Met6, 60000, Locations.Daycare5);
        Util.AddCBWithOffset(locations, s.Gen9.Met3, 30000, Locations.LinkTrade6);
        Util.AddCBWithOffset(locations, s.Gen9.Met0, 00000, Locations9.Met0);
        Util.AddCBWithOffset(locations, s.Gen9.Met3, 30000, Locations9.Met3);
        Util.AddCBWithOffset(locations, s.Gen9.Met4, 40000, Locations9.Met4);
        Util.AddCBWithOffset(locations, s.Gen9.Met6, 60000, Locations9.Met6);
        return locations;
    }

    /// <summary>
    /// Fetches a Met Location list for a <see cref="version"/> that has been transferred away from and overwritten.
    /// </summary>
    /// <param name="version">Origin version</param>
    /// <param name="context">Current format context</param>
    /// <param name="egg">True if an egg location list, false if a regular met location list</param>
    /// <returns>Met location list</returns>
    public IReadOnlyList<ComboItem> GetLocationList(GameVersion version, EntityContext context, bool egg = false)
    {
        if (context == EntityContext.Gen2)
            return MetGen2;

        IReadOnlyList<ComboItem> result;
        if (egg && version < W && context.Generation() >= 5)
            result = MetGen4;
        else
            result = GetLocationListInternal(version, context);

        // Insert the BD/SP none location if the format requires it.
        if (context is EntityContext.Gen8b && !BDSP.Contains(version))
        {
            var bdsp = new ComboItem[result.Count + 1];
            var none = bdsp[0] = result[0];
            bdsp[1] = new ComboItem($"{none.Text} (BD/SP)", Locations.Default8bNone);
            var dest = bdsp.AsSpan(2);
            if (result is ComboItem[] arr)
                arr.AsSpan(1).CopyTo(dest);
            else if (result is List<ComboItem> list)
                CollectionsMarshal.AsSpan(list)[1..].CopyTo(dest);
            return bdsp;
        }

        return result;
    }

    private IReadOnlyList<ComboItem> GetLocationListInternal(GameVersion version, EntityContext context) => version switch
    {
        CXD      when context == EntityContext.Gen3 => MetGen3CXD,
        R or S   when context == EntityContext.Gen3 => Partition1(MetGen3, IsMetLocation3RS),
        E        when context == EntityContext.Gen3 => Partition1(MetGen3, IsMetLocation3E),
        FR or LG when context == EntityContext.Gen3 => Partition1(MetGen3, IsMetLocation3FRLG),
        D or P   when context == EntityContext.Gen4 => Partition2(MetGen4, IsMetLocation4DP, 4),
        Pt       when context == EntityContext.Gen4 => Partition2(MetGen4, IsMetLocation4Pt, 4),
        HG or SS when context == EntityContext.Gen4 => Partition2(MetGen4, IsMetLocation4HGSS, 4),

        B  or W  => Partition2(MetGen5, IsMetLocation5BW), // Abyssal Ruins
        B2 or W2 => MetGen5,

        X  or Y  => Partition2(MetGen6, IsMetLocation6XY),
        OR or AS => Partition2(MetGen6, IsMetLocation6AO),
        SN or MN => Partition2(MetGen7, IsMetLocation7SM),
        US or UM
           or RD or BU or GN or YW
           or GD or SI or C => Partition2(MetGen7, IsMetLocation7USUM),

        GP or GE or GO => Partition2(MetGen7GG, IsMetLocation7GG),
        SW or SH => Partition2(MetGen8, IsMetLocation8SWSH),
        BD or SP => Partition2(MetGen8b, IsMetLocation8BDSP),
        PLA      => Partition2(MetGen8a, IsMetLocation8LA),
        SL or VL => Partition2(MetGen9, IsMetLocation9SV),
        _ => GetLocationListModified(version, context),
    };

    private static ComboItem[] Partition1(List<ComboItem> list, Func<ushort, bool> criteria)
    {
        var span = CollectionsMarshal.AsSpan(list);
        var result = new ComboItem[list.Count];
        ReorderList(span, result, criteria);
        return result;
    }

    private static ComboItem[] Partition2(List<ComboItem> list, Func<ushort, bool> criteria, int keepFirst = 3)
    {
        var span = CollectionsMarshal.AsSpan(list);
        var result = new ComboItem[span.Length];
        for (int i = 0; i < keepFirst; i++)
            result[i] = list[i];
        ReorderList(span, result, criteria, keepFirst);
        return result;
    }

    private static void ReorderList(Span<ComboItem> list, Span<ComboItem> result, Func<ushort, bool> criteria, int start = 0)
    {
        // store values that match criteria at the next available position of the array
        // store non-matches starting at the end. reverse before returning
        int end = list.Length - 1;
        for (var index = start; index < list.Length; index++)
        {
            var item = list[index];
            if (criteria((ushort)item.Value))
                result[start++] = item;
            else
                result[end--] = item;
        }

        // since the non-matches are reversed in order, we reverse that section.
        result[start..].Reverse();
    }

    /// <summary>
    /// Fetches a Met Location list for a <see cref="version"/> that has been transferred away from and overwritten.
    /// </summary>
    /// <param name="version">Origin version</param>
    /// <param name="context">Current format context</param>
    /// <returns>Met location list</returns>
    private IReadOnlyList<ComboItem> GetLocationListModified(GameVersion version, EntityContext context) => version switch
    {
        <= CXD when context == EntityContext.Gen4 => MetGen4Transfer ??= CreateGen4Transfer(),
        < X when context.Generation() >= 5 => MetGen5Transfer ??= CreateGen5Transfer(),
        _ => [],
    };
}
