using System;
using System.Collections.Generic;
using System.Linq;

using static PKHeX.Core.Legal;

namespace PKHeX.Core
{
    public static class EvolutionChain
    {
        private static readonly List<EvoCriteria> NONE = new(0);

        internal static IReadOnlyList<EvoCriteria>[] GetEvolutionChainsAllGens(PKM pkm, IEncounterable Encounter)
        {
            var CompleteEvoChain = GetEvolutionChain(pkm, Encounter, pkm.Species, pkm.CurrentLevel);
            if (Encounter is EncounterInvalid || pkm.IsEgg || CompleteEvoChain.Count == 0)
                return GetChainSingle(pkm, CompleteEvoChain);

            return GetChainAll(pkm, Encounter, CompleteEvoChain);
        }

        private static List<EvoCriteria>[] GetChainBase(int maxgen)
        {
            var GensEvoChains = new List<EvoCriteria>[maxgen + 1];
            for (int i = 0; i <= maxgen; i++)
                GensEvoChains[i] = NONE; // default no-evolutions
            return GensEvoChains;
        }

        private static List<EvoCriteria>[] GetChainSingle(PKM pkm, List<EvoCriteria> CompleteEvoChain)
        {
            var chain = GetChainBase(Math.Max(2, pkm.Format));
            chain[pkm.Format] = CompleteEvoChain;
            return chain;
        }

        private static List<EvoCriteria>[] GetChainAll(PKM pkm, IEncounterable enc, IReadOnlyList<EvoCriteria> CompleteEvoChain)
        {
            int maxgen = pkm is PK1 {Gen1_NotTradeback: false} ? 2 : pkm.Format;
            var GensEvoChains = GetChainBase(maxgen);

            var queue = new Queue<EvoCriteria>(CompleteEvoChain);
            var mostEvolved = queue.Dequeue();

            int lvl = pkm.CurrentLevel;
            int maxLevel = lvl;
            int pkGen = enc.Generation;

            // Iterate generations backwards
            // Maximum level of an earlier generation (GenX) will never be greater than a later generation (GenX+Y).
            int mingen = pkGen >= 3 ? pkGen : pkm.Gen2_NotTradeback ? 2 : 1;
            bool noxfrDecremented = true;
            for (int g = GensEvoChains.Length - 1; g >= mingen; g--)
            {
                if (pkGen <= 2 && g == 6)
                    g = 2;

                if (g <= 4 && pkm.Format > 2 && pkm.Format > g && !pkm.HasOriginalMetLocation && lvl > pkm.Met_Level)
                {
                    // Met location was lost at this point but it also means the pokemon existed in generations 1 to 4 with maximum level equals to met level
                    lvl = pkm.Met_Level;
                }

                int maxspeciesgen = g == 2 && pkm.VC1 ? MaxSpeciesID_1 : GetMaxSpeciesOrigin(g);

                // Remove future gen evolutions after a few special considerations:
                // If the pokemon origin is illegal (e.g. Gen3 Infernape) the list will be emptied -- species lineage did not exist at any evolution stage.
                while (mostEvolved.Species > maxspeciesgen)
                {
                    if (queue.Count == 0)
                    {
                        if (g <= 2 && pkm.VC1)
                            GensEvoChains[pkm.Format] = NONE; // invalidate here since we haven't reached the regular invalidation
                        return GensEvoChains;
                    }
                    if (mostEvolved.RequiresLvlUp)
                    {
                        // This is a Gen3 pokemon in a Gen4 phase evolution that requires level up and then transferred to Gen5+
                        // We can deduce that it existed in Gen4 until met level,
                        // but if current level is met level we can also deduce it existed in Gen3 until maximum met level -1
                        if (g == 3 && pkm.Format > 4 && lvl == maxLevel)
                            lvl--;

                        // The same condition for Gen2 evolution of Gen1 pokemon, level of the pokemon in Gen1 games would be CurrentLevel -1 one level below Gen2 level
                        else if (g == 1 && pkm.Format == 2 && lvl == maxLevel)
                            lvl--;
                    }
                    mostEvolved = queue.Dequeue();
                }

                // Alolan form evolutions, remove from gens 1-6 chains
                if (EvolveToAlolanForms.Contains(mostEvolved.Species))
                {
                    if (g < 7 && pkm.Format >= 7 && mostEvolved.Form > 0)
                    {
                        if (queue.Count == 0)
                            break;
                        mostEvolved = queue.Dequeue();
                    }
                }

                GensEvoChains[g] = GetEvolutionChain(pkm, enc, mostEvolved.Species, lvl);
                if (GensEvoChains[g].Count == 0)
                    continue;

                if (g > 2 && !pkm.HasOriginalMetLocation && g >= pkGen && noxfrDecremented)
                {
                    bool isTransferred = HasMetLocationUpdatedTransfer(pkGen, g);
                    if (!isTransferred)
                        continue;

                    noxfrDecremented = g > (pkGen != 3 ? 4 : 5);

                    // Remove previous evolutions below transfer level
                    // For example a gen3 Charizard in format 7 with current level 36 and met level 36, thus could never be Charmander / Charmeleon in Gen5+.
                    // chain level for Charmander is 35, is below met level.
                    int minlvl = GetMinLevelGeneration(pkm, g);
                    GensEvoChains[g].RemoveAll(e => e.Level < minlvl);
                }
                else if (g == 1)
                {
                    // Remove Gen2 post-evolutions (Scizor, Blissey...)
                    if (GensEvoChains[1][0].Species > MaxSpeciesID_1)
                        GensEvoChains[1].RemoveAt(0);

                    // Remove Gen2 pre-evolutions (Pichu, Cleffa...)
                    int lastIndex = GensEvoChains[1].Count - 1;
                    if (lastIndex >= 0 && GensEvoChains[1][lastIndex].Species > MaxSpeciesID_1)
                        GensEvoChains[1].RemoveAt(lastIndex);

                    // Remove Gen7 pre-evolutions and chain break scenarios
                    if (pkm.VC1)
                        TrimVC1Transfer(pkm, GensEvoChains);
                }
            }
            return GensEvoChains;
        }

