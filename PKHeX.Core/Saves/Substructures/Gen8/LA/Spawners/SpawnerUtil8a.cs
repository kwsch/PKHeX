namespace PKHeX.Core;

/// <summary>
/// Utility logic for interacting with Spawner objects.
/// </summary>
public static class SpawnerUtil8a
{
    // Abuse the fact that Group Seed -> Entry seed generation is new(u64) -> u64 next(); can infer original s0 as we already know s1 (const).
    private static ulong ComputeGroupSeed(ulong seed) => unchecked(seed - Xoroshiro128Plus.XOROSHIRO_CONST);
    public static ulong ComputeGroupSeed(SpawnerEntry8a entry) => ComputeGroupSeed(entry.Seed_00);
    public static ulong ComputeGroupSeed(Spawner8a spawner) => ComputeGroupSeed(spawner[0]);
}
