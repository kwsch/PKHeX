using System;
using static PKHeX.Core.RandomCorrelationRating;

namespace PKHeX.Core;

public sealed record EncounterEgg3(ushort Species, GameVersion Version) : IEncounterEgg, IRandomCorrelation
{
    private byte Location => Version is GameVersion.FR or GameVersion.LG
        ? Locations.HatchLocationFRLG
        : Locations.HatchLocationRSE;

    public string Name => "Egg";
    public string LongName => Name;

    public const byte Level = 5;
    public bool CanHaveVoltTackle => Species is (int)Core.Species.Pichu && Version is GameVersion.E;

    public byte Form => 0; // No forms in Gen3
    public byte Generation => 3;
    public EntityContext Context => EntityContext.Gen3;
    public bool IsShiny => false;
    public byte LevelMin => Level;
    public byte LevelMax => Level;
    ushort ILocation.EggLocation => 0;
    ushort ILocation.Location => Location;
    public AbilityPermission Ability => AbilityPermission.Any12;
    public Ball FixedBall => Ball.Poke;
    public Shiny Shiny => Shiny.Random;
    public bool IsEgg => true;

    // Generation 3 has PID/IV correlations and RNG abuse; assume none.
    public PIDType GetSuggestedCorrelation() => PIDType.None;
    public RandomCorrelationRating IsCompatible(PIDType type, PKM pk)
    {
        if (type is PIDType.None)
            return Match;
        if (ParseSettings.Settings.FramePattern.EggRandomAnyType3)
            return NotIdeal;
        return Mismatch;
    }

    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria) => ConvertToPKM(tr, criteria);
    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr);
    public PK3 ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr, EncounterCriteria.Unrestricted);

    public PK3 ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria)
    {
        int language = (int)Language.GetSafeLanguage(Generation, (LanguageID)tr.Language, Version);

        var pk = new PK3
        {
            Species = Species,
            CurrentLevel = Level,
            Version = Version,
            Ball = (byte)FixedBall,
            ID32 = tr.ID32,
            OriginalTrainerGender = tr.Gender,

            // Force Hatch
            Language = language,
            OriginalTrainerName = tr.OT,
            Nickname = SpeciesName.GetSpeciesNameGeneration(Species, language, Generation),
            OriginalTrainerFriendship = 120,
            MetLevel = 0,
            MetLocation = Location,
        };

        SetEncounterMoves(pk);
        pk.HealPP();

        if (criteria.IsSpecifiedIVsAny(out _))
            criteria.SetRandomIVs(pk);
        else
            criteria.SetRandomIVs(pk, 3);

        // Get a random PID that matches gender/nature/ability criteria
        var pi = PersonalTable.E[Species];
        var gr = pi.Gender;
        var pid = GetRandomPID(criteria, gr);
        pk.PID = pid;
        pk.RefreshAbility((int)(pid % 2));

        return pk;
    }

    private uint GetRandomPID(in EncounterCriteria criteria, byte gr)
    {
        var seed = Util.Rand32();
        while (true)
        {
            // LCRNG is sufficiently random, especially with the nature of vBlanks potentially (super rarely) disjointing rand calls.
            seed = LCRNG.Next(seed);
            var pid = seed;
            var gender = EntityGender.GetFromPIDAndRatio(pid, gr);
            if (criteria.IsSpecifiedGender() && !criteria.IsSatisfiedGender(gender))
                continue;
            if (criteria.IsSpecifiedNature() && !criteria.IsSatisfiedNature((Nature)(pid % 25)))
                continue;
            if (criteria.IsSpecifiedAbility() && !criteria.IsSatisfiedAbility((byte)(pid % 2)))
                continue;

            // For Nidoran and Volbeat/Illumise, match the bit correlation to be most permissive with move inheritance.
            if (Breeding.IsGenderSpeciesDetermination(Species) && !Breeding.IsValidSpeciesBit34(pid, gender))
                continue; // 50/50 chance!

            if (!Daycare3.IsValidProcPID(pid, Version))
                continue; // 0-value PID is invalid

            return pid;
        }
    }

    ILearnSource IEncounterEgg.Learn => Learn;
    public ILearnSource<PersonalInfo3> Learn => Version switch
    {
        GameVersion.R or GameVersion.S => LearnSource3RS.Instance,
        GameVersion.E => LearnSource3RS.Instance,
        GameVersion.FR => LearnSource3FR.Instance,
        GameVersion.LG => LearnSource3FR.Instance,
        _ => throw new ArgumentOutOfRangeException(nameof(Version), Version, null),
    };

    private void SetEncounterMoves(PK3 pk)
    {
        var learn = Learn.GetLearnset(Species, Form);
        var initial = learn.GetBaseEggMoves(LevelMin);
        pk.SetMoves(initial);
    }
}
