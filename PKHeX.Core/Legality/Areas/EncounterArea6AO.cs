using System;
using System.Collections.Generic;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <inheritdoc cref="EncounterArea" />
/// <summary>
/// <see cref="GameVersion.ORAS"/> encounter area
/// </summary>
public sealed record EncounterArea6AO : EncounterArea
{
    public readonly EncounterSlot6AO[] Slots;

    protected override IReadOnlyList<EncounterSlot6AO> Raw => Slots;

    public static EncounterArea6AO[] GetAreas(BinLinkerAccessor input, GameVersion game)
    {
        var result = new EncounterArea6AO[input.Length];
        for (int i = 0; i < result.Length; i++)
            result[i] = new EncounterArea6AO(input[i], game);
        return result;
    }

    private EncounterArea6AO(ReadOnlySpan<byte> data, GameVersion game) : base(game)
    {
        Location = ReadInt16LittleEndian(data);
        Type = (SlotType)data[2];

        Slots = ReadSlots(data);
    }

    private EncounterSlot6AO[] ReadSlots(ReadOnlySpan<byte> data)
    {
        const int size = 4;
        int count = (data.Length - 4) / size;
        var slots = new EncounterSlot6AO[count];
        for (int i = 0; i < slots.Length; i++)
        {
            int offset = 4 + (size * i);
            var entry = data.Slice(offset, size);
            slots[i] = ReadSlot(entry);
        }

        return slots;
    }

    private EncounterSlot6AO ReadSlot(ReadOnlySpan<byte> entry)
    {
        ushort species = ReadUInt16LittleEndian(entry);
        byte form = (byte)(species >> 11);
        species &= 0x3FF;
        byte min = entry[2];
        byte max = entry[3];
        return new EncounterSlot6AO(this, species, form, min, max);
    }

    private const int FluteBoostMin = 4; // White Flute decreases levels.
    private const int FluteBoostMax = 4; // Black Flute increases levels.
    private const int DexNavBoost = 30; // Maximum DexNav chain

    public override IEnumerable<EncounterSlot6AO> GetMatchingSlots(PKM pk, EvoCriteria[] chain)
    {
        foreach (var slot in Slots)
        {
            foreach (var evo in chain)
            {
                if (slot.Species != evo.Species)
                    continue;

                var boostMax = Type != SlotType.Rock_Smash ? DexNavBoost : FluteBoostMax;
                const int boostMin = FluteBoostMin;
                if (!slot.IsLevelWithinRange(pk.Met_Level, boostMin, boostMax))
                    break;

                if (slot.Form != evo.Form && !slot.IsRandomUnspecificForm)
                    break;

                // Track some metadata about how this slot was matched.
                var clone = slot with
                {
                    WhiteFlute = evo.LevelMin < slot.LevelMin,
                    BlackFlute = evo.LevelMin > slot.LevelMax && evo.LevelMin <= slot.LevelMax + FluteBoostMax,
                    DexNav = slot.CanDexNav && (evo.LevelMin != slot.LevelMax || pk.RelearnMove1 != 0 || pk.AbilityNumber == 4),
                };
                yield return clone;
                break;
            }
        }
    }
}
