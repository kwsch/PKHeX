using System;
using static System.Buffers.Binary.BinaryPrimitives;
using static PKHeX.Core.PokedexResearchTaskType8a;

namespace PKHeX.Core;

/// <summary>
/// Research Task definition for <see cref="GameVersion.PLA"/> Pokédex entries.
/// </summary>
public sealed class PokedexResearchTask8a
{
    public readonly PokedexResearchTaskType8a Task;
    public readonly int Threshold;
    public readonly int Move;
    public readonly MoveType Type;
    public readonly PokedexTimeOfDay8a TimeOfDay;
    public readonly ulong Hash_06;
    public readonly ulong Hash_07;
    public readonly ulong Hash_08;
    public readonly byte[] TaskThresholds;
    public readonly int PointsSingle;
    public readonly int PointsBonus;
    public readonly bool RequiredForCompletion;
    public readonly int Index;

    private const int SIZE = 0x28;

    public PokedexResearchTask8a() : this(stackalloc byte[SIZE]) { }

    private PokedexResearchTask8a(ReadOnlySpan<byte> data)
    {
        Task = (PokedexResearchTaskType8a)data[0x00];
        PointsSingle = data[0x01];
        PointsBonus = data[0x02];
        Threshold = data[0x03];
        Move = ReadUInt16LittleEndian(data[0x04..]);
        Type = (MoveType)data[0x06];
        TimeOfDay = (PokedexTimeOfDay8a)data[0x07];
        Hash_06 = ReadUInt64LittleEndian(data[0x08..]);
        Hash_07 = ReadUInt64LittleEndian(data[0x10..]);
        Hash_08 = ReadUInt64LittleEndian(data[0x18..]);
        TaskThresholds = data.Slice(0x21, data[0x20]).ToArray();
        RequiredForCompletion = data[0x26] != 0;

        Index = Task is UseMove or DefeatWithMoveType ? data[0x27] : -1;
    }

    public static PokedexResearchTask8a[] DeserializeFrom(ReadOnlySpan<byte> data)
    {
        // 00: u8 task
        // 01: u8 points_single
        // 02: u8 points_bonus
        // 03: u8 threshold
        // 04: u16 move
        // 06: u8 type
        // 07: u8 time of day
        // 08: u64 hash_06
        // 10: u64 hash_07
        // 18: u64 hash_08
        // 20: u8 num_thresholds
        // 21: u8 thresholds[5]
        // 26: u8 required
        // 27: u8 multi_index

        var result = new PokedexResearchTask8a[data.Length / SIZE];
        for (var i = 0; i < result.Length; i++)
            result[i] = new PokedexResearchTask8a(data.Slice(SIZE * i, SIZE));
        return result;
    }
}
