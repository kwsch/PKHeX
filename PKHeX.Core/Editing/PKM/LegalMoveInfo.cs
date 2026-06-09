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
    public bool CanLearn(ushort move) => GetMoveSources(move) != None;

    /// <summary>
    /// Returns the sources that allow the provided move to be learned, or <see cref="None"/> if it cannot be learned.
    /// </summary>
    /// <param name="move">Move to check the sources for</param>
    /// <returns>Sources that allow the move to be learned</returns>
    public IndicatedSourceType GetMoveSources(ushort move) => AllowedMoves[move];

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
        // Wipe or set as learnable based on learnability; encounter moves will be added later.
        for (int i = 1; i < type.Length; i++)
            type[i] = learn[i] ? Learn : None;

        // If the original moveset is deleted, then encounter moves are not relevant to legality and should not be added.
        if (!la.Entity.IsOriginalMovesetDeleted())
            AddEncounterMoves(type, la.EncounterOriginal);

        type[0] = None; // Move ID 0 is always None
    }

    private static void AddEncounterMoves(Span<IndicatedSourceType> result, IEncounterTemplate enc)
    {
        if (enc is IRelearn { Relearn: { HasMoves: true } relearn })
            relearn.FlagMoves(result, Relearn);

        if (enc is IEncounterEgg egg)
            FlagMoves(result, Egg, egg.Learn.GetEggMoves(enc.Species, enc.Form));
        else if (enc is IMoveset { Moves: { HasMoves: true } set})
            set.FlagMoves(result, Encounter);
        else if (enc is ISingleMoveBonus { IsMoveBonusPossible: true } single)
            FlagIfNone(result, EncounterSingle, single.GetMoveBonusPossible());

        return;

        static void FlagMoves(Span<IndicatedSourceType> result, IndicatedSourceType flag, ReadOnlySpan<ushort> moves)
        {
            foreach (var move in moves)
                result[move] |= flag;
        }
        static void FlagIfNone(Span<IndicatedSourceType> result, IndicatedSourceType flag, ReadOnlySpan<ushort> moves)
        {
            foreach (var move in moves)
            {
                if (result[move] == None)
                    result[move] |= flag;
            }
        }
    }
}

[Flags]
public enum IndicatedSourceType : byte
{
    None = 0,
    Learn           = 1 << 0,
    Egg             = 1 << 1,
    Encounter       = 1 << 2,
    EncounterSingle = 1 << 3,
    Relearn         = 1 << 4,
}
