namespace PKHeX.Core;

public enum EvolutionCheckResult
{
    Valid = 0,
    InsufficientLevel,
    BadGender,
    BadForm,
    WrongEC,
    VisitVersion,
    LowContestStat,
    Untraded,
}
