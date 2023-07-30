using System;
using System.Collections.Generic;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// <see cref="GameVersion.PLA"/> encounter area
/// </summary>
public sealed record EncounterArea8a : IEncounterArea<EncounterSlot8a>
{
    public EncounterSlot8a[] Slots { get; }
    public GameVersion Version { get; }

    private readonly byte[] Locations;
    public readonly SlotType Type;

    public int Location => Locations[0];

    public bool IsMatchLocation(int location)
    {
        return Array.IndexOf(Locations, (byte)location) != -1;
    }

    public IEnumerable<EncounterSlot8a> GetMatchingSlots(PKM pk, EvoCriteria[] chain) => GetMatches(chain, pk.Met_Level);

    private IEnumerable<EncounterSlot8a> GetMatches(EvoCriteria[] chain, int metLevel)
    {
        foreach (var slot in Slots)
        {
            foreach (var evo in chain)
            {
                if (slot.Species != evo.Species)
                    continue;

                if (!slot.IsLevelWithinRange(metLevel))
                    break;

                if (slot.Form != evo.Form && slot.Species is not ((int)Species.Rotom or (int)Species.Burmy or (int)Species.Wormadam))
                    break;

                yield return slot;
                break;
            }
        }
    }

    public static EncounterArea8a[] GetAreas(BinLinkerAccessor input, GameVersion game)
    {
        var result = new EncounterArea8a[input.Length];
        for (int i = 0; i < result.Length; i++)
            result[i] = new EncounterArea8a(input[i], game);
        return result;
    }

    private EncounterArea8a(ReadOnlySpan<byte> areaData, GameVersion game)
    {
        // Area Metadata
        int locationCount = areaData[0];
        Locations = areaData.Slice(1, locationCount).ToArray();
        Version = game;

        int align = (locationCount + 1);
        if ((align & 1) == 1)
            align++;
        areaData = areaData[align..];
        Type = areaData[0] + SlotType.Overworld;
        var count = areaData[1];

        var slots = areaData[2..];
        Slots = ReadSlots(slots, count);
    }

    private EncounterSlot8a[] ReadSlots(ReadOnlySpan<byte> areaData, byte slotCount)
    {
        var slots = new EncounterSlot8a[slotCount];
        const int bpe = 8;
        for (int i = 0; i < slotCount; i++)
        {
            var ofs = i * bpe;
            var entry = areaData.Slice(ofs, bpe);
            byte flawless = entry[7];
            var gender = (Gender)entry[6];
            byte max = entry[5];
            byte min = entry[4];
            var alpha = entry[3];
            var form = entry[2];
            var species = ReadUInt16LittleEndian(entry);

            slots[i] = new EncounterSlot8a(this, species, form, min, max, alpha, flawless, gender);
        }
        return slots;
    }
}
