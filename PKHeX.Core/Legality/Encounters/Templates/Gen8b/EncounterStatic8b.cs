using System;
using static PKHeX.Core.StaticCorrelation8bRequirement;

namespace PKHeX.Core;

/// <summary>
/// Generation 8 Static Encounter
/// </summary>
public sealed record EncounterStatic8b(GameVersion Version)
    : IEncounterable, IEncounterMatch, IEncounterConvertible<PB8>, IFlawlessIVCount, IFatefulEncounterReadOnly, IStaticCorrelation8b
{
    public int Generation => 8;
    public EntityContext Context => EntityContext.Gen8b;
    int ILocation.EggLocation => EggLocation;
    int ILocation.Location => Location;
    public bool EggEncounter => EggLocation != None;
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
    private static ReadOnlySpan<ushort> Roaming_MetLocation_BDSP =>
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
        var version = this.GetCompatibleVersion((GameVersion)tr.Game);
        int lang = (int)Language.GetSafeLanguage(Generation, (LanguageID)tr.Language, version);
        var pi = PersonalTable.BDSP[Species, Form];
        var pk = new PB8
        {
            Species = Species,
            CurrentLevel = Level,
            Met_Location = Location,
            Egg_Location = EggLocation,
            Met_Level = Level,
            MetDate = EncounterDate.GetDateSwitch(),
            Ball = (byte)(FixedBall != Ball.None ? FixedBall : Ball.Poke),
            FatefulEncounter = FatefulEncounter,

            ID32 = tr.ID32,
            Version = (byte)version,
            Language = lang,
            OT_Gender = tr.Gender,
            OT_Name = tr.OT,
            OT_Friendship = pi.BaseFriendship,

            Nickname = SpeciesName.GetSpeciesNameGeneration(Species, lang, Generation),
            HeightScalar = PokeSizeUtil.GetRandomScalar(),
            WeightScalar = PokeSizeUtil.GetRandomScalar(),
        };

        if (EggEncounter)
        {
            // Fake as hatched.
            pk.Met_Location = Locations.HatchLocation8b;
            pk.Met_Level = EggStateLegality.EggMetLevel;
            pk.Egg_Location = EggLocation;
            pk.EggMetDate = pk.MetDate;
        }

        SetPINGA(pk, criteria);

        EncounterUtil1.SetEncounterMoves(pk, version, Level);
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
        if (pk.Met_Level != Level)
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
        if (EggEncounter)
            return !pk.IsEgg || pk.Met_Location == Location || pk.Met_Location == Locations.LinkTrade6NPC;
        if (!Roaming)
            return pk.Met_Location == Location;
        return IsRoamingLocation(pk);
    }

    private bool IsMatchEggLocationExact(PKM pk)
    {
        var eggloc = pk.Egg_Location;
        if (!EggEncounter)
            return eggloc == EggLocation;

        if (!pk.IsEgg) // hatched
            return eggloc == EggLocation || eggloc == Locations.LinkTrade6NPC;

        // Unhatched:
        if (eggloc != EggLocation)
            return false;
        if (pk.Met_Location is not (Locations.Default8bNone or Locations.LinkTrade6NPC))
            return false;
        return true;
    }

    private bool IsMatchLocationRemapped(PKM pk)
    {
        var met = (ushort)pk.Met_Location;
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
        if (!EggEncounter)
            return pk.Egg_Location == 0;
        return LocationsHOME.IsLocationSWSHEgg(pk.Version, pk.Met_Location, pk.Egg_Location, EggLocation);
    }

    private static bool IsRoamingLocation(PKM pk) => Roaming_MetLocation_BDSP.Contains((ushort)pk.Met_Location);

    #endregion
}
