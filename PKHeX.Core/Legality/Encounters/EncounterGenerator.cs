using System;
using System.Collections.Generic;
using System.Linq;
using static PKHeX.Core.Legal;

namespace PKHeX.Core
{
    public static class EncounterGenerator
    {
        public static IEnumerable<IEncounterable> getEncounters(PKM pkm, LegalInfo info)
        {
            switch (info.Generation)
            {
                case 1:
                case 2:
                    foreach (var enc in getEncounters12(pkm, info))
                        yield return enc;
                    yield break;
                case 3:
                    // info.PIDIV = MethodFinder.Analyze(pkm);
                    foreach (var enc in getEncounters3(pkm, info))
                        yield return enc;
                    yield break;
                case 4:
                    // info.PIDIV = MethodFinder.Analyze(pkm);
                    foreach (var enc in getEncounters4(pkm, info))
                        yield return enc;
                    yield break;
                default:
                    foreach (var enc in GenerateRawEncounters(pkm))
                        yield return enc;
                    yield break;
            }
        }

        private static IEnumerable<IEncounterable> getEncounters12(PKM pkm, LegalInfo info)
        {
            int baseSpecies = getBaseSpecies(pkm);
            bool g1 = pkm.VC1 || pkm.Format == 1;

            if (g1 && baseSpecies > MaxSpeciesID_1 || baseSpecies > MaxSpeciesID_2)
                yield break;

            foreach (var z in GenerateFilteredEncounters(pkm))
            {
                info.Generation = z.Generation;
                info.Game = z.Game;
                yield return z.Encounter;
            }
        }
        private static IEnumerable<IEncounterable> getEncounters3(PKM pkm, LegalInfo info)
        {
            info.PIDIV = MethodFinder.Analyze(pkm);
            var deferred = new List<IEncounterable>();
            foreach (var z in GenerateRawEncounters3(pkm))
            {
                if (z is EncounterSlot w && pkm.Version == 15)
                    info.PIDIV = MethodFinder.getPokeSpotSeeds(pkm, w.SlotNumber).FirstOrDefault() ?? info.PIDIV; 
                if (info.PIDIV.Type.IsCompatible3(z, pkm))
                    yield return z;
                else
                    deferred.Add(z);
            }
            info.PIDIVMatches = false;
            foreach (var z in deferred)
                yield return z;
        }
        private static IEnumerable<IEncounterable> getEncounters4(PKM pkm, LegalInfo info)
        {
            info.PIDIV = MethodFinder.Analyze(pkm);
            var deferred = new List<IEncounterable>();
            foreach (var z in GenerateRawEncounters4(pkm))
            {
                if (info.PIDIV.Type.IsCompatible4(z))
                    yield return z;
                else
                    deferred.Add(z);
            }
            info.PIDIVMatches = false;
            foreach (var z in deferred)
                yield return z;
        }
        private static IEnumerable<GBEncounterData> GenerateRawEncounters12(PKM pkm, GameVersion game)
        {
            var gen = game == GameVersion.GSC ? 2 : 1;

            // Since encounter matching is super weak due to limited stored data in the structure
            // Calculate all 3 at the same time and pick the best result (by species).
            // Favor special event move gifts as Static Encounters when applicable
            var maxspeciesorigin = game == GameVersion.GSC ? MaxSpeciesID_2 : MaxSpeciesID_1;
            DexLevel[] vs = getValidPreEvolutions(pkm, maxspeciesorigin: maxspeciesorigin).ToArray();
            HashSet<int> species = new HashSet<int>(vs.Select(p => p.Species).ToList());

            var deferred = new List<IEncounterable>();
            foreach (var t in getValidEncounterTrade(pkm, game))
            {
                yield return new GBEncounterData(pkm, gen, t, game);
            }
            foreach (var s in getValidStaticEncounter(pkm, game))
            {
                // Valid stadium and non-stadium encounters, return only non-stadium encounters, they are less restrictive
                if (!species.Contains(s.Species))
                    continue;
                if (game == GameVersion.RBY && s.Species != 54 && s.Version == GameVersion.Stadium)
                {
                    deferred.Add(s);
                    continue;
                }
                yield return new GBEncounterData(pkm, gen, s, game);
            }
            foreach (var e in getValidWildEncounters(pkm, game))
            {
                if (!species.Contains(e.Species))
                    continue;
                yield return new GBEncounterData(pkm, gen, e, game);
            }

            if (game == GameVersion.GSC || game == GameVersion.C)
            {
                bool WasEgg = !pkm.Gen1_NotTradeback && getWasEgg23(pkm) && !NoHatchFromEgg.Contains(pkm.Species);
                if (WasEgg)
                {
                    // Further Filtering
                    if (pkm.Format < 3)
                    {
                        WasEgg &= pkm.Met_Location == 0 || pkm.Met_Level == 1; // 2->1->2 clears met info
                        WasEgg &= pkm.CurrentLevel >= 5;
                    }
                }
                if (WasEgg)
                {
                    int eggspec = getBaseEggSpecies(pkm);
                    if (AllowGen2Crystal)
                        yield return new GBEncounterData(eggspec, GameVersion.C); // gen2 egg
                    yield return new GBEncounterData(eggspec, GameVersion.GS); // gen2 egg
                }
            }

            foreach (var d in deferred)
                yield return new GBEncounterData(pkm, gen, d, game);
        }
        private static IEnumerable<GBEncounterData> GenerateFilteredEncounters(PKM pkm)
        {
            bool crystal = pkm.Format == 2 && pkm.Met_Location != 0;
            var g1i = new PeekEnumerator<GBEncounterData>(get1().GetEnumerator());
            var g2i = new PeekEnumerator<GBEncounterData>(get2().GetEnumerator());
            var deferred = new List<GBEncounterData>();
            while (g2i.PeekIsNext() || g1i.PeekIsNext())
            {
                PeekEnumerator<GBEncounterData> move;
                if (g1i.PeekIsNext())
                {
                    if (g2i.PeekIsNext())
                        move = g1i.Peek().Type > g2i.Peek().Type ? g1i : g2i;
                    else
                        move = g1i;
                }
                else
                    move = g2i;

                var obj = move.Peek();
                if (obj.Generation == 1 && obj.Encounter is EncounterTrade && !getEncounterTrade1Valid(pkm))
                    deferred.Add(obj);
                else
                    yield return obj;

                move.MoveNext();
            }
            foreach (var z in deferred)
                yield return z;

            IEnumerable<GBEncounterData> get1()
            {
                if (!pkm.Gen2_NotTradeback && !crystal)
                    foreach (var z in GenerateRawEncounters12(pkm, GameVersion.RBY))
                        yield return z;
            }
            IEnumerable<GBEncounterData> get2()
            {
                if (!pkm.Gen1_NotTradeback && AllowGen2VCTransfer)
                    foreach (var z in GenerateRawEncounters12(pkm, crystal ? GameVersion.C : GameVersion.GSC))
                        yield return z;
            }
        }
        private static IEnumerable<IEncounterable> GenerateRawEncounters(PKM pkm)
        {
            int ctr = 0;
            if (pkm.WasLink)
            {
                foreach (var z in getValidLinkGifts(pkm))
                { yield return z; ++ctr; }
                if (ctr != 0) yield break;
            }

            if (pkm.WasEvent || pkm.WasEventEgg)
            {
                foreach (var z in getValidGifts(pkm))
                { yield return z; ++ctr; }
                if (ctr != 0) yield break;
            }

            if (pkm.WasEgg)
            {
                foreach (var z in generateEggs(pkm))
                    yield return z;
            }

            foreach (var z in getValidStaticEncounter(pkm))
            { yield return z; ++ctr; }
            if (ctr != 0) yield break;
            foreach (var z in getValidFriendSafari(pkm))
            { yield return z; ++ctr; }
            if (ctr != 0) yield break;
            foreach (var z in getValidWildEncounters(pkm))
            { yield return z; ++ctr; }
            if (ctr != 0) yield break;
            foreach (var z in getValidEncounterTrade(pkm))
            { yield return z; ++ctr; }
            // if (ctr != 0) yield break;
        }
        private static IEnumerable<IEncounterable> GenerateRawEncounters4(PKM pkm)
        {
            int ctr = 0;
            bool wasEvent = pkm.WasEvent || pkm.WasEventEgg; // egg events?
            if (wasEvent)
            {
                foreach (var z in getValidGifts(pkm))
                { yield return z; ++ctr; }
                if (ctr != 0) yield break;
            }
            if (pkm.WasEgg)
            {
                foreach (var z in generateEggs(pkm))
                { yield return z; ++ctr; }
            }

            bool safariSport = pkm.Ball == 0x05 || pkm.Ball == 0x18; // never static encounters
            if (!safariSport)
            foreach (var z in getValidStaticEncounter(pkm))
            { yield return z; ++ctr; }
            if (ctr != 0) yield break;
            foreach (var z in getValidFriendSafari(pkm))
            { yield return z; ++ctr; }
            if (ctr != 0) yield break;
            foreach (var z in getValidWildEncounters(pkm))
            { yield return z; ++ctr; }
            if (ctr != 0) yield break;
            foreach (var z in getValidEncounterTrade(pkm))
            { yield return z; ++ctr; }
            if (ctr != 0) yield break;

            // do static encounters if they were deferred to end, spit out any possible encounters for invalid pkm
            if (safariSport)
            foreach (var z in getValidStaticEncounter(pkm))
                yield return z;
        }
        private static IEnumerable<IEncounterable> GenerateRawEncounters3(PKM pkm)
        {
            foreach (var z in getValidGifts(pkm))
                yield return z;

            bool safari = pkm.Ball == 0x05; // never static encounters
            if (!safari)
            foreach (var z in getValidStaticEncounter(pkm))
                yield return z;
            foreach (var z in getValidFriendSafari(pkm))
                yield return z;
            foreach (var z in getValidWildEncounters(pkm))
                yield return z;
            foreach (var z in getValidEncounterTrade(pkm))
                yield return z;

            // do static encounters if they were deferred to end, spit out any possible encounters for invalid pkm
            if (safari)
            foreach (var z in getValidStaticEncounter(pkm))
                yield return z;

            if (pkm.Version == 15)
                yield break; // no eggs in C/XD

            foreach (var z in generateEggs(pkm))
                yield return z;
        }

