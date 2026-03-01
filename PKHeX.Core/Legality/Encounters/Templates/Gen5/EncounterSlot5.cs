using System;

namespace PKHeX.Core;

/// <summary>
/// Encounter Slot found in <see cref="EntityContext.Gen5"/>.
/// </summary>
public sealed record EncounterSlot5(EncounterArea5 Parent, ushort Species, byte Form, byte LevelMin, byte LevelMax)
    : IEncounterable, IEncounterMatch, IEncounterConvertible<PK5>
{
    public byte Generation => 5;
    public EntityContext Context => EntityContext.Gen5;
    public bool IsEgg => false;
    public Ball FixedBall => Ball.None;
    public Shiny Shiny => IsHiddenGrotto ? Shiny.Never : Shiny.Random;
    public bool IsShiny => false;
    public ushort EggLocation => 0;

    public string Name => $"Wild Encounter ({Version})";
    public string LongName => $"{Name} {Type.ToString().Replace('_', ' ')}";
    public GameVersion Version => Parent.Version;
    public ushort Location => Parent.Location;
    public SlotType5 Type => Parent.Type;

    public bool IsHiddenGrotto => Type == SlotType5.HiddenGrotto;

    private HiddenAbilityPermission IsHiddenAbilitySlot() => IsHiddenGrotto ? HiddenAbilityPermission.Always : HiddenAbilityPermission.Never;

    public AbilityPermission Ability => IsHiddenAbilitySlot() switch
    {
        HiddenAbilityPermission.Never => AbilityPermission.Any12,
        HiddenAbilityPermission.Always => AbilityPermission.OnlyHidden,
        _ => throw new ArgumentOutOfRangeException(),
    };

    private bool IsDeferredHiddenAbility(bool isHidden) => IsHiddenAbilitySlot() switch
    {
        HiddenAbilityPermission.Never => isHidden,
        HiddenAbilityPermission.Always => !isHidden,
        _ => false,
    };

    #region Generating
    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria) => ConvertToPKM(tr, criteria);
    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr);
    public PK5 ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr, EncounterCriteria.Unrestricted);

    public PK5 ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria)
    {
        int language = (int)Language.GetSafeLanguage456((LanguageID)tr.Language);
        var pi = PersonalTable.B2W2[Species];
        var pk = new PK5
        {
            Species = Species,
            Form = GetWildForm(Form),
            CurrentLevel = LevelMin,
            OriginalTrainerFriendship = pi.BaseFriendship,
            MetLocation = Location,
            MetLevel = LevelMin,
            Version = Version,
            Ball = (byte)Ball.Poke,
            MetDate = EncounterDate.GetDateNDS(),

            Language = language,
            OriginalTrainerName = tr.OT,
            OriginalTrainerGender = tr.Gender,
            ID32 = tr.ID32,
            Nickname = SpeciesName.GetSpeciesNameGeneration(Species, language, Generation),
        };

        SetPINGA(pk, criteria, pi);
        EncounterUtil.SetEncounterMoves(pk, Version, LevelMin);
        pk.ResetPartyStats();
        return pk;
    }

    private byte GetWildForm(byte form)
    {
        if (form != EncounterUtil.FormRandom)
            return form;
        // flagged as totally random
        return (byte)Util.Rand.Next(PersonalTable.B2W2[Species].FormCount);
    }

    private void SetPINGA(PK5 pk, in EncounterCriteria criteria, PersonalInfo5B2W2 pi)
    {
        var seed = Util.Rand.Rand64();
        MonochromeRNG.Generate(pk, criteria, pi.Gender, seed, true, Shiny, Ability);
        pk.Nature = criteria.GetNature();
        var abilityIndex = Ability == AbilityPermission.OnlyHidden ? 2 : (int)((pk.PID >> 16) & 1);
        pk.RefreshAbility(abilityIndex);
        criteria.SetRandomIVs(pk);
    }
    #endregion

    #region Matching

    public bool IsMatchExact(PKM pk, EvoCriteria evo)
    {
        if (!this.IsLevelWithinRange(pk.MetLevel))
            return false;

        // Deerling and Sawsbuck can change forms when seasons change, thus can be any of the [0,3] form values.
        // no other wild forms can change
        if (evo.Form != Form && Species is not ((int)Core.Species.Deerling or (int)Core.Species.Sawsbuck))
            return false;

        return true;
    }

    public EncounterMatchRating GetMatchRating(PKM pk)
    {
        bool isHidden = pk.AbilityNumber == 4;
        if (isHidden && this.IsPartialMatchHidden(pk.Species, Species))
            return EncounterMatchRating.PartialMatch;

        // B2/W2 Vespiquen (Level 24 both evolutions available at same location)
        // Bigender->Fixed (non-Genderless) destination species, accounting for PID-Gender relationship
        if (Species == (int)Core.Species.Combee && pk.Species == (int)Core.Species.Vespiquen && (pk.EncryptionConstant & 0xFF) >= 0x1F) // Combee->Vespiquen Invalid Evolution
            return EncounterMatchRating.DeferredErrors;
        if (IsDeferredHiddenAbility(isHidden))
            return EncounterMatchRating.Deferred;
        return EncounterMatchRating.Match;
    }
    #endregion
}
