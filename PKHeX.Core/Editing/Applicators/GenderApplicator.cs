using System;

namespace PKHeX.Core
{
    public static class GenderApplicator
    {
        /// <summary>
        /// Sets the <see cref="PKM.Gender"/> value, with special consideration for the <see cref="PKM.Format"/> values which derive the <see cref="PKM.Gender"/> value.
        /// </summary>
        /// <param name="pk">Pokémon to modify.</param>
        /// <param name="gender">Desired <see cref="PKM.Gender"/> value to set.</param>
        /// <remarks>Has special logic for an unspecified gender.</remarks>
        public static void SetSaneGender(this PKM pk, int gender)
        {
            int g = gender == -1 ? pk.GetSaneGender() : gender;
            pk.SetGender(g);
        }

        /// <summary>
        /// Sets the <see cref="PKM.Gender"/> value, with special consideration for the <see cref="PKM.Format"/> values which derive the <see cref="PKM.Gender"/> value.
        /// </summary>
        /// <param name="pk">Pokémon to modify.</param>
        /// <param name="gender">Desired <see cref="PKM.Gender"/> value to set.</param>
        public static void SetGender(this PKM pk, int gender)
        {
            gender = Math.Min(2, Math.Max(0, gender));
            if (pk.Format <= 2)
            {
                pk.SetAttackIVFromGender(gender);
            }
            else if (pk.Format <= 5)
            {
                pk.SetPIDGender(gender);
                pk.Gender = gender;
            }
            else
            {
                pk.Gender = gender;
            }
        }

        /// <summary>
        /// Sanity checks the provided <see cref="PKM.Gender"/> value, and returns a sane value.
        /// </summary>
        /// <param name="pk"></param>
        /// <returns>Most-legal <see cref="PKM.Gender"/> value</returns>
        public static int GetSaneGender(this PKM pk)
        {
            int gt = pk.PersonalInfo.Gender;
            switch (gt)
            {
                case 255: return 2; // Genderless
                case 254: return 1; // Female-Only
                case 0: return 0; // Male-Only
            }
            if (!pk.IsGenderValid())
                return PKX.GetGenderFromPIDAndRatio(pk.PID, gt);
            return pk.Gender;
        }

        /// <summary>
        /// Updates the <see cref="PKM.IV_ATK"/> for a Generation 1/2 format <see cref="PKM"/>.
        /// </summary>
        /// <param name="pk">Pokémon to modify.</param>
        /// <param name="gender">Desired <see cref="PKM.Gender"/>.</param>
        public static void SetAttackIVFromGender(this PKM pk, int gender)
        {
            var rnd = Util.Rand;
            while (pk.Gender != gender)
                pk.IV_ATK = rnd.Next(16);
        }
    }
}
