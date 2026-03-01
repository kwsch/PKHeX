using System;
using System.Collections.Generic;
using static PKHeX.Core.InventoryType;

namespace PKHeX.Core;

public sealed class PlayerBag5BW : PlayerBag
{
    public override IReadOnlyList<InventoryPouch4> Pouches { get; } = GetPouches(ItemStorage5BW.Instance);
    public override ItemStorage5BW Info => ItemStorage5BW.Instance;

    private static InventoryPouch4[] GetPouches(ItemStorage5BW info) =>
    [
        new(0x000, 999, info, Items), // 0
        new(0x4D8, 001, info, KeyItems), // 1
        new(0x624, 001, info, TMHMs), // 2
        new(0x7D8, 999, info, Medicine), // 3
        new(0x898, 999, info, Berries), // 4
    ];

    public PlayerBag5BW(SAV5BW sav) : this(sav.Items) { }
    public PlayerBag5BW(MyItem5BW block) : this(block.Data) { }
    public PlayerBag5BW(ReadOnlySpan<byte> data) => Pouches.LoadAll(data);

    public override void CopyTo(SaveFile sav) => CopyTo((SAV5BW)sav);
    public void CopyTo(SAV5BW sav) => CopyTo(sav.Items);
    public void CopyTo(MyItem5BW block) => CopyTo(block.Data);
    public void CopyTo(Span<byte> data) => Pouches.SaveAll(data);
}
