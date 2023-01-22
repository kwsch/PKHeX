using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using static PKHeX.Core.InstructionComparer;

namespace PKHeX.Core;

/// <summary>
/// Batch Editing instruction
/// </summary>
/// <remarks>
/// Can be a filter (skip), or a modification instruction (modify)
/// </remarks>
/// <see cref="FilterNotEqual"/>
/// <see cref="FilterEqual"/>
/// <see cref="Apply"/>
public sealed class StringInstruction
{
    /// <summary> Property to modify. </summary>
    public string PropertyName { get; }
    /// <summary> Value to set to the property. </summary>
    public string PropertyValue { get; private set; }
    /// <summary> Filter Comparison Type </summary>
    public InstructionComparer Comparer { get; private init; }

    public StringInstruction(string name, string value)
    {
        PropertyName = name;
        PropertyValue = value;
    }

    public void SetScreenedValue(ReadOnlySpan<string> arr)
    {
        int index = arr.IndexOf(PropertyValue);
        PropertyValue = (uint)index >= arr.Length ? index.ToString() : PropertyValue;
    }

    public static readonly IReadOnlyList<char> Prefixes = new[] { Apply, FilterEqual, FilterNotEqual, FilterGreaterThan, FilterGreaterThanOrEqual, FilterLessThan, FilterLessThanOrEqual };
    private const char Apply = '.';
    private const char SplitRange = ',';

    private const char FilterEqual = '=';
    private const char FilterNotEqual = '!';
    private const char FilterGreaterThan = '>';
    private const char FilterLessThan = '<';
    private const char FilterGreaterThanOrEqual = '≥';
    private const char FilterLessThanOrEqual = '≤';

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

    /// <summary>
    /// Checks if the input <see cref="str"/> is a valid "random range" specification.
    /// </summary>
    public static bool IsRandomRange(ReadOnlySpan<char> str)
    {
        // Need at least one character on either side of the splitter char.
        int index = str.IndexOf(SplitRange);
        return index > 0 && index < str.Length - 1;
    }

    /// <summary>
    /// Sets a "random range" specification to the instruction.
    /// </summary>
    /// <exception cref="ArgumentException">When the splitter is not present.</exception>
    public void SetRandomRange(ReadOnlySpan<char> str)
    {
        var index = str.IndexOf(SplitRange);
        if (index <= 0)
            throw new ArgumentException($"Invalid Random Range: {str.ToString()}", nameof(str));

        var min = str[..index];
        var max = str[(index + 1)..];
        _ = int.TryParse(min, out RandomMinimum);
        _ = int.TryParse(max, out RandomMaximum);

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

    public static bool TryParseFilter(ReadOnlySpan<char> line, [NotNullWhen(true)] out StringInstruction? entry)
    {
        entry = null;
        if (line.Length is 0)
            return false;
        var comparer = GetComparer(line[0]);
        if (!comparer.IsSupportedComparer())
            return false;
        return TryParseSplitTuple(line[1..], ref entry, comparer);
    }

    public static bool TryParseInstruction(ReadOnlySpan<char> line, [NotNullWhen(true)] out StringInstruction? entry)
    {
        entry = null;
        if (line.Length is 0 || line[0] is not Apply)
            return false;
        return TryParseSplitTuple(line[1..], ref entry);
    }

    public static bool TryParseSplitTuple(ReadOnlySpan<char> tuple, [NotNullWhen(true)] ref StringInstruction? entry, InstructionComparer eval = default)
    {
        if (!TryParseSplitTuple(tuple, out var name, out var value))
            return false;
        entry = new StringInstruction(name.ToString(), value.ToString()) { Comparer = eval };
        return true;
    }

    public static bool TryParseSplitTuple(ReadOnlySpan<char> tuple, out ReadOnlySpan<char> name, out ReadOnlySpan<char> value)
    {
        name = default;
        value = default;
        var splitIndex = tuple.IndexOf(SplitInstruction);
        if (splitIndex <= 0)
            return false;

        name = tuple[..splitIndex];
        if (name.IsWhiteSpace())
            return false;

        value = tuple[(splitIndex + 1)..];
        var noExtra = value.IndexOf(SplitInstruction);
        if (noExtra != -1)
            return false;

        return true;
    }

    public static InstructionComparer GetComparer(char c) => c switch
    {
        FilterEqual => IsEqual,
        FilterNotEqual => IsNotEqual,
        FilterGreaterThan => IsGreaterThan,
        FilterLessThan => IsLessThan,
        FilterGreaterThanOrEqual => IsGreaterThanOrEqual,
        FilterLessThanOrEqual => IsLessThanOrEqual,
        _ => None,
    };
}

/// <summary>
/// Value comparison type
/// </summary>
public enum InstructionComparer : byte
{
    None,
    IsEqual,
    IsNotEqual,
    IsGreaterThan,
    IsGreaterThanOrEqual,
    IsLessThan,
    IsLessThanOrEqual,
}

public static class InstructionComparerExtensions
{
    /// <summary>
    /// Indicates if the <see cref="comparer"/> is supported by the logic.
    /// </summary>
    /// <param name="comparer">Type of comparison requested</param>
    /// <returns>True if supported, false if unsupported.</returns>
    public static bool IsSupportedComparer(this InstructionComparer comparer) => comparer switch
    {
        IsEqual => true,
        IsNotEqual => true,
        IsGreaterThan => true,
        IsGreaterThanOrEqual => true,
        IsLessThan => true,
        IsLessThanOrEqual => true,
        _ => false,
    };

    /// <summary>
    /// Checks if the compare operator is satisfied by a boolean comparison result.
    /// </summary>
    /// <param name="comparer">Type of comparison requested</param>
    /// <param name="compareResult">Result from Equals comparison</param>
    /// <returns>True if satisfied</returns>
    /// <remarks>Only use this method if the comparison is boolean only. Use the <see cref="IsCompareOperator"/> otherwise.</remarks>
    public static bool IsCompareEquivalence(this InstructionComparer comparer, bool compareResult) => comparer switch
    {
        IsEqual => compareResult,
        IsNotEqual => !compareResult,
        _ => false,
    };

    /// <summary>
    /// Checks if the compare operator is satisfied by the <see cref="IComparable.CompareTo"/> result.
    /// </summary>
    /// <param name="comparer">Type of comparison requested</param>
    /// <param name="compareResult">Result from CompareTo</param>
    /// <returns>True if satisfied</returns>
    public static bool IsCompareOperator(this InstructionComparer comparer, int compareResult) => comparer switch
    {
        IsEqual => compareResult is 0,
        IsNotEqual => compareResult is not 0,
        IsGreaterThan => compareResult > 0,
        IsGreaterThanOrEqual => compareResult >= 0,
        IsLessThan => compareResult < 0,
        IsLessThanOrEqual => compareResult <= 0,
        _ => false,
    };
}
