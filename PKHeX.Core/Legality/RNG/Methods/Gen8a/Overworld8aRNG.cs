using System;
using static PKHeX.Core.ShinyUtil;

namespace PKHeX.Core;

/// <summary>
/// Contains logic for the Generation 8 (Legends: Arceus) overworld spawns that walk around the overworld.
/// </summary>
public static class Overworld8aRNG
{
    private const int UNSET = -1;

    // ReSharper disable once UnusedTupleComponentInReturnValue
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

    // ReSharper disable once UnusedTupleComponentInReturnValue
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
        pa8.AlphaMove = PersonalInfo8LA.GetMoveShopMove(alphaIndex);
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

        ForceShinyState(isShiny, ref pid, pk.ID32, 0);

        var xor = GetShinyXor(pk.ID32, pid);
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

        Span<int> ivs = [UNSET, UNSET, UNSET, UNSET, UNSET, UNSET];
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
                ivs[i] = (int)rand.NextInt(MAX + 1);
        }

        if (!criteria.IsIVsCompatibleSpeedLast(ivs, 8))
            return false;

        pk.IV_HP = ivs[0];
        pk.IV_ATK = ivs[1];
        pk.IV_DEF = ivs[2];
        pk.IV_SPA = ivs[3];
        pk.IV_SPD = ivs[4];
        pk.IV_SPE = ivs[5];

        pk.RefreshAbility((int)rand.NextInt(2));

        byte gender = para.GenderRatio switch
        {
            PersonalInfo.RatioMagicGenderless => 2,
            PersonalInfo.RatioMagicFemale => 1,
            PersonalInfo.RatioMagicMale => 0,
            _ => rand.NextInt(252) + 1 < para.GenderRatio ? (byte)1 : (byte)0,
        };
        if (!criteria.IsGenderSatisfied(gender))
            return false;
        pk.Gender = gender;

        var nature = (Nature)rand.NextInt(25);
        pk.Nature = pk.StatNature = nature;

        var (height, weight) = para.IsAlpha
            ? (byte.MaxValue, byte.MaxValue)
            : ((byte)(rand.NextInt(0x81) + rand.NextInt(0x80)),
               (byte)(rand.NextInt(0x81) + rand.NextInt(0x80)));

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

    public static bool Verify(PKM pk, ulong seed, in OverworldParam8a para,
        bool isFixedH = false, bool isFixedW = false)
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

        ForceShinyState(isShiny, ref pid, pk.ID32, 0);

        if (pk.PID != pid)
            return false;
        Span<int> ivs = [UNSET, UNSET, UNSET, UNSET, UNSET, UNSET];
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

        int ability = (int)rand.NextInt(2);
        ability <<= 1; // 1/2/4

        var current = pk.AbilityNumber;
        if (ability == 4 && current != 4)
            return false;
        // else, for things that were made Hidden Ability, defer to Ability Checks (Ability Patch)

        byte gender = para.GenderRatio switch
        {
            PersonalInfo.RatioMagicGenderless => 2,
            PersonalInfo.RatioMagicFemale => 1,
            PersonalInfo.RatioMagicMale => 0,
            _ => rand.NextInt(252) + 1 < para.GenderRatio ? (byte)1 : (byte)0,
        };
        if (pk.Gender != gender)
            return false;

        var nature = (Nature)rand.NextInt(25);
        if (pk.Nature != nature)
            return false;

        var (height, weight) = para.IsAlpha
            ? (byte.MaxValue, byte.MaxValue)
            : ((byte)(rand.NextInt(0x81) + rand.NextInt(0x80)),
               (byte)(rand.NextInt(0x81) + rand.NextInt(0x80)));

        if (pk is IScaledSize s)
        {
            if (!isFixedH && s.HeightScalar != height)
                return false;
            if (!isFixedW && s.WeightScalar != weight)
                return false;
        }

        return true;
    }

    public static byte GetRandomLevel(ulong slotSeed, byte min, byte max)
    {
        var delta = 1ul + max - min;
        var rnd = new Xoroshiro128Plus(slotSeed);
        rnd.Next();
        rnd.Next(); // slot, entitySeed
        var amp = (byte)rnd.NextInt(delta);
        return (byte)(min + amp);
    }
}
