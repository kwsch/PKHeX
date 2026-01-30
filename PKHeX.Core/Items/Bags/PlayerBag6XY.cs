using System;
using System.Collections.Generic;

namespace PKHeX.Core;

public sealed class PlayerBag6XY : PlayerBag
{
    public override ItemStorage6XY Info => ItemStorage6XY.Instance;
    public override IReadOnlyList<InventoryPouch4> Pouches { get; } = GetPouches(ItemStorage6XY.Instance);

    private static InventoryPouch4[] GetPouches(ItemStorage6XY info) =>
    [
        new(0x000, 999, info, InventoryType.Items), // 0
        new(0x640, 001, info, InventoryType.KeyItems), // 1
        new(0x7C0, 001, info, InventoryType.TMHMs), // 2
        new(0x968, 999, info, InventoryType.Medicine), // 3
        new(0xA68, 999, info, InventoryType.Berries), // 4
    ];

    public PlayerBag6XY(SAV6XY sav) : this(sav.Items.Data) { }
    public PlayerBag6XY(Span<byte> data) => Pouches.LoadAll(data);

    public override void CopyTo(SaveFile sav) => CopyTo((SAV6XY)sav);
    public void CopyTo(SAV6XY sav) => CopyTo(sav.Items.Data);
    public void CopyTo(Span<byte> data) => Pouches.SaveAll(data);
}
