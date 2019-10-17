namespace PKHeX.Core
{
    public interface ISaveBlock5BW
    {
        MyItem Items { get; }
        Zukan5 Zukan { get; }
        Misc5 MiscBlock { get; }
        MysteryBlock5 MysteryBlock { get; }
        Daycare5 DaycareBlock { get; }
        BoxLayout5 BoxLayout { get; }
        PlayerData5 PlayerData { get; }
        BattleSubway5 BattleSubwayBlock { get; }
    }
}