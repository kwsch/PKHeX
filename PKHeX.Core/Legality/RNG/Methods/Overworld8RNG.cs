using System;

namespace PKHeX.Core;

/// <summary>
/// Contains logic for the Generation 8 (SW/SH) overworld spawns that walk around the overworld.
/// </summary>
/// <remarks>
/// Entities spawned into the overworld that can be encountered are assigned a 32bit seed, which can be immediately derived from the <see cref="PKM.EncryptionConstant"/>.
/// </remarks>
public static class Overworld8RNG
{
    public static void ApplyDetails(PKM pk, EncounterCriteria criteria, Shiny shiny = Shiny.FixedValue, int flawless = -1, int maxAttempts = 70_000)
    {
        if (shiny == Shiny.FixedValue)
            shiny = criteria.Shiny is Shiny.Random or Shiny.Never ? Shiny.Never : criteria.Shiny;
        if (flawless == -1)
            flawless = 0;

        int ctr = 0;
        var rnd = Util.Rand;
        do
        {
            var seed = rnd.Rand32();
            if (TryApplyFromSeed(pk, criteria, shiny, flawless, seed))
                return;
        } while (++ctr != maxAttempts);
        TryApplyFromSeed(pk, EncounterCriteria.Unrestricted, shiny, flawless, rnd.Rand32());
    }

    private static bool TryApplyFromSeed(PKM pk, EncounterCriteria criteria, Shiny shiny, int flawless, uint seed)
    {
        var xoro = new Xoroshiro128Plus(seed);

        // Encryption Constant
        pk.EncryptionConstant = (uint) xoro.NextInt(uint.MaxValue);

        // PID
        var pid = (uint) xoro.NextInt(uint.MaxValue);
        var xor = GetShinyXor(pk.TID, pk.SID, pid);
        var type = GetRareType(xor);
        if (shiny == Shiny.Never)
        {
            if (type != Shiny.Never)
                return false;
        }
        else if (shiny != Shiny.Random)
        {
            if (type == Shiny.Never)
                return false;

            if (shiny == Shiny.AlwaysSquare && type != Shiny.AlwaysSquare)
                return false;
            if (shiny == Shiny.AlwaysStar && type != Shiny.AlwaysStar)
                return false;
        }

        pk.PID = pid;

        // IVs
        Span<int> ivs = stackalloc[] {UNSET, UNSET, UNSET, UNSET, UNSET, UNSET};
        const int MAX = 31;
        for (int i = 0; i < flawless; i++)
        {
            int index;
            do { index = (int) xoro.NextInt(6); }
            while (ivs[index] != UNSET);

            ivs[index] = MAX;
        }

        for (int i = 0; i < ivs.Length; i++)
        {
            if (ivs[i] == UNSET)
                ivs[i] = (int) xoro.NextInt(32);
        }

        if (!criteria.IsIVsCompatible(ivs, 8))
            return false;

        pk.IV_HP = ivs[0];
        pk.IV_ATK = ivs[1];
        pk.IV_DEF = ivs[2];
        pk.IV_SPA = ivs[3];
        pk.IV_SPD = ivs[4];
        pk.IV_SPE = ivs[5];

        // Remainder
        var scale = (IScaledSize) pk;
        scale.HeightScalar = (byte)((int) xoro.NextInt(0x81) + (int) xoro.NextInt(0x80));
        scale.WeightScalar = (byte)((int) xoro.NextInt(0x81) + (int) xoro.NextInt(0x80));

        return true;
    }

    private static Shiny GetRareType(uint xor) => xor switch
    {
        0 => Shiny.AlwaysSquare,
        < 16 => Shiny.AlwaysStar,
        _ => Shiny.Never,
    };

    public static bool ValidateOverworldEncounter(PKM pk, Shiny shiny = Shiny.FixedValue, int flawless = -1)
    {
        var seed = GetOriginalSeed(pk);
        return ValidateOverworldEncounter(pk, seed, shiny, flawless);
    }

