using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace PKHeX.Core;

/// <summary>
/// Configuration for displaying stats.
/// </summary>
[TypeConverter(typeof(ExpandableObjectConverter))]
public sealed class StatDisplayConfig
{
    /// <summary>
    /// Stat names are displayed without localization; H:X A:X B:X C:X D:X S:X
    /// </summary>
    public static readonly StatDisplayConfig HABCDS = new()
    {
        Names = ["H", "A", "B", "C", "D", "S"],
        Separator = " ",
        ValueGap = ":",
        IsLeft = true,
        AlwaysShow = true,
    };

    /// <summary>
    /// Stat names are displayed without localization; X/X/X/X/X/X
    /// </summary>
    /// <remarks>
    /// Same as <see cref="Raw00"/> but with no leading zeroes.
    /// </remarks>
    public static readonly StatDisplayConfig Raw = new()
    {
        Names = [],
        Separator = "/",
        ValueGap = string.Empty,
        AlwaysShow = true,
    };

    /// <summary>
    /// Stat names are displayed without localization; XX/XX/XX/XX/XX/XX
    /// </summary>
    /// <remarks>
    /// Same as <see cref="Raw"/> but with 2 digits (leading zeroes).
    /// </remarks>
    public static readonly StatDisplayConfig Raw00 = new()
    {
        Names = [],
        Separator = "/",
        ValueGap = string.Empty,
        AlwaysShow = true,
        MinimumDigits = 2,
    };

    /// <summary>
    /// List of stat display styles that are commonly used and not specific to a localization.
    /// </summary>
    public static List<StatDisplayConfig> Custom { get; } = [HABCDS, Raw]; // Raw00 parses equivalent to Raw

    /// <summary>List of stat names to display</summary>
    public required string[] Names { get; init; }

    /// <summary>Separator between each stat+value declaration</summary>
    public string Separator { get; init; } = " / ";

    /// <summary>Separator between the stat name and value</summary>
    public string ValueGap { get; init; } = " ";

    /// <summary><see langword="true"/> if the text is displayed on the left side of the value</summary>
    public bool IsLeft { get; init; }

    /// <summary><see langword="true"/> if the stat is always shown, even if the value is default</summary>
    public bool AlwaysShow { get; init; }

    /// <summary>Minimum number of digits to show for the stat value.</summary>
    public int MinimumDigits { get; init; }

    /// <summary>
    /// Gets the index of the displayed stat name (in visual order) via a case-insensitive search.
    /// </summary>
    /// <param name="stat">Stat name, trimmed.</param>
    /// <returns>-1 if not found, otherwise the index of the stat name.</returns>
    public int GetStatIndex(ReadOnlySpan<char> stat)
    {
        for (int i = 0; i < Names.Length; i++)
        {
            if (stat.Equals(Names[i], StringComparison.OrdinalIgnoreCase))
                return i;
        }
        return -1;
    }

    public override string ToString() => string.Join(Separator, Names);

    /// <summary>
    /// Formats a stat value into a string builder.
    /// </summary>
    /// <param name="sb">Result string builder</param>
    /// <param name="statIndex">Display index of the stat</param>
    /// <param name="statValue">Stat value</param>
    /// <param name="valueSuffix">Optional suffix for the value, to display a stat amplification request</param>
    /// <param name="skipValue"><see langword="true"/> to skip the value, only displaying the stat name and amplification (if provided)</param>
    public void Format<T>(StringBuilder sb, int statIndex, T statValue, ReadOnlySpan<char> valueSuffix = default, bool skipValue = false)
    {
        var statName = statIndex < Names.Length ? Names[statIndex] : "";
        var length = GetStatSize(statName, statValue, valueSuffix, skipValue);

        if (sb.Length + length > sb.Capacity)
            sb.EnsureCapacity(sb.Length + length);
        Append(sb, statName, statValue, valueSuffix, skipValue);
    }

    private void Append<T>(StringBuilder sb, ReadOnlySpan<char> statName, T statValue, ReadOnlySpan<char> valueSuffix, bool skipValue)
    {
        int start = sb.Length;

        if (!skipValue)
        {
            sb.Append(statValue);
            var length = sb.Length - start;
            if (length < MinimumDigits)
                sb.Insert(start, "0", MinimumDigits - length);
        }
        sb.Append(valueSuffix);

        if (IsLeft)
        {
            sb.Insert(start, ValueGap);
            sb.Insert(start, statName);
        }
        else
        {
            sb.Append(ValueGap);
            sb.Append(statName);
        }
    }

    private int GetStatSize<T>(ReadOnlySpan<char> statName, T statValue, ReadOnlySpan<char> valueSuffix, bool skipValue)
    {
        var length = statName.Length + ValueGap.Length + valueSuffix.Length;
        if (!skipValue)
            length += (int)Math.Max(MinimumDigits, Math.Floor(Math.Log10(Convert.ToDouble(statValue)) + 1));
        return length;
    }

    /// <summary>
    /// Gets the separator character used for parsing.
    /// </summary>
    private char GetSeparatorParse() => GetSeparatorParse(Separator);

    private static char GetSeparatorParse(ReadOnlySpan<char> sep) => sep.Length switch
    {
        0 => ' ',
        1 => sep[0],
        _ => sep.Trim()[0]
    };

