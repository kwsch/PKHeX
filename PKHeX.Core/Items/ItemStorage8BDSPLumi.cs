using System;

namespace PKHeX.Core;

public sealed class ItemStorage8BDSPLumi : IItemStorage
{
    public static readonly ItemStorage8BDSPLumi Instance = new();

    private static ReadOnlySpan<ushort> Pouch_Regular_Lumi => new ushort[]
    {
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

        // New to Luminescent
        1825,
        1834, 1836
    };

    private static ReadOnlySpan<ushort> Pouch_Ball_Lumi => new ushort[]
    {
        001, 002, 003, 004, 005, 006, 007, 008, 009, 010, 011, 012, 013, 014, 015, 016,
        492, 493, 494, 495, 496, 497, 498, 499, 500,
        576,
        851,
    };

    private static ReadOnlySpan<ushort> Pouch_Battle_Lumi => new ushort[]
    {
        055, 056, 057, 058, 059, 060, 061, 062, 063,
    };
    private static ReadOnlySpan<ushort> Pouch_Key_Lumi => new ushort[]
    {
        428, 431, 432, 433, 438, 439, 440, 443, 445, 446, 447, 448, 449, 450, 451, 452,
        453, 454, 455, 459, 460, 461, 462, 463, 464, 466, 467, 631, 632,

        1267, 1278, 1822,
        
        // New to Luminescent
        1823, 1824, 1832, 1833,
        1826, 1827, 1828, 1829, 1830, 1831,
        1835
    };

    private static ReadOnlySpan<ushort> Pouch_Medicine_Lumi => new ushort[]
    {
        017, 018, 019, 020, 021, 022, 023, 024, 025, 026, 027, 028, 029, 030, 031, 032, 033, 034, 035, 036, 037,
        038, 039, 040, 041, 042, 043, 044, 054,

        // 134 Sweet Heart (future event item?)
    };

    private static ReadOnlySpan<ushort> Pouch_Berries_Lumi => new ushort[]
    {
        149, 150, 151, 152, 153, 154, 155, 156, 157, 158,
        159, 160, 161, 162, 163, 164, 165, 166, 167, 168,
        169, 170, 171, 172, 173, 174, 175, 176, 177, 178,
        179, 180, 181, 182, 183, 184, 185, 186, 187, 188,
        189, 190, 191, 192, 193, 194, 195, 196, 197, 198,
        199, 200, 201, 202, 203, 204, 205, 206, 207, 208,
        209, 210, 211, 212, 686,
    };

    private static ReadOnlySpan<ushort> Pouch_Treasure_Lumi => new ushort[]
    {
        086, 087, 088, 089, 090, 091, 092, 099, 100, 101, 102, 103, 104, 105, 106, 795, 796,

        1808, 1809, 1810, 1811, 1812, 1813, 1814, 1815, 1816, 1817, 1818, 1819, 1820, 1821,
    };

    private static ReadOnlySpan<ushort> Pouch_TMHM_Lumi => new ushort[]
    {
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
        420, 421, 422, 423, 424, 425, 426, 427,
    };

    internal static ReadOnlySpan<ushort> Unreleased => new ushort[]
    {
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

        1835 // Infinite Repel (Lumi)
    };

    public ReadOnlySpan<ushort> GetItems(InventoryType type) => GetLegal(type);

    public static ReadOnlySpan<ushort> GetLegal(InventoryType type) => type switch
    {
        InventoryType.Items => Pouch_Regular_Lumi,
        InventoryType.KeyItems => Pouch_Key_Lumi,
        InventoryType.TMHMs => Pouch_TMHM_Lumi,
        InventoryType.Medicine => Pouch_Medicine_Lumi,
        InventoryType.Berries => Pouch_Berries_Lumi,
        InventoryType.Balls => Pouch_Ball_Lumi,
        InventoryType.BattleItems => Pouch_Battle_Lumi,
        InventoryType.Treasure => Pouch_Treasure_Lumi,
        _ => throw new ArgumentOutOfRangeException(nameof(type)),
    };

    internal static ReadOnlySpan<InventoryType> ValidTypes => new[]
    {
        InventoryType.Items, InventoryType.KeyItems, InventoryType.TMHMs, InventoryType.Medicine,
        InventoryType.Berries, InventoryType.Balls, InventoryType.BattleItems, InventoryType.Treasure,
    };

    // used in legality/core
    // not yet implemented
    public static ushort[] GetAll()
    {
        return ArrayUtil.ConcatAll(Pouch_Regular_Lumi, Pouch_Ball_Lumi, Pouch_Battle_Lumi, Pouch_Berries_Lumi, Pouch_TMHM_Lumi, Pouch_Medicine_Lumi, Pouch_Treasure_Lumi);
    }

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
