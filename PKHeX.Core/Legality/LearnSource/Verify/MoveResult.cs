using System;

namespace PKHeX.Core;

public readonly record struct MoveResult : ICheckResult
{
    public MoveLearnInfo Info { get; init; }
    public byte EvoStage { get; init; }
    public byte Generation { get; init; }
    public short Expect { get; init; }

    public bool IsParsed => this != default;
    public bool Valid => Info.Method.IsValid();

    internal MoveResult(LearnMethod method, GameVersion game = GameVersion.Any) : this(new MoveLearnInfo(method, game)) { }

    public MoveResult(MoveLearnInfo Info, byte EvoStage = 0, byte Generation = 0, short Expect = 0)
    {
        this.Info = Info;
        this.EvoStage = EvoStage;
        this.Generation = Generation;
        this.Expect = Expect;
    }

    private string Summary() => Info.Summarize();

    /// <summary> Checks if the Move should be present in a Relearn move pool (assuming Gen6+ origins). </summary>
    /// <remarks>Invalid moves that can't be validated should be here, hence the inclusion.</remarks>
    public bool ShouldBeInRelearnMoves() => IsRelearn || !Valid;
    public bool IsRelearn => Info.Method.IsRelearn();

    public Severity Judgement => Info.Method == LearnMethod.EmptyFishy ? Severity.Fishy : Valid ? Severity.Valid : Severity.Invalid;
    public CheckIdentifier Identifier => CheckIdentifier.CurrentMove;
    public string Comment => Summary();
    public string Rating => Judgement.Description();

    public string Format(string format) => string.Format(format, Rating, Comment);
    public string Format(string format, int index) => string.Format(format, Rating, index, Comment);

    public static readonly MoveResult Relearn = new(LearnMethod.Relearn);
    public static readonly MoveResult Empty = new(LearnMethod.Empty);
    public static readonly MoveResult Duplicate = new(LearnMethod.Duplicate);
    public static readonly MoveResult EmptyInvalid = new(LearnMethod.EmptyInvalid);
    public static readonly MoveResult Sketch = new(LearnMethod.Sketch);
    public static MoveResult Unobtainable(int expect) => new(LearnMethod.UnobtainableExpect) { Expect = (short)expect };
    public static MoveResult Unobtainable() => new(LearnMethod.Unobtainable);

    public static bool AllValid(ReadOnlySpan<MoveResult> span)
    {
        foreach (var result in span)
        {
            if (!result.Valid)
                return false;
        }
        return true;
    }

    public static bool AllParsed(ReadOnlySpan<MoveResult> span)
    {
        foreach (var result in span)
        {
            if (!result.IsParsed)
                return false;
        }
        return true;
    }
}
