using System;

namespace PKHeX.Core;

public sealed class EvolutionGroup6 : IEvolutionGroup
{
    public static readonly EvolutionGroup6 Instance = new();
    private static readonly EvolutionTree Tree = EvolutionTree.Evolves6;
    private const byte Generation = 6;
    private static PersonalTable6AO Personal => PersonalTable.AO;
    private static EvolutionRuleTweak Tweak => EvolutionRuleTweak.Default;

    public IEvolutionGroup? GetNext(PKM pk, EvolutionOrigin enc) => pk.Format > Generation ? EvolutionGroup7.Instance : null;
    public IEvolutionGroup? GetPrevious(PKM pk, EvolutionOrigin enc) => enc.Generation < Generation ? EvolutionGroup5.Instance : null;
    public void DiscardForOrigin(Span<EvoCriteria> result, PKM pk, EvolutionOrigin enc) => EvolutionUtil.Discard(result, Personal);

    public int Devolve(Span<EvoCriteria> result, PKM pk, EvolutionOrigin enc)
    {
        int present = 1;
        for (int i = 1; i < result.Length; i++)
        {
            ref var prev = ref result[i - 1];
            RevertMutatedForms(ref prev);
            if (!TryDevolve(prev, pk, prev.LevelMax, enc.LevelMin, enc.SkipChecks, out var evo))
                continue;

            ref var reference = ref result[i];
            if (evo.IsBetterDevolution(reference))
                reference = evo;
            present++;
        }
        return present;
    }

    private static void RevertMutatedForms(ref EvoCriteria evo)
    {
        if (evo is { Species: (ushort)Species.Arceus, Form: 17 }) // Fairy, prevent it hitting Gen5 (as Fairy does not exist yet).
            evo = evo with { Form = 0 }; // Normal
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
        history.Gen6 = EvolutionUtil.SetHistory(result, Personal);
        return present;
    }

    public bool TryEvolve<T>(T head, ISpeciesForm next, PKM pk, byte currentMaxLevel, byte levelMin, bool skipChecks, out EvoCriteria result) where T : ISpeciesForm
    {
        return Tree.Forward.TryEvolve(head, next, pk, currentMaxLevel, levelMin, skipChecks, Tweak, out result);
    }
}
