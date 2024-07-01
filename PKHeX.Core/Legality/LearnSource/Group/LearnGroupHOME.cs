using System;
using System.Buffers;

namespace PKHeX.Core;

/// <summary>
/// Encapsulates logic for HOME's Move Relearner feature.
/// </summary>
/// <remarks>
/// If the Entity knew a move at any point in its history, it can be relearned if the current format can learn it.
/// </remarks>
public sealed class LearnGroupHOME : ILearnGroup
{
    public static readonly LearnGroupHOME Instance = new();
    public ushort MaxMoveID => 0;

    public ILearnGroup? GetPrevious(PKM pk, EvolutionHistory history, IEncounterTemplate enc, LearnOption option) => null;
    public bool HasVisited(PKM pk, EvolutionHistory history) => pk is IHomeTrack { HasTracker: true } || !ParseSettings.IgnoreTransferIfNoTracker;

    public bool Check(Span<MoveResult> result, ReadOnlySpan<ushort> current, PKM pk, EvolutionHistory history,
        IEncounterTemplate enc, MoveSourceType types = MoveSourceType.All, LearnOption option = LearnOption.HOME)
    {
        var context = pk.Context;
        if (context == EntityContext.None)
            return false;

        var local = GetCurrent(context);
        var evos = history.Get(context);
        if (history.HasVisitedGen9 && pk is not PK9)
        {
            var instance = LearnGroup9.Instance;
            instance.Check(result, current, pk, history, enc, types, option);
            if (CleanPurge(result, current, pk, types, local, evos))
                return true;
        }
        if (history.HasVisitedSWSH && pk is not PK8)
        {
            var instance = LearnGroup8.Instance;
            instance.Check(result, current, pk, history, enc, types, option);
            if (CleanPurge(result, current, pk, types, local, evos))
                return true;
        }
        if (history.HasVisitedPLA && pk is not PA8)
        {
            var instance = LearnGroup8a.Instance;
            instance.Check(result, current, pk, history, enc, types, option);
            if (CleanPurge(result, current, pk, types, local, evos))
                return true;
        }
        if (history.HasVisitedBDSP && pk is not PB8)
        {
            var instance = LearnGroup8b.Instance;
            instance.Check(result, current, pk, history, enc, types, option);
            if (CleanPurge(result, current, pk, types, local, evos))
                return true;
        }

        // Ignore Battle Version generally; can be transferred back to SW/SH and wiped after the moves have been shared from HOME.
        // Battle Version is only relevant while in PK8 format, as a wiped moveset can no longer harbor external moves for that format.
        // SW/SH is the only game that can ever harbor external moves, and is the only game that uses Battle Version.
        if (TryAddOriginalMoves(result, current, pk, enc))
        {
            if (CleanPurge(result, current, pk, types, local, evos))
                return true;
        }

        // HOME is silly and allows form exclusive moves to be transferred without ever knowing the move.
        if (TryAddExclusiveMoves(result, current, pk))
        {
            if (CleanPurge(result, current, pk, types, local, evos))
                return true;
        }

        if (history.HasVisitedLGPE)
        {
            // PK8 w/ Battle Version can be ignored, as LGP/E has separate HOME data.
            var instance = LearnGroup7b.Instance;
            instance.Check(result, current, pk, history, enc, types, option);
            if (CleanPurge(result, current, pk, types, local, evos))
                return true;
        }
        else if (history.HasVisitedGen7)
        {
            if (IsWipedPK8(pk))
                return false; // Battle Version wiped Gen7 and below moves.
            ILearnGroup instance = LearnGroup7.Instance;
            while (true)
            {
                instance.Check(result, current, pk, history, enc, types, option);
                if (CleanPurge(result, current, pk, types, local, evos))
                    return true;
                var prev = instance.GetPrevious(pk, history, enc, option);
                if (prev is null)
                    break;
                instance = prev;
            }
        }
        return false;
    }

    private static bool IsWipedPK8(PKM pk) => pk is PK8 { BattleVersion: GameVersion.SW or GameVersion.SH };

