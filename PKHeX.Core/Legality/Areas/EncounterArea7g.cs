using System;
using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core
{
    /// <inheritdoc />
    /// <summary>
    /// <see cref="GameVersion.GO"/> encounter area for <see cref="GameVersion.GG"/>
    /// </summary>
    public sealed class EncounterArea7g : EncounterArea
    {
        public int Species { get; }
        public int Form { get; }

        private EncounterArea7g(int species, int form) : base(GameVersion.GO)
        {
            Species = species;
            Form = form;
            Location = 50;
        }

        internal static EncounterArea7g[] GetArea(byte[][] pickle)
        {
            var areas = new EncounterArea7g[pickle.Length];
            for (int i = 0; i < areas.Length; i++)
                areas[i] = GetArea(pickle[i]);
            return areas;
        }

        private const int entrySize = 2;

        private static EncounterArea7g GetArea(byte[] data)
        {
            var sf = BitConverter.ToInt16(data, 0);
            int species = sf & 0x7FF;
            int form = sf >> 11;

            // Files are padded to be multiples of 4 bytes. The last entry might actually be padding.
            // Since we aren't saving a count up-front, just check if the last entry is valid.
            int count = (data.Length - 2) / entrySize;
            if (data[data.Length - 1] == 0) // type of "None" is not valid
                count--;

            var result = new EncounterSlot7GO[count];
            var area = new EncounterArea7g(species, form) {Slots = result};

            for (int i = 0; i < result.Length; i++)
            {
                var offset = (i * entrySize) + 2;
                var shiny = (Shiny)data[offset];
                var type = (PogoType)data[offset + 1];
                result[i] = new EncounterSlot7GO(area, species, form, shiny, type);
            }

            return area;
        }

        public override IEnumerable<EncounterSlot> GetMatchingSlots(PKM pkm, IReadOnlyList<EvoCriteria> chain)
        {
            bool exists = chain.Any(z => z.Species == Species && z.Form == Form);
            if (!exists)
                yield break;

            foreach (var s in Slots)
            {
                var slot = (EncounterSlot7GO)s;
                if (!slot.IsLevelWithinRange(pkm.Met_Level))
                    continue;
                if (!slot.Shiny.IsValid(pkm))
                    continue;
                yield return slot;
            }
        }
    }
}
