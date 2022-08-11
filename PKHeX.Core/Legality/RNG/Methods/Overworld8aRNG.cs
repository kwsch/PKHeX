using System;
using System.Runtime.CompilerServices;

namespace PKHeX.Core;

/// <summary>
/// Contains logic for the Generation 8 (Legends: Arceus) overworld spawns that walk around the overworld.
/// </summary>
public static class Overworld8aRNG
{
    private static uint GetShinyPID(int tid, int sid, uint pid, int type)
    {
        return (uint)(((tid ^ sid ^ (pid & 0xFFFF) ^ type) << 16) | (pid & 0xFFFF));
    }

    private static bool GetIsShiny(int tid, int sid, uint pid)
    {
        return GetShinyXor(tid, sid, pid) < 16;
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

    private const int UNSET = -1;

    public static (ulong GroupSeed, ulong SlotSeed) ApplyDetails(PKM pk, EncounterCriteria criteria, in OverworldParam8a para, bool giveAlphaMove)
    {
        int ctr = 0;
        const int maxAttempts = 50_000;
        var fakeRand = new Xoroshiro128Plus(Util.Rand.Rand64());

        Xoroshiro128Plus groupRand;
        ulong groupSeed;
        ulong slotSeed;
        do
        {
            groupSeed = fakeRand.Next();
            groupRand = new Xoroshiro128Plus(groupSeed);
            slotSeed = groupRand.Next();
            var slotRand = new Xoroshiro128Plus(slotSeed);
            _ = slotRand.Next();
            var entitySeed = slotRand.Next();
            var result = TryApplyFromSeed(pk, criteria, para, entitySeed);
            if (result)
                break;
        } while (++ctr != maxAttempts);

        // Failed, fall back to Unrestricted and just put whatever.
        if (ctr >= maxAttempts)
        {
            groupSeed = fakeRand.Next();
            groupRand = new Xoroshiro128Plus(groupSeed);
            var slotRand = new Xoroshiro128Plus(slotSeed);
            _ = slotRand.Next();
            var entitySeed = slotRand.Next();
            TryApplyFromSeed(pk, EncounterCriteria.Unrestricted, para, entitySeed);
        }

        if (giveAlphaMove)
            ApplyRandomAlphaMove(pk, groupRand.Next());

        return (groupSeed, slotSeed);
    }

    public static (ulong EntitySeed, ulong SlotRand) ApplyDetails(PKM pk, in OverworldParam8a para, bool giveAlphaMove, ref Xoroshiro128Plus groupRand)
    {
        var slotSeed = groupRand.Next();
        var alphaSeed = groupRand.Next();

        var slotRand = new Xoroshiro128Plus(slotSeed);
        var slotRoll = slotRand.Next();
        var entitySeed = slotRand.Next();
        TryApplyFromSeed(pk, EncounterCriteria.Unrestricted, para, entitySeed);
        if (giveAlphaMove)
            ApplyRandomAlphaMove(pk, alphaSeed);

        return (entitySeed, slotRoll);
    }

    private static void ApplyRandomAlphaMove(PKM pk, ulong seed)
    {
        var pi = PersonalTable.LA.GetFormEntry(pk.Species, pk.Form);
        var count = pi.GetMoveShopCount();
        if (count == 0 || pk is not PA8 pa8)
            return;

        var index = GetRandomAlphaMoveIndex(seed, count);
        var alphaIndex = pi.GetMoveShopIndex(index);
        var alphaMove = Legal.MoveShop8_LA[alphaIndex];

        pa8.AlphaMove = alphaMove;
    }

    private static int GetRandomAlphaMoveIndex(ulong alphaSeed, int count)
    {
        var alphaRand = new Xoroshiro128Plus(alphaSeed);
        return (int)alphaRand.NextInt((uint)count);
    }

    public static bool TryApplyFromSeed(PKM pk, EncounterCriteria criteria, in OverworldParam8a para, ulong seed)
    {
        var rand = new Xoroshiro128Plus(seed);

        // Encryption Constant
        pk.EncryptionConstant = (uint)rand.NextInt();

        // PID
        var fakeTID = (uint)rand.NextInt();
        uint pid = (uint)rand.NextInt();
        bool isShiny = false;
        if (para.Shiny == Shiny.Random) // let's decide if it's shiny or not!
        {
            for (int i = 1; i < para.RollCount; i++)
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

        var xor = GetShinyXor(pk.TID, pk.SID, pid);
        var type = GetRareType(xor);
        if (para.Shiny == Shiny.Never)
        {
            if (type != Shiny.Never)
                return false;
        }
        else if (para.Shiny != Shiny.Random)
        {
            if (type == Shiny.Never)
                return false;

            if (para.Shiny == Shiny.AlwaysSquare && type != Shiny.AlwaysSquare)
                return false;
            if (para.Shiny == Shiny.AlwaysStar && type != Shiny.AlwaysStar)
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

    private static Shiny GetRareType(uint xor) => xor switch
    {
        0 => Shiny.AlwaysSquare,
        < 16 => Shiny.AlwaysStar,
        _ => Shiny.Never,
    };

    public static bool Verify(PKM pk, ulong seed, in OverworldParam8a para)
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
            for (int i = 1; i < para.RollCount; i++)
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
        Span<int> ivs = stackalloc[] { UNSET, UNSET, UNSET, UNSET, UNSET, UNSET };
        const int MAX = 31;
        for (int i = 0; i < para.FlawlessIVs; i++)
        {
            int index;
            do { index = (int)rand.NextInt(6); }
            while (ivs[index] != UNSET);

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

    public static int GetRandomLevel(ulong slotSeed, byte LevelMin, byte LevelMax)
    {
        var delta = LevelMax - LevelMin;
        var xoro = new Xoroshiro128Plus(slotSeed);
        xoro.Next();
        xoro.Next(); // slot, entitySeed
        var amp = (int)xoro.NextInt((ulong)delta + 1);
        return LevelMin + amp;
    }
}

public readonly record struct OverworldParam8a(bool IsAlpha, byte GenderRatio, byte FlawlessIVs, byte RollCount, Shiny Shiny = Shiny.Random);
