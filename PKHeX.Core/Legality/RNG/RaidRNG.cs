using System.Linq;

namespace PKHeX.Core
{
    public static class RaidRNG
    {
        public static bool Verify<T>(this T raid, PK8 pk8, ulong seed) where T: EncounterStatic8Nest<T>
        {
            var pi = PersonalTable.SWSH.GetFormeEntry(raid.Species, raid.Form);
            var ratio = pi.Gender;
            var abil = RemapAbilityToParam(raid.Ability);
            var IVs = raid.IVs.Count == 0 ? GetBlankIVTemplate() : PKX.ReorderSpeedLast((int[])((int[])raid.IVs).Clone());
            return Verify(pk8, seed, IVs, raid.FlawlessIVCount, abil, ratio);
        }

        public static void ApplyDetailsTo<T>(this T raid, PK8 pk8, ulong seed) where T : EncounterStatic8Nest<T>
        {
            // Ensure the species-form is set correctly (nature)
            pk8.Species = raid.Species;
            pk8.AltForm = raid.Form;
            var pi = PersonalTable.SWSH.GetFormeEntry(raid.Species, raid.Form);
            var ratio = pi.Gender;
            var abil = RemapAbilityToParam(raid.Ability);
            var IVs = raid.IVs.Count == 0 ? GetBlankIVTemplate() : PKX.ReorderSpeedLast((int[])((int[])raid.IVs).Clone());
            ApplyDetailsTo(pk8, seed, IVs, raid.FlawlessIVCount, abil, ratio);
        }

        private static int RemapAbilityToParam(int a)
        {
            return a switch
            {
                -1 => 254,
                0 => 255,
                _ => (a >> 1)
            };
        }

        private static int[] GetBlankIVTemplate() => new[] {-1, -1, -1, -1, -1, -1};

        private static bool Verify(PKM pk, ulong seed, int[] ivs, int iv_count, int ability_param, int gender_ratio, sbyte nature_param = -1, Shiny shiny = Shiny.Random)
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

            if (isShiny)
            {
                if (!GetIsShiny(pk.TID, pk.SID, pid))
                    pid = GetShinyPID(pk.TID, pk.SID, pid, 0);
            }
            else
            {
                if (GetIsShiny(pk.TID, pk.SID, pid))
                    pid ^= 0x1000_0000;
            }

            if (pk.PID != pid)
                return false;

            const int UNSET = -1;
            const int MAX = 31;
            for (int i = ivs.Count(z => z == MAX); i < iv_count; i++)
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

            int abil;
            if (ability_param == 254)
                abil = (int)rng.NextInt(3);
            else if (ability_param == 255)
                abil = (int)rng.NextInt(2);
            else
                abil = ability_param;
            abil <<= 1; // 1/2/4

            if ((pk.AbilityNumber == 4) != (abil == 4))
                return false;

            switch (gender_ratio)
            {
                case 255 when pk.Gender != 2:
                    if (pk.Gender != 2)
                        return false;
                    break;
                case 254 when pk.Gender != 1:
                    if (pk.Gender != 1)
                        return false;
                    break;
                case 000:
                    if (pk.Gender != 0)
                        return false;
                    break;
                default:
                    var gender = (int)rng.NextInt(252) + 1 < gender_ratio ? 1 : 0;
                    if (pk.Gender != gender)
                        return false;
                    break;
            }

            if (nature_param == -1)
            {
                if (pk.Species == (int) Species.Toxtricity && pk.AltForm == 0)
                {
                    var table = Nature0;
                    var choice = table[rng.NextInt((uint)table.Length)];
                    if (pk.Nature != choice)
                        return false;
                }
                else if (pk.Species == (int) Species.Toxtricity && pk.AltForm == 1)
                {
                    var table = Nature1;
                    var choice = table[rng.NextInt((uint)table.Length)];
                    if (pk.Nature != choice)
                        return false;
                }
                else
                {
                    var nature = (int)rng.NextInt(25);
                    if (pk.Nature != nature)
                        return false;
                }
            }
            else
            {
                if (pk.Nature != nature_param)
                    return false;
            }

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

        private static bool ApplyDetailsTo(PKM pk, ulong seed, int[] ivs, int iv_count, int ability_param, int gender_ratio, sbyte nature_param = -1, Shiny shiny = Shiny.Random)
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
                if (!GetIsShiny(pk.TID, pk.SID, pid))
                    pid = GetShinyPID(pk.TID, pk.SID, pid, 0);
            }
            else
            {
                if (GetIsShiny(pk.TID, pk.SID, pid))
                    pid ^= 0x1000_0000;
            }

            pk.PID = pid;

            const int UNSET = -1;
            const int MAX = 31;
            for (int i = ivs.Count(z => z == MAX); i < iv_count; i++)
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

            int abil;
            if (ability_param == 254)
                abil = (int)rng.NextInt(3);
            else if (ability_param == 255)
                abil = (int)rng.NextInt(2);
            else
                abil = ability_param;
            pk.RefreshAbility(abil);

            switch (gender_ratio)
            {
                case 255:
                    pk.Gender = 2;
                    break;
                case 254:
                    pk.Gender = 1;
                    break;
                case 000:
                    pk.Gender = 0;
                    break;
                default:
                    var gender = (int)rng.NextInt(252) + 1 < gender_ratio ? 1 : 0;
                    pk.Gender = gender;
                    break;
            }

            int nature;
            if (nature_param == -1)
            {
                if (pk.Species == (int)Species.Toxtricity && pk.AltForm == 0)
                {
                    var table = Nature0;
                    nature = table[rng.NextInt((uint)table.Length)];
                }
                else if (pk.Species == (int)Species.Toxtricity && pk.AltForm == 1)
                {
                    var table = Nature1;
                    nature = table[rng.NextInt((uint)table.Length)];
                }
                else
                {
                    nature = (int)rng.NextInt(25);
                }
            }
            else
            {
                nature = nature_param;
            }

            pk.StatNature = pk.Nature = nature;

            if (pk is IScaledSize s)
            {
                var height = (int)rng.NextInt(0x81) + (int)rng.NextInt(0x80);
                var weight = (int)rng.NextInt(0x81) + (int)rng.NextInt(0x80);
                s.HeightScalar = height;
                s.WeightScalar = weight;
            }

            return true;
        }

        private static uint GetShinyPID(int tid, int sid, uint pid, int type)
        {
            return (uint) (((tid ^ sid ^ (pid & 0xFFFF) ^ type) << 16) | (pid & 0xFFFF));
        }

        private static bool GetIsShiny(int tid, int sid, uint pid)
        {
            return GetShinyXor(pid, (uint) ((sid << 16) | tid)) < 16;
        }

        private static uint GetShinyXor(uint pid, uint oid)
        {
            var xor = pid ^ oid;
            return (xor ^ (xor >> 16)) & 0xFFFF;
        }

        private static readonly int[] Nature0 = {3, 4, 2, 8, 9, 19, 22, 11, 13, 14, 0, 6, 24};
        private static readonly int[] Nature1 = {1, 5, 7, 10, 12, 15, 16, 17, 18, 20, 21, 23};
    }
}