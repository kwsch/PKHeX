using System;
using System.Collections.Generic;

using static PKHeX.Core.Legal;

namespace PKHeX.Core
{
    public static class EvolutionChain
    {
        private static readonly EvoCriteria[] NONE = Array.Empty<EvoCriteria>();

        internal static EvoCriteria[][] GetEvolutionChainsAllGens(PKM pkm, IEncounterTemplate enc)
        {
            var chain = GetEvolutionChain(pkm, enc, pkm.Species, (byte)pkm.CurrentLevel);
            if (chain.Length == 0 || pkm.IsEgg || enc is EncounterInvalid)
                return GetChainSingle(pkm, chain);

            return GetChainAll(pkm, enc, chain);
        }

        private static EvoCriteria[][] GetAllEmpty(int count)
        {
            var result = new EvoCriteria[count][];
            for (int i = 0; i < result.Length; i++)
                result[i] = NONE; // default no-evolutions
            return result;
        }

        private static EvoCriteria[][] GetChainSingle(PKM pkm, EvoCriteria[] fullChain)
        {
            var chain = GetAllEmpty(Math.Max(2, pkm.Format) + 1);
            chain[pkm.Format] = fullChain;
            return chain;
        }

        private static EvoCriteria[][] GetChainAll(PKM pkm, IEncounterTemplate enc, EvoCriteria[] fullChain)
        {
            int maxgen = ParseSettings.AllowGen1Tradeback && pkm is PK1 ? 2 : pkm.Format;
            var GensEvoChains = GetAllEmpty(maxgen + 1);

            var head = 0; // inlined FIFO queue indexing
            var mostEvolved = fullChain[head++];

            var lvl = (byte)pkm.CurrentLevel;
            var maxLevel = lvl;
            int pkGen = enc.Generation;

            // Iterate generations backwards
            // Maximum level of an earlier generation (GenX) will never be greater than a later generation (GenX+Y).
            int mingen = pkGen >= 3 ? pkGen : GBRestrictions.GetTradebackStatusInitial(pkm) == PotentialGBOrigin.Gen2Only ? 2 : 1;
            bool noxfrDecremented = true;
            for (int g = GensEvoChains.Length - 1; g >= mingen; g--)
            {
                if (pkGen <= 2 && g == 6)
                    g = 2; // skip over 6543 as it never existed in these.

                if (g <= 4 && pkm.Format > 2 && pkm.Format > g && !pkm.HasOriginalMetLocation)
                {
                    // Met location was lost at this point but it also means the pokemon existed in generations 1 to 4 with maximum level equals to met level
                    var met = pkm.Met_Level;
                    if (lvl > pkm.Met_Level)
                        lvl = (byte)met;
                }

                int maxspeciesgen = g == 2 && pkm.VC1 ? MaxSpeciesID_1 : GetMaxSpeciesOrigin(g);

                // Remove future gen evolutions after a few special considerations:
                // If the pokemon origin is illegal (e.g. Gen3 Infernape) the list will be emptied -- species lineage did not exist at any evolution stage.
                while (mostEvolved.Species > maxspeciesgen)
                {
                    if (head >= fullChain.Length)
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
                    mostEvolved = fullChain[head++];
                }

                // Alolan form evolutions, remove from gens 1-6 chains
                if (HasAlolanForm(mostEvolved.Species))
                {
                    if (g < 7 && pkm.Format >= 7 && mostEvolved.Form > 0)
                    {
                        if (head >= fullChain.Length)
                            break;
                        mostEvolved = fullChain[head++];
                    }
                }

                GensEvoChains[g] = GetEvolutionChain(pkm, enc, mostEvolved.Species, lvl);
                ref var genChain = ref GensEvoChains[g];
                if (genChain.Length == 0)
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
                    int minIndex = Array.FindIndex(genChain, e => e.LevelMax >= minlvl);
                    if (minIndex != -1)
                        genChain = genChain.AsSpan(minIndex).ToArray();
                }
                else if (g == 1)
                {
                    // Remove Gen7 pre-evolutions and chain break scenarios
                    if (pkm.VC1)
                        TrimVC1Transfer(pkm, GensEvoChains);

                    ref var lastGen = ref GensEvoChains[1];
                    var g1 = lastGen.AsSpan();
                    // Remove Gen2 post-evolutions (Scizor, Blissey...)
                    if (g1[0].Species > MaxSpeciesID_1)
                    {
                        if (g1.Length == 1)
                        {
                            lastGen = Array.Empty<EvoCriteria>();
                            continue; // done
                        }
                        g1 = g1[1..];
                    }

                    // Remove Gen2 pre-evolutions (Pichu, Cleffa...)
                    if (g1[^1].Species > MaxSpeciesID_1)
                    {
                        if (g1.Length == 1)
                        {
                            lastGen = Array.Empty<EvoCriteria>();
                            continue; // done
                        }
                        g1 = g1[..^1];
                    }

                    if (g1.Length != lastGen.Length)
                        lastGen = g1.ToArray();
                    // Update min level for the encounter to prevent certain level up moves.
                    if (g1.Length != 0)
                    {
                        ref var last = ref g1[^1];
                        last = last with { LevelMin = enc.LevelMin };
                    }
                }
            }
            return GensEvoChains;
        }

