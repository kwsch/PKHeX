using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public abstract class Entralink5 : SaveBlock<SAV5>
{
    protected Entralink5(SAV5 SAV, int offset) : base(SAV) => Offset = offset;

    public ushort WhiteForestLevel
    {
        get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x0C));
        set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0x0C), value);
    }

    public ushort BlackCityLevel
    {
        get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x0E));
        set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0x0E), value);
    }
}

public sealed class Entralink5BW(SAV5BW SAV, int offset) : Entralink5(SAV, offset)
{
    public byte MissionsComplete
    {
        get => Data[Offset + 0x1A4];
        set => Data[Offset + 0x1A4] = value;
    }
}

public sealed class Entralink5B2W2(SAV5B2W2 SAV, int offset) : Entralink5(SAV, offset)
{
    public byte PassPower1
    {
        get => Data[Offset + 0x1A0];
        set => Data[Offset + 0x1A0] = value;
    }

    public byte PassPower2
    {
        get => Data[Offset + 0x1A1];
        set => Data[Offset + 0x1A1] = value;
    }

    public byte PassPower3
    {
        get => Data[Offset + 0x1A2];
        set => Data[Offset + 0x1A2] = value;
    }
}
