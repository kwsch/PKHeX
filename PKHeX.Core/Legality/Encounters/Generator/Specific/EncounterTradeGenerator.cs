using System;
using System.Collections.Generic;
using System.Linq;
using static PKHeX.Core.GameVersion;

namespace PKHeX.Core
{
    public static class EncounterTradeGenerator
    {
        public static IEnumerable<EncounterTrade> GetPossible(PKM pkm, IReadOnlyList<DexLevel> chain, GameVersion gameSource)
        {
            if (pkm.Format <= 2 || pkm.VC)
                return GetPossibleVC(chain, gameSource);
            return GetPossible(chain, gameSource);
        }

        private static IEnumerable<EncounterTradeGB> GetPossibleVC(IReadOnlyList<DexLevel> chain, GameVersion game)
        {
            var table = GetTableVC(game);
            return table.Where(e => chain.Any(c => c.Species == e.Species && c.Form == 0));
        }

        private static IEnumerable<EncounterTrade> GetPossible(IReadOnlyList<DexLevel> chain, GameVersion game)
        {
            var table = GetTable(game);
            return table.Where(e => chain.Any(c => c.Species == e.Species));
        }

        public static IEnumerable<EncounterTradeGB> GetValidEncounterTradesVC(PKM pkm, IReadOnlyList<DexLevel> chain, GameVersion game)
        {
            var table = GetTableVC(game);
            foreach (var p in table)
            {
                foreach (var evo in chain)
                {
                    if (evo.Species != p.Species || evo.Form != 0)
                        continue;
                    if (p.IsMatchExact(pkm, evo))
                        yield return p;
                    break;
                }
            }
        }

        public static IEnumerable<EncounterTrade> GetValidEncounterTrades(PKM pkm, IReadOnlyList<DexLevel> chain, GameVersion game = Any)
        {
            if (game == Any)
                game = (GameVersion)pkm.Version;

            int lang = pkm.Language;
            if (lang == (int)LanguageID.UNUSED_6) // invalid language
                return Array.Empty<EncounterTrade>();
            if (lang == (int)LanguageID.Hacked && !EncounterTrade5PID.IsValidMissingLanguage(pkm)) // Japanese trades in BW have no language ID
                return Array.Empty<EncounterTrade>();

            var poss = GetPossible(chain, game);
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

        private static IEnumerable<EncounterTradeGB> GetTableVC(GameVersion game)
        {
            if (RBY.Contains(game))
                return Encounters1.TradeGift_RBY;
            if (GSC.Contains(game))
                return Encounters2.TradeGift_GSC;
            return Array.Empty<EncounterTradeGB>();
        }

        private static IEnumerable<EncounterTrade> GetTable(GameVersion game) => game switch
        {
            R or S or E => Encounters3.TradeGift_RSE,
            FR or LG => Encounters3.TradeGift_FRLG,
            D or P or Pt => Encounters4.TradeGift_DPPt,
            HG or SS => Encounters4.TradeGift_HGSS,
            B or W => Encounters5.TradeGift_BW,
            B2 or W2 => Encounters5.TradeGift_B2W2,
            X or Y => Encounters6.TradeGift_XY,
            AS or OR => Encounters6.TradeGift_AO,
            SN or MN => Encounters7.TradeGift_SM,
            US or UM => Encounters7.TradeGift_USUM,
            GP or GE => Encounters7b.TradeGift_GG,
            SW or SH => Encounters8.TradeGift_SWSH,
            _ => Array.Empty<EncounterTrade>(),
        };
    }
}
