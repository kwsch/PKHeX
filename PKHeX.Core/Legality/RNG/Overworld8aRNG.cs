using System;
using System.Runtime.CompilerServices;

namespace PKHeX.Core;

/// <summary>
/// Contains logic for the Generation 8 (Legends: Arceus) overworld spawns that walk around the overworld.
/// </summary>
public static class Overworld8aRNG
{
    public static uint AdaptPID(PKM pk, Shiny shiny, uint pid)
    {
        if (shiny == Shiny.Never)
        {
            if (GetIsShiny(pk.TID, pk.SID, pid))
                pid ^= 0x1000_0000;
        }
        else if (shiny != Shiny.Random)
        {
            if (!GetIsShiny(pk.TID, pk.SID, pid))
                pid = GetShinyPID(pk.TID, pk.SID, pid, 0);
        }
        return pid;
    }

    private static uint GetShinyPID(int tid, int sid, uint pid, int type)
    {
        return (uint)(((tid ^ sid ^ (pid & 0xFFFF) ^ type) << 16) | (pid & 0xFFFF));
    }

    private static bool GetIsShiny(int tid, int sid, uint pid)
    {
        return GetShinyXor(pid, (uint)((sid << 16) | tid)) < 16;
    }

    private static uint GetShinyXor(uint pid, uint oid)
    {
        var xor = pid ^ oid;
        return (xor ^ (xor >> 16)) & 0xFFFF;
    }

    private const int UNSET = -1;

    public static void ApplyDetails(PKM pk, EncounterCriteria criteria, in OverworldParam8a para)
    {
        int ctr = 0;
        const int maxAttempts = 50_000;
        var rnd = Util.Rand;
        do
        {
            ulong s0 = Util.Rand32(rnd) | (ulong)Util.Rand32(rnd) << 32;
            ulong s1 = Util.Rand32(rnd) | (ulong)Util.Rand32(rnd) << 32;
            var rand = new Xoroshiro128Plus(s0, s1);
            if (TryApplyFromSeed(pk, criteria, para, rand))
                return;
        } while (++ctr != maxAttempts);

        {
            ulong s0 = Util.Rand32(rnd) | (ulong)Util.Rand32(rnd) << 32;
            ulong s1 = Util.Rand32(rnd) | (ulong)Util.Rand32(rnd) << 32;
            var rand = new Xoroshiro128Plus(s0, s1);
            TryApplyFromSeed(pk, EncounterCriteria.Unrestricted, para, rand);
        }
    }

    public static bool TryApplyFromSeed(PKM pk, EncounterCriteria criteria, in OverworldParam8a para, Xoroshiro128Plus rand)
    {
        // Encryption Constant
        pk.EncryptionConstant = (uint)rand.NextInt();

        // PID
        var fakeTID = (uint)rand.NextInt();
        uint pid = (uint)rand.NextInt();
        bool isShiny = false;
        if (para.Shiny == Shiny.Random) // let's decide if it's shiny or not!
        {
            for (int i = 0; i < para.Rerolls; i++)
            {
                isShiny = GetShinyXor(pid, fakeTID) < 16;
                if (isShiny)
                    break;
                pid = (uint)rand.NextInt();
            }
        }
        else
        {
            // no need to calculate a fake trainer
            isShiny = para.Shiny == Shiny.Always;
        }

        ForceShinyState(pk, isShiny, ref pid);

        if (para.Shiny == Shiny.Never)
        {
            if (GetIsShiny(pk.TID, pk.SID, pid))
                return false;
        }
        else if (para.Shiny != Shiny.Random)
        {
            if (!GetIsShiny(pk.TID, pk.SID, pid))
                return false;

            if (para.Shiny == Shiny.AlwaysSquare && pk.ShinyXor != 0)
                return false;
            if (para.Shiny == Shiny.AlwaysStar && pk.ShinyXor == 0)
                return false;
        }
        pk.PID = pid;

        Span<int> ivs = stackalloc[] { UNSET, UNSET, UNSET, UNSET, UNSET, UNSET };
        const int MAX = 31;
        for (int i = 0; i < para.FlawlessIVs; i++)
        {
            int index;
            do { index = (int)rand.NextInt(6); }
            while (ivs[index] != UNSET);

            ivs[index] = MAX;
        }

        for (int i = 0; i < ivs.Length; i++)
        {
            if (ivs[i] == UNSET)
                ivs[i] = (int)rand.NextInt(32);
        }

        if (!criteria.IsIVsCompatible(ivs, 8))
            return false;

        pk.IV_HP = ivs[0];
        pk.IV_ATK = ivs[1];
        pk.IV_DEF = ivs[2];
        pk.IV_SPA = ivs[3];
        pk.IV_SPD = ivs[4];
        pk.IV_SPE = ivs[5];

        pk.RefreshAbility((int)rand.NextInt(2));

        int gender = para.GenderRatio switch
        {
            PersonalInfo.RatioMagicGenderless => 2,
            PersonalInfo.RatioMagicFemale => 1,
            PersonalInfo.RatioMagicMale => 0,
            _ => (int)rand.NextInt(252) + 1 < para.GenderRatio ? 1 : 0,
        };
        if (gender != criteria.Gender && criteria.Gender != -1)
            return false;
        pk.Gender = gender;

        int nature = (int)rand.NextInt(25);
        pk.StatNature = pk.Nature = nature;

        var (height, weight) = para.IsAlpha
            ? (byte.MaxValue, byte.MaxValue)
            : ((byte)((int)rand.NextInt(0x81) + (int)rand.NextInt(0x80)),
               (byte)((int)rand.NextInt(0x81) + (int)rand.NextInt(0x80)));

        if (pk is IScaledSize s)
        {
            s.HeightScalar = height;
            s.WeightScalar = weight;
            if (pk is IScaledSizeValue a)
            {
                a.ResetHeight();
                a.ResetWeight();
            }
        }

        return true;
    }

