using System;
using System.Runtime.CompilerServices;
using static PKHeX.Core.EvolutionUtil;
using static PKHeX.Core.Species;

namespace PKHeX.Core;

public sealed class EvolutionGroupHOME2 : IEvolutionGroup
{
    public static readonly EvolutionGroupHOME2 Instance = new();

    private static readonly EvolutionEnvironment9a ZA = new();

    public IEvolutionGroup? GetNext(PKM pk, EvolutionOrigin enc) => null;

    public IEvolutionGroup? GetPrevious(PKM pk, EvolutionOrigin enc)
    {
        return null; // TODO HOME ZA2: Re-enable when we have more info.
        // if (enc.Generation > 9 || enc.Context is EntityContext.Gen9a)
        //     return null;
        // return EvolutionGroupHOME.Instance;
    }

    public void DiscardForOrigin(Span<EvoCriteria> result, PKM pk, EvolutionOrigin enc)
    {
        if (pk.ZA) // TODO HOME ZA2: did they force realign everything and fix their bug?
        {
            var table = PersonalTable.ZA;
            if (enc.Options.HasFlag(OriginOptions.SkipChecks))
            {
                Discard(result, table);
                return;
            }

            // Check if ability was possibly realigned by form change; if not, discard anything that doesn't have the ability.
            var index = pk.AbilityNumber >> 1;
            if (index is 0 or 1)
            {
                var didRealignAbility = false;
                var ability = pk.Ability;
                var species = enc.Species;
                if (FormInfo.HasMegaForm(species) || species is (int)Aegislash)
                {
                    if (table[species, pk.Form].GetAbilityAtIndex(index) == ability)
                        didRealignAbility = true;
                }
                if (!didRealignAbility)
                {
                    Discard(result, table, ability, index);
                    return;
                }
            }
            Discard(result, table);
            return;
        }

        if (GetPrevious(pk, enc) is { } prev)
            prev.DiscardForOrigin(result, pk, enc);
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
        (int)Raichu when form == 1 => true,
        (int)Exeggutor when form == 1 => true,
        (int)Marowak when form == 1 => true,
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
            if (ZA.TryDevolve(prev, pk, prev.LevelMax, enc.LevelMin, enc.SkipChecks, out var evo))
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
            if (ZA.TryEvolve(devolved, dest, pk, enc.LevelMax, devolved.LevelMin, enc.SkipChecks, out var evo))
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

        history.Gen9a = SetHistory(result, PersonalTable.ZA);

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
        var (species, form) = (evo.Species, evo.Form);
        // Eager check: only reversions are if form is not 0.
        if (form == 0)
            return;
        // None present in Z-A.
        //if (species is (ushort)Dialga or (ushort)Palkia or (ushort)Arceus or (ushort)Silvally)
        //    evo = evo with { Form = 0 }; // Normal
        if (FormInfo.IsBattleOnlyForm(species, form, Latest.Generation))
            evo = evo with { Form = FormInfo.GetOutOfBattleForm(species, form, Latest.Generation) };
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

        if (pk is PA9) history.Gen9a = SetHistory(result, PersonalTable.ZA);

        return present;
    }

    private static IEvolutionEnvironment GetSingleEnv(PKM pk) => pk switch
    {
        PA9 => ZA,
        _ => throw new ArgumentOutOfRangeException(nameof(pk), pk, null),
    };
}

/// <summary>
/// Evolution environment for <see cref="EntityContext.Gen9a"/>.
/// </summary>
public sealed class EvolutionEnvironment9a : IEvolutionEnvironment
{
    private static readonly EvolutionTree Tree = EvolutionTree.Evolves9a;
    private static EvolutionRuleTweak Tweak => EvolutionRuleTweak.Level100;

    public bool TryDevolve<T>(T head, PKM pk, byte currentMaxLevel, byte levelMin, bool skipChecks, out EvoCriteria result) where T : ISpeciesForm
        => Tree.Reverse.TryDevolve(head, pk, currentMaxLevel, levelMin, skipChecks, Tweak, out result);

    public bool TryEvolve<T>(T head, ISpeciesForm next, PKM pk, byte currentMaxLevel, byte levelMin, bool skipChecks, out EvoCriteria result) where T : ISpeciesForm
        => Tree.Forward.TryEvolve(head, next, pk, currentMaxLevel, levelMin, skipChecks, Tweak, out result);
}
