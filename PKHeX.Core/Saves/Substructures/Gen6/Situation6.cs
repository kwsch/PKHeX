using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed class Situation6 : SaveBlock<SAV6>
{
    public Situation6(SAV6 SAV, int offset) : base(SAV) => Offset = offset;

    public int M
    {
        get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x02));
        set
        {
            var span = Data.AsSpan(Offset + 0x02);
            WriteUInt16LittleEndian(span, (ushort)value);
            WriteUInt16LittleEndian(span[0xF4..], (ushort)value);
        }
    }

    public float X
    {
        get => ReadSingleLittleEndian(Data.AsSpan(Offset + 0x10)) / 18;
        set
        {
            var span = Data.AsSpan(Offset + 0x10);
            WriteSingleLittleEndian(span, value * 18);
            WriteSingleLittleEndian(span[0xF4..], value * 18);
        }
    }

    public float Z
    {
        get => ReadSingleLittleEndian(Data.AsSpan(Offset + 0x14)) / 18;
        set
        {
            var span = Data.AsSpan(Offset + 0x14);
            WriteSingleLittleEndian(span, value * 18);
            WriteSingleLittleEndian(span[0xF4..], value * 18);
        }
    }

    public float Y
    {
        get => ReadSingleLittleEndian(Data.AsSpan(Offset + 0x18)) / 18;
        set
        {
            var span = Data.AsSpan(Offset + 0x18);
            WriteSingleLittleEndian(span, value * 18);
            WriteSingleLittleEndian(span[0xF4..], value * 18);
        }
    }

    // xy only
    public int Style
    {
        get => Data[Offset + 0x14D];
        set => Data[Offset + 0x14D] = (byte)value;
    }
}
