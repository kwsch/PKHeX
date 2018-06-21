using System;
using System.Collections.Generic;
using System.Linq;

using static PKHeX.Core.Legal;

namespace PKHeX.Core
{
    public static class EvolutionChain
    {
        private static readonly EvoCriteria[] NONE = new EvoCriteria[0];
        internal static EvoCriteria[][] GetEvolutionChainsAllGens(PKM pkm, IEncounterable Encounter)
        {
            var CompleteEvoChain = GetEvolutionChain(pkm, Encounter);
            int maxgen = !pkm.Gen1_NotTradeback && pkm.Format == 1 ? 2 : pkm.Format;
            int mingen = !pkm.Gen2_NotTradeback && (pkm.Format == 2 || pkm.VC2) ? 1 : pkm.GenNumber;

            var GensEvoChains = new EvoCriteria[maxgen + 1][];
            for (int i = 0; i <= maxgen; i++)
                GensEvoChains[i] = NONE; // default no-evolutions

            if (pkm.Species == 0 || pkm.Format > 2 && pkm.GenU)
            {
                // Illegal origin or empty pokemon, return only chain for current format
                GensEvoChains[pkm.Format] = CompleteEvoChain;
                return GensEvoChains;
            }

            if (pkm.IsEgg)
            {
                // Skip the other checks and just return the evo chain for the Format; contains only the pokemon inside the egg
                int gen = pkm.Format;
                if (GetMaxSpeciesOrigin(gen) >= pkm.Species)
                    GensEvoChains[gen] = CompleteEvoChain;
                return GensEvoChains;
            }

            return TrimEvoChain(pkm, Encounter, CompleteEvoChain, maxgen, mingen, GensEvoChains);
        }
        private static EvoCriteria[][] TrimEvoChain(PKM pkm, IEncounterable Encounter, EvoCriteria[] CompleteEvoChain, int maxgen, int mingen, EvoCriteria[][] GensEvoChains)
        {
            if (CompleteEvoChain.Length == 0)
                return GensEvoChains;

            int lvl = pkm.CurrentLevel;
            int maxLevel = lvl;
            int pkGen = pkm.GenNumber;

            var queue = new Queue<EvoCriteria>(CompleteEvoChain);
            var mostEvolved = queue.Dequeue();

            // Iterate generations backwards
            // Maximum level of an earlier generation (GenX) will never be greater than a later generation (GenX+Y).
            for (int g = maxgen; g >= mingen; g--)
            {
                if (pkGen == 1 && pkm.Gen1_NotTradeback && g == 2)
                    continue;
                if (pkGen <= 2 && 3 <= g && g <= 6)
                    continue;

                if (g <= 4 && 2 < pkm.Format && g < pkm.Format && !pkm.HasOriginalMetLocation && lvl > pkm.Met_Level)
                {
                    // Met location was lost at this point but it also means the pokemon existed in generations 1 to 4 with maximum level equals to met level
                    lvl = pkm.Met_Level;
                }

                int maxspeciesgen = g == 2 && pkm.VC1 ? MaxSpeciesID_1 : GetMaxSpeciesOrigin(g);

                // Remove future gen evolutions after a few special considerations:
                // If the pokemon origin is illegal (e.g. Gen3 Infernape) the list will be emptied -- species lineage did not exist at any evolution stage.
                while (mostEvolved.Species > maxspeciesgen)
                {
                    if (mostEvolved.RequiresLvlUp)
                    {
                        // Eevee requires a single levelup to be Sylveon, it can be deduced in gen 5 and before it existed with maximum one level below current
                        if (g == 5 && mostEvolved.Species == 700)
                            lvl--;

                        // This is a gen 3 pokemon in a gen 4 phase evolution that requieres level up and then transfered to gen 5+
                        // We can deduce that it existed in gen 4 until met level,
                        // but if current level is met level we can also deduce it existed in gen 3 until maximum met level -1
                        else if (g == 3 && pkm.Format > 4 && lvl == maxLevel && mostEvolved.Species > maxspeciesgen)
                            lvl--;

                        // The same condition for gen2 evolution of gen 1 pokemon, level of the pokemon in gen 1 games would be CurrentLevel -1 one level below gen 2 level
                        else if (g == 1 && pkm.Format == 2 && lvl == maxLevel && mostEvolved.Species > maxspeciesgen)
                            lvl--;
                    }
                    if (queue.Count == 1)
                        break;
                    mostEvolved = queue.Dequeue();
                }

                // Alolan form evolutions, remove from gens 1-6 chains
                if (EvolveToAlolanForms.Contains(mostEvolved.Species))
                {
                    if (g < 7 && pkm.Format >= 7 && mostEvolved.Form > 0)
                    {
                        if (queue.Count == 1)
                            break;
                        mostEvolved = queue.Dequeue();
                    }
                }

                GensEvoChains[g] = GetEvolutionChain(pkm, Encounter, mostEvolved.Species, lvl);
                if (GensEvoChains[g].Length == 0)
                    continue;

                if (g > 2 && !pkm.HasOriginalMetLocation && g >= pkGen)
                {
                    bool isTransferred = GetCanPruneChainTransfer(pkm, pkGen, g);
                    if (isTransferred)
                    {
                        // Remove previous evolutions below transfer level
                        // For example a gen3 Charizard in format 7 with current level 36 and met level 36, thus could never be Charmander / Charmeleon in Gen5+.
                        // chain level for charmander is 35, is below met level.
                        int minlvl = GetMinLevelGeneration(pkm, g);
                        GensEvoChains[g] = GensEvoChains[g].Where(e => e.Level >= minlvl).ToArray();
                    }
                }
                else if (g == 2 && pkm.TradebackStatus == TradebackType.Gen1_NotTradeback)
                {
                    GensEvoChains[2] = NONE;
                }
                else if (g == 1 && GensEvoChains[g][GensEvoChains[g].Length - 1].Species > MaxSpeciesID_1)
                {
                    // Remove generation 2 pre-evolutions
                    GensEvoChains[1] = GensEvoChains[1].Take(GensEvoChains[1].Length - 1).ToArray();
                    if (!pkm.VC1)
                        continue;

                    // Remove generation 2 pre-evolutions from gen 7 and future generations
                    for (int fgen = 7; fgen <= maxgen; fgen++)
                    {
                        var chain = GensEvoChains[fgen];
                        var g1Index = Array.FindIndex(chain, e => e.Species <= MaxSpeciesID_1);
                        if (g1Index < 0) // no g2 species present
                            continue;
                        if (g1Index + 1 == chain.Length) // already pruned or no g2prevo
                            continue;
                        GensEvoChains[fgen] = chain.Take(g1Index + 1).ToArray();
                    }
                }
            }
            return GensEvoChains;
        }
        private static EvoCriteria[] GetEvolutionChain(PKM pkm, IEncounterable Encounter)
        {
            return GetEvolutionChain(pkm, Encounter, pkm.Species, 100);
        }
        private static EvoCriteria[] GetEvolutionChain(PKM pkm, IEncounterable Encounter, int maxspec, int maxlevel)
        {
            var vs = GetValidPreEvolutions(pkm).ToArray();

            // Evolution chain is in reverse order (devolution)
            int minspec = Encounter.Species;

            int minindex = Math.Max(0, Array.FindIndex(vs, p => p.Species == minspec));
            Array.Resize(ref vs, minindex + 1);
            var last = vs[vs.Length - 1];
            if (last.MinLevel > 1) // Last entry from vs is removed, turn next entry into the wild/hatched pokemon
            {
                last.MinLevel = Encounter.LevelMin;
                last.RequiresLvlUp = false;
                var first = vs[0];
                if (!first.RequiresLvlUp)
                {
                    if (first.MinLevel == 2)
                    {
                        // Example Raichu in gen 2 or later,
                        // because Pichu requires level up Minimum level of Raichu would be 2
                        // but after removing Pichu because the origin species is Pikachu, Raichu min level should be 1
                        first.MinLevel = 1;
                        first.RequiresLvlUp = false;
                    }
                    else // ingame trade / stone can evolve immediately
                    {
                        first.MinLevel = last.MinLevel;
                    }
                }
            }
            // Maxspec is used to remove future gen evolutions, to gather evolution chain of a pokemon in previous generations
            int skip = Math.Max(0, Array.FindIndex(vs, p => p.Species == maxspec));
            // Maxlevel is also used for previous generations, it removes evolutions imposible before the transfer level
            // For example a fire red charizard whose current level in XY is 50 but met level is 20, it couldnt be a Charizard in gen 3 and 4 games
            vs = vs.Skip(skip).Where(e => e.MinLevel <= maxlevel).ToArray();
            // Reduce the evolution chain levels to max level, because met level is the last one when the pokemon could be and learn moves in that generation
            foreach (var d in vs)
                d.Level = Math.Min(d.Level, maxlevel);
            return vs;
        }
        internal static IReadOnlyList<EvoCriteria> GetValidPreEvolutions(PKM pkm, int maxspeciesorigin = -1, int lvl = -1, bool skipChecks = false)
        {
            if (lvl < 0)
                lvl = pkm.CurrentLevel;
            if (pkm.IsEgg && !skipChecks)
                return new List<EvoCriteria>
                {
                    new EvoCriteria { Species = pkm.Species, Level = lvl, MinLevel = lvl },
                };
            if (pkm.Species == 292 && lvl >= 20 && (!pkm.HasOriginalMetLocation || pkm.Met_Level + 1 <= lvl))
                return new List<EvoCriteria>
                {
                    new EvoCriteria { Species = 292, Level = lvl, MinLevel = 20 },
                    new EvoCriteria { Species = 290, Level = pkm.GenNumber < 5 ? lvl : lvl-1, MinLevel = 1 }
                    // Shedinja spawns after evolving, which is after level up moves were prompted. Not for future generations.
                };
            if (maxspeciesorigin == -1 && pkm.InhabitedGeneration(2) && pkm.GenNumber == 1)
                maxspeciesorigin = MaxSpeciesID_2;

            int tree = maxspeciesorigin == MaxSpeciesID_2 ? 2 : pkm.Format;
            var et = EvolutionTree.GetEvolutionTree(tree);
            return et.GetValidPreEvolutions(pkm, maxLevel: lvl, maxSpeciesOrigin: maxspeciesorigin, skipChecks: skipChecks);
        }

