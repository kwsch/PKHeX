using System;
using System.ComponentModel;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Stores the amount of time the save file has been played.
/// </summary>
[TypeConverter(typeof(ExpandableObjectConverter))]
public sealed class PlayTime8a(SAV8LA sav, SCBlock block) : SaveBlock<SAV8LA>(sav, block.Data)
{
    public ushort PlayedHours
    {
        get => ReadUInt16LittleEndian(Data.AsSpan(Offset));
        set => WriteUInt16LittleEndian(Data.AsSpan(Offset), value);
    }

    public byte PlayedMinutes { get => Data[Offset + 2]; set => Data[Offset + 2] = value; }
    public byte PlayedSeconds { get => Data[Offset + 3]; set => Data[Offset + 3] = value; }
    public string LastSavedTime => $"{PlayedHours:0000}ː{PlayedMinutes:00}ː{PlayedSeconds:00}"; // not :
}
