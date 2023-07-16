namespace PKHeX.Core;

public interface ILevelRange
{
    byte LevelMin { get; }
    byte LevelMax { get; }
}

public static class ILevelRangeExtensions
{
    public static bool IsFixedLevel(this ILevelRange r) => r.LevelMin == r.LevelMax;
    public static bool IsRandomLevel(this ILevelRange r) => r.LevelMin != r.LevelMax;

    /// <summary>
    /// Gets if the specified level inputs are within range of the <see cref="ILevelRange.LevelMin"/> and <see cref="ILevelRange.LevelMax"/>
    /// </summary>
    /// <param name="r">Range reference</param>
    /// <param name="lvl">Single level</param>
    /// <returns>True if within slot's range, false if impossible.</returns>
    public static bool IsLevelWithinRange(this ILevelRange r, int lvl) => r.LevelMin <= lvl && lvl <= r.LevelMax;

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
    public static bool IsLevelWithinRange(this ILevelRange r, int lvl, int minDecrease, int maxIncrease) => r.LevelMin - minDecrease <= lvl && lvl <= r.LevelMax + maxIncrease;

    /// <summary>
    /// Gets if the specified level inputs are within range of the <see cref="ILevelRange.LevelMin"/> and <see cref="ILevelRange.LevelMax"/>
    /// </summary>
    /// <param name="r">Range reference</param>
    /// <param name="min">Lowest level allowed</param>
    /// <param name="max">Highest level allowed</param>
    /// <param name="minDecrease">Highest value the low end of levels can be</param>
    /// <param name="maxIncrease">Lowest value the high end of levels can be</param>
    /// <returns>True if within slot's range, false if impossible.</returns>
    public static bool IsLevelWithinRange(this ILevelRange r, byte min, byte max, int minDecrease, int maxIncrease) => r.LevelMin - minDecrease <= max && min <= r.LevelMax + maxIncrease;
}

