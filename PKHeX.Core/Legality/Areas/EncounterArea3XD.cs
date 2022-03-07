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
        public readonly EncounterSlot3PokeSpot[] Slots;

        protected override IReadOnlyList<EncounterSlot> Raw => Slots;

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

        public override IEnumerable<EncounterSlot> GetMatchingSlots(PKM pkm, IReadOnlyList<EvoCriteria> chain)
        {
            if (pkm.Format != 3) // Met Location and Met Level are changed on PK3->PK4
                return GetSlotsFuzzy(chain);
            if (pkm.Met_Location != Location)
                return Array.Empty<EncounterSlot3PokeSpot>();
            return GetSlotsMatching(chain, pkm.Met_Level);
        }

        private IEnumerable<EncounterSlot3PokeSpot> GetSlotsMatching(IReadOnlyList<EvoCriteria> chain, int lvl)
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

        private IEnumerable<EncounterSlot3PokeSpot> GetSlotsFuzzy(IReadOnlyList<EvoCriteria> chain)
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
