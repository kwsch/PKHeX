using System;
using System.Collections.Generic;
using System.Linq;
using static PKHeX.Core.Legal;

namespace PKHeX.Core
{
    public static class EncounterTradeGenerator
    {
        private static EncounterTrade[] GetEncounterTradeTable(PKM pkm)
        {
            switch (pkm.GenNumber)
            {
                case 3:
                    return pkm.FRLG ? Encounters3.TradeGift_FRLG : Encounters3.TradeGift_RSE;
                case 4:
                    return pkm.HGSS ? Encounters4.TradeGift_HGSS : Encounters4.TradeGift_DPPt;
                case 5:
                    return pkm.B2W2 ? Encounters5.TradeGift_B2W2 : Encounters5.TradeGift_BW;
                case 6:
                    return pkm.XY ? Encounters6.TradeGift_XY : Encounters6.TradeGift_AO;
                case 7:
                    return pkm.SM ? Encounters7.TradeGift_SM : Encounters7.TradeGift_USUM;
            }
            return null;
        }
        private static IEnumerable<EncounterTrade> GetValidEncounterTradesVC(PKM pkm, GameVersion gameSource)
        {
            var p = GetValidPreEvolutions(pkm).ToArray();

            switch (gameSource)
            {
                case GameVersion.RBY:
                    var table = !AllowGen1Tradeback ? Encounters1.TradeGift_RBY_NoTradeback : Encounters1.TradeGift_RBY_Tradeback;
                    return GetValidEncounterTradesVC1(pkm, p, table);
                case GameVersion.GSC:
                case GameVersion.C:
                    return GetValidEncounterTradesVC2(pkm, p);
                default:
                    return null;
            }
        }
        private static IEnumerable<EncounterTrade> GetValidEncounterTradesVC2(PKM pkm, DexLevel[] p)
        {
            // Check GSC trades. Reuse generic table fetch-match
            var possible = GetValidEncounterTradesVC1(pkm, p, Encounters2.TradeGift_GSC);

            foreach (var z in possible)
            {
                // Filter Criteria
                if (z.TID != pkm.TID)
                    continue;
                if (z.Gender >= 0 && z.Gender != pkm.Gender && pkm.Format <= 2)
                    continue;
                if (z.IVs != null && !z.IVs.SequenceEqual(pkm.IVs) && pkm.Format <= 2)
                    continue;
                if (pkm.Met_Location != 0 && pkm.Format == 2 && pkm.Met_Location != 126)
                    continue;

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
                    continue;

                yield return z;
            }
        }
        private static IEnumerable<EncounterTrade> GetValidEncounterTradesVC1(PKM pkm, DexLevel[] p, IEnumerable<EncounterTrade> table)
        {
            var possible = table.Where(f => p.Any(r => r.Species == f.Species));
            foreach (var z in possible)
            {
                if (z == null)
                    continue;
                if (z.Level > pkm.CurrentLevel) // minimum required level
                    continue;
                if (pkm.Format != 1 || !pkm.Gen1_NotTradeback)
                    yield return z;

                // Even if the in game trade uses the tables with source pokemon allowing generation 2 games, the traded pokemon could be a non-tradeback pokemon
                var rate = (pkm as PK1)?.Catch_Rate;
                if (z is EncounterTradeCatchRate r)
                {
                    if (rate != r.Catch_Rate)
                        continue;
                }
                else
                {
                    if (z.Version == GameVersion.YW && rate != PersonalTable.Y[z.Species].CatchRate)
                        continue;
                    if (z.Version != GameVersion.YW && rate != PersonalTable.RB[z.Species].CatchRate)
                        continue;
                }

                yield return z;
            }
        }

        public static IEnumerable<EncounterTrade> GetValidEncounterTrades(PKM pkm, GameVersion gameSource = GameVersion.Any)
        {
            if (gameSource == GameVersion.Any)
                gameSource = (GameVersion)pkm.Version;

            if (pkm.VC || pkm.Format <= 2)
            {
                foreach (var z in GetValidEncounterTradesVC(pkm, gameSource))
                    yield return z;
                yield break;
            }

            int lang = pkm.Language;
            if (lang == (int)LanguageID.UNUSED_6) // invalid language
                yield break;
            if (lang == (int)LanguageID.Hacked && (pkm.Format != 5 || !pkm.BW)) // Japanese trades in BW have no language ID
                yield break;

            int lvl = GetMinLevelEncounter(pkm);
            if (lvl <= 0)
                yield break;

            // Get valid pre-evolutions
            IEnumerable<DexLevel> p = GetValidPreEvolutions(pkm);

            EncounterTrade[] table = GetEncounterTradeTable(pkm);
            if (table == null)
                yield break;
            var poss = table.Where(f => p.Any(r => r.Species == f.Species) && f.Version.Contains((GameVersion)pkm.Version));

            foreach (var z in poss)
            {
                if (IsEncounterTradeValid(pkm, z, lvl))
                    yield return z;
            }
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
                if (z.Shiny ^ pkm.IsShiny)
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
                var loc = z.Location > 0 ? z.Location : EncounterTrade.DefaultMetLocation[pkm.GenNumber - 1];
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

            if (z.Contest != null)
                for (int i = 0; i < 6; i++)
                    if (z.Contest[i] > pkm.Contest[i])
                        return false;

            return true;
        }
    }
}
