using System;
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
            var IVs = g.IVs;
            if (IVs.Length == 0)
                return;

            var ivflag = Array.Find(IVs, iv => (byte)(iv - 0xFC) < 3);
            if (ivflag == 0) // Random IVs
            {
                bool valid = Legal.GetIsFixedIVSequenceValidSkipRand(IVs, data.pkm);
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

        private void VerifyIVsGen8(LegalityAnalysis data)
        {
            // todo special rules
        }

        private void VerifyIVsGen7(LegalityAnalysis data)
        {
            var pkm = data.pkm;
            if (pkm.GG)
            {
                if (pkm.Version == (int)GameVersion.GO)
                    VerifyIVsGoTransfer(data);
            }
            else if (pkm.AbilityNumber == 4)
            {
                VerifyIVsFlawless(data, 2); // Chain of 10 yields 5% HA and 2 flawless IVs
            }
        }

        private void VerifyIVsGen6(LegalityAnalysis data, EncounterSlot w)
        {
            var pkm = data.pkm;
            if (pkm.XY && PersonalTable.XY[data.EncounterMatch.Species].IsEggGroup(15)) // Undiscovered
                VerifyIVsFlawless(data, 3);
            else if (w.Type == SlotType.FriendSafari)
                VerifyIVsFlawless(data, 2);
        }

        private void VerifyIVsFlawless(LegalityAnalysis data, int count)
        {
            if (data.pkm.FlawlessIVCount < count)
                data.AddLine(GetInvalid(string.Format(LIVF_COUNT0_31, count)));
        }

        private void VerifyIVsStatic(LegalityAnalysis data, EncounterStatic s)
        {
            if (s.FlawlessIVCount != 0)
                VerifyIVsFlawless(data, s.FlawlessIVCount);
        }

        private void VerifyIVsGoTransfer(LegalityAnalysis data)
        {
            var pkm = data.pkm;
            if (!IsGoIVSetValid(pkm))
                data.AddLine(GetInvalid(LIVNotCorrect));

            if (!pkm.IsShiny)
                return;
            var banlist = Legal.GoTransferSpeciesShinyBan;

            // all Shiny Alola Forms are legal, while some of their respective Kanto Forms are not
            if (banlist.Contains(pkm.Species) && pkm.AltForm != 1)
                data.AddLine(GetInvalid(LEncStaticPIDShiny, CheckIdentifier.PID));
        }

        private static bool IsGoIVSetValid(PKM pkm)
        {
            // Stamina*2 | 1 -> HP
            // ATK * 2 | 1 -> ATK&SPA
            // DEF * 2 | 1 -> DEF&SPD
            // Speed is random.

            // All IVs must be odd (except speed) and equal to their counterpart.
            if ((pkm.GetIV(1) & 1) != 1 || pkm.GetIV(1) != pkm.GetIV(4)) // ATK=SPA
                return false;
            if ((pkm.GetIV(2) & 1) != 1 || pkm.GetIV(2) != pkm.GetIV(5)) // DEF=SPD
                return false;
            return (pkm.GetIV(0) & 1) == 1; // HP
        }
    }
}