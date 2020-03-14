using System;
using System.Collections.Generic;
using System.Linq;
using static PKHeX.Core.Legal;

using static PKHeX.Core.MysteryGiftGenerator;
using static PKHeX.Core.EncounterTradeGenerator;
using static PKHeX.Core.EncounterSlotGenerator;
using static PKHeX.Core.EncounterStaticGenerator;
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
                case 8: return GenerateRawEncounters8(pkm);
                default: return GenerateRawEncounters(pkm);
            }
        }

        private static IEnumerable<IEncounterable> GetEncounters12(PKM pkm, LegalInfo info)
        {
            int baseSpecies = GetBaseSpecies(pkm).Species;

            if ((pkm.Format == 1 && baseSpecies > MaxSpeciesID_1) || baseSpecies > MaxSpeciesID_2)
                yield break;

            foreach (var z in GenerateFilteredEncounters12(pkm))
            {
                info.Generation = z is IGeneration g ? g.Generation : 2;
                info.Game = ((IVersion)z).Version;
                yield return z;
            }
        }

        private static IEnumerable<IEncounterable> GetEncounters3(PKM pkm, LegalInfo info)
        {
            info.PIDIV = MethodFinder.Analyze(pkm);
            var deferred = new List<IEncounterable>();
            foreach (var z in GenerateRawEncounters3(pkm, info))
            {
                if (pkm.Version == (int)GameVersion.CXD) // C/XD
                {
                    if (z is EncounterSlot w)
                    {
                        var seeds = MethodFinder.GetPokeSpotSeeds(pkm, w.SlotNumber).FirstOrDefault();
                        info.PIDIV = seeds ?? info.PIDIV;
                    }
                    else if (z is EncounterStaticShadow s)
                    {
                        bool valid = false;
                        if (s.IVs.Count == 0) // not ereader
                        {
                            valid = LockFinder.IsAllShadowLockValid(s, info.PIDIV, pkm);
                        }
                        else
                        {
                            var possible = MethodFinder.GetColoEReaderMatches(pkm.EncryptionConstant);
                            foreach (var poss in possible)
                            {
                                if (!LockFinder.IsAllShadowLockValid(s, poss, pkm))
                                    continue;
                                valid = true;
                                info.PIDIV = poss;
                                break;
                            }
                        }

                        if (!valid)
                        {
                            deferred.Add(s);
                            continue;
                        }
                    }
                }
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

        private static IEnumerable<IEncounterable> GenerateRawEncounters12(PKM pkm, GameVersion game)
        {
            bool gsc = GameVersion.GSC.Contains(game);

            // Since encounter matching is super weak due to limited stored data in the structure
            // Calculate all 3 at the same time and pick the best result (by species).
            // Favor special event move gifts as Static Encounters when applicable
            var maxspeciesorigin = gsc ? MaxSpeciesID_2 : MaxSpeciesID_1;
            var vs = EvolutionChain.GetValidPreEvolutions(pkm, maxspeciesorigin: maxspeciesorigin);

            var deferred = new List<IEncounterable>();
            foreach (var t in GetValidEncounterTrades(pkm, vs, game))
            {
                // some OTs are longer than the keyboard entry; don't defer these
                if (pkm.Format >= 7 && pkm.OT_Name.Length <= (pkm.Japanese || pkm.Korean ? 5 : 7))
                {
                    deferred.Add(t);
                    continue;
                }
                yield return t;
            }
            foreach (var s in GetValidStaticEncounter(pkm, vs, game))
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
                        if (s.Species == 251 && ParseSettings.AllowGBCartEra) // no celebi, the GameVersion.EventsGBGen2 will pass thru
                            continue;
                        break;
                }
                yield return s;
            }
            foreach (var e in GetValidWildEncounters12(pkm, vs, game))
            {
                yield return e;
            }

            if (gsc)
            {
                var canBeEgg = GetCanBeEgg(pkm);
                if (canBeEgg)
                {
                    int eggspec = GetBaseEggSpecies(pkm).Species;
                    if (ParseSettings.AllowGen2Crystal(pkm))
                        yield return new EncounterEgg(eggspec, 0, 5) { Version = GameVersion.C }; // gen2 egg
                    yield return new EncounterEgg(eggspec, 0, 5) { Version = GameVersion.GS }; // gen2 egg
                }
            }

            foreach (var d in deferred)
                yield return d;
        }

        private static bool GetCanBeEgg(PKM pkm)
        {
            bool canBeEgg = !pkm.Gen1_NotTradeback && GetCanBeEgg23(pkm) && !NoHatchFromEgg.Contains(pkm.Species);
            if (!canBeEgg)
                return false;

            // Further Filtering
            if (pkm.Format < 3)
            {
                canBeEgg &= pkm.Met_Location == 0 || pkm.Met_Level == 1; // 2->1->2 clears met info
                canBeEgg &= pkm.CurrentLevel >= 5;
            }

            return canBeEgg;
        }

        private static IEnumerable<IEncounterable> GenerateFilteredEncounters12(PKM pkm)
        {
            bool crystal = (pkm.Format == 2 && pkm.Met_Location != 0) || (pkm.Format >= 7 && pkm.OT_Gender == 1);
            bool kadabra = pkm.Species == 64 && pkm is PK1 pk1
                           && (pk1.Catch_Rate == PersonalTable.RB[64].CatchRate
                            || pk1.Catch_Rate == PersonalTable.Y[64].CatchRate); // catch rate outsider, return gen1 first always

            // iterate over both games, consuming from one list at a time until the other list has higher priority encounters
            var get1 = GenerateRawEncounters1(pkm, crystal);
            var get2 = GenerateRawEncounters2(pkm, crystal);
            using var g1i = new PeekEnumerator<IEncounterable>(get1);
            using var g2i = new PeekEnumerator<IEncounterable>(get2);

            var deferred = new List<IEncounterable>();
            while (g2i.PeekIsNext() || g1i.PeekIsNext())
            {
                var move = GetPreferredGBIterator(pkm, g1i, g2i);
                var obj = move.Peek();
                int gen = obj is IGeneration g ? g.Generation : 2; // only eggs don't implement interface

                if (gen == 1 && (pkm.Korean || (obj is EncounterTrade t && !IsEncounterTrade1Valid(pkm, t))))
                    deferred.Add(obj);
                else if (gen == 2 && ((pkm.Korean && (((IVersion)obj).Version == GameVersion.C)) || kadabra))
                    deferred.Add(obj);
                else
                    yield return obj;

                move.MoveNext();
            }
            foreach (var z in deferred)
                yield return z;
        }

        private static IEnumerable<IEncounterable> GenerateRawEncounters1(PKM pkm, bool crystal)
        {
            return pkm.Gen2_NotTradeback || crystal
                ? Enumerable.Empty<IEncounterable>()
                : GenerateRawEncounters12(pkm, GameVersion.RBY);
        }

        private static IEnumerable<IEncounterable> GenerateRawEncounters2(PKM pkm, bool crystal)
        {
            return pkm.Gen1_NotTradeback
                ? Enumerable.Empty<IEncounterable>()
                : GenerateRawEncounters12(pkm, crystal ? GameVersion.C : GameVersion.GSC);
        }

        private static PeekEnumerator<IEncounterable> GetPreferredGBIterator(PKM pkm, PeekEnumerator<IEncounterable> g1i, PeekEnumerator<IEncounterable> g2i)
        {
            if (!g1i.PeekIsNext())
                return g2i;
            if (!g2i.PeekIsNext())
                return g1i;
            var p1 = GetGBEncounterPriority(pkm, g1i.Current);
            var p2 = GetGBEncounterPriority(pkm, g2i.Current);
            return p1 > p2 ? g1i : g2i;
        }

        private static GBEncounterPriority GetGBEncounterPriority(PKM pkm, IEncounterable Encounter)
        {
            switch (Encounter)
            {
                case EncounterTrade t:
                    return t.Generation == 2 ? GBEncounterPriority.TradeEncounterG2 : GBEncounterPriority.TradeEncounterG1;
                case EncounterStatic s:
                    if (s.Moves.Count != 0 && s.Moves[0] != 0 && pkm.Moves.Contains(s.Moves[0]))
                        return GBEncounterPriority.SpecialEncounter;
                    return GBEncounterPriority.StaticEncounter;
                case EncounterSlot _:
                    return GBEncounterPriority.WildEncounter;

                default:
                    return GBEncounterPriority.EggEncounter;
            }
        }

        /// <summary>
        /// Generation 1/2 Encounter Data type, which serves as a 'best match' priority rating when returning from a list.
        /// </summary>
        private enum GBEncounterPriority
        {
            EggEncounter,
            WildEncounter,
            StaticEncounter,
            SpecialEncounter,
            TradeEncounterG1,
            TradeEncounterG2,
        }

        private static IEnumerable<IEncounterable> GenerateRawEncounters(PKM pkm)
        {
            int ctr = 0;

            if (pkm.WasEvent || pkm.WasEventEgg || pkm.WasLink)
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

            if (EncounterArea6XY.WasFriendSafari(pkm))
            {
                foreach (var z in EncounterArea6XY.GetValidFriendSafari(pkm))
                { yield return z; ++ctr; }
                if (ctr != 0) yield break;
            }

            foreach (var z in GetValidWildEncounters(pkm))
            { yield return z; ++ctr; }
            if (ctr != 0) yield break;
            foreach (var z in GetValidEncounterTrades(pkm))
            { yield return z; ++ctr; }
        }

        private static IEnumerable<IEncounterable> GenerateRawEncounters8(PKM pkm)
        {
            // Static Encounters can collide with wild encounters (close match); don't break if a Static Encounter is yielded.
            int ctr = 0;

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
            // if (ctr != 0) yield break;
            foreach (var z in GetValidWildEncounters(pkm))
            { yield return z; ++ctr; }

            if (ctr != 0) yield break;
            foreach (var z in GetValidEncounterTrades(pkm))
            { yield return z; ++ctr; }
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
            {
                foreach (var z in GetValidStaticEncounter(pkm))
                {
                    if (z.Gift && pkm.Ball != 4)
                        deferIncompat.Enqueue(z);
                    else
                        yield return z;
                }
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
            if (!safariSport)
                yield break;
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
            {
                foreach (var z in GetValidStaticEncounter(pkm))
                {
                    if (z.Gift && pkm.Ball != 4)
                        deferIncompat.Enqueue(z);
                    else
                        yield return z;
                }
            }

            int species = pkm.Species;
            var deferNoFrame = new Queue<IEncounterable>();
            var deferFrame = new Queue<IEncounterable>();
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
            {
                foreach (var z in GenerateEggs(pkm))
                    yield return z;
            }

            foreach (var z in deferIncompat)
                yield return z;
            // do static encounters if they were deferred to end, spit out any possible encounters for invalid pkm
            if (!safari)
                yield break;
            foreach (var z in GetValidStaticEncounter(pkm))
                yield return z;
        }

        // Utility
        private static bool IsEncounterTypeMatch(IEncounterable e, int type)
        {
            return e switch
            {
                EncounterStaticTyped t => t.TypeEncounter.Contains(type),
                EncounterSlot w => w.TypeEncounter.Contains(type),
                _ => (type == 0)
            };
        }

        internal static bool IsEncounterTrade1Valid(PKM pkm, EncounterTrade t)
        {
            string ot = pkm.OT_Name;
            if (pkm.Format <= 2)
                return ot == StringConverter12.G1TradeOTStr;
            // Converted string 1/2->7 to language specific value
            var tr = t.GetOT(pkm.Language);
            return ot == tr;
        }

        /// <summary>
        /// Filters a sequence of values based on a predicate.
        /// Any elements that match the predicate are yielded after those that did not match, in the same order they were observed.
        /// </summary>
        /// <remarks>
        /// <see cref="Enumerable"/> OrderBy consumes the entire list when reordering elements, instead of instantly yielding best matches.
        /// https://referencesource.microsoft.com/#System.Core/System/Linq/Enumerable.cs,ffb8de6aefac77cc</remarks>
        /// <typeparam name="T">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of values to reorder.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> that contains elements from the input, with non-deferred results first.</returns>
        internal static IEnumerable<T> DeferByBoolean<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        {
            var deferred = new List<T>();
            foreach (var x in source)
            {
                if (predicate(x))
                {
                    deferred.Add(x);
                    continue;
                }
                yield return x;
            }
            foreach (var d in deferred)
                yield return d;
        }
    }
}
