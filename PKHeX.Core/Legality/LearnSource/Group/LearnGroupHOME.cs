using System;

namespace PKHeX.Core;

public class LearnGroupHOME : ILearnGroup
{
    public static readonly LearnGroupHOME Instance = new();
    public ILearnGroup? GetPrevious(PKM pk, EvolutionHistory history, IEncounterTemplate enc, LearnOption option) => null;
    public bool HasVisited(PKM pk, EvolutionHistory history) => pk is IHomeTrack { HasTracker: true } || !ParseSettings.IgnoreTransferIfNoTracker;

    public bool Check(Span<MoveResult> result, ReadOnlySpan<ushort> current, PKM pk, EvolutionHistory history,
        IEncounterTemplate enc, MoveSourceType types = MoveSourceType.All, LearnOption option = LearnOption.Current)
    {
        // For now until we discover edge cases, just handle the top-species for a given game.
        var evos = history.Get(pk.Context);
        if (history.HasVisitedGen9 && pk is not PK9)
        {
            var instance = LearnSource9SV.Instance;
            for (var i = 0; i < evos.Length; i++)
                Check<LearnSource9SV, PersonalInfo9SV>(result, current, pk, evos[i], 9, i, types, instance, LearnOption.AtAnyTime);
        }
        if (history.HasVisitedSWSH && pk is not PK8)
        {
            var instance = LearnSource8SWSH.Instance;
            for (var i = 0; i < evos.Length; i++)
                Check<LearnSource8SWSH, PersonalInfo8SWSH>(result, current, pk, evos[i], 8, i, types, instance, LearnOption.AtAnyTime);
        }
        if (history.HasVisitedPLA && pk is not PA8)
        {
            var instance = LearnSource8LA.Instance;
            for (var i = 0; i < evos.Length; i++)
                Check<LearnSource8LA, PersonalInfo8LA>(result, current, pk, evos[i], 8, i, types, instance, LearnOption.AtAnyTime);
        }
        if (history.HasVisitedBDSP && pk is not PB8)
        {
            var instance = LearnSource8BDSP.Instance;
            for (var i = 0; i < evos.Length; i++)
                Check<LearnSource8BDSP, PersonalInfo8BDSP>(result, current, pk, evos[i], 8, i, types, instance, LearnOption.AtAnyTime);
        }

        return MoveResult.AllParsed(result);
    }

    private static void Check<TSource, TPersonal>(Span<MoveResult> result, ReadOnlySpan<ushort> current, PKM pk, EvoCriteria evo, byte generation, int stage, MoveSourceType type,
        TSource inst, LearnOption option) where TSource : ILearnSource<TPersonal> where TPersonal : PersonalInfo
    {
        if (!FormChangeUtil.ShouldIterateForms(evo.Species, evo.Form, generation, option))
        {
            CheckInternal<TSource, TPersonal>(result, current, pk, evo, generation, stage, type, inst, option);
            return;
        }

        // Check all forms
        if (!inst.TryGetPersonal(evo.Species, evo.Form, out var pi))
            return;

        var fc = pi.FormCount;
        for (int i = 0; i < fc; i++)
            CheckInternal<TSource, TPersonal>(result, current, pk, evo with { Form = (byte)i }, generation, stage, type, inst, option);
    }

    private static void CheckInternal<TSource, TPersonal>(Span<MoveResult> result, ReadOnlySpan<ushort> current, PKM pk, EvoCriteria evo,
        byte generation, int stage, MoveSourceType type, TSource inst, LearnOption option) where TSource : ILearnSource<TPersonal> where TPersonal : PersonalInfo
    {
        if (!inst.TryGetPersonal(evo.Species, evo.Form, out var pi))
            return;

        for (int i = result.Length - 1; i >= 0; i--)
        {
            if (result[i].Valid)
                continue;

            var move = current[i];
            var chk = inst.GetCanLearn(pk, pi, evo, move, type, option);
            if (chk != default)
                result[i] = new(chk, (byte)stage, generation);
        }
    }

    public void GetAllMoves(Span<bool> result, PKM pk, EvolutionHistory history, IEncounterTemplate enc,
        MoveSourceType types = MoveSourceType.All, LearnOption option = LearnOption.Current)
    {
    }
}
