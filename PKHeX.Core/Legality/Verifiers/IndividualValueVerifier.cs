using System;
using System.Collections.Generic;
using System.Linq;
using static PKHeX.Core.LegalityCheckStrings;

namespace PKHeX.Core
{
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

            var pkm = data.pkm;
            if (pkm.IVTotal == 0)
            {
                data.AddLine(Get(LFatefulMystery, Severity.Fishy));
            }
            else
            {
                var hpiv = pkm.IV_HP;
                if (hpiv < 30 && AllIVsEqual(pkm, hpiv))
                    data.AddLine(Get(LIVAllEqual, Severity.Fishy));
            }
        }

        public static bool AllIVsEqual(PKM pkm) => AllIVsEqual(pkm, pkm.IV_HP);

        private static bool AllIVsEqual(PKM pkm, int hpiv)
        {
            return (pkm.IV_ATK == hpiv) && (pkm.IV_DEF == hpiv) && (pkm.IV_SPA == hpiv) && (pkm.IV_SPD == hpiv) && (pkm.IV_SPE == hpiv);
        }

        private void VerifyIVsMystery(LegalityAnalysis data, MysteryGift g)
        {
            int[] IVs = g.IVs;
            if (IVs == null)
                return;

            var ivflag = Array.Find(IVs, iv => (byte)(iv - 0xFC) < 3);
            if (ivflag == 0) // Random IVs
            {
                bool valid = GetIsFixedIVSequenceValid(IVs, data.pkm.IVs);
                if (!valid)
                    data.AddLine(GetInvalid(LEncGiftIVMismatch));
            }
            else
            {
                int IVCount = ivflag - 0xFB;  // IV2/IV3
                VerifyIVsFlawless(data, IVCount);
            }
        }

        private static bool GetIsFixedIVSequenceValid(IReadOnlyList<int> IVs, IReadOnlyList<int> pkIVs)
        {
            for (int i = 0; i < 6; i++)
            {
                if (IVs[i] <= 31 && IVs[i] != pkIVs[i])
                    return false;
            }
            return true;
        }

        private void VerifyIVsSlot(LegalityAnalysis data, EncounterSlot w)
        {
            var pkm = data.pkm;
            if (w.Generation == 7 && pkm.AbilityNumber == 4) // SOS
                VerifyIVsFlawless(data, 2); // Chain of 10 yields 5% HA and 2 flawless IVs
            else if (pkm.XY && PersonalTable.XY[data.EncounterMatch.Species].IsEggGroup(15)) // Undiscovered
                VerifyIVsFlawless(data, 3);
            else if (w.Type == SlotType.FriendSafari)
                VerifyIVsFlawless(data, 2);
        }

        private void VerifyIVsFlawless(LegalityAnalysis data, int count)
        {
            if (data.pkm.IVs.Count(iv => iv == 31) < count)
                data.AddLine(GetInvalid(string.Format(LIVF_COUNT0_31, count)));
        }

        private void VerifyIVsStatic(LegalityAnalysis data, EncounterStatic s)
        {
            if (s.FlawlessIVCount != 0)
                VerifyIVsFlawless(data, s.FlawlessIVCount);
        }
    }
}