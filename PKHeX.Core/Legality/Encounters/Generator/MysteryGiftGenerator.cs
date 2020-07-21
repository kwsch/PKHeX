using System;
using System.Collections.Generic;
using System.Linq;
using static PKHeX.Core.EncounterEvent;

namespace PKHeX.Core
{
    public static class MysteryGiftGenerator
    {
        public static IEnumerable<MysteryGift> GetPossible(PKM pkm)
        {
            var chain = EvolutionChain.GetOriginChain(pkm);
            return GetPossible(pkm, chain);
        }

        public static IEnumerable<MysteryGift> GetPossible(PKM pkm, IReadOnlyList<DexLevel> chain)
        {
            // Ranger Manaphy is a PGT and is not in the PCD[] for gen4. Check manually.
            int gen = pkm.GenNumber;
            if (gen == 4 && pkm.Species == (int) Species.Manaphy)
                yield return RangerManaphy;

            var table = GetTable(gen, pkm);
            var possible = table.Where(wc => chain.Any(dl => dl.Species == wc.Species));
            foreach (var enc in possible)
                yield return enc;
        }

        public static IEnumerable<MysteryGift> GetValidGifts(PKM pkm)
        {
            int gen = pkm.GenNumber;
            if (pkm.IsEgg && pkm.Format != gen) // transferred
                return Array.Empty<MysteryGift>();

            if (gen == 4) // check for Manaphy gift
                return GetMatchingPCD(pkm, MGDB_G4);
            var table = GetTable(gen, pkm);
            return GetMatchingGifts(pkm, table);
        }

        private static IReadOnlyList<MysteryGift> GetTable(int generation, PKM pkm)
        {
            return generation switch
            {
                3 => MGDB_G3,
                4 => MGDB_G4,
                5 => MGDB_G5,
                6 => MGDB_G6,
                7 => pkm.LGPE ? (IReadOnlyList<MysteryGift>)MGDB_G7GG : MGDB_G7,
                8 => MGDB_G8,
                _ => Array.Empty<MysteryGift>()
            };
        }

        private static IEnumerable<MysteryGift> GetMatchingPCD(PKM pkm, IReadOnlyList<PCD> DB)
        {
            if (PGT.IsRangerManaphy(pkm))
            {
                yield return RangerManaphy;
                yield break;
            }

            foreach (var g in GetMatchingGifts(pkm, DB))
                yield return g;
        }

        private static IEnumerable<MysteryGift> GetMatchingGifts(PKM pkm, IReadOnlyList<MysteryGift> DB)
        {
            var chain = EvolutionChain.GetOriginChain(pkm);
            return GetMatchingGifts(pkm, DB, chain);
        }

        private static IEnumerable<MysteryGift> GetMatchingGifts(PKM pkm, IReadOnlyList<MysteryGift> DB, IReadOnlyList<DexLevel> chain)
        {
            var deferred = new List<MysteryGift>();
            foreach (var mg in DB)
            {
                foreach (var dl in chain)
                {
                    if (dl.Species != mg.Species)
                        continue;
                    var result = mg.IsMatch(pkm, dl);
                    if (result == EncounterMatchRating.Match)
                        yield return mg;
                    else if (result == EncounterMatchRating.Deferred)
                        deferred.Add(mg);
                    break;
                }
            }
            foreach (var z in deferred)
                yield return z;
        }

        // Utility
        private static readonly PGT RangerManaphy = new PGT {Data = {[0] = 7, [8] = 1}};
    }
}
