using System.Collections.Generic;
using System.Linq;
using static PKHeX.Core.EncounterEvent;

namespace PKHeX.Core
{
    public static class MysteryGiftGenerator
    {
        public static IEnumerable<MysteryGift> GetPossible(PKM pkm)
        {
            int maxSpecies = Legal.GetMaxSpeciesOrigin(pkm.Format);
            var vs = EvolutionChain.GetValidPreEvolutions(pkm, maxSpecies);
            return GetPossible(pkm, vs);
        }

        public static IEnumerable<MysteryGift> GetPossible(PKM pkm, IReadOnlyList<DexLevel> vs)
        {
            // Ranger Manaphy is a PGT and is not in the PCD[] for gen4. Check manually.
            int gen = pkm.GenNumber;
            if (gen == 4 && pkm.Species == (int) Species.Manaphy)
                yield return RangerManaphy;

            var table = GetTable(gen, pkm);
            var possible = table.Where(wc => vs.Any(dl => dl.Species == wc.Species));
            foreach (var enc in possible)
                yield return enc;
        }

        public static IEnumerable<MysteryGift> GetValidGifts(PKM pkm)
        {
            int gen = pkm.GenNumber;
            if (pkm.IsEgg && pkm.Format != gen) // transferred
                return Enumerable.Empty<MysteryGift>();

            if (gen == 4) // check for manaphy gift
                return GetMatchingPCD(pkm, MGDB_G4);
            var table = GetTable(gen, pkm);
            return GetMatchingGifts(pkm, table);
        }

        private static IEnumerable<MysteryGift> GetTable(int generation, PKM pkm)
        {
            return generation switch
            {
                3 => MGDB_G3,
                4 => MGDB_G4,
                5 => MGDB_G5,
                6 => MGDB_G6,
                7 => (pkm.GG ? (IEnumerable<MysteryGift>)MGDB_G7GG : MGDB_G7),
                8 => MGDB_G8,
                _ => Enumerable.Empty<MysteryGift>()
            };
        }

        private static IEnumerable<MysteryGift> GetMatchingPCD(PKM pkm, IEnumerable<PCD> DB)
        {
            if (PGT.IsRangerManaphy(pkm))
            {
                yield return RangerManaphy;
                yield break;
            }

            foreach (var g in GetMatchingGifts(pkm, DB))
                yield return g;
        }

        private static IEnumerable<MysteryGift> GetMatchingGifts(PKM pkm, IEnumerable<MysteryGift> DB)
        {
            var vs = EvolutionChain.GetValidPreEvolutions(pkm);
            return GetMatchingGifts(pkm, DB, vs);
        }

        private static IEnumerable<MysteryGift> GetMatchingGifts(PKM pkm, IEnumerable<MysteryGift> DB, IReadOnlyList<DexLevel> vs)
        {
            var deferred = new List<MysteryGift>();
            var gifts = DB.Where(wc => vs.Any(dl => dl.Species == wc.Species));
            foreach (var mg in gifts)
            {
                var result = mg.IsMatch(pkm);
                if (result == EncounterMatchRating.None)
                    continue;
                if (result == EncounterMatchRating.Match)
                    yield return mg;
                else if (result == EncounterMatchRating.Deferred)
                    deferred.Add(mg);
            }
            foreach (var z in deferred)
                yield return z;
        }

        // Utility
        private static readonly PGT RangerManaphy = new PGT {Data = {[0] = 7, [8] = 1}};
    }
}
