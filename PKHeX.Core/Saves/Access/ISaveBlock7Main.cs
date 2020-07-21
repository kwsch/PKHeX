namespace PKHeX.Core
{
    public interface ISaveBlock7Main
    {
        MyItem Items { get; }
        MysteryBlock7 MysteryGift { get; }
        PokeFinder7 PokeFinder { get; }
        JoinFesta7 Festa { get; }
        Daycare7 Daycare { get; }
        RecordBlock6 Records { get; }
        PlayTime6 Played { get; }
        MyStatus7 MyStatus { get; }
        FieldMoveModelSave7 Overworld { get; }
        Situation7 Situation { get; }
        ConfigSave7 Config { get; }
        GameTime7 GameTime { get; }
        Misc7 Misc { get; }
        Zukan7 Zukan { get; }
        BoxLayout7 BoxLayout { get; }
        BattleTree7 BattleTree { get; }
        ResortSave7 ResortSave { get; }
        FieldMenu7 FieldMenu { get; }
        FashionBlock7 Fashion { get; }
    }
}