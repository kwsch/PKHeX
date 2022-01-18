using System;

namespace PKHeX.Core
{
    /// <summary>
    /// Contains logic for the Generation 8b (BD/SP) wild and stationary spawns.
    /// </summary>
    public static class Wild8bRNG
    {
        private const int UNSET = -1;

        public static void ApplyDetails(PKM pk, EncounterCriteria criteria, Shiny shiny = Shiny.FixedValue, int flawless = -1)
        {
            if (shiny == Shiny.FixedValue)
                shiny = criteria.Shiny is Shiny.Random or Shiny.Never ? Shiny.Never : Shiny.Always;
            if (flawless == -1)
                flawless = 0;

            int ctr = 0;
            const int maxAttempts = 50_000;
            var rnd = Util.Rand;
            do
            {
                ulong s0 = Util.Rand32(rnd) | (ulong)Util.Rand32(rnd) << 32;
                ulong s1 = Util.Rand32(rnd) | (ulong)Util.Rand32(rnd) << 32;
                var xors = new XorShift128(s0, s1);
                if (TryApplyFromSeed(pk, criteria, shiny, flawless, xors))
                    return;
            } while (++ctr != maxAttempts);

            {
                ulong s0 = Util.Rand32(rnd) | (ulong)Util.Rand32(rnd) << 32;
                ulong s1 = Util.Rand32(rnd) | (ulong)Util.Rand32(rnd) << 32;
                var xors = new XorShift128(s0, s1);
                TryApplyFromSeed(pk, EncounterCriteria.Unrestricted, shiny, flawless, xors);
            }
        }

        public static bool TryApplyFromSeed(PKM pk, EncounterCriteria criteria, Shiny shiny, int flawless, XorShift128 xors)
        {
            // Encryption Constant
            pk.EncryptionConstant = xors.NextUInt();

            // PID
            var fakeTID = xors.NextUInt(); // fakeTID
            var pid = xors.NextUInt();
            pid = GetRevisedPID(fakeTID, pid, pk);
            if (shiny == Shiny.Never)
            {
                if (GetIsShiny(pk.TID, pk.SID, pid))
                    return false;
            }
            else if (shiny != Shiny.Random)
            {
                if (!GetIsShiny(pk.TID, pk.SID, pid))
                    return false;

                if (shiny == Shiny.AlwaysSquare && pk.ShinyXor != 0)
                    return false;
                if (shiny == Shiny.AlwaysStar && pk.ShinyXor == 0)
                    return false;
            }
            pk.PID = pid;

            // Check IVs: Create flawless IVs at random indexes, then the random IVs for not flawless.
            Span<int> ivs = stackalloc[] { UNSET, UNSET, UNSET, UNSET, UNSET, UNSET };
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

            if (!criteria.IsIVsCompatible(ivs, 8))
                return false;

            pk.IV_HP = ivs[0];
            pk.IV_ATK = ivs[1];
            pk.IV_DEF = ivs[2];
            pk.IV_SPA = ivs[3];
            pk.IV_SPD = ivs[4];
            pk.IV_SPE = ivs[5];

            // Ability
            pk.SetAbilityIndex((int)xors.NextUInt(2));

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
                var next = (((int)xors.NextUInt(253) + 1 < genderRatio) ? 1 : 0);
                if (criteria.Gender is 0 or 1 && next != criteria.Gender)
                    return false;
                pk.Gender = next;
            }

            if (criteria.Nature is Nature.Random)
                pk.Nature = (int)xors.NextUInt(25);
            else // Skip nature, assuming Synchronize
                pk.Nature = (int)criteria.Nature;
            pk.StatNature = pk.Nature;

            // Remainder
            var scale = (IScaledSize)pk;
            scale.HeightScalar = (int)xors.NextUInt(0x81) + (int)xors.NextUInt(0x80);
            scale.WeightScalar = (int)xors.NextUInt(0x81) + (int)xors.NextUInt(0x80);

            // Item, don't care
            return true;
        }

        private static uint GetRevisedPID(uint fakeTID, uint pid, ITrainerID tr)
        {
            var xor = GetShinyXor(pid, fakeTID);
            var newXor = GetShinyXor(pid, (uint)(tr.TID | (tr.SID << 16)));

            var fakeRare = GetRareType(xor);
            var newRare = GetRareType(newXor);

            if (fakeRare == newRare)
                return pid;

            var isShiny = xor < 16;
            if (isShiny)
                return (((uint)(tr.TID ^ tr.SID) ^ (pid & 0xFFFF) ^ (xor == 0 ? 0u : 1u)) << 16) | (pid & 0xFFFF); // force same shiny star type
            return pid ^ 0x1000_0000;
        }

        private static Shiny GetRareType(uint xor) => xor switch
        {
            0 => Shiny.AlwaysSquare,
            < 16 => Shiny.AlwaysStar,
            _ => Shiny.Never,
        };

        private static bool GetIsShiny(int tid, int sid, uint pid)
        {
            return GetIsShiny(pid, (uint)((sid << 16) | tid));
        }

        private static bool GetIsShiny(uint pid, uint oid) => GetShinyXor(pid, oid) < 16;

        private static uint GetShinyXor(uint pid, uint oid)
        {
            var xor = pid ^ oid;
            return (xor ^ (xor >> 16)) & 0xFFFF;
        }
    }
}
