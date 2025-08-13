using static PKHeX.Core.RandomCorrelationRating;

namespace PKHeX.Core;

/// <summary>
/// Generation 3 Static Encounter
/// </summary>
public sealed record EncounterStarter3Colo(ushort Species, byte Level)
    : IEncounterable, IEncounterMatch, IEncounterConvertible<CK3>, IFixedGender, IRandomCorrelation, IMoveset
{
    public byte Generation => 3;
    public EntityContext Context => EntityContext.Gen3;
    public GameVersion Version => GameVersion.COLO;
    ushort ILocation.EggLocation => 0;
    ushort ILocation.Location => Location;
    public bool IsShiny => false;
    public Shiny Shiny => Shiny.Never;
    public byte Form => 0;
    public bool IsEgg => false;

    public AbilityPermission Ability => AbilityPermission.Any12;

    public Ball FixedBall => Ball.Poke;
    public byte Location => 254;
    public byte Gender => 0;
    public required Moveset Moves { get; init; }

    public string Name => "Static Encounter";
    public string LongName => Name;
    public byte LevelMin => Level;
    public byte LevelMax => Level;

    #region Generating
    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria) => ConvertToPKM(tr, criteria);
    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr);
    public CK3 ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr, EncounterCriteria.Unrestricted);

    public CK3 ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria)
    {
        int lang = GetTemplateLanguage(tr);
        var pi = PersonalTable.E[Species];
        var pk = new CK3
        {
            Species = Species,
            CurrentLevel = Level,
            OriginalTrainerFriendship = pi.BaseFriendship,

            MetLocation = Location,
            MetLevel = Level,
            Version = GameVersion.CXD,
            Ball = (byte)Ball.Poke,

            Language = lang,
            OriginalTrainerName = tr.OT,
            OriginalTrainerGender = 0,
            ID32 = tr.ID32,
            Nickname = SpeciesName.GetSpeciesNameGeneration(Species, lang, Generation),
        };

        SetPINGA(pk, criteria);
        if (Moves.HasMoves)
            pk.SetMoves(Moves);
        else
            EncounterUtil.SetEncounterMoves(pk, Version, Level);

        pk.ResetPartyStats();
        return pk;
    }

    private int GetTemplateLanguage(ITrainerInfo tr) => (int)Language.GetSafeLanguage(Generation, (LanguageID)tr.Language);

    private void SetPINGA(CK3 pk, EncounterCriteria criteria)
    {
        // Prefer IVs if requested, rather than Trainer Matching.
        if (criteria.IsSpecifiedIVsAll())
        {
            bool valid = Species is (ushort)Core.Species.Umbreon
                ? MethodCXD.SetStarterFirstFromIVs(pk, criteria)
                : MethodCXD.SetStarterSecondFromIVs(pk, criteria);
            if (valid)
                return;
        }
        // Fall back to Trainer ID matching.
        if (MethodCXD.SetStarterFromTrainerID(pk, criteria, pk.TID16, pk.SID16))
            return;
        // Fall back to generating a random PID.
        criteria = EncounterCriteria.Unrestricted;
        if (MethodCXD.SetStarterFromTrainerID(pk, criteria, pk.TID16, pk.SID16))
            return;
        // TID/SID impossible? Just generate anything.
        _ = Species is (ushort)Core.Species.Umbreon
            ? MethodCXD.SetStarterFirstFromIVs(pk, criteria)
            : MethodCXD.SetStarterSecondFromIVs(pk, criteria);
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
        if (pk.Gender != Gender)
            return false;
        if (Form != evo.Form && !FormInfo.IsFormChangeable(Species, Form, pk.Form, Context, pk.Context))
            return false;
        return true;
    }

    public EncounterMatchRating GetMatchRating(PKM pk)
    {
        if (IsMatchPartial(pk))
            return EncounterMatchRating.PartialMatch;
        if (pk.FatefulEncounter) // Clash with XD's starter Eevee
            return EncounterMatchRating.DeferredErrors;
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
        if (pk.Ball != (byte)FixedBall)
            return true;
        return false;
    }
    #endregion

    public RandomCorrelationRating IsCompatible(PIDType type, PKM pk) => type is PIDType.CXD_ColoStarter ? Match : Mismatch;

    public PIDType GetSuggestedCorrelation() => PIDType.CXD_ColoStarter;
}
