namespace PKHeX.Core
{
    public interface ISaveBlock6Main : ISaveBlock6Core, IPokePuff, IOPower, ILink
    {
        BoxLayout6 BoxLayout { get; }
        BattleBox6 BattleBoxBlock { get; }
        MysteryBlock6 MysteryBlock { get; }
        SuperTrainBlock SuperTrain { get; }
    }
}