using System;
using System.Diagnostics.CodeAnalysis;
using static System.Buffers.Binary.BinaryPrimitives;
using static PKHeX.Core.SlotType8b;

namespace PKHeX.Core;

/// <summary>
/// <see cref="GameVersion.BDSP"/> encounter area
/// </summary>
public sealed record EncounterArea8b : IEncounterArea<EncounterSlot8b>, IAreaLocation
{
    public EncounterSlot8b[] Slots { get; }
    public GameVersion Version { get; }

    public readonly ushort Location;
    public readonly SlotType8b Type;

    public static EncounterArea8b[] GetAreas(BinLinkerAccessor input, [ConstantExpected] GameVersion game)
    {
        var result = new EncounterArea8b[input.Length];
        for (int i = 0; i < result.Length; i++)
            result[i] = new EncounterArea8b(input[i], game);
        return result;
    }

    private EncounterArea8b(ReadOnlySpan<byte> data, [ConstantExpected] GameVersion game)
    {
        Location = ReadUInt16LittleEndian(data);
        Type = (SlotType8b)data[2];
        Version = game;

        Slots = ReadSlots(data[4..]);
    }

    private EncounterSlot8b[] ReadSlots(ReadOnlySpan<byte> data)
    {
        const int size = 4;
        int count = data.Length / size;
        var slots = new EncounterSlot8b[count];
        for (int i = 0; i < slots.Length; i++)
        {
            int offset = size * i;
            var entry = data.Slice(offset, size);
            slots[i] = ReadSlot(entry);
        }
        return slots;
    }

    private EncounterSlot8b ReadSlot(ReadOnlySpan<byte> data)
    {
        ushort species = ReadUInt16LittleEndian(data);
        byte form = (byte)(species >> 11);
        species &= 0x3FF;
        byte min = data[2];
        byte max = data[3];
        return new EncounterSlot8b(this, species, form, min, max);
    }

    public bool IsMatchLocation(ushort location)
    {
        if (location == Location)
            return true;
        return CanCrossoverTo(location);
    }

    private bool CanCrossoverTo(ushort location)
    {
        if (Type is Surf)
        {
            return Location switch
            {
                486 => location is 167, // Route 223 -> Pokémon League
                167 => location is 486, // Pokémon League -> Route 223
                420 => location is 489, // Route 229 -> Route 230
                489 => location is 420, // Route 230 -> Route 229

                // All other crossover surf locations are identical slot lists.
                _ => false,
            };
        }

        // No crossover
        return false;
    }

    public bool IsMunchlaxTree(ITrainerID32 pk) => IsMunchlaxTree(pk, Location);

    private static bool IsMunchlaxTree(ITrainerID32 pk, ushort location)
    {
        // We didn't encode the honey tree index to the encounter slot resource.
        // Check if any of the slot's location doesn't match any of the groupC trees' area location ID.
        Span<byte> trees = stackalloc byte[4];
        HoneyTreeUtil.CalculateMunchlaxTrees(pk.ID32, trees);
        return IsMunchlaxTree(trees, location);
    }

    private static bool IsMunchlaxTree(ReadOnlySpan<byte> trees, ushort location)
    {
        return LocationID_HoneyTree[trees[0]] == location
            || LocationID_HoneyTree[trees[1]] == location
            || LocationID_HoneyTree[trees[2]] == location
            || LocationID_HoneyTree[trees[3]] == location;
    }

    private static ReadOnlySpan<ushort> LocationID_HoneyTree =>
    [
        359, // 00 Route 205 Floaroma
        361, // 01 Route 205 Eterna
        362, // 02 Route 206
        364, // 03 Route 207
        365, // 04 Route 208
        367, // 05 Route 209
        373, // 06 Route 210 Solaceon
        375, // 07 Route 210 Celestic
        378, // 08 Route 211
        379, // 09 Route 212 Hearthome
        383, // 10 Route 212 Pastoria
        385, // 11 Route 213
        392, // 12 Route 214
        394, // 13 Route 215
        400, // 14 Route 218
        404, // 15 Route 221
        407, // 16 Route 222
        197, // 17 Valley Windworks
        199, // 18 Eterna Forest
        201, // 19 Fuego Ironworks
        253, // 20 Floaroma Meadow
    ];
}

public enum SlotType8b : byte
{
    // Unused; relic from previous PKHeX codebase and pkl not updated to remove.
    Any = 0,

    Grass = 1,
    Surf = 2,
    Old_Rod = 3,
    Good_Rod = 4,
    Super_Rod = 5,
    Rock_Smash = 6,

    // Unused; relic from previous PKHeX codebase and pkl not updated to remove.
    Headbutt = 7,

    HoneyTree = 8,
}
