using static PKHeX.Core.LegalityCheckResultCode;
using static PKHeX.Core.CheckIdentifier;

namespace PKHeX.Core;

internal sealed class MiscScaleVerifier : Verifier
{
    protected override CheckIdentifier Identifier => Misc;

    public override void Verify(LegalityAnalysis data) => Verify(data, data.Entity, data.EncounterMatch);

    internal void Verify(LegalityAnalysis data, PKM pk, IEncounterTemplate enc)
    {
        if (pk is not IScaledSize s2)
            return;

        VerifyHeightWeight(data, pk, enc, s2);
        if (pk is IScaledSize3 s3)
            VerifyScale(data, pk, enc, s2, s3);
    }

    private static void VerifyHeightWeight(LegalityAnalysis data, PKM pk, IEncounterTemplate enc, IScaledSize s2)
    {
        if (enc.Generation < 8 && pk.Format >= 9)
        {
            // Gen1-7 can have 0-0 if kept in PLA before HOME 3.0
            if (s2 is { HeightScalar: 0, WeightScalar: 0 } && !data.Info.EvoChainsAllGens.HasVisitedPLA && enc is not IPogoSlot)
                data.AddLine(Get(Encounter, Severity.Invalid, StatInvalidHeightWeight));
        }
        else if (enc.Context is EntityContext.Gen9a)
        {
            // TODO HOME ZA
            if (s2.HeightScalar != 0)
                data.AddLine(GetInvalid(Encounter, StatIncorrectHeightValue_0, 0));
            if (s2.WeightScalar != 0)
                data.AddLine(GetInvalid(Encounter, StatIncorrectWeightValue_0, 0));
        }
        else if (CheckHeightWeightOdds(enc))
        {
            if (s2 is { HeightScalar: 0, WeightScalar: 0 })
            {
                if (ParseSettings.Settings.HOMETransfer.ZeroHeightWeight != Severity.Valid)
                    data.AddLine(Get(Encounter, ParseSettings.Settings.HOMETransfer.ZeroHeightWeight, StatInvalidHeightWeight));
            }
        }
    }

    private void VerifyScale(LegalityAnalysis data, PKM pk, IEncounterTemplate enc, IScaledSize s2, IScaledSize3 s3)
    {
        // PLA static Alphas have potential for 127 scale; this is already checked explicitly in the matching check.
        // Ensure all Alphas have 255 scale.
        // Otherwise, ensure scale matches height scalar if required.
        if (enc is IAlphaReadOnly { IsAlpha: true })
        {
            byte expect = enc switch
            {
                EncounterStatic8a { IsAlpha127: true } when s2 is { HeightScalar: 127, WeightScalar: 127 } => 127, // Fixed size Alphas not yet converted by HOME >= 3.0.1
                _ => byte.MaxValue,
            };
            if (s3.Scale != expect)
                data.AddLine(GetInvalid(StatIncorrectScaleValue_0, expect));
        }
        else if (IsHeightScaleMatchRequired(pk) && s2.HeightScalar != s3.Scale)
        {
            data.AddLine(GetInvalid(StatIncorrectScaleValue_0, s2.HeightScalar));
        }
    }

    private static bool IsHeightScaleMatchRequired(PKM pk) => pk is IHomeTrack { HasTracker: true };

    private static bool CheckHeightWeightOdds(IEncounterTemplate enc)
    {
        if (enc.Generation < 8)
            return false;
        if (enc.Context is EntityContext.Gen9a) // TODO HOME ZA
            return true;
        if (enc is WC8 { IsHOMEGift: true })
            return false;
        if (enc is WC9) // fixed values (usually 0 or 128)
            return false;
        return true;
    }
}