        private static bool HasMetLocationUpdatedTransfer(int originalGeneration, int currentGeneration)
        {
            if (originalGeneration < 3)
                return currentGeneration >= 3;
            if (originalGeneration <= 4)
                return currentGeneration != originalGeneration;
            return false;
        }

        private static void TrimVC1Transfer(PKM pkm, IList<List<EvoCriteria>> GensEvoChains)
        {
            if (GensEvoChains[7].All(z => z.Species > MaxSpeciesID_1))
                GensEvoChains[pkm.Format] = NONE; // needed a Gen1 species present; invalidate the chain.
        }

        internal static int GetEvoChainSpeciesIndex(IReadOnlyList<EvoCriteria> chain, int species)
        {
            for (int i = 0; i < chain.Count; i++)
            {
                if (chain[i].Species == species)
                    return i;
            }
            return -1;
        }

        private static List<EvoCriteria> GetEvolutionChain(PKM pkm, IEncounterable Encounter, int maxspec, int maxlevel)
        {
            var chain = GetValidPreEvolutions(pkm, minLevel: Encounter.LevelMin);
            if (Encounter.Species == maxspec)
            {
                if (chain.Count != 1)
                {
                    chain.RemoveAll(z => z.Species != Encounter.Species);
                    chain[0].MinLevel = Encounter.LevelMin;
                }
                return chain;
            }

            // Evolution chain is in reverse order (devolution)
            // Find the index of the minimum species to determine the end of the chain
            int minindex = GetEvoChainSpeciesIndex(chain, Encounter.Species);
            bool last = minindex < 0 || minindex == chain.Count - 1;

            // If we remove a pre-evolution, update the chain if appropriate.
            if (!last)
            {
                // Remove chain species after the encounter
                int count = chain.Count;
                for (int i = minindex + 1; i < count; i++)
                    chain.RemoveAt(i);

                if (chain.Count == 0)
                    return chain; // no species left in chain
                CheckLastEncounterRemoval(Encounter, chain);
            }

            // maxspec is used to remove future geneneration evolutions, to gather evolution chain of a pokemon in previous generations
            int skip = Math.Max(0, GetEvoChainSpeciesIndex(chain, maxspec));
            for (int i = 0; i < skip; i++)
                chain.RemoveAt(0);

            // Gen3->4 and Gen4->5 transfer sets the Met Level property to the Pokémon's current level.
            // Removes evolutions impossible before the transfer level.
            // For example a FireRed Charizard with a current level (in XY) is 50 but Met Level is 20; it couldn't be a Charizard in Gen3 and Gen4 games
            chain.RemoveAll(e => e.MinLevel > maxlevel);

            // Reduce the evolution chain levels to max level to limit any later analysis/results.
            foreach (var d in chain)
                d.Level = Math.Min(d.Level, maxlevel);

            return chain;
        }

        private static void CheckLastEncounterRemoval(IEncounterable enc, IReadOnlyList<EvoCriteria> chain)
        {
            // Last entry from chain is removed, turn next entry into the encountered Pokémon
            var last = chain[chain.Count - 1];
            last.MinLevel = enc.LevelMin;
            last.RequiresLvlUp = false;

            var first = chain[0];
            if (first.RequiresLvlUp)
                return;

            if (first.MinLevel == 2)
            {
                // Example: Raichu in Gen2 or later
                // Because Pichu requires a level up, the minimum level of the resulting Raichu must be be >2
                // But after removing Pichu (because the origin species is Pikachu), the Raichu minimum level should be 1.
                first.MinLevel = 1;
                first.RequiresLvlUp = false;
            }
            else // in-game trade or evolution stone can evolve immediately
            {
                first.MinLevel = last.MinLevel;
            }
        }

        internal static List<EvoCriteria> GetValidPreEvolutions(PKM pkm, int maxspeciesorigin = -1, int maxLevel = -1, int minLevel = 1, bool skipChecks = false)
        {
            if (maxLevel < 0)
                maxLevel = pkm.CurrentLevel;

            if (maxspeciesorigin == -1 && pkm.InhabitedGeneration(2) && pkm.Format <= 2 && pkm.Generation == 1)
                maxspeciesorigin = MaxSpeciesID_2;

            int tree = Math.Max(2, pkm.Format);
            var et = EvolutionTree.GetEvolutionTree(pkm, tree);
            return et.GetValidPreEvolutions(pkm, maxLevel: maxLevel, maxSpeciesOrigin: maxspeciesorigin, skipChecks: skipChecks, minLevel: minLevel);
        }

        private static int GetMinLevelGeneration(PKM pkm, int generation)
        {
            if (!pkm.InhabitedGeneration(generation))
                return 0;

            if (pkm.Format <= 2)
                return 2;

            var origin = pkm.Generation;
            if (!pkm.HasOriginalMetLocation && generation != origin)
                return pkm.Met_Level;

            // gen 3 and prior can't obtain anything at level 1
            if (origin <= 3)
                return 2;

            return 1;
        }
    }
}
