namespace PKHeX.Core;

public sealed record EncounterEgg4(ushort Species, GameVersion Version) : IEncounterEgg, IRandomCorrelation
{
    private ushort Location => GetHatchLocation(Version);

    private static ushort GetHatchLocation(GameVersion version)
    {
        return version is GameVersion.HG or GameVersion.SS
            ? Locations.HatchLocationHGSS
            : Locations.HatchLocationDPPt;
    }

    public string Name => "Egg";
    public string LongName => Name;

    public const byte Level = 1;
    public bool CanHaveVoltTackle => Species is (int)Core.Species.Pichu;

    public byte Form => 0; // No forms in Gen3
    public byte Generation => 4;
    public EntityContext Context => EntityContext.Gen4;
    public bool IsShiny => false;
    public byte LevelMin => Level;
    public byte LevelMax => Level;
    ushort ILocation.EggLocation => Locations.Daycare4;
    ushort ILocation.Location => Location;
    public AbilityPermission Ability => AbilityPermission.Any12;
    public Ball FixedBall => Ball.Poke;
    public Shiny Shiny => Shiny.Random;
    public bool IsEgg => true;

    // Generation 4 has PID/IV correlations and RNG abuse; assume none.
    public bool IsCompatible(PIDType type, PKM pk) => type is PIDType.None;
    public PIDType GetSuggestedCorrelation() => PIDType.None;
    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria) => ConvertToPKM(tr, criteria);
    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr);
    public PK4 ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr, EncounterCriteria.Unrestricted);

    public PK4 ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria)
    {
        var version = Version;
        int language = (int)Language.GetSafeLanguage(Generation, (LanguageID)tr.Language, version);
        var date = EncounterDate.GetDateNDS();

        var pk = new PK4
        {
            Species = Species,
            CurrentLevel = Level,
            Version = version,
            Ball = (byte)FixedBall,
            TID16 = tr.TID16,
            SID16 = tr.SID16,
            OriginalTrainerGender = tr.Gender,

            // Force Hatch
            Language = language,
            OriginalTrainerName = tr.OT,
            Nickname = SpeciesName.GetSpeciesNameGeneration(Species, language, Generation),
            OriginalTrainerFriendship = 120,
            MetLevel = 0,
            MetLocation = GetHatchLocation(tr.Version),
            EggLocation = tr.Version == Version ? Locations.Daycare4 : Locations.LinkTrade4,

            MetDate = date,
            EggMetDate = date,
        };

        SetEncounterMoves(pk, version);
        pk.HealPP();

        if (criteria.IsSpecifiedIVsAny(out _))
            criteria.SetRandomIVs(pk);
        else
            criteria.SetRandomIVs(pk, 3);

        // Get a random PID that matches gender/nature/ability criteria
        var pi = pk.PersonalInfo;
        var gr = pi.Gender;
        var pid = GetRandomPID(criteria, gr, out var gender);
        pk.PID = pid;
        pk.Gender = gender;
        pk.RefreshAbility((int)(pid & 1));

        return pk;
    }

    private static uint GetRandomPID(in EncounterCriteria criteria, byte gr, out byte gender)
    {
        var seed = Util.Rand32();
        while (true)
        {
            seed = LCRNG.Next(seed);
            var pid = seed;
            gender = EntityGender.GetFromPIDAndRatio(pid, gr);
            if (criteria.IsSpecifiedGender() && !criteria.IsSatisfiedGender(gender))
                continue;
            if (criteria.IsSpecifiedNature() && !criteria.IsSatisfiedNature((Nature)(pid % 25)))
                continue;
            if (criteria.IsSpecifiedAbility() && !criteria.IsSatisfiedAbility((byte)(pid % 2)))
                continue;
            return pid;
        }
    }

    private void SetEncounterMoves(PK4 pk, GameVersion version)
    {
        var ls = GameData.GetLearnSource(version);
        var learn = ls.GetLearnset(Species, Form);
        var initial = learn.GetBaseEggMoves(LevelMin);
        pk.SetMoves(initial);
    }
}
