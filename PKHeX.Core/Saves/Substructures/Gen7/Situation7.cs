using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed class Situation7(SAV7 sav, Memory<byte> raw) : SaveBlock<SAV7>(sav, raw)
{
    // "StartLocation"
    public int M { get => ReadUInt16LittleEndian(Data); set => WriteUInt16LittleEndian(Data, (ushort)value); }
    public float X { get => ReadSingleLittleEndian(Data[0x08..]); set => WriteSingleLittleEndian(Data[0x08..], value); }
    public float Z { get => ReadSingleLittleEndian(Data[0x0C..]); set => WriteSingleLittleEndian(Data[0x0C..], value); }
    public float Y { get => ReadSingleLittleEndian(Data[0x10..]); set => WriteSingleLittleEndian(Data[0x10..], value); }
    public float RX { get => ReadSingleLittleEndian(Data[0x14..]); set => WriteSingleLittleEndian(Data[0x14..], value); }
    public float RZ { get => ReadSingleLittleEndian(Data[0x18..]); set => WriteSingleLittleEndian(Data[0x18..], value); }
    public float RY { get => ReadSingleLittleEndian(Data[0x1C..]); set => WriteSingleLittleEndian(Data[0x1C..], value); }
    public float RW { get => ReadSingleLittleEndian(Data[0x20..]); set => WriteSingleLittleEndian(Data[0x20..], value); }

    public void UpdateOverworldCoordinates()
    {
        var o = SAV.Overworld;
        o.X = X;
        o.Z = Z;
        o.Y = Y;
        o.RX = RX;
        o.RZ = RZ;
        o.RY = RY;
        o.RW = RW;
    }

    public int SpecialLocation
    {
        get => Data[0x24];
        set => Data[0x24] = (byte)value;
    }

    public int WarpContinueRequest
    {
        get => Data[0x6E];
        set => Data[0x6E] = (byte)value;
    }

    public int StepCountEgg
    {
        get => ReadInt32LittleEndian(Data[0x70..]);
        set => WriteInt32LittleEndian(Data[0x70..], value);
    }

    public int LastZoneID
    {
        get => ReadUInt16LittleEndian(Data[0x74..]);
        set => WriteUInt16LittleEndian(Data[0x74..], (ushort)value);
    }

    public int StepCountFriendship
    {
        get => ReadUInt16LittleEndian(Data[0x76..]);
        set => WriteUInt16LittleEndian(Data[0x76..], (ushort)value);
    }

    public int StepCountAffection // Kawaigari
    {
        get => ReadUInt16LittleEndian(Data[0x78..]);
        set => WriteUInt16LittleEndian(Data[0x78..], (ushort)value);
    }
}
