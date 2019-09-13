using System;
using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core
{
    /// <summary>
    /// Represents an Area where <see cref="PKM"/> can be encountered, which contains a Location ID and <see cref="EncounterSlot"/> data.
    /// </summary>
    public abstract class EncounterArea
    {
        public int Location;
        public EncounterSlot[] Slots;

        /// <summary>
        /// Creates an empty encounter area ready for initialization.
        /// </summary>

        /// <summary>
        /// Gets the encounter areas for species with same level range and same slottype at same location
        /// </summary>
        /// <param name="species">List of species that exist in the Area.</param>
        /// <param name="lvls">Paired LevelMins and LevelMaxs of the encounter slots.</param>
        /// <param name="location">Location index of the encounter area.</param>
        /// <param name="t">Encounter slot type of the encounter area.</param>
        /// <returns></returns>
        public static T[] GetSimpleEncounterArea<T>(int[] species, int[] lvls, int location, SlotType t) where T : EncounterArea, new()
        {
            // levels data not paired
            if ((lvls.Length & 1) != 0)
                return new[] { new T { Location = location, Slots = Array.Empty<EncounterSlot>() } };

            var count = species.Length * (lvls.Length / 2);
            var slots = new EncounterSlot[count];
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
            return new[] { new T { Location = location, Slots = slots } };
        }

        /// <summary>
        /// Gets an array of areas from an array of raw area data
        /// </summary>
        /// <param name="entries">Simplified raw format of an Area</param>
        /// <returns>Array of areas</returns>
        public static T[] GetArray<T>(byte[][] entries) where T : EncounterArea32, new()
        {
            T[] data = new T[entries.Length];
            for (int i = 0; i < data.Length; i++)
            {
                var loc = data[i] = new T();
                loc.LoadSlots(entries[i]);
            }
            return data;
        }

        /// <summary>
        /// Gets the slots contained in the area that match the provided data.
        /// </summary>
        /// <param name="pkm">Pokémon Data</param>
        /// <param name="vs">Evolution lineage</param>
        /// <param name="minLevel">Minimum level of the encounter</param>
        /// <returns>Enumerable list of encounters</returns>
        public virtual IEnumerable<EncounterSlot> GetMatchingSlots(PKM pkm, IReadOnlyList<DexLevel> vs, int minLevel = 0)
        {
            if (minLevel == 0) // any
                return Slots.Where(slot => vs.Any(evo => evo.Species == slot.Species));

            var slots = GetMatchFromEvoLevel(pkm, vs, minLevel);
            return GetFilteredSlots(pkm, slots, minLevel);
        }

        protected virtual IEnumerable<EncounterSlot> GetMatchFromEvoLevel(PKM pkm, IEnumerable<DexLevel> vs, int minLevel)
        {
            var slots = Slots.Where(slot => vs.Any(evo => evo.Species == slot.Species && evo.Level >= slot.LevelMin));
            // Get slots where pokemon can exist with respect to level constraints
            return slots.Where(slot => slot.IsLevelWithinRange(minLevel));
        }

        protected virtual IEnumerable<EncounterSlot> GetFilteredSlots(PKM pkm, IEnumerable<EncounterSlot> slots, int minLevel)
        {
            return Legal.WildForms.Contains(pkm.Species)
                ? slots.Where(slot => slot.Form == pkm.AltForm)
                : slots;
        }
    }
}
