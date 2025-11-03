using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Threading;

namespace PKHeX.Core;

/// <summary>
/// Finding the crypto-secure encounter seed for <see cref="LumioseCorrelation"/> encounters.
/// </summary>
public static class LumioseSolver
{
    /// <summary>
    /// Allow searching for shiny seeds when <see cref="GenerateParam9a.Shiny"/> is <see cref="Shiny.Random"/> and <see cref="PKM.IsShiny"/> is <see langword="true"/> with only 1 roll.
    /// </summary>
    /// <remarks>
    /// Recovery of seeds is too slow for realtime checks. Default is <see langword="false"/>.
    /// </remarks>
    public static bool SearchShiny1 { get; set; }

    /// <summary>
    /// Allow searching for seeds when <see cref="GenerateParam9a.RollCount"/> is more than 1 roll.
    /// </summary>
    /// <remarks>
    /// Recovery of seeds is extremely slow. Default is <see langword="false"/>.
    /// </remarks>
    public static bool SearchShinyN { get; set; }

    /// <summary>
    /// Tries to get the <see cref="seed"/> that originated the <see cref="pk"/>.
    /// </summary>
    /// <param name="param">Generating parameters for FixInitSpec</param>
    /// <param name="pk">Entity that was generated</param>
    /// <param name="seed">Seed that was used to generate the entity.</param>
    /// <returns><see langword="true"/> if the seed was found, otherwise <see langword="false"/>.</returns>
    public static bool TryGetSeed(this in GenerateParam9a param, PKM pk, out ulong seed)
    {
        // Technically a rand() result of 0xFFFFFFFF for either EC/PID will double-roll, but we'll just ignore that case due to it being so rare.

        if (param.Shiny is Shiny.Random && pk.IsShiny)
        {
            if (param.RollCount == 1 && SearchShiny1) // O(131072)
                return TryGetSeedShiny(param, pk, out seed);
            if (SearchShinyN) // O(4,294,967,296) -- this will take a while.
                return TryGetSeedNoPID(param, pk, out seed);
            seed = 0;
            return false;
        }

        // Assume the PID is the first result; otherwise we would need to brute force the other unknown bits.
        if (TryGetSeedRegular(param, pk, out seed))
            return true;
        if (SearchShinyN && param.RollCount != 1)
            return TryGetSeedNoPID(param, pk, out seed);
        seed = 0;
        return false;
    }

    private static bool TryGetSeedRegular(in GenerateParam9a param, PKM pk, out ulong seed)
    {
        var ec = pk.EncryptionConstant;
        var pid = pk.PID;
        if (param.Correlation is LumioseCorrelation.SkipTrainer)
            return TryGetSeedConsecutive(param, pk, ec, pid, out seed);
        return TryGetSeedSkip(param, pk, ec, pid, out seed);
    }

    private static bool TryGetSeedShiny(in GenerateParam9a param, PKM pk, out ulong seed)
    {
        if (param.Correlation is LumioseCorrelation.SkipTrainer)
            return TryGetSeedConsecutiveShiny(param, pk, out seed);
        return TryGetSeedSkipShiny(param, pk, out seed);
    }

    private static bool TryGetSeedConsecutive(in GenerateParam9a param, PKM pk, uint ec, uint pid, out ulong seed)
    {
        var solver = new XoroMachineConsecutive(ec, pid);
        return TryGetSeed(param, pk, solver, out seed);
    }

    private static bool TryGetSeedSkip(in GenerateParam9a param, PKM pk, uint ec, uint pid, out ulong seed)
    {
        var solver = new XoroMachineSkip(ec, pid);
        return TryGetSeed(param, pk, solver, out seed);
    }

    private static bool TryGetSeed<T>(in GenerateParam9a param, PKM pk, T solver, out ulong seed) where T : struct, IEnumerator<ulong>
    {
        while (solver.MoveNext())
        {
            seed = solver.Current;
            if (LumioseRNG.Verify(pk, param, seed))
                return true;
        }
        seed = 0;
        return false;
    }

