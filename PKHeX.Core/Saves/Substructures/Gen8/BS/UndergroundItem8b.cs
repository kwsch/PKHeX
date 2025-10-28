using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed class UndergroundItem8b
{
    public const int SIZE = 0xC;
    public readonly int Index; // not serialized
    public UgItemType Type => UgItemUtil.GetType(Index);
    public int MaxValue => UgItemUtil.GetMax(Index);

    public int Count { get; set; }
    public bool HideNewFlag { get; set; }
    public bool IsFavoriteFlag { get; set; }

    public UndergroundItem8b(ReadOnlySpan<byte> data, int index)
    {
        Index = index;
        var offset = (SIZE * index);
        var span = data.Slice(offset, SIZE);
        ReadAbsolute(span);
    }

    public void Write(Span<byte> data)
    {
        var offset = (SIZE * Index);
        var span = data.Slice(offset, SIZE);
        WriteAbsolute(span);
    }

    private void ReadAbsolute(ReadOnlySpan<byte> span)
    {
        Count = ReadInt32LittleEndian(span);
        HideNewFlag = ReadUInt32LittleEndian(span[4..]) == 1;
        IsFavoriteFlag = ReadUInt32LittleEndian(span[8..]) == 1;
    }

    private void WriteAbsolute(Span<byte> span)
    {
        WriteInt32LittleEndian(span, Count);
        WriteUInt32LittleEndian(span[4..], HideNewFlag ? 1u : 0u);
        WriteUInt32LittleEndian(span[8..], IsFavoriteFlag ? 1u : 0u);
    }
}
