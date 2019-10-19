namespace PKHeX.Core
{
    public interface ISaveBlock5BW
    {
        MyItem Items { get; }
        Zukan5 Zukan { get; }
        Misc5 Misc { get; }
        MysteryBlock5 Mystery { get; }
        Daycare5 Daycare { get; }
        BoxLayout5 BoxLayout { get; }
        PlayerData5 PlayerData { get; }
        BattleSubway5 BattleSubway { get; }
    }
}