namespace PKHeX.Core;

/// <summary>
/// Generation 5 Entrée Forest static encounter
/// </summary>
public sealed record EncounterStatic5Entree(GameVersion Version, ushort Species, byte Level, byte Form, byte Gender, AbilityPermission Ability, GlobalLinkPromotion Promotion)
    : IEncounterable, IEncounterMatch, IEncounterConvertible<PK5>, IMoveset, IFixedGender
{
    public byte Generation => 5;
    public EntityContext Context => EntityContext.Gen5;
    public Ball FixedBall => Ball.None;
    public Shiny Shiny => Shiny.Never;
    public bool IsShiny => false;
    public bool IsEgg => false;
    ushort ILocation.EggLocation => 0;
    public ushort Location => 075;
    public string Name => $"Entree Forest Encounter ({Version})";
    public string LongName => Name;
    public byte LevelMin => Level;
    public byte LevelMax => Level;

    public Moveset Moves { get; }

    public EncounterStatic5Entree(GameVersion version, ushort species, byte level, byte form, byte gender, AbilityPermission ability, ushort Move, GlobalLinkPromotion promotion)
        : this(version, species, level, form, gender, ability, promotion) => Moves = new Moveset(Move);

    #region Generating

    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr);
    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria) => ConvertToPKM(tr, criteria);
    public PK5 ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr, EncounterCriteria.Unrestricted);

    public PK5 ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria)
    {
        var language = (int)Promotion.GetSafeLanguage((LanguageID)tr.Language);
        var version = this.GetCompatibleVersion(tr.Version);
        var trainerName = EncounterUtil.GetTrainerName(tr, language); // in case it is remapped by PGL promotion restriction
        var pi = PersonalTable.B2W2[Species];
        var pk = new PK5
        {
            Species = Species,
            Form = Form,
            CurrentLevel = Level,
            MetLocation = Location,
            MetLevel = Level,
            MetDate = EncounterDate.GetDateNDS(),
            Ball = (byte)Ball.Dream,

            ID32 = tr.ID32,
            Version = version,
            Language = language,
            OriginalTrainerGender = tr.Gender,
            OriginalTrainerName = trainerName,

            OriginalTrainerFriendship = pi.BaseFriendship,

            Nickname = SpeciesName.GetSpeciesNameGeneration(Species, language, Generation),
        };

        if (Moves.HasMoves)
            pk.SetMoves(Moves);
        else
            EncounterUtil.SetEncounterMoves(pk, version, Level);

        SetPINGA(pk, criteria, pi);
        pk.ResetPartyStats();

        return pk;
    }

    private void SetPINGA(PK5 pk, in EncounterCriteria criteria, PersonalInfo5B2W2 pi)
    {
        var seed = Util.Rand.Rand64();
        MonochromeRNG.Generate(pk, criteria, pi.Gender, seed, false, Shiny, Ability, Gender);

        pk.Nature = criteria.GetNature();
        var abilityIndex = criteria.GetAbilityFromNumber(Ability); // 0 or H
        pk.RefreshAbility(abilityIndex);
        criteria.SetRandomIVs(pk);
    }

    #endregion

    #region Matching

    public bool IsMatchExact(PKM pk, EvoCriteria evo)
    {
        if (!this.IsMatchEggLocation(pk))
            return false;
        if (pk.MetLocation != Location)
            return false;
        if (pk.MetLevel != Level)
            return false;
        if (Gender != FixedGenderUtil.GenderRandom && pk.Gender != Gender)
            return false;
        if (Form != evo.Form && !FormInfo.IsFormChangeable(Species, Form, pk.Form, Context, pk.Context))
            return false;
        if (!Promotion.CanBeReceivedBy((LanguageID)pk.Language))
            return false;
        return true;
    }

    public EncounterMatchRating GetMatchRating(PKM pk) => EncounterMatchRating.Match;

    #endregion
}
