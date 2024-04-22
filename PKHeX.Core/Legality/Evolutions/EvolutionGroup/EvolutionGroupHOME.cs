using System;
using System.Runtime.CompilerServices;
using static PKHeX.Core.GameVersion;
using static PKHeX.Core.EvolutionUtil;

namespace PKHeX.Core;

public sealed class EvolutionGroupHOME : IEvolutionGroup
{
    public static readonly EvolutionGroupHOME Instance = new();

    private static readonly EvolutionEnvironment8 SWSH = new();
    private static readonly EvolutionEnvironment8a LA = new();
    private static readonly EvolutionEnvironment8b BDSP = new();
    private static readonly EvolutionEnvironment9 SV = new();

    public IEvolutionGroup? GetNext(PKM pk, EvolutionOrigin enc) => null;

    public IEvolutionGroup? GetPrevious(PKM pk, EvolutionOrigin enc)
    {
        if (enc.Generation >= 8)
            return null;
        if (enc.Version is GP or GE or GG or GO)
            return EvolutionGroup7b.Instance;
        return EvolutionGroup7.Instance;
    }

    public void DiscardForOrigin(Span<EvoCriteria> result, PKM pk, EvolutionOrigin enc)
    {
        if (pk.SV)
            Discard(result, PersonalTable.SV);
        else if (pk.LA)
            Discard(result, PersonalTable.LA);
        else if (pk.BDSP)
            Discard(result, PersonalTable.BDSP);
        else if (pk.SWSH)
            Discard(result, PersonalTable.SWSH);
        else if (pk.GO && result.Length >= 2 && result[1].Species == (ushort)Species.Gimmighoul)
            result[1] = result[1] with { Form = 1 }; // Roaming, exclusive GO form
        // GO can be otherwise, don't discard any.
    }

    /// <summary>
    /// Checks if we should check all adjacent evolution sources in addition to the current one.
    /// </summary>
    /// <returns>True if we should check all adjacent evolution sources.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool CheckAllAdjacent(PKM pk, EvolutionOrigin enc) => enc.SkipChecks || pk is IHomeTrack { HasTracker: true } || !ParseSettings.IgnoreTransferIfNoTracker;

    public int Devolve(Span<EvoCriteria> result, PKM pk, EvolutionOrigin enc)
    {
        if (CheckAllAdjacent(pk, enc))
            return DevolveMulti(result, pk, enc);
        return DevolveSingle(result, pk, enc);
    }

    public int Evolve(Span<EvoCriteria> result, PKM pk, EvolutionOrigin enc, EvolutionHistory history)
    {
        if (IsUnavailableEvoChain(pk.Species, pk.Form))
            result = result[..1]; // Only allow the highest (current).
        if (CheckAllAdjacent(pk, enc))
            return EvolveMulti(result, pk, enc, history);
        return EvolveSingle(result, pk, enc, history);
    }

    private static bool IsUnavailableEvoChain(ushort species, byte form) => species switch
    {
        // Split-evolution Alolans can't be reached in any game Gen8+. Must have been via Gen7.
        (int)Species.Raichu when form == 1 => true,
        (int)Species.Exeggutor when form == 1 => true,
        (int)Species.Marowak when form == 1 => true,
        _ => false,
    };

    private int DevolveMulti(Span<EvoCriteria> result, PKM pk, in EvolutionOrigin enc)
    {
        int present = 1;
        for (int i = 1; i < result.Length; i++)
        {
            ref var prev = ref result[i - 1];
            RevertMutatedForms(ref prev);
            ref var reference = ref result[i];

            bool devolvedAny = false;
            if (SWSH.TryDevolve(prev, pk, prev.LevelMax, enc.LevelMin, enc.SkipChecks, out var evo))
                devolvedAny = UpdateIfBetter(ref reference, evo);
            if (LA  .TryDevolve(prev, pk, prev.LevelMax, enc.LevelMin, enc.SkipChecks, out evo))
                devolvedAny = UpdateIfBetter(ref reference, evo);
            if (BDSP.TryDevolve(prev, pk, prev.LevelMax, enc.LevelMin, enc.SkipChecks, out evo))
                devolvedAny = UpdateIfBetter(ref reference, evo);
            if (SV  .TryDevolve(prev, pk, prev.LevelMax, enc.LevelMin, enc.SkipChecks, out evo))
                devolvedAny = UpdateIfBetter(ref reference, evo);

            if (devolvedAny)
                present++;

            static bool UpdateIfBetter(ref EvoCriteria reference, in EvoCriteria evo)
            {
                if (evo.IsBetterDevolution(reference))
                    reference = evo;
                return true;
            }
        }

        return present;
    }

