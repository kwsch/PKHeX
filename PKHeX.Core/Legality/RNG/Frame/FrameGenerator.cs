namespace PKHeX.Core
{
    public class FrameGenerator
    {
        public uint Nature;
        public readonly bool Gendered;
        public readonly int GenderHigh;
        public readonly int GenderLow;
        public readonly bool DPPt;
        public readonly bool CanSync;
        public readonly FrameType FrameType = FrameType.None;
        public Frame GetFrame(uint seed, LeadRequired lead) => new Frame(seed, FrameType, lead);

        /// <summary>
        /// Gets the Search Criteria parameters necessary for generating <see cref="SeedInfo"/> and <see cref="Frame"/> objects.
        /// </summary>
        /// <param name="pk"><see cref="PKM"/> object containing various accessible information required for the encounter.</param>
        /// <param name="type">Type info used to determine the <see cref="FrameType"/>.</param>
        /// <returns>Object containing search criteria to be passed by reference to search/filter methods.</returns>
        public FrameGenerator(PKM pk, PIDType type)
        {
            var ver = (GameVersion)pk.Version;
            switch (ver)
            {
                // Method H
                case GameVersion.R:
                case GameVersion.S:
                case GameVersion.FR:
                case GameVersion.LG:
                case GameVersion.E:
                    DPPt = false;
                    FrameType = getFrameType(type);

                    if (ver != GameVersion.E)
                        return;

                    CanSync = true;

                    // Cute Charm waits for gender too!
                    var gender = pk.Gender;
                    bool gendered = gender != 2;
                    if (!gendered)
                        return;

                    var gr = pk.PersonalInfo.Gender;
                    Gendered = true;
                    GenderLow = getGenderMinMax(gender, gr, false);
                    GenderHigh = getGenderMinMax(gender, gr, true);
                    return;

                // Method J
                case GameVersion.D:
                case GameVersion.P:
                case GameVersion.Pt:
                    DPPt = true;
                    CanSync = true;
                    FrameType = FrameType.MethodJ;
                    return;

                // Method K
                case GameVersion.HG:
                case GameVersion.SS:
                    DPPt = false;
                    CanSync = true;
                    FrameType = FrameType.MethodK;
                    return;
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

        private static FrameType getFrameType(PIDType type)
        {
            switch (type)
            {
                case PIDType.Method_1:
                case PIDType.Method_1_Unown:
                    return FrameType.MethodH1;
                case PIDType.Method_2:
                case PIDType.Method_2_Unown:
                    return FrameType.MethodH2;
                case PIDType.Method_4:
                case PIDType.Method_4_Unown:
                    return FrameType.MethodH4;
            }
            return FrameType.None;
        }
    }
}
