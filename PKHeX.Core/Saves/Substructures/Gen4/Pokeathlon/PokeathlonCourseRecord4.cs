using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public struct PokeathlonCourseRecord4(Memory<byte> Raw)
{
    public const int SIZE = 0x2C;

    private Span<byte> Data => Raw.Span;

    public ushort Score0 { get => ReadUInt16LittleEndian(Data); set => WriteUInt16LittleEndian(Data, value); }
    public ushort Score1 { get => ReadUInt16LittleEndian(Data[2..]); set => WriteUInt16LittleEndian(Data[2..], value); }
    public ushort Score2 { get => ReadUInt16LittleEndian(Data[4..]); set => WriteUInt16LittleEndian(Data[4..], value); }
    public ushort ScoreMax { get => ReadUInt16LittleEndian(Data[6..]); set => WriteUInt16LittleEndian(Data[6..], value); }

    public const int CountParticipant = 3;

    public PokeathlonParticipant4 GetParticipant(int index)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual<uint>((uint)index, CountParticipant);
        return new(Raw.Slice(8 + (index * PokeathlonParticipant4.SIZE), PokeathlonParticipant4.SIZE));
    }
}
