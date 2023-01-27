using System;
using System.Runtime.CompilerServices;

namespace PKHeX.Core;

/// <summary>
/// Logic for Generating and Verifying Gen8 Raid Templates against PKM data.
/// </summary>
public static class RaidRNG
{
    public static bool Verify<T>(this T raid, PK8 pk8, ulong seed) where T: EncounterStatic8Nest<T>
    {
        var pi = PersonalTable.SWSH.GetFormEntry(raid.Species, raid.Form);
        var ratio = pi.Gender;
        var abil = RemapAbilityToParam(raid.Ability);

        Span<int> IVs = stackalloc int[6];
        LoadIVs(raid, IVs);
        return Verify(pk8, seed, IVs, raid.Species, raid.FlawlessIVCount, abil, ratio);
    }

    public static void ApplyDetailsTo<T>(this T raid, PK8 pk8, ulong seed) where T : EncounterStatic8Nest<T>
    {
        // Ensure the species-form is set correctly (nature)
        pk8.Species = raid.Species;
        pk8.Form = raid.Form;
        var pi = PersonalTable.SWSH.GetFormEntry(raid.Species, raid.Form);
        var ratio = pi.Gender;
        var abil = RemapAbilityToParam(raid.Ability);

        Span<int> IVs = stackalloc int[6];
        LoadIVs(raid, IVs);
        ApplyDetailsTo(pk8, seed, IVs, raid.Species, raid.FlawlessIVCount, abil, ratio);
    }

    private static void LoadIVs<T>(T raid, Span<int> span) where T : EncounterStatic8Nest<T>
    {
        // Template stores with speed in middle (standard), convert for generator purpose.
        var ivs = raid.IVs;
        if (ivs.IsSpecified)
            ivs.CopyToSpeedLast(span);
        else
            span.Fill(-1);
    }

    private static byte RemapAbilityToParam(AbilityPermission a) => a switch
    {
        AbilityPermission.Any12H => 254,
        AbilityPermission.Any12  => 255,
        _ => a.GetSingleValue(),
    };

    private static bool Verify(PKM pk, ulong seed, Span<int> ivs, ushort species, byte iv_count, byte ability_param, byte gender_ratio, sbyte nature_param = -1, Shiny shiny = Shiny.Random)
    {
        var rng = new Xoroshiro128Plus(seed);
        var ec = (uint)rng.NextInt();
        if (ec != pk.EncryptionConstant)
            return false;

        uint pid;
        bool isShiny;
        if (shiny == Shiny.Random) // let's decide if it's shiny or not!
        {
            var trID = (uint)rng.NextInt();
            pid = (uint)rng.NextInt();
            isShiny = GetShinyXor(pid, trID) < 16;
        }
        else
        {
            // no need to calculate a fake trainer
            pid = (uint)rng.NextInt();
            isShiny = shiny == Shiny.Always;
        }

        ForceShinyState(pk, isShiny, ref pid);

        if (pk.PID != pid)
            return false;

        const int UNSET = -1;
        const int MAX = 31;
        for (int i = ivs.Count(MAX); i < iv_count; i++)
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

        int abil = ability_param switch
        {
            254 => (int)rng.NextInt(3),
            255 => (int)rng.NextInt(2),
            _ => ability_param,
        };
        abil <<= 1; // 1/2/4

        var current = pk.AbilityNumber;
        if (abil == 4)
        {
            if (current != 4)
                return false;
        }
        // else, for things that were made Hidden Ability, defer to Ability Checks (Ability Patch)

        switch (gender_ratio)
        {
            case PersonalInfo.RatioMagicGenderless when pk.Gender != 2:
                if (pk.Gender != 2)
                    return false;
                break;
            case PersonalInfo.RatioMagicFemale when pk.Gender != 1:
                if (pk.Gender != 1)
                    return false;
                break;
            case PersonalInfo.RatioMagicMale:
                if (pk.Gender != 0)
                    return false;
                break;
            default:
                var gender = (int)rng.NextInt(252) + 1 < gender_ratio ? 1 : 0;
                if (pk.Gender != gender)
                    return false;
                break;
        }

        int nature = nature_param != -1 ? nature_param
            : species == (int)Species.Toxtricity
                ? ToxtricityUtil.GetRandomNature(ref rng, pk.Form)
                : (byte)rng.NextInt(25);
        if (pk.Nature != nature)
            return false;

        if (pk is IScaledSize s)
        {
            var height = (int)rng.NextInt(0x81) + (int)rng.NextInt(0x80);
            if (s.HeightScalar != height)
                return false;
            var weight = (int)rng.NextInt(0x81) + (int)rng.NextInt(0x80);
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
            if (!GetIsShiny(pk.ID32, pid))
                pid = GetShinyPID(pk.TID16, pk.SID16, pid, 0);
        }
        else
        {
            if (GetIsShiny(pk.ID32, pid))
                pid ^= 0x1000_0000;
        }
    }

