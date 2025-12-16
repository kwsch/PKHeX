using System;

namespace PKHeX.Core;

public static class LumioseRNG
{
    private const int UNSET = -1;
    private const int MAX = 31;

    public static bool Verify(PKM pk, in GenerateParam9a enc, ulong seed) => IsMatch(pk, enc, seed);

    /// <summary>
    /// Sets the <see cref="pk"/> with random data based on the <see cref="enc"/> and <see cref="criteria"/>.
    /// </summary>
    /// <returns>True if the generated data matches the <see cref="criteria"/>.</returns>
    public static bool TryApply64<TEnc>(this TEnc enc, PA9 pk, in ulong init, in GenerateParam9a param, in EncounterCriteria criteria)
        where TEnc : ISpeciesForm
    {
        var rand = new Xoroshiro128Plus(init);
        const int maxCtr = 100_000;
        for (int ctr = 0; ctr < maxCtr; ctr++)
        {
            ulong seed = rand.Next(); // fake cryptosecure
            if (!GenerateData(pk, param, criteria, seed))
                continue;
            return true; // done.
        }
        return false;
    }

    /// <summary>
    /// Fills out an entity with details from the provided encounter template.
    /// </summary>
    /// <returns>False if the seed cannot generate data matching the criteria.</returns>
    public static bool GenerateData(PA9 pk, in GenerateParam9a enc, in EncounterCriteria criteria, in ulong seed)
    {
        var rand = new Xoroshiro128Plus(seed);
        pk.EncryptionConstant = (uint)rand.NextInt(uint.MaxValue);
        pk.PID = GetAdaptedPID(ref rand, pk, enc);

        if (enc.Shiny is Shiny.Random && criteria.Shiny.IsShiny() != pk.IsShiny)
            return false;

        Span<int> ivs = [UNSET, UNSET, UNSET, UNSET, UNSET, UNSET];
        if (enc.IVs.IsSpecified)
        {
            enc.IVs.CopyToSpeedLast(ivs);
        }
        else if (enc.Correlation is LumioseCorrelation.PreApplyIVs)
        {
            if (enc.FlawlessIVs != 0)
                GenerouslyPreApplyIVs(criteria, ivs, enc.FlawlessIVs);
        }
        else
        {
            for (int i = 0; i < enc.FlawlessIVs; i++)
            {
                int index;
                do { index = (int)rand.NextInt(6); }
                while (ivs[index] != UNSET);
                ivs[index] = MAX;
            }
        }

        for (int i = 0; i < 6; i++)
        {
            if (ivs[i] == UNSET)
                ivs[i] = (int)rand.NextInt(MAX + 1);
        }

        if (enc.Correlation == LumioseCorrelation.ReApplyIVs)
        {
            criteria.SetRandomIVs(pk, flawless: enc.FlawlessIVs);
        }
        else
        {
            if (!criteria.IsIVsCompatibleSpeedLast(ivs))
                return false;
            pk.IV32 = (uint)ivs[0] |
                      (uint)(ivs[1] << 05) |
                      (uint)(ivs[2] << 10) |
                      (uint)(ivs[5] << 15) | // speed is last in the array, but in the middle of the 32bit value
                      (uint)(ivs[3] << 20) |
                      (uint)(ivs[4] << 25);
        }

        int ability = enc.Ability switch
        {
            AbilityPermission.Any12H => (int)rand.NextInt(3) << 1,
            AbilityPermission.Any12 => (int)rand.NextInt(2) << 1,
            _ => (int)enc.Ability,
        };
        pk.RefreshAbility(ability >> 1);

        var gender_ratio = enc.GenderRatio;
        byte gender = gender_ratio switch
        {
            PersonalInfo.RatioMagicGenderless => 2,
            PersonalInfo.RatioMagicFemale => 1,
            PersonalInfo.RatioMagicMale => 0,
            _ => GetGender(gender_ratio, rand.NextInt(100)),
        };
        if (criteria.IsSpecifiedGender() && !criteria.IsSatisfiedGender(gender))
            return false;
        pk.Gender = gender;

        var nature = enc.Nature != Nature.Random ? enc.Nature
            : (Nature)rand.NextInt(25);

        // Compromise on Nature -- some are fixed, some are random. If the request wants a specific nature, just mint it.
        if (criteria.IsSpecifiedNature() && !criteria.IsSatisfiedNature(nature))
            return false;
        pk.Nature = pk.StatNature = nature;

        // If Hyperspace, the player can have an active Teensy/Humungo boost. The scale is pre-determined outside of the seed=>pa9, consider it not correlated or traceable.
        // When calling the method to verify the entity, pass SizeType9.VALUE instead.
        pk.Scale = enc.SizeType.GetSizeValue(enc.Scale, ref rand);
        return true;
    }

