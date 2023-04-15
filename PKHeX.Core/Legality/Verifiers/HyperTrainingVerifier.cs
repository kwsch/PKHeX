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
            if (HasVisitedGoldBottleFlawless(data.Info.EvoChainsAllGens))
                return;
            // Otherwise, could not have hyper trained a flawless IV. Flag a flawless IV with the usual logic.
        }

        if (IsFlawlessHyperTrained(pk, t, max))
            data.AddLine(GetInvalid(LHyperPerfectOne));
    }

    private static bool HasVisitedGoldBottleFlawless(EvolutionHistory evos)
    {
        // S/V gold bottle cap applies to all IVs regardless
        // LGP/E gold bottle cap applies to all IVs regardless
        foreach (ref var x in evos.Gen9.AsSpan())
        {
            if (x.LevelMax >= 50)
                return true;
        }
        foreach (ref var x in evos.Gen7b.AsSpan())
        {
            if (x.LevelMax >= 100)
                return true;
        }
        return false;
    }

    public static bool IsFlawlessHyperTrained(PKM pk, IHyperTrain t, int max)
    {
        for (int i = 0; i < 6; i++) // Check individual IVs
        {
            if (pk.GetIV(i) == max && t.IsHyperTrained(i))
                return true;
        }
        return false;
    }
}
