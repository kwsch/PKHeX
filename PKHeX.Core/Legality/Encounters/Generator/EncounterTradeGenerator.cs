using System;
using System.Collections.Generic;
using System.Linq;
using static PKHeX.Core.Legal;

namespace PKHeX.Core
{
    public static class EncounterTradeGenerator
    {
        public static IEnumerable<EncounterTrade> GetPossible(PKM pkm, GameVersion gameSource = GameVersion.Any)
        {
            if (gameSource == GameVersion.Any)
                gameSource = (GameVersion)pkm.Version;

            if (pkm.VC || pkm.Format <= 2)
                return GetPossibleVC(pkm, gameSource);
            return GetPossibleNonVC(pkm, gameSource);
        }
        private static IEnumerable<EncounterTrade> GetPossibleNonVC(PKM pkm, GameVersion gameSource = GameVersion.Any)
        {
            if (gameSource == GameVersion.Any)
                gameSource = (GameVersion)pkm.Version;

            if (pkm.VC || pkm.Format <= 2)
                return GetValidEncounterTradesVC(pkm, gameSource);
            
            var p = GetValidPreEvolutions(pkm);
            var table = GetEncounterTradeTable(pkm);
            return table?.Where(f => p.Any(r => r.Species == f.Species)) ?? Enumerable.Empty<EncounterTrade>();
        }
        private static IEnumerable<EncounterTrade> GetPossibleVC(PKM pkm, GameVersion gameSource = GameVersion.Any)
        {
            var table = GetEncounterTradeTableVC(gameSource);
            var p = GetValidPreEvolutions(pkm);
            return table.Where(f => p.Any(r => r.Species == f.Species));
        }
        private static IEnumerable<EncounterTrade> GetEncounterTradeTableVC(GameVersion gameSource)
        {
            switch (gameSource)
            {
                case GameVersion.RBY:
                    return !AllowGen1Tradeback ? Encounters1.TradeGift_RBY_NoTradeback : Encounters1.TradeGift_RBY_Tradeback;
                case GameVersion.GSC:
                case GameVersion.C:
                    return Encounters2.TradeGift_GSC;
                default:
                    return null;
            }
        }
        private static IEnumerable<EncounterTrade> GetEncounterTradeTable(PKM pkm)
        {
            switch (pkm.GenNumber)
            {
                case 3: return pkm.FRLG ? Encounters3.TradeGift_FRLG : Encounters3.TradeGift_RSE;
                case 4: return pkm.HGSS ? Encounters4.TradeGift_HGSS : Encounters4.TradeGift_DPPt;
                case 5: return pkm.B2W2 ? Encounters5.TradeGift_B2W2 : Encounters5.TradeGift_BW;
                case 6: return pkm.XY ? Encounters6.TradeGift_XY : Encounters6.TradeGift_AO;
                case 7: return pkm.SM ? Encounters7.TradeGift_SM : Encounters7.TradeGift_USUM;
            }
            return null;
        }
        private static IEnumerable<EncounterTrade> GetValidEncounterTradesVC(PKM pkm, GameVersion gameSource)
        {
            var poss = GetPossibleVC(pkm, gameSource);
            if (gameSource == GameVersion.RBY)
                return poss.Where(z => GetIsValidTradeVC1(pkm, z));
            return poss.Where(z => GetIsValidTradeVC2(pkm, z));
        }
        private static bool GetIsValidTradeVC1(PKM pkm, EncounterTrade z)
        {
            if (z.Level > pkm.CurrentLevel) // minimum required level
                return false;
            if (pkm.Format != 1 || !pkm.Gen1_NotTradeback)
                return true;

            // Even if the in game trade uses the tables with source pokemon allowing generation 2 games, the traded pokemon could be a non-tradeback pokemon
            var rate = (pkm as PK1)?.Catch_Rate;
            if (z is EncounterTradeCatchRate r)
            {
                if (rate != r.Catch_Rate)
                    return false;
            }
            else
            {
                if (z.Version == GameVersion.YW && rate != PersonalTable.Y[z.Species].CatchRate)
                    return false;
                if (z.Version != GameVersion.YW && rate != PersonalTable.RB[z.Species].CatchRate)
                    return false;
            }
            return true;
        }
        private static bool GetIsValidTradeVC2(PKM pkm, EncounterTrade z)
        {
            if (z.Level > pkm.CurrentLevel) // minimum required level
                return false;
            if (z.TID != pkm.TID)
                return false;
            if (z.Gender >= 0 && z.Gender != pkm.Gender && pkm.Format <= 2)
                return false;
            if (z.IVs != null && !z.IVs.SequenceEqual(pkm.IVs) && pkm.Format <= 2)
                return false;
            if (pkm.Met_Location != 0 && pkm.Format == 2 && pkm.Met_Location != 126)
                return false;

            int index = Array.IndexOf(Encounters2.TradeGift_GSC, z);
            int otIndex = Encounters2.TradeGift_GSC.Length + index;
            bool valid;
            if (pkm.Japanese)
                valid = Encounters2.TradeGift_GSC_OTs[(int)LanguageID.Japanese][otIndex] == pkm.OT_Name;
            else if (pkm.Korean)
                valid = Encounters2.TradeGift_GSC_OTs[(int)LanguageID.Korean][otIndex] == pkm.OT_Name;
            else
                valid = Array.FindIndex(Encounters2.TradeGift_GSC_OTs, 2, 6, arr => arr.Length > index && arr[otIndex] == pkm.OT_Name) >= 0;
            if (!valid)
                return false;

            return true;
        }

