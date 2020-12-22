using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace PKHeX.Core
{
    public static class LocalizationUtil
    {
        private const string TranslationSplitter = " = ";

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
        private static void SetLocalization(Type t, IReadOnlyCollection<string> lines)
        {
            if (lines.Count == 0)
                return;
            foreach (var line in lines)
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
#pragma warning disable CA1031 // Do not catch general exception types
                // Malformed translation files, log
                catch (Exception e)
#pragma warning restore CA1031 // Do not catch general exception types
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
            var lines = Util.GetStringList($"{languageFilePrefix}_{currentCultureCode}");
            SetLocalization(t, lines);
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
    }
}
