using System;
using System.Diagnostics.CodeAnalysis;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// <see cref="GameVersion.GG"/> encounter area
/// </summary>
public sealed record EncounterArea7b : IEncounterArea<EncounterSlot7b>, IAreaLocation
{
    public EncounterSlot7b[] Slots { get; }
    public GameVersion Version { get; }

    public readonly ushort Location;

    public bool IsMatchLocation(ushort location) => Location == location;

    public static EncounterArea7b[] GetAreas(BinLinkerAccessor input, [ConstantExpected] GameVersion game)
    {
        var result = new EncounterArea7b[input.Length];
        for (int i = 0; i < result.Length; i++)
            result[i] = new EncounterArea7b(input[i], game);
        return result;
    }

    private EncounterArea7b(ReadOnlySpan<byte> data, [ConstantExpected] GameVersion game)
    {
        Location = ReadUInt16LittleEndian(data);
        Version = game;
        Slots = ReadSlots(data[2..]);
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
        // form is always 0
        byte min = entry[2];
        byte max = entry[3];
        return new EncounterSlot7b(this, species, min, max);
    }
}
