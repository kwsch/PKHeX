using System;
using System.Collections.Generic;
using System.Linq;
using static PKHeX.Core.Legal;

namespace PKHeX.Core
{
    /// <summary>
    /// Generates matching <see cref="IEncounterable"/> data and relevant <see cref="LegalInfo"/> for a <see cref="PKM"/>.
    /// Logic for generating possible in-game encounter data.
    /// </summary>
    public static class EncounterGenerator
    {
        /// <summary>
        /// Generates possible <see cref="IEncounterable"/> data according to the input PKM data and legality info.
        /// </summary>
        /// <param name="pkm">PKM data</param>
        /// <param name="info">Legality information</param>
        /// <returns>Possible encounters</returns>
        /// <remarks>
        /// The iterator lazily finds possible encounters. If no encounters are possible, the enumerable will be empty.
        /// </remarks>
        public static IEnumerable<IEncounterable> GetEncounters(PKM pkm, LegalInfo info)
        {
            switch (info.Generation)
            {
                case 1:
                case 2: return GetEncounters12(pkm, info);
                case 3: return GetEncounters3(pkm, info);
                case 4: return GetEncounters4(pkm, info);
                default: return GenerateRawEncounters(pkm);
            }
        }

        private static IEnumerable<IEncounterable> GetEncounters12(PKM pkm, LegalInfo info)
        {
            int baseSpecies = GetBaseSpecies(pkm);
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
        private static IEnumerable<IEncounterable> GetEncounters3(PKM pkm, LegalInfo info)
        {
            info.PIDIV = MethodFinder.Analyze(pkm);
            var deferred = new List<IEncounterable>();
            foreach (var z in GenerateRawEncounters3(pkm, info))
            {
                if (z is EncounterSlot w && pkm.Version == 15)
                    info.PIDIV = MethodFinder.GetPokeSpotSeeds(pkm, w.SlotNumber).FirstOrDefault() ?? info.PIDIV; 
                if (info.PIDIV.Type.IsCompatible3(z, pkm))
                    yield return z;
                else
                    deferred.Add(z);
            }
            if (deferred.Count == 0)
                yield break;

            info.PIDIVMatches = false;
            foreach (var z in deferred)
                yield return z;
        }
        private static IEnumerable<IEncounterable> GetEncounters4(PKM pkm, LegalInfo info)
        {
            info.PIDIV = MethodFinder.Analyze(pkm);
            var deferredPIDIV = new List<IEncounterable>();
            var deferredEType = new List<IEncounterable>();

            foreach (var z in GenerateRawEncounters4(pkm, info))
            {
                if (!info.PIDIV.Type.IsCompatible4(z, pkm))
                    deferredPIDIV.Add(z);
                else if (pkm.Format <= 6 && !IsEncounterTypeMatch(z, pkm.EncounterType))
                    deferredEType.Add(z);
                else
                    yield return z;
            }

            foreach (var z in deferredEType)
                yield return z;

            if (deferredPIDIV.Count == 0)
                yield break;

            info.PIDIVMatches = false;
            foreach (var z in deferredPIDIV)
                yield return z;
        }
        private static IEnumerable<GBEncounterData> GenerateRawEncounters12(PKM pkm, GameVersion game)
        {
            bool gsc = GameVersion.GSC.Contains(game);
            var gen = gsc ? 2 : 1;

            // Since encounter matching is super weak due to limited stored data in the structure
            // Calculate all 3 at the same time and pick the best result (by species).
            // Favor special event move gifts as Static Encounters when applicable
            var maxspeciesorigin = gsc ? MaxSpeciesID_2 : MaxSpeciesID_1;
            DexLevel[] vs = GetValidPreEvolutions(pkm, maxspeciesorigin: maxspeciesorigin).ToArray();
            HashSet<int> species = new HashSet<int>(vs.Select(p => p.Species).ToList());

            var deferred = new List<IEncounterable>();
            foreach (var t in GetValidEncounterTrades(pkm, game))
            {
                if (pkm.Format >= 7)
                {
                    deferred.Add(t);
                    continue;
                }
                yield return new GBEncounterData(pkm, gen, t, t.Version);
            }
            foreach (var s in GetValidStaticEncounter(pkm, game).Where(z => species.Contains(z.Species)))
            {
                // Valid stadium and non-stadium encounters, return only non-stadium encounters, they are less restrictive
                switch (s.Version)
                {
                    case GameVersion.Stadium:
                    case GameVersion.Stadium2:
                        deferred.Add(s);
                        continue;
                    case GameVersion.EventsGBGen2:
                        if (!s.EggEncounter && !pkm.HasOriginalMetLocation)
                            continue;
                        if (pkm.Japanese)
                            deferred.Add(s);
                        continue;
                    case GameVersion.C when gsc && pkm.Format == 2: // Crystal specific data needs to be present
                        if (!s.EggEncounter && !pkm.HasOriginalMetLocation)
                            continue;
                        if (s.Species == 251 && AllowGBCartEra) // no celebi, the GameVersion.EventsGBGen2 will pass thru
                            continue;
                        break;
                }
                yield return new GBEncounterData(pkm, gen, s, s.Version);
            }
            foreach (var e in GetValidWildEncounters(pkm, game).OfType<EncounterSlot1>())
            {
                if (!species.Contains(e.Species))
                    continue;
                yield return new GBEncounterData(pkm, gen, e, e.Version);
            }

            if (gsc)
            {
                bool WasEgg = !pkm.Gen1_NotTradeback && GetWasEgg23(pkm) && !NoHatchFromEgg.Contains(pkm.Species);
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
                    int eggspec = GetBaseEggSpecies(pkm);
                    if (AllowGen2Crystal(pkm))
                        yield return new GBEncounterData(eggspec, GameVersion.C); // gen2 egg
                    yield return new GBEncounterData(eggspec, GameVersion.GS); // gen2 egg
                }
            }

            foreach (var d in deferred)
                yield return new GBEncounterData(pkm, gen, d, game);
        }
        private static IEnumerable<GBEncounterData> GenerateFilteredEncounters(PKM pkm)
        {
            bool crystal = pkm.Format == 2 && pkm.Met_Location != 0 || pkm.Format >= 7 && pkm.OT_Gender == 1;
            var g1i = new PeekEnumerator<GBEncounterData>(get1());
            var g2i = new PeekEnumerator<GBEncounterData>(get2());
            var deferred = new List<GBEncounterData>();
            while (g2i.PeekIsNext() || g1i.PeekIsNext())
            {
                var move = GetPreferredGBIterator(g1i, g2i);
                var obj = move.Peek();

                if (obj.Generation == 1 && obj.Encounter is EncounterTrade && !IsEncounterTrade1Valid(pkm))
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
                if (!pkm.Gen1_NotTradeback)
                    foreach (var z in GenerateRawEncounters12(pkm, crystal ? GameVersion.C : GameVersion.GSC))
                        yield return z;
            }
        }
        /// <summary>
        /// Gets the preferred iterator from a pair of <see cref="GBEncounterData"/> iterators based on the highest value <see cref="GBEncounterData.Type"/>.
        /// </summary>
        /// <param name="g1i">Generation 1 Iterator</param>
        /// <param name="g2i">Generation 2 Iterator</param>
        /// <returns>Preferred iterator </returns>
        private static PeekEnumerator<GBEncounterData> GetPreferredGBIterator(PeekEnumerator<GBEncounterData> g1i, PeekEnumerator<GBEncounterData> g2i)
        {
            if (!g1i.PeekIsNext())
                return g2i;
            if (!g2i.PeekIsNext())
                return g1i;
            return g1i.Peek().Type > g2i.Peek().Type ? g1i : g2i;
        }

