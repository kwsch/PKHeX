using System;
using System.Collections.Generic;
using static PKHeX.Core.Move;
using static PKHeX.Core.EntityContext;

namespace PKHeX.Core;

/// <summary>
/// Overall information about Moves
/// </summary>
public static class MoveInfo
{
    /// <summary>
    /// Gets the base PP of a move ID depending on the <see cref="context"/>.
    /// </summary>
    /// <param name="context">Game context</param>
    /// <param name="move">Move ID</param>
    /// <returns>Amount of PP the move has by default (no PP Ups).</returns>
    public static byte GetPP(EntityContext context, ushort move)
    {
        var table = GetPPTable(context);
        if ((uint)move >= table.Length)
            return 0;
        return table[move];
    }

    /// <summary>
    /// Gets the move PP table for the <see cref="context"/>.
    /// </summary>
    public static ReadOnlySpan<byte> GetPPTable(EntityContext context) => context switch
    {
        Gen1 => MoveInfo1.MovePP_RBY,
        Gen2 => MoveInfo2.MovePP_GSC,
        Gen3 => MoveInfo3.MovePP_RS,
        Gen4 => MoveInfo4.MovePP_DP,
        Gen5 => MoveInfo5.MovePP_BW,
        Gen6 => MoveInfo6.MovePP,
        Gen7 => MoveInfo7.MovePP_SM,
        Gen8 => MoveInfo8.MovePP_SWSH,

        Gen7b => MoveInfo7b.MovePP_GG,
        Gen8a => MoveInfo8a.MovePP_LA,
        Gen8b => MoveInfo8.MovePP_SWSH,
        _ => throw new ArgumentOutOfRangeException(nameof(context)),
    };

    /// <summary>
    /// Gets a collection that can be used to check if a move cannot be used in battle.
    /// </summary>
    public static ICollection<ushort> GetDummiedMovesHashSet(EntityContext context) => context switch
    {
        Gen8 => MoveInfo8.DummiedMoves_SWSH,
        Gen8a => MoveInfo8a.DummiedMoves_LA,
        Gen8b => MoveInfo8b.DummiedMoves_BDSP,
        _ => Array.Empty<ushort>(),
    };

    /// <summary>
    /// Checks if the move is a Z-move.
    /// </summary>
    public static bool IsMoveZ(ushort move) => move switch
    {
        >= (int)BreakneckBlitzP      and <= (int)Catastropika        => true, // [622-658]
        >= (int)SinisterArrowRaid    and <= (int)GenesisSupernova    => true, // [695-703]
           (int)TenMVoltThunderbolt                                  => true, // [719]
        >= (int)LightThatBurnstheSky and <= (int)ClangorousSoulblaze => true, // [723-728]
        _ => false,
    };

    /// <summary>
    /// Checks if the move is a Dynamax-only move.
    /// </summary>
    public static bool IsMoveDynamax(ushort move) => move is (>= (int)MaxFlare and <= (int)MaxSteelspike);

    /// <summary>
    /// Checks if the move can be known by anything in any context.
    /// </summary>
    /// <remarks> Assumes the move ID is within [0,max]. </remarks>
    public static bool IsMoveKnowable(ushort move) => !IsMoveZ(move) && !IsMoveDynamax(move);

    /// <summary>
    /// Checks if the move can be sketched in any game.
    /// </summary>
    public static bool IsMoveSketch(ushort move) => move switch
    {
        // Can't Sketch
        (int)Struggle => false,
        (int)Chatter => false,

        // Unreleased
        (int)LightofRuin => false,

        _ => IsMoveKnowable(move),
    };

    /// <summary>
    /// Checks if the <see cref="move"/> is unable to be used in battle.
    /// </summary>
    public static bool IsDummiedMove(PKM pk, ushort move)
    {
        var hashSet = GetDummiedMovesHashSet(pk.Context);
        return hashSet.Contains(move);
    }

    /// <summary>
    /// Checks if any Move in the currently known moves is an unusable move (yellow triangle).
    /// </summary>
    public static bool IsDummiedMoveAny(PKM pk)
    {
        var hs = GetDummiedMovesHashSet(pk.Context);
        if (hs.Count == 0)
            return false;

        for (int i = 0; i < 4; i++)
        {
            var move = (ushort)pk.GetMove(i);
            if (hs.Contains(move))
                return true;
        }
        return false;
    }

    /// <summary>
    /// Checks if Sketch can obtain the <see cref="move"/> in the requested <see cref="context"/>
    /// </summary>
    /// <param name="move">Move ID</param>
    /// <param name="context">Generation to check</param>
    /// <returns>True if can be sketched, false if not available.</returns>
    public static bool IsValidSketch(ushort move, EntityContext context)
    {
        if (!IsMoveSketch(move))
            return false;
        if (context is Gen6 && move is ((int)ThousandArrows or (int)ThousandWaves))
            return false;
        if (context is Gen8b) // can't Sketch unusable moves in BDSP, no Sketch in PLA
        {
            if (MoveInfo8b.DummiedMoves_BDSP.Contains(move))
                return false;
            if (move > Legal.MaxMoveID_8)
                return false;
        }

        return move <= GetMaxMoveID(context);
    }

    private static int GetMaxMoveID(EntityContext context) => context switch
    {
        Gen1 => Legal.MaxMoveID_1,
        Gen2 => Legal.MaxMoveID_2,
        Gen3 => Legal.MaxMoveID_3,
        Gen4 => Legal.MaxMoveID_4,
        Gen5 => Legal.MaxMoveID_5,
        Gen6 => Legal.MaxMoveID_6_AO,
        Gen7 => Legal.MaxMoveID_7_USUM,
        Gen7b => Legal.MaxMoveID_7b,
        Gen8 => Legal.MaxMoveID_8a,
        Gen8a => Legal.MaxMoveID_8a,
        Gen8b => Legal.MaxMoveID_8b,
        _ => -1,
    };
}
