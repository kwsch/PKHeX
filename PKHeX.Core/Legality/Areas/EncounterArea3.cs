using System;
using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core
{
    /// <inheritdoc />
    /// <summary>
    /// <see cref="GameVersion.Gen3"/> encounter area
    /// </summary>
    public sealed class EncounterArea3 : EncounterArea
    {
        private static IEnumerable<EncounterSlot> GetSlots3(byte[] data, ref int ofs, int numslots, SlotType t)
        {
            var slots = new List<EncounterSlot>();
            int Ratio = data[ofs];
            //1 byte padding
            if (Ratio > 0)
                ReadInSlots(data, ofs, numslots, t, slots);
            ofs += 2 + (numslots * 4);
            return slots;
        }

        private static void ReadInSlots(byte[] data, int ofs, int numslots, SlotType t, List<EncounterSlot> slots)
        {
            for (int i = 0; i < numslots; i++)
            {
                int o = ofs + (i * 4);
                int species = BitConverter.ToInt16(data, o + 4);
                if (species <= 0)
                    continue;

                slots.Add(new EncounterSlot
                {
                    LevelMin = data[o + 2],
                    LevelMax = data[o + 3],
                    Species = species,
                    SlotNumber = i,
                    Type = t
                });
            }
        }

        private static IEnumerable<EncounterSlot> GetSlots3Fishing(byte[] data, ref int ofs, int numslots)
        {
            var slots = new List<EncounterSlot>();
            int Ratio = data[ofs];
            //1 byte padding
            if (Ratio > 0)
                ReadFishingSlots(data, ofs, numslots, slots);
            ofs += 2 + (numslots * 4);
            return slots;
        }

        private static void ReadFishingSlots(byte[] data, int ofs, int numslots, List<EncounterSlot> slots)
        {
            for (int i = 0; i < numslots; i++)
            {
                int Species = BitConverter.ToInt16(data, ofs + 4 + (i * 4));
                if (Species <= 0)
                    continue;

                var slot = new EncounterSlot
                {
                    LevelMin = data[ofs + 2 + (i * 4)],
                    LevelMax = data[ofs + 3 + (i * 4)],
                    Species = Species,
                };
                if (i < 2)
                {
                    slot.Type = SlotType.Old_Rod;
                    slot.SlotNumber = i; // 0,1
                }
                else if (i < 5)
                {
                    slot.Type = SlotType.Good_Rod;
                    slot.SlotNumber = i - 2; // 0,1,2
                }
                else
                {
                    slot.Type = SlotType.Super_Rod;
                    slot.SlotNumber = i - 5; // 0,1,2,3,4
                }

                slots.Add(slot);
            }
        }

        private static EncounterArea3 GetArea3(byte[] data)
        {
            var HaveGrassSlots = data[1] == 1;
            var HaveSurfSlots = data[2] == 1;
            var HaveRockSmashSlots = data[3] == 1;
            var HaveFishingSlots = data[4] == 1;

            int offset = 5;
            var slots = new List<EncounterSlot>();
            if (HaveGrassSlots)
                slots.AddRange(GetSlots3(data, ref offset, 12, SlotType.Grass));
            if (HaveSurfSlots)
                slots.AddRange(GetSlots3(data, ref offset, 5, SlotType.Surf));
            if (HaveRockSmashSlots)
                slots.AddRange(GetSlots3(data, ref offset, 5, SlotType.Rock_Smash));
            if (HaveFishingSlots)
                slots.AddRange(GetSlots3Fishing(data, ref offset, 10));

            var area = new EncounterArea3
            {
                Location = data[0],
                Slots = slots.ToArray()
            };
            foreach (var slot in area.Slots)
                slot.Area = area;

            return area;
        }

        /// <summary>
        /// Gets the encounter areas with <see cref="EncounterSlot"/> information from Generation 3 data.
        /// </summary>
        /// <param name="entries">Raw data, one byte array per encounter area</param>
        /// <returns>Array of encounter areas.</returns>
        public static EncounterArea3[] GetArray3(byte[][] entries)
        {
            return entries.Select(GetArea3).Where(Area => Area.Slots.Length != 0).ToArray();
        }

        protected override IEnumerable<EncounterSlot> GetMatchFromEvoLevel(PKM pkm, IEnumerable<EvoCriteria> vs, int minLevel)
        {
            var slots = Slots.Where(slot => vs.Any(evo => evo.Species == slot.Species && evo.Level >= slot.LevelMin));

            if (pkm.Format != 3) // transferred to Gen4+
                return slots.Where(slot => slot.LevelMin <= minLevel);
            return slots.Where(s => s.IsLevelWithinRange(minLevel));
        }

        protected override IEnumerable<EncounterSlot> GetFilteredSlots(PKM pkm, IEnumerable<EncounterSlot> slots, int minLevel)
        {
            if (pkm.Species == (int) Species.Unown)
                return slots.Where(z => z.Form == pkm.AltForm);
            return slots;
        }
    }
}