        private static IEnumerable<IEncounterable> GenerateRawEncounters(PKM pkm)
        {
            int ctr = 0;
            if (pkm.WasLink)
            {
                foreach (var z in GetValidLinkGifts(pkm))
                { yield return z; ++ctr; }
                if (ctr != 0) yield break;
            }

            if (pkm.WasEvent || pkm.WasEventEgg)
            {
                foreach (var z in GetValidGifts(pkm))
                { yield return z; ++ctr; }
                if (ctr != 0) yield break;
            }

            if (pkm.WasEgg)
            {
                foreach (var z in GenerateEggs(pkm))
                { yield return z; ++ctr; }
                if (ctr == 0) yield break;
            }

            foreach (var z in GetValidStaticEncounter(pkm))
            { yield return z; ++ctr; }
            if (ctr != 0) yield break;
            foreach (var z in GetValidFriendSafari(pkm))
            { yield return z; ++ctr; }
            if (ctr != 0) yield break;
            foreach (var z in GetValidWildEncounters(pkm))
            { yield return z; ++ctr; }
            if (ctr != 0) yield break;
            foreach (var z in GetValidEncounterTrades(pkm))
            { yield return z; ++ctr; }
            // if (ctr != 0) yield break;
        }
        private static IEnumerable<IEncounterable> GenerateRawEncounters4(PKM pkm, LegalInfo info)
        {
            bool wasEvent = pkm.WasEvent || pkm.WasEventEgg; // egg events?
            if (wasEvent)
            {
                int ctr = 0;
                foreach (var z in GetValidGifts(pkm))
                { yield return z; ++ctr; }
                if (ctr != 0) yield break;
            }
            if (pkm.WasEgg)
            {
                foreach (var z in GenerateEggs(pkm))
                    yield return z;
            }
            foreach (var z in GetValidEncounterTrades(pkm))
                yield return z;

            var deferred = new LinkedList<IEncounterable>();
            bool sport = pkm.Ball == 0x18; // never static encounters (conflict with non bcc / bcc)
            bool safari = pkm.Ball == 0x05; // never static encounters
            bool safariSport = safari || sport;
            if (!safariSport)
            foreach (var z in GetValidStaticEncounter(pkm))
            {
                if (z.Gift && pkm.Ball != 4)
                    deferred.AddLast(z);
                else
                    yield return z;
            }
            
            var slots = FrameFinder.GetFrames(info.PIDIV, pkm).ToList();
            foreach (var z in GetValidWildEncounters(pkm))
            {
                if (sport != z.Type.HasFlag(SlotType.BugContest))
                {
                    deferred.AddLast(z);
                    continue;
                }

                var frame = slots.FirstOrDefault(s => s.IsSlotCompatibile(z, pkm));
                if (frame != null || pkm.Species == 201) // Unown -- don't really care to figure this out
                    yield return z;
                else
                    deferred.AddFirst(z);
            }
            info.FrameMatches = false;

            foreach (var z in deferred)
                yield return z;

            // do static encounters if they were deferred to end, spit out any possible encounters for invalid pkm
            if (safariSport)
            foreach (var z in GetValidStaticEncounter(pkm))
                yield return z;
        }
        private static IEnumerable<IEncounterable> GenerateRawEncounters3(PKM pkm, LegalInfo info)
        {
            foreach (var z in GetValidGifts(pkm))
                yield return z;
            foreach (var z in GetValidEncounterTrades(pkm))
                yield return z;

            var deferred = new Queue<IEncounterable>();
            bool safari = pkm.Ball == 0x05; // never static encounters
            if (!safari)
            foreach (var z in GetValidStaticEncounter(pkm))
            {
                if (z.Gift && pkm.Ball != 4)
                    deferred.Enqueue(z);
                else
                    yield return z;
            }
            var slots = FrameFinder.GetFrames(info.PIDIV, pkm).ToList();
            foreach (var z in GetValidWildEncounters(pkm))
            {
                var frame = slots.FirstOrDefault(s => s.IsSlotCompatibile(z, pkm));
                if (frame != null)
                    yield return z;
                else
                    deferred.Enqueue(z);
            }
            info.FrameMatches = false;

            if (pkm.Version != 15) // no eggs in C/XD
            foreach (var z in GenerateEggs(pkm))
                yield return z;

            foreach (var z in deferred)
                yield return z;
            // do static encounters if they were deferred to end, spit out any possible encounters for invalid pkm
            if (safari)
            foreach (var z in GetValidStaticEncounter(pkm))
                yield return z;
        }

