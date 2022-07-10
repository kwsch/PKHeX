using System;

namespace PKHeX.Core;

/// <summary>
/// Group that checks the source of a move in <see cref="GameVersion.Gen8"/>.
/// </summary>
public sealed class LearnGroup8 : ILearnGroup
{
    public static readonly LearnGroup8 Instance = new();
    private const int Generation = 8;

    public ILearnGroup? GetPrevious(Span<MoveResult> result, PKM pk, EvolutionHistory history, IEncounterTemplate enc)
    {
        if (enc.Generation >= Generation)
            return null;
        if (pk.IsOriginalMovesetDeleted())
            return null;
        return LearnGroup7.Instance;
    }

    public bool HasVisited(PKM pk, EvolutionHistory history) => history.Gen8.Length != 0;

    public bool Check(Span<MoveResult> result, ReadOnlySpan<int> current, PKM pk, EvolutionHistory history, IEncounterTemplate enc, LearnOption option = LearnOption.Current)
    {
        var evos = history.Gen8;
        for (var i = 0; i < evos.Length; i++)
            Check(result, current, pk, evos[i], i, option: option);

        if (enc is EncounterStatic8N r && r.IsDownLeveled(pk))
            Check(result, current, pk, evos[^1] with { LevelMax = r.LevelMax, LevelMin = evos[^1].LevelMax }, evos.Length - 1, MoveSourceType.LevelUp);

        if (enc is EncounterEgg { Generation: Generation } egg)
            CheckEncounterMoves(result, current, egg);
        else // Check for shared egg moves.
            CheckSharedMoves(result, current, pk);

        return MoveResult.AllParsed(result);
    }

    private static void CheckSharedMoves(Span<MoveResult> result, ReadOnlySpan<int> current, PKM pk)
    {
        var game = LearnSource8SWSH.Instance;
        var entry = (PersonalInfoSWSH)pk.PersonalInfo;
        var baseSpecies = entry.HatchSpecies;
        var baseForm = entry.HatchFormIndexEverstone;
        var eggMoves = game.GetEggMoves(baseSpecies, baseForm);

        for (var i = result.Length - 1; i >= 0; i--)
        {
            if (result[i].Valid)
                continue;
            var move = current[i];
            if (eggMoves.Contains(move))
                result[i] = new(LearnMethod.Shared);
        }
    }

    private static void CheckEncounterMoves(Span<MoveResult> result, ReadOnlySpan<int> current, EncounterEgg egg)
    {
        var game = LearnSource8SWSH.Instance;
        ReadOnlySpan<int> eggMoves = game.GetEggMoves(egg.Species, egg.Form);
        ReadOnlySpan<int> levelMoves = egg.CanInheritMoves
            ? game.GetLearnset(egg.Species, egg.Form).GetAllMoves()
            : ReadOnlySpan<int>.Empty;

        for (var i = result.Length - 1; i >= 0; i--)
        {
            if (result[i].Valid)
                continue;
            var move = current[i];
            if (eggMoves.Contains(move))
                result[i] = new(LearnMethod.EggMove);
            else if (levelMoves.Contains(move))
                result[i] = new(LearnMethod.InheritLevelUp);
            else if (move is (int)Move.VoltTackle && egg.CanHaveVoltTackle)
                result[i] = new(LearnMethod.SpecialEgg);
        }
    }

    private static void Check(Span<MoveResult> result, ReadOnlySpan<int> current, PKM pk, EvoCriteria evo, int stage, MoveSourceType type = MoveSourceType.All, LearnOption option = LearnOption.Current)
    {
        if (!FormChangeUtil.ShouldIterateForms(evo.Species, evo.Form, Generation, option))
        {
            CheckInternal(result, current, pk, evo, stage, type, option);
            return;
        }

        // Check all forms
        var inst = LearnSource8SWSH.Instance;
        if (!inst.TryGetPersonal(evo.Species, evo.Form, out var pi))
            return;

        var fc = pi.FormCount;
        for (int i = 0; i < fc; i++)
            CheckInternal(result, current, pk, evo with { Form = (byte)i }, stage, type, option);
    }

    private static void CheckInternal(Span<MoveResult> result, ReadOnlySpan<int> current, PKM pk, EvoCriteria evo, int stage, MoveSourceType type, LearnOption option)
    {
        var game = LearnSource8SWSH.Instance;
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
}
