using System;
using System.Collections.Generic;

namespace PKHeX.Core;

/// <summary>
/// Event number storage for more complex logic events.
/// </summary>
/// <typeparam name="T"></typeparam>
public sealed class EventWork<T> : EventVar where T : struct
{
    public T Value;

    /// <summary>
    /// Values with known behavior. They are labeled with a humanized string.
    /// </summary>
    public readonly IList<EventWorkVal> Options = new List<EventWorkVal> { new() };

    public EventWork(int index, EventVarType t, IReadOnlyList<string> pieces) : base(index, t, pieces[1])
    {
        if (pieces.Count >= 3)
            AddLabeledValueOptions(pieces[2], Options);
    }

    private const char OptionSeparator = ','; // KVP,KVP
    private const char OptionValueSeparator = ':'; // Name:Value

    /// <summary>
    /// Adds the key-value pair options to the <see cref="list"/>.
    /// </summary>
    /// <param name="text">Text line containing key-value pairs.</param>
    /// <param name="list">Object that stores the results</param>
    /// <remarks>
    /// Decodes in the format of Value:Text,Value:Text.
    /// Key-value pairs are separated by <see cref="OptionSeparator"/>.
    /// Key and value are separated by <see cref="OptionValueSeparator"/>.
    /// </remarks>
    private static void AddLabeledValueOptions(ReadOnlySpan<char> text, ICollection<EventWorkVal> list)
    {
        int i = 0;
        while (i < text.Length)
        {
            // Scan for the length of this key-value pair.
            int commaIndex = text[i..].IndexOf(OptionSeparator);
            if (commaIndex == -1)
                commaIndex = text.Length - i; // end of string

            // Scan for the length of the key.
            var pair = text.Slice(i, commaIndex);
            i += commaIndex + 1; // skip to next kvp for next iteration
            int colonIndex = pair.IndexOf(OptionValueSeparator);
            if (colonIndex == -1)
                continue; // invalid kvp

            // Split to get the key and value.
            var valueText = pair[..colonIndex];
            var display = pair[(colonIndex + 1)..];
            if (int.TryParse(valueText, out int value))
                list.Add(new EventWorkVal(display.ToString(), value));
        }
    }
}