        // EncounterStatic
        private static bool IsValidCatchRatePK1(EncounterStatic e, PK1 pk1)
        {
            var catch_rate = pk1.Catch_Rate;
            // Pure gen 1, trades can be filter by catch rate
            if (pk1.Species == 25 || pk1.Species == 26)
            {
                if (catch_rate == 190) // Red Blue Pikachu, is not a static encounter
                    return false;
                if (catch_rate == 163 && e.Level == 5) // Light Ball (Yellow) starter
                    return true;
            }

            if (e.Version == GameVersion.Stadium)
            {
                // Amnesia Psyduck has different catch rates depending on language
                if (e.Species == 054)
                    return catch_rate == (pk1.Japanese ? 167 : 168);
                return Stadium_CatchRate.Contains(catch_rate);
            }

            // Encounters can have different Catch Rates (RBG vs Y)
            var table = e.Version == GameVersion.Y ? PersonalTable.Y : PersonalTable.RB;
            var rate = table[e.Species].CatchRate;
            return catch_rate == rate;
        }
        private static IEnumerable<EncounterStatic> GetValidStaticEncounter(PKM pkm, GameVersion gameSource = GameVersion.Any)
        {
            if (gameSource == GameVersion.Any)
                gameSource = (GameVersion)pkm.Version;

            // Get possible encounters
            IEnumerable<EncounterStatic> poss = GetStaticEncounters(pkm, gameSource: gameSource);

            int lvl = GetMinLevelEncounter(pkm);
            if (lvl < 0)
                yield break;

            // Back Check against pkm
            var enc = GetMatchingStaticEncounters(pkm, poss, lvl);
            foreach (var z in enc)
                yield return z;
        }
        private static IEnumerable<EncounterStatic> GetMatchingStaticEncounters(PKM pkm, IEnumerable<EncounterStatic> poss, int lvl)
        {
            // check for petty rejection scenarios that will be flagged by other legality checks
            var deferred = new List<EncounterStatic>();
            foreach (EncounterStatic e in poss)
            {
                if (!GetIsMatchStatic(pkm, e, lvl))
                    continue;

                if (pkm.FatefulEncounter != e.Fateful)
                    deferred.Add(e);
                else
                    yield return e;
            }
            foreach (var e in deferred)
                yield return e;
        }
        private static bool GetIsMatchStatic(PKM pkm, EncounterStatic e, int lvl)
        {
            if (e.Nature != Nature.Random && pkm.Nature != (int) e.Nature)
                return false;
            if (pkm.WasEgg != e.EggEncounter && pkm.Egg_Location == 0 && pkm.Format > 3 && pkm.GenNumber > 3 && !pkm.IsEgg)
                return false;
            if (e is EncounterStaticPID p && p.PID != pkm.PID)
                return false;

            if (pkm.Gen3 && e.EggLocation != 0) // Gen3 Egg
            {
                if (pkm.Format == 3 && pkm.IsEgg && e.EggLocation != pkm.Met_Location)
                    return false;
            }
            else if (pkm.VC || pkm.GenNumber <= 2 && e.EggLocation != 0) // Gen2 Egg
            {
                if (pkm.Format <= 2)
                {
                    if (pkm.IsEgg)
                    {
                        if (pkm.Met_Location != 0 && pkm.Met_Level != 0)
                            return false;
                    }
                    else
                    {
                        switch (pkm.Met_Level)
                        {
                            case 0 when pkm.Met_Location != 0:
                                return false;
                            case 1 when pkm.Met_Location == 0:
                                return false;
                            default: if (pkm.Met_Location == 0)
                                return false;
                                break;
                        }
                    }
                    if (pkm.Met_Level == 1)
                        lvl = 5; // met @ 1, hatch @ 5.
                }
            }
            else if (e.EggLocation != pkm.Egg_Location)
            {
                if (pkm.IsEgg) // unhatched
                {
                    if (e.EggLocation != pkm.Met_Location)
                        return false;
                    if (pkm.Egg_Location != 0)
                        return false;
                }
                else if (pkm.Gen4)
                {
                    if (pkm.Egg_Location != 2002) // Link Trade
                    {
                        // check Pt/HGSS data
                        if (pkm.Format <= 4)
                            return false; // must match
                        if (e.EggLocation >= 3000 || e.EggLocation <= 2010) // non-Pt/HGSS egg gift
                            return false;

                        // transferring 4->5 clears pt/hgss location value and keeps Faraway Place
                        if (pkm.Egg_Location != 3002) // Faraway Place
                            return false;
                    }
                }
                else
                {
                    if (pkm.Egg_Location != 30002) // Link Trade
                        return false;
                }
            }
            else if (e.EggLocation != 0 && pkm.Gen4)
            {
                // Check the inverse scenario for 4->5 eggs
                if (e.EggLocation < 3000 && e.EggLocation > 2010) // Pt/HGSS egg gift
                {
                    if (pkm.Format > 4)
                        return false; // locations match when it shouldn't
                }
            }

            if (pkm.HasOriginalMetLocation)
            {
                if (!e.EggEncounter && e.Location != 0 && e.Location != pkm.Met_Location)
                    return false;
                if (e.Level != lvl)
                {
                    if (!(pkm.Format == 3 && e.EggEncounter && lvl == 0))
                        return false;
                }
            }
            else if (e.Level > lvl)
                return false;

            if (e.Gender != -1 && e.Gender != pkm.Gender)
                return false;
            if (e.Form != pkm.AltForm && !e.SkipFormCheck && !IsFormChangeable(pkm, e.Species))
                return false;
            if (e.EggLocation == 60002 && e.Relearn[0] == 0 && pkm.RelearnMoves.Any(z => z != 0)) // gen7 eevee edge case
                return false;

            // Defer to EC/PID check
            // if (e.Shiny != null && e.Shiny != pkm.IsShiny)
            // continue;

            // Defer ball check to later
            // if (e.Gift && pkm.Ball != 4) // PokéBall
            // continue;

            if (pkm is PK1 pk1 && pk1.Gen1_NotTradeback)
                if (!IsValidCatchRatePK1(e, pk1))
                    return false;

            if (!AllowGBCartEra && GameVersion.GBCartEraOnly.Contains(e.Version))
                return false;
            return true;
        }
        private static IEnumerable<EncounterStatic> GetStaticEncounters(PKM pkm, GameVersion gameSource = GameVersion.Any)
        {
            if (gameSource == GameVersion.Any)
                gameSource = (GameVersion)pkm.Version;

            var table = GetEncounterStaticTable(pkm, gameSource);
            switch (pkm.GenNumber)
            {
                case 1:
                    return GetStatic(pkm, table, maxspeciesorigin: MaxSpeciesID_1);
                case 2:
                    return GetStatic(pkm, table, maxspeciesorigin: MaxSpeciesID_2);
                default:
                    return GetStatic(pkm, table);
            }
        }
        private static IEnumerable<EncounterStatic> GetStatic(PKM pkm, IEnumerable<EncounterStatic> table, int maxspeciesorigin = -1, int lvl = -1, bool skip = false)
        {
            IEnumerable<DexLevel> dl = GetValidPreEvolutions(pkm, maxspeciesorigin: maxspeciesorigin, lvl: lvl, skipChecks: skip);
            return table.Where(e => dl.Any(d => d.Species == e.Species));
        }

