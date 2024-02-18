using System;

namespace PKHeX.Core;

/// <summary>
/// Contains logic for the Generation 8b (BD/SP) wild and stationary spawns.
/// </summary>
public static class Wild8bRNG
{
    private const int UNSET = -1;

    public static void ApplyDetails(PKM pk, EncounterCriteria criteria,
        Shiny shiny = Shiny.FixedValue,
        int flawless = -1,
        AbilityPermission ability = AbilityPermission.Any12,
        int maxAttempts = 70_000)
    {
        if (shiny == Shiny.FixedValue)
            shiny = criteria.Shiny is Shiny.Random or Shiny.Never ? Shiny.Never : criteria.Shiny;
        if (flawless == -1)
            flawless = 0;

        int ctr = 0;
        var rnd = Util.Rand;
        do
        {
            ulong s0 = rnd.Rand64();
            ulong s1 = rnd.Rand64();
            var xors = new XorShift128(s0, s1);
            if (TryApplyFromSeed(pk, criteria, shiny, flawless, xors, ability))
                return;
        } while (++ctr != maxAttempts);

        {
            ulong s0 = rnd.Rand64();
            ulong s1 = rnd.Rand64();
            var xors = new XorShift128(s0, s1);
            TryApplyFromSeed(pk, EncounterCriteria.Unrestricted, shiny, flawless, xors, ability);
        }
    }

    public static bool TryApplyFromSeed(PKM pk, EncounterCriteria criteria, Shiny shiny, int flawless, XorShift128 xors, AbilityPermission ability)
    {
        // Encryption Constant
        pk.EncryptionConstant = xors.NextUInt();

        // PID
        var fakeTID = xors.NextUInt(); // fakeTID
        var pid = xors.NextUInt();
        pid = GetRevisedPID(fakeTID, pid, pk);
        var xor = GetShinyXor(pk.ID32, pid);
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

        // Check IVs: Create flawless IVs at random indexes, then the random IVs for not flawless.
        Span<int> ivs = [UNSET, UNSET, UNSET, UNSET, UNSET, UNSET];
        const int MAX = 31;
        var determined = 0;
        while (determined < flawless)
        {
            var idx = (int)xors.NextUInt(6);
            if (ivs[idx] != UNSET)
                continue;
            ivs[idx] = 31;
            determined++;
        }

        for (var i = 0; i < ivs.Length; i++)
        {
            if (ivs[i] == UNSET)
                ivs[i] = xors.NextInt(0, MAX + 1);
        }

        if (!criteria.IsIVsCompatibleSpeedLast(ivs, 8))
            return false;

        pk.IV_HP = ivs[0];
        pk.IV_ATK = ivs[1];
        pk.IV_DEF = ivs[2];
        pk.IV_SPA = ivs[3];
        pk.IV_SPD = ivs[4];
        pk.IV_SPE = ivs[5];

        // Ability
        var n = ability switch
        {
            AbilityPermission.Any12  => (int)xors.NextUInt(2),
            AbilityPermission.Any12H => (int)xors.NextUInt(3),
            _ => (int)ability >> 1,
        };
        pk.RefreshAbility(n);

        // Gender (skip this if gender is fixed)
        var genderRatio = PersonalTable.BDSP.GetFormEntry(pk.Species, pk.Form).Gender;
        if (genderRatio == PersonalInfo.RatioMagicGenderless)
        {
            pk.Gender = 2;
        }
        else if (genderRatio == PersonalInfo.RatioMagicMale)
        {
            pk.Gender = 0;
        }
        else if (genderRatio == PersonalInfo.RatioMagicFemale)
        {
            pk.Gender = 1;
        }
        else
        {
            byte gender = xors.NextUInt(253) + 1 < genderRatio ? (byte)1 : (byte)0;
            if (!criteria.IsGenderSatisfied(gender))
                return false;
            pk.Gender = gender;
        }

        if (!criteria.IsSpecifiedNature())
            pk.Nature = (Nature)xors.NextUInt(25);
        else // Skip nature, assuming Synchronize
            pk.Nature = criteria.Nature;
        pk.StatNature = pk.Nature;

        // Remainder
        var scale = (IScaledSize)pk;
        scale.HeightScalar = (byte)(xors.NextUInt(0x81) + xors.NextUInt(0x80));
        scale.WeightScalar = (byte)(xors.NextUInt(0x81) + xors.NextUInt(0x80));

        // Item, don't care
        return true;
    }

    private static uint GetRevisedPID(uint fakeTID, uint pid, ITrainerID32 tr)
    {
        var xor = GetShinyXor(pid, fakeTID);
        var newXor = GetShinyXor(pid, tr.ID32);

        var fakeRare = GetRareType(xor);
        var newRare = GetRareType(newXor);

        if (fakeRare == newRare)
            return pid;

        var isShiny = xor < 16;
        if (isShiny)
            return pid ^ 0x1000_0000;
        var low = pid & 0xFFFF;
        return (((xor == 0 ? 0u : 1u) ^ tr.TID16 ^ tr.SID16 ^ low) << 16) | low; // force same shiny star type
    }

    private static Shiny GetRareType(uint xor) => xor switch
    {
        0 => Shiny.AlwaysSquare,
        < 16 => Shiny.AlwaysStar,
        _ => Shiny.Never,
    };

    private static uint GetShinyXor(uint pid, uint id32)
    {
        var xor = pid ^ id32;
        return (xor ^ (xor >> 16)) & 0xFFFF;
    }
}
