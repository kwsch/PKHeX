using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Data structure containing all the spawner objects for the area.
/// </summary>
public readonly ref struct AreaSpawnerSet8a
{
    public const int EntryCount = 0x200;
    private const int MetaSize = 0x40;
    private const int MetaOffset = EntryCount * Spawner8a.SIZE;
    public const int SIZE = MetaOffset + MetaSize; // 0x88040

    public readonly Span<byte> Data;

    // Layout:
    // spawner[512]
    // metadata (count)

    public AreaSpawnerSet8a(SCBlock block) : this(block.Data) { }
    // ReSharper disable once ConvertToPrimaryConstructor
    public AreaSpawnerSet8a(Span<byte> data) => Data = data;

    private Span<byte> Meta => Data[MetaOffset..];

    public int SpawnerCount => ReadInt32LittleEndian(Meta);

    public Spawner8a this[int index] => GetEntry(index);

    public Spawner8a GetEntry(int index)
    {
        if ((uint)index >= EntryCount)
            throw new ArgumentOutOfRangeException(nameof(index));

        return new Spawner8a(Data[(Spawner8a.SIZE * index)..]);
    }

    /// <summary>
    /// Finds a <see cref="Spawner8a"/> index within this object.
    /// </summary>
    /// <param name="key">Field 01 from the Spawner FlatBuffer data.</param>
    /// <returns>Returns -1 if no match found.</returns>
    public int Find(ulong key)
    {
        var count = SpawnerCount;
        for (int i = 0; i < count; i++)
        {
            var spawner = this[i];
            if (spawner.Meta.SpawnerHash == key)
                return i;
        }
        return -1;
    }
}