        private static bool GetCanPruneChainTransfer(PKM pkm, int originGen, int currentGen)
        {
            // For transferred species, rule out pre-evolutions where their max level is not obtainable in the specified generation.
            // Only prune entries for gen values that would overwrite the met data.
            bool isTransferred = HasMetLocationUpdatedTransfer(originGen, currentGen);
            if (pkm.Format >= 5 && currentGen == 4 && originGen == 3)
                return false; // can't prune as the 3->4 data is overwritten again 4->5
            return isTransferred;
        }
        private static int GetMinLevelGeneration(PKM pkm, int generation)
        {
            if (!pkm.InhabitedGeneration(generation))
                return 0;

            if (pkm.Format <= 2)
                return 2;

            if (!pkm.HasOriginalMetLocation && generation != pkm.GenNumber)
                return pkm.Met_Level;

            if (pkm.GenNumber <= 3)
                return 2;

            return 1;
        }

        /// <summary>
        /// Gets an enumerable list of species IDs from the possible lineage of the <see cref="pkm"/> based on its current format.
        /// </summary>
        /// <param name="pkm">Pokémon to fetch the lineage for.</param>
        internal static IEnumerable<int> GetLineage(PKM pkm)
        {
            if (pkm.IsEgg)
                return new[] { pkm.Species };

            var table = EvolutionTree.GetEvolutionTree(pkm.Format);
            var lineage = table.GetValidPreEvolutions(pkm, maxLevel: pkm.CurrentLevel);
            return lineage.Select(evolution => evolution.Species);
        }
    }
}
