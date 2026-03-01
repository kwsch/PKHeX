namespace PKHeX.Core;

/// <summary>
/// Simple record containing trainer data
/// </summary>
/// <remarks>
/// See <see cref="MutableTrainerInfo"/> for a mutable version of this record.
/// </remarks>
public sealed record SimpleTrainerInfo : ITrainerInfo, IRegionOriginReadOnly, ITrainerID
{
    public string OT { get; init; } = TrainerName.ProgramINT;
    public ushort TID16 { get; init; } = 12345;
    public ushort SID16 { get; init; } = 54321;
    public byte Gender { get; init; }
    public int Language { get; init; } = (int)LanguageID.English;
    public uint ID32 { get => (uint)(TID16 | (SID16 << 16)); init => (TID16, SID16) = ((ushort)value, (ushort)(value >> 16)); }
    public TrainerIDFormat TrainerIDDisplayFormat => this.GetTrainerIDFormat();

    // IRegionOrigin for generation 6/7
    public byte ConsoleRegion { get; init; } = 1; // North America
    public byte Region { get; init; } = 7; // California
    public byte Country { get; init; } = 49; // USA

    public GameVersion Version { get; }
    public byte Generation { get; init; } = Latest.Generation;
    public EntityContext Context { get; init; } = Latest.Context;

    public SimpleTrainerInfo(GameVersion version = Latest.Version)
    {
        Version = version;
        Context = Version.Context;
        Generation = Context.Generation;
        if (Context is not (EntityContext.Gen6 or EntityContext.Gen7))
            ConsoleRegion = Region = Country = 0;
    }

    public SimpleTrainerInfo(ITrainerInfo other) : this(other, other.Version) { }

    public SimpleTrainerInfo(ITrainerInfo other, GameVersion specified) : this(specified)
    {
        OT = other.OT;
        TID16 = other.TID16;
        SID16 = other.SID16;
        Gender = other.Gender;
        Language = other.Language;
        Generation = other.Generation;
        Context = other.Context;
        if (Context is not (EntityContext.Gen6 or EntityContext.Gen7))
            return;

        var geo = other.GetRegionOrigin(other.Language);
        ConsoleRegion = geo.ConsoleRegion;
        Region = geo.Region;
        Country = geo.Country;
    }
}

/// <summary>
/// Mutable record containing trainer data
/// </summary>
/// <remarks>
/// See <see cref="SimpleTrainerInfo"/> for an immutable version of this record.
/// </remarks>
public sealed record MutableTrainerInfo : ITrainerInfo, IRegionOrigin, ITrainerID
{
    public string OT { get; set; } = TrainerName.ProgramINT;
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

    public GameVersion Version { get; set; }
    public byte Generation { get; set; } = Latest.Generation;
    public EntityContext Context { get; set; } = Latest.Context;

    public MutableTrainerInfo(GameVersion version = Latest.Version)
    {
        Version = version;
        Context = Version.Context;
        Generation = Context.Generation;
        if (Context is not (EntityContext.Gen6 or EntityContext.Gen7))
            ConsoleRegion = Region = Country = 0;
    }

    public MutableTrainerInfo(ITrainerInfo other) : this(other, other.Version) { }

    public MutableTrainerInfo(ITrainerInfo other, GameVersion specified) : this(specified)
    {
        OT = other.OT;
        TID16 = other.TID16;
        SID16 = other.SID16;
        Gender = other.Gender;
        Language = other.Language;
        Generation = other.Generation;
        Context = other.Context;
        if (Context is not (EntityContext.Gen6 or EntityContext.Gen7))
            return;

        var geo = other.GetRegionOrigin(other.Language);
        ConsoleRegion = geo.ConsoleRegion;
        Region = geo.Region;
        Country = geo.Country;
    }
}