        private static bool GetIsFromGB(PKM pkm) => pkm.VC || pkm.Format <= 2;
        public static IEnumerable<EncounterTrade> GetValidEncounterTrades(PKM pkm, GameVersion gameSource = GameVersion.Any)
        {
            if (GetIsFromGB(pkm))
                return GetValidEncounterTradesVC(pkm, gameSource);

            int lvl = IsNotTrade(pkm);
            if (lvl <= 0)
                return Enumerable.Empty<EncounterTrade>();

            var poss = GetPossibleNonVC(pkm);
            return poss.Where(z => IsEncounterTradeValid(pkm, z, lvl));
        }
        private static bool IsEncounterTradeValid(PKM pkm, EncounterTrade z, int lvl)
        {
            if (z.IVs != null)
                for (int i = 0; i < 6; i++)
                    if (z.IVs[i] != -1 && z.IVs[i] != pkm.IVs[i])
                        return false;

            if (z is EncounterTradePID p)
            {
                if (p.PID != pkm.EncryptionConstant)
                    return false;
                if (z.Nature != Nature.Random && (int)z.Nature != pkm.Nature) // gen5 BW only
                    return false;
            }
            else
            {
                if (!z.Shiny.IsValid(pkm))
                    return false;
                if (z.Nature != Nature.Random && (int)z.Nature != pkm.Nature)
                    return false;
                if (z.Gender != -1 && z.Gender != pkm.Gender)
                    return false;
            }
            if (z.TID != pkm.TID)
                return false;
            if (z.SID != pkm.SID)
                return false;
            if (pkm.HasOriginalMetLocation)
            {
                var loc = z.Location > 0 ? z.Location : EncounterTrade.DefaultMetLocation[z.Generation - 1];
                if (loc != pkm.Met_Location)
                    return false;
                if (pkm.Format < 5)
                {
                    if (z.Level > lvl)
                        return false;
                }
                else if (z.Level != lvl)
                    return false;
            }
            else if (z.Level > lvl)
                return false;

            if (z.CurrentLevel != -1 && z.CurrentLevel > pkm.CurrentLevel)
                return false;

            if (z.Form != pkm.AltForm && !IsFormChangeable(pkm, pkm.Species))
                return false;
            if (z.OTGender != -1 && z.OTGender != pkm.OT_Gender)
                return false;
            if (z.EggLocation != pkm.Egg_Location)
                return false;
            // if (z.Ability == 4 ^ pkm.AbilityNumber == 4) // defer to Ability 
            //    countinue;

            if (pkm.IsContestBelow(z))
                return false;

            return true;
        }

        private static int IsNotTrade(PKM pkm)
        {
            int lang = pkm.Language;
            if (lang == (int)LanguageID.UNUSED_6) // invalid language
                return 0;
            if (lang == (int)LanguageID.Hacked && (pkm.Format != 5 || !pkm.BW)) // Japanese trades in BW have no language ID
                return 0;

            return GetMinLevelEncounter(pkm);
        }
    }
}
