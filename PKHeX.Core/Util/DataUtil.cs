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

        public static string[][] GetLanguageStrings7(string fileName)
        {
            return new[]
            {
                Array.Empty<string>(), // 0 - None
                GetStringList(fileName, "ja"), // 1
                GetStringList(fileName, "en"), // 2
                GetStringList(fileName, "fr"), // 3
                GetStringList(fileName, "it"), // 4
                GetStringList(fileName, "de"), // 5
                Array.Empty<string>(), // 6 - None
                GetStringList(fileName, "es"), // 7
            };
        }

        public static string[][] GetLanguageStrings8(string fileName)
        {
            return new[]
            {
                Array.Empty<string>(), // 0 - None
                GetStringList(fileName, "ja"), // 1
                GetStringList(fileName, "en"), // 2
                GetStringList(fileName, "fr"), // 3
                GetStringList(fileName, "it"), // 4
                GetStringList(fileName, "de"), // 5
                Array.Empty<string>(), // 6 - None
                GetStringList(fileName, "es"), // 7
                GetStringList(fileName, "ko"), // 8
            };
        }

        public static string[][] GetLanguageStrings10(string fileName, string zh2 = "zh")
        {
            return new[]
            {
                Array.Empty<string>(), // 0 - None
                GetStringList(fileName, "ja"), // 1
                GetStringList(fileName, "en"), // 2
                GetStringList(fileName, "fr"), // 3
                GetStringList(fileName, "it"), // 4
                GetStringList(fileName, "de"), // 5
                Array.Empty<string>(), // 6 - None
                GetStringList(fileName, "es"), // 7
                GetStringList(fileName, "ko"), // 8
                GetStringList(fileName, "zh"), // 9
                GetStringList(fileName, zh2), // 10
            };
        }

        #endregion

        public static string[] GetStringList(string fileName)
        {
            if (IsStringListCached(fileName, out var result))
                return result;
            var txt = GetStringResource(fileName); // Fetch File, \n to list.
            return LoadStringList(fileName, txt);
        }

        public static bool IsStringListCached(string fileName, out string[] result)
        {
            lock (getStringListLoadLock) // Make sure only one thread can read the cache
                return stringListCache.TryGetValue(fileName, out result);
        }

        public static string[] LoadStringList(string file, string? txt)
        {
            if (txt == null)
                return Array.Empty<string>();
            string[] rawlist = txt.Split('\n');
            for (int i = 0; i < rawlist.Length; i++)
                rawlist[i] = rawlist[i].TrimEnd('\r');

            lock (getStringListLoadLock) // Make sure only one thread can write to the cache
            {
                if (!stringListCache.ContainsKey(file)) // Check cache again in case of race condition
                    stringListCache.Add(file, rawlist);
            }

            return (string[])rawlist.Clone();
        }

        public static string[] GetStringList(string fileName, string lang2char, string type = "text") => GetStringList($"{type}_{fileName}_{lang2char}");

        public static byte[] GetBinaryResource(string name)
        {
            using var resource = thisAssembly.GetManifestResourceStream($"PKHeX.Core.Resources.byte.{name}");
            var buffer = new byte[resource.Length];
            resource.Read(buffer, 0, (int)resource.Length);
            return buffer;
        }

        public static string? GetStringResource(string name)
        {
            if (!resourceNameMap.TryGetValue(name, out var resname))
            {
                bool Match(string x) => x.StartsWith("PKHeX.Core.Resources.text.") && x.EndsWith($"{name}.txt", StringComparison.OrdinalIgnoreCase);
                resname = Array.Find(manifestResourceNames, Match);
                if (resname == null)
                    return null;
                resourceNameMap.Add(name, resname);
            }

            using var resource = thisAssembly.GetManifestResourceStream(resname);
            if (resource == null)
                return null;
            using var reader = new StreamReader(resource);
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
        public static string[] GetLocalization(Type t) => DumpStrings(t).ToArray();

        /// <summary>
        /// Gets the current localization in a static class containing language-specific strings
        /// </summary>
        /// <param name="t"></param>
        /// <param name="existingLines">Existing localization lines (if provided)</param>
        public static string[] GetLocalization(Type t, string[] existingLines)
        {
            var currentLines = GetLocalization(t);
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
        private static void SetLocalization(Type t, IReadOnlyList<string> lines)
        {
            if (lines.Count == 0)
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

        private static readonly string[] CountryRegionLanguages = {"ja", "en", "fr", "de", "it", "es", "zh", "ko"};

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
            var list = new List<ComboItem>(allowed.Count + 1) { new ComboItem(inStrings[0], 0) };
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

        public static List<ComboItem> GetVariedCBListBall(string[] inStrings, int[] stringNum, int[] stringVal)
        {
            const int forcedTop = 3; // 3 Balls are preferentially first
            var list = new List<ComboItem>(forcedTop + stringNum.Length)
            {
                new ComboItem(inStrings[4], (int)Ball.Poke),
                new ComboItem(inStrings[3], (int)Ball.Great),
                new ComboItem(inStrings[2], (int)Ball.Ultra),
            };

            for (int i = 0; i < stringNum.Length; i++)
            {
                int index = stringNum[i];
                var val = stringVal[i];
                var txt = inStrings[index];
                list.Add(new ComboItem(txt, val));
            }

            list.Sort(forcedTop, stringNum.Length, Comparer);
            return list
;
        }

        private static readonly FunctorComparer<ComboItem> Comparer =
            new FunctorComparer<ComboItem>((a, b) => string.CompareOrdinal(a.Text, b.Text));

        private sealed class FunctorComparer<T> : IComparer<T>
        {
            private readonly Comparison<T> Comparison;
            public FunctorComparer(Comparison<T> comparison) => Comparison = comparison;
            public int Compare(T x, T y) => Comparison(x, y);
        }
        #endregion
    }
}
