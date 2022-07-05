namespace PKHeX.Core;

public readonly record struct MoveResult(MoveLearnInfo Info, byte EvoStage)
{
    public bool IsParsed => this != default;
    public bool IsValid => Info.Method.IsValid();

    public string Summary() => Info.Summarize();
}
