using System;
using System.Diagnostics.CodeAnalysis;
using static System.Buffers.Binary.BinaryPrimitives;
using static PKHeX.Core.SlotType3;

namespace PKHeX.Core;

/// <summary>
/// <see cref="EntityContext.Gen3"/> encounter area
/// </summary>
public sealed record EncounterArea3 : IEncounterArea<EncounterSlot3>, IAreaLocation
{
    public EncounterSlot3[] Slots { get; }
    public GameVersion Version { get; }
    public SlotType3 Type { get; }

    public readonly byte Rate;
    public readonly byte Location;

    public bool IsMatchLocation(ushort location) => location == Location;

    public static EncounterArea3[] GetAreas(BinLinkerAccessor input, [ConstantExpected] GameVersion version)
    {
        var result = new EncounterArea3[input.Length];
        for (int i = 0; i < result.Length; i++)
            result[i] = new EncounterArea3(input[i], version);
        return result;
    }

    public static EncounterArea3[] GetAreasSwarm(BinLinkerAccessor input, [ConstantExpected] GameVersion version)
    {
        var result = new EncounterArea3[input.Length];
        for (int i = 0; i < result.Length; i++)
            result[i] = new EncounterArea3(input[i], version, SwarmGrass50);
        return result;
    }

    private EncounterArea3(ReadOnlySpan<byte> data, [ConstantExpected] GameVersion version)
    {
        Location = data[0];
        // data[1] is unused because location is always <= 255.
        Type = (SlotType3)data[2];
        Rate = data[3];
        Version = version;

        Slots = ReadRegularSlots(data[4..]);
    }

    private EncounterArea3(ReadOnlySpan<byte> data, [ConstantExpected] GameVersion version, [ConstantExpected] SlotType3 type)
    {
        Location = data[0];
        // data[1] is unused because location is always <= 255.
        Type = type; // data[2] but it's always the same value
        Rate = data[3];
        Version = version;

        Slots = ReadSwarmSlots(data[4..]);
    }

    private EncounterSlot3[] ReadRegularSlots(ReadOnlySpan<byte> data)
    {
        const int size = 10;
        int count = data.Length / size;
        var slots = new EncounterSlot3[count];
        for (int i = 0; i < slots.Length; i++)
        {
            int offset = size * i;
            var entry = data.Slice(offset, size);
            slots[i] = ReadRegularSlot(entry);
        }

        return slots;
    }

    private EncounterSlot3 ReadRegularSlot(ReadOnlySpan<byte> entry)
    {
        ushort species = ReadUInt16LittleEndian(entry);
        byte form = entry[2];
        byte slotNum = entry[3];
        byte min = entry[4];
        byte max = entry[5];

        byte mpi = entry[6];
        byte mpc = entry[7];
        byte sti = entry[8];
        byte stc = entry[9];
        return new EncounterSlot3(this, species, form, min, max, slotNum, mpi, mpc, sti, stc);
    }

    private EncounterSlot3[] ReadSwarmSlots(ReadOnlySpan<byte> data)
    {
        const int size = 14;
        int count = data.Length / size;
        var slots = new EncounterSlot3[count];
        for (int i = 0; i < slots.Length; i++)
        {
            int offset = size * i;
            var entry = data.Slice(offset, size);
            slots[i] = ReadSwarmSlot(entry);
        }

        return slots;
    }

    private EncounterSlot3Swarm ReadSwarmSlot(ReadOnlySpan<byte> entry)
    {
        ushort species = ReadUInt16LittleEndian(entry);
        // form always 0
        byte slotNum = entry[3];
        byte min = entry[4];
        byte max = entry[5];

        var moves = new Moveset(
            ReadUInt16LittleEndian(entry[06..]),
            ReadUInt16LittleEndian(entry[08..]),
            ReadUInt16LittleEndian(entry[10..]),
            ReadUInt16LittleEndian(entry[12..])
        );

        return new EncounterSlot3Swarm(this, species, min, max, slotNum, moves);
    }

    public byte GetPressureMax(ushort species, byte levelMax)
    {
        foreach (var slot in Slots)
        {
            if (slot.Species != species)
                continue;
            if (slot.LevelMax < levelMax)
                continue;
            levelMax = slot.LevelMax;
        }
        return levelMax;
    }
}

/// <summary>
/// Wild Encounter data <see cref="IEncounterTemplate"/> Type
/// </summary>
public enum SlotType3 : byte
{
    /// <summary> Grass tiles </summary>
    Grass = 0,
    /// <summary> Water tiles </summary>
    Surf = 1,
    /// <summary> Fishing with Old Rod </summary>
    Old_Rod = 2,
    /// <summary> Fishing with Good Rod </summary>
    Good_Rod = 3,
    /// <summary> Fishing with Super Rod </summary>
    Super_Rod = 4,
    /// <summary> Using Rock Smash move </summary>
    Rock_Smash = 5,

    /// <summary> Swarm in grass tiles </summary>
    SwarmGrass50 = 6,
    /// <summary> Swarm in water tiles </summary>
    SwarmFish50 = 7,
}
