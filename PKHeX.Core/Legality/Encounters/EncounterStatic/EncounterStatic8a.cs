using System;

namespace PKHeX.Core;

/// <summary>
/// Generation 8 Static Encounter
/// </summary>
/// <inheritdoc cref="EncounterStatic"/>
public sealed record EncounterStatic8a(GameVersion Version) : EncounterStatic(Version), IAlpha, IMasteryInitialMoveShop8
{
    public override int Generation => 8;
    public override EntityContext Context => EntityContext.Gen8a;

    public byte HeightScalar { get; }
    public byte WeightScalar { get; }
    public bool IsAlpha { get; set; }
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
        pa.HeightScalarCopy = pa.HeightScalar;

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
            if (HasFixedHeight && s.HeightScalar != HeightScalar)
                return false;
            if (HasFixedWeight && s.WeightScalar != WeightScalar)
                return false;
        }

        if (pk is IAlpha a && a.IsAlpha != IsAlpha)
            return false;

        return true;
    }

    protected override bool IsMatchLocation(PKM pk)
    {
        if (pk is PK8)
            return pk.Met_Location == Locations.HOME_SWLA;
        if (pk is PB8 { Version: (int)GameVersion.PLA, Met_Location: Locations.HOME_SWLA })
            return true;

        return base.IsMatchLocation(pk);
    }

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
        var index = PersonalTable.LA.GetFormIndex(Species, Form);
        var learn = Legal.LevelUpLA[index];
        if (!p.IsValidPurchasedEncounter(learn, level, alpha, allowAlphaPurchaseBug))
            return false;

        Span<ushort> moves = stackalloc ushort[4];
        var mastery = Legal.MasteryLA[index];
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
        var index = PersonalTable.LA.GetFormIndex(Species, Form);
        var learn = Legal.LevelUpLA[index];
        var mastery = Legal.MasteryLA[index];
        return (learn, mastery);
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
        return (byte)entry.Gender;
    }
}

public enum EncounterStatic8aCorrelation : byte
{
    WildGroup,
    Fixed,
}
