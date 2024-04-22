using System;
using static PKHeX.Core.ShinyUtil;

namespace PKHeX.Core;

/// <summary>
/// Contains logic for the Generation 8b (BD/SP) roaming spawns.
/// </summary>
/// <remarks>
/// Roaming encounters use the Pokémon's 32-bit <see cref="PKM.EncryptionConstant"/> as RNG seed.
/// </remarks>
public static class Roaming8bRNG
{
    private const int UNSET = -1;

    public static void ApplyDetails(PKM pk, EncounterCriteria criteria, Shiny shiny = Shiny.FixedValue, int flawless = -1, int maxAttempts = 70_000)
    {
        if (shiny == Shiny.FixedValue)
            shiny = criteria.Shiny is Shiny.Random or Shiny.Never ? Shiny.Never : criteria.Shiny;
        if (flawless == -1)
            flawless = 0;

        // Since the inner methods do not set Gender (only fixed Genders are applicable) and Nature (assume Synchronize used), set them here.
        pk.Gender = (byte)(pk.Species == (int)Species.Cresselia ? 1 : 2); // Mesprit
        pk.Nature = pk.StatNature = criteria.GetNature();

        int ctr = 0;
        var rnd = Util.Rand;
        do
        {
            var seed = rnd.Rand32();
            if (seed == int.MaxValue)
                continue; // Unity's Rand is [int.MinValue, int.MaxValue)
            if (TryApplyFromSeed(pk, criteria, shiny, flawless, seed))
                return;
        } while (++ctr != maxAttempts);
        TryApplyFromSeed(pk, EncounterCriteria.Unrestricted, shiny, flawless, rnd.Rand32());
    }

    private static bool TryApplyFromSeed(PKM pk, EncounterCriteria criteria, Shiny shiny, int flawless, uint seed)
    {
        var xoro = new Xoroshiro128Plus8b(seed);

        // Encryption Constant
        pk.EncryptionConstant = seed;

        // PID
        var fakeTID = xoro.NextUInt(); // fakeTID
        var pid = xoro.NextUInt();
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
            var idx = (int)xoro.NextUInt(6);
            if (ivs[idx] != UNSET)
                continue;
            ivs[idx] = 31;
            determined++;
        }

        for (var i = 0; i < ivs.Length; i++)
        {
            if (ivs[i] == UNSET)
                ivs[i] = (int)xoro.NextUInt(MAX + 1);
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
        pk.RefreshAbility((int)xoro.NextUInt(2));

        // Remainder
        var scale = (IScaledSize)pk;
        scale.HeightScalar = (byte)(xoro.NextUInt(0x81) + xoro.NextUInt(0x80));
        scale.WeightScalar = (byte)(xoro.NextUInt(0x81) + xoro.NextUInt(0x80));

        return true;
    }

    public static bool ValidateRoamingEncounter(PKM pk, Shiny shiny = Shiny.Random, int flawless = 0)
    {
        var seed = pk.EncryptionConstant;
        if (seed == int.MaxValue)
            return false; // Unity's Rand is [int.MinValue, int.MaxValue)
        var xoro = new Xoroshiro128Plus8b(seed);

        // Check PID
        var fakeTID = xoro.NextUInt(); // fakeTID
        var pid = xoro.NextUInt();
        pid = GetRevisedPID(fakeTID, pid, pk);
        if (pk.PID != pid)
            return false;

        // Check IVs: Create flawless IVs at random indexes, then the random IVs for not flawless.
        Span<int> ivs = [UNSET, UNSET, UNSET, UNSET, UNSET, UNSET];

        var determined = 0;
        while (determined < flawless)
        {
            var idx = (int)xoro.NextUInt(6);
            if (ivs[idx] != UNSET)
                continue;
            ivs[idx] = 31;
            determined++;
        }

        for (var i = 0; i < ivs.Length; i++)
        {
            if (ivs[i] == UNSET)
                ivs[i] = (int)xoro.NextUInt(31 + 1);
        }

        if (ivs[0] != pk.IV_HP ) return false;
        if (ivs[1] != pk.IV_ATK) return false;
        if (ivs[2] != pk.IV_DEF) return false;
        if (ivs[3] != pk.IV_SPA) return false;
        if (ivs[4] != pk.IV_SPD) return false;
        if (ivs[5] != pk.IV_SPE) return false;

        // Don't check Hidden ability, as roaming encounters are 1/2 only.
        if (pk.AbilityNumber != (1 << (int)xoro.NextUInt(2)))
            return false;

        return GetIsMatchEnd(pk, xoro) || GetIsMatchEndWithCuteCharm(pk, xoro) || GetIsMatchEndWithSynchronize(pk, xoro);
    }

    private static bool GetIsMatchEnd(PKM pk, Xoroshiro128Plus8b xoro)
    {
        // Check that gender matches
        var genderRatio = PersonalTable.BDSP.GetFormEntry(pk.Species, pk.Form).Gender;
        if (genderRatio == PersonalInfo.RatioMagicGenderless)
        {
            if (pk.Gender != (int)Gender.Genderless)
                return false;
        }
        else if (genderRatio == PersonalInfo.RatioMagicMale)
        {
            if (pk.Gender != (int)Gender.Male)
                return false;
        }
        else if (genderRatio == PersonalInfo.RatioMagicFemale)
        {
            if (pk.Gender != (int)Gender.Female)
                return false;
        }
        else
        {
            if (pk.Gender != (((int)xoro.NextUInt(253) + 1 < genderRatio) ? 1 : 0))
                return false;
        }

        // Check that the nature matches
        if (pk.Nature != (Nature)xoro.NextUInt(25))
            return false;

        return GetIsHeightWeightMatch(pk, xoro);
    }

    private static bool GetIsMatchEndWithCuteCharm(PKM pk, Xoroshiro128Plus8b xoro)
    {
        // Check that gender matches
        // Assume that the gender is a match due to cute charm.

        // Check that the nature matches
        if (pk.Nature != (Nature)xoro.NextUInt(25))
            return false;

        return GetIsHeightWeightMatch(pk, xoro);
    }

    private static bool GetIsMatchEndWithSynchronize(PKM pk, Xoroshiro128Plus8b xoro)
    {
        // Check that gender matches
        var genderRatio = PersonalTable.BDSP.GetFormEntry(pk.Species, pk.Form).Gender;
        if (genderRatio == PersonalInfo.RatioMagicGenderless)
        {
            if (pk.Gender != (int)Gender.Genderless)
                return false;
        }
        else if (genderRatio == PersonalInfo.RatioMagicMale)
        {
            if (pk.Gender != (int)Gender.Male)
                return false;
        }
        else if (genderRatio == PersonalInfo.RatioMagicFemale)
        {
            if (pk.Gender != (int)Gender.Female)
                return false;
        }
        else
        {
            if (pk.Gender != (((int)xoro.NextUInt(253) + 1 < genderRatio) ? 1 : 0))
                return false;
        }

        // Assume that the nature is a match due to synchronize.

        return GetIsHeightWeightMatch(pk, xoro);
    }

    private static bool GetIsHeightWeightMatch(PKM pk, Xoroshiro128Plus8b xoro)
    {
        // Check height/weight
        if (pk is not IScaledSize s)
            return false;

        var height = xoro.NextUInt(0x81) + xoro.NextUInt(0x80);
        var weight = xoro.NextUInt(0x81) + xoro.NextUInt(0x80);
        return s.HeightScalar == height && s.WeightScalar == weight;
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
        if (!isShiny)
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
}
