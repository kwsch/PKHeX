using System;

namespace PKHeX.Core;

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
        var gt = PKX.GetGenderRatio(species);
        return GetFromPIDAndRatio(pid, gt);
    }

    public static byte GetFromPIDAndRatio(uint pid, byte gr) => gr switch
    {
        PersonalInfo.RatioMagicGenderless => Genderless,
        PersonalInfo.RatioMagicFemale => Female,
        PersonalInfo.RatioMagicMale => Male,
        _ => (pid & 0xFF) < gr ? Female : Male,
    };
}
