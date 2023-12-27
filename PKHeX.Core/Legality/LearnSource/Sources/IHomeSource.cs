namespace PKHeX.Core;

public interface IHomeSource
{
    LearnEnvironment Environment { get; }
    MoveLearnInfo GetCanLearnHOME(PKM pk, EvoCriteria evo, ushort move, MoveSourceType types = MoveSourceType.All);
}
