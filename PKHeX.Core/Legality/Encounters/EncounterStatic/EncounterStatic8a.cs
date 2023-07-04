using System;

namespace PKHeX.Core;

/// <summary>
/// Generation 8 Static Encounter
/// </summary>
/// <inheritdoc cref="EncounterStatic"/>
public sealed record EncounterStatic8a(GameVersion Version) : EncounterStatic(Version), IAlphaReadOnly, IMasteryInitialMoveShop8, IScaledSizeReadOnly
{
    public override int Generation => 8;
    public override EntityContext Context => EntityContext.Gen8a;

    public byte HeightScalar { get; }
    public byte WeightScalar { get; }
    public bool IsAlpha { get; init; }
    public EncounterStatic8aCorrelation Method { get; init; }

    public bool HasFixedHeight => HeightScalar != NoScalar;
    public bool HasFixedWeight => WeightScalar != NoScalar;
    private const byte NoScalar = 0;

    public EncounterStatic8a(ushort species, byte form, byte level, byte h = NoScalar, byte w = NoScalar) : this(GameVersion.PLA)
    {
        Species = species;
        Form = form;
        Level = level;
        HeightScalar = h;
        WeightScalar = w;
        Shiny = Shiny.Never;
    }

    protected override void ApplyDetails(ITrainerInfo tr, EncounterCriteria criteria, PKM pk)
    {
        base.ApplyDetails(tr, criteria, pk);

        var pa = (PA8)pk;

        if (IsAlpha)
            pa.IsAlpha = true;

        if (HasFixedHeight)
            pa.HeightScalar = HeightScalar;
        if (HasFixedWeight)
            pa.WeightScalar = WeightScalar;
        pa.Scale = pa.HeightScalar;

        pa.ResetHeight();
        pa.ResetWeight();
    }

    protected override void SetPINGA(PKM pk, EncounterCriteria criteria)
    {
        var para = GetParams();
        var (_, slotSeed) = Overworld8aRNG.ApplyDetails(pk, criteria, para, IsAlpha);
        // We don't override LevelMin, so just handle the two species cases.
        if (Species == (int)Core.Species.Zorua)
            pk.CurrentLevel = pk.Met_Level = Overworld8aRNG.GetRandomLevel(slotSeed, 27, 29);
        else if (Species == (int)Core.Species.Phione)
            pk.CurrentLevel = pk.Met_Level = Overworld8aRNG.GetRandomLevel(slotSeed, 33, 36);

        if (Method == EncounterStatic8aCorrelation.Fixed)
            pk.EncryptionConstant = Util.Rand32();
    }

    protected override void ApplyDetailsBall(PKM pk) => pk.Ball = Gift ? Ball : (int)Core.Ball.LAPoke;

    public override bool IsMatchExact(PKM pk, EvoCriteria evo)
    {
        if (!base.IsMatchExact(pk, evo))
            return false;

        if (pk is IScaledSize s)
        {
            // 3 of the Alpha statics were mistakenly set as 127 scale. If they enter HOME on 3.0.1, they'll get bumped to 255.
            if (IsAlpha && this is { HeightScalar: 127, WeightScalar: 127 }) // Average Size Alphas
            {
                // HOME >=3.0.1 ensures 255 scales for the 127's
                // PLA and S/V could have safe-harbored them via <=3.0.0
                if (pk.Context is EntityContext.Gen8a or EntityContext.Gen9)
                {
                    if (s is not { HeightScalar: 127, WeightScalar: 127 }) // Original? 
                    {
                        // Must match the HOME updated values AND must have the Alpha ribbon (visited HOME).
                        if (s is not { HeightScalar: 255, WeightScalar: 255 })
                            return false;
                        if (pk is IRibbonSetMark9 { RibbonMarkAlpha: false })
                            return false;
                    }
                }
                else
                {
                    // Must match the HOME updated values
                    if (s is not { HeightScalar: 255, WeightScalar: 255 })
                        return false;
                }
            }
            else
            {
                if (HasFixedHeight && s.HeightScalar != HeightScalar)
                    return false;
                if (HasFixedWeight && s.WeightScalar != WeightScalar)
                    return false;
            }
        }

        if (pk is IAlpha a && a.IsAlpha != IsAlpha)
            return false;

        return true;
    }

