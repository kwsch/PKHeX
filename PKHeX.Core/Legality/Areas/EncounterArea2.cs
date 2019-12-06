using System;
using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core
{
    /// <inheritdoc />
    /// <summary>
    /// <see cref="GameVersion.GSC"/> encounter area
    /// </summary>
    public sealed class EncounterArea2 : EncounterAreaGB
    {
        /// <summary>
        /// Gets the encounter areas with <see cref="EncounterSlot"/> information from Generation 2 Grass/Water data.
        /// </summary>
        /// <param name="data">Input raw data.</param>
        /// <returns>Array of encounter areas.</returns>
        public static EncounterArea2[] GetArray2GrassWater(byte[] data)
        {
            int ofs = 0;
            var areas = new List<EncounterArea2>();
            areas.AddRange(GetAreas2(data, ref ofs, SlotType.Grass, 3, 7)); // Johto Grass
            areas.AddRange(GetAreas2(data, ref ofs, SlotType.Surf, 1, 3)); // Johto Water
            areas.AddRange(GetAreas2(data, ref ofs, SlotType.Grass, 3, 7)); // Kanto Grass
            areas.AddRange(GetAreas2(data, ref ofs, SlotType.Surf, 1, 3)); // Kanto Water
            areas.AddRange(GetAreas2(data, ref ofs, SlotType.Swarm, 3, 7)); // Swarm
            areas.AddRange(GetAreas2(data, ref ofs, SlotType.Special, 1, 3)); // Union Cave
            return areas.ToArray();
        }

        // Fishing Tables are not associated to a single map; a map picks a table to use.
        // For all maps that use a table, create a new EncounterArea with reference to the table's slots.
        private static readonly sbyte[] convMapIDtoFishLocationID =
        {
            -1,  1, -1,  0,  3,  3,  3, -1, 10,  3,  2, -1, -1,  2,  3,  0,
            -1, -1,  3, -1, -1, -1,  3, -1, -1, -1, -1,  0, -1, -1,  0,  9,
             1,  0,  2,  2, -1,  3,  7,  3, -1,  3,  4,  8,  2, -1,  2,  1,
            -1,  3, -1, -1, -1, -1, -1,  0,  2,  2, -1, -1,  3,  1, -1, -1,
            -1,  2, -1,  2, -1, -1, -1, -1, -1, -1, 10, 10, -1, -1, -1, -1,
            -1,  7,  0,  1, -1,  1,  1,  3, -1, -1, -1,  1,  1,  2,  3, -1,
            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
        };

        /// <summary>
        /// Gets the encounter areas with <see cref="EncounterSlot"/> information from Generation 2 Grass/Water data.
        /// </summary>
        /// <param name="data">Input raw data.</param>
        /// <returns>Array of encounter areas.</returns>
        public static EncounterArea2[] GetArray2Fishing(byte[] data)
        {
            int ofs = 0;
            var f = GetAreas2Fishing(data, ref ofs);

            var areas = new List<EncounterArea2>();
            for (int i = 0; i < convMapIDtoFishLocationID.Length; i++)
            {
                var loc = convMapIDtoFishLocationID[i];
                if (loc == -1) // no table for map
                    continue;
                areas.Add(new EncounterArea2 { Location = i, Slots = f[loc].Slots });
            }

            // Some maps have two tables. Fortunately, there's only two. Add the second table.
            areas.Add(new EncounterArea2 { Location = 0x1B, Slots = f[1].Slots }); // Olivine City (0: Harbor, 1: City)
            areas.Add(new EncounterArea2 { Location = 0x2E, Slots = f[3].Slots }); // Silver Cave (2: Inside, 3: Outside)
            return areas.ToArray();
        }

        public static EncounterArea2[] GetArray2Headbutt(byte[] data)
        {
            int ofs = 0;
            return GetAreas2Headbutt(data, ref ofs).ToArray();
        }

        private static EncounterSlot1[] GetSlots2GrassWater(byte[] data, ref int ofs, SlotType t, int slotSets, int slotCount)
        {
            byte[] rates = new byte[slotSets];
            for (int i = 0; i < rates.Length; i++)
                rates[i] = data[ofs++];

            var slots = ReadSlots1(data, ref ofs, slotSets * slotCount, t, rates[0]);
            if (slotSets <= 1)
                return slots;

            for (int i = 0; i < slotCount; i++)
            {
                slots[i].Time = EncounterTime.Morning;
            }
            for (int r = 1; r < slotSets; r++)
            {
                for (int i = 0; i < slotCount; i++)
                {
                    int index = i + (r * slotCount);
                    slots[index].Rate = rates[r];
                    slots[index].SlotNumber = i;
                    slots[index].Time = r == 1 ? EncounterTime.Day : EncounterTime.Night;
                }
            }

            return slots;
        }

        private static EncounterSlot1[] GetSlots2Fishing(byte[] data, ref int ofs, SlotType t)
        {
            // slot set ends with final slot having 0xFF 0x** 0x**
            const int size = 3;
            int end = ofs; // scan for count
            while (data[end] != 0xFF)
                end += size;
            var count = ((end - ofs) / size) + 1;
            var slots = new EncounterSlot1[count];
            for (int i = 0; i < slots.Length; i++)
            {
                int rate = data[ofs++];
                int species = data[ofs++];
                int level = data[ofs++];

                slots[i] = new EncounterSlot1
                {
                    Rate = rate,
                    Species = species,
                    LevelMin = level,
                    LevelMax = level,
                    SlotNumber = i,
                    Type = species == 0 ? SlotType.Special : t // day/night specific
                };
            }
            return slots;
        }

        private static EncounterSlot1[] GetSlots2Headbutt(byte[] data, ref int ofs, SlotType t)
        {
            // slot set ends in 0xFF
            var slots = new List<EncounterSlot1>();
            int tableCount = t == SlotType.Headbutt ? 2 : 1;
            SlotType slottype = t;
            while (tableCount != 0)
            {
                if (t == SlotType.Headbutt)
                    slottype = tableCount == 2 ? SlotType.Headbutt_Special : SlotType.Headbutt;
                int rate = data[ofs++];
                if (rate == 0xFF) // end of table
                {
                    tableCount--;
                    continue;
                }

                int species = data[ofs++];
                int level = data[ofs++];

                slots.Add(new EncounterSlot1
                {
                    Rate = rate,
                    Species = species,
                    LevelMin = level,
                    LevelMax = level,
                    Type = slottype
                });
            }
            return slots.ToArray();
        }

        private static IEnumerable<EncounterArea2> GetAreas2(byte[] data, ref int ofs, SlotType t, int slotSets, int slotCount)
        {
            var areas = new List<EncounterArea2>();
            while (data[ofs] != 0xFF) // end
            {
                var location = data[ofs++] << 8 | data[ofs++];
                var slots = GetSlots2GrassWater(data, ref ofs, t, slotSets, slotCount);
                var area = new EncounterArea2
                {
                    Location = location,
                    Slots = slots,
                };
                foreach (var slot in slots)
                    slot.Area = area;
                areas.Add(area);
            }
            ofs++;
            return areas;
        }

        private static List<EncounterArea2> GetAreas2Fishing(byte[] data, ref int ofs)
        {
            var areas = new List<EncounterArea2>();
            var types = new[] { SlotType.Old_Rod, SlotType.Good_Rod, SlotType.Super_Rod };
            while (ofs != 0x18C)
            {
                areas.Add(new EncounterArea2
                {
                    Slots = GetSlots2Fishing(data, ref ofs, types[0])
                        .Concat(GetSlots2Fishing(data, ref ofs, types[1]))
                        .Concat(GetSlots2Fishing(data, ref ofs, types[2])).ToArray()
                });
            }

            // Read TimeFishGroups
            var dl = new List<SlotTemplate>();
            while (ofs < data.Length)
                dl.Add(new SlotTemplate(data[ofs++], data[ofs++]));

            // Add TimeSlots
            foreach (var area in areas)
            {
                var slots = area.Slots;
                for (int i = 0; i < slots.Length; i++)
                {
                    var slot = slots[i];
                    if (slot.Type != SlotType.Special)
                        continue;
                    Array.Resize(ref slots, slots.Length + 1);
                    Array.Copy(slots, i, slots, i + 1, slots.Length - i - 1); // shift slots down
                    slots[i + 1] = slot.Clone(); // differentiate copied slot

                    int index = slot.LevelMin * 2;
                    for (int j = 0; j < 2; j++) // load special slot info
                    {
                        var s = (EncounterSlot1)slots[i + j];
                        s.Species = dl[index + j].Species;
                        s.LevelMin = s.LevelMax = dl[index + j].Level;
                        s.Type = slots[i - 1].Type; // special slots are never first in a set, so copy previous type
                        s.Time = j == 0 ? EncounterTime.Morning | EncounterTime.Day : EncounterTime.Night;
                    }
                }
                area.Slots = slots;
            }
            return areas;
        }

        private readonly struct SlotTemplate
        {
            public readonly byte Species;
            public readonly byte Level;

            public SlotTemplate(byte species, byte level)
            {
                Species = species;
                Level = level;
            }
        }

        private static IEnumerable<EncounterArea2> GetAreas2Headbutt(byte[] data, ref int ofs)
        {
            // Read Location Table
            var head = new List<EncounterArea2>();
            var headID = new List<int>();
            while (data[ofs] != 0xFF)
            {
                head.Add(new EncounterArea2
                {
                    Location = (data[ofs++] << 8) | data[ofs++],
                    //Slots = null, // later
                });
                headID.Add(data[ofs++]);
            }
            ofs++;

            var rock = new List<EncounterArea2>();
            var rockID = new List<int>();
            while (data[ofs] != 0xFF)
            {
                rock.Add(new EncounterArea2
                {
                    Location = (data[ofs++] << 8) | data[ofs++],
                    //Slots = null, // later
                });
                rockID.Add(data[ofs++]);
            }
            ofs++;
            ofs += 0x16; // jump over GetTreeMons

            // Read ptr table
            int[] ptr = new int[data.Length == 0x109 ? 6 : 9]; // GS : C
            for (int i = 0; i < ptr.Length; i++)
                ptr[i] = data[ofs++] | (data[ofs++] << 8);

            int baseOffset = ptr.Min() - ofs;

            // Read Tables
            for (int i = 0; i < head.Count; i++)
            {
                int o = ptr[headID[i]] - baseOffset;
                head[i].Slots = GetSlots2Headbutt(data, ref o, SlotType.Headbutt);
            }
            for (int i = 0; i < rock.Count; i++)
            {
                int o = ptr[rockID[i]] - baseOffset;
                rock[i].Slots = GetSlots2Headbutt(data, ref o, SlotType.Rock_Smash);
            }

            return head.Concat(rock);
        }
    }
}