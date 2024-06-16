using System.Collections.Generic;

namespace PKHeX.Core;

/// <summary>
/// Generates matching <see cref="IEncounterable"/> data and relevant <see cref="LegalInfo"/> for a <see cref="PKM"/>.
/// Logic for generating possible in-game encounter data.
/// </summary>
public static class EncounterGenerator
{
    /// <summary>
    /// Generates possible <see cref="IEncounterable"/> data according to the input PKM data and legality info.
    /// </summary>
    /// <param name="pk">PKM data</param>
    /// <param name="info">Legality information</param>
    /// <returns>Possible encounters</returns>
    /// <remarks>
    /// The iterator lazily finds possible encounters. If no encounters are possible, the enumerable will be empty.
    /// </remarks>
    public static IEnumerable<IEncounterable> GetEncounters(PKM pk, LegalInfo info) => info.Generation switch
    {
        1 => EncounterGenerator12.Instance.GetEncounters(pk, info),
        2 => EncounterGenerator12.Instance.GetEncounters(pk, info),
        3 => pk.Version == GameVersion.CXD
            ? EncounterGenerator3GC.Instance.GetEncounters(pk, info)
            : EncounterGenerator3.Instance.GetEncounters(pk, info),
        4 => EncounterGenerator4.Instance.GetEncounters(pk, info),
        5 => EncounterGenerator5.Instance.GetEncounters(pk, info),
        6 => EncounterGenerator6.Instance.GetEncounters(pk, info),
        7 => EncounterGenerator7X.Instance.GetEncounters(pk, info),
        8 => EncounterGenerator8X.Instance.GetEncounters(pk, info),
        9 => EncounterGenerator9.Instance.GetEncounters(pk, info),
        _ => EncounterGeneratorDummy.Instance.GetEncounters(pk, info),
    };

    /// <summary>
    /// Gets the <see cref="IEncounterGenerator"/> for the given <see cref="GameVersion"/>.
    /// </summary>
    /// <param name="version">Original encounter version</param>
    public static IEncounterGenerator GetGenerator(GameVersion version) => GetGeneration(version, version.GetGeneration());

    /// <summary>
    /// Gets the <see cref="IEncounterGenerator"/> for the given <see cref="GameVersion"/>.
    /// </summary>
    /// <param name="version">Original encounter version</param>
    /// <param name="generation">Generation group</param>
    public static IEncounterGenerator GetGeneration(GameVersion version, byte generation) => generation switch
    {
        1 => EncounterGenerator1.Instance,
        2 => EncounterGenerator2.Instance,
        3 => version == GameVersion.CXD
            ? EncounterGenerator3GC.Instance
            : EncounterGenerator3.Instance,
        4 => EncounterGenerator4.Instance,
        5 => EncounterGenerator5.Instance,
        6 => EncounterGenerator6.Instance,
        7 => version switch
        {
            GameVersion.GP or GameVersion.GE => EncounterGenerator7GG.Instance,
            GameVersion.GO => EncounterGeneratorGO.Instance,
            _ => EncounterGenerator7.Instance,
        },
        8 => version switch
        {
            GameVersion.GO => EncounterGeneratorGO.Instance,
            GameVersion.PLA => EncounterGenerator8a.Instance,
            GameVersion.BD or GameVersion.SP => EncounterGenerator8b.Instance,
            _ => EncounterGenerator8.Instance,
        },
        9 => EncounterGenerator9.Instance,
        _ => EncounterGeneratorDummy.Instance,
    };
}