        // EncounterSlot
        private static IEnumerable<EncounterSlot> GetRawEncounterSlots(PKM pkm, int lvl, GameVersion gameSource = GameVersion.Any)
        {
            int maxspeciesorigin = -1;
            if (gameSource == GameVersion.RBY) maxspeciesorigin = MaxSpeciesID_1;
            else if (GameVersion.GSC.Contains(gameSource)) maxspeciesorigin = MaxSpeciesID_2;

            var vs = GetValidPreEvolutions(pkm, maxspeciesorigin: maxspeciesorigin);
            return GetEncounterAreas(pkm, gameSource).SelectMany(area => GetValidEncounterSlots(pkm, area, vs, DexNav: pkm.AO, lvl: lvl));
        }
        private static IEnumerable<EncounterSlot> GetValidWildEncounters(PKM pkm, GameVersion gameSource = GameVersion.Any)
        {
            if (gameSource == GameVersion.Any)
                gameSource = (GameVersion)pkm.Version;

            int lvl = GetMinLevelEncounter(pkm);
            if (lvl <= 0)
                return Enumerable.Empty<EncounterSlot>();
            var s = GetRawEncounterSlots(pkm, lvl, gameSource);
            bool IsSafariBall = pkm.Ball == 5;
            bool IsSportsBall = pkm.Ball == 0x18;
            bool IsHidden = pkm.AbilityNumber == 4; // hidden Ability
            int species = pkm.Species;

            bool IsDeferred(EncounterSlot slot)
            {
                if (slot.Species == 265 && species != 265 && !IsWurmpleEvoValid(pkm))
                    return true; // bad wurmple evolution
                if (IsHidden ^ IsHiddenAbilitySlot(slot))
                    return true; // ability mismatch
                if (IsSafariBall ^ IsSafariSlot(slot.Type))
                    return true; // Safari Zone only ball
                if (IsSportsBall ^ slot.Type == SlotType.BugContest)
                    return true;
                return false; // BCC only ball
            }
            return s.OrderBy(IsDeferred); // non-deferred first
        }
        private static IEnumerable<EncounterSlot> GetValidFriendSafari(PKM pkm)
        {
            if (!pkm.XY || pkm.Met_Location != 148 || pkm.Met_Level != 30) // Friend Safari
                return Enumerable.Empty<EncounterSlot>();
            var vs = GetValidPreEvolutions(pkm).Where(d => d.Level >= 30);
            return vs.SelectMany(z => Encounters6.FriendSafari[z.Species]);
        }
        private static IEnumerable<EncounterSlot> GetValidEncounterSlots(PKM pkm, EncounterArea loc, IEnumerable<DexLevel> vs, bool DexNav = false, int lvl = -1, bool ignoreLevel = false)
        {
            if (pkm.WasEgg)
                return Enumerable.Empty<EncounterSlot>();
            if (lvl < 0)
                lvl = GetMinLevelEncounter(pkm);
            if (lvl <= 0)
                return Enumerable.Empty<EncounterSlot>();

            int gen = pkm.GenNumber;
            if (gen < 3)
                return GetValidEncounterSlots12(pkm, loc, vs, lvl, ignoreLevel);

            const int fluteBoost = 4;
            const int dexnavBoost = 30;
            int df = DexNav ? fluteBoost : 0;
            int dn = DexNav ? fluteBoost + dexnavBoost : 0;

            // Get Valid levels
            var encounterSlots = GetValidEncounterSlotsByEvoLevel(pkm, loc.Slots, lvl, ignoreLevel, vs, df, dn);

            // Return enumerable of slots pkm might have originated from
            if (gen <= 5)
                return GetFilteredSlotsByForm(pkm, encounterSlots);
            if (DexNav && gen == 6)
                return GetFilteredSlots6DexNav(pkm, lvl, encounterSlots, fluteBoost);
            return GetFilteredSlots67(pkm, encounterSlots);
        }
        private static IEnumerable<EncounterSlot> GetValidEncounterSlots12(PKM pkm, EncounterArea loc, IEnumerable<DexLevel> vs, int lvl = -1, bool ignoreLevel = false)
        {
            if (lvl < 0)
                lvl = GetMinLevelEncounter(pkm);
            if (lvl <= 0)
                return Enumerable.Empty<EncounterSlot>();

            var Gen1Version = GameVersion.RBY;
            bool RBDragonair = false;
            if (!ignoreLevel && !FilterGBSlotsCatchRate(pkm, ref vs, ref Gen1Version, ref RBDragonair))
                return Enumerable.Empty<EncounterSlot>();

            var encounterSlots = GetValidEncounterSlotsByEvoLevel(pkm, loc.Slots, lvl, ignoreLevel, vs);
            return GetFilteredSlots12(pkm, pkm.GenNumber, Gen1Version, encounterSlots, RBDragonair).OrderBy(slot => slot.LevelMin); // prefer lowest levels
        }
        private static IEnumerable<EncounterSlot> GetValidEncounterSlotsByEvoLevel(PKM pkm, IEnumerable<EncounterSlot> slots, int lvl, bool ignoreLevel, IEnumerable<DexLevel> vs, int df = 0, int dn = 0)
        {
            // Get slots where pokemon can exist with respect to the evolution chain
            if (ignoreLevel)
                return slots.Where(slot => vs.Any(evo => evo.Species == slot.Species)).ToList();

            slots = slots.Where(slot => vs.Any(evo => evo.Species == slot.Species && evo.Level >= slot.LevelMin - df));
            // Get slots where pokemon can exist with respect to level constraints
            if (pkm.HasOriginalMetLocation)
                return slots.Where(slot => slot.LevelMin - df <= lvl && lvl <= slot.LevelMax + (slot.Permissions.AllowDexNav ? dn : df)).ToList();
            // check for any less than current level
            return slots.Where(slot => slot.LevelMin <= lvl).ToList();
        }
        private static IEnumerable<EncounterSlot> GetFilteredSlotsByForm(PKM pkm, IEnumerable<EncounterSlot> encounterSlots)
        {
            return WildForms.Contains(pkm.Species)
                ? encounterSlots.Where(slot => slot.Form == pkm.AltForm)
                : encounterSlots;
        }
        private static IEnumerable<EncounterSlot> GetFilteredSlots67(PKM pkm, IEnumerable<EncounterSlot> encounterSlots)
        {
            int species = pkm.Species;
            int form = pkm.AltForm;

            // Edge Case Handling
            switch (species)
            {
                case 744 when form == 1:
                case 745 when form == 2:
                    yield break;
            }

            var slots = new List<EncounterSlot>();
            if (AlolanVariantEvolutions12.Contains(species)) // match form if same species, else form 0.
            {
                foreach (var slot in encounterSlots)
                {
                    if (species == slot.Species ? slot.Form == form : slot.Form == 0)
                        yield return slot;
                    slots.Add(slot);
                }
            }
            else if (ShouldMatchSlotForm()) // match slot form
            {
                foreach (var slot in encounterSlots)
                {
                    if (slot.Form == form)
                        yield return slot;
                    slots.Add(slot);
                }
            }
            else
            {
                foreach (var slot in encounterSlots)
                {
                    yield return slot; // no form checking
                    slots.Add(slot);
                }
            }

            // Filter for Form Specific
            // Pressure Slot
            EncounterSlot slotMax = slots.OrderByDescending(slot => slot.LevelMax).FirstOrDefault();
            if (slotMax == null)
                yield break; // yield break;

            if (AlolanVariantEvolutions12.Contains(species)) // match form if same species, else form 0.
            {
                if (species == slotMax.Species ? slotMax.Form == form : slotMax.Form == 0)
                    yield return GetPressureSlot(slotMax, pkm);
            }
            else if (ShouldMatchSlotForm()) // match slot form
            {
                if (slotMax.Form == form)
                    yield return GetPressureSlot(slotMax, pkm);
            }
            else
                yield return GetPressureSlot(slotMax, pkm);

            bool ShouldMatchSlotForm() => WildForms.Contains(species) || AlolanOriginForms.Contains(species) || FormConverter.IsTotemForm(species, form);
        }
        private static IEnumerable<EncounterSlot> GetFilteredSlots6DexNav(PKM pkm, int lvl, IEnumerable<EncounterSlot> encounterSlots, int fluteBoost)
        {
            var slots = new List<EncounterSlot>();
            foreach (EncounterSlot s in encounterSlots)
            {
                if (WildForms.Contains(pkm.Species) && s.Form != pkm.AltForm)
                {
                    slots.Add(s);
                    continue;
                }
                bool nav = s.Permissions.AllowDexNav && (pkm.RelearnMove1 != 0 || pkm.AbilityNumber == 4);
                EncounterSlot slot = s.Clone();
                slot.Permissions.DexNav = nav;

                if (slot.LevelMin > lvl)
                    slot.Permissions.WhiteFlute = true;
                if (slot.LevelMax + 1 <= lvl && lvl <= slot.LevelMax + fluteBoost)
                    slot.Permissions.BlackFlute = true;
                if (slot.LevelMax != lvl && slot.Permissions.AllowDexNav)
                    slot.Permissions.DexNav = true;
                yield return slot;
                slots.Add(s);
            }
            // Pressure Slot
            EncounterSlot slotMax = slots.OrderByDescending(slot => slot.LevelMax).FirstOrDefault();
            if (slotMax != null)
                yield return GetPressureSlot(slotMax, pkm);
        }
        private static EncounterSlot GetPressureSlot(EncounterSlot s, PKM pkm)
        {
            var max = s.Clone();
            max.Permissions.Pressure = true;
            max.Form = pkm.AltForm;
            return max;
        }

