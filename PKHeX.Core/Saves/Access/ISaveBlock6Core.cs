namespace PKHeX.Core
{
    public interface ISaveBlock6Core
    {
        MyItem Items { get; }
        ItemInfo6 ItemInfo { get; }
        GameTime6 GameTime { get; }
        Situation6 Situation { get; }
        PlayTime6 Played { get; }
        MyStatus6 Status { get; }
        RecordBlock6 Records { get; }
    }
}