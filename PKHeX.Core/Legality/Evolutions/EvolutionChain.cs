using System;

using static PKHeX.Core.Legal;

namespace PKHeX.Core;

public static class EvolutionChain
{
    internal static EvolutionHistory GetEvolutionChainsAllGens(PKM pk, IEncounterTemplate enc)
    {
        var chain = GetEvolutionChain(pk, enc, pk.Species, (byte)pk.CurrentLevel);
        if (chain.Length == 0 || pk.IsEgg || enc is EncounterInvalid)
            return GetChainSingle(pk, chain);

        return GetChainAll(pk, enc, chain);
    }

    private static EvolutionHistory GetChainSingle(PKM pk, EvoCriteria[] fullChain)
    {
        var count = Math.Max(2, pk.Format) + 1;
        return new EvolutionHistory(fullChain, count)
        {
            [pk.Format] = fullChain,
        };
    }

    private static EvolutionHistory GetChainAll(PKM pk, IEncounterTemplate enc, EvoCriteria[] fullChain)
    {
        int maxgen = ParseSettings.AllowGen1Tradeback && pk.Context == EntityContext.Gen1 ? 2 : pk.Format;
        var GensEvoChains = new EvolutionHistory(fullChain, maxgen + 1);

        var head = 0; // inlined FIFO queue indexing
        var mostEvolved = fullChain[head++];

        var lvl = (byte)pk.CurrentLevel;
        var maxLevel = lvl;

        // Iterate generations backwards
        // Maximum level of an earlier generation (GenX) will never be greater than a later generation (GenX+Y).
        int mingen = enc.Generation;
        if (mingen is 1 or 2)
            mingen = GBRestrictions.GetTradebackStatusInitial(pk) == PotentialGBOrigin.Gen2Only ? 2 : 1;

        bool noxfrDecremented = true;
        for (int g = GensEvoChains.Length - 1; g >= mingen; g--)
        {
            if (g == 6 && enc.Generation < 3)
                g = 2; // skip over 6543 as it never existed in these.

            if (g <= 4 && pk.Format > 2 && pk.Format > g && !pk.HasOriginalMetLocation)
            {
                // Met location was lost at this point but it also means the pokemon existed in generations 1 to 4 with maximum level equals to met level
                var met = pk.Met_Level;
                if (lvl > pk.Met_Level)
                    lvl = (byte)met;
            }

            int maxspeciesgen = g == 2 && pk.VC1 ? MaxSpeciesID_1 : GetMaxSpeciesOrigin(g);

            // Remove future gen evolutions after a few special considerations:
            // If the pokemon origin is illegal (e.g. Gen3 Infernape) the list will be emptied -- species lineage did not exist at any evolution stage.
            while (mostEvolved.Species > maxspeciesgen)
            {
                if (head >= fullChain.Length)
                {
                    if (g <= 2 && pk.VC1)
                        GensEvoChains.Invalidate(); // invalidate here since we haven't reached the regular invalidation
                    return GensEvoChains;
                }
                if (mostEvolved.RequiresLvlUp)
                    ReviseMaxLevel(ref lvl, pk, g, maxLevel);

                mostEvolved = fullChain[head++];
            }

            // Alolan form evolutions, remove from gens 1-6 chains
            if (g < 7 && HasAlolanForm(mostEvolved.Species) && pk.Format >= 7 && mostEvolved.Form > 0)
            {
                if (head >= fullChain.Length)
                    return GensEvoChains;
                mostEvolved = fullChain[head++];
            }

            var tmp = GetEvolutionChain(pk, enc, mostEvolved.Species, lvl);
            if (tmp.Length == 0)
                continue;

            GensEvoChains[g] = tmp;
            if (g == 1)
            {
                CleanGen1(pk, enc, GensEvoChains);
                continue;
            }

            if (g >= 3 && !pk.HasOriginalMetLocation && g >= enc.Generation && noxfrDecremented)
            {
                bool isTransferred = HasMetLocationUpdatedTransfer(enc.Generation, g);
                if (!isTransferred)
                    continue;

                noxfrDecremented = g > (enc.Generation != 3 ? 4 : 5);

                // Remove previous evolutions below transfer level
                // For example a gen3 Charizard in format 7 with current level 36 and met level 36, thus could never be Charmander / Charmeleon in Gen5+.
                // chain level for Charmander is 35, is below met level.
                int minlvl = GetMinLevelGeneration(pk, g);

                ref var genChain = ref GensEvoChains[g];
                int minIndex = Array.FindIndex(genChain, e => e.LevelMax >= minlvl);
                if (minIndex != -1)
                    genChain = genChain.AsSpan(minIndex).ToArray();
            }
        }
        return GensEvoChains;
    }

    private static void ReviseMaxLevel(ref byte lvl, PKM pk, int g, byte maxLevel)
    {
        // This is a Gen3 pokemon in a Gen4 phase evolution that requires level up and then transferred to Gen5+
        // We can deduce that it existed in Gen4 until met level,
        // but if current level is met level we can also deduce it existed in Gen3 until maximum met level -1
        if (g == 3 && pk.Format > 4 && lvl == maxLevel)
            lvl--;

        // The same condition for Gen2 evolution of Gen1 pokemon, level of the pokemon in Gen1 games would be CurrentLevel -1 one level below Gen2 level
        else if (g == 1 && pk.Format == 2 && lvl == maxLevel)
            lvl--;
    }

