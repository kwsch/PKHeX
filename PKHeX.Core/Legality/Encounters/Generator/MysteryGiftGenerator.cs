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
            var table = GetTable(pkm.GenNumber, pkm);
            return table.Where(wc => vs.Any(dl => dl.Species == wc.Species));
        }

        public static IEnumerable<MysteryGift> GetValidGifts(PKM pkm)
        {
            int gen = pkm.GenNumber;
            if (pkm.IsEgg && pkm.Format != gen) // transferred
                return Enumerable.Empty<MysteryGift>();

            if (gen == 4) // check for manaphy gift
                return GetMatchingPCD(pkm, MGDB_G4);
            var table = GetTable(pkm.GenNumber, pkm);
            return GetMatchingGifts(pkm, table);
        }

        private static IEnumerable<MysteryGift> GetTable(int generation, PKM pkm)
        {
            switch (generation)
            {
                case 3: return MGDB_G3;
                case 4: return MGDB_G4;
                case 5: return MGDB_G5;
                case 6: return MGDB_G6;
                case 7:
                    if (pkm.GG)
                        return MGDB_G7GG;
                    return MGDB_G7;
                default: return Enumerable.Empty<MysteryGift>();
            }
        }

        private static IEnumerable<MysteryGift> GetMatchingPCD(PKM pkm, IEnumerable<PCD> DB)
        {
            if (IsRangerManaphy(pkm))
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
                var result = mg.IsMatch(pkm, vs);
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

        private static bool IsRangerManaphy(PKM pkm)
        {
            var egg = pkm.Egg_Location;
            if (!pkm.IsEgg) // Link Trade Egg or Ranger
                return egg == Locations.LinkTrade4 || egg == Locations.Ranger4;
            if (egg != Locations.Ranger4)
                return false;

            if (pkm.Language == (int)LanguageID.Korean) // never korean
                return false;

            var met = pkm.Met_Location;
            return met == Locations.LinkTrade4 || met == 0;
        }
    }
}
