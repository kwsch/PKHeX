using System;
using System.Collections.Generic;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <inheritdoc cref="EncounterArea" />
/// <summary>
/// <see cref="GameVersion.SWSH"/> encounter area
/// </summary>
public sealed record EncounterArea8a : EncounterArea
{
    public readonly EncounterSlot8a[] Slots;
    private readonly byte[] Locations;

    protected override IReadOnlyList<EncounterSlot8a> Raw => Slots;

    public override bool IsMatchLocation(int location)
    {
        return Array.IndexOf(Locations, (byte)location) != -1;
    }

    public override IEnumerable<EncounterSlot8a> GetMatchingSlots(PKM pk, EvoCriteria[] chain) => GetMatches(chain, pk.Met_Level);

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

    private EncounterArea8a(ReadOnlySpan<byte> areaData, GameVersion game) : base(game)
    {
        // Area Metadata
        int locationCount = areaData[0];
        Locations = areaData.Slice(1, locationCount).ToArray();
        Location = Locations[0];

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
