namespace PKHeX.Core;

/// <summary>
/// Generation 3 Static Encounter
/// </summary>
public sealed record EncounterStatic3XD(ushort Species, byte Level)
    : IEncounterable, IEncounterMatch, IEncounterConvertible<XK3>, IFatefulEncounterReadOnly, IRandomCorrelation, IMoveset
{
    public int Generation => 3;
    public EntityContext Context => EntityContext.Gen3;
    public GameVersion Version => GameVersion.XD;
    int ILocation.EggLocation => 0;
    int ILocation.Location => Location;
    public bool IsShiny => false;
    private bool Gift => FixedBall == Ball.Poke;
    public Shiny Shiny => Shiny.Random;
    public AbilityPermission Ability => AbilityPermission.Any12;

    public Ball FixedBall { get; init; }
    public bool FatefulEncounter { get; init; }

    public required byte Location { get; init; }
    public byte Form => 0;
    public bool EggEncounter => false;
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
        int lang = GetTemplateLanguage(tr);
        var pk = new XK3
        {
            Species = Species,
            CurrentLevel = LevelMin,
            OT_Friendship = PersonalTable.E[Species].BaseFriendship,

            Met_Location = Location,
            Met_Level = LevelMin,
            Version = (byte)GameVersion.CXD,
            Ball = (byte)(FixedBall != Ball.None ? FixedBall : Ball.Poke),
            FatefulEncounter = FatefulEncounter,

            Language = lang,
            OT_Name = tr.Language == lang ? tr.OT : lang == 1 ? "ゲーフリ" : "GF",
            OT_Gender = tr.Gender,
            ID32 = tr.ID32,
            Nickname = SpeciesName.GetSpeciesNameGeneration(Species, lang, Generation),
        };

        SetPINGA(pk, criteria);
        if (Moves.HasMoves)
            pk.SetMoves(Moves);
        else
            EncounterUtil1.SetEncounterMoves(pk, Version, Level);

        pk.ResetPartyStats();
        return pk;
    }

    private int GetTemplateLanguage(ITrainerInfo tr) => (int)Language.GetSafeLanguage(Generation, (LanguageID)tr.Language);

    private void SetPINGA(XK3 pk, EncounterCriteria criteria)
    {
        var pi = pk.PersonalInfo;
        int gender = criteria.GetGender(-1, pi);
        int nature = (int)criteria.GetNature(Nature.Random);
        var ability = criteria.GetAbilityFromNumber(Ability);
        if (Species == (int)Core.Species.Unown)
        {
            do
            {
                PIDGenerator.SetRandomWildPID4(pk, nature, ability, gender, PIDType.Method_1_Unown);
                ability ^= 1; // some nature-forms cannot have a certain PID-ability set, so just flip it as Unown doesn't have dual abilities.
            } while (pk.Form != Form);
        }
        else
        {
            const PIDType type = PIDType.CXD;
            do
            {
                PIDGenerator.SetRandomWildPID4(pk, nature, ability, gender, type);
            } while (Shiny == Shiny.Never && pk.IsShiny);
        }
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
        return pk.Egg_Location == expect;
    }

    private bool IsMatchLevel(PKM pk, EvoCriteria evo)
    {
        if (pk.Format != 3) // Met Level lost on PK3=>PK4
            return evo.LevelMax >= Level;
        return pk.Met_Level == Level;
    }

    private bool IsMatchLocation(PKM pk)
    {
        if (pk.Format != 3)
            return true; // transfer location verified later

        var met = pk.Met_Location;
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
