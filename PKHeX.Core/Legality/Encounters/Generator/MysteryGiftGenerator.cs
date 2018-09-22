using System.Collections.Generic;
using System.Linq;
using static PKHeX.Core.Legal;
using static PKHeX.Core.EncounterEvent;

namespace PKHeX.Core
{
    public static class MysteryGiftGenerator
    {
        public static IEnumerable<MysteryGift> GetPossible(PKM pkm)
        {
            int maxSpecies = GetMaxSpeciesOrigin(pkm.Format);
            var vs = EvolutionChain.GetValidPreEvolutions(pkm, maxSpecies);
            return GetPossible(pkm, vs);
        }

        public static IEnumerable<MysteryGift> GetPossible(PKM pkm, IReadOnlyList<DexLevel> vs)
        {
            var table = GetTable(pkm.GenNumber);
            return table.Where(wc => vs.Any(dl => dl.Species == wc.Species));
        }

        public static IEnumerable<MysteryGift> GetValidGifts(PKM pkm)
        {
            switch (pkm.GenNumber)
            {
                case 3: return GetMatchingWC3(pkm, MGDB_G3);
                case 4: return GetMatchingPCD(pkm, MGDB_G4);
                case 5: return GetMatchingPGF(pkm, MGDB_G5);
                case 6: return GetMatchingWC6(pkm, MGDB_G6);
                case 7: return GetMatchingWC7(pkm, MGDB_G7);
                default: return Enumerable.Empty<MysteryGift>();
            }
        }

        private static IEnumerable<MysteryGift> GetTable(int generation)
        {
            switch (generation)
            {
                case 3: return MGDB_G3;
                case 4: return MGDB_G4;
                case 5: return MGDB_G5;
                case 6: return MGDB_G6;
                case 7: return MGDB_G7;
                default: return Enumerable.Empty<MysteryGift>();
            }
        }

        private static IEnumerable<WC3> GetMatchingWC3(PKM pkm, IEnumerable<WC3> DB)
        {
            var validWC3 = new List<WC3>();
            var vs = EvolutionChain.GetValidPreEvolutions(pkm, MaxSpeciesID_3);
            var enumerable = DB.Where(wc => vs.Any(dl => dl.Species == wc.Species));
            foreach (var wc in enumerable)
            {
                if (!GetIsMatchWC3(pkm, wc))
                    continue;

                if (wc.Species == pkm.Species) // best match
                    yield return wc;
                else
                    validWC3.Add(wc);
            }
            foreach (var z in validWC3)
                yield return z;
        }

        private static IEnumerable<MysteryGift> GetMatchingPCD(PKM pkm, IEnumerable<PCD> DB)
        {
            if (pkm.IsEgg && pkm.Format != 4) // transferred
                yield break;

            if (IsRangerManaphy(pkm))
            {
                if (pkm.Language != (int)LanguageID.Korean) // never korean
                    yield return RangerManaphy;
                yield break;
            }

            var deferred = new List<PCD>();
            var vs = EvolutionChain.GetValidPreEvolutions(pkm);
            var enumerable = DB.Where(wc => vs.Any(dl => dl.Species == wc.Species));
            foreach (var mg in enumerable)
            {
                var wc = mg.Gift.PK;
                if (!GetIsMatchPCD(pkm, wc, vs))
                    continue;

                bool receivable = mg.CanBeReceivedBy(pkm.Version);
                if (wc.Species == pkm.Species && receivable) // best match
                    yield return mg;
                else
                    deferred.Add(mg);
            }
            foreach (var z in deferred)
                yield return z;
        }

        private static IEnumerable<PGF> GetMatchingPGF(PKM pkm, IEnumerable<PGF> DB)
        {
            var deferred = new List<PGF>();
            var vs = EvolutionChain.GetValidPreEvolutions(pkm);
            var enumerable = DB.Where(wc => vs.Any(dl => dl.Species == wc.Species));
            foreach (var wc in enumerable)
            {
                if (!GetIsMatchPGF(pkm, wc, vs))
                    continue;

                if (wc.Species == pkm.Species) // best match
                    yield return wc;
                else
                    deferred.Add(wc);
            }
            foreach (var z in deferred)
                yield return z;
        }

