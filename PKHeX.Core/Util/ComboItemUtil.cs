using System;
using System.Collections.Generic;
using static PKHeX.Core.MessageStrings;
using static PKHeX.Core.GeonetPoint;

namespace PKHeX.Core;

public static partial class Util
{
    public static List<ComboItem> GetCountryRegionList(string textFile, string lang)
    {
        string[] inputCSV = GetStringList(textFile);
        int index = GeoLocation.GetLanguageIndex(lang);
        var list = GetCBListFromCSV(inputCSV, index);
        if (list.Count > 1)
            list.Sort(1, list.Count - 1, Comparer); // keep null value as first
        return list;
    }

    public static List<ComboItem> GetGeonetPointList() =>
    [
        new (MsgGeonetPointNone,   (int)None),
        new (MsgGeonetPointBlue,   (int)Blue),
        new (MsgGeonetPointYellow, (int)Yellow),
        new (MsgGeonetPointRed,    (int)Red),
    ];

    private static List<ComboItem> GetCBListFromCSV(ReadOnlySpan<string> inputCSV, int index)
    {
        var arr = new List<ComboItem>(inputCSV.Length);
        foreach (var line in inputCSV)
        {
            var span = line.AsSpan();
            // 3 digits index; 1 tab space, then the string entries.
            var value = int.Parse(span[..3]);
            var text = StringUtil.GetNthEntry(span[4..], index);
            var item = new ComboItem(new string(text), value);
            arr.Add(item);
        }
        return arr;
    }

    public static List<ComboItem> GetCBList(ReadOnlySpan<string> inStrings)
    {
        var list = new List<ComboItem>(inStrings.Length);
        for (int i = 0; i < inStrings.Length; i++)
            list.Add(new ComboItem(inStrings[i], i));
        list.Sort(Comparer);
        return list;
    }

    public static List<ComboItem> GetCBList(ReadOnlySpan<string> inStrings, ReadOnlySpan<ushort> allowed)
    {
        var list = new List<ComboItem>(allowed.Length + 1) { new(inStrings[0], 0) };
        foreach (var index in allowed)
            list.Add(new ComboItem(inStrings[index], index));
        list.Sort(Comparer);
        return list;
    }

    public static List<ComboItem> GetCBList(ReadOnlySpan<string> inStrings, int index, int offset = 0)
    {
        var list = new List<ComboItem>();
        AddCBWithOffset(list, inStrings, offset, index);
        return list;
    }

    public static ComboItem[] GetUnsortedCBList(ReadOnlySpan<string> inStrings, ReadOnlySpan<byte> allowed)
    {
        var count = allowed.Length;
        var list = new ComboItem[count];
        for (var i = 0; i < allowed.Length; i++)
        {
            var index = allowed[i];
            var item = new ComboItem(inStrings[index], index);
            list[i] = item;
        }

        return list;
    }

    public static void AddCBWithOffset(List<ComboItem> list, ReadOnlySpan<string> inStrings, int offset, int index)
    {
        var item = new ComboItem(inStrings[index - offset], index);
        list.Add(item);
    }

    public static void AddCBWithOffset(List<ComboItem> cbList, ReadOnlySpan<string> inStrings, int offset, ReadOnlySpan<byte> allowed)
    {
        int beginCount = cbList.Count;
        cbList.Capacity += allowed.Length;
        foreach (var index in allowed)
        {
            var item = new ComboItem(inStrings[index - offset], index);
            cbList.Add(item);
        }
        cbList.Sort(beginCount, allowed.Length, Comparer);
    }

    public static void AddCBWithOffset(List<ComboItem> cbList, ReadOnlySpan<string> inStrings, int offset, ReadOnlySpan<ushort> allowed)
    {
        int beginCount = cbList.Count;
        cbList.Capacity += allowed.Length;
        foreach (var index in allowed)
        {
            var item = new ComboItem(inStrings[index - offset], index);
            cbList.Add(item);
        }
        cbList.Sort(beginCount, allowed.Length, Comparer);
    }

    public static void AddCBWithOffset(List<ComboItem> cbList, ReadOnlySpan<string> inStrings, int offset)
    {
        int beginCount = cbList.Count;
        cbList.Capacity += inStrings.Length;
        for (int i = 0; i < inStrings.Length; i++)
        {
            var x = inStrings[i];
            var item = new ComboItem(x, i + offset);
            cbList.Add(item);
        }
        cbList.Sort(beginCount, inStrings.Length, Comparer);
    }

    public static ComboItem[] GetVariedCBListBall(ReadOnlySpan<string> itemNames, ReadOnlySpan<byte> ballIndex, ReadOnlySpan<ushort> ballItemID)
    {
        var list = new ComboItem[ballItemID.Length];
        for (int i = 0; i < ballItemID.Length; i++)
            list[i] = new ComboItem(itemNames[ballItemID[i]], ballIndex[i]);

        // 3 Balls are preferentially first, sort Master Ball with the rest Alphabetically.
        list.AsSpan(3).Sort(Comparer);
        return list;
    }

    private static readonly FunctorComparer<ComboItem> Comparer =
        new((a, b) => string.CompareOrdinal(a.Text, b.Text));

    private sealed class FunctorComparer<T>(Comparison<T> Comparison) : IComparer<T>
    {
        public int Compare(T? x, T? y)
        {
            if (x == null)
                return y == null ? 0 : -1;
            return y == null ? 1 : Comparison(x, y);
        }
    }
}
