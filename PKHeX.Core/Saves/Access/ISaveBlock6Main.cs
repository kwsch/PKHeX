namespace PKHeX.Core
{
    public interface ISaveBlock6Main : ISaveBlock6Core, IPokePuff, IOPower, ILink
    {
        BoxLayout6 BoxLayout { get; }
        BattleBox6 BattleBox { get; }
        MysteryBlock6 MysteryGift { get; }
        SuperTrainBlock SuperTrain { get; }
        MaisonBlock Maison { get; }
        SubEventLog6 SUBE { get; }
    }
}