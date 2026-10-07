using System.Collections.Generic;
using static PKHeX.Core.GameVersion;

namespace PKHeX.Core;

/// <summary>Matches obtainable FR/LG Switch encounters and their later evolutions without visiting Bank-era games.</summary>
public static class EncounterGenerator3HOME
{
    private const EncounterTypeGroup Groups = EncounterTypeGroup.Egg | EncounterTypeGroup.Static | EncounterTypeGroup.Trade | EncounterTypeGroup.Slot;

    public static IEnumerable<IEncounterable> GetEncounters(PKM pk, LegalInfo info)
    {
        var version = EncounterTransfer3HOME.GetOriginVersion(pk);
        var originalVersion = version == FR_NX ? FR : LG;
        // Keep the full HOME evolution chain, then restrict the species actually transferred to those obtainable in FR/LG.
        var origin = new EvolutionOrigin(pk.Species, EntityContext.Gen8, 8, pk.MetLevel, pk.CurrentLevel);
        var chain = EvolutionChain.GetOriginChain(pk, origin, discard: false);
        foreach (var evo in chain)
        {
            if (evo.Species is 0 or > Legal.MaxSpeciesID_3 || Legal.IsForeignFRLG(evo.Species) ||
                !PersonalTable.FR.IsPresentInGame(evo.Species, evo.Form))
                continue;

            var transferred = pk.Clone();
            transferred.Version = originalVersion;
            transferred.Species = evo.Species;
            transferred.Form = evo.Form;
            transferred.CurrentLevel = pk.MetLevel;
            transferred.EggLocation = 0;
            var nativeChain = EncounterOrigin.GetOriginChain(transferred, 3, EntityContext.Gen3);
            foreach (var enc in EncounterGenerator3.Instance.GetPossible(transferred, nativeChain, originalVersion, Groups))
            {
                // Breeding cannot introduce species that require R/S/E, Colosseum/XD, or old event distributions.
                if (Legal.IsForeignFRLG(enc.Species))
                    continue;
                foreach (var native in nativeChain)
                {
                    if (native.Species != enc.Species || native.LevelMax < enc.LevelMin)
                        continue;
                    if (enc is IEncounterMatch match && !match.IsMatchExact(transferred, native))
                        continue;

                    var source = transferred.Clone();
                    source.Species = enc.Species;
                    source.Form = enc.Form;
                    var pidiv = MethodFinder.Analyze(source);
                    if (enc is not IEncounterEgg && enc is IRandomCorrelation rng && rng.IsCompatible(pidiv.Type, source) == RandomCorrelationRating.Mismatch)
                        continue;
                    if (enc is IEncounterEgg && !Daycare3.IsValidProcPID(source.EncryptionConstant, originalVersion))
                        continue;

                    var transfer = new EncounterTransfer3HOME(enc, version, evo.Species, evo.Form, pk.MetLevel);
                    if (!transfer.IsMatchExact(pk, evo))
                        continue;
                    info.PIDIV = pidiv;
                    yield return transfer;
                    break;
                }
            }
        }
    }
}