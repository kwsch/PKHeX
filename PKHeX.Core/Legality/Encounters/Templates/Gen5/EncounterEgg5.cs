namespace PKHeX.Core;

public sealed record EncounterEgg5(ushort Species, GameVersion Version) : IEncounterEgg, IRandomCorrelation
{
    private const ushort Location = Locations.HatchLocation5;

    public string Name => "Egg";
    public string LongName => Name;

    public const byte Level = 1;
    public bool CanHaveVoltTackle => Species is (int)Core.Species.Pichu;

    public byte Form => 0;
    public byte Generation => 5;
    public EntityContext Context => EntityContext.Gen5;
    public bool IsShiny => false;
    public byte LevelMin => Level;
    public byte LevelMax => Level;
    ushort ILocation.EggLocation => Locations.Daycare5;
    ushort ILocation.Location => Location;
    public AbilityPermission Ability => AbilityBreedLegality.IsHiddenPossible5(Species) ? AbilityPermission.Any12H : AbilityPermission.Any12;
    public Ball FixedBall => Ball.Poke;
    public Shiny Shiny => Shiny.Random;
    public bool IsEgg => true;

    // Generation 5 has PID/IV correlations and RNG abuse; assume none.
    public bool IsCompatible(PIDType type, PKM pk) => type is PIDType.None;
    public PIDType GetSuggestedCorrelation() => PIDType.None;
    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria) => ConvertToPKM(tr, criteria);
    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr);
    public PK5 ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr, EncounterCriteria.Unrestricted);

    public PK5 ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria)
    {
        var version = Version;
        int language = (int)Language.GetSafeLanguage(Generation, (LanguageID)tr.Language, version);
        var date = EncounterDate.GetDateNDS();

        var pk = new PK5
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
            MetLevel = 1,
            MetLocation = Location,
            EggLocation = tr.Version == Version ? Locations.Daycare5 : Locations.LinkTrade5,

            MetDate = date,
            EggMetDate = date,

            Nature = criteria.GetNature(Nature.Random),
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
        var ability = criteria.GetAbilityFromNumber(Ability);
        var pid = GetRandomPID(criteria, gr);
        pid = pid & 0xFFFEFFFF | (uint)(ability & 1) << 16; // 0x00000000 or 0x00010000
        pk.PID = pid;
        pk.RefreshAbility(ability);

        return pk;
    }

    private static uint GetRandomPID(in EncounterCriteria criteria, byte gr)
    {
        var seed = Util.Rand32();
        while (true)
        {
            seed = LCRNG.Next(seed);
            var pid = seed;
            var gender = EntityGender.GetFromPIDAndRatio(pid, gr);
            if (criteria.IsSpecifiedGender() && !criteria.IsSatisfiedGender(gender))
                continue;
            return pid;
        }
    }

    private void SetEncounterMoves(PK5 pk, GameVersion version)
    {
        var ls = GameData.GetLearnSource(version);
        var learn = ls.GetLearnset(Species, Form);
        var initial = learn.GetBaseEggMoves(LevelMin);
        pk.SetMoves(initial);
    }
}
