using System;

namespace PKHeX.Core;

public sealed class ItemStorage8BDSP : IItemStorage
{
    public static readonly ItemStorage8BDSP Instance = new();

    private static ReadOnlySpan<ushort> Pouch_Regular_BS =>
    [
        045, 046, 047, 048, 049, 050, 051, 052, 053, 072, 073, 074, 075, 076, 077, 078,
        079, 080, 081, 082, 083, 084, 085, 093, 094, 107, 108, 109, 110, 111, 112, 135,
        136, 213, 214, 215, 217, 218, 219, 220, 221, 222, 223, 224, 225, 226, 227, 228,
        229, 230, 231, 232, 233, 234, 235, 236, 237, 238, 239, 240, 241, 242, 243, 244,
        245, 246, 247, 248, 249, 250, 251, 252, 253, 254, 255, 256, 257, 258, 259, 260,
        261, 262, 263, 264, 265, 266, 267, 268, 269, 270, 271, 272, 273, 274, 275, 276,
        277, 278, 279, 280, 281, 282, 283, 284, 285, 286, 287, 288, 289, 290, 291, 292,
        293, 294, 295, 296, 297, 298, 299, 300, 301, 302, 303, 304, 305, 306, 307, 308,
        309, 310, 311, 312, 313, 314, 315, 316, 317, 318, 319, 320, 321, 322, 323, 324,
        325, 326, 327, 537, 565, 566, 567, 568, 569, 570, 644, 645, 849,

        1231, 1232, 1233, 1234, 1235, 1236, 1237, 1238, 1239, 1240, 1241, 1242, 1243, 1244,
        1245, 1246, 1247, 1248, 1249, 1250, 1251, 1606,
    ];

    private static ReadOnlySpan<ushort> Pouch_Ball_BS =>
    [
        001, 002, 003, 004, 005, 006, 007, 008, 009, 010, 011, 012, 013, 014, 015, 016,
        492, 493, 494, 495, 496, 497, 498, 499, 500,
        576,
        851,
    ];

    private static ReadOnlySpan<ushort> Pouch_Battle_BS =>
    [
        055, 056, 057, 058, 059, 060, 061, 062, 063,
    ];

    internal static ReadOnlySpan<ushort> Unreleased =>
    [
        005, // Safari Ball
        016, // Cherish Ball
        044, // Sacred Ash
        499, // Sport Ball
        500, // Park Ball
        537, // Prism Scale
        565, // Health Feather
        566, // Muscle Feather
        567, // Resist Feather
        568, // Genius Feather
        569, // Clever Feather
        570, // Swift Feather
        576, // Dream Ball
        849, // Ice Stone
        851, // Beast Ball
    ];

    internal static ReadOnlySpan<ushort> DisallowHeldTreasure =>
    [
        // new BD/SP items, but they can't be held
        1808, // Mysterious Shard S
        1809, // Mysterious Shard L
        1810, // Digger Drill
        1811, // Kanto Slate
        1812, // Johto Slate
        1813, // Soul Slate
        1814, // Rainbow Slate
        1815, // Squall Slate
        1816, // Oceanic Slate
        1817, // Tectonic Slate
        1818, // Stratospheric Slate
        1819, // Genome Slate
        1820, // Discovery Slate
        1821, // Distortion Slate
    ];

    private static ReadOnlySpan<ushort> Pouch_Key_BS =>
    [
        428, 431, 432, 433, 438, 439, 440, 443, 445, 446, 447, 448, 449, 450, 451, 452,
        453, 454, 455, 459, 460, 461, 462, 463, 464, 466, 467, 631, 632,

        1267, 1278, 1822,
    ];

    private static ReadOnlySpan<ushort> Pouch_Medicine_BS =>
    [
        017, 018, 019, 020, 021, 022, 023, 024, 025, 026, 027, 028, 029, 030, 031, 032, 033, 034, 035, 036, 037,
        038, 039, 040, 041, 042, 043, 044, 054,
    ];

