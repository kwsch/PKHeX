using System;

namespace PKHeX.Core;

public static class Gen4GlobalTradeRules
{
    /// <summary>
    /// Checks if the GTS is required to be used for the entity with <see cref="currentLanguage"/> to reach <see cref="tr"/>.
    /// </summary>
    public static bool IsRequiredGTS(ITrainerInfo tr, LanguageID currentLanguage)
    {
        if (tr.Generation != 4)
            return false; // only applies to Gen4

        // Cannot trade only if the trainer is Korean and the game language is not Korean.
        return (LanguageID)tr.Language == LanguageID.Korean && currentLanguage != LanguageID.Korean;
    }

    /// <inheritdoc cref="IsRomanizedKoreanTrainerName(ReadOnlySpan{char})"/>
    public static bool IsRomanizedKoreanTrainerName(PKM pk)
    {
        // Check for GTS name sanitization of Korean GTS trades to International games.
        Span<char> trainer = stackalloc char[pk.TrashCharCountTrainer];
        int len = pk.LoadString(pk.OriginalTrainerTrash, trainer);
        trainer = trainer[..len];

        return IsRomanizedKoreanTrainerName(trainer);
    }

    /// <summary>
    /// Although Korean and non-Korean versions of the Generation IV games cannot trade with each other directly via the Union Room or Wi-Fi Club, trades could be conducted via the GTS.
    /// However, because non-Korean games do not support Korean characters, the data of Pokémon originating from a Korean game are modified.
    /// </summary>
    /// <param name="trainerName">Current (final) trainer name of the entity.</param>
    /// <returns><see langword="true"/> if the trainer name is one of the romanized Korean names, <see langword="false"/> otherwise.</returns>
    public static bool IsRomanizedKoreanTrainerName(ReadOnlySpan<char> trainerName) => trainerName switch
    {
        // This is very un-documented. If you have any samples, please share!
        // Refer to https://github.com/kwsch/PKHeX/issues/4811
        // It's entirely possible that there are only 4 trainer names (determined via OT version & OT gender).
        // It's also possible that the game only sanitizes if the name+nickname has a Korean char.
        "Ahn" => true,   // 안
        "Han" => true,   // 한
        "Jeong" => true, // 정 (41076, Male)
        "Hwang" => true, // 황 (46364, Female)
        _ => false,
    };

    /// <summary>
    /// Fetches a sanitized trainer name for a Korean GTS trade.
    /// </summary>
    /// <remarks>
    /// Please don't use this; we do not know what determines the sanitized name, and it may be subject to change. It's only here as an API stub of sorts.
    /// </remarks>
    public static string GetRomanizedKoreanTrainerName(PK4 pk4) => "Han";
}
