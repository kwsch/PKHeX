using System;

namespace PKHeX.Core
{
    /// <summary>
    /// Restriction Flags for receiving 3DS/NDS era events.
    /// </summary>
    [Flags]
    public enum MysteryGiftRestriction
    {
        None = 0,
        LangJapanese = 1 << LanguageID.Japanese,
        LangEnglish = 1 << LanguageID.English,
        LangFrench = 1 << LanguageID.French,
        LangItalian = 1 << LanguageID.Italian,
        LangGerman = 1 << LanguageID.German,
        LangSpanish = 1 << LanguageID.Spanish,
        LangKorean = 1 << LanguageID.Korean,

        RegionBase = LangKorean,
        LangRestrict = LangJapanese | LangEnglish | LangFrench | LangItalian | LangGerman | LangSpanish | LangKorean,

        RegJP = RegionBase << RegionID.Japan,
        RegNA = RegionBase << RegionID.NorthAmerica,
        RegEU = RegionBase << RegionID.Europe,
        RegZH = RegionBase << RegionID.China,
        RegKO = RegionBase << RegionID.Korea,
        RegTW = RegionBase << RegionID.Taiwan,

        RegionRestrict = RegJP | RegNA | RegEU | RegZH | RegKO | RegTW,

        OTReplacedOnTrade = RegTW << 1,
    }

    public static class MysteryGiftRestrictionExtensions
    {
        public static bool HasFlagFast(this MysteryGiftRestriction value, MysteryGiftRestriction flag)
        {
            return (value & flag) != 0;
        }

        /// <summary>
        /// Checks the flags to pick out a language that can receive the gift.
        /// </summary>
        /// <param name="value">Flag value</param>
        /// <returns>Language ID; -1 if none</returns>
        public static int GetSuggestedLanguage(this MysteryGiftRestriction value)
        {
            for (int i = (int)LanguageID.Japanese; i <= (int)LanguageID.Korean; i++)
            {
                if (value.HasFlagFast((MysteryGiftRestriction)(1 << i)))
                    return i;
            }
            return -1;
        }

        public static int GetSuggestedRegion(this MysteryGiftRestriction value)
        {
            for (int i = (int)RegionID.Japan; i <= (int)RegionID.Taiwan; i++)
            {
                if (value.HasFlagFast((MysteryGiftRestriction)((int)MysteryGiftRestriction.RegionBase << i)))
                    return i;
            }
            return -1;
        }
    }
}