    private static ReadOnlySpan<ushort> Pouch_Berries_BS =>
    [
        149, 150, 151, 152, 153, 154, 155, 156, 157, 158,
        159, 160, 161, 162, 163, 164, 165, 166, 167, 168,
        169, 170, 171, 172, 173, 174, 175, 176, 177, 178,
        179, 180, 181, 182, 183, 184, 185, 186, 187, 188,
        189, 190, 191, 192, 193, 194, 195, 196, 197, 198,
        199, 200, 201, 202, 203, 204, 205, 206, 207, 208,
        209, 210, 211, 212, 686,
    ];

    private static ReadOnlySpan<ushort> Pouch_Treasure_BS =>
    [
        086, 087, 088, 089, 090, 091, 092, 099, 100, 101, 102, 103, 104, 105, 106, 795, 796,

        1808, 1809, 1810, 1811, 1812, 1813, 1814, 1815, 1816, 1817, 1818, 1819, 1820, 1821,
    ];

    private static ReadOnlySpan<ushort> Pouch_TMHM_BS => // TM01-TM100
    [
        328, 329, 330, 331, 332, 333, 334, 335, 336, 337,
        338, 339, 340, 341, 342, 343, 344, 345, 346, 347,
        348, 349, 350, 351, 352, 353, 354, 355, 356, 357,
        358, 359, 360, 361, 362, 363, 364, 365, 366, 367,
        368, 369, 370, 371, 372, 373, 374, 375, 376, 377,
        378, 379, 380, 381, 382, 383, 384, 385, 386, 387,
        388, 389, 390, 391, 392, 393, 394, 395, 396, 397,
        398, 399, 400, 401, 402, 403, 404, 405, 406, 407,
        408, 409, 410, 411, 412, 413, 414, 415, 416, 417,
        418, 419,
        420, 421, 422, 423, 424, 425, 426, 427, // Previously called HM0X, in BD/SP they're now called TM93-TM100
    ];

    public int GetMax(InventoryType type) => type switch
    {
        InventoryType.Items => 999,
        InventoryType.KeyItems => 1,
        InventoryType.TMHMs => 999,
        InventoryType.Medicine => 999,
        InventoryType.Berries => 999,
        InventoryType.Balls => 999,
        InventoryType.BattleItems => 999,
        InventoryType.Treasure => 999,
        _ => throw new ArgumentOutOfRangeException(nameof(type)),
    };

    public ReadOnlySpan<ushort> GetItems(InventoryType type) => GetLegal(type);

    public static ReadOnlySpan<ushort> GetLegal(InventoryType type) => type switch
    {
        InventoryType.Items => Pouch_Regular_BS,
        InventoryType.KeyItems => Pouch_Key_BS,
        InventoryType.TMHMs => Pouch_TMHM_BS,
        InventoryType.Medicine => Pouch_Medicine_BS,
        InventoryType.Berries => Pouch_Berries_BS,
        InventoryType.Balls => Pouch_Ball_BS,
        InventoryType.BattleItems => Pouch_Battle_BS,
        InventoryType.Treasure => Pouch_Treasure_BS,
        _ => throw new ArgumentOutOfRangeException(nameof(type)),
    };

    internal static ReadOnlySpan<InventoryType> ValidTypes =>
    [
        InventoryType.Items, InventoryType.KeyItems, InventoryType.TMHMs, InventoryType.Medicine,
        InventoryType.Berries, InventoryType.Balls, InventoryType.BattleItems, InventoryType.Treasure,
    ];

    public static ushort[] GetAllHeld() => [..Pouch_Regular_BS, ..Pouch_Ball_BS, ..Pouch_Battle_BS, ..Pouch_Berries_BS, ..Pouch_TMHM_BS, ..Pouch_Medicine_BS, ..Pouch_Treasure_BS];

    public static InventoryType GetInventoryPouch(ushort itemIndex)
    {
        foreach (var type in ValidTypes)
        {
            var legal = GetLegal(type);
            if (legal.Contains(itemIndex))
                return type;
        }
        return InventoryType.None;
    }

    public bool IsLegal(InventoryType type, int itemIndex, int itemCount)
    {
        if (type is InventoryType.KeyItems)
            return true;

        return Unreleased.BinarySearch((ushort)itemIndex) < 0;
    }
}
