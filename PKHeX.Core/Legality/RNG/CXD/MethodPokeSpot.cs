using System;

namespace PKHeX.Core;

/// <summary>
/// Logic for interacting with PokeSpot encounters in Pokémon XD: Gale of Darkness.
/// </summary>
public static class MethodPokeSpot
{
    /// <summary>
    /// Checks if the PID generating seed is valid for the given slot.
    /// </summary>
    /// <param name="slot">Encounter slot index (0-2).</param>
    /// <param name="seed">PID generating seed.</param>
    /// <param name="origin">Origin seed.</param>
    public static PokeSpotSetup IsValidActivation(byte slot, uint seed, out uint origin)
    {
        // Forward call structure: depends on if Munchlax (10%) or Bonsly (30%) available to spawn
        // 0: Origin
        // 1: rand(003) == 0 - Activation
        // w: rand(100) < [10 or 30 or 40] -- if neither available to spawn, skip.
        // x: rand(100) encounter slot
        // yz: pid

        origin = 0;
        // wild
        var esv = (seed >> 16) % 100;
        if (!IsSlotValid(slot, esv))
            return PokeSpotSetup.Invalid;

        // Assume neither available to spawn.
        var preSlot = XDRNG.Prev16(ref seed);
        if (preSlot % 3 == 0)
        {
            origin = XDRNG.Prev(seed);
            return PokeSpotSetup.Neither;
        }

        // Assume only Munchlax available
        if (preSlot % 100 < 10)
            return PokeSpotSetup.Invalid;
        preSlot = XDRNG.Prev16(ref seed);
        if (preSlot % 3 == 0)
        {
            origin = XDRNG.Prev(seed);
            return PokeSpotSetup.Munchlax;
        }

        return PokeSpotSetup.Invalid;
    }

    /// <summary>
    /// Attempts to retrieve the origin seeds for the specified Pokémon and encounter slot.
    /// </summary>
    /// <param name="pk">The Pokémon for which to retrieve the origin seeds.</param>
    /// <param name="slot">The encounter slot associated with the Pokémon.</param>
    /// <param name="pid">When this method returns, contains an origin seed for the Pokémon's PID, if the operation succeeds.</param>
    /// <param name="ivs">When this method returns, contains an origin seed for the Pokémon's IVs, if the operation succeeds.</param>
    /// <returns><see langword="true"/> if both the PID and IV origin seeds are successfully retrieved; otherwise, <see langword="false"/>.</returns>
    public static bool TryGetOriginSeeds(PKM pk, EncounterSlot3XD slot, out uint pid, out uint ivs)
    {
        pid = 0;
        ivs = 0;
        if (!TryGetOriginSeedPID(pk.PID, slot.SlotNumber, out pid))
            return false;
        if (!TryGetOriginSeedIVs(pk, slot.LevelMin, slot.LevelMax, out ivs))
            return false;
        return true;
    }

    /// <summary>
    /// Attempts to determine the origin seed based on the provided Pokémon's IVs and level constraints.
    /// </summary>
    /// <param name="pk">The Pokémon whose IVs and metadata are used to calculate the origin seed.</param>
    /// <param name="levelMin">The minimum level of the encounter slot.</param>
    /// <param name="levelMax">The maximum level of the encounter slot.</param>
    /// <param name="origin">Origin seed if the operation succeeds; otherwise, is 0.</param>
    /// <returns><see langword="true"/> if the origin seed was successfully determined; otherwise, <see langword="false"/>.</returns>
    public static bool TryGetOriginSeedIVs(PKM pk, byte levelMin, byte levelMax, out uint origin)
    {
        var hp = (uint)pk.IV_HP;
        var atk = (uint)pk.IV_ATK;
        var def = (uint)pk.IV_DEF;
        var spa = (uint)pk.IV_SPA;
        var spd = (uint)pk.IV_SPD;
        var spe = (uint)pk.IV_SPE;
        var iv1 = hp | (atk << 5) | (def << 10);
        var iv2 = spe | (spa << 5) | (spd << 10);

        return TryGetOriginSeedIVs(iv1 << 16, iv2 << 16, levelMin, levelMax, pk.MetLevel, pk.Format == 3, out origin);
    }

