namespace PKHeX.Core;

/// <summary>
/// Encounter data from <see cref="GameVersion.GO"/>, which has multiple generations of origin.
/// </summary>
#if !DEBUG
internal static class EncountersGO
{
    internal const int MAX_LEVEL = 50;

    internal static readonly EncounterArea7g[] SlotsGO_GG = EncounterArea7g.GetArea(EncounterUtil.Get("go_lgpe", "go"));
    internal static readonly EncounterArea8g[] SlotsGO = EncounterArea8g.GetArea(EncounterUtil.Get("go_home", "go"));
}
#else
public static class EncountersGO
{
    internal const int MAX_LEVEL = 50;

    internal static EncounterArea7g[] SlotsGO_GG = EncounterArea7g.GetArea(Get("go_lgpe", "go"));
    internal static EncounterArea8g[] SlotsGO = EncounterArea8g.GetArea(Get("go_home", "go"));

    public static void Reload()
    {
        SlotsGO_GG = EncounterArea7g.GetArea(Get("go_lgpe", "go"));
        SlotsGO = EncounterArea8g.GetArea(Get("go_home", "go"));
    }

    private static BinLinkerAccessor Get(string resource, string ident)
    {
        var name = $"encounter_{resource}.pkl";
        var data = System.IO.File.Exists(name) ? System.IO.File.ReadAllBytes(name) : Util.GetBinaryResource(name);
        return BinLinkerAccessor.Get(data, ident);
    }
}
#endif
