using System.Collections.Generic;

namespace PKHeX.Core;

public interface IPersonalAbility
{
    /// <summary>
    /// Full list of <see cref="PKM.Ability"/> values the entry can have.
    /// </summary>
    IReadOnlyList<int> Abilities { get; set; }

    /// <summary>
    /// Gets the ability index without creating an array and looking through it.
    /// </summary>
    /// <param name="abilityID">Ability ID</param>
    /// <returns>Ability Index</returns>
    int GetAbilityIndex(int abilityID);
}

public interface IPersonalAbility12
{
    int Ability1 { get; set; }
    int Ability2 { get; set; }
}

public interface IPersonalAbility12H : IPersonalAbility12
{
    int AbilityH { get; set; }
}
