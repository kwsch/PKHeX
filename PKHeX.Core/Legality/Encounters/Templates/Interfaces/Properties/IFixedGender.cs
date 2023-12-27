namespace PKHeX.Core;

/// <summary>
/// Contains information about the initial gender of the encounter.
/// </summary>
public interface IFixedGender
{
    /// <summary>
    /// Magic gender index for the encounter.
    /// </summary>
    byte Gender { get; }

    /// <summary>
    /// Indicates if the gender is a single value (not random).
    /// </summary>
    bool IsFixedGender => Gender != FixedGenderUtil.GenderRandom;
}

public static class FixedGenderUtil
{
    public const byte GenderRandom = 0xFF;
}
