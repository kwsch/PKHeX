using System;
using System.Collections.Generic;

namespace PKHeX.Core;

public sealed class FashionUnlock8 : SaveBlock<SAV8SWSH>
{
    private const int SIZE_ENTRY = 0x80;
    private const int REGIONS = 15;

    public FashionUnlock8(SAV8SWSH sav, SCBlock block) : base(sav, block.Data) { }

    public bool[] GetArrayOwnedFlag(int region) => ArrayUtil.GitBitFlagArray(Data.AsSpan(region * SIZE_ENTRY), SIZE_ENTRY * 8);
    public bool[] GetArrayNewFlag(int region) => ArrayUtil.GitBitFlagArray(Data.AsSpan((region + REGIONS) * SIZE_ENTRY), SIZE_ENTRY * 8);
    public int[] GetIndexesOwnedFlag(int region) => GetIndexes(GetArrayOwnedFlag(region));
    public int[] GetIndexesNewFlag(int region) => GetIndexes(GetArrayNewFlag(region));

    public void SetArrayOwnedFlag(int region, Span<bool> value) => ArrayUtil.SetBitFlagArray(Data.AsSpan(region * SIZE_ENTRY), value);
    public void SetArrayNewFlag(int region, Span<bool> value) => ArrayUtil.SetBitFlagArray(Data.AsSpan((region + REGIONS) * SIZE_ENTRY), value);
    public void SetIndexesOwnedFlag(int region, ReadOnlySpan<int> value) => SetArrayOwnedFlag(region, SetIndexes(value));
    public void SetIndexesNewFlag(int region, ReadOnlySpan<int> value) => SetArrayNewFlag(region, SetIndexes(value));

    public static int[] GetIndexes(ReadOnlySpan<bool> arr)
    {
        var list = new List<int>();
        for (int i = 0; i < arr.Length; i++)
        {
            if (arr[i])
                list.Add(i);
        }
        return list.ToArray();
    }

    public static bool[] SetIndexes(ReadOnlySpan<int> arr)
    {
        var max = GetMax(arr);
        var result = new bool[max];
        foreach (var index in arr)
            result[index] = true;
        return result;
    }

    private static int GetMax(ReadOnlySpan<int> arr)
    {
        int max = -1;
        foreach (var x in arr)
        {
            if (x > max)
                max = x;
        }
        return max;
    }
}
