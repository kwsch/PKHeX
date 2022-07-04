namespace PKHeX.Core;

public readonly record struct MoveLearnInfo(LearnMethod Method, GameVersion Environment, byte Argument = 0)
{
    public readonly LearnMethod Method = Method;
    public readonly byte Argument = Argument;
    public readonly GameVersion Environment = Environment;
}
