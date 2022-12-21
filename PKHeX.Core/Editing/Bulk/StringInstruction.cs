using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace PKHeX.Core;

/// <summary>
/// Batch Editing instruction
/// </summary>
/// <remarks>
/// Can be a filter (skip), or a modification instruction (modify)
/// </remarks>
/// <see cref="Exclude"/>
/// <see cref="Require"/>
/// <see cref="Apply"/>
public sealed class StringInstruction
{
    public string PropertyName { get; }
    public string PropertyValue { get; private set; }

    /// <summary> True if ==, false if != </summary>
    public bool Evaluator { get; private init; }

    public StringInstruction(string name, string value)
    {
        PropertyName = name;
        PropertyValue = value;
    }

    public void SetScreenedValue(string[] arr)
    {
        int index = Array.IndexOf(arr, PropertyValue);
        PropertyValue = index > -1 ? index.ToString() : PropertyValue;
    }

    public static readonly IReadOnlyList<char> Prefixes = new[] { Apply, Require, Exclude };
    private const char Exclude = '!';
    private const char Require = '=';
    private const char Apply = '.';
    private const char SplitRange = ',';

    /// <summary>
    /// Character which divides a property and a value.
    /// </summary>
    /// <remarks>
    /// Example:
    /// =Species=1
    /// The second = is the split.
    /// </remarks>
    public const char SplitInstruction = '=';

    // Extra Functionality
    private int RandomMinimum, RandomMaximum;
    public bool Random { get; private set; }
    public int RandomValue => Util.Rand.Next(RandomMinimum, RandomMaximum + 1);

    public void SetRandRange(string pv)
    {
        string str = pv[1..];
        var split = str.Split(SplitRange);
        int.TryParse(split[0], out RandomMinimum);
        int.TryParse(split[1], out RandomMaximum);

        if (RandomMinimum == RandomMaximum)
        {
            PropertyValue = RandomMinimum.ToString();
            Debug.WriteLine($"{PropertyName} randomization range Min/Max same?");
        }
        else
        {
            Random = true;
        }
    }

    public static List<StringInstruction> GetFilters(ReadOnlySpan<char> text) => GetFilters(text.EnumerateLines());

    public static List<StringInstruction> GetFilters(ReadOnlySpan<string> lines)
    {
        var result = new List<StringInstruction>(lines.Length);
        foreach (var line in lines)
        {
            if (TryParseFilter(line, out var entry))
                result.Add(entry);
        }
        return result;
    }

    public static List<StringInstruction> GetFilters(SpanLineEnumerator lines)
    {
        var result = new List<StringInstruction>();
        foreach (var line in lines)
        {
            if (TryParseFilter(line, out var entry))
                result.Add(entry);
        }
        return result;
    }

    public static List<StringInstruction> GetFilters(IReadOnlyList<string> lines)
    {
        var result = new List<StringInstruction>(lines.Count);
        foreach (var line in lines)
        {
            if (TryParseFilter(line, out var entry))
                result.Add(entry);
        }
        return result;
    }

    public static List<StringInstruction> GetFilters(IEnumerable<string> lines)
    {
        var result = new List<StringInstruction>();
        foreach (var line in lines)
        {
            if (TryParseFilter(line, out var entry))
                result.Add(entry);
        }
        return result;
    }
    
    public static List<StringInstruction> GetInstructions(ReadOnlySpan<char> text) => GetInstructions(text.EnumerateLines());

    public static List<StringInstruction> GetInstructions(ReadOnlySpan<string> lines)
    {
        var result = new List<StringInstruction>(lines.Length);
        foreach (var line in lines)
        {
            if (TryParseInstruction(line, out var entry))
                result.Add(entry);
        }
        return result;
    }

    public static List<StringInstruction> GetInstructions(SpanLineEnumerator lines)
    {
        var result = new List<StringInstruction>();
        foreach (var line in lines)
        {
            if (TryParseInstruction(line, out var entry))
                result.Add(entry);
        }
        return result;
    }

    public static List<StringInstruction> GetInstructions(IReadOnlyList<string> lines)
    {
        var result = new List<StringInstruction>(lines.Count);
        foreach (var line in lines)
        {
            if (TryParseInstruction(line, out var entry))
                result.Add(entry);
        }
        return result;
    }

    public static List<StringInstruction> GetInstructions(IEnumerable<string> lines)
    {
        var result = new List<StringInstruction>();
        foreach (var line in lines)
        {
            if (TryParseInstruction(line, out var entry))
                result.Add(entry);
        }
        return result;
    }

    private static bool TryParseFilter(ReadOnlySpan<char> line, [NotNullWhen(true)] out StringInstruction? entry)
    {
        entry = null;
        if (line.Length is 0 || line[0] is not (Exclude or Require))
            return false;
        return TryParseSplitTuple(line[1..], ref entry, line[0] == Require);
    }

    private static bool TryParseInstruction(ReadOnlySpan<char> line, [NotNullWhen(true)] out StringInstruction? entry)
    {
        entry = null;
        if (line.Length is 0 || line[0] is not Apply)
            return false;
        return TryParseSplitTuple(line[1..], ref entry);
    }

    private static bool TryParseSplitTuple(ReadOnlySpan<char> tuple, [NotNullWhen(true)] ref StringInstruction? entry, bool eval = default)
    {
        var splitIndex = tuple.IndexOf(SplitInstruction);
        if (splitIndex <= 0)
            return false;

        var name = tuple[..splitIndex];
        if (name.IsWhiteSpace())
            return false;

        var value = tuple[(splitIndex + 1)..];
        var noExtra = value.IndexOf(SplitInstruction);
        if (noExtra != -1)
            return false;

        entry = new StringInstruction(name.ToString(), value.ToString()) { Evaluator = eval };
        return true;
    }
}
