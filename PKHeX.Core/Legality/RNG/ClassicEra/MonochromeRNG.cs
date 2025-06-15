namespace PKHeX.Core;

/// <summary>
/// RNG logic for Generation 5 (Black/White/Black2/White2) games.
/// </summary>
public static class MonochromeRNG
{
    /// <summary>
    /// Calculates the XOR result of the Trainer ID and Secret ID, masked to the least significant bit.
    /// </summary>
    public static uint GetTrainerBitXor(ITrainerID32ReadOnly tr) => ((uint)tr.TID16 ^ tr.SID16) & 1;

    /// <summary>
    /// Generates a Pokémon with a valid PID, gender, nature, ability, and IVs based on the specified criteria.
    /// </summary>
    /// <param name="pk">The Pokémon object to be modified with the generated attributes.</param>
    /// <param name="criteria">The encounter criteria that define the desired attributes for the Pokémon.</param>
    /// <param name="gr">The gender ratio used to determine the Pokémon's gender.</param>
    /// <param name="seed">The initial random seed used for generating the Pokémon's PID and other attributes.</param>
    /// <param name="abilityIndex">The index of the ability to assign to the Pokémon.</param>
    /// <remarks>
    /// <see cref="GenerateShiny"/> should be used instead if shiny state is required.
    /// </remarks>
    public static void Generate(PK5 pk, in EncounterCriteria criteria, byte gr, uint seed, int abilityIndex)
    {
        // The RNG is random enough (64-bit) that our results won't be traceable to a specific seed (sufficiently random).
        // Therefore, we can skip the whole advancement sequence and "create" a PID directly from our random seed.

        // Nature: Separate from PID.
        // Gender: still follows the Gen3/4 correlation based on low-8 bits of PID.
        // Ability: bit 16
        // Correlation: Trainer ID low-bits with PID high-bit and low-bit xor must be 0.
        var bitXor = GetTrainerBitXor(pk);
        var id32 = pk.ID32;

        // Logic: get a PID that matches Gender, then force the other correlations since they won't invalidate the gender.
        // We could probably do this in a single step (shiny, gender), but it's not worth the complexity.
        while (true)
        {
            var pid = seed;

            // Force correlations
            var abit = (pid >> 16) & 1;
            if (abit != (abilityIndex & 1))
                pid ^= 1u << 16;
            var corr = (pid >> 31) ^ (pid & 1) ^ bitXor;
            if (corr != 0)
                pid ^= 1u;

            var gender = EntityGender.GetFromPIDAndRatio(pid, gr);
            if (!IsSatisfied(criteria, pid, id32, gender))
            {
                seed = LCRNG.Next(seed); // arbitrary scramble
                continue;
            }

            pk.PID = pid;
            pk.Gender = gender;
            break;
        }

        pk.Nature = criteria.GetNature();
        pk.RefreshAbility(abilityIndex);
        criteria.SetRandomIVs(pk);
    }

    private static bool IsSatisfied(in EncounterCriteria criteria, uint pid, uint id32, byte gender)
    {
        if (criteria.IsSpecifiedGender() && !criteria.IsSatisfiedGender(gender))
            return false;

        var xor = ShinyUtil.GetShinyXor(pid, id32);
        return criteria.Shiny switch
        {
            Shiny.AlwaysSquare when xor != 0 => false,
            Shiny.AlwaysStar when xor is >= 8 or 0 => false,
            Shiny.Never when xor < 8 => false,
            _ => true
        };
    }

    /// <summary>
    /// Generates a shiny Pokémon based on the specified encounter criteria and randomization parameters.
    /// </summary>
    /// <param name="pk">The Pokémon object to be modified with the generated attributes.</param>
    /// <param name="criteria">The encounter criteria that define the desired attributes for the Pokémon.</param>
    /// <param name="gr">The gender ratio used to determine the Pokémon's gender.</param>
    /// <param name="seed">The initial random seed used for generating the Pokémon's PID and other attributes.</param>
    /// <param name="abilityIndex">The index of the ability to assign to the Pokémon.</param>
    /// <remarks>
    /// <see cref="Generate"/> should be used instead if shiny state is not required.
    /// </remarks>
    public static void GenerateShiny(PK5 pk, in EncounterCriteria criteria, byte gr, uint seed, int abilityIndex)
    {
        // Get a random 8-bit number that satisfies the gender.
        uint genderByte = GetRandomGenderComponent(criteria, gr, seed, out var gender);
        pk.Gender = gender;

        byte shinyType = criteria.Shiny switch
        {
            Shiny.AlwaysSquare => 0,
            Shiny.AlwaysStar => 1,
            _ => (LCRNG.Next(seed) & 7u) == 0 // random choice, evenly distributed
                ? (byte)0
                : (byte)1,
        };
        pk.PID = GetShinyPID(genderByte, (uint)(abilityIndex & 1), pk.TID16, pk.SID16, shinyType);
        pk.Nature = criteria.GetNature();
        pk.RefreshAbility(abilityIndex);
        criteria.SetRandomIVs(pk);
    }

    private static byte GetRandomGenderComponent(in EncounterCriteria criteria, byte gr, uint seed, out byte gender)
    {
        while (true)
        {
            var genderByte = (byte)(seed & 0xFF);
            gender = EntityGender.GetFromPIDAndRatio(genderByte, gr);
            if (!criteria.IsSpecifiedGender() || criteria.IsSatisfiedGender(gender))
                return genderByte;

            seed = LCRNG.Next(seed); // arbitrary scramble
        }
    }

    private static uint GetShinyPID(uint gval, uint abilityBit, ushort TID16, ushort SID16, byte shinyType)
    {
        // Logic: get a PID that matches Gender, then force the other correlations since they won't invalidate the gender.
        uint pid = ((gval ^ TID16 ^ SID16) << 16) | gval;
        var xor = 0u;
        if ((pid & 0x10000) != abilityBit << 16)
        {
            pid ^= 0x10000;
            xor = 1;
        }
        if (xor != shinyType)
            pid ^= 1u;

        return pid;
    }

    /// <summary>
    /// Calculates a shiny PID based on the provided parameters.
    /// </summary>
    /// <param name="gval">The random gender value that gives the desired gender.</param>
    /// <param name="abilityBit">A bit indicating the desired ability slot. Must be 0 or 1. This affects the resulting PID's ability-related bit.</param>
    /// <param name="TID16">The 16-bit Trainer ID (TID) of the Pokémon's trainer.</param>
    /// <param name="SID16">The 16-bit Secret ID (SID) of the Pokémon's trainer.</param>
    public static uint GetShinyPID(uint gval, uint abilityBit, ushort TID16, ushort SID16)
    {
        uint pid = ((gval ^ TID16 ^ SID16) << 16) | gval;
        if ((pid & 0x10000) != (abilityBit & 1) << 16)
            pid ^= 0x10000;
        return pid;
    }
}
