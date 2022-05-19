using System;
using System.Collections.Generic;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core
{
    /// <inheritdoc cref="EncounterArea" />
    /// <summary>
    /// <see cref="GameVersion.GO"/> encounter area for direct-to-HOME transfers.
    /// </summary>
    public sealed record EncounterArea8g : EncounterArea, ISpeciesForm
    {
        /// <summary> Species for the area </summary>
        /// <remarks> Due to how the encounter data is packaged by PKHeX, each species-form is grouped together. </remarks>
        public int Species { get; }
        /// <summary> Form of the Species </summary>
        public int Form { get; }
        public readonly EncounterSlot8GO[] Slots;

        protected override IReadOnlyList<EncounterSlot> Raw => Slots;

        private EncounterArea8g(ushort species, byte form, EncounterSlot8GO[] slots) : base(GameVersion.GO)
        {
            Species = species;
            Form = form;
            Location = Locations.GO8;
            Slots = slots;
        }

        internal static EncounterArea8g[] GetArea(BinLinkerAccessor data)
        {
            var areas = new EncounterArea8g[data.Length];
            for (int i = 0; i < areas.Length; i++)
                areas[i] = GetArea(data[i]);
            return areas;
        }

        private const int entrySize = (2 * sizeof(int)) + 2;

        private static EncounterArea8g GetArea(ReadOnlySpan<byte> data)
        {
            var species = ReadUInt16LittleEndian(data);
            byte form = (byte)(species >> 11);
            species &= 0x3FF;

            var group = GetGroup(species, form);

            var result = new EncounterSlot8GO[(data.Length - 2) / entrySize];
            var area = new EncounterArea8g(species, form, result);
            for (int i = 0; i < result.Length; i++)
            {
                var offset = (i * entrySize) + 2;
                var entry = data.Slice(offset, entrySize);
                result[i] = ReadSlot(entry, area, species, form, group);
            }

            return area;
        }

        private static EncounterSlot8GO ReadSlot(ReadOnlySpan<byte> entry, EncounterArea8g area, ushort species, byte form, GameVersion group)
        {
            int start = ReadInt32LittleEndian(entry);
            int end = ReadInt32LittleEndian(entry[4..]);
            var sg = entry[8];
            var shiny = (Shiny)(sg & 0x3F);
            var gender = (Gender)(sg >> 6);
            var type = (PogoType)entry[9];
            return new EncounterSlot8GO(area, species, form, start, end, shiny, gender, type, group);
        }

        private static GameVersion GetGroup(int species, int form)
        {
            // Transfer Rules:
            // If it can exist in LGP/E, it uses LGP/E's move data for the initial moves.
            // Else, if it can exist in SW/SH, it uses SW/SH's move data for the initial moves.
            // Else, it must exist in US/UM, thus it uses US/UM's moves.

            var pt8 = PersonalTable.SWSH;
            var ptGG = PersonalTable.GG;

            var pi8 = (PersonalInfoSWSH)pt8[species];
            if (pi8.IsPresentInGame)
            {
                bool lgpe = (species is (<= 151 or 808 or 809)) && (form == 0 || ptGG[species].HasForm(form));
                return lgpe ? GameVersion.GG : GameVersion.SWSH;
            }
            if (species <= Legal.MaxSpeciesID_7_USUM)
            {
                bool lgpe = species <= 151 && (form == 0 || ptGG[species].HasForm(form));
                return lgpe ? GameVersion.GG : GameVersion.USUM;
            }

            throw new ArgumentOutOfRangeException(nameof(species));
        }

        public override IEnumerable<EncounterSlot> GetMatchingSlots(PKM pkm, EvoCriteria[] chain)
        {
            if (pkm.TSV == 0) // HOME doesn't assign TSV=0 to accounts.
                yield break;

            // Find the first chain that has slots defined.
            // Since it is possible to evolve before transferring, we only need the highest evolution species possible.
            // PoGoEncTool has already extrapolated the evolutions to separate encounters!
            var sf = Array.Find(chain, z => z.Species == Species && (z.Form == Form || FormInfo.IsFormChangeable(Species, Form, z.Form, pkm.Format)));
            if (sf == default)
                yield break;

            var species = pkm.Species;
            var ball = (Ball)pkm.Ball;
            var met = Math.Max(sf.LevelMin, pkm.Met_Level);
            EncounterSlot8GO? deferredIV = null;

            bool checkBall = pkm is not PA8;
            foreach (var slot in Slots)
            {
                if (!slot.IsLevelWithinRange(met))
                    continue;
                if (checkBall && !slot.IsBallValid(ball, species))
                    continue;
                if (!slot.Shiny.IsValid(pkm))
                    continue;
                if (slot.Gender != Gender.Random && (int)slot.Gender != pkm.Gender)
                    continue;

                if (!slot.GetIVsValid(pkm))
                {
                    deferredIV ??= slot;
                    continue;
                }

                yield return slot;
            }

            if (deferredIV != null)
                yield return deferredIV;
        }
    }
}