    /// <summary>
    /// Gives a quantity of flawless IVs based on the criteria, unrelated to an RNG correlation.
    /// </summary>
    private static void GenerouslyPreApplyIVs(in EncounterCriteria criteria, Span<int> ivs, byte encFlawlessIVs)
    {
        // The game uses a separate seed to pre-fill flawless IVs.
        // Sure we can iterate [0,n), but that usually results in a less-than-random result (HP, ATK, DEF get filled first always).
        // So, we can "randomize" our indexes we apply to.
        Span<int> indexes = stackalloc int[6];
        for (int i = 0; i < indexes.Length; i++)
            indexes[i] = i;
        Util.Rand.Shuffle(indexes);

        // Try to give a perfect IV where it's wanted first
        for (int i = 0; i < ivs.Length; i++)
        {
            var index = indexes[i];
            if (ivs[index] != UNSET)
                continue;
            var desire = criteria.GetIV(index);
            if (desire is not MAX)
                continue;

            ivs[index] = MAX;
            encFlawlessIVs--;
            if (encFlawlessIVs == 0)
                return;
        }

        // Then try to give a perfect IV where it's allowed
        for (int i = 0; i < ivs.Length; i++)
        {
            var index = indexes[i];
            if (ivs[index] != UNSET)
                continue;
            var desire = criteria.GetIV(index);
            if (desire >= 0)
                continue;

            ivs[index] = MAX;
            encFlawlessIVs--;
            if (encFlawlessIVs == 0)
                return;
        }

        // Apply remaining flawless IVs if not <= 1
        for (int i = 0; i < ivs.Length; i++)
        {
            var index = indexes[i];
            if (ivs[index] != UNSET)
                continue;
            var desire = criteria.GetIV(index);
            if (desire is 0 or 1)
                continue;

            ivs[index] = MAX;
            encFlawlessIVs--;
            if (encFlawlessIVs == 0)
                return;
        }

        // If we reach here... we couldn't satisfy the flawless IV count requested. Probably the encounter isn't what the user expected/wanted.
        for (int i = 0; i < ivs.Length; i++)
        {
            var index = indexes[i];
            if (ivs[index] == UNSET)
                ivs[index] = MAX;
        }
    }

    public static bool IsMatch(PKM pk, in GenerateParam9a enc, in ulong seed)
    {
        // same as above method
        var rand = new Xoroshiro128Plus(seed);
        if (pk.EncryptionConstant != (uint)rand.NextInt(uint.MaxValue))
            return false;

        var pid = GetAdaptedPID(ref rand, pk, enc);
        if (pk.PID != pid)
            return false;

        if (enc.Correlation is LumioseCorrelation.PreApplyIVs)
            return IsMatchUnknownPreFillIVs(pk, in enc, rand);
        return IsMatchIVsAndFollowing(pk, enc, rand);
    }

