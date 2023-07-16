namespace PKHeX.Core;

/// <summary>
/// Encounter Slot found in <see cref="GameVersion.Gen7"/>.
/// </summary>
/// <inheritdoc cref="EncounterSlot"/>
public sealed record EncounterSlot7(EncounterArea7 Parent, ushort Species, byte Form, byte LevelMin, byte LevelMax)
    : EncounterSlot, IEncounterConvertible<PK7>, ILevelRange, IEncounterFormRandom
{
    public int Generation => 7;
    public EntityContext Context => EntityContext.Gen7;
    public bool EggEncounter => false;
    public Ball FixedBall => Location == Locations.Pelago7 ? Ball.Poke : Ball.None;
    public Shiny Shiny => Shiny.Random;
    public bool IsShiny => false;
    public int EggLocation => 0;
    public bool IsRandomUnspecificForm => Form >= EncounterUtil1.FormDynamic;

    public string Name => $"Wild Encounter ({Version})";
    public string LongName => $"{Name} {Parent.Type.ToString().Replace('_', ' ')}";
    public GameVersion Version => Parent.Version;
    public int Location => Parent.Location;
    public SlotType Type => Parent.Type;

    public bool IsSOS => Type == SlotType.SOS;

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

    private HiddenAbilityPermission IsHiddenAbilitySlot() => IsSOS ? HiddenAbilityPermission.Possible : HiddenAbilityPermission.Never;

    #region Generating
    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria) => ConvertToPKM(tr, criteria);
    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr);
    public PK7 ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr, EncounterCriteria.Unrestricted);
    public PK7 ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria)
    {
        int lang = (int)Language.GetSafeLanguage(Generation, (LanguageID)tr.Language);
        var pk = new PK7
        {
            Species = Species,
            Form = GetWildForm(Form),
            CurrentLevel = LevelMin,
            Met_Location = Location,
            Met_Level = LevelMin,
            Version = (byte)Version,
            MetDate = EncounterDate.GetDate3DS(),
            Ball = (byte)Ball.Poke,

            Language = lang,
            OT_Name = tr.OT,
            OT_Gender = tr.Gender,
            ID32 = tr.ID32,
        };
        if (tr is IRegionOrigin r)
            r.CopyRegionOrigin(pk);
        else
            pk.SetDefaultRegionOrigins();

        pk.OT_Friendship = PersonalTable.USUM[Species, pk.Form].BaseFriendship;
        SetPINGA(pk, criteria);
        EncounterUtil1.SetEncounterMoves(pk, Version, LevelMin);
        pk.Nickname = SpeciesName.GetSpeciesNameGeneration(Species, lang, Generation);
        pk.ResetPartyStats();
        return pk;
    }

    private byte GetWildForm(byte form)
    {
        if (form != EncounterUtil1.FormRandom)
            return form; // flagged as totally random
        if (Species == (int)Core.Species.Minior)
            return (byte)Util.Rand.Next(7, 14);
        return (byte)Util.Rand.Next(PersonalTable.USUM[Species].FormCount);
    }

    private void SetPINGA(PK7 pk, EncounterCriteria criteria)
    {
        var pi = pk.PersonalInfo;
        pk.PID = Util.Rand32();
        pk.EncryptionConstant = Util.Rand32();
        pk.Nature = (int)criteria.GetNature(Nature.Random);
        pk.Gender = criteria.GetGender(-1, pi);
        criteria.SetRandomIVs(pk);

        var num = Ability;
        if (IsSOS && pk.FlawlessIVCount < 2)
            num = 0; // let's fake it as an insufficient chain, no HA possible.
        var ability = criteria.GetAbilityFromNumber(num);
        pk.RefreshAbility(ability);
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
