using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core;

/// <summary>
/// Provides a summary for <see cref="IEncounterTemplate"/> objects.
/// </summary>
public sealed record EncounterSummary
{
    private readonly GameVersion Version;
    private readonly string LocationName;

    private EncounterSummary(IEncounterTemplate z)
    {
        Version = z.Version;
        LocationName = GetLocationName(z);
    }

    private static string GetLocationName(IEncounterTemplate z)
    {
        var generation = z.Generation;
        var version = z.Version;
        if (generation == 0 && version > 0)
            generation = version.GetGeneration();

        if (z is not ILocation l)
            return $"[Gen{generation}]\t";
        var loc = l.GetEncounterLocation(generation, version);

        if (string.IsNullOrWhiteSpace(loc))
            return $"[Gen{generation}]\t";
        return $"[Gen{generation}]\t{loc}: ";
    }

    public static IEnumerable<string> SummarizeGroup(IEnumerable<IEncounterTemplate> items, string header = "", bool advanced = false)
    {
        if (!string.IsNullOrWhiteSpace(header))
            yield return $"=={header}==";
        var summaries = advanced ? GetSummaries(items) : items.Select(z => new EncounterSummary(z));
        var objs = summaries.GroupBy(z => z.LocationName);
        foreach (var g in objs)
            yield return $"\t{g.Key}{string.Join(", ", g.Select(z => z.Version).Distinct())}";
    }

    public static IEnumerable<EncounterSummary> GetSummaries(IEnumerable<IEncounterTemplate> items) => items.Select(GetSummary);

    private static EncounterSummary GetSummary(IEncounterTemplate item) => item switch
    {
        _ => new EncounterSummary(item),
    };
}