        // EncounterStatic
        private static bool IsEncounterTypeMatch(IEncounterable e, int type)
        {
            return type == 0 && !(e is EncounterStaticTyped)
                   || e is EncounterStaticTyped t && t.TypeEncounter.Contains(type);
        }
        private static IEnumerable<EncounterStatic> getValidStaticEncounter(PKM pkm, GameVersion gameSource = GameVersion.Any)
        {
            if (gameSource == GameVersion.Any)
                gameSource = (GameVersion)pkm.Version;

            // Get possible encounters
            IEnumerable<EncounterStatic> poss = getStaticEncounters(pkm, gameSource: gameSource);

            int lvl = getMinLevelEncounter(pkm);
            if (lvl <= 0)
                yield break;

            // Back Check against pkm
            var enc = getMatchingStaticEncounters(pkm, poss, lvl).ToList();

            // Filter for encounter types; type is cleared on 6->7 transfer
            if (!pkm.Gen4 || pkm.Format >= 7)
            {
                foreach (var e in enc)
                    yield return e;
                yield break;
            }

            // Yield out if type matches, else defer to end if no matches were yielded
            int ctr = 0;
            int type = pkm.EncounterType;
            var pass = new List<EncounterStatic>();
            foreach (var e in enc)
            {
                if (IsEncounterTypeMatch(e, type))
                { yield return e; ++ctr; }
                else pass.Add(e);
            }
            if (ctr != 0)
                yield break;

            foreach (var e in pass)
                yield return e;
        }
        private static IEnumerable<EncounterStatic> getMatchingStaticEncounters(PKM pkm, IEnumerable<EncounterStatic> poss, int lvl)
        {
            foreach (EncounterStatic e in poss)
            {
                if (e.Nature != Nature.Random && pkm.Nature != (int)e.Nature)
                    continue;
                if (pkm.Gen3 && e.EggLocation != 0) // Gen3 Egg
                {
                    if (pkm.Format == 3 && pkm.IsEgg && e.EggLocation != pkm.Met_Location)
                        continue;
                }
                else if (pkm.VC || e.EggLocation != 0) // Gen2 Egg
                {
                    if (pkm.Format <= 2)
                    {
                        if (pkm.IsEgg)
                        {
                            if (pkm.Met_Location != 0 && pkm.Met_Level != 0)
                                continue;
                        }
                        else
                        {
                            switch (pkm.Met_Level)
                            {
                                case 0:
                                    if (pkm.Met_Location != 0)
                                        continue;
                                    break;
                                case 1:
                                    if (pkm.Met_Location == 0)
                                        continue;
                                    break;
                                default:
                                    if (pkm.Met_Location == 0)
                                        continue;
                                    break;
                            }
                        }
                        lvl = 5; // met @ 1, hatch @ 5.
                    }
                }
                else if (e.EggLocation != pkm.Egg_Location)
                {
                    switch (pkm.GenNumber)
                    {
                        case 4:
                            if (pkm.Egg_Location != 2002) // Link Trade
                                continue;
                            break;
                        default:
                            if (pkm.Egg_Location != 30002) // Link Trade
                                continue;
                            break;
                    }
                }
                if (pkm.HasOriginalMetLocation)
                {
                    if (!e.EggEncounter && e.Location != 0 && e.Location != pkm.Met_Location)
                        continue;
                    if (e.Level != lvl)
                        continue;
                }
                else if (e.Level > lvl)
                    continue;
                if (e.Gender != -1 && e.Gender != pkm.Gender)
                    continue;
                if (e.Form != pkm.AltForm && !e.SkipFormCheck && !getCanFormChange(pkm, e.Species))
                    continue;
                if (e.EggLocation == 60002 && e.Relearn[0] == 0 && pkm.RelearnMoves.Any(z => z != 0)) // gen7 eevee edge case
                    continue;

                if (pkm is PK1 pk1 && pkm.Gen1_NotTradeback)
                {
                    var catch_rate = pk1.Catch_Rate;
                    var japanese = pk1.Japanese;
                    // Pure gen 1, trades can be filter by catch rate
                    if ((pkm.Species == 25 || pkm.Species == 26) && catch_rate == 190)
                        // Red Blue Pikachu, is not a static encounter
                        continue;

                    if (e.Version == GameVersion.Stadium)
                    {
                        if (e.Species != 054 && !Stadium_CatchRate.Contains(catch_rate))
                            continue;
                        // Amnesia Psyduck have different catch rate in japanese stadium and international stadium
                        if (e.Species == 054 && japanese && catch_rate != 167)
                            continue;
                        if (e.Species == 054 && !japanese && catch_rate != 168)
                            continue;
                    }
                    else if (catch_rate != PersonalTable.RB[e.Species].CatchRate && catch_rate != PersonalTable.Y[e.Species].CatchRate)
                        continue;
                }

                // Defer to EC/PID check
                // if (e.Shiny != null && e.Shiny != pkm.IsShiny)
                // continue;

                // Defer ball check to later
                // if (e.Gift && pkm.Ball != 4) // PokéBall
                // continue;

                if (!AllowGBCartEra && GameVersion.GBCartEraOnly.Contains(e.Version))
                    continue; // disallow gb cart era encounters (as they aren't obtainable by Main/VC series)

                yield return e;
            }
        }
        private static IEnumerable<EncounterStatic> getStaticEncounters(PKM pkm, int lvl = -1, GameVersion gameSource = GameVersion.Any)
        {
            if (gameSource == GameVersion.Any)
                gameSource = (GameVersion)pkm.Version;

            var table = getEncounterStaticTable(pkm, gameSource);
            switch (pkm.GenNumber)
            {
                case 1:
                    return getStatic(pkm, table, maxspeciesorigin: MaxSpeciesID_1, lvl: lvl);
                case 2:
                    return getStatic(pkm, table, maxspeciesorigin: MaxSpeciesID_2, lvl: lvl);
                default:
                    return getStatic(pkm, table, lvl);
            }
        }
        private static IEnumerable<EncounterStatic> getStatic(PKM pkm, IEnumerable<EncounterStatic> table, int maxspeciesorigin = -1, int lvl = -1)
        {
            IEnumerable<DexLevel> dl = getValidPreEvolutions(pkm, maxspeciesorigin: maxspeciesorigin, lvl: lvl);
            return table.Where(e => dl.Any(d => d.Species == e.Species));
        }