        private static bool FilterGBSlotsCatchRate(PKM pkm, ref IEnumerable<DexLevel> vs, ref GameVersion Gen1Version, ref bool RBDragonair)
        {
            if (!(pkm is PK1 pk1) || !pkm.Gen1_NotTradeback)
                return true;

            // Pure gen 1, slots can be filter by catch rate
            var rate = pk1.Catch_Rate;
            switch (pkm.Species)
            {
                // Pikachu
                case 25 when rate == 163:
                case 26 when rate == 163:
                    return false; // Yellow Pikachu is not a wild encounter

                // Kadabra (YW)
                case 64 when rate == 96:
                case 65 when rate == 96:
                    vs = vs.Where(s => s.Species == 64);
                    Gen1Version = GameVersion.YW;
                    return true;

                // Kadabra (RB)
                case 64 when rate == 100:
                case 65 when rate == 100:
                    vs = vs.Where(s => s.Species == 64);
                    Gen1Version = GameVersion.RB;
                    return true;

                // Dragonair (YW)
                case 148 when rate == 27:
                case 149 when rate == 27:
                    vs = vs.Where(s => s.Species == 148); // Yellow Dragonair, ignore Dratini encounters
                    Gen1Version = GameVersion.YW;
                    return true;
                
                // Dragonair (RB)
                case 148:
                case 149:
                    // Red blue dragonair have the same catch rate as dratini, it could also be a dratini from any game
                    vs = vs.Where(s => rate == PersonalTable.RB[s.Species].CatchRate);
                    RBDragonair = true;
                    return true;

                default:
                    vs = vs.Where(s => rate == PersonalTable.RB[s.Species].CatchRate);
                    return true;
            }
        }
        private static IEnumerable<EncounterSlot> GetFilteredSlots12(PKM pkm, int gen, GameVersion Gen1Version, IEnumerable<EncounterSlot> slots, bool RBDragonair)
        {
            switch (gen)
            {
                case 1:
                    if (Gen1Version != GameVersion.RBY)
                        slots = slots.Where(slot => Gen1Version.Contains(((EncounterSlot1)slot).Version));

                    // Red Blue dragonair or dratini from any gen 1 games
                    if (RBDragonair)
                        return slots.Where(slot => GameVersion.RB.Contains(((EncounterSlot1)slot).Version) || slot.Species == 147);

                    return slots;

                case 2:
                    if (pkm is PK2 pk2 && pk2.Met_Day != 0)
                        return slots.Where(slot => ((EncounterSlot1)slot).Time.Contains(pk2.Met_Day));
                    return slots;

                default:
                    return slots;
            }
        }
        private static IEnumerable<EncounterArea> GetEncounterSlots(PKM pkm, GameVersion gameSource = GameVersion.Any)
        {
            if (gameSource == GameVersion.Any)
                gameSource = (GameVersion)pkm.Version;

            return GetEncounterTable(pkm, gameSource);
        }
        private static IEnumerable<EncounterArea> GetEncounterAreas(PKM pkm, GameVersion gameSource = GameVersion.Any)
        {
            if (gameSource == GameVersion.Any)
                gameSource = (GameVersion)pkm.Version;

            var slots = GetEncounterSlots(pkm, gameSource: gameSource);
            bool noMet = !pkm.HasOriginalMetLocation || pkm.Format == 2 && gameSource != GameVersion.C;
            return noMet ? slots : slots.Where(area => area.Location == pkm.Met_Location);
        }

