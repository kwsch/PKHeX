using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PKHeX.Core;

/// <summary>
/// Miscellaneous setup utility for legality checking <see cref="IEncounterTemplate"/> data sources.
/// </summary>
internal static class EncounterUtil
{
    internal static ReadOnlySpan<byte> Get([Length(2, 2)] string resource) => Util.GetBinaryResource($"encounter_{resource}.pkl");
    internal static BinLinkerAccessor Get([Length(2, 2)] string resource, [Length(2, 2)] ReadOnlySpan<byte> ident) => BinLinkerAccessor.Get(Get(resource), ident);

    internal static T? GetMinByLevel<T>(ReadOnlySpan<EvoCriteria> chain, IEnumerable<T> possible) where T : class, IEncounterTemplate
    {
        // MinBy grading: prefer species-form match, select lowest min level encounter.
        // Minimum allocation :)
        T? result = null;
        int min = int.MaxValue;

        foreach (var enc in possible)
        {
            int m = int.MaxValue;
            foreach (var evo in chain)
            {
                bool specDiff = enc.Species != evo.Species || enc.Form != evo.Form;
                var val = (Convert.ToInt32(specDiff) << 16) | enc.LevelMin;
                if (val < m)
                    m = val;
            }

            if (m >= min)
                continue;
            min = m;
            result = enc;
        }

        return result;
    }

    /// <summary>
    /// Grabs the localized names for individual templates for all languages from the specified <see cref="index"/> of the <see cref="names"/> list.
    /// </summary>
    /// <param name="names">Arrays of strings grouped by language</param>
    /// <param name="index">Index to grab from the language arrays</param>
    /// <returns>Row of localized strings for the template.</returns>
    public static string[] GetNamesForLanguage(ReadOnlySpan<string[]> names, uint index)
    {
        var result = new string[names.Length];
        for (int i = 0; i < result.Length; i++)
        {
            var arr = names[i];
            result[i] = index < arr.Length ? arr[index] : string.Empty;
        }
        return result;
    }
}
