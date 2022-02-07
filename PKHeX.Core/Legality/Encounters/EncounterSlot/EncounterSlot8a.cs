using System;

namespace PKHeX.Core;

/// <summary>
/// Encounter Slot found in <see cref="GameVersion.SWSH"/>.
/// </summary>
/// <inheritdoc cref="EncounterSlot"/>
public sealed record EncounterSlot8a : EncounterSlot, IAlpha
{
    public override int Generation => 8;
    public SlotType Type => Area.Type;

    public bool IsAlpha { get => AlphaType is not 0; set => throw new InvalidOperationException("Do not mutate this field."); }
    public byte FlawlessIVCount { get; }
    public Gender Gender { get; }
    public byte AlphaType { get; } // 0=Never, 1=Random, 2=Guaranteed

    public EncounterSlot8a(EncounterArea8a area, int species, int form, int min, int max, byte alphaType, byte flawlessIVs, Gender gender) : base(area, species, form, min, max)
    {
        AlphaType = alphaType;
        FlawlessIVCount = flawlessIVs;
        Gender = gender;
    }

    protected override void ApplyDetails(ITrainerInfo sav, EncounterCriteria criteria, PKM pk)
    {
        base.ApplyDetails(sav, criteria, pk);
        pk.SetRandomEC();
        if (Gender != Gender.Random)
            pk.Gender = (int)Gender;

        if (IsAlpha)
        {
            if (pk is IAlpha a)
                a.IsAlpha = true;
            if (pk is IScaledSize s)
                s.HeightScalar = s.WeightScalar = byte.MaxValue;
            if (Type is not SlotType.Landmark && pk is PA8 pa)
                pa.SetMasteryFlagMove(pa.AlphaMove = pa.GetRandomAlphaMove());
        }
        if (pk is IScaledSizeValue v)
        {
            v.ResetHeight();
            v.ResetWeight();
        }
        if (pk is PA8 pa8)
        {
            pa8.HeightScalarCopy = pa8.HeightScalar;
            pa8.SetMasteryFlags();
        }
        if (FlawlessIVCount > 0)
            pk.SetRandomIVs(flawless: FlawlessIVCount);
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

        return GetAlphaMoveCompatibility(pkm);
    }

    private EncounterMatchRating GetAlphaMoveCompatibility(PKM pkm)
    {
        // Check for Alpha move compatibility.
        if (pkm is not PA8 pa)
            return EncounterMatchRating.Match;

        var alphaMove = pa.AlphaMove;
        bool hasAlphaMove = alphaMove != 0;
        if (!pa.IsAlpha)
            return !hasAlphaMove ? EncounterMatchRating.Match : EncounterMatchRating.DeferredErrors;
        if (Type is SlotType.Landmark == hasAlphaMove)
            return EncounterMatchRating.DeferredErrors;

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
}
