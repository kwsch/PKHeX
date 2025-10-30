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
        if (Correlation is LumioseCorrelation.ReApplyIVs)
            criteria = criteria.WithoutIVs();

        var param = GetParams(pi);
        if (criteria.Shiny.IsShiny())
            param = param with { RollCount = 1 + 3 }; // Give the +3 for Shiny Charm so that the generator search is more likely to succeed.
        ulong init = Util.Rand.Rand64();
        var success = this.TryApply64(pk, init, param, criteria);
        if (!success && !this.TryApply64(pk, init, param, criteria.WithoutIVs()))
            this.TryApply64(pk, init, param, EncounterCriteria.Unrestricted);

        if (Correlation is LumioseCorrelation.ReApplyIVs)
            criteria.SetRandomIVs(pk, FlawlessIVCount);
    }

    private void SetMoves(PA9 pk, PersonalInfo9ZA pi, byte level)
    {
        var (learn, plus) = LearnSource9ZA.GetLearnsetAndPlus(Species, Form);
        Span<ushort> moves = stackalloc ushort[4];
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
        if (Form != evo.Form && !IsRandomUnspecificForm && !IsValidOutOfBoundsForm(pk))
            return false;
        if (!this.IsLevelWithinRange(pk.MetLevel))
            return false;
        if (pk is IAlphaReadOnly a && a.IsAlpha != IsAlpha)
            return false;
        return true;
    }

    private bool IsValidOutOfBoundsForm(PKM pk) => Species switch
    {
        (int)Core.Species.Furfrou => true, // Can change forms in-game.
        _ => false,
    };

    public EncounterMatchRating GetMatchRating(PKM pk)
    {
        if (!IsMatchCorrelation(pk))
            return EncounterMatchRating.DeferredErrors;

        // Prefer encounters where the species matches exactly over evolution line matches
        if (pk.Species != Species)
            return EncounterMatchRating.PartialMatch;

        return EncounterMatchRating.Match;
    }

    private bool IsMatchCorrelation(PKM pk)
    {
        if (TryGetSeed(pk, out _))
            return true;
        if (!LumioseSolver.SearchShinyN)
            return true; // can't check further without brute forcing

        return false;
    }

    #endregion

    public bool TryGetSeed(PKM pk, out ulong seed) => GetParams(PersonalTable.ZA[Species, Form]).TryGetSeed(pk, out seed);

    public LumioseCorrelation Correlation => IsAlpha ? LumioseCorrelation.PreApplyIVs : LumioseCorrelation.Normal;

    public GenerateParam9a GetParams(PersonalInfo9ZA pi)
    {
        const byte rollCount = 1;
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
