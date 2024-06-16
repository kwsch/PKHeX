using System;
using static PKHeX.Core.StaticCorrelation8bRequirement;

namespace PKHeX.Core;

/// <summary>
/// Generation 8 Static Encounter
/// </summary>
public sealed record EncounterStatic8b(GameVersion Version)
    : IEncounterable, IEncounterMatch, IEncounterConvertible<PB8>, IFlawlessIVCount, IFatefulEncounterReadOnly, IStaticCorrelation8b
{
    public byte Generation => 8;
    public EntityContext Context => EntityContext.Gen8b;
    ushort ILocation.EggLocation => EggLocation;
    ushort ILocation.Location => Location;
    public bool IsEgg => EggLocation != None;
    private const ushort None = Locations.Default8bNone;
    public byte Form => 0;
    public bool IsShiny => Shiny == Shiny.Always;
    public byte LevelMin => Level;
    public byte LevelMax => Level;

    public required ushort Species { get; init; }
    public required ushort Location { get; init; }
    public ushort EggLocation { get; init; } = None;
    public required byte Level { get; init; }
    public Ball FixedBall { get; init; }
    public byte FlawlessIVCount { get; init; }
    public bool Roaming { get; init; }
    public AbilityPermission Ability { get; init; }
    public Shiny Shiny { get; init; }
    public bool FatefulEncounter { get; init; }

    public string Name => "Static Encounter";
    public string LongName => Name;

    public StaticCorrelation8bRequirement GetRequirement(PKM pk) => Roaming
        ? MustHave
        : MustNotHave;

    public bool IsStaticCorrelationCorrect(PKM pk)
    {
        return Roaming8bRNG.ValidateRoamingEncounter(pk, Shiny == Shiny.Random ? Shiny.FixedValue : Shiny, FlawlessIVCount);
    }

    // defined by mvpoke in encounter data
    private static ReadOnlySpan<ushort> RoamingLocations =>
    [
        197, 201, 354, 355, 356, 357, 358, 359, 361, 362, 364, 365, 367, 373, 375, 377,
        378, 379, 383, 385, 392, 394, 395, 397, 400, 403, 404, 407,
        485,
    ];

    #region Generating

    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr);
    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria) => ConvertToPKM(tr, criteria);

    public PB8 ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr, EncounterCriteria.Unrestricted);

    public PB8 ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria)
    {
        var version = this.GetCompatibleVersion(tr.Version);
        int lang = (int)Language.GetSafeLanguage(Generation, (LanguageID)tr.Language, version);
        var pi = PersonalTable.BDSP[Species, Form];
        var pk = new PB8
        {
            Species = Species,
            CurrentLevel = Level,
            MetLocation = Location,
            EggLocation = EggLocation,
            MetLevel = Level,
            MetDate = EncounterDate.GetDateSwitch(),
            Ball = (byte)(FixedBall != Ball.None ? FixedBall : Ball.Poke),
            FatefulEncounter = FatefulEncounter,

            ID32 = tr.ID32,
            Version = version,
            Language = lang,
            OriginalTrainerGender = tr.Gender,
            OriginalTrainerName = tr.OT,
            OriginalTrainerFriendship = pi.BaseFriendship,

            Nickname = SpeciesName.GetSpeciesNameGeneration(Species, lang, Generation),
        };

        if (IsEgg)
        {
            // Fake as hatched.
            pk.MetLocation = Locations.HatchLocation8b;
            pk.MetLevel = EggStateLegality.EggMetLevel;
            pk.EggLocation = EggLocation;
            pk.EggMetDate = pk.MetDate;
        }

        SetPINGA(pk, criteria);

        EncounterUtil.SetEncounterMoves(pk, version, Level);
        pk.ResetPartyStats();

        return pk;
    }

    private void SetPINGA(PB8 pk, EncounterCriteria criteria)
    {
        var req = GetRequirement(pk);
        if (req == MustHave) // Roamers
        {
            var shiny = Shiny == Shiny.Random ? Shiny.FixedValue : Shiny;
            Roaming8bRNG.ApplyDetails(pk, criteria, shiny, FlawlessIVCount);
        }
        else
        {
            var shiny = Shiny == Shiny.Never ? Shiny.Never : Shiny.Random;
            Wild8bRNG.ApplyDetails(pk, criteria, shiny, FlawlessIVCount, Ability);
        }
    }

    #endregion

    #region Matching

    public EncounterMatchRating GetMatchRating(PKM pk) => EncounterMatchRating.Match;

    public bool IsMatchExact(PKM pk, EvoCriteria evo)
    {
        if (pk.MetLevel != Level)
            return false;
        if (!IsMatchLocation(pk))
            return false;
        if (evo.Form != Form && !FormInfo.IsFormChangeable(Species, Form, pk.Form, Context, pk.Context))
            return false;
        if (FlawlessIVCount != 0 && pk.FlawlessIVCount < FlawlessIVCount)
            return false;
        if (!IsMatchEggLocation(pk))
            return false;
        return true;
    }

    private bool IsMatchLocation(PKM pk)
    {
        var metState = LocationsHOME.GetRemapState(Context, pk.Context);
        if (metState == LocationRemapState.Original)
            return IsMatchLocationExact(pk);
        if (metState == LocationRemapState.Remapped)
            return IsMatchLocationRemapped(pk);
        return IsMatchLocationExact(pk) || IsMatchLocationRemapped(pk);
    }

    private bool IsMatchLocationExact(PKM pk)
    {
        if (IsEgg)
            return !pk.IsEgg || pk.MetLocation == Location || pk.MetLocation == Locations.LinkTrade6NPC;
        if (!Roaming)
            return pk.MetLocation == Location;
        return IsRoamingLocation(pk);
    }

    private bool IsMatchEggLocationExact(PKM pk)
    {
        var eggLoc = pk.EggLocation;
        if (!IsEgg)
            return eggLoc == EggLocation;

        if (!pk.IsEgg) // hatched
            return eggLoc == EggLocation || eggLoc == Locations.LinkTrade6NPC;

        // Unhatched:
        if (eggLoc != EggLocation)
            return false;
        if (pk.MetLocation is not (Locations.Default8bNone or Locations.LinkTrade6NPC))
            return false;
        return true;
    }

    private bool IsMatchLocationRemapped(PKM pk)
    {
        var met = pk.MetLocation;
        var version = pk.Version;
        if (pk.Context == EntityContext.Gen8)
            return LocationsHOME.IsValidMetBDSP(met, version);
        return LocationsHOME.GetMetSWSH(Location, version) == met;
    }

    private bool IsMatchEggLocation(PKM pk)
    {
        var metState = LocationsHOME.GetRemapState(Context, pk.Context);
        if (metState == LocationRemapState.Original)
            return IsMatchEggLocationExact(pk);
        if (metState == LocationRemapState.Remapped)
            return IsMatchEggLocationRemapped(pk);
        // Either
        return IsMatchEggLocationExact(pk) || IsMatchEggLocationRemapped(pk);
    }

    private bool IsMatchEggLocationRemapped(PKM pk)
    {
        if (!IsEgg)
            return pk.EggLocation == 0;
        return LocationsHOME.IsLocationSWSHEgg(pk.Version, pk.MetLocation, pk.EggLocation, EggLocation);
    }

    private static bool IsRoamingLocation(PKM pk) => RoamingLocations.Contains(pk.MetLocation);

    #endregion
}
