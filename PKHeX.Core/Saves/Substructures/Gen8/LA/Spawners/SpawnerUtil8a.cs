namespace PKHeX.Core;

/// <summary>
/// Utility logic for interacting with Spawner objects.
/// </summary>
public static class SpawnerUtil8a
{
    // Abuse the fact that Group Seed -> Entry seed generation is new(u64) -> u64 Next(); can infer original s0 as we already know s1 (const).
    private static ulong ComputeGroupSeed(ulong seed) => unchecked(seed - Xoroshiro128Plus.XOROSHIRO_CONST);
    public static ulong ComputeGroupSeed(this SpawnerEntry8a entry) => ComputeGroupSeed(entry.GenerateSeed);
    public static ulong ComputeGroupSeed(this Spawner8a spawner) => ComputeGroupSeed(spawner[0]);

    public static bool IsOriginalSeed(this Spawner8a spawner, ulong groupSeed)
    {
        var rand = new Xoroshiro128Plus(groupSeed);
        for (int i = 0; i < Spawner8a.EntryCount; i++)
        {
            var entry = spawner[i];
            if (entry.IsEmpty != 0)
                break;

            if (entry.GenerateSeed != rand.Next())
                return false;
            if (entry.AlphaSeed != rand.Next())
                return false;
        }

        return rand.Next() == spawner.Meta.GroupSeed;
    }
}
