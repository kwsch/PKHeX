using System;
using System.Collections.Generic;
using System.Linq;
using static PKHeX.Core.EncounterEvent;

namespace PKHeX.Core
{
    public static class MysteryGiftGenerator
    {
        public static IEnumerable<MysteryGift> GetPossible(PKM pkm, EvoCriteria[] chain, GameVersion game)
        {
            // Ranger Manaphy is a PGT and is not in the PCD[] for gen4. Check manually.
            int gen = pkm.Generation;
            if (gen == 4 && pkm.Species == (int) Species.Manaphy)
                yield return RangerManaphy;

            var table = GetTable(gen, game);
            var possible = table.Where(wc => chain.Any(evo => evo.Species == wc.Species));
            foreach (var enc in possible)
                yield return enc;
        }

        public static IEnumerable<MysteryGift> GetValidGifts(PKM pkm, EvoCriteria[] chain, GameVersion game)
        {
            int gen = pkm.Generation;
            if (pkm.IsEgg && pkm.Format != gen) // transferred
                return Array.Empty<MysteryGift>();

            if (gen == 4) // check for Manaphy gift
                return GetMatchingPCD(pkm, MGDB_G4, chain);
            var table = GetTable(gen, game);
            return GetMatchingGifts(pkm, table, chain);
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

        private static IEnumerable<MysteryGift> GetMatchingPCD(PKM pkm, IReadOnlyCollection<PCD> DB, EvoCriteria[] chain)
        {
            if (PGT.IsRangerManaphy(pkm))
            {
                yield return RangerManaphy;
                yield break;
            }

            foreach (var g in GetMatchingGifts(pkm, DB, chain))
                yield return g;
        }

        private static IEnumerable<MysteryGift> GetMatchingGifts(PKM pkm, IReadOnlyCollection<MysteryGift> DB, EvoCriteria[] chain)
        {
            foreach (var mg in DB)
            {
                foreach (var evo in chain)
                {
                    if (evo.Species != mg.Species)
                        continue;
                    if (mg.IsMatchExact(pkm, evo))
                        yield return mg;
                }
            }
        }

        // Utility
        private static readonly PGT RangerManaphy = new() {Data = {[0] = 7, [8] = 1}};
    }
}
