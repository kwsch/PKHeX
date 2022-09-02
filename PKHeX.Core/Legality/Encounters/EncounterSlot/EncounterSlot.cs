using System;
using static PKHeX.Core.Species;

namespace PKHeX.Core;

/// <summary>
/// Wild Encounter Slot data
/// </summary>
/// <remarks>Wild encounter slots are found as random encounters in-game.</remarks>
public abstract record EncounterSlot(EncounterArea Area, ushort Species, byte Form, byte LevelMin, byte LevelMax) : IEncounterable, IEncounterMatch
{
    public abstract int Generation { get; }
    public abstract EntityContext Context { get; }
    public bool EggEncounter => false;
    public virtual bool IsShiny => false;

    protected readonly EncounterArea Area = Area;
    public GameVersion Version => Area.Version;
    public int Location => Area.Location;
    public int EggLocation => 0;
    public virtual Ball FixedBall => Ball.None;
    public virtual Shiny Shiny => Shiny.Random;

    public bool IsFixedLevel => LevelMin == LevelMax;
    public bool IsRandomLevel => LevelMin != LevelMax;

    private protected const string wild = "Wild Encounter";
    public string Name => wild;

    /// <summary>
    /// Gets if the specified level inputs are within range of the <see cref="LevelMin"/> and <see cref="LevelMax"/>
    /// </summary>
    /// <param name="lvl">Single level</param>
    /// <returns>True if within slot's range, false if impossible.</returns>
    public bool IsLevelWithinRange(int lvl) => LevelMin <= lvl && lvl <= LevelMax;

    /// <summary>
    /// Gets if the specified level inputs are within range of the <see cref="LevelMin"/> and <see cref="LevelMax"/>
    /// </summary>
    /// <param name="min">Highest value the low end of levels can be</param>
    /// <param name="max">Lowest value the high end of levels can be</param>
    /// <returns>True if within slot's range, false if impossible.</returns>
    public bool IsLevelWithinRange(byte min, byte max) => LevelMin <= max && min <= LevelMax;

    /// <summary>
    /// Gets if the specified level inputs are within range of the <see cref="LevelMin"/> and <see cref="LevelMax"/>
    /// </summary>
    /// <param name="lvl">Single level</param>
    /// <param name="minDecrease">Highest value the low end of levels can be</param>
    /// <param name="maxIncrease">Lowest value the high end of levels can be</param>
    /// <returns>True if within slot's range, false if impossible.</returns>
    public bool IsLevelWithinRange(int lvl, int minDecrease, int maxIncrease) => LevelMin - minDecrease <= lvl && lvl <= LevelMax + maxIncrease;

    /// <summary>
    /// Gets if the specified level inputs are within range of the <see cref="LevelMin"/> and <see cref="LevelMax"/>
    /// </summary>
    /// <param name="min">Lowest level allowed</param>
    /// <param name="max">Highest level allowed</param>
    /// <param name="minDecrease">Highest value the low end of levels can be</param>
    /// <param name="maxIncrease">Lowest value the high end of levels can be</param>
    /// <returns>True if within slot's range, false if impossible.</returns>
    public bool IsLevelWithinRange(byte min, byte max, int minDecrease, int maxIncrease) => LevelMin - minDecrease <= max && min <= LevelMax + maxIncrease;

    public virtual string LongName
    {
        get
        {
            if (Area.Type == SlotType.Any)
                return wild;
            return $"{wild} {Area.Type.ToString().Replace('_', ' ')}";
        }
    }

