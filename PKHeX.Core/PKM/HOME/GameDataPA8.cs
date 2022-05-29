using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed class GameDataPA8 : IGameDataSide, IScaledSizeAbsolute
{
    // Internal Attributes set on creation
    public readonly byte[] Data; // Raw Storage
    public readonly int Offset;

    private const int SIZE = HomeCrypto.SIZE_1GAME_PA8;
    private const HomeGameDataFormat Format = HomeGameDataFormat.PA8;

    public GameDataPA8 Clone() => new(Data.AsSpan(Offset, SIZE).ToArray());

    public GameDataPA8()
    {
        Data = new byte[SIZE];
        Data[0] = (byte)Format;
        WriteUInt16LittleEndian(Data.AsSpan(1, 2), SIZE);
    }

    public GameDataPA8(byte[] data, int offset = 0)
    {
        if ((HomeGameDataFormat)data[offset] != Format)
            throw new ArgumentException($"Invalid GameDataFormat for {Format}");

        if (ReadUInt16LittleEndian(data.AsSpan(offset + 1)) != SIZE)
            throw new ArgumentException($"Invalid GameDataSize for {Format}");

        Data = data;
        Offset = offset + 3;
    }

    public bool IsAlpha { get => Data[Offset + 0x00] != 0; set => Data[Offset + 0x00] = (byte)(value ? 1 : 0); }
    public bool IsNoble { get => Data[Offset + 0x01] != 0; set => Data[Offset + 0x01] = (byte)(value ? 1 : 0); }
    public ushort AlphaMove { get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x02)); set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0x02), value); }
    public byte HeightScalarCopy { get => Data[Offset + 0x04]; set => Data[Offset + 0x04] = value; }

    public int Move1 { get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x05)); set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0x05), (ushort)value); }
    public int Move2 { get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x07)); set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0x07), (ushort)value); }
    public int Move3 { get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x09)); set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0x09), (ushort)value); }
    public int Move4 { get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x0B)); set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0x0B), (ushort)value); }

    public int Move1_PP { get => Data[Offset + 0x0D]; set => Data[Offset + 0x0D] = (byte)value; }
    public int Move2_PP { get => Data[Offset + 0x0E]; set => Data[Offset + 0x0E] = (byte)value; }
    public int Move3_PP { get => Data[Offset + 0x0F]; set => Data[Offset + 0x0F] = (byte)value; }
    public int Move4_PP { get => Data[Offset + 0x10]; set => Data[Offset + 0x10] = (byte)value; }
    public int RelearnMove1 { get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x11)); set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0x11), (ushort)value); }
    public int RelearnMove2 { get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x13)); set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0x13), (ushort)value); }
    public int RelearnMove3 { get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x15)); set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0x15), (ushort)value); }
    public int RelearnMove4 { get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x17)); set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0x17), (ushort)value); }
    public byte GV_HP  { get => Data[Offset + 0x19]; set => Data[Offset + 0x19] = value; }
    public byte GV_ATK { get => Data[Offset + 0x1A]; set => Data[Offset + 0x1A] = value; }
    public byte GV_DEF { get => Data[Offset + 0x1B]; set => Data[Offset + 0x1B] = value; }
    public byte GV_SPE { get => Data[Offset + 0x1C]; set => Data[Offset + 0x1C] = value; }
    public byte GV_SPA { get => Data[Offset + 0x1D]; set => Data[Offset + 0x1D] = value; }
    public byte GV_SPD { get => Data[Offset + 0x1E]; set => Data[Offset + 0x1E] = value; }
    public float HeightAbsolute { get => ReadSingleLittleEndian(Data.AsSpan(Offset + 0x1F)); set => WriteSingleLittleEndian(Data.AsSpan(Offset + 0x1F), value); }
    public float WeightAbsolute { get => ReadSingleLittleEndian(Data.AsSpan(Offset + 0x23)); set => WriteSingleLittleEndian(Data.AsSpan(Offset + 0x23), value); }
    public int Ball { get => Data[Offset + 0x27]; set => Data[Offset + 0x27] = (byte)value; }

    public bool GetPurchasedRecordFlag(int index)
    {
        if ((uint)index > 63) // 8 bytes, 8 bits
            throw new ArgumentOutOfRangeException(nameof(index));
        int ofs = index >> 3;
        return FlagUtil.GetFlag(Data, Offset + 0x28 + ofs, index & 7);
    }

    public void SetPurchasedRecordFlag(int index, bool value)
    {
        if ((uint)index > 63) // 8 bytes, 8 bits
            throw new ArgumentOutOfRangeException(nameof(index));
        int ofs = index >> 3;
        FlagUtil.SetFlag(Data, Offset + 0x28 + ofs, index & 7, value);
    }

    public bool GetPurchasedRecordFlagAny() => Array.FindIndex(Data, Offset + 0x28, 8, z => z != 0) >= 0;

    public int GetPurchasedCount()
    {
        var value = ReadUInt64LittleEndian(Data.AsSpan(0x155));
        ulong result = 0;
        for (int i = 0; i < 64; i++)
            result += ((value >> i) & 1);
        return (int)result;
    }

    public bool GetMasteredRecordFlag(int index)
    {
        if ((uint)index > 63) // 8 bytes, 8 bits
            throw new ArgumentOutOfRangeException(nameof(index));
        int ofs = index >> 3;
        return FlagUtil.GetFlag(Data, Offset + 0x30 + ofs, index & 7);
    }

    public void SetMasteredRecordFlag(int index, bool value)
    {
        if ((uint)index > 63) // 8 bytes, 8 bits
            throw new ArgumentOutOfRangeException(nameof(index));
        int ofs = index >> 3;
        FlagUtil.SetFlag(Data, Offset + 0x30 + ofs, index & 7, value);
    }

    public bool GetMasteredRecordFlagAny() => Array.FindIndex(Data, Offset + 0x30, 8, z => z != 0) >= 0;

    public int Egg_Location { get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x38)); set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0x38), (ushort)value); }
    public int Met_Location { get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x3A)); set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0x3A), (ushort)value); }

    // Not stored.
    public PersonalInfo GetPersonalInfo(int species, int form) => PersonalTable.LA.GetFormEntry(species, form);
    public int Move1_PPUps { get => 0; set { } }
    public int Move2_PPUps { get => 0; set { } }
    public int Move3_PPUps { get => 0; set { } }
    public int Move4_PPUps { get => 0; set { } }

    public int CopyTo(Span<byte> result)
    {
        result[0] = (byte)Format;
        WriteUInt16LittleEndian(result[1..], SIZE);
        Data.AsSpan(Offset, SIZE).CopyTo(result[3..]);
        return 3 + SIZE;
    }
}