    private static void CleanGen1(PKM pk, IEncounterTemplate enc, EvolutionHistory chains)
    {
        // Remove Gen7 pre-evolutions and chain break scenarios
        if (pk.VC1)
        {
            var index = Array.FindLastIndex(chains.Gen7, z => z.Species <= MaxSpeciesID_1);
            if (index == -1)
            {
                chains.Invalidate(); // needed a Gen1 species present; invalidate the chain.
                return;
            }
        }

        TrimSpeciesAbove(enc, MaxSpeciesID_1, ref chains.Gen1);
    }

    private static void TrimSpeciesAbove(IEncounterTemplate enc, int species, ref EvoCriteria[] chain)
    {
        var span = chain.AsSpan();

        // Remove post-evolutions
        if (span[0].Species > species)
        {
            if (span.Length == 1)
            {
                chain = Array.Empty<EvoCriteria>();
                return;
            }

            span = span[1..];
        }

        // Remove pre-evolutions
        if (span[^1].Species > species)
        {
            if (span.Length == 1)
            {
                chain = Array.Empty<EvoCriteria>();
                return;
            }

            span = span[..^1];
        }

        if (span.Length != chain.Length)
            chain = span.ToArray();

        // Update min level for the encounter to prevent certain level up moves.
        if (span.Length != 0)
        {
            ref var last = ref span[^1];
            last = last with { LevelMin = enc.LevelMin };
        }
    }

    private static bool HasMetLocationUpdatedTransfer(int originalGeneration, int currentGeneration) => originalGeneration switch
    {
        <  3 => currentGeneration >= 3,
        <= 4 => currentGeneration != originalGeneration,
        _    => false,
    };

    private static EvoCriteria[] GetEvolutionChain(PKM pk, IEncounterTemplate enc, int mostEvolvedSpecies, byte maxlevel)
    {
        int min = enc.LevelMin;
        if (pk.HasOriginalMetLocation && pk.Met_Level != 0)
            min = pk.Met_Level;

        var chain = GetValidPreEvolutions(pk, minLevel: min);
        return TrimChain(chain, enc, mostEvolvedSpecies, maxlevel);
    }

    private static EvoCriteria[] TrimChain(EvoCriteria[] chain, IEncounterTemplate enc, int mostEvolvedSpecies, byte maxlevel)
    {
        if (enc.Species == mostEvolvedSpecies)
            return TrimChainSingle(chain, enc);

        // Evolution chain is in reverse order (devolution)
        // Find the index of the minimum species to determine the end of the chain
        int minIndex = Array.FindLastIndex(chain, z => z.Species == enc.Species);
        bool last = minIndex < 0 || minIndex == chain.Length - 1;

        // If we remove a pre-evolution, update the chain if appropriate.
        if (!last)
        {
            // Remove chain species after the encounter
            if (minIndex + 1 == chain.Length)
                return Array.Empty<EvoCriteria>(); // no species left in chain

            chain = chain.AsSpan(0, minIndex + 1).ToArray();
            CheckLastEncounterRemoval(enc, chain);
        }

        return TrimChainMore(chain, mostEvolvedSpecies, maxlevel);
    }

    private static EvoCriteria[] TrimChainMore(EvoCriteria[] chain, int mostEvolvedSpecies, byte maxlevel)
    {
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
        SanitizeMaxLevel(chain, maxlevel);

        return chain;
    }

    private static void SanitizeMaxLevel(EvoCriteria[] chain, byte maxlevel)
    {
        for (var i = 0; i < chain.Length; i++)
        {
            ref var c = ref chain[i];
            c = c with { LevelMax = Math.Min(c.LevelMax, maxlevel) };
        }
    }

    private static EvoCriteria[] TrimChainSingle(EvoCriteria[] chain, IEncounterTemplate enc)
    {
        if (chain.Length == 1)
            return chain;
        var index = Array.FindLastIndex(chain, z => z.Species == enc.Species);
        if (index == -1)
            return Array.Empty<EvoCriteria>();
        return new[] { chain[index] };
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

    internal static EvoCriteria[] GetValidPreEvolutions(PKM pk, int maxspeciesorigin = -1, int maxLevel = -1, int minLevel = 1, bool skipChecks = false)
    {
        if (maxLevel < 0)
            maxLevel = pk.CurrentLevel;

        if (maxspeciesorigin == -1 && pk.InhabitedGeneration(2) && pk.Format <= 2 && pk.Generation == 1)
            maxspeciesorigin = MaxSpeciesID_2;

        var context = pk.Context;
        if (context < EntityContext.Gen2)
            context = EntityContext.Gen2;
        var et = EvolutionTree.GetEvolutionTree(context);
        return et.GetValidPreEvolutions(pk, maxLevel: (byte)maxLevel, maxSpeciesOrigin: maxspeciesorigin, skipChecks: skipChecks, minLevel: (byte)minLevel);
    }

    private static int GetMinLevelGeneration(PKM pk, int generation)
    {
        if (!pk.InhabitedGeneration(generation))
            return 0;

        if (pk.Format <= 2)
            return 2;

        var origin = pk.Generation;
        if (!pk.HasOriginalMetLocation && generation != origin)
            return pk.Met_Level;

        // gen 3 and prior can't obtain anything at level 1
        if (origin <= 3)
            return 2;

        return 1;
    }
}
