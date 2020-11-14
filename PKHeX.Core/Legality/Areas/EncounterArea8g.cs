using System;
using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core
{
    /// <inheritdoc />
    /// <summary>
    /// <see cref="GameVersion.GO"/> encounter area for direct-to-HOME transfers.
    /// </summary>
    public sealed class EncounterArea8g : EncounterArea
    {
        public int Species { get; }
        public int Form { get; }

        private EncounterArea8g(int species, int form) : base(GameVersion.GO)
        {
            Species = species;
            Form = form;
            Location = Locations.GO8;
        }

        internal static EncounterArea8g[] GetArea(byte[][] pickle)
        {
            var areas = new EncounterArea8g[pickle.Length];
            for (int i = 0; i < areas.Length; i++)
                areas[i] = GetArea(pickle[i]);
            return areas;
        }

        private const int entrySize = (2 * sizeof(int)) + 2;

        private static EncounterArea8g GetArea(byte[] data)
        {
            var sf = BitConverter.ToInt16(data, 0);
            int species = sf & 0x7FF;
            int form = sf >> 11;

            var group = GetGroup(species, form);

            var result = new EncounterSlot8GO[(data.Length - 2) / entrySize];
            var area = new EncounterArea8g(species, form) {Slots = result};
            for (int i = 0; i < result.Length; i++)
            {
                var offset = (i * entrySize) + 2;
                result[i] = ReadSlot(data, offset, area, species, form, group);
            }

            return area;
        }

        private static EncounterSlot8GO ReadSlot(byte[] data, int offset, EncounterArea8g area, int species, int form, GameVersion group)
        {
            int start = BitConverter.ToInt32(data, offset);
            int end = BitConverter.ToInt32(data, offset + 4);
            var shiny = (Shiny)data[offset + 8];
            var type = (PogoType)data[offset + 9];
            return new EncounterSlot8GO(area, species, form, group, type, shiny, start, end);
        }

        private static GameVersion GetGroup(int species, int form)
        {
            var pt8 = PersonalTable.SWSH;
            var ptGG = PersonalTable.GG;

            var pi8 = (PersonalInfoSWSH)pt8[species];
            if (pi8.IsPresentInGame)
            {
                bool lgpe = (species <= 151 || species == 808 || species == 809) && (form == 0 || ptGG[species].HasForme(form));
                return lgpe ? GameVersion.GG : GameVersion.SWSH;
            }
            if (species <= Legal.MaxSpeciesID_7_USUM)
            {
                bool lgpe = species <= 151 && (form == 0 || ptGG[species].HasForme(form));
                return lgpe ? GameVersion.GG : GameVersion.USUM;
            }

            throw new ArgumentOutOfRangeException(nameof(species));
        }

        public override IEnumerable<EncounterSlot> GetMatchingSlots(PKM pkm, IReadOnlyList<EvoCriteria> chain)
        {
            if (pkm.TSV == 0) // HOME doesn't assign TSV=0 to accounts.
                yield break;

            bool exists = chain.Any(z => z.Species == Species && z.Form == Form);
            if (!exists)
                yield break;

            var ball = (Ball)pkm.Ball;
            foreach (var s in Slots)
            {
                var slot = (EncounterSlot8GO)s;
                if (!slot.IsLevelWithinRange(pkm.Met_Level))
                    continue;
                if (!slot.IsBallValid(ball))
                    continue;
                if (!slot.Shiny.IsValid(pkm))
                    continue;
                yield return slot;
            }
        }
    }
}
