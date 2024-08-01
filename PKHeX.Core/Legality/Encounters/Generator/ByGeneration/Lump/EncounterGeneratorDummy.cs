using System.Collections.Generic;

namespace PKHeX.Core;

internal sealed class EncounterGeneratorDummy : IEncounterGenerator
{
    public static readonly EncounterGeneratorDummy Instance = new();
    public bool CanGenerateEggs => false;

    public IEnumerable<IEncounterable> GetPossible(PKM pk, EvoCriteria[] chain, GameVersion game, EncounterTypeGroup groups) => [];
    public IEnumerable<IEncounterable> GetEncounters(PKM pk, EvoCriteria[] chain, LegalInfo info) => [];
    public IEnumerable<IEncounterable> GetEncounters(PKM _, LegalInfo __) => [];
}
