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
        public readonly IList<bool> IsMoveBoxOrdered = new bool[4];
        public IReadOnlyList<ComboItem> DataSource => (ComboItem[])MoveDataAllowed.Clone();
        public bool CanLearn(int move) => AllowedMoves.Contains(move);

        private readonly HashSet<int> AllowedMoves = new HashSet<int>();
        private ComboItem[] MoveDataAllowed = Array.Empty<ComboItem>();

        public void ReloadMoves(IReadOnlyList<int> moves)
        {
            // check prior movepool to not needlessly refresh the dataset
            if (AllowedMoves.Count == moves.Count && AllowedMoves.SetEquals(moves))
                return;

            AllowedMoves.Clear();
            foreach (var m in moves)
                AllowedMoves.Add(m);
            Array.Sort(MoveDataAllowed, Compare);
            // MoveDataAllowed = MoveDataAllowed.OrderByDescending(m => AllowedMoves.Contains(m.Value)).ToArray();

            // defer repop until dropdown is opened; handled by dropdown event
            for (int i = 0; i < IsMoveBoxOrdered.Count; i++)
                IsMoveBoxOrdered[i] = false;
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
