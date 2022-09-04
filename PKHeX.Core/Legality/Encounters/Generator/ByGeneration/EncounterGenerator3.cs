using System;
using System.Collections.Generic;
using System.Linq;

using static PKHeX.Core.MysteryGiftGenerator;
using static PKHeX.Core.EncounterTradeGenerator;
using static PKHeX.Core.EncounterSlotGenerator;
using static PKHeX.Core.EncounterStaticGenerator;
using static PKHeX.Core.EncounterEggGenerator;
using static PKHeX.Core.EncounterMatchRating;

namespace PKHeX.Core;

public static class EncounterGenerator3
{
    public static IEnumerable<IEncounterable> GetEncounters(PKM pk, LegalInfo info)
    {
        if (pk.Version == (int) GameVersion.CXD)
            return GetEncounters3CXD(pk, info);
        return GetEncounters3(pk, info);
    }

    private static IEnumerable<IEncounterable> GetEncounters3(PKM pk, LegalInfo info)
    {
        info.PIDIV = MethodFinder.Analyze(pk);
        IEncounterable? Partial = null;

        foreach (var z in GenerateRawEncounters3(pk, info))
        {
            if (info.PIDIV.Type.IsCompatible3(z, pk))
                yield return z;
            else
                Partial ??= z;
        }
        if (Partial == null)
            yield break;

        info.PIDIVMatches = false;
        yield return Partial;
    }

    private static IEnumerable<IEncounterable> GetEncounters3CXD(PKM pk, LegalInfo info)
    {
        info.PIDIV = MethodFinder.Analyze(pk);
        IEncounterable? Partial = null;
        foreach (var z in GenerateRawEncounters3CXD(pk))
        {
            if (z is EncounterSlot3PokeSpot w)
            {
                var pidiv = MethodFinder.GetPokeSpotSeedFirst(pk, w.SlotNumber);
                if (pidiv.Type == PIDType.PokeSpot)
                    info.PIDIV = pidiv;
            }
            else if (z is EncounterStaticShadow s)
            {
                bool valid = GetIsShadowLockValid(pk, info, s);
                if (!valid)
                {
                    Partial ??= s;
                    continue;
                }
            }

            if (info.PIDIV.Type.IsCompatible3(z, pk))
                yield return z;
            else
                Partial ??= z;
        }
        if (Partial == null)
            yield break;

        info.PIDIVMatches = false;
        yield return Partial;
    }

    private static IEnumerable<IEncounterable> GenerateRawEncounters3CXD(PKM pk)
    {
        var chain = EncounterOrigin.GetOriginChain(pk);

        var game = (GameVersion)pk.Version;
        // Mystery Gifts
        foreach (var z in GetValidGifts(pk, chain, game))
        {
            // Don't bother deferring matches.
            var match = z.GetMatchRating(pk);
            if (match != PartialMatch)
                yield return z;
        }

        // Trades
        foreach (var z in GetValidEncounterTrades(pk, chain, game))
        {
            // Don't bother deferring matches.
            var match = z.GetMatchRating(pk);
            if (match != PartialMatch)
                yield return z;
        }

        IEncounterable? partial = null;

        // Static Encounter
        foreach (var z in GetValidStaticEncounter(pk, chain, game))
        {
            var match = z.GetMatchRating(pk);
            if (match == PartialMatch)
                partial ??= z;
            else
                yield return z;
        }

        // Encounter Slots
        foreach (var z in GetValidWildEncounters(pk, chain, game))
        {
            var match = z.GetMatchRating(pk);
            if (match == PartialMatch)
            {
                partial ??= z;
                continue;
            }
            yield return z;
        }

        if (partial is not null)
            yield return partial;
    }

    private static IEnumerable<IEncounterable> GenerateRawEncounters3(PKM pk, LegalInfo info)
    {
        var chain = EncounterOrigin.GetOriginChain(pk);
        var game = (GameVersion)pk.Version;

        // Mystery Gifts
        foreach (var z in GetValidGifts(pk, chain, game))
        {
            // Don't bother deferring matches.
            var match = z.GetMatchRating(pk);
            if (match != PartialMatch)
                yield return z;
        }

        // Trades
        foreach (var z in GetValidEncounterTrades(pk, chain, game))
        {
            // Don't bother deferring matches.
            var match = z.GetMatchRating(pk);
            if (match != PartialMatch)
                yield return z;
        }

        IEncounterable? deferred = null;
        IEncounterable? partial = null;

        // Static Encounter
        // Defer everything if Safari Ball
        bool safari = pk.Ball == 0x05; // never static encounters
        if (!safari)
        {
            foreach (var z in GetValidStaticEncounter(pk, chain, game))
            {
                var match = z.GetMatchRating(pk);
                if (match == PartialMatch)
                    partial ??= z;
                else
                    yield return z;
            }
        }

        // Encounter Slots
        var slots = FrameFinder.GetFrames(info.PIDIV, pk).ToList();
        foreach (var z in GetValidWildEncounters(pk, chain, game))
        {
            var match = z.GetMatchRating(pk);
            if (match == PartialMatch)
            {
                partial ??= z;
                continue;
            }

            var frame = slots.Find(s => s.IsSlotCompatibile((EncounterSlot3)z, pk));
            if (frame == null)
            {
                deferred ??= z;
                continue;
            }
            yield return z;
        }

        info.FrameMatches = false;
        if (deferred is EncounterSlot3 x)
            yield return x;

        if (pk.Version != (int)GameVersion.CXD) // no eggs in C/XD
        {
            foreach (var z in GenerateEggs(pk, 3))
                yield return z;
        }

        if (partial is EncounterSlot3 y)
        {
            var frame = slots.Find(s => s.IsSlotCompatibile(y, pk));
            info.FrameMatches = frame != null;
            yield return y;
        }

        // do static encounters if they were deferred to end, spit out any possible encounters for invalid pk
        if (!safari)
            yield break;

        partial = null;

        foreach (var z in GetValidStaticEncounter(pk, chain, game))
        {
            var match = z.GetMatchRating(pk);
            if (match == PartialMatch)
                partial ??= z;
            else
                yield return z;
        }

        if (partial is not null)
            yield return partial;
    }

    private static bool GetIsShadowLockValid(PKM pk, LegalInfo info, EncounterStaticShadow s)
    {
        if (!s.EReader)
            return LockFinder.IsAllShadowLockValid(s, info.PIDIV, pk);

        // E-Reader have fixed IVs, and aren't recognized as CXD (no PID-IV correlation).
        Span<uint> seeds = stackalloc uint[4];
        var count = XDRNG.GetSeeds(seeds, pk.EncryptionConstant);
        var xdc = seeds[..count];
        foreach (var seed in xdc)
        {
            var pidiv = new PIDIV(PIDType.CXD, XDRNG.Next4(seed));
            if (!LockFinder.IsAllShadowLockValid(s, pidiv, pk))
                continue;
            info.PIDIV = pidiv;
            return true;
        }

        return false;
    }
}
