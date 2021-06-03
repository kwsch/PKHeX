using System;
using System.Collections.Generic;

namespace PKHeX.Core
{
    public static partial class Util
    {
        public static List<ComboItem> GetCountryRegionList(string textFile, string lang)
        {
            string[] inputCSV = GetStringList(textFile);
            int index = GeoLocation.GetLanguageIndex(lang);
            var list = GetCBListFromCSV(inputCSV, index + 1);
            list.Sort(Comparer);
            return list;
        }

        private static List<ComboItem> GetCBListFromCSV(IReadOnlyList<string> inputCSV, int index)
        {
            var arr = new List<ComboItem>(inputCSV.Count);
            foreach (var line in inputCSV)
            {
                var val = line[..3];
                var text = StringUtil.GetNthEntry(line, index, 4);
                var item = new ComboItem(text, Convert.ToInt32(val));
                arr.Add(item);
            }
            return arr;
        }

        public static List<ComboItem> GetCBList(ReadOnlySpan<string> inStrings)
        {
            var list = new List<ComboItem>(inStrings.Length);
            for (int i = 0; i < inStrings.Length; i++)
                list.Add(new ComboItem(inStrings[i], i));
            list.Sort(Comparer);
            return list;
        }

        public static List<ComboItem> GetCBList(IReadOnlyList<string> inStrings, IReadOnlyList<ushort> allowed)
        {
            var list = new List<ComboItem>(allowed.Count + 1) { new(inStrings[0], 0) };
            foreach (var index in allowed)
                list.Add(new ComboItem(inStrings[index], index));
            list.Sort(Comparer);
            return list;
        }

        public static List<ComboItem> GetCBList(IReadOnlyList<string> inStrings, int index, int offset = 0)
        {
            var list = new List<ComboItem>();
            AddCBWithOffset(list, inStrings, offset, index);
            return list;
        }

        public static IReadOnlyList<ComboItem> GetUnsortedCBList(IReadOnlyList<string> inStrings, IReadOnlyList<byte> allowed)
        {
            var count = allowed.Count;
            var list = new ComboItem[count];
            for (var i = 0; i < allowed.Count; i++)
            {
                var index = allowed[i];
                var item = new ComboItem(inStrings[index], index);
                list[i] = item;
            }

            return list;
        }

        public static List<ComboItem> GetCBList(IReadOnlyList<string> inStrings, int[] allowed)
        {
            var list = new List<ComboItem>(allowed.Length);
            AddCB(list, inStrings, allowed);
            return list;
        }

        public static void AddCBWithOffset(List<ComboItem> list, IReadOnlyList<string> inStrings, int offset, int index)
        {
            var item = new ComboItem(inStrings[index - offset], index);
            list.Add(item);
        }

        public static void AddCBWithOffset(List<ComboItem> cbList, IReadOnlyList<string> inStrings, int offset, int[] allowed)
        {
            int beginCount = cbList.Count;
            cbList.Capacity += allowed.Length;
            foreach (var index in allowed)
            {
                var item = new ComboItem(inStrings[index - offset], index);
                cbList.Add(item);
            }
            cbList.Sort(beginCount, allowed.Length, Comparer);
        }

        public static void AddCBWithOffset(List<ComboItem> cbList, Span<string> inStrings, int offset)
        {
            int beginCount = cbList.Count;
            cbList.Capacity += inStrings.Length;
            for (int i = 0; i < inStrings.Length; i++)
            {
                var x = inStrings[i];
                var item = new ComboItem(x, i + offset);
                cbList.Add(item);
            }
            cbList.Sort(beginCount, inStrings.Length, Comparer);
        }

        public static void AddCB(List<ComboItem> cbList, IReadOnlyList<string> inStrings, int[] allowed)
        {
            int beginCount = cbList.Count;
            cbList.Capacity += allowed.Length;
            foreach (var index in allowed)
            {
                var item = new ComboItem(inStrings[index], index);
                cbList.Add(item);
            }
            cbList.Sort(beginCount, allowed.Length, Comparer);
        }

        public static ComboItem[] GetVariedCBListBall(string[] inStrings, ushort[] stringNum, byte[] stringVal)
        {
            const int forcedTop = 3; // 3 Balls are preferentially first
            var list = new ComboItem[forcedTop + stringNum.Length];
            list[0] = new ComboItem(inStrings[4], (int)Ball.Poke);
            list[1] = new ComboItem(inStrings[3], (int)Ball.Great);
            list[2] = new ComboItem(inStrings[2], (int)Ball.Ultra);

            for (int i = 0; i < stringNum.Length; i++)
            {
                int index = stringNum[i];
                var val = stringVal[i];
                var txt = inStrings[index];
                list[i + 3] = new ComboItem(txt, val);
            }

            Array.Sort(list, 3, list.Length - 3, Comparer);
            return list;
        }

        private static readonly FunctorComparer<ComboItem> Comparer =
            new((a, b) => string.CompareOrdinal(a.Text, b.Text));

        private sealed class FunctorComparer<T> : IComparer<T>
        {
            private readonly Comparison<T> Comparison;
            public FunctorComparer(Comparison<T> comparison) => Comparison = comparison;
            public int Compare(T x, T y) => Comparison(x, y);
        }
    }
}
