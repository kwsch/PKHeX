using System;
using System.Collections.Generic;
using static PKHeX.Core.InventoryType;

namespace PKHeX.Core;

public sealed class PlayerBag6XY : PlayerBag
{
    public override ItemStorage6XY Info => ItemStorage6XY.Instance;
    public override IReadOnlyList<InventoryPouch4> Pouches { get; } = GetPouches(ItemStorage6XY.Instance);

    private static InventoryPouch4[] GetPouches(ItemStorage6XY info) =>
    [
        new(0x000, 999, info, Items), // 0
        new(0x640, 001, info, KeyItems), // 1
        new(0x7C0, 001, info, TMHMs), // 2
        new(0x968, 999, info, Medicine), // 3
        new(0xA68, 999, info, Berries), // 4
    ];

    public PlayerBag6XY(SAV6XY sav) : this(sav.Items) { }
    public PlayerBag6XY(MyItem6XY block) : this(block.Data) { }
    public PlayerBag6XY(ReadOnlySpan<byte> data) => Pouches.LoadAll(data);

    public override void CopyTo(SaveFile sav) => CopyTo((SAV6XY)sav);
    public void CopyTo(SAV6XY sav) => CopyTo(sav.Items);
    public void CopyTo(MyItem6XY block) => CopyTo(block.Data);
    public void CopyTo(Span<byte> data) => Pouches.SaveAll(data);
}
