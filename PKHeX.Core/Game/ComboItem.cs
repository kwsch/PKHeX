using System.Collections.Generic;

namespace PKHeX.Core
{
    /// <summary>
    /// Key Value pair for a displayed <see cref="string"/> and underlying <see cref="int"/> value.
    /// </summary>
    public struct ComboItem
    {
        public string Text { get; set; }
        public int Value { get; set; }
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
