using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace PKHeX.Core
{
    public partial class Util
    {
        private const string TranslationSplitter = " = ";

        #region String Lists        

        /// <summary>
        /// Gets a list of all Pokémon species names.
        /// </summary>
        /// <param name="language">Language of the Pokémon species names to select (e.g. "en", "fr", "jp", etc.)</param>
        /// <returns>An array of strings whose indexes correspond to the IDs of each Pokémon species name.</returns>
        public static string[] GetSpeciesList(string language)
        {
            return GetStringList("species", language);
        }

        /// <summary>
        /// Gets a list of all move names.
        /// </summary>
        /// <param name="language">Language of the move names to select (e.g. "en", "fr", "jp", etc.)</param>
        /// <returns>An array of strings whose indexes correspond to the IDs of each move name.</returns>
        public static string[] GetMovesList(string language)
        {
            return GetStringList("moves", language);
        }

        /// <summary>
        /// Gets a list of all Pokémon ability names.
        /// </summary>
        /// <param name="language">Language of the Pokémon ability names to select (e.g. "en", "fr", "jp", etc.)</param>
        /// <returns>An array of strings whose indexes correspond to the IDs of each Pokémon ability name.</returns>
        public static string[] GetAbilitiesList(string language)
        {
            return GetStringList("abilities", language);
        }

        /// <summary>
        /// Gets a list of all Pokémon nature names.
        /// </summary>
        /// <param name="language">Language of the Pokémon nature names to select (e.g. "en", "fr", "jp", etc.)</param>
        /// <returns>An array of strings whose indexes correspond to the IDs of each Pokémon nature name.</returns>
        public static string[] GetNaturesList(string language)
        {
            return GetStringList("natures", language);
        }

        /// <summary>
        /// Gets a list of all Pokémon form names.
        /// </summary>
        /// <param name="language">Language of the Pokémon form names to select (e.g. "en", "fr", "jp", etc.)</param>
        /// <returns>An array of strings whose indexes correspond to the IDs of each Pokémon form name.</returns>
        public static string[] GetFormsList(string language)
        {
            return GetStringList("forms", language);
        }

        /// <summary>
        /// Gets a list of all Pokémon type names.
        /// </summary>
        /// <param name="language">Language of the Pokémon type names to select (e.g. "en", "fr", "jp", etc.)</param>
        /// <returns>An array of strings whose indexes correspond to the IDs of each Pokémon type name.</returns>
        public static string[] GetTypesList(string language)
        {
            return GetStringList("types", language);
        }

        /// <summary>
        /// Gets a list of all Pokémon characteristic.
        /// </summary>
        /// <param name="language">Language of the Pokémon characteristic to select (e.g. "en", "fr", "jp", etc.)</param>
        /// <returns>An array of strings whose indexes correspond to the IDs of each Pokémon characteristic.</returns>
        public static string[] GetCharacteristicsList(string language)
        {
            return GetStringList("character", language);
        }

        /// <summary>
        /// Gets a list of all items.
        /// </summary>
        /// <param name="language">Language of the items to select (e.g. "en", "fr", "jp", etc.)</param>
        /// <returns>An array of strings whose indexes correspond to the IDs of each item.</returns>
        public static string[] GetItemsList(string language)
        {
            return GetStringList("items", language);
        }

        #endregion

        public static string[] GetStringList(string f)
        {
            var txt = Properties.Resources.ResourceManager.GetString(f); // Fetch File, \n to list.
            if (txt == null) return new string[0];
            string[] rawlist = (txt).Split('\n');
            for (int i = 0; i < rawlist.Length; i++)
                rawlist[i] = rawlist[i].Trim();
            return rawlist;
        }
        public static string[] GetStringList(string f, string l)
        {
            var txt = Properties.Resources.ResourceManager.GetString("text_" + f + "_" + l); // Fetch File, \n to list.
            if (txt == null) return new string[0];
            string[] rawlist = (txt).Split('\n');
            for (int i = 0; i < rawlist.Length; i++)
                rawlist[i] = rawlist[i].Trim();
            return rawlist;
        }
        public static string[] GetStringListFallback(string f, string l, string fallback)
        {
            string[] text = GetStringList(f, l);
            if (text.Length == 0)
                text = GetStringList(f, fallback);
            return text;
        }
        public static string[] GetNulledStringArray(string[] SimpleStringList)
        {
            try
            {
                string[] newlist = new string[ToInt32(SimpleStringList[SimpleStringList.Length - 1].Split(',')[0]) + 1];
                for (int i = 1; i < SimpleStringList.Length; i++)
                    newlist[ToInt32(SimpleStringList[i].Split(',')[0])] = SimpleStringList[i].Split(',')[1];
                return newlist;
            }
            catch { return null; }
        }

        public static byte[] GetBinaryResource(string name)
        {
            using (var resource = typeof(Util).GetTypeInfo().Assembly.GetManifestResourceStream("PKHeX.Core.Resources.byte." + name))
            {
                var buffer = new byte[resource.Length];
                resource.Read(buffer, 0, (int)resource.Length);
                return buffer;
            }               
        }

        #region Non-Form Translation
        /// <summary>
        /// Gets the names of the properties defined in the given input
        /// </summary>
        /// <param name="input">Enumerable of translation definitions in the form "Property = Value".</param>
        /// <returns></returns>
        private static string[] GetProperties(IEnumerable<string> input)
        {
            return input.Select(l => l.Substring(0, l.IndexOf(TranslationSplitter, StringComparison.Ordinal))).ToArray();
        }

        private static IEnumerable<string> DumpStrings(Type t)
        {
            var props = ReflectUtil.GetPropertiesStartWithPrefix(t, "V");
            return props.Select(p => $"{p}{TranslationSplitter}{ReflectUtil.GetValue(t, p).ToString()}");
        }

        /// <summary>
        /// Gets the current localization in a static class containing language-specific strings
        /// </summary>
        public static string[] GetLocalization(Type t, string[] existingLines = null)
        {
            existingLines = existingLines ?? new string[0];
            var currentLines = DumpStrings(t).ToArray();
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
                    ReflectUtil.SetValue(t, prop.ToUpper(), value);
                }
                catch
                {
                    Console.WriteLine($"Property not present: {prop} || Value written: {value}");
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
        public static List<ComboItem> GetCBList(string textfile, string lang)
        {
            // Set up
            string[] inputCSV = GetStringList(textfile);

            // Get Language we're fetching for
            int index = Array.IndexOf(new[] { "ja", "en", "fr", "de", "it", "es", "ko", "zh", }, lang);

            // Set up our Temporary Storage
            string[] unsortedList = new string[inputCSV.Length - 1];
            int[] indexes = new int[inputCSV.Length - 1];

            // Gather our data from the input file
            for (int i = 1; i < inputCSV.Length; i++)
            {
                string[] countryData = inputCSV[i].Split(',');
                indexes[i - 1] = Convert.ToInt32(countryData[0]);
                unsortedList[i - 1] = countryData[index + 1];
            }

            // Sort our input data
            string[] sortedList = new string[inputCSV.Length - 1];
            Array.Copy(unsortedList, sortedList, unsortedList.Length);
            Array.Sort(sortedList);

            // Arrange the input data based on original number
            return sortedList.Select(s => new ComboItem
            {
                Text = s,
                Value = indexes[Array.IndexOf(unsortedList, s)]
            }).ToList();
        }
        public static List<ComboItem> GetCBList(string[] inStrings, params int[][] allowed)
        {
            List<ComboItem> cbList = new List<ComboItem>();
            if (allowed?.First() == null)
                allowed = new[] { Enumerable.Range(0, inStrings.Length).ToArray() };

            foreach (int[] list in allowed)
            {
                // Sort the Rest based on String Name
                string[] unsortedChoices = new string[list.Length];
                for (int i = 0; i < list.Length; i++)
                    unsortedChoices[i] = inStrings[list[i]];

                string[] sortedChoices = new string[unsortedChoices.Length];
                Array.Copy(unsortedChoices, sortedChoices, unsortedChoices.Length);
                Array.Sort(sortedChoices);

                // Add the rest of the items
                cbList.AddRange(sortedChoices.Select(s => new ComboItem
                {
                    Text = s,
                    Value = list[Array.IndexOf(unsortedChoices, s)]
                }));
            }
            return cbList;
        }
        public static List<ComboItem> GetOffsetCBList(List<ComboItem> cbList, string[] inStrings, int offset, int[] allowed)
        {
            if (allowed == null)
                allowed = Enumerable.Range(0, inStrings.Length).ToArray();

            int[] list = (int[])allowed.Clone();
            for (int i = 0; i < list.Length; i++)
                list[i] -= offset;

            // Sort the Rest based on String Name
            string[] unsortedChoices = new string[allowed.Length];
            for (int i = 0; i < allowed.Length; i++)
                unsortedChoices[i] = inStrings[list[i]];

            string[] sortedChoices = new string[unsortedChoices.Length];
            Array.Copy(unsortedChoices, sortedChoices, unsortedChoices.Length);
            Array.Sort(sortedChoices);

            var indices = new Dictionary<string, int>();
            foreach (var str in unsortedChoices.Where(str => !indices.ContainsKey(str)))
                indices.Add(str, 0);

            // Add the rest of the items
            foreach (var s in sortedChoices)
            {
                var index = Array.IndexOf(unsortedChoices, s, indices[s]);
                cbList.Add(new ComboItem
                {
                    Text = s,
                    Value = allowed[index]
                });
                indices[s] = index + 1;
            }

            
            return cbList;
        }
        public static List<ComboItem> GetVariedCBList(string[] inStrings, int[] stringNum, int[] stringVal)
        {
            // Set up
            List<ComboItem> newlist = new List<ComboItem>();

            for (int i = 4; i > 1; i--) // add 4,3,2
            {
                // First 3 Balls are always first
                ComboItem ncbi = new ComboItem
                {
                    Text = inStrings[i],
                    Value = i
                };
                newlist.Add(ncbi);
            }

            // Sort the Rest based on String Name
            string[] ballnames = new string[stringNum.Length];
            for (int i = 0; i < stringNum.Length; i++)
                ballnames[i] = inStrings[stringNum[i]];

            string[] sortedballs = new string[stringNum.Length];
            Array.Copy(ballnames, sortedballs, ballnames.Length);
            Array.Sort(sortedballs);

            // Add the rest of the balls
            newlist.AddRange(sortedballs.Select(s => new ComboItem
            {
                Text = s,
                Value = stringVal[Array.IndexOf(ballnames, s)]
            }));
            return newlist;
        }
        public static List<ComboItem> GetUnsortedCBList(string textfile)
        {
            // Set up
            List<ComboItem> cbList = new List<ComboItem>();
            string[] inputCSV = GetStringList(textfile);

            // Gather our data from the input file
            for (int i = 1; i < inputCSV.Length; i++)
            {
                string[] inputData = inputCSV[i].Split(',');
                ComboItem ncbi = new ComboItem
                {
                    Text = inputData[1],
                    Value = Convert.ToInt32(inputData[0])
                };
                cbList.Add(ncbi);
            }
            return cbList;
        }
        #endregion
    }
}
