using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public interface IMisc6
{
    uint Money { get; set; }
}

public sealed class Misc6AO : SaveBlock<SAV6>, IMisc6
{
    public Misc6AO(SAV6AO sav, Memory<byte> raw) : base(sav, raw) { }
    public Misc6AO(SAV6AODemo sav, Memory<byte> raw) : base(sav, raw) { }

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
        get => ReadUInt16LittleEndian(Data[0x30..]);
        set => WriteUInt16LittleEndian(Data[0x30..], (ushort)value);
    }

    public int Vivillon
    {
        get => Data[0x44];
        set => Data[0x44] = (byte)value;
    }
}
