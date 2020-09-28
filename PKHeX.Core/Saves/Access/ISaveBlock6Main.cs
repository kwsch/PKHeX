namespace PKHeX.Core
{
    public interface ISaveBlock6Main : ISaveBlock6Core
    {
        Puff6 Puff { get; }
        OPower6 OPower { get; }
        LinkBlock6 Link { get; }
        BoxLayout6 BoxLayout { get; }
        BattleBox6 BattleBox { get; }
        ConfigSave6 Config { get; }
        MysteryBlock6 MysteryGift { get; }
        SuperTrainBlock SuperTrain { get; }
        MaisonBlock Maison { get; }
        SubEventLog6 SUBE { get; }
        Encount6 Encount { get; }
    }
}