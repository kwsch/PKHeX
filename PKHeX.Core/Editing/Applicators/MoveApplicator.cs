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
        /// <param name="moves"><see cref="PKM.Moves"/> to use (if already known). Will fetch the current <see cref="PKM.Moves"/> if not provided.</param>
        public void SetMaximumPPCurrent(ReadOnlySpan<ushort> moves)
        {
            pk.Move1_PP = moves.Length == 0 ? 0 : pk.GetMovePP(moves[0], pk.Move1_PPUps);
            pk.Move2_PP = moves.Length <= 1 ? 0 : pk.GetMovePP(moves[1], pk.Move2_PPUps);
            pk.Move3_PP = moves.Length <= 2 ? 0 : pk.GetMovePP(moves[2], pk.Move3_PPUps);
            pk.Move4_PP = moves.Length <= 3 ? 0 : pk.GetMovePP(moves[3], pk.Move4_PPUps);
        }

        /// <summary>
        /// Updates the individual PP count values for each move slot based on the maximum possible value.
        /// </summary>
        /// <param name="moves"><see cref="PKM.Moves"/> to use (if already known). Will fetch the current <see cref="PKM.Moves"/> if not provided.</param>
        public void SetMaximumPPCurrent(Moveset moves)
        {
            pk.Move1_PP = moves.Move1 == 0 ? 0 : pk.GetMovePP(moves.Move1, pk.Move1_PPUps);
            pk.Move2_PP = moves.Move2 == 0 ? 0 : pk.GetMovePP(moves.Move2, pk.Move2_PPUps);
            pk.Move3_PP = moves.Move3 == 0 ? 0 : pk.GetMovePP(moves.Move3, pk.Move3_PPUps);
            pk.Move4_PP = moves.Move4 == 0 ? 0 : pk.GetMovePP(moves.Move4, pk.Move4_PPUps);
        }
    }
}
