namespace PKHeX.Core;

/// <summary>
/// Encounter Slot found in <see cref="GameVersion.SV"/>.
/// </summary>
public sealed record EncounterSlot9(EncounterArea9 Parent, ushort Species, byte Form, byte LevelMin, byte LevelMax, byte Gender, byte Time, AreaWeather9 Weather)
    : IEncounterable, IEncounterMatch, IEncounterConvertible<PK9>, IEncounterFormRandom, IFixedGender
{
    public byte Generation => 9;
    public EntityContext Context => EntityContext.Gen9;
    public bool IsEgg => false;
    public AbilityPermission Ability => AbilityPermission.Any12;
    public Ball FixedBall => Ball.None;
    public Shiny Shiny => Shiny.Random;
    public bool IsShiny => false;
    public ushort EggLocation => 0;
    public bool IsRandomUnspecificForm => Form >= EncounterUtil.FormDynamic;

    public string Name => $"Wild Encounter ({Version})";
    public string LongName => $"{Name}";
    public GameVersion Version => Parent.Version;
    public ushort Location => Parent.ActualLocation();

    private static int GetTime(RibbonIndex mark) => mark switch
    {
        RibbonIndex.MarkLunchtime => 0,
        RibbonIndex.MarkSleepyTime => 1,
        RibbonIndex.MarkDusk => 2,
        RibbonIndex.MarkDawn => 3,
        _ => 4,
    };

    public bool CanSpawnAtTime(RibbonIndex mark) => (Time & (1 << GetTime(mark))) == 0;
    public bool CanSpawnInWeather(RibbonIndex mark) => Weather.IsMarkCompatible(mark);

    #region Generating

    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria) => ConvertToPKM(tr, criteria);
    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr);
    public PK9 ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr, EncounterCriteria.Unrestricted);
    public PK9 ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria)
    {
        int language = (int)Language.GetSafeLanguage(Generation, (LanguageID)tr.Language);
        var form = GetWildForm(Form);
        var version = Version != GameVersion.SV ? Version : tr.Version is GameVersion.SL or GameVersion.VL ? tr.Version : GameVersion.SL;
        var pi = PersonalTable.SV[Species, form];
        var pk = new PK9
        {
            Species = Species,
            Form = form,
            CurrentLevel = LevelMin,
            MetLocation = Location,
            MetLevel = LevelMin,
            Version = version,
            Ball = (byte)Ball.Poke,
            MetDate = EncounterDate.GetDateSwitch(),

            Language = language,
            OriginalTrainerName = tr.OT,
            OriginalTrainerGender = tr.Gender,
            ID32 = tr.ID32,
            ObedienceLevel = LevelMin,
            OriginalTrainerFriendship = pi.BaseFriendship,
            Nickname = SpeciesName.GetSpeciesNameGeneration(Species, language, Generation),
        };
        SetPINGA(pk, criteria, pi);
        EncounterUtil.SetEncounterMoves(pk, Version, LevelMin);
        pk.ResetPartyStats();
        return pk;
    }

    private byte GetWildForm(byte form)
    {
        if (form < EncounterUtil.FormDynamic)
            return form;
        if (form == EncounterUtil.FormVivillon)
            return Vivillon3DS.FancyFormID; // Fancy Vivillon
        if (Species == (int)Core.Species.Minior)
            return (byte)Util.Rand.Next(7, 14);
        // flagged as totally random
        return (byte)Util.Rand.Next(PersonalTable.SV[Species].FormCount);
    }

    private void SetPINGA(PK9 pk, in EncounterCriteria criteria, PersonalInfo9SV pi)
    {
        var rnd = Util.Rand;
        pk.PID = EncounterUtil.GetRandomPID(pk, rnd, criteria.Shiny);
        pk.EncryptionConstant = rnd.Rand32();
        criteria.SetRandomIVs(pk);

        pk.Nature = pk.StatNature = criteria.GetNature();
        pk.Gender = criteria.GetGender(Gender, pi);
        pk.RefreshAbility(criteria.GetAbilityFromNumber(Ability));

        var rand = new Xoroshiro128Plus(rnd.Rand64());
        var type = Tera9RNG.GetTeraTypeFromPersonal(Species, Form, rand.Next());
        pk.TeraTypeOriginal = (MoveType)type;
        if (Species == (int)Core.Species.Toxtricity)
            pk.Nature = ToxtricityUtil.GetRandomNature(ref rand, Form);

        pk.HeightScalar = PokeSizeUtil.GetRandomScalar(rnd);
        pk.WeightScalar = PokeSizeUtil.GetRandomScalar(rnd);
        pk.Scale = PokeSizeUtil.GetRandomScalar(rnd);
    }

    #endregion

    #region Matching
    public bool IsMatchExact(PKM pk, EvoCriteria evo)
    {
        if (Form != evo.Form && !IsRandomUnspecificForm && !IsFormOkayWild(Species, evo.Form))
            return false;
        if (Gender != FixedGenderUtil.GenderRandom && pk.Gender != Gender)
            return false;
        if (!this.IsLevelWithinRange(pk.MetLevel))
            return false;

        if (pk is ITeraType t)
        {
            var orig = (byte)t.TeraTypeOriginal;
            var pi = PersonalTable.SV[Species, Form];
            if (pi.Type1 != orig && pi.Type2 != orig)
                return false;
        }

        return true;
    }

    private static bool IsFormOkayWild(ushort species, byte form) => species switch
    {
        (int)Core.Species.Rotom => form <= 5,
        (int)Core.Species.Deerling or (int)Core.Species.Sawsbuck => form < 4,
        (int)Core.Species.Oricorio => form < 4,
        _ => false,
    };

    public EncounterMatchRating GetMatchRating(PKM pk)
    {
        bool isHidden = pk.AbilityNumber == 4;
        if (isHidden && this.IsPartialMatchHidden(pk.Species, Species))
            return EncounterMatchRating.PartialMatch;
        if (pk is IRibbonSetMark8 { HasMarkEncounter8: true } m)
        {
            if (m.RibbonMarkLunchtime && !CanSpawnAtTime(RibbonIndex.MarkLunchtime))
                return EncounterMatchRating.DeferredErrors;
            if (m.RibbonMarkSleepyTime && !CanSpawnAtTime(RibbonIndex.MarkSleepyTime))
                return EncounterMatchRating.DeferredErrors;
            if (m.RibbonMarkDusk && !CanSpawnAtTime(RibbonIndex.MarkDusk))
                return EncounterMatchRating.DeferredErrors;
            if (m.RibbonMarkDawn && !CanSpawnAtTime(RibbonIndex.MarkDawn))
                return EncounterMatchRating.DeferredErrors;

            // Some encounters can cross over into non-snow, and their encounter match might not cross back over to snow.
            // Imagine a venn diagram, one circle is Desert, the other is Snow. The met location is in the middle, so both satisfy.
            // But if we pick the Desert circle, it's wrong, and we need to defer to the other.
            if (m.HasWeatherMark(out var weather) && !CanSpawnInWeather(weather))
                return EncounterMatchRating.DeferredErrors;
        }

        if (IsFormArgDeferralRelevant(pk.Species) && pk is IFormArgument f)
        {
            bool isFormArg0 = f.FormArgument == 0;
            bool mustBeZero = IsFormArgDeferralRelevant(Species);
            if (isFormArg0 != mustBeZero)
                return EncounterMatchRating.DeferredErrors;
        }
        return EncounterMatchRating.Match;

        static bool IsFormArgDeferralRelevant(ushort species) => species is (ushort)Core.Species.Kingambit or (ushort)Core.Species.Annihilape;
    }
    #endregion
}
