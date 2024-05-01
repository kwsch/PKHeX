using System;
using static PKHeX.Core.LegalityCheckStrings;

namespace PKHeX.Core;

/// <summary>
/// Verifies the <see cref="PKM.IVs"/>.
/// </summary>
public sealed class IndividualValueVerifier : Verifier
{
    protected override CheckIdentifier Identifier => CheckIdentifier.IVs;

    public override void Verify(LegalityAnalysis data)
    {
        switch (data.EncounterMatch)
        {
            case IPogoSlot s:
                VerifyIVsGoTransfer(data, s);
                break;
            case IFlawlessIVCount s:
                VerifyIVsFlawless(data, s);
                break;
            case EncounterSlot7:
                VerifyIVsGen7(data);
                break;
            case MysteryGift g:
                VerifyIVsMystery(data, g);
                break;
        }
        var pk = data.Entity;
        var hp = pk.IV_HP;
        if (hp < 30 && AllIVsEqual(pk, hp))
            data.AddLine(Get(string.Format(LIVAllEqual_0, hp), Severity.Fishy));
    }

    private static bool AllIVsEqual(PKM pk, int hp) => pk.IV_ATK == hp
                                                    && pk.IV_DEF == hp
                                                    && pk.IV_SPA == hp
                                                    && pk.IV_SPD == hp
                                                    && pk.IV_SPE == hp;

    private void VerifyIVsMystery(LegalityAnalysis data, MysteryGift g)
    {
        if (!g.HasFixedIVs)
            return; // PID/IV style instead of fixed IVs.

        Span<int> IVs = stackalloc int[6];
        g.GetIVs(IVs);
        var ivflag = IVs.IndexOfAny(0xFC, 0xFD, 0xFE);
        if (ivflag == -1) // Random IVs
        {
            bool valid = Legal.GetIsFixedIVSequenceValidSkipRand(IVs, data.Entity);
            if (!valid)
                data.AddLine(GetInvalid(LEncGiftIVMismatch));
        }
        else
        {
            int IVCount = IVs[ivflag] - 0xFB;  // IV2/IV3
            VerifyIVsFlawless(data, IVCount);
        }
    }

    private void VerifyIVsGen7(LegalityAnalysis data)
    {
        var pk = data.Entity;
        if (pk.AbilityNumber == 4)
        {
            var abilities = (IPersonalAbility12H)pk.PersonalInfo;
            if (!AbilityVerifier.CanAbilityPatch(pk.Format, abilities, pk.Species))
                VerifyIVsFlawless(data, 2); // Chain of 10 yields 5% HA and 2 flawless IVs
        }
    }

    private void VerifyIVsFlawless(LegalityAnalysis data, IFlawlessIVCount s)
    {
        if (s.FlawlessIVCount != 0)
            VerifyIVsFlawless(data, s.FlawlessIVCount);
    }

    private void VerifyIVsFlawless(LegalityAnalysis data, int count)
    {
        if (data.Entity.FlawlessIVCount < count)
            data.AddLine(GetInvalid(string.Format(LIVF_COUNT0_31, count)));
    }

    private void VerifyIVsGoTransfer(LegalityAnalysis data, IPogoSlot g)
    {
        if (!g.GetIVsValid(data.Entity))
            data.AddLine(GetInvalid(LIVNotCorrect));
    }
}
