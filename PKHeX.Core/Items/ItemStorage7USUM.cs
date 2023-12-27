using System;
using static PKHeX.Core.ItemStorage7SM;

namespace PKHeX.Core;

public sealed class ItemStorage7USUM : IItemStorage
{
    public static readonly ItemStorage7USUM Instance = new();

    private static ReadOnlySpan<ushort> Pouch_Key_USUM =>
    [
        216,
        440, // US/UM
        465, 466, 628, 629, 631, 632, 638,
        705, 706, 765, 773, 797,
        841, 842, 843, 845, 847, 850, 857, 858, 860,
        // US/UM
        933, 934, 935, 936, 937, 938, 939, 940, 941, 942, 943, 944, 945, 946, 947, 948,
    ];

    private static ReadOnlySpan<ushort> Pouch_Roto_USUM =>
    [
        949, 950, 951, 952, 953, 954, 955, 956, 957, 958, 959,
    ];

    internal static ReadOnlySpan<ushort> Pouch_ZCrystal_USUM =>
    [
        // SM
        807, 808, 809, 810, 811, 812, 813, 814, 815, 816, 817, 818, 819, 820, 821, 822, 823, 824, 825, 826, 827, 828, 829, 830, 831, 832, 833, 834, 835,
        // US/UM
        927, 928, 929, 930, 931, 932,
    ];

    private static ReadOnlySpan<ushort> Pouch_ZCrystalHeld_USUM =>
    [
        // SM
        776, 777, 778, 779, 780, 781, 782, 783, 784, 785, 786, 787, 788, 789, 790, 791, 792, 793, 794, 798, 799, 800, 801, 802, 803, 804, 805, 806, 836,
        // US/UM Additions
        921, 922, 923, 924, 925, 926,
    ];

    public static ushort[] GetAllHeld() => [..Pouch_Regular_SM, ..Pouch_Berries_SM, ..Pouch_Medicine_SM, ..Pouch_ZCrystalHeld_USUM, ..Pouch_Roto_USUM];

    public static bool GetCrystalHeld(ushort itemKey, out ushort itemHeld)
    {
        var index = Pouch_ZCrystal_USUM.IndexOf(itemKey);
        if (index < 0)
        {
            itemHeld = 0;
            return false;
        }
        itemHeld = Pouch_ZCrystalHeld_USUM[index];
        return true;
    }

    public static bool GetCrystalKey(ushort itemHeld, out ushort itemKey)
    {
        var index = Pouch_ZCrystalHeld_USUM.IndexOf(itemHeld);
        if (index < 0)
        {
            itemKey = 0;
            return false;
        }
        itemKey = Pouch_ZCrystal_USUM[index];
        return true;
    }
    public bool IsLegal(InventoryType type, int itemIndex, int itemCount)
    {
        if (type is InventoryType.KeyItems or InventoryType.ZCrystals)
            return true;

        var items = GetItems(type);
        if (items.BinarySearch((ushort)itemIndex) < 0)
            return false;

        return Unreleased.BinarySearch((ushort)itemIndex) < 0;
    }

    public ReadOnlySpan<ushort> GetItems(InventoryType type) => type switch
    {
        InventoryType.Medicine => Pouch_Medicine_SM,
        InventoryType.Items => Pouch_Regular_SM,
        InventoryType.TMHMs => Pouch_TMHM_SM,
        InventoryType.Berries => Pouch_Berries_SM,
        InventoryType.KeyItems => Pouch_Key_USUM,
        InventoryType.ZCrystals => Pouch_ZCrystal_USUM,
        InventoryType.BattleItems => Pouch_Roto_USUM,
        _ => throw new ArgumentOutOfRangeException(nameof(type), type, null),
    };
}