    private int EvolveMulti(Span<EvoCriteria> result, PKM pk, in EvolutionOrigin enc, EvolutionHistory history)
    {
        int present = 1;
        for (int i = result.Length - 1; i >= 1; i--)
        {
            ref var dest = ref result[i - 1];
            var devolved = result[i];

            bool devolvedAny = false;
            if (SWSH.TryEvolve(devolved, dest, pk, enc.LevelMax, devolved.LevelMin, enc.SkipChecks, out var evo))
                devolvedAny = UpdateIfBetter(ref dest, evo);
            if (LA  .TryEvolve(devolved, dest, pk, enc.LevelMax, devolved.LevelMin, enc.SkipChecks, out evo))
                devolvedAny = UpdateIfBetter(ref dest, evo);
            if (BDSP.TryEvolve(devolved, dest, pk, enc.LevelMax, devolved.LevelMin, enc.SkipChecks, out evo))
                devolvedAny = UpdateIfBetter(ref dest, evo);
            if (SV  .TryEvolve(devolved, dest, pk, enc.LevelMax, devolved.LevelMin, enc.SkipChecks, out evo))
                devolvedAny = UpdateIfBetter(ref dest, evo);

            if (devolvedAny)
                present++;
            else if (dest.Method == EvoCriteria.SentinelNotReached)
                break; // Don't continue for higher evolutions.

            static bool UpdateIfBetter(ref EvoCriteria reference, in EvoCriteria evo)
            {
                if (evo.IsBetterEvolution(reference))
                    reference = evo;
                return true;
            }
        }

        // Can't leave BD/SP. Abort early.
        if (pk is { Species: (int)Species.Nincada or (int)Species.Spinda, BDSP: true })
        {
            history.Gen8b = SetHistory(result, PersonalTable.BDSP);
            return present;
        }

        history.Gen9 = SetHistory(result, PersonalTable.SV);
        history.Gen8  = SetHistory(result, PersonalTable.SWSH);
        if (pk is { Species: (int)Species.Raichu, Form: 1 } && history.HasVisitedGen7)
            return present; // Alolan Raichu can't enter Gen8a/b.
        history.Gen8a = SetHistory(result, PersonalTable.LA);
        if (pk is { Species: (int)Species.Nincada or (int)Species.Spinda })
            return present; // Nincada/Spinda can't enter Gen8b.
        history.Gen8b = SetHistory(result, PersonalTable.BDSP);

        return present;
    }

    private static int DevolveSingle(Span<EvoCriteria> result, PKM pk, in EvolutionOrigin enc)
    {
        int present = 1;
        var env = GetSingleEnv(pk);
        for (int i = 1; i < result.Length; i++)
        {
            ref var prev = ref result[i - 1];
            RevertMutatedForms(ref prev);
            if (!env.TryDevolve(prev, pk, prev.LevelMax, enc.LevelMin, enc.SkipChecks, out var evo))
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
        if (evo is { Species: (ushort)Species.Dialga, Form: not 0 })
            evo = evo with { Form = 0 }; // Normal
        else if (evo is { Species: (ushort)Species.Palkia, Form: not 0 })
            evo = evo with { Form = 0 }; // Normal
        else if (evo is { Species: (ushort)Species.Silvally, Form: not 0 })
            evo = evo with { Form = 0 }; // Normal
    }

    private int EvolveSingle(Span<EvoCriteria> result, PKM pk, in EvolutionOrigin enc, EvolutionHistory history)
    {
        int present = 1;
        var env = GetSingleEnv(pk);
        for (int i = result.Length - 1; i >= 1; i--)
        {
            ref var dest = ref result[i - 1];
            var devolved = result[i];
            if (!env.TryEvolve(devolved, dest, pk, enc.LevelMax, devolved.LevelMin, enc.SkipChecks, out var evo))
            {
                if (dest.Method == EvoCriteria.SentinelNotReached)
                    break; // Don't continue for higher evolutions.
                continue;
            }

            if (evo.IsBetterEvolution(dest))
                dest = evo;
            present++;
        }

        if      (pk is PK8) history.Gen8  = SetHistory(result, PersonalTable.SWSH);
        else if (pk is PA8) history.Gen8a = SetHistory(result, PersonalTable.LA);
        else if (pk is PB8) history.Gen8b = SetHistory(result, PersonalTable.BDSP);
        else if (pk is PK9) history.Gen9  = SetHistory(result, PersonalTable.SV);

        return present;
    }

    private static IEvolutionEnvironment GetSingleEnv(PKM pk) => pk switch
    {
        PK8 => SWSH,
        PA8 => LA,
        PB8 => BDSP,
        PK9 => SV,
        _ => throw new ArgumentOutOfRangeException(nameof(pk), pk, null),
    };
}

public sealed class EvolutionEnvironment8 : IEvolutionEnvironment
{
    private static readonly EvolutionTree Tree = EvolutionTree.Evolves8;
    private static EvolutionRuleTweak Tweak => EvolutionRuleTweak.Default;

