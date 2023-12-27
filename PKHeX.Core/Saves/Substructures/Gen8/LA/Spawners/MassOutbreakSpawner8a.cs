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

    // ReSharper disable once ConvertToPrimaryConstructor
    public MassOutbreakSpawner8a(Span<byte> data) => Data = data;

    public ushort DisplaySpecies { get => ReadUInt16LittleEndian(Data); set => WriteUInt16LittleEndian(Data, value); }
    public ushort DisplayForm { get => ReadUInt16LittleEndian(Data[4..]); set => WriteUInt16LittleEndian(Data[4..], value); }

    public ulong AreaHash { get => ReadUInt64LittleEndian(Data[0x18..]); set => WriteUInt64LittleEndian(Data[0x18..], value); }
    public float X { get => ReadSingleLittleEndian(Data[0x20..]); set => WriteSingleLittleEndian(Data[0x20..], value); }
    public float Y { get => ReadSingleLittleEndian(Data[0x24..]); set => WriteSingleLittleEndian(Data[0x24..], value); }
    public float Z { get => ReadSingleLittleEndian(Data[0x28..]); set => WriteSingleLittleEndian(Data[0x28..], value); }
    public ulong CountSeed { get=> ReadUInt64LittleEndian(Data[0x30..]); set => WriteUInt64LittleEndian (Data[0x30..], value); }
    public ulong GroupSeed { get => ReadUInt64LittleEndian(Data[0x38..]); set => WriteUInt64LittleEndian(Data[0x38..], value); }
    public byte BaseCount { get => Data[0x40]; set => Data[0x40] = value; }

    public uint SpawnedCount { get => ReadUInt32LittleEndian(Data[0x44..]); set => WriteUInt32LittleEndian(Data[0x44..], value); }

    public bool HasOutbreak => AreaHash is not (0 or 0xCBF29CE484222645);
    public bool IsValid => DisplaySpecies is not 0;
}
