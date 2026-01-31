using System;

namespace PKHeX.Core;

/// <summary>
/// Represents the data in a pouch pocket containing items of a similar type group.
/// </summary>
/// <remarks>
/// Used by <see cref="GameVersion.PLA"/>.
/// </remarks>
public sealed class InventoryPouch8a(int size, int maxCount, IItemStorage info, InventoryType type)
    : InventoryPouch(type, info, maxCount, 0)
{
    public override InventoryItem8a GetEmpty(int itemID = 0, int count = 0) => new() { Index = itemID, Count = count };
    private InventoryItem8a[] _items = [];
    public override InventoryItem8a[] Items => _items;

    public override void GetPouch(ReadOnlySpan<byte> data)
    {
        var items = new InventoryItem8a[size];
        for (int i = 0; i < items.Length; i++)
            items[i] = GetItem(data, i * 4);
        _items = items;
    }

    public static InventoryItem8a GetItem(ReadOnlySpan<byte> data, int ofs) => InventoryItem8a.Read(data[ofs..]);

    public override void SetPouch(Span<byte> data)
    {
        var items = _items;
        for (var i = 0; i < items.Length; i++)
            items[i].Write(data[(i * 4)..]);
    }
}
