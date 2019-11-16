using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core
{
    /// <inheritdoc />
    /// <summary>
    /// <see cref="GameVersion.GBCartEraOnly"/> encounter area
    /// </summary>
    public abstract class EncounterAreaGB : EncounterArea
    {
        /// <summary>
        /// RBY Format Slot Getter from data.
        /// </summary>
        /// <param name="data">Byte array containing complete slot data table.</param>
        /// <param name="ofs">Offset to start reading from.</param>
        /// <param name="count">Amount of slots to read.</param>
        /// <param name="t">Type of encounter slot.</param>
        /// <param name="rate">Slot type encounter rate.</param>
        /// <returns>Array of encounter slots.</returns>
        protected static EncounterSlot1[] ReadSlots1(byte[] data, ref int ofs, int count, SlotType t, int rate)
        {
            EncounterSlot1[] slots = new EncounterSlot1[count];
            for (int i = 0; i < count; i++)
            {
                int lvl = data[ofs++];
                int spec = data[ofs++];

                slots[i] = new EncounterSlot1
                {
                    LevelMax = t == SlotType.Surf ? lvl + 4 : lvl,
                    LevelMin = lvl,
                    Species = spec,
                    Type = t,
                    Rate = rate,
                    SlotNumber = i,
                };
            }
            return slots;
        }

        public override IEnumerable<EncounterSlot> GetMatchingSlots(PKM pkm, IReadOnlyList<EvoCriteria> vs, int minLevel = 0)
        {
            if (minLevel == 0) // any
                return Slots.Where(slot => vs.Any(evo => evo.Species == slot.Species));

            var Gen1Version = GameVersion.RBY;
            bool RBDragonair = false;

            if (minLevel != 0 && !FilterGBSlotsCatchRate(pkm, ref vs, ref Gen1Version, ref RBDragonair))
                return Enumerable.Empty<EncounterSlot>();

            var encounterSlots = GetMatchFromEvoLevel(pkm, vs, minLevel);
            return GetFilteredSlots(pkm, encounterSlots, Gen1Version, RBDragonair).OrderBy(slot => slot.LevelMin); // prefer lowest levels
        }

        private static bool FilterGBSlotsCatchRate(PKM pkm, ref IReadOnlyList<EvoCriteria> vs, ref GameVersion Gen1Version, ref bool RBDragonair)
        {
            if (!(pkm is PK1 pk1) || !pkm.Gen1_NotTradeback)
                return true;

            // Pure gen 1, slots can be filter by catch rate
            var rate = pk1.Catch_Rate;
            switch (pkm.Species)
            {
                // Pikachu
                case (int)Species.Pikachu when rate == 163:
                case (int)Species.Raichu when rate == 163:
                    return false; // Yellow Pikachu is not a wild encounter

                // Kadabra (YW)
                case (int)Species.Kadabra when rate == 96:
                case (int)Species.Alakazam when rate == 96:
                    vs = vs.Where(s => s.Species == (int)Species.Kadabra).ToArray();
                    Gen1Version = GameVersion.YW;
                    return true;

                // Kadabra (RB)
                case (int)Species.Kadabra when rate == 100:
                case (int)Species.Alakazam when rate == 100:
                    vs = vs.Where(s => s.Species == (int)Species.Kadabra).ToArray();
                    Gen1Version = GameVersion.RB;
                    return true;

                // Dragonair (YW)
                case (int)Species.Dragonair when rate == 27:
                case (int)Species.Dragonite when rate == 27:
                    vs = vs.Where(s => s.Species == (int)Species.Dragonair).ToArray(); // Yellow Dragonair, ignore Dratini encounters
                    Gen1Version = GameVersion.YW;
                    return true;

                // Dragonair (RB)
                case (int)Species.Dragonair:
                case (int)Species.Dragonite:
                    // Red blue dragonair have the same catch rate as dratini, it could also be a dratini from any game
                    vs = vs.Where(s => rate == PersonalTable.RB[s.Species].CatchRate).ToArray();
                    RBDragonair = true;
                    return true;

                default:
                    vs = vs.Where(s => rate == PersonalTable.RB[s.Species].CatchRate).ToArray();
                    return true;
            }
        }

        private static IEnumerable<EncounterSlot> GetFilteredSlots(PKM pkm, IEnumerable<EncounterSlot> slots,
            GameVersion Gen1Version, bool RBDragonair)
        {
            int gen = pkm.GenNumber;
            switch (gen)
            {
                case 1:
                    if (Gen1Version != GameVersion.RBY)
                        slots = slots.Where(slot => Gen1Version.Contains(slot.Version));

                    // Red Blue dragonair or dratini from any gen 1 games
                    if (RBDragonair)
                        return slots.Where(slot => GameVersion.RB.Contains(slot.Version) || slot.Species == 147);

                    return slots;

                case 2:
                    if (pkm is PK2 pk2 && pk2.Met_TimeOfDay != 0)
                        return slots.Where(slot => ((EncounterSlot1)slot).Time.Contains(pk2.Met_TimeOfDay));
                    return slots;

                default:
                    return slots;
            }
        }

        protected override IEnumerable<EncounterSlot> GetMatchFromEvoLevel(PKM pkm, IEnumerable<EvoCriteria> vs, int minLevel)
        {
            var slots = Slots.Where(slot => vs.Any(evo => evo.Species == slot.Species && evo.Level >= slot.LevelMin));

            if (pkm.Format >= 7 || !(pkm is PK2 pk2 && pk2.CaughtData != 0)) // transferred to Gen7+, or does not have Crystal met data
                return slots.Where(slot => slot.LevelMin <= minLevel);
            return slots.Where(s => s.IsLevelWithinRange(minLevel));
        }
    }
}