    public bool TryDevolve<T>(T head, PKM pk, byte currentMaxLevel, byte levelMin, bool skipChecks, out EvoCriteria result) where T : ISpeciesForm
    {
        return Tree.Reverse.TryDevolve(head, pk, currentMaxLevel, levelMin, skipChecks, Tweak, out result);
    }

    public bool TryEvolve<T>(T head, ISpeciesForm next, PKM pk, byte currentMaxLevel, byte levelMin, bool skipChecks, out EvoCriteria result) where T : ISpeciesForm
    {
        var b = Tree.Forward.TryEvolve(head, next, pk, currentMaxLevel, levelMin, skipChecks, Tweak, out result);
        return b && !IsEvolutionBanned(pk, head);
    }

    // Gigantamax Pikachu, Meowth, and Eevee are prevented from evolving.
    private static bool IsEvolutionBanned(PKM pk, in ISpeciesForm head) => head.Species switch
    {
        (int)Species.Pikachu => pk is PK8 { CanGigantamax: true },
        (int)Species.Meowth => pk is PK8 { CanGigantamax: true },
        (int)Species.Eevee => pk is PK8 { CanGigantamax: true },
        _ => false,
    };
}

public sealed class EvolutionEnvironment8a : IEvolutionEnvironment
{
    private static readonly EvolutionTree Tree = EvolutionTree.Evolves8a;
    private static EvolutionRuleTweak Tweak => EvolutionRuleTweak.Default;

    public bool TryDevolve<T>(T head, PKM pk, byte currentMaxLevel, byte levelMin, bool skipChecks, out EvoCriteria result) where T : ISpeciesForm
        => Tree.Reverse.TryDevolve(head, pk, currentMaxLevel, levelMin, skipChecks, Tweak, out result);

    public bool TryEvolve<T>(T head, ISpeciesForm next, PKM pk, byte currentMaxLevel, byte levelMin, bool skipChecks, out EvoCriteria result) where T : ISpeciesForm
        => Tree.Forward.TryEvolve(head, next, pk, currentMaxLevel, levelMin, skipChecks, Tweak, out result);
}

public sealed class EvolutionEnvironment8b : IEvolutionEnvironment
{
    private static readonly EvolutionTree Tree = EvolutionTree.Evolves8b;
    private static EvolutionRuleTweak Tweak => EvolutionRuleTweak.Default;

    public bool TryDevolve<T>(T head, PKM pk, byte currentMaxLevel, byte levelMin, bool skipChecks, out EvoCriteria result) where T : ISpeciesForm
        => Tree.Reverse.TryDevolve(head, pk, currentMaxLevel, levelMin, skipChecks, Tweak, out result);

    public bool TryEvolve<T>(T head, ISpeciesForm next, PKM pk, byte currentMaxLevel, byte levelMin, bool skipChecks, out EvoCriteria result) where T : ISpeciesForm
        => Tree.Forward.TryEvolve(head, next, pk, currentMaxLevel, levelMin, skipChecks, Tweak, out result);
}

public sealed class EvolutionEnvironment9 : IEvolutionEnvironment
{
    private static readonly EvolutionTree Tree = EvolutionTree.Evolves9;
    private static EvolutionRuleTweak Tweak => EvolutionRuleTweak.Level100;

    public bool TryDevolve<T>(T head, PKM pk, byte currentMaxLevel, byte levelMin, bool skipChecks, out EvoCriteria result) where T : ISpeciesForm
        => Tree.Reverse.TryDevolve(head, pk, currentMaxLevel, levelMin, skipChecks, Tweak, out result);

    public bool TryEvolve<T>(T head, ISpeciesForm next, PKM pk, byte currentMaxLevel, byte levelMin, bool skipChecks, out EvoCriteria result) where T : ISpeciesForm
        => Tree.Forward.TryEvolve(head, next, pk, currentMaxLevel, levelMin, skipChecks, Tweak, out result);
}
