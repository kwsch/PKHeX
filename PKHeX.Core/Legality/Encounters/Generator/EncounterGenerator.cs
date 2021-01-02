using System;
using System.Collections.Generic;
using System.Linq;
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
        public static IEnumerable<IEncounterable> GetEncounters(PKM pkm, LegalInfo info) => info.Generation switch
        {
            1 => EncounterGenerator12.GetEncounters12(pkm, info),
            2 => EncounterGenerator12.GetEncounters12(pkm, info),
            3 => GetEncounters3(pkm, info),
            4 => GetEncounters4(pkm, info),
            8 => GenerateRawEncounters8(pkm),
            _ => GenerateRawEncounters(pkm)
        };

        private static IEnumerable<IEncounterable> GetEncounters3(PKM pkm, LegalInfo info)
        {
            info.PIDIV = MethodFinder.Analyze(pkm);
            var deferred = new List<IEncounterable>();
            foreach (var z in GenerateRawEncounters3(pkm, info))
            {
                if (pkm.Version == (int)GameVersion.CXD) // C/XD
                {
                    if (z is EncounterSlot3PokeSpot w)
                    {
                        var seeds = MethodFinder.GetPokeSpotSeeds(pkm, w.SlotNumber).FirstOrDefault();
                        info.PIDIV = seeds ?? info.PIDIV;
                    }
                    else if (z is EncounterStaticShadow s)
                    {
                        bool valid = GetIsShadowLockValid(pkm, info, s);
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

        private static bool GetIsShadowLockValid(PKM pkm, LegalInfo info, EncounterStaticShadow s)
        {
            if (s.IVs.Count == 0) // not E-Reader
                return LockFinder.IsAllShadowLockValid(s, info.PIDIV, pkm);

            // E-Reader have fixed IVs, and aren't recognized as CXD (no PID-IV correlation).
            var possible = MethodFinder.GetColoEReaderMatches(pkm.EncryptionConstant);
            foreach (var poss in possible)
            {
                if (!LockFinder.IsAllShadowLockValid(s, poss, pkm))
                    continue;
                info.PIDIV = poss;
                return true;
            }

            return false;
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
                else if (pkm.Format <= 6 && !(z is IEncounterTypeTile t ? t.TypeEncounter.Contains(pkm.EncounterType) : pkm.EncounterType == 0))
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

        private static IEnumerable<IEncounterable> GenerateRawEncounters(PKM pkm)
        {
            int ctr = 0;

            var chain = EncounterOrigin.GetOriginChain(pkm);
            if (pkm.WasEvent || pkm.WasEventEgg || pkm.WasLink)
            {
                foreach (var z in GetValidGifts(pkm, chain))
                { yield return z; ++ctr; }
                if (ctr != 0) yield break;
            }

            if (pkm.WasBredEgg)
            {
                foreach (var z in GenerateEggs(pkm))
                { yield return z; ++ctr; }
                if (ctr == 0) yield break;
            }

            foreach (var z in GetValidStaticEncounter(pkm, chain))
            { yield return z; ++ctr; }
            if (ctr != 0) yield break;

            foreach (var z in GetValidWildEncounters(pkm, chain))
            { yield return z; ++ctr; }
            if (ctr != 0) yield break;

            foreach (var z in GetValidEncounterTrades(pkm, chain))
            { yield return z; ++ctr; }
        }

        private static IEnumerable<IEncounterable> GenerateRawEncounters8(PKM pkm)
        {
            // Static Encounters can collide with wild encounters (close match); don't break if a Static Encounter is yielded.
            int ctr = 0;

            var chain = EncounterOrigin.GetOriginChain(pkm);
            if (pkm.WasEvent || pkm.WasEventEgg)
            {
                foreach (var z in GetValidGifts(pkm, chain))
                { yield return z; ++ctr; }
                if (ctr != 0) yield break;
            }

            if (pkm.WasBredEgg)
            {
                foreach (var z in GenerateEggs(pkm))
                { yield return z; ++ctr; }
                if (ctr == 0) yield break;
            }

            foreach (var z in GetValidStaticEncounter(pkm, chain))
            { yield return z; ++ctr; }
            // if (ctr != 0) yield break;
            foreach (var z in GetValidWildEncounters(pkm, chain))
            { yield return z; ++ctr; }

            if (ctr != 0) yield break;
            foreach (var z in GetValidEncounterTrades(pkm, chain))
            { yield return z; ++ctr; }
        }

        private static IEnumerable<IEncounterable> GenerateRawEncounters4(PKM pkm, LegalInfo info)
        {
            bool wasEvent = pkm.WasEvent || pkm.WasEventEgg; // egg events?
            var chain = EncounterOrigin.GetOriginChain(pkm);
            if (wasEvent)
            {
                int ctr = 0;
                foreach (var z in GetValidGifts(pkm, chain))
                { yield return z; ++ctr; }
                if (ctr != 0) yield break;
            }
            if (pkm.WasBredEgg)
            {
                foreach (var z in GenerateEggs(pkm))
                    yield return z;
            }
            foreach (var z in GetValidEncounterTrades(pkm, chain))
                yield return z;

            var deferIncompat = new Queue<IEncounterable>();
            bool sport = pkm.Ball == (int)Ball.Sport; // never static encounters (conflict with non bcc / bcc)
            bool safari = pkm.Ball == (int)Ball.Safari; // never static encounters
            bool safariSport = safari || sport;
            if (!safariSport)
            {
                foreach (var z in GetValidStaticEncounter(pkm, chain))
                {
                    if (z.Gift && pkm.Ball != z.Ball)
                        deferIncompat.Enqueue(z);
                    else
                        yield return z;
                }
            }

            int species = pkm.Species;
            var deferNoFrame = new Queue<IEncounterable>();
            var deferFrame = new Queue<IEncounterable>();
            var slots = FrameFinder.GetFrames(info.PIDIV, pkm).ToList();
            foreach (var z in GetValidWildEncounters34(pkm, chain))
            {
                bool defer = z.IsDeferred4(species, pkm, safari, sport);
                var frame = slots.Find(s => s.IsSlotCompatibile((EncounterSlot4)z, pkm));
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
            foreach (var z in GetValidStaticEncounter(pkm, chain))
                yield return z;
        }

        private static IEnumerable<IEncounterable> GenerateRawEncounters3(PKM pkm, LegalInfo info)
        {
            var chain = EncounterOrigin.GetOriginChain(pkm);
            foreach (var z in GetValidGifts(pkm, chain))
                yield return z;
            foreach (var z in GetValidEncounterTrades(pkm, chain))
                yield return z;

            var deferIncompat = new Queue<IEncounterable>();
            bool safari = pkm.Ball == 0x05; // never static encounters
            if (!safari)
            {
                foreach (var z in GetValidStaticEncounter(pkm, chain))
                {
                    if (z.Gift && pkm.Ball != z.Ball)
                        deferIncompat.Enqueue(z);
                    else
                        yield return z;
                }
            }

            int species = pkm.Species;
            var deferNoFrame = new Queue<IEncounterable>();
            var deferFrame = new Queue<IEncounterable>();
            var slots = FrameFinder.GetFrames(info.PIDIV, pkm).ToList();
            foreach (var z in GetValidWildEncounters34(pkm, chain))
            {
                bool defer = z.IsDeferred3(species, pkm, safari);
                var frame = slots.Find(s => s.IsSlotCompatibile((EncounterSlot3)z, pkm));
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
            foreach (var z in GetValidStaticEncounter(pkm, chain))
                yield return z;
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
