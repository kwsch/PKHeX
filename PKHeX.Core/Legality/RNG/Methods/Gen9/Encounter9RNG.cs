using System;
using static PKHeX.Core.ShinyUtil;

namespace PKHeX.Core;

/// <summary>
/// Logic for Generation 9 Encounter RNG.
/// </summary>
public static class Encounter9RNG
{
    /// <summary>
    /// Sets the <see cref="pk"/> with random data based on the <see cref="enc"/> and <see cref="criteria"/>.
    /// </summary>
    /// <returns>True if the generated data matches the <see cref="criteria"/>.</returns>
    public static bool TryApply32<TEnc>(this TEnc enc, PK9 pk, in ulong init, in GenerateParam9 param, EncounterCriteria criteria)
        where  TEnc : IEncounterTemplate, ITeraRaid9
    {
        const int maxCtr = 100_000;
        var rand = new Xoroshiro128Plus(init);
        for (int ctr = 0; ctr < maxCtr; ctr++)
        {
            uint seed = (uint)rand.NextInt(uint.MaxValue);
            if (!enc.CanBeEncountered(seed))
                continue;
            if (!GenerateData(pk, param, criteria, seed, param.IVs.IsSpecified))
                continue;

            var type = Tera9RNG.GetTeraType(seed, enc.TeraType, enc.Species, enc.Form);
            pk.TeraTypeOriginal = (MoveType)type;
            if (criteria.IsSpecifiedTeraType() && type != criteria.TeraType && TeraTypeUtil.CanChangeTeraType(enc.Species))
                pk.SetTeraType((MoveType)criteria.TeraType); // sets the override type
            return true; // done.
        }
        return false;
    }

    public static bool TryApply64<TEnc>(this TEnc enc, PK9 pk, in ulong init, in GenerateParam9 param, EncounterCriteria criteria, bool ignoreIVs)
        where TEnc : ISpeciesForm, IGemType
    {
        var rand = new Xoroshiro128Plus(init);
        const int maxCtr = 100_000;
        for (int ctr = 0; ctr < maxCtr; ctr++)
        {
            ulong seed = rand.Next(); // fake cryptosecure
            if (!GenerateData(pk, param, criteria, seed, ignoreIVs))
                continue;

            var type = Tera9RNG.GetTeraType(seed, enc.TeraType, enc.Species, enc.Form);
            pk.TeraTypeOriginal = (MoveType)type;
            if (criteria.IsSpecifiedTeraType() && type != criteria.TeraType && TeraTypeUtil.CanChangeTeraType(enc.Species))
                pk.SetTeraType((MoveType)criteria.TeraType); // sets the override type
            return true; // done.
        }
        return false;
    }

