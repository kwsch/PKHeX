namespace PKHeX.Core;

/// <summary>
/// Interface for Accessing named blocks within a Generation 7 LGP/E save file.
/// </summary>
/// <remarks>Blocks common for <see cref="SAV7b"/></remarks>
public interface ISaveBlock7b
{
    MyItem7b Items { get; }
    Misc7b Misc { get; }
    Zukan7b Zukan { get; }
    MyStatus7b Status { get; }
    PlayTime7b Played { get; }
    ConfigSave7b Config { get; }
    EventWork7b EventWork { get; }
    PokeListHeader Storage { get; }
    WB7Records GiftRecords { get; }
    Daycare7b Daycare { get; }
    CaptureRecords Captured { get; }
    GoParkStorage Park { get; }
    PlayerGeoLocation7b PlayerGeoLocation { get; }
}