    private static bool Verify(PKM pk, ulong seed, Span<int> ivs, in OverworldParam8a para)
    {
        var rand = new Xoroshiro128Plus(seed);
        var ec = (uint)rand.NextInt();
        if (ec != pk.EncryptionConstant)
            return false;

        var fakeTID = (uint)rand.NextInt();
        uint pid = (uint)rand.NextInt();
        bool isShiny = false;
        if (para.Shiny == Shiny.Random) // let's decide if it's shiny or not!
        {
            for (int i = 0; i <= para.Rerolls; i++)
            {
                isShiny = GetShinyXor(pid, fakeTID) < 16;
                if (isShiny)
                    break;
                pid = (uint)rand.NextInt();
            }
        }
        else
        {
            // no need to calculate a fake trainer
            isShiny = para.Shiny == Shiny.Always;
        }

        ForceShinyState(pk, isShiny, ref pid);

        if (pk.PID != pid)
            return false;
        const int MAX = 31;
        for (int i = ivs.Count(MAX); i < para.Rerolls; i++)
        {
            int index = (int)rand.NextInt(6);
            while (ivs[index] != UNSET)
                index = (int)rand.NextInt(6);
            ivs[index] = MAX;
        }

        for (int i = 0; i < 6; i++)
        {
            if (ivs[i] == UNSET)
                ivs[i] = (int)rand.NextInt(32);
        }

        if (pk.IV_HP != ivs[0])
            return false;
        if (pk.IV_ATK != ivs[1])
            return false;
        if (pk.IV_DEF != ivs[2])
            return false;
        if (pk.IV_SPA != ivs[3])
            return false;
        if (pk.IV_SPD != ivs[4])
            return false;
        if (pk.IV_SPE != ivs[5])
            return false;

        int abil = (int)rand.NextInt(2);
        abil <<= 1; // 1/2/4

        var current = pk.AbilityNumber;
        if (abil == 4 && current != 4)
            return false;
        // else, for things that were made Hidden Ability, defer to Ability Checks (Ability Patch)

        int gender = para.GenderRatio switch
        {
            PersonalInfo.RatioMagicGenderless => 2,
            PersonalInfo.RatioMagicFemale => 1,
            PersonalInfo.RatioMagicMale => 0,
            _ => (int)rand.NextInt(252) + 1 < para.GenderRatio ? 1 : 0,
        };
        if (pk.Gender != gender)
            return false;

        var nature = (int)rand.NextInt(25);
        if (pk.Nature != nature)
            return false;

        var (height, weight) = para.IsAlpha
            ? (byte.MaxValue, byte.MaxValue)
            : ((byte)((int)rand.NextInt(0x81) + (int)rand.NextInt(0x80)),
               (byte)((int)rand.NextInt(0x81) + (int)rand.NextInt(0x80)));

        if (pk is IScaledSize s)
        {
            if (s.HeightScalar != height)
                return false;
            if (s.WeightScalar != weight)
                return false;
        }

        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ForceShinyState(PKM pk, bool isShiny, ref uint pid)
    {
        if (isShiny)
        {
            if (!GetIsShiny(pk.TID, pk.SID, pid))
                pid = GetShinyPID(pk.TID, pk.SID, pid, 0);
        }
        else
        {
            if (GetIsShiny(pk.TID, pk.SID, pid))
                pid ^= 0x1000_0000;
        }
    }
}

public readonly record struct OverworldParam8a
{
    public byte GenderRatio { get; init; }
    public bool IsAlpha { get; init; }
    public byte LevelMin { get; init; }
    public byte LevelMax { get; init; }

    public Shiny Shiny { get; init; } = Shiny.Random;
    public byte Rerolls { get; init; }
    public byte FlawlessIVs { get; init; }
}
