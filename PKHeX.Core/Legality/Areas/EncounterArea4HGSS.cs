using System;
using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core
{
    /// <inheritdoc />
    /// <summary>
    /// <see cref="GameVersion.HGSS"/> encounter area
    /// </summary>
    public sealed class EncounterArea4HGSS : EncounterArea4
    {
        /// <summary>
        /// Gets the encounter areas with <see cref="EncounterSlot"/> information from Generation 4 Heart Gold and Soul Silver data.
        /// </summary>
        /// <param name="entries">Raw data, one byte array per encounter area</param>
        /// <returns>Array of encounter areas.</returns>
        public static EncounterArea4HGSS[] GetArray4HGSS(byte[][] entries)
        {
            return entries.Select(GetArea4HGSS).Where(Area => Area.Slots.Length != 0).ToArray();
        }

        /// <summary>
        /// Gets the encounter areas with <see cref="EncounterSlot"/> information from Generation 4 Heart Gold and Soul Silver Headbutt tree data.
        /// </summary>
        /// <param name="entries">Raw data, one byte array per encounter area</param>
        /// <returns>Array of encounter areas.</returns>
        public static EncounterArea4HGSS[] GetArray4HGSS_Headbutt(byte[][] entries)
        {
            return entries.Select(GetArea4HeadbuttHGSS).Where(Area => Area.Slots.Length != 0).ToArray();
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

        private static EncounterArea4HGSS GetArea4HGSS(byte[] data)
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

                foreach (var time in new[] { grass1, grass2, grass3 })
                {
                    // non radio
                    var regular = time.Where(z => !Legal.Slot4_Sound.Contains(z.SlotNumber)).ToList(); // every other slot is in the product
                    var radio = new List<List<EncounterSlot>> { time.Where(z => Legal.Slot4_Sound.Contains(z.SlotNumber)).ToList() };
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

            var Area4 = new EncounterArea4HGSS
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

        private static EncounterArea4HGSS GetArea4HeadbuttHGSS(byte[] data)
        {
            if (data.Length < 78)
                return new EncounterArea4HGSS(); // bad data

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

            var Area = new EncounterArea4HGSS
            {
                Location = BitConverter.ToUInt16(data, 0),
                Slots = Slots.ToArray()
            };
            foreach (var slot in Area.Slots)
                slot.Area = Area;
            return Area;
        }
    }
}