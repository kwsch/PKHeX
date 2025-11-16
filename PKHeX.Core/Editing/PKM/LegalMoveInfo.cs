using System;
using System.Buffers;
using static PKHeX.Core.IndicatedSourceType;

namespace PKHeX.Core;

/// <summary>
/// Caches move learn data via <see cref="ReloadMoves"/>
/// </summary>
public sealed class LegalMoveInfo
{
    // Use a byte array instead of a HashSet; we have a limited range of moves.
    // This implementation is faster (no hashcode or bucket search) with lower memory overhead (1 byte per move ID).
    private readonly IndicatedSourceType[] AllowedMoves = new IndicatedSourceType[(int)Move.MAX_COUNT + 1];

    /// <summary>
    /// Checks if the requested <see cref="move"/> is legally able to be learned.
    /// </summary>
    /// <param name="move">Move to check if it can be learned</param>
    /// <returns>True if it can learn the move</returns>
    public bool CanLearn(ushort move) => AllowedMoves[move] != None;

    /// <summary>
    /// Reloads the legality sources to permit the provided legal info.
    /// </summary>
    /// <param name="la">Details of analysis, moves to allow</param>
    public bool ReloadMoves(LegalityAnalysis la)
    {
        var rentLearn = ArrayPool<bool>.Shared.Rent(AllowedMoves.Length);
        var spanLearn = rentLearn.AsSpan(0, AllowedMoves.Length);
        var rentEval = ArrayPool<IndicatedSourceType>.Shared.Rent(spanLearn.Length);
        var spanEval = rentEval.AsSpan(0, spanLearn.Length);
        try
        {
            LearnPossible.Get(la.Entity, la.EncounterOriginal, la.Info.EvoChainsAllGens, spanLearn);
            ComputeEval(spanEval, spanLearn, la);
            if (spanEval.SequenceEqual(AllowedMoves))
                return false;
            spanEval.CopyTo(AllowedMoves);
            return true;
        }
        catch
        {
            if (Array.TrueForAll(AllowedMoves, z => z == None))
                return false;
            AllowedMoves.AsSpan().Clear();
            return true;
        }
        finally
        {
            spanLearn.Clear();
            spanEval.Clear();
            ArrayPool<IndicatedSourceType>.Shared.Return(rentEval);
            ArrayPool<bool>.Shared.Return(rentLearn);
        }
    }

    private static void ComputeEval(Span<IndicatedSourceType> type, ReadOnlySpan<bool> learn, LegalityAnalysis la)
    {
        for (int i = 0; i < type.Length; i++)
            type[i] = learn[i] ? Learn : None;

        if (!la.Entity.IsOriginalMovesetDeleted())
            AddEncounterMoves(type, la.EncounterOriginal);

        type[0] = None; // Move ID 0 is always None
    }

    private static void AddEncounterMoves(Span<IndicatedSourceType> type, IEncounterTemplate enc)
    {
        if (enc is IEncounterEgg egg)
        {
            var moves = egg.Learn.GetEggMoves(enc.Species, enc.Form);
            foreach (var move in moves)
                type[move] = Egg;
        }
        else if (enc is IMoveset {Moves: {HasMoves: true} set})
        {
            if (type[set.Move1] == None)
                type[set.Move1] = Encounter;
            if (type[set.Move2] == None)
                type[set.Move2] = Encounter;
            if (type[set.Move3] == None)
                type[set.Move3] = Encounter;
            if (type[set.Move4] == None)
                type[set.Move4] = Encounter;
        }
        else if (enc is ISingleMoveBonus single)
        {
            var moves = single.GetMoveBonusPossible();
            foreach (var move in moves)
            {
                if (type[move] == None)
                    type[move] = EncounterSingle;
            }
        }

        if (enc is IRelearn { Relearn: {HasMoves: true} relearn})
        {
            if (type[relearn.Move1] == None)
                type[relearn.Move1] = Relearn;
            if (type[relearn.Move2] == None)
                type[relearn.Move2] = Relearn;
            if (type[relearn.Move3] == None)
                type[relearn.Move3] = Relearn;
            if (type[relearn.Move4] == None)
                type[relearn.Move4] = Relearn;
        }
    }
}

public enum IndicatedSourceType : byte
{
    None = 0,
    Learn,
    Egg,
    Encounter,
    EncounterSingle,
    Relearn,
}
