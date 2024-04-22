using System;

namespace PKHeX.Core;

/// <summary>
/// Group that checks the source of a move in <see cref="GameVersion.Gen1"/>.
/// </summary>
public sealed class LearnGroup1 : ILearnGroup
{
    public static readonly LearnGroup1 Instance = new();
    private const byte Generation = 1;
    public ushort MaxMoveID => Legal.MaxMoveID_1;

    public ILearnGroup? GetPrevious(PKM pk, EvolutionHistory history, IEncounterTemplate enc, LearnOption option) => pk.Context switch
    {
        EntityContext.Gen1 when enc.Generation == 1 && pk is PK1 pk1 && HasDefinitelyVisitedGen2(pk1) => LearnGroup2.Instance,
        EntityContext.Gen1 when enc.Generation == 2 => LearnGroup2.Instance,
        EntityContext.Gen2 => null,
        _ => enc.Generation == 1 ? LearnGroup2.Instance : null,
    };

    public bool HasVisited(PKM pk, EvolutionHistory history) => history.HasVisitedGen1;

    public bool Check(Span<MoveResult> result, ReadOnlySpan<ushort> current, PKM pk, EvolutionHistory history, IEncounterTemplate enc,
        MoveSourceType types = MoveSourceType.All, LearnOption option = LearnOption.Current)
    {
        if (enc.Generation == Generation && types.HasFlag(MoveSourceType.Encounter))
            CheckEncounterMoves(result, current, enc, pk);

        var evos = history.Gen1;
        for (var i = 0; i < evos.Length; i++)
            Check(result, current, pk, evos[i], i, option, types);

        if (GetPrevious(pk, history, enc, LearnOption.Current) is null)
            FlagEvolutionSlots(result, current, history);

        return MoveResult.AllParsed(result);
    }

    private static void FlagEvolutionSlots(Span<MoveResult> result, ReadOnlySpan<ushort> current, EvolutionHistory history)
    {
        for (int i = 0; i < result.Length; i++)
        {
            if (current[i] == 0)
                continue;

            var detail = result[i];
            if (!detail.Valid || detail.Generation is not (1 or 2))
                continue;

            var info = detail.Info;
            if (info.Method is LearnMethod.LevelUp)
            {
                var level = info.Argument;
                var stage = detail.EvoStage;
                var chain = detail.Generation is 1 ? history.Gen1 : history.Gen2;
                var species = chain[stage].Species;
                if (IsAnyOtherResultALowerEvolutionStageAndHigherLevel(result, i, history, level, species))
                    result[i] = MoveResult.Unobtainable();
            }
        }
    }

    private static bool IsAnyOtherResultALowerEvolutionStageAndHigherLevel(Span<MoveResult> result, int index, EvolutionHistory history, byte level, ushort species)
    {
        // Check if any other result is a lower evolution stage and higher level.
        for (int i = 0; i < result.Length; i++)
        {
            if (i == index)
                continue;
            var detail = result[i];
            if (!detail.Valid || detail.Generation is not (1 or 2))
                continue;

            (var method, _, byte level2) = detail.Info;
            if (method is not LearnMethod.LevelUp)
                continue;

            var stage = detail.EvoStage;
            var chain = detail.Generation is 1 ? history.Gen1 : history.Gen2;
            var species2 = chain[stage].Species;
            if (level2 > level && species2 < species)
                return true;
        }

        return false;
    }

    private static void FlagFishyMoveSlots(Span<MoveResult> result, ReadOnlySpan<ushort> current, IEncounterTemplate enc)
    {
        if (!current.Contains<ushort>(0))
            return;

        Span<ushort> moves = stackalloc ushort[4];
        if (enc is IMoveset m)
            m.Moves.CopyTo(moves);
        else
            GetEncounterMoves(enc, moves);

        // Count the amount of initial moves not present in the current list.
        int count = CountMissing(current, moves);
        if (count == 0)
            return;

        // There are ways to skip level up moves by leveling up more than once.
        // Evolving at targeted levels can evade learning moves too.
        // https://bulbapedia.bulbagarden.net/wiki/List_of_glitches_(Generation_I)#Level-up_learnset_skipping
        // Just flag missing initial move slots.
        int ctr = count;
        for (int i = 0; i < result.Length; i++)
        {
            if (current[i] != 0)
                continue;
            result[i] = MoveResult.EmptyInvalid;
            if (--ctr == 0)
                break;
        }
    }