    public static bool ValidateOverworldEncounter(PKM pk, uint seed, Shiny shiny = Shiny.FixedValue, int flawless = -1)
    {
        // is the seed Xoroshiro determined, or just truncated state?
        if (seed == uint.MaxValue)
            return false;

        var xoro = new Xoroshiro128Plus(seed);
        var ec = (uint) xoro.NextInt(uint.MaxValue);
        if (ec != pk.EncryptionConstant)
            return false;

        var pid = (uint) xoro.NextInt(uint.MaxValue);
        if (!IsPIDValid(pk, pid, shiny))
            return false;

        var actualCount = flawless == -1 ? GetIsMatchEnd(pk, xoro) : GetIsMatchEnd(pk, xoro, flawless, flawless);
        return actualCount != NoMatchIVs;
    }

    private static bool IsPIDValid(PKM pk, uint pid, Shiny shiny)
    {
        if (shiny == Shiny.Random)
            return pid == pk.PID;

        if (pid == pk.PID)
            return true;

        // Check forced shiny
        if (pk.IsShiny)
        {
            if (GetIsShiny(pk.TID, pk.SID, pid))
                return false;

            pid = GetShinyPID(pk.TID, pk.SID, pid, 0);
            return pid == pk.PID;
        }

        // Check forced non-shiny
        if (!GetIsShiny(pk.TID, pk.SID, pid))
            return false;

        pid ^= 0x1000_0000;
        return pid == pk.PID;
    }

    private const int NoMatchIVs = -1;
    private const int UNSET = -1;

    private static int GetIsMatchEnd(PKM pk, Xoroshiro128Plus xoro, int start = 0, int end = 3)
    {
        bool skip1 = start == 0 && end == 3;
        for (int iv_count = start; iv_count <= end; iv_count++)
        {
            if (skip1 && iv_count == 1)
                continue;

            var copy = xoro;
            Span<int> ivs = stackalloc [] { UNSET, UNSET, UNSET, UNSET, UNSET, UNSET };
            const int MAX = 31;
            for (int i = 0; i < iv_count; i++)
            {
                int index;
                do { index = (int)copy.NextInt(6); } while (ivs[index] != UNSET);
                ivs[index] = MAX;
            }

            for (var i = 0; i < ivs.Length; i++)
            {
                if (ivs[i] == UNSET)
                    ivs[i] = (int)copy.NextInt(31 + 1);
            }

            if (ivs[0] != pk.IV_HP)  continue;
            if (ivs[1] != pk.IV_ATK) continue;
            if (ivs[2] != pk.IV_DEF) continue;
            if (ivs[3] != pk.IV_SPA) continue;
            if (ivs[4] != pk.IV_SPD) continue;
            if (ivs[5] != pk.IV_SPE) continue;

            if (pk is not IScaledSize s)
                continue;
            var height = (int) copy.NextInt(0x81) + (int) copy.NextInt(0x80);
            if (s.HeightScalar != height)
                continue;
            var weight = (int) copy.NextInt(0x81) + (int) copy.NextInt(0x80);
            if (s.WeightScalar != weight)
                continue;

            return iv_count;
        }
        return NoMatchIVs;
    }

    private static uint GetShinyPID(int tid, int sid, uint pid, int type)
    {
        return (uint)(((tid ^ sid ^ (pid & 0xFFFF) ^ type) << 16) | (pid & 0xFFFF));
    }

    private static bool GetIsShiny(int tid, int sid, uint pid)
    {
        return GetShinyXor(pid, (uint)((sid << 16) | tid)) < 16;
    }

    private static uint GetShinyXor(int tid, int sid, uint pid)
    {
        return GetShinyXor(pid, (uint)((sid << 16) | tid));
    }

    private static uint GetShinyXor(uint pid, uint oid)
    {
        var xor = pid ^ oid;
        return (xor ^ (xor >> 16)) & 0xFFFF;
    }

    /// <summary>
    /// Obtains the original seed for the Generation 8 overworld wild encounter.
    /// </summary>
    /// <param name="pk">Entity to check for</param>
    /// <returns>Seed</returns>
    public static uint GetOriginalSeed(PKM pk)
    {
        var seed = pk.EncryptionConstant - unchecked((uint)Xoroshiro128Plus.XOROSHIRO_CONST);
        if (seed == 0xD5B9C463) // Collision seed with the 0xFFFFFFFF re-roll.
        {
            var xoro = new Xoroshiro128Plus(seed);
            /*  ec */ xoro.NextInt(uint.MaxValue);
            var pid = xoro.NextInt(uint.MaxValue);
            if (pid != pk.PID)
                return 0xDD6295A4;
        }

        return seed;
    }
}
