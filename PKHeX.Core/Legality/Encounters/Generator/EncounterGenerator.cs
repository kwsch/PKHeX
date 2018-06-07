using System.Collections.Generic;
using System.Linq;
using static PKHeX.Core.Legal;

using static PKHeX.Core.MysteryGiftGenerator;
using static PKHeX.Core.EncounterTradeGenerator;
using static PKHeX.Core.EncounterSlotGenerator;
using static PKHeX.Core.EncounterStaticGenerator;
using static PKHeX.Core.EncounterLinkGenerator;
using static PKHeX.Core.EncounterEggGenerator;

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
                pkm.WasEgg = z.Encounter.EggEncounter;
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
            foreach (var t in GetValidEncounterTrades(pkm, vs, game))
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

                if ((obj.Generation == 1 && (pkm.Korean || (obj.Encounter is EncounterTrade && !IsEncounterTrade1Valid(pkm))))
                 || (obj.Generation == 2 && (pkm.Korean && (obj.Encounter is IVersion v && v.Version == GameVersion.C))))
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

            if (pkm.WasBredEgg)
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
            if (pkm.WasBredEgg)
            {
                foreach (var z in GenerateEggs(pkm))
                    yield return z;
            }
            foreach (var z in GetValidEncounterTrades(pkm))
                yield return z;

            var deferIncompat = new Queue<IEncounterable>();
            bool sport = pkm.Ball == 0x18; // never static encounters (conflict with non bcc / bcc)
            bool safari = pkm.Ball == 0x05; // never static encounters
            bool safariSport = safari || sport;
            if (!safariSport)
            foreach (var z in GetValidStaticEncounter(pkm))
            {
                if (z.Gift && pkm.Ball != 4)
                    deferIncompat.Enqueue(z);
                else
                    yield return z;
            }

            int species = pkm.Species;
            var deferNoFrame = new Queue<IEncounterable>();
            var deferFrame = new Queue<IEncounterable>();
            var slots = FrameFinder.GetFrames(info.PIDIV, pkm).ToList();
            foreach (var z in GetValidWildEncounters34(pkm))
            {
                bool defer = z.IsDeferred4(species, pkm, safari, sport);
                var frame = slots.Find(s => s.IsSlotCompatibile(z, pkm));
                if (defer)
                {
                    if (frame != null)
                        deferFrame.Enqueue(z);
                    else
                        deferIncompat.Enqueue(z);
                    continue;
                }
                if (frame == null)
                {
                    deferNoFrame.Enqueue(z);
                    continue;
                }
                yield return z;
            }
            info.FrameMatches = false;
            foreach (var z in deferNoFrame)
                yield return z;
            info.FrameMatches = true;
            foreach (var z in deferFrame)
                yield return z;
            info.FrameMatches = false;

            foreach (var z in deferIncompat)
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

            var deferIncompat = new Queue<IEncounterable>();
            bool safari = pkm.Ball == 0x05; // never static encounters
            if (!safari)
            foreach (var z in GetValidStaticEncounter(pkm))
            {
                if (z.Gift && pkm.Ball != 4)
                    deferIncompat.Enqueue(z);
                else
                    yield return z;
            }

            int species = pkm.Species;
            var deferNoFrame = new Queue<IEncounterable>();
            var deferFrame = new Queue<IEncounterable>();
            pkm.WasEgg = false; // clear flag if set from static
            var slots = FrameFinder.GetFrames(info.PIDIV, pkm).ToList();
            foreach (var z in GetValidWildEncounters34(pkm))
            {
                bool defer = z.IsDeferred3(species, pkm, safari);
                var frame = slots.Find(s => s.IsSlotCompatibile(z, pkm));
                if (defer)
                {
                    if (frame != null)
                        deferFrame.Enqueue(z);
                    else
                        deferIncompat.Enqueue(z);
                    continue;
                }
                if (frame == null)
                {
                    deferNoFrame.Enqueue(z);
                    continue;
                }
                yield return z;
            }
            info.FrameMatches = false;
            foreach (var z in deferNoFrame)
                yield return z;
            info.FrameMatches = true;
            foreach (var z in deferFrame)
                yield return z;
            info.FrameMatches = false;

            if (pkm.Version != 15) // no eggs in C/XD
            foreach (var z in GenerateEggs(pkm))
                yield return z;

            foreach (var z in deferIncompat)
                yield return z;
            // do static encounters if they were deferred to end, spit out any possible encounters for invalid pkm
            if (safari)
            foreach (var z in GetValidStaticEncounter(pkm))
                yield return z;
        }

        // Utility
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
        internal static bool IsEncounterTrade1Valid(PKM pkm)
        {
            string ot = pkm.OT_Name;
            string tr = pkm.Format <= 2 ? "TRAINER" : "Trainer"; // decaps on transfer
            return ot == "トレーナー" || ot == tr;
        }
    }
}
