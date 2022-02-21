using System;
using System.Buffers.Binary;

namespace PKHeX.Core;

/// <summary>
/// Data structure representing a predetermined Pokémon-to-be-encountered, with fixed randomness and details like position if already determined.
/// </summary>
/// <remarks>
/// The game takes the <see cref="Seed_00"/> to roll for which encounter slot, then uses the next rand() to generate the entity's data when interacted with.
/// </remarks>
public readonly ref struct SpawnerEntry8a
{
    public const int SIZE = 0x80;
    private readonly Span<byte> Data;

    public SpawnerEntry8a(Span<byte> data) => Data = data;

    public float Coordinate_00 { get => ReadSingleLittleEndian(Data); set => WriteSingleLittleEndian(Data, value); }
    public float Coordinate_04 { get => ReadSingleLittleEndian(Data[0x04..]); set => WriteSingleLittleEndian(Data[0x04..], value); }
    public float Coordinate_08 { get => ReadSingleLittleEndian(Data[0x08..]); set => WriteSingleLittleEndian(Data[0x08..], value); }
    // 10 - float?
    public float Field_14 { get => ReadSingleLittleEndian(Data[0x14..]); set => WriteSingleLittleEndian(Data[0x14..], value); }
    // 18 - float?
    public float Field_1C { get => ReadSingleLittleEndian(Data[0x1C..]); set => WriteSingleLittleEndian(Data[0x1C..], value); }
    public ulong Seed_00  { get => BinaryPrimitives.ReadUInt64LittleEndian(Data[0x20..]); set => BinaryPrimitives.WriteUInt64LittleEndian(Data[0x20..], value); }

    public float Field_34 { get => ReadSingleLittleEndian(Data[0x34..]); set => WriteSingleLittleEndian(Data[0x34..], value); }
    public float Field_38 { get => ReadSingleLittleEndian(Data[0x38..]); set => WriteSingleLittleEndian(Data[0x38..], value); }
    public float Field_3C { get => ReadSingleLittleEndian(Data[0x3C..]); set => WriteSingleLittleEndian(Data[0x3C..], value); }

    public byte  IsEmpty  { get => Data[0x43]; set => Data[0x43] = value; } // 0 for slots with data, 1 for empty
    public byte  Field_45 { get => Data[0x45]; set => Data[0x45] = value; }
    public byte  Field_46 { get => Data[0x46]; set => Data[0x46] = value; }
    public short Field_48 { get => BinaryPrimitives.ReadInt16LittleEndian (Data[0x48..]); set => BinaryPrimitives.WriteInt16LittleEndian (Data[0x48..], value); }
    public byte  Field_4A { get => Data[0x4A]; set => Data[0x4A] = value; }
    public ulong Seed_01  { get => BinaryPrimitives.ReadUInt64LittleEndian(Data[0x58..]); set => BinaryPrimitives.WriteUInt64LittleEndian(Data[0x58..], value); }
}
