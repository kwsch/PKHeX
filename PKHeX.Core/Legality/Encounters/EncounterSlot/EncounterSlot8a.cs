using System;

namespace PKHeX.Core;

/// <summary>
/// Encounter Slot found in <see cref="GameVersion.SWSH"/>.
/// </summary>
/// <inheritdoc cref="EncounterSlot"/>
public sealed record EncounterSlot8a : EncounterSlot, IAlpha, IMasteryInitialMoveShop8
{
    public override int Generation => 8;
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

    protected override void SetPINGA(PKM pk, EncounterCriteria criteria)
    {
        base.SetPINGA(pk, criteria);
        if (Gender != Gender.Random)
            pk.Gender = (int)Gender;

        var para = GetParams();
        var (_, slotSeed) = Overworld8aRNG.ApplyDetails(pk, criteria, para, HasAlphaMove);
        if (LevelMin != LevelMax)
            pk.CurrentLevel = pk.Met_Level = Overworld8aRNG.GetRandomLevel(slotSeed, LevelMin, LevelMax);
    }

    protected override void SetEncounterMoves(PKM pk, GameVersion version, int level)
    {
        var pa8 = (PA8)pk;
        Span<int> moves = stackalloc int[4];
        var index = PersonalTable.LA.GetFormIndex(Species, Form);
        var mastery = Legal.MasteryLA[index];
        var learn = Legal.LevelUpLA[index];
        if (pa8.AlphaMove != 0)
        {
            moves[0] = pa8.AlphaMove;
            learn.SetEncounterMovesBackwards(level, moves, 1);
        }
        else
        {
            learn.SetEncounterMoves(level, moves);
        }
        pk.SetMoves(moves);
        pk.SetMaximumPPCurrent(moves);
        foreach (var move in moves)
        {
            if (mastery.GetMoveLevel(move) <= level)
                pa8.SetMasteryFlagMove(move);
        }
    }

    protected override void SetFormatSpecificData(PKM pk)
    {
        var pa8 = (PA8)pk;
        if (IsAlpha)
            pa8.IsAlpha = true;
        pa8.HeightScalarCopy = pa8.HeightScalar;
    }

    protected override void ApplyDetailsBall(PKM pk) => pk.Ball = (int)Ball.LAPoke;

    public override EncounterMatchRating GetMatchRating(PKM pkm)
    {
        if (Gender is not Gender.Random && pkm.Gender != (int)Gender)
            return EncounterMatchRating.PartialMatch;

        var result = GetMatchRatingInternal(pkm);
        var orig = base.GetMatchRating(pkm);
        return result > orig ? result : orig;
    }

    private EncounterMatchRating GetMatchRatingInternal(PKM pkm)
    {
        if (pkm is IAlpha a && a.IsAlpha != IsAlpha)
            return EncounterMatchRating.DeferredErrors;
        if (FlawlessIVCount is not 0 && pkm.FlawlessIVCount < FlawlessIVCount)
            return EncounterMatchRating.DeferredErrors;
        if (IsFormArgMismatch(pkm))
            return EncounterMatchRating.DeferredErrors;
        if (!IsForcedMasteryCorrect(pkm))
            return EncounterMatchRating.DeferredErrors;

        return GetMoveCompatibility(pkm);
    }

    private bool IsFormArgMismatch(PKM pkm) => pkm.Species switch
    {
        (int)Core.Species.Wyrdeer     when Species is not (int)Core.Species.Wyrdeer     && pkm is IFormArgument { FormArgument: 0 } => true,
        (int)Core.Species.Overqwil    when Species is not (int)Core.Species.Overqwil    && pkm is IFormArgument { FormArgument: 0 } => true,
        (int)Core.Species.Basculegion when Species is not (int)Core.Species.Basculegion && pkm is IFormArgument { FormArgument: 0 } => true,
        _ => false,
    };

    private EncounterMatchRating GetMoveCompatibility(PKM pkm)
    {
        // Check for Alpha move compatibility.
        if (pkm is not PA8 pa)
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

    public bool IsForcedMasteryCorrect(PKM pkm)
    {
        if (pkm is not IMoveShop8Mastery p)
            return true; // Can't check.

        bool allowAlphaPurchaseBug = Area.Type is not SlotType.OverworldMMO; // Everything else Alpha is pre-1.1
        var level = pkm.Met_Level;
        var index = PersonalTable.LA.GetFormIndex(Species, Form);
        var learn = Legal.LevelUpLA[index];
        ushort alpha = pkm is PA8 pa ? pa.AlphaMove : (ushort)0;
        if (!p.IsValidPurchasedEncounter(learn, level, alpha, allowAlphaPurchaseBug))
            return false;

        Span<int> moves = stackalloc int[4];
        var mastery = Legal.MasteryLA[index];
        if (pkm is PA8 { AlphaMove: not 0 } pa8)
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
        var pt = PersonalTable.LA;
        var entry = pt.GetFormEntry(Species, Form);
        var gender = (byte)entry.Gender;
        return new OverworldParam8a
        {
            IsAlpha = IsAlpha,
            FlawlessIVs = FlawlessIVCount,
            Shiny = Shiny,
            RollCount = GetRollCount(Type),
            GenderRatio = gender,
        };
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
