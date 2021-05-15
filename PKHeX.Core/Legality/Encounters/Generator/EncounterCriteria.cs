using System.Collections.Generic;
using static PKHeX.Core.AbilityPermission;

namespace PKHeX.Core
{
    /// <summary>
    /// Object that can be fed to a <see cref="IEncounterConvertible"/> converter to ensure that the resulting <see cref="PKM"/> meets rough specifications.
    /// </summary>
    public sealed record EncounterCriteria
    {
        /// <summary>
        /// Default criteria with no restrictions (random) for all fields.
        /// </summary>
        public static readonly EncounterCriteria Unrestricted = new();

        /// <summary> End result's gender. </summary>
        /// <remarks> Leave as -1 to not restrict gender. </remarks>
        public int Gender { get; init; } = -1;

        /// <summary> End result's ability numbers permitted. </summary>
        /// <remarks> Leave as <see cref="Any12H"/> to not restrict ability. </remarks>
        public AbilityPermission AbilityNumber { get; init; } = Any12H;

        /// <summary> End result's nature. </summary>
        /// <remarks> Leave as <see cref="Core.Nature.Random"/> to not restrict nature. </remarks>
        public Nature Nature { get; init; } = Nature.Random;

        /// <summary> End result's nature. </summary>
        /// <remarks> Leave as <see cref="Core.Shiny.Random"/> to not restrict shininess. </remarks>
        public Shiny Shiny { get; init; } = Shiny.Random;

        public int IV_HP  { get; init; } = RandomIV;
        public int IV_ATK { get; init; } = RandomIV;
        public int IV_DEF { get; init; } = RandomIV;
        public int IV_SPA { get; init; } = RandomIV;
        public int IV_SPD { get; init; } = RandomIV;
        public int IV_SPE { get; init; } = RandomIV;

        // unused
        public int HPType { get; init; } = -1;

        private const int RandomIV = -1;

        /// <summary>
        /// Checks if the IVs are compatible with the encounter's defined IV restrictions.
        /// </summary>
        /// <param name="encounterIVs">Encounter template's IV restrictions. Speed is not last.</param>
        /// <param name="generation">Destination generation</param>
        /// <returns>True if compatible, false if incompatible.</returns>
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
                return encounterIV == RandomIV || requestedIV == RandomIV || requestedIV == encounterIV;
            }

            return true;
        }

        /// <inheritdoc cref="GetCriteria(IBattleTemplate, PersonalTable)"/>
        /// <remarks>Uses the latest generation personal table (PKX.Personal); you really should pass the table.</remarks>
        public static EncounterCriteria GetCriteria(IBattleTemplate s) => GetCriteria(s, PKX.Personal);

        /// <inheritdoc cref="GetCriteria(IBattleTemplate, PersonalInfo)"/>
        /// <param name="s">Template data (end result).</param>
        /// <param name="t">Personal table the end result will exist with.</param>
        public static EncounterCriteria GetCriteria(IBattleTemplate s, PersonalTable t)
        {
            var pi = t.GetFormEntry(s.Species, s.Form);
            return GetCriteria(s, pi);
        }

        /// <summary>
        /// Creates a new <see cref="EncounterCriteria"/> by loading parameters from the provided <see cref="IBattleTemplate"/>.
        /// </summary>
        /// <param name="s">Template data (end result).</param>
        /// <param name="pi">Personal info the end result will exist with.</param>
        /// <returns>Initialized criteria data to be passed to generators.</returns>
        public static EncounterCriteria GetCriteria(IBattleTemplate s, PersonalInfo pi)
        {
            int gender = s.Gender;
            return new EncounterCriteria
            {
                Gender = gender,
                IV_HP = s.IVs[0],
                IV_ATK = s.IVs[1],
                IV_DEF = s.IVs[2],
                IV_SPE = s.IVs[3],
                IV_SPA = s.IVs[4],
                IV_SPD = s.IVs[5],
                HPType = s.HiddenPowerType,

                AbilityNumber = GetAbilityNumber(s.Ability, pi),
                Nature = NatureUtil.GetNature(s.Nature),
                Shiny = s.Shiny ? Shiny.Always : Shiny.Never,
            };
        }

        private static AbilityPermission GetAbilityNumber(int ability, PersonalInfo pi)
        {
            var abilities = pi.Abilities;
            if (abilities.Count < 2)
                return 0;
            var dual = GetAbilityValueDual(ability, abilities);
            if (abilities.Count == 2) // prior to gen5
                return dual;
            if (abilities[2] == ability)
                return dual == 0 ? Any12H : OnlyHidden;
            return dual;
        }

        private static AbilityPermission GetAbilityValueDual(int ability, IReadOnlyList<int> abilities)
        {
            if (ability == abilities[0])
                return ability != abilities[1] ? OnlyFirst : Any12;
            return ability == abilities[1] ? OnlySecond : Any12;
        }

        /// <summary>
        /// Gets a random nature to generate, based off an encounter's <see cref="encValue"/>.
        /// </summary>
        public Nature GetNature(Nature encValue)
        {
            if ((uint)encValue < 25)
                return encValue;
            if (Nature != Nature.Random)
                return Nature;
            return (Nature)Util.Rand.Next(25);
        }

        /// <summary>
        /// Gets a random gender to generate, based off an encounter's <see cref="gender"/>.
        /// </summary>
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

        /// <summary>
        /// Gets a random ability index (0/1/2) to generate, based off an encounter's <see cref="num"/>.
        /// </summary>
        public int GetAbilityFromNumber(int num)
        {
            if (num > 0) // fixed number
                return num >> 1;

            bool canBeHidden = num == -1;
            return GetAbilityIndexPreference(canBeHidden);
        }

        /// <summary>
        /// Gets a random ability index (0/1/2) to generate, based off an encounter's <see cref="type"/>.
        /// </summary>
        /// <remarks>This is used for the Mystery Gift ability type arguments.</remarks>
        public int GetAbilityFromType(int type)
        {
            if ((uint)type < 3)
                return type;

            bool canBeHidden = type == 4;
            return GetAbilityIndexPreference(canBeHidden);
        }

        private int GetAbilityIndexPreference(bool canBeHidden = false) => AbilityNumber switch
        {
            OnlyFirst => 0,
            OnlySecond => 1,
            OnlyHidden or Any12H when canBeHidden => 2, // hidden allowed
            _ => Util.Rand.Next(2),
        };

        /// <summary>
        /// Applies random IVs without any correlation.
        /// </summary>
        /// <param name="pk">Entity to mutate.</param>
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

    public enum AbilityPermission : sbyte
    {
        Any12H = -1,
        Any12 = 0,
        OnlyFirst = 1,
        OnlySecond = 2,
        OnlyHidden = 4,
    }
}
