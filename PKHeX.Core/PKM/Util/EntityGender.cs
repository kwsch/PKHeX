namespace PKHeX.Core;

public static class EntityGender
{
    /// <summary>
    /// Translates a Gender string to Gender integer.
    /// </summary>
    /// <param name="s">Gender string</param>
    /// <returns>Gender integer</returns>
    public static int GetFromString(string s)
    {
        if (s.Length != 1)
            return 2;
        return GetFromChar(s[0]);
    }

    /// <summary>
    /// Translates a Gender char to Gender integer.
    /// </summary>
    public static int GetFromChar(char c) => c switch
    {
        '♂' or 'M' => 0,
        '♀' or 'F' => 1,
        _ => 2,
    };

    /// <summary>
    /// Gets the gender ID of the species based on the Personality ID.
    /// </summary>
    /// <param name="species">National Dex ID.</param>
    /// <param name="pid">Personality ID.</param>
    /// <returns>Gender ID (0/1/2)</returns>
    /// <remarks>This method should only be used for Generations 3-5 origin.</remarks>
    public static int GetFromPID(ushort species, uint pid)
    {
        int gt = PKX.Personal[species].Gender;
        return GetFromPIDAndRatio(pid, gt);
    }

    public static int GetFromPIDAndRatio(uint pid, int gr) => gr switch
    {
        PersonalInfo.RatioMagicGenderless => 2,
        PersonalInfo.RatioMagicFemale => 1,
        PersonalInfo.RatioMagicMale => 0,
        _ => (pid & 0xFF) < gr ? 1 : 0,
    };
}
