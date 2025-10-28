using System;

namespace PKHeX.Core;

public interface IPersonalAbility
{
    /// <summary>
    /// Gets the index of the <see cref="abilityID"/> within the specification's list of abilities.
    /// </summary>
    /// <param name="abilityID">Ability ID</param>
    /// <returns>Ability Index</returns>
    int GetIndexOfAbility(int abilityID);

    /// <summary>
    /// Gets the ability ID at the specified ability index.
    /// </summary>
    /// <param name="abilityIndex">Ability Index</param>
    /// <returns>Ability ID</returns>
    int GetAbilityAtIndex(int abilityIndex);

    /// <summary>
    /// Gets the count of abilities able to be selected.
    /// </summary>
    /// <remarks>Duplicate abilities still count separately.</remarks>
    int AbilityCount { get; }
}

public interface IPersonalAbility12 : IPersonalAbility
{
    int Ability1 { get; set; }
    int Ability2 { get; set; }
}

public interface IPersonalAbility12H : IPersonalAbility12
{
    int AbilityH { get; set; }
}

public static class PersonalAbilityExtensions
{
    public static bool GetIsAbility12Same(this IPersonalAbility12 pi) => pi.Ability1 == pi.Ability2;
    public static bool GetIsAbilityHiddenUnique(this IPersonalAbility12H pi) => pi.Ability1 != pi.AbilityH;
    public static bool GetIsAbilityPatchPossible(this IPersonalAbility12H pi) => pi.Ability1 != pi.AbilityH || pi.Ability2 != pi.AbilityH;

    public static void GetAbilities(this IPersonalAbility pi, Span<int> result)
    {
        if (pi is not IPersonalAbility12 a)
            return;
        result[0] = a.Ability1;
        result[1] = a.Ability2;
        if (a is not IPersonalAbility12H h)
            return;
        result[2] = h.AbilityH;
    }

    public static void SetAbilities(this IPersonalAbility pi, Span<int> result)
    {
        if (pi is not IPersonalAbility12 a)
            return;
        a.Ability1 = result[0];
        a.Ability2 = result[1];
        if (a is not IPersonalAbility12H h)
            return;
        h.AbilityH = result[2];
    }
}
