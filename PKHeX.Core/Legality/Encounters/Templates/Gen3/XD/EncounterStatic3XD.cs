namespace PKHeX.Core;

/// <summary>
/// Generation 3 Static Encounter
/// </summary>
public sealed record EncounterStatic3XD(ushort Species, byte Level)
    : IEncounterable, IEncounterMatch, IEncounterConvertible<XK3>, IFatefulEncounterReadOnly, IRandomCorrelation, IMoveset
{
    public byte Generation => 3;
    public EntityContext Context => EntityContext.Gen3;
    public GameVersion Version => GameVersion.XD;
    ushort ILocation.EggLocation => 0;
    ushort ILocation.Location => Location;
    public bool IsShiny => false;
    private bool Gift => FixedBall == Ball.Poke;
    public Shiny Shiny => Shiny.Random;
    public AbilityPermission Ability => AbilityPermission.Any12;

    public Ball FixedBall { get; init; }
    public bool FatefulEncounter { get; init; }

    public required byte Location { get; init; }
    public byte Form => 0;
    public bool IsEgg => false;
    public Moveset Moves { get; init; }

    public string Name => "Static Encounter";
    public string LongName => Name;
    public byte LevelMin => Level;
    public byte LevelMax => Level;

    public bool IsColoStarter => Species is (ushort)Core.Species.Espeon or (ushort)Core.Species.Umbreon;

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

            MetLocation = Location,
            MetLevel = LevelMin,
            Version = GameVersion.CXD,
            Ball = (byte)(FixedBall != Ball.None ? FixedBall : Ball.Poke),
            FatefulEncounter = FatefulEncounter,

            Language = lang,
            OriginalTrainerName = tr.OT,
            OriginalTrainerGender = 0,
            ID32 = tr.ID32,
            Nickname = SpeciesName.GetSpeciesNameGeneration(Species, lang, Generation),
        };

        SetPINGA(pk, criteria, pi);
        if (Moves.HasMoves)
            pk.SetMoves(Moves);
        else
            EncounterUtil.SetEncounterMoves(pk, Version, Level);

        pk.ResetPartyStats();
        return pk;
    }

    private void SetPINGA(XK3 pk, EncounterCriteria criteria, PersonalInfo3 pi)
    {
        var gender = criteria.GetGender(pi);
        var nature = criteria.GetNature();
        var ability = criteria.GetAbilityFromNumber(Ability);
        do
        {
            PIDGenerator.SetRandomWildPID4(pk, nature, ability, gender, PIDType.CXD);
        } while (Shiny == Shiny.Never && pk.IsShiny);
    }
    #endregion

    #region Matching
    public bool IsMatchExact(PKM pk, EvoCriteria evo)
    {
        if (!IsMatchEggLocation(pk))
            return false;
        if (!IsMatchLocation(pk))
            return false;
        if (!IsMatchLevel(pk, evo))
            return false;
        if (Form != evo.Form && !FormInfo.IsFormChangeable(Species, Form, pk.Form, Context, pk.Context))
            return false;
        return true;
    }

    public EncounterMatchRating GetMatchRating(PKM pk)
    {
        if (IsMatchPartial(pk))
            return EncounterMatchRating.PartialMatch;
        return EncounterMatchRating.Match;
    }

    private static bool IsMatchEggLocation(PKM pk)
    {
        if (pk.Format == 3)
            return true;

        var expect = pk is PB8 ? Locations.Default8bNone : 0;
        return pk.EggLocation == expect;
    }

    private bool IsMatchLevel(PKM pk, EvoCriteria evo)
    {
        if (pk.Format != 3) // Met Level lost on PK3=>PK4
            return evo.LevelMax >= Level;
        return pk.MetLevel == Level;
    }

    private bool IsMatchLocation(PKM pk)
    {
        if (pk.Format != 3)
            return true; // transfer location verified later

        var met = pk.MetLocation;
        return Location == met;
    }

    private bool IsMatchPartial(PKM pk)
    {
        if (Gift && pk.Ball != (byte)FixedBall)
            return true;
        return false;
    }
    #endregion

    public bool IsCompatible(PIDType val, PKM pk)
    {
        if (IsColoStarter)
            return val is PIDType.CXD_ColoStarter;
        if (val is PIDType.CXD)
            return true;
        return val is PIDType.CXDAnti && FatefulEncounter;
    }

    public PIDType GetSuggestedCorrelation() => PIDType.CXD;
}