    private static bool IsMatchIVsAndFollowing(PKM pk, in GenerateParam9a enc, Xoroshiro128Plus rand)
    {
        Span<int> ivs = [UNSET, UNSET, UNSET, UNSET, UNSET, UNSET];
        if (enc.IVs.IsSpecified)
            enc.IVs.CopyToSpeedLast(ivs);
        for (int i = 0; i < enc.FlawlessIVs; i++)
        {
            int index;
            do { index = (int)rand.NextInt(6); }
            while (ivs[index] != UNSET);
            ivs[index] = MAX;
        }
        return IsMatchIVsAndFollowing(pk, in enc, rand, ivs);
    }

    private static bool IsMatchIVsAndFollowing(PKM pk, in GenerateParam9a enc, Xoroshiro128Plus rand, Span<int> ivs)
    {
        for (int i = 0; i < 6; i++)
        {
            if (ivs[i] == UNSET)
                ivs[i] = (int)rand.NextInt(32);
        }

        if (enc.Correlation is not LumioseCorrelation.ReApplyIVs)
        {
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
        }

        // No way to change abilities. Index must match.
        int ability = enc.Ability switch
        {
            AbilityPermission.Any12H => 1 << (int)rand.NextInt(3),
            AbilityPermission.Any12 => 1 << (int)rand.NextInt(2),
            _ => (int)enc.Ability,
        };
        if (pk.AbilityNumber != ability)
            return false;

        var gender_ratio = enc.GenderRatio;
        byte gender = gender_ratio switch
        {
            PersonalInfo.RatioMagicGenderless => 2,
            PersonalInfo.RatioMagicFemale => 1,
            PersonalInfo.RatioMagicMale => 0,
            _ => GetGender(gender_ratio, rand.NextInt(100)),
        };
        if (pk.Gender != gender)
            return false;

        var nature = enc.Nature != Nature.Random ? enc.Nature
            : (Nature)rand.NextInt(25);
        if (pk.Nature != nature)
            return false;

        // Scale
        // If Hyperspace, the player can have an active Teensy/Humungo boost. The scale is pre-determined outside of the seed=>pa9, consider it not correlated or traceable.
        // When calling the method to verify the entity, pass SizeType9.VALUE instead.
        {
            var value = enc.SizeType.GetSizeValue(enc.Scale, ref rand);
            if (pk is IScaledSize3 s)
            {
                if (s.Scale != value)
                    return false;
            }
        }
        return true;
    }

    private static uint GetAdaptedPID(ref Xoroshiro128Plus rand, PKM pk, in GenerateParam9a enc)
    {
        var fakeTID = enc.Correlation is LumioseCorrelation.SkipTrainer ? pk.ID32 : (uint)rand.NextInt();
        uint pid = (uint)rand.NextInt();
        if (enc.Shiny == Shiny.Random) // let's decide if it's shiny or not!
        {
            int i = 1;
            bool isShiny;
            uint xor;
            while (true)
            {
                xor = ShinyUtil.GetShinyXor(pid, fakeTID);
                isShiny = xor < 16;
                if (isShiny)
                {
                    if (xor != 0)
                        xor = 1;
                    break;
                }
                if (i >= enc.RollCount)
                    break;
                pid = (uint)rand.NextInt();
                i++;
            }
            ShinyUtil.ForceShinyState(isShiny, ref pid, pk.ID32, xor);
        }
        else if (enc.Shiny == Shiny.Always)
        {
            var tid = (ushort)fakeTID;
            var sid = (ushort)(fakeTID >> 16);
            if (!ShinyUtil.GetIsShiny6(fakeTID, pid)) // battled
                pid = ShinyUtil.GetShinyPID(tid, sid, pid, 0);
            if (!ShinyUtil.GetIsShiny6(pk.ID32, pid)) // captured
                pid = ShinyUtil.GetShinyPID(pk.TID16, pk.SID16, pid, ShinyUtil.GetShinyXor(pid, fakeTID) == 0 ? 0u : 1u);
        }
        else // Never
        {
            if (ShinyUtil.GetIsShiny6(fakeTID, pid)) // battled
                pid ^= 0x1000_0000;
            if (ShinyUtil.GetIsShiny6(pk.ID32, pid)) // captured
                pid ^= 0x1000_0000;
        }
        return pid;
    }

