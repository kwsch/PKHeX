using System;

namespace PKHeX.Core;

/// <summary>
/// Encounter Slot found in <see cref="GameVersion.PLA"/>.
/// </summary>
/// <param name="AlphaType">0=Never, 1=Random, 2=Guaranteed</param>
/// <param name="FlawlessIVCount"></param>
public sealed record EncounterSlot8a(EncounterArea8a Parent, ushort Species, byte Form, byte LevelMin, byte LevelMax, byte AlphaType, byte FlawlessIVCount, Gender Gender)
    : IEncounterable, IEncounterMatch, IEncounterConvertible<PA8>, IAlphaReadOnly, IMasteryInitialMoveShop8, IFlawlessIVCount, ISeedCorrelation64<PKM>
{
    public byte Generation => 8;
    public EntityContext Context => EntityContext.Gen8a;
    public bool IsEgg => false;
    public AbilityPermission Ability => AbilityPermission.Any12;
    public Ball FixedBall => Ball.None;
    public Shiny Shiny => Shiny.Random;
    public bool IsShiny => false;
    public ushort EggLocation => 0;

    public bool IsAlpha => AlphaType is not 0;

    public string Name => $"Wild Encounter ({Version})";
    public string LongName => $"{Name} {Type.ToString().Replace('_', ' ')}";
    public GameVersion Version => Parent.Version;
    public ushort Location => Parent.Location;
    public SlotType8a Type => Parent.Type;

    public bool HasAlphaMove => IsAlpha && Type is not SlotType8a.Landmark;

    #region Generating
    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria) => ConvertToPKM(tr, criteria);
    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr);
    public PA8 ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr, EncounterCriteria.Unrestricted);
    public PA8 ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria)
    {
        int lang = (int)Language.GetSafeLanguage(Generation, (LanguageID)tr.Language);
        var pi = PersonalTable.LA[Species, Form];
        var pk = new PA8
        {
            Language = lang,
            Species = Species,
            Form = Form,
            CurrentLevel = LevelMin,
            MetLocation = Location,
            MetLevel = LevelMin,
            MetDate = EncounterDate.GetDateSwitch(),
            Version = GameVersion.PLA,
            IsAlpha = IsAlpha,
            Ball = (int)Ball.LAPoke,

            OriginalTrainerName = tr.OT,
            OriginalTrainerGender = tr.Gender,
            ID32 = tr.ID32,
            OriginalTrainerFriendship = pi.BaseFriendship,
            Nickname = SpeciesName.GetSpeciesNameGeneration(Species, lang, Generation),
        };
        SetPINGA(pk, criteria, pi);
        pk.Scale = pk.HeightScalar;
        pk.ResetHeight();
        pk.ResetWeight();
        SetEncounterMoves(pk, LevelMin);
        pk.ResetPartyStats();
        return pk;
    }

    private void SetPINGA(PA8 pk, EncounterCriteria criteria, PersonalInfo8LA pi)
    {
        var para = GetParams(pi);
        while (true)
        {
            var (_, slotSeed) = Overworld8aRNG.ApplyDetails(pk, criteria, para, HasAlphaMove);
            if (this.IsRandomLevel())
            {
                // Give a random level according to the RNG correlation.
                var lvl = Overworld8aRNG.GetRandomLevel(slotSeed, LevelMin, LevelMax);
                if (criteria.ForceMinLevelRange && lvl != LevelMin)
                    continue;
                pk.MetLevel = pk.CurrentLevel = lvl;
            }
            break;
        }
    }

    private OverworldParam8a GetParams(PersonalInfo8LA pi) => new()
    {
        Shiny = Shiny,
        IsAlpha = IsAlpha,
        FlawlessIVs = FlawlessIVCount,
        RollCount = GetRollCount(Type),
        GenderRatio = Gender switch
        {
            Gender.Male => PersonalInfo.RatioMagicMale,
            Gender.Female => PersonalInfo.RatioMagicFemale,
            _ => pi.Gender,
        },
    };

    // hardcoded 7 to assume max dex progress + shiny charm.
    private const int MaxRollCount = 7;

    private static byte GetRollCount(SlotType8a type) => (byte)(MaxRollCount + type switch
    {
        SlotType8a.MassOutbreakMassive => 12,
        SlotType8a.MassOutbreakRegular => 25,
        _ => 0,
    });

    private void SetEncounterMoves(PKM pk, int level)
    {
        var pa8 = (PA8)pk;
        Span<ushort> moves = stackalloc ushort[4];
        var (learn, mastery) = GetLevelUpInfo();
        LoadInitialMoveset(pa8, moves, learn, level);
        pk.SetMoves(moves);
        pa8.SetEncounterMasteryFlags(moves, mastery, level);
        if (pa8.AlphaMove != 0)
            pa8.SetMasteryFlagMove(pa8.AlphaMove);
    }

    public void LoadInitialMoveset(PA8 pa8, Span<ushort> moves, Learnset learn, int level)
    {
        if (pa8.AlphaMove != 0)
        {
            moves[0] = pa8.AlphaMove;
            learn.SetEncounterMovesBackwards(level, moves, 1);
        }
        else
        {
            learn.SetEncounterMoves(level, moves);
        }
    }

    public (Learnset Learn, Learnset Mastery) GetLevelUpInfo() => LearnSource8LA.GetLearnsetAndMastery(Species, Form);
    #endregion

    #region Matching

    public bool IsMatchExact(PKM pk, EvoCriteria evo)
    {
        if (!this.IsLevelWithinRange(pk.MetLevel))
            return false;
        if (Form != evo.Form && Species is not ((int)Core.Species.Rotom or (int)Core.Species.Burmy or (int)Core.Species.Wormadam))
            return false;
        return true;
    }

    private bool IsDeferredWurmple(PKM pk) => Species == (int)Core.Species.Wurmple && pk.Species != (int)Core.Species.Wurmple && !WurmpleUtil.IsWurmpleEvoValid(pk);

    public EncounterMatchRating GetMatchRating(PKM pk)
    {
        if (Gender is not Gender.Random && pk.Gender != (int)Gender)
            return EncounterMatchRating.PartialMatch;
        if (IsDeferredWurmple(pk))
            return EncounterMatchRating.PartialMatch;
        if (!MarkRules.IsMarkValidAlpha(pk, IsAlpha))
            return EncounterMatchRating.DeferredErrors;
        if (FlawlessIVCount is not 0 && pk.FlawlessIVCount < FlawlessIVCount)
            return EncounterMatchRating.DeferredErrors;
        if (IsFormArgMismatch(pk))
            return EncounterMatchRating.DeferredErrors;
        if (!IsForcedMasteryCorrect(pk))
            return EncounterMatchRating.DeferredErrors;

        return GetMoveCompatibility(pk);
    }

    private bool IsFormArgMismatch(PKM pk) => pk.Species switch
    {
        (int)Core.Species.Wyrdeer     when Species is not (int)Core.Species.Wyrdeer     && pk is IFormArgument { FormArgument: 0 } => true,
        (int)Core.Species.Overqwil    when Species is not (int)Core.Species.Overqwil    && pk is IFormArgument { FormArgument: 0 } and IHomeTrack { HasTracker: false } => true,
        (int)Core.Species.Basculegion when Species is not (int)Core.Species.Basculegion && pk is IFormArgument { FormArgument: 0 } => true,
        _ => false,
    };

    private EncounterMatchRating GetMoveCompatibility(PKM pk)
    {
        // Check for Alpha move compatibility.
        if (pk is not PA8 pa)
            return EncounterMatchRating.Match;

        var alphaMove = pa.AlphaMove;
        bool hasAlphaMove = alphaMove != 0;
        if (!pa.IsAlpha || Type is SlotType8a.Landmark)
            return !hasAlphaMove ? EncounterMatchRating.Match : EncounterMatchRating.DeferredErrors;

        var pi = PersonalTable.LA.GetFormEntry(Species, Form);

        // Alpha encounters grant one Alpha move from the MoveShop list, if any exists.
        if (alphaMove is 0)
        {
            // None set, but if any are available, it's a mismatch.
            if (pi.HasMoveShop)
                return EncounterMatchRating.Deferred;
        }
        else
        {
            var permit = pa.Permit;
            var idx = permit.RecordPermitIndexes;
            var index = idx.IndexOf(alphaMove);
            if (index == -1)
                return EncounterMatchRating.Deferred;
            if (!permit.IsRecordPermitted(index))
                return EncounterMatchRating.Deferred;
        }
        return EncounterMatchRating.Match;
    }

    public bool IsForcedMasteryCorrect(PKM pk)
    {
        if (pk is not IMoveShop8Mastery p)
            return true; // Can't check.

        bool allowAlphaPurchaseBug = Type is not SlotType8a.MassOutbreakMassive; // Everything else Alpha is pre-1.1
        var level = pk.MetLevel;
        var (learn, mastery) = GetLevelUpInfo();
        ushort alpha = pk is PA8 pa ? pa.AlphaMove : (ushort)0;
        if (!p.IsValidPurchasedEncounter(learn, level, alpha, allowAlphaPurchaseBug))
            return false;

        Span<ushort> moves = stackalloc ushort[4];
        if (pk is PA8 { AlphaMove: not 0 } pa8)
        {
            moves[0] = pa8.AlphaMove;
            learn.SetEncounterMovesBackwards(level, moves, 1);
        }
        else
        {
            learn.SetEncounterMoves(level, moves);
        }

        return p.IsValidMasteredEncounter(moves, learn, mastery, level, alpha, allowAlphaPurchaseBug);
    }
    #endregion

    public bool TryGetSeed(PKM pk, out ulong seed)
    {
        // Check if it matches any single-roll seed.
        var pi = PersonalTable.LA[Species, Form];
        var param = GetParams(pi) with { RollCount = 1 };
        var solver = new XoroMachineSkip(pk.EncryptionConstant, pk.PID);
        foreach (var s in solver)
        {
            if (!Overworld8aRNG.Verify(pk, s, param))
                continue;
            seed = s;
            return true;
        }
        seed = default;
        return false;
    }
}
