namespace PKHeX.Core;

public readonly record struct MoveLearnInfo(LearnMethod Method, byte Argument, GameVersion Environment);
