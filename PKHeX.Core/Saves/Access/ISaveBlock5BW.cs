namespace PKHeX.Core;

/// <summary>
/// Interface for Accessing named blocks within a Generation 5 save file.
/// </summary>
public interface ISaveBlock5BW
{
    MyItem Items { get; }
    Zukan5 Zukan { get; }
    Misc5 Misc { get; }
    MysteryBlock5 Mystery { get; }
    Chatter5 Chatter { get; }
    Daycare5 Daycare { get; }
    BoxLayout5 BoxLayout { get; }
    PlayerData5 PlayerData { get; }
    BattleSubway5 BattleSubway { get; }
    Entralink5 Entralink { get; }
    Musical5 Musical { get; }
    Encount5 Encount { get; }
    EventWork5 EventWork { get; }
    BattleBox5 BattleBox { get; }
    GlobalLink5 GlobalLink { get; }
    GTS5 GTS { get; }
    AdventureInfo5 AdventureInfo { get; }
    Record5 Records { get; }
}
