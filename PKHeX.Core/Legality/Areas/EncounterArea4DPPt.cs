using System;
using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core
{
    /// <inheritdoc />
    /// <summary>
    /// <see cref="GameVersion.DPPt"/> encounter area
    /// </summary>
    public sealed class EncounterArea4DPPt : EncounterArea4
    {
        /// <summary>
        /// Gets the encounter areas with <see cref="EncounterSlot"/> information from Generation 4 Diamond, Pearl and Platinum data.
        /// </summary>
        /// <param name="entries">Raw data, one byte array per encounter area</param>
        /// <param name="pt">Platinum flag (for Trophy Garden slot insertion)</param>
        /// <returns>Array of encounter areas.</returns>
        public static EncounterArea4DPPt[] GetArray4DPPt(byte[][] entries, bool pt = false)
        {
            return entries.Select(z => GetArea4DPPt(z, pt)).Where(Area => Area.Slots.Length != 0).ToArray();
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

        private static EncounterArea4DPPt GetArea4DPPt(byte[] data, bool pt = false)
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
                var regular = new List<List<EncounterSlot>> { GrassSlots.Where(z => z.SlotNumber == 6 || z.SlotNumber == 7).ToList() }; // every other slot is in the product
                var pair0 = new List<List<EncounterSlot>> { GrassSlots.Where(z => Legal.Slot4_Swarm.Contains(z.SlotNumber)).ToList() };
                var pair1 = new List<List<EncounterSlot>> { GrassSlots.Where(z => Legal.Slot4_Time.Contains(z.SlotNumber)).ToList() };
                var pair2 = new List<List<EncounterSlot>> { GrassSlots.Where(z => Legal.Slot4_Radar.Contains(z.SlotNumber)).ToList() };
                var pair3 = new List<List<EncounterSlot>> { GrassSlots.Where(z => Legal.Slot4_Dual.Contains(z.SlotNumber)).ToList() };
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
                            regular.Add(new List<EncounterSlot> { trophy[i], trophy[j] });
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

            var Area4 = new EncounterArea4DPPt
            {
                Location = location,
                Slots = Slots.ToArray()
            };
            foreach (var slot in Area4.Slots)
                slot.Area = Area4;

            return Area4;
        }

        private EncounterArea4DPPt Clone(int location) => new EncounterArea4DPPt { Slots = Slots, Location = location};

        public EncounterArea4DPPt[] Clone(int[] locations)
        {
            var Areas = new EncounterArea4DPPt[locations.Length];
            for (int i = 0; i < locations.Length; i++)
                Areas[i] = Clone(locations[i]);
            return Areas;
        }
    }

    public static class DPEncounterExtensions
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