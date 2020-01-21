namespace PKHeX.Core
{
    /// <summary>
    /// Source the Move was learned from
    /// </summary>
    public enum MoveSource
    {
        Unknown,
        None,
        Relearn,
        Initial,
        LevelUp,
        TMHM,
        Tutor,
        EggMove,
        InheritLevelUp,
        Special,
        SpecialEgg,
        ShedinjaEvo,
        Sketch,
        Shared,
    }

    /// <summary>
    /// Move specific <see cref="CheckResult"/> to contain in which Generation it was learned &amp; source.
    /// </summary>
    public sealed class CheckMoveResult : CheckResult
    {
        public readonly MoveSource Source;
        public readonly int Generation;
        public bool Flag;

        internal CheckMoveResult(MoveSource m, int g, CheckIdentifier i)
            : base(i)
        {
            Source = m;
            Generation = g;
        }

        internal CheckMoveResult(MoveSource m, int g, Severity s, string c, CheckIdentifier i)
            : base(s, c, i)
        {
            Source = m;
            Generation = g;
        }

        internal CheckMoveResult(CheckMoveResult Org, Severity s, string c, CheckIdentifier i)
            : this(Org.Source, Org.Generation, s, c, i)
        {
        }
    }
}
