using System;
using System.Collections.Generic;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <inheritdoc cref="EncounterArea" />
/// <summary>
/// <see cref="GameVersion.Gen3"/> encounter area
/// </summary>
public sealed record EncounterArea3 : EncounterArea
{
    public readonly int Rate;
    public readonly EncounterSlot3[] Slots;

    protected override IReadOnlyList<EncounterSlot> Raw => Slots;

    public static EncounterArea3[] GetAreas(BinLinkerAccessor input, GameVersion game)
    {
        var result = new EncounterArea3[input.Length];
        for (int i = 0; i < result.Length; i++)
            result[i] = new EncounterArea3(input[i], game);
        return result;
    }

    public static EncounterArea3[] GetAreasSwarm(BinLinkerAccessor input, GameVersion game)
    {
        var result = new EncounterArea3[input.Length];
        for (int i = 0; i < result.Length; i++)
            result[i] = new EncounterArea3(input[i], game, SlotType.Swarm | SlotType.Grass);
        return result;
    }

    private EncounterArea3(ReadOnlySpan<byte> data, GameVersion game) : base(game)
    {
        Location = ReadInt16LittleEndian(data);
        Type = (SlotType)data[2];
        Rate = data[3];

        Slots = ReadRegularSlots(data);
    }

    private EncounterArea3(ReadOnlySpan<byte> data, GameVersion game, SlotType type) : base(game)
    {
        Location = ReadInt16LittleEndian(data);
        Type = type;
        Rate = data[3];

        Slots = ReadSwarmSlots(data);
    }

    private EncounterSlot3[] ReadRegularSlots(ReadOnlySpan<byte> data)
    {
        const int size = 10;
        int count = (data.Length - 4) / size;
        var slots = new EncounterSlot3[count];
        for (int i = 0; i < slots.Length; i++)
        {
            int offset = 4 + (size * i);
            var entry = data.Slice(offset, size);
            slots[i] = ReadRegularSlot(entry);
        }

        return slots;
    }

    private EncounterSlot3 ReadRegularSlot(ReadOnlySpan<byte> entry)
    {
        ushort species = ReadUInt16LittleEndian(entry);
        byte form = entry[2];
        byte slotNum = entry[3];
        byte min = entry[4];
        byte max = entry[5];

        byte mpi = entry[6];
        byte mpc = entry[7];
        byte sti = entry[8];
        byte stc = entry[9];
        return new EncounterSlot3(this, species, form, min, max, slotNum, mpi, mpc, sti, stc);
    }

    private EncounterSlot3[] ReadSwarmSlots(ReadOnlySpan<byte> data)
    {
        const int size = 14;
        int count = (data.Length - 4) / size;
        var slots = new EncounterSlot3[count];
        for (int i = 0; i < slots.Length; i++)
        {
            int offset = 4 + (size * i);
            var entry = data.Slice(offset, size);
            slots[i] = ReadSwarmSlot(entry);
        }

        return slots;
    }

    private EncounterSlot3Swarm ReadSwarmSlot(ReadOnlySpan<byte> entry)
    {
        ushort species = ReadUInt16LittleEndian(entry);
        // form always 0
        byte slotNum = entry[3];
        byte min = entry[4];
        byte max = entry[5];

        var moves = new Moveset(
            ReadUInt16LittleEndian(entry[06..]),
            ReadUInt16LittleEndian(entry[08..]),
            ReadUInt16LittleEndian(entry[10..]),
            ReadUInt16LittleEndian(entry[12..])
        );

        return new EncounterSlot3Swarm(this, species, min, max, slotNum, moves);
    }

    public override IEnumerable<EncounterSlot> GetMatchingSlots(PKM pk, EvoCriteria[] chain)
    {
        if (pk.Format != 3) // Met Location and Met Level are changed on PK3->PK4
            return GetSlotsFuzzy(chain);
        if (pk.Met_Location != Location)
            return Array.Empty<EncounterSlot>();
        return GetSlotsMatching(chain, pk.Met_Level);
    }

    private IEnumerable<EncounterSlot3> GetSlotsMatching(EvoCriteria[] chain, int lvl)
    {
        foreach (var slot in Slots)
        {
            foreach (var evo in chain)
            {
                if (slot.Species != evo.Species)
                    continue;

                if (slot.Form != evo.Form)
                    break;
                if (!slot.IsLevelWithinRange(lvl))
                    break;

                yield return slot;
                break;
            }
        }
    }

    private IEnumerable<EncounterSlot3> GetSlotsFuzzy(EvoCriteria[] chain)
    {
        foreach (var slot in Slots)
        {
            foreach (var evo in chain)
            {
                if (slot.Species != evo.Species)
                    continue;

                if (slot.Form != evo.Form)
                    break;
                if (slot.LevelMin > evo.LevelMax)
                    break;

                yield return slot;
                break;
            }
        }
    }
}
