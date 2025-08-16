using System.Collections.Generic;

namespace PKHeX.Core;

public interface IEncounterGenerator
{
    IEnumerable<IEncounterable> GetEncounters(PKM pk, EvoCriteria[] chain, LegalInfo info);

    IEnumerable<IEncounterable> GetPossible(PKM pk, EvoCriteria[] chain, GameVersion version, EncounterTypeGroup groups);
    bool CanGenerateEggs { get; }
}

public interface IEncounterGeneratorSWSH
{
    IEnumerable<IEncounterable> GetEncountersSWSH(PKM pk, EvoCriteria[] chain, GameVersion version);
}
