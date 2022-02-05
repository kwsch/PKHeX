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
    public readonly int ParentLocation;

    protected override IReadOnlyList<EncounterSlot> Raw => Slots;

    public override bool IsMatchLocation(int location)
    {
        if (base.IsMatchLocation(location))
            return true;
        return CanCrossoverTo(location);
    }

    private bool CanCrossoverTo(int location)
    {
        return location == ParentLocation;
    }

    public override IEnumerable<EncounterSlot> GetMatchingSlots(PKM pkm, IReadOnlyList<EvoCriteria> chain) => GetMatches(chain, pkm.Met_Level);

    private IEnumerable<EncounterSlot8a> GetMatches(IReadOnlyList<EvoCriteria> chain, int metLevel)
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
        Location = areaData[0];
        ParentLocation = areaData[1];
        Type = areaData[2] + SlotType.Overworld;
        var count = areaData[3];

        var slots = areaData[4..];
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
            int max = entry[5];
            int min = entry[4];
            var alpha = entry[3];
            var form = entry[2];
            var species = ReadUInt16LittleEndian(entry);

            slots[i] = new EncounterSlot8a(this, species, form, min, max, alpha, flawless, gender);
        }
        return slots;
    }
}
