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

    // ReSharper disable once ConvertToPrimaryConstructor
    public MassiveOutbreakSpawner8a(Span<byte> data) => Data = data;

    public float X { get => ReadSingleLittleEndian(Data);      set => WriteSingleLittleEndian(Data, value); }
    public float Y { get => ReadSingleLittleEndian(Data[4..]); set => WriteSingleLittleEndian(Data[4..], value); }
    public float Z { get => ReadSingleLittleEndian(Data[8..]); set => WriteSingleLittleEndian(Data[8..], value); }

    public MassiveOutbreakSpawnerStatus Status => (MassiveOutbreakSpawnerStatus)Data[0x10];
    public ushort DisplaySpecies { get => ReadUInt16LittleEndian(Data[0x14..]); set => WriteUInt16LittleEndian(Data[0x14..], value); }
    public ushort DisplayForm    { get => ReadUInt16LittleEndian(Data[0x18..]); set => WriteUInt16LittleEndian(Data[0x18..], value); }

    public ulong BaseTable  { get => ReadUInt64LittleEndian(Data[0x38..]); set => WriteUInt64LittleEndian(Data[0x38..], value); }
    public ulong BonusTable { get => ReadUInt64LittleEndian(Data[0x40..]); set => WriteUInt64LittleEndian(Data[0x40..], value); }
    public ulong AguavSeed  { get => ReadUInt64LittleEndian(Data[0x48..]); set => WriteUInt64LittleEndian(Data[0x48..], value); }
    public ulong CountSeed  { get => ReadUInt64LittleEndian(Data[0x50..]); set => WriteUInt64LittleEndian(Data[0x50..], value); }
    public ulong GroupSeed  { get => ReadUInt64LittleEndian(Data[0x58..]); set => WriteUInt64LittleEndian(Data[0x58..], value); }

    public byte BaseCount    { get => Data[0x60]; set => Data[0x60] = value; }
    public uint SpawnedCount { get => ReadUInt32LittleEndian(Data[0x64..]); set => WriteUInt32LittleEndian(Data[0x64..], value); }
    public ulong SpawnerName { get => ReadUInt64LittleEndian(Data[0x68..]); set => WriteUInt64LittleEndian(Data[0x68..], value); }

    public byte BonusCount   { get => Data[0x74]; set => Data[0x74] = value; }

    public bool HasBase => BaseTable is not (0 or 0xCBF29CE484222645);
    public bool HasBonus => BonusTable is not (0 or 0xCBF29CE484222645);
}
