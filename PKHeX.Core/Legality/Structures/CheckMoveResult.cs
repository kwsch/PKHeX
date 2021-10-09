namespace PKHeX.Core
{
    /// <summary>
    /// Move specific <see cref="CheckResult"/> to contain in which Generation it was learned &amp; source.
    /// </summary>
    public sealed class CheckMoveResult : CheckResult
    {
        /// <summary>
        /// Method of learning the move.
        /// </summary>
        public readonly MoveSource Source;

        /// <summary>
        /// Generation the move was learned in.
        /// </summary>
        public readonly int Generation;

        /// <summary>
        /// Indicates if the source of the move was validated from the <see cref="PKM.RelearnMoves"/>
        /// </summary>
        public bool IsRelearn => Source == MoveSource.Relearn || (Source == MoveSource.EggMove && Generation >= 6);

        /// <summary>
        /// Indicates if the source of the move was validated as originating from an egg.
        /// </summary>
        public bool IsEggSource => Source is MoveSource.EggMove or MoveSource.InheritLevelUp;

        /// <summary>
        /// Checks if the Move should be present in a Relearn move pool (assuming Gen6+ origins).
        /// </summary>
        /// <remarks>Invalid moves that can't be validated should be here, hence the inclusion.</remarks>
        public bool ShouldBeInRelearnMoves() => Source != MoveSource.None && (!Valid || IsRelearn);

        internal CheckMoveResult(MoveSource m, int g, CheckIdentifier i) : base(i)
        {
            Source = m;
            Generation = g;
        }

        internal CheckMoveResult(MoveSource m, int g, Severity s, string c, CheckIdentifier i) : base(s, c, i)
        {
            Source = m;
            Generation = g;
        }

        internal CheckMoveResult(CheckMoveResult original, Severity s, string c, CheckIdentifier i) : this(original.Source, original.Generation, s, c, i) { }
        internal CheckMoveResult(CheckMoveResult original, Severity s, string c) : this(original.Source, original.Generation, s, c, original.Identifier) { }
    }
}
