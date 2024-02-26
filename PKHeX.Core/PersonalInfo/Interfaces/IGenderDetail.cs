using System;

namespace PKHeX.Core;

/// <summary>
/// Exposes gender details
/// </summary>
public interface IGenderDetail
{
    /// <summary>
    /// Indicates that the entry has two genders.
    /// </summary>
    bool IsDualGender { get; }

    /// <summary>
    /// Indicates that the entry is exclusively Genderless.
    /// </summary>
    bool Genderless { get; }

    /// <summary>
    /// Indicates that the entry is exclusively Female gendered.
    /// </summary>
    bool OnlyFemale { get; }

    /// <summary>
    /// Indicates that the entry is exclusively Male gendered.
    /// </summary>
    bool OnlyMale { get; }
}

public static class GenderDetailExtensions
{
    /// <summary>
    /// Gets a random valid gender for the entry.
    /// </summary>
    public static byte RandomGender(this IGenderDetail detail)
    {
        if (detail.Genderless)
            return 2;
        if (detail.OnlyFemale)
            return 1;
        if (detail.OnlyMale)
            return 0;
        return (byte)Util.Rand.Next(2);
    }

    public static byte FixedGender(this IGenderDetail detail)
    {
        if (detail.Genderless)
            return 2;
        if (detail.OnlyFemale)
            return 1;
        if (detail.OnlyMale)
            return 0;
        throw new ArgumentOutOfRangeException(nameof(detail));
    }
}
