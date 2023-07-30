using System;
using System.Collections.Generic;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// <see cref="GameVersion.Gen7"/> encounter area
/// </summary>
public sealed record EncounterArea7 : IEncounterArea<EncounterSlot7>
{
    public EncounterSlot7[] Slots { get; }
    public GameVersion Version { get; }

    public readonly ushort Location;
    public readonly SlotType Type;

    public bool IsMatchLocation(int location) => Location == location;

    public static EncounterArea7[] GetAreas(BinLinkerAccessor input, GameVersion game)
    {
        var result = new EncounterArea7[input.Length];
        for (int i = 0; i < result.Length; i++)
            result[i] = new EncounterArea7(input[i], game);
        return result;
    }

    private EncounterArea7(ReadOnlySpan<byte> data, GameVersion game)
    {
        Location = ReadUInt16LittleEndian(data);
        Type = (SlotType)data[2];
        Version = game;

        Slots = ReadSlots(data);
    }

    private EncounterSlot7[] ReadSlots(ReadOnlySpan<byte> data)
    {
        const int size = 4;
        int count = (data.Length - 4) / size;
        var slots = new EncounterSlot7[count];
        for (int i = 0; i < slots.Length; i++)
        {
            int offset = 4 + (size * i);
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

    public IEnumerable<EncounterSlot7> GetMatchingSlots(PKM pk, EvoCriteria[] chain)
    {
        foreach (var slot in Slots)
        {
            foreach (var evo in chain)
            {
                if (slot.Species != evo.Species)
                    continue;

                if (!slot.IsLevelWithinRange(pk.Met_Level))
                    break;

                if (slot.Form != evo.Form && slot.Species is not ((int)Species.Furfrou or (int)Species.Oricorio))
                {
                    if (!slot.IsRandomUnspecificForm) // Minior, etc
                        break;
                }

                yield return slot;
                break;
            }
        }
    }
}
