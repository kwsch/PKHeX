using System;

namespace PKHeX.Core;

/// <summary>
/// Data structure containing 8 spawn slots and extra details about the spawner.
/// </summary>
public readonly ref struct Spawner8a
{
    public const int EntryCount = 8;
    public const int SIZE = (EntryCount * SpawnerEntry8a.SIZE) + SpawnerMeta8a.SIZE;
    private readonly Span<byte> Data;

    // Layout:
    // entry[8]
    // metadata

    // ReSharper disable once ConvertToPrimaryConstructor
    public Spawner8a(Span<byte> data) => Data = data;

    public SpawnerEntry8a this[int index] => GetEntry(index);

    public SpawnerEntry8a GetEntry(int index)
    {
        if ((uint)index >= EntryCount)
            throw new ArgumentOutOfRangeException(nameof(index));

        return new SpawnerEntry8a(Data[(SpawnerEntry8a.SIZE * index)..]);
    }

    public SpawnerMeta8a Meta => new(Data[(EntryCount * SpawnerEntry8a.SIZE)..]);
    public int Count => Meta.Count;

    /// <summary>
    /// Single regeneration of a specific index.
    /// </summary>
    /// <param name="index">Entry index to regenerate.</param>
    public void Regenerate(int index)
    {
        var entry = this[index];
        (entry.GenerateSeed, entry.AlphaSeed) = Meta.Regenerate();
    }

    /// <summary>
    /// Single specific indexes.
    /// </summary>
    /// <param name="indexes">Entry indexes to regenerate.</param>
    public void Regenerate(Span<int> indexes)
    {
        Span<(ulong, ulong)> output = stackalloc (ulong, ulong)[indexes.Length];
        Meta.Regenerate(output);
        for (int index = 0; index < output.Length; index++)
        {
            var entry = this[index];
            (entry.GenerateSeed, entry.AlphaSeed) = output[index];
        }
    }

    /// <inheritdoc cref="Regenerate(Span{int})"/>
    public void Regenerate(params int[] indexes) => Regenerate(indexes.AsSpan());

    /// <summary>
    /// Regenerates all entries for the spawner.
    /// </summary>
    public void RegenerateAll()
    {
        Span<int> indexes = stackalloc int[Count];
        for (int i = 0; i < indexes.Length; i++)
            indexes[i] = i;
        Regenerate(indexes);
    }
}