    /// <summary>
    /// Attempts to determine the origin seed for a given PID and encounter slot.
    /// </summary>
    /// <param name="pid">The PID to analyze.</param>
    /// <param name="slot">The encounter slot index to validate against.</param>
    /// <param name="origin">Origin seed if the operation succeeds; otherwise, is 0.</param>
    /// <returns><see langword="true"/> if the origin seed was successfully determined; otherwise, <see langword="false"/>.</returns>
    public static bool TryGetOriginSeedPID(uint pid, byte slot, out uint origin)
    {
        Span<uint> seeds = stackalloc uint[XDRNG.MaxCountSeedsPID];
        var count = XDRNG.GetSeeds(seeds, pid);
        if (count == 0)
        {
            origin = 0;
            return false;
        }

        var reg = seeds[..count];
        foreach (var seed in reg)
        {
            // check for valid encounter slot info
            if (IsValidActivation(slot, seed, out origin) == PokeSpotSetup.Invalid)
                continue;

            return true;
        }
        origin = 0;
        return false;
    }

    /// <summary>
    /// Attempts to determine the origin seed based on the provided IVs and level constraints.
    /// </summary>
    /// <param name="iv1">The first individual value (IV) component used for seed determination, already shifted left 16 bits.</param>
    /// <param name="iv2">The second individual value (IV) component used for seed determination, already shifted left 16 bits.</param>
    /// <param name="levelMin">The minimum level of the encounter slot.</param>
    /// <param name="levelMax">The maximum level of the encounter slot.</param>
    /// <param name="metLevel">The level at which the entity was met. This is used to validate the origin seed.</param>
    /// <param name="hasOriginalMetLevel">
    /// Whether the <paramref name="metLevel"/> must match the calculated level exactly.
    /// If <see langword="true"/>, the calculated level must equal <paramref name="metLevel"/>;
    /// otherwise, the calculated level must be greater than or equal to <paramref name="metLevel"/>.
    /// </param>
    /// <param name="origin">Origin seed if the operation succeeds; otherwise, is 0.</param>
    /// <returns><see langword="true"/> if the origin seed was successfully determined; otherwise, <see langword="false"/>.</returns>
    public static bool TryGetOriginSeedIVs(uint iv1, uint iv2, byte levelMin, byte levelMax, byte metLevel, bool hasOriginalMetLevel, out uint origin)
    {
        Span<uint> seeds = stackalloc uint[XDRNG.MaxCountSeedsIV];
        var count = XDRNG.GetSeedsIVs(seeds, iv1, iv2);
        if (count == 0)
        {
            origin = 0;
            return false;
        }

        var levelDelta = 1u + levelMax - levelMin;
        foreach (var preIV in seeds[..count])
        {
            // origin
            // {u16 fakeID, u16 fakeID}
            // ???
            // levelRand
            // {u16 fakePID, u16 fakePID} => you are here (origin)
            // {u16 iv1, u16 iv2}
            // ability
            origin = XDRNG.Prev6(preIV);
            if (!IsValidAnimation(origin, out origin, out _))
                continue;

            var lvl16 = XDRNG.Prev2(preIV) >> 16;
            var lvlRnd = lvl16 % levelDelta;
            var lvl = (byte)(levelMin + lvlRnd);
            if (hasOriginalMetLevel ? (metLevel != lvl) : (metLevel < lvl))
                continue;

            var abil16 = XDRNG.Next3(preIV) >> 16;
            var abit = abil16 & 1; // don't care about ability, might be reset on evolution

            return true;
        }

        origin = 0;
        return false;
    }

    /// <summary>
    /// Attempts to set the individual values (IVs), level, and ability of the specified Pokémon based on the provided encounter criteria and level range.
    /// </summary>
    /// <param name="pk">The Pokémon object to modify.</param>
    /// <param name="criteria">The encounter criteria used to determine permissible random IVs and levels.</param>
    /// <param name="levelMin">The minimum level of the encounter slot.</param>
    /// <param name="levelMax">The maximum level of the encounter slot.</param>
    /// <returns><see langword="true"/> if all values were successfully applied; otherwise, <see langword="false"/>.</returns>
    public static bool TrySetIVs(XK3 pk, in EncounterCriteria criteria, byte levelMin, byte levelMax)
    {
        Span<uint> seeds = stackalloc uint[XDRNG.MaxCountSeedsIV];
        criteria.GetCombinedIVs(out var iv1, out var iv2);
        var count = XDRNG.GetSeedsIVs(seeds, iv1 << 16, iv2 << 16);
        if (count == 0)
            return false;

        var levelDelta = 1u + levelMax - levelMin;
        bool checkLevel = criteria.IsSpecifiedLevelRange() && criteria.IsLevelWithinRange(levelMin, levelMax);
        foreach (var preIV in seeds[..count])
        {
            var origin = XDRNG.Prev6(preIV);
            if (!IsValidAnimation(origin, out origin, out _))
                continue;

            // origin
            // {u16 fakeID, u16 fakeID}
            // ???
            // levelRand
            // {u16 fakePID, u16 fakePID} => you are here (origin)
            // {u16 iv1, u16 iv2}
            // ability

            var lvl16 = XDRNG.Prev2(preIV) >> 16;
            var lvlRnd = lvl16 % levelDelta;
            var lvl = (byte)(levelMin + lvlRnd);
            if (checkLevel && !criteria.IsSatisfiedLevelRange(lvl))
                continue;

            var abil16 = XDRNG.Next3(preIV) >> 16;
            var abit = abil16 & 1; // don't care about ability, might be reset on evolution

            SetIVs(pk, iv1, iv2);
            pk.RefreshAbility((int)(abit & 1));
            pk.CurrentLevel = pk.MetLevel = lvl;
            return true;
        }

        return false;
    }

