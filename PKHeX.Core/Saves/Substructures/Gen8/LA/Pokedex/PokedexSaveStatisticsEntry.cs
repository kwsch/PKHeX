using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Per-species/form research logs used for <see cref="GameVersion.PLA"/> Pokédex entries.
/// </summary>
public sealed class PokedexSaveStatisticsEntry(Memory<byte> raw)
{
    private Span<byte> Data => raw.Span;

    public const int SIZE = 0x18;

    public uint Flags { get => ReadUInt32LittleEndian(Data); set => WriteUInt32LittleEndian(Data, value); }
    public bool HasMaximumStatistics { get => (Flags & (1u << 0)) != 0; set => Flags = (Flags & ~(1u << 0)) | ((value ? 1u : 0u) << 0); }
    public byte SeenInWildFlags { get => Data[4]; set => Data[4] = value; }
    public byte ObtainFlags { get => Data[5]; set => Data[5] = value; }
    public byte CaughtInWildFlags { get => Data[6]; set => Data[6] = value; }
    // 7 padding alignment

    public float MinHeight { get => ReadSingleLittleEndian(Data[0x08..]); set => WriteSingleLittleEndian(Data[0x08..], value); }
    public float MaxHeight { get => ReadSingleLittleEndian(Data[0x0C..]); set => WriteSingleLittleEndian(Data[0x0C..], value); }
    public float MinWeight { get => ReadSingleLittleEndian(Data[0x10..]); set => WriteSingleLittleEndian(Data[0x10..], value); }
    public float MaxWeight { get => ReadSingleLittleEndian(Data[0x14..]); set => WriteSingleLittleEndian(Data[0x14..], value); }
}
