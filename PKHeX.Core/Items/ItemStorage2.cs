using System;

namespace PKHeX.Core;

public sealed class ItemStorage2 : IItemStorage
{
    public static readonly ItemStorage2 InstanceGS = new(false);
    public static readonly ItemStorage2 InstanceC = new(true);

    private readonly bool Crystal;
    private ItemStorage2(bool crystal) => Crystal = crystal;

    private static ReadOnlySpan<ushort> Pouch_Items_GSC =>
    [
                       003,                     008, 009,
        010, 011, 012, 013, 014, 015, 016, 017, 018, 019,
        020, 021, 022, 023, 024,      026, 027, 028, 029,
        030, 031, 032, 033, 034, 035, 036, 037, 038, 039,
        040, 041, 042, 043, 044,      046, 047, 048, 049,
             051, 052, 053,                057,
        060,      062, 063, 064, 065,
                  072, 073, 074, 075, 076, 077, 078, 079,
        080, 081, 082, 083, 084, 085, 086, 087, 088, 089,
             091, 092, 093, 094, 095, 096, 097, 098, 099,
             101, 102, 103, 104, 105, 106, 107, 108, 109,
        110, 111, 112, 113, 114,           117, 118, 119,
             121, 122, 123, 124, 125, 126,
             131, 132,                          138, 139,
        140, 143, 144, 146,
        150, 151, 152,                156,      158,
                       163,                167, 168, 169,
        170,      172, 173, 174,
        180, 181, 182, 183, 184, 185, 186, 187, 188, 189,
    ];

    private static ReadOnlySpan<ushort> Pouch_Ball_GSC =>
    [
        1, 2, 4, 5, 157, 159, 160, 161, 164, 165, 166,
    ];

    private static ReadOnlySpan<ushort> Pouch_Key_GS =>
    [
        007, 054, 055, 058, 059, 061, 066, 067, 068, 069, 071, 127, 128, 130, 133, 134, 175, 178,
    ];

    private const int ExtraKeyCrystal = 4;

    private static ReadOnlySpan<ushort> Pouch_Key_C =>
    [
        007, 054, 055, 058, 059, 061, 066, 067, 068, 069, 071, 127, 128, 130, 133, 134, 175, 178,
        070, 115, 116, 129,
    ];

    private static ReadOnlySpan<ushort> Pouch_TMHM_GSC =>
    [
             191, 192, 193, 194,      196, 197, 198, 199,
        200, 201, 202, 203, 204, 205, 206, 207, 208, 209,
        210, 211, 212, 213, 214, 215, 216, 217, 218, 219,
             221, 222, 223, 224, 225, 226, 227, 228, 229,
        230, 231, 232, 233, 234, 235, 236, 237, 238, 239,
        240, 241, 242, 243, 244, 245, 246, 247, 248, 249,
    ];

    public static ushort[] GetAllHeld() => [..Pouch_Items_GSC, ..Pouch_Ball_GSC, ..Pouch_TMHM_GSC];

    private static readonly ushort[] PCItemsC  = [..Pouch_Items_GSC, ..Pouch_Ball_GSC, ..Pouch_TMHM_GSC, ..Pouch_Key_C];

    private static ReadOnlySpan<ushort> PCItemsGS => PCItemsC.AsSpan(..^ExtraKeyCrystal);

    public bool IsLegal(InventoryType type, int itemIndex, int itemCount) => true;

    public ReadOnlySpan<ushort> GetItems(InventoryType type) => type switch
    {

        InventoryType.Items => Pouch_Items_GSC,
        InventoryType.KeyItems => Crystal ? Pouch_Key_C : Pouch_Key_GS,
        InventoryType.TMHMs => Pouch_TMHM_GSC,
        InventoryType.Balls => Pouch_Ball_GSC,
        InventoryType.PCItems => Crystal ? PCItemsC : PCItemsGS,
        _ => throw new ArgumentOutOfRangeException(nameof(type), type, null),
    };
}