        // EncounterSlot
        private static IEnumerable<EncounterSlot> getRawEncounterSlots(PKM pkm, GameVersion gameSource = GameVersion.Any)
        {
            return getEncounterAreas(pkm, gameSource).SelectMany(area => getValidEncounterSlots(pkm, area, DexNav: pkm.AO, gameSource: gameSource));
        }
        private static IEnumerable<EncounterSlot> getValidWildEncounters(PKM pkm, GameVersion gameSource = GameVersion.Any)
        {
            if (gameSource == GameVersion.Any)
                gameSource = (GameVersion)pkm.Version;

            var s = getRawEncounterSlots(pkm, gameSource);
            bool IsSafariBall = pkm.Ball == 5;
            bool IsSportsBall = pkm.Ball == 0x18;
            bool IsHidden = pkm.AbilityNumber == 4; // hidden Ability
            int gen = pkm.GenNumber;
            int species = pkm.Species;
            bool CheckEncounterType = gen == 4 && pkm.Format != 7;

            var deferred = new List<EncounterSlot>();
            foreach (EncounterSlot slot in s)
            {
                if (slot.Species == 265 && species != 265 && !getWurmpleEvoValid(pkm)) { } // bad wurmple evolution
                else if (IsHidden ^ IsHiddenAbilitySlot(slot)) { } // ability mismatch
                else if (IsSafariBall ^ IsSafariSlot(slot.Type)) { } // Safari Zone only ball
                else if (IsSportsBall ^ slot.Type == SlotType.BugContest) { } // BCC only ball
                else if (CheckEncounterType && !slot.TypeEncounter.Contains(pkm.EncounterType)) { } // mismatch
                else
                {
                    yield return slot;
                    continue;
                }
                deferred.Add(slot);
            }
            foreach (var d in deferred)
                yield return d;
        }
        private static IEnumerable<EncounterSlot> getValidFriendSafari(PKM pkm)
        {
            if (!pkm.XY)
                yield break;
            if (pkm.Met_Location != 148) // Friend Safari
                yield break;
            if (pkm.Met_Level != 30)
                yield break;

            IEnumerable<DexLevel> vs = getValidPreEvolutions(pkm);
            foreach (DexLevel d in vs.Where(d => FriendSafari.Contains(d.Species) && d.Level >= 30))
            {
                yield return new EncounterSlot
                {
                    Species = d.Species,
                    LevelMin = 30,
                    LevelMax = 30,
                    Form = 0,
                    Type = SlotType.FriendSafari,
                };
            }
        }
        private static IEnumerable<EncounterSlot> getValidEncounterSlots(PKM pkm, EncounterArea loc, bool DexNav, bool ignoreLevel = false, GameVersion gameSource = GameVersion.Any)
        {
            int fluteBoost = pkm.Format < 3 ? 0 : 4;
            const int dexnavBoost = 30;

            int df = DexNav ? fluteBoost : 0;
            int dn = DexNav ? fluteBoost + dexnavBoost : 0;
            List<EncounterSlot> slotdata = new List<EncounterSlot>();

            var maxspeciesorigin = -1;
            if (gameSource == GameVersion.RBY) maxspeciesorigin = MaxSpeciesID_1;
            if (gameSource == GameVersion.GSC) maxspeciesorigin = MaxSpeciesID_2;

            // Get Valid levels
            IEnumerable<DexLevel> vs = getValidPreEvolutions(pkm, maxspeciesorigin: maxspeciesorigin, lvl: ignoreLevel ? 100 : -1, skipChecks: ignoreLevel);

            bool IsRGBKadabra = false;
            if (pkm.Format == 1 && pkm.Gen1_NotTradeback)
            {
                // Pure gen 1, slots can be filter by catch rate
                if ((pkm.Species == 25 || pkm.Species == 26) && (pkm as PK1).Catch_Rate == 163)
                    // Yellow Pikachu, is not a wild encounter
                    return slotdata;
                if ((pkm.Species == 64 || pkm.Species == 65) && (pkm as PK1).Catch_Rate == 96)
                    // Yellow Kadabra, ignore Abra encounters
                    vs = vs.Where(s => s.Species == 64);
                if ((pkm.Species == 148 || pkm.Species == 149) && (pkm as PK1).Catch_Rate == 27)
                    // Yellow Dragonair, ignore Dratini encounters
                    vs = vs.Where(s => s.Species == 148);
                else
                {
                    IsRGBKadabra = (pkm.Species == 64 || pkm.Species == 65) && (pkm as PK1).Catch_Rate == 100;
                    vs = vs.Where(s => (pkm as PK1).Catch_Rate == PersonalTable.RB[s.Species].CatchRate);
                }
            }

            // Get slots where pokemon can exist
            bool ignoreSlotLevel = ignoreLevel;
            IEnumerable<EncounterSlot> slots = loc.Slots.Where(slot => vs.Any(evo => evo.Species == slot.Species && (ignoreSlotLevel || evo.Level >= slot.LevelMin - df)));

            int lvl = getMinLevelEncounter(pkm);
            if (lvl <= 0)
                return slotdata;
            int gen = pkm.GenNumber;

            List<EncounterSlot> encounterSlots;
            if (ignoreLevel)
                encounterSlots = slots.ToList();
            else if (pkm.HasOriginalMetLocation)
                encounterSlots = slots.Where(slot => slot.LevelMin - df <= lvl && lvl <= slot.LevelMax + (slot.AllowDexNav ? dn : df)).ToList();
            else // check for any less than current level
                encounterSlots = slots.Where(slot => slot.LevelMin <= lvl).ToList();

            if (gen <= 2)
            {
                if (IsRGBKadabra)
                    //Red Kadabra slots : Level 49 and 51 in RGB, but level 20 and 27 in Yellow
                    encounterSlots = encounterSlots.Where(slot => slot.LevelMin >= 49).ToList();

                // For gen 1 and 2 return Minimum level slot
                // Minimum level is needed to check available moves, because there is no move reminder in gen 1,
                // There are moves in the level up table that cant be legally obtained
                EncounterSlot slotMin = encounterSlots.OrderBy(slot => slot.LevelMin).FirstOrDefault();
                if (slotMin != null)
                    slotdata.Add(slotMin);
                return slotdata;
            }

            // Pressure Slot
            EncounterSlot slotMax = encounterSlots.OrderByDescending(slot => slot.LevelMax).FirstOrDefault();
            if (slotMax != null)
            {
                slotMax = slotMax.Clone();
                slotMax.Pressure = true;
                slotMax.Form = pkm.AltForm;
            }

            if (gen >= 6 && !DexNav)
            {
                // Filter for Form Specific
                slotdata.AddRange(WildForms.Contains(pkm.Species)
                    ? encounterSlots.Where(slot => slot.Form == pkm.AltForm)
                    : encounterSlots);
                if (slotMax != null)
                    slotdata.Add(slotMax);
                return slotdata;
            }

            List<EncounterSlot> eslots = encounterSlots.Where(slot => !WildForms.Contains(pkm.Species) || slot.Form == pkm.AltForm).ToList();
            if (gen <= 5)
            {
                slotdata.AddRange(eslots);
                return slotdata;
            }
            if (slotMax != null)
                eslots.Add(slotMax);
            foreach (EncounterSlot s in eslots)
            {
                bool nav = s.AllowDexNav && (pkm.RelearnMove1 != 0 || pkm.AbilityNumber == 4);
                EncounterSlot slot = s.Clone();
                slot.DexNav = nav;

                if (slot.LevelMin > lvl)
                    slot.WhiteFlute = true;
                if (slot.LevelMax + 1 <= lvl && lvl <= slot.LevelMax + fluteBoost)
                    slot.BlackFlute = true;
                if (slot.LevelMax != lvl && slot.AllowDexNav)
                    slot.DexNav = true;
                slotdata.Add(slot);
            }
            return slotdata;
        }

