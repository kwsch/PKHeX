using System;
using System.Collections.Generic;
using static PKHeX.Core.InventoryType;

namespace PKHeX.Core;

public sealed class PlayerBag5B2W2 : PlayerBag
{
    public override IReadOnlyList<InventoryPouch4> Pouches { get; } = GetPouches(ItemStorage5B2W2.Instance);
    public override ItemStorage5B2W2 Info => ItemStorage5B2W2.Instance;

    private static InventoryPouch4[] GetPouches(ItemStorage5B2W2 info) =>
    [
        new(0x000, 999, info, Items), // 0
        new(0x4D8, 001, info, KeyItems), // 1
        new(0x624, 001, info, TMHMs), // 2
        new(0x7D8, 999, info, Medicine), // 3
        new(0x898, 999, info, Berries), // 4
    ];

    public PlayerBag5B2W2(SAV5B2W2 sav) : this(sav.Items) { }
    public PlayerBag5B2W2(MyItem5B2W2 block) : this(block.Data) { }
    public PlayerBag5B2W2(ReadOnlySpan<byte> data) => Pouches.LoadAll(data);

    public override void CopyTo(SaveFile sav) => CopyTo((SAV5B2W2)sav);
    public void CopyTo(SAV5B2W2 sav) => CopyTo(sav.Items);
    public void CopyTo(MyItem5B2W2 block) => CopyTo(block.Data);
    public void CopyTo(Span<byte> data) => Pouches.SaveAll(data);
}
