using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed class Misc6XY(SAV6XY sav, Memory<byte> raw) : SaveBlock<SAV6XY>(sav, raw), IMisc6
{
    public uint Money
    {
        get => ReadUInt32LittleEndian(Data[0x8..]);
        set => WriteUInt32LittleEndian(Data[0x8..], value);
    }

    public int Badges
    {
        get => Data[0xC];
        set => Data[0xC] = (byte)value;
    }

    public int BP
    {
        get => ReadUInt16LittleEndian(Data[0x3C..]);
        set => WriteUInt16LittleEndian(Data[0x3C..], (ushort)value);
    }

    public int Vivillon
    {
        get => Data[0x50];
        set => Data[0x50] = (byte)value;
    }
}
