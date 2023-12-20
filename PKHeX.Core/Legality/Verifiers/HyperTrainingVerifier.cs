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

        var minLevel = t.GetHyperTrainMinLevel(data.Info.EvoChainsAllGens, pk.Context);
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
            // S/V gold bottle cap applies to all IVs regardless
            // LGP/E gold bottle cap applies to all IVs regardless
            // As of S/V update 3.0.0 and HOME, HOME will fix any with this issue, and S/V no longer behaves incorrectly.
            // Ignore the fact that <3.0.0 S/V touched can be broken while still in S/V or previous games, as it's not worth the effort to check.
            // - Needs to be able to inhabit S/V before 3.0.0, which has ball/date/species restrictions
            // Really isn't worth checking. Just flag it for anything outside LGP/E similar to other GameFreak bugs (like incorrect move PP).
            if (pk.Context == EntityContext.Gen7b)
                return;
            // Otherwise, could not have hyper trained a flawless IV. Flag a flawless IV with the usual logic.
        }

        if (IsFlawlessHyperTrained(pk, t, max))
            data.AddLine(GetInvalid(LHyperPerfectOne));
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
