using System;
using static PKHeX.Core.GameVersion;

namespace PKHeX.Core
{
    /// <summary>
    /// Generator class for Gen3/4 Frame patterns
    /// </summary>
    public sealed class FrameGenerator
    {
        public uint Nature { get; init; }
        public readonly bool Gendered;
        public readonly int GenderHigh;
        public readonly int GenderLow;
        public readonly bool DPPt;
        public readonly bool AllowLeads;
        public readonly FrameType FrameType;
        public readonly RNG RNG = RNG.LCRNG;
        public readonly bool Safari3;

        public Frame GetFrame(uint seed, LeadRequired lead) => new(seed, FrameType, lead);
        public Frame GetFrame(uint seed, LeadRequired lead, uint esv, uint origin) => GetFrame(seed, lead, esv, esv, origin);

        public Frame GetFrame(uint seed, LeadRequired lead, uint esv, uint lvl, uint origin) => new(seed, FrameType, lead)
        {
            RandESV = esv,
            RandLevel = lvl,
            OriginSeed = origin,
        };

        /// <summary>
        /// Gets the Search Criteria parameters necessary for generating <see cref="SeedInfo"/> and <see cref="Frame"/> objects for Gen3/4 mainline games.
        /// </summary>
        /// <param name="pk"><see cref="PKM"/> object containing various accessible information required for the encounter.</param>
        /// <returns>Object containing search criteria to be passed by reference to search/filter methods.</returns>
        public FrameGenerator(PKM pk)
        {
            var ver = (GameVersion)pk.Version;
            switch (ver)
            {
                // Method H
                case R or S or E or FR or LG:
                    DPPt = false;
                    FrameType = FrameType.MethodH;
                    Safari3 = pk.Ball == 5 && ver is not (FR or LG);

                    if (ver != E)
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
                case D or P or Pt:
                    DPPt = true;
                    AllowLeads = true;
                    FrameType = FrameType.MethodJ;
                    return;

                // Method K
                case HG or SS:
                    DPPt = false;
                    AllowLeads = true;
                    FrameType = FrameType.MethodK;
                    return;
                default:
                    throw new ArgumentException(nameof(ver));
            }
        }

        /// <summary>
        /// Gets the span of values for a given Gender
        /// </summary>
        /// <param name="gender">Gender</param>
        /// <param name="ratio">Gender Ratio</param>
        /// <param name="max">Return Max (or Min)</param>
        /// <returns>Returns the maximum or minimum gender value that corresponds to the input gender ratio.</returns>
        private static int GetGenderMinMax(int gender, int ratio, bool max) => ratio switch
        {
            0 or >254 => max ? 255 : 0,
            _ => gender switch
            {
                0 => max ? 255 : ratio, // male
                1 => max ? ratio - 1 : 0, // female
                _ => max ? 255 : 0,
            }
        };
    }
}
