using System;

namespace PKHeX.Core;

/// <summary>
/// Group that checks the source of a move in <see cref="GameVersion.Gen2"/>.
/// </summary>
public sealed class LearnGroup2 : ILearnGroup
{
    public static readonly LearnGroup2 Instance = new();
    private const int Generation = 2;

    public ILearnGroup? GetPrevious(PKM pk, EvolutionHistory history, IEncounterTemplate enc, LearnOption option) => pk.Context switch
    {
        EntityContext.Gen2 when enc.Generation == 1 => LearnGroup1.Instance,
        EntityContext.Gen1 => null,
        _ =>  enc.Generation != 1 && !pk.Korean && history.HasVisitedGen1 ? LearnGroup1.Instance : null,
    };

    public bool HasVisited(PKM pk, EvolutionHistory history) => history.HasVisitedGen2;

    public bool Check(Span<MoveResult> result, ReadOnlySpan<int> current, PKM pk, EvolutionHistory history, IEncounterTemplate enc,
        MoveSourceType types = MoveSourceType.All, LearnOption option = LearnOption.Current)
    {
        if (enc.Generation == Generation && types.HasFlagFast(MoveSourceType.Encounter))
            CheckEncounterMoves(result, current, enc);

        var evos = history.Gen2;
        for (var i = 0; i < evos.Length; i++)
            Check(result, current, pk, evos[i], i, option, types);

        if (enc is EncounterEgg { Generation: Generation } egg)
            CheckEncounterMoves(result, current, egg);

        return MoveResult.AllParsed(result);
    }

    private static void CheckEncounterMoves(Span<MoveResult> result, ReadOnlySpan<int> current, IEncounterTemplate enc)
    {
        Span<int> moves = stackalloc int[4];
        if (enc is IMoveset { Moves: int[] { Length: not 0 } x })
            x.CopyTo(moves);
        else
            GetEncounterMoves(enc, moves);
        LearnVerifierHistory.MarkInitialMoves(result, current, moves);
    }

    private static void GetEncounterMoves(IEncounterTemplate enc, Span<int> moves)
    {
        if (enc.Version is GameVersion.C or GameVersion.GSC)
            LearnSource2C.GetEncounterMoves(enc, moves);
        else
            LearnSource2GS.GetEncounterMoves(enc, moves);
    }

    private static void CheckEncounterMoves(Span<MoveResult> result, ReadOnlySpan<int> current, EncounterEgg egg)
    {
        ReadOnlySpan<int> eggMoves, levelMoves;
        if (egg.Version is GameVersion.C)
        {
            var inst = LearnSource2C.Instance;
            eggMoves = inst.GetEggMoves(egg.Species, egg.Form);
            levelMoves = egg.CanInheritMoves
                ? inst.GetLearnset(egg.Species, egg.Form).Moves
                : ReadOnlySpan<int>.Empty;
        }
        else
        {
            var inst = LearnSource2GS.Instance;
            eggMoves = inst.GetEggMoves(egg.Species, egg.Form);
            levelMoves = egg.CanInheritMoves
                ? inst.GetLearnset(egg.Species, egg.Form).Moves
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
        }
    }

    private static void Check(Span<MoveResult> result, ReadOnlySpan<int> current, PKM pk, EvoCriteria evo, int stage, LearnOption option = LearnOption.Current, MoveSourceType types = MoveSourceType.All)
    {
        var gs = LearnSource2GS.Instance;
        if (!gs.TryGetPersonal(evo.Species, evo.Form, out var gp))
            return; // should never happen.

        var c = LearnSource2C.Instance;
        if (!c.TryGetPersonal(evo.Species, evo.Form, out var cp))
            return; // should never happen.

        if (ParseSettings.AllowGen2MoveReminder(pk))
            evo = evo with { LevelMin = 1 };

        bool kor = pk.Korean; // Crystal is not available to Korean games.

        for (int i = result.Length - 1; i >= 0; i--)
        {
            ref var entry = ref result[i];
            if (entry.Valid && entry.Generation > 2)
                continue;

            var move = current[i];
            var chk = gs.GetCanLearn(pk, gp, evo, move, types);
            if (chk != default && GetIsPreferable(entry, chk, stage))
            {
                entry = new(chk, (byte)stage, Generation);
                continue;
            }

            if (kor)
                continue;

            chk = c.GetCanLearn(pk, cp, evo, move, types);
            if (chk != default && GetIsPreferable(entry, chk, stage))
                entry = new(chk, (byte)stage, Generation);
        }
    }

    private static bool GetIsPreferable(in MoveResult entry, in MoveLearnInfo chk, int stage)
    {
        if (entry == default)
            return true;

        if (entry.Info.Method is LearnMethod.LevelUp)
        {
            if (chk.Method is not LearnMethod.LevelUp)
                return true;
            if (entry.EvoStage == stage)
                return entry.Info.Argument < chk.Argument;
        }
        else if (entry.Info.Method.IsEggSource())
        {
            return true;
        }
        else if (chk.Method is LearnMethod.LevelUp)
        {
            return false;
        }
        return entry.EvoStage < stage;
    }

    public void GetAllMoves(Span<bool> result, PKM pk, EvolutionHistory history, IEncounterTemplate enc, MoveSourceType types = MoveSourceType.All, LearnOption option = LearnOption.Current)
    {
        if (types.HasFlagFast(MoveSourceType.Encounter) && enc.Generation == Generation)
            FlagEncounterMoves(enc, result);

        foreach (var evo in history.Gen2)
            GetAllMoves(result, pk, evo, types);
    }

    private static void GetAllMoves(Span<bool> result, PKM pk, EvoCriteria evo, MoveSourceType types)
    {
        if (ParseSettings.AllowGen2MoveReminder(pk))
            evo = evo with { LevelMin = 1 };

        LearnSource2GS.Instance.GetAllMoves(result, pk, evo, types);
        if (pk.Korean)
            return;
        LearnSource2C.Instance.GetAllMoves(result, pk, evo, types);
    }

    private static void FlagEncounterMoves(IEncounterTemplate enc, Span<bool> result)
    {
        if (enc is IMoveset { Moves: int[] { Length: not 0 } x })
        {
            foreach (var move in x)
                result[move] = true;
        }
        else
        {
            Span<int> moves = stackalloc int[4];
            GetEncounterMoves(enc, moves);
            foreach (var move in moves)
                result[move] = true;
        }
    }
}
