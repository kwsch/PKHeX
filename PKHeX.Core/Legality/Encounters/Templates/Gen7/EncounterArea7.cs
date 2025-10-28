using System;
using System.Diagnostics.CodeAnalysis;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public enum SlotType7 : byte
{
    Standard = 0,
    SOS = 1,
}

/// <summary>
/// <see cref="EntityContext.Gen7"/> encounter area
/// </summary>
public sealed record EncounterArea7 : IEncounterArea<EncounterSlot7>, IAreaLocation
{
    public EncounterSlot7[] Slots { get; }
    public GameVersion Version { get; }

    public readonly ushort Location;
    public readonly SlotType7 Type;

    public bool IsMatchLocation(ushort location) => Location == location;

    public static EncounterArea7[] GetAreas(BinLinkerAccessor input, [ConstantExpected] GameVersion version)
    {
        var result = new EncounterArea7[input.Length];
        for (int i = 0; i < result.Length; i++)
            result[i] = new EncounterArea7(input[i], version);
        return result;
    }

    private EncounterArea7(ReadOnlySpan<byte> data, [ConstantExpected] GameVersion version)
    {
        Location = ReadUInt16LittleEndian(data);
        Type = (SlotType7)data[2];
        Version = version;

        Slots = ReadSlots(data[4..]);
    }

    private EncounterSlot7[] ReadSlots(ReadOnlySpan<byte> data)
    {
        const int size = 4;
        int count = data.Length / size;
        var slots = new EncounterSlot7[count];
        for (int i = 0; i < slots.Length; i++)
        {
            int offset = size * i;
            var entry = data.Slice(offset, size);
            slots[i] = ReadSlot(entry);
        }

        return slots;
    }

    private EncounterSlot7 ReadSlot(ReadOnlySpan<byte> entry)
    {
        ushort species = ReadUInt16LittleEndian(entry);
        byte form = (byte)(species >> 11);
        species &= 0x3FF;
        byte min = entry[2];
        byte max = entry[3];
        return new EncounterSlot7(this, species, form, min, max);
    }
}