        private static IEnumerable<EncounterArea> getEncounterSlots(PKM pkm, int lvl = -1, GameVersion gameSource = GameVersion.Any)
        {
            if (gameSource == GameVersion.Any)
                gameSource = (GameVersion)pkm.Version;

            return getSlots(pkm, getEncounterTable(pkm, gameSource), lvl);
        }
        private static IEnumerable<EncounterArea> getEncounterAreas(PKM pkm, GameVersion gameSource = GameVersion.Any)
        {
            if (gameSource == GameVersion.Any)
                gameSource = (GameVersion)pkm.Version;

            var slots = getEncounterSlots(pkm, gameSource: gameSource);
            bool noMet = !pkm.HasOriginalMetLocation || pkm.Format == 2 && gameSource != GameVersion.C;
            return noMet ? slots : slots.Where(area => area.Location == pkm.Met_Location);
        }
        private static IEnumerable<EncounterArea> getSlots(PKM pkm, IEnumerable<EncounterArea> tables, int lvl = -1)
        {
            IEnumerable<DexLevel> vs = getValidPreEvolutions(pkm, lvl: lvl);
            foreach (var loc in tables)
            {
                IEnumerable<EncounterSlot> slots = loc.Slots.Where(slot => vs.Any(evo => evo.Species == slot.Species));

                EncounterSlot[] es = slots.ToArray();
                if (es.Length > 0)
                    yield return new EncounterArea { Location = loc.Location, Slots = es };
            }
        }