        private static IEnumerable<WC6> GetMatchingWC6(PKM pkm, IEnumerable<WC6> DB)
        {
            var deferred = new List<WC6>();
            var vs = EvolutionChain.GetValidPreEvolutions(pkm);
            var enumerable = DB.Where(wc => vs.Any(dl => dl.Species == wc.Species));
            foreach (var wc in enumerable)
            {
                if (!GetIsMatchWC6(pkm, wc, vs))
                    continue;
                if (GetIsDeferredWC6(pkm, wc))
                    deferred.Add(wc);
                else
                    yield return wc;
            }
            foreach (var z in deferred)
                yield return z;
        }

        private static IEnumerable<WC7> GetMatchingWC7(PKM pkm, IEnumerable<WC7> DB)
        {
            var deferred = new List<WC7>();
            var vs = EvolutionChain.GetValidPreEvolutions(pkm);
            var enumerable = DB.Where(wc => vs.Any(dl => dl.Species == wc.Species));
            foreach (var wc in enumerable)
            {
                if (!GetIsMatchWC7(pkm, wc, vs))
                    continue;

                if ((pkm.SID << 16 | pkm.TID) == 0x79F57B49) // Greninja WC has variant PID and can arrive @ 36 or 37
                {
                    if (!pkm.IsShiny)
                        deferred.Add(wc);
                    continue;
                }
                if (wc.PIDType == 0 && pkm.PID != wc.PID)
                    continue;

                if (GetIsDeferredWC7(pkm, wc))
                    deferred.Add(wc);
                else
                    yield return wc;
            }
            foreach (var z in deferred)
                yield return z;
        }

        private static bool GetIsMatchWC3(PKM pkm, WC3 wc)
        {
            // Gen3 Version MUST match.
            if (wc.Version != 0 && !(wc.Version).Contains((GameVersion)pkm.Version))
                return false;

            bool hatchedEgg = wc.IsEgg && !pkm.IsEgg;
            if (!hatchedEgg)
            {
                if (wc.SID != -1 && wc.SID != pkm.SID) return false;
                if (wc.TID != -1 && wc.TID != pkm.TID) return false;
                if (wc.OT_Name != null && wc.OT_Name != pkm.OT_Name) return false;
                if (wc.OT_Gender < 3 && wc.OT_Gender != pkm.OT_Gender) return false;
            }

            if (wc.Language != -1 && wc.Language != pkm.Language) return false;
            if (wc.Ball != pkm.Ball) return false;
            if (wc.Fateful != pkm.FatefulEncounter)
            {
                // XD Gifts only at level 20 get flagged after transfer
                if (wc.Version == GameVersion.XD != pkm is XK3)
                    return false;
            }

            if (pkm.IsNative)
            {
                if (hatchedEgg)
                    return true; // defer egg specific checks to later.
                if (wc.Met_Level != pkm.Met_Level)
                    return false;
                if (wc.Location != pkm.Met_Location)
                    return false;
            }
            else
            {
                if (pkm.IsEgg)
                    return false;
                if (wc.Level > pkm.Met_Level)
                    return false;
            }
            return true;
        }

        private static bool GetIsMatchPCD(PKM pkm, PK4 wc, IEnumerable<DexLevel> vs)
        {
            if (!wc.IsEgg)
            {
                if (wc.TID != pkm.TID) return false;
                if (wc.SID != pkm.SID) return false;
                if (wc.OT_Name != pkm.OT_Name) return false;
                if (wc.OT_Gender != pkm.OT_Gender) return false;
                if (wc.Language != 0 && wc.Language != pkm.Language) return false;

                if (pkm.Format != 4) // transferred
                {
                    // met location: deferred to general transfer check
                    if (wc.CurrentLevel > pkm.Met_Level) return false;
                }
                else
                {
                    if (wc.Egg_Location + 3000 != pkm.Met_Location) return false;
                    if (wc.CurrentLevel != pkm.Met_Level) return false;
                }
            }
            else // Egg
            {
                if (wc.Egg_Location + 3000 != pkm.Egg_Location && pkm.Egg_Location != 2002) // traded
                    return false;
                if (wc.CurrentLevel != pkm.Met_Level)
                    return false;
                if (pkm.IsEgg && !pkm.IsNative)
                    return false;
            }

            if (wc.AltForm != pkm.AltForm && vs.All(dl => !IsFormChangeable(pkm, dl.Species)))
                return false;

            if (wc.Ball != pkm.Ball) return false;
            if (wc.OT_Gender < 3 && wc.OT_Gender != pkm.OT_Gender) return false;
            if (wc.PID == 1 && pkm.IsShiny) return false;
            if (wc.Gender != 3 && wc.Gender != pkm.Gender) return false;

            if (pkm is IContestStats s && s.IsContestBelow(wc))
                return false;

            return true;
        }

