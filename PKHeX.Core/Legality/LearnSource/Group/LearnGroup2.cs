using System;

namespace PKHeX.Core;

/// <summary>
/// Group that checks the source of a move in <see cref="GameVersion.Gen2"/>.
/// </summary>
public sealed class LearnGroup2 : ILearnGroup
{
    public static readonly LearnGroup2 Instance = new();
    private const int Generation = 2;

    public ILearnGroup? GetPrevious(Span<MoveResult> result, PKM pk, EvolutionHistory history, IEncounterTemplate enc) => pk.Context switch
    {
        EntityContext.Gen1 => null,
        _ =>  enc.Generation == 2 && !pk.Korean && history.Gen1.Length != 0 ? LearnGroup1.Instance : null,
    };

    public bool HasVisited(PKM pk, EvolutionHistory history) => history.Gen2.Length != 0;

    public bool Check(Span<MoveResult> result, ReadOnlySpan<int> current, PKM pk, EvolutionHistory history, IEncounterTemplate enc, LearnOption option = LearnOption.Current)
    {
        var evos = history.Gen2;
        for (var i = 0; i < evos.Length; i++)
            Check(result, current, pk, evos[i], i);

        if (enc is EncounterEgg { Generation: Generation } egg)
            CheckEncounterMoves(result, current, egg);

        return MoveResult.AllParsed(result);
    }

    private static void CheckEncounterMoves(Span<MoveResult> result, ReadOnlySpan<int> current, EncounterEgg egg)
    {
        ReadOnlySpan<int> eggMoves, levelMoves;
        if (egg.Version is GameVersion.C)
        {
            var inst = LearnSource2C.Instance;
            eggMoves = inst.GetEggMoves(egg.Species, egg.Form);
            levelMoves = egg.CanInheritMoves
                ? inst.GetLearnset(egg.Species, egg.Form).GetAllMoves()
                : ReadOnlySpan<int>.Empty;
        }
        else
        {
            var inst = LearnSource2GS.Instance;
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
        }
    }

    private static void Check(Span<MoveResult> result, ReadOnlySpan<int> current, PKM pk, EvoCriteria evo, int stage)
    {
        var gs = LearnSource2GS.Instance;
        if (!gs.TryGetPersonal(evo.Species, evo.Form, out var gp))
            return; // should never happen.

        var c = LearnSource2C.Instance;
        if (!c.TryGetPersonal(evo.Species, evo.Form, out var cp))
            return; // should never happen.

        bool kor = pk.Korean; // Crystal is not available to Korean games.

        for (int i = result.Length - 1; i >= 0; i--)
        {
            ref var entry = ref result[i];
            if (entry.Valid && entry.Generation > 2)
                continue;

            var move = current[i];
            var chk = gs.GetCanLearn(pk, gp, evo, move);
            if (chk != default && GetIsPreferable(entry, chk, stage))
            {
                entry = new(chk, (byte)stage, Generation);
                continue;
            }

            if (kor)
                continue;

            chk = c.GetCanLearn(pk, cp, evo, move);
            if (chk != default && GetIsPreferable(entry, chk, stage))
                entry = new(chk, (byte)stage, Generation);
        }
    }

    private static bool GetIsPreferable(in MoveResult entry, in MoveLearnInfo chk, int stage)
    {
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
}
