using System;

namespace PKHeX.Core;

/// <summary>
/// Generator methods for Generation 4.
/// </summary>
public static class GenerateMethodK
{
    extension<T>(T enc) where T : IEncounterSlot4
    {
        /// <summary>
        /// Applies a random PID and IVs to the entity, based on the <see cref="PersonalInfo4"/> and <see cref="EncounterCriteria"/>.
        /// </summary>
        /// <param name="pk">Entity to modify</param>
        /// <param name="pi">Personal Info of the entity</param>
        /// <param name="criteria">Criteria to match</param>
        /// <param name="seed">Initial seed to use; will be modified during the loop if the first seed fails</param>
        /// <returns>Method 1 origin seed applied</returns>
        public uint SetRandomK(PK4 pk, PersonalInfo4 pi, in EncounterCriteria criteria, uint seed)
        {
            var id32 = pk.ID32;
            var gr = pi.Gender;
            var (min, max) = SlotMethodK.GetRange(enc.Type, enc.SlotNumber);
            bool randLevel = MethodK.IsLevelRand(enc);
            var modulo = enc.Type.IsSafari ? 10u : 100u;
            bool checkProc = MethodK.IsEncounterCheckApplicable(enc.Type);
            bool checkLevel = criteria.IsSpecifiedLevelRange() && enc.IsLevelWithinRange(criteria);
            bool filterIVs = criteria.IsSpecifiedIVs(2);

            // Generate Method K correlated PID and IVs, no lead (keep things simple).
            while (true)
            {
                if (checkProc)
                {
                    var check = new LeadSeed(seed, LeadRequired.None);
                    if (!MethodK.CheckEncounterActivation(enc, ref check))
                    {
                        seed = LCRNG.Next(seed);
                        continue;
                    }
                }
                var esv = LCRNG.Next16(ref seed) % modulo;
                if (esv < min || esv > max)
                    continue;
                var lv = randLevel ? LCRNG.Next16(ref seed) : 0;
                var nature = MethodK.GetNature(LCRNG.Next16(ref seed));
                if (criteria.IsSpecifiedNature() && !criteria.IsSatisfiedNature((Nature)nature))
                    continue;

                while (true)
                {
                    var a = LCRNG.Next16(ref seed);
                    var b = LCRNG.Next16(ref seed);
                    var pid = GetPIDRegular(a, b);
                    if (pid % 25 != nature)
                        continue;
                    if (ShinyUtil.GetIsShiny3(id32, pid) != criteria.Shiny.IsShiny())
                        break; // try again
                    if (criteria.IsSpecifiedAbility() && !criteria.IsSatisfiedAbility((int)(pid & 1)))
                        break; // try again
                    var gender = EntityGender.GetFromPIDAndRatio(pid, gr);
                    if (criteria.IsSpecifiedGender() && !criteria.IsSatisfiedGender(gender))
                        break; // try again

                    if (enc.Species is (ushort)Species.Unown)
                    {
                        if (criteria.IsSpecifiedForm())
                        {
                            var form = (byte)criteria.Form;
                            if (!RuinsOfAlph4.IsFormValid(LCRNG.Prev4(seed), form))
                                break; // try again
                            pk.Form = form;
                        }
                        else
                        {
                            pk.Form = RuinsOfAlph4.GetEntranceForm(LCRNG.Next2(seed));
                        }
                    }

                    if (randLevel)
                    {
                        var level = (byte)MethodK.GetRandomLevel(enc, lv, LeadRequired.None);
                        if (checkLevel && !criteria.IsSatisfiedLevelRange(level))
                            break; // try again
                        pk.MetLevel = pk.CurrentLevel = level;
                    }

                    var iv32 = ClassicEraRNG.GetSequentialIVs(ref seed);
                    if (criteria.IsSpecifiedHiddenPower() && !criteria.IsSatisfiedHiddenPower(iv32))
                        break; // try again
                    if (filterIVs && !criteria.IsSatisfiedIVs(iv32))
                        continue;

                    if (enc.Type is SlotType4.BugContest && !MethodK.IsAny31(iv32) && !MethodK.IsAny31(iv32 >> 16))
                        break; // try again

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
        /// <param name="pk">Entity to modify</param>
        /// <param name="pi">Personal Info of the entity</param>
        /// <param name="criteria">Criteria to match</param>
        /// <param name="origin">Method 1 origin seed applied</param>
        /// <returns>True if the PID/IV was valid &amp; applied to the entity.</returns>
        public bool SetFromIVsK(PK4 pk, PersonalInfo4 pi, in EncounterCriteria criteria, out uint origin)
        {
            var gr = pi.Gender;
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
                if (criteria.IsSpecifiedNature() && !criteria.IsSatisfiedNature((Nature)(pid % 25)))
                    continue;

                var gender = EntityGender.GetFromPIDAndRatio(pid, gr);
                if (criteria.IsSpecifiedGender() && !criteria.IsSatisfiedGender(gender))
                    continue;
                var lead = criteria.IsSpecifiedLevelRange()
                    ? MethodK.GetSeed(enc, seed, criteria)
                    : MethodK.GetSeed(enc, seed);
                if (!lead.IsValid) // Verifies the slot, (min) level, and nature loop; if it passes, apply the details.
                    continue;

                if (enc.Species is (ushort)Species.Unown)
                {
                    if (criteria.IsSpecifiedForm())
                    {
                        var form = (byte)criteria.Form;
                        if (!RuinsOfAlph4.IsFormValid(seed, form))
                            continue;
                        pk.Form = form;
                    }
                    else
                    {
                        pk.Form = RuinsOfAlph4.GetEntranceForm(LCRNG.Next6(seed)); // ABCD|E(Item)|F(Form) determination
                    }
                }

                if (MethodK.IsLevelRand(enc))
                {
                    var rand16 = MethodK.SkipToLevelRand(enc, lead.Seed, lead.Lead) >> 16;
                    var level = MethodK.GetRandomLevel(enc, rand16, lead.Lead);
                    if (pk.MetLevel != level)
                        pk.MetLevel = pk.CurrentLevel = (byte)level;
                }

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
    }

    /// <summary>
    /// Combines two 16-bit unsigned integers into a single 32-bit unsigned integer.
    /// </summary>
    /// <param name="a">The first set of 16 bits from a Rand() call.</param>
    /// <param name="b">The second set of 16 bits from a Rand() call.</param>
    public static uint GetPIDRegular(uint a, uint b) => b << 16 | a;
}
