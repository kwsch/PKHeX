using System;
using System.Text;

namespace PKHeX.Core;

/// <summary>
/// Stores parsed data about how a move was learned.
/// </summary>
/// <param name="Info">Info about the game it was learned in.</param>
/// <param name="EvoStage">Evolution stage index within the <see cref="MoveLearnInfo.Environment"/> evolution list it existed in.</param>
/// <param name="Generation">Rough indicator of generation the <see cref="MoveLearnInfo.Environment"/> was.</param>
/// <param name="Expect">Optional value used when the move is not legal, to indicate that another move ID should have been in that move slot instead.</param>
public readonly record struct MoveResult(MoveLearnInfo Info, byte EvoStage = 0, byte Generation = 0, ushort Expect = 0)
{
    public bool IsParsed => this != default;
    public bool Valid => Info.Method.IsValid();

    internal MoveResult(LearnMethod method, LearnEnvironment game = 0) : this(new MoveLearnInfo(method, game), Generation: game.GetGeneration()) { }

    public string Summary(ISpeciesForm current, EvolutionHistory history)
    {
        var sb = new StringBuilder(48);
        Info.Summarize(sb);
        if (Info.Method.HasExpectedMove())
        {
            var name = ParseSettings.MoveStrings[Expect];
            var str = LegalityCheckStrings.LMoveFExpectSingle_0;
            sb.Append(' ').AppendFormat(str, name);
            return sb.ToString();
        }

        var detail = GetDetail(history);
        if (detail.Species == 0)
            return sb.ToString();
        if (detail.Species == current.Species && detail.Form == current.Form)
            return sb.ToString();

        sb.Append(' ').Append(ParseSettings.SpeciesStrings[detail.Species]);
        if (detail.Form != current.Form)
            sb.Append('-').Append(detail.Form);
        return sb.ToString();
    }

    private EvoCriteria GetDetail(EvolutionHistory history)
    {
        var evos = Info.Environment.GetEvolutions(history);
        var stage = EvoStage;
        if (stage >= evos.Length)
            return default;
        return evos[stage];
    }

    /// <summary> Checks if the Move should be present in a Relearn move pool (assuming Gen6+ origins). </summary>
    /// <remarks>Invalid moves that can't be validated should be here, hence the inclusion.</remarks>
    public bool ShouldBeInRelearnMoves() => IsRelearn || !Valid;
    public bool IsRelearn => Info.Method.IsRelearn();

    public Severity Judgement => Valid ? Severity.Valid : Severity.Invalid;
    public string Rating => Judgement.Description();

    public string Format(string format, int index, PKM pk, EvolutionHistory history) => string.Format(format, Rating, index, Summary(pk, history));

    public static MoveResult Initial(LearnEnvironment game) => new(LearnMethod.Initial, game);
    public static readonly MoveResult Relearn = new(LearnMethod.Relearn);
    public static readonly MoveResult Empty = new(LearnMethod.Empty);
    public static readonly MoveResult Duplicate = new(LearnMethod.Duplicate);
    public static readonly MoveResult EmptyInvalid = new(LearnMethod.EmptyInvalid);
    public static readonly MoveResult Sketch = new(LearnMethod.Sketch);
    public static MoveResult Unobtainable(ushort expect) => new(LearnMethod.UnobtainableExpect) { Expect = expect };
    public static MoveResult Unobtainable() => new(LearnMethod.Unobtainable);

    /// <summary>
    /// Checks if all <see cref="MoveResult"/>s in the span are <see cref="Valid"/>.
    /// </summary>
    public static bool AllValid(ReadOnlySpan<MoveResult> span)
    {
        foreach (var result in span)
        {
            if (!result.Valid)
                return false;
        }
        return true;
    }

    /// <summary>
    /// Checks if all <see cref="MoveResult"/>s in the span are <see cref="IsParsed"/>.
    /// </summary>
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
