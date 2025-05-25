using System;

namespace PKHeX.Core;

/// <summary>
/// Logic for interacting with Entity Gender
/// </summary>
public static class EntityGender
{
    public const byte Male = 0;
    public const byte Female = 1;
    public const byte Genderless = 2;

    /// <summary>
    /// Translates a Gender string to Gender integer.
    /// </summary>
    /// <param name="s">Gender string</param>
    /// <returns>Gender integer</returns>
    public static byte GetFromString(ReadOnlySpan<char> s)
    {
        if (s.Length != 1)
            return Genderless;
        return GetFromChar(s[0]);
    }

    /// <summary>
    /// Translates a Gender char to Gender integer.
    /// </summary>
    public static byte GetFromChar(char c) => c switch
    {
        '♂' or 'M' => Male,
        '♀' or 'F' => Female,
        _ => Genderless,
    };

    /// <summary>
    /// Gets the gender ID of the species based on the Personality ID.
    /// </summary>
    /// <param name="species">National Dex ID.</param>
    /// <param name="pid">Personality ID.</param>
    /// <returns>Gender ID (0/1/2)</returns>
    /// <remarks>This method should only be used for Generations 3-5 origin.</remarks>
    public static byte GetFromPID(ushort species, uint pid)
    {
        var gt = GetGenderRatio(species);
        return GetFromPIDAndRatio(pid, gt);
    }

    /// <summary>
    /// Gets the gender from the <see cref="pid"/> and <see cref="gr"/> values.
    /// </summary>
    /// <param name="pid">Personality ID.</param>
    /// <param name="gr">Gender Ratio.</param>
    /// <returns>Gender ID (0/1/2)</returns>
    /// <remarks>This method should only be used for Generations 3-5 origin.</remarks>
    public static byte GetFromPIDAndRatio(uint pid, byte gr) => gr switch
    {
        PersonalInfo.RatioMagicGenderless => Genderless,
        PersonalInfo.RatioMagicFemale => Female,
        PersonalInfo.RatioMagicMale => Male,
        _ => (pid & 0xFF) < gr ? Female : Male,
    };

    /// <summary>
    /// Checks if the species (base form) can be female.
    /// </summary>
    public static bool IsFemaleOrDualGender(ushort species) => GetGenderRatio(species) is not (OM or NG);

    /// <summary>
    /// Gets the magic gender value used in past generations (3-8) for logic checks.
    /// </summary>
    public static byte GetGenderRatio(ushort species)
    {
        var arr = GenderRatios;
        if (species < arr.Length)
            return arr[species];
        return NG;
    }

