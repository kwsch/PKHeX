using System;
using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core
{
    public static partial class Util
    {
        public static List<ComboItem> GetCountryRegionList(string textFile, string lang)
        {
            string[] inputCSV = GetStringList(textFile);
            int index = GeoLocation.GetLanguageIndex(lang);
            return GetCBListCSVSorted(inputCSV, index);
        }

        private static List<ComboItem> GetCBListCSVSorted(string[] inputCSV, int index = 0)
        {
            var list = GetCBListFromCSV(inputCSV, index);
            list.Sort(Comparer);
            return list;
        }

        public static List<ComboItem> GetCSVUnsortedCBList(string textFile)
        {
            string[] inputCSV = GetStringList(textFile);
            return GetCBListFromCSV(inputCSV, 0);
        }

        private static List<ComboItem> GetCBListFromCSV(IReadOnlyList<string> inputCSV, int index)
        {
            var arr = new List<ComboItem>(inputCSV.Count - 1); // skip header
            index++;
            for (int i = 1; i < inputCSV.Count; i++)
            {
                var line = inputCSV[i];
                var zeroth = line.IndexOf(',');

                var val = line.Substring(0, zeroth);
                var text = StringUtil.GetNthEntry(line, index, zeroth);
                var item = new ComboItem(text, Convert.ToInt32(val));
                arr.Add(item);
            }
            return arr;
        }

        public static List<ComboItem> GetCBList(IReadOnlyList<string> inStrings)
        {
            var list = new List<ComboItem>(inStrings.Count);
            for (int i = 0; i < inStrings.Count; i++)
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

        public static List<ComboItem> GetCBList(IReadOnlyList<string> inStrings, params int[][] allowed)
        {
            var count = allowed.Sum(z => z.Length);
            var list = new List<ComboItem>(count);
            foreach (var arr in allowed)
                AddCB(list, inStrings, arr);
            return list;
        }

        public static void AddCBWithOffset(List<ComboItem> list, IReadOnlyList<string> inStrings, int offset, int index)
        {
            var item = new ComboItem(inStrings[index - offset], index);
            list.Add(item);
        }

        public static void AddCBWithOffset(List<ComboItem> cbList, IReadOnlyList<string> inStrings, int offset, params int[] allowed)
        {
            int beginCount = cbList.Count;
            foreach (var index in allowed)
            {
                var item = new ComboItem(inStrings[index - offset], index);
                cbList.Add(item);
            }
            cbList.Sort(beginCount, allowed.Length, Comparer);
        }

        public static void AddCB(List<ComboItem> cbList, IReadOnlyList<string> inStrings, int[] allowed)
        {
            int beginCount = cbList.Count;
            foreach (var index in allowed)
            {
                var item = new ComboItem(inStrings[index], index);
                cbList.Add(item);
            }
            cbList.Sort(beginCount, allowed.Length, Comparer);
        }

        public static List<ComboItem> GetVariedCBListBall(string[] inStrings, ushort[] stringNum, byte[] stringVal)
        {
            const int forcedTop = 3; // 3 Balls are preferentially first
            var list = new List<ComboItem>(forcedTop + stringNum.Length)
            {
                new(inStrings[4], (int)Ball.Poke),
                new(inStrings[3], (int)Ball.Great),
                new(inStrings[2], (int)Ball.Ultra),
            };

            for (int i = 0; i < stringNum.Length; i++)
            {
                int index = stringNum[i];
                var val = stringVal[i];
                var txt = inStrings[index];
                list.Add(new ComboItem(txt, val));
            }

            list.Sort(forcedTop, stringNum.Length, Comparer);
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
