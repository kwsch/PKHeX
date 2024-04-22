using System;
using System.Collections.Generic;

namespace PKHeX.Core;

public sealed class FashionUnlock8(SAV8SWSH sav, SCBlock block) : SaveBlock<SAV8SWSH>(sav, block.Data)
{
    private const int SIZE_ENTRY = 0x80;
    private const int REGIONS = 15;

    public const int REGION_EYEWEAR   =  6;
    public const int REGION_HEADWEAR  =  7;
    public const int REGION_OUTERWEAR =  8;
    public const int REGION_TOPS      =  9;
    public const int REGION_BAGS      = 10;
    public const int REGION_GLOVES    = 11;
    public const int REGION_BOTTOMS   = 12;
    public const int REGION_LEGWEAR   = 13;
    public const int REGION_FOOTWEAR  = 14;

    private Span<byte> GetOwnedRegion(int region) => Data.Slice(region * SIZE_ENTRY, SIZE_ENTRY);
    private Span<byte> GetNewRegion(int region) => Data.Slice((region + REGIONS) * SIZE_ENTRY, SIZE_ENTRY);

    public bool[] GetArrayOwnedFlag(int region) => FlagUtil.GetBitFlagArray(GetOwnedRegion(region), SIZE_ENTRY * 8);
    public bool[] GetArrayNewFlag(int region) => FlagUtil.GetBitFlagArray(GetNewRegion(region), SIZE_ENTRY * 8);
    public int[] GetIndexesOwnedFlag(int region) => GetIndexes(GetArrayOwnedFlag(region));
    public int[] GetIndexesNewFlag(int region) => GetIndexes(GetArrayNewFlag(region));

    public void SetArrayOwnedFlag(int region, Span<bool> value) => FlagUtil.SetBitFlagArray(GetOwnedRegion(region), value);
    public void SetArrayNewFlag(int region, Span<bool> value) => FlagUtil.SetBitFlagArray(GetNewRegion(region), value);
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
        return [.. list];
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
