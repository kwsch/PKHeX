namespace PKHeX.Core
{
    public class FrameGenerator
    {
        public uint Nature;
        public readonly bool Gendered;
        public readonly int GenderHigh;
        public readonly int GenderLow;
        public readonly bool DPPt;
        public readonly bool AllowLeads;
        public readonly FrameType FrameType = FrameType.None;
        public readonly RNG RNG;
        public readonly bool Safari3;

        public Frame GetFrame(uint seed, LeadRequired lead) => new Frame(seed, FrameType, RNG, lead);
        public Frame GetFrame(uint seed, LeadRequired lead, uint esv, uint origin) => GetFrame(seed, lead, esv, esv, origin);

        public Frame GetFrame(uint seed, LeadRequired lead, uint esv, uint lvl, uint origin) => new Frame(seed, FrameType, RNG, lead)
        {
            RandESV = esv,
            RandLevel = lvl,
            OriginSeed = origin,
        };

        /// <summary>
        /// Gets the Search Criteria parameters necessary for generating <see cref="SeedInfo"/> and <see cref="Frame"/> objects.
        /// </summary>
        /// <param name="pidiv">Info used to determine the <see cref="FrameType"/>.</param>
        /// <param name="pk"><see cref="PKM"/> object containing various accessible information required for the encounter.</param>
        /// <returns>Object containing search criteria to be passed by reference to search/filter methods.</returns>
        public FrameGenerator(PIDIV pidiv, PKM pk)
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
                    FrameType = FrameType.MethodH;
                    RNG = pidiv.RNG;
                    Safari3 = pk.Ball == 5 && !pk.FRLG;

                    if (ver != GameVersion.E)
                        return;

                    AllowLeads = true;

                    // Cute Charm waits for gender too!
                    var gender = pk.Gender;
                    bool gendered = gender != 2;
                    if (!gendered)
                        return;

                    var gr = pk.PersonalInfo.Gender;
                    Gendered = true;
                    GenderLow = GetGenderMinMax(gender, gr, false);
                    GenderHigh = GetGenderMinMax(gender, gr, true);
                    return;

                // Method J
                case GameVersion.D:
                case GameVersion.P:
                case GameVersion.Pt:
                    DPPt = true;
                    AllowLeads = true;
                    FrameType = FrameType.MethodJ;
                    RNG = pidiv.RNG;
                    return;

                // Method K
                case GameVersion.HG:
                case GameVersion.SS:
                    DPPt = false;
                    AllowLeads = true;
                    FrameType = FrameType.MethodK;
                    RNG = pidiv.RNG;
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
        private static int GetGenderMinMax(int gender, int ratio, bool max)
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
