using System;
using System.ComponentModel;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Details about berry tree plots.
/// </summary>
/// <remarks>size: 0x808</remarks>
[TypeConverter(typeof(ExpandableObjectConverter))]
public sealed class BerryTreeGrowSave8b(SAV8BS sav, Memory<byte> raw) : SaveBlock<SAV8BS>(sav, raw)
{
    public const int KinomiGrowsCount = 128;

    public const int KinomiSize = 0x10;
    // structure:
    // KinomiGrow[] kinomiGrows; // 0x0
    // long LastUpdateMinutes; // 0x8

    public long LastUpdateMinutes { get => ReadInt64LittleEndian(Data[0x800..]); set => WriteInt64LittleEndian(Data[0x800..], value); }
}
