using System;
using static PKHeX.Core.RandomCorrelationRating;

namespace PKHeX.Core;

public sealed record EncounterEgg5(ushort Species, byte Form, GameVersion Version) : IEncounterEgg, IRandomCorrelation
{
    private const ushort Location = Locations.HatchLocation5;

    public string Name => "Egg";
    public string LongName => Name;

    public const byte Level = 1;
    public bool CanHaveVoltTackle => Species is (int)Core.Species.Pichu;

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
    public RandomCorrelationRating IsCompatible(PIDType type, PKM pk) => type is PIDType.None ? Match : Mismatch;
    public PIDType GetSuggestedCorrelation() => PIDType.None;
    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria) => ConvertToPKM(tr, criteria);
    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr);
    public PK5 ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr, EncounterCriteria.Unrestricted);

    public PK5 ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria)
    {
        int language = (int)Language.GetSafeLanguage(Generation, (LanguageID)tr.Language, Version);
        var date = EncounterDate.GetDateNDS();

        var pk = new PK5
        {
            Species = Species,
            Form = Form,
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
            MetLevel = 1,
            MetDate = date,
            MetLocation = Location,
            EggMetDate = date,
            EggLocation = tr.Version == Version ? Locations.Daycare5 : Locations.LinkTrade5,

            Nature = criteria.GetNature(),
        };

        SetEncounterMoves(pk);
        pk.HealPP();

        if (criteria.IsSpecifiedIVsAny(out _))
            criteria.SetRandomIVs(pk);
        else
            criteria.SetRandomIVs(pk, 3);

        // Get a random PID that matches gender/nature/ability criteria
        var pi = PersonalTable.B2W2[Species];
        var gr = pi.Gender;
        var ability = criteria.GetAbilityFromNumber(Ability);
        var pid = GetRandomPID(criteria, gr, out var gender);
        pid = (pid & 0xFFFEFFFFu) | (uint)(ability & 1) << 16; // 0x00000000 or 0x00010000
        pk.PID = pid;
        pk.Gender = gender;
        pk.RefreshAbility(ability);

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
            return pid;
        }
    }

    public ILearnSource Learn => Version switch
    {
        GameVersion.B or GameVersion.W => LearnSource5BW.Instance,
        GameVersion.B2 or GameVersion.W2 => LearnSource5B2W2.Instance,
        _ => throw new ArgumentOutOfRangeException(nameof(Version), Version, null),
    };

    private void SetEncounterMoves(PK5 pk)
    {
        var learn = Learn.GetLearnset(Species, Form);
        var initial = learn.GetBaseEggMoves(LevelMin);
        pk.SetMoves(initial);
    }
}
