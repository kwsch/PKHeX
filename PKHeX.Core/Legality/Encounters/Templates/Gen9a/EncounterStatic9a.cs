using System;
using static PKHeX.Core.AbilityPermission;

namespace PKHeX.Core;

/// <summary>
/// Static Encounter found in <see cref="GameVersion.ZA"/>.
/// </summary>
public sealed record EncounterStatic9a(ushort Species, byte Form, byte Level, byte Size = EncounterStatic9a.NoScale)
    : IEncounter9a, IEncounterConvertible<PA9>, IFixedGender, IFixedNature, IFixedIVSet, IMoveset
{
    public byte Generation => 9;
    private const GameVersion Version = GameVersion.ZA;
    GameVersion IVersion.Version => GameVersion.ZA;
    public EntityContext Context => EntityContext.Gen9a;
    public bool IsEgg => false;
    public AbilityPermission Ability => Any12;
    public Ball FixedBall => Ball.None;
    public bool IsShiny => false;
    public ushort EggLocation => 0;
    public byte FlawlessIVCount { get; init; }
    public byte LevelMin => Level;
    public byte LevelMax => Level;

    private const byte NoScale = 0;
    private bool NoScalarsDefined => Size == NoScale;

    public Moveset Moves { get; init; }
    public IndividualValueSet IVs { get; init; }
    public Nature Nature { get; init; } = Nature.Random;
    public required ushort Location { get; init; }
    public byte Gender { get; init; } = FixedGenderUtil.GenderRandom;
    public Shiny Shiny { get; init; } = Shiny.Never;
    public bool IsAlpha { get; init; }

    ushort ILocation.Location => Location;

    private bool IsHyperspace => Location == EncounterArea9a.LocationHyperspace;
    private bool IsHyperspaceShinyPossible => Shiny != Shiny.Never && IsHyperspace;

    public string Name => "Static Encounter";
    public string LongName => Name;

    #region Generating

    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria) => ConvertToPKM(tr, criteria);
    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr);
    public PA9 ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr, EncounterCriteria.Unrestricted);
    public PA9 ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria)
    {
        int lang = (int)Language.GetSafeLanguage9a((LanguageID)tr.Language);
        var pi = PersonalTable.ZA[Species, Form];
        var pk = new PA9
        {
            Language = lang,
            Species = Species,
            Form = Form,
            CurrentLevel = LevelMin,
            OriginalTrainerFriendship = pi.BaseFriendship,
            MetLocation = Location,
            MetLevel = LevelMin,
            MetDate = EncounterDate.GetDateSwitch(),
            Version = Version,
            IsAlpha = IsAlpha,
            Ball = (byte)Ball.Poke,

            Nickname = SpeciesName.GetSpeciesNameGeneration(Species, lang, Generation),
            ObedienceLevel = LevelMin,
            OriginalTrainerName = tr.OT,
            OriginalTrainerGender = tr.Gender,
            ID32 = tr.ID32,
        };

        SetPINGA(pk, criteria, pi);
        SetMoves(pk, pi, LevelMin);
        pk.HealPP();

        pk.ResetPartyStats();
        return pk;
    }

    private void SetPINGA(PA9 pk, EncounterCriteria criteria, PersonalInfo9ZA pi)
    {
        var generate = criteria;
        if (IVs.IsSpecified || Correlation is LumioseCorrelation.ReApplyIVs)
            generate = criteria.WithoutIVs();

        bool shinyPlease = criteria.Shiny.IsShiny();
        var param = GetParams(pi, shinyPlease, shinyPlease);
        ulong init = Util.Rand.Rand64();
        var success = this.TryApply64(pk, init, param, generate);
        if (!success && !this.TryApply64(pk, init, param, generate.WithoutIVs()))
            this.TryApply64(pk, init, param, EncounterCriteria.Unrestricted);

        if (IVs.IsSpecified)
            criteria.SetRandomIVs(pk, IVs);
        else if (Correlation is LumioseCorrelation.ReApplyIVs)
            criteria.SetRandomIVs(pk, FlawlessIVCount);
    }

    private void SetMoves(PA9 pk, PersonalInfo9ZA pi, byte level)
    {
        var (learn, plus) = LearnSource9ZA.GetLearnsetAndPlus(Species, Form);
        pk.SetPlusFlagsEncounter(pi, plus, level);
        if (Moves.HasMoves)
        {
            pk.SetMoves(Moves);
            return;
        }

        Span<ushort> moves = stackalloc ushort[4];
        learn.SetEncounterMovesBackwards(level, moves, sameDescend: false);
        if (pk.IsAlpha)
            pk.SetPlusFlagsSpecific(pi, moves[0] = pi.AlphaMove);
        pk.SetMoves(moves);
    }

    #endregion

    #region Matching

    public bool IsMatchExact(PKM pk, EvoCriteria evo)
    {
        if (!this.IsLevelWithinRange(pk.MetLevel))
            return false;
        if (Gender != FixedGenderUtil.GenderRandom && pk.Gender != Gender)
            return false;
        if (!IsMatchEggLocation(pk))
            return false;
        if (!IsMatchLocation(pk))
            return false;
        if (Form != evo.Form && !FormInfo.IsFormChangeable(Species, Form, pk.Form, Context, pk.Context))
            return false;
        if (IVs.IsSpecified && !Legal.GetIsFixedIVSequenceValidSkipRand(IVs, pk))
            return false;
        if (FlawlessIVCount != 0 && pk.FlawlessIVCount < FlawlessIVCount)
            return false;
        if (Nature != Nature.Random && pk.Nature != Nature)
            return false;
        if (pk is IAlphaReadOnly a && a.IsAlpha != IsAlpha)
            return false;

        return true;
    }

    private bool IsMatchEggLocation(PKM pk) => pk.EggLocation == EggLocation;
    private bool IsMatchLocation(PKM pk)
    {
        var loc = pk.MetLocation;
        if (loc == Location)
            return true;
        if (Species is (ushort)Core.Species.Meltan && loc == 00070) // Rouge Sector 1
            return true; // crossover (depends on player location)
        return false;
    }

    public EncounterMatchRating GetMatchRating(PKM pk)
    {
        if (IsMatchPartial(pk))
            return EncounterMatchRating.PartialMatch;
        return IsMatchDeferred(pk);
    }

    private EncounterMatchRating IsMatchDeferred(PKM pk)
    {
        if (Shiny != Shiny.Random && !Shiny.IsValid(pk))
            return EncounterMatchRating.DeferredErrors;

        if (IsAlpha && pk is IPlusRecord pa9 && pk.PersonalInfo is IPermitPlus p && !pa9.GetMovePlusFlag(p.PlusMoveIndexes.IndexOf(PersonalTable.ZA[Species, Form].AlphaMove)))
            return EncounterMatchRating.DeferredErrors;

        var pidiv = TryGetSeed(pk, out _);
        if (pidiv is SeedCorrelationResult.Invalid)
            return EncounterMatchRating.DeferredErrors;
        if (pidiv is SeedCorrelationResult.Ignore)
            return EncounterMatchRating.Deferred; // might be a better match with another template

        return EncounterMatchRating.Match;
    }

    private bool IsMatchPartial(PKM pk)
    {
        switch (Shiny)
        {
            case Shiny.Never when pk.IsShiny:
            case Shiny.Always when !pk.IsShiny:
                return true;
        }

        if (!NoScalarsDefined && pk is IScaledSize3 size3 && size3.Scale != Size)
            return true;

        return false;
    }

    #endregion

    public SeedCorrelationResult TryGetSeed(PKM pk, out ulong seed)
    {
        // Try with 1 shiny roll, since all encounters besides the hyperspace sublegends are.
        var param = GetParams(PersonalTable.ZA[Species, Form]);
        if (param.TryGetSeed(pk, out seed))
            return SeedCorrelationResult.Success;
        if (!IsHyperspaceShinyPossible)
            return SeedCorrelationResult.Invalid;
        if ((pk.IsShiny && !LumioseSolver.SearchShiny1) || !LumioseSolver.SearchShinyN)
            return SeedCorrelationResult.Ignore;

        // Assume any combination of shiny charm and donut boost is active.
        param = param with { RollCount = 1 + ShinyCharm + ShinyHyperspace };
        if (param.TryGetSeed(pk, out seed))
            return SeedCorrelationResult.Success;
        return SeedCorrelationResult.Invalid;
    }

    public LumioseCorrelation Correlation
    {
        get
        {
            if (IsAlpha)
                return LumioseCorrelation.PreApplyIVs;
            if (FlawlessIVCount != 0)
                return LumioseCorrelation.ReApplyIVs;
            return LumioseCorrelation.Normal;
        }
    }

    private const byte ShinyCharm = 3;
    private const byte ShinyHyperspace = 3;

    public GenerateParam9a GetParams(PersonalInfo9ZA pi) => GetParams(pi, shinyCharm: false, activeShinyPower: false);

    public GenerateParam9a GetParams(PersonalInfo9ZA pi, bool shinyCharm, bool activeShinyPower)
    {
        byte rollCount = 1;
        if (IsHyperspaceShinyPossible)
        {
            if (shinyCharm)
                rollCount += ShinyCharm;
            if (activeShinyPower)
                rollCount += ShinyHyperspace;
        }
        var gender = Gender switch
        {
            0 => PersonalInfo.RatioMagicMale,
            1 => PersonalInfo.RatioMagicFemale,
            2 => PersonalInfo.RatioMagicGenderless,
            _ => pi.Gender,
        };
        var scaleType = NoScalarsDefined ? SizeType9.RANDOM : SizeType9.VALUE;
        return new GenerateParam9a(gender, FlawlessIVCount, rollCount, Correlation, scaleType, Size, Nature, Ability, Shiny, IVs);
    }
}
