using System;
using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core
{
    /// <inheritdoc />
    /// <summary>
    /// <see cref="GameVersion.RBY"/> encounter area
    /// </summary>
    public sealed class EncounterArea1 : IEncounterArea<EncounterSlot1>
    {
        public int Location { get; }
        public IReadOnlyList<EncounterSlot1> Slots { get; }
        public bool IsMatchLocation(int location) => Location == location;

        private static readonly byte[] levels = { 0xFF, 0x15, 0x67, 0x1D, 0x3B, 0x5C, 0x72, 0x16, 0x71, 0x18, 0x00, 0x6D, 0x80, };
        private static readonly byte[] g1DexIDs = { 0x47, 0x6E, 0x18, 0x9B, 0x17, 0x4E, 0x8A, 0x5C, 0x5D, 0x9D, 0x9E, 0x1B, 0x85, 0x16, 0x58, 0x59, };
        private static readonly int[] speciesIDs = { 060, 061, 072, 073, 090, 098, 099, 116, 117, 118, 119, 120, 129, 130, 147, 148, };

        // ReadSlots1FishingYellow
        public EncounterArea1(byte[] data, ref int ofs, int count, SlotType t, int rate, int location = 0)
        {
            // Convert byte to actual number

            var slots = new EncounterSlot1[count];
            for (int slot = 0; slot < count; slot++)
            {
                int species = speciesIDs[Array.IndexOf(g1DexIDs, data[ofs++])];
                int lvl = Array.IndexOf(levels, data[ofs++]) * 5;
                slots[slot] = new EncounterSlot1(species, lvl, lvl, rate, t, slot);
            }
            Slots = slots;
        }

        /// <summary>
        /// Gets the encounter areas with <see cref="EncounterSlot"/> information from Generation 1 Grass/Water data.
        /// </summary>
        /// <param name="data">Input raw data.</param>
        /// <param name="count">Count of areas in the binary.</param>
        /// <returns>Array of encounter areas.</returns>
        public static EncounterArea1[] GetArray1GrassWater(byte[] data, int count)
        {
            EncounterArea1[] areas = new EncounterArea1[count];
            for (int i = 0; i < areas.Length; i++)
                areas[i] = new EncounterArea1(data, i);

            return areas.Where(area => area.Slots.Count != 0).ToArray();
        }

        private EncounterArea1(byte[] data, int i)
        {
            int ptr = BitConverter.ToInt16(data, i * 2);
            var grass = GetSlots1GrassWater(data, ref ptr, SlotType.Grass);
            var water = GetSlots1GrassWater(data, ref ptr, SlotType.Surf);
            Location = i;
            Slots = ArrayUtil.ConcatAll(grass, water);
        }

        /// <summary>
        /// Gets the encounter areas with <see cref="EncounterSlot"/> information from Pokémon Yellow (Generation 1) Fishing data.
        /// </summary>
        /// <param name="data">Input raw data.</param>
        /// <returns>Array of encounter areas.</returns>
        public static EncounterArea1[] GetArray1FishingYellow(byte[] data)
        {
            const int size = 9;
            int count = data.Length / size;
            EncounterArea1[] areas = new EncounterArea1[count];
            for (int i = 0; i < count; i++)
            {
                int ofs = (i * size) + 1;
                int loc = data[(i * size) + 0];
                areas[i] = new EncounterArea1(data, ref ofs, 4, SlotType.Super_Rod, -1, loc);
            }
            return areas;
        }

        /// <summary>
        /// Gets the encounter areas with <see cref="EncounterSlot"/> information from Generation 1 Fishing data.
        /// </summary>
        /// <param name="data">Input raw data.</param>
        /// <param name="count">Count of areas in the binary.</param>
        /// <returns>Array of encounter areas.</returns>
        public static EncounterArea1[] GetArray1Fishing(byte[] data, int count)
        {
            EncounterArea1[] areas = new EncounterArea1[count];
            for (int i = 0; i < areas.Length; i++)
            {
                int loc = data[(i * 3) + 0];
                int ptr = BitConverter.ToInt16(data, (i * 3) + 1);
                areas[i] = new EncounterArea1
                {
                    Location = loc,
                    Slots = GetSlots1Fishing(data, ptr)
                };
            }
            return areas;
        }

        private static EncounterSlot1[] GetSlots1GrassWater(byte[] data, ref int ofs, SlotType t)
        {
            int rate = data[ofs++];
            return rate == 0 ? Array.Empty<EncounterSlot1>() : ReadSlots(data, ref ofs, 10, t, rate);
        }

        private static EncounterSlot1[] GetSlots1Fishing(byte[] data, int ofs)
        {
            int count = data[ofs++];
            return ReadSlots(data, ref ofs, count, SlotType.Super_Rod, -1);
        }

        /// <summary>
        /// Deserializes Gen1 Encounter Slots from data.
        /// </summary>
        /// <param name="data">Byte array containing complete slot data table.</param>
        /// <param name="ofs">Offset to start reading from.</param>
        /// <param name="count">Amount of slots to read.</param>
        /// <param name="type">Type of encounter slot table.</param>
        /// <param name="rate">Slot type encounter rate.</param>
        /// <returns>Array of encounter slots.</returns>
        public static EncounterSlot1[] ReadSlots(EncounterArea1 area, byte[] data, ref int ofs, int count, SlotType type, int rate)
        {
            var bump = type == SlotType.Surf ? 4 : 0;
            var slots = new EncounterSlot1[count];
            for (int slot = 0; slot < count; slot++)
            {
                int min = data[ofs++];
                int species = data[ofs++];
                int max = min + bump;
                slots[slot] = new EncounterSlot1(species, min, max, rate, type, slot);
            }
            return slots;
        }

        public override IEnumerable<EncounterSlot> GetMatchingSlots(PKM pkm, IReadOnlyList<EvoCriteria> chain)
        {
            int rate = pkm is PK1 pk1 && pkm.Gen1_NotTradeback ? pk1.Catch_Rate : -1;
            foreach (var slot in Slots)
            {
                foreach (var evo in chain)
                {
                    if (slot.Species != evo.Species)
                        continue;
                    if (!slot.IsLevelWithinRange(evo.MinLevel, evo.Level))
                        continue;
                    if (!IsMatch(pkm, slot, evo))
                        continue;

                    if (rate != -1)
                    {
                        var expect = (slot.Version == GameVersion.YW ? PersonalTable.Y : PersonalTable.RB)[slot.Species].CatchRate;
                        if (expect != rate)
                            continue;
                    }
                    yield return slot;
                }
            }
        }
    }
}
