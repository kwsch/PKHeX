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
/// <param name="PropertyName">Property to modify.</param>
/// <param name="PropertyValue">Value to set to the property.</param>
/// <param name="Comparer">Filter Comparison Type</param>
public sealed record StringInstruction(string PropertyName, string PropertyValue, InstructionComparer Comparer, InstructionOperation Operation = InstructionOperation.Set)
{
    public string PropertyValue { get; private set; } = PropertyValue;

    /// <summary>
    /// Sets the <see cref="PropertyValue"/> to the index of the value in the input <see cref="arr"/>, if it exists.
    /// </summary>
    /// <param name="arr">List of values to search for the <see cref="PropertyValue"/>.</param>
    /// <returns>True if the value was found and set, false otherwise.</returns>
    public bool SetScreenedValue(ReadOnlySpan<string> arr)
    {
        int index = arr.IndexOf(PropertyValue);
        if ((uint)index >= arr.Length)
            return false;
        PropertyValue = index.ToString();
        return true;
    }

    /// <summary>
    /// Valid prefixes that are recognized for <see cref="InstructionComparer"/> value comparison types.
    /// </summary>
    public static ReadOnlySpan<char> Prefixes =>
    [
        Apply,
        FilterEqual, FilterNotEqual, FilterGreaterThan, FilterGreaterThanOrEqual, FilterLessThan, FilterLessThanOrEqual,
        ApplyAdd, ApplySubtract, ApplyMultiply, ApplyDivide, ApplyModulo,
        ApplyBitwiseAnd, ApplyBitwiseOr, ApplyBitwiseXor, ApplyBitwiseShiftRight, ApplyBitwiseShiftLeft,
    ];

    public static bool IsFilterInstruction(char c) => c switch
    {
        FilterEqual => true,
        FilterNotEqual => true,
        FilterGreaterThan => true,
        FilterLessThan => true,
        FilterGreaterThanOrEqual => true,
        FilterLessThanOrEqual => true,
        _ => false,
    };

    public static bool IsMutationInstruction(char c) => !IsFilterInstruction(c);

    private const char Apply = '.';
    private const char ApplyAdd = '+';
    private const char ApplySubtract = '-';
    private const char ApplyMultiply = '*';
    private const char ApplyDivide = '/';
    private const char ApplyModulo = '%';
    private const char ApplyBitwiseAnd = '&';
    private const char ApplyBitwiseOr = '|';
    private const char ApplyBitwiseXor = '^';
    private const char ApplyBitwiseShiftRight = '»';
    private const char ApplyBitwiseShiftLeft = '«';

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

    /// <summary>
    /// Apply a <see cref="RandomValue"/> instead of fixed value, based on the <see cref="RandomMinimum"/> and <see cref="RandomMaximum"/> values.
    /// </summary>
    public bool Random { get; private set; }

    /// <summary>
    /// Gets a <see cref="Random"/> value, based on the <see cref="RandomMinimum"/> and <see cref="RandomMaximum"/> values.
    /// </summary>
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
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(index);

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

    /// <summary>
    /// Gets a list of <see cref="StringInstruction"/>s from the input <see cref="text"/>.
    /// </summary>
    public static List<StringInstruction> GetFilters(ReadOnlySpan<char> text) => GetFilters(text.EnumerateLines());

    /// <summary>
    /// Gets a list of <see cref="StringInstruction"/> filters from the input <see cref="lines"/>.
    /// </summary>
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

    /// <summary>
    /// Gets a list of <see cref="StringInstruction"/> filters from the input <see cref="lines"/>.
    /// </summary>
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

    /// <summary>
    /// Gets a list of <see cref="StringInstruction"/> filters from the input <see cref="lines"/>.
    /// </summary>
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

    /// <summary>
    /// Gets a list of <see cref="StringInstruction"/> filters from the input <see cref="lines"/>.
    /// </summary>
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

    /// <summary>
    /// Gets a list of <see cref="StringInstruction"/> instructions from the input <see cref="text"/>.
    /// </summary>
    public static List<StringInstruction> GetInstructions(ReadOnlySpan<char> text) => GetInstructions(text.EnumerateLines());