    /// <summary>
    /// Scan the results and remove any that are not valid for the game <see cref="local"/> game.
    /// </summary>
    /// <returns>True if all results are valid.</returns>
    private static bool CleanPurge(Span<MoveResult> result, ReadOnlySpan<ushort> current, PKM pk, MoveSourceType types, IHomeSource local, ReadOnlySpan<EvoCriteria> evos)
    {
        // The logic used to update the results did not check if the move could be learned in the local game.
        // Double-check the results and remove any that are not valid for the local game.
        // SW/SH will continue to iterate downwards to previous groups after HOME is checked, so we can exactly check via Environment.
        for (int i = 0; i < result.Length; i++)
        {
            ref var r = ref result[i];
            if (!r.Valid || r.Generation == 0)
                continue;

            if (r.Info.Environment == local.Environment)
                continue;

            // Check if any evolution in the local context can learn the move via HOME instruction. If none can, the move is invalid.
            var move = current[i];
            if (move == 0)
                continue;

            bool valid = false;
            foreach (var evo in evos)
            {
                var chk = local.GetCanLearnHOME(pk, evo, move, types);
                if (chk.Method != LearnMethod.None)
                    valid = true;
            }

            // HOME has special handling to allow Volt Tackle outside learnset possibilities.
            // Most games do not have a Learn Source for Volt Tackle besides it being specially inserted for Egg Encounters.
            if (!valid && move is not (ushort)Move.VoltTackle)
            {
                if (r.Generation >= 8 || local is not LearnSource8SWSH)
                    r = default;
            }
        }

        return MoveResult.AllParsed(result);
    }

    public void GetAllMoves(Span<bool> result, PKM pk, EvolutionHistory history, IEncounterTemplate enc,
        MoveSourceType types = MoveSourceType.All, LearnOption option = LearnOption.HOME)
    {
        option = LearnOption.HOME;
        var local = GetCurrent(pk.Context);
        var evos = history.Get(pk.Context);

        // Check all adjacent games
        if (history.HasVisitedGen9 && pk is not PK9)
            RentLoopGetAll(LearnGroup9. Instance, result, pk, history, enc, types, option, evos, local);
        if (history.HasVisitedSWSH && pk is not PK8)
            RentLoopGetAll(LearnGroup8. Instance, result, pk, history, enc, types, option, evos, local);
        if (history.HasVisitedPLA && pk is not PA8)
            RentLoopGetAll(LearnGroup8a.Instance, result, pk, history, enc, types, option, evos, local);
        if (history.HasVisitedBDSP && pk is not PB8)
            RentLoopGetAll(LearnGroup8b.Instance, result, pk, history, enc, types, option, evos, local);
        AddOriginalMoves(result, pk, enc, types, local, evos);
        AddExclusiveMoves(result, pk);

        // Looking backwards before HOME
        if (history.HasVisitedLGPE)
        {
            RentLoopGetAll(LearnGroup7b.Instance, result, pk, history, enc, types, option, evos, local);
        }
        else if (history.HasVisitedGen7)
        {
            ILearnGroup instance = LearnGroup7.Instance;
            while (true)
            {
                RentLoopGetAll(instance, result, pk, history, enc, types, option, evos, local);
                var prev = instance.GetPrevious(pk, history, enc, option);
                if (prev is null)
                    break;
                instance = prev;
            }
        }
    }

    private static bool TryAddOriginalMoves(Span<MoveResult> result, ReadOnlySpan<ushort> current, PKM pk, IEncounterTemplate enc)
    {
        if (enc is IMoveset { Moves: { HasMoves: true } x })
        {
            if (enc is { Generation: <= 7, Context: not EntityContext.Gen7b } && IsWipedPK8(pk))
                return false; // Battle Version wiped Gen7 and below moves.
            Span<ushort> moves = stackalloc ushort[4];
            x.CopyTo(moves);
            var ls = GameData.GetLearnSource(enc.Version);
            return AddOriginalMoves(result, current, moves, ls.Environment);
        }
        if (enc is EncounterSlot8GO { OriginFormat: PogoImportFormat.PK7 or PogoImportFormat.PB7 } g8)
        {
            if (g8.OriginFormat is PogoImportFormat.PK7 && IsWipedPK8(pk))
                return false; // Battle Version wiped Gen7 and below moves.
            Span<ushort> moves = stackalloc ushort[4];
            g8.GetInitialMoves(pk.MetLevel, moves);
            return AddOriginalMoves(result, current, moves, g8.OriginFormat == PogoImportFormat.PK7 ? LearnEnvironment.USUM : LearnEnvironment.GG);
        }
        return false;
    }

    private static void AddOriginalMoves(Span<bool> result, PKM pk, IEncounterTemplate enc, MoveSourceType types, IHomeSource local, ReadOnlySpan<EvoCriteria> evos)
    {
        if (enc is IMoveset { Moves: { HasMoves: true } x })
        {
            Span<ushort> moves = stackalloc ushort[4];
            x.CopyTo(moves);
            AddOriginalMoves(result, pk, evos, types, local, moves);
        }
        else if (enc is EncounterSlot8GO { OriginFormat: PogoImportFormat.PK7 or PogoImportFormat.PB7 } g8)
        {
            Span<ushort> moves = stackalloc ushort[4];
            g8.GetInitialMoves(pk.MetLevel, moves);
            AddOriginalMoves(result, pk, evos, types, local, moves);
        }
    }