    public PKM ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr, EncounterCriteria.Unrestricted);

    public PKM ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria)
    {
        var pk = GetBlank();
        tr.ApplyTo(pk);
        ApplyDetails(tr, criteria, pk);
        return pk;
    }

    protected virtual PKM GetBlank() => EntityBlank.GetBlank(Generation, Version);

    protected virtual void ApplyDetails(ITrainerInfo sav, EncounterCriteria criteria, PKM pk)
    {
        var version = this.GetCompatibleVersion((GameVersion) sav.Game);
        int lang = (int)Language.GetSafeLanguage(Generation, (LanguageID) sav.Language, version);
        int level = LevelMin;
        pk.Species = Species;
        pk.Form = GetWildForm(pk, Form, sav);
        pk.Language = lang;
        pk.CurrentLevel = level;
        pk.Version = (int)version;
        pk.Nickname = SpeciesName.GetSpeciesNameGeneration(Species, lang, Generation);

        ApplyDetailsBall(pk);
        pk.Language = lang;
        pk.OT_Friendship = pk.PersonalInfo.BaseFriendship;

        SetMetData(pk, level, Location);
        SetPINGA(pk, criteria);
        SetEncounterMoves(pk, version, pk.CurrentLevel);

        SetFormatSpecificData(pk);

        if (pk.Format < 6)
            return;

        sav.ApplyHandlingTrainerInfo(pk);
        if (pk is IScaledSize { HeightScalar: 0, WeightScalar: 0 } s)
        {
            s.HeightScalar = PokeSizeUtil.GetRandomScalar();
            s.WeightScalar = PokeSizeUtil.GetRandomScalar();
        }
    }

    protected virtual void ApplyDetailsBall(PKM pk)
    {
        var ball = FixedBall;
        pk.Ball = (int)(ball == Ball.None ? Ball.Poke : ball);
    }

    protected virtual void SetEncounterMoves(PKM pk, GameVersion version, int level)
    {
        Span<ushort> moves = stackalloc ushort[4];
        MoveLevelUp.GetEncounterMoves(moves, pk, level, version);
        pk.SetMoves(moves);
        pk.SetMaximumPPCurrent(moves);
    }

    protected virtual void SetFormatSpecificData(PKM pk) { }

    protected virtual void SetPINGA(PKM pk, EncounterCriteria criteria)
    {
        var pi = pk.PersonalInfo;
        int gender = criteria.GetGender(-1, pi);
        int nature = (int)criteria.GetNature(Nature.Random);
        var ability = criteria.GetAbilityFromNumber(Ability);

        if (Generation == 3 && Species == (int)Unown)
        {
            do
            {
                PIDGenerator.SetRandomWildPID(pk, pk.Format, nature, ability, gender);
                ability ^= 1; // some nature-forms cannot have a certain PID-ability set, so just flip it as Unown doesn't have dual abilities.
            } while (pk.Form != Form);
        }
        else
        {
            PIDGenerator.SetRandomWildPID(pk, pk.Format, nature, ability, gender);
        }

        pk.Gender = gender;
        pk.StatNature = nature;
    }

    private void SetMetData(PKM pk, int level, int location)
    {
        if (pk.Format <= 2 && Version != GameVersion.C)
            return;

        pk.Met_Location = location;
        pk.Met_Level = level;

        if (pk.Format >= 4)
            pk.MetDate = DateTime.Today;
    }

    public bool IsRandomUnspecificForm => Form >= FormDynamic;
    private const int FormDynamic = FormVivillon;
    protected internal const byte FormVivillon = 30;
    protected internal const byte FormRandom = 31;

    private static byte GetWildForm(PKM pk, byte form, ITrainerInfo sav)
    {
        if (form < FormDynamic) // specified form
            return form;

        if (form == FormRandom) // flagged as totally random
        {
            if (pk.Species == (int)Minior)
                return (byte)Util.Rand.Next(7, 14);
            return (byte)Util.Rand.Next(pk.PersonalInfo.FormCount);
        }

        ushort species = pk.Species;
        if (species is >= (int)Scatterbug and <= (int)Vivillon)
        {
            if (sav is IRegionOrigin o)
                return Vivillon3DS.GetPattern(o.Country, o.Region);
        }
        return 0;
    }

    public virtual string GetConditionString(out bool valid)
    {
        valid = true;
        return LegalityCheckStrings.LEncCondition;
    }

    public bool IsMatchExact(PKM pk, EvoCriteria evo) => true; // Matched by Area

    public virtual EncounterMatchRating GetMatchRating(PKM pk)
    {
        if (IsDeferredWurmple(pk))
            return EncounterMatchRating.PartialMatch;

        if (pk.Format >= 5)
        {
            bool isHidden = pk.AbilityNumber == 4;
            if (isHidden && this.IsPartialMatchHidden(pk.Species, Species))
                return EncounterMatchRating.PartialMatch;
            if (IsDeferredHiddenAbility(isHidden))
                return EncounterMatchRating.Deferred;
        }

        return EncounterMatchRating.Match;
    }

    protected virtual HiddenAbilityPermission IsHiddenAbilitySlot() => HiddenAbilityPermission.Never;

    public AbilityPermission Ability => IsHiddenAbilitySlot() switch
    {
        HiddenAbilityPermission.Never => AbilityPermission.Any12,
        HiddenAbilityPermission.Always => AbilityPermission.OnlyHidden,
        _ => AbilityPermission.Any12H,
    };

    private bool IsDeferredWurmple(PKM pk) => Species == (int)Wurmple && pk.Species != (int)Wurmple && !WurmpleUtil.IsWurmpleEvoValid(pk);

    private bool IsDeferredHiddenAbility(bool IsHidden) => IsHiddenAbilitySlot() switch
    {
        HiddenAbilityPermission.Never => IsHidden,
        HiddenAbilityPermission.Always => !IsHidden,
        _ => false,
    };

    protected enum HiddenAbilityPermission
    {
        Always,
        Never,
        Possible,
    }
}
