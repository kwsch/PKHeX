using System;
using System.Collections.Generic;

using static PKHeX.Core.Legal;

namespace PKHeX.Core
{
    public static class EvolutionChain
    {
        private static readonly List<EvoCriteria> NONE = new List<EvoCriteria>(0);

        internal static IReadOnlyList<EvoCriteria>[] GetEvolutionChainsAllGens(PKM pkm, IEncounterable enc)
        {
            var fullChain = GetValidPreEvolutions(pkm, skipChecks: true);
            if (enc is EncounterInvalid || pkm.IsEgg || fullChain.Count == 0)
                return GetChainSingle(pkm, enc, fullChain);

            return GetChainAll(pkm, enc, fullChain);
        }

        private static List<EvoCriteria>[] GetChainBase(int maxgen)
        {
            var GensEvoChains = new List<EvoCriteria>[maxgen + 1];
            for (int i = 0; i <= maxgen; i++)
                GensEvoChains[i] = NONE; // default no-evolutions
            return GensEvoChains;
        }

        private static List<EvoCriteria>[] GetChainSingle(PKM pkm, IEncounterable enc, List<EvoCriteria> CompleteEvoChain)
        {
            var chain = GetChainBase(Math.Max(2, pkm.Format));
            if (pkm.Species != enc.Species)
                chain[pkm.Format] = NONE;
            else
                chain[pkm.Format] = CompleteEvoChain;
            return chain;
        }

        private static List<EvoCriteria>[] GetChainAll(PKM pkm, IEncounterable enc, IReadOnlyList<EvoCriteria> fullChain)
        {
            int genBirth = enc.Generation;
            int genMin = genBirth >= 3 ? genBirth : pkm.Gen2_NotTradeback ? 2 : 1;
            int genMax = pkm is PK1 && !pkm.Gen1_NotTradeback ? 2 : pkm.Format;

            // Fill up a template array for all generations it might inhabit.
            var all = GetChainBase(genMax);

            // The maximum level of a generation will never be greater than that of a future generation.
            // Decrement the permitted level when appropriate.
            int maxLevel = pkm.CurrentLevel;

            var queue = new Queue<EvoCriteria>(fullChain.Count);

            // Iterate from the current generation down to the minimum generation it can inhabit.
            for (int gen = genMax; gen >= genMin; gen--)
            {
                if (genBirth <= 2 && 3 <= gen && gen <= 6)
                    continue; // can't exist in these games

                // Reset evolution queue
                queue.Clear();
                foreach (var elem in fullChain)
                    queue.Enqueue(elem);
                var mostEvolved = queue.Dequeue();

                maxLevel = GetMaxLevelForGeneration(pkm, gen, maxLevel);
                int minLevel = GetMinLevelForGeneration(pkm, gen, enc.LevelMin, genBirth);

                while (true)
                {
                    var chain = GetEvolutionChain(pkm, gen, mostEvolved.Species, mostEvolved.Form, minLevel, maxLevel);
                    if (chain.Count == 0)
                        return all;

                    int encIndex = chain.FindIndex(z => z.Species == enc.Species);
                    if (encIndex == -1) // not found
                    {
                        if (queue.Count == 0)
                            return all; // no valid evolution chain for this encounter
                        if (chain.Count == 1 && fullChain.Count == 3)
                        {
                            // get lowest evolved species from future chain, if exists
                            mostEvolved = queue.Dequeue();
                            continue;
                        }
                    }

                    chain.RemoveAll(e => e.MinLevel > maxLevel);

                    // if stuff is after the encounter species, discard them
                    if (encIndex >= 0 && encIndex + 1 != chain.Count)
                        chain.RemoveRange(encIndex + 1, chain.Count - 1 - encIndex);
                    all[gen] = chain;
                    break;
                }
            }

            // Prune the chains for backwards transfer situations
            if (pkm.VC1)
            {
                if (pkm.Gen1_NotTradeback)
                    all[2] = NONE;

                // Remove any Gen2 pre-evolution as it can only exist as a Gen1 species prior to transfer.
                for (int i = 7; i < all.Length; i++)
                {
                    var chain = all[i];
                    int g1Species = chain.FindLastIndex(z => z.Species <= MaxSpeciesID_1);
                    if (g1Species >= 0 && g1Species + 1 != chain.Count)
                        chain.RemoveRange(g1Species + 1, chain.Count - 1 - g1Species);
                }
            }

            // Invalidate current-gen chain if the current species can't exist
            if (all[genMax].FindIndex(z => z.Species == pkm.Species) < 0)
                all[genMax] = NONE;

            return all;
        }

        private static int GetMinLevelForGeneration(PKM pkm, int gen, int lvl, int birth)
        {
            return birth switch
            {
                // VC Transfers
                1 => (gen >= 7 ? Math.Max(lvl, pkm.Met_Level) : lvl),
                2 => (gen >= 7 ? Math.Max(lvl, pkm.Met_Level) : lvl),

                // Pal Park
                3 => (gen >= 4 ? Math.Max(lvl, pkm.Met_Level) : lvl),

                // Poke Transfer
                4 => (gen >= 5 ? Math.Max(lvl, pkm.Met_Level) : lvl),

                _ => lvl
            };
        }

        private static int GetMaxLevelForGeneration(PKM pkm, int gen, int lvl)
        {
            return gen switch
            {
                // VC Transfers
                1 => (pkm.Format >= 7 ? Math.Min(lvl, pkm.Met_Level) : lvl),
                2 => (pkm.Format >= 7 ? Math.Min(lvl, pkm.Met_Level) : lvl),

                // Pal Park
                3 => (pkm.Format >= 4 ? Math.Min(lvl, pkm.Met_Level) : lvl),

                // Poke Transfer
                4 => (pkm.Format >= 5 ? Math.Min(lvl, pkm.Met_Level) : lvl),

                _ => lvl
            };
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

        private static List<EvoCriteria> GetEvolutionChain(PKM pkm, int gen, int species, int form, int minLevel, int maxLevel)
        {
            var et = EvolutionTree.GetEvolutionTree(pkm, gen);
            var chain = EvolutionTree.GetEmptyEvoChain();
            int maxSpeciesOrigin = GetMaxSpeciesOrigin(gen);
            et.FillTree(pkm, chain, species, form, minLevel, maxLevel, false, maxSpeciesOrigin: maxSpeciesOrigin);
            return chain;
        }

        internal static List<EvoCriteria> GetValidPreEvolutions(PKM pkm, int maxspeciesorigin = -1, int lvl = -1, int minLevel = 1, bool skipChecks = false)
        {
            if (lvl < 0)
                lvl = pkm.CurrentLevel;

            if (maxspeciesorigin == -1 && pkm.InhabitedGeneration(2) && pkm.Format <= 2 && pkm.GenNumber == 1)
                maxspeciesorigin = MaxSpeciesID_2;

            int tree = Math.Max(2, pkm.Format);
            var et = EvolutionTree.GetEvolutionTree(pkm, tree);
            return et.GetValidPreEvolutions(pkm, maxLevel: lvl, maxSpeciesOrigin: maxspeciesorigin, skipChecks: skipChecks, minLevel: minLevel);
        }
    }
}
