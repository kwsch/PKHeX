using System;
using System.Diagnostics.CodeAnalysis;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public enum SlotType6 : byte
{
    Standard = 0,
    Grass = 1,
    Surf = 2,
    Old_Rod = 3,
    Good_Rod = 4,
    Super_Rod = 5,
    Rock_Smash = 6,

    Horde = 7,
    FriendSafari = 8,
}

/// <summary>
/// <see cref="GameVersion.XY"/> encounter area
/// </summary>
public sealed record EncounterArea6XY : IEncounterArea<EncounterSlot6XY>, IAreaLocation
{
    public EncounterSlot6XY[] Slots { get; }
    public GameVersion Version { get; }

    public readonly ushort Location;
    public readonly SlotType6 Type;

    public bool IsMatchLocation(ushort location) => Location == location;

    public static EncounterArea6XY[] GetAreas(BinLinkerAccessor input, [ConstantExpected] GameVersion game, EncounterArea6XY safari)
    {
        int count = input.Length;
        var result = new EncounterArea6XY[count + 1];
        for (int i = 0; i < count; i++)
            result[i] = new EncounterArea6XY(input[i], game);
        result[^1] = safari;
        return result;
    }

    public EncounterArea6XY()
    {
        Location = 148; // Friend Safari
        Type = SlotType6.FriendSafari;
        Version = GameVersion.XY;

        Slots = LoadSafariSlots();
    }

    private EncounterArea6XY(ReadOnlySpan<byte> data, [ConstantExpected] GameVersion game)
    {
        Location = ReadUInt16LittleEndian(data);
        Type = (SlotType6)data[2];
        Version = game;

        Slots = ReadSlots(data[4..]);
    }

    private EncounterSlot6XY[] LoadSafariSlots()
    {
        const int SpeciesFormSlots = 4;
        const byte Level = 30;

        // Single form species
        var species = AllFriendSafariSpecies;
        var slots = new EncounterSlot6XY[species.Length + SpeciesFormSlots];
        int i = 0;
        for (; i < species.Length; i++)
            slots[i] = new EncounterSlot6XY(this, species[i], 0, Level, Level);

        // Floette has 3 separate forms (RBY)
        slots[i++] = new EncounterSlot6XY(this, (int)Species.Floette, 0, Level, Level);
        slots[i++] = new EncounterSlot6XY(this, (int)Species.Floette, 1, Level, Level);
        slots[i++] = new EncounterSlot6XY(this, (int)Species.Floette, 3, Level, Level);

        // Region Random Vivillon
        slots[i] = new EncounterSlot6XY(this, (int)Species.Vivillon, EncounterUtil.FormVivillon, Level, Level);
        return slots;
    }

    /// <summary>
    /// All species available in the Friend Safari.
    /// </summary>
    /// <remarks> Handle Vivillon (666) and Floette (670) separately </remarks>
    private static ReadOnlySpan<ushort> AllFriendSafariSpecies =>
    [
        002, 005, 008, 012, 014, 016, 021, 025, 027, 035,
        038, 039, 043, 044, 046, 049, 049, 051, 056, 058,
        061, 063, 067, 077, 082, 083, 084, 087, 089, 091,
        095, 096, 098, 101, 105, 112, 113, 114, 125, 126,
        127, 130, 131, 132, 133, 148, 163, 165, 168, 175,
        178, 184, 190, 191, 194, 195, 202, 203, 205, 206,
        209, 213, 214, 215, 215, 216, 218, 219, 221, 222,
        224, 225, 227, 231, 235, 236, 247, 262, 267, 268,
        274, 281, 284, 286, 290, 294, 297, 299, 302, 303,
        303, 307, 310, 313, 314, 317, 323, 326, 328, 332,
        336, 342, 352, 353, 356, 357, 359, 361, 363, 372,
        375, 400, 404, 415, 417, 419, 423, 426, 437, 442,
        444, 447, 452, 454, 459, 506, 510, 511, 513, 515,
        517, 520, 523, 525, 527, 530, 531, 536, 538, 539,
        541, 544, 548, 551, 556, 557, 561, 569, 572, 575,
        578, 581, 586, 587, 596, 597, 600, 608, 611, 614,
        618, 619, 621, 623, 624, 627, 629, 636, 651, 654,
        657, 660, 662, 662, 668, 673, 674, 677, 682, 684,
        686, 689, 694, 701, 702, 702, 705, 707, 708, 710,
        712, 714,
    ];

    private EncounterSlot6XY[] ReadSlots(ReadOnlySpan<byte> data)
    {
        const int size = 4;
        int count = data.Length / size;
        var slots = new EncounterSlot6XY[count];
        for (int i = 0; i < slots.Length; i++)
        {
            int offset = size * i;
            var entry = data.Slice(offset, size);
            ushort species = ReadUInt16LittleEndian(entry);
            byte form = (byte)(species >> 11);
            species &= 0x3FF;
            byte min = entry[2];
            byte max = entry[3];
            slots[i] = new EncounterSlot6XY(this, species, form, min, max);
        }

        return slots;
    }
}