    private static bool IsMatchUnknownPreFillIVs(PKM pk, in GenerateParam9a enc, Xoroshiro128Plus rand)
    {
        int k = enc.FlawlessIVs;
        if (k == 0)
            return IsMatchIVsAndFollowing(pk, in enc, rand, 0); // none
        if (k == 6)
            return IsMatchIVsAndFollowing(pk, in enc, rand, (1 << 6) - 1); // all

        // Treat flawless IVs as a combination problem: choose k flawless IVs from 6 stats.
        // Since we don't know the IVs that were filled before entering FixInitSpec, we must guess every combination.
        // We can do this efficiently using Gosper's hack to iterate combinations.
        // https://rosettacode.org/wiki/Gosper%27s_hack
        // Each bit set in our combination represents a flawless IV.

        // Usually, only k flawless IVs will be present in the result entity.
        // Only a small subset of combinations will be valid based on the flawless IVs we can observe from the result entity.
        // This will allow us to skip checking the majority of combinations.
        var permit = GetBitmaskFlawlessIVs(pk);
        if (System.Numerics.BitOperations.PopCount((uint)permit) == k)
            return IsMatchIVsAndFollowing(pk, in enc, rand, permit); // only one possible combination

        const int limit = 1 << 6;
        var comb = (1 << k) - 1; // initial combination: k low bits set

        while (true)
        {
            // If the combination sets a flawless IV that is not flawless in the entity, skip it.
            // comb is a valid number with k bits set
            if ((comb & permit) == comb)
            {
                if (IsMatchIVsAndFollowing(pk, in enc, rand, comb))
                    return true;
            }

            // Gosper's hack to get the next combination with same popcount
            int c = comb;
            int u = c & -c;
            int v = c + u;
            if (v is >= limit or 0)
                break; // no further n-bit combinations
            comb = v + (((v ^ c) / u) >> 2);
        }

        return false;
    }

    private static int GetBitmaskFlawlessIVs(PKM pk)
    {
        int result = 0;
        if (pk.IV_HP == MAX)
            result |= 1 << 0;
        if (pk.IV_ATK == MAX)
            result |= 1 << 1;
        if (pk.IV_DEF == MAX)
            result |= 1 << 2;
        if (pk.IV_SPA == MAX)
            result |= 1 << 3;
        if (pk.IV_SPD == MAX)
            result |= 1 << 4;
        if (pk.IV_SPE == MAX)
            result |= 1 << 5;
        return result;
    }

    private static bool IsMatchIVsAndFollowing(PKM pk, in GenerateParam9a enc, Xoroshiro128Plus rand, int flawlessBits)
    {
        Span<int> ivs = [UNSET, UNSET, UNSET, UNSET, UNSET, UNSET];
        for (int i = 0; i < 6; i++)
        {
            if ((flawlessBits & (1 << i)) != 0)
                ivs[i] = MAX;
        }

        return IsMatchIVsAndFollowing(pk, in enc, rand, ivs);
    }

    public static byte GetGender(in byte ratio, in ulong rand100) => ratio switch
    {
        EntityGender.VM => rand100 < 12 ? (byte)1 : (byte)0, // 12.5%
        EntityGender.MM => rand100 < 25 ? (byte)1 : (byte)0, // 25%
        EntityGender.HH => rand100 < 50 ? (byte)1 : (byte)0, // 50%
        EntityGender.MF => rand100 < 75 ? (byte)1 : (byte)0, // 75%
        EntityGender.VF => rand100 < 89 ? (byte)1 : (byte)0, // 87.5%

        _ => throw new ArgumentOutOfRangeException(nameof(ratio), ratio, null),
    };
}
