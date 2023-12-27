using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace PKHeX.Core;

/// <summary>
/// Encounter data from <see cref="GameVersion.GO"/>, which has multiple generations of origin.
/// </summary>
#if !DEBUG
internal static class EncountersGO
{
    internal const int MAX_LEVEL = 50;

    internal static readonly EncounterArea7g[] SlotsGO_GG = EncounterArea7g.GetArea(EncounterUtil.Get("go_lgpe", "go"u8));
    internal static readonly EncounterArea8g[] SlotsGO = EncounterArea8g.GetArea(EncounterUtil.Get("go_home", "go"u8));
}
#else
public static class EncountersGO
{
    internal const int MAX_LEVEL = 50;

    internal static EncounterArea7g[] SlotsGO_GG = EncounterArea7g.GetArea(Get("go_lgpe", "go"u8));
    internal static EncounterArea8g[] SlotsGO = EncounterArea8g.GetArea(Get("go_home", "go"u8));

    public static void Reload()
    {
        SlotsGO_GG = EncounterArea7g.GetArea(Get("go_lgpe", "go"u8));
        SlotsGO = EncounterArea8g.GetArea(Get("go_home", "go"u8));
    }

    private static BinLinkerAccessor Get([ConstantExpected] string resource, [Length(2, 2)] ReadOnlySpan<byte> ident)
    {
        var name = $"encounter_{resource}.pkl";
        var data = System.IO.File.Exists(name) ? System.IO.File.ReadAllBytes(name) : Util.GetBinaryResource(name);
        return BinLinkerAccessor.Get(data, ident);
    }
}
#endif
