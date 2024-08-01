using System;

namespace PKHeX.Core;

/// <summary>
/// Group that checks the source of a move in <see cref="GameVersion.Gen6"/>.
/// </summary>
public sealed class LearnGroup6 : ILearnGroup
{
    public static readonly LearnGroup6 Instance = new();
    private const byte Generation = 6;
    public ushort MaxMoveID => Legal.MaxMoveID_6_AO;

    public ILearnGroup? GetPrevious(PKM pk, EvolutionHistory history, IEncounterTemplate enc, LearnOption option) => enc.Generation is Generation ? null : LearnGroup5.Instance;
    public bool HasVisited(PKM pk, EvolutionHistory history) => history.HasVisitedGen6;

    public bool Check(Span<MoveResult> result, ReadOnlySpan<ushort> current, PKM pk, EvolutionHistory history, IEncounterTemplate enc,
        MoveSourceType types = MoveSourceType.All, LearnOption option = LearnOption.Current)
    {
        var mode = GetCheckMode(enc, pk);
        var evos = history.Gen6;
        for (var i = 0; i < evos.Length; i++)
            Check(result, current, pk, evos[i], i, types, option, mode);

        if (option.IsPast() && types.HasFlag(MoveSourceType.Encounter))
        {
            if (enc is EncounterEgg { Generation: Generation } egg)
                CheckEncounterMoves(result, current, egg);
            else if (enc is EncounterSlot6AO { CanDexNav: true } dexnav && pk.IsOriginalMovesetDeleted())
                CheckDexNavMoves(result, current, dexnav);
        }

        return MoveResult.AllParsed(result);
    }

    private static void CheckEncounterMoves(Span<MoveResult> result, ReadOnlySpan<ushort> current, EncounterEgg egg)
    {
        ILearnSource inst = egg.Version > GameVersion.Y ? LearnSource6AO.Instance : LearnSource6XY.Instance;
        var eggMoves = inst.GetEggMoves(egg.Species, egg.Form);
        var levelMoves = inst.GetInheritMoves(egg.Species, egg.Form);

        for (var i = result.Length - 1; i >= 0; i--)
        {
            if (result[i].Valid)
                continue;
            var move = current[i];
            if (eggMoves.Contains(move))
                result[i] = new(LearnMethod.EggMove, inst.Environment);
            else if (levelMoves.Contains(move))
                result[i] = new(LearnMethod.InheritLevelUp, inst.Environment);
            else if (move is (int)Move.VoltTackle && egg.CanHaveVoltTackle)
                result[i] = new(LearnMethod.SpecialEgg, inst.Environment);
        }
    }

    private static void CheckDexNavMoves(Span<MoveResult> result, ReadOnlySpan<ushort> current, EncounterSlot6AO dexnav)
    {
        // DexNav moves are only available in OR/AS
        // Can only have one DexNav move
        for (var i = 0; i < result.Length; i++)
        {
            if (result[i].Valid)
                continue;
            var move = current[i];
            if (!dexnav.CanBeDexNavMove(move))
                continue;
            result[i] = new(new(LearnMethod.Encounter, LearnEnvironment.ORAS), Generation: Generation);
            break;
        }
    }

    private static void Check(Span<MoveResult> result, ReadOnlySpan<ushort> current, PKM pk, EvoCriteria evo, int stage, MoveSourceType types, LearnOption option, CheckMode mode)
    {
        if (!FormChangeUtil.ShouldIterateForms(evo.Species, evo.Form, Generation, option))
        {
            CheckInternal(result, current, pk, evo, stage, types, option, mode);
            return;
        }

        // Check all forms
        var inst = LearnSource6AO.Instance;
        if (!inst.TryGetPersonal(evo.Species, evo.Form, out var pi))
            return;

        var fc = pi.FormCount;
        for (int i = 0; i < fc; i++)
            CheckInternal(result, current, pk, evo with { Form = (byte)i }, stage, types, option, mode);
    }

    private static CheckMode GetCheckMode(IEncounterTemplate enc, PKM pk)
    {
        // We can check if it has visited specific sources. We won't check the games it hasn't visited.
        if (enc.Context != EntityContext.Gen6 || !pk.IsUntraded)
            return CheckMode.Both;
        if (pk.AO)
            return CheckMode.AO;
        return CheckMode.XY;
    }

    private enum CheckMode
    {
        Both,
        XY,
        AO,
    }

    private static void CheckInternal(Span<MoveResult> result, ReadOnlySpan<ushort> current, PKM pk, EvoCriteria evo, int stage, MoveSourceType types, LearnOption option, CheckMode mode)
    {
        if (mode == CheckMode.Both)
            CheckBoth(result, current, pk, evo, stage, types, option);
        else if (mode == CheckMode.AO)
            CheckSingle<ILearnSource<PersonalInfo6AO>, PersonalInfo6AO>(result, current, pk, evo, stage, LearnSource6AO.Instance, types, option);
        else
            CheckSingle<ILearnSource<PersonalInfo6XY>, PersonalInfo6XY>(result, current, pk, evo, stage, LearnSource6XY.Instance, types, option);
    }

