using System;
using static PKHeX.Core.RandomCorrelationRating;

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
    public PIDType GetSuggestedCorrelation() => PIDType.None;
    public RandomCorrelationRating IsCompatible(PIDType type, PKM pk)
    {
        if (type is PIDType.None)
            return Match;
        if (ParseSettings.Settings.FramePattern.EggRandomAnyType4)
            return NotIdeal;
        return Mismatch;
    }

    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria) => ConvertToPKM(tr, criteria);
    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr);
    public PK4 ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr, EncounterCriteria.Unrestricted);

    public PK4 ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria)
    {
        int language = (int)Language.GetSafeLanguage(Generation, (LanguageID)tr.Language, Version);
        var date = EncounterDate.GetDateNDS();

        var pk = new PK4
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
            MetDate = date,
            MetLocation = GetHatchLocation(tr.Version),
            EggMetDate = date,
            EggLocation = tr.Version == Version ? Locations.Daycare4 : Locations.LinkTrade4,
        };

        SetEncounterMoves(pk);

        if (criteria.IsSpecifiedIVs())
            criteria.SetRandomIVs(pk);
        else
            criteria.SetRandomIVs(pk, 3);

        // Get a random PID that matches gender/nature/ability criteria
        var pi = PersonalTable.HGSS[Species];
        var gr = pi.Gender;
        var pid = GetRandomPID(criteria, gr, out var gender);
        pk.PID = pid;
        pk.Gender = gender;
        pk.RefreshAbility((int)(pid & 1));

        return pk;
    }

    private uint GetRandomPID(in EncounterCriteria criteria, byte gr, out byte gender)
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

            // For Nidoran and Volbeat/Illumise, match the bit correlation to be most permissive with move inheritance.
            if (Breeding.IsGenderSpeciesDetermination(Species) && !Breeding.IsValidSpeciesBit34(pid, gender))
                continue; // 50/50 chance!

            // A 0-value PID is possible via Masuda Method even though a 0-value saved indicates "no egg available".
            // PID is rolled forward upon picking up the egg.
            // Not worth skipping 0-value PIDs. Too rare to be worth trying again, since it can be a valid PID.

            return pid;
        }
    }

    ILearnSource IEncounterEgg.Learn => Learn;
    public ILearnSource<PersonalInfo4> Learn => Version switch
    {
        GameVersion.D or GameVersion.P => LearnSource4DP.Instance,
        GameVersion.Pt => LearnSource4DP.Instance,
        GameVersion.HG or GameVersion.SS => LearnSource4HGSS.Instance,
        _ => throw new ArgumentOutOfRangeException(nameof(Version), Version, null),
    };

    private void SetEncounterMoves(PK4 pk)
    {
        var learn = Learn.GetLearnset(Species, Form);
        var initial = learn.GetBaseEggMoves(Level);
        pk.SetMoves(initial);
    }
}
