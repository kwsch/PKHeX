namespace PKHeX.Core;

/// <summary>
/// Contains information about the initial gender of the encounter.
/// </summary>
public interface IFixedGender
{
    /// <summary>
    /// Magic gender index for the encounter.
    /// </summary>
    sbyte Gender { get; }

    /// <summary>
    /// Indicates if the gender is a single value (not random).
    /// </summary>
    bool IsFixedGender => Gender != -1;
}
