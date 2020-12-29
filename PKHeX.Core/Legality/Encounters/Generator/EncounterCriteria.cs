namespace PKHeX.Core
{
    /// <summary>
    /// Object that can be fed to a <see cref="IEncounterable"/> converter to ensure that the resulting <see cref="PKM"/> meets rough specifications.
    /// </summary>
    public sealed record EncounterCriteria
    {
        public static readonly EncounterCriteria Unrestricted = new();

        public int Ability { get; init; } = -1;
        public int Gender { get; init; } = -1;
        public Nature Nature { get; init; } = Nature.Random;
        public Shiny Shiny { get; init; } = Shiny.Random;

        public int IV_HP  { get; init; } = RandomIV;
        public int IV_ATK { get; init; } = RandomIV;
        public int IV_DEF { get; init; } = RandomIV;
        public int IV_SPA { get; init; } = RandomIV;
        public int IV_SPD { get; init; } = RandomIV;
        public int IV_SPE { get; init; } = RandomIV;

        public int HPType { get; init; } = -1;

        private const int RandomIV = -1;

        public bool IsIVsCompatible(int[] encounterIVs, int generation)
        {
            var IVs = encounterIVs;
            if (!ivCanMatch(IV_HP , IVs[0])) return false;
            if (!ivCanMatch(IV_ATK, IVs[1])) return false;
            if (!ivCanMatch(IV_DEF, IVs[2])) return false;
            if (!ivCanMatch(IV_SPE, IVs[3])) return false;
            if (!ivCanMatch(IV_SPA, IVs[4])) return false;
            if (!ivCanMatch(IV_SPD, IVs[5])) return false;

            bool ivCanMatch(int requestedIV, int encounterIV)
            {
                if (requestedIV >= 30 && generation >= 6) // hyper training possible
                    return true;
                return encounterIV == RandomIV || requestedIV == encounterIV;
            }

            return true;
        }

        public static EncounterCriteria GetCriteria(IBattleTemplate s)
        {
            int gender = string.IsNullOrWhiteSpace(s.Gender) ? -1 : PKX.GetGenderFromString(s.Gender);
            return new EncounterCriteria
            {
                Gender = gender,
                Ability = s.Ability,
                IV_HP = s.IVs[0],
                IV_ATK = s.IVs[1],
                IV_DEF = s.IVs[2],
                IV_SPE = s.IVs[3],
                IV_SPA = s.IVs[4],
                IV_SPD = s.IVs[5],
                HPType = s.HiddenPowerType,

                Nature = (Nature)s.Nature,
                Shiny = s.Shiny ? Shiny.Always : Shiny.Never,
            };
        }

        public Nature GetNature(Nature encValue)
        {
            if ((uint)encValue < 25)
                return encValue;
            if (Nature != Nature.Random)
                return Nature;
            return (Nature)Util.Rand.Next(25);
        }

        public int GetGender(int gender, PersonalInfo pkPersonalInfo)
        {
            if ((uint)gender < 3)
                return gender;
            if (!pkPersonalInfo.IsDualGender)
                return pkPersonalInfo.FixedGender;
            if (Gender >= 0)
                return Gender;
            return pkPersonalInfo.RandomGender();
        }

        public int GetAbilityFromNumber(int num, PersonalInfo pkPersonalInfo)
        {
            if (num > 0) // fixed number
                return num >> 1;

            var abils = pkPersonalInfo.Abilities;
            if (abils.Count > 2 && abils[2] == Ability && num == -1) // hidden allowed
                return 2;
            if (abils.Count > 0 && abils[0] == Ability)
                return 0;
            return 1;
        }

        public int GetAbilityFromType(int type, PersonalInfo pkPersonalInfo)
        {
            if ((uint)type < 3)
                return type;

            var abils = pkPersonalInfo.Abilities;
            if (type == 4 && abils.Count > 2 && abils[2] == Ability) // hidden allowed
                return 2;
            if (abils[0] == Ability)
                return 0;
            return 1;
        }

        public void SetRandomIVs(PKM pk)
        {
            pk.IV_HP = IV_HP != RandomIV ? IV_HP : Util.Rand.Next(32);
            pk.IV_ATK = IV_ATK != RandomIV ? IV_ATK : Util.Rand.Next(32);
            pk.IV_DEF = IV_DEF != RandomIV ? IV_DEF : Util.Rand.Next(32);
            pk.IV_SPA = IV_SPA != RandomIV ? IV_SPA : Util.Rand.Next(32);
            pk.IV_SPD = IV_SPD != RandomIV ? IV_SPD : Util.Rand.Next(32);
            pk.IV_SPE = IV_SPE != RandomIV ? IV_SPE : Util.Rand.Next(32);
        }
    }
}
