using System;

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
    public static bool ShouldIterateForms(ushort species, byte form, byte generation, LearnOption option)
    {
        if (option.IsPast())
            return IsFormChangeDifferentMoves(species, generation);
        return IterateAllForms(species);
    }

    private static bool IterateAllForms(ushort species) => FormChangeMovesRetain.Contains(species);

    /// <summary>
    /// Species that can change between their forms and get access to form-specific moves.
    /// </summary>
    private static ReadOnlySpan<ushort> FormChangeMovesRetain =>
    [
        (int)Species.Deoxys,
        (int)Species.Giratina,
        (int)Species.Shaymin,
        (int)Species.Hoopa,
    ];

    /// <summary>
    /// Species that can change between their forms and get access to form-specific moves.
    /// </summary>
    private static bool IsFormChangeDifferentMoves(ushort species, byte generation) => species switch
    {
        (int)Species.Deoxys    => generation >= 6,
        (int)Species.Giratina  => generation >= 6,
        (int)Species.Shaymin   => generation >= 6,
        (int)Species.Rotom     => generation >= 6,
        (int)Species.Hoopa     => generation >= 6,
        (int)Species.Tornadus  => generation >= 6,
        (int)Species.Thundurus => generation >= 6,
        (int)Species.Landorus  => generation >= 6,
        (int)Species.Urshifu   => generation >= 8,
        (int)Species.Enamorus  => generation >= 8,
        // Fuse
        (int)Species.Kyurem    => generation >= 6,
        (int)Species.Necrozma  => generation >= 8,
        (int)Species.Calyrex   => generation >= 8,

        (int)Species.Pikachu   => generation == 6,

        _ => false,
    };
}
