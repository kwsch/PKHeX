using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed class GameDataPB8 : IGameDataSide
{
    // Internal Attributes set on creation
    public readonly byte[] Data; // Raw Storage
    public readonly int Offset;

    private const int SIZE = HomeCrypto.SIZE_1GAME_PB8;
    private const HomeGameDataFormat Format = HomeGameDataFormat.PB8;

    public GameDataPB8 Clone() => new(Data.AsSpan(Offset, SIZE).ToArray());

    public GameDataPB8()
    {
        Data = new byte[SIZE];
        Data[0] = (byte)Format;
        WriteUInt16LittleEndian(Data.AsSpan(1, 2), SIZE);
    }

    public GameDataPB8(byte[] data, int offset = 0)
    {
        if ((HomeGameDataFormat)data[offset] != Format)
            throw new ArgumentException($"Invalid GameDataFormat for {Format}");

        if (ReadUInt16LittleEndian(data.AsSpan(offset + 1)) != SIZE)
            throw new ArgumentException($"Invalid GameDataSize for {Format}");

        Data = data;
        Offset = offset + 3;
    }

    public int Move1 { get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x00)); set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0x00), (ushort)value); }
    public int Move2 { get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x02)); set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0x02), (ushort)value); }
    public int Move3 { get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x04)); set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0x04), (ushort)value); }
    public int Move4 { get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x06)); set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0x06), (ushort)value); }

    public int Move1_PP { get => Data[Offset + 0x08]; set => Data[Offset + 0x08] = (byte)value; }
    public int Move2_PP { get => Data[Offset + 0x09]; set => Data[Offset + 0x09] = (byte)value; }
    public int Move3_PP { get => Data[Offset + 0x0A]; set => Data[Offset + 0x0A] = (byte)value; }
    public int Move4_PP { get => Data[Offset + 0x0B]; set => Data[Offset + 0x0B] = (byte)value; }
    public int Move1_PPUps { get => Data[Offset + 0x0C]; set => Data[Offset + 0x0C] = (byte)value; }
    public int Move2_PPUps { get => Data[Offset + 0x0D]; set => Data[Offset + 0x0D] = (byte)value; }
    public int Move3_PPUps { get => Data[Offset + 0x0E]; set => Data[Offset + 0x0E] = (byte)value; }
    public int Move4_PPUps { get => Data[Offset + 0x0F]; set => Data[Offset + 0x0F] = (byte)value; }

    public int RelearnMove1 { get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x10)); set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0x10), (ushort)value); }
    public int RelearnMove2 { get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x12)); set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0x12), (ushort)value); }
    public int RelearnMove3 { get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x14)); set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0x14), (ushort)value); }
    public int RelearnMove4 { get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x16)); set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0x16), (ushort)value); }
    public bool GetMoveRecordFlag(int index)
    {
        if ((uint)index > 112) // 14 bytes, 8 bits
            throw new ArgumentOutOfRangeException(nameof(index));
        int ofs = index >> 3;
        return FlagUtil.GetFlag(Data, Offset + 0x18 + ofs, index & 7);
    }

    public void SetMoveRecordFlag(int index, bool value)
    {
        if ((uint)index > 112) // 14 bytes, 8 bits
            throw new ArgumentOutOfRangeException(nameof(index));
        int ofs = index >> 3;
        FlagUtil.SetFlag(Data, Offset + 0x18 + ofs, index & 7, value);
    }

    public bool GetMoveRecordFlagAny() => Array.FindIndex(Data, Offset + 0x18, 14, z => z != 0) >= 0;

    public int Ball { get => Data[Offset + 0x26]; set => Data[Offset + 0x26] = (byte)value; }
    public int Egg_Location { get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x27)); set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0x27), (ushort)value); }
    public int Met_Location { get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x29)); set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0x29), (ushort)value); }

    // Not stored.
    public PersonalInfo GetPersonalInfo(int species, int form) => PersonalTable.BDSP.GetFormEntry(species, form);

    public int CopyTo(Span<byte> result)
    {
        result[0] = (byte)Format;
        WriteUInt16LittleEndian(result[1..], SIZE);
        Data.AsSpan(Offset, SIZE).CopyTo(result[3..]);
        return 3 + SIZE;
    }
}
