using System;
using System.Collections.Generic;

using static PKHeX.Core.EncounterGeneratorUtil;
using static PKHeX.Core.EncounterStateUtil;
using static PKHeX.Core.EncounterTypeGroup;
using static PKHeX.Core.EncounterMatchRating;

namespace PKHeX.Core;

public sealed class EncounterGenerator3GC : IEncounterGenerator
{
    public static readonly EncounterGenerator3GC Instance = new();

    public IEnumerable<IEncounterable> GetPossible(PKM pk, EvoCriteria[] chain, GameVersion game, EncounterTypeGroup groups)
    {
        if (chain.Length == 0)
            yield break;

        if (groups.HasFlag(Mystery))
        {
            var table = EncountersWC3.Encounter_WC3CXD;
            foreach (var enc in GetPossibleAll(chain, table))
                yield return enc;
        }
        if (groups.HasFlag(Slot))
        {
            var areas = Encounters3XD.SlotsXD;
            foreach (var enc in GetPossibleSlots<EncounterArea3XD, EncounterSlot3PokeSpot>(chain, areas))
                yield return enc;
        }
        if (groups.HasFlag(Static))
        {
            foreach (var enc in GetPossibleAll(chain, Encounters3Colo.Encounter_Colo))
                yield return enc;
            foreach (var enc in GetPossibleAll(chain, Encounters3XD.Encounter_XD))
                yield return enc;
            foreach (var enc in GetPossibleAll(chain, Encounters3Colo.Encounter_ColoGift))
                yield return enc;
            foreach (var enc in GetPossibleAll(chain, Encounters3XD.Encounter_XDGift))
                yield return enc;
            foreach (var enc in GetPossibleAll(chain, Encounters3Colo.EReader))
                yield return enc;
        }
    }

    public IEnumerable<IEncounterable> GetEncounters(PKM pk, LegalInfo info)
    {
        var chain = EncounterOrigin.GetOriginChain(pk, 3);
        return GetEncounters(pk, chain, info);
    }

    public IEnumerable<IEncounterable> GetEncounters(PKM pk, EvoCriteria[] chain, LegalInfo info)
    {
        if (chain.Length == 0)
            yield break;
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
            else if (z is IShadow3 s)
            {
                bool valid = GetIsShadowLockValid(pk, info, s);
                if (!valid)
                {
                    partial ??= z;
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
                if (match < PartialMatch)
                    yield return enc;
                break;
            }
        }
        foreach (var enc in Encounters3Colo.Encounter_Colo)
        {
            foreach (var evo in chain)
            {
                if (evo.Species != enc.Species)
                    continue;
                if (!enc.IsMatchExact(pk, evo))
                    break;

                var match = enc.GetMatchRating(pk);
                if (match < PartialMatch)
                    yield return enc;
                else
                    partial ??= enc;
                break;
            }
        }
        foreach (var enc in Encounters3XD.Encounter_XD)
        {
            foreach (var evo in chain)
            {
                if (evo.Species != enc.Species)
                    continue;
                if (!enc.IsMatchExact(pk, evo))
                    break;

                var match = enc.GetMatchRating(pk);
                if (match < PartialMatch)
                    yield return enc;
                else
                    partial ??= enc;
                break;
            }
        }
        foreach (var enc in Encounters3Colo.Encounter_ColoGift)
        {
            foreach (var evo in chain)
            {
                if (evo.Species != enc.Species)
                    continue;
                if (!enc.IsMatchExact(pk, evo))
                    break;

                var match = enc.GetMatchRating(pk);
                if (match < PartialMatch)
                    yield return enc;
                else
                    partial ??= enc;
                break;
            }
        }
        foreach (var enc in Encounters3XD.Encounter_XDGift)
        {
            foreach (var evo in chain)
            {
                if (evo.Species != enc.Species)
                    continue;
                if (!enc.IsMatchExact(pk, evo))
                    break;

                var match = enc.GetMatchRating(pk);
                if (match < PartialMatch)
                    yield return enc;
                else
                    partial ??= enc;
                break;
            }
        }
        if (CanBeWildEncounter(pk))
        {
            foreach (var area in Encounters3XD.SlotsXD)
            {
                var slots = area.GetMatchingSlots(pk, chain);
                foreach (var slot in slots)
                {
                    var match = slot.GetMatchRating(pk);
                    if (match < PartialMatch)
                        yield return slot;
                    else
                        partial ??= slot;
                }
            }
        }

        if (partial != null)
            yield return partial;
    }

    private static bool GetIsShadowLockValid(PKM pk, LegalInfo info, IShadow3 s) => s switch
    {
        EncounterShadow3Colo { EReader: true } => GetIsShadowLockValidEReader(pk, info, s),
        _ => LockFinder.IsAllShadowLockValid(s, info.PIDIV, pk),
    };

    private static bool GetIsShadowLockValidEReader(PKM pk, LegalInfo info, IShadow3 s)
    {
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
