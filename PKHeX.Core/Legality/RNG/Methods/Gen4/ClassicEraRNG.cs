namespace PKHeX.Core;

/// <summary>
/// Generation 3 &amp; 4 RNG logic.
/// </summary>
public static class ClassicEraRNG
{
    /// <summary>
    /// Generate a chain shiny PID for the provided trainer ID.
    /// </summary>
    /// <param name="seed">Seed to use for the RNG.</param>
    /// <param name="id32">Trainer ID to use for the PID generation.</param>
    /// <returns>Shiny PID.</returns>
    /// <remarks>Consumes 15 RNG calls</remarks>
    public static uint GetChainShinyPID(ref uint seed, in uint id32)
    {
        // 1 3-bit for lower
        // 1 3-bit for upper
        // 13 rand bits
        uint lower = LCRNG.Next16(ref seed) & 7;
        uint upper = LCRNG.Next16(ref seed) & 7;
        for (int i = 3; i < 16; i++)
            lower |= (LCRNG.Next16(ref seed) & 1) << i;

        var tid16 = (ushort)(id32 & 0xFFFFu);
        var sid16 = (ushort)(id32 >> 16);
        upper = ((lower ^ tid16 ^ sid16) & 0xFFF8) | (upper & 0x7);
        return (upper << 16) | lower;
    }

    /// <summary>
    /// Generates a PID for a given seed.
    /// </summary>
    /// <param name="seed">Seed to use for the RNG.</param>
    /// <returns>32-bit value containing the PID (high | low).</returns>
    public static uint GetSequentialPID(ref uint seed)
    {
        var rand1 = LCRNG.Next16(ref seed);
        var rand2 = LCRNG.Next16(ref seed);
        return (rand2 << 16) | rand1;
    }

    /// <summary>
    /// Generates IVs for a given seed.
    /// </summary>
    /// <param name="seed">Seed to use for the RNG.</param>
    /// <returns>32-bit value containing the IVs (HABSCD, low->high).</returns>
    public static uint GetSequentialIVs(ref uint seed)
    {
        var rand1 = LCRNG.Next16(ref seed);
        var rand2 = LCRNG.Next16(ref seed);
        return (rand2 << 15) | rand1;
    }
}
