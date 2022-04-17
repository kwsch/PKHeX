using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Mass Outbreak data for an individual spawner, indicating all useful parameters for permutation / display.
/// </summary>
public readonly ref struct MassOutbreakSpawner8a
{
    public const int SIZE = 0x50;

    private readonly Span<byte> Data;

    public MassOutbreakSpawner8a(Span<byte> data) => Data = data;

    public ushort DisplaySpecies => ReadUInt16LittleEndian(Data);
    public ushort DisplayForm => ReadUInt16LittleEndian(Data[4..]);
    public ulong AreaHash => ReadUInt64LittleEndian(Data[0x18..]);
    public float X => ReadSingleLittleEndian(Data[0x20..]);
    public float Y => ReadSingleLittleEndian(Data[0x24..]);
    public float Z => ReadSingleLittleEndian(Data[0x28..]);
    public ulong UnknownSeed => ReadUInt64LittleEndian(Data[0x30..]);
    public ulong SpawnSeed => ReadUInt64LittleEndian(Data[0x38..]);
    public byte BaseCount => Data[0x40];
    public uint SpawnedCount => ReadUInt32LittleEndian(Data[0x44..]);
    public bool HasOutbreak => AreaHash is not (0 or 0xCBF29CE484222645);
    public bool IsValid => DisplaySpecies is not 0;
}
