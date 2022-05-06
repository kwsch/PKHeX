using System;

namespace PKHeX.Core;

/// <summary>
/// Generation 8 Static Encounter
/// </summary>
/// <inheritdoc cref="EncounterStatic"/>
public sealed record EncounterStatic8a(GameVersion Version) : EncounterStatic(Version), IAlpha
{
    public override int Generation => 8;

    public byte HeightScalar { get; }
    public byte WeightScalar { get; }
    public bool IsAlpha { get; set; }
    public EncounterStatic8aCorrelation Method { get; init; }

    public bool HasFixedHeight => HeightScalar != NoScalar;
    public bool HasFixedWeight => WeightScalar != NoScalar;
    private const byte NoScalar = 0;

    public EncounterStatic8a(ushort species, ushort form, byte level, byte h = NoScalar, byte w = NoScalar) : this(GameVersion.PLA)
    {
        Species = species;
        Form = form;
        Level = level;
        HeightScalar = h;
        WeightScalar = w;
        Shiny = Shiny.Never;
    }

    protected override void ApplyDetails(ITrainerInfo sav, EncounterCriteria criteria, PKM pk)
    {
        base.ApplyDetails(sav, criteria, pk);
        if (pk is IScaledSize s)
        {
            if (HasFixedHeight)
                s.HeightScalar = HeightScalar;
            if (HasFixedWeight)
                s.WeightScalar = WeightScalar;
            if (pk is IScaledSizeValue v)
            {
                v.ResetHeight();
                v.ResetWeight();
            }
        }

        if (IsAlpha && pk is IAlpha a)
            a.IsAlpha = true;

        if (pk is PA8 pa)
        {
            if (IsAlpha && Moves.Count != 0)
                pa.AlphaMove = (ushort)Moves[0];
            if (pa.AlphaMove != 0)
                pk.PushMove(pa.AlphaMove);
            pa.SetMasteryFlags();
            pa.HeightScalarCopy = pa.HeightScalar;
        }
    }

    protected override void SetPINGA(PKM pk, EncounterCriteria criteria)
    {
        var para = GetParams();
        var (_, slotSeed) = Overworld8aRNG.ApplyDetails(pk, criteria, para, IsAlpha);
        // We don't override LevelMin, so just handle the two species cases.
        if (Species == (int)Core.Species.Zorua)
            pk.CurrentLevel = pk.Met_Level = Overworld8aRNG.GetRandomLevel(slotSeed, 26, 28);
        else if (Species == (int)Core.Species.Phione)
            pk.CurrentLevel = pk.Met_Level = Overworld8aRNG.GetRandomLevel(slotSeed, 33, 36);

        if (Method == EncounterStatic8aCorrelation.Fixed)
            pk.EncryptionConstant = Util.Rand32();
    }

    protected override void ApplyDetailsBall(PKM pk) => pk.Ball = Gift ? Ball : (int)Core.Ball.LAPoke;

    public override bool IsMatchExact(PKM pkm, IDexLevel evo)
    {
        if (!base.IsMatchExact(pkm, evo))
            return false;

        if (pkm is IScaledSize s)
        {
            if (HasFixedHeight && s.HeightScalar != HeightScalar)
                return false;
            if (HasFixedWeight && s.WeightScalar != WeightScalar)
                return false;
        }

        if (pkm is IAlpha a && a.IsAlpha != IsAlpha)
            return false;

        return true;
    }

    public override EncounterMatchRating GetMatchRating(PKM pkm)
    {
        if (!IsForcedMasteryCorrect(pkm))
            return EncounterMatchRating.PartialMatch;

        var result = GetMatchRatingInternal(pkm);
        var orig = base.GetMatchRating(pkm);
        return result > orig ? result : orig;
    }

    private EncounterMatchRating GetMatchRatingInternal(PKM pkm)
    {
        if (Shiny != Shiny.Random && !Shiny.IsValid(pkm))
            return EncounterMatchRating.DeferredErrors;
        if (Gift && pkm.Ball != Ball)
            return EncounterMatchRating.DeferredErrors;

        var orig = base.GetMatchRating(pkm);
        if (orig is not EncounterMatchRating.Match)
            return orig;

        if (IsAlpha && pkm is PA8 { AlphaMove: 0 })
            return EncounterMatchRating.Deferred;

        return EncounterMatchRating.Match;
    }

    private bool IsForcedMasteryCorrect(PKM pkm)
    {
        if (IsAlpha && Moves.Count != 0)
        {
            if (pkm is PA8 pa && pa.AlphaMove != Moves[0])
                return false;
        }

        if (pkm is not IMoveShop8Mastery p)
            return true;

        Span<int> m = stackalloc int[4];
        var level = pkm.Met_Level;
        var index = PersonalTable.LA.GetFormIndex(Species, Form);
        var mastery = Legal.MasteryLA[index];
        if (Moves.Count != 0)
            m = (int[])Moves;
        else
            Legal.LevelUpLA[index].SetEncounterMoves(level, m);

        return p.IsValidMasteredEncounter(m, mastery, level);
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
        2 => PersonalInfo.RatioMagicGenderless,
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
