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
        get => ReadUInt16LittleEndian(Data);
        set => WriteUInt16LittleEndian(Data, value);
    }

    public byte PlayedMinutes { get => Data[2]; set => Data[2] = value; }
    public byte PlayedSeconds { get => Data[3]; set => Data[3] = value; }
    public string PlayedTime => $"{PlayedHours:0000}ː{PlayedMinutes:00}ː{PlayedSeconds:00}"; // not :
}
