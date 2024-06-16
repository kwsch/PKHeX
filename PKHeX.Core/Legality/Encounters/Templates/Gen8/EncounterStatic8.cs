using static PKHeX.Core.OverworldCorrelation8Requirement;

namespace PKHeX.Core;

/// <summary>
/// Generation 8 Static Encounter
/// </summary>
public sealed record EncounterStatic8(GameVersion Version = GameVersion.SWSH)
    : IEncounterable, IEncounterMatch, IEncounterConvertible<PK8>, IMoveset, IRelearn,
        IFlawlessIVCount, IFixedIVSet, IFixedGender, IFixedNature, IDynamaxLevelReadOnly, IGigantamaxReadOnly, IOverworldCorrelation8, IFatefulEncounterReadOnly
{
    public byte Generation => 8;
    public EntityContext Context => EntityContext.Gen8;
    ushort ILocation.Location => Location;
    public bool Gift => FixedBall != Ball.None;
    public bool IsShiny => Shiny == Shiny.Always;
    public bool IsEgg => false;
    ushort ILocation.EggLocation => 0;

    public required ushort Location { get; init; }
    public required ushort Species { get; init; }
    public byte Form { get; init; }
    public required byte Level { get; init; }
    public Moveset Moves { get; init; }
    public Moveset Relearn { get; init; }
    public IndividualValueSet IVs { get; init; }
    public Crossover8 Crossover { get; init; }
    public AreaWeather8 Weather { get; init; } = AreaWeather8.Normal;
    public byte DynamaxLevel { get; init; }
    public Nature Nature { get; init; }
    public Shiny Shiny { get; init; }
    public AbilityPermission Ability { get; init; }
    public byte Gender { get; init; } = FixedGenderUtil.GenderRandom;
    public Ball FixedBall { get; init; }
    public byte FlawlessIVCount { get; init; }
    public bool ScriptedNoMarks { get; init; }
    public bool CanGigantamax { get; init; }
    public bool FatefulEncounter { get; init; }

    public string Name => "Static Encounter";
    public string LongName => Name;
    public byte LevelMin => Level;
    public byte LevelMax => Level;

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

    #region Generating

    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr);
    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria) => ConvertToPKM(tr, criteria);

    public PK8 ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr, EncounterCriteria.Unrestricted);

    public PK8 ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria)
    {
        var version = this.GetCompatibleVersion(tr.Version);
        int lang = (int)Language.GetSafeLanguage(Generation, (LanguageID)tr.Language, version);
        var pk = new PK8
        {
            Species = Species,
            Form = Form,
            CurrentLevel = Level,
            MetLocation = Location,
            MetLevel = Level,
            MetDate = EncounterDate.GetDateSwitch(),
            Ball = (byte)(FixedBall != Ball.None ? FixedBall : Ball.Poke),
            FatefulEncounter = FatefulEncounter,

            ID32 = tr.ID32,
            Version = version,
            Language = lang,
            OriginalTrainerGender = tr.Gender,
            OriginalTrainerName = tr.OT,
            OriginalTrainerFriendship = PersonalTable.SWSH[Species, Form].BaseFriendship,

            Nickname = SpeciesName.GetSpeciesNameGeneration(Species, lang, Generation),

            DynamaxLevel = DynamaxLevel,
            CanGigantamax = CanGigantamax,
        };

        SetPINGA(pk, criteria);

        if (Moves.HasMoves)
            pk.SetMoves(Moves);
        else
            EncounterUtil.SetEncounterMoves(pk, version, Level);
        pk.ResetPartyStats();

        return pk;
    }

    private void SetPINGA(PK8 pk, EncounterCriteria criteria)
    {
        if (Weather is AreaWeather8.Heavy_Fog && EncounterArea8.IsBoostedArea60Fog(Location))
            pk.MetLevel = pk.CurrentLevel = EncounterArea8.BoostLevel;

        var pi = PersonalTable.SWSH[Species, Form];
        pk.RefreshAbility(criteria.GetAbilityFromNumber(Ability));
        pk.Nature = pk.StatNature = criteria.GetNature();
        pk.Gender = criteria.GetGender(Gender, pi);

        var req = GetRequirement(pk);
        if (req != MustHave)
        {
            var rand = Util.Rand;
            pk.EncryptionConstant = rand.Rand32();
            pk.PID = rand.Rand32();
            pk.HeightScalar = PokeSizeUtil.GetRandomScalar(rand);
            pk.WeightScalar = PokeSizeUtil.GetRandomScalar(rand);
            if (IVs.IsSpecified)
                criteria.SetRandomIVs(pk, IVs);
            else
                criteria.SetRandomIVs(pk, FlawlessIVCount);
            return;
        }
        var shiny = Shiny == Shiny.Random ? Shiny.FixedValue : Shiny;
        Overworld8RNG.ApplyDetails(pk, criteria, shiny, FlawlessIVCount);
    }

    #endregion

    #region Matching

    public bool IsMatchExact(PKM pk, EvoCriteria evo)
    {
        if (!IsMatchLevel(pk))
            return false;
        if (!IsMatchLocation(pk))
            return false;
        if (!IsMatchEggLocation(pk))
            return false;
        if (pk is PK8 d && d.DynamaxLevel < DynamaxLevel)
            return false;
        if (pk.MetLevel < EncounterArea8.BoostLevel && Weather is AreaWeather8.Heavy_Fog && EncounterArea8.IsBoostedArea60Fog(Location))
            return false;
        if (Gender != FixedGenderUtil.GenderRandom && pk.Gender != Gender)
            return false;
        if (Form != evo.Form && !FormInfo.IsFormChangeable(Species, Form, pk.Form, Context, pk.Context))
            return false;
        if (IVs.IsSpecified && !Legal.GetIsFixedIVSequenceValidSkipRand(IVs, pk))
            return false;
        if (FlawlessIVCount != 0 && pk.FlawlessIVCount < FlawlessIVCount)
            return false;
        return true;
    }

    private static bool IsMatchEggLocation(PKM pk)
    {
        var expect = pk is PB8 ? Locations.Default8bNone : 0;
        return pk.EggLocation == expect;
    }

    private bool IsMatchLocation(PKM pk)
    {
        var met = pk.MetLocation;
        if (met == Location)
            return true;
        if ((uint)met > byte.MaxValue)
            return false;
        return Crossover.IsMatchLocation((byte)met);
    }

    private bool IsMatchLevel(PKM pk)
    {
        var met = pk.MetLevel;
        var lvl = Level;
        if (met == lvl)
            return true;
        if (lvl < EncounterArea8.BoostLevel && EncounterArea8.IsBoostedArea60(Location))
            return met == EncounterArea8.BoostLevel;
        return false;
    }

    public EncounterMatchRating GetMatchRating(PKM pk)
    {
        if (pk is { AbilityNumber: 4 } && this.IsPartialMatchHidden(pk.Species, Species))
            return EncounterMatchRating.PartialMatch;

        var req = GetRequirement(pk);
        bool correlation = IsOverworldCorrelationCorrect(pk);
        if ((req == MustHave) != correlation)
            return EncounterMatchRating.DeferredErrors;

        // Only encounter slots can have these marks; defer for collisions.
        if (pk.Species == (int)Core.Species.Shedinja)
        {
            // Loses Mark on evolution to Shedinja, but not affixed ribbon value.
            return pk switch
            {
                IRibbonSetMark8 { RibbonMarkCurry: true } => EncounterMatchRating.DeferredErrors,
                PK8 { AffixedRibbon: (int)RibbonIndex.MarkCurry } => EncounterMatchRating.Deferred,
                _ => EncounterMatchRating.Match,
            };
        }

        if (pk is IRibbonSetMark8 m && (m.RibbonMarkCurry || m.RibbonMarkFishing || !Weather.IsMarkCompatible(m)))
            return EncounterMatchRating.DeferredErrors;

        return EncounterMatchRating.Match;
    }

    #endregion
}
