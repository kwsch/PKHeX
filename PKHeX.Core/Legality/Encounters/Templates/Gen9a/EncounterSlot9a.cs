using System;

namespace PKHeX.Core;

/// <summary>
/// Encounter Slot found in <see cref="GameVersion.ZA"/>.
/// </summary>
public sealed record EncounterSlot9a(EncounterArea9a Parent, ushort Species, byte Form, byte LevelMin, byte LevelMax, bool IsAlpha, byte Gender, Shiny Shiny)
    : IEncounter9a, IEncounterConvertible<PA9>, IFixedGender
{
    public byte Generation => 9;
    private const GameVersion Version = GameVersion.ZA;

    private bool IsHyperspace => Location == EncounterArea9a.LocationHyperspace;

    GameVersion IVersion.Version => GameVersion.ZA;
    public EntityContext Context => EntityContext.Gen9a;
    public bool IsEgg => false;
    public AbilityPermission Ability => AbilityPermission.Any12;
    public Ball FixedBall => Ball.None;
    public bool IsShiny => false;
    public ushort EggLocation => 0;
    public bool IsRandomUnspecificForm => Form >= EncounterUtil.FormDynamic;

    public string Name => "Wild Encounter";
    public string LongName => $"{Name} {Type.ToString().Replace('_', ' ')}";
    public ushort Location => Parent.Location;
    public SlotType9a Type => Parent.Type;

    public byte FlawlessIVCount => IsAlpha ? (byte)3 : (byte)0;

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
            MetLocation = Location,
            MetLevel = LevelMin,
            MetDate = EncounterDate.GetDateSwitch(),
            Version = Version,
            IsAlpha = IsAlpha,
            Ball = (int)Ball.Poke,

            OriginalTrainerName = tr.OT,
            OriginalTrainerGender = tr.Gender,
            ID32 = tr.ID32,
            OriginalTrainerFriendship = pi.BaseFriendship,
            Nickname = SpeciesName.GetSpeciesNameGeneration(Species, lang, Generation),
            ObedienceLevel = LevelMin,
        };
        SetPINGA(pk, criteria, pi);
        SetMoves(pk, pi, LevelMin);
        pk.HealPP();

        pk.ResetPartyStats();
        return pk;
    }

    private void SetPINGA(PA9 pk, EncounterCriteria criteria, PersonalInfo9ZA pi)
    {
        bool shinyPlease = criteria.Shiny.IsShiny();
        var param = GetParams(pi, shinyPlease, shinyPlease);
        ulong init = Util.Rand.Rand64();
        var success = this.TryApply64(pk, init, param, criteria);
        if (!success && !this.TryApply64(pk, init, param, criteria.WithoutIVs()))
            this.TryApply64(pk, init, param, EncounterCriteria.Unrestricted);
    }

    private void SetMoves(PA9 pk, PersonalInfo9ZA pi, byte level)
    {
        var (learn, plus) = LearnSource9ZA.GetLearnsetAndPlus(Species, Form);
        pk.SetPlusFlagsEncounter(pi, plus, level);

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
        if (Form != evo.Form && !IsRandomUnspecificForm && !IsValidOutOfBoundsForm())
            return false;
        if (!this.IsLevelWithinRange(pk.MetLevel))
            return false;
        if (pk is IAlphaReadOnly a && a.IsAlpha != IsAlpha)
            return false;
        return true;
    }

    private bool IsValidOutOfBoundsForm() => Species switch
    {
        (int)Core.Species.Rotom => true, // Can change forms in-game.
        (int)Core.Species.Furfrou => true, // Can change forms in-game.
        _ => false,
    };

    public EncounterMatchRating GetMatchRating(PKM pk)
    {
        if (IsAlpha && pk is IPlusRecord pa9 && pk.PersonalInfo is IPermitPlus p && !pa9.GetMovePlusFlag(p.PlusMoveIndexes.IndexOf(PersonalTable.ZA[Species, Form].AlphaMove)))
            return EncounterMatchRating.DeferredErrors;
        if (Shiny is Shiny.Never && pk.IsShiny) // Some encounters are shiny locked until a sub-quest is completed.
            return EncounterMatchRating.DeferredErrors;

        if (IsFormArgMismatch(pk))
            return EncounterMatchRating.DeferredErrors;

        var pidiv = TryGetSeed(pk, out _);
        if (pidiv is SeedCorrelationResult.Invalid)
            return EncounterMatchRating.DeferredErrors;
        if (pidiv is SeedCorrelationResult.Ignore)
            return EncounterMatchRating.Deferred; // might be a better match with another template

        return EncounterMatchRating.Match;
    }

    private bool IsFormArgMismatch(PKM pk) => pk.Species switch
    {
        (int)Core.Species.Overqwil when Species is not (int)Core.Species.Overqwil && pk is IFormArgument { FormArgument: 0 } and IHomeTrack { HasTracker: false } => true,
        _ => false,
    };

    #endregion

    public SeedCorrelationResult TryGetSeed(PKM pk, out ulong seed)
    {
        var param = GetParams(PersonalTable.ZA[Species, Form], false, false);
        if (param.TryGetSeed(pk, out seed))
            return SeedCorrelationResult.Success;
        if ((pk.IsShiny && !LumioseSolver.SearchShiny1) || !LumioseSolver.SearchShinyN)
            return SeedCorrelationResult.Ignore;

        var rollCount = (byte)(1 + ShinyCharm + (IsHyperspace ? ShinyHyperspace : 0));
        param = param with { RollCount = rollCount };
        if (param.TryGetSeed(pk, out seed))
            return SeedCorrelationResult.Success;
        return SeedCorrelationResult.Invalid;
    }

    public LumioseCorrelation Correlation => IsAlpha ? LumioseCorrelation.PreApplyIVs : LumioseCorrelation.Normal;

    private const byte ShinyCharm = 3;
    private const byte ShinyHyperspace = 3;

    public GenerateParam9a GetParams(PersonalInfo9ZA pi) => GetParams(pi, shinyCharm: false, activeShinyPower: false);

    public GenerateParam9a GetParams(PersonalInfo9ZA pi, bool shinyCharm, bool activeShinyPower)
    {
        // Give the +3 for Shiny Charm so that the generator search is more likely to succeed.
        var rollCount = (byte)(1 + (shinyCharm ? ShinyCharm : 0) + (IsHyperspace && activeShinyPower ? ShinyHyperspace : 0));
        var scaleValue = IsAlpha ? (byte)255 : (byte)0;
        var scaleType = IsAlpha ? SizeType9.VALUE : SizeType9.RANDOM;
        var gender = Gender switch
        {
            0 => PersonalInfo.RatioMagicMale,
            1 => PersonalInfo.RatioMagicFemale,
            2 => PersonalInfo.RatioMagicGenderless,
            _ => pi.Gender,
        };
        return new GenerateParam9a(gender, FlawlessIVCount, rollCount, Correlation, scaleType, Scale: scaleValue, Nature.Random, Ability, Shiny);
    }
}
