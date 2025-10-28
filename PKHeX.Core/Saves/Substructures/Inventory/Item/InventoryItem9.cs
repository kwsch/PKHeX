using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed record InventoryItem9 : InventoryItem, IItemFavorite, IItemNewFlag
{
    public const int SIZE = 0x10;

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

    /// <summary>
    /// Invalid pouch ID.
    /// </summary>
    /// <remarks>
    /// Only used internally to this program, not stored in the data.
    /// The game did have behavior that initialized to this value on early patches, but was later changed to initialize to 0.
    /// Therefore, we cannot use this value to set. Technically it can be used to determine if the game was started on an early patch.
    /// </remarks>
    public const uint PouchInvalid = 0xFFFFFFFF;

    public uint Pouch { get; set; }
    public uint Flags { get; set; }
    public uint Padding { get; set; }

    public bool IsNew      { get => (Flags & 0x1) != 0; set => Flags = (Flags & ~0x1u) | (value ? 0x1u : 0x0u); } // red dot
    public bool IsFavorite { get => (Flags & 0x2) != 0; set => Flags = (Flags & ~0x2u) | (value ? 0x2u : 0x0u); }
    public bool IsObtained { get => (Flags & 0x4) != 0; set => Flags = (Flags & ~0x4u) | (value ? 0x4u : 0x0u); } // Has Been Obtained At Least Once

    public override string ToString()
    {
        if (!IsObtained)
            return $"{Index:0000} Empty";
        return $"{Index:0000} x{Count}{(IsNew ? "*" : "")}{(IsFavorite ? "F" : "")} - {Flags:X8}";
    }

    public override void Clear()
    {
        Index = 0;
        Pouch = 0;
        Count = 0;
        Flags = Padding = 0;
    }

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
        // Index is not saved.
        WriteUInt32LittleEndian(data, Pouch);
        WriteUInt32LittleEndian(data[4..], (uint)Count);
        WriteUInt32LittleEndian(data[8..], Flags);
        WriteUInt32LittleEndian(data[12..], Padding);
    }

    public static uint GetItemCount(Span<byte> data) => ReadUInt32LittleEndian(data[4..]);

    public override void SetNewDetails(int count)
    {
        base.SetNewDetails(count);
        IsNew = true;
        IsObtained = true;
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
