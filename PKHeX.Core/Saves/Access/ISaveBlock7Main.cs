namespace PKHeX.Core
{
    public interface ISaveBlock7Main
    {
        MyItem Items { get; }
        MysteryBlock7 MysteryBlock { get; }
        PokeFinder7 PokeFinder { get; }
        JoinFesta7 Festa { get; }
        Daycare7 DaycareBlock { get; }
        Record6 Records { get; }
        PlayTime6 Played { get; }
        MyStatus7 MyStatus { get; }
        FieldMoveModelSave7 OverworldBlock { get; }
        Situation7 Situation { get; }
        ConfigSave7 Config { get; }
        GameTime7 GameTime { get; }
        Misc7 MiscBlock { get; }
        Zukan7 Zukan { get; }
        BoxLayout7 BoxLayout { get; }
        BattleTree7 BattleTreeBlock { get; }
        ResortSave7 ResortSave { get; }
        FieldMenu7 FieldMenu { get; }
        FashionBlock7 FashionBlock { get; }
    }
}