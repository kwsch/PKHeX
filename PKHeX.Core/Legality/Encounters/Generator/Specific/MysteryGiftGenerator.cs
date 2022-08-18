using System;
using System.Collections.Generic;
using static PKHeX.Core.EncounterEvent;

namespace PKHeX.Core;

public static class MysteryGiftGenerator
{
    public static IEnumerable<MysteryGift> GetPossible(PKM pk, EvoCriteria[] chain, GameVersion game)
    {
        // Ranger Manaphy is a PGT and is not in the PCD[] for gen4. Check manually.
        int gen = pk.Generation;
        if (gen == 4 && pk.Species == (int) Species.Manaphy)
            yield return RangerManaphy;

        var table = GetTable(gen, game);
        foreach (var enc in table)
        {
            foreach (var evo in chain)
            {
                if (evo.Species != enc.Species)
                    continue;
                yield return enc;
                break;
            }
        }
    }

    public static IEnumerable<MysteryGift> GetValidGifts(PKM pk, EvoCriteria[] chain, GameVersion game)
    {
        int gen = pk.Generation;
        if (pk.IsEgg && pk.Format != gen) // transferred
            return Array.Empty<MysteryGift>();

        if (gen == 4) // check for Manaphy gift
            return GetMatchingPCD(pk, MGDB_G4, chain);
        var table = GetTable(gen, game);
        return GetMatchingGifts(pk, table, chain);
    }

    private static IReadOnlyCollection<MysteryGift> GetTable(int generation, GameVersion game) => generation switch
    {
        3 => MGDB_G3,
        4 => MGDB_G4,
        5 => MGDB_G5,
        6 => MGDB_G6,
        7 => game is GameVersion.GP or GameVersion.GE ? MGDB_G7GG : MGDB_G7,
        8 => game switch
        {
            GameVersion.BD or GameVersion.SP => MGDB_G8B,
            GameVersion.PLA => MGDB_G8A,
            _ => MGDB_G8,
        },
        _ => Array.Empty<MysteryGift>(),
    };

    private static IEnumerable<MysteryGift> GetMatchingPCD(PKM pk, IReadOnlyCollection<PCD> table, EvoCriteria[] chain)
    {
        if (PGT.IsRangerManaphy(pk))
        {
            yield return RangerManaphy;
            yield break;
        }

        foreach (var g in GetMatchingGifts(pk, table, chain))
            yield return g;
    }

    private static IEnumerable<MysteryGift> GetMatchingGifts(PKM pk, IReadOnlyCollection<MysteryGift> table, EvoCriteria[] chain)
    {
        foreach (var mg in table)
        {
            foreach (var evo in chain)
            {
                if (evo.Species != mg.Species)
                    continue;
                if (mg.IsMatchExact(pk, evo))
                    yield return mg;
            }
        }
    }

    // Utility
    private static readonly PGT RangerManaphy = new() {Data = {[0] = 7, [8] = 1}};
}