    /// <summary>
    /// Sets a random PID for the specified Pokémon based on the provided criteria.
    /// </summary>
    /// <param name="pk">The Pokémon object whose PID will be set.</param>
    /// <param name="criteria">The encounter criteria used to determine the PID.</param>
    /// <param name="gender">
    /// The gender of the Pokémon.
    /// Use <see langword="0"/> for male, <see langword="1"/> for female, or <see langword="2"/> for genderless.
    /// </param>
    /// <param name="slot">The encounter slot index that the encounter originated from.</param>
    public static void SetRandomPID(XK3 pk, in EncounterCriteria criteria, byte gender, byte slot)
        => pk.PID = GetRandomPID(pk.ID32, criteria, gender, slot, Util.Rand32(), out _);

    /// <summary>
    /// Generates a random PID for the specified Pokémon based on the provided criteria.
    /// </summary>
    /// <param name="id32">The Trainer ID used to determine shiny status.</param>
    /// <param name="criteria">The encounter criteria that the generated PID must satisfy, such as nature, gender, and shiny status.</param>
    /// <param name="gender">The gender ratio of the Pokémon, used to determine valid gender values.</param>
    /// <param name="slot">The encounter slot index that the encounter originated from.</param>
    /// <param name="seed">The initial RNG seed used to generate the PID. Will repeatedly attempt with different adjacent seeds.</param>
    /// <param name="origin">Origin seed that was used to generate the PID.</param>
    public static uint GetRandomPID(uint id32, in EncounterCriteria criteria, byte gender, byte slot, uint seed, out uint origin)
    {
        while (true)
        {
            var result = IsValidActivation(slot, seed, out origin);
            if (result == PokeSpotSetup.Invalid)
            {
                seed = XDRNG.Next(seed);
                continue;
            }
            var pid = GetPIDRegular(ref seed);
            if (criteria.IsSpecifiedNature() && !criteria.IsSatisfiedNature((Nature)(pid % 25)))
                continue;
            if (criteria.IsSpecifiedGender() && !criteria.IsSatisfiedGender(EntityGender.GetFromPIDAndRatio(pid, gender)))
                continue;

            if (criteria.Shiny.IsShiny() != ShinyUtil.GetIsShiny3(id32, pid))
                continue;

            return pid;
        }
    }

    /// <summary>
    /// Sets random Individual Values (IVs) and level for the specified Pokémon based on the given criteria and seed.
    /// </summary>
    /// <param name="pk">The Pokémon object whose IVs and level will be set.</param>
    /// <param name="criteria">The encounter criteria used to filter valid IVs, level range, and other conditions.</param>
    /// <param name="levelMin">The minimum level of the encounter slot.</param>
    /// <param name="levelMax">The maximum level of the encounter slot.</param>
    /// <param name="seed">The initial RNG seed used to generate the random IVs and level. Will repeatedly attempt with different adjacent seeds.</param>
    public static void SetRandomIVs(XK3 pk, in EncounterCriteria criteria, byte levelMin, byte levelMax, uint seed)
    {
        var levelDelta = 1u + levelMax - levelMin;

        bool filterIVs = criteria.IsSpecifiedIVs(2);
        bool checkLevel = criteria.IsSpecifiedLevelRange() && criteria.IsLevelWithinRange(levelMin, levelMax);
        while (true)
        {
            // origin
            // {u16 fakeID, u16 fakeID}
            // ???
            // levelRand
            // {u16 fakePID, u16 fakePID} => you are here (origin)
            // {u16 iv1, u16 iv2}
            // ability
            var preIV = seed;

            var origin = XDRNG.Prev6(preIV);
            if (!IsValidAnimation(origin, out origin, out _))
            {
                seed = XDRNG.Next(seed); // avoid infinite loop
                continue;
            }

            var iv1 = XDRNG.Next15(ref seed);
            var iv2 = XDRNG.Next15(ref seed);
            var iv32 = iv2 << 15 | iv1;
            if (criteria.IsSpecifiedHiddenPower() && !criteria.IsSatisfiedHiddenPower(iv32))
                continue;
            if (filterIVs && !criteria.IsSatisfiedIVs(iv32))
                continue;

            var lvl16 = XDRNG.Prev2(preIV) >> 16;
            var lvlRnd = lvl16 % levelDelta;
            var lvl = (byte)(levelMin + lvlRnd);
            if (checkLevel && !criteria.IsSatisfiedLevelRange(lvl))
                continue;

            var abil16 = XDRNG.Next3(preIV) >> 16;
            var abit = abil16 & 1; // don't care about ability, might be reset on evolution

            SetIVs(pk, iv1, iv2);
            pk.RefreshAbility((int)(abit & 1));
            pk.CurrentLevel = pk.MetLevel = lvl;
            return;
        }
    }

