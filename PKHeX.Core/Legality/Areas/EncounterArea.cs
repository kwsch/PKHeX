using System;
using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core
{
    /// <summary>
    /// Represents an Area where <see cref="PKM"/> can be encountered, which contains a Location ID and <see cref="EncounterSlot"/> data.
    /// </summary>
    public class EncounterArea
    {
        public int Location;
        public EncounterSlot[] Slots;

        /// <summary>
        /// Creates an empty encounter area ready for initialization.
        /// </summary>
        public EncounterArea() { }

        /// <summary>
        /// Creates an array of encounter data with a specified location ID.
        /// </summary>
        /// <param name="data">Encounter data</param>
        /// <remarks>
        /// Encounter Data is stored in the following format: (u16 Location, n*[u16 Species/Form, u8 Min, u8 Max])
        /// </remarks>
        private EncounterArea(byte[] data)
        {
            Location = BitConverter.ToUInt16(data, 0);
            Slots = new EncounterSlot[(data.Length - 2) / 4];
            for (int i = 0; i < Slots.Length; i++)
            {
                int ofs = 2 + (i * 4);
                ushort SpecForm = BitConverter.ToUInt16(data, ofs);
                Slots[i] = new EncounterSlot
                {
                    Species = SpecForm & 0x7FF,
                    Form = SpecForm >> 11,
                    LevelMin = data[ofs + 2],
                    LevelMax = data[ofs + 3],
                };
            }
            foreach (var slot in Slots)
                slot.Area = this;
        }

        public EncounterArea Clone(int location)
        {
            var Area = new EncounterArea
            {
                Location = location,
                Slots = new EncounterSlot[Slots.Length]
            };
            for (int i = 0; i < Slots.Length; i++)
            {
                Area.Slots[i] = Slots[i].Clone();
                Area.Slots[i].Area = Area;
            }
            return Area;
        }

        public EncounterArea[] Clone(int[] locations)
        {
            EncounterArea[] Areas = new EncounterArea[locations.Length];
            for (int i = 0; i < locations.Length; i++)
                Areas[i] = Clone(locations[i]);
            return Areas;
        }

        private static IEnumerable<EncounterSlot1> GetSlots1GrassWater(byte[] data, ref int ofs, SlotType t)
        {
            int rate = data[ofs++];
            return rate == 0 ? Enumerable.Empty<EncounterSlot1>() : ReadSlots1(data, ref ofs, 10, t, rate);
        }

        private static EncounterSlot1[] GetSlots1Fishing(byte[] data, ref int ofs)
        {
            int count = data[ofs++];
            return ReadSlots1(data, ref ofs, count, SlotType.Super_Rod, -1);
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

        private static IEnumerable<EncounterArea> GetAreas2(byte[] data, ref int ofs, SlotType t, int slotSets, int slotCount)
        {
            var areas = new List<EncounterArea>();
            while (data[ofs] != 0xFF) // end
            {
                var location = data[ofs++] << 8 | data[ofs++];
                var slots = GetSlots2GrassWater(data, ref ofs, t, slotSets, slotCount);
                var area = new EncounterArea
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

        private static List<EncounterArea> GetAreas2Fishing(byte[] data, ref int ofs)
        {
            var areas = new List<EncounterArea>();
            var types = new[] {SlotType.Old_Rod, SlotType.Good_Rod, SlotType.Super_Rod};
            while (ofs != 0x18C)
            {
                areas.Add(new EncounterArea {
                    Slots = GetSlots2Fishing(data, ref ofs, types[0])
                    .Concat(GetSlots2Fishing(data, ref ofs, types[1]))
                    .Concat(GetSlots2Fishing(data, ref ofs, types[2])).ToArray() });
            }

            // Read TimeFishGroups
            var dl = new List<DexLevel>();
            while (ofs < data.Length)
                dl.Add(new DexLevel { Species = data[ofs++], Level = data[ofs++]});

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
                    Array.Copy(slots, i, slots, i+1, slots.Length - i - 1); // shift slots down
                    slots[i+1] = slot.Clone(); // differentiate copied slot

                    int index = slot.LevelMin*2;
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

        private static IEnumerable<EncounterArea> GetAreas2Headbutt(byte[] data, ref int ofs)
        {
            // Read Location Table
            var head = new List<EncounterArea>();
            var headID = new List<int>();
            while (data[ofs] != 0xFF)
            {
                head.Add(new EncounterArea
                {
                    Location = (data[ofs++] << 8) | data[ofs++],
                    //Slots = null, // later
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

        private static IEnumerable<EncounterSlot> GetSlots3(byte[] data, ref int ofs, int numslots, SlotType t)
        {
            var slots = new List<EncounterSlot>();
            int Ratio = data[ofs];
            //1 byte padding
            if (Ratio > 0)
            {
                for (int i = 0; i < numslots; i++)
                {
                    int o = ofs + (i * 4);
                    int Species = BitConverter.ToInt16(data, o + 4);
                    if (Species <= 0)
                        continue;

                    slots.Add(new EncounterSlot
                    {
                        LevelMin = data[o + 2],
                        LevelMax = data[o + 3],
                        Species = Species,
                        SlotNumber = i,
                        Type = t
                    });
                }
            }
            ofs += 2 + (numslots * 4);
            return slots;
        }

        private static IEnumerable<EncounterSlot> GetSlots3Fishing(byte[] data, ref int ofs, int numslots)
        {
            var slots = new List<EncounterSlot>();
            int Ratio = data[ofs];
            //1 byte padding
            if (Ratio > 0)
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
            ofs += 2 + (numslots * 4);
            return slots;
        }

        private static EncounterSlot[] GetSlots4GrassDPPt(byte[] data, int ofs, int numslots, SlotType t)
        {
            var slots = new EncounterSlot[numslots];

            for (int i = 0; i < numslots; i++)
            {
                int o = ofs + (i * 8);
                int level = data[o];
                int species = BitConverter.ToInt32(data, o + 4);
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

        private static EncounterSlot[] GetSlots4GrassHGSS(byte[] data, int ofs, int numslots, SlotType t)
        {
            var slots = new EncounterSlot[numslots * 3];
            // First 36 slots are morning, day and night grass slots
            // The order is 12 level values, 12 morning species, 12 day species and 12 night species
            for (int i = 0; i < numslots; i++)
            {
                int level = data[ofs + i];
                int species = BitConverter.ToUInt16(data, ofs + numslots + (i * 2));
                slots[i] = new EncounterSlot
                {
                    LevelMin = level,
                    LevelMax = level,
                    Species = species,
                    SlotNumber = i,
                    Type = t
                };
                slots[numslots + i] = slots[i].Clone();
                slots[numslots + i].Species = BitConverter.ToUInt16(data, ofs + (numslots * 3) + (i * 2));
                slots[numslots + i].Type = t;
                slots[(numslots * 2) + i] = slots[i].Clone();
                slots[(numslots * 2) + i].Species = BitConverter.ToUInt16(data, ofs + (numslots * 5) + (i * 2));
                slots[(numslots * 2) + i].Type = t;
            }

            return slots;
        }

        /// <summary>
        /// Reads the GBA Pak Special slots, cloning <see cref="EncounterSlot"/> data from the area's base encounter slots.
        /// </summary>
        /// <remarks>
        /// These special slots only contain the info of species id; the level is copied from the corresponding <see cref="slotnums"/> index.
        /// </remarks>
        /// <param name="data">Encounter binary data</param>
        /// <param name="ofs">Offset to read from</param>
        /// <param name="slotSize">DP/Pt slotSize = 4 bytes/entry, HG/SS slotSize = 2 bytes/entry</param>
        /// <param name="ReplacedSlots">Slots from regular encounter table that end up replaced by in-game conditions</param>
        /// <param name="slotnums">Slot indexes to replace with read species IDs</param>
        /// <param name="t">Slot type of the special encounter</param>
        private static List<EncounterSlot> GetSlots4GrassSlotReplace(byte[] data, int ofs, int slotSize, EncounterSlot[] ReplacedSlots, int[] slotnums, SlotType t = SlotType.Grass)
        {
            var slots = new List<EncounterSlot>();

            int numslots = slotnums.Length;
            for (int i = 0; i < numslots; i++)
            {
                var baseSlot = ReplacedSlots[slotnums[i]];
                if (baseSlot.LevelMin <= 0)
                    continue;

                int species = BitConverter.ToUInt16(data, ofs + (i / (4 / slotSize) * slotSize));
                if (species <= 0 || baseSlot.Species == species) // Empty or duplicate
                    continue;

                var slot = baseSlot.Clone();
                slot.Species = species;
                slot.Type = t;
                slot.SlotNumber = i;
                slots.Add(slot);
            }
            return slots;
        }

        private static IEnumerable<EncounterSlot> GetSlots4WaterFishingDPPt(byte[] data, int ofs, int numslots, SlotType t)
        {
            var slots = new List<EncounterSlot>();
            for (int i = 0; i < numslots; i++)
            {
                // max, min, unused, unused, [32bit species]
                int Species = BitConverter.ToInt32(data, ofs + 4 + (i * 8));
                if (Species <= 0)
                    continue;
                // Fishing and Surf slots without a species ID are not added
                // DPPt does not have fishing or surf swarms, and does not have any Rock Smash encounters.
                slots.Add(new EncounterSlot
                {
                    LevelMax = data[ofs + 0 + (i * 8)],
                    LevelMin = data[ofs + 1 + (i * 8)],
                    Species = Species,
                    SlotNumber = i,
                    Type = t
                });
            }
            EncounterUtil.MarkEncountersStaticMagnetPull(slots, PersonalTable.HGSS);
            return slots;
        }

        private static IEnumerable<EncounterSlot> GetSlots4WaterFishingHGSS(byte[] data, int ofs, int numslots, SlotType t)
        {
            var slots = new List<EncounterSlot>();
            for (int i = 0; i < numslots; i++)
            {
                // min, max, [16bit species]
                int Species = BitConverter.ToInt16(data, ofs + 2 + (i * 4));
                if (t == SlotType.Rock_Smash && Species <= 0)
                    continue;
                // Fishing and surf Slots without a species ID are added too; these are needed for the swarm encounters.
                // These empty slots will will be deleted after we add swarm slots.

                slots.Add(new EncounterSlot
                {
                    LevelMin = data[ofs + 0 + (i * 4)],
                    LevelMax = data[ofs + 1 + (i * 4)],
                    Species = Species,
                    SlotNumber = i,
                    Type = t
                });
            }
            EncounterUtil.MarkEncountersStaticMagnetPull(slots, PersonalTable.HGSS);
            return slots;
        }

        private static EncounterArea GetArea3(byte[] data)
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

            EncounterArea Area3 = new EncounterArea
            {
                Location = data[0],
                Slots = slots.ToArray()
            };
            foreach (var slot in Area3.Slots)
                slot.Area = Area3;

            return Area3;
        }

        private static EncounterArea GetArea4DPPt(byte[] data, bool pt = false)
        {
            var Slots = new List<EncounterSlot>();

            int location = BitConverter.ToUInt16(data, 0x00);
            var GrassRatio = BitConverter.ToInt32(data, 0x02);
            if (GrassRatio > 0)
            {
                EncounterSlot[] GrassSlots = GetSlots4GrassDPPt(data, 0x06, 12, SlotType.Grass);
                //Swarming slots replace slots 0 and 1
                var swarm = GetSlots4GrassSlotReplace(data, 0x66, 4, GrassSlots, Legal.Slot4_Swarm, SlotType.Swarm);
                //Morning and Night slots replace slots 2 and 3
                var morning = GetSlots4GrassSlotReplace(data, 0x6E, 4, GrassSlots, Legal.Slot4_Time); // Morning
                var night = GetSlots4GrassSlotReplace(data, 0x76, 4, GrassSlots, Legal.Slot4_Time); // Night
                //Pokéradar slots replace slots 4,5,10 and 11
                //Pokéradar is marked with different slot type because it have different PID-IV generationn
                var radar = GetSlots4GrassSlotReplace(data, 0x7E, 4, GrassSlots, Legal.Slot4_Radar, SlotType.Pokeradar);

                //24 bytes padding

                //Dual Slots replace slots 8 and 9
                var ruby = GetSlots4GrassSlotReplace(data, 0xA6, 4, GrassSlots, Legal.Slot4_Dual); // Ruby
                var sapphire = GetSlots4GrassSlotReplace(data, 0xAE, 4, GrassSlots, Legal.Slot4_Dual); // Sapphire
                var emerald = GetSlots4GrassSlotReplace(data, 0xB6, 4, GrassSlots, Legal.Slot4_Dual); // Emerald
                var firered = GetSlots4GrassSlotReplace(data, 0xBE, 4, GrassSlots, Legal.Slot4_Dual); // FireRed
                var leafgreen = GetSlots4GrassSlotReplace(data, 0xC6, 4, GrassSlots, Legal.Slot4_Dual); // LeafGreen

                Slots.AddRange(GrassSlots);
                Slots.AddRange(swarm);
                Slots.AddRange(morning);
                Slots.AddRange(night);
                Slots.AddRange(radar);
                Slots.AddRange(ruby);
                Slots.AddRange(sapphire);
                Slots.AddRange(emerald);
                Slots.AddRange(firered);
                Slots.AddRange(leafgreen);

                // Permute Static-Magnet Pull combinations
                // [None/Swarm]-[None/Morning/Night]-[None/Radar]-[None/R/S/E/F/L] [None/TrophyGarden]
                // 2 * 3 * 2 * 6 = 72 different combinations of slots (more with trophy garden)
                var regular = new List<List<EncounterSlot>> {GrassSlots.Where(z => z.SlotNumber == 6 || z.SlotNumber == 7).ToList()}; // every other slot is in the product
                var pair0 = new List<List<EncounterSlot>> {GrassSlots.Where(z => Legal.Slot4_Swarm.Contains(z.SlotNumber)).ToList()};
                var pair1 = new List<List<EncounterSlot>> {GrassSlots.Where(z => Legal.Slot4_Time.Contains(z.SlotNumber)).ToList()};
                var pair2 = new List<List<EncounterSlot>> {GrassSlots.Where(z => Legal.Slot4_Radar.Contains(z.SlotNumber)).ToList()};
                var pair3 = new List<List<EncounterSlot>> {GrassSlots.Where(z => Legal.Slot4_Dual.Contains(z.SlotNumber)).ToList()};
                if (swarm.Count != 0) pair0.Add(swarm);
                if (morning.Count != 0) pair1.Add(morning); if (night.Count != 0) pair1.Add(night);
                if (radar.Count != 0) pair2.Add(radar);
                if (ruby.Count != 0) pair3.Add(ruby); if (sapphire.Count != 0) pair3.Add(sapphire); if (emerald.Count != 0) pair3.Add(emerald);
                if (firered.Count != 0) pair3.Add(firered); if (leafgreen.Count != 0) pair3.Add(leafgreen);
                if (location == 68) // Trophy Garden
                {
                    // Occupy Slots 6 & 7
                    var species = pt ? Encounters4.TrophyPt : Encounters4.TrophyDP;
                    var slots = new List<EncounterSlot>();
                    foreach (var s in species)
                    {
                        var slot = regular[0][0].Clone();
                        slot.Species = s;
                        slots.Add(slot);

                        slot = regular[0][1].Clone();
                        slot.Species = s;
                        slots.Add(slot);
                    }
                    Slots.AddRange(slots);
                    // get all permutations of trophy inhabitants
                    var trophy = regular[0].Concat(slots).ToArray();
                    for (int i = 0; i < trophy.Length; i++)
                    {
                        for (int j = i + 1; j < trophy.Length; j++)
                            regular.Add(new List<EncounterSlot>{trophy[i], trophy[j]});
                    }
                }

                var set = new[] { regular, pair0, pair1, pair2, pair3 };
                var product = set.CartesianProduct();
                var extra = MarkStaticMagnetExtras(product);
                Slots.AddRange(extra);
            }

            var SurfRatio = BitConverter.ToInt32(data, 0xCE);
            if (SurfRatio > 0)
                Slots.AddRange(GetSlots4WaterFishingDPPt(data, 0xD2, 5, SlotType.Surf));

            //44 bytes padding

            var OldRodRatio = BitConverter.ToInt32(data, 0x126);
            if (OldRodRatio > 0)
                Slots.AddRange(GetSlots4WaterFishingDPPt(data, 0x12A, 5, SlotType.Old_Rod));

            var GoodRodRatio = BitConverter.ToInt32(data, 0x152);
            if (GoodRodRatio > 0)
                Slots.AddRange(GetSlots4WaterFishingDPPt(data, 0x156, 5, SlotType.Good_Rod));

            var SuperRodRatio = BitConverter.ToInt32(data, 0x17E);
            if (SuperRodRatio > 0)
                Slots.AddRange(GetSlots4WaterFishingDPPt(data, 0x182, 5, SlotType.Super_Rod));

            EncounterArea Area4 = new EncounterArea
            {
                Location = location,
                Slots = Slots.ToArray()
            };
            foreach (var slot in Area4.Slots)
                slot.Area = Area4;

            return Area4;
        }

        private static IEnumerable<EncounterSlot> MarkStaticMagnetExtras(IEnumerable<IEnumerable<List<EncounterSlot>>> product)
        {
            var trackPermute = new List<EncounterSlot>();
            foreach (var p in product)
                MarkStaticMagnetPermute(p.SelectMany(z => z), trackPermute);
            return trackPermute;
        }

        private static void MarkStaticMagnetPermute(IEnumerable<EncounterSlot> grp, List<EncounterSlot> trackPermute)
        {
            EncounterUtil.MarkEncountersStaticMagnetPullPermutation(grp, PersonalTable.HGSS, trackPermute);
        }

        private static EncounterArea GetArea4HGSS(byte[] data)
        {
            var Slots = new List<EncounterSlot>();

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
                var GrassSlots = GetSlots4GrassHGSS(data, 0x0A, 12, SlotType.Grass);
                //Grass slots with species = 0 are added too, it is needed for the swarm encounters, it will be deleted after swarms are added

                // Hoenn Sound and Sinnoh Sound replace slots 4 and 5
                var hoenn = GetSlots4GrassSlotReplace(data, 0x5E, 2, GrassSlots, Legal.Slot4_Sound); // Hoenn
                var sinnoh = GetSlots4GrassSlotReplace(data, 0x62, 2, GrassSlots, Legal.Slot4_Sound); // Sinnoh

                Slots.AddRange(GrassSlots);
                Slots.AddRange(hoenn);
                Slots.AddRange(sinnoh);

                // Static / Magnet Pull
                var grass1 = GrassSlots.Take(12).ToList();
                var grass2 = GrassSlots.Skip(12).Take(12).ToList();
                var grass3 = GrassSlots.Skip(24).ToList();
                // Swarm slots do not displace electric/steel types, with exception of SoulSilver Mawile (which doesn't displace) -- handle separately

                foreach (var time in new[] {grass1, grass2, grass3})
                {
                    // non radio
                    var regular = time.Where(z => !Legal.Slot4_Sound.Contains(z.SlotNumber)).ToList(); // every other slot is in the product
                    var radio = new List<List<EncounterSlot>> {time.Where(z => Legal.Slot4_Sound.Contains(z.SlotNumber)).ToList()};
                    if (hoenn.Count > 0)
                        radio.Add(hoenn);
                    if (sinnoh.Count > 0)
                        radio.Add(sinnoh);

                    var extra = new List<EncounterSlot>();
                    foreach (var t in radio)
                        MarkStaticMagnetPermute(regular.Concat(t), extra);
                    Slots.AddRange(extra);
                }
            }

            if (SurfRatio > 0)
                Slots.AddRange(GetSlots4WaterFishingHGSS(data, 0x66, 5, SlotType.Surf));

            if (RockSmashRatio > 0)
                Slots.AddRange(GetSlots4WaterFishingHGSS(data, 0x7A, 2, SlotType.Rock_Smash));

            if (OldRodRatio > 0)
                Slots.AddRange(GetSlots4WaterFishingHGSS(data, 0x82, 5, SlotType.Old_Rod));

            if (GoodRodRatio > 0)
                Slots.AddRange(GetSlots4WaterFishingHGSS(data, 0x96, 5, SlotType.Good_Rod));

            if (SuperRodRatio > 0)
                Slots.AddRange(GetSlots4WaterFishingHGSS(data, 0xAA, 5, SlotType.Super_Rod));

            // Last 6 bytes only have species ID info
            if (data[0xC2] == 120) // Location = 182, 127, 130, 132, 167, 188, 210
                Slots.AddRange(SlotsHGSS_Staryu);

            EncounterArea Area4 = new EncounterArea
            {
                Location = BitConverter.ToUInt16(data, 0x00),
                Slots = Slots.ToArray()
            };
            foreach (var slot in Area4.Slots)
                slot.Area = Area4;
            return Area4;
        }

        private static readonly EncounterSlot[] SlotsHGSS_Staryu =
        {
           new EncounterSlot { Species = 120, LevelMin = 20, LevelMax = 20, Type = SlotType.Good_Rod },
           new EncounterSlot { Species = 120, LevelMin = 40, LevelMax = 40, Type = SlotType.Super_Rod },
        };

        private static EncounterArea GetArea4HeadbuttHGSS(byte[] data)
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
                int Species = BitConverter.ToInt16(data, 6 + (i * 4));
                if (Species <= 0)
                    continue;
                Slots.Add(new EncounterSlot
                {
                    Species = Species,
                    LevelMin = data[8 + (i * 4)],
                    LevelMax = data[9 + (i * 4)],
                    Type = i <= 11 ? SlotType.Headbutt : SlotType.Headbutt_Special
                });
            }

            var Area = new EncounterArea
            {
                Location = BitConverter.ToUInt16(data, 0),
                Slots = Slots.ToArray()
            };
            foreach (var slot in Area.Slots)
                slot.Area = Area;
            return Area;
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
        private static EncounterSlot1[] ReadSlots1(byte[] data, ref int ofs, int count, SlotType t, int rate)
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

        private static EncounterSlot1[] ReadSlots1FishingYellow(byte[] data, ref int ofs, int count, SlotType t, int rate)
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
        public static EncounterArea[] GetArray1GrassWater(byte[] data)
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
                var grass = GetSlots1GrassWater(data, ref ptr[i], SlotType.Grass);
                var water = GetSlots1GrassWater(data, ref ptr[i], SlotType.Surf);
                areas[i] = new EncounterArea
                {
                    Location = i,
                    Slots = grass.Concat(water).ToArray()
                };
            }
            return areas.Where(area => area.Slots.Length != 0).ToArray();
        }

        /// <summary>
        /// Gets the encounter areas with <see cref="EncounterSlot"/> information from Pokémon Yellow (Generation 1) Fishing data.
        /// </summary>
        /// <param name="data">Input raw data.</param>
        /// <returns>Array of encounter areas.</returns>
        public static EncounterArea[] GetArray1FishingYellow(byte[] data)
        {
            const int size = 9;
            int count = data.Length/size;
            EncounterArea[] areas = new EncounterArea[count];
            for (int i = 0; i < count; i++)
            {
                int ofs = (i * size) + 1;
                areas[i] = new EncounterArea
                {
                    Location = data[(i * size) + 0],
                    Slots = ReadSlots1FishingYellow(data, ref ofs, 4, SlotType.Super_Rod, -1)
                };
            }
            return areas;
        }

        /// <summary>
        /// Gets the encounter areas with <see cref="EncounterSlot"/> information from Generation 1 Fishing data.
        /// </summary>
        /// <param name="data">Input raw data.</param>
        /// <returns>Array of encounter areas.</returns>
        public static EncounterArea[] GetArray1Fishing(byte[] data)
        {
            var ptr = new int[255];
            var map = new int[255];
            int count = 0;
            for (int i = 0; i < ptr.Length; i++)
            {
                map[i] = data[(i * 3) + 0];
                if (map[i] == 0xFF)
                {
                    count = i;
                    break;
                }
                ptr[i] = BitConverter.ToInt16(data, (i * 3) + 1);
            }

            EncounterArea[] areas = new EncounterArea[count];
            for (int i = 0; i < areas.Length; i++)
            {
                areas[i] = new EncounterArea
                {
                    Location = map[i],
                    Slots = GetSlots1Fishing(data, ref ptr[i])
                };
            }
            return areas;
        }

        /// <summary>
        /// Gets the encounter areas with <see cref="EncounterSlot"/> information from Generation 2 Grass/Water data.
        /// </summary>
        /// <param name="data">Input raw data.</param>
        /// <returns>Array of encounter areas.</returns>
        public static EncounterArea[] GetArray2GrassWater(byte[] data)
        {
            int ofs = 0;
            var areas = new List<EncounterArea>();
            areas.AddRange(GetAreas2(data, ref ofs, SlotType.Grass,     3, 7)); // Johto Grass
            areas.AddRange(GetAreas2(data, ref ofs, SlotType.Surf,      1, 3)); // Johto Water
            areas.AddRange(GetAreas2(data, ref ofs, SlotType.Grass,     3, 7)); // Kanto Grass
            areas.AddRange(GetAreas2(data, ref ofs, SlotType.Surf,      1, 3)); // Kanto Water
            areas.AddRange(GetAreas2(data, ref ofs, SlotType.Swarm,     3, 7)); // Swarm
            areas.AddRange(GetAreas2(data, ref ofs, SlotType.Special,   1, 3)); // Union Cave
            return areas.ToArray();
        }

        /// <summary>
        /// Gets the encounter areas with <see cref="EncounterSlot"/> information from Generation 2 Grass/Water data.
        /// </summary>
        /// <param name="data">Input raw data.</param>
        /// <returns>Array of encounter areas.</returns>
        public static EncounterArea[] GetArray2Fishing(byte[] data)
        {
            int ofs = 0;
            var f = GetAreas2Fishing(data, ref ofs);

            // Fishing Tables are not associated to a single map; a map picks a table to use.
            // For all maps that use a table, create a new EncounterArea with reference to the table's slots.
            sbyte[] convMapIDtoFishLocationID =
            {
                -1,  1, -1,  0,  3,  3,  3, -1, 10,  3,  2, -1, -1,  2,  3,  0,
                -1, -1,  3, -1, -1, -1,  3, -1, -1, -1, -1,  0, -1, -1,  0,  9,
                 1,  0,  2,  2, -1,  3,  7,  3, -1,  3,  4,  8,  2, -1,  2,  1,
                -1,  3, -1, -1, -1, -1, -1,  0,  2,  2, -1, -1,  3,  1, -1, -1,
                -1,  2, -1,  2, -1, -1, -1, -1, -1, -1, 11, 11,  0, -1, -1, -1,
                -1,  7,  0,  1, -1,  1,  1,  3, -1, -1, -1,  1,  1,  2,  3, -1,
                -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
                -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            };
            var areas = new List<EncounterArea>();
            for (int i = 0; i < convMapIDtoFishLocationID.Length; i++)
            {
                var loc = convMapIDtoFishLocationID[i];
                if (loc == -1) // no table for map
                    continue;
                areas.Add(new EncounterArea { Location = i, Slots = f[loc].Slots });
            }

            // Some maps have two tables. Fortunately, there's only two. Add the second table.
            areas.Add(new EncounterArea { Location = 0x1B, Slots = f[1].Slots }); // Olivine City (0: Harbor, 1: City)
            areas.Add(new EncounterArea { Location = 0x2E, Slots = f[3].Slots }); // Silver Cave (2: Inside, 3: Outside)
            return areas.ToArray();
        }

        public static EncounterArea[] GetArray2Headbutt(byte[] data)
        {
            int ofs = 0;
            return GetAreas2Headbutt(data, ref ofs).ToArray();
        }

        /// <summary>
        /// Gets the encounter areas with <see cref="EncounterSlot"/> information from Generation 3 data.
        /// </summary>
        /// <param name="entries">Raw data, one byte array per encounter area</param>
        /// <returns>Array of encounter areas.</returns>
        public static EncounterArea[] GetArray3(byte[][] entries)
        {
            return entries.Select(GetArea3).Where(Area => Area.Slots.Length != 0).ToArray();
        }

        /// <summary>
        /// Gets the encounter areas with <see cref="EncounterSlot"/> information from Generation 4 Diamond, Pearl and Platinum data.
        /// </summary>
        /// <param name="entries">Raw data, one byte array per encounter area</param>
        /// <param name="pt">Platinum flag (for Trophy Garden slot insertion)</param>
        /// <returns>Array of encounter areas.</returns>
        public static EncounterArea[] GetArray4DPPt(byte[][] entries, bool pt = false)
        {
            return entries.Select(z => GetArea4DPPt(z, pt)).Where(Area => Area.Slots.Length != 0).ToArray();
        }

        /// <summary>
        /// Gets the encounter areas with <see cref="EncounterSlot"/> information from Generation 4 Heart Gold and Soul Silver data.
        /// </summary>
        /// <param name="entries">Raw data, one byte array per encounter area</param>
        /// <returns>Array of encounter areas.</returns>
        public static EncounterArea[] GetArray4HGSS(byte[][] entries)
        {
            return entries.Select(GetArea4HGSS).Where(Area => Area.Slots.Length != 0).ToArray();
        }

        /// <summary>
        /// Gets the encounter areas with <see cref="EncounterSlot"/> information from Generation 4 Heart Gold and Soul Silver Headbutt tree data.
        /// </summary>
        /// <param name="entries">Raw data, one byte array per encounter area</param>
        /// <returns>Array of encounter areas.</returns>
        public static EncounterArea[] GetArray4HGSS_Headbutt(byte[][] entries)
        {
            return entries.Select(GetArea4HeadbuttHGSS).Where(Area => Area.Slots.Length != 0).ToArray();
        }

        /// <summary>
        /// Gets the encounter areas for species with same level range and same slottype at same location
        /// </summary>
        /// <param name="species">List of species that exist in the Area.</param>
        /// <param name="lvls">Paired LevelMins and LevelMaxs of the encounter slots.</param>
        /// <param name="location">Location index of the encounter area.</param>
        /// <param name="t">Encounter slot type of the encounter area.</param>
        /// <returns></returns>
        public static EncounterArea[] GetSimpleEncounterArea(int[] species, int[] lvls, int location, SlotType t)
        {
            // levels data not paired
            if ((lvls.Length & 1) != 0)
                return new[] { new EncounterArea { Location = location, Slots = Array.Empty<EncounterSlot>() } };

            var slots = new EncounterSlot[species.Length * (lvls.Length / 2)];
            int ctr = 0;
            foreach (var s in species)
            {
                for (int i = 0; i < lvls.Length;)
                {
                    slots[ctr++] = new EncounterSlot
                    {
                        LevelMin = lvls[i++],
                        LevelMax = lvls[i++],
                        Species = s,
                        Type = t
                    };
                }
            }
            return new[] { new EncounterArea { Location = location, Slots = slots } };
        }

        /// <summary>
        /// Gets an array of areas from an array of raw area data
        /// </summary>
        /// <param name="entries">Simplified raw format of an Area</param>
        /// <returns>Array of areas</returns>
        public static EncounterArea[] GetArray(byte[][] entries)
        {
            EncounterArea[] data = new EncounterArea[entries.Length];
            for (int i = 0; i < data.Length; i++)
                data[i] = new EncounterArea(entries[i]);
            return data;
        }
    }

    public static partial class Extensions
    {
        public static IEnumerable<IEnumerable<T>> CartesianProduct<T>(this IEnumerable<IEnumerable<T>> sequences)
        {
            IEnumerable<IEnumerable<T>> emptyProduct = new[] { Enumerable.Empty<T>() };
            return sequences.Aggregate(
                emptyProduct,
                (accumulator, sequence) =>
                    from accseq in accumulator
                    from item in sequence
                    select accseq.Concat(new[] { item }));
        }
    }
}
