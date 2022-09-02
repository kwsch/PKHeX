using System;

namespace PKHeX.Core;

/// <summary>
/// Static Encounter Data
/// </summary>
/// <remarks>
/// Static Encounters are fixed position encounters with properties that are not subject to Wild Encounter conditions.
/// </remarks>
public abstract record EncounterStatic(GameVersion Version) : IEncounterable, IMoveset, IEncounterMatch
{
    public ushort Species { get; init; }
    public byte Form { get; init; }
    public virtual byte Level { get; init; }
    public virtual byte LevelMin => Level;
    public virtual byte LevelMax => Level;
    public abstract int Generation { get; }
    public abstract EntityContext Context { get; }

    public virtual int Location { get; init; }
    public AbilityPermission Ability { get; init; }
    public Shiny Shiny { get; init; }
    public Nature Nature { get; init; } = Nature.Random;
    public sbyte Gender { get; init; } = -1;

    public ushort HeldItem { get; init; }
    public bool Gift { get; init; }
    public bool Fateful { get; init; }

    public byte EggCycles { get; init; }
    public byte FlawlessIVCount { get; init; }
    public byte Ball { get; init; } = 4; // Only checked when is Gift

    public int EggLocation { get; init; }

    public Ball FixedBall => Gift ? (Ball)Ball : Core.Ball.None;

    public Moveset Moves { get; init; }
    public IndividualValueSet IVs { get; init; }

    public virtual bool EggEncounter => EggLocation != 0;

    private const string _name = "Static Encounter";
    public string Name => _name;
    public string LongName => $"{_name} ({Version})";
    public bool IsShiny => Shiny.IsShiny();

    public bool IsRandomUnspecificForm => Form >= FormDynamic;
    private const int FormDynamic = FormVivillon;
    internal const int FormVivillon = 30;
    //protected const int FormRandom = 31;

    protected virtual PKM GetBlank(ITrainerInfo tr) => EntityBlank.GetBlank(Generation, Version);

