using System;
using System.ComponentModel;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Playtime storage
/// </summary>
[TypeConverter(typeof(ExpandableObjectConverter))]
public sealed class PlayTime8b(SAV8BS sav, Memory<byte> raw) : SaveBlock<SAV8BS>(sav, raw)
{
    public ushort PlayedHours
    {
        get => ReadUInt16LittleEndian(Data);
        set => WriteUInt16LittleEndian(Data, value);
    }

    public byte PlayedMinutes { get => Data[2]; set => Data[2] = value; }
    public byte PlayedSeconds { get => Data[3]; set => Data[3] = value; }
    public string LastSavedTime => $"{PlayedHours:0000}ː{PlayedMinutes:00}ː{PlayedSeconds:00}"; // not :
}
