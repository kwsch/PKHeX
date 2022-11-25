using System;
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
        1 => EncounterGenerator12.GetEncounters12(pk, info),
        2 => EncounterGenerator12.GetEncounters12(pk, info),
        3 => EncounterGenerator3.GetEncounters(pk, info),
        4 => EncounterGenerator4.GetEncounters(pk, info),
        5 => EncounterGenerator5.GetEncounters(pk),
        6 => EncounterGenerator6.GetEncounters(pk),
        7 => EncounterGenerator7.GetEncounters(pk),
        8 => EncounterGenerator8.GetEncounters(pk),
        9 => EncounterGenerator9.GetEncounters(pk),
        _ => Array.Empty<IEncounterable>(),
    };
}
