namespace PKHeX.Core;

/// <summary>
/// Encounter Slot found in <see cref="GameVersion.XY"/>.
/// </summary>
/// <inheritdoc cref="EncounterSlot"/>
public sealed record EncounterSlot6XY(EncounterArea6XY Parent, ushort Species, byte Form, byte LevelMin, byte LevelMax)
    : EncounterSlot, IEncounterConvertible<PK6>, ILevelRange, IEncounterFormRandom
{
    public int Generation => 6;
    public EntityContext Context => EntityContext.Gen6;
    public bool EggEncounter => false;
    public Ball FixedBall => Ball.None;
    public Shiny Shiny => Shiny.Random;
    public bool IsShiny => false;
    public int EggLocation => 0;
    public bool IsRandomUnspecificForm => Form >= EncounterUtil1.FormDynamic;

    private byte FlawlessIVCount
    {
        get
        {
            var pi = PersonalTable.AO.GetFormEntry(Species, Form);
            return pi.EggGroup1 == 15 ? (byte)3 : IsFriendSafari ? (byte)2 : (byte)0;
        }
    }

    public string Name => $"Wild Encounter ({Version})";
    public string LongName => $"{Name} {Parent.Type.ToString().Replace('_', ' ')}";
    public GameVersion Version => Parent.Version;
    public int Location => Parent.Location;
    public SlotType Type => Parent.Type;

    public bool Pressure { get; init; }
    public bool IsFriendSafari => Type == SlotType.FriendSafari;
    public bool IsHorde => Type == SlotType.Horde;

    public string GetConditionString() => Pressure ? LegalityCheckStrings.LEncConditionLead : LegalityCheckStrings.LEncCondition;

    public EncounterSlot6XY CreatePressureFormCopy(byte form) => this with {Form = form, Pressure = true};

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
        int lang = (int)Language.GetSafeLanguage(Generation, (LanguageID)tr.Language);
        var version = Version != GameVersion.XY ? Version : GameVersion.XY.Contains(tr.Game) ? (GameVersion)tr.Game : GameVersion.X;
        var pk = new PK6
        {
            Species = Species,
            Form = GetWildForm(Form),
            CurrentLevel = LevelMin,
            Met_Location = Location,
            Met_Level = LevelMin,
            Ball = (byte)Ball.Poke,
            MetDate = EncounterDate.GetDate3DS(),

            Version = (byte)version,
            Language = lang,
            OT_Name = tr.OT,
            OT_Gender = tr.Gender,
            ID32 = tr.ID32,
        };
        if (tr is IRegionOrigin r)
            r.CopyRegionOrigin(pk);
        else
            pk.SetDefaultRegionOrigins();

        pk.OT_Friendship = PersonalTable.XY[Species, pk.Form].BaseFriendship;
        SetPINGA(pk, criteria);
        EncounterUtil1.SetEncounterMoves(pk, Version, LevelMin);
        pk.Nickname = SpeciesName.GetSpeciesNameGeneration(Species, lang, Generation);
        pk.SetRandomMemory6();
        pk.ResetPartyStats();
        return pk;
    }

    private byte GetWildForm(byte form)
    {
        if (form != EncounterUtil1.FormRandom)
            return form;
        // flagged as totally random
        return (byte)Util.Rand.Next(PersonalTable.XY[Species].FormCount);
    }

    private void SetPINGA(PK6 pk, EncounterCriteria criteria)
    {
        var pi = PersonalTable.XY.GetFormEntry(pk.Species, pk.Form);
        pk.PID = Util.Rand32();
        pk.EncryptionConstant = Util.Rand32();
        pk.Nature = (int)criteria.GetNature(Nature.Random);
        pk.Gender = criteria.GetGender(-1, pi);
        pk.RefreshAbility(criteria.GetAbilityFromNumber(Ability));
        pk.SetRandomIVs(FlawlessIVCount);
    }

    #endregion

    #region Matching
    public bool IsMatchExact(PKM pk, EvoCriteria evo) => true; // Matched by Area

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
