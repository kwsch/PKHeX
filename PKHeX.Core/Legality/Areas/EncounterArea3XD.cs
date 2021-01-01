using System;
using System.Collections.Generic;

namespace PKHeX.Core
{
    /// <inheritdoc cref="EncounterArea" />
    /// <summary>
    /// <see cref="GameVersion.XD"/> encounter area
    /// </summary>
    public sealed record EncounterArea3XD : EncounterArea
    {
        public EncounterArea3XD(int loc, int s0, int l0, int s1, int l1, int s2, int l2) : base(GameVersion.XD)
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

        public override IEnumerable<EncounterSlot> GetMatchingSlots(PKM pkm, IReadOnlyList<EvoCriteria> chain)
        {
            if (pkm.Format != 3) // Met Location and Met Level are changed on PK3->PK4
                return GetSlotsFuzzy(chain);
            if (pkm.Met_Location != Location)
                return Array.Empty<EncounterSlot>();
            return GetSlotsMatching(chain, pkm.Met_Level);
        }

        private IEnumerable<EncounterSlot> GetSlotsMatching(IReadOnlyList<EvoCriteria> chain, int lvl)
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

        private IEnumerable<EncounterSlot> GetSlotsFuzzy(IReadOnlyList<EvoCriteria> chain)
        {
            foreach (var slot in Slots)
            {
                foreach (var evo in chain)
                {
                    if (slot.Species != evo.Species)
                        continue;

                    if (slot.Form != evo.Form)
                        break;
                    if (slot.LevelMin > evo.Level)
                        break;

                    yield return slot;
                    break;
                }
            }
        }
    }
}
