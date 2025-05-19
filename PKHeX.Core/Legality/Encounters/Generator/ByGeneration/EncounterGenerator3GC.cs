using System;
using System.Collections.Generic;

namespace PKHeX.Core;

public sealed class EncounterGenerator3GC : IEncounterGenerator
{
    public static readonly EncounterGenerator3GC Instance = new();
    public bool CanGenerateEggs => false;

    public IEnumerable<IEncounterable> GetPossible(PKM _, EvoCriteria[] chain, GameVersion __, EncounterTypeGroup groups)
    {
        var iterator = new EncounterPossible3GC(chain, groups);
        foreach (var enc in iterator)
            yield return enc;
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
            if (z is EncounterSlot3XD w)
            {
                if (MethodPokeSpot.TryGetOriginSeeds(pk, w, out var pid, out var ivs))
                {
                    const PIDType type = PIDType.PokeSpot;
                    info.PIDIV = new PIDIV(type, pid).AsMutated(type, ivs);
                }
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
            static bool IsTypeCompatible(IEncounterTemplate enc, PKM pk, PIDType type)
            {
                // boolean results only from this set of games (no correlation confusion to be concerned with)
                if (enc is IRandomCorrelation r)
                    return r.IsCompatible(type, pk) == RandomCorrelationRating.Match;
                return type is PIDType.None;
            }

            if (IsTypeCompatible(z, pk, info.PIDIV.Type))
                yield return z;
            else
                partial ??= z;
        }

        if (partial is not null)
        {
            info.ManualFlag = EncounterYieldFlag.InvalidPIDIV;
            yield return partial;
        }
    }

    private static IEnumerable<IEncounterable> IterateInner(PKM pk, EvoCriteria[] chain)
    {
        var iterator = new EncounterEnumerator3GC(pk, chain);
        foreach (var enc in iterator)
            yield return enc.Encounter;
    }

    private static bool GetIsShadowLockValid(PKM pk, LegalInfo info, IShadow3 s) => s switch
    {
        EncounterShadow3Colo { IsEReader: true } c => GetIsShadowLockValidEReader(pk, info, c),
        _ => LockFinder.IsAllShadowLockValid(s, info.PIDIV.OriginSeed, pk),
    };

    private static bool GetIsShadowLockValidEReader(PKM pk, LegalInfo info, EncounterShadow3Colo s)
    {
        // E-Reader have fixed IVs, and aren't recognized as CXD (no PID-IV correlation).
        Span<uint> seeds = stackalloc uint[XDRNG.MaxCountSeedsPID];
        var count = XDRNG.GetSeeds(seeds, pk.EncryptionConstant);
        var xdc = seeds[..count];
        foreach (var seed in xdc)
        {
            var origin = XDRNG.Next4(seed);
            if (!LockFinder.IsAllShadowLockValid(s, origin, pk))
                continue;
            info.PIDIV = new PIDIV(PIDType.CXD, origin);
            return true;
        }

        return false;
    }
}
