using System;

namespace PKHeX.Core;

/// <summary>
/// Logic for manipulating <see cref="PKM.Gender"/> data.
/// </summary>
public static class GenderApplicator
{
    /// <summary>
    /// Sets the <see cref="PKM.Gender"/> value, with special consideration for the <see cref="PKM.Format"/> values which derive the <see cref="PKM.Gender"/> value.
    /// </summary>
    /// <param name="pk">Pokémon to modify.</param>
    /// <param name="gender">Desired <see cref="PKM.Gender"/> value to set.</param>
    /// <remarks>Has special logic for an unspecified gender.</remarks>
    public static void SetSaneGender(this PKM pk, byte gender)
    {
        var g = gender > 2 ? pk.GetSaneGender() : gender;
        pk.SetGender(g);
    }

    /// <inheritdoc cref="SetSaneGender(PKM, byte)"/>
    public static void SetSaneGender(this PKM pk, byte? gender)
    {
        var g = gender ?? pk.GetSaneGender();
        pk.SetGender(g);
    }

    /// <summary>
    /// Sets the <see cref="PKM.Gender"/> value, with special consideration for the <see cref="PKM.Format"/> values which derive the <see cref="PKM.Gender"/> value.
    /// </summary>
    /// <param name="pk">Pokémon to modify.</param>
    /// <param name="gender">Desired <see cref="PKM.Gender"/> value to set.</param>
    public static void SetGender(this PKM pk, byte gender)
    {
        gender = Math.Clamp(gender, (byte)0, (byte)2);
        if (pk.Gender == gender)
            return;

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
    /// <returns>Most-legal <see cref="PKM.Gender"/> value</returns>
    public static byte GetSaneGender(this PKM pk)
    {
        var gt = pk.PersonalInfo.Gender;
        switch (gt)
        {
            case PersonalInfo.RatioMagicGenderless: return 2;
            case PersonalInfo.RatioMagicFemale: return 1;
            case PersonalInfo.RatioMagicMale: return 0;
        }
        if (!pk.IsGenderValid())
            return EntityGender.GetFromPIDAndRatio(pk.PID, gt);
        return pk.Gender;
    }

    /// <summary>
    /// Updates the <see cref="PKM.IV_ATK"/> for a Generation 1/2 format <see cref="PKM"/>.
    /// </summary>
    /// <param name="pk">Pokémon to modify.</param>
    /// <param name="gender">Desired <see cref="PKM.Gender"/>.</param>
    public static void SetAttackIVFromGender(this PKM pk, byte gender)
    {
        var rnd = Util.Rand;
        while (pk.Gender != gender)
            pk.IV_ATK = rnd.Next(16);
    }
}
