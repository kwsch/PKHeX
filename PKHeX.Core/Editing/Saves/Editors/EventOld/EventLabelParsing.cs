using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace PKHeX.Core;

/// <summary>
/// Logic for parsing labeled scripted event schemas into labeled tags.
/// </summary>
public static class EventLabelParsing
{
    private const char Split = '\t';

    private static readonly NamedEventConst Custom = new("Custom", NamedEventConst.CustomMagicValue);
    private static readonly NamedEventConst[] Empty = [Custom];

    public static List<NamedEventValue> GetFlags(ReadOnlySpan<string> strings, int maxValue = int.MaxValue)
    {
        var result = new List<NamedEventValue>(strings.Length);
        var processed = new HashSet<int>(strings.Length);
        foreach (var line in strings)
        {
            if (!TryParseValue(line, out var item))
                continue;

            SanityCheck(item, processed, maxValue);
            result.Add(item);
        }
        return result;
    }

    public static List<NamedEventWork> GetWork(ReadOnlySpan<string> strings, int maxValue = int.MaxValue)
    {
        var result = new List<NamedEventWork>(strings.Length);
        var processed = new HashSet<int>(strings.Length);
        foreach (var line in strings)
        {
            if (!TryParseWork(line, out var item))
                continue;

            SanityCheck(item, processed, maxValue);
            result.Add(item);
        }
        return result;
    }

    private static void SanityCheck(NamedEventValue item, ISet<int> processed, int maxValue) => SanityCheck(processed, maxValue, item.Index);

    private static void SanityCheck(ISet<int> processed, int maxValue, int index)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, maxValue);
        if (!processed.Add(index))
            throw new ArgumentOutOfRangeException(nameof(index), index, "Already have an entry for this!");
    }

    public static bool TryParseValue(ReadOnlySpan<char> value, [NotNullWhen(true)] out NamedEventValue? result)
    {
        result = null;
        var split = value.IndexOf(Split);
        if (split < 0)
            return false;

        var number = value[..split];
        var index = TryParseHexDec(number, "0x");

        value = value[(split + 1)..];
        split = value.IndexOf(Split);
        if (split < 0)
            return false;

        var category = value[..split];
        var type = NamedEventTypeUtil.GetEventType(category);

        value = value[(split + 1)..];
        var desc = value.ToString();
        result = new NamedEventValue(desc, index, type);
        return true;
    }

    public static bool TryParseWork(ReadOnlySpan<char> value, [NotNullWhen(true)] out NamedEventWork? item)
    {
        item = null;
        var split = value.IndexOf(Split);
        if (split < 0)
            return false;

        var number = value[..split];
        var index = TryParseHexDec(number, "0x4");

        value = value[(split + 1)..];
        split = value.IndexOf(Split);
        if (split < 0)
            return false;

        var category = value[..split];
        var type = NamedEventTypeUtil.GetEventType(category);

        value = value[(split + 1)..];
        var (desc, predefined) = GetDescPredefined(value);
        item = new NamedEventWork(desc, index, type, predefined);
        return true;
    }

    private static (string Description, IReadOnlyList<NamedEventConst> Named) GetDescPredefined(ReadOnlySpan<char> remainder)
    {
        var split3 = remainder.IndexOf(Split);
        if (split3 < 0)
            return (remainder.ToString(), Empty);

        var values = remainder[(split3 + 1)..];
        var desc = remainder[..split3].ToString();
        var predefined = GetPredefinedArray(values);
        return (desc, predefined);
    }

    private static List<NamedEventConst> GetPredefinedArray(ReadOnlySpan<char> combined)
    {
        List<NamedEventConst> result = [..Empty];

        // x:y tuples separated by ,
        while (true)
        {
            // grab the next tuple span
            var next = combined.IndexOf(',');
            var tuple = next < 0 ? combined : combined[..next];
            var split = tuple.IndexOf(':');
            if (split < 0)
                break;

            // parse the tuple into the name and value object
            var value = tuple[..split];
            var name = tuple[(split + 1)..];
            var val = TryParseHexDec(value, "0x");
            var item = new NamedEventConst(name.ToString(), (ushort)val);
            result.Add(item);

            if (next < 0)
                break; // no more tuples

            // advance the span to next potential tuple
            combined = combined[(next + 1)..];
        }
        return result;
    }

    private static int TryParseHexDec(ReadOnlySpan<char> text, ReadOnlySpan<char> hexPrefix)
    {
        if (!text.StartsWith(hexPrefix))
            return int.TryParse(text, out var value) ? value : 0;
        text = text[hexPrefix.Length..];
        return int.TryParse(text, NumberStyles.HexNumber, null, out var hex) ? hex : 0;
    }
}
