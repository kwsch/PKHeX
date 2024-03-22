using System;

namespace PKHeX.Core;

/// <summary>
/// Generator methods for Generation 4.
/// </summary>
public static class GenerateMethodJ
{
    /// <summary>
    /// Applies a random PID and IVs to the entity, based on the <see cref="PersonalInfo4"/> and <see cref="EncounterCriteria"/>.
    /// </summary>
    /// <param name="enc">Encounter slot to generate for</param>
    /// <param name="pk">Entity to modify</param>
    /// <param name="pi">Personal Info of the entity</param>
    /// <param name="criteria">Criteria to match</param>
    /// <param name="seed">Initial seed to use; will be modified during the loop if the first seed fails</param>
    /// <returns>Method 1 origin seed applied</returns>
    public static uint SetRandomJ<T>(this T enc, PK4 pk, PersonalInfo4 pi, EncounterCriteria criteria, uint seed)
        where T : IEncounterSlot4
    {
        var gr = pi.Gender;
        var ability = criteria.GetAbilityFromNumber(AbilityPermission.Any12);
        var (min, max) = SlotMethodJ.GetRange(enc.Type, enc.SlotNumber);
        bool randLevel = MethodJ.IsLevelRand(enc);
        bool checkProc = MethodJ.IsEncounterCheckApplicable(enc.Type);

        // Generate Method J correlated PID and IVs, no lead (keep things simple).
        while (true)
        {
            if (checkProc)
            {
                var check = new LeadSeed(seed, LeadRequired.None);
                if (!MethodJ.CheckEncounterActivation(enc, ref check))
                {
                    seed = LCRNG.Next(seed);
                    continue;
                }
            }
            var esv = LCRNG.Next16(ref seed) / 656;
            if (esv < min || esv > max)
                continue;
            var lv = randLevel ? LCRNG.Next16(ref seed) : 0;
            var nature = LCRNG.Next16(ref seed) % 25;
            if (criteria.IsSpecifiedNature() && nature != (byte)criteria.Nature)
                continue;

            while (true)
            {
                var a = LCRNG.Next16(ref seed);
                var b = LCRNG.Next16(ref seed);
                var pid = GetPIDRegular(a, b);
                if (pid % 25 != nature)
                    continue;
                if ((pid & 1) != ability)
                    break; // try again
                var gender = EntityGender.GetFromPIDAndRatio(pid, gr);
                if (!criteria.IsGenderSatisfied(gender))
                    break; // try again

                if (randLevel)
                {
                    var lvl = enc.Type is SlotType4.HoneyTree
                        ? MethodJ.GetHoneyTreeLevel(lv)
                        : ((lv % (enc.LevelMax - enc.LevelMin + 1)) + enc.LevelMin);
                    pk.MetLevel = pk.CurrentLevel = (byte)lvl;
                }
                pk.PID = pid;
                var iv1 = LCRNG.Next16(ref seed);
                var iv2 = LCRNG.Next16(ref seed);
                pk.IV32 = ((iv2 & 0x7FFF) << 15) | (iv1 & 0x7FFF);
                pk.Gender = gender;
                pk.Ability = (pid & 1) == 0 ? pi.Ability1 : pi.Ability2;
                return LCRNG.Prev4(seed);
            }
        }
    }

    /// <summary>
    /// Applies the IVs from the <see cref="EncounterCriteria"/>, assuming the IVs are possible and a Method 1 spread is possible.
    /// </summary>
    /// <param name="enc">Encounter slot to generate for</param>
    /// <param name="pk">Entity to modify</param>
    /// <param name="pi">Personal Info of the entity</param>
    /// <param name="criteria">Criteria to match</param>
    /// <param name="origin">Method 1 origin seed applied</param>
    /// <returns>True if the PID/IV was valid &amp; applied to the entity.</returns>
    public static bool SetFromIVsJ<T>(this T enc, PK4 pk, PersonalInfo4 pi, EncounterCriteria criteria, out uint origin)
        where T : IEncounterSlot4
    {
        var gr = pi.Gender;
        (uint iv1, uint iv2) = GetCombinedIVs(criteria);
        Span<uint> all = stackalloc uint[LCRNG.MaxCountSeedsIV];
        var count = LCRNGReversal.GetSeedsIVs(all, iv1 << 16, iv2 << 16);
        var seeds = all[..count];
        foreach (ref var seed in seeds)
        {
            seed = LCRNG.Prev2(seed);
            var s = seed;

            var a = LCRNG.Next16(ref s);
            var b = LCRNG.Next16(ref s);
            var pid = GetPIDRegular(a, b);
            if (criteria.IsSpecifiedNature() && (Nature)(pid % 25) != criteria.Nature)
                continue;

            var gender = EntityGender.GetFromPIDAndRatio(pid, gr);
            if (!criteria.IsGenderSatisfied(gender))
                continue;
            var lead = MethodJ.GetSeed(enc, seed, enc, 4);
            if (!lead.IsValid()) // Verifies the slot, (min) level, and nature loop; if it passes, apply the details.
                continue;

            pk.PID = pid;
            pk.IV32 = ((iv2 & 0x7FFF) << 15) | (iv1 & 0x7FFF);
            pk.Gender = gender;
            pk.Ability = (pid & 1) == 0 ? pi.Ability1 : pi.Ability2;
            origin = seed;
            return true;
        }

        origin = 0;
        return false;
    }

    public static uint GetPIDRegular(uint a, uint b) => b << 16 | a;

    private static (uint iv1, uint iv2) GetCombinedIVs(EncounterCriteria criteria)
    {
        uint iv1 = (uint)criteria.IV_HP | (uint)criteria.IV_ATK << 5 | (uint)criteria.IV_DEF << 10;
        uint iv2 = (uint)criteria.IV_SPE | (uint)criteria.IV_SPA << 5 | (uint)criteria.IV_SPD << 10;
        return (iv1, iv2);
    }
}
