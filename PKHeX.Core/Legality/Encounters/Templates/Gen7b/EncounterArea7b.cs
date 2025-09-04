using System;
using System.Diagnostics.CodeAnalysis;

namespace PKHeX.Core;

/// <summary>
/// <see cref="GameVersion.GG"/> encounter area
/// </summary>
public sealed record EncounterArea7b : IEncounterArea<EncounterSlot7b>, IAreaLocation
{
    public EncounterSlot7b[] Slots { get; }
    public GameVersion Version { get; }

    public readonly byte Location;
    public readonly byte ToArea1;
    public readonly byte ToArea2; // None have more than 2 areas to feed into.

    public bool IsMatchLocation(ushort location)
    {
        if (Location == location)
            return true;
        // Check if it matches either crossover location.
        if (location is 0)
            return false;
        return location == ToArea1 || location == ToArea2;
    }

    public static EncounterArea7b[] GetAreas(BinLinkerAccessor input, [ConstantExpected] GameVersion version)
    {
        var result = new EncounterArea7b[input.Length];
        for (int i = 0; i < result.Length; i++)
            result[i] = new EncounterArea7b(input[i], version);
        return result;
    }

    private EncounterArea7b(ReadOnlySpan<byte> data, [ConstantExpected] GameVersion version)
    {
        Location = data[0]; // Always < 255
        ToArea1 = data[2];
        ToArea2 = data[3];
        Version = version;
        Slots = ReadSlots(data[4..]);
    }

    private EncounterSlot7b[] ReadSlots(ReadOnlySpan<byte> data)
    {
        const int size = 4;
        int count = data.Length / size;
        var slots = new EncounterSlot7b[count];
        for (int i = 0; i < slots.Length; i++)
        {
            int offset = size * i;
            var entry = data.Slice(offset, size);
            slots[i] = ReadSlot(entry);
        }

        return slots;
    }

    private EncounterSlot7b ReadSlot(ReadOnlySpan<byte> entry)
    {
        ushort species = entry[0]; // always < 255; only original 151
        // form is always 0 so is not included in this data.
        byte flags = entry[1];
        byte min = entry[2];
        byte max = entry[3];
        return new EncounterSlot7b(this, species, min, max, flags);
    }
}
