namespace PKHeX.Core;

/// <summary>
/// Nature ID values for the corresponding English nature name.
/// </summary>
public enum Nature : byte
{
    Hardy = 0,
    Lonely = 1,
    Brave = 2,
    Adamant = 3,
    Naughty = 4,
    Bold = 5,
    Docile = 6,
    Relaxed = 7,
    Impish = 8,
    Lax = 9,
    Timid = 10,
    Hasty = 11,
    Serious = 12,
    Jolly = 13,
    Naive = 14,
    Modest = 15,
    Mild = 16,
    Quiet = 17,
    Bashful = 18,
    Rash = 19,
    Calm = 20,
    Gentle = 21,
    Sassy = 22,
    Careful = 23,
    Quirky = 24,

    Random = 25,
}

/// <summary>
/// Extension methods for <see cref="Nature"/>.
/// </summary>
public static class NatureUtil
{
    /// <summary>
    /// Gets the <see cref="Nature"/> value that corresponds to the provided <see cref="value"/>.
    /// </summary>
    /// <remarks>Actual nature values will be unchanged; only out-of-bounds values re-map to <see cref="Nature.Random"/>.</remarks>
    public static Nature GetNature(Nature value) => value switch
    {
        >= Nature.Random => Nature.Random,
        _ => value,
    };

    /// <summary>
    /// Checks if the provided <see cref="value"/> is a valid stored <see cref="Nature"/> value.
    /// </summary>
    /// <returns>True if value is an actual nature.</returns>
    public static bool IsFixed(this Nature value) => value < Nature.Random;

    /// <summary>
    /// Checks if the provided <see cref="value"/> is a possible mint nature.
    /// </summary>
    public static bool IsMint(this Nature value) => (value.IsFixed() && (byte)value % 6 != 0) || value == Nature.Serious;

    /// <summary>
    /// Checks if the provided <see cref="value"/> is a neutral nature which has no stat amps applied.
    /// </summary>
    public static bool IsNeutral(this Nature value) => value.IsFixed() && (byte)value % 6 == 0;
}
