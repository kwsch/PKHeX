using System;
using System.Collections.Generic;
using System.Linq;
using static PKHeX.Core.EncounterEvent;

namespace PKHeX.Core
{
    public static class MysteryGiftGenerator
    {
        public static IEnumerable<MysteryGift> GetPossible(PKM pkm, IReadOnlyList<DexLevel> chain)
        {
            // Ranger Manaphy is a PGT and is not in the PCD[] for gen4. Check manually.
            int gen = pkm.Generation;
            if (gen == 4 && pkm.Species == (int) Species.Manaphy)
                yield return RangerManaphy;

            var table = GetTable(gen, pkm);
            var possible = table.Where(wc => chain.Any(dl => dl.Species == wc.Species));
            foreach (var enc in possible)
                yield return enc;
        }

        public static IEnumerable<MysteryGift> GetValidGifts(PKM pkm, IReadOnlyList<DexLevel> chain)
        {
            int gen = pkm.Generation;
            if (pkm.IsEgg && pkm.Format != gen) // transferred
                return Array.Empty<MysteryGift>();

            if (gen == 4) // check for Manaphy gift
                return GetMatchingPCD(pkm, MGDB_G4, chain);
            var table = GetTable(gen, pkm);
            return GetMatchingGifts(pkm, table, chain);
        }

        private static IReadOnlyCollection<MysteryGift> GetTable(int generation, PKM pkm) => generation switch
        {
            3 => MGDB_G3,
            4 => MGDB_G4,
            5 => MGDB_G5,
            6 => MGDB_G6,
            7 => pkm.LGPE ? MGDB_G7GG : MGDB_G7,
            8 => MGDB_G8,
            _ => Array.Empty<MysteryGift>()
        };

        private static IEnumerable<MysteryGift> GetMatchingPCD(PKM pkm, IReadOnlyCollection<PCD> DB, IReadOnlyList<DexLevel> chain)
        {
            if (PGT.IsRangerManaphy(pkm))
            {
                yield return RangerManaphy;
                yield break;
            }

            foreach (var g in GetMatchingGifts(pkm, DB, chain))
                yield return g;
        }

        private static IEnumerable<MysteryGift> GetMatchingGifts(PKM pkm, IReadOnlyCollection<MysteryGift> DB, IReadOnlyList<DexLevel> chain)
        {
            foreach (var mg in DB)
            {
                foreach (var dl in chain)
                {
                    if (dl.Species != mg.Species)
                        continue;
                    if (mg.IsMatchExact(pkm, dl))
                        yield return mg;
                }
            }
        }

        // Utility
        private static readonly PGT RangerManaphy = new() {Data = {[0] = 7, [8] = 1}};
    }
}
