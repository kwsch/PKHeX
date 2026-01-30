using System;
using System.Collections.Generic;

namespace PKHeX.Core;

public sealed class PlayerBag5BW : PlayerBag
{
    public override IReadOnlyList<InventoryPouch4> Pouches { get; } = GetPouches(ItemStorage5BW.Instance);
    public override ItemStorage5BW Info => ItemStorage5BW.Instance;

    private static InventoryPouch4[] GetPouches(ItemStorage5BW info) =>
    [
        new(0x000, 999, info, InventoryType.Items), // 0
        new(0x4D8, 001, info, InventoryType.KeyItems), // 1
        new(0x624, 001, info, InventoryType.TMHMs), // 2
        new(0x7D8, 999, info, InventoryType.Medicine), // 3
        new(0x898, 999, info, InventoryType.Berries), // 4
    ];

    public PlayerBag5BW(SAV5BW sav) : this(sav.Items.Data) { }
    public PlayerBag5BW(ReadOnlySpan<byte> data) => Pouches.LoadAll(data);

    public override void CopyTo(SaveFile sav) => CopyTo((SAV5BW)sav);
    public void CopyTo(SAV5BW sav) => CopyTo(sav.Items.Data);
    public void CopyTo(Span<byte> data) => Pouches.SaveAll(data);
}
