using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public abstract class Entralink5(SAV5 SAV, Memory<byte> raw) : SaveBlock<SAV5>(SAV, raw)
{
    public ushort WhiteForestLevel
    {
        get => ReadUInt16LittleEndian(Data[0x0C..]);
        set => WriteUInt16LittleEndian(Data[0x0C..], value);
    }

    public ushort BlackCityLevel
    {
        get => ReadUInt16LittleEndian(Data[0x0E..]);
        set => WriteUInt16LittleEndian(Data[0x0E..], value);
    }
}

public sealed class Entralink5BW(SAV5BW SAV, Memory<byte> raw) : Entralink5(SAV, raw)
{
    public byte MissionsComplete
    {
        get => Data[0x1A4];
        set => Data[0x1A4] = value;
    }
}

public sealed class Entralink5B2W2(SAV5B2W2 SAV, Memory<byte> raw) : Entralink5(SAV, raw)
{
    public byte PassPower1
    {
        get => Data[0x1A0];
        set => Data[0x1A0] = value;
    }

    public byte PassPower2
    {
        get => Data[0x1A1];
        set => Data[0x1A1] = value;
    }

    public byte PassPower3
    {
        get => Data[0x1A2];
        set => Data[0x1A2] = value;
    }
}
