using System;

namespace PKHeX.Core;

public sealed class EvolutionGroup1 : IEvolutionGroup, IEvolutionEnvironment
{
    public static readonly EvolutionGroup1 Instance = new();
    private static readonly EvolutionTree Tree = EvolutionTree.Evolves1;
    private static EvolutionRuleTweak Tweak => EvolutionRuleTweak.Default;

    public IEvolutionGroup GetNext(PKM pk, EvolutionOrigin enc) => EvolutionGroup2.Instance;
    public IEvolutionGroup? GetPrevious(PKM pk, EvolutionOrigin enc) => pk.Format == 1 && ParseSettings.AllowGen1Tradeback ? EvolutionGroup2.Instance : null;

    public void DiscardForOrigin(Span<EvoCriteria> result, PKM pk, EvolutionOrigin enc)
    {
        if (!ParseSettings.AllowGen1Tradeback)
            return; // no other groups were iterated, so no need to discard

        if (enc.Generation == 1)
            EvolutionUtil.Discard(result, PersonalTable.RB);
        else
            EvolutionUtil.Discard(result, PersonalTable.C);
    }

    public int Devolve(Span<EvoCriteria> result, PKM pk, EvolutionOrigin enc)
    {
        if (pk.Format >= 7 && !enc.SkipChecks)
        {
            var max = pk.MetLevel;
            enc = enc with { LevelMin = 2, LevelMax = max };
        }
        int present = 1;
        for (int i = 1; i < result.Length; i++)
        {
            var prev = result[i - 1];
            if (!TryDevolve(prev, pk, prev.LevelMax, enc.LevelMin, enc.SkipChecks, out var evo))
                continue;

            ref var reference = ref result[i];
            if (evo.IsBetterDevolution(reference))
                reference = evo;
            present++;
        }
        return present;
    }

    public bool TryDevolve<T>(T head, PKM pk, byte currentMaxLevel, byte levelMin, bool skipChecks, out EvoCriteria result)
        where T : ISpeciesForm
    {
        return Tree.Reverse.TryDevolve(head, pk, currentMaxLevel, levelMin, skipChecks, Tweak, out result);
    }

    public int Evolve(Span<EvoCriteria> result, PKM pk, EvolutionOrigin enc, EvolutionHistory history)
    {
        if (pk.Format > 2)
            enc = enc with { LevelMax = pk.MetLevel };

        int present = 1;
        for (int i = result.Length - 1; i >= 1; i--)
        {
            ref var dest = ref result[i - 1];
            var devolved = result[i];
            if (!TryEvolve(devolved, dest, pk, enc.LevelMax, devolved.LevelMin, enc.SkipChecks, out var evo))
            {
                if (dest.Method == EvoCriteria.SentinelNotReached)
                    break; // Don't continue for higher evolutions.
                continue;
            }

            if (evo.IsBetterEvolution(dest))
                dest = evo;
            present++;
        }
        history.Gen1 = EvolutionUtil.SetHistory(result, PersonalTable.RB);
        return present;
    }

    public bool TryEvolve<T>(T head, ISpeciesForm next, PKM pk, byte currentMaxLevel, byte levelMin, bool skipChecks, out EvoCriteria result) where T : ISpeciesForm
    {
        return Tree.Forward.TryEvolve(head, next, pk, currentMaxLevel, levelMin, skipChecks, Tweak, out result);
    }
}
