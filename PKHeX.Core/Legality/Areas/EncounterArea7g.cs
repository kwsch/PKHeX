using System;
using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core
{
    /// <inheritdoc cref="EncounterArea" />
    /// <summary>
    /// <see cref="GameVersion.GO"/> encounter area for <see cref="GameVersion.GG"/>
    /// </summary>
    public sealed record EncounterArea7g : EncounterArea, ISpeciesForm
    {
        /// <summary> Species for the area </summary>
        /// <remarks> Due to how the encounter data is packaged by PKHeX, each species-form is grouped together. </remarks>
        public int Species { get; }
        /// <summary> Form of the Species </summary>
        public int Form { get; }

        private EncounterArea7g(int species, int form) : base(GameVersion.GO)
        {
            Species = species;
            Form = form;
            Location = Locations.GO7;
        }

        internal static EncounterArea7g[] GetArea(byte[][] data)
        {
            var areas = new EncounterArea7g[data.Length];
            for (int i = 0; i < areas.Length; i++)
                areas[i] = GetArea(data[i]);
            return areas;
        }

        private const int entrySize = (2 * sizeof(int)) + 2;

        private static EncounterArea7g GetArea(byte[] data)
        {
            var sf = BitConverter.ToInt16(data, 0);
            int species = sf & 0x7FF;
            int form = sf >> 11;

            var result = new EncounterSlot7GO[(data.Length - 2) / entrySize];
            var area = new EncounterArea7g(species, form) { Slots = result };
            for (int i = 0; i < result.Length; i++)
            {
                var offset = (i * entrySize) + 2;
                result[i] = ReadSlot(data, offset, area, species, form);
            }

            return area;
        }

        private static EncounterSlot7GO ReadSlot(byte[] data, int offset, EncounterArea7g area, int species, int form)
        {
            int start = BitConverter.ToInt32(data, offset);
            int end = BitConverter.ToInt32(data, offset + 4);
            var sg = data[offset + 8];
            var shiny = (Shiny)(sg & 0x3F);
            var gender = (Gender)(sg >> 6);
            var type = (PogoType)data[offset + 9];
            return new EncounterSlot7GO(area, species, form, start, end, shiny, gender, type);
        }

        public override IEnumerable<EncounterSlot> GetMatchingSlots(PKM pkm, IReadOnlyList<EvoCriteria> chain)
        {
            var sf = chain.FirstOrDefault(z => z.Species == Species && z.Form == Form);
            if (sf == null)
                yield break;

            var stamp = EncounterSlotGO.GetTimeStamp(pkm.Met_Year + 2000, pkm.Met_Month, pkm.Met_Day);
            var met = Math.Max(sf.MinLevel, pkm.Met_Level);
            foreach (var s in Slots)
            {
                var slot = (EncounterSlot7GO)s;
                if (!slot.IsLevelWithinRange(met))
                    continue;
                //if (!slot.IsBallValid(ball)) -- can have any of the in-game balls due to re-capture
                //    continue;
                if (!slot.Shiny.IsValid(pkm))
                    continue;
                //if (slot.Gender != Gender.Random && (int) slot.Gender != pkm.Gender)
                //    continue;
                if (!slot.IsWithinStartEnd(stamp))
                    continue;

                yield return slot;
            }
        }
    }
}
