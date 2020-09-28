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
            if (!(pkm is IRibbonIndex m))
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

                bool result = VerifyMarking(data, mark);
                if (!result)
                {
                    data.AddLine(GetInvalid(string.Format(LRibbonMarkingFInvalid_0, mark)));
                    return;
                }

                hasOne = true;
            }
        }

        private static bool VerifyMarking(LegalityAnalysis data, RibbonIndex mark)
        {
            var pkm = data.pkm;
            switch (mark)
            {
                case RibbonIndex.MarkCurry:
                {
                    var ball = pkm.Ball;
                    if (!(2 <= ball && ball <= 4)) // Poke,Great,Ultra only
                        return false;
                    break;
                }
            }
            return true;
        }

        private void VerifyAffixedRibbonMark(LegalityAnalysis data, IRibbonIndex m)
        {
            if (!(m is PK8 pk8))
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
