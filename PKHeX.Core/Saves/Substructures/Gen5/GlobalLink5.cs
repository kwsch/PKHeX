using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed class GlobalLink5(SAV5 sav, Memory<byte> raw) : SaveBlock<SAV5>(sav, raw)
{
    public DateQuad5 UploadDate => new(Raw[..4]);

    // -1 if never interacted
    public int UploadCount { get => ReadInt32LittleEndian(Data[0x04..]); set => WriteInt32LittleEndian(Data[0x04..], value); }

    // 0x08: Stored Upload
    private const int SizeStored = PokeCrypto.SIZE_5PARTY;

    public Memory<byte> Upload => Raw.Slice(8, SizeStored);

    public const int CountItems = 20;
    public ushort GetItem(int index) => ReadUInt16LittleEndian(GetItemSpan(index));
    public void SetItem(int index, ushort value) => WriteUInt16LittleEndian(GetItemSpan(index), value);

    private Span<byte> GetItemSpan(int index)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual<uint>((uint)index, CountItems);
        return Data.Slice(0xE4 + (index * 2), 2);
    }

    public byte GetItemQuantity(int index)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual<uint>((uint)index, CountItems);
        return Data[0xE4 + (CountItems * 2) + index];
    }

    public void SetItemQuantity(int index, byte value)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual<uint>((uint)index, CountItems);
        Data[0xE4 + (CountItems * 2) + index] = value;
    }

    public const int CountFurniture = 5;

    public DreamFurniture5 GetFurniture(int index)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual<uint>((uint)index, CountFurniture);
        return new DreamFurniture5(Raw.Slice(0x120 + (index * DreamFurniture5.SIZE), DreamFurniture5.SIZE));
    }

    // 0x1A2
    public byte UploadStatus { get => Data[0x1A2]; set => Data[0x1A2] = value; }
    public bool IsSlotPresent { get => Data[0x1A3] != 0; set => Data[0x1A3] = (byte)(value ? 1 : 0); }

    /// <summary>
    /// The player can enter the Dream World as if it has a game Pokémon Black or White registered.
    /// No Black 2 or White 2 Pokémon Dream World Pokémon appears on demo mode.
    /// </summary>
    public bool IsRegistered { get => Data[0x1A4] != 0; set => Data[0x1A4] = (byte)(value ? 1 : 0); }

    /// <summary>
    /// To have full access to one's account, players first had to send a Pokémon to the Dream World by using their C-Gear's only Online feature, Game Sync.
    /// After doing so, players had full access to the Global Link site.
    /// </summary>
    public bool IsAccountFullAccess { get => Data[0x1A5] != 0; set => Data[0x1A5] = (byte)(value ? 1 : 0); }


    // 7 bits for selecting one furniture, 0x7F if none.
    private byte Furniture { get => Data[0x1A6]; set => Data[0x1A6] = value; }
    public byte SelectedFurnitureIndex { get => (byte)(Furniture & 0x7F); set => Furniture = (byte)((Furniture & 0x80) | (value & 0x7F)); }
    public bool IsFurnitureSynchronized { get => (Furniture & 0x80) != 0; set => Furniture = (byte)((Furniture & 0x7F) | (value ? 0x80 : 0)); }

    // Track downloaded content.
    public byte Musical { get => Data[0x1A7]; set => Data[0x1A7] = value; }
    public byte CGearSkin { get => Data[0x1A8]; set => Data[0x1A8] = value; }
    public byte DexSkin { get => Data[0x1A9]; set => Data[0x1A9] = value; }

    // 0x1AA,0x1AB: Unused padding
}

public struct DreamFurniture5(Memory<byte> Raw)
{
    public const int SIZE = 0x1A;
    public Span<byte> Data => Raw.Span;

    // 0x7E uninitialized individual slots.
    public ushort Value { get => ReadUInt16LittleEndian(Data); set => WriteUInt16LittleEndian(Data, value); }

    private const int NameLength = 12;
    public Span<byte> NameTrash => Data.Slice(2, NameLength * sizeof(ushort));
    public string Name { get => StringConverter5.GetString(NameTrash); set => StringConverter5.SetString(NameTrash, value, NameLength, 0); }

    public const string Extension = "dwf5";
    public string FileName => $"{Value:000} - {Name}.{Extension}";

    public void Clear()
    {
        Value = 0x7E;
        NameTrash.Clear();
    }
}
