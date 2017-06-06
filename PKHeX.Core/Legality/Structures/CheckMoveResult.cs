namespace PKHeX.Core
{
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
    }

    public class CheckMoveResult : CheckResult
    {
        public readonly MoveSource Source;
        public readonly int Generation;

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
            : base(s, c, i)
        {
            Source = Org?.Source ?? MoveSource.Unknown;
            Generation = Org?.Generation ?? 0;
        }
    }
}
