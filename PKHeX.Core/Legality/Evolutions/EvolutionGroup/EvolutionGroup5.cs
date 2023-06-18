using System;

namespace PKHeX.Core;

public sealed class EvolutionGroup5 : IEvolutionGroup
{
    public static readonly EvolutionGroup5 Instance = new();
    private static readonly EvolutionTree Tree = EvolutionTree.Evolves5;
    private const int Generation = 5;
    private static PersonalTable5B2W2 Personal => PersonalTable.B2W2;

    public IEvolutionGroup? GetNext(PKM pk, EvolutionOrigin enc) => pk.Format > Generation ? EvolutionGroup6.Instance : null;
    public IEvolutionGroup? GetPrevious(PKM pk, EvolutionOrigin enc) => enc.Generation < Generation ? EvolutionGroup4.Instance : null;
    public void DiscardForOrigin(Span<EvoCriteria> result, PKM pk) => EvolutionUtil.Discard(result, Personal);

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

    public bool TryDevolve(ISpeciesForm head, PKM pk, byte currentMaxLevel, int levelMin, bool skipChecks, out EvoCriteria result)
    {
        return Tree.Reverse.TryDevolve(head, pk, currentMaxLevel, levelMin, skipChecks, out result);
    }

    public int Evolve(Span<EvoCriteria> result, PKM pk, EvolutionOrigin enc, EvolutionHistory history)
    {
        int present = 1;
        for (int i = result.Length - 1; i >= 1; i--)
        {
            ref var dest = ref result[i - 1];
            var devolved = result[i];
            if (!TryEvolve(devolved, dest, pk, enc.LevelMax, devolved.LevelMin, enc.SkipChecks, out var evo))
                continue;

            if (evo.IsBetterEvolution(dest))
                dest = evo;
            present++;
        }
        history.Gen5 = EvolutionUtil.SetHistory(result, Personal);
        return present;
    }

    public bool TryEvolve(ISpeciesForm head, ISpeciesForm next, PKM pk, byte currentMaxLevel, byte levelMin, bool skipChecks, out EvoCriteria result)
    {
        return Tree.Forward.TryEvolve(head, next, pk, currentMaxLevel, levelMin, skipChecks, out result);
    }
}
