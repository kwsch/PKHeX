using System;
using static PKHeX.Core.ItemStorage3RS;

namespace PKHeX.Core;

/// <summary>
/// Item storage for <see cref="GameVersion.FR"/> and <see cref="GameVersion.LG"/>
/// </summary>
public sealed class ItemStorage3FRLG : IItemStorage
{
    public static readonly ItemStorage3FRLG Instance = new();

    public static ReadOnlySpan<ushort> Key =>
    [
        // R/S
        259, 260, 261, 262, 263, 264, 265, 266, 268, 269, 270, 271, 272, 273, 274, 275, 276, 277, 278, 279, 280, 281, 282, 283, 284, 285, 286, 287, 288,
        // FR/LG
        349, 350, 351, 352, 353, 354, 355, 356, 357, 358, 359, 360, 361, 362, 363, 364, 365, 366, 367, 368, 369, 370, 371, 372, 373, 374,
    ];

    public bool IsLegal(InventoryType type, int itemIndex, int itemCount) => !Unreleased.Contains((ushort)itemIndex);

    public ReadOnlySpan<ushort> GetItems(InventoryType type) => type switch
    {
        InventoryType.Items => General,
        InventoryType.KeyItems => Key,
        InventoryType.Balls => Balls,
        InventoryType.TMHMs => Machine,
        InventoryType.Berries => Berry,
        InventoryType.PCItems => General,
        _ => throw new ArgumentOutOfRangeException(nameof(type), type, null),
    };
}

/// <summary>
/// Item storage for <see cref="GameVersion.FR"/> and <see cref="GameVersion.LG"/> on <see cref="GameConsole.NX"/>.
/// </summary>
public sealed class ItemStorage3FRLG_VC : IItemStorage // TODO VC RSE: delete me and any usages as RSE gives the remainder of items.
{
    public static readonly ItemStorage3FRLG_VC Instance = new();
    public ReadOnlySpan<ushort> GetItems(InventoryType type) => ItemStorage3FRLG.Instance.GetItems(type);

    public bool IsLegal(InventoryType type, int itemIndex, int itemCount) => !IsUnreleasedHeld(itemIndex); // use VC unreleased list

    public static bool IsUnreleasedHeld(int itemIndex) => Unreleased.Contains((ushort)itemIndex);

    private static ReadOnlySpan<ushort> Unreleased =>
    [
        005, // Safari
        039,      041, 042, 043, // Flutes (Yellow is obtainable via Coins)
        044, // Berry Juice
        046, 047, // Shoal Salt, Shoal Shell
        048, 049, 050, 051, // Shards
        081, // Fluffy Tail

        121, 122, 123, 124, 125, 126, 127, 128, 129, // Mail

        168, // Liechi Berry (Mirage Island)
        169, // Ganlon Berry (Event)
        170, // Salac Berry (Event)
        171, // Petaya Berry (Event)
        172, // Apicot Berry (Event)
        173, // Lansat Berry (Event)
        174, // Starf Berry (Event)
        175, // Enigma Berry (Event)

        179, // BrightPowder
        180, // White Herb
        185, // Mental Herb
        186, // Choice Band
        191, // Soul Dew
        192, // DeepSeaTooth
        193, // DeepSeaScale

        198, // Scope Lens

        202, // Light Ball

        219, // Shell Bell

        254, 255, 256, 257, 258, 259, // Scarves
    ];
}
