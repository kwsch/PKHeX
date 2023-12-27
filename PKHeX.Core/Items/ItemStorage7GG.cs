using System;

namespace PKHeX.Core;

public sealed class ItemStorage7GG : IItemStorage
{
    public static readonly ItemStorage7GG Instance = new();

    private static ReadOnlySpan<ushort> Pouch_Candy_GG =>
    [
        050, // Rare Candy
        960, 961, 962, 963, 964, 965, // S
        966, 967, 968, 969, 970, 971, // L
        972, 973, 974, 975, 976, 977, // XL

        // Species
        978, 979,
        980, 981, 982, 983, 984, 985, 986, 987, 988, 989,
        990, 991, 992, 993, 994, 995, 996, 997, 998, 999,
        1000, 1001, 1002, 1003, 1004, 1005, 1006, 1007, 1008, 1009,
        1010, 1011, 1012, 1013, 1014, 1015, 1016, 1017, 1018, 1019,
        1020, 1021, 1022, 1023, 1024, 1025, 1026, 1027, 1028, 1029,
        1030, 1031, 1032, 1033, 1034, 1035, 1036, 1037, 1038, 1039,
        1040, 1041, 1042, 1043, 1044, 1045, 1046, 1047, 1048, 1049,
        1050, 1051, 1052, 1053, 1054, 1055, 1056,
        1057,
    ];

    private static ReadOnlySpan<ushort> Pouch_Medicine_GG =>
    [
        017, 018, 019, 020, 021, 022, 023, 024, 025, 026, 027, 028, 029, 030, 031, 032, 038, 039, 040, 041, 709, 903,
    ];

    private static ReadOnlySpan<ushort> Pouch_TM_GG =>
    [
        328, 329, 330, 331, 332, 333, 334, 335, 336, 337,
        338, 339, 340, 341, 342, 343, 344, 345, 346, 347,
        348, 349, 350, 351, 352, 353, 354, 355, 356, 357,
        358, 359, 360, 361, 362, 363, 364, 365, 366, 367,
        368, 369, 370, 371, 372, 373, 374, 375, 376, 377,
        378, 379, 380, 381, 382, 383, 384, 385, 386, 387,
    ];

    private static ReadOnlySpan<ushort> Pouch_PowerUp_GG =>
    [
        051, 053, 081, 082, 083, 084, 085,
        849,
    ];

    private static ReadOnlySpan<ushort> Pouch_Catching_GG =>
    [
        001, 002, 003, 004, 012, 164, 166, 168,
        861, 862, 863, 864, 865, 866,
    ];

    private static ReadOnlySpan<ushort> Pouch_Battle_GG =>
    [
        055, 056, 057, 058, 059, 060, 061, 062,
        656, 659, 660, 661, 662, 663, 671, 672, 675, 676, 678, 679,
        760, 762, 770, 773,
    ];

    private static ReadOnlySpan<ushort> Pouch_Regular_GG =>
    [
        076, 077, 078, 079, 086, 087, 088, 089,
        090, 091, 092, 093, 101, 102, 103, 113, 115,
        121, 122, 123, 124, 125, 126, 127, 128,
        442,
        571,
        632, 651,
        795, 796,
        872, 873, 874, 875, 876, 877, 878, 885, 886, 887, 888, 889, 890, 891, 892, 893, 894, 895, 896, 900, 901, 902,
    ];

    internal static ReadOnlySpan<ushort> Pouch_Regular_GG_Key =>
    [
        113, // Tea
        115, // Autograph
        121, // Pokémon Box
        122, // Medicine Pocket
        123, // TM Case
        124, // Candy Jar
        125, // Power-Up Pocket
        126, // Clothing Trunk
        127, // Catching Pocket
        128, // Battle Pocket
        442, // Town Map
        632, // Shiny Charm
        651, // Poké Flute

        872, // Secret Key
        873, // S.S. Ticket
        874, // Silph Scope
        875, // Parcel
        876, // Card Key
        877, // Gold Teeth
        878, // Lift Key
        885, // Stretchy Spring
        886, // Chalky Stone
        887, // Marble
        888, // Lone Earring
        889, // Beach Glass
        890, // Gold Leaf
        891, // Silver Leaf
        892, // Polished Mud Ball
        893, // Tropical Shell
        894, // Leaf Letter (P)
        895, // Leaf Letter (E)
        896, // Small Bouquet
    ];

    public bool IsLegal(InventoryType type, int itemIndex, int itemCount) => true;

    public ReadOnlySpan<ushort> GetItems(InventoryType type) => type switch
    {
        InventoryType.Medicine => Pouch_Medicine_GG,
        InventoryType.TMHMs => Pouch_TM_GG,
        InventoryType.Balls => Pouch_Catching_GG,
        InventoryType.Items => Pouch_Regular_GG,
        InventoryType.BattleItems => Pouch_Battle_GG,
        InventoryType.ZCrystals => Pouch_PowerUp_GG,
        InventoryType.Candy => Pouch_Candy_GG,
        _ => throw new ArgumentOutOfRangeException(nameof(type), type, null),
    };
}
