using System.Linq;
using static PKHeX.Core.LegalityCheckStrings;

namespace PKHeX.Core
{
    /// <summary>
    /// Verifies the <see cref="PKM.OT_Name"/>.
    /// </summary>
    public sealed class TrainerNameVerifier : Verifier
    {
        protected override CheckIdentifier Identifier => CheckIdentifier.Trainer;

        private static readonly string[] SuspiciousOTNames =
        {
            "PKHeX",
            "ＰＫＨｅＸ",
        };

        public override void Verify(LegalityAnalysis data)
        {
            var pkm = data.pkm;
            switch (data.EncounterMatch)
            {
                case EncounterTrade:
                case MysteryGift {IsEgg: false}:
                case EncounterStatic5N:
                    return; // already verified
            }

            var ot = pkm.OT_Name;
            if (ot.Length == 0)
                data.AddLine(GetInvalid(LOTShort));

            if (pkm.TID == 0 && pkm.SID == 0)
            {
                data.AddLine(Get(LOT_IDs0, Severity.Fishy));
            }
            else if (pkm.VC)
            {
                if (pkm.SID != 0)
                    data.AddLine(GetInvalid(LOT_SID0Invalid));
            }
            else if (pkm.TID == pkm.SID)
            {
                data.AddLine(Get(LOT_IDEqual, Severity.Fishy));
            }
            else if (pkm.TID == 0)
            {
                data.AddLine(Get(LOT_TID0, Severity.Fishy));
            }
            else if (pkm.SID == 0)
            {
                data.AddLine(Get(LOT_SID0, Severity.Fishy));
            }
            else if (IsOTNameSuspicious(ot))
            {
                data.AddLine(Get(LOTSuspicious, Severity.Fishy));
            }
            else if (IsOTIDSuspicious(pkm.TID, pkm.SID))
            {
                data.AddLine(Get(LOTSuspicious, Severity.Fishy));
            }

            if (pkm.VC)
            {
                VerifyOTG1(data);
            }
            else if (ot.Length > Legal.GetMaxLengthOT(data.Info.Generation, (LanguageID)pkm.Language))
            {
                if (!IsEdgeCaseLength(pkm, data.EncounterOriginal, ot))
                    data.AddLine(Get(LOTLong, Severity.Invalid));
            }

            if (ParseSettings.CheckWordFilter)
            {
                if (WordFilter.IsFiltered(ot, out string bad))
                    data.AddLine(GetInvalid($"Wordfilter: {bad}"));
                if (WordFilter.IsFiltered(pkm.HT_Name, out bad))
                    data.AddLine(GetInvalid($"Wordfilter: {bad}"));
                if (ContainsTooManyNumbers(ot, data.Info.Generation))
                    data.AddLine(GetInvalid("Wordfilter: Too many numbers."));
            }
        }

        public static bool IsEdgeCaseLength(PKM pkm, IEncounterable e, string ot)
        {
            if (e.EggEncounter)
            {
                if (e is WC3 wc3 && pkm.IsEgg && wc3.OT_Name == ot)
                    return true; // Fixed OT Mystery Gift Egg
                bool eggEdge = pkm.IsEgg ? pkm.IsTradedEgg || pkm.Format == 3 : pkm.WasTradedEgg;
                if (!eggEdge)
                    return false;
                var len = Legal.GetMaxLengthOT(e.Generation, LanguageID.English); // max case
                return ot.Length <= len;
            }

            if (e is MysteryGift mg && mg.OT_Name.Length == ot.Length)
                return true; // Mattle Ho-Oh
            return false;
        }

        public void VerifyOTG1(LegalityAnalysis data)
        {
            var pkm = data.pkm;
            string tr = pkm.OT_Name;

            if (tr.Length == 0)
            {
                if (pkm is SK2 {TID: 0, IsRental: true})
                {
                    data.AddLine(Get(LOTShort, Severity.Fishy));
                }
                else
                {
                    data.AddLine(GetInvalid(LOTShort));
                    return;
                }
            }

            VerifyG1OTWithinBounds(data, tr);

            if (pkm.OT_Gender == 1 && ((pkm.Format == 2 && pkm.Met_Location == 0) || (pkm.Format > 2 && pkm.VC1)))
                data.AddLine(GetInvalid(LG1OTGender));
        }

        private void VerifyG1OTWithinBounds(LegalityAnalysis data, string str)
        {
            if (StringConverter12.GetIsG1English(str))
            {
                if (str.Length > 7 && data.EncounterOriginal is not EncounterTradeGB) // OT already verified; GER shuckle has 8 chars
                    data.AddLine(GetInvalid(LOTLong));
            }
            else if (StringConverter12.GetIsG1Japanese(str))
            {
                if (str.Length > 5)
                    data.AddLine(GetInvalid(LOTLong));
            }
            else if (data.pkm.Korean && StringConverter2KOR.GetIsG2Korean(str))
            {
                if (str.Length > 5)
                    data.AddLine(GetInvalid(LOTLong));
            }
            else if (data.EncounterOriginal is not EncounterTrade2) // OT already verified; SPA Shuckle/Voltorb transferred from French can yield 2 inaccessible chars
            {
                data.AddLine(GetInvalid(LG1CharOT));
            }
        }

        private static bool IsOTNameSuspicious(string name)
        {
            return SuspiciousOTNames.Any(name.StartsWith);
        }

        public static bool IsOTIDSuspicious(int tid16, int sid16)
        {
            if (tid16 == 12345 && sid16 == 54321)
                return true;

            // 1234_123456 (SID7_TID7)
            if (tid16 == 15040 && sid16 == 18831)
                return true;

            return false;
        }

        public static bool ContainsTooManyNumbers(string str, int originalGeneration)
        {
            if (originalGeneration <= 3)
                return false; // no limit from these generations
            int max = originalGeneration < 6 ? 4 : 5;
            if (str.Length <= max)
                return false;
            int count = GetNumberCount(str);
            return count > max;
        }

        private static int GetNumberCount(string str)
        {
            static bool IsNumber(char c)
            {
                if ('０' <= c)
                    return c <= '９';
                return c is >= '0' and <= '9';
            }

            return str.Count(IsNumber);
        }
    }
}
