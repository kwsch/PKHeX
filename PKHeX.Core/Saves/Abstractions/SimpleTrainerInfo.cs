namespace PKHeX.Core;

/// <summary>
/// Simple record containing trainer data
/// </summary>
public sealed record SimpleTrainerInfo : ITrainerInfo, IRegionOrigin
{
    public string OT { get; set; } = "PKHeX";
    public ushort TID16 { get; set; } = 12345;
    public ushort SID16 { get; set; } = 54321;
    public byte Gender { get; set; }
    public int Language { get; set; } = (int)LanguageID.English;
    public uint ID32 { get => (uint)(TID16 | (SID16 << 16)); set => (TID16, SID16) = ((ushort)value, (ushort)(value >> 16)); }
    public TrainerIDFormat TrainerIDDisplayFormat => this.GetTrainerIDFormat();

    // IRegionOrigin for generation 6/7
    public byte ConsoleRegion { get; set; } = 1; // North America
    public byte Region { get; set; } = 7; // California
    public byte Country { get; set; } = 49; // USA

    public GameVersion Version { get; }
    public byte Generation { get; init; } = PKX.Generation;
    public EntityContext Context { get; init; } = PKX.Context;

    public SimpleTrainerInfo(GameVersion game = PKX.Version)
    {
        Version = game;
        Context = Version.GetContext();
        SanityCheckRegionOrigin(game);
    }

    private void SanityCheckRegionOrigin(GameVersion game)
    {
        if (GameVersion.Gen7b.Contains(game) || game.GetGeneration() >= 8)
            this.ClearRegionOrigin();
    }

    public SimpleTrainerInfo(ITrainerInfo other) : this(other.Version)
    {
        OT = other.OT;
        TID16 = other.TID16;
        SID16 = other.SID16;
        Gender = other.Gender;
        Language = other.Language;
        Generation = other.Generation;
        Context = other.Context;

        if (other is IRegionOrigin r)
            r.CopyRegionOrigin(this);

        SanityCheckRegionOrigin(Version);
    }
}
