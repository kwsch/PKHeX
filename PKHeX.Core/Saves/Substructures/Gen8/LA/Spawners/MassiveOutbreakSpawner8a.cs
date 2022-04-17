using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Massive Mass Outbreak data for an individual spawner, indicating all useful parameters for permutation / display.
/// </summary>
public readonly ref struct MassiveOutbreakSpawner8a
{
    public const int SIZE = 0x90;

    private readonly Span<byte> Data;

    public MassiveOutbreakSpawner8a(Span<byte> data) => Data = data;

    public float X => ReadSingleLittleEndian(Data);
    public float Y => ReadSingleLittleEndian(Data[4..]);
    public float Z => ReadSingleLittleEndian(Data[8..]);

    public MassiveOutbreakSpawnerStatus Status => (MassiveOutbreakSpawnerStatus)Data[0x10];
    public ushort DisplaySpecies => ReadUInt16LittleEndian(Data[0x14..]);
    public ushort DisplayForm => ReadUInt16LittleEndian(Data[0x18..]);
    public ulong BaseTable => ReadUInt64LittleEndian(Data[0x38..]);
    public ulong BonusTable => ReadUInt64LittleEndian(Data[0x40..]);
    public ulong AguavSeed => ReadUInt64LittleEndian(Data[0x48..]);
    public ulong UnknownSeed => ReadUInt64LittleEndian(Data[0x50..]);
    public ulong SpawnSeed => ReadUInt64LittleEndian(Data[0x58..]);
    public byte BaseCount => Data[0x60];
    public uint SpawnedCount => ReadUInt32LittleEndian(Data[0x64..]);
    public ulong SpawnerName => ReadUInt64LittleEndian(Data[0x68..]);
    public byte BonusCount => Data[0x74];

    public bool HasBase => BaseTable is not (0 or 0xCBF29CE484222645);
    public bool HasBonus => BonusTable is not (0 or 0xCBF29CE484222645);
}
