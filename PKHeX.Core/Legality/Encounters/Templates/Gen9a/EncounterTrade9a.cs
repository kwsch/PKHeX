using System;

namespace PKHeX.Core;

/// <summary>
/// Trade Encounter found in <see cref="GameVersion.ZA"/>.
/// </summary>
public sealed record EncounterTrade9a : IEncounter9a,
    IFixedTrainer, IFixedNickname, IFixedGender, IFixedNature, IMoveset, IFixedIVSet, ITrainerID32ReadOnly
{
    public byte Generation => 9;
    private const GameVersion Version = GameVersion.ZA;
    GameVersion IVersion.Version => GameVersion.ZA;
    public EntityContext Context => EntityContext.Gen9a;
    public ushort Location => Locations.LinkTrade6NPC;
    public bool IsEgg => false;
    public AbilityPermission Ability => AbilityPermission.Any12;
    public Ball FixedBall => Ball.Poke;
    public Shiny Shiny => Shiny.Never;
    public bool IsShiny => false;
    public ushort EggLocation => 0;
    public byte FlawlessIVCount { get; init; }
    public byte LevelMin => Level;
    public byte LevelMax => Level;
    public bool IsFixedTrainer => true;
    public bool IsFixedNickname => true;
    public bool IsAlpha => false;

    private readonly ReadOnlyMemory<string> TrainerNames;
    private readonly ReadOnlyMemory<string> Nicknames;

    public ushort Species { get; }
    public byte Form { get; }
    public byte Level { get; }
    public Moveset Moves { get; init; }
    public required uint ID32 { get; init; }
    public ushort TID16 => (ushort)ID32;
    public ushort SID16 => (ushort)(ID32 >> 16);
    public required byte OTGender { get; init; }
    public byte Gender { get; init; }
    public IndividualValueSet IVs { get; init; }
    public Nature Nature { get; init; } = Nature.Random;
    private const byte Scale = 128;

    public string Name => "Trade Encounter";
    public string LongName => Name;

    public EncounterTrade9a(ReadOnlySpan<string[]> names, byte index, ushort species, byte form, byte level)
    {
        Nicknames = EncounterUtil.GetNamesForLanguage(names, index);
        TrainerNames = EncounterUtil.GetNamesForLanguage(names, (uint)(index + (names[1].Length >> 1)));
        Species = species;
        Form = form;
        Level = level;
    }

    #region Generating

    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr);
    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria) => ConvertToPKM(tr, criteria);

    public PA9 ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr, EncounterCriteria.Unrestricted);

    public PA9 ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria)
    {
        int lang = (int)Language.GetSafeLanguage9a((LanguageID)tr.Language);
        var pi = PersonalTable.ZA[Species, Form];
        var pk = new PA9
        {
            Species = Species,
            Form = Form,
            CurrentLevel = Level,
            MetLocation = Location,
            MetLevel = Level,
            MetDate = EncounterDate.GetDateSwitch(),
            Gender = Gender,
            Nature = Nature,
            StatNature = Nature,
            Ball = (byte)FixedBall,

            ID32 = ID32,
            Version = Version,
            Language = lang,
            OriginalTrainerGender = OTGender,
            OriginalTrainerName = TrainerNames.Span[lang],

            OriginalTrainerFriendship = pi.BaseFriendship,

            IsNicknamed = true,
            Nickname = Nicknames.Span[lang],

            Scale = Scale,

            HandlingTrainerName = tr.OT,
            HandlingTrainerLanguage = (byte)tr.Language,
            CurrentHandler = 1,
            HandlingTrainerFriendship = pi.BaseFriendship,
            ObedienceLevel = Level,
        };

        SetPINGA(pk, criteria, pi);
        SetMoves(pk, pi, Level);
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

    public bool IsTrainerMatch(PKM pk, ReadOnlySpan<char> trainer, int language) => (uint)language < TrainerNames.Length && trainer.SequenceEqual(TrainerNames.Span[language]);
    public bool IsNicknameMatch(PKM pk, ReadOnlySpan<char> nickname, int language) => (uint)language < Nicknames.Length && nickname.SequenceEqual(Nicknames.Span[language]);
    public string GetNickname(int language) => Nicknames.Span[(uint)language < Nicknames.Length ? language : 0];

    private bool IsMatchNatureGenderShiny(PKM pk)
    {
        if (!Shiny.IsValid(pk))
            return false;
        if (pk.Gender != Gender)
            return false;
        if (Nature != Nature.Random && pk.Nature != Nature)
            return false;
        return true;
    }

    public EncounterMatchRating GetMatchRating(PKM pk)
    {
        if (pk is IScaledSize3 size3 && size3.Scale != Scale)
            return EncounterMatchRating.PartialMatch;

        var pidiv = TryGetSeed(pk, out _);
        if (pidiv is not SeedCorrelationResult.Success)
            return EncounterMatchRating.DeferredErrors;

        return EncounterMatchRating.Match;
    }

    public bool IsMatchExact(PKM pk, EvoCriteria evo)
    {
        if (pk.MetLevel != Level)
            return false;
        if (!IsMatchLocation(pk))
            return false;
        if (!IsMatchNatureGenderShiny(pk))
            return false;
        if (pk.ID32 != ID32)
            return false;
        if (evo.Form != Form && !FormInfo.IsFormChangeable(Species, Form, pk.Form, Context, pk.Context))
            return false;
        if (pk.OriginalTrainerGender != OTGender)
            return false;
        if (!IsMatchEggLocation(pk))
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

    #endregion

    public SeedCorrelationResult TryGetSeed(PKM pk, out ulong seed)
    {
        if (GetParams(PersonalTable.ZA[Species, Form]).TryGetSeed(pk, out seed))
            return SeedCorrelationResult.Success;
        return SeedCorrelationResult.Invalid;
    }

    public LumioseCorrelation Correlation => LumioseCorrelation.ReApplyIVs;

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
        return new GenerateParam9a(gender, FlawlessIVCount, rollCount, Correlation, SizeType9.VALUE, Scale, Nature, Ability, Shiny, IVs);
    }
}
