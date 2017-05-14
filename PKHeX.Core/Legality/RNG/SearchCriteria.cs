namespace PKHeX.Core
{
    public class SearchCriteria
    {
        public uint Nature;
        public bool Gendered;
        public int GenderHigh;
        public int GenderLow;
        public bool DPPt;
        public bool CanSync;
        public bool MethodH;

        /// <summary>
        /// Gets the Search Criteria parameters necessary for generating <see cref="SeedInfo"/> and <see cref="SlotResult"/> objects.
        /// </summary>
        /// <param name="pk"><see cref="PKM"/> object containing various accessible information required for the encounter.</param>
        /// <returns>Object containing search criteria to be passed by reference to search/filter methods.</returns>
        public static SearchCriteria getSearchCriteria(PKM pk)
        {
            var ver = (GameVersion)pk.Version;
            switch (ver)
            {
                // Method H
                case GameVersion.R:
                case GameVersion.S:
                case GameVersion.FR:
                case GameVersion.LG:
                    return new SearchCriteria
                    {
                        Nature = pk.EncryptionConstant % 25,
                        DPPt = false,
                        MethodH = true,
                    };

                // Method H with Emerald Features
                case GameVersion.E:

                    // Cute Charm waits for gender too!
                    var gender = pk.Gender;
                    bool gendered = ver == GameVersion.E && gender != 2;

                    var criteria = new SearchCriteria
                    {
                        Nature = pk.EncryptionConstant % 25,
                        DPPt = false,
                        CanSync = true,
                        MethodH = true,
                    };
                    if (gendered)
                    {
                        var gr = pk.PersonalInfo.Gender;
                        criteria.Gendered = true;
                        criteria.GenderLow = getGenderMinMax(gender, gr, false);
                        criteria.GenderHigh = getGenderMinMax(gender, gr, true);
                    }
                    return criteria;

                // Method J
                case GameVersion.D:
                case GameVersion.P:
                case GameVersion.Pt:
                    return new SearchCriteria
                    {
                        Nature = pk.EncryptionConstant % 25,
                        DPPt = true,
                        CanSync = true,
                    };

                // Method K
                case GameVersion.HG:
                case GameVersion.SS:
                    return new SearchCriteria
                    {
                        Nature = pk.EncryptionConstant % 25,
                        DPPt = false,
                        CanSync = true,
                    };

                default:
                    return null;
            }
        }

        /// <summary>
        /// Gets the span of values for a given Gender
        /// </summary>
        /// <param name="gender">Gender</param>
        /// <param name="ratio">Gender Ratio</param>
        /// <param name="max">Return Max (or Min)</param>
        /// <returns>Returns the maximum or minimum gender value that corresponds to the input gender ratio.</returns>
        private static int getGenderMinMax(int gender, int ratio, bool max)
        {
            if (ratio == 0 || ratio == 0xFE || ratio == 0xFF)
                gender = 2;
            switch (gender)
            {
                case 0: return max ? 255 : ratio; // male
                case 1: return max ? ratio - 1 : 0; // female
                default: return max ? 255 : 0; // fixed/genderless
            }
        }
    }
}
