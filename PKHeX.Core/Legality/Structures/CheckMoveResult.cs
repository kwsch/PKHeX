namespace PKHeX.Core;

/// <summary>
/// Move specific <see cref="CheckResult"/> to contain in which Generation it was learned &amp; source.
/// </summary>
public sealed record CheckMoveResult : ICheckResult
{
    public Severity Judgement { get; private set; }
    public CheckIdentifier Identifier { get; private set; }
    public string Comment { get; private set; }

    public bool Valid => Judgement >= Severity.Fishy;
    public string Rating => Judgement.Description();

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

    internal CheckMoveResult(MoveSource m, int g, Severity s, string c, CheckIdentifier i)
    {
        Judgement = s;
        Comment = c;
        Identifier = i;
        Source = m;
        Generation = g;
    }

    internal void FlagIllegal(string comment)
    {
        Judgement = Severity.Invalid;
        Comment = comment;
    }

    internal void FlagIllegal(string comment, CheckIdentifier identifier)
    {
        Judgement = Severity.Invalid;
        Comment = comment;
        Identifier = identifier;
    }

    public string Format(string format) => string.Format(format, Rating, Comment);
    public string Format(string format, int index) => string.Format(format, Rating, index, Comment);
}
