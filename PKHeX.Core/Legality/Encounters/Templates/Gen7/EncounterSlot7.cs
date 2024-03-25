namespace PKHeX.Core;

/// <summary>
/// Encounter Slot found in <see cref="GameVersion.Gen7"/>.
/// </summary>
public sealed record EncounterSlot7(EncounterArea7 Parent, ushort Species, byte Form, byte LevelMin, byte LevelMax)
    : IEncounterable, IEncounterMatch, IEncounterConvertible<PK7>, IEncounterFormRandom, IFlawlessIVCountConditional
{
    public byte Generation => 7;
    public EntityContext Context => EntityContext.Gen7;
    public bool IsEgg => false;
    public Ball FixedBall => Location == Locations.Pelago7 ? Ball.Poke : Ball.None;
    public Shiny Shiny => Shiny.Random;
    public bool IsShiny => false;
    public ushort EggLocation => 0;
    public bool IsRandomUnspecificForm => Form >= EncounterUtil.FormDynamic;

    public string Name => $"Wild Encounter ({Version})";
    public string LongName => $"{Name} {Type.ToString().Replace('_', ' ')}";
    public GameVersion Version => Parent.Version;
    public ushort Location => Parent.Location;
    public SlotType7 Type => Parent.Type;

    public bool IsSOS => Type == SlotType7.SOS;

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
        var form = GetWildForm(Form);
        var pi = PersonalTable.USUM[Species, form];
        var pk = new PK7
        {
            Species = Species,
            Form = form,
            CurrentLevel = LevelMin,
            MetLocation = Location,
            MetLevel = LevelMin,
            Version = Version,
            MetDate = EncounterDate.GetDate3DS(),
            Ball = (byte)Ball.Poke,

            Language = lang,
            OriginalTrainerName = tr.OT,
            OriginalTrainerGender = tr.Gender,
            ID32 = tr.ID32,
            Nickname = SpeciesName.GetSpeciesNameGeneration(Species, lang, Generation),
            OriginalTrainerFriendship = pi.BaseFriendship,
        };
        if (tr is IRegionOrigin r)
            r.CopyRegionOrigin(pk);
        else
            pk.SetDefaultRegionOrigins(lang);

        SetPINGA(pk, criteria, pi);
        EncounterUtil.SetEncounterMoves(pk, Version, LevelMin);
        pk.ResetPartyStats();
        return pk;
    }

    private byte GetWildForm(byte form)
    {
        if (form != EncounterUtil.FormRandom)
            return form; // flagged as totally random
        if (Species == (int)Core.Species.Minior)
            return (byte)Util.Rand.Next(7, 14);
        return (byte)Util.Rand.Next(PersonalTable.USUM[Species].FormCount);
    }

    private void SetPINGA(PK7 pk, EncounterCriteria criteria, PersonalInfo7 pi)
    {
        var rnd = Util.Rand;
        pk.PID = rnd.Rand32();
        pk.EncryptionConstant = rnd.Rand32();
        pk.Nature = criteria.GetNature();
        pk.Gender = criteria.GetGender(pi);
        criteria.SetRandomIVs(pk);

        var num = Ability;
        if (IsSOS && pk.FlawlessIVCount < 2)
            num = 0; // let's fake it as an insufficient chain, no HA possible.
        var ability = criteria.GetAbilityFromNumber(num);
        pk.RefreshAbility(ability);
    }
    #endregion

    #region Matching

    public bool IsMatchExact(PKM pk, EvoCriteria evo)
    {
        if (!this.IsLevelWithinRange(pk.MetLevel))
            return false;

        if (Form != evo.Form && Species is not ((int)Core.Species.Furfrou or (int)Core.Species.Oricorio))
        {
            if (!IsRandomUnspecificForm) // Minior, etc
                return false;
        }

        return true;
    }
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

    public (byte Min, byte Max) GetFlawlessIVCount(PKM pk)
    {
        if (!IsSOS)
            return default;
        // Chain of 10 yields 5% HA and 2 flawless IVs
        if (pk is { Context: EntityContext.Gen7, AbilityNumber: 4 })
            return (2, 2);
        // Player could have changed the ability from a regular encounter via Ability Patch.
        return default; // Don't restrict.
    }
}