        private static bool GetIsMatchPGF(PKM pkm, PGF wc, IEnumerable<DexLevel> vs)
        {
            if (!wc.IsEgg)
            {
                if (wc.SID != pkm.SID) return false;
                if (wc.TID != pkm.TID) return false;
                if (wc.OT_Name != pkm.OT_Name) return false;
                if (wc.OTGender < 3 && wc.OTGender != pkm.OT_Gender) return false;
                if (wc.PID != 0 && pkm.PID != wc.PID) return false;
                if (wc.PIDType == 0 && pkm.IsShiny) return false;
                if (wc.PIDType == 2 && !pkm.IsShiny) return false;
                if (wc.OriginGame != 0 && wc.OriginGame != pkm.Version) return false;
                if (wc.Language != 0 && wc.Language != pkm.Language) return false;

                if (wc.EggLocation != pkm.Egg_Location) return false;
                if (wc.MetLocation != pkm.Met_Location) return false;
            }
            else
            {
                if (wc.EggLocation != pkm.Egg_Location) // traded
                {
                    if (pkm.Egg_Location != 30003)
                        return false;
                }
                else if (wc.PIDType == 0 && pkm.IsShiny)
                {
                    return false; // can't be traded away for unshiny
                }

                if (pkm.IsEgg && !pkm.IsNative)
                    return false;
            }

            if (wc.Form != pkm.AltForm && vs.All(dl => !IsFormChangeable(pkm, dl.Species))) return false;

            if (wc.Level != pkm.Met_Level) return false;
            if (wc.Ball != pkm.Ball) return false;
            if (wc.Nature != 0xFF && wc.Nature != pkm.Nature) return false;
            if (wc.Gender != 2 && wc.Gender != pkm.Gender) return false;

            if (pkm is IContestStats s && s.IsContestBelow(wc))
                return false;

            return true;
        }

        private static bool GetIsMatchWC6(PKM pkm, WC6 wc, IEnumerable<DexLevel> vs)
        {
            if (pkm.Egg_Location == 0) // Not Egg
            {
                if (wc.CardID != pkm.SID) return false;
                if (wc.TID != pkm.TID) return false;
                if (wc.OT_Name != pkm.OT_Name) return false;
                if (wc.OTGender != pkm.OT_Gender) return false;
                if (wc.PIDType == Shiny.FixedValue && pkm.PID != wc.PID) return false;
                if (!wc.PIDType.IsValid(pkm)) return false;
                if (wc.OriginGame != 0 && wc.OriginGame != pkm.Version) return false;
                if (wc.EncryptionConstant != 0 && wc.EncryptionConstant != pkm.EncryptionConstant) return false;
                if (wc.Language != 0 && wc.Language != pkm.Language) return false;
            }
            if (wc.Form != pkm.AltForm && vs.All(dl => !IsFormChangeable(pkm, dl.Species))) return false;

            if (wc.IsEgg)
            {
                if (wc.EggLocation != pkm.Egg_Location) // traded
                {
                    if (pkm.Egg_Location != 30002)
                        return false;
                }
                else if (wc.PIDType == 0 && pkm.IsShiny)
                {
                    return false; // can't be traded away for unshiny
                }

                if (pkm.IsEgg && !pkm.IsNative)
                    return false;
            }
            else
            {
                if (wc.EggLocation != pkm.Egg_Location) return false;
                if (wc.MetLocation != pkm.Met_Location) return false;
            }

            if (wc.Level != pkm.Met_Level) return false;
            if (wc.Ball != pkm.Ball) return false;
            if (wc.OTGender < 3 && wc.OTGender != pkm.OT_Gender) return false;
            if (wc.Nature != 0xFF && wc.Nature != pkm.Nature) return false;
            if (wc.Gender != 3 && wc.Gender != pkm.Gender) return false;

            if (pkm is IContestStats s && s.IsContestBelow(wc))
                return false;

            return true;
        }

