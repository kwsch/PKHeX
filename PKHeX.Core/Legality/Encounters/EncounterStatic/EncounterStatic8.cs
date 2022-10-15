using static PKHeX.Core.OverworldCorrelation8Requirement;

namespace PKHeX.Core;

/// <summary>
/// Generation 8 Static Encounter
/// </summary>
/// <inheritdoc cref="EncounterStatic"/>
public sealed record EncounterStatic8(GameVersion Version) : EncounterStatic(Version), IDynamaxLevel, IGigantamax, IRelearn, IOverworldCorrelation8
{
    public override int Generation => 8;
    public override EntityContext Context => EntityContext.Gen8;
    public bool ScriptedNoMarks { get; init; }
    public bool CanGigantamax { get; set; }
    public byte DynamaxLevel { get; set; }
    public Moveset Relearn { get; init; }
    public Crossover8 Crossover { get; init; }

    public AreaWeather8 Weather {get; init; } = AreaWeather8.Normal;

    protected override bool IsMatchLocation(PKM pk)
    {
        var met = pk.Met_Location;
        if (met == Location)
            return true;
        if ((uint)met > byte.MaxValue)
            return false;
        return Crossover.IsMatchLocation((byte)met);
    }

    protected override bool IsMatchLevel(PKM pk, EvoCriteria evo)
    {
        var met = pk.Met_Level;
        var lvl = Level;
        if (met == lvl)
            return true;
        if (lvl < EncounterArea8.BoostLevel && EncounterArea8.IsBoostedArea60(Location))
            return met == EncounterArea8.BoostLevel;
        return false;
    }

    public override bool IsMatchExact(PKM pk, EvoCriteria evo)
    {
        if (pk is PK8 d && d.DynamaxLevel < DynamaxLevel)
            return false;
        if (pk.Met_Level < EncounterArea8.BoostLevel && Weather is AreaWeather8.Heavy_Fog && EncounterArea8.IsBoostedArea60Fog(Location))
            return false;
        return base.IsMatchExact(pk, evo);
    }

    protected override void ApplyDetails(ITrainerInfo tr, EncounterCriteria criteria, PKM pk)
    {
        base.ApplyDetails(tr, criteria, pk);
        if (Weather is AreaWeather8.Heavy_Fog && EncounterArea8.IsBoostedArea60Fog(Location))
            pk.CurrentLevel = pk.Met_Level = EncounterArea8.BoostLevel;

        var req = GetRequirement(pk);
        if (req != MustHave)
        {
            pk.SetRandomEC();
            return;
        }
        var shiny = Shiny == Shiny.Random ? Shiny.FixedValue : Shiny;
        Overworld8RNG.ApplyDetails(pk, criteria, shiny, FlawlessIVCount);
    }

    public bool IsOverworldCorrelation
    {
        get
        {
            if (Gift)
                return false; // gifts can have any 128bit seed from overworld
            if (ScriptedNoMarks)
                return false;  // scripted encounters don't act as saved spawned overworld encounters
            return true;
        }
    }

    public OverworldCorrelation8Requirement GetRequirement(PKM pk) => IsOverworldCorrelation
        ? MustHave
        : MustNotHave;

    public bool IsOverworldCorrelationCorrect(PKM pk)
    {
        return Overworld8RNG.ValidateOverworldEncounter(pk, Shiny == Shiny.Random ? Shiny.FixedValue : Shiny, FlawlessIVCount);
    }

    public override EncounterMatchRating GetMatchRating(PKM pk)
    {
        var rating = base.GetMatchRating(pk);
        if (rating != EncounterMatchRating.Match)
            return rating;

        var req = GetRequirement(pk);
        bool correlation = IsOverworldCorrelationCorrect(pk);
        if ((req == MustHave) != correlation)
            return EncounterMatchRating.DeferredErrors;

        // Only encounter slots can have these marks; defer for collisions.
        if (pk.Species == (int) Core.Species.Shedinja)
        {
            // Loses Mark on evolution to Shedinja, but not affixed ribbon value.
            return pk switch
            {
                IRibbonSetMark8 {RibbonMarkCurry: true} => EncounterMatchRating.DeferredErrors,
                PK8 {AffixedRibbon: (int) RibbonIndex.MarkCurry} => EncounterMatchRating.Deferred,
                _ => EncounterMatchRating.Match,
            };
        }

        if (pk is IRibbonSetMark8 m && (m.RibbonMarkCurry || m.RibbonMarkFishing || m.HasWeatherMark()))
            return EncounterMatchRating.DeferredErrors;

        return EncounterMatchRating.Match;
    }
}

public interface IOverworldCorrelation8
{
    OverworldCorrelation8Requirement GetRequirement(PKM pk);
    bool IsOverworldCorrelationCorrect(PKM pk);
}

public enum OverworldCorrelation8Requirement
{
    CanBeEither,
    MustHave,
    MustNotHave,
}

public readonly record struct Crossover8(byte L1 = 0, byte L2 = 0, byte L3 = 0, byte L4 = 0, byte L5 = 0, byte L6 = 0, byte L7 = 0)
{
    public bool IsMatchLocation(byte location) => location != 0 && L1 != 0 && (
        location == L1 || location == L2 || location == L3 ||
        location == L4 || location == L5 || location == L6 || location == L7);
}
