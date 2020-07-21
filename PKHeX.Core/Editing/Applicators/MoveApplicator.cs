using System;
using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core
{
    public static class MoveApplicator
    {
        /// <summary>
        /// Sets the individual PP Up count values depending if a Move is present in the move's slot or not.
        /// </summary>
        /// <param name="pk">Pokémon to modify.</param>
        /// <param name="moves"><see cref="PKM.Moves"/> to use (if already known). Will fetch the current <see cref="PKM.Moves"/> if not provided.</param>
        public static void SetMaximumPPUps(this PKM pk, int[] moves)
        {
            pk.Move1_PPUps = GetPPUpCount(moves[0]);
            pk.Move2_PPUps = GetPPUpCount(moves[1]);
            pk.Move3_PPUps = GetPPUpCount(moves[2]);
            pk.Move4_PPUps = GetPPUpCount(moves[3]);

            pk.SetMaximumPPCurrent(moves);
            static int GetPPUpCount(int moveID) => moveID > 0 ? 3 : 0;
        }

        /// <summary>
        /// Sets the individual PP Up count values depending if a Move is present in the move slot or not.
        /// </summary>
        /// <param name="pk">Pokémon to modify.</param>
        public static void SetMaximumPPUps(this PKM pk) => pk.SetMaximumPPUps(pk.Moves);

        /// <summary>
        /// Updates the <see cref="PKM.Moves"/> and updates the current PP counts.
        /// </summary>
        /// <param name="pk">Pokémon to modify.</param>
        /// <param name="moves"><see cref="PKM.Moves"/> to set. Will be resized if 4 entries are not present.</param>
        /// <param name="maxPP">Option to maximize PP Ups</param>
        public static void SetMoves(this PKM pk, int[] moves, bool maxPP = false)
        {
            if (moves.Any(z => z > pk.MaxMoveID))
                moves = moves.Where(z => z <= pk.MaxMoveID).ToArray();
            if (moves.Length != 4)
                Array.Resize(ref moves, 4);

            pk.Moves = moves;
            if (maxPP)
                pk.SetMaximumPPUps(moves);
            else
                pk.SetMaximumPPCurrent(moves);
            pk.FixMoves();
        }

        /// <summary>
        /// Updates the individual PP count values for each move slot based on the maximum possible value.
        /// </summary>
        /// <param name="pk">Pokémon to modify.</param>
        /// <param name="moves"><see cref="PKM.Moves"/> to use (if already known). Will fetch the current <see cref="PKM.Moves"/> if not provided.</param>
        public static void SetMaximumPPCurrent(this PKM pk, IReadOnlyList<int> moves)
        {
            pk.Move1_PP = moves.Count == 0 ? 0 : pk.GetMovePP(moves[0], pk.Move1_PPUps);
            pk.Move2_PP = moves.Count <= 1 ? 0 : pk.GetMovePP(moves[1], pk.Move2_PPUps);
            pk.Move3_PP = moves.Count <= 2 ? 0 : pk.GetMovePP(moves[2], pk.Move3_PPUps);
            pk.Move4_PP = moves.Count <= 3 ? 0 : pk.GetMovePP(moves[3], pk.Move4_PPUps);
        }

        /// <summary>
        /// Updates the individual PP count values for each move slot based on the maximum possible value.
        /// </summary>
        /// <param name="pk">Pokémon to modify.</param>
        public static void SetMaximumPPCurrent(this PKM pk) => pk.SetMaximumPPCurrent(pk.Moves);

        /// <summary>
        /// Refreshes the Move PP for the desired move.
        /// </summary>
        /// <param name="pk">Pokémon to modify.</param>
        /// <param name="index">Move PP to refresh.</param>
        public static void SetSuggestedMovePP(this PKM pk, int index)
        {
            switch (index)
            {
                case 0: pk.Move1_PP = pk.GetMovePP(pk.Move1, pk.Move1_PPUps); return;
                case 1: pk.Move2_PP = pk.GetMovePP(pk.Move2, pk.Move2_PPUps); return;
                case 2: pk.Move3_PP = pk.GetMovePP(pk.Move3, pk.Move3_PPUps); return;
                case 3: pk.Move4_PP = pk.GetMovePP(pk.Move4, pk.Move4_PPUps); return;
                default: throw new ArgumentException(nameof(index));
            }
        }
    }
}