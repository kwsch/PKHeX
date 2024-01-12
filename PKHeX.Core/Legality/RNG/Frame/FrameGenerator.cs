using System;
using static PKHeX.Core.GameVersion;

namespace PKHeX.Core;

/// <summary>
/// Generator information for Gen3/4 Frame patterns
/// </summary>
public readonly struct FrameGenerator
{
    public required byte Nature { get; init; }
    public readonly byte GenderHigh;
    public readonly byte GenderLow;
    public readonly FrameType FrameType;
    public readonly bool DPPt;
    public readonly bool AllowLeads;
    public readonly bool Gendered;
    public readonly bool Safari3;

    public Frame GetFrame(uint seed, LeadRequired lead) => new(seed, FrameType, lead);
    public Frame GetFrame(uint seed, LeadRequired lead, ushort esv, uint origin) => GetFrame(seed, lead, esv, esv, origin);

    public Frame GetFrame(uint seed, LeadRequired lead, ushort esv, ushort lvl, uint origin) => new(seed, FrameType, lead)
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
        var version = (GameVersion)pk.Version;
        switch (version)
        {
            // Method H
            case R or S or E or FR or LG:
                DPPt = false;
                FrameType = FrameType.MethodH;
                Safari3 = pk.Ball == 5 && version is not (FR or LG);

                if (version != E)
                    return;

                AllowLeads = true;

                // Cute Charm waits for gender too!
                var gender = pk.Gender;
                bool gendered = gender != 2;
                if (!gendered)
                    return;

                var gr = pk.PersonalInfo.Gender;
                Gendered = true;
                (GenderLow, GenderHigh) = GetGenderMinMax(gender, gr);
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
                throw new ArgumentOutOfRangeException(nameof(version), version, "Unknown version.");
        }
    }

    /// <summary>
    /// Gets the span of values for a given Gender
    /// </summary>
    /// <param name="gender">Gender</param>
    /// <param name="ratio">Gender Ratio</param>
    /// <returns>Returns the maximum or minimum gender value that corresponds to the input gender ratio.</returns>
    private static (byte Min, byte Max) GetGenderMinMax(int gender, byte ratio) => ratio switch
    {
        PersonalInfo.RatioMagicMale => (0, 255),
        PersonalInfo.RatioMagicFemale => (0, 255),
        PersonalInfo.RatioMagicGenderless => (0, 255),
        _ => gender switch
        {
            0 => (ratio, 255), // male
            1 => (0, --ratio), // female
            _ => (0, 255),
        },
    };
}
