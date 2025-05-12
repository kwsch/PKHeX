using static PKHeX.Core.RandomCorrelationRating;

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
    public string LongName => $"{Name} - {Parent.Type} Spot";
    public GameVersion Version => Parent.Version;
    public ushort Location => Parent.Location;

    #region Generating
    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria) => ConvertToPKM(tr, criteria);
    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr);
    public XK3 ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr, EncounterCriteria.Unrestricted);

    public XK3 ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria)
    {
        int language = (int)Language.GetSafeLanguage(Generation, (LanguageID)tr.Language);
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

            Language = language,
            OriginalTrainerName = tr.OT,
            OriginalTrainerGender = 0,
            ID32 = tr.ID32,
            Nickname = SpeciesName.GetSpeciesNameGeneration(Species, language, Generation),
        };

        SetPINGA(pk, criteria, pi);
        EncounterUtil.SetEncounterMoves(pk, GameVersion.E, LevelMin);

        pk.ResetPartyStats();
        return pk;
    }

    private void SetPINGA(XK3 pk, EncounterCriteria criteria, PersonalInfo3 pi)
    {
        MethodPokeSpot.SetRandomPID(pk, criteria, pi.Gender, SlotNumber);
        if (criteria.IsSpecifiedIVsAll() && !MethodPokeSpot.TrySetIVs(pk, criteria, LevelMin, LevelMax))
            return;
        MethodPokeSpot.SetRandomIVs(pk, criteria, LevelMin, LevelMax, Util.Rand32());
    }

    #endregion

    #region Matching
    public bool IsMatchExact(PKM pk, EvoCriteria evo) => true; // Handled by Area
    public EncounterMatchRating GetMatchRating(PKM pk) => EncounterMatchRating.Match;
    #endregion

    public RandomCorrelationRating IsCompatible(PIDType type, PKM pk) => type is PIDType.PokeSpot ? Match : Mismatch;
    public PIDType GetSuggestedCorrelation() => PIDType.PokeSpot;
}