    private static int CountMissing(ReadOnlySpan<ushort> current, ReadOnlySpan<ushort> moves)
    {
        int count = 0;
        foreach (var expect in moves)
        {
            if (expect == 0)
                break;
            if (!current.Contains(expect))
                count++;
        }
        return count;
    }

    private static void CheckEncounterMoves(Span<MoveResult> result, ReadOnlySpan<ushort> current, IEncounterTemplate enc, PKM pk)
    {
        Span<ushort> moves = stackalloc ushort[4];
        if (enc is IMoveset {Moves: {HasMoves: true} x})
            x.CopyTo(moves);
        else
            GetEncounterMoves(enc, moves);
        LearnVerifierHistory.MarkInitialMoves(result, current, moves, enc.Version == GameVersion.YW ? LearnEnvironment.YW : LearnEnvironment.RB);

        // Flag empty slots if never visited Gen2 move deleter.
        if (pk is not PK1 pk1)
            return;
        if (HasDefinitelyVisitedGen2(pk1))
            return;
        FlagFishyMoveSlots(result, current, enc);
    }

    private static bool HasDefinitelyVisitedGen2(PK1 pk1)
    {
        if (!ParseSettings.AllowGen1Tradeback)
            return false;
        var rate = pk1.CatchRate;
        return rate is 0 || GBRestrictions.IsTradebackCatchRate(rate);
    }

    private static void GetEncounterMoves(IEncounterTemplate enc, Span<ushort> moves)
    {
        ILearnSource ls = enc.Version is GameVersion.YW or GameVersion.RBY
            ? LearnSource1YW.Instance
            : LearnSource1RB.Instance;
        ls.SetEncounterMoves(enc.Species, 0, enc.LevelMin, moves);
    }

    private static void Check(Span<MoveResult> result, ReadOnlySpan<ushort> current, PKM pk, EvoCriteria evo, int stage, LearnOption option = LearnOption.Current, MoveSourceType types = MoveSourceType.All)
    {
        var rb = LearnSource1RB.Instance;
        if (!rb.TryGetPersonal(evo.Species, evo.Form, out var rp))
            return; // should never happen.

        var yw = LearnSource1YW.Instance;
        if (!yw.TryGetPersonal(evo.Species, evo.Form, out var yp))
            return; // should never happen.

        if (ParseSettings.AllowGen1Tradeback && ParseSettings.AllowGen2MoveReminder(pk))
            evo = evo with { LevelMin = 1 };

        for (int i = result.Length - 1; i >= 0; i--)
        {
            ref var entry = ref result[i];
            if (entry is { Valid: true, Generation: > 2 })
                continue;

            var move = current[i];
            var chk = yw.GetCanLearn(pk, yp, evo, move, types);
            if (chk != default && GetIsPreferable(entry, chk, stage))
            {
                entry = new(chk, (byte)stage, Generation);
                continue;
            }
            chk = rb.GetCanLearn(pk, rp, evo, move, types);
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
            FlagEncounterMoves(enc, result);

        foreach (var evo in history.Gen1)
            GetAllMoves(result, pk, evo, types);
    }

    private static void GetAllMoves(Span<bool> result, PKM pk, EvoCriteria evo, MoveSourceType types)
    {
        if (ParseSettings.AllowGen1Tradeback && ParseSettings.AllowGen2MoveReminder(pk))
            evo = evo with { LevelMin = 1 };

        LearnSource1YW.Instance.GetAllMoves(result, pk, evo, types);
        LearnSource1RB.Instance.GetAllMoves(result, pk, evo, types);
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
        else
        {
            Span<ushort> moves = stackalloc ushort[4];
            GetEncounterMoves(enc, moves);
            foreach (var move in moves)
                result[move] = true;
        }
    }
}
