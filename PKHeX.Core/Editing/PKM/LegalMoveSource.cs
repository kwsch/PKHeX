using System;
using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core
{
    /// <summary>
    /// Legal Move information for a single <see cref="PKM"/>, for indicating if a move is legal or not.
    /// </summary>
    public sealed class LegalMoveSource
    {
        public readonly bool[] IsMoveBoxOrdered = new bool[4];

        /// <summary> Creates a shallow copy of the array reference for use in binding. </summary>
        public IReadOnlyList<ComboItem> DataSource => (ComboItem[])MoveDataAllowed.Clone();

        /// <summary>
        /// Checks if the requested <see cref="move"/> is legally able to be learned.
        /// </summary>
        /// <param name="move">Move to check if can be learned</param>
        /// <returns>True if can learn the move</returns>
        public bool CanLearn(int move) => AllowedMoves.Contains(move);

        private readonly HashSet<int> AllowedMoves = new();
        private ComboItem[] MoveDataAllowed = Array.Empty<ComboItem>();

        /// <summary>
        /// Reloads the legality sources to permit the provided legal <see cref="moves"/>.
        /// </summary>
        /// <param name="moves">Legal moves to allow</param>
        public void ReloadMoves(IReadOnlyList<int> moves)
        {
            // check prior move-pool to not needlessly refresh the data set
            if (AllowedMoves.Count == moves.Count && AllowedMoves.SetEquals(moves))
                return;

            AllowedMoves.Clear();
            foreach (var m in moves)
                AllowedMoves.Add(m);
            Array.Sort(MoveDataAllowed, Compare);
            // MoveDataAllowed = MoveDataAllowed.OrderByDescending(m => AllowedMoves.Contains(m.Value)).ToArray();

            // defer re-population until dropdown is opened; handled by dropdown event
            Array.Clear(IsMoveBoxOrdered, 0, IsMoveBoxOrdered.Length);
        }

        private int Compare(ComboItem i1, ComboItem i2)
        {
            // split into 2 groups: Allowed & Not, and sort each sublist
            var c1 = AllowedMoves.Contains(i1.Value);
            var c2 = AllowedMoves.Contains(i2.Value);
            if (c1)
                return c2 ? string.CompareOrdinal(i1.Text, i2.Text) : -1;
            return c2 ? 1 : string.CompareOrdinal(i1.Text, i2.Text);
        }

        public void ReloadMoves(IReadOnlyList<ComboItem> moves)
        {
            MoveDataAllowed = moves.ToArray();
        }
    }
}
