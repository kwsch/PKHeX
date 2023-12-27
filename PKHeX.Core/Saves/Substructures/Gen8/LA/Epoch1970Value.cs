using System;
using System.ComponentModel;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Stores the <see cref="Timestamp"/> to indicate the seconds since 1970 that an event occurred.
/// </summary>
[TypeConverter(typeof(ExpandableObjectConverter))]
public sealed class Epoch1970Value(Memory<byte> Data)
{
    private Span<byte> Span => Data.Span;

    public Epoch1970Value(SCBlock block) : this(block.Data) { }

    /// <summary>
    /// time_t (seconds since 1970 Epoch)
    /// </summary>
    public ulong Seconds
    {
        get => ReadUInt64LittleEndian(Span);
        set => WriteUInt64LittleEndian(Span, value);
    }

    private static DateTime Epoch => new(1970, 1, 1);

    public string DisplayValue => $"{Timestamp.Year:0000}-{Timestamp.Month:00}-{Timestamp.Day:00} {Timestamp.Hour:00}ː{Timestamp.Minute:00}ː{Timestamp.Second:00}"; // not :

    public DateTime Timestamp
    {
        get => Epoch.AddSeconds(Seconds);
        set => Seconds = (ulong)value.Subtract(Epoch).TotalSeconds;
    }
}