    private static bool TryGetSeedConsecutiveShiny(in GenerateParam9a param, PKM pk, out ulong seed)
    {
        var ec = pk.EncryptionConstant;
        var partial = pk.PID & 0x0000_FFF0;
        var p = param; // copy to avoid capturing an in-parameter in lambdas

        // Parallelize over ranges of the high 16 bits. Each high value iterates the 16 low-nibble variants.
        const int highMaxExclusive = 0x1_0000; // 0..65535 inclusive
        // Batch a reasonable amount of highs per task to avoid tiny work-items. 2048 highs => 32768 PID attempts per task.
        const int highBatchSize = 2048;

        var rangePartitioner = Partitioner.Create(0, highMaxExclusive, highBatchSize);

        ulong resultSeed = 0;
        bool found = false;
        Lock gate = new();

        Parallel.ForEach(rangePartitioner, (range, state) =>
        {
            if (Volatile.Read(ref found)) { state.Stop(); return; }

            for (int highStart = range.Item1; highStart < range.Item2; highStart++)
            {
                if (Volatile.Read(ref found)) { state.Stop(); return; }

                uint high = (uint)highStart << 16;
                var pid = high | partial;
                // Try all 16 low-nibble permutations
                do
                {
                    if (TryGetSeedConsecutive(p, pk, ec, pid, out var s))
                    {
                        lock (gate)
                        {
                            if (!found)
                            {
                                resultSeed = s;
                                found = true;
                            }
                        }
                        state.Stop();
                        return;
                    }
                } while ((++pid & 0xF) != 0); // stop on low reset
            }
        });

        seed = resultSeed;
        return found;
    }

    private static bool TryGetSeedSkipShiny(in GenerateParam9a param, PKM pk, out ulong seed)
    {
        var ec = pk.EncryptionConstant;
        var partial = pk.PID & 0x0000_FFF0;
        var p = param;

        const int highMaxExclusive = 0x1_0000; // 0..65535 inclusive
        const int highBatchSize = 2048; // keep work units chunky

        var rangePartitioner = Partitioner.Create(0, highMaxExclusive, highBatchSize);

        ulong resultSeed = 0;
        bool found = false;
        Lock gate = new();

        Parallel.ForEach(rangePartitioner, (range, state) =>
        {
            if (Volatile.Read(ref found)) { state.Stop(); return; }

            for (int highStart = range.Item1; highStart < range.Item2; highStart++)
            {
                if (Volatile.Read(ref found)) { state.Stop(); return; }

                uint high = (uint)highStart << 16;
                var pid = high | partial;
                do
                {
                    if (!TryGetSeedSkip(p, pk, ec, pid, out var s))
                        continue;

                    lock (gate)
                    {
                        if (!found)
                        {
                            resultSeed = s;
                            found = true;
                        }
                    }
                    state.Stop();
                    return;
                } while ((++pid & 0xF) != 0); // stop on low reset
            }
        });

        seed = resultSeed;
        return found;
    }

    private static bool TryGetSeedNoPID(in GenerateParam9a param, PKM pk, out ulong seed)
    {
        // Abuse the fact that EC is the first result from the rand() output, and gives away 32-bits of the seed.
        // We then only need to guess the high 32-bits of the seed, and can brute-force over that space.
        var ec = pk.EncryptionConstant;
        var p = param;

        // 32-bit PID space: [0,uint.MaxValue] (inclusive due to mutation). Chunk into large ranges for better throughput.
        const ulong total = uint.MaxValue + 1UL;
        // Use a large batch size to reduce scheduling overhead; ~16M PIDs per work item.
        const ulong batch = 1UL << 24; // 16,777,216

        var partitions = CreateULongRangePartitions(total, batch);

        ulong resultSeed = 0;
        bool found = false;
        Lock gate = new();
        ulong partial = ec - unchecked((uint)Xoroshiro128Plus.XOROSHIRO_CONST);

        Parallel.ForEach(partitions, (range, state) =>
        {
            if (Volatile.Read(ref found)) { state.Stop(); return; }

            uint start = (uint)range.start;
            uint endExclusive = (uint)range.end; // safe due to batching within 0..2^32

            for (ulong high = start; high < endExclusive; high++)
            {
                if (Volatile.Read(ref found)) { state.Stop(); return; }

                var s = (high << 32) | partial;
                if (!LumioseRNG.Verify(pk, p, s))
                    continue;

                lock (gate)
                {
                    if (!found)
                    {
                        resultSeed = s;
                        found = true;
                    }
                }
                state.Stop();
                return;
            }
        });

        seed = resultSeed;
        return found;
    }

    // Helper to build large range partitions for ulong-sized spaces, but kept within int-counted chunks for Parallel.ForEach.
    private static IEnumerable<(ulong start, ulong end)> CreateULongRangePartitions(ulong totalCount, ulong batchSize)
    {
        for (ulong start = 0; start < totalCount; start += batchSize)
        {
            ulong end = start + batchSize;
            if (end > totalCount)
                end = totalCount;
            yield return (start, end);
        }
    }
}
