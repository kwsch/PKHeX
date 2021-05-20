namespace PKHeX.Core
{
    /// <summary>
    /// Interface for Accessing named blocks within a Generation 6 save file.
    /// </summary>
    /// <remarks>Used by all Gen 6 games, including <see cref="SAV6AODemo"/>.</remarks>
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
