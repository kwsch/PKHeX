namespace PKHeX.Core;

public sealed record EncounterEgg8b(ushort Species, byte Form, GameVersion Version) : IEncounterEgg
{
    private const ushort Location = Locations.HatchLocation8b;

    public string Name => "Egg";
    public string LongName => Name;

    public const byte Level = 1;
    public bool CanHaveVoltTackle => Species is (int)Core.Species.Pichu;

    public byte Generation => 8;
    public EntityContext Context => EntityContext.Gen8b;
    public bool IsShiny => false;
    public byte LevelMin => Level;
    public byte LevelMax => Level;
    ushort ILocation.EggLocation => Locations.Daycare8b;
    ushort ILocation.Location => Location;
    public AbilityPermission Ability => AbilityBreedLegality.IsHiddenPossibleHOME(Species) ? AbilityPermission.Any12H : AbilityPermission.Any12;
    public Ball FixedBall => Ball.None; // Inheritance allowed.
    public Shiny Shiny => Shiny.Random;
    public bool IsEgg => true;

    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria) => ConvertToPKM(tr, criteria);
    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr);
    public PB8 ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr, EncounterCriteria.Unrestricted);

    public PB8 ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria)
    {
        var version = Version;
        int language = (int)Language.GetSafeLanguage(Generation, (LanguageID)tr.Language, version);
        var date = EncounterDate.GetDateSwitch();
        var pi = PersonalTable.BDSP[Species, Form];
        var rnd = Util.Rand;

        var pk = new PB8
        {
            Species = Species,
            CurrentLevel = Level,
            Version = version,
            Ball = (byte)Ball.Poke,
            TID16 = tr.TID16,
            SID16 = tr.SID16,
            OriginalTrainerGender = tr.Gender,

            // Force Hatch
            Language = language,
            OriginalTrainerName = tr.OT,
            Nickname = SpeciesName.GetSpeciesNameGeneration(Species, language, Generation),
            OriginalTrainerFriendship = 100,
            MetLevel = 1,
            MetLocation = Location,
            EggLocation = tr.Version == Version ? Locations.Daycare8b : Locations.LinkTrade6NPC,

            MetDate = date,
            EggMetDate = date,

            PID = rnd.Rand32(),
            EncryptionConstant = rnd.Rand32(),
            Nature = criteria.GetNature(Nature.Random),
            Gender = criteria.GetGender(pi),
        };
        pk.StatNature = pk.Nature;

        SetEncounterMoves(pk, version);
        pk.HealPP();
        pk.RelearnMove1 = pk.Move1;
        pk.RelearnMove2 = pk.Move2;
        pk.RelearnMove3 = pk.Move3;
        pk.RelearnMove4 = pk.Move4;

        if (criteria.IsSpecifiedIVsAny(out _))
            criteria.SetRandomIVs(pk);
        else
            criteria.SetRandomIVs(pk, 3);

        pk.HeightScalar = PokeSizeUtil.GetRandomScalar(rnd);
        pk.WeightScalar = PokeSizeUtil.GetRandomScalar(rnd);

        var ability = criteria.GetAbilityFromNumber(Ability);
        pk.RefreshAbility(ability);

        return pk;
    }

    private void SetEncounterMoves(PB8 pk, GameVersion version)
    {
        var ls = GameData.GetLearnSource(version);
        var learn = ls.GetLearnset(Species, Form);
        var initial = learn.GetBaseEggMoves(LevelMin);
        pk.SetMoves(initial);
    }
}
