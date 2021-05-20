using System.Collections.Generic;

namespace PKHeX.Core
{
    /// <summary>
    /// Logic for determining the least-evolved species (baby/seed).
    /// </summary>
    internal static class EvoBase
    {
        internal static EvoCriteria GetBaseSpecies(PKM pkm, int skipOption = 0)
        {
            return GetBaseSpecies(pkm, -1, pkm.Format, skipOption);
        }

        internal static EvoCriteria GetBaseSpecies(PKM pkm, int maxSpeciesOrigin, int generation, int skipOption = 0)
        {
            int tree = generation;
            var table = EvolutionTree.GetEvolutionTree(pkm, tree);
            var evos = table.GetValidPreEvolutions(pkm, maxLevel: 100, maxSpeciesOrigin: maxSpeciesOrigin, skipChecks: true);
            return GetBaseSpecies(evos, skipOption);
        }

        private static readonly EvoCriteria Nincada = new(290, 0)
        {
            Method = (int)EvolutionType.LevelUp,
            MinLevel = 20,
            Level = 20,
            RequiresLvlUp = true,
        };

        private static readonly EvoCriteria EvoEmpty = new(0, 0)
        {
            Method = (int)EvolutionType.None,
        };

        internal static EvoCriteria GetBaseSpecies(IReadOnlyList<EvoCriteria> evolutions, int skipOption = 0)
        {
            int species = evolutions[0].Species;
            if (species == (int)Species.Shedinja) // Shedinja
                return Nincada; // Nincada

            // skip n from end, return empty if invalid index
            int index = evolutions.Count - 1 - skipOption;
            return (uint)index >= evolutions.Count ? EvoEmpty : evolutions[index];
        }
    }
}
