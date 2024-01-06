using System;

namespace PKHeX.Core;

/// <summary>
/// Encounter Slot found in <see cref="GameVersion.ORAS"/>.
/// </summary>
public sealed record EncounterSlot6AO(EncounterArea6AO Parent, ushort Species, byte Form, byte LevelMin, byte LevelMax)
    : IEncounterable, IEncounterMatch, IEncounterConvertible<PK6>, IEncounterFormRandom
{
    public int Generation => 6;
    public EntityContext Context => EntityContext.Gen6;
    public bool EggEncounter => false;
    public Ball FixedBall => Ball.None;
    public Shiny Shiny => Shiny.Random;
    public bool IsShiny => false;
    public int EggLocation => 0;
    public bool IsRandomUnspecificForm => Form >= EncounterUtil.FormDynamic;

    public string Name => $"Wild Encounter ({Version})";
    public string LongName => $"{Name} {Type.ToString().Replace('_', ' ')}";
    public GameVersion Version => Parent.Version;
    public int Location => Parent.Location;
    public SlotType Type => Parent.Type;
    public bool CanDexNav => Type != SlotType.Rock_Smash;
    public bool IsHorde => Type == SlotType.Horde;

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
        var pi = PersonalTable.AO[Species];
        var pk = new PK6
        {
            Species = Species,
            Form = GetWildForm(Form),
            CurrentLevel = LevelMin,
            OT_Friendship = pi.BaseFriendship,
            Met_Location = Location,
            Met_Level = LevelMin,
            Version = (byte)Version,
            Ball = (byte)Ball.Poke,
            MetDate = EncounterDate.GetDate3DS(),

            Language = lang,
            OT_Name = tr.OT,
            OT_Gender = tr.Gender,
            ID32 = tr.ID32,
            Nickname = SpeciesName.GetSpeciesNameGeneration(Species, lang, Generation),
        };
        if (tr is IRegionOrigin r)
            r.CopyRegionOrigin(pk);
        else
            pk.SetDefaultRegionOrigins(lang);

        SetPINGA(pk, criteria, pi);
        EncounterUtil.SetEncounterMoves(pk, Version, LevelMin);
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
        if (form != EncounterUtil.FormRandom)
            return form;
        // flagged as totally random
        return (byte)Util.Rand.Next(PersonalTable.AO[Species].FormCount);
    }

    private void SetPINGA(PK6 pk, EncounterCriteria criteria, PersonalInfo6AO pi)
    {
        pk.PID = Util.Rand32();
        pk.EncryptionConstant = Util.Rand32();
        pk.Nature = (int)criteria.GetNature();
        pk.Gender = criteria.GetGender(pi);
        pk.RefreshAbility(criteria.GetAbilityFromNumber(Ability));
        criteria.SetRandomIVs(pk);
    }
    #endregion

    #region Matching

    private const int FluteBoostMin = 4; // White Flute decreases levels.
    private const int FluteBoostMax = 4; // Black Flute increases levels.
    private const int DexNavBoost = 30; // Maximum DexNav chain

    public bool IsMatchExact(PKM pk, EvoCriteria evo)
    {
        var boostMax = Type != SlotType.Rock_Smash ? DexNavBoost : FluteBoostMax;
        const int boostMin = FluteBoostMin;
        if (!this.IsLevelWithinRange(pk.Met_Level, boostMin, boostMax))
            return false;

        if (evo.Form != Form && !IsRandomUnspecificForm)
            return false;

        return true;
    }

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
