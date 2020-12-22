using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace PKHeX.Core
{
    public static partial class Util
    {
        private static readonly Assembly thisAssembly = typeof(Util).GetTypeInfo().Assembly;
        private static readonly string[] manifestResourceNames = thisAssembly.GetManifestResourceNames();
        private static readonly Dictionary<string, string> resourceNameMap = new();
        private static readonly Dictionary<string, string[]> stringListCache = new();

        private static readonly object getStringListLoadLock = new();

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
            string[] raw = txt.Split('\n');
            for (int i = 0; i < raw.Length; i++)
                raw[i] = raw[i].TrimEnd('\r');

            lock (getStringListLoadLock) // Make sure only one thread can write to the cache
            {
                if (!stringListCache.ContainsKey(file)) // Check cache again in case of race condition
                    stringListCache.Add(file, raw);
            }

            return (string[])raw.Clone();
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
            if (!resourceNameMap.TryGetValue(name, out var resourceName))
            {
                bool Match(string x) => x.StartsWith("PKHeX.Core.Resources.text.") && x.EndsWith($"{name}.txt", StringComparison.OrdinalIgnoreCase);
                resourceName = Array.Find(manifestResourceNames, Match);
                if (resourceName == null)
                    return null;
                resourceNameMap.Add(name, resourceName);
            }

            using var resource = thisAssembly.GetManifestResourceStream(resourceName);
            if (resource == null)
                return null;
            using var reader = new StreamReader(resource);
            return reader.ReadToEnd();
        }
    }
}
