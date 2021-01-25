using System;
using System.Collections.Generic;
using System.Linq;
using static PKHeX.Core.Legal;

namespace PKHeX.Core
{
    public static class EncounterTradeGenerator
    {
        public static IEnumerable<EncounterTrade> GetPossible(PKM pkm, IReadOnlyList<DexLevel> chain, GameVersion gameSource = GameVersion.Any)
        {
            if (gameSource == GameVersion.Any)
                gameSource = (GameVersion)pkm.Version;

            if (pkm.VC || pkm.Format <= 2)
                return GetPossibleVC(chain, gameSource);
            return GetPossibleNonVC(pkm, chain, gameSource);
        }

        public static IEnumerable<EncounterTrade> GetValidEncounterTrades(PKM pkm, IReadOnlyList<DexLevel> chain, GameVersion gameSource = GameVersion.Any)
        {
            if (GetIsFromGB(pkm))
                return GetValidEncounterTradesVC(pkm, chain, gameSource);

            int lang = pkm.Language;
            if (lang == (int)LanguageID.UNUSED_6) // invalid language
                return Array.Empty<EncounterTrade>();
            if (lang == (int)LanguageID.Hacked && !IsValidMissingLanguage(pkm)) // Japanese trades in BW have no language ID
                return Array.Empty<EncounterTrade>();

            var poss = GetPossibleNonVC(pkm, chain, gameSource);
            return GetValidEncounterTrades(pkm, chain, poss);
        }

        private static IEnumerable<EncounterTrade> GetValidEncounterTrades(PKM pkm, IReadOnlyList<DexLevel> chain, IEnumerable<EncounterTrade> poss)
        {
            foreach (var p in poss)
            {
                foreach (var evo in chain)
                {
                    if (evo.Species != p.Species)
                        continue;
                    if (p.IsMatchExact(pkm, evo))
                        yield return p;
                    break;
                }
            }
        }

        private static IEnumerable<EncounterTrade> GetPossibleNonVC(PKM pkm, IReadOnlyList<DexLevel> chain, GameVersion gameSource = GameVersion.Any)
        {
            if (gameSource == GameVersion.Any)
                gameSource = (GameVersion)pkm.Version;

            if (GetIsFromGB(pkm))
                return GetValidEncounterTradesVC(pkm, chain, gameSource);

            var table = GetEncounterTradeTable(pkm);
            return table.Where(f => chain.Any(r => r.Species == f.Species));
        }

        private static IEnumerable<EncounterTradeGB> GetPossibleVC(IReadOnlyList<DexLevel> chain, GameVersion gameSource = GameVersion.Any)
        {
            var table = GetEncounterTradeTableVC(gameSource);
            return table.Where(f => chain.Any(r => r.Species == f.Species && r.Form == 0));
        }

        private static IEnumerable<EncounterTradeGB> GetEncounterTradeTableVC(GameVersion gameSource)
        {
            if (GameVersion.RBY.Contains(gameSource))
                return Encounters1.TradeGift_RBY;
            if (GameVersion.GSC.Contains(gameSource))
                return Encounters2.TradeGift_GSC;
            return Array.Empty<EncounterTradeGB>();
        }

        private static IEnumerable<EncounterTrade> GetEncounterTradeTable(PKM pkm) => pkm.Generation switch
        {
            3 => (pkm.FRLG ? Encounters3.TradeGift_FRLG : Encounters3.TradeGift_RSE),
            4 => (pkm.HGSS ? Encounters4.TradeGift_HGSS : Encounters4.TradeGift_DPPt),
            5 => (pkm.B2W2 ? Encounters5.TradeGift_B2W2 : Encounters5.TradeGift_BW),
            6 => (pkm.XY ? Encounters6.TradeGift_XY : Encounters6.TradeGift_AO),
            7 => (pkm.LGPE ? Encounters7b.TradeGift_GG : pkm.SM ? Encounters7.TradeGift_SM : Encounters7.TradeGift_USUM),
            8 => Encounters8.TradeGift_SWSH,
            _ => Array.Empty<EncounterTrade>(),
        };

        private static IEnumerable<EncounterTradeGB> GetValidEncounterTradesVC(PKM pkm, IReadOnlyList<DexLevel> chain, GameVersion gameSource)
        {
            var table = GetEncounterTradeTableVC(gameSource);
            foreach (var t in table)
            {
                foreach (var dl in chain)
                {
                    if (dl.Species != pkm.Species || dl.Form != 0)
                        continue;
                    if (!t.IsMatchExact(pkm, dl))
                        continue;
                    yield return t;
                }
            }
        }

        private static bool GetIsFromGB(PKM pkm) => pkm.VC || pkm.Format <= 2;
    }
}
