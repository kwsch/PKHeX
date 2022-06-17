using System;
using System.ComponentModel;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Tracks the 4 select bound item slots. Size: 0x8 (4 * u16)
/// </summary>
[TypeConverter(typeof(ExpandableObjectConverter))]
public sealed class SaveItemShortcut8b : SaveBlock<SAV8BS>
{
    public SaveItemShortcut8b(SAV8BS sav, int offset) : base(sav) => Offset = offset;

    public int Item0 { get => ReadInt32LittleEndian(Data.AsSpan(Offset + 0x00)); set => WriteInt32LittleEndian(Data.AsSpan(Offset + 0x00), value); }
    public int Item1 { get => ReadInt32LittleEndian(Data.AsSpan(Offset + 0x02)); set => WriteInt32LittleEndian(Data.AsSpan(Offset + 0x02), value); }
    public int Item2 { get => ReadInt32LittleEndian(Data.AsSpan(Offset + 0x04)); set => WriteInt32LittleEndian(Data.AsSpan(Offset + 0x04), value); }
    public int Item3 { get => ReadInt32LittleEndian(Data.AsSpan(Offset + 0x06)); set => WriteInt32LittleEndian(Data.AsSpan(Offset + 0x06), value); }
}
