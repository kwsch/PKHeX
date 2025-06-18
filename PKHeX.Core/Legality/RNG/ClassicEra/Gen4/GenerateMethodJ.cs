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
    public static uint SetRandomJ<T>(this T enc, PK4 pk, PersonalInfo4 pi, in EncounterCriteria criteria, uint seed)
        where T : IEncounterSlot4
    {
        var gr = pi.Gender;
        var id32 = pk.ID32;
        var (min, max) = SlotMethodJ.GetRange(enc.Type, enc.SlotNumber);
        bool randLevel = MethodJ.IsLevelRand(enc);
        bool checkProc = MethodJ.IsEncounterCheckApplicable(enc.Type);
        bool checkLevel = criteria.IsSpecifiedLevelRange() && enc.IsLevelWithinRange(criteria);
        bool filterIVs = criteria.IsSpecifiedIVs(2);

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
            var esv = LCRNG.Next16(ref seed) / 656u;
            if (esv < min || esv > max)
                continue;
            var lv = randLevel ? LCRNG.Next16(ref seed) : 0;
            var nature = MethodJ.GetNature(LCRNG.Next16(ref seed));
            if (criteria.IsSpecifiedNature() && !criteria.IsSatisfiedNature((Nature)nature))
                continue;

            while (true)
            {
                var a = LCRNG.Next16(ref seed);
                var b = LCRNG.Next16(ref seed);
                var pid = GetPIDRegular(a, b);
                if (pid % 25 != nature)
                    continue;
                if (ShinyUtil.GetIsShiny(id32, pid, 8) != criteria.Shiny.IsShiny())
                    break; // try again
                if (criteria.IsSpecifiedAbility() && !criteria.IsSatisfiedAbility((int)(pid & 1)))
                    break; // try again
                var gender = EntityGender.GetFromPIDAndRatio(pid, gr);
                if (criteria.IsSpecifiedGender() && !criteria.IsSatisfiedGender(gender))
                    break; // try again

                var iv32 = ClassicEraRNG.GetSequentialIVs(ref seed);
                if (criteria.IsSpecifiedHiddenPower() && !criteria.IsSatisfiedHiddenPower(iv32))
                    break; // try again
                if (filterIVs && !criteria.IsSatisfiedIVs(iv32))
                    continue;

                if (enc.Species is (ushort)Species.Unown)
                {
                    if (criteria.IsSpecifiedForm())
                    {
                        var form = (byte)criteria.Form;
                        if (!SolaceonRuins4.IsFormValid(LCRNG.Prev4(seed), form))
                            break; // try again
                        pk.Form = form;
                    }
                    else
                    {
                        pk.Form = 8; // Always 100% form as 'I' in one of the rooms. Don't need to check rand(1) choice.
                    }
                }

                if (randLevel)
                {
                    var level = (byte)MethodJ.GetRandomLevel(enc, lv, LeadRequired.None);
                    if (checkLevel && !criteria.IsSatisfiedLevelRange(level))
                        break; // try again
                    pk.MetLevel = pk.CurrentLevel = level;
                }

                pk.PID = pid;
                pk.IV32 = iv32;
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
    public static bool SetFromIVsJ<T>(this T enc, PK4 pk, PersonalInfo4 pi, in EncounterCriteria criteria, out uint origin)
        where T : IEncounterSlot4
    {
        var gr = pi.Gender;
        var id32 = pk.ID32;
        criteria.GetCombinedIVs(out var iv1, out var iv2);
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
            if (criteria.Shiny.IsShiny() != ShinyUtil.GetIsShiny(id32, pid, 8))
                continue;
            if (criteria.IsSpecifiedNature() && !criteria.IsSatisfiedNature((Nature)(pid % 25)))
                continue;

            var gender = EntityGender.GetFromPIDAndRatio(pid, gr);
            if (criteria.IsSpecifiedGender() && !criteria.IsSatisfiedGender(gender))
                continue;

            var lead = criteria.IsSpecifiedLevelRange()
                ? MethodJ.GetSeed(enc, seed, criteria)
                : MethodJ.GetSeed(enc, seed);
            if (!lead.IsValid()) // Verifies the slot, (min) level, and nature loop; if it passes, apply the details.
                continue;

            if (enc.Species is (ushort)Species.Unown)
            {
                if (criteria.IsSpecifiedForm())
                {
                    var form = (byte)criteria.Form;
                    if (!SolaceonRuins4.IsFormValid(seed, form))
                        continue;
                    pk.Form = form;
                }
                else
                {
                    pk.Form = 8; // Always 100% form as 'I' in one of the rooms. Don't need to check rand(1) choice.
                }
            }

            if (MethodJ.IsLevelRand(enc))
            {
                var rand16 = MethodJ.SkipToLevelRand(enc, lead.Seed) >> 16;
                var level = MethodJ.GetRandomLevel(enc, rand16, lead.Lead);
                if (pk.MetLevel != level)
                    pk.MetLevel = pk.CurrentLevel = (byte)level;
            }

            pk.PID = pid;
            pk.IV32 = iv2 << 15 | iv1;
            pk.Gender = gender;
            pk.Ability = (pid & 1) == 0 ? pi.Ability1 : pi.Ability2;
            origin = seed;
            return true;
        }

        origin = 0;
        return false;
    }

    /// <summary>
    /// Combines two 16-bit unsigned integers into a single 32-bit unsigned integer.
    /// </summary>
    /// <param name="a">The first set of 16 bits from a Rand() call.</param>
    /// <param name="b">The second set of 16 bits from a Rand() call.</param>
    public static uint GetPIDRegular(uint a, uint b) => b << 16 | a;
}
