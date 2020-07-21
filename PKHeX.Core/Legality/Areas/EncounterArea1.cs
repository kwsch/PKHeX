using System;
using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core
{
    /// <inheritdoc />
    /// <summary>
    /// <see cref="GameVersion.RBY"/> encounter area
    /// </summary>
    public sealed class EncounterArea1 : EncounterArea
    {
        private static EncounterSlot1[] ReadSlots1FishingYellow(byte[] data, ref int ofs, int count, SlotType t, int rate)
        {
            // Convert byte to actual number
            byte[] levels = { 0xFF, 0x15, 0x67, 0x1D, 0x3B, 0x5C, 0x72, 0x16, 0x71, 0x18, 0x00, 0x6D, 0x80, };
            byte[] g1DexIDs = { 0x47, 0x6E, 0x18, 0x9B, 0x17, 0x4E, 0x8A, 0x5C, 0x5D, 0x9D, 0x9E, 0x1B, 0x85, 0x16, 0x58, 0x59, };
            int[] speciesIDs = { 060, 061, 072, 073, 090, 098, 099, 116, 117, 118, 119, 120, 129, 130, 147, 148, };

            var slots = new EncounterSlot1[count];
            for (int slot = 0; slot < count; slot++)
            {
                int species = speciesIDs[Array.IndexOf(g1DexIDs, data[ofs++])];
                int lvl = Array.IndexOf(levels, data[ofs++]) * 5;
                slots[slot] = new EncounterSlot1(species, lvl, lvl, rate, t, slot);
            }
            return slots;
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
            {
                int ptr = BitConverter.ToInt16(data, i * 2);
                var grass = GetSlots1GrassWater(data, ref ptr, SlotType.Grass);
                var water = GetSlots1GrassWater(data, ref ptr, SlotType.Surf);
                areas[i] = new EncounterArea1
                {
                    Location = i,
                    Slots = ArrayUtil.ConcatAll(grass, water),
                };
            }
            return areas.Where(area => area.Slots.Length != 0).ToArray();
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
                areas[i] = new EncounterArea1
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
            return rate == 0 ? Array.Empty<EncounterSlot1>() : EncounterSlot1.ReadSlots(data, ref ofs, 10, t, rate);
        }

        private static EncounterSlot1[] GetSlots1Fishing(byte[] data, int ofs)
        {
            int count = data[ofs++];
            return EncounterSlot1.ReadSlots(data, ref ofs, count, SlotType.Super_Rod, -1);
        }

        public override IEnumerable<EncounterSlot> GetMatchingSlots(PKM pkm, IReadOnlyList<DexLevel> chain, int minLevel = 0)
        {
            if (minLevel == 0) // any
                return Slots.Where(slot => chain.Any(evo => evo.Species == slot.Species));

            var encounterSlots = GetMatchFromEvoLevel(pkm, chain, minLevel);
            if (pkm is PK1 pk1 && pkm.Gen1_NotTradeback)
                encounterSlots = FilterByCatchRate(encounterSlots, pk1.Catch_Rate);

            return encounterSlots.OrderBy(slot => slot.LevelMin); // prefer lowest levels
        }

        protected override IEnumerable<EncounterSlot> GetMatchFromEvoLevel(PKM pkm, IReadOnlyList<DexLevel> chain, int minLevel)
        {
            var slots = Slots.Where(slot => chain.Any(evo => evo.Species == slot.Species && evo.Level >= slot.LevelMin));
            if (pkm.Format >= 7) // transferred to Gen7+
                return slots.Where(slot => slot.LevelMin <= minLevel);
            return slots.Where(s => s.IsLevelWithinRange(minLevel));
        }

        private static IEnumerable<EncounterSlot> FilterByCatchRate(IEnumerable<EncounterSlot> slots, int rate)
        {
            return slots.Where(z =>
                rate == (z.Version == GameVersion.YW ? PersonalTable.Y : PersonalTable.RB)[z.Species].CatchRate);
        }
    }
}
