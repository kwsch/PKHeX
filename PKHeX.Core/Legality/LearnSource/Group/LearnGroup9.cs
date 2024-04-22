using System;

namespace PKHeX.Core;

/// <summary>
/// Group that checks the source of a move in <see cref="GameVersion.Gen9"/>.
/// </summary>
public sealed class LearnGroup9 : ILearnGroup
{
    public static readonly LearnGroup9 Instance = new();
    private const byte Generation = 9;
    private const EntityContext Context = EntityContext.Gen9;
    public ushort MaxMoveID => Legal.MaxMoveID_9;

    public ILearnGroup? GetPrevious(PKM pk, EvolutionHistory history, IEncounterTemplate enc, LearnOption option) => null;
    public bool HasVisited(PKM pk, EvolutionHistory history) => history.HasVisitedGen9;

    public bool Check(Span<MoveResult> result, ReadOnlySpan<ushort> current, PKM pk, EvolutionHistory history,
        IEncounterTemplate enc, MoveSourceType types = MoveSourceType.All, LearnOption option = LearnOption.Current)
    {
        var evos = history.Gen9;
        if (evos.Length == 0)
            return false;

        for (var i = 0; i < evos.Length; i++)
            Check(result, current, pk, evos[i], i, types, option);

        CheckSharedMoves(result, current, evos[0]);

        if (option.IsPast() && types.HasFlag(MoveSourceType.Encounter) && pk.IsOriginalMovesetDeleted() && enc is EncounterEgg { Generation: Generation } egg)
            CheckEncounterMoves(result, current, egg);

        if (MoveResult.AllParsed(result))
            return true;

        var home = LearnGroupHOME.Instance;
        if (option != LearnOption.HOME && home.HasVisited(pk, history))
            return home.Check(result, current, pk, history, enc, types);
        return false;
    }

    private static void CheckSharedMoves(Span<MoveResult> result, ReadOnlySpan<ushort> current, EvoCriteria evo)
    {
        var game = LearnSource9SV.Instance;
        var entry = PersonalTable.SV.GetFormEntry(evo.Species, evo.Form);
        var baseSpecies = entry.HatchSpecies;
        var baseForm = entry.HatchFormIndexEverstone;
        var eggMoves = game.GetEggMoves(baseSpecies, baseForm);

        for (var i = result.Length - 1; i >= 0; i--)
        {
            if (result[i].Valid)
                continue;
            var move = current[i];
            if (eggMoves.Contains(move))
                result[i] = new(LearnMethod.Shared, game.Environment);
        }
    }

    private static void CheckEncounterMoves(Span<MoveResult> result, ReadOnlySpan<ushort> current, EncounterEgg egg)
    {
        var game = LearnSource9SV.Instance;
        var eggMoves = game.GetEggMoves(egg.Species, egg.Form);
        var levelMoves = ((ILearnSource)game).GetInheritMoves(egg.Species, egg.Form);

        for (var i = result.Length - 1; i >= 0; i--)
        {
            if (result[i].Valid)
                continue;
            var move = current[i];
            if (eggMoves.Contains(move))
                result[i] = new(LearnMethod.EggMove, game.Environment);
            else if (levelMoves.Contains(move))
                result[i] = new(LearnMethod.InheritLevelUp, game.Environment);
            else if (move is (int)Move.VoltTackle && egg.CanHaveVoltTackle)
                result[i] = new(LearnMethod.SpecialEgg, game.Environment);
        }
    }

    private static void Check(Span<MoveResult> result, ReadOnlySpan<ushort> current, PKM pk, EvoCriteria evo, int stage, MoveSourceType type = MoveSourceType.All, LearnOption option = LearnOption.Current)
    {
        if (!FormChangeUtil.ShouldIterateForms(evo.Species, evo.Form, Generation, option))
        {
            CheckInternal(result, current, pk, evo, stage, type, option);
            return;
        }

        // Check all forms
        var inst = LearnSource9SV.Instance;
        if (!inst.TryGetPersonal(evo.Species, evo.Form, out var pi))
            return;

        var fc = pi.FormCount;
        for (int i = 0; i < fc; i++)
            CheckInternal(result, current, pk, evo with { Form = (byte)i }, stage, type, option);
    }

    private static void CheckInternal(Span<MoveResult> result, ReadOnlySpan<ushort> current, PKM pk, EvoCriteria evo, int stage, MoveSourceType type, LearnOption option)
    {
        var game = LearnSource9SV.Instance;
        if (!game.TryGetPersonal(evo.Species, evo.Form, out var pi))
            return;

        for (int i = result.Length - 1; i >= 0; i--)
        {
            if (result[i].Valid)
                continue;

            var move = current[i];
            var chk = game.GetCanLearn(pk, pi, evo, move, type, option);
            if (chk != default)
                result[i] = new(chk, (byte)stage, Generation);
        }
    }

    public void GetAllMoves(Span<bool> result, PKM pk, EvolutionHistory history, IEncounterTemplate enc, MoveSourceType types = MoveSourceType.All, LearnOption option = LearnOption.Current)
    {
        var evos = history.Gen9;
        if (types.HasFlag(MoveSourceType.Encounter) && enc.Context == Context)
            FlagEncounterMoves(enc, result);

        foreach (var evo in evos)
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
        var inst = LearnSource9SV.Instance;
        if (!inst.TryGetPersonal(evo.Species, evo.Form, out var pi))
            return;

        var fc = pi.FormCount;
        for (int i = 0; i < fc; i++)
            GetAllMovesInternal(result, pk, evo with { Form = (byte)i }, types);
    }

    private static void GetAllMovesInternal(Span<bool> result, PKM pk, EvoCriteria evo, MoveSourceType types)
    {
        LearnSource9SV.Instance.GetAllMoves(result, pk, evo, types);
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
