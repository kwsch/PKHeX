using System;
using System.Collections.Generic;

namespace PKHeX.Core;

/// <summary>
/// <see cref="GameVersion.XD"/> encounter area
/// </summary>
public sealed record EncounterArea3XD : IVersion, IEncounterArea<EncounterSlot3PokeSpot>, IAreaLocation
{
    public EncounterSlot3PokeSpot[] Slots { get; }
    public SlotType Type => SlotType.Grass;
    public GameVersion Version => GameVersion.XD;
    public readonly byte Location;

    public bool IsMatchLocation(int location) => location == Location;

    public EncounterArea3XD(byte loc, ushort s0, byte l0, ushort s1, byte l1, ushort s2, byte l2)
    {
        Location = loc;
        Slots = new[]
        {
            new EncounterSlot3PokeSpot(this, s0, 10, l0, 0),
            new EncounterSlot3PokeSpot(this, s1, 10, l1, 1),
            new EncounterSlot3PokeSpot(this, s2, 10, l2, 2),
        };
    }

    public IEnumerable<EncounterSlot3PokeSpot> GetMatchingSlots(PKM pk, EvoCriteria[] chain)
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
