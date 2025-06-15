namespace PKHeX.Core;

/// <summary>
/// Encounter Slot found in <see cref="GameVersion.XY"/>.
/// </summary>
public sealed record EncounterSlot6XY(EncounterArea6XY Parent, ushort Species, byte Form, byte LevelMin, byte LevelMax)
    : IEncounterable, IEncounterMatch, IEncounterConvertible<PK6>, IEncounterFormRandom, IFlawlessIVCount
{
    public byte Generation => 6;
    public EntityContext Context => EntityContext.Gen6;
    public bool IsEgg => false;
    public Ball FixedBall => Ball.None;
    public Shiny Shiny => Shiny.Random;
    public bool IsShiny => false;
    public ushort EggLocation => 0;
    public bool IsRandomUnspecificForm => Form >= EncounterUtil.FormDynamic;

    private PersonalInfo6XY PersonalInfo => PersonalTable.XY[Species];
    public byte FlawlessIVCount => PersonalInfo.EggGroup1 == 15 ? (byte)3 : IsFriendSafari ? (byte)2 : (byte)0;

    public string Name => $"Wild Encounter ({Version})";
    public string LongName => $"{Name} {Type.ToString().Replace('_', ' ')}";
    public GameVersion Version => Parent.Version;
    public ushort Location => Parent.Location;
    public SlotType6 Type => Parent.Type;

    public bool IsFriendSafari => Type == SlotType6.FriendSafari;
    public bool IsHorde => Type == SlotType6.Horde;

    private HiddenAbilityPermission IsHiddenAbilitySlot() => IsHorde || IsFriendSafari ? HiddenAbilityPermission.Possible : HiddenAbilityPermission.Never;

    public AbilityPermission Ability => IsHiddenAbilitySlot() switch
    {
        HiddenAbilityPermission.Never => AbilityPermission.Any12,
        HiddenAbilityPermission.Always => AbilityPermission.OnlyHidden,
        _ => AbilityPermission.Any12H,
    };
    private bool IsDeferredHiddenAbility(bool IsHidden) => IsHiddenAbilitySlot() switch
    {
        HiddenAbilityPermission.Never => IsHidden,
        HiddenAbilityPermission.Always => !IsHidden,
        _ => false,
    };

    #region Generating
    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria) => ConvertToPKM(tr, criteria);
    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr);
    public PK6 ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr, EncounterCriteria.Unrestricted);

    public PK6 ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria)
    {
        int language = (int)Language.GetSafeLanguage(Generation, (LanguageID)tr.Language);
        var version = Version != GameVersion.XY ? Version : tr.Version is GameVersion.X or GameVersion.Y ? tr.Version : GameVersion.X;
        var form = GetWildForm(Form);
        var pi = PersonalTable.XY[Species, form];
        var geo = tr.GetRegionOrigin(language);
        var pk = new PK6
        {
            Species = Species,
            Form = form,
            CurrentLevel = LevelMin,
            MetLocation = Location,
            MetLevel = LevelMin,
            Ball = (byte)Ball.Poke,
            MetDate = EncounterDate.GetDate3DS(),

            Version = version,
            Language = language,
            OriginalTrainerName = tr.OT,
            OriginalTrainerGender = tr.Gender,
            ID32 = tr.ID32,
            Nickname = SpeciesName.GetSpeciesNameGeneration(Species, language, Generation),
            OriginalTrainerFriendship = pi.BaseFriendship,

            ConsoleRegion = geo.ConsoleRegion,
            Country = geo.Country,
            Region = geo.Region,
        };

        if (IsRandomUnspecificForm && Form == EncounterUtil.FormVivillon)
            pk.Form = Vivillon3DS.GetPattern(pk.Country, pk.Region);

        SetPINGA(pk, criteria, pi);
        EncounterUtil.SetEncounterMoves(pk, Version, LevelMin);
        pk.SetRandomMemory6();
        pk.ResetPartyStats();
        return pk;
    }

    private byte GetWildForm(byte form)
    {
        if (form < EncounterUtil.FormDynamic)
            return form;
        if (form == EncounterUtil.FormVivillon)
            return 0; // rectify later

        // flagged as totally random
        return (byte)Util.Rand.Next(PersonalTable.XY[Species].FormCount);
    }

    private void SetPINGA(PK6 pk, EncounterCriteria criteria, PersonalInfo6XY pi)
    {
        var rnd = Util.Rand;
        pk.PID = rnd.Rand32();
        if (criteria.Shiny.IsShiny())
            pk.PID = ShinyUtil.GetShinyPID(pk.TID16, pk.SID16, pk.PID, criteria.Shiny == Shiny.AlwaysSquare ? 0 : (uint)rnd.Next(1, 15));
        else if (criteria.Shiny == Shiny.Never && pk.IsShiny)
            pk.PID ^= 0x80000000; // flip top bit to ensure non-shiny

        pk.EncryptionConstant = rnd.Rand32();
        pk.Nature = criteria.GetNature();
        pk.Gender = criteria.GetGender(pi);
        pk.RefreshAbility(criteria.GetAbilityFromNumber(Ability));
        criteria.SetRandomIVs(pk, FlawlessIVCount, rnd);
    }

    #endregion

    #region Matching

    public bool IsMatchExact(PKM pk, EvoCriteria evo)
    {
        if (!this.IsLevelWithinRange(pk.MetLevel))
            return false;

        if (Form != evo.Form && !IsRandomUnspecificForm && !IsValidOutOfBoundsForm(pk))
            return false;

        return true;
    }

    private bool IsValidOutOfBoundsForm(PKM pk) => Species switch
    {
        (int)Core.Species.Burmy or (int)Core.Species.Furfrou => true, // Can change forms in-game.
        (int)Core.Species.Sawsbuck => pk.Format >= 8, // Friend Safari can change between forms if imported to a future Gen8+
        _ => false,
    };

    public EncounterMatchRating GetMatchRating(PKM pk)
    {
        bool isHidden = pk.AbilityNumber == 4;
        if (isHidden && this.IsPartialMatchHidden(pk.Species, Species))
            return EncounterMatchRating.PartialMatch;
        if (IsDeferredHiddenAbility(isHidden))
            return EncounterMatchRating.Deferred;
        return EncounterMatchRating.Match;
    }
    #endregion
}