    private static bool TryAddExclusiveMoves(Span<MoveResult> result, ReadOnlySpan<ushort> current, PKM pk)
    {
        if (pk.Species is (int)Species.Hoopa)
        {
            var move = pk.Form == 0 ? (ushort)Move.HyperspaceHole : (ushort)Move.HyperspaceFury;
            var index = current.IndexOf(move);
            if (index < 0)
                return false;
            ref var exist = ref result[index];
            if (exist.Valid)
                return false;
            exist = new MoveResult(new MoveLearnInfo(LearnMethod.HOME, LearnEnvironment.HOME));
            return true;
        }
        // Kyurem as Fused cannot move into HOME and trigger move sharing.
        return false;
    }

    private static void AddExclusiveMoves(Span<bool> result, PKM pk)
    {
        if (pk.Species == (int)Species.Hoopa)
            result[pk.Form == 0 ? (int)Move.HyperspaceHole : (int)Move.HyperspaceFury] = true;
    }

    /// <summary>
    /// Get the current HOME source for the given context.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    private static IHomeSource GetCurrent(EntityContext context) => context switch
    {
        EntityContext.Gen8 => LearnSource8SWSH.Instance,
        EntityContext.Gen8a => LearnSource8LA.Instance,
        EntityContext.Gen8b => LearnSource8BDSP.Instance,
        EntityContext.Gen9 => LearnSource9SV.Instance,
        _ => throw new ArgumentOutOfRangeException(nameof(context), context, null),
    };

    private static void RentLoopGetAll<T>(T instance, Span<bool> result, PKM pk, EvolutionHistory history,
        IEncounterTemplate enc,
        MoveSourceType types, LearnOption option, ReadOnlySpan<EvoCriteria> evos, IHomeSource local) where T : ILearnGroup
    {
        var length = instance.MaxMoveID + 1;
        var rent = ArrayPool<bool>.Shared.Rent(length);
        var temp = rent.AsSpan(0, length);
        instance.GetAllMoves(temp, pk, history, enc, types, option);
        LoopMerge(result, pk, evos, types, local, temp);
        temp.Clear();
        ArrayPool<bool>.Shared.Return(rent);
    }

    /// <summary>
    /// For each move that is possible to learn in another game, check if it is possible to learn in the current game.
    /// </summary>
    /// <param name="result">Resulting array of moves that are possible to learn in the current game.</param>
    /// <param name="pk">Entity to check.</param>
    /// <param name="evos">Evolutions to check.</param>
    /// <param name="types">Move source types to check.</param>
    /// <param name="dest">Destination game to check.</param>
    /// <param name="temp">Temporary array of moves that are possible to learn in the checked game.</param>
    private static void LoopMerge(Span<bool> result, PKM pk, ReadOnlySpan<EvoCriteria> evos, MoveSourceType types, IHomeSource dest, Span<bool> temp)
    {
        var length = Math.Min(result.Length, temp.Length);
        for (ushort move = 0; move < length; move++)
        {
            if (!temp[move])
                continue; // not possible to learn in other game
            if (result[move])
                continue; // already possible to learn in current game

            foreach (var evo in evos)
            {
                var chk = dest.GetCanLearnHOME(pk, evo, move, types);
                // HOME has special handling to allow Volt Tackle outside learnset possibilities.
                // Most games do not have a Learn Source for Volt Tackle besides it being specially inserted for Egg Encounters.
                if (chk.Method == LearnMethod.None && move is not (int)Move.VoltTackle)
                    continue;
                result[move] = true;
                break;
            }
        }
    }

    private static void AddOriginalMoves(Span<bool> result, PKM pk, ReadOnlySpan<EvoCriteria> evos, MoveSourceType types, IHomeSource dest, ReadOnlySpan<ushort> moves)
    {
        foreach (var move in moves)
        {
            if (move == 0)
                break;
            if (move >= result.Length)
                continue;
            if (result[move])
                continue; // already possible to learn in current game

            foreach (var evo in evos)
            {
                var chk = dest.GetCanLearnHOME(pk, evo, move, types);
                if (chk.Method == LearnMethod.None)
                    continue;
                result[move] = true;
                break;
            }
        }
    }

    private static bool AddOriginalMoves(Span<MoveResult> result, ReadOnlySpan<ushort> current, Span<ushort> moves, LearnEnvironment game)
    {
        bool addedAny = false;
        foreach (var move in moves)
        {
            if (move == 0)
                break;
            var index = current.IndexOf(move);
            if (index == -1)
                continue;
            if (result[index].Valid)
                continue;

            result[index] = MoveResult.Initial(game);
            addedAny = true;
        }
        return addedAny;
    }
}
