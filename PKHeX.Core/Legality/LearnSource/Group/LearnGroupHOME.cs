using System;
using System.Buffers;

namespace PKHeX.Core;

public class LearnGroupHOME : ILearnGroup
{
    public static readonly LearnGroupHOME Instance = new();
    public ILearnGroup? GetPrevious(PKM pk, EvolutionHistory history, IEncounterTemplate enc, LearnOption option) => null;
    public bool HasVisited(PKM pk, EvolutionHistory history) => pk is IHomeTrack { HasTracker: true } || !ParseSettings.IgnoreTransferIfNoTracker;

    public bool Check(Span<MoveResult> result, ReadOnlySpan<ushort> current, PKM pk, EvolutionHistory history,
        IEncounterTemplate enc, MoveSourceType types = MoveSourceType.All, LearnOption option = LearnOption.Current)
    {
        option = LearnOption.AtAnyTime;
        var local = GetCurrent(pk.Context);
        var evos = history.Get(pk.Context);
        if (history.HasVisitedGen9 && pk is not PK9)
        {
            var instance = LearnGroup9.Instance;
            instance.Check(result, current, pk, history, enc, types, option);
            if (CleanPurge(result, current, pk, types, local, evos))
                return true;
        }
        if (history.HasVisitedSWSH && pk is not PK8)
        {
            var instance = LearnGroup8.Instance;
            instance.Check(result, current, pk, history, enc, types, option);
            if (CleanPurge(result, current, pk, types, local, evos))
                return true;
        }
        if (history.HasVisitedPLA && pk is not PA8)
        {
            var instance = LearnGroup8a.Instance;
            instance.Check(result, current, pk, history, enc, types, option);
            if (CleanPurge(result, current, pk, types, local, evos))
                return true;
        }
        if (history.HasVisitedBDSP && pk is not PB8)
        {
            var instance = LearnGroup8b.Instance;
            instance.Check(result, current, pk, history, enc, types, option);
            if (CleanPurge(result, current, pk, types, local, evos))
                return true;
        }
        if (history.HasVisitedLGPE)
        {
            var instance = LearnGroup7b.Instance;
            instance.Check(result, current, pk, history, enc, types, option);
            if (CleanPurge(result, current, pk, types, local, evos))
                return true;
        }
        else if (history.HasVisitedGen7)
        {
            ILearnGroup instance = LearnGroup7.Instance;
            while (true)
            {
                instance.Check(result, current, pk, history, enc, types, option);
                if (CleanPurge(result, current, pk, types, local, evos))
                    return true;
                var prev = instance.GetPrevious(pk, history, enc, option);
                if (prev is null)
                    break;
                instance = prev;
            }
        }
        return false;
    }

    private static bool CleanPurge(Span<MoveResult> result, ReadOnlySpan<ushort> current, PKM pk, MoveSourceType types, IHomeSource local, EvoCriteria[] evos)
    {
        for (int i = 0; i < result.Length; i++)
        {
            var r = result[i];
            if (!r.Valid)
                continue;

            if (r.Info.Environment == local.Environment)
                continue;

            var move = current[i];
            bool valid = true;
            foreach (var evo in evos)
            {
                var chk = local.GetCanLearnHOME(pk, evo, move, types);
                if (chk.Method != LearnMethod.None)
                    valid = true;
            }
            if (!valid)
                result[i] = default;
        }

        return MoveResult.AllParsed(result);
    }

    private static IHomeSource GetCurrent(EntityContext context) => context switch
    {
        EntityContext.Gen8 => LearnSource8SWSH.Instance,
        EntityContext.Gen8a => LearnSource8LA.Instance,
        EntityContext.Gen8b => LearnSource8BDSP.Instance,
        EntityContext.Gen9 => LearnSource9SV.Instance,
        _ => throw new ArgumentOutOfRangeException(nameof(context), context, null),
    };

    public void GetAllMoves(Span<bool> result, PKM pk, EvolutionHistory history, IEncounterTemplate enc,
        MoveSourceType types = MoveSourceType.All, LearnOption option = LearnOption.Current)
    {
        option = LearnOption.AtAnyTime;
        var evos = history.Get(pk.Context);
        var local = GetCurrent(pk.Context);
        var rent = ArrayPool<bool>.Shared.Rent(result.Length);
        var temp = rent.AsSpan(0, result.Length);

        if (history.HasVisitedGen9 && pk is not PK9)
        {
            var instance = LearnGroup9.Instance;
            instance.GetAllMoves(temp, pk, history, enc, types, option);
            foreach (var evo in evos)
                LoopMerge(result, pk, evo, types, local, temp);
        }
        if (history.HasVisitedSWSH && pk is not PK8)
        {
            var instance = LearnGroup8.Instance;
            instance.GetAllMoves(temp, pk, history, enc, types, option);
            foreach (var evo in evos)
                LoopMerge(result, pk, evo, types, local, temp);
        }
        if (history.HasVisitedPLA && pk is not PA8)
        {
            var instance = LearnGroup8a.Instance;
            instance.GetAllMoves(temp, pk, history, enc, types, option);
            foreach (var evo in evos)
                LoopMerge(result, pk, evo, types, local, temp);
        }
        if (history.HasVisitedBDSP && pk is not PB8)
        {
            var instance = LearnGroup8b.Instance;
            instance.GetAllMoves(temp, pk, history, enc, types, option);
            foreach (var evo in evos)
                LoopMerge(result, pk, evo, types, local, temp);
        }
        if (history.HasVisitedLGPE)
        {
            var instance = LearnGroup7b.Instance;
            instance.GetAllMoves(temp, pk, history, enc, types, option);
            foreach (var evo in evos)
                LoopMerge(result, pk, evo, types, local, temp);
        }
        else if (history.HasVisitedGen7)
        {
            ILearnGroup instance = LearnGroup7.Instance;
            while (true)
            {
                instance.GetAllMoves(temp, pk, history, enc, types, option);
                foreach (var evo in evos)
                    LoopMerge(result, pk, evo, types, local, temp);
                var prev = instance.GetPrevious(pk, history, enc, option);
                if (prev is null)
                    break;
                instance = prev;
            }
        }

        ArrayPool<bool>.Shared.Return(rent);
    }

    private static void LoopMerge(Span<bool> result, PKM pk, EvoCriteria evo, MoveSourceType types, IHomeSource dest, Span<bool> temp)
    {
        for (int i = 0; i < temp.Length; i++)
        {
            if (!temp[i])
                continue;
            if (result[i])
                continue;

            var chk = dest.GetCanLearnHOME(pk, evo, (ushort)i, types);
            if (chk.Method != LearnMethod.None)
                result[i] = true;
        }
    }
}