    private static ReadOnlySpan<byte> GenderRatios =>
    [
        NG, VM, VM, VM, VM, VM, VM, VM, VM, VM, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, OF, OF, OF, OM, OM, OM, MF, MF, MF, MF, MF,
        MF, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, MM, MM, HH, HH, HH, MM, MM, MM, MM, MM, MM, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH,
        HH, NG, NG, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, NG, NG, HH, HH, HH, HH, OM, OM, HH, HH, HH, HH, HH, OF, HH, OF, HH, HH, HH, HH,
        NG, NG, HH, HH, OF, MM, MM, HH, OM, HH, HH, HH, NG, VM, VM, VM, VM, NG, VM, VM, VM, VM, VM, VM, NG, NG, NG, HH, HH, HH, NG, NG, VM, VM, VM, VM, VM, VM, VM, VM,
        VM, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, MF, MF, VM, VM, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, VM, VM, HH, HH,
        // 200
        HH, NG, HH, HH, HH, HH, HH, HH, HH, MF, MF, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, MF, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, NG, HH, HH, OM, OM, OF, MM,
        MM, OF, OF, NG, NG, NG, HH, HH, HH, NG, NG, NG, VM, VM, VM, VM, VM, VM, VM, VM, VM, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH,
        HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, NG, HH, HH, HH, MM, MM, MF, HH, MF, MF, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, OM, OF, HH, HH, HH, HH, HH,
        HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, NG, NG, HH, HH, HH, HH, NG, NG, VM, VM, VM, VM, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH,
        HH, HH, HH, HH, HH, HH, HH, HH, HH, VM, MF, HH, HH, HH, NG, NG, NG, NG, NG, NG, OF, OM, NG, NG, NG, NG, NG, VM, VM, VM, VM, VM, VM, VM, VM, VM, HH, HH, HH, HH,
        // 400
        HH, HH, HH, HH, HH, HH, HH, HH, VM, VM, VM, VM, HH, OF, OM, VM, OF, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, MF, MF, HH, HH, HH, NG, NG, HH, HH,
        OF, HH, HH, HH, HH, HH, VM, VM, VM, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, NG, HH, HH, HH, MM, MM, VM, HH, VM, VM, HH, HH, NG, OM, HH, HH, OF, NG,
        NG, NG, NG, NG, NG, HH, NG, NG, OF, NG, NG, NG, NG, NG, NG, VM, VM, VM, VM, VM, VM, VM, VM, VM, HH, HH, HH, HH, HH, HH, HH, VM, VM, VM, VM, VM, VM, HH, HH, HH,
        HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, MM, MM, MM, HH, HH, HH, OM, OM, HH, HH, HH, HH, HH, HH, HH, HH, OF, OF, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH,
        HH, HH, HH, HH, VM, VM, VM, VM, HH, HH, VM, VM, MF, MF, MF, MF, MF, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, NG,
        // 600
        NG, NG, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, NG, HH, HH, HH, HH, HH, HH, NG, NG, HH, HH, HH, OM, OM, OF, OF, HH, HH, HH, HH, HH, HH, HH, NG, NG,
        NG, OM, OM, NG, NG, OM, NG, NG, NG, NG, VM, VM, VM, VM, VM, VM, VM, VM, VM, HH, HH, HH, HH, HH, HH, HH, HH, VF, VF, OF, OF, OF, HH, HH, HH, HH, HH, HH, OM, HH,
        HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, VM, VM, VM, VM, VM, HH, HH, NG, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, NG, NG, NG, NG,
        NG, NG, VM, VM, VM, VM, VM, VM, VM, VM, VM, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, MF, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, VM, OF, HH,
        HH, OF, OF, OF, MF, HH, HH, HH, HH, HH, HH, HH, NG, NG, NG, HH, HH, HH, HH, HH, HH, NG, HH, HH, HH, NG, NG, NG, NG, NG, NG, NG, NG, NG, NG, NG, NG, NG, NG, NG,
        // 800
        NG, NG, NG, NG, NG, NG, NG, NG, NG, NG, VM, VM, VM, VM, VM, VM, VM, VM, VM, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH,
        HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, NG, NG, OF, OF, OF, OM, OM, OM, HH, HH, MF, HH, HH, HH, OF, OF, NG, HH, HH, HH, HH, HH, OM, HH, HH, HH,
        NG, NG, NG, NG, HH, HH, HH, HH, NG, NG, NG, VM, VM, NG, NG, NG, NG, NG, NG, HH, HH, HH, OM, HH, HH, OF, VM, VM, VM, VM, VM, VM, VM, VM, VM, HH, OM, HH, HH, HH,
        HH, HH, HH, HH, NG, NG, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, OF, OF, OF,
        HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, HH, NG, NG, NG, NG, NG, NG, NG, NG, NG, NG, NG, NG, HH, HH, HH, NG,
        // 1000
        NG, NG, NG, NG, NG, NG, NG, NG, NG, NG, NG, HH, NG, NG, OM, OM, OM, OF, HH, HH, NG, NG, NG, NG, NG,
    ];

    // 256/100% gender ratios

    /// <summary> Only Male: 0x00 </summary>
    private const byte OM = 000; // only male
    /// <summary> 7:1 Male: 0x1F </summary>
    public const byte VM = 031; // very male
    /// <summary> 3:1 Male: 0x3F </summary>
    public const byte MM = 063; // mostly male
    /// <summary> 1:1 50%: 0x7F </summary>
    public const byte HH = 127; // half & half
    /// <summary> 3:1 Female: 0xBF </summary>
    public const byte MF = 191; // mostly female
    /// <summary> 7:1 Female: 0xDF </summary>
    public const byte VF = 225; // very female
    /// <summary> Only Female: 0xFE </summary>
    private const byte OF = 254; // only female
    /// <summary> Genderless 0xFF </summary>
    private const byte NG = 255; // no gender
}
