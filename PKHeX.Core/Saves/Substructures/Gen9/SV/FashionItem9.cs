using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed class FashionItem9 : IItemNewFlag
{
    public const int SIZE = 8;
    public const uint None = ushort.MaxValue;

    // Structure:
    // 0x00: uint Fashion ItemID
    // 0x04: uint Bitflags
    // - 0 IsNew
    // remainder: unused
    public required uint Value { get; set; }
    public uint Flags { get; set; }

    public bool IsNew      { get => (Flags & 0x1) != 0; set => Flags = value ? (Flags | 0x1u) : (Flags & ~0x1u); }

    public static FashionItem9 Read(ReadOnlySpan<byte> data) => new()
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

    public static FashionItem9[] GetArray(ReadOnlySpan<byte> data)
    {
        int count = data.Length / SIZE;
        var items = new FashionItem9[count];
        for (int i = 0; i < count; i++)
            items[i] = Read(data.Slice(i * SIZE, SIZE));
        return items;
    }

    public static void SetArray(ReadOnlySpan<FashionItem9> items, Span<byte> data)
    {
        for (int i = 0; i < items.Length; i++)
            items[i].Write(data.Slice(i * SIZE, SIZE));
    }
}
