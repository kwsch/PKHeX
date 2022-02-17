using System;
using System.Buffers.Binary;

namespace PKHeX.Core;

/// <summary>
/// Data structure containing metadata about a <see cref="Spawner8a"/>.
/// </summary>
public readonly ref struct SpawnerMeta8a
{
    public const int SIZE = 0x40;
    private readonly Span<byte> Data;

    public SpawnerMeta8a(Span<byte> data) => Data = data;

    public ulong Seed_00    { get => BinaryPrimitives.ReadUInt64LittleEndian(Data);         set => BinaryPrimitives.WriteUInt64LittleEndian(Data        , value); }

    /// <summary> Seed that regenerates seeds for the entries as a group, regenerating multiple or single entries. </summary>
    public ulong GroupSeed  { get => BinaryPrimitives.ReadUInt64LittleEndian(Data[0x08..]); set => BinaryPrimitives.WriteUInt64LittleEndian(Data[0x08..], value); }

    // flatbuffer Field_01 to match
    public ulong Spawner_01 { get => BinaryPrimitives.ReadUInt64LittleEndian(Data[0x10..]); set => BinaryPrimitives.WriteUInt64LittleEndian(Data[0x10..], value); }

    public int Count        { get => BinaryPrimitives.ReadInt32LittleEndian (Data[0x18..]); set => BinaryPrimitives.WriteInt32LittleEndian (Data[0x18..], value); }
    public int Flags        { get => BinaryPrimitives.ReadInt32LittleEndian (Data[0x1C..]); set => BinaryPrimitives.WriteInt32LittleEndian (Data[0x1C..], value); }

    // Flags?
    public bool IsOutbreak => (Flags & 0x40) != 0; // 0x40
    public bool IsRegular => (Flags & 0x80) != 0; // 0x80
    public bool IsInactive => (Flags & 0x100) != 0; // 1 for no spawns in this spawner

    /// <summary>
    /// Creates a new entry seed pair and updates the <see cref="GroupSeed"/>.
    /// </summary>
    public (ulong, ulong) Regenerate()
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
    public void Regenerate(Span<(ulong, ulong)> output)
    {
        var rand = new Xoroshiro128Plus(GroupSeed);
        for (int i = 0; i < output.Length; i++)
            output[i] = (rand.Next(), rand.Next());
        GroupSeed = rand.Next();
    }
}