    /// <summary>
    /// Gets a list of <see cref="StringInstruction"/> instructions from the input <see cref="lines"/>.
    /// </summary>
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

    /// <summary>
    /// Gets a list of <see cref="StringInstruction"/> instructions from the input <see cref="lines"/>.
    /// </summary>
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

    /// <summary>
    /// Gets a list of <see cref="StringInstruction"/> instructions from the input <see cref="lines"/>.
    /// </summary>
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

    /// <summary>
    /// Gets a list of <see cref="StringInstruction"/> instructions from the input <see cref="lines"/>.
    /// </summary>
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

    /// <summary>
    /// Tries to parse a <see cref="StringInstruction"/> filter from the input <see cref="line"/>.
    /// </summary>
    public static bool TryParseFilter(ReadOnlySpan<char> line, [NotNullWhen(true)] out StringInstruction? entry)
    {
        entry = null;
        if (line.Length is 0)
            return false;
        var comparer = GetComparer(line[0]);
        if (!comparer.IsSupported)
            return false;
        return TryParseSplitTuple(line[1..], ref entry, comparer);
    }

    /// <summary>
    /// Tries to parse a <see cref="StringInstruction"/> instruction from the input <see cref="line"/>.
    /// </summary>
    public static bool TryParseInstruction(ReadOnlySpan<char> line, [NotNullWhen(true)] out StringInstruction? entry)
    {
        entry = null;
        if (line.Length is 0 || !TryGetOperation(line[0], out var operation))
            return false;
        return TryParseSplitTuple(line[1..], ref entry, default, operation);
    }

    /// <summary>
    /// Tries to split a <see cref="StringInstruction"/> tuple from the input <see cref="tuple"/>.
    /// </summary>
    public static bool TryParseSplitTuple(ReadOnlySpan<char> tuple, [NotNullWhen(true)] ref StringInstruction? entry, InstructionComparer eval = default, InstructionOperation operation = InstructionOperation.Set)
    {
        if (!TryParseSplitTuple(tuple, out var name, out var value))
            return false;
        entry = new StringInstruction(name.ToString(), value.ToString(), eval, operation);
        return true;
    }

    /// <summary>
    /// Tries to split a <see cref="StringInstruction"/> tuple from the input <see cref="tuple"/>.
    /// </summary>
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
        return noExtra == -1;
    }

    /// <summary>
    /// Gets the <see cref="InstructionComparer"/> from the input <see cref="opCode"/>.
    /// </summary>
    public static InstructionComparer GetComparer(char opCode) => opCode switch
    {
        FilterEqual => IsEqual,
        FilterNotEqual => IsNotEqual,
        FilterGreaterThan => IsGreaterThan,
        FilterLessThan => IsLessThan,
        FilterGreaterThanOrEqual => IsGreaterThanOrEqual,
        FilterLessThanOrEqual => IsLessThanOrEqual,
        _ => None,
    };

    /// <summary>
    /// Gets the <see cref="InstructionOperation"/> from the input <see cref="opCode"/>.
    /// </summary>
    public static bool TryGetOperation(char opCode, out InstructionOperation operation)
    {
        switch (opCode)
        {
            case Apply:
                operation = InstructionOperation.Set;
                return true;
            case ApplyAdd:
                operation = InstructionOperation.Add;
                return true;
            case ApplySubtract:
                operation = InstructionOperation.Subtract;
                return true;
            case ApplyMultiply:
                operation = InstructionOperation.Multiply;
                return true;
            case ApplyDivide:
                operation = InstructionOperation.Divide;
                return true;
            case ApplyModulo:
                operation = InstructionOperation.Modulo;
                return true;
            case ApplyBitwiseAnd:
                operation = InstructionOperation.BitwiseAnd;
                return true;
            case ApplyBitwiseOr:
                operation = InstructionOperation.BitwiseOr;
                return true;
            case ApplyBitwiseXor:
                operation = InstructionOperation.BitwiseXor;
                return true;
            case ApplyBitwiseShiftRight:
                operation = InstructionOperation.BitwiseShiftRight;
                return true;
            case ApplyBitwiseShiftLeft:
                operation = InstructionOperation.BitwiseShiftLeft;
                return true;
            default:
                operation = default;
                return false;
        }
    }
}
