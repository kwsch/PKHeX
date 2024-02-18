using System;
using System.Diagnostics.CodeAnalysis;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// <see cref="GameVersion.Gen5"/> encounter area
/// </summary>
public sealed record EncounterArea5 : IEncounterArea<EncounterSlot5>, IAreaLocation
{
    public EncounterSlot5[] Slots { get; }
    public GameVersion Version { get; }

    public readonly ushort Location;
    public readonly SlotType5 Type;

    public bool IsMatchLocation(ushort location) => Location == location;

    public static EncounterArea5[] GetAreas(BinLinkerAccessor input, [ConstantExpected] GameVersion game)
    {
        var result = new EncounterArea5[input.Length];
        for (int i = 0; i < result.Length; i++)
            result[i] = new EncounterArea5(input[i], game);
        return result;
    }

    private EncounterArea5(ReadOnlySpan<byte> data, [ConstantExpected] GameVersion game)
    {
        Location = ReadUInt16LittleEndian(data);
        Type = (SlotType5)data[2];
        Version = game;

        Slots = ReadSlots(data[4..]);
    }

    private EncounterSlot5[] ReadSlots(ReadOnlySpan<byte> data)
    {
        const int size = 4;
        int count = data.Length / size;
        var slots = new EncounterSlot5[count];
        for (int i = 0; i < slots.Length; i++)
        {
            int offset = size * i;
            var entry = data.Slice(offset, size);
            slots[i] = ReadSlot(entry);
        }

        return slots;
    }

    private EncounterSlot5 ReadSlot(ReadOnlySpan<byte> entry)
    {
        ushort species = ReadUInt16LittleEndian(entry);
        byte form = (byte)(species >> 11);
        species &= 0x3FF;
        byte min = entry[2];
        byte max = entry[3];
        return new EncounterSlot5(this, species, form, min, max);
    }
}

public enum SlotType5 : byte
{
    Standard = 0,
    Grass = 1,
    Surf = 2,
    Super_Rod = 3,

    Swarm = 4,
    HiddenGrotto = 5,
}
