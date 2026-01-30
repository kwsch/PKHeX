using System;
using System.Collections.Generic;

namespace PKHeX.Core;

public sealed class PlayerBag6AO : PlayerBag
{
    public override IReadOnlyList<InventoryPouch4> Pouches { get; } = GetPouches(ItemStorage6AO.Instance);
    public override ItemStorage6AO Info => ItemStorage6AO.Instance;

    private static InventoryPouch4[] GetPouches(ItemStorage6AO info) =>
    [
        new(0x000, 999, info, InventoryType.Items), // 0
        new(0x640, 001, info, InventoryType.KeyItems), // 1
        new(0x7C0, 001, info, InventoryType.TMHMs), // 2
        new(0x970, 999, info, InventoryType.Medicine), // 3, +2 items shift because 2 HMs added
        new(0xA70, 999, info, InventoryType.Berries), // 4
    ];

    public PlayerBag6AO(SAV6AO sav) : this(sav.Items.Data) { }
    public PlayerBag6AO(SAV6AODemo sav) : this(sav.Items.Data) { }
    public PlayerBag6AO(Span<byte> data) => Pouches.LoadAll(data);

    public override void CopyTo(SaveFile sav) => CopyTo((SAV6)sav);
    public void CopyTo(SAV6 sav) => CopyTo(sav.Items.Data);
    public void CopyTo(Span<byte> data) => Pouches.SaveAll(data);
}
