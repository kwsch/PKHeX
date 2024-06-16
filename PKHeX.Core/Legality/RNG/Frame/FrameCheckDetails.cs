namespace PKHeX.Core;

public readonly ref struct FrameCheckDetails<T>
{
    public readonly T Encounter;
    public readonly byte LevelMin;
    public readonly byte LevelMax;
    public readonly uint Seed1;
    public readonly uint Seed2;
    public readonly uint Seed3;
    public readonly byte Format;

    public uint Seed4 => LCRNG.Prev(Seed3);
    public uint Prev1 => Seed1 >> 16;
    public uint Prev2 => Seed2 >> 16;
    public uint Prev3 => Seed3 >> 16;

    public FrameCheckDetails(T enc, uint seed, byte levelMin, byte levelMax, byte format)
    {
        Encounter = enc;
        LevelMin = levelMin;
        LevelMax = levelMax;
        Format = format;
        seed = Seed1 = LCRNG.Prev(seed);
        seed = Seed2 = LCRNG.Prev(seed);
        Seed3 = LCRNG.Prev(seed);
    }
}
