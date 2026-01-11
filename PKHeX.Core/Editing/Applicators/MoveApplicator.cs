using System;

namespace PKHeX.Core;

/// <summary>
/// Logic for applying a moveset to a <see cref="PKM"/>.
/// </summary>
public static class MoveApplicator
{
    extension(PKM pk)
    {
        /// <summary>
        /// Sets the individual PP Up count values depending on if a Move is present in the move's slot or not.
        /// </summary>
        /// <param name="moves"><see cref="PKM.Moves"/> to use.</param>
        public void SetMaximumPPUps(ReadOnlySpan<ushort> moves)
        {
            pk.Move1_PPUps = GetPPUpCount(moves[0]);
            pk.Move2_PPUps = GetPPUpCount(moves[1]);
            pk.Move3_PPUps = GetPPUpCount(moves[2]);
            pk.Move4_PPUps = GetPPUpCount(moves[3]);

            pk.SetMaximumPPCurrent(moves);
            static int GetPPUpCount(ushort moveID)
            {
                if (Legal.IsPPUpAvailable(moveID))
                    return 3;
                return 0;
            }
        }

        /// <summary>
        /// Sets the individual PP Up count values depending on if a Move is present in the move slot or not.
        /// </summary>
        public void SetMaximumPPUps()
        {
            Span<ushort> moves = stackalloc ushort[4];
            pk.GetMoves(moves);
            pk.SetMaximumPPUps(moves);
        }

        /// <summary>
        /// Updates the <see cref="PKM.Moves"/> and updates the current PP counts.
        /// </summary>
        /// <param name="input"><see cref="PKM.Moves"/> to set.</param>
        /// <param name="maxPP">Option to maximize PP Ups</param>
        public void SetMoves(ReadOnlySpan<ushort> input, bool maxPP = false)
        {
            Span<ushort> moves = stackalloc ushort[4];
            if (input.Length <= 4)
                input.CopyTo(moves);
            else
                input[..4].CopyTo(moves);

            // Remote all indexes with a value above the maximum move ID allowed by the format.
            var max = pk.MaxMoveID;
            for (int i = 0; i < moves.Length; i++)
            {
                if (moves[i] > max)
                    moves[i] = 0;
            }

            pk.SetMoves(moves);
            if (maxPP && Legal.IsPPUpAvailable(pk))
                pk.SetMaximumPPUps(moves);
            pk.FixMoves();
        }

        /// <summary>
        /// Updates the individual PP count values for each move slot based on the maximum possible value.
        /// </summary>
        /// <param name="moves"><see cref="PKM.Moves"/> to use (if already known).</param>
        public void SetMaximumPPCurrent(ReadOnlySpan<ushort> moves)
        {
            // In some games, move[i] == 0` *should* set 0, but the game's configuration has a non-zero PP for `(None)`
            // (I'm looking at you, S/V and Z-A)
            pk.Move1_PP = pk.GetMovePP(moves.Length > 0 ? moves[0] : (ushort)0, pk.Move1_PPUps);
            pk.Move2_PP = pk.GetMovePP(moves.Length > 1 ? moves[1] : (ushort)0, pk.Move2_PPUps);
            pk.Move3_PP = pk.GetMovePP(moves.Length > 2 ? moves[2] : (ushort)0, pk.Move3_PPUps);
            pk.Move4_PP = pk.GetMovePP(moves.Length > 3 ? moves[3] : (ushort)0, pk.Move4_PPUps);
        }
    }
}
