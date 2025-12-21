using System;
using System.Diagnostics.CodeAnalysis;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// <see cref="EntityContext.Gen4"/> encounter area
/// </summary>
public sealed record EncounterArea4 : IEncounterArea<EncounterSlot4>, IGroundTypeTile, IAreaLocation
{
    public EncounterSlot4[] Slots { get; }
    public GameVersion Version { get; }
    public SlotType4 Type { get; }
    public GroundTileAllowed GroundTile { get; }

    public readonly byte Location;
    public readonly byte Rate;

    public bool IsMatchLocation(ushort location) => location == Location;

    public static EncounterArea4[] GetAreas(BinLinkerAccessor input, [ConstantExpected] GameVersion version)
    {
        var result = new EncounterArea4[input.Length];
        for (int i = 0; i < result.Length; i++)
            result[i] = new EncounterArea4(input[i], version);
        return result;
    }

    private EncounterArea4(ReadOnlySpan<byte> data, [ConstantExpected] GameVersion version)
    {
        Location = data[0];
        // data[1] is unused because location is always <= 255.
        Type = (SlotType4)data[2];
        Rate = data[3];
        Version = version;
        // although flags are 32bit, none have values > 16bit.
        GroundTile = (GroundTileAllowed)ReadUInt16LittleEndian(data[4..]);

        Slots = ReadRegularSlots(data[6..]);
    }

    private EncounterSlot4[] ReadRegularSlots(ReadOnlySpan<byte> data)
    {
        const int size = 10;
        int count = data.Length / size;
        var slots = new EncounterSlot4[count];
        for (int i = 0; i < slots.Length; i++)
        {
            int offset = size * i;
            var entry = data.Slice(offset, size);
            slots[i] = ReadRegularSlot(entry);
        }

        return slots;
    }

    private EncounterSlot4 ReadRegularSlot(ReadOnlySpan<byte> entry)
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
        return new EncounterSlot4(this, species, form, min, max, slotNum, mpi, mpc, sti, stc);
    }

    public byte GetPressureMax(ushort species, byte levelMax)
    {
        foreach (var slot in Slots)
        {
            if (slot.Species != species)
                continue;
            if (slot.LevelMax <= levelMax)
                continue;
            levelMax = slot.LevelMax;
        }
        return levelMax;
    }

    public bool IsMunchlaxTree(ITrainerID32 pk) => IsMunchlaxTree(pk, Location);

    public bool IsCoronetFeebasArea => Rate is byte.MaxValue; // Sentinel only for Fishing slots.

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

    private static ReadOnlySpan<byte> LocationID_HoneyTree =>
    [
        20, // 00 Route 205 Floaroma
        20, // 01 Route 205 Eterna
        21, // 02 Route 206
        22, // 03 Route 207
        23, // 04 Route 208
        24, // 05 Route 209
        25, // 06 Route 210 Solaceon
        25, // 07 Route 210 Celestic
        26, // 08 Route 211
        27, // 09 Route 212 Hearthome
        27, // 10 Route 212 Pastoria
        28, // 11 Route 213
        29, // 12 Route 214
        30, // 13 Route 215
        33, // 14 Route 218
        36, // 15 Route 221
        37, // 16 Route 222
        47, // 17 Valley Windworks
        48, // 18 Eterna Forest
        49, // 19 Fuego Ironworks
        58, // 20 Floaroma Meadow
    ];

    public static bool IsUnownFormValid(PKM pk, byte form, bool isRuinsOfAlph) => isRuinsOfAlph
        ?   RuinsOfAlph4.IsFormValid(pk, form)
        : SolaceonRuins4.IsFormValid(pk, form);
}

/// <summary>
/// Wild Encounter data <see cref="IEncounterTemplate"/> Type
/// </summary>
/// <remarks>
/// Different from <see cref="GroundTileAllowed"/>, this corresponds to the method that the <see cref="IEncounterTemplate"/> may be encountered.</remarks>
public enum SlotType4 : byte
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

    /// <summary> Using Headbutt on capable trees </summary>
    Headbutt = 6,
    /// <summary> Using Headbutt on special trees </summary>
    HeadbuttSpecial = 7,
    /// <summary> Grass tiles during a Bug Catching Contest </summary>
    BugContest = 8,
    /// <summary> Shaking Honey Trees </summary>
    HoneyTree = 9,

    /// <summary> Grass tiles in the Safari Zone </summary>
    Safari_Grass = 10,
    /// <summary> Water tiles in the Safari Zone </summary>
    Safari_Surf = 11,
    /// <summary> Fishing with Old Rod in the Safari Zone </summary>
    Safari_Old_Rod = 12,
    /// <summary> Fishing with Good Rod in the Safari Zone </summary>
    Safari_Good_Rod = 13,
    /// <summary> Fishing with Super Rod in the Safari Zone </summary>
    Safari_Super_Rod = 14,
}

public static class SlotType4Extensions
{
    extension(SlotType4 type)
    {
        /// <summary>
        /// Checks if the <see cref="type"/> is an encounter within the Safari Zone.
        /// </summary>
        public bool IsSafari => type >= SlotType4.Safari_Grass;

        /// <summary>
        /// Checks if the <see cref="type"/> has a level range that is random. For D/P/Pt; this is all types except Grass.
        /// </summary>
        public bool IsLevelRandDPPt => type != SlotType4.Grass;

        /// <summary>
        /// Checks if the <see cref="type"/> has a level range that is random. For HG/SS; this is all types except Grass and Safari.
        /// </summary>
        public bool IsLevelRandHGSS => type != SlotType4.Grass && !type.IsSafari;
    }
}