    protected override bool IsMatchLocation(PKM pk)
    {
        var metState = LocationsHOME.GetRemapState(Context, pk.Context);
        if (metState == LocationRemapState.Original)
            return base.IsMatchLocation(pk);
        if (metState == LocationRemapState.Remapped)
            return IsMetRemappedSWSH(pk);
        return base.IsMatchLocation(pk) || IsMetRemappedSWSH(pk);
    }

    private static bool IsMetRemappedSWSH(PKM pk) => pk.Met_Location == LocationsHOME.SWLA;

    public override EncounterMatchRating GetMatchRating(PKM pk)
    {
        var result = GetMatchRatingInternal(pk);
        var orig = base.GetMatchRating(pk);
        return result > orig ? result : orig;
    }

    private EncounterMatchRating GetMatchRatingInternal(PKM pk)
    {
        if (Shiny != Shiny.Random && !Shiny.IsValid(pk))
            return EncounterMatchRating.DeferredErrors;
        if (Gift && pk.Ball != Ball)
            return EncounterMatchRating.DeferredErrors;

        var orig = base.GetMatchRating(pk);
        if (orig is not EncounterMatchRating.Match)
            return orig;

        if (!IsForcedMasteryCorrect(pk))
            return EncounterMatchRating.DeferredErrors;

        if (!MarkRules.IsMarkValidAlpha(pk, IsAlpha))
            return EncounterMatchRating.DeferredErrors;

        if (IsAlpha && pk is PA8 { AlphaMove: 0 })
            return EncounterMatchRating.Deferred;

        return EncounterMatchRating.Match;
    }

    public bool IsForcedMasteryCorrect(PKM pk)
    {
        ushort alpha = 0;
        if (IsAlpha && Moves.HasMoves)
        {
            if (pk is PA8 pa && (alpha = pa.AlphaMove) != Moves.Move1)
                return false;
        }

        if (pk is not IMoveShop8Mastery p)
            return true;

        const bool allowAlphaPurchaseBug = true; // Everything else Alpha is pre-1.1
        var level = pk.Met_Level;
        var (learn, mastery) = GetLevelUpInfo();
        if (!p.IsValidPurchasedEncounter(learn, level, alpha, allowAlphaPurchaseBug))
            return false;

        Span<ushort> moves = stackalloc ushort[4];
        if (Moves.HasMoves)
            Moves.CopyTo(moves);
        else
            learn.SetEncounterMoves(level, moves);

        return p.IsValidMasteredEncounter(moves, learn, mastery, level, alpha, allowAlphaPurchaseBug);
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

    public (Learnset Learn, Learnset Mastery) GetLevelUpInfo()
    {
        return LearnSource8LA.GetLearnsetAndMastery(Species, Form);
    }

    public void LoadInitialMoveset(PA8 pa8, Span<ushort> moves, Learnset learn, int level)
    {
        if (Moves.HasMoves)
            Moves.CopyTo(moves);
        else
            learn.SetEncounterMoves(level, moves);
        if (IsAlpha)
            pa8.AlphaMove = moves[0];
    }

    private OverworldParam8a GetParams()
    {
        var gender = GetGenderRatio();
        return new OverworldParam8a
        {
            IsAlpha = IsAlpha,
            FlawlessIVs = FlawlessIVCount,
            Shiny = Shiny,
            RollCount = 1, // Everything is shiny locked anyways
            GenderRatio = gender,
        };
    }

    private byte GetGenderRatio() => Gender switch
    {
        0 => PersonalInfo.RatioMagicMale,
        1 => PersonalInfo.RatioMagicFemale,
        _ => GetGenderRatioPersonal(),
    };

    private byte GetGenderRatioPersonal()
    {
        var pt = PersonalTable.LA;
        var entry = pt.GetFormEntry(Species, Form);
        return entry.Gender;
    }
}

public enum EncounterStatic8aCorrelation : byte
{
    WildGroup,
    Fixed,
}
