using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core
{
    /// <summary>
    /// Information pertaining to a <see cref="PKM"/>'s evolution lineage.
    /// </summary>
    public sealed class EvolutionLineage
    {
        public readonly List<List<EvolutionMethod>> Chain = new List<List<EvolutionMethod>>();

        public void Insert(EvolutionMethod entry)
        {
            int matchChain = -1;
            for (int i = 0; i < Chain.Count; i++)
            {
                if (Chain[i].Any(e => e.Species == entry.Species))
                    matchChain = i;
            }

            if (matchChain != -1)
                Chain[matchChain].Add(entry);
            else
                Chain.Insert(0, new List<EvolutionMethod> {entry});
        }

        public List<EvoCriteria> GetExplicitLineage(PKM pkm, int maxLevel, bool skipChecks, int maxSpeciesTree, int maxSpeciesOrigin, int minLevel)
        {
            int lvl = maxLevel;
            var first = new EvoCriteria {Species = pkm.Species, Level = lvl, Form = pkm.AltForm};
            var dl = new List<EvoCriteria>(3) { first };
            for (int i = Chain.Count - 1; i >= 0; i--) // reverse evolution!
            {
                bool oneValid = false;
                foreach (var evo in Chain[i])
                {
                    if (!evo.Valid(pkm, lvl, skipChecks))
                        continue;

                    if (evo.RequiresLevelUp && minLevel >= lvl)
                        break; // impossible evolution

                    oneValid = true;
                    UpdateMinValues(dl, evo);
                    int species = evo.Species;

                    // Gen7 Personal Formes -- unmap the forme personal entry ID to the actual species ID since species are consecutive
                    if (evo.Species > maxSpeciesTree)
                        species = pkm.Species - Chain.Count + i;

                    if (evo.RequiresLevelUp)
                        lvl--;
                    dl.Add(evo.GetEvoCriteria(species, lvl));
                    break;
                }
                if (!oneValid)
                    break;
            }

            // Remove future gen pre-evolutions; no Munchlax from a Gen3 Snorlax, no Pichu from a Gen1-only Raichu, etc
            var last = dl[dl.Count - 1];
            if (last.Species > maxSpeciesOrigin && dl.Any(d => d.Species <= maxSpeciesOrigin))
                dl.RemoveAt(dl.Count - 1);

            // Last species is the wild/hatched species, the minimum level is 1 because it has not evolved from previous species
            last = dl[dl.Count - 1];
            last.MinLevel = 1;
            last.RequiresLvlUp = false;
            return dl;
        }

        private static void UpdateMinValues(IReadOnlyList<EvoCriteria> dl, EvolutionMethod evo)
        {
            var last = dl[dl.Count - 1];
            if (!evo.RequiresLevelUp)
            {
                // Evolutions like elemental stones, trade, etc
                last.MinLevel = 1;
                return;
            }
            if (evo.Level == 0)
            {
                // Friendship based Evolutions, Pichu -> Pikachu, Eevee -> Umbreon, etc
                last.MinLevel = 2;

                var first = dl[0];
                if (dl.Count > 1 && !first.RequiresLvlUp)
                    first.MinLevel = 2; // Raichu from Pikachu would have a minimum level of 1; accounting for Pichu (level up required) results in a minimum level of 2
            }
            else // level up evolutions
            {
                last.MinLevel = evo.Level;

                var first = dl[0];
                if (dl.Count > 1)
                {
                    if (first.RequiresLvlUp)
                    {
                        if (first.MinLevel <= evo.Level)
                            first.MinLevel = evo.Level + 1; // Pokemon like Crobat, its minimum level is Golbat minimum level + 1
                    }
                    else
                    {
                        if (first.MinLevel < evo.Level)
                            first.MinLevel = evo.Level; // Pokemon like Nidoqueen who evolve with an evolution stone, minimum level is prior evolution minimum level
                    }
                }
            }
            last.RequiresLvlUp = evo.RequiresLevelUp;
        }
    }
}