    private static void CheckBoth(Span<MoveResult> result, ReadOnlySpan<ushort> current, PKM pk, EvoCriteria evo, int stage, MoveSourceType types, LearnOption option)
    {
        var ao = LearnSource6AO.Instance;
        var species = evo.Species;
        if (!ao.TryGetPersonal(species, evo.Form, out var ao_pi))
            return; // should never happen.

        var xy = LearnSource6XY.Instance;
        xy.TryGetPersonal(species, evo.Form, out var xy_pi);

        for (int i = result.Length - 1; i >= 0; i--)
        {
            if (result[i].Valid)
                continue;

            // Level Up moves are different for each game, but others (TM/Tutor) are same.
            if (xy_pi is null)
                continue;

            var move = current[i];
            var chk = ao.GetCanLearn(pk, ao_pi, evo, move, types, option);
            if (chk != default)
            {
                result[i] = new(chk, (byte)stage, Generation);
                continue;
            }

            chk = xy.GetCanLearn(pk, xy_pi, evo, move, types & MoveSourceType.LevelUp, option);
            if (chk != default)
                result[i] = new(chk, (byte)stage, Generation);
        }
    }

    private static void CheckSingle<TSource, TPersonal>(Span<MoveResult> result, ReadOnlySpan<ushort> current, PKM pk, EvoCriteria evo, int stage, TSource game, MoveSourceType types, LearnOption option)
        where TSource : ILearnSource<TPersonal>
        where TPersonal : PersonalInfo
    {
        var species = evo.Species;
        if (!game.TryGetPersonal(species, evo.Form, out var pi))
            return; // should never happen.

        for (int i = result.Length - 1; i >= 0; i--)
        {
            if (result[i].Valid)
                continue;

            var move = current[i];
            var chk = game.GetCanLearn(pk, pi, evo, move, types, option);
            if (chk != default)
                result[i] = new(chk, (byte)stage, Generation);
        }
    }

    public void GetAllMoves(Span<bool> result, PKM pk, EvolutionHistory history, IEncounterTemplate enc, MoveSourceType types = MoveSourceType.All, LearnOption option = LearnOption.Current)
    {
        if (types.HasFlag(MoveSourceType.Encounter) && enc.Generation == Generation)
            FlagEncounterMoves(enc, result);

        var mode = GetCheckMode(enc, pk);
        foreach (var evo in history.Gen6)
            GetAllMoves(result, pk, evo, types, option, mode);
    }

    private static void GetAllMoves(Span<bool> result, PKM pk, EvoCriteria evo, MoveSourceType types, LearnOption option, CheckMode mode)
    {
        if (!FormChangeUtil.ShouldIterateForms(evo.Species, evo.Form, Generation, option))
        {
            GetAllMovesInternal(result, pk, evo, types, mode);
            return;
        }

        // Check all forms
        var inst = LearnSource6AO.Instance;
        if (!inst.TryGetPersonal(evo.Species, evo.Form, out var pi))
            return;

        var fc = pi.FormCount;
        for (int i = 0; i < fc; i++)
            GetAllMovesInternal(result, pk, evo with { Form = (byte)i }, types, mode);
    }

    private static void GetAllMovesInternal(Span<bool> result, PKM pk, EvoCriteria evo, MoveSourceType types, CheckMode mode)
    {
        if (mode is CheckMode.Both)
            GetAllMovesBoth(result, pk, evo, types);
        else if (mode is CheckMode.AO)
            GetAllMovesSingle(result, pk, evo, LearnSource6AO.Instance, types);
        else
            GetAllMovesSingle(result, pk, evo, LearnSource6XY.Instance, types);
    }

    private static void GetAllMovesSingle<T>(Span<bool> result, PKM pk, EvoCriteria evo, T instance, MoveSourceType types) where T : ILearnSource
    {
        instance.GetAllMoves(result, pk, evo, types);
    }

    private static void GetAllMovesBoth(Span<bool> result, PKM pk, EvoCriteria evo, MoveSourceType types)
    {
        LearnSource6AO.Instance.GetAllMoves(result, pk, evo, types);
        LearnSource6XY.Instance.GetAllMoves(result, pk, evo, types & MoveSourceType.LevelUp);
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
        if (enc is IRelearn { Relearn: { HasMoves: true } r})
        {
            result[r.Move4] = true;
            result[r.Move3] = true;
            result[r.Move2] = true;
            result[r.Move1] = true;
        }
    }
}
