using System;
using static PKHeX.Core.LegalityCheckResultCode;
using static PKHeX.Core.CheckIdentifier;

namespace PKHeX.Core;

internal sealed class MiscEncounterDetailsVerifier : Verifier
{
    protected override CheckIdentifier Identifier => Misc;

    public override void Verify(LegalityAnalysis data)
    {
        var pk = data.Entity;
        var enc = data.EncounterMatch;
        Verify(data, pk, enc);
    }

    internal void Verify(LegalityAnalysis data, PKM pk, IEncounterTemplate enc)
    {
        if (enc is IEncounterServerDate { IsDateRestricted: true } encounterDate)
            VerifyServerDate2000(data, pk, enc, encounterDate);

        VerifyRandomCorrelationSwitch(data, pk, enc);
    }

    private void VerifyRandomCorrelationSwitch(LegalityAnalysis data, PKM pk, IEncounterTemplate enc)
    {
        if (enc is IOverworldCorrelation8 z)
        {
            VerifyCorrelation8(data, z, pk);
        }
        else if (enc is IStaticCorrelation8b s8b)
        {
            VerifyCorrelation8b(data, s8b, pk);
        }
        else if (enc is ISeedCorrelation64<PKM> s64)
        {
            var pidiv = s64.TryGetSeed(pk, out var seed);
            if (pidiv == SeedCorrelationResult.Success)
                data.Info.PIDIV = new PIDIV(PIDType.Xoroshiro, seed);
            else if (pidiv is SeedCorrelationResult.Invalid)
                data.AddLine(GetInvalid(PIDTypeMismatch));

            if (enc is IMasteryInitialMoveShop8 m && !m.IsForcedMasteryCorrect(pk))
                data.AddLine(GetInvalid(EncMasteryInitial));
        }
    }

    private void VerifyCorrelation8b(LegalityAnalysis data, IStaticCorrelation8b s8b, PKM pk)
    {
        var match = s8b.IsStaticCorrelationCorrect(pk);
        var req = s8b.GetRequirement(pk);
        if (match)
            data.Info.PIDIV = new PIDIV(PIDType.Roaming8b, pk.EncryptionConstant);

        bool valid = req switch
        {
            StaticCorrelation8bRequirement.MustHave => match,
            StaticCorrelation8bRequirement.MustNotHave => !match,
            _ => true,
        };

        if (!valid)
            data.AddLine(GetInvalid(PIDTypeMismatch));
    }

    private void VerifyCorrelation8(LegalityAnalysis data, IOverworldCorrelation8 z, PKM pk)
    {
        var match = z.IsOverworldCorrelationCorrect(pk);
        var req = z.GetRequirement(pk);
        if (match)
        {
            var seed = Overworld8RNG.GetOriginalSeed(pk);
            data.Info.PIDIV = new PIDIV(PIDType.Overworld8, seed);
        }

        bool valid = req switch
        {
            OverworldCorrelation8Requirement.MustHave => match,
            OverworldCorrelation8Requirement.MustNotHave => !match,
            _ => true,
        };

        if (!valid)
            data.AddLine(GetInvalid(PIDTypeMismatch));
    }

    private void VerifyServerDate2000(LegalityAnalysis data, PKM pk, IEncounterTemplate enc, IEncounterServerDate date)
    {
        const int epoch = 2000;
        var actualDay = new DateOnly(pk.MetYear + epoch, pk.MetMonth, pk.MetDay);

        // HOME Gifts for Sinnoh/Hisui starters were forced JPN until May 20, 2022 (UTC).
        if (enc is WB8 { IsDateLockJapanese: true } or WA8 { IsDateLockJapanese: true })
        {
            if (actualDay < new DateOnly(2022, 5, 20) && pk.Language != (int)LanguageID.Japanese)
                data.AddLine(GetInvalid(DateOutsideDistributionWindow));
        }

        var result = date.IsWithinDistributionWindow(actualDay);
        if (result == EncounterServerDateCheck.Invalid)
            data.AddLine(GetInvalid(DateOutsideDistributionWindow));
    }
}
