namespace PKHeX.Core;

/// <summary>
/// Simple record containing trainer data
/// </summary>
public sealed record SimpleTrainerInfo : ITrainerInfo, IRegionOrigin
{
    public string OT { get; set; } = "PKHeX";
    public int TID { get; set; } = 12345;
    public int SID { get; set; } = 54321;
    public int Gender { get; set; }
    public int Language { get; set; } = (int)LanguageID.English;

    // IRegionOrigin for generation 6/7
    public byte ConsoleRegion { get; set; } = 1; // North America
    public byte Region { get; set; } = 7; // California
    public byte Country { get; set; } = 49; // USA

    public int Game { get; }
    public int Generation { get; set; } = PKX.Generation;
    public EntityContext Context { get; set; } = PKX.Context;

    public SimpleTrainerInfo(GameVersion game = GameVersion.SW)
    {
        Game = (int) game;
        SanityCheckRegionOrigin(game);
    }

    private void SanityCheckRegionOrigin(GameVersion game)
    {
        if (GameVersion.Gen7b.Contains(game) || game.GetGeneration() >= 8)
            this.ClearRegionOrigin();
    }

    public SimpleTrainerInfo(ITrainerInfo other) : this((GameVersion)other.Game)
    {
        OT = other.OT;
        TID = other.TID;
        SID = other.SID;
        Gender = other.Gender;
        Language = other.Language;
        Generation = other.Generation;

        if (other is IRegionOrigin r)
            r.CopyRegionOrigin(this);

        SanityCheckRegionOrigin((GameVersion)Game);
    }
}
