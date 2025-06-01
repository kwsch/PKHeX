using System;

namespace PKHeX.Core;

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

    public static bool TrySetIVs(XK3 pk, EncounterCriteria criteria, byte levelMin, byte levelMax)
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

    public static void SetRandomPID(XK3 pk, EncounterCriteria criteria, byte gender, byte slot)
        => pk.PID = GetRandomPID(pk.ID32, criteria, gender, slot, Util.Rand32(), out _);

    public static uint GetRandomPID(uint id32, EncounterCriteria criteria, byte gender, byte slot, uint seed, out uint origin)
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

            if (criteria.Shiny.IsShiny() != ShinyUtil.GetIsShiny(id32, pid, 8))
                continue;

            return pid;
        }
    }

    public static void SetRandomIVs(XK3 pk, EncounterCriteria criteria, byte levelMin, byte levelMax, uint seed)
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

    public static bool IsSlotValid(byte slot, uint esv) => GetSlot(esv) == slot;

    public static byte GetSlot(uint esv) => esv switch
    {
        < 50 => 0,
        < 85 => 1,
        _ => 2,
    };

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

public enum PokeSpotSetup : byte
{
    Invalid,
    Neither,
    Munchlax,
}