        private static bool HasMetLocationUpdatedTransfer(int originalGeneration, int currentGeneration) => originalGeneration switch
        {
            <  3 => currentGeneration >= 3,
            <= 4 => currentGeneration != originalGeneration,
            _    => false,
        };

        private static void TrimVC1Transfer(PKM pkm, EvoCriteria[][] allChains)
        {
            var vc7 = allChains[7];
            var gen1Index = Array.FindIndex(vc7, z => z.Species <= MaxSpeciesID_1);
            if (gen1Index == -1)
                allChains[pkm.Format] = NONE; // needed a Gen1 species present; invalidate the chain.
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

        private static EvoCriteria[] GetEvolutionChain(PKM pkm, IEncounterTemplate enc, int mostEvolvedSpecies, byte maxlevel)
        {
            int min = enc.LevelMin;
            if (pkm.HasOriginalMetLocation && pkm.Met_Level != 0)
                min = pkm.Met_Level;
            var chain = GetValidPreEvolutions(pkm, minLevel: min);
            if (enc.Species == mostEvolvedSpecies)
            {
                if (chain.Length == 1)
                    return chain;
                var index = Array.FindIndex(chain, z => z.Species == enc.Species);
                if (index == -1)
                    return Array.Empty<EvoCriteria>();
                return new[] { chain[index] };
            }

            // Evolution chain is in reverse order (devolution)
            // Find the index of the minimum species to determine the end of the chain
            int minIndex = Array.FindIndex(chain, z => z.Species == enc.Species);
            bool last = minIndex < 0 || minIndex == chain.Length - 1;

            // If we remove a pre-evolution, update the chain if appropriate.
            if (!last)
            {
                // Remove chain species after the encounter
                if (minIndex + 1 == chain.Length)
                    return Array.Empty<EvoCriteria>(); // no species left in chain

                chain = chain.AsSpan(0, minIndex).ToArray();
                CheckLastEncounterRemoval(enc, chain);
            }

            // maxspec is used to remove future geneneration evolutions, to gather evolution chain of a pokemon in previous generations
            var maxSpeciesIndex = Array.FindIndex(chain, z => z.Species == mostEvolvedSpecies);
            if (maxSpeciesIndex > 0)
                chain = chain.AsSpan(maxSpeciesIndex).ToArray();

            // Gen3->4 and Gen4->5 transfer sets the Met Level property to the Pokémon's current level.
            // Removes evolutions impossible before the transfer level.
            // For example a FireRed Charizard with a current level (in XY) is 50 but Met Level is 20; it couldn't be a Charizard in Gen3 and Gen4 games
            var clampIndex = Array.FindIndex(chain, z => z.LevelMin > maxlevel);
            if (clampIndex != -1)
                chain = Array.FindAll(chain, z => z.LevelMin <= maxlevel);

            // Reduce the evolution chain levels to max level to limit any later analysis/results.
            for (var i = 0; i < chain.Length; i++)
            {
                ref var c = ref chain[i];
                c = c with { LevelMax = Math.Min(c.LevelMax, maxlevel) };
            }

            return chain;
        }

        private static void CheckLastEncounterRemoval(IEncounterTemplate enc, EvoCriteria[] chain)
        {
            // Last entry from chain is removed, turn next entry into the encountered Pokémon
            ref var last = ref chain[^1];
            last = last with { LevelMin = enc.LevelMin, LevelUpRequired = 1 };

            ref var first = ref chain[0];
            if (first.RequiresLvlUp)
                return;

            if (first.LevelMin == 2)
            {
                // Example: Raichu in Gen2 or later
                // Because Pichu requires a level up, the minimum level of the resulting Raichu must be be >2
                // But after removing Pichu (because the origin species is Pikachu), the Raichu minimum level should be 1.
                first = first with { LevelMin = 1, LevelUpRequired = 0 };
            }
            else // in-game trade or evolution stone can evolve immediately
            {
                first = first with { LevelMin = enc.LevelMin };
            }
        }

        internal static EvoCriteria[] GetValidPreEvolutions(PKM pkm, int maxspeciesorigin = -1, int maxLevel = -1, int minLevel = 1, bool skipChecks = false)
        {
            if (maxLevel < 0)
                maxLevel = pkm.CurrentLevel;

            if (maxspeciesorigin == -1 && pkm.InhabitedGeneration(2) && pkm.Format <= 2 && pkm.Generation == 1)
                maxspeciesorigin = MaxSpeciesID_2;

            int tree = Math.Max(2, pkm.Format);
            var et = EvolutionTree.GetEvolutionTree(pkm, tree);
            return et.GetValidPreEvolutions(pkm, maxLevel: (byte)maxLevel, maxSpeciesOrigin: maxspeciesorigin, skipChecks: skipChecks, minLevel: (byte)minLevel);
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
