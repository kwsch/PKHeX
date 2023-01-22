using System;
using System.Collections.Generic;

namespace PKHeX.Core;

internal sealed class EncounterGeneratorDummy : IEncounterGenerator
{
    public static readonly EncounterGeneratorDummy Instance = new();

    public IEnumerable<IEncounterable> GetPossible(PKM pk, EvoCriteria[] chain, GameVersion game, EncounterTypeGroup groups)
    {
        return Array.Empty<IEncounterable>();
    }

    public IEnumerable<IEncounterable> GetEncounters(PKM pk, EvoCriteria[] chain, LegalInfo info)
    {
        return Array.Empty<IEncounterable>();
    }

    public IEnumerable<IEncounterable> GetEncounters(PKM _, LegalInfo __)
    {
        return Array.Empty<IEncounterable>();
    }
}