    private static void SetIVs(XK3 pk, uint iv1, uint iv2)
        => pk.SetIVs(iv2 << 15 | iv1);

    private static uint GetPIDRegular(ref uint seed)
    {
        var a = XDRNG.Next16(ref seed);
        var b = XDRNG.Next16(ref seed);
        return GetPIDRegular(a, b);
    }

    private static uint GetPIDRegular(uint a, uint b) => a << 16 | b;

    /// <summary>
    /// Determines whether the specified slot is valid for the given ESV value.
    /// </summary>
    /// <param name="slot">The slot index to validate.</param>
    /// <param name="esv">The ESV value [0,99] from the RNG.</param>
    public static bool IsSlotValid(byte slot, uint esv) => GetSlot(esv) == slot;

    /// <summary>
    /// Determines the slot number based on the provided ESV (Encounter Slot Value).
    /// </summary>
    /// <param name="esv">The Encounter Slot Value (ESV) used to determine the slot.</param>
    /// <returns>
    /// A byte representing the slot number:
    /// <list type="bullet">
    ///   <item><description>0 if <paramref name="esv"/> is less than 50.</description></item>
    ///   <item><description>1 if <paramref name="esv"/> is between 50 (inclusive) and 85 (exclusive).</description></item>
    ///   <item><description>2 if <paramref name="esv"/> is 85 or greater.</description></item>
    /// </list>
    /// </returns>
    public static byte GetSlot(uint esv) => esv switch
    {
        < 50 => 0,
        < 85 => 1,
        _ => 2,
    };

    /// <summary>
    /// Determines whether a valid animation pattern can be generated based on the given seed.
    /// </summary>
    /// <param name="seed">The initial seed value used to generate the encounter slot.</param>
    /// <param name="origin">The origin seed that generates the animation pattern, followed by the encounter slot.</param>
    /// <param name="animation">Observed animation pattern.</param>
    /// <returns><see langword="true"/> if a valid pre-animation seed is found; otherwise, <see langword="false"/>.</returns>
    public static bool IsValidAnimation(uint seed, out uint origin, out uint animation)
    {
        // Origin
        // Get a random animation that isn't 3:
        //   rand(10) until != 3
        // Based on animation, sub-randomize it:
        //   if 8, Next5
        //   if >= 5, Next2
        //   if <= 4, Next

        // Look backwards, starting with Prev
        var prev16 = XDRNG.Prev16(ref seed);
        animation = prev16 % 10;
        if (animation is < 5 and not 3)
        {
            origin = XDRNG.Prev(seed);
            return true;
        }

        prev16 = XDRNG.Prev16(ref seed);
        animation = prev16 % 10;
        if (animation is >= 5 and not 8)
        {
            origin = XDRNG.Prev(seed);
            return true;
        }

        seed = XDRNG.Prev3(seed);
        animation = (seed >> 16) % 10;
        if (animation is 8)
        {
            origin = XDRNG.Prev(seed);
            return true;
        }

        origin = 0;
        return false;
    }
}

/// <summary>
/// Represents the current game state when generating a PokéSpot in the game.
/// </summary>
public enum PokeSpotSetup : byte
{
    /// <summary>
    /// Represents an invalid or uninitialized state.
    /// </summary>
    Invalid,

    /// <summary>
    /// Neither Munchlax nor Bonsly is available to spawn in the PokéSpot.
    /// </summary>
    Neither,

    /// <summary>
    /// Munchlax is not available to spawn; either not unlocked or already at another PokéSpot.
    /// </summary>
    Munchlax,

    // No need to differentiate Bonsly from Munchlax, as having only Bonsly available is less forgiving odds (30%) than having only Munchlax (10%) available.
}