        // EncounterLink
        private static IEnumerable<EncounterLink> getValidLinkGifts(PKM pkm)
        {
            switch (pkm.GenNumber)
            {
                case 6:
                    return LinkGifts6.Where(g => g.Species == pkm.Species && g.Level == pkm.Met_Level);
                default:
                    return new EncounterLink[0];
            }
        }

        // EncounterTrade
        private static EncounterTrade[] getEncounterTradeTable(PKM pkm)
        {
            switch (pkm.GenNumber)
            {
                case 3:
                    return pkm.FRLG ? TradeGift_FRLG : TradeGift_RSE;
                case 4:
                    return pkm.HGSS ? TradeGift_HGSS : TradeGift_DPPt;
                case 5:
                    return pkm.B2W2 ? TradeGift_B2W2 : TradeGift_BW;
                case 6:
                    return pkm.XY ? TradeGift_XY : TradeGift_AO;
                case 7:
                    return pkm.SM ? TradeGift_SM : null;
            }
            return null;
        }
        private static IEnumerable<EncounterTrade> getValidEncounterTradeVC(PKM pkm, GameVersion gameSource)
        {
            var p = getValidPreEvolutions(pkm).ToArray();

            switch (gameSource)
            {
                case GameVersion.RBY:
                    var table = !AllowGen1Tradeback ? TradeGift_RBY_NoTradeback : TradeGift_RBY_Tradeback;
                    return getValidEncounterTradeVC1(pkm, p, table);
                case GameVersion.GSC:
                case GameVersion.C:
                    return getValidEncounterTradeVC2(pkm, p);
                default:
                    return null;
            }
        }
        private static IEnumerable<EncounterTrade> getValidEncounterTradeVC2(PKM pkm, DexLevel[] p)
        {
            // Check GSC trades. Reuse generic table fetch-match
            var possible = getValidEncounterTradeVC1(pkm, p, TradeGift_GSC);

            foreach (var z in possible)
            {
                // Filter Criteria
                if (z.TID != pkm.TID)
                    continue;
                if (z.Gender != pkm.Gender && pkm.Format <= 2)
                    continue;
                if (!z.IVs.SequenceEqual(pkm.IVs) && pkm.Format <= 2)
                    continue;
                if (pkm.Met_Location != 0 && pkm.Format == 2 && pkm.Met_Location != z.Location)
                    continue;

                int index = Array.IndexOf(TradeGift_GSC, z);
                if (TradeGift_GSC_OTs[index].All(ot => ot != pkm.OT_Name))
                    continue;

                yield return z;
            }
        }
        private static IEnumerable<EncounterTrade> getValidEncounterTradeVC1(PKM pkm, DexLevel[] p, IEnumerable<EncounterTrade> table)
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
                if (z is EncounterTradeCatchRate r && rate != r.Catch_Rate)
                    continue;
                if (rate != PersonalTable.RB[z.Species].CatchRate && rate != PersonalTable.Y[z.Species].CatchRate)
                    continue;

