using System;
using System.Collections.Generic;

namespace PKHeX.Core
{
    /// <summary>
    /// Generates matching <see cref="IEncounterable"/> data and relevant <see cref="LegalInfo"/> for a <see cref="PKM"/>.
    /// Logic for generating possible in-game encounter data.
    /// </summary>
    public static class EncounterGenerator
    {
        /// <summary>
        /// Generates possible <see cref="IEncounterable"/> data according to the input PKM data and legality info.
        /// </summary>
        /// <param name="pkm">PKM data</param>
        /// <param name="info">Legality information</param>
        /// <returns>Possible encounters</returns>
        /// <remarks>
        /// The iterator lazily finds possible encounters. If no encounters are possible, the enumerable will be empty.
        /// </remarks>
        public static IEnumerable<IEncounterable> GetEncounters(PKM pkm, LegalInfo info) => info.Generation switch
        {
            1 => EncounterGenerator12.GetEncounters12(pkm, info),
            2 => EncounterGenerator12.GetEncounters12(pkm, info),
            3 => EncounterGenerator3.GetEncounters(pkm, info),
            4 => EncounterGenerator4.GetEncounters(pkm, info),
            5 => EncounterGenerator5.GetEncounters(pkm),
            6 => EncounterGenerator6.GetEncounters(pkm),
            7 => EncounterGenerator7.GetEncounters(pkm),
            8 => EncounterGenerator8.GetEncounters(pkm),
            _ => Array.Empty<IEncounterable>(),
        };
    }
}
