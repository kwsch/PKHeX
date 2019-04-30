using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace PKHeX.Core
{
    public static partial class Util
    {
        private const string TranslationSplitter = " = ";
        private static readonly Assembly thisAssembly = typeof(Util).GetTypeInfo().Assembly;
        private static readonly string[] manifestResourceNames = thisAssembly.GetManifestResourceNames();
        private static readonly Dictionary<string, string> resourceNameMap = new Dictionary<string, string>();
        private static readonly Dictionary<string, string[]> stringListCache = new Dictionary<string, string[]>();

        private static readonly object getStringListLoadLock = new object();

        #region String Lists

        /// <summary>
        /// Gets a list of all Pokémon species names.
        /// </summary>
        /// <param name="language">Language of the Pokémon species names to select (e.g. "en", "fr", "jp", etc.)</param>
        /// <returns>An array of strings whose indexes correspond to the IDs of each Pokémon species name.</returns>
        public static string[] GetSpeciesList(string language) => GetStringList("species", language);

        /// <summary>
        /// Gets a list of all move names.
        /// </summary>
        /// <param name="language">Language of the move names to select (e.g. "en", "fr", "jp", etc.)</param>
        /// <returns>An array of strings whose indexes correspond to the IDs of each move name.</returns>
        public static string[] GetMovesList(string language) => GetStringList("moves", language);

        /// <summary>
        /// Gets a list of all Pokémon ability names.
        /// </summary>
        /// <param name="language">Language of the Pokémon ability names to select (e.g. "en", "fr", "jp", etc.)</param>
        /// <returns>An array of strings whose indexes correspond to the IDs of each Pokémon ability name.</returns>
        public static string[] GetAbilitiesList(string language) => GetStringList("abilities", language);

        /// <summary>
        /// Gets a list of all Pokémon nature names.
        /// </summary>
        /// <param name="language">Language of the Pokémon nature names to select (e.g. "en", "fr", "jp", etc.)</param>
        /// <returns>An array of strings whose indexes correspond to the IDs of each Pokémon nature name.</returns>
        public static string[] GetNaturesList(string language) => GetStringList("natures", language);

        /// <summary>
        /// Gets a list of all Pokémon form names.
        /// </summary>
        /// <param name="language">Language of the Pokémon form names to select (e.g. "en", "fr", "jp", etc.)</param>
        /// <returns>An array of strings whose indexes correspond to the IDs of each Pokémon form name.</returns>
        public static string[] GetFormsList(string language) => GetStringList("forms", language);

        /// <summary>
        /// Gets a list of all Pokémon type names.
        /// </summary>
        /// <param name="language">Language of the Pokémon type names to select (e.g. "en", "fr", "jp", etc.)</param>
        /// <returns>An array of strings whose indexes correspond to the IDs of each Pokémon type name.</returns>
        public static string[] GetTypesList(string language) => GetStringList("types", language);

        /// <summary>
        /// Gets a list of all Pokémon characteristic.
        /// </summary>
        /// <param name="language">Language of the Pokémon characteristic to select (e.g. "en", "fr", "jp", etc.)</param>
        /// <returns>An array of strings whose indexes correspond to the IDs of each Pokémon characteristic.</returns>
        public static string[] GetCharacteristicsList(string language) => GetStringList("character", language);

        /// <summary>
        /// Gets a list of all items.
        /// </summary>
        /// <param name="language">Language of the items to select (e.g. "en", "fr", "jp", etc.)</param>
        /// <returns>An array of strings whose indexes correspond to the IDs of each item.</returns>
        public static string[] GetItemsList(string language) => GetStringList("items", language);

        #endregion

        public static string[] GetStringList(string f)
        {
            lock (getStringListLoadLock) // Make sure only one thread can read the cache
            {
                if (stringListCache.TryGetValue(f, out var result))
                    return (string[])result.Clone();
            }

            var txt = GetStringResource(f); // Fetch File, \n to list.
            if (txt == null)
                return Array.Empty<string>();
            string[] rawlist = txt.Split('\n');
            for (int i = 0; i < rawlist.Length; i++)
                rawlist[i] = rawlist[i].TrimEnd('\r');

            lock (getStringListLoadLock) // Make sure only one thread can write to the cache
            {
                if (!stringListCache.ContainsKey(f)) // Check cache again in case of race condition
                    stringListCache.Add(f, rawlist);
            }

            return (string[])rawlist.Clone();
        }

        public static string[] GetStringList(string f, string l, string type = "text") => GetStringList($"{type}_{f}_{l}");

        public static byte[] GetBinaryResource(string name)
        {
            using (var resource = thisAssembly.GetManifestResourceStream(
                $"PKHeX.Core.Resources.byte.{name}"))
            {
                var buffer = new byte[resource.Length];
                resource.Read(buffer, 0, (int)resource.Length);
                return buffer;
            }
        }

        public static string GetStringResource(string name)
        {
            if (!resourceNameMap.ContainsKey(name))
            {
                bool Match(string x) => x.StartsWith("PKHeX.Core.Resources.text.") && x.EndsWith($"{name}.txt", StringComparison.OrdinalIgnoreCase);
                var resname = Array.Find(manifestResourceNames, Match);
                resourceNameMap.Add(name, resname);
            }

            if (resourceNameMap[name] == null)
                return null;

            using (var resource = thisAssembly.GetManifestResourceStream(resourceNameMap[name]))
            using (var reader = new StreamReader(resource))
                return reader.ReadToEnd();
        }

        #region Non-Form Translation
        /// <summary>
        /// Gets the names of the properties defined in the given input
        /// </summary>
        /// <param name="input">Enumerable of translation definitions in the form "Property = Value".</param>
        private static string[] GetProperties(IEnumerable<string> input)
        {
            return input.Select(l => l.Substring(0, l.IndexOf(TranslationSplitter, StringComparison.Ordinal))).ToArray();
        }

        private static IEnumerable<string> DumpStrings(Type t)
        {
            var props = ReflectUtil.GetPropertiesStartWithPrefix(t, string.Empty);
            return props.Select(p => $"{p}{TranslationSplitter}{ReflectUtil.GetValue(t, p)}");
        }

        /// <summary>
        /// Gets the current localization in a static class containing language-specific strings
        /// </summary>
        /// <param name="t"></param>
        /// <param name="existingLines">Existing localization lines (if provided)</param>
        public static string[] GetLocalization(Type t, string[] existingLines = null)
        {
            var currentLines = DumpStrings(t).ToArray();
            if (existingLines == null)
                return currentLines;
            var existing = GetProperties(existingLines);
            var current = GetProperties(currentLines);

            var result = new string[currentLines.Length];
            for (int i = 0; i < current.Length; i++)
            {
                int index = Array.IndexOf(existing, current[i]);
                result[i] = index < 0 ? currentLines[i] : existingLines[index];
            }
            return result;
        }

        /// <summary>
        /// Applies localization to a static class containing language-specific strings.
        /// </summary>
        /// <param name="t">Type of the static class containing the desired strings.</param>
        /// <param name="lines">Lines containing the localized strings</param>
        private static void SetLocalization(Type t, IEnumerable<string> lines)
        {
            if (lines == null)
                return;
            foreach (var line in lines.Where(l => l != null))
            {
                var index = line.IndexOf(TranslationSplitter, StringComparison.Ordinal);
                if (index < 0)
                    continue;
                var prop = line.Substring(0, index);
                var value = line.Substring(index + TranslationSplitter.Length);

                try
                {
                    ReflectUtil.SetValue(t, prop, value);
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"Property not present: {prop} || Value written: {value}");
                    Debug.WriteLine(e.Message);
                }
            }
        }

        /// <summary>
        /// Applies localization to a static class containing language-specific strings.
        /// </summary>
        /// <param name="t">Type of the static class containing the desired strings.</param>
        /// <param name="languageFilePrefix">Prefix of the language file to use.  Example: if the target is legality_en.txt, <paramref name="languageFilePrefix"/> should be "legality".</param>
        /// <param name="currentCultureCode">Culture information</param>
        private static void SetLocalization(Type t, string languageFilePrefix, string currentCultureCode)
        {
            SetLocalization(t, GetStringList($"{languageFilePrefix}_{currentCultureCode}"));
        }

        /// <summary>
        /// Applies localization to a static class containing language-specific strings.
        /// </summary>
        /// <param name="t">Type of the static class containing the desired strings.</param>
        /// <remarks>The values used to translate the given static class are retrieved from [TypeName]_[CurrentLangCode2].txt in the resource manager of PKHeX.Core.</remarks>
        /// <param name="currentCultureCode">Culture information</param>
        public static void SetLocalization(Type t, string currentCultureCode)
        {
            SetLocalization(t, t.Name, currentCultureCode);
        }

        #endregion

        #region DataSource Providing

        private static readonly string[] CountryRegionLanguages = {"ja", "en", "fr", "de", "it", "es", "ko", "zh"};

        public static List<ComboItem> GetCountryRegionList(string textfile, string lang)
        {
            string[] inputCSV = GetStringList(textfile);
            int index = Array.IndexOf(CountryRegionLanguages, lang);
            return GetCBListCSVSorted(inputCSV, index);
        }

        private static List<ComboItem> GetCBListCSVSorted(string[] inputCSV, int index = 0)
        {
            var list = GetCBListFromCSV(inputCSV, index);
            list.Sort(Comparer);
            return list;
        }

        public static List<ComboItem> GetCSVUnsortedCBList(string textfile)
        {
            string[] inputCSV = GetStringList(textfile);
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
                var text = GetNthEntry(line, index, zeroth);
                var item = new ComboItem {Text = text, Value = Convert.ToInt32(val)};
                arr.Add(item);
            }
            return arr;
        }

        private static string GetNthEntry(string line, int nth, int start)
        {
            if (nth != 1)
                start = line.IndexOfNth(',', nth - 1, start + 1);
            var end = line.IndexOfNth(',', 1, start + 1);
            return end < 0 ? line.Substring(start + 1) : line.Substring(start + 1, end - start - 1);
        }

        private static int IndexOfNth(this string s, char t, int n, int start)
        {
            int count = 0;
            for (int i = start; i < s.Length; i++)
            {
                if (s[i] != t)
                    continue;
                if (++count == n)
                    return i;
            }
            return -1;
        }

        public static List<ComboItem> GetCBList(IReadOnlyList<string> inStrings)
        {
            var list = new List<ComboItem>(inStrings.Count);
            for (int i = 0; i < inStrings.Count; i++)
                list.Add(new ComboItem {Text = inStrings[i], Value = i});
            list.Sort(Comparer);
            return list;
        }

        public static List<ComboItem> GetCBList(IReadOnlyList<string> inStrings, int index, int offset = 0)
        {
            var list = new List<ComboItem>();
            AddCBWithOffset(list, inStrings, offset, index);
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
            var item = new ComboItem {Text = inStrings[index - offset], Value = index};
            list.Add(item);
        }

        public static void AddCBWithOffset(List<ComboItem> cbList, IReadOnlyList<string> inStrings, int offset, params int[] allowed)
        {
            int beginCount = cbList.Count;
            for (int i = 0; i < allowed.Length; i++)
            {
                int index = allowed[i];
                var item = new ComboItem {Text = inStrings[index - offset], Value = index};
                cbList.Add(item);
            }
            cbList.Sort(beginCount, allowed.Length, Comparer);
        }

        public static void AddCB(List<ComboItem> cbList, IReadOnlyList<string> inStrings, int[] allowed)
        {
            int beginCount = cbList.Count;
            for (int i = 0; i < allowed.Length; i++)
            {
                int index = allowed[i];
                var item = new ComboItem {Text = inStrings[index], Value = index};
                cbList.Add(item);
            }
            cbList.Sort(beginCount, allowed.Length, Comparer);
        }

        public static List<ComboItem> GetVariedCBListBall(string[] inStrings, int[] stringNum, int[] stringVal)
        {
            const int forcedTop = 3; // 3 Balls are preferentially first
            var list = new List<ComboItem>(forcedTop + stringNum.Length)
            {
                new ComboItem {Text = inStrings[4], Value = (int)Ball.Poke},
                new ComboItem {Text = inStrings[3], Value = (int)Ball.Great},
                new ComboItem {Text = inStrings[2], Value = (int)Ball.Ultra},
            };

            for (int i = 0; i < stringNum.Length; i++)
            {
                int index = stringNum[i];
                var val = stringVal[i];
                var txt = inStrings[index];
                list.Add(new ComboItem {Text = txt, Value = val});
            }

            list.Sort(forcedTop, stringNum.Length, Comparer);
            return list
;
        }

        private static readonly FunctorComparer<ComboItem> Comparer =
            new FunctorComparer<ComboItem>((a, b) => string.Compare(a.Text, b.Text, StringComparison.Ordinal));

        private sealed class FunctorComparer<T> : IComparer<T>
        {
            private readonly Comparison<T> Comparison;
            public FunctorComparer(Comparison<T> comparison) => Comparison = comparison;
            public int Compare(T x, T y) => Comparison(x, y);
        }
        #endregion
    }
}
