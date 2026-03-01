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
    extension(IGenderDetail detail)
    {
        /// <summary>
        /// Gets a random valid gender for the entry.
        /// </summary>
        public byte RandomGender()
        {
            if (detail.Genderless)
                return 2;
            if (detail.OnlyFemale)
                return 1;
            if (detail.OnlyMale)
                return 0;
            return (byte)Util.Rand.Next(2);
        }

        public byte FixedGender()
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
}
