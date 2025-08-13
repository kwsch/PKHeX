namespace PKHeX.Core;

/// <summary>
/// Contains information about the level range the object originates with.
/// </summary>
public interface ILevelRange
{
    /// <summary>
    /// Minimum level.
    /// </summary>
    byte LevelMin { get; }

    /// <summary>
    /// Maximum level.
    /// </summary>
    byte LevelMax { get; }
}

public static class LevelRangeExtensions
{
    public static bool IsFixedLevel(this ILevelRange r) => r.LevelMin == r.LevelMax;
    public static bool IsRandomLevel(this ILevelRange r) => r.LevelMin != r.LevelMax;

    /// <summary>
    /// Gets if the specified level inputs are within range of the <see cref="ILevelRange.LevelMin"/> and <see cref="ILevelRange.LevelMax"/>
    /// </summary>
    /// <param name="r">Range reference</param>
    /// <param name="lvl">Single level</param>
    /// <returns>True if within slot's range, false if impossible.</returns>
    public static bool IsLevelWithinRange(this ILevelRange r, byte lvl) => r.LevelMin <= lvl && lvl <= r.LevelMax;

    /// <inheritdoc cref="IsLevelWithinRange(ILevelRange,byte)"/>
    public static bool IsLevelWithinRange(byte level, byte min, byte max) => min <= level && level <= max;

    /// <inheritdoc cref="IsLevelWithinRange(ILevelRange,byte)"/>
    public static bool IsLevelWithinRange<T>(this ILevelRange r, T other) where T : ILevelRange => IsLevelWithinRange(r, other.LevelMin, other.LevelMax);

    /// <summary>
    /// Gets if the specified level inputs are within range of the <see cref="ILevelRange.LevelMin"/> and <see cref="ILevelRange.LevelMax"/>
    /// </summary>
    /// <param name="r">Range reference</param>
    /// <param name="min">Highest value the low end of levels can be</param>
    /// <param name="max">Lowest value the high end of levels can be</param>
    /// <returns>True if within slot's range, false if impossible.</returns>
    public static bool IsLevelWithinRange(this ILevelRange r, byte min, byte max) => r.LevelMin <= max && min <= r.LevelMax;

    /// <summary>
    /// Gets if the specified level inputs are within range of the <see cref="ILevelRange.LevelMin"/> and <see cref="ILevelRange.LevelMax"/>
    /// </summary>
    /// <param name="r">Range reference</param>
    /// <param name="lvl">Single level</param>
    /// <param name="minDecrease">Highest value the low end of levels can be</param>
    /// <param name="maxIncrease">Lowest value the high end of levels can be</param>
    /// <returns>True if within slot's range, false if impossible.</returns>
    public static bool IsLevelWithinRange(this ILevelRange r, byte lvl, byte minDecrease, byte maxIncrease) => r.LevelMin - minDecrease <= lvl && lvl <= r.LevelMax + maxIncrease;

    /// <summary>
    /// Gets if the specified level inputs are within range of the <see cref="ILevelRange.LevelMin"/> and <see cref="ILevelRange.LevelMax"/>
    /// </summary>
    /// <param name="r">Range reference</param>
    /// <param name="min">Lowest level allowed</param>
    /// <param name="max">Highest level allowed</param>
    /// <param name="minDecrease">Highest value the low end of levels can be</param>
    /// <param name="maxIncrease">Lowest value the high end of levels can be</param>
    /// <returns>True if within slot's range, false if impossible.</returns>
    public static bool IsLevelWithinRange(this ILevelRange r, byte min, byte max, int minDecrease, byte maxIncrease) => r.LevelMin - minDecrease <= max && min <= r.LevelMax + maxIncrease;
}
