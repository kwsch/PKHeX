using System;

namespace PKHeX.Core;

/// <summary>
/// Group that checks the source of a move in <see cref="GameVersion.Gen2"/>.
/// </summary>
public sealed class LearnGroup2 : ILearnGroup
{
    public static readonly LearnGroup2 Instance = new();
    private const byte Generation = 2;
    public ushort MaxMoveID => Legal.MaxMoveID_2;

    public ILearnGroup? GetPrevious(PKM pk, EvolutionHistory history, IEncounterTemplate enc, LearnOption option) => pk.Context switch
    {
        EntityContext.Gen2 when enc.Generation == 1 => LearnGroup1.Instance,
        EntityContext.Gen1 => null,
        _ =>  enc.Generation != 1 && !pk.Korean && history.HasVisitedGen1 ? LearnGroup1.Instance : null,
    };

    public bool HasVisited(PKM pk, EvolutionHistory history) => history.HasVisitedGen2;

    public bool Check(Span<MoveResult> result, ReadOnlySpan<ushort> current, PKM pk, EvolutionHistory history, IEncounterTemplate enc,
        MoveSourceType types = MoveSourceType.All, LearnOption option = LearnOption.Current)
    {
        if (enc.Generation == Generation && types.HasFlag(MoveSourceType.Encounter))
            CheckEncounterMoves(pk, result, current, enc);

        var evos = history.Gen2;
        for (var i = 0; i < evos.Length; i++)
        {
            // Disallow Evolution moves if the evo is the last in the list (encounter species).
            if (i == evos.Length - 1 && types.HasFlag(MoveSourceType.Evolve))
                types &= ~MoveSourceType.Evolve;
            Check(result, current, pk, evos[i], i, option, types);
        }

        if (enc is EncounterEgg { Generation: Generation } egg)
            CheckEncounterMoves(result, current, egg);

        bool vc1 = pk.VC1;
        if (!vc1 && MoveResult.AllParsed(result))
            return true;

        // Uh-oh, not all moves are verified yet.
        // To visit Gen1, we need to invalidate moves that can't be learned in Gen1 or re-learned in Gen2.
        for (int i = 0; i < result.Length; i++)
        {
            if (current[i] <= Legal.MaxMoveID_1)
                continue;
            var move = result[i];
            if (!move.IsParsed)
                continue;
            var method = move.Info.Method;
            if ((vc1 && move.Generation == 2) || method is LearnMethod.Initial || method.IsEggSource())
                result[i] = MoveResult.Unobtainable();
        }

        return false;
    }

    private static void CheckEncounterMoves(PKM pk, Span<MoveResult> result, ReadOnlySpan<ushort> current, IEncounterTemplate enc)
    {
        Span<ushort> moves = stackalloc ushort[4];
        if (enc is IMoveset { Moves: { HasMoves: true } x })
            x.CopyTo(moves);
        else
            GetEncounterMoves(pk, enc, moves);

        var game = enc.Version is GameVersion.C or GameVersion.GSC ? LearnEnvironment.C : LearnEnvironment.GS;
        LearnVerifierHistory.MarkInitialMoves(result, current, moves, game);
    }

    private static void GetEncounterMoves(PKM pk, IEncounterTemplate enc, Span<ushort> moves)
    {
        if (enc.Version is GameVersion.C or GameVersion.GSC)
            LearnSource2C.GetEncounterMoves(pk, enc, moves);
        else
            LearnSource2GS.GetEncounterMoves(enc, moves);
    }

    private static void CheckEncounterMoves(Span<MoveResult> result, ReadOnlySpan<ushort> current, EncounterEgg egg)
    {
        ILearnSource inst = egg.Version == GameVersion.C ? LearnSource2C.Instance : LearnSource2GS.Instance;
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
        }
    }

    private static void Check(Span<MoveResult> result, ReadOnlySpan<ushort> current, PKM pk, EvoCriteria evo, int stage, LearnOption option = LearnOption.Current, MoveSourceType types = MoveSourceType.All)
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
            if (entry is { Valid: true, Generation: > 2 })
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
        if (types.HasFlag(MoveSourceType.Encounter) && enc.Generation == Generation)
            FlagEncounterMoves(pk, enc, result);

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

    private static void FlagEncounterMoves(PKM pk, IEncounterTemplate enc, Span<bool> result)
    {
        if (enc is IMoveset { Moves: { HasMoves: true } x })
        {
            result[x.Move4] = true;
            result[x.Move3] = true;
            result[x.Move2] = true;
            result[x.Move1] = true;
        }
        else
        {
            Span<ushort> moves = stackalloc ushort[4];
            GetEncounterMoves(pk, enc, moves);
            foreach (var move in moves)
                result[move] = true;
        }
    }
}
