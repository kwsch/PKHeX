using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed class FashionItem9a : IItemNewFlag, IItemNewShopFlag
{
    public const int SIZE = 8;
    public const uint None = ushort.MaxValue;

    // Structure:
    // 0x00: uint Fashion ItemID
    // 0x04: uint Bitflags
    // - 0 IsNew
    // - 1 IsNewShop
    // - 2 IsNewGroup
    // - 3 IsEquipped
    // - 4 IsOwned
    // remainder: unused
    public required uint Value { get; set; }
    public uint Flags { get; set; }

    public bool IsNew      { get => (Flags & 0x1) != 0; set => Flags = value ? (Flags | 0x1u) : (Flags & ~0x1u); }
    public bool IsNewShop  { get => (Flags & 0x2) != 0; set => Flags = value ? (Flags | 0x2u) : (Flags & ~0x2u); }
    public bool IsNewGroup { get => (Flags & 0x4) != 0; set => Flags = value ? (Flags | 0x4u) : (Flags & ~0x4u); }
    public bool IsEquipped { get => (Flags & 0x8) != 0; set => Flags = value ? (Flags | 0x8u) : (Flags & ~0x8u); }
    public bool IsOwned    { get => (Flags & 0x10) != 0; set => Flags = value ? (Flags | 0x10u) : (Flags & ~0x10u); }

    public static FashionItem9a Read(ReadOnlySpan<byte> data) => new()
    {
        Value = ReadUInt32LittleEndian(data),
        Flags = ReadUInt32LittleEndian(data[4..]),
    };

    public void Write(Span<byte> data)
    {
        WriteUInt32LittleEndian(data, Value);
        WriteUInt32LittleEndian(data[4..], Flags);
    }

    public void Clear()
    {
        Value = None;
        Flags = 0;
    }

    public static FashionItem9a[] GetArray(ReadOnlySpan<byte> data)
    {
        int count = data.Length / SIZE;
        var items = new FashionItem9a[count];
        for (int i = 0; i < count; i++)
            items[i] = Read(data.Slice(i * SIZE, SIZE));
        return items;
    }

    public static void SetArray(ReadOnlySpan<FashionItem9a> items, Span<byte> data)
    {
        for (int i = 0; i < items.Length; i++)
            items[i].Write(data.Slice(i * SIZE, SIZE));
    }

    public static void ModifyAll(Span<byte> data, Action<FashionItem9a> action)
    {
        int count = data.Length / SIZE;
        for (int i = 0; i < count; i++)
        {
            var slice = data.Slice(i * SIZE, SIZE);
            var item = Read(slice);
            if (item.Value is not None)
                action(item);
            item.Write(slice);
        }
    }
}
