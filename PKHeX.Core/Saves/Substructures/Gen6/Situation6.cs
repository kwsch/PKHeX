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
            WriteUInt16LittleEndian(span[0xF2..], (ushort)value);
        }
    }

    public int R
    {
        get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x06));
        set
        {
            var span = Data.AsSpan(Offset + 0x06);
            WriteUInt16LittleEndian(span, (ushort)value);
            WriteUInt16LittleEndian(span[0xF0..], (ushort)value);
        }
    }

    public float X
    {
        get => ReadSingleLittleEndian(Data.AsSpan(Offset + 0x10));
        set
        {
            var span = Data.AsSpan(Offset + 0x10);
            WriteSingleLittleEndian(span, value);
            WriteSingleLittleEndian(span[0xF4..], value);
        }
    }

    public float Z
    {
        get => ReadSingleLittleEndian(Data.AsSpan(Offset + 0x14));
        set
        {
            var span = Data.AsSpan(Offset + 0x14);
            WriteSingleLittleEndian(span, value);
            WriteSingleLittleEndian(span[0xF4..], value);
        }
    }

    public float Y
    {
        get => ReadSingleLittleEndian(Data.AsSpan(Offset + 0x18));
        set
        {
            var span = Data.AsSpan(Offset + 0x18);
            WriteSingleLittleEndian(span, value);
            WriteSingleLittleEndian(span[0xF4..], value);
        }
    }

    // xy only
    public int Style
    {
        get => Data[Offset + 0x14D];
        set => Data[Offset + 0x14D] = (byte)value;
    }
}
