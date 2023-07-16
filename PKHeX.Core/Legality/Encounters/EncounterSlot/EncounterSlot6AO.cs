using System;

namespace PKHeX.Core;

/// <summary>
/// Encounter Slot found in <see cref="GameVersion.ORAS"/>.
/// </summary>
/// <inheritdoc cref="EncounterSlot"/>
public sealed record EncounterSlot6AO(EncounterArea6AO Parent, ushort Species, byte Form, byte LevelMin, byte LevelMax)
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

    public string Name => $"Wild Encounter ({Version})";
    public string LongName => $"{Name} {Parent.Type.ToString().Replace('_', ' ')}";
    public GameVersion Version => Parent.Version;
    public int Location => Parent.Location;
    public SlotType Type => Parent.Type;
    public bool CanDexNav => Type != SlotType.Rock_Smash;
    public bool IsHorde => Type == SlotType.Horde;

    public bool DexNav { get; init; }
    public bool WhiteFlute { get; init; }
    public bool BlackFlute { get; init; }

    public string GetConditionString() => this switch
    {
        { DexNav:     true } => LegalityCheckStrings.LEncConditionDexNav,
        { WhiteFlute: true } => LegalityCheckStrings.LEncConditionWhite, // Decreased Level Encounters
        { BlackFlute: true } => LegalityCheckStrings.LEncConditionBlack, // Increased Level Encounters
        _ => LegalityCheckStrings.LEncCondition,
    };

    private HiddenAbilityPermission IsHiddenAbilitySlot() => CanDexNav || IsHorde ? HiddenAbilityPermission.Possible : HiddenAbilityPermission.Never;

    private ReadOnlySpan<ushort> GetDexNavMoves()
    {
        var et = EvolutionTree.Evolves6;
        var baby = et.GetBaseSpeciesForm(Species, Form);
        return LearnSource6AO.Instance.GetEggMoves(baby.Species, baby.Form);
    }

    public bool CanBeDexNavMove(ushort move) => GetDexNavMoves().Contains(move);

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
        var pk = new PK6
        {
            Species = Species,
            Form = GetWildForm(Form),
            CurrentLevel = LevelMin,
            OT_Friendship = PersonalTable.AO[Species].BaseFriendship,
            Met_Location = Location,
            Met_Level = LevelMin,
            Version = (byte)Version,
            Ball = (byte)Ball.Poke,
            MetDate = EncounterDate.GetDate3DS(),

            Language = lang,
            OT_Name = tr.OT,
            OT_Gender = tr.Gender,
            ID32 = tr.ID32,
        };
        if (tr is IRegionOrigin r)
            r.CopyRegionOrigin(pk);
        else
            pk.SetDefaultRegionOrigins();

        SetPINGA(pk, criteria);
        EncounterUtil1.SetEncounterMoves(pk, Version, LevelMin);
        pk.Nickname = SpeciesName.GetSpeciesNameGeneration(Species, lang, Generation);
        if (CanDexNav)
        {
            var eggMoves = GetDexNavMoves();
            if (eggMoves.Length > 0)
                pk.RelearnMove1 = eggMoves[Util.Rand.Next(eggMoves.Length)];
        }
        pk.SetRandomMemory6();
        pk.ResetPartyStats();
        return pk;
    }

    private byte GetWildForm(byte form)
    {
        if (form != EncounterUtil1.FormRandom)
            return form;
        // flagged as totally random
        return (byte)Util.Rand.Next(PersonalTable.AO[Species].FormCount);
    }

    private void SetPINGA(PK6 pk, EncounterCriteria criteria)
    {
        pk.PID = Util.Rand32();
        pk.EncryptionConstant = Util.Rand32();
        pk.Nature = (int)criteria.GetNature(Nature.Random);
        pk.Gender = criteria.GetGender(-1, PersonalTable.AO.GetFormEntry(pk.Species, pk.Form));
        pk.RefreshAbility(criteria.GetAbilityFromNumber(Ability));
        pk.SetRandomIVs();
    }
    #endregion

    #region Matching
    public bool IsMatchExact(PKM pk, EvoCriteria evo) => true; // Matched by Area
    public EncounterMatchRating GetMatchRating(PKM pk)
    {
        if (IsDeferredWurmple(pk))
            return EncounterMatchRating.PartialMatch;

        bool isHidden = pk.AbilityNumber == 4;
        if (isHidden && this.IsPartialMatchHidden(pk.Species, Species))
            return EncounterMatchRating.PartialMatch;
        if (IsDeferredHiddenAbility(isHidden))
            return EncounterMatchRating.Deferred;
        return EncounterMatchRating.Match;
    }

    private bool IsDeferredWurmple(PKM pk) => Species == (int)Core.Species.Wurmple && pk.Species != (int)Core.Species.Wurmple && !WurmpleUtil.IsWurmpleEvoValid(pk);
    #endregion
}
