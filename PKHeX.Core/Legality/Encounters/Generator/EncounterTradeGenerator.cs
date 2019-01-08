using System.Collections.Generic;
using System.Linq;
using static PKHeX.Core.Legal;

namespace PKHeX.Core
{
    public static class EncounterTradeGenerator
    {
        public static IEnumerable<EncounterTrade> GetPossible(PKM pkm, GameVersion gameSource = GameVersion.Any)
        {
            var p = EvolutionChain.GetValidPreEvolutions(pkm);
            return GetPossible(pkm, p, gameSource);
        }

        public static IEnumerable<EncounterTrade> GetPossible(PKM pkm, IReadOnlyList<DexLevel> vs, GameVersion gameSource = GameVersion.Any)
        {
            if (gameSource == GameVersion.Any)
                gameSource = (GameVersion)pkm.Version;

            if (pkm.VC || pkm.Format <= 2)
                return GetPossibleVC(vs, gameSource);
            return GetPossibleNonVC(pkm, vs, gameSource);
        }

        public static IEnumerable<EncounterTrade> GetValidEncounterTrades(PKM pkm, GameVersion gameSource = GameVersion.Any)
        {
            var p = EvolutionChain.GetValidPreEvolutions(pkm);
            return GetValidEncounterTrades(pkm, p, gameSource);
        }

        public static IEnumerable<EncounterTrade> GetValidEncounterTrades(PKM pkm, IReadOnlyList<DexLevel> p, GameVersion gameSource = GameVersion.Any)
        {
            if (GetIsFromGB(pkm))
                return GetValidEncounterTradesVC(pkm, p, gameSource);

            int lvl = IsNotTrade(pkm);
            if (lvl <= 0)
                return Enumerable.Empty<EncounterTrade>();

            var poss = GetPossibleNonVC(pkm, p, gameSource);
            return poss.Where(z => z.IsMatch(pkm, lvl));
        }

        private static IEnumerable<EncounterTrade> GetPossibleNonVC(PKM pkm, IReadOnlyList<DexLevel> p, GameVersion gameSource = GameVersion.Any)
        {
            if (gameSource == GameVersion.Any)
                gameSource = (GameVersion)pkm.Version;

            if (pkm.VC || pkm.Format <= 2)
                return GetValidEncounterTradesVC(pkm, p, gameSource);

            var table = GetEncounterTradeTable(pkm);
            return table?.Where(f => p.Any(r => r.Species == f.Species)) ?? Enumerable.Empty<EncounterTrade>();
        }

        private static IEnumerable<EncounterTrade> GetPossibleVC(IReadOnlyList<DexLevel> p, GameVersion gameSource = GameVersion.Any)
        {
            var table = GetEncounterTradeTableVC(gameSource);
            return table.Where(f => p.Any(r => r.Species == f.Species));
        }

        private static IEnumerable<EncounterTrade> GetEncounterTradeTableVC(GameVersion gameSource)
        {
            if (GameVersion.RBY.Contains(gameSource))
                return !ParseSettings.AllowGen1Tradeback ? Encounters1.TradeGift_RBY_NoTradeback : Encounters1.TradeGift_RBY_Tradeback;
            if (GameVersion.GSC.Contains(gameSource))
                return Encounters2.TradeGift_GSC;
            return null;
        }

        private static IEnumerable<EncounterTrade> GetEncounterTradeTable(PKM pkm)
        {
            switch (pkm.GenNumber)
            {
                case 3: return pkm.FRLG ? Encounters3.TradeGift_FRLG : Encounters3.TradeGift_RSE;
                case 4: return pkm.HGSS ? Encounters4.TradeGift_HGSS : Encounters4.TradeGift_DPPt;
                case 5: return pkm.B2W2 ? Encounters5.TradeGift_B2W2 : Encounters5.TradeGift_BW;
                case 6: return pkm.XY ? Encounters6.TradeGift_XY : Encounters6.TradeGift_AO;
                case 7: return pkm.GG ? Encounters7b.TradeGift_GG : pkm.SM ? Encounters7.TradeGift_SM : Encounters7.TradeGift_USUM;
            }
            return null;
        }

        private static IEnumerable<EncounterTrade> GetValidEncounterTradesVC(PKM pkm, IReadOnlyList<DexLevel> p, GameVersion gameSource)
        {
            var poss = GetPossibleVC(p, gameSource);
            if (gameSource == GameVersion.RBY)
                return poss.Where(z => z.IsMatchVC1(pkm));
            return poss.Where(z => z.IsMatchVC2(pkm));
        }

        private static bool GetIsFromGB(PKM pkm) => pkm.VC || pkm.Format <= 2;

        private static int IsNotTrade(PKM pkm)
        {
            int lang = pkm.Language;
            if (lang == (int)LanguageID.UNUSED_6) // invalid language
                return 0;
            if (lang == (int)LanguageID.Hacked && !IsValidMissingLanguage(pkm)) // Japanese trades in BW have no language ID
                return 0;

            return GetMinLevelEncounter(pkm);
        }
    }
}
