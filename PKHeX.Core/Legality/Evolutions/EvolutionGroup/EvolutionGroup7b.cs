using System;

namespace PKHeX.Core;

public sealed class EvolutionGroup7b : IEvolutionGroup
{
    public static readonly EvolutionGroup7b Instance = new();
    private static readonly EvolutionTree Tree = EvolutionTree.Evolves7b;
    private const byte Generation = 7;
    private static PersonalTable7GG Personal => PersonalTable.GG;
    private static EvolutionRuleTweak Tweak => EvolutionRuleTweak.Default;

    public IEvolutionGroup? GetNext(PKM pk, EvolutionOrigin enc) => pk.Format > Generation ? EvolutionGroupHOME.Instance : null;
    public IEvolutionGroup? GetPrevious(PKM pk, EvolutionOrigin enc) => null;
    public void DiscardForOrigin(Span<EvoCriteria> result, PKM pk, EvolutionOrigin enc) => EvolutionUtil.Discard(result, Personal);

    public int Devolve(Span<EvoCriteria> result, PKM pk, EvolutionOrigin enc)
    {
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

    public bool TryDevolve<T>(T head, PKM pk, byte currentMaxLevel, byte levelMin, bool skipChecks, out EvoCriteria result) where T : ISpeciesForm
    {
        return Tree.Reverse.TryDevolve(head, pk, currentMaxLevel, levelMin, skipChecks, Tweak, out result);
    }

    public int Evolve(Span<EvoCriteria> result, PKM pk, EvolutionOrigin enc, EvolutionHistory history)
    {
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
        history.Gen7b = EvolutionUtil.SetHistory(result, Personal);
        return present;
    }

    public bool TryEvolve<T>(T head, ISpeciesForm next, PKM pk, byte currentMaxLevel, byte levelMin, bool skipChecks, out EvoCriteria result) where T : ISpeciesForm
    {
        return Tree.Forward.TryEvolve(head, next, pk, currentMaxLevel, levelMin, skipChecks, Tweak, out result);
    }
}
