using System;

namespace PKHeX.Core;

public sealed record EncounterEgg7(ushort Species, byte Form, GameVersion Version) : IEncounterEgg
{
    private const ushort Location = Locations.HatchLocation7;

    public string Name => "Egg";
    public string LongName => Name;

    public const byte Level = 1;
    public bool CanHaveVoltTackle => Species is (int)Core.Species.Pichu;

    public byte Generation => 7;
    public EntityContext Context => EntityContext.Gen7;
    public bool IsShiny => false;
    public byte LevelMin => Level;
    public byte LevelMax => Level;
    ushort ILocation.EggLocation => Locations.Daycare5;
    ushort ILocation.Location => Location;
    public AbilityPermission Ability => AbilityBreedLegality.IsHiddenPossible7(Species, Form) ? AbilityPermission.Any12H : AbilityPermission.Any12;
    public Ball FixedBall => Ball.None; // Inheritance allowed.
    public Shiny Shiny => Shiny.Random;
    public bool IsEgg => true;

    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria) => ConvertToPKM(tr, criteria);
    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr);
    public PK7 ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr, EncounterCriteria.Unrestricted);

    public PK7 ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria)
    {
        int language = (int)Language.GetSafeLanguage(Generation, (LanguageID)tr.Language, Version);
        var date = EncounterDate.GetDate3DS();
        var pi = PersonalTable.USUM[Species, Form];
        var rnd = Util.Rand;
        var geo = tr.GetRegionOrigin(language);

        var pk = new PK7
        {
            Species = Species,
            CurrentLevel = Level,
            Version = Version,
            Ball = (byte)Ball.Poke,
            ID32 = tr.ID32,
            OriginalTrainerGender = tr.Gender,

            // Force Hatch
            Language = language,
            Nickname = SpeciesName.GetSpeciesNameGeneration(Species, language, Generation),
            OriginalTrainerName = tr.OT,
            OriginalTrainerFriendship = 120,
            MetLevel = 1,
            MetDate = date,
            MetLocation = Location,
            EggMetDate = date,
            EggLocation = tr.Version == Version ? Locations.Daycare5 : Locations.LinkTrade6,

            EncryptionConstant = rnd.Rand32(),
            PID = rnd.Rand32(),
            Nature = criteria.GetNature(),
            Gender = criteria.GetGender(pi),

            ConsoleRegion = geo.ConsoleRegion,
            Country = geo.Country,
            Region = geo.Region,
        };
        pk.StatNature = pk.Nature;

        if (Species is (int)Core.Species.Scatterbug)
            pk.Form = Vivillon3DS.GetPattern(pk.Country, pk.Region);

        SetEncounterMoves(pk);
        pk.HealPP();
        pk.RelearnMove1 = pk.Move1;
        pk.RelearnMove2 = pk.Move2;
        pk.RelearnMove3 = pk.Move3;
        pk.RelearnMove4 = pk.Move4;

        if (criteria.IsSpecifiedIVsAny(out _))
            criteria.SetRandomIVs(pk);
        else
            criteria.SetRandomIVs(pk, 3);

        var ability = criteria.GetAbilityFromNumber(Ability);
        pk.RefreshAbility(ability);

        return pk;
    }

    public ILearnSource Learn => Version switch
    {
        GameVersion.SN or GameVersion.MN => LearnSource7SM.Instance,
        GameVersion.US or GameVersion.UM => LearnSource7USUM.Instance,
        _ => throw new ArgumentOutOfRangeException(nameof(Version), Version, null),
    };

    private void SetEncounterMoves(PK7 pk)
    {
        var learn = Learn.GetLearnset(Species, Form);
        var initial = learn.GetBaseEggMoves(LevelMin);
        pk.SetMoves(initial);
    }
}
