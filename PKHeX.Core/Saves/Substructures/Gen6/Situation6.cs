using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed class Situation6(SAV6 SAV, Memory<byte> raw) : SaveBlock<SAV6>(SAV, raw)
{
    public int M
    {
        get => ReadUInt16LittleEndian(Data[0x02..]);
        set
        {
            var span = Data[0x02..];
            WriteUInt16LittleEndian(span, (ushort)value);
            WriteUInt16LittleEndian(span[0xF2..], (ushort)value);
        }
    }

    public int R
    {
        get => ReadUInt16LittleEndian(Data[0x06..]);
        set
        {
            var span = Data[0x06..];
            WriteUInt16LittleEndian(span, (ushort)value);
            WriteUInt16LittleEndian(span[0xF0..], (ushort)value);
        }
    }

    public float X
    {
        get => ReadSingleLittleEndian(Data[0x10..]);
        set
        {
            var span = Data[0x10..];
            WriteSingleLittleEndian(span, value);
            WriteSingleLittleEndian(span[0xF4..], value);
        }
    }

    public float Z
    {
        get => ReadSingleLittleEndian(Data[0x14..]);
        set
        {
            var span = Data[0x14..];
            WriteSingleLittleEndian(span, value);
            WriteSingleLittleEndian(span[0xF4..], value);
        }
    }

    public float Y
    {
        get => ReadSingleLittleEndian(Data[0x18..]);
        set
        {
            var span = Data[0x18..];
            WriteSingleLittleEndian(span, value);
            WriteSingleLittleEndian(span[0xF4..], value);
        }
    }

    // xy only
    public int Style
    {
        get => Data[0x14D];
        set => Data[0x14D] = (byte)value;
    }
}
