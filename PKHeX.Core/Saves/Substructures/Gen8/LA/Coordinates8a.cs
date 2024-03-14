using System;
using System.ComponentModel;
using System.Text;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Stores the position of the player.
/// </summary>
[TypeConverter(typeof(ExpandableObjectConverter))]
public sealed class Coordinates8a(SAV8LA sav, SCBlock block) : SaveBlock<SAV8LA>(sav, block.Data)
{
    // Map
    private Span<byte> MapName() => Data.Slice(0x08, 0x48);

    public string M
    {
        get
        {
            var span = MapName();
            var trim = span.IndexOf<byte>(0);
            if (trim >= 0)
                span = span[..trim];
            return Encoding.ASCII.GetString(span);
        }
        set
        {
            var span = MapName();
            span.Clear();
            Encoding.ASCII.GetBytes(value, span);
        }
    }

    // Position
    public float X { get => ReadSingleLittleEndian(Data[0x50..]); set => WriteSingleLittleEndian(Data[0x50..], value); }
    public float Z { get => ReadSingleLittleEndian(Data[0x54..]); set => WriteSingleLittleEndian(Data[0x54..], value); }
    public float Y { get => ReadSingleLittleEndian(Data[0x58..]); set => WriteSingleLittleEndian(Data[0x58..], value); }

    // Rotation
    public float RX { get => ReadSingleLittleEndian(Data[0x60..]); set => WriteSingleLittleEndian(Data[0x60..], value); }
    public float RZ { get => ReadSingleLittleEndian(Data[0x64..]); set => WriteSingleLittleEndian(Data[0x64..], value); }
    public float RY { get => ReadSingleLittleEndian(Data[0x68..]); set => WriteSingleLittleEndian(Data[0x68..], value); }
    public float RW { get => ReadSingleLittleEndian(Data[0x6C..]); set => WriteSingleLittleEndian(Data[0x6C..], value); }
}
