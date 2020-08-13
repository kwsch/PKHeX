using System;
using System.Collections.Generic;

namespace PKHeX.Core
{
    /// <summary>
    /// Represents an Area where <see cref="PKM"/> can be encountered, which contains a Location ID and <see cref="EncounterSlot"/> data.
    /// </summary>
    public class EncounterArea
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
        public static TArea[] GetSimpleEncounterArea<TArea, TSlot>(int[] species, int[] lvls, int location, SlotType t)
            where TArea : EncounterArea, new()
            where TSlot : EncounterSlot, new()
        {
            if ((lvls.Length & 1) != 0) // levels data not paired; expect multiple of 2
                throw new ArgumentException(nameof(lvls));

            var count = species.Length * (lvls.Length / 2);
            var slots = new TSlot[count];
            int ctr = 0;
            foreach (var s in species)
            {
                for (int i = 0; i < lvls.Length;)
                {
                    slots[ctr++] = new TSlot
                    {
                        LevelMin = lvls[i++],
                        LevelMax = lvls[i++],
                        Species = s,
                        Type = t
                    };
                }
            }
            return new[] { new TArea { Location = location, Slots = slots } };
        }

        /// <summary>
        /// Gets the slots contained in the area that match the provided data.
        /// </summary>
        /// <param name="pkm">Pokémon Data</param>
        /// <param name="chain">Evolution lineage</param>
        /// <returns>Enumerable list of encounters</returns>
        public virtual IEnumerable<EncounterSlot> GetMatchingSlots(PKM pkm, IReadOnlyList<EvoCriteria> chain)
        {
            foreach (var slot in Slots)
            {
                foreach (var evo in chain)
                {
                    if (slot.Species != evo.Species)
                        continue;
                    if (!slot.IsLevelWithinRange(pkm.Met_Level))
                        continue;
                    if (slot.Form != evo.Form && !Legal.WildChangeFormAfter.Contains(slot.Species))
                        continue;
                    yield return slot;
                }
            }
        }

        /// <summary>
        /// Checks if the provided met location ID matches the parameters for the area.
        /// </summary>
        /// <param name="location">Met Location ID</param>
        /// <returns>True if possibly originated from this area, false otherwise.</returns>
        public virtual bool IsMatchLocation(int location) => Location == location;
    }
}
