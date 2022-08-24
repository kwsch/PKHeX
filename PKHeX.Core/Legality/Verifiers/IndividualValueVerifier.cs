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
            case EncounterStatic s:
                VerifyIVsStatic(data, s);
                break;
            case EncounterSlot w:
                VerifyIVsSlot(data, w);
                break;
            case MysteryGift g:
                VerifyIVsMystery(data, g);
                break;
        }

        var pk = data.Entity;
        {
            var hpiv = pk.IV_HP;
            if (hpiv < 30 && AllIVsEqual(pk, hpiv))
                data.AddLine(Get(string.Format(LIVAllEqual_0, hpiv), Severity.Fishy));
        }
    }

    private static bool AllIVsEqual(PKM pk, int hpiv)
    {
        return (pk.IV_ATK == hpiv) && (pk.IV_DEF == hpiv) && (pk.IV_SPA == hpiv) && (pk.IV_SPD == hpiv) && (pk.IV_SPE == hpiv);
    }

    private void VerifyIVsMystery(LegalityAnalysis data, MysteryGift g)
    {
        if (!g.HasFixedIVs)
            return; // PID/IV style instead of fixed IVs.

        Span<int> IVs = stackalloc int[6];
        g.GetIVs(IVs);
        var ivflag = IVs.Find(static iv => (byte)(iv - 0xFC) < 3);
        if (ivflag == default) // Random IVs
        {
            bool valid = Legal.GetIsFixedIVSequenceValidSkipRand(IVs, data.Entity);
            if (!valid)
                data.AddLine(GetInvalid(LEncGiftIVMismatch));
        }
        else
        {
            int IVCount = ivflag - 0xFB;  // IV2/IV3
            VerifyIVsFlawless(data, IVCount);
        }
    }

    private void VerifyIVsSlot(LegalityAnalysis data, EncounterSlot w)
    {
        switch (w.Generation)
        {
            case 6: VerifyIVsGen6(data, w); break;
            case 7: VerifyIVsGen7(data); break;
            case 8: VerifyIVsGen8(data); break;
        }
    }

    private void VerifyIVsGen7(LegalityAnalysis data)
    {
        var pk = data.Entity;
        if (pk.GO)
            VerifyIVsGoTransfer(data);
        else if (pk.AbilityNumber == 4 && !AbilityVerifier.CanAbilityPatch(pk.Format, pk.PersonalInfo.Abilities, pk.Species))
            VerifyIVsFlawless(data, 2); // Chain of 10 yields 5% HA and 2 flawless IVs
    }

    private void VerifyIVsGen8(LegalityAnalysis data)
    {
        var pk = data.Entity;
        if (pk.GO)
            VerifyIVsGoTransfer(data);

        if (data.EncounterMatch is EncounterSlot8a s)
            VerifyIVsFlawless(data, s.FlawlessIVCount);
    }

    private void VerifyIVsGen6(LegalityAnalysis data, EncounterSlot w)
    {
        if (w is EncounterSlot6XY xy)
        {
            if (PersonalTable.XY[xy.Species].IsEggGroup(15)) // Undiscovered
                VerifyIVsFlawless(data, 3);
            if (xy.IsFriendSafari)
                VerifyIVsFlawless(data, 2);
        }
    }

    private void VerifyIVsFlawless(LegalityAnalysis data, int count)
    {
        if (data.Entity.FlawlessIVCount < count)
            data.AddLine(GetInvalid(string.Format(LIVF_COUNT0_31, count)));
    }

    private void VerifyIVsStatic(LegalityAnalysis data, EncounterStatic s)
    {
        if (s.FlawlessIVCount != 0)
            VerifyIVsFlawless(data, s.FlawlessIVCount);
    }

    private void VerifyIVsGoTransfer(LegalityAnalysis data)
    {
        if (data.EncounterMatch is EncounterSlotGO g && !g.GetIVsValid(data.Entity))
            data.AddLine(GetInvalid(LIVNotCorrect));
    }
}