    private static bool ApplyDetailsTo(PKM pk, ulong seed, Span<int> ivs, ushort species, byte iv_count, byte ability_param, byte gender_ratio, sbyte nature_param = -1, Shiny shiny = Shiny.Random)
    {
        var rng = new Xoroshiro128Plus(seed);
        pk.EncryptionConstant = (uint)rng.NextInt();

        uint pid;
        bool isShiny;
        if (shiny == Shiny.Random) // let's decide if it's shiny or not!
        {
            var trID = (uint)rng.NextInt();
            pid = (uint)rng.NextInt();
            isShiny = GetShinyXor(pid, trID) < 16;
        }
        else
        {
            // no need to calculate a fake trainer
            pid = (uint)rng.NextInt();
            isShiny = shiny == Shiny.Always;
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
        for (int i = ivs.Count(MAX); i < iv_count; i++)
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

        pk.IV_HP = ivs[0];
        pk.IV_ATK = ivs[1];
        pk.IV_DEF = ivs[2];
        pk.IV_SPA = ivs[3];
        pk.IV_SPD = ivs[4];
        pk.IV_SPE = ivs[5];

        int abil = ability_param switch
        {
            254 => (int)rng.NextInt(3),
            255 => (int)rng.NextInt(2),
            _ => ability_param,
        };
        pk.RefreshAbility(abil);

        pk.Gender = gender_ratio switch
        {
            PersonalInfo.RatioMagicGenderless => 2,
            PersonalInfo.RatioMagicFemale => 1,
            PersonalInfo.RatioMagicMale => 0,
            _ => (int) rng.NextInt(252) + 1 < gender_ratio ? 1 : 0,
        };

        int nature = nature_param != -1 ? nature_param
            : species == (int)Species.Toxtricity
                ? ToxtricityUtil.GetRandomNature(ref rng, pk.Form)
                : (byte)rng.NextInt(25);

        pk.StatNature = pk.Nature = nature;

        if (pk is IScaledSize s)
        {
            var height = (int)rng.NextInt(0x81) + (int)rng.NextInt(0x80);
            var weight = (int)rng.NextInt(0x81) + (int)rng.NextInt(0x80);
            s.HeightScalar = (byte)height;
            s.WeightScalar = (byte)weight;
        }

        return true;
    }

    private static uint GetShinyPID(ushort tid, ushort sid, uint pid, uint type)
    {
        return (type ^ tid ^ sid ^ (pid & 0xFFFF)) << 16 | (pid & 0xFFFF);
    }

    private static bool GetIsShiny(uint id32, uint pid) => GetShinyXor(pid, id32) < 16;

    private static uint GetShinyXor(uint pid, uint id32)
    {
        var xor = pid ^ id32;
        return (xor ^ (xor >> 16)) & 0xFFFF;
    }
}