        // EncounterLink
        private static IEnumerable<EncounterLink> GetValidLinkGifts(PKM pkm)
        {
            switch (pkm.GenNumber)
            {
                case 6:
                    return Encounters6.LinkGifts6.Where(g => g.Species == pkm.Species && g.Level == pkm.Met_Level);
                default:
                    return Enumerable.Empty<EncounterLink>();
            }
        }

        // EncounterTrade
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
                if (z.IVs[0] >= 0 && !z.IVs.SequenceEqual(pkm.IVs) && pkm.Format <= 2)
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
        private static IEnumerable<EncounterTrade> GetValidEncounterTrades(PKM pkm, GameVersion gameSource = GameVersion.Any)
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

            return true;
        }

        // MysteryGift
        private static IEnumerable<MysteryGift> GetValidGifts(PKM pkm)
        {
            switch (pkm.GenNumber)
            {
                case 3:
                    return GetMatchingWC3(pkm, MGDB_G3);
                case 4:
                    return GetMatchingPCD(pkm, MGDB_G4);
                case 5:
                    return GetMatchingPGF(pkm, MGDB_G5);
                case 6:
                    return GetMatchingWC6(pkm, MGDB_G6);
                case 7:
                    return GetMatchingWC7(pkm, MGDB_G7);
                default:
                    return Enumerable.Empty<MysteryGift>();
            }
        }
        private static IEnumerable<MysteryGift> GetMatchingWC3(PKM pkm, IEnumerable<MysteryGift> DB)
        {
            if (DB == null)
                yield break;

            var validWC3 = new List<MysteryGift>();
            var vs = GetValidPreEvolutions(pkm, MaxSpeciesID_3).ToArray();
            var enumerable = DB.OfType<WC3>().Where(wc => vs.Any(dl => dl.Species == wc.Species));
            foreach (WC3 wc in enumerable)
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
        private static IEnumerable<MysteryGift> GetMatchingPCD(PKM pkm, IEnumerable<MysteryGift> DB)
        {
            if (DB == null || pkm.IsEgg && pkm.Format != 4) // transferred
                yield break;

            if (IsRangerManaphy(pkm))
            {
                if (pkm.Language != (int)LanguageID.Korean) // never korean
                    yield return new PGT { Data = { [0] = 7, [8] = 1 } };
                yield break;
            }

            var deferred = new List<MysteryGift>();
            var vs = GetValidPreEvolutions(pkm).ToArray();
            var enumerable = DB.OfType<PCD>().Where(wc => vs.Any(dl => dl.Species == wc.Species));
            foreach (PCD mg in enumerable)
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
        private static IEnumerable<MysteryGift> GetMatchingPGF(PKM pkm, IEnumerable<MysteryGift> DB)
        {
            if (DB == null)
                yield break;

            var deferred = new List<MysteryGift>();
            var vs = GetValidPreEvolutions(pkm).ToArray();
            var enumerable = DB.OfType<PGF>().Where(wc => vs.Any(dl => dl.Species == wc.Species));
            foreach (PGF wc in enumerable)
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
        private static IEnumerable<MysteryGift> GetMatchingWC6(PKM pkm, IEnumerable<MysteryGift> DB)
        {
            if (DB == null)
                yield break;
            var deferred = new List<MysteryGift>();
            var vs = GetValidPreEvolutions(pkm).ToArray();
            var enumerable = DB.OfType<WC6>().Where(wc => vs.Any(dl => dl.Species == wc.Species));
            foreach (WC6 wc in enumerable)
            {
                if (!GetIsMatchWC6(pkm, wc, vs))
                    continue;

                switch (wc.CardID)
                {
                    case 0525 when wc.IV_HP == 0xFE: // Diancie was distributed with no IV enforcement & 3IVs
                    case 0504 when wc.RibbonClassic != ((IRibbonSetEvent4)pkm).RibbonClassic: // magmar with/without classic
                        deferred.Add(wc);
                        continue;
                }
                if (wc.Species == pkm.Species) // best match
                    yield return wc;
                else
                    deferred.Add(wc);
            }
            foreach (var z in deferred)
                yield return z;
        }
        private static IEnumerable<MysteryGift> GetMatchingWC7(PKM pkm, IEnumerable<MysteryGift> DB)
        {
            if (DB == null)
                yield break;
            var deferred = new List<MysteryGift>();
            var vs = GetValidPreEvolutions(pkm).ToArray();
            var enumerable = DB.OfType<WC7>().Where(wc => vs.Any(dl => dl.Species == wc.Species));
            foreach (WC7 wc in enumerable)
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

                if (wc.Species == pkm.Species) // best match
                    yield return wc;
                else
                    deferred.Add(wc);
            }
            foreach (var z in deferred)
                yield return z;
        }
        private static bool GetIsMatchWC3(PKM pkm, WC3 wc)
        {
            // Gen3 Version MUST match.
            if (wc.Version != 0 && !((GameVersion)wc.Version).Contains((GameVersion)pkm.Version))
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
                bool valid = wc.Level == 20 && pkm is XK3;
                if (!valid)
                    return false;
            }

            if (pkm.IsNative)
            {
                if (wc.Met_Level != pkm.Met_Level)
                    return false;
                if (wc.Location != pkm.Met_Location && (!wc.IsEgg || pkm.IsEgg))
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
        private static bool GetIsMatchPCD(PKM pkm, PKM wc, IEnumerable<DexLevel> vs)
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

            if (wc.CNT_Cool > pkm.CNT_Cool) return false;
            if (wc.CNT_Beauty > pkm.CNT_Beauty) return false;
            if (wc.CNT_Cute > pkm.CNT_Cute) return false;
            if (wc.CNT_Smart > pkm.CNT_Smart) return false;
            if (wc.CNT_Tough > pkm.CNT_Tough) return false;
            if (wc.CNT_Sheen > pkm.CNT_Sheen) return false;

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
                    return false; // can't be traded away for unshiny
                if (pkm.IsEgg && !pkm.IsNative)
                    return false;
            }

            if (wc.Form != pkm.AltForm && vs.All(dl => !IsFormChangeable(pkm, dl.Species))) return false;

            if (wc.Level != pkm.Met_Level) return false;
            if (wc.Ball != pkm.Ball) return false;
            if (wc.Nature != 0xFF && wc.Nature != pkm.Nature) return false;
            if (wc.Gender != 2 && wc.Gender != pkm.Gender) return false;

            if (wc.CNT_Cool > pkm.CNT_Cool) return false;
            if (wc.CNT_Beauty > pkm.CNT_Beauty) return false;
            if (wc.CNT_Cute > pkm.CNT_Cute) return false;
            if (wc.CNT_Smart > pkm.CNT_Smart) return false;
            if (wc.CNT_Tough > pkm.CNT_Tough) return false;
            if (wc.CNT_Sheen > pkm.CNT_Sheen) return false;

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
                if (wc.PIDType == 0 && pkm.PID != wc.PID) return false;
                if (wc.PIDType == 2 && !pkm.IsShiny) return false;
                if (wc.PIDType == 3 && pkm.IsShiny) return false;
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
                    return false; // can't be traded away for unshiny
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

            if (wc.CNT_Cool > pkm.CNT_Cool) return false;
            if (wc.CNT_Beauty > pkm.CNT_Beauty) return false;
            if (wc.CNT_Cute > pkm.CNT_Cute) return false;
            if (wc.CNT_Smart > pkm.CNT_Smart) return false;
            if (wc.CNT_Tough > pkm.CNT_Tough) return false;
            if (wc.CNT_Sheen > pkm.CNT_Sheen) return false;

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
                    return false;
            }

            if (wc.IsEgg)
            {
                if (wc.EggLocation != pkm.Egg_Location) // traded
                {
                    if (pkm.Egg_Location != 30002)
                        return false;
                }
                else if (wc.PIDType == 0 && pkm.IsShiny)
                    return false; // can't be traded away for unshiny
                if (pkm.IsEgg && !pkm.IsNative)
                    return false;
            }
            else
            {
                if (wc.EggLocation != pkm.Egg_Location) return false;
                if (wc.MetLocation != pkm.Met_Location) return false;
            }

            if (wc.MetLevel != pkm.Met_Level) return false;
            if (wc.Ball != pkm.Ball) return false;
            if (wc.OTGender < 3 && wc.OTGender != pkm.OT_Gender) return false;
            if (wc.Nature != 0xFF && wc.Nature != pkm.Nature) return false;
            if (wc.Gender != 3 && wc.Gender != pkm.Gender) return false;

            if (wc.CNT_Cool > pkm.CNT_Cool) return false;
            if (wc.CNT_Beauty > pkm.CNT_Beauty) return false;
            if (wc.CNT_Cute > pkm.CNT_Cute) return false;
            if (wc.CNT_Smart > pkm.CNT_Smart) return false;
            if (wc.CNT_Tough > pkm.CNT_Tough) return false;
            if (wc.CNT_Sheen > pkm.CNT_Sheen) return false;

            if (wc.PIDType == 2 && !pkm.IsShiny) return false;
            if (wc.PIDType == 3 && pkm.IsShiny) return false;

            switch (wc.CardID)
            {
                case 1624: // Rockruff
                    if (pkm.Species == 745 && pkm.AltForm != 2)
                        return false;
                    if (pkm.Version == (int)GameVersion.US)
                        return wc.Move3 == 424; // Fire Fang
                    if (pkm.Version == (int)GameVersion.UM)
                        return wc.Move3 == 422; // Thunder Fang
                    return false;
                case 2046: // Ash Greninja
                    return pkm.SM; // not USUM
            }
            return true;
        }

