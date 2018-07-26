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
                case EncounterStaticPID s when s.NSparkle:
                    return; // already verified
            }

            var ot = pkm.OT_Name;
            if (ot.Length == 0)
                data.AddLine(GetInvalid(V106));

            if (pkm.TID == 0 && pkm.SID == 0)
                data.AddLine(Get(V33, Severity.Fishy));
            else if (pkm.VC)
            {
                if (pkm.SID != 0)
                    data.AddLine(GetInvalid(V34));
            }
            else if (pkm.TID == pkm.SID)
                data.AddLine(Get(V35, Severity.Fishy));
            else if (pkm.TID == 0)
                data.AddLine(Get(V36, Severity.Fishy));
            else if (pkm.SID == 0)
                data.AddLine(Get(V37, Severity.Fishy));
            else if (pkm.TID == 12345 && pkm.SID == 54321 || IsOTNameSuspicious(ot))
                data.AddLine(Get(V417, Severity.Fishy));

            if (pkm.VC)
                VerifyOTG1(data);

            if (Legal.CheckWordFilter)
            {
                if (WordFilter.IsFiltered(ot, out string bad))
                    data.AddLine(GetInvalid($"Wordfilter: {bad}"));
                if (WordFilter.IsFiltered(pkm.HT_Name, out bad))
                    data.AddLine(GetInvalid($"Wordfilter: {bad}"));
            }
        }
        public void VerifyOTG1(LegalityAnalysis data)
        {
            var pkm = data.pkm;
            string tr = pkm.OT_Name;

            VerifyG1OTWithinBounds(data, tr);
            if (data.EncounterOriginal is EncounterStatic s && (s.Version == GameVersion.Stadium || s.Version == GameVersion.Stadium2))
                data.AddLine(VerifyG1OTStadium(pkm, tr, s));

            if (pkm.Species == 151)
            {
                var OTMatch = (tr == Legal.GetG1OT_GFMew((int)LanguageID.Japanese))
                              || (tr == Legal.GetG1OT_GFMew((int)LanguageID.English));
                if (!OTMatch || pkm.TID != 22796)
                    data.AddLine(GetInvalid(V39));
            }

            if (pkm.OT_Gender == 1 && (pkm.Format == 2 && pkm.Met_Location == 0 || pkm.Format > 2 && pkm.VC1))
                data.AddLine(GetInvalid(V408));
        }
        private void VerifyG1OTWithinBounds(LegalityAnalysis data, string str)
        {
            if (StringConverter.GetIsG1English(str))
            {
                if (str.Length > 7 && !(data.EncounterOriginal is EncounterTrade)) // OT already verified; GER shuckle has 8 chars
                    data.AddLine(GetInvalid(V38));
            }
            else if (StringConverter.GetIsG1Japanese(str))
            {
                if (str.Length > 5)
                    data.AddLine(GetInvalid(V38));
            }
            else if (data.pkm.Korean && StringConverter.GetIsG2Korean(str))
            {
                if (str.Length > 5)
                    data.AddLine(GetInvalid(V38));
            }
            else
            {
                data.AddLine(GetInvalid(V421));
            }
        }
        private CheckResult VerifyG1OTStadium(PKM pkm, string tr, EncounterStatic s)
        {
            bool jp = pkm.Japanese;
            bool valid;
            if (s.Version == GameVersion.Stadium)
                valid = pkm.TID == 1999 && tr == (jp ? "スタジアム" : "STADIUM");
            else // == GameVersion.Stadium2
                valid = pkm.TID == 2000 && tr == (jp ? "スタジアム" : "Stadium");
            return valid ? GetValid(jp ? V404 : V403) : GetInvalid(V402);
        }

        private bool IsOTNameSuspicious(string name)
        {
            foreach (var ot in SuspiciousOTNames)
                if (name.StartsWith(ot))
                    return true;
            return false;
        }
    }
}
