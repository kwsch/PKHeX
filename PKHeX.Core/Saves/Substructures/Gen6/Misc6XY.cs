using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed class Misc6XY : SaveBlock<SAV6XY>, IMisc6
{
    public Misc6XY(SAV6XY sav, int offset) : base(sav) => Offset = offset;

    public uint Money
    {
        get => ReadUInt32LittleEndian(Data.AsSpan(Offset + 0x8));
        set => WriteUInt32LittleEndian(Data.AsSpan(Offset + 0x8), value);
    }

    public int Badges
    {
        get => Data[Offset + 0xC];
        set => Data[Offset + 0xC] = (byte)value;
    }

    public int BP
    {
        get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x3C));
        set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0x3C), (ushort)value);
    }

    public int Vivillon
    {
        get => Data[Offset + 0x50];
        set => Data[Offset + 0x50] = (byte)value;
    }
}
