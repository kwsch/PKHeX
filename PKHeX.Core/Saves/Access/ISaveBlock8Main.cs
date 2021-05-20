namespace PKHeX.Core
{
    /// <summary>
    /// Interface for Accessing named blocks within a Generation 8 save file.
    /// </summary>
    public interface ISaveBlock8Main
    {
        Box8 BoxInfo { get; }
        Party8 PartyInfo { get; }
        MyItem Items { get; }
        MyStatus8 MyStatus { get; }
        Misc8 Misc { get; }
        Zukan8 Zukan { get; }
        BoxLayout8 BoxLayout { get; }
        PlayTime8 Played { get; }
        Fused8 Fused { get; }
        Daycare8 Daycare { get; }
        Record8 Records { get; }
        TrainerCard8 TrainerCard { get; }
        RaidSpawnList8 Raid { get; }
        RaidSpawnList8 RaidArmor { get; }
        RaidSpawnList8 RaidCrown { get; }
        TitleScreen8 TitleScreen { get; }
        TeamIndexes8 TeamIndexes { get; }
    }
}
