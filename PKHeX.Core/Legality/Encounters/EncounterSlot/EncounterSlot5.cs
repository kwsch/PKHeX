namespace PKHeX.Core;

/// <summary>
/// Encounter Slot found in <see cref="GameVersion.Gen5"/>.
/// </summary>
/// <inheritdoc cref="EncounterSlot"/>
public sealed record EncounterSlot5(EncounterArea5 Parent, ushort Species, byte Form, byte LevelMin, byte LevelMax) : EncounterSlot, IEncounterConvertible<PK5>, ILevelRange
{
    public int Generation => 5;
    public EntityContext Context => EntityContext.Gen5;
    public bool EggEncounter => false;
    public Ball FixedBall => Ball.None;
    public Shiny Shiny => IsHiddenGrotto ? Shiny.Never : Shiny.Random;
    public bool IsShiny => false;
    public int EggLocation => 0;

    public string Name => $"Wild Encounter ({Version})";
    public string LongName => $"{Name} {Parent.Type.ToString().Replace('_', ' ')}";
    public GameVersion Version => Parent.Version;
    public int Location => Parent.Location;
    public SlotType Type => Parent.Type;

    public bool IsHiddenGrotto => Type == SlotType.HiddenGrotto;

    private HiddenAbilityPermission IsHiddenAbilitySlot() => Parent.Type == SlotType.HiddenGrotto ? HiddenAbilityPermission.Always : HiddenAbilityPermission.Never;

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
    public PK5 ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr, EncounterCriteria.Unrestricted);

    public PK5 ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria)
    {
        int lang = (int)Language.GetSafeLanguage(Generation, (LanguageID)tr.Language);
        var pk = new PK5
        {
            Species = Species,
            Form = GetWildForm(Form),
            CurrentLevel = LevelMin,
            OT_Friendship = PersonalTable.B2W2[Species].BaseFriendship,
            Met_Location = Location,
            Met_Level = LevelMin,
            Version = (byte)Version,
            Ball = (byte)Ball.Poke,
            MetDate = EncounterDate.GetDateNDS(),

            Language = lang,
            OT_Name = tr.OT,
            OT_Gender = tr.Gender,
            ID32 = tr.ID32,
        };

        SetPINGA(pk, criteria);
        EncounterUtil1.SetEncounterMoves(pk, Version, LevelMin);
        pk.Nickname = SpeciesName.GetSpeciesNameGeneration(Species, lang, Generation);
        pk.ResetPartyStats();
        return pk;
    }

    private byte GetWildForm(byte form)
    {
        if (form != EncounterUtil1.FormRandom)
            return form;
        // flagged as totally random
        return (byte)Util.Rand.Next(PersonalTable.B2W2[Species].FormCount);
    }

    private void SetPINGA(PK5 pk, EncounterCriteria criteria)
    {
        var pi = pk.PersonalInfo;
        int gender = criteria.GetGender(-1, pi);
        int nature = (int)criteria.GetNature(Nature.Random);
        var ability = criteria.GetAbilityFromNumber(Ability);
        PIDGenerator.SetRandomWildPID5(pk, nature, ability, gender);
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
