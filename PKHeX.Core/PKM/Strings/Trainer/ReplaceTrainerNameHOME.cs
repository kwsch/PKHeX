using System;
using static PKHeX.Core.EntityContext;
using static PKHeX.Core.PersonalTable;

namespace PKHeX.Core;

/// <summary>
/// Logic for replacing the name of a Pokémon in any game adjacent to HOME.
/// </summary>
public static class ReplaceTrainerNameHOME
{
    /// <summary>
    /// Checks if the original name is a trigger for replacement, and if the current name is a valid replacement.
    /// </summary>
    /// <param name="original">Original name to check for replacement trigger.</param>
    /// <param name="current">Current name to check for valid replacement.</param>
    /// <param name="language">Entity language.</param>
    /// <param name="origin">Entity game version.</param>
    /// <param name="species">Original species to check for game presence.</param>
    /// <param name="form">Original form to check for game presence.</param>
    public static EntityContext IsTriggerAndReplace(ReadOnlySpan<char> original, ReadOnlySpan<char> current, LanguageID language, GameVersion origin, ushort species, byte form)
    {
        // Use the encounter species-form to determine if the Pokémon can visit the game.
        if (SWSH.IsPresentInGame(species, form) && ReplaceTrainerName8 .IsTriggerAndReplace(original, current, language))
            return Gen8;
        if (LA  .IsPresentInGame(species, form) && ReplaceTrainerName8a.IsTriggerAndReplace(original, current, language))
            return Gen8a;
        if (BDSP.IsPresentInGame(species, form) && ReplaceTrainerName8b.IsTriggerAndReplace(original, current, language, origin))
            return Gen8b;
        if (SV  .IsPresentInGame(species, form) && ReplaceTrainerName9 .IsTriggerAndReplace(original, current, language))
            return Gen9;
        return None; // No replacement
    }

    /// <inheritdoc cref="IsTriggerAndReplace(ReadOnlySpan{char},ReadOnlySpan{char},LanguageID,GameVersion,ushort,byte)"/>
    public static EntityContext IsTriggerAndReplace(ReadOnlySpan<char> original, ReadOnlySpan<char> current, LanguageID language, GameVersion origin, EvolutionHistory history)
    {
        if (history.HasVisitedSWSH && ReplaceTrainerName8 .IsTriggerAndReplace(original, current, language))
            return Gen8;
        if (history.HasVisitedPLA  && ReplaceTrainerName8a.IsTriggerAndReplace(original, current, language))
            return Gen8a;
        if (history.HasVisitedBDSP && ReplaceTrainerName8b.IsTriggerAndReplace(original, current, language, origin))
            return Gen8b;
        if (history.HasVisitedGen9 && ReplaceTrainerName9 .IsTriggerAndReplace(original, current, language))
            return Gen9;
        return None; // No replacement
    }

    /// <summary>
    /// Checks if the current string is a replaced string for the given game version and language.
    /// </summary>
    /// <param name="current">The current trainer name as a read-only span of characters.</param>
    /// <param name="language">The language identifier for the trainer name.</param>
    /// <param name="origin">The game version where the Pokémon originated.</param>
    /// <param name="species">Original species to check for game presence.</param>
    /// <param name="form">Original form to check for game presence.</param>
    public static EntityContext IsReplace(ReadOnlySpan<char> current, LanguageID language, GameVersion origin, ushort species, byte form)
    {
        // Use the encounter species-form to determine if the Pokémon can visit the game.
        if (SWSH.IsPresentInGame(species, form) && ReplaceTrainerName8 .IsReplace(current, language))
            return Gen8;
        if (LA  .IsPresentInGame(species, form) && ReplaceTrainerName8a.IsReplace(current, language))
            return Gen8a;
        if (BDSP.IsPresentInGame(species, form) && ReplaceTrainerName8b.IsReplace(current, language, origin))
            return Gen8b;
        if (SV  .IsPresentInGame(species, form) && ReplaceTrainerName9 .IsReplace(current, language))
            return Gen9;
        return None; // No replacement
    }

    /// <inheritdoc cref="IsReplace(ReadOnlySpan{char},LanguageID,GameVersion,ushort,byte)"/>
    public static EntityContext IsReplace(ReadOnlySpan<char> current, LanguageID language, GameVersion origin, EvolutionHistory history)
    {
        if (history.HasVisitedSWSH && ReplaceTrainerName8 .IsReplace(current, language))
            return Gen8;
        if (history.HasVisitedPLA  && ReplaceTrainerName8a.IsReplace(current, language))
            return Gen8a;
        if (history.HasVisitedBDSP && ReplaceTrainerName8b.IsReplace(current, language, origin))
            return Gen8b;
        if (history.HasVisitedGen9 && ReplaceTrainerName9 .IsReplace(current, language))
            return Gen9;
        return None; // No replacement
    }
}
