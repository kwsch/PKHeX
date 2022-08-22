using System;
using System.Collections.Generic;

namespace PKHeX.Core;

/// <summary>
/// Logic for checking if an entity can freely change <see cref="PKM.Form"/>.
/// </summary>
public static class FormChangeUtil
{
    /// <summary>
    /// Checks if all forms should be iterated when checking for moves.
    /// </summary>
    /// <param name="species">Entity species</param>
    /// <param name="form">Entity form</param>
    /// <param name="generation">Generation we're checking in</param>
    /// <param name="option">Conditions we're checking with</param>
    public static bool ShouldIterateForms(ushort species, byte form, int generation, LearnOption option)
    {
        if (option is not LearnOption.Current)
            return FormChangeMoves.TryGetValue(species, out var func) && func((generation, form));
        return IterateAllForms(species);
    }

    private static bool IterateAllForms(int species) => FormChangeMovesRetain.Contains(species);

    /// <summary>
    /// Species that can change between their forms and get access to form-specific moves.
    /// </summary>
    private static readonly HashSet<int> FormChangeMovesRetain = new()
    {
        (int)Species.Deoxys,
        (int)Species.Giratina,
        (int)Species.Shaymin,
        (int)Species.Hoopa,
    };

    /// <summary>
    /// Species that can change between their forms and get access to form-specific moves.
    /// </summary>
    private static readonly Dictionary<int, Func<(int Generation, int Form), bool>> FormChangeMoves = new()
    {
        {(int)Species.Deoxys,   g => g.Generation >= 6},
        {(int)Species.Giratina, g => g.Generation >= 6},
        {(int)Species.Shaymin,  g => g.Generation >= 6},
        {(int)Species.Rotom,    g => g.Generation >= 6},
        {(int)Species.Hoopa,    g => g.Generation >= 6},
        {(int)Species.Tornadus, g => g.Generation >= 6},
        {(int)Species.Thundurus,g => g.Generation >= 6},
        {(int)Species.Landorus, g => g.Generation >= 6},
        {(int)Species.Urshifu,  g => g.Generation >= 8},
        {(int)Species.Enamorus, g => g.Generation >= 8},
        // Fuse
        {(int)Species.Kyurem,   g => g.Generation >= 6},
        {(int)Species.Necrozma, g => g.Generation >= 8},
        {(int)Species.Calyrex,  g => g.Generation >= 8},

        {(int)Species.Pikachu,  g => g.Generation == 6},
    };
}
