using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed class InventoryPouch3(int offset, int size, int maxCount, IItemStorage info, InventoryType type)
    : InventoryPouch(type, info, maxCount, offset, size)
{
    public uint SecurityKey { private get; set; } // FR/LG/E Only (not R/S)
    private InventoryItem[] _items = [];
    public override InventoryItem[] Items => _items;

    public override void GetPouch(ReadOnlySpan<byte> data)
    {
        var span = data[Offset..];
        var items = new InventoryItem[PouchDataSize];
        for (int i = 0; i < items.Length; i++)
            items[i] = ReadItem(span.Slice(i * 4, 4));
        _items = items;
    }

    private InventoryItem ReadItem(ReadOnlySpan<byte> data) => new()
    {
        Index = ReadUInt16LittleEndian(data),
        Count = ReadUInt16LittleEndian(data[2..]) ^ (ushort)SecurityKey,
    };

    public override void SetPouch(Span<byte> data)
    {
        ArgumentOutOfRangeException.ThrowIfNotEqual(_items.Length, PouchDataSize);
        var span = data[Offset..];
        for (int i = 0; i < _items.Length; i++)
            WriteItem(span.Slice(i * 4, 4), _items[i]);
    }

    private void WriteItem(Span<byte> data, InventoryItem item)
    {
        WriteUInt16LittleEndian(data, (ushort)item.Index);
        WriteUInt16LittleEndian(data[2..], (ushort)(item.Count ^ SecurityKey));
    }

    public override InventoryItem GetEmpty(int itemID = 0, int count = 0) => new() { Index = itemID, Count = count };
}
