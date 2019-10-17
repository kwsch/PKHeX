namespace PKHeX.Core
{
    public interface ISaveBlock8Main
    {
        MyItem Items { get; }
        Record8 Records { get; }
        PlayTime8 Played { get; }
        MyStatus8 MyStatus { get; }
        ConfigSave8 Config { get; }
        GameTime8 GameTime { get; }
        Misc8 MiscBlock { get; }
        Zukan8 Zukan { get; }
        EventWork8 EventWork { get; }
        BoxLayout8 BoxLayout { get; }
        Situation8 Situation { get; }
        FieldMoveModelSave8 OverworldBlock { get; }
    }
}