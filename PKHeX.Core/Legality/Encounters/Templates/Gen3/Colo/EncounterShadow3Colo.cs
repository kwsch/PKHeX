using System;

namespace PKHeX.Core;

/// <summary>
/// Shadow Pokémon Encounter found in <see cref="GameVersion.CXD"/>
/// </summary>
/// <param name="ID">Initial Shadow Gauge value.</param>
/// <param name="Gauge">Initial Shadow Gauge value.</param>
/// <param name="PartyPrior">Team Specification with required <see cref="Species"/>, <see cref="Nature"/> and Gender.</param>
// ReSharper disable NotAccessedPositionalProperty.Global
public sealed record EncounterShadow3Colo(byte ID, short Gauge, ReadOnlyMemory<TeamLock> PartyPrior)
    : IEncounterable, IEncounterMatch, IEncounterConvertible<CK3>, IShadow3, IMoveset, IRandomCorrelation
{
    // ReSharper restore NotAccessedPositionalProperty.Global
    public byte Generation => 3;
    public EntityContext Context => EntityContext.Gen3;
    public GameVersion Version => GameVersion.COLO;
    ushort ILocation.EggLocation => 0;
    ushort ILocation.Location => Location;
    public bool IsShiny => false;
    public bool IsEgg => false;
    public Shiny Shiny => Shiny.Random;
    public AbilityPermission Ability => AbilityPermission.Any12;
    public Ball FixedBall => Ball.None;
    public byte Form => 0;

    public required ushort Species { get; init; }
    public required byte Level { get; init; }
    public required byte Location { get; init; }
    public required Moveset Moves { get; init; }

    public string Name => "Shadow Encounter";
    public string LongName => Name;
    public byte LevelMin => Level;
    public byte LevelMax => Level;

    /// <summary>
    /// Originates from the EReader scans (Japanese Only)
    /// </summary>
    public bool IsEReader => Location == 128; // @ Card e Room (Japanese games only)

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
            CurrentLevel = LevelMin,
            OriginalTrainerFriendship = pi.BaseFriendship,

            MetLocation = Location,
            MetLevel = LevelMin,
            Version = GameVersion.CXD,
            Ball = (byte)Ball.Poke,

            Language = lang,
            OriginalTrainerName = EncounterUtil.GetTrainerName(tr, lang),
            OriginalTrainerGender = 0,
            ID32 = tr.ID32,
            Nickname = SpeciesName.GetSpeciesNameGeneration(Species, lang, Generation),

            // Fake as Purified
            RibbonNational = true,
        };

        SetPINGA(pk, criteria, pi);
        pk.SetMoves(Moves);

        pk.ResetPartyStats();
        return pk;
    }

    private int GetTemplateLanguage(ITrainerInfo tr) => IsEReader ? 1 : (int)Language.GetSafeLanguage(Generation, (LanguageID)tr.Language);

    private void SetPINGA(CK3 pk, EncounterCriteria criteria, PersonalInfo3 pi)
    {
        if (!IsEReader)
            SetPINGA_Regular(pk, criteria, pi);
        else
            SetPINGA_EReader(pk);
    }

    private void SetPINGA_Regular(CK3 pk, EncounterCriteria criteria, PersonalInfo3 pi)
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

    private void SetPINGA_EReader(CK3 pk)
    {
        // E-Reader have all IVs == 0
        // Skip setting IVs.

        // All E-Reader shadows are actually nature/gender locked.
        var locked = PartyPrior.Span[0].Locks[^1];
        var (nature, gender) = locked.GetLock;

        // Ensure that any generated specimen has valid Shadow Locks
        // This can be kinda slow, depending on how many locks / how strict they are.
        // Cancel this operation if too many attempts are made to prevent infinite loops.
        int ctr = 0;
        const int max = 100_000;
        var rnd = Util.Rand;
        do
        {
            var seed = rnd.Rand32();
            PIDGenerator.SetValuesFromSeedXDRNG_EReader(pk, seed);
            if (pk.Nature != nature || pk.Gender != gender)
                continue;
            var pidiv = new PIDIV(PIDType.CXD, seed);
            var result = LockFinder.IsAllShadowLockValid(this, pidiv, pk);
            if (result)
                break;
        }
        while (++ctr <= max);

#if DEBUG
        System.Diagnostics.Debug.Assert(ctr < max);
#endif
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
        if (pk.FatefulEncounter)
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
        return pk.MetLocation == Location;
    }

    #endregion

    public bool IsCompatible(PIDType val, PKM pk)
    {
        if (IsEReader)
            return true;
        return val is PIDType.CXD;
    }

    public PIDType GetSuggestedCorrelation()
    {
        if (IsEReader)
            return PIDType.None;
        return PIDType.CXD;
    }
}
