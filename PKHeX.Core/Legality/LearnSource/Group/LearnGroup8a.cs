using System;

namespace PKHeX.Core;

/// <summary>
/// Group that checks the source of a move in <see cref="GameVersion.PLA"/>.
/// </summary>
public sealed class LearnGroup8a : ILearnGroup
{
    public static readonly LearnGroup8a Instance = new();
    private const byte Generation = 8;
    private const EntityContext Context = EntityContext.Gen8a;
    public ushort MaxMoveID => Legal.MaxMoveID_8a;

    public ILearnGroup? GetPrevious(PKM pk, EvolutionHistory history, IEncounterTemplate enc, LearnOption option) => null;
    public bool HasVisited(PKM pk, EvolutionHistory history) => history.HasVisitedPLA;

    public bool Check(Span<MoveResult> result, ReadOnlySpan<ushort> current, PKM pk, EvolutionHistory history,
        IEncounterTemplate enc, MoveSourceType types = MoveSourceType.All, LearnOption option = LearnOption.Current)
    {
        var evos = history.Gen8a;
        for (var i = 0; i < evos.Length; i++)
            Check(result, current, pk, evos[i], i);

        if (MoveResult.AllParsed(result))
            return true;

        var home = LearnGroupHOME.Instance;
        if (option != LearnOption.HOME && home.HasVisited(pk, history))
            return home.Check(result, current, pk, history, enc, types);
        return false;
    }

    private static void Check(Span<MoveResult> result, ReadOnlySpan<ushort> current, PKM pk, EvoCriteria evo, int stage)
    {
        var game = LearnSource8LA.Instance;
        if (!game.TryGetPersonal(evo.Species, evo.Form, out var pi))
            return; // should never happen.

        for (int i = result.Length - 1; i >= 0; i--)
        {
            if (result[i].Valid)
                continue;

            var move = current[i];
            var chk = game.GetCanLearn(pk, pi, evo, move);
            if (chk != default)
                result[i] = new(chk, (byte)stage, Generation);
        }
    }

    public void GetAllMoves(Span<bool> result, PKM pk, EvolutionHistory history, IEncounterTemplate enc, MoveSourceType types = MoveSourceType.All, LearnOption option = LearnOption.Current)
    {
        if (types.HasFlag(MoveSourceType.Encounter) && enc.Context == Context)
            FlagEncounterMoves(enc, result);

        foreach (var evo in history.Gen8a)
            GetAllMoves(result, pk, evo, types, option);

        var home = LearnGroupHOME.Instance;
        if (option != LearnOption.HOME && home.HasVisited(pk, history))
            home.GetAllMoves(result, pk, history, enc, types);
    }

    private static void GetAllMoves(Span<bool> result, PKM pk, EvoCriteria evo, MoveSourceType types, LearnOption option)
    {
        if (!FormChangeUtil.ShouldIterateForms(evo.Species, evo.Form, Generation, option))
        {
            GetAllMovesInternal(result, pk, evo, types);
            return;
        }

        // Check all forms
        var inst = LearnSource8LA.Instance;
        if (!inst.TryGetPersonal(evo.Species, evo.Form, out var pi))
            return;

        var fc = pi.FormCount;
        for (int i = 0; i < fc; i++)
            GetAllMovesInternal(result, pk, evo with { Form = (byte)i }, types);
    }

    private static void GetAllMovesInternal(Span<bool> result, PKM pk, EvoCriteria evo, MoveSourceType types)
    {
        LearnSource8LA.Instance.GetAllMoves(result, pk, evo, types);
    }

    private static void FlagEncounterMoves(IEncounterTemplate enc, Span<bool> result)
    {
        if (enc is IMoveset { Moves: { HasMoves: true } x })
        {
            result[x.Move4] = true;
            result[x.Move3] = true;
            result[x.Move2] = true;
            result[x.Move1] = true;
        }
        if (enc is IRelearn { Relearn: { HasMoves: true } r })
        {
            result[r.Move4] = true;
            result[r.Move3] = true;
            result[r.Move2] = true;
            result[r.Move1] = true;
        }
    }
}