    /// <summary>
    /// Imports a list of stats from a string.
    /// </summary>
    /// <param name="message">Input string</param>
    /// <param name="result">Result storage</param>
    /// <returns>Parse result</returns>
    public StatParseResult TryParse(ReadOnlySpan<char> message, Span<int> result)
    {
        var separator = GetSeparatorParse();
        var gap = ValueGap.AsSpan().Trim();
        // If stats are not labeled, parse with the straightforward parser.
        if (Names.Length == 0)
            return TryParseRaw(message, result, separator);
        else if (IsLeft)
            return TryParseIsLeft(message, result, separator, gap);
        else
            return TryParseRight(message, result, separator, gap);
    }

    private StatParseResult TryParseIsLeft(ReadOnlySpan<char> message, Span<int> result, char separator, ReadOnlySpan<char> valueGap)
    {
        var rec = new StatParseResult();

        for (int i = 0; i < Names.Length; i++)
        {
            if (message.Length == 0)
                break;

            var statName = Names[i];
            var index = message.IndexOf(statName, StringComparison.OrdinalIgnoreCase);
            if (index == -1)
                continue;

            if (index != 0)
                rec.MarkDirty(); // We have something before our stat name, so it isn't clean.

            message = message[statName.Length..].TrimStart();
            if (valueGap.Length > 0 && message.StartsWith(valueGap))
                message = message[valueGap.Length..].TrimStart();

            var value = message;

            var indexSeparator = value.IndexOf(separator);
            if (indexSeparator != -1)
                value = value[..indexSeparator].Trim();
            else
                message = default; // everything remaining belongs in the value we are going to parse.

            if (value.Length != 0)
            {
                var amped = TryPeekAmp(ref value, ref rec, i);
                if (amped && value.Length == 0)
                    rec.MarkParsed(index);
                else
                    TryParse(result, ref rec, value, i);
            }

            if (indexSeparator != -1)
                message = message[(indexSeparator+1)..].TrimStart();
            else
                break;
        }

        if (!message.IsWhiteSpace()) // shouldn't be anything left in the message to parse
            rec.MarkDirty();
        rec.FinishParse(Names.Length);
        return rec;
    }

    private StatParseResult TryParseRight(ReadOnlySpan<char> message, Span<int> result, char separator, ReadOnlySpan<char> valueGap)
    {
        var rec = new StatParseResult();

        for (int i = 0; i < Names.Length; i++)
        {
            if (message.Length == 0)
                break;

            var statName = Names[i];
            var index = message.IndexOf(statName, StringComparison.OrdinalIgnoreCase);
            if (index == -1)
                continue;

            var value = message[..index].Trim();
            var indexSeparator = value.LastIndexOf(separator);
            if (indexSeparator != -1)
            {
                rec.MarkDirty(); // We have something before our stat name, so it isn't clean.
                value = value[(indexSeparator + 1)..].TrimStart();
            }

            if (valueGap.Length > 0 && value.EndsWith(valueGap))
                value = value[..^valueGap.Length];

            if (value.Length != 0)
            {
                var amped = TryPeekAmp(ref value, ref rec, i);
                if (amped && value.Length == 0)
                    rec.MarkParsed(index);
                else
                    TryParse(result, ref rec, value, i);
            }

            message = message[(index + statName.Length)..].TrimStart();
            if (message.StartsWith(separator))
                message = message[1..].TrimStart();
        }

        if (!message.IsWhiteSpace()) // shouldn't be anything left in the message to parse
            rec.MarkDirty();
        rec.FinishParse(Names.Length);
        return rec;
    }

    /// <summary>
    /// Parses a raw stat string.
    /// </summary>
    /// <param name="message">Input string</param>
    /// <param name="result">Output storage</param>
    /// <param name="separator">Separator character</param>
    public static StatParseResult TryParseRaw(ReadOnlySpan<char> message, Span<int> result, char separator)
    {
        var rec = new StatParseResult();

        // Expect the message to contain all entries of `result` separated by the separator and an arbitrary amount of spaces permitted.
        // The message is split by the separator, and each part is trimmed of whitespace.
        for (int i = 0; i < result.Length; i++)
        {
            var index = message.IndexOf(separator);
            ReadOnlySpan<char> value;
            if (index != -1)
            {
                value = message[..index].TrimEnd();
                message = message[(index + 1)..].TrimStart();
            }
            else // no further iterations to be done
            {
                value = message;
                message = default;
            }

            if (value.Length == 0)
            {
                rec.MarkDirty(); // Something is wrong with the message, as we have an empty stat.
                continue; // Maybe it's a duplicate separator; keep parsing and hope that the required amount are parsed.
            }

            var amped = TryPeekAmp(ref value, ref rec, i);
            if (amped && value.Length == 0)
                rec.MarkParsed(index);
            else
                TryParse(result, ref rec, value, i);
        }

        if (!message.IsWhiteSpace()) // shouldn't be anything left in the message to parse
            rec.MarkDirty();
        rec.FinishParseOnly(result.Length);
        return rec;
    }

    private static void TryParse(Span<int> result, ref StatParseResult rec, ReadOnlySpan<char> value, int statIndex)
    {
        if (!int.TryParse(value, out var stat) || stat < 0)
        {
            rec.MarkDirty();
            return;
        }
        result[statIndex] = stat;
        rec.MarkParsed(statIndex);
    }

    private static bool TryPeekAmp(ref ReadOnlySpan<char> value, ref StatParseResult rec, int statIndex)
    {
        var last = value[^1];
        if (last == '+')
        {
            rec.Plus = (sbyte)statIndex;
            value = value[..^1].TrimEnd();
            return true;
        }
        if (last == '-')
        {
            rec.Minus = (sbyte)statIndex;
            value = value[..^1].TrimEnd();
            return true;
        }
        return false;
    }
}
