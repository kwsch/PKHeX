using System;
using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core
{
    public class EncounterArea
    {
        public int Location;
        public EncounterSlot[] Slots;
        public EncounterArea() { }

        private EncounterArea(byte[] data)
        {
            Location = BitConverter.ToUInt16(data, 0);
            Slots = new EncounterSlot[(data.Length - 2) / 4];
            for (int i = 0; i < Slots.Length; i++)
            {
                ushort SpecForm = BitConverter.ToUInt16(data, 2 + i * 4);
                Slots[i] = new EncounterSlot
                {
                    Species = SpecForm & 0x7FF,
                    Form = SpecForm >> 11,
                    LevelMin = data[4 + i * 4],
                    LevelMax = data[5 + i * 4],
                };
            }
        }

        public EncounterArea Clone(int location)
        {
            EncounterArea Areas = new EncounterArea
            {
                Location = location,
                Slots = new EncounterSlot[Slots.Length]
            };
            for (int i = 0; i < Slots.Length; i++)
            {
                Areas.Slots[i] = Slots[i].Clone();
            }
            return Areas;
        }

        public EncounterArea[] Clone(int[] locations)
        {
            EncounterArea[] Areas = new EncounterArea[locations.Length];
            for (int i = 0; i < locations.Length; i++)
                Areas[i] = Clone(locations[i]);
            return Areas;
        }

        private static EncounterSlot1[] getSlots1_GW(byte[] data, ref int ofs, SlotType t)
        {
            int rate = data[ofs++];
            return rate == 0 ? new EncounterSlot1[0] : readSlots(data, ref ofs, 10, t, rate);
        }
        private static EncounterSlot1[] getSlots1_F(byte[] data, ref int ofs)
        {
            int count = data[ofs++];
            return readSlots(data, ref ofs, count, SlotType.Super_Rod, -1);
        }
        
        private static EncounterSlot1[] getSlots2_GW(byte[] data, ref int ofs, SlotType t, int slotSets, int slotCount)
        {
            byte[] rates = new byte[slotSets];
            for (int i = 0; i < rates.Length; i++)
                rates[i] = data[ofs++];
            
            var slots = readSlots(data, ref ofs, slotSets * slotCount, t, rates[0]);
            for (int r = 1; r < slotSets; r++)
            {
                for (int i = 0; i < slotCount; i++)
                {
                    int index = i + r*slotCount;
                    slots[index].Rate = rates[r];
                    slots[index].SlotNumber = i;
                }
            }

            return slots;
        }

        private static EncounterSlot1[] getSlots2_F(byte[] data, ref int ofs, SlotType t)
        {
            // slot set ends in 0xFF 0x** 0x**
            var slots = new List<EncounterSlot1>();
            while (true)
            {
                int rate = data[ofs++];
                int species = data[ofs++];
                int level = data[ofs++];

                slots.Add(new EncounterSlot1
                {
                    Rate = rate,
                    Species = species,
                    LevelMin = level,
                    LevelMax = level,
                    Type = species == 0 ? SlotType.Special : t // day/night specific
                });

                if (rate == 0xFF)
                    break;
            }
            return slots.ToArray();
        }
        private static EncounterSlot1[] getSlots2_H(byte[] data, ref int ofs, SlotType t)
        {
            // slot set ends in 0xFF
            var slots = new List<EncounterSlot1>();
            int tableCount = t == SlotType.Headbutt ? 2 : 1;
            while (tableCount != 0)
            {
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
                    Type = t
                });
            }
            return slots.ToArray();
        }

        private static IEnumerable<EncounterArea> getAreas2(byte[] data, ref int ofs, SlotType t, int slotSets, int slotCount)
        {
            var areas = new List<EncounterArea>();
            while (data[ofs] != 0xFF) // end
            {
                areas.Add(new EncounterArea
                {
                    Location = data[ofs++] << 8 | data[ofs++],
                    Slots = getSlots2_GW(data, ref ofs, t, slotSets, slotCount),
                });
            }
            ofs++;
            return areas;
        }
        private static IEnumerable<EncounterArea> getAreas2_F(byte[] data, ref int ofs)
        {
            var areas = new List<EncounterArea>();
            var types = new[] {SlotType.Old_Rod, SlotType.Good_Rod, SlotType.Super_Rod};
            while (data.Length < ofs)
            {
                int count = 0;
                while (ofs != 0x18D)
                {
                    areas.Add(new EncounterArea
                    {
                        Location = count++,
                        Slots = getSlots2_F(data, ref ofs, types[count%3]),
                    });
                }
            }
            // Read TimeFishGroups
            var dl = new List<DexLevel>();
            while (data.Length < ofs)
                dl.Add(new DexLevel {Species = data[ofs++], Level = data[ofs++]});

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
                    Array.Copy(slots, i, slots, i+1, slots.Length - i);
                    slots[i+1] = slot.Clone(); // differentiate copied slot

                    int index = slot.LevelMin*2;
                    for (int j = 0; j < 2; j++) // load special slot info
                    {
                        var s = slots[i + j];
                        s.Species = dl[index + j].Species;
                        s.LevelMin = s.LevelMax = dl[index + j].MinLevel;
                        s.Type = slots[0].Type; // special slots are never first, so copy first slot type
                    }
                }
            }
            return areas;
        }
        private static IEnumerable<EncounterArea> getAreas2_H(byte[] data, ref int ofs)
        {
            // Read Location Table
            var head = new List<EncounterArea>();
            var headID = new List<int>();
            while (data[ofs] != 0xFF)
            {
                head.Add(new EncounterArea
                {
                    Location = (data[ofs++] << 8) | data[ofs++],
                    Slots = null, // later
                });
                headID.Add(data[ofs++]);
            }
            ofs++;

            var rock = new List<EncounterArea>();
            var rockID = new List<int>();
            while (data[ofs] != 0xFF)
            {
                rock.Add(new EncounterArea
                {
                    Location = (data[ofs++] << 8) | data[ofs++],
                    Slots = null, // later
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
                head[i].Slots = getSlots2_H(data, ref o, SlotType.Headbutt);
            }
            for (int i = 0; i < rock.Count; i++)
            {
                int o = ptr[rockID[i]] - baseOffset;
                rock[i].Slots = getSlots2_H(data, ref o, SlotType.Rock_Smash);
            }

            return head.Concat(rock);
        }

        private static IEnumerable<EncounterSlot> getSlots3(byte[] data, ref int ofs, int numslots, SlotType t)
        {
            var slots = new List<EncounterSlot>();
            int Ratio = data[ofs];
            //1 byte padding
            if (Ratio > 0)
            {
                for (int i = 0; i < numslots; i++)
                {
                    int Species = BitConverter.ToInt16(data, ofs + 4 + i * 4);
                    if (Species <= 0)
                        continue;
                    
                    slots.Add(new EncounterSlot
                    {
                        LevelMin = data[ofs + 2 + i * 4],
                        LevelMax = data[ofs + 3 + i * 4],
                        Species = Species,
                        SlotNumber = i,
                        Type = t
                    });
                }
            }
            ofs += 2 + numslots * 4;
            return slots;
        }
        private static IEnumerable<EncounterSlot> getSlots3_F(byte[] data, ref int ofs, int numslots)
        {
            var slots = new List<EncounterSlot>();
            int Ratio = data[ofs];
            //1 byte padding
            if (Ratio > 0)
            {
                for (int i = 0; i < numslots; i++)
                {
                    int Species = BitConverter.ToInt16(data, ofs + 4 + i * 4);
                    if (Species <= 0)
                        continue;
                    
                    var slot = new EncounterSlot
                    {
                        LevelMin = data[ofs + 2 + i*4],
                        LevelMax = data[ofs + 3 + i*4],
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
            ofs += 2 + numslots * 4;
            return slots;
        }

        private static EncounterSlot[] getSlots4_DPPt_G(byte[] data, int ofs, int numslots, SlotType t)
        {
            var slots = new EncounterSlot[numslots];
            
            for (int i = 0; i < numslots; i++)
            {
                int level = data[ofs + i*8];
                int species = BitConverter.ToInt32(data, ofs + i*8 + 4);
                slots[i] = new EncounterSlot
                {
                    LevelMax = level,
                    LevelMin = level,
                    Species = species,
                    SlotNumber = i,
                    Type = t
                };
            }
            return slots;
        }
        private static EncounterSlot[] getSlots4_HGSS_G(byte[] data, int ofs, int numslots, SlotType t)
        {
            var slots = new EncounterSlot[numslots * 3];
            // First 36 slots are morning, day and night grass slots
            // The order is 12 level values, 12 morning species, 12 day species and 12 night species
            for (int i = 0; i < numslots; i++)
            {
                int level = data[ofs + i];
                int species = BitConverter.ToUInt16(data,  ofs + numslots + i * 2);
                slots[i] = new EncounterSlot
                {
                    LevelMin = level,
                    LevelMax = level,
                    Species = species,
                    SlotNumber = i,
                    Type = t
                };
                slots[numslots + i] = slots[i].Clone();
                slots[numslots + i].Species = BitConverter.ToUInt16(data, ofs + numslots * 3 + i * 2);
                slots[numslots + i].Type = t;
                slots[numslots * 2 + i] = slots[i].Clone();
                slots[numslots * 2 + i].Species = BitConverter.ToUInt16(data, ofs + numslots * 5 + i * 2);
                slots[numslots * 2 + i].Type = t;
            }

            return slots;
        }
        private static IEnumerable<EncounterSlot> getSlots4_G_Replace(byte[] data, int ofs, int slotSize, EncounterSlot[] ReplacedSlots, int[] slotnums, SlotType t = SlotType.Grass)
        {
            //Special slots like GBA Dual Slot. Those slot only contain the info of species id, the level is copied from one of the first grass slots
            //for dppt slotSize = 4, for hgss slotSize = 2
            var slots = new List<EncounterSlot>();

            int numslots = slotnums.Length;
            for (int i = 0; i < numslots; i++)
            {
                var baseSlot = ReplacedSlots[slotnums[i]];
                if (baseSlot.LevelMin <= 0)
                    continue;

                int species = BitConverter.ToUInt16(data, ofs + i / (4 / slotSize) * slotSize);
                if (species <= 0 || baseSlot.Species == species) // Empty or duplicate
                    continue;

                var slot = baseSlot.Clone();
                slot.Species = species;
                slot.Type = t;
                slots.Add(slot);
            }
            return slots;
        }
        private static IEnumerable<EncounterSlot> getSlots4DPPt_WFR(byte[] data, int ofs, int numslots, SlotType t)
        {
            var slots = new List<EncounterSlot>();
            for (int i = 0; i < numslots; i++)
            {
                // max, min, unused, unused, [32bit species]
                int Species = BitConverter.ToInt32(data, ofs + 4 + i * 8);
                if (Species <= 0)
                    continue;
                // fishing and surf slots with species = 0 are not added
                // DPPt does not have fishing or surf swarms
                slots.Add(new EncounterSlot
                {
                    LevelMax = data[ofs + 0 + i * 8],
                    LevelMin = data[ofs + 1 + i * 8],
                    Species = Species,
                    Type = t
                });
            }
            return slots;
        }
        private static IEnumerable<EncounterSlot> getSlots4HGSS_WFR(byte[] data, int ofs, int numslots, SlotType t)
        {
            var slots = new List<EncounterSlot>();
            for (int i = 0; i < numslots; i++)
            {
                // min, max, [16bit species]
                int Species = BitConverter.ToInt16(data, ofs + 2 + i * 4);
                if (t == SlotType.Rock_Smash && Species <= 0)
                    continue;
                // fishing and surf slots with species = 0 are added too, it is needed for the swarm encounters, 
                // it will be deleted after add swarm slots

                slots.Add(new EncounterSlot
                {
                    LevelMin = data[ofs + 0 + i * 4],
                    LevelMax = data[ofs + 1 + i * 4],
                    Species = Species,
                    Type = t
                });
            }
            return slots;
        }

        private static EncounterArea getArea3(byte[] data)
        {
            EncounterArea Area3 = new EncounterArea();

            if (data.Length < 6)
            { Area3.Location = 0; Area3.Slots = new EncounterSlot[0]; return Area3; }

            Area3.Location = data[0];
            var HaveGrassSlots = data[1] == 1;
            var HaveSurfSlots = data[2] == 1;
            var HaveRockSmashSlots = data[3] == 1;
            var HaveFishingSlots = data[4] == 1;

            int offset = 5;
            var slots = new List<EncounterSlot>();
            if (HaveGrassSlots)
                slots.AddRange(getSlots3(data, ref offset, 12, SlotType.Grass));
            if (HaveSurfSlots)
                slots.AddRange(getSlots3(data, ref offset, 5, SlotType.Surf));
            if (HaveRockSmashSlots)
                slots.AddRange(getSlots3(data, ref offset, 5, SlotType.Rock_Smash));
            if (HaveFishingSlots)
                slots.AddRange(getSlots3_F(data, ref offset, 10));
            Area3.Slots = slots.ToArray();
            return Area3;
        }

        private static EncounterArea getArea4DPPt(byte[] data)
        {
            EncounterArea Area4 = new EncounterArea();
            if (data.Length != 0x1AA) // 426 Bytes
            { Area4.Location = 0; Area4.Slots = new EncounterSlot[0]; return Area4; }

            var Slots = new List<EncounterSlot>();
            Area4.Location = BitConverter.ToUInt16(data, 0x00);

            var GrassRatio = BitConverter.ToInt32(data, 0x02);
            if (GrassRatio > 0)
            {
                EncounterSlot[] GrassSlots = getSlots4_DPPt_G(data, 0x06, 12, SlotType.Grass);
                Slots.AddRange(GrassSlots);
                //Swarming slots replace slots 0 and 1
                Slots.AddRange(getSlots4_G_Replace(data, 0x66, 4, GrassSlots, Legal.Slot4_Swarm, SlotType.Swarm));
                //Morning and Night slots replace slots 2 and 3
                Slots.AddRange(getSlots4_G_Replace(data, 0x6E, 4, GrassSlots, Legal.Slot4_Time)); // Morning
                Slots.AddRange(getSlots4_G_Replace(data, 0x76, 4, GrassSlots, Legal.Slot4_Time)); // Night
                //Pokéradar slots replace slots 4,5,10 and 11
                //Pokéradar is marked with different slot type because it have different PID-IV generationn
                Slots.AddRange(getSlots4_G_Replace(data, 0x7E, 4, GrassSlots, Legal.Slot4_Radar, SlotType.Pokeradar));
                //24 bytes padding
                //Dual Slots replace slots 8 and 9
                Slots.AddRange(getSlots4_G_Replace(data, 0xA6, 4, GrassSlots, Legal.Slot4_Dual)); // Ruby
                Slots.AddRange(getSlots4_G_Replace(data, 0xAE, 4, GrassSlots, Legal.Slot4_Dual)); // Sapphire
                Slots.AddRange(getSlots4_G_Replace(data, 0xB6, 4, GrassSlots, Legal.Slot4_Dual)); // Emerald
                Slots.AddRange(getSlots4_G_Replace(data, 0xBE, 4, GrassSlots, Legal.Slot4_Dual)); // FireRed
                Slots.AddRange(getSlots4_G_Replace(data, 0xC6, 4, GrassSlots, Legal.Slot4_Dual)); // LeafGreen
            }

            var SurfRatio = BitConverter.ToInt32(data, 0xCE);
            if (SurfRatio > 0)
                Slots.AddRange(getSlots4DPPt_WFR(data, 0xD2, 5, SlotType.Surf));

            //44 bytes padding

            var OldRodRatio = BitConverter.ToInt32(data, 0x126);
            if (OldRodRatio > 0)
                Slots.AddRange(getSlots4DPPt_WFR(data, 0x12A, 5, SlotType.Old_Rod));

            var GoodRodRatio = BitConverter.ToInt32(data, 0x152);
            if (GoodRodRatio > 0)
                Slots.AddRange(getSlots4DPPt_WFR(data, 0x156, 5, SlotType.Good_Rod));

            var SuperRodRatio = BitConverter.ToInt32(data, 0x17E);
            if (SuperRodRatio > 0)
                Slots.AddRange(getSlots4DPPt_WFR(data, 0x182, 5, SlotType.Super_Rod));

            Area4.Slots = Slots.ToArray();
            return Area4;
        }

        private static EncounterArea getArea4HGSS(byte[] data)
        {
            EncounterArea Area4 = new EncounterArea();
            if (data.Length != 0xC6)
            { Area4.Location = 0; Area4.Slots = new EncounterSlot[0]; return Area4; }

            var Slots = new List<EncounterSlot>();
            Area4.Location = BitConverter.ToUInt16(data, 0x00);

            var GrassRatio = data[0x02];
            var SurfRatio = data[0x03];
            var RockSmashRatio = data[0x04];
            var OldRodRatio = data[0x05];
            var GoodRodRatio = data[0x06];
            var SuperRodRatio = data[0x07];
            // 2 bytes padding

            if (GrassRatio > 0)
            {
                // First 36 slots are morning, day and night grass slots
                // The order is 12 level values, 12 morning species, 12 day species and 12 night species
                var GrassSlots = getSlots4_HGSS_G(data, 0x0A, 12, SlotType.Grass);
                //Grass slots with species = 0 are added too, it is needed for the swarm encounters, it will be deleted after add swarms
                Slots.AddRange(GrassSlots);

                // Hoenn Sound and Sinnoh Sound replace slots 4 and 5
                Slots.AddRange(getSlots4_G_Replace(data, 0x5E, 2, GrassSlots, Legal.Slot4_Sound)); // Hoenn
                Slots.AddRange(getSlots4_G_Replace(data, 0x62, 2, GrassSlots, Legal.Slot4_Sound)); // Sinnoh
            }

            if (SurfRatio > 0)
                Slots.AddRange(getSlots4HGSS_WFR(data, 0x66, 5, SlotType.Surf));

            if (RockSmashRatio > 0)
                Slots.AddRange(getSlots4HGSS_WFR(data, 0x7A, 2, SlotType.Rock_Smash));

            if (OldRodRatio > 0)
                Slots.AddRange(getSlots4HGSS_WFR(data, 0x82, 5, SlotType.Old_Rod));

            if (GoodRodRatio > 0)
                Slots.AddRange(getSlots4HGSS_WFR(data, 0x96, 5, SlotType.Good_Rod));

            if (SuperRodRatio > 0)
                Slots.AddRange(getSlots4HGSS_WFR(data, 0xAA, 5, SlotType.Super_Rod));

            // Last 6 bytes only have species ID info
            if (data[0xC2] == 120) // Location = 182, 127, 130, 132, 167, 188, 210
                Slots.AddRange(SlotsHGSS_Staryu);

            Area4.Slots = Slots.ToArray();
            return Area4;
        }
        private static readonly EncounterSlot[] SlotsHGSS_Staryu =
        {
           new EncounterSlot { Species = 120, LevelMin = 20, LevelMax = 20, Type = SlotType.Good_Rod },
           new EncounterSlot { Species = 120, LevelMin = 40, LevelMax = 40, Type = SlotType.Super_Rod },
        };

        private static EncounterArea getArea4HGSS_Headbutt(byte[] data)
        {
            if (data.Length < 78)
                return new EncounterArea(); // bad data

            //2 byte location ID (defer to end)
            //4 bytes padding
            var Slots = new List<EncounterSlot>();

			// 00-11 Normal trees
			// 12-17 Special trees
            for (int i = 0; i < 18; i++)
            {
                int Species = BitConverter.ToInt16(data, 6 + i*4);
                if (Species <= 0)
                    continue;
                Slots.Add(new EncounterSlot
                {
                    Species = Species,
                    LevelMin = data[8 + i*4],
                    LevelMax = data[9 + i*4],
                    Type = i <= 11 ? SlotType.Headbutt : SlotType.Headbutt_Special
                });
            }

            return new EncounterArea
            {
                Location = BitConverter.ToUInt16(data, 0),
                Slots = Slots.ToArray()
            };
        }

        /// <summary>
        /// RBY Format Slot Getter from data.
        /// </summary>
        /// <param name="data">Byte array containing complete slot data table.</param>
        /// <param name="ofs">Offset to start reading from.</param>
        /// <param name="count">Amount of slots to read.</param>
        /// <param name="t">Type of encounter slot.</param>
        /// <param name="rate">Slot type encounter rate.</param>
        /// <returns>Array of encounter slots.</returns>
        private static EncounterSlot1[] readSlots(byte[] data, ref int ofs, int count, SlotType t, int rate)
        {
            EncounterSlot1[] slots = new EncounterSlot1[count];
            for (int i = 0; i < count; i++)
            {
                int lvl = data[ofs++];
                int spec = data[ofs++];

                slots[i] = new EncounterSlot1
                {
                    LevelMax = lvl,
                    LevelMin = lvl,
                    Species = spec,
                    Type = t,
                    Rate = rate,
                    SlotNumber = i,
                };
            }
            return slots;
        }

        private static EncounterSlot1[] readSlots_FY(byte[] data, ref int ofs, int count, SlotType t, int rate)
        {
            // Convert byte to actual number
            int[] Levelbytelist = { 0xFF, 0x15, 0x67, 0x1D, 0x3B, 0x5C, 0x72, 0x16, 0x71, 0x18, 0x00, 0x6D, 0x80, };
            int[] dexbytelist = { 0x47, 0x6E, 0x18, 0x9B, 0x17, 0x4E, 0x8A, 0x5C, 0x5D, 0x9D, 0x9E, 0x1B, 0x85, 0x16, 0x58, 0x59, };
            int[] specieslist = { 060, 061, 072, 073, 090, 098, 099, 116, 117, 118, 119, 120, 129, 130, 147, 148, };

            EncounterSlot1[] slots = new EncounterSlot1[count];
            for (int i = 0; i < count; i++)
            {
                int spec = specieslist[Array.IndexOf(dexbytelist, data[ofs++])];
                int lvl = Array.IndexOf(Levelbytelist, data[ofs++]) * 5;

                slots[i] = new EncounterSlot1
                {
                    LevelMax = lvl,
                    LevelMin = lvl,
                    Species = spec,
                    Type = t,
                    Rate = rate,
                    SlotNumber = i,
                };
            }
            return slots;
        }

        /// <summary>
        /// Gets the encounter areas with <see cref="EncounterSlot"/> information from Generation 1 Grass/Water data.
        /// </summary>
        /// <param name="data">Input raw data.</param>
        /// <returns>Array of encounter areas.</returns>
        public static EncounterArea[] getArray1_GW(byte[] data)
        {
            // RBY Format
            var ptr = new int[255];
            int count = 0;
            for (int i = 0; i < ptr.Length; i++)
            {
                ptr[i] = BitConverter.ToInt16(data, i*2);
                if (ptr[i] != -1)
                    continue;

                count = i;
                break;
            }

            EncounterArea[] areas = new EncounterArea[count];
            for (int i = 0; i < areas.Length; i++)
            {
                var grass = getSlots1_GW(data, ref ptr[i], SlotType.Grass);
                var water = getSlots1_GW(data, ref ptr[i], SlotType.Surf);
                areas[i] = new EncounterArea
                {
                    Location = i,
                    Slots = grass.Concat(water).ToArray()
                };
            }
            return areas.Where(area => area.Slots.Any()).ToArray();
        }
        /// <summary>
        /// Gets the encounter areas with <see cref="EncounterSlot"/> information from Pokémon Yellow (Generation 1) Fishing data.
        /// </summary>
        /// <param name="data">Input raw data.</param>
        /// <returns>Array of encounter areas.</returns>
        public static EncounterArea[] getArray1_FY(byte[] data)
        {
            const int size = 9;
            int count = data.Length/size;
            EncounterArea[] areas = new EncounterArea[count];
            for (int i = 0; i < count; i++)
            {
                int ofs = i*size + 1;
                areas[i] = new EncounterArea
                {
                    Location = data[i*size + 0],
                    Slots = readSlots_FY(data, ref ofs, 4, SlotType.Super_Rod, -1)
                };
            }
            return areas;
        }
        /// <summary>
        /// Gets the encounter areas with <see cref="EncounterSlot"/> information from Generation 1 Fishing data.
        /// </summary>
        /// <param name="data">Input raw data.</param>
        /// <returns>Array of encounter areas.</returns>
        public static EncounterArea[] getArray1_F(byte[] data)
        {
            var ptr = new int[255];
            var map = new int[255];
            int count = 0;
            for (int i = 0; i < ptr.Length; i++)
            {
                map[i] = data[i*3 + 0];
                if (map[i] == 0xFF)
                {
                    count = i;
                    break;
                }
                ptr[i] = BitConverter.ToInt16(data, i * 3 + 1);
            }

            EncounterArea[] areas = new EncounterArea[count];
            for (int i = 0; i < areas.Length; i++)
            {
                areas[i] = new EncounterArea
                {
                    Location = map[i],
                    Slots = getSlots1_F(data, ref ptr[i])
                };
            }
            return areas;
        }

        /// <summary>
        /// Gets the encounter areas with <see cref="EncounterSlot"/> information from Generation 2 Grass/Water data.
        /// </summary>
        /// <param name="data">Input raw data.</param>
        /// <returns>Array of encounter areas.</returns>
        public static EncounterArea[] getArray2_GW(byte[] data)
        {
            int ofs = 0;
            var areas = new List<EncounterArea>();
            areas.AddRange(getAreas2(data, ref ofs, SlotType.Grass,     3, 7)); // Johto Grass
            areas.AddRange(getAreas2(data, ref ofs, SlotType.Surf,      1, 3)); // Johto Water
            areas.AddRange(getAreas2(data, ref ofs, SlotType.Grass,     3, 7)); // Kanto Grass
            areas.AddRange(getAreas2(data, ref ofs, SlotType.Surf,      1, 3)); // Kanto Water
            areas.AddRange(getAreas2(data, ref ofs, SlotType.Swarm,     3, 7)); // Swarm
            areas.AddRange(getAreas2(data, ref ofs, SlotType.Special,   1, 3)); // Union Cave
            return areas.ToArray();
        }

        /// <summary>
        /// Gets the encounter areas with <see cref="EncounterSlot"/> information from Generation 2 Grass/Water data.
        /// </summary>
        /// <param name="data">Input raw data.</param>
        /// <returns>Array of encounter areas.</returns>
        public static EncounterArea[] getArray2_F(byte[] data)
        {
            int ofs = 0;
            return getAreas2_F(data, ref ofs).ToArray();
        }
        public static EncounterArea[] getArray2_H(byte[] data)
        {
            int ofs = 0;
            return getAreas2_H(data, ref ofs).ToArray();
        }

        /// <summary>
        /// Gets the encounter areas with <see cref="EncounterSlot"/> information from Generation 3 data.
        /// </summary>
        /// <param name="entries">Raw data, one byte array per encounter area</param>
        /// <returns>Array of encounter areas.</returns>
        public static EncounterArea[] getArray3(byte[][] entries)
        {
            if (entries == null)
                return null;

            var Areas = new List<EncounterArea>();
            foreach (byte[] t in entries)
            {
                EncounterArea Area = getArea3(t);
                if (Area.Slots.Any())
                    Areas.Add(Area);
            }
            return Areas.ToArray();
        }

        /// <summary>
        /// Gets the encounter areas with <see cref="EncounterSlot"/> information from Generation 4 Diamond, Pearl and Platinum data.
        /// </summary>
        /// <param name="entries">Raw data, one byte array per encounter area</param>
        /// <returns>Array of encounter areas.</returns>
        public static EncounterArea[] getArray4DPPt(byte[][] entries)
        {
            return entries?.Select(getArea4DPPt).Where(Area => Area.Slots.Any()).ToArray();
        }

        /// <summary>
        /// Gets the encounter areas with <see cref="EncounterSlot"/> information from Generation 4 Hearth Gold and Soul Silver data.
        /// </summary>
        /// <param name="entries">Raw data, one byte array per encounter area</param>
        /// <returns>Array of encounter areas.</returns>
        public static EncounterArea[] getArray4HGSS(byte[][] entries)
        {
            return entries?.Select(getArea4HGSS).Where(Area => Area.Slots.Any()).ToArray();
        }

        /// <summary>
        /// Gets the encounter areas with <see cref="EncounterSlot"/> information from Generation 4 Hearth Gold and Soul Silver Headbutt tree data.
        /// </summary>
        /// <param name="entries">Raw data, one byte array per encounter area</param>
        /// <returns>Array of encounter areas.</returns>
        public static EncounterArea[] getArray4HGSS_Headbutt(byte[][] entries)
        {
            return entries?.Select(getArea4HGSS_Headbutt).Where(Area => Area.Slots.Any()).ToArray();
        }

        /// <summary>
        /// Gets the encounter areas for the Trophy Garden
        /// </summary>
        /// <param name="species">List of special species that can exist in the garden.</param>
        /// <param name="lvls">Levels of the two encounter slots they can replace. <see cref="GameVersion.DP"/> differs from <see cref="GameVersion.Pt"/></param>
        /// <returns></returns>
        public static EncounterArea[] getTrophyArea(IEnumerable<int> species, int[] lvls)
        {
            int[] slotnums = {6, 7};
            var l = new List<EncounterSlot>();
            foreach (var s in species)
            {
                for (int i = 0; i < 2; i++)
                {
                    l.Add(new EncounterSlot
                    {
                        LevelMax = lvls[i],
                        LevelMin = lvls[i],
                        Species = s,
                        SlotNumber = slotnums[i],
                        Type = SlotType.Grass
                    });
                }
            }
            return new[] { new EncounterArea { Location = 68, Slots = l.ToArray() } };
        }

        /// <summary>
        /// Gets the encounter areas for species with same level range and same slottype at same location
        /// </summary>
        /// <param name="species">List of species that exist in the Area.</param>
        /// <param name="lvls">Paired LevelMins and LevelMaxs of the encounter slots.</param>
        /// <param name="location">Location index of the encounter area.</param>
        /// <param name="t">Encounter slot type of the encounter area.</param>
        /// <returns></returns>
        public static EncounterArea[] getSimpleEncounterArea(IEnumerable<int> species, int[] lvls, int location, SlotType t)
        {
            var l = new List<EncounterSlot>();
            // levels data not paired
            if ((lvls.Length & 1) == 1)
                return new[] { new EncounterArea { Location = location, Slots = l.ToArray() } };

            foreach (var s in species)
            {
                for (int i = 0; i < lvls.Length;)
                {
                    l.Add(new EncounterSlot
                    {
                        LevelMin = lvls[i++],
                        LevelMax = lvls[i++],
                        Species = s,
                        Type = t
                    });
                }
            }
            return new[] { new EncounterArea { Location = location, Slots = l.ToArray() } };
        }

        public static EncounterArea[] getArray(byte[][] entries)
        {
            if (entries == null)
                return null;

            EncounterArea[] data = new EncounterArea[entries.Length];
            for (int i = 0; i < data.Length; i++)
                data[i] = new EncounterArea(entries[i]);
            return data;
        }
    }
}
