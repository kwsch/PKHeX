using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed record InventoryItem9 : InventoryItem, IItemFavorite, IItemNewFlag
{
    public const int SIZE = 0x10;

    public const uint PouchNone = 0xFFFFFFFF;
    public const uint PouchMedicine = 0;
    public const uint PouchBall = 1;
    public const uint PouchBattle = 2;
    public const uint PouchBerries = 3;
    public const uint PouchOther = 4;
    public const uint PouchTMHM = 5;
    public const uint PouchTreasure = 6;
    public const uint PouchPicnic = 7;
    public const uint PouchEvent = 8;
    public const uint PouchMaterial = 9;
    public const uint PouchRecipe = 10;

    public uint Pouch { get; set; }
    public uint Flags { get; set; }
    public uint Padding { get; set; }

    public bool IsNew      { get => (Flags & 0x1) != 0; set => Flags = (Flags & ~0x1u) | (value ? 0x1u : 0x0u); } // red dot
    public bool IsFavorite { get => (Flags & 0x2) != 0; set => Flags = (Flags & ~0x2u) | (value ? 0x2u : 0x0u); }
    public bool IsUpdated  { get => (Flags & 0x4) != 0; set => Flags = (Flags & ~0x4u) | (value ? 0x4u : 0x0u); } // always true if pouch is set

    public override string ToString() => $"{Index:000} x{Count}{(IsNew ? "*" : "")}{(IsFavorite ? "F" : "")} - {Flags:X8}";

    public override void Clear()
    {
        Index = Count = 0;
        Flags = Padding = 0;
        IsFavorite = false;
        IsUpdated = false;
        IsNew = false;
        Pouch = PouchNone;
    }

    /// <summary>
    /// Indicates if the item has been acquired by the player.
    /// </summary>
    public bool IsValidPouch => Pouch != PouchNone;

    public static InventoryItem9 Read(ushort index, ReadOnlySpan<byte> data) => new()
    {
        Index = index,
        Pouch = ReadUInt32LittleEndian(data),
        Count = ReadInt32LittleEndian(data[4..]),
        Flags = ReadUInt32LittleEndian(data[8..]),
        Padding = ReadUInt32LittleEndian(data[12..]),
    };

    public void Write(Span<byte> data)
    {
        IsUpdated = Pouch != PouchNone;

        // Index is not saved.
        WriteUInt32LittleEndian(data, Pouch);
        WriteUInt32LittleEndian(data[4..], (uint)Count);
        WriteUInt32LittleEndian(data[8..], Flags);
        WriteUInt32LittleEndian(data[12..], Padding);
    }

    public static void Clear(Span<byte> data, int offset) {
        data.Slice(offset, SIZE).Clear();
        WriteUInt32LittleEndian(data[offset..], PouchNone);
    }

    public override void SetNewDetails(int count)
    {
        base.SetNewDetails(count);
        if (IsValidPouch)
            return;
        IsNew = true;
        IsUpdated = true;
        IsFavorite = false;
    }

    /// <summary>
    /// Item has been matched to a previously existing item. Copy over the misc details.
    /// </summary>
    public override void MergeOverwrite<T>(T other)
    {
        base.MergeOverwrite(other);
        if (other is not InventoryItem9 item)
            return;
        Pouch = item.Pouch;
        Flags = item.Flags;
        Padding = item.Padding;
    }
}
