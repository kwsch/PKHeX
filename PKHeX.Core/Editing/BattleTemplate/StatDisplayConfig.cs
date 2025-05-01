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
    public static readonly StatDisplayConfig HABCDS = new()
    {
        Names = ["H", "A", "B", "C", "D", "S"],
        Separator = " ",
        ValueGap = ":",
        IsLeft = true,
        AlwaysShow = true,
    };

    public static readonly StatDisplayConfig Raw = new()
    {
        Names = [],
        Separator = "/",
        ValueGap = "",
        AlwaysShow = true,
    };

    public static readonly StatDisplayConfig Raw00 = new()
    {
        Names = [],
        Separator = "/",
        ValueGap = "",
        AlwaysShow = true,
        MinimumDigits = 2,
    };

    public static List<StatDisplayConfig> Custom { get; } = [HABCDS, Raw]; // Raw00 parses equivalent to Raw

    /// <summary>List of stat names to display</summary>
    public required string[] Names { get; init; }
    public string Separator { get; init; } = " / ";
    public string ValueGap { get; init; } = " ";

    /// <summary><c>true</c> if the text is displayed on the left side of the value</summary>
    public bool IsLeft { get; init; }

    /// <summary><c>true</c> if the stat is always shown, even if the value is default</summary>
    public bool AlwaysShow { get; init; }

    /// <summary>Minimum number of digits to show for the stat value.</summary>
    public int MinimumDigits { get; init; }

    public int GetStatIndex(ReadOnlySpan<char> stat)
    {
        for (int i = 0; i < Names.Length; i++)
        {
            if (stat.Equals(Names[i], StringComparison.OrdinalIgnoreCase))
                return i;
        }
        return -1;
    }

    public string Format<T>(int statIndex, T statValue, ReadOnlySpan<char> valueSuffix = default, bool skipValue = false)
    {
        var statName = statIndex < Names.Length ? Names[statIndex] : "";
        var length = GetStatSize(statName, statValue, valueSuffix, skipValue);

        var sb = new StringBuilder(length);
        Append(sb, statName, statValue, valueSuffix, skipValue);
        return sb.ToString();
    }

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
            var end = sb.Length;
            if (end < MinimumDigits)
                sb.Insert(start, "0", MinimumDigits - end);
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
    public ParseResult TryParse(ReadOnlySpan<char> message, Span<int> result)
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

    public record struct ParseResult()
    {
        public byte CountParsed { get; set; } = 0;
        public sbyte Plus { get; set; } = -1;
        public sbyte Minus { get; set; } = -1;

        public bool IsParseClean { get; private set; } = true;
        public bool IsParsedAllStats { get; private set; } = false;

        public void FinishParse(int expect)
        {
            if (CountParsed == 0 && Plus < 0 && Minus < 0)
                Dirty();
            IsParsedAllStats = CountParsed == expect || IsParseClean;
        }

        public void FinishParseOnly(int expect) => IsParsedAllStats = CountParsed == expect;
        public void Dirty() => IsParseClean = false;
        public bool HasAmps => Plus != -1 || Minus != -1;
    }

    private ParseResult TryParseIsLeft(ReadOnlySpan<char> message, Span<int> result, char separator, ReadOnlySpan<char> valueGap)
    {
        var rec = new ParseResult();

        for (int i = 0; i < Names.Length; i++)
        {
            var statName = Names[i];
            var index = message.IndexOf(statName, StringComparison.OrdinalIgnoreCase);
            if (index == -1)
                continue;

            if (index != 0)
                rec.Dirty(); // We have something before our stat name, so it isn't clean.

            message = message[statName.Length..].TrimStart();
            if (valueGap.Length > 0 && message.StartsWith(valueGap))
                message = message[valueGap.Length..].TrimStart();

            var value = message;

            var indexSeparator = value.IndexOf(separator);
            if (indexSeparator != -1)
                value = value[..indexSeparator].TrimStart();

            if (value.Length != 0)
            {
                TryPeekAmp(ref value, ref rec, i);
                TryParse(result, ref rec, value, i);
            }

            if (indexSeparator != -1)
                message = message[(indexSeparator+1)..].TrimStart();
            else
                break;
        }
        rec.FinishParse(Names.Length);
        return rec;
    }

    private ParseResult TryParseRight(ReadOnlySpan<char> message, Span<int> result, char separator, ReadOnlySpan<char> valueGap)
    {
        var rec = new ParseResult();

        for (int i = 0; i < Names.Length; i++)
        {
            var statName = Names[i];
            var index = message.IndexOf(statName, StringComparison.OrdinalIgnoreCase);
            if (index == -1)
                continue;

            var value = message[..index].Trim();
            var indexSeparator = value.LastIndexOf(separator);
            if (indexSeparator != -1)
            {
                rec.Dirty(); // We have something before our stat name, so it isn't clean.
                value = value[(indexSeparator + 1)..].TrimStart();
            }

            if (valueGap.Length > 0 && value.EndsWith(valueGap))
                value = value[..^valueGap.Length];

            if (value.Length != 0)
            {
                TryPeekAmp(ref value, ref rec, i);
                TryParse(result, ref rec, value, i);
            }

            message = message[(index + statName.Length)..].TrimStart();
            if (message.StartsWith(separator))
                message = message[1..].TrimStart();
            if (message.Length == 0)
                break;
        }

        rec.FinishParse(Names.Length);
        return rec;
    }

    // Existing TryParseRaw method remains unchanged.
    public static ParseResult TryParseRaw(ReadOnlySpan<char> message, Span<int> result, char separator)
    {
        var rec = new ParseResult();

        // Expect the message to contain all entries of `result` separated by the separator and an arbitrary amount of spaces permitted.
        // The message is split by the separator, and each part is trimmed of whitespace.
        for (int i = 0; i < result.Length; i++)
        {
            var index = message.IndexOf(separator);

            var value = index != -1 ? message[..index].Trim() : message.Trim();
            message = message[value.Length..].TrimStart();

            if (value.Length == 0)
            {
                rec.Dirty(); // Something is wrong with the message, as we have an empty stat.
                break;
            }

            TryPeekAmp(ref value, ref rec, i);
            if (!int.TryParse(value, out var stat) || stat < 0)
            {
                rec.Dirty();
                break;
            }
            result[i] = stat;
            rec.CountParsed++;
        }

        rec.FinishParseOnly(result.Length);
        return rec;
    }

    private static void TryParse(Span<int> result, ref ParseResult rec, ReadOnlySpan<char> value, int statIndex)
    {
        if (!int.TryParse(value, out var stat) || stat < 0)
        {
            rec.Dirty();
            return;
        }
        result[statIndex] = stat;
        rec.CountParsed++;
    }

    private static void TryPeekAmp(ref ReadOnlySpan<char> value, ref ParseResult rec, int statIndex)
    {
        var last = value[^1];
        if (last == '+')
        {
            rec.Plus = (sbyte)statIndex;
            value = value[..^1].TrimEnd();
        }
        else if (last == '-')
        {
            rec.Minus = (sbyte)statIndex;
            value = value[..^1].TrimEnd();
        }
    }
}
