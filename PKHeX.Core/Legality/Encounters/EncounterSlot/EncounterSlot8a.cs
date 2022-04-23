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
        if (Gender != Gender.Random)
            pk.Gender = (int)Gender;

        var para = GetParams();
        var (_, slotSeed) = Overworld8aRNG.ApplyDetails(pk, criteria, para, HasAlphaMove);
        if (LevelMin != LevelMax)
            pk.CurrentLevel = pk.Met_Level = Overworld8aRNG.GetRandomLevel(slotSeed, LevelMin, LevelMax);

        if (IsAlpha)
        {
            if (pk is IAlpha a)
                a.IsAlpha = true;
            if (pk is PA8 { AlphaMove: not 0 } pa)
                pk.PushMove(pa.AlphaMove);
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
