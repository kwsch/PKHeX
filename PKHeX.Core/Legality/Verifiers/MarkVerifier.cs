using static PKHeX.Core.LegalityCheckStrings;

namespace PKHeX.Core
{
    /// <summary>
    /// Verifies the <see cref="RibbonIndex"/> values for markings.
    /// </summary>
    public sealed class MarkVerifier : Verifier
    {
        protected override CheckIdentifier Identifier => CheckIdentifier.RibbonMark;

        public override void Verify(LegalityAnalysis data)
        {
            var pkm = data.pkm;
            if (pkm is not IRibbonIndex m)
                return;

            if (data.Info.Generation != 8)
                VerifyNoMarksPresent(data, m);
            else
                VerifyMarksPresent(data, m);

            VerifyAffixedRibbonMark(data, m);
        }

        private void VerifyNoMarksPresent(LegalityAnalysis data, IRibbonIndex m)
        {
            for (var x = RibbonIndex.MarkLunchtime; x <= RibbonIndex.MarkSlump; x++)
            {
                if (m.GetRibbon((int)x))
                    data.AddLine(GetInvalid(string.Format(LRibbonMarkingFInvalid_0, x)));
            }
        }

        private void VerifyMarksPresent(LegalityAnalysis data, IRibbonIndex m)
        {
            bool hasOne = false;
            for (var mark = RibbonIndex.MarkLunchtime; mark <= RibbonIndex.MarkSlump; mark++)
            {
                bool has = m.GetRibbon((int) mark);
                if (!has)
                    continue;

                if (hasOne)
                {
                    data.AddLine(GetInvalid(string.Format(LRibbonMarkingFInvalid_0, mark)));
                    return;
                }

                bool result = IsMarkValid(mark, data.pkm, data.EncounterMatch);
                if (!result)
                {
                    data.AddLine(GetInvalid(string.Format(LRibbonMarkingFInvalid_0, mark)));
                    return;
                }

                hasOne = true;
            }
        }

        public static bool IsMarkValid(RibbonIndex mark, PKM pk, IEncounterable enc)
        {
            return IsMarkAllowedAny(enc) && IsMarkAllowedSpecific(mark, pk, enc);
        }

        public static bool IsMarkAllowedSpecific(RibbonIndex mark, PKM pk, IEncounterable x) => mark switch
        {
            RibbonIndex.MarkCurry when !IsMarkAllowedCurry(pk, x) => false,
            RibbonIndex.MarkFishing when !IsMarkAllowedFishing(x) => false,
            RibbonIndex.MarkDestiny => false,
            _ => true
        };

        public static bool IsMarkAllowedAny(IEncounterable enc) => enc.Generation == 8 && enc switch
        {
            // Gen 8
            WC8 or EncounterEgg or EncounterTrade or EncounterSlot8GO
                or EncounterStatic8U or EncounterStatic8N or EncounterStatic8ND or EncounterStatic8NC
                or EncounterStatic8 {Gift: true}
                or EncounterStatic8 {ScriptedNoMarks: true}
                => false,
            _ => true,
        };

        public static bool IsMarkAllowedCurry(PKM pkm, IEncounterable enc)
        {
            // Curry are only encounter slots, from the hidden table (not symbol). Slots taken from area's current weather(?).
            if (enc is not EncounterSlot8 s)
                return false;

            var area = (EncounterArea8)s.Area;
            if (area.PermitCrossover)
                return false;

            var weather = s.Weather;
            if ((weather & AreaWeather8.All) == 0)
                return false;

            if (EncounterArea8.IsWildArea(s.Location))
                return false;
            var ball = pkm.Ball;
            return (uint)(ball - 2) <= 2;
        }

        public static bool IsMarkAllowedFishing(IEncounterable enc)
        {
            // Fishing are only encounter slots, from the hidden table (not symbol).
            if (enc is not EncounterSlot8 s)
                return false;

            var area = (EncounterArea8)s.Area;
            if (area.PermitCrossover)
                return false;

            var weather = s.Weather;
            return (weather & AreaWeather8.Fishing) != 0;
        }

        private void VerifyAffixedRibbonMark(LegalityAnalysis data, IRibbonIndex m)
        {
            if (m is not PK8 pk8)
                return;

            var affix = pk8.AffixedRibbon;
            if (affix == -1) // None
                return;

            if ((byte)affix > (int)RibbonIndex.MarkSlump)
                data.AddLine(GetInvalid(string.Format(LRibbonMarkingAffixedF_0, affix)));
            else if (!pk8.GetRibbonIndex((RibbonIndex)affix))
                data.AddLine(GetInvalid(string.Format(LRibbonMarkingAffixedF_0, (RibbonIndex)affix)));
        }
    }
}
