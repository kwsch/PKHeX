using System;
using System.Collections.Generic;

namespace PKHeX.Core;

/// <inheritdoc cref="EncounterArea" />
/// <summary>
/// <see cref="GameVersion.XD"/> encounter area
/// </summary>
public sealed record EncounterArea3XD : EncounterArea
{
    public readonly EncounterSlot3PokeSpot[] Slots;

    protected override IReadOnlyList<EncounterSlot3PokeSpot> Raw => Slots;

    public EncounterArea3XD(int loc, ushort s0, byte l0, ushort s1, byte l1, ushort s2, byte l2) : base(GameVersion.XD)
    {
        Location = loc;
        Type = SlotType.Grass;
        Slots = new[]
        {
            new EncounterSlot3PokeSpot(this, s0, 10, l0, 0),
            new EncounterSlot3PokeSpot(this, s1, 10, l1, 1),
            new EncounterSlot3PokeSpot(this, s2, 10, l2, 2),
        };
    }

    public override IEnumerable<EncounterSlot3PokeSpot> GetMatchingSlots(PKM pk, EvoCriteria[] chain)
    {
        if (pk.Format != 3) // Met Location and Met Level are changed on PK3->PK4
            return GetSlotsFuzzy(chain);
        if (pk.Met_Location != Location)
            return Array.Empty<EncounterSlot3PokeSpot>();
        return GetSlotsMatching(chain, pk.Met_Level);
    }

    private IEnumerable<EncounterSlot3PokeSpot> GetSlotsMatching(EvoCriteria[] chain, int lvl)
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

    private IEnumerable<EncounterSlot3PokeSpot> GetSlotsFuzzy(EvoCriteria[] chain)
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
