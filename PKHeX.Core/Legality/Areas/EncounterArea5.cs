using System;
using System.Collections.Generic;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <inheritdoc cref="EncounterArea" />
/// <summary>
/// <see cref="GameVersion.Gen5"/> encounter area
/// </summary>
public sealed record EncounterArea5 : EncounterArea
{
    public readonly EncounterSlot5[] Slots;

    protected override IReadOnlyList<EncounterSlot5> Raw => Slots;

    public static EncounterArea5[] GetAreas(BinLinkerAccessor input, GameVersion game)
    {
        var result = new EncounterArea5[input.Length];
        for (int i = 0; i < result.Length; i++)
            result[i] = new EncounterArea5(input[i], game);
        return result;
    }

    private EncounterArea5(ReadOnlySpan<byte> data, GameVersion game) : base(game)
    {
        Location = ReadUInt16LittleEndian(data);
        Type = (SlotType)data[2];

        Slots = ReadSlots(data);
    }

    private EncounterSlot5[] ReadSlots(ReadOnlySpan<byte> data)
    {
        const int size = 4;
        int count = (data.Length - 4) / size;
        var slots = new EncounterSlot5[count];
        for (int i = 0; i < slots.Length; i++)
        {
            int offset = 4 + (size * i);
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

    public override IEnumerable<EncounterSlot5> GetMatchingSlots(PKM pk, EvoCriteria[] chain)
    {
        foreach (var slot in Slots)
        {
            foreach (var evo in chain)
            {
                if (slot.Species != evo.Species)
                    continue;

                if (!slot.IsLevelWithinRange(pk.Met_Level))
                    break;

                // Deerling and Sawsbuck can change forms when seasons change, thus can be any of the [0,3] form values.
                // no other wild forms can change
                if (slot.Form != evo.Form && slot.Species is not ((int)Species.Deerling or (int)Species.Sawsbuck))
                    break;

                yield return slot;
                break;
            }
        }
    }
}
