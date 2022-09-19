using System;

namespace PKHeX.Core;

/// <summary>
/// Encounter Slot found in <see cref="GameVersion.SWSH"/>.
/// </summary>
/// <inheritdoc cref="EncounterSlot"/>
public sealed record EncounterSlot8a : EncounterSlot, IAlpha, IMasteryInitialMoveShop8
{
    public override int Generation => 8;
    public override EntityContext Context => EntityContext.Gen8a;
    public SlotType Type => Area.Type;

    public bool IsAlpha { get => AlphaType is not 0; set => throw new InvalidOperationException("Do not mutate this field."); }
    public byte FlawlessIVCount { get; }
    public Gender Gender { get; }
    public byte AlphaType { get; } // 0=Never, 1=Random, 2=Guaranteed

    public EncounterSlot8a(EncounterArea8a area, ushort species, byte form, byte min, byte max, byte alphaType, byte flawlessIVs, Gender gender) : base(area, species, form, min, max)
    {
        AlphaType = alphaType;
        FlawlessIVCount = flawlessIVs;
        Gender = gender;
    }

    public bool HasAlphaMove => IsAlpha && Type is not SlotType.Landmark;

    protected override void ApplyDetails(ITrainerInfo sav, EncounterCriteria criteria, PKM pk)
    {
        base.ApplyDetails(sav, criteria, pk);

        var pa = (PA8)pk;
        if (IsAlpha)
            pa.HeightScalarCopy = pa.HeightScalar = pa.WeightScalar = 255;
        pa.ResetHeight();
        pa.ResetWeight();
    }

    protected override void SetPINGA(PKM pk, EncounterCriteria criteria)
    {
        base.SetPINGA(pk, criteria);
        if (Gender != Gender.Random)
            pk.Gender = (int)Gender;

        var para = GetParams();
        while (true)
        {
            var (_, slotSeed) = Overworld8aRNG.ApplyDetails(pk, criteria, para, HasAlphaMove);
            if (LevelMin != LevelMax)
            {
                var lvl = Overworld8aRNG.GetRandomLevel(slotSeed, LevelMin, LevelMax);
                if (criteria.ForceMinLevelRange && lvl != LevelMin)
                    continue;
                pk.CurrentLevel = pk.Met_Level = lvl;
            }
            break;
        }
    }

    protected override void SetEncounterMoves(PKM pk, GameVersion version, int level)
    {
        var pa8 = (PA8)pk;
        Span<ushort> moves = stackalloc ushort[4];
        var (learn, mastery) = GetLevelUpInfo();
        LoadInitialMoveset(pa8, moves, learn, level);
        pk.SetMoves(moves);
        pk.SetMaximumPPCurrent(moves);
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

    public (Learnset Learn, Learnset Mastery) GetLevelUpInfo()
    {
        var index = PersonalTable.LA.GetFormIndex(Species, Form);
        var learn = Legal.LevelUpLA[index];
        var mastery = Legal.MasteryLA[index];
        return (learn, mastery);
    }

    protected override void SetFormatSpecificData(PKM pk)
    {
        var pa8 = (PA8)pk;
        if (IsAlpha)
            pa8.IsAlpha = true;
        pa8.HeightScalarCopy = pa8.HeightScalar;
    }

    protected override void ApplyDetailsBall(PKM pk) => pk.Ball = (int)Ball.LAPoke;

    public override EncounterMatchRating GetMatchRating(PKM pk)
    {
        if (Gender is not Gender.Random && pk.Gender != (int)Gender)
            return EncounterMatchRating.PartialMatch;

        var result = GetMatchRatingInternal(pk);
        var orig = base.GetMatchRating(pk);
        return result > orig ? result : orig;
    }

    private EncounterMatchRating GetMatchRatingInternal(PKM pk)
    {
        if (pk is IAlpha a && a.IsAlpha != IsAlpha)
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
        (int)Core.Species.Overqwil    when Species is not (int)Core.Species.Overqwil    && pk is IFormArgument { FormArgument: 0 } => true,
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
        if (!pa.IsAlpha || Type is SlotType.Landmark)
            return !hasAlphaMove ? EncounterMatchRating.Match : EncounterMatchRating.DeferredErrors;

        var pi = PersonalTable.LA.GetFormEntry(Species, Form);
        var tutors = pi.SpecialTutors[0];

        if (alphaMove is 0)
        {
            bool hasAnyTutor = Array.IndexOf(tutors, true) >= 0;
            if (hasAnyTutor)
                return EncounterMatchRating.Deferred;
        }
        else
        {
            var idx = pa.MoveShopPermitIndexes;
            var index = idx.IndexOf(idx);
            if (index == -1)
                return EncounterMatchRating.Deferred;
            if (!tutors[index])
                return EncounterMatchRating.Deferred;
        }
        return EncounterMatchRating.Match;
    }

    public bool IsForcedMasteryCorrect(PKM pk)
    {
        if (pk is not IMoveShop8Mastery p)
            return true; // Can't check.

        bool allowAlphaPurchaseBug = Area.Type is not SlotType.OverworldMMO; // Everything else Alpha is pre-1.1
        var level = pk.Met_Level;
        var index = PersonalTable.LA.GetFormIndex(Species, Form);
        var learn = Legal.LevelUpLA[index];
        ushort alpha = pk is PA8 pa ? pa.AlphaMove : (ushort)0;
        if (!p.IsValidPurchasedEncounter(learn, level, alpha, allowAlphaPurchaseBug))
            return false;

        Span<ushort> moves = stackalloc ushort[4];
        var mastery = Legal.MasteryLA[index];
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

    private OverworldParam8a GetParams()
    {
        var gender = GetGenderRatio();
        return new OverworldParam8a
        {
            IsAlpha = IsAlpha,
            FlawlessIVs = FlawlessIVCount,
            Shiny = Shiny,
            RollCount = GetRollCount(Type),
            GenderRatio = gender,
        };
    }

    private byte GetGenderRatio() => Gender switch
    {
        Gender.Male => PersonalInfo.RatioMagicMale,
        Gender.Female => PersonalInfo.RatioMagicFemale,
        _ => GetGenderRatioPersonal(),
    };

    private byte GetGenderRatioPersonal()
    {
        var pt = PersonalTable.LA;
        var entry = pt.GetFormEntry(Species, Form);
        return (byte)entry.Gender;
    }

    // hardcoded 7 to assume max dex progress + shiny charm.
    private const int MaxRollCount = 7;

    private static byte GetRollCount(SlotType type) => (byte)(MaxRollCount + type switch
    {
        SlotType.OverworldMMO => 12,
        SlotType.OverworldMass => 25,
        _ => 0,
    });
}
