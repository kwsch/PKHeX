using System;
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
        if ((GameVersion)enc.Version is GP or GE or GG or GO)
            return EvolutionGroup7b.Instance;
        if (enc.Generation < 8)
            return EvolutionGroup7.Instance;

        return null;
    }

    public void DiscardForOrigin(Span<EvoCriteria> result, PKM pk)
    {
        if (pk.SV)
            Discard(result, PersonalTable.SV);
        else if (pk.LA)
            Discard(result, PersonalTable.LA);
        else if (pk.BDSP)
            Discard(result, PersonalTable.BDSP);
        else
            Discard(result, PersonalTable.SWSH);
    }

    public int Devolve(Span<EvoCriteria> result, PKM pk, EvolutionOrigin enc)
    {
        if (enc.SkipChecks || pk is IHomeTrack { HasTracker: true } || !ParseSettings.IgnoreTransferIfNoTracker)
            return DevolveMulti(result, pk, enc);
        return DevolveSingle(result, pk, enc);
    }

    public int Evolve(Span<EvoCriteria> result, PKM pk, EvolutionOrigin enc, EvolutionHistory history)
    {
        if (enc.SkipChecks || pk is IHomeTrack { HasTracker: true } || !ParseSettings.IgnoreTransferIfNoTracker)
            return EvolveMulti(result, pk, enc, history);
        return EvolveSingle(result, pk, enc, history);
    }

    private int DevolveMulti(Span<EvoCriteria> result, PKM pk, in EvolutionOrigin enc)
    {
        int present = 1;
        for (int i = 1; i < result.Length; i++)
        {
            var prev = result[i - 1];
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

            static bool UpdateIfBetter(ref EvoCriteria reference, in EvoCriteria evo)
            {
                if (evo.IsBetterEvolution(reference))
                    reference = evo;
                return true;
            }
        }

        SetHistory(result, PersonalTable.SWSH, out history.Gen8);
        SetHistory(result, PersonalTable.LA,   out history.Gen8a);
        SetHistory(result, PersonalTable.BDSP, out history.Gen8b);
        SetHistory(result, PersonalTable.SV,   out history.Gen9);

        return present;
    }

    private static int DevolveSingle(Span<EvoCriteria> result, PKM pk, in EvolutionOrigin enc)
    {
        int present = 1;
        var env = GetSingleEnv(pk);
        for (int i = 1; i < result.Length; i++)
        {
            var prev = result[i - 1];
            if (!env.TryDevolve(prev, pk, prev.LevelMax, enc.LevelMin, enc.SkipChecks, out var evo))
                continue;

            ref var reference = ref result[i];
            if (evo.IsBetterDevolution(reference))
                reference = evo;
            present++;
        }

        return present;
    }

    private int EvolveSingle(Span<EvoCriteria> result, PKM pk, in EvolutionOrigin enc, EvolutionHistory history)
    {
        int present = 0;
        var env = GetSingleEnv(pk);
        for (int i = result.Length - 1; i >= 1; i--)
        {
            var prev = result[i - 1];
            ref var reference = ref result[i];
            if (!env.TryEvolve(prev, reference, pk, enc.LevelMax, prev.LevelMin, enc.SkipChecks, out var evo))
                continue;

            if (evo.IsBetterEvolution(reference))
                reference = evo;
            present++;
        }

        if      (pk is PK8) SetHistory(result, PersonalTable.SWSH, out history.Gen8);
        else if (pk is PA8) SetHistory(result, PersonalTable.LA,   out history.Gen8a);
        else if (pk is PB8) SetHistory(result, PersonalTable.BDSP, out history.Gen8b);
        else if (pk is PK9) SetHistory(result, PersonalTable.SV,   out history.Gen9);

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

    public bool TryDevolve(ISpeciesForm head, PKM pk, byte currentMaxLevel, byte levelMin, bool skipChecks, out EvoCriteria result)
        => Tree.Reverse.TryDevolve(head, pk, currentMaxLevel, levelMin, skipChecks, out result);

    public bool TryEvolve(ISpeciesForm head, ISpeciesForm next, PKM pk, byte currentMaxLevel, byte levelMin, bool skipChecks, out EvoCriteria result)
        => Tree.Forward.TryEvolve(head, next, pk, currentMaxLevel, levelMin, skipChecks, out result);
}

public sealed class EvolutionEnvironment8a : IEvolutionEnvironment
{
    private static readonly EvolutionTree Tree = EvolutionTree.Evolves8a;

    public bool TryDevolve(ISpeciesForm head, PKM pk, byte currentMaxLevel, byte levelMin, bool skipChecks, out EvoCriteria result)
        => Tree.Reverse.TryDevolve(head, pk, currentMaxLevel, levelMin, skipChecks, out result);

    public bool TryEvolve(ISpeciesForm head, ISpeciesForm next, PKM pk, byte currentMaxLevel, byte levelMin, bool skipChecks, out EvoCriteria result)
        => Tree.Forward.TryEvolve(head, next, pk, currentMaxLevel, levelMin, skipChecks, out result);
}

public sealed class EvolutionEnvironment8b : IEvolutionEnvironment
{
    private static readonly EvolutionTree Tree = EvolutionTree.Evolves8b;

    public bool TryDevolve(ISpeciesForm head, PKM pk, byte currentMaxLevel, byte levelMin, bool skipChecks, out EvoCriteria result)
        => Tree.Reverse.TryDevolve(head, pk, currentMaxLevel, levelMin, skipChecks, out result);

    public bool TryEvolve(ISpeciesForm head, ISpeciesForm next, PKM pk, byte currentMaxLevel, byte levelMin, bool skipChecks, out EvoCriteria result)
        => Tree.Forward.TryEvolve(head, next, pk, currentMaxLevel, levelMin, skipChecks, out result);
}

public sealed class EvolutionEnvironment9 : IEvolutionEnvironment
{
    private static readonly EvolutionTree Tree = EvolutionTree.Evolves9;

    public bool TryDevolve(ISpeciesForm head, PKM pk, byte currentMaxLevel, byte levelMin, bool skipChecks, out EvoCriteria result)
        => Tree.Reverse.TryDevolve(head, pk, currentMaxLevel, levelMin, skipChecks, out result);

    public bool TryEvolve(ISpeciesForm head, ISpeciesForm next, PKM pk, byte currentMaxLevel, byte levelMin, bool skipChecks, out EvoCriteria result)
        => Tree.Forward.TryEvolve(head, next, pk, currentMaxLevel, levelMin, skipChecks, out result);
}
