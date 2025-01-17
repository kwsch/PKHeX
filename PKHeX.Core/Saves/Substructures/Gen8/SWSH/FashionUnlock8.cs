using System;
using System.Collections.Generic;

namespace PKHeX.Core;

public sealed class FashionUnlock8(SAV8SWSH sav, SCBlock block) : SaveBlock<SAV8SWSH>(sav, block.Raw)
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

    public void Clear()
    {
        Data[(REGION_EYEWEAR * SIZE_ENTRY)..(REGIONS * SIZE_ENTRY)].Clear();
        Data[((REGION_EYEWEAR + REGIONS) * SIZE_ENTRY)..(REGIONS * 2 * SIZE_ENTRY)].Clear();
    }

    /// <summary>
    /// Unlocks all fashion items.
    /// </summary>
    public void UnlockAll()
    {
        var countList = SAV.MyStatus.GenderAppearance == 0 ? CountM : CountF;
        for (int region = 6; region < REGIONS; region++)
        {
            var ownedRegion = GetOwnedRegion(region);
            var newRegion = GetNewRegion(region);
            var count = countList[region - 6];
            for (int i = 0; i < count; i++)
            {
                FlagUtil.SetFlag(ownedRegion, i, true); // owned
                FlagUtil.SetFlag(newRegion, i, true); // not new
            }
        }

        // Exclude invalid items.
        var invalidOffsetList = SAV.MyStatus.GenderAppearance == 0 ? InvalidFashionOffset_M : InvalidFashionOffset_F;
        foreach (var ofs in invalidOffsetList)
        {
            FlagUtil.SetFlag(GetOwnedRegion(ofs >> 8), ofs & 0xFF, false);
            FlagUtil.SetFlag(GetNewRegion(ofs >> 8), ofs & 0xFF, false);
        }
    }

    /// <summary>
    /// Unlocks all legal fashion items.
    /// </summary>
    public void UnlockAllLegal()
    {
        UnlockAll();

        // Exclude unobtainable items.
        var illegalOffsetList = SAV.MyStatus.GenderAppearance == 0 ? IllegalFashionOffset_M : IllegalFashionOffset_F;
        foreach (var ofs in illegalOffsetList)
        {
            FlagUtil.SetFlag(GetOwnedRegion(ofs >> 8), ofs & 0xFF, false);
            FlagUtil.SetFlag(GetNewRegion(ofs >> 8), ofs & 0xFF, false);
        }

        // Exclude the Klara/Avery 4-ever Casual Tee for the opposite version.
        var versionOffset = GetIllegalCasualTee(SAV);
        FlagUtil.SetFlag(GetOwnedRegion(versionOffset >> 8), versionOffset & 0xFF, false);
        FlagUtil.SetFlag(GetNewRegion(versionOffset >> 8), versionOffset & 0xFF, false);
    }

    /// <summary>
    /// Resets the fashion unlocks to default values.
    /// </summary>
    public void Reset()
    {
        var offsetList = SAV.MyStatus.GenderAppearance == 0 ? DefaultFashionOffset_M : DefaultFashionOffset_F;
        foreach (var ofs in offsetList)
            FlagUtil.SetFlag(GetOwnedRegion(ofs >> 8), ofs & 0xFF, true);
    }

    private static ReadOnlySpan<byte> CountM => [097, 093, 070, 146, 057, 072, 094, 093, 070];
    private static ReadOnlySpan<byte> CountF => [097, 093, 087, 122, 065, 072, 162, 130, 080];

    private static ReadOnlySpan<ushort> DefaultFashionOffset_M => [0x616, 0x619, 0x72A, 0x919, 0x92A, 0x92D, 0x936, 0xA19, 0xB1C, 0xB1F, 0xC2D, 0xC3A, 0xD1C, 0xD29, 0xD2A, 0xE1D];
    private static ReadOnlySpan<ushort> DefaultFashionOffset_F => [0x616, 0x619, 0x724, 0x835, 0x918, 0x929, 0x92C, 0xA19, 0xB1C, 0xB1F, 0xC2D, 0xC7B, 0xD2A, 0xD2B, 0xD5A, 0xE1D];

    private static ushort GetIllegalCasualTee(SAV8SWSH sav) => sav switch
    {
        { Version: GameVersion.SW, MyStatus.GenderAppearance: 0 } => 0x976, // Casual Tee (Avery 4-ever)
        { Version: GameVersion.SH, MyStatus.GenderAppearance: 0 } => 0x975, // Casual Tee (Klara 4-ever)
        { Version: GameVersion.SW, MyStatus.GenderAppearance: 1 } => 0x96A, // Casual Tee (Avery 4-ever)
        { Version: GameVersion.SH, MyStatus.GenderAppearance: 1 } => 0x969, // Casual Tee (Klara 4-ever)
        _ => throw new ArgumentOutOfRangeException(nameof(sav)),
    };

    private static ReadOnlySpan<ushort> IllegalFashionOffset_M => [
        0x824, // Satin Varsity Jacket (Shocking Berry)
        0x95B, // Casual Tee (Chosen Design)
    ];
    private static ReadOnlySpan<ushort> IllegalFashionOffset_F => [
        0x827, // Satin Varsity Jacket (Shocking Berry)
        0x94F, // Casual Tee (Chosen Design)
    ];

    private static ReadOnlySpan<ushort> InvalidFashionOffset_M => [
        0x969, 0xB00, 0xC48, 0xC49, 0xC4A, 0xD57, 0xE37, 0xE38, 0xE39,
    ];
    private static ReadOnlySpan<ushort> InvalidFashionOffset_F => [
        0x95D, 0xB00, 0xD76, 0xD77, 0xE41, 0xE47,
    ];
}
