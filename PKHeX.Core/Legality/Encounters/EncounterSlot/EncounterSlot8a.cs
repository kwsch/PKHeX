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
        if (Gender != Gender.Random)
            pk.Gender = (int)Gender;

        var para = GetParams(criteria);
        Overworld8aRNG.ApplyDetails(pk, criteria, para);

        if (IsAlpha)
        {
            if (pk is IAlpha a)
                a.IsAlpha = true;
            if (Type is not SlotType.Landmark && pk is PA8 pa)
            {
                var extra = pa.AlphaMove = pa.GetRandomAlphaMove();
                pa.SetMasteryFlagMove(extra);
                pk.PushMove(extra);
            }
        }

        if (pk is PA8 pa8)
        {
            pa8.HeightScalarCopy = pa8.HeightScalar;
            pa8.SetMasteryFlags();
        }
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

        return GetAlphaMoveCompatibility(pkm);
    }

    private bool IsFormArgMismatch(PKM pkm) => pkm.Species switch
    {
        (int)Core.Species.Wyrdeer     when Species is not (int)Core.Species.Wyrdeer     && pkm is IFormArgument { FormArgument: 0 } => true,
        (int)Core.Species.Overqwil    when Species is not (int)Core.Species.Overqwil    && pkm is IFormArgument { FormArgument: 0 } => true,
        (int)Core.Species.Basculegion when Species is not (int)Core.Species.Basculegion && pkm is IFormArgument { FormArgument: 0 } => true,
        _ => false,
    };

    private EncounterMatchRating GetAlphaMoveCompatibility(PKM pkm)
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

    private OverworldParam8a GetParams(EncounterCriteria criteria)
    {
        var pt = PersonalTable.LA;
        var entry = pt.GetFormEntry(Species, Form);
        var gender = (byte)entry.Gender;
        return new OverworldParam8a
        {
            IsAlpha = IsAlpha,
            FlawlessIVs = FlawlessIVCount,
            Shiny = Shiny,
            RollCount = criteria.Shiny.IsShiny() ? Type is SlotType.Swarm ? (byte)32 : (byte)7 : (byte)1,
            GenderRatio = gender,
        };
    }
}
