using System;
using System.Collections.Generic;

namespace PKHeX.Core
{
    /// <summary>
    /// Key Value pair for a displayed <see cref="T:System.String" /> and underlying <see cref="T:System.Int32" /> value.
    /// </summary>
    public readonly struct ComboItem : IEquatable<int>
    {
        public string Text { get; }
        public int Value { get; }

        public ComboItem(string text, int value)
        {
            Text = text;
            Value = value;
        }

        public bool Equals(ComboItem other) => Value == other.Value && string.Equals(Text, other.Text);
        public bool Equals(int other) => Value == other;
        public override bool Equals(object obj) => obj is ComboItem other && Equals(other);
        public override int GetHashCode() => Value;
        public static bool operator ==(ComboItem left, ComboItem right) => left.Equals(right);
        public static bool operator !=(ComboItem left, ComboItem right) => !(left == right);
    }

    public static class ComboItemExtensions
    {
        public static string[] GetArray(this IReadOnlyList<ComboItem> list)
        {
            var max = list[list.Count - 1].Value;
            return GetArray(list, max);
        }

        public static string[] GetArray(this IEnumerable<ComboItem> list, int max)
        {
            var arr = new string[max + 1];
            foreach (var item in list)
                arr[item.Value] = item.Text;
            return arr;
        }
    }
}
