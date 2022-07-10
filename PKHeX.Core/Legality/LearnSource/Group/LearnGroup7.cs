using System;

namespace PKHeX.Core;

/// <summary>
/// Group that checks the source of a move in <see cref="GameVersion.Gen7"/>.
/// </summary>
public sealed class LearnGroup7 : ILearnGroup
{
    public static readonly LearnGroup7 Instance = new();
    private const int Generation = 7;

    public ILearnGroup? GetPrevious(Span<MoveResult> result, PKM pk, EvolutionHistory history, IEncounterTemplate enc) => enc.Generation switch
    {
        1 => history.Gen2.Length != 0 ? LearnGroup2.Instance : LearnGroup1.Instance,
        2 => history.Gen1.Length != 0 && !pk.Korean ? LearnGroup1.Instance : LearnGroup2.Instance,
        _ => null,
    };

    public bool HasVisited(PKM pk, EvolutionHistory history) => history.Gen7.Length != 0;

    public bool Check(Span<MoveResult> result, ReadOnlySpan<int> current, PKM pk, EvolutionHistory history, IEncounterTemplate enc, LearnOption option = LearnOption.Current)
    {
        var mode = GetCheckMode(enc, pk);
        var evos = history.Gen6;
        for (var i = 0; i < evos.Length; i++)
            Check(result, current, pk, evos[i], i, option, mode);

        if (enc is EncounterEgg { Generation: Generation } egg)
            CheckEncounterMoves(result, current, egg);

        return MoveResult.AllParsed(result);
    }

    private static void CheckEncounterMoves(Span<MoveResult> result, ReadOnlySpan<int> current, EncounterEgg egg)
    {
        ReadOnlySpan<int> eggMoves, levelMoves;
        if (egg.Version > GameVersion.Y) // OR/AS
        {
            var inst = LearnSource7USUM.Instance;
            eggMoves = inst.GetEggMoves(egg.Species, egg.Form);
            levelMoves = egg.CanInheritMoves
                ? inst.GetLearnset(egg.Species, egg.Form).GetAllMoves()
                : ReadOnlySpan<int>.Empty;
        }
        else
        {
            var inst = LearnSource7SM.Instance;
            eggMoves = inst.GetEggMoves(egg.Species, egg.Form);
            levelMoves = egg.CanInheritMoves
                ? inst.GetLearnset(egg.Species, egg.Form).GetAllMoves()
                : ReadOnlySpan<int>.Empty;
        }

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

    private static CheckMode GetCheckMode(IGeneration enc, PKM pk)
    {
        // We can check if it has visited specific sources. We won't check the games it hasn't visited.
        if (enc.Generation != Generation || !pk.IsUntraded)
            return CheckMode.Both;
        if (pk.USUM)
            return CheckMode.USUM;
        return CheckMode.SM;
    }

    private enum CheckMode
    {
        Both,
        SM,
        USUM,
    }

    private static void Check(Span<MoveResult> result, ReadOnlySpan<int> current, PKM pk, EvoCriteria evo, int stage, LearnOption option, CheckMode mode)
    {
        if (!FormChangeUtil.ShouldIterateForms(evo.Species, evo.Form, Generation, option))
        {
            CheckInternal(result, current, pk, evo, stage, option, mode);
            return;
        }

        // Check all forms
        var inst = LearnSource7USUM.Instance;
        if (!inst.TryGetPersonal(evo.Species, evo.Form, out var pi))
            return;

        var fc = pi.FormCount;
        for (int i = 0; i < fc; i++)
            CheckInternal(result, current, pk, evo with { Form = (byte)i }, stage, option, mode);
    }

    private static void CheckInternal(Span<MoveResult> result, ReadOnlySpan<int> current, PKM pk, EvoCriteria evo, int stage, LearnOption option, CheckMode mode)
    {
        // We can check if it has visited specific sources. We won't check the games it hasn't visited.
        if (mode == CheckMode.Both)
            CheckBoth(result, current, pk, evo, stage, option);
        else if (mode == CheckMode.USUM)
            CheckSingle(result, current, pk, evo, stage, LearnSource7USUM.Instance, option);
        else
            CheckSingle(result, current, pk, evo, stage, LearnSource7SM.Instance, option);
    }

    private static void CheckBoth(Span<MoveResult> result, ReadOnlySpan<int> current, PKM pk, EvoCriteria evo, int stage, LearnOption option)
    {
        var uu = LearnSource7USUM.Instance;
        var species = evo.Species;
        if (!uu.TryGetPersonal(species, evo.Form, out var ao_uu))
            return; // should never happen.

        var sm = LearnSource7SM.Instance;
        for (int i = result.Length - 1; i >= 0; i--)
        {
            if (result[i].Valid)
                continue;

            // Level Up moves are different for each game, but others (TM/Tutor) are same.
            var move = current[i];
            var chk = uu.GetCanLearn(pk, ao_uu, evo, move, option: option);
            if (chk != default)
            {
                result[i] = new(chk, (byte)stage, Generation);
                continue;
            }
            sm.GetCanLearn(pk, ao_uu, evo, move, MoveSourceType.LevelUp, option: option);
            if (chk != default)
                result[i] = new(chk, (byte)stage, Generation);
        }
    }

    private static void CheckSingle<T>(Span<MoveResult> result, ReadOnlySpan<int> current, PKM pk, EvoCriteria evo, int stage, T game, LearnOption option) where T : ILearnSource
    {
        var species = evo.Species;
        if (!game.TryGetPersonal(species, evo.Form, out var pi))
            return; // should never happen.

        for (int i = result.Length - 1; i >= 0; i--)
        {
            if (result[i].Valid)
                continue;

            var move = current[i];
            var chk = game.GetCanLearn(pk, pi, evo, move, option: option);
            if (chk != default)
                result[i] = new(chk, (byte)stage, Generation);
        }
    }
}