    /// <summary>
    /// Fills out an entity with details from the provided encounter template.
    /// </summary>
    /// <returns>False if the seed cannot generate data matching the criteria.</returns>
    public static bool GenerateData(PK9 pk, in GenerateParam9 enc, EncounterCriteria criteria, in ulong seed, bool ignoreIVs = false)
    {
        var rand = new Xoroshiro128Plus(seed);
        pk.EncryptionConstant = (uint)rand.NextInt(uint.MaxValue);
        pk.PID = GetAdaptedPID(ref rand, pk, enc);

        const int UNSET = -1;
        const int MAX = 31;
        Span<int> ivs = [UNSET, UNSET, UNSET, UNSET, UNSET, UNSET];
        if (enc.IVs.IsSpecified)
        {
            enc.IVs.CopyToSpeedLast(ivs);
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

        if (!ignoreIVs && !criteria.IsIVsCompatibleSpeedLast(ivs, 9))
            return false;

        pk.IV_HP = ivs[0];
        pk.IV_ATK = ivs[1];
        pk.IV_DEF = ivs[2];
        pk.IV_SPA = ivs[3];
        pk.IV_SPD = ivs[4];
        pk.IV_SPE = ivs[5];

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
        if (!criteria.IsGenderSatisfied(gender))
            return false;
        pk.Gender = gender;

        var nature = enc.Nature != Nature.Random ? enc.Nature : enc.Species == (int)Species.Toxtricity
                ? ToxtricityUtil.GetRandomNature(ref rand, pk.Form)
                : (Nature)rand.NextInt(25);
        if (criteria.Nature != Nature.Random && nature != criteria.Nature)
            return false;
        pk.Nature = pk.StatNature = nature;

        pk.HeightScalar = enc.Height != 0 ? enc.Height : (byte)(rand.NextInt(0x81) + rand.NextInt(0x80));
        pk.WeightScalar = enc.Weight != 0 ? enc.Weight : (byte)(rand.NextInt(0x81) + rand.NextInt(0x80));
        pk.Scale        = enc.ScaleType.GetSizeValue(enc.Scale, ref rand);
        return true;
    }

    public static bool IsMatch(PKM pk, in GenerateParam9 enc, in ulong seed)
    {
        // same as above method
        var rand = new Xoroshiro128Plus(seed);
        if (pk.EncryptionConstant != (uint)rand.NextInt(uint.MaxValue))
            return false;

        var pid = GetAdaptedPID(ref rand, pk, enc);
        if (pk.PID != pid)
            return false;

        const int UNSET = -1;
        Span<int> ivs = [UNSET, UNSET, UNSET, UNSET, UNSET, UNSET];
        if (enc.IVs.IsSpecified)
        {
            enc.IVs.CopyToSpeedLast(ivs);
        }
        else
        {
            const int MAX = 31;
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

        // Ability can be changed by Capsule/Patch.
        // Defer this check to later.
        // ReSharper disable once UnusedVariable
        int ability = enc.Ability switch
        {
            AbilityPermission.Any12H => (int)rand.NextInt(3) << 1,
            AbilityPermission.Any12 => (int)rand.NextInt(2) << 1,
            _ => (int)enc.Ability,
        };

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

        var nature = enc.Nature != Nature.Random ? enc.Nature : enc.Species == (int)Species.Toxtricity
                ? ToxtricityUtil.GetRandomNature(ref rand, pk.Form)
                : (Nature)rand.NextInt(25);
        if (pk.Nature != nature)
            return false;

        if (enc.Height == 0)
        {
            var value = (byte)(rand.NextInt(0x81) + rand.NextInt(0x80));
            if (!IsHeightMatchSV(pk, value))
                return false;
        }
        if (enc.Weight == 0)
        {
            var value = (byte)(rand.NextInt(0x81) + rand.NextInt(0x80));
            if (pk is IScaledSize s && s.WeightScalar != value)
                return false;
        }
        // Scale
        {
            var value = enc.ScaleType.GetSizeValue(enc.Scale, ref rand);
            if (pk is IScaledSize3 s)
            {
                if (s.Scale != value)
                    return false;
            }
            else if (pk is IScaledSize s2)
            {
                if (s2.HeightScalar != value)
                    return false;
            }
        }
        return true;
    }

    public static bool IsHeightMatchSV(PKM pk, byte value)
    {
        // HOME copies Scale to Height. Untouched by HOME must match the value.
        // Viewing the save file in HOME will alter it too. Tracker definitely indicates it was viewed.
        if (pk is not (IScaledSize s2 and IScaledSize3 s3))
            return true;

        // Viewed in HOME.
        if (s2.HeightScalar == s3.Scale)
            return true;
        if (pk is IHomeTrack { HasTracker: true })
            return false;

        return s2.HeightScalar == value;
    }

    private static uint GetAdaptedPID(ref Xoroshiro128Plus rand, PKM pk, in GenerateParam9 enc)
    {
        var fakeTID = (uint)rand.NextInt();
        uint pid = (uint)rand.NextInt();
        if (enc.Shiny == Shiny.Random) // let's decide if it's shiny or not!
        {
            int i = 1;
            bool isShiny;
            uint xor;
            while (true)
            {
                xor = GetShinyXor(pid, fakeTID);
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
            ForceShinyState(isShiny, ref pid, pk.ID32, xor);
        }
        else if (enc.Shiny == Shiny.Always)
        {
            var tid = (ushort)fakeTID;
            var sid = (ushort)(fakeTID >> 16);
            if (!GetIsShiny(fakeTID, pid)) // battled
                pid = GetShinyPID(tid, sid, pid, 0);
            if (!GetIsShiny(pk.ID32, pid)) // captured
                pid = GetShinyPID(pk.TID16, pk.SID16, pid, GetShinyXor(pid, fakeTID) == 0 ? 0u : 1u);
        }
        else // Never
        {
            if (GetIsShiny(fakeTID, pid)) // battled
                pid ^= 0x1000_0000;
            if (GetIsShiny(pk.ID32, pid)) // captured
                pid ^= 0x1000_0000;
        }
        return pid;
    }

    public static byte GetGender(in int ratio, in ulong rand100) => ratio switch
    {
        0x1F => rand100 < 12 ? (byte)1 : (byte)0, // 12.5%
        0x3F => rand100 < 25 ? (byte)1 : (byte)0, // 25%
        0x7F => rand100 < 50 ? (byte)1 : (byte)0, // 50%
        0xBF => rand100 < 75 ? (byte)1 : (byte)0, // 75%
        0xE1 => rand100 < 89 ? (byte)1 : (byte)0, // 87.5%

        _ => throw new ArgumentOutOfRangeException(nameof(ratio)),
    };
}
