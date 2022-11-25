using System;
using static PKHeX.Core.LegalityCheckStrings;

namespace PKHeX.Core;

/// <summary>
/// Verifies the <see cref="IHyperTrain"/> values.
/// </summary>
public sealed class HyperTrainingVerifier : Verifier
{
    protected override CheckIdentifier Identifier => CheckIdentifier.Training;

    public override void Verify(LegalityAnalysis data)
    {
        var pk = data.Entity;
        if (pk is not IHyperTrain t)
            return; // No Hyper Training before Gen7

        if (!t.IsHyperTrained())
            return;

        if (!t.IsHyperTrainingAvailable(data.Info.EvoChainsAllGens))
        {
            data.AddLine(GetInvalid(LHyperPerfectUnavailable));
            return;
        }

        var minLevel = t.GetHyperTrainMinLevel(data.Info.EvoChainsAllGens);
        if (pk.CurrentLevel < minLevel)
        {
            data.AddLine(GetInvalid(string.Format(LHyperTooLow_0, minLevel)));
            return;
        }

        int max = pk.MaxIV;
        if (pk.IVTotal == max * 6)
        {
            data.AddLine(GetInvalid(LHyperPerfectAll));
            return;
        }

        // already checked for 6IV, therefore we're flawed on at least one IV
        if (t.IsHyperTrainedAll())
        {
            // SV gold bottle cap applies to all IVs regardless
            // LGPE gold bottle cap applies to all IVs regardless
            var evos = data.Info.EvoChainsAllGens;
            if (evos.HasVisitedGen9 && Array.Exists(evos.Gen9, x => x.LevelMax >= 50))
                return;
            if (evos.HasVisitedLGPE && Array.Exists(evos.Gen7b, x => x.LevelMax >= 100))
                return;
        }

        for (int i = 0; i < 6; i++) // Check individual IVs
        {
            if (pk.GetIV(i) != max || !t.IsHyperTrained(i))
                continue;
            data.AddLine(GetInvalid(LHyperPerfectOne));
            break;
        }
    }
}
