using System;

namespace PKHeX.Core;

/// <summary>
/// Shadow Pokémon Encounter found in <see cref="GameVersion.CXD"/>
/// </summary>
/// <param name="ID">Initial Shadow Gauge value.</param>
/// <param name="Gauge">Initial Shadow Gauge value.</param>
/// <param name="PartyPrior">Team Specification with required <see cref="Species"/>, <see cref="Nature"/> and Gender.</param>
// ReSharper disable NotAccessedPositionalProperty.Global
public sealed record EncounterShadow3XD(byte ID, short Gauge, ReadOnlyMemory<TeamLock> PartyPrior)
    : IEncounterable, IEncounterMatch, IEncounterConvertible<XK3>, IShadow3, IFatefulEncounterReadOnly, IMoveset, IRandomCorrelation
{
    // ReSharper restore NotAccessedPositionalProperty.Global
    public byte Generation => 3;
    public EntityContext Context => EntityContext.Gen3;
    public GameVersion Version => GameVersion.XD;
    ushort ILocation.EggLocation => 0;
    ushort ILocation.Location => Location;
    public bool IsShiny => false;
    public bool IsEgg => false;
    public Shiny Shiny => Shiny.Never; // Different from Colosseum!
    public AbilityPermission Ability => AbilityPermission.Any12;
    public bool FatefulEncounter => true;
    public byte Form => 0;

    public required ushort Species { get; init; }
    public required byte Level { get; init; }
    public required byte Location { get; init; }
    public Ball FixedBall { get; init; } = Ball.None;
    public required Moveset Moves { get; init; }

    public string Name => "Shadow Encounter";
    public string LongName => Name;
    public byte LevelMin => Level;
    public byte LevelMax => Level;

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

            // Fake as Purified
            RibbonNational = true,
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
        int ability = criteria.GetAbilityFromNumber(Ability);

        // Ensure that any generated specimen has valid Shadow Locks
        // This can be kinda slow, depending on how many locks / how strict they are.
        // Cancel this operation if too many attempts are made to prevent infinite loops.
        int ctr = 0;
        const int max = 100_000;
        do
        {
            PIDGenerator.SetRandomWildPID4(pk, nature, ability, gender, PIDType.CXD);
            var pidiv = MethodFinder.Analyze(pk);
            var result = LockFinder.IsAllShadowLockValid(this, pidiv, pk);
            if (result)
                break;
        }
        while (++ctr <= max);

        System.Diagnostics.Debug.Assert(ctr < max);
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

    private bool IsMatchPartial(PKM pk)
    {
        if (!pk.FatefulEncounter)
            return true;
        return FixedBall != Ball.None && pk.Ball != (byte)FixedBall;
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
        if (met == Location)
            return true;

        // XD can re-battle with Miror B
        // Realgam Tower, Rock, Oasis, Cave, Pyrite Town
        return Version == GameVersion.XD && met is (59 or 90 or 91 or 92 or 113);
    }

    #endregion

    public bool IsCompatible(PIDType val, PKM pk) => val is PIDType.CXD or PIDType.CXDAnti;
    public PIDType GetSuggestedCorrelation() => PIDType.CXD;
}
