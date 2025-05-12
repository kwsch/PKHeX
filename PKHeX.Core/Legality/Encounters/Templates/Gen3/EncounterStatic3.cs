using System;
using static PKHeX.Core.RandomCorrelationRating;

namespace PKHeX.Core;

/// <summary>
/// Generation 3 Static Encounter
/// </summary>
public sealed record EncounterStatic3(ushort Species, byte Level, GameVersion Version)
    : IEncounterable, IEncounterMatch, IEncounterConvertible<PK3>, IFatefulEncounterReadOnly, IRandomCorrelation, IMoveset
{
    public byte Generation => 3;
    public EntityContext Context => EntityContext.Gen3;
    public bool IsRoaming { get; init; }
    public bool IsRoamingTruncatedIVs => IsRoaming && Version != GameVersion.E;
    ushort ILocation.EggLocation => 0;
    ushort ILocation.Location => Location;
    public bool IsShiny => false;
    private bool Gift => FixedBall == Ball.Poke;
    public Shiny Shiny => Shiny.Random;

    public AbilityPermission Ability => AbilityPermission.Any12;

    public Ball FixedBall { get; init; }
    public bool FatefulEncounter { get; init; }

    public required byte Location { get; init; }
    public byte Form { get; init; }
    public bool IsEgg { get; init; }
    public Moveset Moves { get; init; }

    public string Name => "Static Encounter";
    public string LongName => Name;
    public byte LevelMin => Level;
    public byte LevelMax => Level;

    #region Generating
    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria) => ConvertToPKM(tr, criteria);
    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr);
    public PK3 ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr, EncounterCriteria.Unrestricted);

    public PK3 ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria)
    {
        int lang = GetTemplateLanguage(tr);
        var version = this.GetCompatibleVersion(tr.Version);
        var pi = PersonalTable.E[Species];
        var pk = new PK3
        {
            Species = Species,
            CurrentLevel = LevelMin,
            OriginalTrainerFriendship = pi.BaseFriendship,

            MetLocation = Location,
            MetLevel = LevelMin,
            Version = version,
            Ball = (byte)(FixedBall != Ball.None ? FixedBall : Ball.Poke),
            FatefulEncounter = FatefulEncounter,

            Language = lang,
            OriginalTrainerName = EncounterUtil.GetTrainerName(tr, lang),
            OriginalTrainerGender = tr.Gender,
            ID32 = tr.ID32,
            Nickname = SpeciesName.GetSpeciesNameGeneration(Species, lang, Generation),
        };

        if (IsEgg)
        {
            // Fake as hatched.
            pk.MetLevel = EggStateLegality.EggMetLevel34;
            pk.MetLocation = version is GameVersion.FR or GameVersion.LG
                ? Locations.HatchLocationFRLG
                : Locations.HatchLocationRSE;
        }

        SetPINGA(pk, criteria, pi);
        if (Moves.HasMoves)
            pk.SetMoves(Moves);
        else
            EncounterUtil.SetEncounterMoves(pk, Version, LevelMin);

        pk.ResetPartyStats();
        return pk;
    }

    private int GetTemplateLanguage(ITrainerInfo tr)
    {
        // Old Sea Map was only distributed to Japanese games.
        if (Species is (ushort)Core.Species.Mew)
            return (int)LanguageID.Japanese;

        // Deoxys for Emerald was not available for Japanese games.
        if (Species is (ushort)Core.Species.Deoxys && tr.Language == 1)
            return (int)LanguageID.English;

        return (int)Language.GetSafeLanguage(Generation, (LanguageID)tr.Language);
    }

    private void SetPINGA(PK3 pk, EncounterCriteria criteria, PersonalInfo3 pi)
    {
        var gr = pi.Gender;
        bool truncate = IsRoamingTruncatedIVs;
        if (criteria.IsSpecifiedIVsAll() && !truncate)
        {
            if (TrySetMethod1(pk, criteria, gr))
                return;
        }
        SetMethod1(pk, criteria, gr, Util.Rand32());
        if (truncate) // Never happens for eggs or dual-ability encounters.
            pk.IV32 &= 0xFF;
    }

    private static bool TrySetMethod1(PK3 pk, EncounterCriteria criteria, byte gr)
    {
        criteria.GetCombinedIVs(out var iv1, out var iv2);

        Span<uint> seeds = stackalloc uint[LCRNG.MaxCountSeedsIV];
        var count = LCRNGReversal.GetSeedsIVs(seeds, iv1 << 16, iv2 << 16);
        foreach (var s in seeds[..count])
        {
            var seed = LCRNG.Prev2(s); // Unwind the RNG to get the real origin seed for the PID/IV
            var pid = ClassicEraRNG.GetSequentialPID(seed);
            if (criteria.IsSpecifiedNature() && !criteria.IsSatisfiedNature((Nature)(pid % 25)))
                continue;

            var gender = EntityGender.GetFromPIDAndRatio(pid, gr);
            if (criteria.IsSpecifiedGender() && !criteria.IsSatisfiedGender(gender))
                continue;

            var abit = (int)(pid & 1);
            if (criteria.IsSpecifiedAbility() && !criteria.IsSatisfiedAbility(abit))
                continue;

            pk.PID = pid;
            pk.IV32 |= iv2 << 15 | iv1;
            pk.Gender = gender;
            pk.RefreshAbility(abit);
            return true;
        }

        return false;
    }

    private static void SetMethod1(PK3 pk, in EncounterCriteria criteria, byte gr, uint seed)
    {
        var id32 = pk.ID32;
        while (true)
        {
            var pid = ClassicEraRNG.GetSequentialPID(ref seed);
            var shiny = ShinyUtil.GetIsShiny(id32, pid, 8);
            if (criteria.Shiny.IsShiny() != shiny)
                continue;

            if (criteria.IsSpecifiedNature() && !criteria.IsSatisfiedNature((Nature)(pid % 25)))
                continue;

            var gender = EntityGender.GetFromPIDAndRatio(pid, gr);
            if (criteria.IsSpecifiedGender() && !criteria.IsSatisfiedGender(gender))
                continue;

            var abit = (int)(pid & 1);
            if (criteria.IsSpecifiedAbility() && !criteria.IsSatisfiedAbility(abit))
                continue;

            var iv32 = ClassicEraRNG.GetSequentialIVs(ref seed);
            if (criteria.IsSpecifiedHiddenPower() && !criteria.IsSatisfiedHiddenPower(iv32))
                continue;

            pk.PID = pid;
            pk.IV32 |= iv32;
            pk.Gender = gender;
            pk.RefreshAbility(abit);
            break;
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

    private bool IsDeferredSafari3(bool isSafariBall) => isSafariBall != Locations.IsSafariZoneLocation3(Location);

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
        if (!IsEgg)
            return pk.MetLevel == Level;
        return pk is { MetLevel: EggStateLegality.EggMetLevel34, CurrentLevel: >= 5 }; // met level 0, origin level 5
    }

    private bool IsMatchLocation(PKM pk)
    {
        if (pk.Format != 3)
            return true; // transfer location verified later

        if (IsEgg)
            return !pk.IsEgg || pk.MetLocation == Location;

        var met = pk.MetLocation;
        if (!IsRoaming)
            return Location == met;

        // Route 101-138
        if (Version <= GameVersion.E)
            return met is >= 16 and <= 49;
        // Route 1-25 encounter is possible either in grass or on water
        return met is >= 101 and <= 125;
    }

    private bool IsMatchPartial(PKM pk)
    {
        if (IsDeferredSafari3(pk.Ball == (int)Ball.Safari))
            return true;
        if (Gift && pk.Ball != (byte)FixedBall)
            return true;
        if (FatefulEncounter != pk.FatefulEncounter)
            return true;
        return false;
    }
    #endregion

    public RandomCorrelationRating IsCompatible(PIDType type, PKM pk)
    {
        var version = pk.Version;
        if (version is GameVersion.E)
            return type is PIDType.Method_1 ? Match : Mismatch;

        if (IsRoaming) // Glitched IVs
            return IsRoamerPIDIV(type, pk) ? Match : Mismatch;

        if (type is PIDType.Method_1)
            return Match;
        // RS: Only Method 1, but RSBox s/w emulation can yield Method 4.
        if (version is GameVersion.R or GameVersion.S)
            return type is PIDType.Method_4 ? NotIdeal : Mismatch;
        // FR/LG: Only Method 1, but Togepi gift can be Method 4 via PID modulo VBlank abuse
        return type is PIDType.Method_4 && Species is (ushort)Core.Species.Togepi ? NotIdeal : Mismatch;
    }

    private static bool IsRoamerPIDIV(PIDType val, PKM pk)
    {
        // Roamer PID/IV is always Method 1.
        // M1 is checked before M1R. A M1R PID/IV can also be a M1 PID/IV, so check that collision.
        if (PIDType.Method_1_Roamer == val)
            return true;
        if (PIDType.Method_1 != val)
            return false;

        // only 8 bits are stored instead of 32 -- 5 bits HP, 3 bits for ATK.
        // return pk.IV32 <= 0xFF; -- not always in right order, and can have nickname flagged.
        var ivs = pk.GetIVs();
        return ivs <= 0xFF;
    }

    public PIDType GetSuggestedCorrelation() => PIDType.Method_1;
}
