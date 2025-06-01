#if !DEBUG
namespace PKHeX.Core;

/// <summary>
/// Encounter data from <see cref="GameVersion.GO"/>, which has multiple generations of origin.
/// </summary>
internal static class EncountersGO
{
    internal const byte MAX_LEVEL = 50;

    internal static readonly EncounterArea7g[] SlotsGO_GG = EncounterArea7g.GetArea(EncounterUtil.Get("go_lgpe", "go"u8));
    internal static readonly EncounterArea8g[] SlotsGO = EncounterArea8g.GetArea(EncounterUtil.Get("go_home", "go"u8));
}
#else
using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace PKHeX.Core;
public static class EncountersGO
{
    internal const byte MAX_LEVEL = 50;

    internal static EncounterArea7g[] SlotsGO_GG = EncounterArea7g.GetArea(Get("go_lgpe", "go"u8));
    internal static EncounterArea8g[] SlotsGO = EncounterArea8g.GetArea(Get("go_home", "go"u8));

    /// <summary>
    /// Debug method to reload the encounter data from the binary resources next to the executable.
    /// </summary>
    public static void Reload()
    {
        SlotsGO_GG = EncounterArea7g.GetArea(Get("go_lgpe", "go"u8));
        SlotsGO = EncounterArea8g.GetArea(Get("go_home", "go"u8));
    }

    private static BinLinkerAccessor Get([ConstantExpected] string resource, [Length(2, 2)] ReadOnlySpan<byte> ident)
    {
        var exePath = Path.GetDirectoryName(Environment.ProcessPath)!;
        var name = Path.Combine(exePath, $"encounter_{resource}.pkl");
        var data = File.Exists(name) ? File.ReadAllBytes(name) : Util.GetBinaryResource(name);
        return BinLinkerAccessor.Get(data, ident);
    }
}
#endif