        private static bool GetIsMatchWC7(PKM pkm, WC7 wc, IEnumerable<DexLevel> vs)
        {
            if (pkm.Egg_Location == 0) // Not Egg
            {
                if (wc.OTGender != 3)
                {
                    if (wc.SID != pkm.SID) return false;
                    if (wc.TID != pkm.TID) return false;
                    if (wc.OTGender != pkm.OT_Gender) return false;
                }
                if (!string.IsNullOrEmpty(wc.OT_Name) && wc.OT_Name != pkm.OT_Name) return false;
                if (wc.OriginGame != 0 && wc.OriginGame != pkm.Version) return false;
                if (wc.EncryptionConstant != 0 && wc.EncryptionConstant != pkm.EncryptionConstant) return false;
                if (wc.Language != 0 && wc.Language != pkm.Language) return false;
            }
            if (wc.Form != pkm.AltForm && vs.All(dl => !IsFormChangeable(pkm, dl.Species)))
            {
                if (wc.Species == 744 && wc.Form == 1 && pkm.Species == 745 && pkm.AltForm == 2)
                {
                    // Rockruff gift edge case; has altform 1 then evolves to altform 2
                }
                else
                {
                    return false;
                }
            }

            if (wc.IsEgg)
            {
                if (wc.EggLocation != pkm.Egg_Location) // traded
                {
                    if (pkm.Egg_Location != 30002)
                        return false;
                }
                else if (wc.PIDType == 0 && pkm.IsShiny)
                {
                    return false; // can't be traded away for unshiny
                }

                if (pkm.IsEgg && !pkm.IsNative)
                    return false;
            }
            else
            {
                if (!wc.PIDType.IsValid(pkm)) return false;
                if (wc.EggLocation != pkm.Egg_Location) return false;
                if (wc.MetLocation != pkm.Met_Location) return false;
            }

            if (wc.MetLevel != pkm.Met_Level) return false;
            if (wc.Ball != pkm.Ball) return false;
            if (wc.OTGender < 3 && wc.OTGender != pkm.OT_Gender) return false;
            if (wc.Nature != 0xFF && wc.Nature != pkm.Nature) return false;
            if (wc.Gender != 3 && wc.Gender != pkm.Gender) return false;

            if (pkm is IContestStats s && s.IsContestBelow(wc))
                return false;

            switch (wc.CardID)
            {
                case 2046: // Ash Greninja
                    return pkm.SM; // not USUM
            }
            return true;
        }

        private static bool GetIsDeferredWC6(PKM pkm, WC6 wc)
        {
            switch (wc.CardID)
            {
                case 0525 when wc.IV_HP == 0xFE: // Diancie was distributed with no IV enforcement & 3IVs
                case 0504 when wc.RibbonClassic != ((IRibbonSetEvent4)pkm).RibbonClassic: // magmar with/without classic
                    return true;
            }
            if (wc.RestrictLanguage != 0 && wc.RestrictLanguage != pkm.Language)
                return true;
            if (!wc.CanBeReceivedByVersion(pkm.Version))
                return true;
            return wc.Species != pkm.Species;
        }

        private static bool GetIsDeferredWC7(PKM pkm, WC7 wc)
        {
            if (wc.RestrictLanguage != 0 && wc.RestrictLanguage != pkm.Language)
                return true;
            if (!wc.CanBeReceivedByVersion(pkm.Version))
                return true;
            return wc.Species != pkm.Species;
        }

        // Utility
        private static readonly PGT RangerManaphy = new PGT {Data = {[0] = 7, [8] = 1}};

        private static bool IsRangerManaphy(PKM pkm)
        {
            var egg = pkm.Egg_Location;
            const int ranger = 3001;
            const int linkegg = 2002;
            if (!pkm.IsEgg) // Link Trade Egg or Ranger
                return egg == linkegg || egg == ranger;
            if (egg != ranger)
                return false;
            var met = pkm.Met_Location;
            return met == linkegg || met == 0;
        }
    }
}
