using static PKHeX.Core.OverworldCorrelation8Requirement;

namespace PKHeX.Core;

/// <summary>
/// Encounter Slot found in <see cref="GameVersion.SWSH"/>.
/// </summary>
public sealed record EncounterSlot8(EncounterArea8 Parent, ushort Species, byte Form, byte LevelMin, byte LevelMax, AreaWeather8 Weather, SlotType8 Type)
    : IEncounterable, IEncounterMatch, IEncounterConvertible<PK8>, IOverworldCorrelation8
{
    public byte Generation => 8;
    public EntityContext Context => EntityContext.Gen8;
    public bool IsEgg => false;
    public Ball FixedBall => Ball.None;
    public AbilityPermission Ability => AbilityPermission.Any12;
    public Shiny Shiny => Shiny.Random;
    public bool IsShiny => false;
    public ushort EggLocation => 0;

    public string Name => $"Wild Encounter ({Version})";
    public string LongName => $"{Name} [{Type}] - {Weather.ToString().Replace("_", string.Empty)}";
    public GameVersion Version => Parent.Version;
    public ushort Location => Parent.Location;

    // Fishing are only from the hidden table (not symbol).
    public bool CanEncounterViaFishing => Type.CanEncounterViaFishing(Weather);
    public bool CanEncounterViaCurry
    {
        get
        {
            if (!Type.CanEncounterViaCurry())
                return false;

            if ((Weather & AreaWeather8.All) == 0)
                return false;

            if (EncounterArea8.IsWildArea(Location))
                return false;

            return true;
        }
    }

    #region Generating
    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria) => ConvertToPKM(tr, criteria);
    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr);
    public PK8 ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr, EncounterCriteria.Unrestricted);
    public PK8 ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria)
    {
        int lang = (int)Language.GetSafeLanguage(Generation, (LanguageID)tr.Language);
        var form = GetWildForm(Form);
        var pi = PersonalTable.SWSH[Species, form];
        var pk = new PK8
        {
            Species = Species,
            Form = form,
            CurrentLevel = LevelMin,
            MetLocation = Location,
            MetLevel = LevelMin,
            Version = Version,
            MetDate = EncounterDate.GetDateSwitch(),
            Ball = (byte)Ball.Poke,

            Language = lang,
            OriginalTrainerName = tr.OT,
            OriginalTrainerGender = tr.Gender,
            ID32 = tr.ID32,
            Nickname = SpeciesName.GetSpeciesNameGeneration(Species, lang, Generation),
            OriginalTrainerFriendship = pi.BaseFriendship,
        };
        SetPINGA(pk, criteria, pi);
        EncounterUtil.SetEncounterMoves(pk, Version, LevelMin);

        bool symbol = Parent.PermitCrossover;
        if (!symbol && Location is 30 or 54 && (Weather & AreaWeather8.Fishing) == 0)
            pk.RibbonMarkCurry = true;

        if (Weather is AreaWeather8.Heavy_Fog && EncounterArea8.IsBoostedArea60Fog(Location))
            pk.MetLevel = pk.CurrentLevel = EncounterArea8.BoostLevel;
        pk.ResetPartyStats();
        return pk;
    }

    private byte GetWildForm(byte form)
    {
        if (form == EncounterUtil.FormRandom) // flagged as totally random
            return (byte)Util.Rand.Next(PersonalTable.SWSH[Species].FormCount);
        return form;
    }

    #endregion

    private void SetPINGA(PK8 pk, EncounterCriteria criteria, PersonalInfo8SWSH pi)
    {
        bool symbol = Parent.PermitCrossover;
        var c = symbol ? EncounterCriteria.Unrestricted : criteria;
        pk.RefreshAbility(criteria.GetAbilityFromNumber(Ability));
        pk.Nature = pk.StatNature = criteria.GetNature();
        pk.Gender = criteria.GetGender(pi);

        var req = GetRequirement(pk);
        if (req != MustHave)
        {
            var rand = Util.Rand;
            pk.EncryptionConstant = rand.Rand32();
            pk.PID = rand.Rand32();
            pk.HeightScalar = PokeSizeUtil.GetRandomScalar(rand);
            pk.WeightScalar = PokeSizeUtil.GetRandomScalar(rand);
            criteria.SetRandomIVs(pk);
            return;
        }
        // Don't bother honoring shiny state.
        Overworld8RNG.ApplyDetails(pk, c, Shiny.Random);
    }

    public OverworldCorrelation8Requirement GetRequirement(PKM pk)
    {
        if (Parent.PermitCrossover)
            return MustHave; // symbol walking overworld

        bool curry = pk is IRibbonSetMark8 {RibbonMarkCurry: true} || (pk.Species == (int)Core.Species.Shedinja && pk is IRibbonSetAffixed { AffixedRibbon:(int)RibbonIndex.MarkCurry});
        if (curry)
            return MustNotHave;

        // Tree encounters are generated via the global seed, not the u32
        if ((Weather & AreaWeather8.Shaking_Trees) != 0)
        {
            // Some tree encounters are present in the regular encounters.
            return Weather == AreaWeather8.Shaking_Trees
                ? MustNotHave
                : CanBeEither;
        }

        return MustHave;
    }

    public bool IsOverworldCorrelationCorrect(PKM pk)
    {
        var flawless = GetFlawlessIVCount(pk.MetLevel);
        return Overworld8RNG.ValidateOverworldEncounter(pk, flawless: flawless);
    }

    private int GetFlawlessIVCount(int metLevel)
    {
        const int none = 0;
        const int any023 = -1;

        // Brilliant encounters are boosted to max level for the slot.
        if (metLevel < LevelMax)
            return none;

        if (Parent.PermitCrossover)
            return any023; // Symbol
        if ((Weather & AreaWeather8.Fishing) != 0)
            return any023; // Fishing
        return none; // Hidden
    }

    #region Matching

    public bool IsMatchExact(PKM pk, EvoCriteria evo)
    {
        if (Form != evo.Form && Species is not (int)Core.Species.Rotom)
            return false;

        var metLocation = pk.MetLocation;
        if (Location != metLocation && !EncounterArea8.CanCrossoverTo(Location, metLocation, Type))
            return false;

        var met = pk.MetLevel;
        if (met == EncounterArea8.BoostLevel && EncounterArea8.IsBoostedArea60(Location))
            return true;

        if (!this.IsLevelWithinRange(met))
            return false;

        if (Weather is AreaWeather8.Heavy_Fog && EncounterArea8.IsWildArea8(Location))
            return false; // Heavy Fog not available until post-game, which would return true above.

        return true;
    }

    public EncounterMatchRating GetMatchRating(PKM pk)
    {
        bool isHidden = pk.AbilityNumber == 4;
        if (isHidden && this.IsPartialMatchHidden(pk.Species, Species))
            return EncounterMatchRating.PartialMatch;

        if (pk is IRibbonSetMark8 m)
        {
            if (m.RibbonMarkCurry && (Weather & AreaWeather8.All) == 0)
                return EncounterMatchRating.DeferredErrors;
            if (m.RibbonMarkFishing && (Weather & AreaWeather8.Fishing) == 0)
                return EncounterMatchRating.DeferredErrors;

            // Check if it has a mark and the weather does not permit the mark.
            // Tree/Fishing slots should be deferred here and are checked later.
            if (!Weather.IsMarkCompatible(m))
                return EncounterMatchRating.DeferredErrors;

            // Galar Mine hidden encounters can only be found via Curry or Fishing.
            if (Location is (30 or 54) && Type is SlotType8.HiddenMain && !m.RibbonMarkCurry && !Type.CanEncounterViaFishing(Weather))
                return EncounterMatchRating.DeferredErrors;
        }

        var req = GetRequirement(pk);
        return req switch
        {
            MustHave when !IsOverworldCorrelationCorrect(pk) => EncounterMatchRating.DeferredErrors,
            MustNotHave when IsOverworldCorrelationCorrect(pk) => EncounterMatchRating.DeferredErrors,
            _ => EncounterMatchRating.Match,
        };
    }

    #endregion
}