    public PKM ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr, EncounterCriteria.Unrestricted);

    public PKM ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria)
    {
        var pk = GetBlank(tr);
        tr.ApplyTo(pk);

        ApplyDetails(tr, criteria, pk);
        return pk;
    }

    protected virtual void ApplyDetails(ITrainerInfo tr, EncounterCriteria criteria, PKM pk)
    {
        pk.EncryptionConstant = Util.Rand32();
        pk.Species = Species;
        pk.Form = Form;

        var version = this.GetCompatibleVersion((GameVersion)tr.Game);
        int lang = (int)Language.GetSafeLanguage(Generation, (LanguageID)tr.Language, version);
        int level = GetMinimalLevel();

        pk.Version = (int)version;
        pk.Language = lang = GetEdgeCaseLanguage(pk, lang);
        pk.Nickname = SpeciesName.GetSpeciesNameGeneration(Species, lang, Generation);

        pk.CurrentLevel = level;
        ApplyDetailsBall(pk);
        pk.HeldItem = HeldItem;
        pk.OT_Friendship = pk.PersonalInfo.BaseFriendship;

        var today = DateTime.Today;
        SetMetData(pk, level, today);
        if (EggEncounter)
            SetEggMetData(pk, tr, today);

        SetPINGA(pk, criteria);
        SetEncounterMoves(pk, version, level);

        if (Fateful)
            pk.FatefulEncounter = true;

        if (pk.Format < 6)
            return;

        if (this is IRelearn relearn)
            pk.SetRelearnMoves(relearn.Relearn);

        tr.ApplyHandlingTrainerInfo(pk);

        if (pk is IScaledSize { HeightScalar: 0, WeightScalar: 0 } s)
        {
            s.HeightScalar = PokeSizeUtil.GetRandomScalar();
            s.WeightScalar = PokeSizeUtil.GetRandomScalar();
        }
        if (this is IGigantamax g && pk is PK8 pg)
            pg.CanGigantamax = g.CanGigantamax;
        if (this is IDynamaxLevel d && pk is PK8 pd)
            pd.DynamaxLevel = d.DynamaxLevel;
    }

    protected virtual void ApplyDetailsBall(PKM pk) => pk.Ball = Ball;

    protected virtual int GetMinimalLevel() => LevelMin;

    protected virtual void SetPINGA(PKM pk, EncounterCriteria criteria)
    {
        var pi = pk.PersonalInfo;
        int gender = criteria.GetGender(Gender, pi);
        int nature = (int)criteria.GetNature(Nature);
        int ability = criteria.GetAbilityFromNumber(Ability);

        var pidtype = GetPIDType();
        PIDGenerator.SetRandomWildPID(pk, pk.Format, nature, ability, gender, pidtype);
        SetIVs(pk);
        pk.StatNature = pk.Nature;
    }

    private void SetEggMetData(PKM pk, ITrainerInfo tr, DateTime today)
    {
        pk.Met_Location = Math.Max(0, EncounterSuggestion.GetSuggestedEggMetLocation(pk));
        pk.Met_Level = EncounterSuggestion.GetSuggestedEncounterEggMetLevel(pk);

        if (Generation >= 4)
        {
            bool traded = (int)Version == tr.Game;
            pk.Egg_Location = EncounterSuggestion.GetSuggestedEncounterEggLocationEgg(Generation, Version, traded);
            pk.EggMetDate = today;
        }
        pk.Egg_Location = EggLocation;
        pk.EggMetDate = today;
    }

    protected virtual void SetMetData(PKM pk, int level, DateTime today)
    {
        if (pk.Format <= 2)
            return;

        pk.Met_Location = Location;
        pk.Met_Level = level;
        if (pk.Format >= 4)
            pk.MetDate = today;
    }

    protected virtual void SetEncounterMoves(PKM pk, GameVersion version, int level)
    {
        if (Moves.HasMoves)
        {
            pk.SetMoves(Moves);
            pk.SetMaximumPPCurrent(Moves);
        }
        else
        {
            Span<ushort> moves = stackalloc ushort[4];
            MoveLevelUp.GetEncounterMoves(moves, pk, level, version);
            pk.SetMoves(moves);
            pk.SetMaximumPPCurrent(moves);
        }
    }

    protected void SetIVs(PKM pk)
    {
        if (IVs.IsSpecified)
            pk.SetRandomIVsTemplate(IVs, FlawlessIVCount);
        else if (FlawlessIVCount > 0)
            pk.SetRandomIVs(minFlawless: FlawlessIVCount);
    }

    private int GetEdgeCaseLanguage(PKM pk, int lang)
    {
        switch (this)
        {
            // Cannot trade between languages
            case IFixedGBLanguage e:
                return e.Language == EncounterGBLanguage.Japanese ? 1 : 2;

            // E-Reader was only available to Japanese games.
            case EncounterStaticShadow {EReader: true}:
            // Old Sea Map was only distributed to Japanese games.
            case EncounterStatic3 when Species == (int)Core.Species.Mew:
                pk.OT_Name = "ゲーフリ";
                return (int)LanguageID.Japanese;

            // Deoxys for Emerald was not available for Japanese games.
            case EncounterStatic3 when Species == (int)Core.Species.Deoxys && Version == GameVersion.E && lang == 1:
                pk.OT_Name = "GF";
                return (int)LanguageID.English;

            default:
                return lang;
        }
    }

    private PIDType GetPIDType()
    {
        switch (Generation)
        {
            case 3 when this is EncounterStatic3 {Roaming: true, Version: not GameVersion.E}: // Roamer IV glitch was fixed in Emerald
                return PIDType.Method_1_Roamer;
            case 4 when Shiny == Shiny.Always: // Lake of Rage Gyarados
                return PIDType.ChainShiny;
            case 4 when Species == (int)Core.Species.Pichu: // Spiky Eared Pichu
            case 4 when Location == Locations.PokeWalker4: // Pokéwalker
                return PIDType.Pokewalker;
            case 5 when Shiny == Shiny.Always:
                return PIDType.G5MGShiny;

            default: return PIDType.None;
        }
    }

    public virtual bool IsMatchExact(PKM pk, EvoCriteria evo)
    {
        if (Nature != Nature.Random && pk.Nature != (int) Nature)
            return false;

        if (!IsMatchEggLocation(pk))
            return false;
        if (!IsMatchLocation(pk))
            return false;
        if (!IsMatchLevel(pk, evo))
            return false;
        if (!IsMatchGender(pk))
            return false;
        if (!IsMatchForm(pk, evo))
            return false;
        if (!IsMatchIVs(pk))
            return false;

        if (this is IContestStats es && pk is IContestStats s && s.IsContestBelow(es))
            return false;

        // Defer to EC/PID check
        // if (e.Shiny != null && e.Shiny != pk.IsShiny)
        // continue;

        // Defer ball check to later
        // if (e.Gift && pk.Ball != 4) // PokéBall
        // continue;

        return true;
    }

    private bool IsMatchIVs(PKM pk)
    {
        if (!IVs.IsSpecified)
            return true; // nothing to check, IVs are random
        if (Generation <= 2 && pk.Format > 2)
            return true; // IVs are regenerated on VC transfer upward

        return Legal.GetIsFixedIVSequenceValidSkipRand(IVs, pk);
    }

    protected virtual bool IsMatchForm(PKM pk, EvoCriteria evo)
    {
        if (IsRandomUnspecificForm)
            return true;
        return Form == evo.Form || FormInfo.IsFormChangeable(Species, Form, pk.Form, pk.Format);
    }

    // override me if the encounter type has any eggs
    protected virtual bool IsMatchEggLocation(PKM pk)
    {
        var expect = pk is PB8 ? Locations.Default8bNone : 0;
        return pk.Egg_Location == expect;
    }

    private bool IsMatchGender(PKM pk)
    {
        if (Gender == -1 || Gender == pk.Gender)
            return true;

        if (Species == (int) Core.Species.Azurill && Generation == 4 && Location == 233 && pk.Gender == 0)
            return EntityGender.GetFromPIDAndRatio(pk.PID, 0xBF) == 1;

        return false;
    }

    protected virtual bool IsMatchLocation(PKM pk)
    {
        if (EggEncounter)
            return true;
        if (Location == 0)
            return true;
        if (!pk.HasOriginalMetLocation)
            return true;
        return Location == pk.Met_Location;
    }

    protected virtual bool IsMatchLevel(PKM pk, EvoCriteria evo)
    {
        return pk.Met_Level == Level;
    }

    public virtual EncounterMatchRating GetMatchRating(PKM pk)
    {
        if (IsMatchPartial(pk))
            return EncounterMatchRating.PartialMatch;
        return IsMatchDeferred(pk);
    }

    /// <summary>
    /// Checks if the provided <see cref="pk"/> might not be the best match, or even a bad match due to minor reasons.
    /// </summary>
    protected virtual EncounterMatchRating IsMatchDeferred(PKM pk) => EncounterMatchRating.Match;

    /// <summary>
    /// Checks if the provided <see cref="pk"/> is not an exact match due to minor reasons.
    /// </summary>
    protected virtual bool IsMatchPartial(PKM pk)
    {
        if (pk.Format >= 5 && pk.AbilityNumber == 4 && this.IsPartialMatchHidden(pk.Species, Species))
            return true;
        return pk.FatefulEncounter != Fateful;
    }
}
