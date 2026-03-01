using System;

namespace PKHeX.Core;

/// <summary>
/// Gift Encounter found in <see cref="GameVersion.ZA"/>.
/// </summary>
public sealed record EncounterGift9a(ushort Species, byte Form, byte Level, byte Size = EncounterGift9a.NoScale)
    : IEncounter9a, IEncounterConvertible<PA9>, IFixedGender, IFixedNature, IFixedIVSet, IMoveset, IFixedTrainer, IFixedNickname
{
    public byte Generation => 9;
    private const GameVersion Version = GameVersion.ZA;
    GameVersion IVersion.Version => GameVersion.ZA;
    public EntityContext Context => EntityContext.Gen9a;
    public bool IsEgg => false;
    public AbilityPermission Ability => AbilityPermission.Any12;
    public Ball FixedBall => Ball.Poke;
    public Shiny Shiny { get; init; } = Shiny.Never;
    public bool IsShiny => false;
    public ushort EggLocation => 0;
    public byte FlawlessIVCount { get; init; }
    private const byte NoScale = 0;
    private bool NoScalarsDefined => Size == NoScale;
    public Moveset Moves { get; init; }
    public IndividualValueSet IVs { get; init; }
    public Nature Nature { get; init; } = Nature.Random;
    public byte LevelMin => Level;
    public byte LevelMax => Level;

    public required ushort Location { get; init; }
    public byte Gender { get; init; } = FixedGenderUtil.GenderRandom;
    public bool IsAlpha { get; init; }
    public TrainerGift9a Trainer { get; init; }

    public string Name => "Gift Encounter";
    public string LongName => Name;

    public bool IsFixedTrainer => Trainer != TrainerGift9a.None;
    public bool IsFixedNickname => Trainer == TrainerGift9a.Gimmighoul;
    public bool IsTrainerMatch(PKM pk, ReadOnlySpan<char> trainer, int language)
    {
        var expect = GetFixedTrainerName(Trainer, pk.Language);
        return trainer.SequenceEqual(expect);
    }
    public bool IsNicknameMatch(PKM pk, ReadOnlySpan<char> nickname, int language)
    {
        var expect = GetGimmighoulFixedNickname(Trainer, pk.Language);
        return nickname.SequenceEqual(expect);
    }
    public string GetNickname(int language) => IsFixedNickname ? GetGimmighoulFixedNickname(Trainer, language) : SpeciesName.GetSpeciesNameGeneration(Species, language, Generation);

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

            Nickname = GetNickname(lang),
            ObedienceLevel = LevelMin,
        };

        if (Trainer is TrainerGift9a.None)
        {
            pk.OriginalTrainerName = tr.OT;
            pk.OriginalTrainerGender = tr.Gender;
            pk.ID32 = tr.ID32;
        }
        else
        {
            pk.OriginalTrainerName = GetFixedTrainerName(Trainer, lang);
            pk.OriginalTrainerGender = GetFixedTrainerGender(Trainer);
            pk.ID32 = GetFixedTrainerID32(Trainer);

            if (Trainer is TrainerGift9a.Gimmighoul)
            {
                pk.IsNicknamed = true;
                pk.Nickname = GetGimmighoulFixedNickname(Trainer, lang);
            }
        }

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
        if (Trainer != 0 && !IsMatchFixedTrainer(pk, Trainer))
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

        var pidiv = TryGetSeed(pk, out _);
        if (pidiv != SeedCorrelationResult.Success)
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
        if (pk.IsShiny && !LumioseSolver.SearchShiny1)
            return SeedCorrelationResult.Ignore;
        return SeedCorrelationResult.Invalid;
    }

    public LumioseCorrelation Correlation => IsAlpha ? LumioseCorrelation.PreApplyIVs : IVs.IsSpecified || FlawlessIVCount != 0 ? LumioseCorrelation.ReApplyIVs : LumioseCorrelation.Normal;

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

    private static bool IsMatchFixedTrainer(PKM pk, TrainerGift9a trainer)
    {
        if (pk.ID32 != GetFixedTrainerID32(trainer))
            return false;
        if (pk.OriginalTrainerGender != GetFixedTrainerGender(trainer))
            return false;
        if (pk.OriginalTrainerName != GetFixedTrainerName(trainer, pk.Language))
            return false;
        return true;
    }

    private static uint GetFixedTrainerID32(TrainerGift9a trainer) => trainer switch
    {
        TrainerGift9a.Lucario => 912562,
        TrainerGift9a.Floette => 1,
        TrainerGift9a.Stunfisk => 250932,
        TrainerGift9a.Gimmighoul => 115090,
        TrainerGift9a.Magearna => 981300,
        _ => throw new ArgumentOutOfRangeException(nameof(trainer), trainer, null),
    };

    private static byte GetFixedTrainerGender(TrainerGift9a trainer) => trainer switch
    {
        TrainerGift9a.Lucario => 1,
        TrainerGift9a.Floette => 0,
        TrainerGift9a.Stunfisk => 0,
        TrainerGift9a.Gimmighoul => 0,
        TrainerGift9a.Magearna => 0, // encounter is set as male OT, even though Jett is female
        _ => throw new ArgumentOutOfRangeException(nameof(trainer), trainer, null),
    };

    private static string GetFixedTrainerName(TrainerGift9a trainer, int language) => trainer switch
    {
        TrainerGift9a.Lucario => language switch
        {
            (int)LanguageID.Japanese => "コルニ",
            (int)LanguageID.English => "Korrina",
            (int)LanguageID.French => "Cornélia",
            (int)LanguageID.Italian => "Ornella",
            (int)LanguageID.German => "Connie",
            (int)LanguageID.Spanish => "Corelia",
            (int)LanguageID.Korean => "코르니",
            (int)LanguageID.ChineseS => "可尔妮",
            (int)LanguageID.ChineseT => "可爾妮",
            (int)LanguageID.SpanishL => "Korrina",
            _ => throw new ArgumentOutOfRangeException(nameof(language), language, null),
        },
        TrainerGift9a.Floette => language switch
        {
            (int)LanguageID.Japanese => "ＡＺ",
            (int)LanguageID.English => "AZ",
            (int)LanguageID.French => "A.Z.",
            (int)LanguageID.Italian => "AZ",
            (int)LanguageID.German => "Azett",
            (int)LanguageID.Spanish => "A. Z.", // nbsp 0xA0 instead of 0x20
            (int)LanguageID.Korean => "AZ",
            (int)LanguageID.ChineseS => "ＡＺ",
            (int)LanguageID.ChineseT => "ＡＺ",
            (int)LanguageID.SpanishL => "A. Z.", // nbsp 0xA0 instead of 0x20
            _ => throw new ArgumentOutOfRangeException(nameof(language), language, null),
        },
        TrainerGift9a.Stunfisk => language switch
        {
            (int)LanguageID.Japanese => "グラウン",
            (int)LanguageID.English => "Terri",
            (int)LanguageID.French => "Gad",
            (int)LanguageID.Italian => "Terrence",
            (int)LanguageID.German => "Terry",
            (int)LanguageID.Spanish => "Terry",
            (int)LanguageID.Korean => "그라운",
            (int)LanguageID.ChineseS => "帝尚",
            (int)LanguageID.ChineseT => "帝尚",
            (int)LanguageID.SpanishL => "René",
            _ => throw new ArgumentOutOfRangeException(nameof(language), language, null),
        },
        TrainerGift9a.Gimmighoul => language switch
        {
            (int)LanguageID.Japanese => "セロ",
            (int)LanguageID.English => "Thelo",
            (int)LanguageID.French => "Thélo",
            (int)LanguageID.Italian => "Petens",
            (int)LanguageID.German => "Will",
            (int)LanguageID.Spanish => "Thelo",
            (int)LanguageID.Korean => "텔로",
            (int)LanguageID.ChineseS => "法才",
            (int)LanguageID.ChineseT => "法才",
            (int)LanguageID.SpanishL => "Turian",
            _ => throw new ArgumentOutOfRangeException(nameof(language), language, null),
        },
        TrainerGift9a.Magearna => language switch
        {
            (int)LanguageID.Japanese => "ジェット",
            (int)LanguageID.English => "Jett",
            (int)LanguageID.French => "Bridjet",
            (int)LanguageID.Italian => "Aviona",
            (int)LanguageID.German => "Jette",
            (int)LanguageID.Spanish => "Viona",
            (int)LanguageID.Korean => "제트",
            (int)LanguageID.ChineseS => "捷朵",
            (int)LanguageID.ChineseT => "捷朵",
            (int)LanguageID.SpanishL => "Viona",
            _ => throw new ArgumentOutOfRangeException(nameof(language), language, null),
        },
        _ => throw new ArgumentOutOfRangeException(nameof(trainer), trainer, null),
    };

    private static string GetGimmighoulFixedNickname(TrainerGift9a trainer, int language) => language switch
    {
        (int)LanguageID.Japanese => "はこいり",
        (int)LanguageID.English => "Chestly",
        (int)LanguageID.French => "Trésor",
        (int)LanguageID.Italian => "Bauletto",
        (int)LanguageID.German => "Schätzchen",
        (int)LanguageID.Spanish => "Tesorina",
        (int)LanguageID.Korean => "보무리",
        (int)LanguageID.ChineseS => "千金",
        (int)LanguageID.ChineseT => "千金",
        (int)LanguageID.SpanishL => "Tesorita",
        _ => throw new ArgumentOutOfRangeException(nameof(language), language, null),
    };
}

public enum TrainerGift9a : byte
{
    None = 0,
    Lucario,
    Floette,
    Stunfisk,
    Gimmighoul,
    Magearna,
}
