using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed record InventoryItem8b : InventoryItem, IItemFavorite, IItemNewFlag
{
    public const int SIZE = 0x10;
    private const ushort SortOrderNone = 0;

    public bool IsFavorite { get; set; }
    public bool IsNew { get; set; }
    public ushort SortOrder { get; set; }

    public override string ToString() => $"{SortOrder:00} - {Index:000} x{Count}{(IsNew ? "*" : "")}{(IsFavorite ? "F" : "")}";

    public override void Clear()
    {
        Index = Count = 0;
        IsFavorite = false;
        IsNew = true;
        SortOrder = SortOrderNone;
    }

    /// <summary>
    /// Indicates if the item has been acquired by the player.
    /// </summary>
    public bool IsValidSaveSortNumberCount => SortOrder != SortOrderNone;

    public static InventoryItem8b Read(ushort index, ReadOnlySpan<byte> data) => new()
    {
        Index = index,
        Count = ReadInt32LittleEndian(data),
        IsNew = ReadInt32LittleEndian(data[4..]) == 0,
        IsFavorite = ReadInt32LittleEndian(data[8..]) == 1,
        SortOrder = ReadUInt16LittleEndian(data[12..]),
        // 0xE alignment
    };

    public void Write(Span<byte> data)
    {
        // Index is not saved.
        WriteUInt32LittleEndian(data, (uint)Count);
        WriteUInt32LittleEndian(data[4..], IsNew ? 0u : 1u);
        WriteUInt32LittleEndian(data[8..], IsFavorite ? 1u : 0u);
        WriteUInt16LittleEndian(data[12..], SortOrder);
        WriteUInt16LittleEndian(data[14..], 0);
    }

    public static void Clear(Span<byte> data, int offset) => data.Slice(offset, SIZE).Clear();

    public override void SetNewDetails(int count)
    {
        base.SetNewDetails(count);
        if (IsValidSaveSortNumberCount)
            return;
        IsNew = true;
        IsFavorite = false;
    }

    /// <summary>
    /// Item has been matched to a previously existing item. Copy over the misc details.
    /// </summary>
    public override void MergeOverwrite<T>(T other)
    {
        base.MergeOverwrite(other);
        if (other is not InventoryItem8b item)
            return;
        SortOrder = item.SortOrder;
        IsNew = item.IsNew;
        IsFavorite = item.IsFavorite;
    }
}
