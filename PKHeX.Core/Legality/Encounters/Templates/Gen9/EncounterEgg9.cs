namespace PKHeX.Core;

public sealed record EncounterEgg9(ushort Species, byte Form, GameVersion Version) : IEncounterEgg
{
    private const ushort Location = Locations.HatchLocation9;

    public string Name => "Egg";
    public string LongName => Name;

    public const byte Level = 1;
    public bool CanHaveVoltTackle => Species is (int)Core.Species.Pichu;

    public byte Generation => 9;
    public EntityContext Context => EntityContext.Gen9;
    public bool IsShiny => false;
    public byte LevelMin => Level;
    public byte LevelMax => Level;
    ushort ILocation.EggLocation => Locations.Picnic9;
    ushort ILocation.Location => Location;
    public AbilityPermission Ability => AbilityBreedLegality.IsHiddenPossibleHOME(Species) ? AbilityPermission.Any12H : AbilityPermission.Any12;
    public Ball FixedBall => Ball.None; // Inheritance allowed.
    public Shiny Shiny => Shiny.Random;
    public bool IsEgg => true;

    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria) => ConvertToPKM(tr, criteria);
    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr);
    public PK9 ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr, EncounterCriteria.Unrestricted);

    public PK9 ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria)
    {
        int language = (int)Language.GetSafeLanguage(Generation, (LanguageID)tr.Language, Version);
        var date = EncounterDate.GetDateSwitch();
        var pi = PersonalTable.SV[Species, Form];
        var rnd = Util.Rand;

        var pk = new PK9
        {
            Species = Species,
            Form = Form,
            CurrentLevel = Level,
            Version = Version,
            Ball = (byte)Ball.Poke,
            ID32 = tr.ID32,
            OriginalTrainerGender = tr.Gender,

            // Force Hatch
            Language = language,
            Nickname = SpeciesName.GetSpeciesNameGeneration(Species, language, Generation),
            OriginalTrainerName = tr.OT,
            OriginalTrainerFriendship = 100,
            ObedienceLevel = 1,
            MetLevel = 1,
            MetDate = date,
            MetLocation = Location,
            EggMetDate = date,
            EggLocation = tr.Version == Version ? Locations.Picnic9 : Locations.LinkTrade6,

            EncryptionConstant = rnd.Rand32(),
            PID = rnd.Rand32(),
            Nature = criteria.GetNature(),
            Gender = criteria.GetGender(pi),
        };
        pk.StatNature = pk.Nature;

        SetEncounterMoves(pk);

        if (criteria.IsSpecifiedIVs())
            criteria.SetRandomIVs(pk);
        else
            criteria.SetRandomIVs(pk, 3);

        pk.HeightScalar = PokeSizeUtil.GetRandomScalar(rnd);
        pk.WeightScalar = PokeSizeUtil.GetRandomScalar(rnd);
        pk.Scale = PokeSizeUtil.GetRandomScalar(rnd);
        var type = Tera9RNG.GetTeraTypeFromPersonal(Species, Form, rnd.Rand64());
        pk.TeraTypeOriginal = (MoveType)type;

        var ability = criteria.GetAbilityFromNumber(Ability);
        pk.RefreshAbility(ability);

        return pk;
    }

    ILearnSource IEncounterEgg.Learn => Learn;
    public ILearnSource<PersonalInfo9SV> Learn => LearnSource9SV.Instance;

    private void SetEncounterMoves(PK9 pk)
    {
        var learn = Learn.GetLearnset(Species, Form);
        var initial = learn.GetBaseEggMoves(Level);
        pk.SetMoves(initial);
        pk.SetRelearnMoves(initial);
    }
}
