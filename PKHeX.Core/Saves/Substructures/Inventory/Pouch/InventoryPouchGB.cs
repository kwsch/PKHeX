using System;

namespace PKHeX.Core;

public sealed class InventoryPouchGB(int offset, int size, int maxCount, IItemStorage info, InventoryType type)
    : InventoryPouch(type, info, maxCount, offset, size)
{
    public override InventoryItem GetEmpty(int itemID = 0, int count = 0) => new() { Index = itemID, Count = count };

    private InventoryItem[] _items = [];
    public override InventoryItem[] Items => _items;

    public override void GetPouch(ReadOnlySpan<byte> data)
    {
        var legal = Info.GetItems(Type);
        var items = new InventoryItem[PouchDataSize];
        data = data[Offset..];
        if (Type == InventoryType.TMHMs)
        {
            int slot = 0;
            for (int i = 0; i < items.Length; i++)
            {
                if (data[i] == 0)
                    continue;
                items[slot++] = new InventoryItem { Index = legal[i], Count = data[i] };
            }
            // Fill remainder with empty entries.
            while (slot < items.Length)
                items[slot++] = GetEmpty();
        }
        else
        {
            int numStored = data[0];
            if (numStored > PouchDataSize) // uninitialized yellow (0xFF), sanity check for out-of-bounds values
                numStored = 0;
            for (int i = 0; i < numStored; i++)
            {
                items[i] = Type switch
                {
                    InventoryType.KeyItems => new InventoryItem {Index = data[i + 1], Count = 1},
                    _ => new InventoryItem {Index = data[(i * 2) + 1], Count = data[(i * 2) + 2]},
                };
            }
            // Fill remainder with empty entries.
            for (int i = numStored; i < items.Length; i++)
                items[i] = GetEmpty();
        }
        _items = items;
    }

    public override void SetPouch(Span<byte> data)
    {
        ArgumentOutOfRangeException.ThrowIfNotEqual(_items.Length, PouchDataSize);

        ClearCount0();

        data = data[Offset..];

        switch (Type)
        {
            case InventoryType.TMHMs:
                var legal = Info.GetItems(Type);
                foreach (var item in _items)
                {
                    int index = legal.IndexOf((ushort)item.Index);
                    if (index < 0) // enforce correct pouch
                        continue;
                    data[index] = (byte)item.Count;
                }
                break;
            case InventoryType.KeyItems:
                data[0] = (byte)Count;
                for (int i = 0; i < _items.Length; i++)
                    data[i + 1] = (byte)_items[i].Index;
                data[1 + Count] = 0xFF;
                break;
            default:
                data[0] = (byte)Count;
                for (int i = 0; i < _items.Length; i++)
                {
                    var item = _items[i];
                    data[(i * 2) + 1] = (byte)item.Index;
                    data[(i * 2) + 2] = (byte)item.Count;
                }
                data[1 + (2 * Count)] = 0xFF;
                break;
        }
    }
}
