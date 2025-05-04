using System;

namespace PKHeX.Core;

/// <summary>
/// Value result object of parsing a stat string.
/// </summary>
public record struct StatParseResult()
{
    private const uint MaxStatCount = 6; // Number of stats in the game
    public const sbyte NoStatAmp = -1;

    /// <summary>
    /// Count of parsed stats.
    /// </summary>
    public byte CountParsed { get; private set; } = 0; // could potentially make this a computed value (popcnt), but it's not worth it

    /// <summary>
    /// Bitflag indexes of parsed stats, indexed in visual order.
    /// </summary>
    public byte IndexesParsed { get; private set; } = 0;

    /// <summary>
    /// Stat index of increased stat, indexed in visual order.
    /// </summary>
    public sbyte Plus { get; set; } = NoStatAmp;

    /// <summary>
    /// Stat index of decreased stat, indexed in visual order.
    /// </summary>
    public sbyte Minus { get; set; } = NoStatAmp;

    /// <summary>
    /// Indicates if the parsing was clean (no un-parsed text).
    /// </summary>
    public bool IsParseClean { get; private set; } = true;

    /// <summary>
    /// Indicates if all stat indexes available were parsed.
    /// </summary>
    public bool IsParsedAllStats { get; private set; } = false;

    /// <summary>
    /// Marks the stat index as parsed, and updates the count of parsed stats.
    /// </summary>
    /// <param name="statIndex">Visual index of the stat to mark as parsed.</param>
    /// <returns>True if the stat had not been parsed before, false if it was already parsed.</returns>
    public bool MarkParsed(int statIndex)
    {
        // Check if the stat index is valid (0-5)
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual((uint)statIndex, MaxStatCount);
        if (WasParsed(statIndex))
            return false;
        // Mark the stat index as parsed
        IndexesParsed |= (byte)(1 << statIndex);
        ++CountParsed;
        return true;
    }

    /// <summary>
    /// Checks if the stat index was parsed.
    /// </summary>
    /// <param name="statIndex">Visual index of the stat to check.</param>
    /// <returns>True if the stat was parsed, false otherwise.</returns>
    public bool WasParsed(int statIndex)
    {
        // Check if the stat index is valid (0-5)
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual((uint)statIndex, MaxStatCount);
        return (IndexesParsed & (1 << statIndex)) != 0;
    }

    /// <summary>
    /// Marks the parsing as finished, and updates the internal state to indicate if all stats were parsed.
    /// </summary>
    /// <remarks>
    /// This is used when not all stats are required to be parsed.
    /// </remarks>
    /// <param name="expect"></param>
    public void FinishParse(int expect)
    {
        if (CountParsed == 0 && !HasAmps)
            MarkDirty();
        IsParsedAllStats = CountParsed == expect || IsParseClean;
    }

    /// <summary>
    /// Marks the parsing as finished, and updates the internal state to indicate if all stats were parsed.
    /// </summary>
    /// <remarks>
    /// This is used when a specific number of stats is expected.
    /// </remarks>
    /// <param name="expect"></param>
    public void FinishParseOnly(int expect) => IsParsedAllStats = CountParsed == expect;

    /// <summary>
    /// Marks the parsing as dirty, indicating that the string was not a clean input string (user modified or the syntax doesn't match the spec).
    /// </summary>
    public void MarkDirty() => IsParseClean = false;

    /// <summary>
    /// Indicates if any stat has any amplified (+/-) requested, indicative of nature.
    /// </summary>
    public bool HasAmps => Plus != NoStatAmp || Minus != NoStatAmp;

    /// <summary>
    /// Reorders the speed stat to be in the middle of the stats.
    /// </summary>
    /// <remarks>
    /// Speed is visually represented as the last stat in the list, but it is actually the 3rd stat stored.
    /// </remarks>
    public void TreatAmpsAsSpeedNotLast()
    {
        Plus = GetSpeedMiddleIndex(Plus);
        Minus = GetSpeedMiddleIndex(Minus);
    }

    /// <summary>
    /// Adjusts stat indexes from visual to stored, and ignoring HP's index.
    /// </summary>
    /// <param name="amp">Visual index of the stat to get the adjusted value for.</param>
    /// <returns>Stored index of the stat.</returns>
    private static sbyte GetSpeedMiddleIndex(sbyte amp) => amp switch
    {
        // 0 => NoStatAmp -- handle via default case
        1 => 0, // Atk
        2 => 1, // Def
        3 => 3, // SpA
        4 => 4, // SpD
        5 => 2, // Spe
        _ => NoStatAmp,
    };
}
