using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Massive Mass Outbreak data for a single area, containing multiple spawner objects and some metadata.
/// </summary>
public readonly ref struct MassiveOutbreakArea8a
{
    public const int SIZE = 0xB80;
    public const int SpawnerCount = 20;

    private readonly Span<byte> Data;

    // ReSharper disable once ConvertToPrimaryConstructor
    public MassiveOutbreakArea8a(Span<byte> data) => Data = data;

    public ulong AreaHash => ReadUInt64LittleEndian(Data);
    public bool IsActive => Data[0x8] == 1;
    public bool IsValid => AreaHash is not (0 or 0xCBF29CE484222645);

    public MassiveOutbreakSpawner8a this[int index] => new(Data.Slice(0x10 + (MassiveOutbreakSpawner8a.SIZE * index), MassiveOutbreakSpawner8a.SIZE));
}
