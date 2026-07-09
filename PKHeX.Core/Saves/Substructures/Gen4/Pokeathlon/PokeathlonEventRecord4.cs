using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Event record storage for a given event type.
/// </summary>
/// <remarks>
/// <see cref="PokeathlonEvent4"/>
/// </remarks>
public struct PokeathlonEventRecord4(Memory<byte> Raw)
{
    public const int SIZE = 8;

    public Span<byte> Data => Raw.Span;

    /// <summary>
    /// Stores the record for the particular event. The meaning of this value depends on the event type.
    /// </summary>
    /// <remarks>
    /// Hurdle Dash: Time in frames (lower is better).
    /// </remarks>
    public ushort Record { get => ReadUInt16LittleEndian(Data); set => WriteUInt16LittleEndian(Data, value); }

    // team of three Pokémon responsible for this record, simply (species,form)*3.
    public SpeciesForm10 Entry0 { get => ReadUInt16LittleEndian(Data[2..]); set => WriteUInt16LittleEndian(Data[2..], value); }
    public SpeciesForm10 Entry1 { get => ReadUInt16LittleEndian(Data[4..]); set => WriteUInt16LittleEndian(Data[4..], value); }
    public SpeciesForm10 Entry2 { get => ReadUInt16LittleEndian(Data[6..]); set => WriteUInt16LittleEndian(Data[6..], value); }
}
