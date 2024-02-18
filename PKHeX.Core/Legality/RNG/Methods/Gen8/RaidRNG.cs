using System;
using static PKHeX.Core.ShinyUtil;

namespace PKHeX.Core;

/// <summary>
/// Logic for Generating and Verifying Gen8 Raid Templates against PKM data.
/// </summary>
public static class RaidRNG
{
    /// <summary>
    /// Verify a Raid Seed against a PKM.
    /// </summary>
    /// <param name="pk">Entity to verify against</param>
    /// <param name="seed">Seed that generated the entity</param>
    /// <param name="ivs">Buffer of IVs (potentially with already specified values)</param>
    /// <param name="param">Parameters to generate with</param>
    /// <param name="forceNoShiny">Force the entity to be non-shiny via special handling</param>
    /// <returns>True if the seed matches the entity</returns>
    public static bool Verify(PKM pk, ulong seed, Span<int> ivs, in GenerateParam8 param, bool forceNoShiny = false)
    {
        var rng = new Xoroshiro128Plus(seed);
        var ec = (uint)rng.NextInt();
        if (ec != pk.EncryptionConstant)
            return false;

        uint pid;
        bool isShiny;
        {
            var trID = (uint)rng.NextInt();
            pid = (uint)rng.NextInt();
            var xor = GetShinyXor(pid, trID);
            isShiny = xor < 16;
            if (isShiny && forceNoShiny)
            {
                ForceShinyState(false, ref pid, trID, 0);
                isShiny = false;
            }
        }

        ForceShinyState(isShiny, ref pid, pk.ID32, 0);

        if (pk.PID != pid)
            return false;

        const int UNSET = -1;
        const int MAX = 31;
        if (param.IVs.IsSpecified)
            param.IVs.CopyToSpeedLast(ivs);
        else
            ivs.Fill(UNSET);

        for (int i = ivs.Count(MAX); i < param.FlawlessIVs; i++)
        {
            int index = (int)rng.NextInt(6);
            while (ivs[index] != UNSET)
                index = (int)rng.NextInt(6);
            ivs[index] = MAX;
        }

        for (int i = 0; i < 6; i++)
        {
            if (ivs[i] == UNSET)
                ivs[i] = (int)rng.NextInt(32);
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

        int ability = param.Ability switch
        {
            AbilityPermission.Any12H => (int)rng.NextInt(3),
            AbilityPermission.Any12 => (int)rng.NextInt(2),
            _ => param.Ability.GetSingleValue(),
        };
        ability <<= 1; // 1/2/4

        var current = pk.AbilityNumber;
        if (ability == 4)
        {
            if (current != 4 && pk is PK8)
                return false;
        }
        // else, for things that were made Hidden Ability, defer to Ability Checks (Ability Patch)

        switch (param.GenderRatio)
        {
            case PersonalInfo.RatioMagicGenderless:
                if (pk.Gender != 2)
                    return false;
                break;
            case PersonalInfo.RatioMagicFemale:
                if (pk.Gender != 1)
                    return false;
                break;
            case PersonalInfo.RatioMagicMale:
                if (pk.Gender != 0)
                    return false;
                break;
            default:
                var gender = (int)rng.NextInt(253) + 1 < param.GenderRatio ? 1 : 0;
                if (pk.Gender != gender && pk.Gender != 2) // allow Nincada(0/1)->Shedinja(2)
                    return false;
                break;
        }

        var nature = param.Nature != Nature.Random ? param.Nature
            : param.Species == (int)Species.Toxtricity
                ? ToxtricityUtil.GetRandomNature(ref rng, pk.Form)
                : (Nature)rng.NextInt(25);
        if (pk.Nature != nature)
            return false;

        if (pk is IScaledSize s)
        {
            var height = rng.NextInt(0x81) + rng.NextInt(0x80);
            var weight = rng.NextInt(0x81) + rng.NextInt(0x80);
            if (height == 0 && weight == 0 && pk is IHomeTrack { HasTracker: true})
            {
                // HOME rerolls height/weight if both are 0
                // This behavior started in 3.0.0, so only flag if the context is 9 or above.
                if (pk.Context is not (EntityContext.Gen8 or EntityContext.Gen8a or EntityContext.Gen8b))
                    return false;
            }
            else
            {
                if (s.HeightScalar != height)
                    return false;
                if (s.WeightScalar != weight)
                    return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Apply the details to the PKM
    /// </summary>
    /// <param name="pk">Entity to verify against</param>
    /// <param name="seed">Seed that generated the entity</param>
    /// <param name="ivs">Buffer of IVs (potentially with already specified values)</param>
    /// <param name="param">Parameters to generate with</param>
    /// <param name="criteria">Criteria to generate with</param>
    /// <returns>True if the seed matches the entity</returns>
    public static bool TryApply(PK8 pk, ulong seed, Span<int> ivs, in GenerateParam8 param, EncounterCriteria criteria)
    {
        var rng = new Xoroshiro128Plus(seed);
        pk.EncryptionConstant = (uint)rng.NextInt();

        uint pid;
        bool isShiny;
        {
            var trID = (uint)rng.NextInt();
            pid = (uint)rng.NextInt();
            var xor = GetShinyXor(pid, trID);
            isShiny = xor < 16;
            if (isShiny && param.Shiny == Shiny.Never)
            {
                ForceShinyState(false, ref pid, trID, 0);
                isShiny = false;
            }
        }

        if (isShiny)
        {
            if (!GetIsShiny(pk.ID32, pid))
                pid = GetShinyPID(pk.TID16, pk.SID16, pid, 0);
        }
        else
        {
            if (GetIsShiny(pk.ID32, pid))
                pid ^= 0x1000_0000;
        }

        pk.PID = pid;

        const int UNSET = -1;
        const int MAX = 31;
        if (param.IVs.IsSpecified)
        {
            param.IVs.CopyToSpeedLast(ivs);
        }
        else
        {
            ivs.Fill(UNSET);
            for (int i = ivs.Count(MAX); i < param.FlawlessIVs; i++)
            {
                int index;
                do { index = (int)rng.NextInt(6); }
                while (ivs[index] != UNSET);
                ivs[index] = MAX;
            }
        }

        for (int i = 0; i < 6; i++)
        {
            if (ivs[i] == UNSET)
                ivs[i] = (int)rng.NextInt(MAX + 1);
        }

        if (!param.IVs.IsSpecified && !criteria.IsIVsCompatibleSpeedLast(ivs, 8))
            return false;

        pk.IV_HP = ivs[0];
        pk.IV_ATK = ivs[1];
        pk.IV_DEF = ivs[2];
        pk.IV_SPA = ivs[3];
        pk.IV_SPD = ivs[4];
        pk.IV_SPE = ivs[5];

        int ability = param.Ability switch
        {
            AbilityPermission.Any12H => (int)rng.NextInt(3),
            AbilityPermission.Any12 => (int)rng.NextInt(2),
            _ => param.Ability.GetSingleValue(),
        };
        pk.RefreshAbility(ability);

        byte gender = param.GenderRatio switch
        {
            PersonalInfo.RatioMagicGenderless => 2,
            PersonalInfo.RatioMagicFemale => 1,
            PersonalInfo.RatioMagicMale => 0,
            _ => rng.NextInt(253) + 1 < param.GenderRatio ? (byte)1 : (byte)0,
        };
        if (!criteria.IsGenderSatisfied(gender))
            return false;
        pk.Gender = gender;

        var nature = param.Nature != Nature.Random ? param.Nature
            : param.Species == (int)Species.Toxtricity
                ? ToxtricityUtil.GetRandomNature(ref rng, pk.Form)
                : (Nature)rng.NextInt(25);

        pk.Nature = pk.StatNature = nature;

        var height = rng.NextInt(0x81) + rng.NextInt(0x80);
        var weight = rng.NextInt(0x81) + rng.NextInt(0x80);
        pk.HeightScalar = (byte)height;
        pk.WeightScalar = (byte)weight;

        return true;
    }
}
