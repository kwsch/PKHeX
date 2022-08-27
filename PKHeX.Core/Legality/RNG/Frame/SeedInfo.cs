using System.Collections.Generic;

namespace PKHeX.Core;

/// <summary>
/// Seed result value storage for passing frame seeds &amp; conditions.
/// </summary>
public readonly record struct SeedInfo(uint Seed, bool Charm3 = false)
{
    /// <summary>
    /// Yields an enumerable list of seeds until another valid PID breaks the chain.
    /// </summary>
    /// <param name="pidiv">Seed and RNG data</param>
    /// <param name="info">Verification information</param>
    /// <returns>Seed information data, which needs to be unrolled once for the nature call.</returns>
    public static IEnumerable<SeedInfo> GetSeedsUntilNature(PIDIV pidiv, FrameGenerator info)
    {
        var seed = pidiv.OriginSeed;
        yield return new SeedInfo(seed);

        var s1 = seed;
        var s2 = RNG.LCRNG.Prev(s1);
        bool charm3 = false;
        while (true)
        {
            var a = s2 >> 16;
            var b = s1 >> 16;

            var pid = b << 16 | a;

            // Process Conditions
            switch (VerifyPIDCriteria(pid, info))
            {
                case LockInfo.Pass:
                    yield break;
                case LockInfo.Gender:
                    charm3 = true;
                    break;
            }

            s1 = RNG.LCRNG.Prev(s2);
            s2 = RNG.LCRNG.Prev(s1);

            yield return new SeedInfo(s1, charm3);
        }
    }

    /// <summary>
    /// Yields an enumerable list of seeds until another valid PID breaks the chain.
    /// </summary>
    /// <param name="pk">Entity data created from the ambiguous cute charm seeds.</param>
    /// <returns>Seed information data, which needs to be unrolled once for the nature call.</returns>
    public static IEnumerable<SeedInfo> GetSeedsUntilNature4Cute(PKM pk)
    {
        // We cannot rely on a PID-IV origin seed. Since IVs are 2^30, they are not strong enough to assume a single seed was the source.
        // We must reverse the IVs to find all seeds that could generate this.
        // ESV,Proc,Nature,IV1,IV2; these do not do the nature loop for Method J/K so each seed originates a single seed frame.
        var seeds = MethodFinder.GetCuteCharmSeeds(pk);
        foreach (var seed in seeds)
            yield return new SeedInfo(seed);
    }
    
    /// <summary>
    /// Yields an enumerable list of seeds until another valid PID breaks the chain.
    /// </summary>
    /// <param name="pidiv">Seed and RNG data</param>
    /// <param name="info">Verification information</param>
    /// <param name="form">Unown Form lock value</param>
    /// <returns>Seed information data, which needs to be unrolled once for the nature call.</returns>
    public static IEnumerable<SeedInfo> GetSeedsUntilUnownForm(PIDIV pidiv, FrameGenerator info, byte form)
    {
        var seed = pidiv.OriginSeed;
        yield return new SeedInfo(seed);

        var s1 = seed;
        var s2 = RNG.LCRNG.Prev(s1);
        while (true)
        {
            var a = s2 >> 16;
            var b = s1 >> 16;
            // PID is in reverse for FRLG Unown
            var pid = (a << 16) | b;

            // Process Conditions
            if (EntityPID.GetUnownForm3(pid) == form) // matches form, does it match nature?
            {
                switch (VerifyPIDCriteria(pid, info))
                {
                    case LockInfo.Pass: // yes
                        yield break;
                }
            }

            s1 = RNG.LCRNG.Prev(s2);
            s2 = RNG.LCRNG.Prev(s1);

            yield return new SeedInfo(s1);
        }
    }

    private static LockInfo VerifyPIDCriteria(uint pid, FrameGenerator info)
    {
        // Nature locks are always a given
        var nval = pid % 25;
        if (nval != info.Nature)
            return LockInfo.Nature;

        if (!info.Gendered)
            return LockInfo.Pass;

        var gender = pid & 0xFF;
        if (info.GenderLow > gender || gender > info.GenderHigh)
            return LockInfo.Gender;

        return LockInfo.Pass;
    }
}
