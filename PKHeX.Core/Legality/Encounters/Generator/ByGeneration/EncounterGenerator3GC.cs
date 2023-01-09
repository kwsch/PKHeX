using System;
using System.Collections.Generic;

using static PKHeX.Core.EncounterTypeGroup;
using static PKHeX.Core.EncounterMatchRating;

namespace PKHeX.Core;

public sealed class EncounterGenerator3GC : IEncounterGenerator
{
    public static readonly EncounterGenerator3GC Instance = new();

    public IEnumerable<IEncounterable> GetPossible(PKM pk, EvoCriteria[] chain, GameVersion game, EncounterTypeGroup groups)
    {
        if (groups.HasFlag(Mystery))
        {
            var table = EncountersWC3.Encounter_WC3CXD;
            foreach (var enc in GetPossibleGifts(chain, table))
                yield return enc;
        }
        if (groups.HasFlag(Slot))
        {
            var areas = Encounters3XD.SlotsXD;
            foreach (var enc in GetPossibleSlots(chain, areas))
                yield return enc;
        }
        if (groups.HasFlag(Static))
        {
            var table = Encounters3XD.Encounter_CXD;
            foreach (var enc in GetPossibleStatic(chain, table))
                yield return enc;
        }
    }

    private static IEnumerable<IEncounterable> GetPossibleGifts(EvoCriteria[] chain, WC3[] table)
    {
        foreach (var enc in table)
        {
            foreach (var evo in chain)
            {
                if (evo.Species != enc.Species)
                    continue;
                yield return enc;
                break;
            }
        }
    }

    private static IEnumerable<IEncounterable> GetPossibleSlots(EvoCriteria[] chain, EncounterArea3XD[] areas)
    {
        foreach (var area in areas)
        {
            foreach (var slot in area.Slots)
            {
                foreach (var evo in chain)
                {
                    if (evo.Species != slot.Species)
                        continue;
                    yield return slot;
                    break;
                }
            }
        }
    }

    private static IEnumerable<IEncounterable> GetPossibleStatic(EvoCriteria[] chain, EncounterStatic[] table)
    {
        foreach (var enc in table)
        {
            foreach (var evo in chain)
            {
                if (evo.Species != enc.Species)
                    continue;
                yield return enc;
                break;
            }
        }
    }

    public IEnumerable<IEncounterable> GetEncounters(PKM pk, LegalInfo info)
    {
        var chain = EncounterOrigin.GetOriginChain(pk);
        return GetEncounters(pk, chain, info);
    }

    public IEnumerable<IEncounterable> GetEncounters(PKM pk, EvoCriteria[] chain, LegalInfo info)
    {
        IEncounterable? partial = null;
        info.PIDIV = MethodFinder.Analyze(pk);
        foreach (var z in IterateInner(pk, chain))
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
                    partial ??= s;
                    continue;
                }
            }

            if (info.PIDIV.Type.IsCompatible3(z, pk))
                yield return z;
            else
                partial ??= z;
        }

        if (partial == null)
            yield break;

        info.PIDIVMatches = false;
        yield return partial;
    }

    private static IEnumerable<IEncounterable> IterateInner(PKM pk, EvoCriteria[] chain)
    {
        IEncounterable? partial = null;
        foreach (var enc in EncountersWC3.Encounter_WC3CXD)
        {
            foreach (var evo in chain)
            {
                if (evo.Species != enc.Species)
                    continue;
                if (!enc.IsMatchExact(pk, evo))
                    break;

                // Don't bother deferring matches.
                var match = enc.GetMatchRating(pk);
                if (match != PartialMatch)
                    yield return enc;
                break;
            }
        }
        foreach (var enc in Encounters3XD.Encounter_CXD)
        {
            foreach (var evo in chain)
            {
                if (evo.Species != enc.Species)
                    continue;
                if (!enc.IsMatchExact(pk, evo))
                    break;

                var match = enc.GetMatchRating(pk);
                if (match == PartialMatch)
                    partial ??= enc;
                else
                    yield return enc;
                break;
            }
        }
        foreach (var area in Encounters3XD.SlotsXD)
        {
            var slots = area.GetMatchingSlots(pk, chain);
            foreach (var slot in slots)
            {
                var match = slot.GetMatchRating(pk);
                if (match == PartialMatch)
                    partial ??= slot;
                else
                    yield return slot;
            }
        }
    }

    private static bool GetIsShadowLockValid(PKM pk, LegalInfo info, EncounterStaticShadow s)
    {
        if (!s.EReader)
            return LockFinder.IsAllShadowLockValid(s, info.PIDIV, pk);

        // E-Reader have fixed IVs, and aren't recognized as CXD (no PID-IV correlation).
        Span<uint> seeds = stackalloc uint[XDRNG.MaxCountSeedsPID];
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
