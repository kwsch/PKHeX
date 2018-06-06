using System;
using System.Collections.Generic;
using static PKHeX.Core.LegalityCheckStrings;

namespace PKHeX.Core
{
    public static class MysteryGiftVerifier
    {
        private static readonly Dictionary<int, MysteryGiftRestriction>[] RestrictionSet = Get();
        private static Dictionary<int, MysteryGiftRestriction>[] Get()
        {
            var s = new Dictionary<int, MysteryGiftRestriction>[PKX.Generation + 1];
            for (int i = 3; i < s.Length; i++)
                s[i] = GetRestriction(i);
            return s;
        }

        private static string RestrictionSetName(int i) => $"mgrestrict{i}.pkl";
        private static Dictionary<int, MysteryGiftRestriction> GetRestriction(int generation)
        {
            var resource = RestrictionSetName(generation);
            var data = Util.GetBinaryResource(resource);
            var dict = new Dictionary<int, MysteryGiftRestriction>();
            for (int i = 0; i < data.Length; i += 4 + 2)
            {
                int hash = BitConverter.ToInt32(data, i + 0);
                var restrict = BitConverter.ToUInt16(data, i + 4);
                dict.Add(hash, (MysteryGiftRestriction)restrict);
            }
            return dict;
        }

        public static CheckResult VerifyGift(PKM pk, MysteryGift g)
        {
            bool restricted = TryGetRestriction(g, out var val);
            if (!restricted)
                return new CheckResult(CheckIdentifier.GameOrigin);

            var lang = val & MysteryGiftRestriction.LangRestrict;
            if (lang != 0)
            {
                var current = 1 << pk.Language;
                if (!lang.HasFlagFast((MysteryGiftRestriction) current))
                {
                    int suggest = lang.GetSuggestedLanguage();
                    return new CheckResult(Severity.Invalid, string.Format(V5, suggest, pk.Language), CheckIdentifier.GameOrigin);
                }
            }
            var region = val & MysteryGiftRestriction.RegionRestrict;
            if (region != 0)
            {
                var current = (int)MysteryGiftRestriction.RegionBase << pk.ConsoleRegion;
                if (!region.HasFlagFast((MysteryGiftRestriction)current))
                    return new CheckResult(Severity.Invalid, V301, CheckIdentifier.GameOrigin);
            }

            return new CheckResult(CheckIdentifier.GameOrigin);
        }

        private static bool TryGetRestriction(MysteryGift g, out MysteryGiftRestriction val)
        {
            var restrict = RestrictionSet[g.Generation];
            if (restrict != null)
                return restrict.TryGetValue(g.GetHashCode(), out val);
            val = MysteryGiftRestriction.None;
            return false;
        }

        public static bool IsValidChangedOTName(PKM pk, MysteryGift g)
        {
            bool restricted = TryGetRestriction(g, out var val);
            if (!restricted)
                return false; // no data
            if (!val.HasFlagFast(MysteryGiftRestriction.OTReplacedOnTrade))
                return false;
            return CurrentOTMatchesReplaced(g.Format, pk.OT_Name);
        }

        private static bool CurrentOTMatchesReplaced(int format, string pkOtName)
        {
            if (format <= 4 && IsMatchName(pkOtName, 4))
                return true;
            if (format <= 5 && IsMatchName(pkOtName, 5))
                return true;
            if (format <= 6 && IsMatchName(pkOtName, 6))
                return true;
            if (format <= 7 && IsMatchName(pkOtName, 7))
                return true;
            return false;
        }

        private static bool IsMatchName(string pkOtName, int generation)
        {
            switch (generation)
            {
                // todo
                default:
                    return false;
            }
        }
    }
}