        // EncounterEgg
        private static IEnumerable<EncounterEgg> GenerateEggs(PKM pkm)
        {
            if (NoHatchFromEgg.Contains(pkm.Species))
                yield break;
            if (FormConverter.IsTotemForm(pkm.Species, pkm.AltForm, pkm.GenNumber))
                yield break; // no totem eggs

            int gen = pkm.GenNumber;
            // version is a true indicator for all generation 3-5 origins
            var ver = (GameVersion) pkm.Version;
            int max = GetMaxSpeciesOrigin(gen);

            var baseSpecies = GetBaseSpecies(pkm, 0);
            int lvl = gen < 4 ? 5 : 1;
            if (baseSpecies <= max)
            {
                yield return new EncounterEgg { Game = ver, Level = lvl, Species = baseSpecies };
                if (gen > 5 && pkm.WasTradedEgg)
                    yield return new EncounterEgg { Game = tradePair(), Level = lvl, Species = baseSpecies };
            }

            if (!GetSplitBreedGeneration(pkm).Contains(pkm.Species))
                yield break; // no other possible species

            baseSpecies = GetBaseSpecies(pkm, 1);
            if (baseSpecies <= max)
            {
                yield return new EncounterEgg { Game = ver, Level = lvl, Species = baseSpecies, SplitBreed = true };
                if (gen > 5 && pkm.WasTradedEgg)
                    yield return new EncounterEgg { Game = tradePair(), Level = lvl, Species = baseSpecies, SplitBreed = true };
            }

            // Gen6+ update the origin game when hatched. Quick manip for X.Y<->A.O | S.M<->US.UM, ie X->A
            GameVersion tradePair()
            {
                if (ver <= GameVersion.OR) // gen6
                    return (GameVersion)((int)ver ^ 2);
                if (ver <= GameVersion.MN) // gen7
                    return ver + 2;
                return ver - 2;
            }
        }

