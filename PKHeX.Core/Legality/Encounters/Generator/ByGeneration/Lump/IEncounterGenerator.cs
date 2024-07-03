using System.Collections.Generic;

namespace PKHeX.Core;

public interface IEncounterGenerator
{
    IEnumerable<IEncounterable> GetEncounters(PKM pk, EvoCriteria[] chain, LegalInfo info);

    IEnumerable<IEncounterable> GetPossible(PKM pk, EvoCriteria[] chain, GameVersion game, EncounterTypeGroup groups);
    bool CanGenerateEggs { get; }
}
