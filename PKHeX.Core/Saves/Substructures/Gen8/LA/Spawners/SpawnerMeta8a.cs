using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Data structure containing metadata about a <see cref="Spawner8a"/>.
/// </summary>
public readonly ref struct SpawnerMeta8a
{
    public const int SIZE = 0x40;
    private readonly Span<byte> Data;

    // ReSharper disable once ConvertToPrimaryConstructor
    public SpawnerMeta8a(Span<byte> data) => Data = data;

    public ulong CountSeed { get => ReadUInt64LittleEndian(Data);       set => WriteUInt64LittleEndian(Data        , value); }

    /// <summary> Seed that regenerates seeds for the entries as a group, regenerating multiple or single entries. </summary>
    public ulong GroupSeed  { get => ReadUInt64LittleEndian(Data[0x08..]); set => WriteUInt64LittleEndian(Data[0x08..], value); }

    // flatbuffer PlacementSpawner8a.Field_01 to match
    public ulong SpawnerHash { get => ReadUInt64LittleEndian(Data[0x10..]); set => WriteUInt64LittleEndian(Data[0x10..], value); }

    public int Count        { get => ReadInt32LittleEndian (Data[0x18..]); set => WriteInt32LittleEndian (Data[0x18..], value); }
    public int Flags        { get => ReadInt32LittleEndian (Data[0x1C..]); set => WriteInt32LittleEndian (Data[0x1C..], value); }

    // Flags?
    public bool IsOutbreak => (Flags & 0x40) != 0; // 0x40
    public bool IsRegular => (Flags & 0x80) != 0; // 0x80
    public bool IsInactive => (Flags & 0x100) != 0; // 1 for no spawns in this spawner

    /// <summary>
    /// Creates a new entry seed pair and updates the <see cref="GroupSeed"/>.
    /// </summary>
    public (ulong GenerateSeed, ulong AlphaSeed) Regenerate()
    {
        var rand = new Xoroshiro128Plus(GroupSeed);
        var result = (rand.Next(), rand.Next());
        GroupSeed = rand.Next();
        return result;
    }

    /// <summary>
    /// Creates new entry seed pairs and updates the <see cref="GroupSeed"/>.
    /// </summary>
    /// <param name="output">Result buffer</param>
    public void Regenerate(Span<(ulong GenerateSeed, ulong AlphaSeed)> output)
    {
        var rand = new Xoroshiro128Plus(GroupSeed);
        for (int i = 0; i < output.Length; i++)
            output[i] = (rand.Next(), rand.Next());
        GroupSeed = rand.Next();
    }

    /// <summary>
    /// Gets the next count of entities to be present for a given appearance cycle.
    /// </summary>
    /// <param name="min">Minimum spawn count</param>
    /// <param name="max">Maximum spawn count</param>
    /// <returns>Count for the cycle.</returns>
    /// <remarks>Does not advance the <see cref="CountSeed"/> if the input <see cref="min"/> and <see cref="max"/> are equivalent.</remarks>
    public int GetNextQuantity(int min, int max)
    {
        if (min == max)
            return min;
        var delta = max - min;
        var rand = new Xoroshiro128Plus(CountSeed);
        var result = (int)rand.NextInt((uint)delta + 1);
        CountSeed = rand.Next();
        return result;
    }
}
