using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed class GameDataPB7 : IGameDataSide, IScaledSizeAbsolute
{
    // Internal Attributes set on creation
    public readonly byte[] Data; // Raw Storage
    public readonly int Offset;

    private const int SIZE = HomeCrypto.SIZE_1GAME_PB7;
    private const HomeGameDataFormat Format = HomeGameDataFormat.PB7;

    public GameDataPB7 Clone() => new(Data.AsSpan(Offset, SIZE).ToArray());

    public GameDataPB7()
    {
        Data = new byte[SIZE];
        Data[0] = (byte)Format;
        WriteUInt16LittleEndian(Data.AsSpan(1, 2), SIZE);
    }

    public GameDataPB7(byte[] data, int offset = 0)
    {
        if ((HomeGameDataFormat)data[offset] != Format)
            throw new ArgumentException($"Invalid GameDataFormat for {Format}");

        if (ReadUInt16LittleEndian(data.AsSpan(offset + 1)) != SIZE)
            throw new ArgumentException($"Invalid GameDataSize for {Format}");

        Data = data;
        Offset = offset + 3;
    }

    public byte AV_HP  { get => Data[Offset + 0x00]; set => Data[Offset + 0x00] = value; }
    public byte AV_ATK { get => Data[Offset + 0x01]; set => Data[Offset + 0x01] = value; }
    public byte AV_DEF { get => Data[Offset + 0x02]; set => Data[Offset + 0x02] = value; }
    public byte AV_SPE { get => Data[Offset + 0x03]; set => Data[Offset + 0x03] = value; }
    public byte AV_SPA { get => Data[Offset + 0x04]; set => Data[Offset + 0x04] = value; }
    public byte AV_SPD { get => Data[Offset + 0x05]; set => Data[Offset + 0x05] = value; }
    public byte ResortEventState { get => Data[Offset + 0x06]; set => Data[Offset + 0x06] = value; }

    public int Move1 { get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x07)); set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0x07), (ushort)value); }
    public int Move2 { get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x09)); set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0x09), (ushort)value); }
    public int Move3 { get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x0B)); set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0x0B), (ushort)value); }
    public int Move4 { get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x0D)); set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0x0D), (ushort)value); }

    public int Move1_PP { get => Data[Offset + 0x0F]; set => Data[Offset + 0x0F] = (byte)value; }
    public int Move2_PP { get => Data[Offset + 0x10]; set => Data[Offset + 0x10] = (byte)value; }
    public int Move3_PP { get => Data[Offset + 0x11]; set => Data[Offset + 0x11] = (byte)value; }
    public int Move4_PP { get => Data[Offset + 0x12]; set => Data[Offset + 0x12] = (byte)value; }
    public int Move1_PPUps { get => Data[Offset + 0x13]; set => Data[Offset + 0x13] = (byte)value; }
    public int Move2_PPUps { get => Data[Offset + 0x14]; set => Data[Offset + 0x14] = (byte)value; }
    public int Move3_PPUps { get => Data[Offset + 0x15]; set => Data[Offset + 0x15] = (byte)value; }
    public int Move4_PPUps { get => Data[Offset + 0x16]; set => Data[Offset + 0x16] = (byte)value; }

    public int RelearnMove1 { get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x17)); set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0x17), (ushort)value); }
    public int RelearnMove2 { get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x19)); set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0x19), (ushort)value); }
    public int RelearnMove3 { get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x1B)); set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0x1B), (ushort)value); }
    public int RelearnMove4 { get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x1D)); set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0x1D), (ushort)value); }
    public float HeightAbsolute { get => ReadSingleLittleEndian(Data.AsSpan(Offset + 0x1F)); set => WriteSingleLittleEndian(Data.AsSpan(Offset + 0x1F), value); }
    public float WeightAbsolute { get => ReadSingleLittleEndian(Data.AsSpan(Offset + 0x23)); set => WriteSingleLittleEndian(Data.AsSpan(Offset + 0x23), value); }

    public byte FieldEventFatigue1 { get => Data[Offset + 0x27]; set => Data[Offset + 0x27] = value; }
    public byte FieldEventFatigue2 { get => Data[Offset + 0x28]; set => Data[Offset + 0x28] = value; }
    public byte Fullness { get => Data[Offset + 0x29]; set => Data[Offset + 0x29] = value; }
    public byte Rank { get => Data[Offset + 0x2A]; set => Data[Offset + 0x2A] = value; }
    public int OT_Affection { get => Data[Offset + 0x2B]; set => Data[Offset + 0x2B] = (byte)value; }
    public byte OT_Intensity { get => Data[Offset + 0x2C]; set => Data[Offset + 0x2C] = value; }
    public byte OT_Memory { get => Data[Offset + 0x2D]; set => Data[Offset + 0x2D] = value; }
    public ushort OT_TextVar { get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x2E)); set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0x2E), value); }
    public byte OT_Feeling { get => Data[Offset + 0x30]; set => Data[Offset + 0x30] = value; }
    public byte Enjoyment { get => Data[Offset + 0x31]; set => Data[Offset + 0x31] = value; }
    public uint GeoPadding { get => ReadUInt32LittleEndian(Data.AsSpan(Offset + 0x32)); set => WriteUInt32LittleEndian(Data.AsSpan(Offset + 0x32), value); }
    public int Ball { get => Data[Offset + 0x36]; set => Data[Offset + 0x36] = (byte)value; }
    public int Egg_Location { get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x37)); set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0x37), (ushort)value); }
    public int Met_Location { get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x39)); set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0x39), (ushort)value); }

    // Not stored.
    public PersonalInfo GetPersonalInfo(int species, int form) => PersonalTable.GG.GetFormEntry(species, form);

    public int CopyTo(Span<byte> result)
    {
        result[0] = (byte)Format;
        WriteUInt16LittleEndian(result[1..], SIZE);
        Data.AsSpan(Offset, SIZE).CopyTo(result[3..]);
        return 3 + SIZE;
    }
}
