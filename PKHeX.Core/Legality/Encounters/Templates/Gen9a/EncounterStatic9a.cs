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
    public required byte Location { get; init; }
    public byte Gender { get; init; } = FixedGenderUtil.GenderRandom;
    public Shiny Shiny { get; init; } = Shiny.Never;
    public bool IsAlpha { get; init; }

    ushort ILocation.Location => Location;

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

        var param = GetParams(pi);
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
        Span<ushort> moves = stackalloc ushort[4];
        if (Moves.HasMoves)
        {
            pk.SetMoves(Moves);
            pk.GetMoves(moves);
            PlusRecordApplicator.SetPlusFlagsEncounter(pk, pi, plus, level);
            return;
        }

        if (!IsAlpha)
        {
            learn.SetEncounterMoves(level, moves);
            PlusRecordApplicator.SetPlusFlagsEncounter(pk, pi, plus, level);
        }
        else
        {
            learn.SetEncounterMovesBackwards(level, moves, sameDescend: false);
            PlusRecordApplicator.SetPlusFlagsEncounter(pk, pi, plus, level, moves[0] = pi.AlphaMove);
        }
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
    private bool IsMatchLocation(PKM pk) => pk.MetLocation == Location;

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
        if (pidiv is not SeedCorrelationResult.Success)
            return EncounterMatchRating.DeferredErrors;

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
        if (GetParams(PersonalTable.ZA[Species, Form]).TryGetSeed(pk, out seed))
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

    public GenerateParam9a GetParams(PersonalInfo9ZA pi)
    {
        const byte rollCount = 1;
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
