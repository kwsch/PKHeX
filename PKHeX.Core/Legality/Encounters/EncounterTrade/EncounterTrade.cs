using System;
using System.Collections.Generic;

namespace PKHeX.Core;

/// <summary>
/// Trade Encounter data
/// </summary>
/// <remarks>
/// Trade data is fixed level in all cases except for the first few generations of games.
/// </remarks>
public abstract record EncounterTrade(GameVersion Version) : IEncounterable, IMoveset, IEncounterMatch
{
    public ushort Species { get; init; }
    public byte Form { get; init; }
    public byte Level { get; init; }
    public virtual byte LevelMin => Level;
    public byte LevelMax => 100;
    public abstract int Generation { get; }
    public abstract EntityContext Context { get; }

    public int CurrentLevel { get; init; } = -1;
    public abstract int Location { get; }

    public AbilityPermission Ability { get; init; }
    public Nature Nature { get; init; } = Nature.Random;
    public virtual Shiny Shiny => Shiny.Never;
    public sbyte Gender { get; init; } = -1;

    public sbyte OTGender { get; init; } = -1;
    public bool IsNicknamed { get; init; } = true;
    public bool EvolveOnTrade { get; init; }
    public byte Ball { get; init; } = 4;

    public int EggLocation { get; init; }

    public ushort TID { get; init; }
    public ushort SID { get; init; }

    public Moveset Moves { get; init; }
    public IndividualValueSet IVs { get; init; }

    public Ball FixedBall => (Ball)Ball;
    public bool EggEncounter => false;

    public int TID7
    {
        init
        {
            TID = (ushort) value;
            SID = (ushort)(value >> 16);
        }
    }

    private const string _name = "In-game Trade";
    public string Name => _name;
    public string LongName => _name;
    public bool IsShiny => Shiny.IsShiny();

    public IReadOnlyList<string> Nicknames { get; internal set; } = Array.Empty<string>();
    public IReadOnlyList<string> TrainerNames { get; internal set; } = Array.Empty<string>();
    public string GetNickname(int language) => (uint)language < Nicknames.Count ? Nicknames[language] : string.Empty;
    public string GetOT(int language) => (uint)language < TrainerNames.Count ? TrainerNames[language] : string.Empty;
    public bool HasNickname => Nicknames.Count != 0 && IsNicknamed;
    public bool HasTrainerName => TrainerNames.Count != 0;

