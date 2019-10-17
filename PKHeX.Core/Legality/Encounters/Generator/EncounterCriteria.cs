namespace PKHeX.Core
{
    /// <summary>
    /// Object that can be fed to a <see cref="IEncounterable"/> converter to ensure that the resulting <see cref="PKM"/> meets rough specifications.
    /// </summary>
    public sealed class EncounterCriteria
    {
        public static readonly EncounterCriteria Unrestricted = new EncounterCriteria();

        public int Ability { get; set; } = -1;
        public int Gender { get; set; } = -1;
        public Nature Nature { get; set; } = Nature.Random;
        public Shiny Shiny { get; set; } = Shiny.Random;

        public int IV_HP  { get; set; } = RandomIV;
        public int IV_ATK { get; set; } = RandomIV;
        public int IV_DEF { get; set; } = RandomIV;
        public int IV_SPA { get; set; } = RandomIV;
        public int IV_SPD { get; set; } = RandomIV;
        public int IV_SPE { get; set; } = RandomIV;

        public int HPType { get; set; } = -1;

        private const int RandomIV = -1;

        public bool IsIVsCompatible(int[] encounterIV, int gen)
        {
            var IVs = encounterIV;
            if (!ivCanMatch(IV_HP , IVs[0])) return false;
            if (!ivCanMatch(IV_ATK, IVs[1])) return false;
            if (!ivCanMatch(IV_DEF, IVs[2])) return false;
            if (!ivCanMatch(IV_SPE, IVs[3])) return false;
            if (!ivCanMatch(IV_SPA, IVs[4])) return false;
            if (!ivCanMatch(IV_SPD, IVs[5])) return false;

            bool ivCanMatch(int spec, int enc)
            {
                if (spec >= 30 && gen >= 6) // hyper training possible
                    return true;
                return enc == RandomIV || spec == enc;
            }

            return true;
        }

        public static EncounterCriteria GetCriteria(ShowdownSet s)
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

        public int GetAbility(int abilityType, PersonalInfo pkPersonalInfo)
        {
            if (abilityType < 3)
                return abilityType;

            var abils = pkPersonalInfo.Abilities;
            if (abilityType == 4 && abils.Length > 2 && abils[2] == Ability) // hidden allowed
                return 2;
            if (abils[1] == Ability)
                return 1;
            return 0;
        }
    }
}
