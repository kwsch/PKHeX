namespace PKHeX.Core;

/// <summary>
/// Generation 5 Dream Radar gift encounters
/// </summary>
public sealed record EncounterStatic5Radar(ushort Species, byte Form, AbilityPermission Ability = AbilityPermission.OnlyHidden)
    : IEncounterable, IEncounterMatch, IEncounterConvertible<PK5>
{
    public byte Generation => 5;
    public EntityContext Context => EntityContext.Gen5;
    public GameVersion Version => GameVersion.B2W2;
    public ushort Location => 30015;
    public Ball FixedBall => Ball.Dream;
    public bool IsShiny => false;
    public Shiny Shiny => Shiny.Never;
    public bool IsEgg => false;
    public ushort EggLocation => 0;
    public byte LevelMin => 5;
    public byte LevelMax => 40;
    public string Name => "Dream Radar Encounter";
    public string LongName => Name;

    #region Generating

    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr);
    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria) => ConvertToPKM(tr, criteria);
    public PK5 ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr, EncounterCriteria.Unrestricted);

    public PK5 ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria)
    {
        int language = (int)Language.GetSafeLanguage(Generation, (LanguageID)tr.Language);
        var version = this.GetCompatibleVersion(tr.Version);
        var pi = PersonalTable.B2W2[Species];
        var pk = new PK5
        {
            Species = Species,
            Form = Form,
            CurrentLevel = LevelMin,
            MetLocation = Location,
            MetLevel = LevelMin,
            MetDate = EncounterDate.GetDateNDS(),
            Ball = (byte)FixedBall,

            ID32 = tr.ID32,
            Version = version,
            Language = language,
            OriginalTrainerGender = tr.Gender,
            OriginalTrainerName = tr.OT,

            OriginalTrainerFriendship = pi.BaseFriendship,

            Nickname = SpeciesName.GetSpeciesNameGeneration(Species, language, Generation),
        };

        EncounterUtil.SetEncounterMoves(pk, version, LevelMin);

        SetPINGA(pk, criteria, pi);
        pk.ResetPartyStats();

        return pk;
    }

    private void SetPINGA(PK5 pk, in EncounterCriteria criteria, PersonalInfo5B2W2 pi)
    {
        var abilityIndex = criteria.GetAbilityFromNumber(Ability);
        var seed = Util.Rand32();
        MonochromeRNG.Generate(pk, criteria with { Shiny = Shiny.Never }, pi.Gender, seed, abilityIndex);

        pk.Nature = criteria.GetNature();
        pk.RefreshAbility(abilityIndex);
        criteria.SetRandomIVs(pk);
    }

    #endregion

    #region Matching

    public bool IsMatchExact(PKM pk, EvoCriteria evo)
    {
        if (!IsMatchEggLocation(pk))
            return false;
        if (pk.MetLocation != Location)
            return false;
        if (!IsMatchLevel(pk.MetLevel))
            return false;
        if (Form != evo.Form && !FormInfo.IsFormChangeable(Species, Form, pk.Form, Context, pk.Context))
            return false;
        return true;
    }

    private static bool IsMatchLevel(byte metLevel)
    {
        // Level from 5->40 depends on the number of badges
        if (metLevel % 5 != 0)
            return false; // must be a multiple of 5
        return metLevel - 5u <= 35; // 5 <= x <= 40
    }

    private bool IsMatchEggLocation(PKM pk)
    {
        var expect = pk is PB8 ? Locations.Default8bNone : EggLocation;
        return pk.EggLocation == expect;
    }

    public EncounterMatchRating GetMatchRating(PKM pk) => EncounterMatchRating.Match;

    #endregion
}
