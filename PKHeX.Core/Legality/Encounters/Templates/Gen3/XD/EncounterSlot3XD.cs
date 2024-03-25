namespace PKHeX.Core;

/// <summary>
/// Encounter Slot found in <see cref="GameVersion.XD"/>.
/// </summary>
public sealed record EncounterSlot3XD(EncounterArea3XD Parent, ushort Species, byte LevelMin, byte LevelMax, byte SlotNumber)
    : IEncounterable, IEncounterMatch, IEncounterConvertible<XK3>, INumberedSlot, IFatefulEncounterReadOnly, IRandomCorrelation
{
    public byte Generation => 3;
    public EntityContext Context => EntityContext.Gen3;
    public bool FatefulEncounter => true;
    public bool IsEgg => false;
    public Ball FixedBall => Ball.None;
    public AbilityPermission Ability => AbilityPermission.Any12;
    public Shiny Shiny => Shiny.Random;
    public bool IsShiny => false;
    public ushort EggLocation => 0;

    public byte Form => 0;

    public string Name => $"Wild Encounter ({Version})";
    public string LongName => $"{Name} - Cave Spot";
    public GameVersion Version => Parent.Version;
    public ushort Location => Parent.Location;

    #region Generating
    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria) => ConvertToPKM(tr, criteria);
    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr);
    public XK3 ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr, EncounterCriteria.Unrestricted);

    public XK3 ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria)
    {
        int lang = (int)Language.GetSafeLanguage(Generation, (LanguageID)tr.Language);
        var pi = PersonalTable.E[Species];
        var pk = new XK3
        {
            Species = Species,
            CurrentLevel = LevelMin,
            OriginalTrainerFriendship = pi.BaseFriendship,
            FatefulEncounter = FatefulEncounter,
            MetLocation = Location,
            MetLevel = LevelMin,
            Version = GameVersion.CXD,
            Ball = (byte)Ball.Poke,

            Language = lang,
            OriginalTrainerName = tr.OT,
            OriginalTrainerGender = 0,
            ID32 = tr.ID32,
            Nickname = SpeciesName.GetSpeciesNameGeneration(Species, lang, Generation),
        };

        SetPINGA(pk, criteria, pi);
        EncounterUtil.SetEncounterMoves(pk, GameVersion.XD, LevelMin);

        pk.ResetPartyStats();
        return pk;
    }

    private void SetPINGA(XK3 pk, EncounterCriteria criteria, PersonalInfo3 pi)
    {
        var gender = criteria.GetGender(pi);
        var nature = criteria.GetNature();
        int ability = criteria.GetAbilityFromNumber(Ability);
        PIDGenerator.SetRandomPokeSpotPID(pk, nature, gender, ability, SlotNumber);
    }

    #endregion

    #region Matching
    public bool IsMatchExact(PKM pk, EvoCriteria evo) => true; // Handled by Area
    public EncounterMatchRating GetMatchRating(PKM pk) => EncounterMatchRating.Match;
    #endregion

    public bool IsCompatible(PIDType val, PKM pk) => val == PIDType.PokeSpot;
    public PIDType GetSuggestedCorrelation() => PIDType.PokeSpot;
}