        // Utility
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
        private static bool IsHiddenAbilitySlot(EncounterSlot slot)
        {
            return slot.Permissions.DexNav || slot.Type == SlotType.FriendSafari || slot.Type == SlotType.Horde || slot.Type == SlotType.SOS;
        }
        internal static bool IsSafariSlot(SlotType t)
        {
            return t.HasFlag(SlotType.Safari);
        }
        internal static bool IsDexNavValid(PKM pkm)
        {
            if (!pkm.AO || !pkm.InhabitedGeneration(6))
                return false;

            var vs = GetValidPreEvolutions(pkm);
            IEnumerable<EncounterArea> locs = GetDexNavAreas(pkm);
            var d_areas = locs.Select(loc => GetValidEncounterSlots(pkm, loc, vs, DexNav: true));
            return d_areas.Any(slots => slots.Any(slot => slot.Permissions.AllowDexNav && slot.Permissions.DexNav));
        }
        private static bool IsEncounterTypeMatch(IEncounterable e, int type)
        {
            switch (e)
            {
                case EncounterStaticTyped t:
                    return t.TypeEncounter.Contains(type);
                case EncounterSlot w:
                    return w.TypeEncounter.Contains(type);
                default:
                    return type == 0;
            }
        }
        internal static EncounterArea GetCaptureLocation(PKM pkm)
        {
            var vs = GetValidPreEvolutions(pkm);
            return (from area in GetEncounterSlots(pkm)
                let slots = GetValidEncounterSlots(pkm, area, vs, DexNav: pkm.AO, ignoreLevel: true).ToArray()
                where slots.Length != 0
                select new EncounterArea
                {
                    Location = area.Location,
                    Slots = slots,
                }).OrderBy(area => area.Slots.Min(x => x.LevelMin)).FirstOrDefault();
        }
        internal static EncounterStatic GetStaticLocation(PKM pkm, int species = -1)
        {
            switch (pkm.GenNumber)
            {
                case 1:
                    return GetRBYStaticTransfer(species, pkm.Met_Level);
                case 2:
                    return GetGSStaticTransfer(species, pkm.Met_Level);
                default:
                    var table = GetEncounterStaticTable(pkm, (GameVersion)pkm.Version);
                    return GetStatic(pkm, table, lvl: 100, skip: true).FirstOrDefault();
            }
        }
        internal static bool IsVCStaticTransferEncounterValid(PKM pkm, EncounterStatic e)
        {
            return pkm.Met_Location == e.Location && pkm.Egg_Location == e.EggLocation;
        }
        internal static IEncounterable GetVCStaticTransferEncounter(PKM pkm)
        {
            if (pkm.VC1)
                return GetRBYStaticTransfer(pkm.Species, pkm.Met_Level);
            if (pkm.VC2)
                return GetGSStaticTransfer(pkm.Species, pkm.Met_Level);
            return new EncounterInvalid(pkm);
        }
        private static EncounterStatic GetRBYStaticTransfer(int species, int pkmMetLevel)
        {
            return new EncounterStatic
            {
                Species = species,
                Gift = true, // Forces Poké Ball
                Ability = TransferSpeciesDefaultAbility_1.Contains(species) ? 1 : 4, // Hidden by default, else first
                Shiny = species == 151 ? (bool?)false : null,
                Fateful = species == 151,
                Location = Transfer1,
                EggLocation = 0,
                IV3 = true,
                Level = pkmMetLevel,
                Version = GameVersion.RBY
            };
        }
        private static EncounterStatic GetGSStaticTransfer(int species, int pkmMetLevel)
        {
            return new EncounterStatic
            {
                Species = species,
                Gift = true, // Forces Poké Ball
                Ability = TransferSpeciesDefaultAbility_2.Contains(species) ? 1 : 4, // Hidden by default, else first
                Shiny = species == 151 ? (bool?)false : null,
                Fateful = species == 151 || species == 251,
                Location = Transfer2,
                EggLocation = 0,
                IV3 = true,
                Level = pkmMetLevel,
                Version = GameVersion.GSC
            };
        }
        internal static bool IsEncounterTrade1Valid(PKM pkm)
        {
            string ot = pkm.OT_Name;
            string tr = pkm.Format <= 2 ? "TRAINER" : "Trainer"; // decaps on transfer
            return ot == "トレーナー" || ot == tr;
        }
        private static bool IsWurmpleEvoValid(PKM pkm)
        {
            uint evoVal = PKX.GetWurmpleEvoVal(pkm.EncryptionConstant);
            int wIndex = Array.IndexOf(WurmpleEvolutions, pkm.Species) / 2;
            return evoVal == wIndex;
        }
    }
}
