using System;
using System.Collections.Generic;
using System.Text;

namespace PKHeX.Core;

/// <summary>
/// Processes input of strings into a list of valid Filters and Instructions.
/// </summary>
public sealed class StringInstructionSet
{
    /// <summary>
    /// Filters to check if the object should be modified.
    /// </summary>
    public readonly IReadOnlyList<StringInstruction> Filters;

    /// <summary>
    /// Instructions to modify the object.
    /// </summary>
    public readonly IReadOnlyList<StringInstruction> Instructions;

    private const char SetSeparatorChar = ';';

    public StringInstructionSet(IReadOnlyList<StringInstruction> filters, IReadOnlyList<StringInstruction> instructions)
    {
        Filters = filters;
        Instructions = instructions;
    }

    public StringInstructionSet(ReadOnlySpan<char> text)
    {
        var set = text.EnumerateLines();
        Filters = StringInstruction.GetFilters(set);
        Instructions = StringInstruction.GetInstructions(set);
    }

    public StringInstructionSet(SpanLineEnumerator set)
    {
        Filters = StringInstruction.GetFilters(set);
        Instructions = StringInstruction.GetInstructions(set);
    }

    public StringInstructionSet(ReadOnlySpan<string> set)
    {
        Filters = StringInstruction.GetFilters(set);
        Instructions = StringInstruction.GetInstructions(set);
    }

    /// <summary>
    /// Checks if the input <see cref="text"/> is potentially formatted incorrectly.
    /// </summary>
    /// <remarks>Normally, no blank lines should be present in the input.</remarks>
    /// <returns>True if a blank line is found in the input.</returns>
    public static bool HasEmptyLine(ReadOnlySpan<char> text) => HasEmptyLine(text.EnumerateLines());

    /// <inheritdoc cref="HasEmptyLine(ReadOnlySpan{char})"/>
    public static bool HasEmptyLine(SpanLineEnumerator lines)
    {
        foreach (var line in lines)
        {
            if (line.IsEmpty || line.IsWhiteSpace())
                return true;
        }
        return false;
    }

    /// <summary>
    /// Gets a list of <see cref="StringInstructionSet"/>s from the input <see cref="lines"/>.
    /// </summary>
    public static StringInstructionSet[] GetBatchSets(ReadOnlySpan<string> lines)
    {
        int ctr = 0;
        int start = 0;
        while (start < lines.Length)
        {
            var slice = lines[start..];
            var count = GetInstructionSetLength(slice);
            ctr++;
            start += count + 1;
        }

        var result = new StringInstructionSet[ctr];
        ctr = 0;
        start = 0;
        while (start < lines.Length)
        {
            var slice = lines[start..];
            var count = GetInstructionSetLength(slice);
            var set = slice[..count];
            result[ctr++] = new StringInstructionSet(set);
            start += count + 1;
        }
        return result;
    }

    /// <summary>
    /// Gets a list of <see cref="StringInstructionSet"/>s from the input <see cref="text"/>.
    /// </summary>
    public static StringInstructionSet[] GetBatchSets(ReadOnlySpan<char> text)
    {
        int ctr = 0;
        int start = 0;
        while (start < text.Length)
        {
            var slice = text[start..];
            var count = GetInstructionSetLength(slice);
            ctr++;
            start += count + 1;
        }

        var result = new StringInstructionSet[ctr];
        ctr = 0;
        start = 0;
        while (start < text.Length)
        {
            var slice = text[start..];
            var count = GetInstructionSetLength(slice);
            var set = slice[..count];
            result[ctr++] = new StringInstructionSet(set);
            start += count + 1;
        }
        return result;
    }

    /// <summary>
    /// Scans through the <see cref="text"/> to count the amount of characters to consume.
    /// </summary>
    /// <param name="text">Multi line string</param>
    /// <returns>Amount of characters comprising a set of instructions</returns>
    public static int GetInstructionSetLength(ReadOnlySpan<char> text)
    {
        int start = 0;
        while (start < text.Length)
        {
            var line = text[start..];
            if (line.Length != 0 && line[0] == SetSeparatorChar)
                return start;
            var next = line.IndexOf('\n');
            if (next == -1)
                return text.Length;
            start += next + 1;
        }
        return start;
    }

    /// <summary>
    /// Scans through the <see cref="lines"/> to count the amount of valid lines to consume.
    /// </summary>
    /// <returns>Amount of lines comprising a set of instructions.</returns>
    public static int GetInstructionSetLength(ReadOnlySpan<string> lines)
    {
        int start = 0;
        while (start < lines.Length)
        {
            var line = lines[start++];
            if (line.StartsWith(SetSeparatorChar))
                return start;
        }
        return start;
    }
}
