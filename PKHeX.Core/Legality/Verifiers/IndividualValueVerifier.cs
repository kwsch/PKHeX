using System;
using System.Collections.Generic;
using System.Linq;
using static PKHeX.Core.LegalityCheckStrings;

namespace PKHeX.Core
{
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
                data.AddLine(Get(V321, Severity.Fishy));
            else if (pkm.IVs[0] < 30 && pkm.IVs.All(iv => pkm.IVs[0] == iv))
                data.AddLine(Get(V32, Severity.Fishy));
        }

        private void VerifyIVsMystery(LegalityAnalysis data, MysteryGift g)
        {
            var pkm = data.pkm;
            int[] IVs = GetMysteryGiftIVs(g);
            if (IVs == null)
                return;

            var pkIVs = pkm.IVs;
            var ivflag = Array.Find(IVs, iv => (byte)(iv - 0xFC) < 3);
            if (ivflag == 0) // Random IVs
            {
                bool valid = GetIsFixedIVSequenceValid(IVs, pkIVs);
                if (!valid)
                    data.AddLine(GetInvalid(V30));
            }
            else
            {
                int IVCount = ivflag - 0xFB;  // IV2/IV3
                if (pkIVs.Count(iv => iv == 31) < IVCount)
                    data.AddLine(GetInvalid(string.Format(V28, IVCount)));
            }
        }

        private static bool GetIsFixedIVSequenceValid(IReadOnlyList<int> IVs, IReadOnlyList<int> pkIVs)
        {
            for (int i = 0; i < 6; i++)
                if (IVs[i] <= 31 && IVs[i] != pkIVs[i])
                    return false;
            return true;
        }

        private static int[] GetMysteryGiftIVs(MysteryGift g)
        {
            switch (g)
            {
                case WC7 wc7: return wc7.IVs;
                case WC6 wc6: return wc6.IVs;
                case PGF pgf: return pgf.IVs;
                default: return null;
            }
        }

        private void VerifyIVsSlot(LegalityAnalysis data, EncounterSlot w)
        {
            var pkm = data.pkm;
            bool force2 = w.Type == SlotType.FriendSafari || w.Generation == 7 && pkm.AbilityNumber == 4;
            if (force2 && pkm.IVs.Count(iv => iv == 31) < 2)
                data.AddLine(GetInvalid(w.Type == SlotType.FriendSafari ? V29 : string.Format(V28, 2)));
        }

        private void VerifyIVsStatic(LegalityAnalysis data, EncounterStatic s)
        {
            var pkm = data.pkm;
            if (s.FlawlessIVCount != 0 && pkm.IVs.Count(iv => iv == 31) < s.FlawlessIVCount)
                data.AddLine(GetInvalid(string.Format(V28, s.FlawlessIVCount)));
        }
    }
}