                yield return z;
            }
        }
        private static IEnumerable<EncounterTrade> getValidEncounterTrade(PKM pkm, GameVersion gameSource = GameVersion.Any)
        {
            if (gameSource == GameVersion.Any)
                gameSource = (GameVersion)pkm.Version;

            if (pkm.VC || pkm.Format <= 2)
            {
                foreach (var z in getValidEncounterTradeVC(pkm, gameSource))
                    yield return z;
                yield break;
            }

            int lang = pkm.Language;
            if (lang == 0 || lang == 6)
                yield break;

            int lvl = getMinLevelEncounter(pkm);
            if (lvl <= 0)
                yield break;

            // Get valid pre-evolutions
            IEnumerable<DexLevel> p = getValidPreEvolutions(pkm);

            EncounterTrade[] table = getEncounterTradeTable(pkm);
            if (table == null)
                yield break;
            var poss = table.Where(f => p.Any(r => r.Species == f.Species) && f.Version.Contains((GameVersion)pkm.Version));

            foreach (var z in poss)
            {
                if (getEncounterTradeValid(pkm, z, lvl))
                    yield return z;
            }
        }
        private static bool getEncounterTradeValid(PKM pkm, EncounterTrade z, int lvl)
        {
            for (int i = 0; i < 6; i++)
                if (z.IVs[i] != -1 && z.IVs[i] != pkm.IVs[i])
                    return false;

            if (z.Shiny ^ pkm.IsShiny) // Are PIDs static?
                return false;
            if (z.TID != pkm.TID)
                return false;
            if (z.SID != pkm.SID)
                return false;
            if (pkm.HasOriginalMetLocation)
            {
                z.Location = z.Location > 0 ? z.Location : EncounterTrade.DefaultMetLocation[pkm.GenNumber - 3];
                if (z.Location != pkm.Met_Location)
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

            if (z.Nature != Nature.Random && (int)z.Nature != pkm.Nature)
                return false;
            if (z.Gender != -1 && z.Gender != pkm.Gender)
                return false;
            if (z.OTGender != -1 && z.OTGender != pkm.OT_Gender)
                return false;
            // if (z.Ability == 4 ^ pkm.AbilityNumber == 4) // defer to Ability 
            //    countinue;

            return true;
        }

        // MysteryGift
        private static IEnumerable<MysteryGift> getValidGifts(PKM pkm)
        {
            switch (pkm.GenNumber)
            {
                case 3:
                    return getMatchingWC3(pkm, MGDB_G3);
                case 4:
                    return getMatchingPCD(pkm, MGDB_G4);
                case 5:
                    return getMatchingPGF(pkm, MGDB_G5);
                case 6:
                    return getMatchingWC6(pkm, MGDB_G6);
                case 7:
                    return getMatchingWC7(pkm, MGDB_G7);
                default:
                    return new List<MysteryGift>();
            }
        }
        private static IEnumerable<MysteryGift> getMatchingWC3(PKM pkm, IEnumerable<MysteryGift> DB)
        {
            if (DB == null)
                yield break;

            var validWC3 = new List<MysteryGift>();
            var vs = getValidPreEvolutions(pkm, MaxSpeciesID_3).ToArray();
            foreach (WC3 wc in DB.OfType<WC3>().Where(wc => vs.Any(dl => dl.Species == wc.Species)))
            {
                // Gen3 Version MUST match.
                if (wc.Version != 0 && !((GameVersion)wc.Version).Contains((GameVersion)pkm.Version))
                    continue;

                if (!wc.IsEgg)
                {
                    if (wc.SID != pkm.SID) continue;
                    if (wc.TID != pkm.TID) continue;
                    if (wc.OT_Name != pkm.OT_Name) continue;
                    if (wc.OT_Gender < 3 && wc.OT_Gender != pkm.OT_Gender) continue;
                    if (wc.Language != 0 && wc.Language != pkm.Language) continue;

                    if (wc.Met_Location != pkm.Met_Location && pkm.HasOriginalMetLocation)
                        continue;
                }
                else if (wc.IsEgg)
                {
                    if (pkm.IsNative)
                    {
                        if (wc.Level != pkm.Met_Level)
                            continue;
                    }
                    else
                    {
                        if (wc.Level > pkm.Met_Level)
                            continue;
                        if (pkm.IsEgg)
                            continue;
                    }
                }
                
                if (wc.Ball != pkm.Ball) continue;

                // Some checks are best performed separately as they are caused by users screwing up valid data.
                // if (wc.Level > pkm.CurrentLevel) continue; // Defer to level legality
                // RIBBONS: Defer to ribbon legality

                if (wc.Species == pkm.Species) // best match
                    yield return wc;
                else
                    validWC3.Add(wc);
            }
            foreach (var z in validWC3)
                yield return z;
        }
        private static IEnumerable<MysteryGift> getMatchingPCD(PKM pkm, IEnumerable<MysteryGift> DB)
        {
            if (DB == null)
                yield break;

            if (pkm.Species == 490 && (pkm.WasEgg || pkm.IsEgg)) // Manaphy
            {
                int loc = pkm.IsEgg ? pkm.Met_Location : pkm.Egg_Location;
                bool valid = loc == 2002; // Link Trade Egg
                valid |= loc == 3001 && !pkm.IsShiny; // Ranger & notShiny
                if (pkm.IsEgg && !pkm.IsNative) // transferred
                    valid = false;
                if (valid)
                    yield return new PGT { Data = { [0] = 7, [8] = 1 } };
                yield break;
            }

            var validPCD = new List<MysteryGift>();
            var vs = getValidPreEvolutions(pkm).ToArray();
            foreach (PCD mg in DB.OfType<PCD>().Where(wc => vs.Any(dl => dl.Species == wc.Species)))
            {
                var wc = mg.Gift.PK;
                if (pkm.Egg_Location == 0) // Not Egg
                {
                    if (wc.SID != pkm.SID) continue;
                    if (wc.TID != pkm.TID) continue;
                    if (wc.OT_Name != pkm.OT_Name) continue;
                    if (wc.OT_Gender != pkm.OT_Gender) continue;
                    if (wc.Version != 0 && wc.Version != pkm.Version) continue;
                    if (wc.Language != 0 && wc.Language != pkm.Language) continue;
                }
                if (wc.AltForm != pkm.AltForm && vs.All(dl => !getCanFormChange(pkm, dl.Species))) continue;

                if (wc.IsEgg)
                {
                    if (wc.Egg_Location + 3000 != pkm.Egg_Location && pkm.Egg_Location != 2002) // traded
                        continue;
                    if (wc.CurrentLevel != pkm.Met_Level)
                        continue;
                    if (pkm.IsEgg && !pkm.IsNative)
                        continue;
                }
                else
                {
                    if (pkm.Format != 4) // transferred
                    {
                        // met location: deferred to general transfer check
                        if (wc.CurrentLevel > pkm.Met_Level) continue;
                    }
                    else
                    {
                        if (wc.Egg_Location + 3000 != pkm.Met_Location) continue;
                        if (wc.CurrentLevel != pkm.Met_Level) continue;
                    }
                }

                if (wc.Ball != pkm.Ball) continue;
                if (wc.OT_Gender < 3 && wc.OT_Gender != pkm.OT_Gender) continue;
                if (wc.PID == 1 && pkm.IsShiny) continue;
                if (wc.Gender != 3 && wc.Gender != pkm.Gender) continue;

                if (wc.CNT_Cool > pkm.CNT_Cool) continue;
                if (wc.CNT_Beauty > pkm.CNT_Beauty) continue;
                if (wc.CNT_Cute > pkm.CNT_Cute) continue;
                if (wc.CNT_Smart > pkm.CNT_Smart) continue;
                if (wc.CNT_Tough > pkm.CNT_Tough) continue;
                if (wc.CNT_Sheen > pkm.CNT_Sheen) continue;

                // Some checks are best performed separately as they are caused by users screwing up valid data.
                // if (wc.Level > pkm.CurrentLevel) continue; // Defer to level legality
                // RIBBONS: Defer to ribbon legality

                if (wc.Species == pkm.Species) // best match
                    yield return mg;
                else
                    validPCD.Add(mg);
            }
            foreach (var z in validPCD)
                yield return z;
        }
        private static IEnumerable<MysteryGift> getMatchingPGF(PKM pkm, IEnumerable<MysteryGift> DB)
        {
            if (DB == null)
                yield break;

            var validPGF = new List<MysteryGift>();
            var vs = getValidPreEvolutions(pkm).ToArray();
            foreach (PGF wc in DB.OfType<PGF>().Where(wc => vs.Any(dl => dl.Species == wc.Species)))
            {
                if (pkm.Egg_Location == 0) // Not Egg
                {
                    if (wc.SID != pkm.SID) continue;
                    if (wc.TID != pkm.TID) continue;
                    if (wc.OT != pkm.OT_Name) continue;
                    if (wc.PID != 0 && pkm.PID != wc.PID) continue;
                    if (wc.PIDType == 0 && pkm.IsShiny) continue;
                    if (wc.PIDType == 2 && !pkm.IsShiny) continue;
                    if (wc.OriginGame != 0 && wc.OriginGame != pkm.Version) continue;
                    if (wc.Language != 0 && wc.Language != pkm.Language) continue;
                }
                if (wc.Form != pkm.AltForm && vs.All(dl => !getCanFormChange(pkm, dl.Species))) continue;

                if (wc.IsEgg)
                {
                    if (wc.EggLocation != pkm.Egg_Location && pkm.Egg_Location != 30002) // traded
                        continue;
                    if (pkm.IsEgg && !pkm.IsNative)
                        continue;
                }
                else
                {
                    if (wc.EggLocation != pkm.Egg_Location) continue;
                    if (wc.MetLocation != pkm.Met_Location) continue;
                }

                if (wc.Level != pkm.Met_Level) continue;
                if (wc.Ball != pkm.Ball) continue;
                if (wc.OTGender < 3 && wc.OTGender != pkm.OT_Gender) continue;
                if (wc.Nature != 0xFF && wc.Nature != pkm.Nature) continue;
                if (wc.Gender != 2 && wc.Gender != pkm.Gender) continue;

                if (wc.CNT_Cool > pkm.CNT_Cool) continue;
                if (wc.CNT_Beauty > pkm.CNT_Beauty) continue;
                if (wc.CNT_Cute > pkm.CNT_Cute) continue;
                if (wc.CNT_Smart > pkm.CNT_Smart) continue;
                if (wc.CNT_Tough > pkm.CNT_Tough) continue;
                if (wc.CNT_Sheen > pkm.CNT_Sheen) continue;

                // Some checks are best performed separately as they are caused by users screwing up valid data.
                // if (wc.Level > pkm.CurrentLevel) continue; // Defer to level legality
                // RIBBONS: Defer to ribbon legality

                if (wc.Species == pkm.Species) // best match
                    yield return wc;
                else
                    validPGF.Add(wc);
            }
            foreach (var z in validPGF)
                yield return z;
        }
        private static IEnumerable<MysteryGift> getMatchingWC6(PKM pkm, IEnumerable<MysteryGift> DB)
        {
            if (DB == null)
                yield break;
            List<MysteryGift> validWC6 = new List<MysteryGift>();
            var vs = getValidPreEvolutions(pkm).ToArray();
            foreach (WC6 wc in DB.OfType<WC6>().Where(wc => vs.Any(dl => dl.Species == wc.Species)))
            {
                if (pkm.Egg_Location == 0) // Not Egg
                {
                    if (wc.CardID != pkm.SID) continue;
                    if (wc.TID != pkm.TID) continue;
                    if (wc.OT != pkm.OT_Name) continue;
                    if (wc.OTGender != pkm.OT_Gender) continue;
                    if (wc.PIDType == 0 && pkm.PID != wc.PID) continue;
                    if (wc.PIDType == 2 && !pkm.IsShiny) continue;
                    if (wc.PIDType == 3 && pkm.IsShiny) continue;
                    if (wc.OriginGame != 0 && wc.OriginGame != pkm.Version) continue;
                    if (wc.EncryptionConstant != 0 && wc.EncryptionConstant != pkm.EncryptionConstant) continue;
                    if (wc.Language != 0 && wc.Language != pkm.Language) continue;
                }
                if (wc.Form != pkm.AltForm && vs.All(dl => !getCanFormChange(pkm, dl.Species))) continue;

                if (wc.IsEgg)
                {
                    if (wc.EggLocation != pkm.Egg_Location && pkm.Egg_Location != 30002) // traded
                        continue;
                    if (pkm.IsEgg && !pkm.IsNative)
                        continue;
                }
                else
                {
                    if (wc.EggLocation != pkm.Egg_Location) continue;
                    if (wc.MetLocation != pkm.Met_Location) continue;
                }

                if (wc.Level != pkm.Met_Level) continue;
                if (wc.Ball != pkm.Ball) continue;
                if (wc.OTGender < 3 && wc.OTGender != pkm.OT_Gender) continue;
                if (wc.Nature != 0xFF && wc.Nature != pkm.Nature) continue;
                if (wc.Gender != 3 && wc.Gender != pkm.Gender) continue;

                if (wc.CNT_Cool > pkm.CNT_Cool) continue;
                if (wc.CNT_Beauty > pkm.CNT_Beauty) continue;
                if (wc.CNT_Cute > pkm.CNT_Cute) continue;
                if (wc.CNT_Smart > pkm.CNT_Smart) continue;
                if (wc.CNT_Tough > pkm.CNT_Tough) continue;
                if (wc.CNT_Sheen > pkm.CNT_Sheen) continue;

                // Some checks are best performed separately as they are caused by users screwing up valid data.
                // if (!wc.RelearnMoves.SequenceEqual(pkm.RelearnMoves)) continue; // Defer to relearn legality
                // if (wc.OT.Length > 0 && pkm.CurrentHandler != 1) continue; // Defer to ownership legality
                // if (wc.OT.Length > 0 && pkm.OT_Friendship != PKX.getBaseFriendship(pkm.Species)) continue; // Friendship
                // if (wc.Level > pkm.CurrentLevel) continue; // Defer to level legality
                // RIBBONS: Defer to ribbon legality

                if (wc.Species == pkm.Species) // best match
                    yield return wc;
                else
                    validWC6.Add(wc);
            }
            foreach (var z in validWC6)
                yield return z;
        }
        private static IEnumerable<MysteryGift> getMatchingWC7(PKM pkm, IEnumerable<MysteryGift> DB)
        {
            if (DB == null)
                yield break;
            List<MysteryGift> validWC7 = new List<MysteryGift>();
            var vs = getValidPreEvolutions(pkm).ToArray();
            foreach (WC7 wc in DB.OfType<WC7>().Where(wc => vs.Any(dl => dl.Species == wc.Species)))
            {
                if (pkm.Egg_Location == 0) // Not Egg
                {
                    if (wc.OTGender != 3)
                    {
                        if (wc.SID != pkm.SID) continue;
                        if (wc.TID != pkm.TID) continue;
                        if (wc.OTGender != pkm.OT_Gender) continue;
                    }
                    if (!string.IsNullOrEmpty(wc.OT) && wc.OT != pkm.OT_Name) continue;
                    if (wc.OriginGame != 0 && wc.OriginGame != pkm.Version) continue;
                    if (wc.EncryptionConstant != 0 && wc.EncryptionConstant != pkm.EncryptionConstant) continue;
                    if (wc.Language != 0 && wc.Language != pkm.Language) continue;
                }
                if (wc.Form != pkm.AltForm && vs.All(dl => !getCanFormChange(pkm, dl.Species))) continue;

                if (wc.IsEgg)
                {
                    if (wc.EggLocation != pkm.Egg_Location && pkm.Egg_Location != 30002) // traded
                        continue;
                    if (pkm.IsEgg && !pkm.IsNative)
                        continue;
                }
                else
                {
                    if (wc.EggLocation != pkm.Egg_Location) continue;
                    if (wc.MetLocation != pkm.Met_Location) continue;
                }

                if (wc.MetLevel != pkm.Met_Level) continue;
                if (wc.Ball != pkm.Ball) continue;
                if (wc.OTGender < 3 && wc.OTGender != pkm.OT_Gender) continue;
                if (wc.Nature != 0xFF && wc.Nature != pkm.Nature) continue;
                if (wc.Gender != 3 && wc.Gender != pkm.Gender) continue;

                if (wc.CNT_Cool > pkm.CNT_Cool) continue;
                if (wc.CNT_Beauty > pkm.CNT_Beauty) continue;
                if (wc.CNT_Cute > pkm.CNT_Cute) continue;
                if (wc.CNT_Smart > pkm.CNT_Smart) continue;
                if (wc.CNT_Tough > pkm.CNT_Tough) continue;
                if (wc.CNT_Sheen > pkm.CNT_Sheen) continue;

                if (wc.PIDType == 2 && !pkm.IsShiny) continue;
                if (wc.PIDType == 3 && pkm.IsShiny) continue;

                if ((pkm.SID << 16 | pkm.TID) == 0x79F57B49) // Greninja WC has variant PID and can arrive @ 36 or 37
                {
                    if (!pkm.IsShiny)
                        validWC7.Add(wc);
                    continue;
                }
                if (wc.PIDType == 0 && pkm.PID != wc.PID) continue;

                // Some checks are best performed separately as they are caused by users screwing up valid data.
                // if (!wc.RelearnMoves.SequenceEqual(pkm.RelearnMoves)) continue; // Defer to relearn legality
                // if (wc.OT.Length > 0 && pkm.CurrentHandler != 1) continue; // Defer to ownership legality
                // if (wc.OT.Length > 0 && pkm.OT_Friendship != PKX.getBaseFriendship(pkm.Species)) continue; // Friendship
                // if (wc.Level > pkm.CurrentLevel) continue; // Defer to level legality
                // RIBBONS: Defer to ribbon legality

                if (wc.Species == pkm.Species) // best match
                    yield return wc;
                else
                    validWC7.Add(wc);
            }
            foreach (var z in validWC7)
                yield return z;
        }

        // EncounterEgg
        private static IEnumerable<EncounterEgg> generateEggs(PKM pkm)
        {
            if (NoHatchFromEgg.Contains(pkm.Species))
                yield break;
            
            int lvl = pkm.GenNumber < 4 ? 5 : 1;
            var ver = (GameVersion) pkm.Version; // version is a true indicator for all generation 3+ origins
            yield return new EncounterEgg { Game = (GameVersion)pkm.Version, Level = lvl, Species = getBaseSpecies(pkm, 0) };

            if (getSplitBreedGeneration(pkm).Contains(pkm.Species))
                yield return new EncounterEgg { Game = ver, Level = lvl, Species = getBaseSpecies(pkm, 1), SplitBreed = true };
        }

        // Utility
        internal static bool IsHiddenAbilitySlot(EncounterSlot slot)
        {
            return slot.DexNav || slot.Type == SlotType.FriendSafari || slot.Type == SlotType.Horde || slot.Type == SlotType.SOS;
        }
        internal static bool IsSafariSlot(SlotType t)
        {
            return t == SlotType.Grass_Safari || t == SlotType.Surf_Safari ||
                   t == SlotType.Rock_Smash_Safari || t == SlotType.Pokeradar_Safari ||
                   t == SlotType.Old_Rod_Safari || t == SlotType.Good_Rod_Safari || t == SlotType.Super_Rod_Safari;
        }
        internal static bool getDexNavValid(PKM pkm)
        {
            if (!pkm.AO || !pkm.InhabitedGeneration(6))
                return false;

            IEnumerable<EncounterArea> locs = getDexNavAreas(pkm);
            return locs.Select(loc => getValidEncounterSlots(pkm, loc, DexNav: true)).Any(slots => slots.Any(slot => slot.AllowDexNav && slot.DexNav));
        }
        internal static EncounterArea getCaptureLocation(PKM pkm)
        {
            return (from area in getEncounterSlots(pkm, 100)
                let slots = getValidEncounterSlots(pkm, area, pkm.AO, ignoreLevel: true).ToArray()
                where slots.Any()
                select new EncounterArea
                {
                    Location = area.Location,
                    Slots = slots,
                }).OrderBy(area => area.Slots.Min(x => x.LevelMin)).FirstOrDefault();
        }
        internal static EncounterStatic getStaticLocation(PKM pkm, int species = -1)
        {
            switch (pkm.GenNumber)
            {
                case 1:
                    return getRBYStaticTransfer(species);
                default:
                    return getStaticEncounters(pkm, 100).OrderBy(s => s.Level).FirstOrDefault();
            }
        }
        internal static EncounterStatic getRBYStaticTransfer(int species)
        {
            return new EncounterStatic
            {
                Species = species,
                Gift = true, // Forces Poké Ball
                Ability = TransferSpeciesDefaultAbility_1.Contains(species) ? 1 : 4, // Hidden by default, else first
                Shiny = species == 151 ? (bool?)false : null,
                Fateful = species == 151,
                Location = 30013,
                EggLocation = 0,
                IV3 = true,
                Version = GameVersion.RBY
            };
        }
        internal static EncounterStatic getGSStaticTransfer(int species)
        {
            return new EncounterStatic
            {
                Species = species,
                Gift = true, // Forces Poké Ball
                Ability = TransferSpeciesDefaultAbility_2.Contains(species) ? 1 : 4, // Hidden by default, else first
                Shiny = species == 151 || species == 251 ? (bool?)false : null,
                Fateful = species == 151 || species == 251,
                Location = 30004, // todo
                EggLocation = 0,
                IV3 = true,
                Version = GameVersion.GS
            };
        }
        internal static bool getEncounterTrade1Valid(PKM pkm)
        {
            string ot = pkm.OT_Name;
            string tr = pkm.Format <= 2 ? "TRAINER" : "Trainer"; // decaps on transfer
            return ot == "トレーナー" || ot == tr;
        }
        private static bool getWurmpleEvoValid(PKM pkm)
        {
            uint evoVal = PKX.getWurmpleEvoVal(pkm.GenNumber, pkm.EncryptionConstant);
            int wIndex = Array.IndexOf(WurmpleEvolutions, pkm.Species) / 2;
            return evoVal == wIndex;
        }
    }
}
