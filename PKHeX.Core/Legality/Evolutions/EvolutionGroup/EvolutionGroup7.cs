using System;

namespace PKHeX.Core;

public sealed class EvolutionGroup7 : IEvolutionGroup
{
    public static readonly EvolutionGroup7 Instance = new();
    private static readonly EvolutionTree Tree = EvolutionTree.Evolves7;
    private const byte Generation = 7;
    private static PersonalTable7 Personal => PersonalTable.USUM;
    private static EvolutionRuleTweak Tweak => EvolutionRuleTweak.Default;

    public IEvolutionGroup? GetNext(PKM pk, EvolutionOrigin enc) => pk.Format > Generation ? EvolutionGroupHOME.Instance : null;
    public IEvolutionGroup? GetPrevious(PKM pk, EvolutionOrigin enc)
    {
        if (enc.Generation is 1 or 2)
            return EvolutionGroup2.Instance;
        if (enc.Generation < Generation)
            return EvolutionGroup6.Instance;
        return null;
    }

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
        // Zygarde's 10% Form and 50% Form can be changed with the help of external tools: the Reassembly Unit and the Zygarde Cube.
        if (evo is { Species: (ushort)Species.Zygarde, Form: not (0 or 1) })
            evo = evo with { Form = evo.LevelMax == 63 ? (byte)1 : (byte)0 }; // 50% Forme
        else if (evo is { Species: (ushort)Species.Silvally, Form: not 0 })
            evo = evo with { Form = 0 }; // Normal
    }

    public int Evolve(Span<EvoCriteria> result, PKM pk, EvolutionOrigin enc, EvolutionHistory history)
    {
        if (enc.Generation <= 2) // VC Transfer
            EvolutionUtil.UpdateFloor(result, pk.MetLevel, enc.LevelMax);

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
        history.Gen7 = EvolutionUtil.SetHistory(result, Personal);
        return present;
    }

    public bool TryDevolve<T>(T head, PKM pk, byte currentMaxLevel, byte levelMin, bool skipChecks, out EvoCriteria result) where T : ISpeciesForm
    {
        return Tree.Reverse.TryDevolve(head, pk, currentMaxLevel, levelMin, skipChecks, Tweak, out result);
    }

    public bool TryEvolve<T>(T head, ISpeciesForm next, PKM pk, byte currentMaxLevel, byte levelMin, bool skipChecks, out EvoCriteria result) where T : ISpeciesForm
    {
        var b = Tree.Forward.TryEvolve(head, next, pk, currentMaxLevel, levelMin, skipChecks, Tweak, out result);
        return b && !IsEvolutionBanned(pk, result);
    }

    // Kanto Evolutions are not accessible unless it visits US/UM.
    private static bool IsEvolutionBanned(PKM pk, in ISpeciesForm dest) => pk is PK7 { SM: true, IsUntraded: true } && dest switch
    {
        { Species: (ushort)Species.Raichu, Form: 0 } => true,
        { Species: (ushort)Species.Marowak, Form: 0 } => true,
        { Species: (ushort)Species.Exeggutor, Form: 0 } => true,
        _ => false,
    };
}
