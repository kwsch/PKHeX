using System;
using System.Collections.Generic;
using System.Reflection;

namespace PKHeX.Core;

/// <summary>
/// Localization utility for changing all properties of a static type.
/// </summary>
public static class LocalizationUtil
{
    private const string TranslationSplitter = " = ";
    private const char TranslationFirst = ' '; // perf; no properties contain spaces.

    /// <summary>
    /// Gets the names of the properties defined in the given input
    /// </summary>
    /// <param name="input">Enumerable of translation definitions in the form "Property = Value".</param>
    private static string[] GetProperties(ReadOnlySpan<string> input)
    {
        var result = new string[input.Length];
        for (int i = 0; i < input.Length; i++)
        {
            var l = input[i];
            var split = l.IndexOf(TranslationFirst);
            result[i] = l[..split];
        }
        return result;
    }

    private static string[] DumpStrings(Type t)
    {
        var ti = t.GetTypeInfo();
        var props = (PropertyInfo[])ti.DeclaredProperties;
        var result = new string[props.Length];
        for (int i = 0; i < props.Length; i++)
        {
            var p = props[i];
            var value = p.GetValue(null);
            result[i] = $"{p.Name}{TranslationSplitter}{value}";
        }
        return result;
    }

    /// <summary>
    /// Gets the current localization in a static class containing language-specific strings
    /// </summary>
    /// <param name="t"></param>
    public static string[] GetLocalization(Type t) => DumpStrings(t);

    /// <summary>
    /// Gets the current localization in a static class containing language-specific strings
    /// </summary>
    /// <param name="t"></param>
    /// <param name="existingLines">Existing localization lines (if provided)</param>
    public static string[] GetLocalization(Type t, ReadOnlySpan<string> existingLines)
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
    private static void SetLocalization(Type t, ReadOnlySpan<string> lines)
    {
        if (lines.Length == 0)
            return;

        var dict = GetTranslationDict(lines);
        var ti = t.GetTypeInfo();
        foreach (var p in ti.DeclaredProperties)
        {
            if (dict.TryGetValue(p.Name, out var value))
                p.SetValue(null, value);
        }
    }

    private static Dictionary<string, string> GetTranslationDict(ReadOnlySpan<string> lines)
    {
        var result = new Dictionary<string, string>(lines.Length);
        foreach (var line in lines)
        {
            var index = line.IndexOf(TranslationFirst);
            if (index < 0)
                continue;
            var prop = line[..index];
            result[prop] = line[(index + TranslationSplitter.Length)..];
        }
        return result;
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