    public PKM ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr, EncounterCriteria.Unrestricted);

    public PKM ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria)
    {
        var pk = EntityBlank.GetBlank(Generation, Version);
        tr.ApplyTo(pk);

        ApplyDetails(tr, criteria, pk);
        return pk;
    }

    protected virtual void ApplyDetails(ITrainerInfo sav, EncounterCriteria criteria, PKM pk)
    {
        var version = this.GetCompatibleVersion((GameVersion)sav.Game);
        int lang = (int)Language.GetSafeLanguage(Generation, (LanguageID)sav.Language, version);
        int level = CurrentLevel > 0 ? CurrentLevel : LevelMin;
        if (level == 0)
            level = Math.Max((byte)1, LevelMin);

        ushort species = Species;
        if (EvolveOnTrade)
            species++;

        pk.EncryptionConstant = Util.Rand32();
        pk.Species = species;
        pk.Form = Form;
        pk.Language = lang;
        pk.OT_Name = pk.Format == 1 ? StringConverter12.G1TradeOTStr : HasTrainerName ? GetOT(lang) : sav.OT;
        pk.OT_Gender = HasTrainerName ? Math.Max(0, (int)OTGender) : sav.Gender;
        pk.SetNickname(HasNickname ? GetNickname(lang) : string.Empty);

        pk.CurrentLevel = level;
        pk.Version = (int) version;
        pk.TID = TID;
        pk.SID = SID;
        pk.Ball = Ball;
        pk.OT_Friendship = pk.PersonalInfo.BaseFriendship;

        SetPINGA(pk, criteria);
        SetMoves(pk, version, level);

        var time = DateTime.Now;
        if (pk.Format != 2 || version == GameVersion.C)
        {
            SetMetData(pk, level, Location, time);
        }
        else
        {
            pk.OT_Gender = 0;
        }

        if (EggLocation != 0)
            SetEggMetData(pk, time);

        if (pk.Format < 6)
            return;

        sav.ApplyHandlingTrainerInfo(pk, force: true);
        pk.SetRandomEC();

        if (pk is IScaledSize s)
        {
            s.HeightScalar = PokeSizeUtil.GetRandomScalar();
            s.WeightScalar = PokeSizeUtil.GetRandomScalar();
        }
        if (pk is PK6 pk6)
            pk6.SetRandomMemory6();
    }

    protected virtual void SetPINGA(PKM pk, EncounterCriteria criteria)
    {
        var pi = pk.PersonalInfo;
        int gender = criteria.GetGender(Gender, pi);
        int nature = (int)criteria.GetNature(Nature);
        int ability = criteria.GetAbilityFromNumber(Ability);

        PIDGenerator.SetRandomWildPID(pk, Generation, nature, ability, gender);
        pk.Nature = pk.StatNature = nature;
        pk.Gender = gender;
        pk.RefreshAbility(ability);

        SetIVs(pk);
    }

    protected void SetIVs(PKM pk)
    {
        if (IVs.IsSpecified)
            pk.SetRandomIVsTemplate(IVs, 0);
        else
            pk.SetRandomIVs(minFlawless: 3);
    }

    private void SetMoves(PKM pk, GameVersion version, int level)
    {
        if (Moves.HasMoves)
        {
            pk.SetMoves(Moves);
            pk.SetMaximumPPCurrent(Moves);
        }
        else
        {
            var moves = MoveLevelUp.GetEncounterMoves(pk, level, version);
            pk.SetMoves(moves);
            pk.SetMaximumPPCurrent(moves);
        }
    }

    private void SetEggMetData(PKM pk, DateTime time)
    {
        pk.Egg_Location = EggLocation;
        pk.EggMetDate = time;
    }

    private static void SetMetData(PKM pk, int level, int location, DateTime time)
    {
        pk.Met_Level = level;
        pk.Met_Location = location;
        pk.MetDate = time;
    }

    public virtual bool IsMatchExact(PKM pk, EvoCriteria evo)
    {
        if (IVs.IsSpecified)
        {
            if (!Legal.GetIsFixedIVSequenceValidSkipRand(IVs, pk))
                return false;
        }

        if (!IsMatchNatureGenderShiny(pk))
            return false;
        if (TID != pk.TID)
            return false;
        if (SID != pk.SID)
            return false;

        if (!IsMatchLevel(pk, evo))
            return false;

        if (CurrentLevel != -1 && CurrentLevel > pk.CurrentLevel)
            return false;

        if (Form != evo.Form && !FormInfo.IsFormChangeable(Species, Form, pk.Form, pk.Format))
            return false;
        if (OTGender != -1 && OTGender != pk.OT_Gender)
            return false;
        if (!IsMatchEggLocation(pk))
            return false;
        // if (z.Ability == 4 ^ pk.AbilityNumber == 4) // defer to Ability
        //    continue;
        if (!Version.Contains((GameVersion)pk.Version))
            return false;

        return true;
    }

    protected virtual bool IsMatchEggLocation(PKM pk)
    {
        var expect = EggLocation;
        if (pk is PB8 && expect is 0)
            expect = Locations.Default8bNone;
        return pk.Egg_Location == expect;
    }

    private bool IsMatchLevel(PKM pk, EvoCriteria evo)
    {
        if (!pk.HasOriginalMetLocation)
            return evo.LevelMax >= Level;

        if (Location != pk.Met_Location)
            return false;

        if (pk.Format < 5)
            return evo.LevelMax >= Level;

        return pk.Met_Level == Level;
    }

    protected virtual bool IsMatchNatureGenderShiny(PKM pk)
    {
        if (!Shiny.IsValid(pk))
            return false;
        if (Gender != -1 && Gender != pk.Gender)
            return false;

        if (Nature != Nature.Random && pk.Nature != (int)Nature)
            return false;

        return true;
    }

    public EncounterMatchRating GetMatchRating(PKM pk)
    {
        if (IsMatchPartial(pk))
            return EncounterMatchRating.PartialMatch;
        if (IsMatchDeferred(pk))
            return EncounterMatchRating.Deferred;
        return EncounterMatchRating.Match;
    }

    protected virtual bool IsMatchDeferred(PKM pk) => false;
    protected virtual bool IsMatchPartial(PKM pk) => false;
}
