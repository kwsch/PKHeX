using System;
using static PKHeX.Core.ModelBR;
using static PKHeX.Core.GearCategory;

namespace PKHeX.Core;

/// <summary>
/// Flags for unlockable gear for Battle Passes in Pokémon Battle Revolution.
/// </summary>
public sealed class GearUnlock(Memory<byte> raw)
{
    public const int Offset = 0x584EC;
    public const int Size = 6 * 0x20;
    public const int CategoryCount = 10;

    public Span<byte> Data => raw.Span;
    public bool Get(int index) => FlagUtil.GetFlag(Data, index);
    public void Set(int index, bool value = true) => FlagUtil.SetFlag(Data, index, value);

    /// <summary>
    /// Resets all gear to the default.
    /// </summary>
    public void Clear()
    {
        Data.Clear();
        for (ModelBR model = YoungBoy; model <= LittleGirl; model++)
        {
            for (GearCategory category = Head; category <= Badges; category++)
            {
                var (offset, _, @default) = Info(model, category);
                Set(offset + @default);
            }
        }
    }

    /// <summary>
    /// Unlocks all gear.
    /// </summary>
    public void UnlockAll()
    {
        Data.Clear();
        for (ModelBR model = YoungBoy; model <= LittleGirl; model++)
        {
            for (GearCategory category = Head; category <= Badges; category++)
            {
                var (offset, count) = GetOffsetCount(model, category);
                for (int i = 0; i < count; i++)
                    Set(offset + i);
            }
        }
    }

    public static (ushort Offset, byte Count) GetOffsetCount(ModelBR model, GearCategory category) {
        var info = Info(model, category);
        return (info.Offset, info.Count);
    }

    public static int GetDefault(ModelBR model, GearCategory category) => Info(model, category).Default;

    private static (ushort Offset, byte Count, byte Default) Info(ModelBR model, GearCategory category) => model switch
    {
        YoungBoy => category switch
        {
            Head => (0x000, 17, 0), // Summer Cap A
            Hair => (0x011, 8, 0), // Chestnut
            Face => (0x019, 11, 0), // Take Off
            Top => (0x024, 18, 0), // Summer T Shirt A
            Bottom => (0x036, 18, 0), // Summer Jeans A
            Shoes => (0x048, 14, 0), // Summer Shoes A
            Hands => (0x056, 17, 1), // Summer Wristband A
            Bags => (0x067, 17, 1), // Summer Bag A
            Glasses => (0x078, 9, 0), // Take Off
            Badges => (0x081, 17, 0), // Take Off
            _ => throw new ArgumentOutOfRangeException(nameof(category), category, null),
        },
        CoolBoy => category switch
        {
            Head => (0x100, 17, 0), // Cool Hat A
            Hair => (0x111, 8, 0), // Light Blue
            Face => (0x119, 11, 0), // Take Off
            Top => (0x124, 18, 0), // Cool Jacket A
            Bottom => (0x136, 18, 0), // Cool Pants A
            Shoes => (0x148, 14, 0), // Cool Shoes A
            Hands => (0x156, 9, 0), // Take Off
            Bags => (0x15F, 5, 0), // Take Off
            Glasses => (0x164, 9, 0), // Take Off
            Badges => (0x16D, 1, 0), // Take Off
            _ => throw new ArgumentOutOfRangeException(nameof(category), category, null),
        },
        MuscleMan => category switch
        {
            Head => (0x200, 14, 0), // Spiky Hair
            Hair => (0x20E, 8, 0), // Beige
            Face => (0x216, 10, 0), // Take Off
            Top => (0x220, 18, 0), // Army Tank Top A
            Bottom => (0x232, 18, 0), // Army Pants A
            Shoes => (0x244, 14, 0), // Army Boots A
            Hands => (0x252, 17, 1), // Army Wristband A
            Bags => (0x263, 17, 1), // Army Bag A
            Glasses => (0x274, 9, 0), // Take Off
            Badges => (0x27D, 1, 0), // Take Off
            _ => throw new ArgumentOutOfRangeException(nameof(category), category, null),
        },
        YoungGirl => category switch
        {
            Head => (0x300, 17, 0), // Girls' Cap A
            Hair => (0x311, 8, 0), // Sky Blue
            Face => (0x319, 11, 0), // Take Off
            Top => (0x324, 18, 0), // Girls' Minidress A
            Bottom => (0x336, 18, 0), // Girls' Shorts A
            Shoes => (0x348, 14, 0), // Girls' Shoes A
            Hands => (0x356, 17, 1), // Girls' Wristband A
            Bags => (0x367, 17, 1), // Girls' Purse A
            Glasses => (0x378, 9, 0), // Take Off
            Badges => (0x381, 1, 0), // Take Off
            _ => throw new ArgumentOutOfRangeException(nameof(category), category, null),
        },
        CoolGirl => category switch
        {
            Head => (0x400, 17, 0), // Cool Cap A
            Hair => (0x411, 8, 0), // Purple
            Face => (0x419, 11, 0), // Take Off
            Top => (0x424, 18, 0), // Cool Tank Top A
            Bottom => (0x436, 18, 0), // Cool Pants A
            Shoes => (0x448, 14, 0), // Cool Sandals A
            Hands => (0x456, 17, 1), // Cool Bangle A
            Bags => (0x467, 7, 0), // Take Off
            Glasses => (0x46E, 15, 0), // Take Off
            Badges => (0x47D, 1, 0), // Take Off
            _ => throw new ArgumentOutOfRangeException(nameof(category), category, null),
        },
        LittleGirl => category switch
        {
            Head => (0x500, 17, 0), // Cute Headband A
            Hair => (0x511, 8, 0), // Pink
            Face => (0x519, 11, 0), // Take Off
            Top => (0x524, 18, 0), // Cute Dress A
            Bottom => (0x536, 2, 0), // Unused
            Shoes => (0x538, 14, 0), // Cute Shoes A
            Hands => (0x546, 17, 1), // Cute Bracelet A
            Bags => (0x557, 9, 0), // Take Off
            Glasses => (0x560, 13, 0), // Take Off
            Badges => (0x56D, 1, 0), // Take Off
            _ => throw new ArgumentOutOfRangeException(nameof(category), category, null),
        },
        _ => throw new ArgumentOutOfRangeException(nameof(model), model, null),
    };
}
