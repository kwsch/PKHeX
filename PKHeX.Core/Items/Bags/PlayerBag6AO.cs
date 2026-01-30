using System;
using System.Collections.Generic;
using static PKHeX.Core.InventoryType;

namespace PKHeX.Core;

public sealed class PlayerBag6AO : PlayerBag
{
    public override IReadOnlyList<InventoryPouch4> Pouches { get; } = GetPouches(ItemStorage6AO.Instance);
    public override ItemStorage6AO Info => ItemStorage6AO.Instance;

    private static InventoryPouch4[] GetPouches(ItemStorage6AO info) =>
    [
        new(0x000, 999, info, Items), // 0
        new(0x640, 001, info, KeyItems), // 1
        new(0x7C0, 001, info, TMHMs), // 2
        new(0x970, 999, info, Medicine), // 3, +2 items shift because 2 HMs added
        new(0xA70, 999, info, Berries), // 4
    ];

    public PlayerBag6AO(SAV6AO sav) : this(sav.Items) { }
    public PlayerBag6AO(SAV6AODemo sav) : this(sav.Items) { }
    public PlayerBag6AO(MyItem6AO block) : this(block.Data) { }
    public PlayerBag6AO(ReadOnlySpan<byte> data) => Pouches.LoadAll(data);

    public override void CopyTo(SaveFile sav)
    {
        if (sav is SAV6AO ao)
            CopyTo(ao);
        else if (sav is SAV6AODemo demo)
            CopyTo(demo);
        else
            throw new ArgumentException($"Incompatible save type {sav.GetType().Name} for {nameof(PlayerBag6AO)}");
    }
    public void CopyTo(SAV6AO sav) => CopyTo(sav.Items);
    public void CopyTo(SAV6AODemo sav) => CopyTo(sav.Items);
    public void CopyTo(MyItem6AO block) => CopyTo(block.Data);
    public void CopyTo(Span<byte> data) => Pouches.SaveAll(data);
}
