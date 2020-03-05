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

        private readonly string[] SuspiciousOTNames =
        {
            "PKHeX",
        };

        public override void Verify(LegalityAnalysis data)
        {
            var pkm = data.pkm;
            switch (data.EncounterMatch)
            {
                case EncounterTrade _:
                case MysteryGift g when !g.IsEgg:
                case EncounterStatic5N _:
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
            else if ((pkm.TID == 12345 && pkm.SID == 54321) || IsOTNameSuspicious(ot))
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
                    data.AddLine(GetInvalid($"Wordfilter: Too many numbers."));
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
                var len = Legal.GetMaxLengthOT(pkm.GenNumber, LanguageID.English); // max case
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

            VerifyG1OTWithinBounds(data, tr);
            if (data.EncounterOriginal is EncounterStatic s && (s.Version == GameVersion.Stadium || s.Version == GameVersion.Stadium2))
                data.AddLine(VerifyG1OTStadium(pkm, tr, s));

            if (pkm.Species == (int)Species.Mew)
            {
                var OTMatch = (tr == Legal.GetG1OT_GFMew((int)LanguageID.Japanese))
                              || (tr == Legal.GetG1OT_GFMew((int)LanguageID.English));
                if (!OTMatch || pkm.TID != 22796)
                    data.AddLine(GetInvalid(LG1OTEvent));
            }

            if (pkm.OT_Gender == 1 && ((pkm.Format == 2 && pkm.Met_Location == 0) || (pkm.Format > 2 && pkm.VC1)))
                data.AddLine(GetInvalid(LG1OTGender));
        }

        private void VerifyG1OTWithinBounds(LegalityAnalysis data, string str)
        {
            if (StringConverter12.GetIsG1English(str))
            {
                if (str.Length > 7 && !(data.EncounterOriginal is EncounterTrade)) // OT already verified; GER shuckle has 8 chars
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
            else
            {
                data.AddLine(GetInvalid(LG1CharOT));
            }
        }

        private CheckResult VerifyG1OTStadium(PKM pkm, string tr, IVersion s)
        {
            if (pkm.OT_Gender != 0)
                return GetInvalid(LG1OTGender);

            int tid = pkm.TID;
            if (pkm.Japanese)
            {
                if (tid == Legal.GetGBStadiumOTID_JPN(s.Version) && Legal.Stadium1JP == tr)
                    return GetValid(LG1StadiumJapanese);
            }
            else
            {
                if (s.Version == GameVersion.Stadium && tid == 2000)
                {
                    if (tr == "STADIUM" || tr == "STADE" || tr == "STADIO" || tr == "ESTADIO")
                        return GetValid(LG1StadiumInternational);
                }
                else // Stadium2
                {
                    if (tid == 2000 && tr == "Stadium")
                        return GetValid(LG1StadiumInternational);

                    if (tid == 2001 && (tr == "Stade" || tr == "Stadion" || tr == "Stadio" || tr == "Estadio"))
                        return GetValid(LG1StadiumInternational);
                }
            }
            return GetInvalid(LG1Stadium);
        }

        private bool IsOTNameSuspicious(string name)
        {
            return SuspiciousOTNames.Any(name.StartsWith);
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
                return c <= '9' && '0' <= c;
            }

            return str.Count(IsNumber);
        }
    }
}
