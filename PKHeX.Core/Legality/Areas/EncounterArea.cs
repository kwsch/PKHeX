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
        public EncounterSlot[] Slots = Array.Empty<EncounterSlot>();

        /// <summary>
        /// Gets the encounter areas for species with same level range and same slot type at same location
        /// </summary>
        /// <param name="species">List of species that exist in the Area.</param>
        /// <param name="lvls">Paired min and max levels of the encounter slots.</param>
        /// <param name="location">Location index of the encounter area.</param>
        /// <param name="t">Encounter slot type of the encounter area.</param>
        /// <returns>Encounter area with slots</returns>
        public static T[] GetSimpleEncounterArea<T>(int[] species, int[] lvls, int location, SlotType t) where T : EncounterArea, new()
        {
            if ((lvls.Length & 1) != 0) // levels data not paired; expect multiple of 2
                throw new ArgumentException(nameof(lvls));

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
        /// Gets the slots contained in the area that match the provided data.
        /// </summary>
        /// <param name="pkm">Pokémon Data</param>
        /// <param name="vs">Evolution lineage</param>
        /// <param name="minLevel">Minimum level of the encounter</param>
        /// <returns>Enumerable list of encounters</returns>
        public virtual IEnumerable<EncounterSlot> GetMatchingSlots(PKM pkm, IReadOnlyList<EvoCriteria> vs, int minLevel = 0)
        {
            if (minLevel == 0) // any
                return Slots.Where(slot => vs.Any(evo => evo.Species == slot.Species));

            var slots = GetMatchFromEvoLevel(pkm, vs, minLevel);
            return GetFilteredSlots(pkm, slots, minLevel);
        }

        protected virtual IEnumerable<EncounterSlot> GetMatchFromEvoLevel(PKM pkm, IEnumerable<EvoCriteria> vs, int minLevel)
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

        /// <summary>
        /// Checks if the provided met location ID matches the parameters for the area.
        /// </summary>
        /// <param name="location">Met Location ID</param>
        /// <returns>True if possibly originated from this area, false otherwise.</returns>
        public virtual bool IsMatchLocation(int location) => Location == location;
    }
}
