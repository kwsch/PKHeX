using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Stores the position of the player.
/// </summary>
public sealed class Coordinates7b(SAV7b sav, Memory<byte> raw) : SaveBlock<SAV7b>(sav, raw)
{
    // Position
    public float X { get => ReadSingleLittleEndian(Data[0x10..]); set => WriteSingleLittleEndian(Data[0x10..], value); }
    public float Z { get => ReadSingleLittleEndian(Data[0x14..]); set => WriteSingleLittleEndian(Data[0x14..], value); }
    public float Y { get => ReadSingleLittleEndian(Data[0x18..]); set => WriteSingleLittleEndian(Data[0x18..], value); }

    // Scale
    public float SX { get => ReadSingleLittleEndian(Data[0x20..]); set => WriteSingleLittleEndian(Data[0x20..], value); }
    public float SZ { get => ReadSingleLittleEndian(Data[0x24..]); set => WriteSingleLittleEndian(Data[0x24..], value); }
    public float SY { get => ReadSingleLittleEndian(Data[0x28..]); set => WriteSingleLittleEndian(Data[0x28..], value); }

    // Rotation
    public float RX { get => ReadSingleLittleEndian(Data[0x30..]); set => WriteSingleLittleEndian(Data[0x30..], value); }
    public float RZ { get => ReadSingleLittleEndian(Data[0x34..]); set => WriteSingleLittleEndian(Data[0x34..], value); }
    public float RY { get => ReadSingleLittleEndian(Data[0x38..]); set => WriteSingleLittleEndian(Data[0x38..], value); }
    public float RW { get => ReadSingleLittleEndian(Data[0x3C..]); set => WriteSingleLittleEndian(Data[0x3C..], value); }

    // Map
    public ulong M { get => ReadUInt64LittleEndian(Data[0x6000..]); set => WriteUInt64LittleEndian(Data[0x6000..], value); }
}
