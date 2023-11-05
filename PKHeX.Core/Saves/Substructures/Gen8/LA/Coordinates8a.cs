using System;
using System.ComponentModel;
using System.Text;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Stores the position of the player.
/// </summary>
[TypeConverter(typeof(ExpandableObjectConverter))]
public sealed class Coordinates8a : SaveBlock<SAV8LA>
{
    public Coordinates8a(SAV8LA sav, SCBlock block) : base(sav, block.Data) { }

    // Map
    public string M
    {
        get => Util.TrimFromZero(Encoding.ASCII.GetString(Data, 0x08, 0x48));
        set
        {
            for (int i = 0; i < 0x48; i++)
                Data[0x08 + i] = (byte)(value.Length > i ? value[i] : '\0');
        }
    }

    // Position
    public float X { get => ReadSingleLittleEndian(Data.AsSpan(Offset + 0x50)); set => WriteSingleLittleEndian(Data.AsSpan(Offset + 0x50), value); }
    public float Z { get => ReadSingleLittleEndian(Data.AsSpan(Offset + 0x54)); set => WriteSingleLittleEndian(Data.AsSpan(Offset + 0x54), value); }
    public float Y { get => ReadSingleLittleEndian(Data.AsSpan(Offset + 0x58)); set => WriteSingleLittleEndian(Data.AsSpan(Offset + 0x58), value); }

    // Rotation
    public float RX { get => ReadSingleLittleEndian(Data.AsSpan(Offset + 0x60)); set => WriteSingleLittleEndian(Data.AsSpan(Offset + 0x60), value); }
    public float RZ { get => ReadSingleLittleEndian(Data.AsSpan(Offset + 0x64)); set => WriteSingleLittleEndian(Data.AsSpan(Offset + 0x64), value); }
    public float RY { get => ReadSingleLittleEndian(Data.AsSpan(Offset + 0x68)); set => WriteSingleLittleEndian(Data.AsSpan(Offset + 0x68), value); }
    public float RW { get => ReadSingleLittleEndian(Data.AsSpan(Offset + 0x6C)); set => WriteSingleLittleEndian(Data.AsSpan(Offset + 0x6C), value); }
}
