using System;
using static PKHeX.Core.LanguageID;

namespace PKHeX.Core;

/// <summary>
/// Transfers from Gen1/2=>Gen7 don't set PP correctly (ignoring PP Ups and bad range fetch). Transporter app language determines which table (not ROM of source or destination).
/// This class contains the expected PP values for each move in each language, as well as methods to check if a given set of moves and PP values match the expected values.
/// </summary>
public static class VirtualConsolePP
{
    // Skip move IDs 0-109, no need to duplicate them since we have the valid table elsewhere.
    private const byte MoveFirst = 110; // Withdraw

    private static ReadOnlySpan<byte> TableENG => [000, 000, 109, 000, 000, 165, 255, 000, 000, 017, 023, 047, 213, 201, 207, 232, 236, 037, 070, 205, 100, 118, 211, 238, 006, 016, 089, 086, 172, 032, 151, 000, 089, 000, 022, 000, 023, 000, 024, 000, 025, 000, 025, 000, 026, 000, 027, 000, 027, 000, 028, 000, 029, 000, 030, 000, 030, 000, 031, 000, 032, 000, 032, 000, 033, 000, 034, 000, 034, 000, 035, 000, 036, 000, 036, 000, 037, 000, 038, 000, 039, 000, 039, 000, 040, 000, 041, 000, 042, 000, 042, 000, 043, 000, 044, 000, 044, 000, 045, 000, 046, 000, 047, 000, 048, 000, 048, 000, 049, 000, 050, 000, 051, 000, 052, 000, 052, 000, 053, 000, 054, 000, 055, 000, 055, 000, 056, 000, 057, 000, 058, 000, 059, 000, 060, 000, 060, 000, 061, 000, 062, 000];
    private static ReadOnlySpan<byte> TableJPN => [000, 000, 123, 000, 000, 164, 255, 000, 000, 022, 030, 059, 224, 028, 037, 065, 069, 142, 180, 065, 015, 039, 147, 178, 206, 219, 032, 087, 082, 003, 045, 000, 105, 000, 023, 000, 024, 000, 024, 000, 025, 000, 026, 000, 027, 000, 028, 000, 029, 000, 030, 000, 031, 000, 031, 000, 032, 000, 033, 000, 034, 000, 035, 000, 036, 000, 037, 000, 038, 000, 039, 000, 039, 000, 040, 000, 041, 000, 042, 000, 043, 000, 044, 000, 045, 000, 046, 000, 046, 000, 047, 000, 048, 000, 049, 000, 050, 000, 051, 000, 052, 000, 053, 000, 054, 000, 055, 000, 055, 000, 056, 000, 057, 000, 058, 000, 059, 000, 060, 000, 061, 000, 062, 000, 063, 000, 064, 000, 065, 000, 066, 000, 067, 000, 068, 000, 069, 000, 070, 000, 062, 000];
    private static ReadOnlySpan<byte> TableSPA => [000, 000, 109, 000, 000, 161, 255, 000, 000, 017, 023, 048, 147, 147, 153, 179, 182, 242, 022, 161, 054, 072, 170, 197, 221, 232, 226, 231, 064, 219, 105, 000, 093, 000, 023, 000, 023, 000, 024, 000, 025, 000, 026, 000, 026, 000, 027, 000, 028, 000, 029, 000, 030, 000, 030, 000, 031, 000, 032, 000, 033, 000, 033, 000, 034, 000, 035, 000, 036, 000, 037, 000, 037, 000, 038, 000, 039, 000, 040, 000, 040, 000, 041, 000, 042, 000, 043, 000, 044, 000, 045, 000, 045, 000, 046, 000, 046, 000, 047, 000, 048, 000, 049, 000, 050, 000, 050, 000, 051, 000, 052, 000, 053, 000, 054, 000, 054, 000, 055, 000, 056, 000, 057, 000, 058, 000, 058, 000, 059, 000, 060, 000, 061, 000, 062, 000, 063, 000, 064, 000, 064, 000];
    private static ReadOnlySpan<byte> TableFRE => [000, 000, 113, 000, 000, 165, 255, 000, 000, 017, 023, 049, 173, 169, 176, 201, 204, 007, 042, 199, 093, 111, 206, 236, 003, 014, 054, 059, 154, 091, 016, 000, 092, 000, 023, 000, 023, 000, 024, 000, 025, 000, 026, 000, 026, 000, 027, 000, 028, 000, 029, 000, 029, 000, 030, 000, 031, 000, 031, 000, 032, 000, 033, 000, 034, 000, 034, 000, 035, 000, 036, 000, 036, 000, 037, 000, 038, 000, 039, 000, 039, 000, 040, 000, 041, 000, 042, 000, 043, 000, 043, 000, 044, 000, 045, 000, 046, 000, 046, 000, 047, 000, 048, 000, 049, 000, 050, 000, 050, 000, 051, 000, 052, 000, 053, 000, 054, 000, 054, 000, 055, 000, 056, 000, 057, 000, 058, 000, 058, 000, 059, 000, 060, 000, 061, 000, 062, 000, 063, 000, 063, 000];
    private static ReadOnlySpan<byte> TableDEU => [000, 000, 110, 000, 000, 166, 255, 000, 000, 017, 023, 049, 191, 177, 184, 208, 212, 022, 057, 207, 099, 118, 222, 251, 017, 028, 065, 065, 114, 036, 251, 000, 093, 000, 023, 000, 023, 000, 024, 000, 025, 000, 026, 000, 026, 000, 027, 000, 028, 000, 028, 000, 029, 000, 030, 000, 031, 000, 032, 000, 032, 000, 033, 000, 034, 000, 035, 000, 035, 000, 036, 000, 037, 000, 038, 000, 038, 000, 039, 000, 040, 000, 041, 000, 041, 000, 042, 000, 043, 000, 044, 000, 045, 000, 045, 000, 046, 000, 047, 000, 048, 000, 049, 000, 049, 000, 050, 000, 051, 000, 052, 000, 053, 000, 053, 000, 054, 000, 055, 000, 056, 000, 057, 000, 057, 000, 058, 000, 059, 000, 060, 000, 061, 000, 062, 000, 063, 000, 063, 000, 064, 000];
    private static ReadOnlySpan<byte> TableKOR => [000, 000, 082, 000, 000, 162, 255, 000, 000, 017, 022, 039, 237, 154, 160, 178, 180, 219, 239, 050, 186, 203, 253, 014, 029, 036, 039, 247, 228, 151, 138, 000, 056, 000, 022, 000, 023, 000, 023, 000, 024, 000, 024, 000, 024, 000, 025, 000, 025, 000, 025, 000, 026, 000, 026, 000, 027, 000, 027, 000, 027, 000, 028, 000, 028, 000, 028, 000, 029, 000, 029, 000, 030, 000, 030, 000, 030, 000, 031, 000, 031, 000, 031, 000, 032, 000, 032, 000, 033, 000, 033, 000, 033, 000, 034, 000, 034, 000, 034, 000, 035, 000, 035, 000, 036, 000, 036, 000, 036, 000, 037, 000, 037, 000, 038, 000, 038, 000, 038, 000, 039, 000, 039, 000, 040, 000, 040, 000, 040, 000, 041, 000, 041, 000, 041, 000, 042, 000, 042, 000, 043, 000];
    private static ReadOnlySpan<byte> TableITA => [000, 000, 109, 000, 000, 166, 255, 000, 000, 017, 023, 048, 185, 173, 180, 206, 209, 012, 046, 195, 090, 108, 197, 225, 248, 003, 074, 078, 163, 046, 202, 000, 093, 000, 022, 000, 023, 000, 024, 000, 025, 000, 025, 000, 026, 000, 027, 000, 028, 000, 029, 000, 029, 000, 030, 000, 031, 000, 032, 000, 032, 000, 033, 000, 034, 000, 035, 000, 035, 000, 036, 000, 037, 000, 037, 000, 038, 000, 039, 000, 040, 000, 041, 000, 041, 000, 042, 000, 043, 000, 044, 000, 045, 000, 045, 000, 046, 000, 047, 000, 048, 000, 048, 000, 049, 000, 050, 000, 051, 000, 052, 000, 052, 000, 053, 000, 054, 000, 055, 000, 056, 000, 057, 000, 057, 000, 058, 000, 059, 000, 060, 000, 061, 000, 062, 000, 062, 000, 063, 000, 064, 000];
    private static ReadOnlySpan<byte> TableZHS => [000, 000, 082, 000, 000, 162, 255, 000, 000, 017, 022, 038, 154, 068, 074, 092, 094, 126, 146, 198, 075, 092, 132, 147, 160, 167, 106, 051, 200, 227, 050, 000, 051, 000, 022, 000, 023, 000, 023, 000, 023, 000, 024, 000, 024, 000, 024, 000, 025, 000, 025, 000, 025, 000, 026, 000, 026, 000, 026, 000, 027, 000, 027, 000, 027, 000, 028, 000, 028, 000, 028, 000, 028, 000, 029, 000, 029, 000, 029, 000, 030, 000, 030, 000, 030, 000, 031, 000, 031, 000, 031, 000, 032, 000, 032, 000, 032, 000, 033, 000, 033, 000, 033, 000, 034, 000, 034, 000, 034, 000, 035, 000, 035, 000, 035, 000, 036, 000, 036, 000, 036, 000, 037, 000, 037, 000, 037, 000, 038, 000, 038, 000, 038, 000, 039, 000, 039, 000, 040, 000, 040, 000];
    private static ReadOnlySpan<byte> TableZHT => [000, 000, 082, 000, 000, 163, 255, 000, 000, 017, 022, 039, 151, 065, 072, 089, 091, 124, 143, 195, 073, 089, 130, 145, 158, 165, 107, 052, 199, 227, 060, 000, 051, 000, 022, 000, 023, 000, 023, 000, 023, 000, 024, 000, 024, 000, 024, 000, 025, 000, 025, 000, 025, 000, 026, 000, 026, 000, 026, 000, 027, 000, 027, 000, 027, 000, 028, 000, 028, 000, 028, 000, 028, 000, 029, 000, 029, 000, 029, 000, 030, 000, 030, 000, 030, 000, 031, 000, 031, 000, 031, 000, 032, 000, 032, 000, 032, 000, 033, 000, 033, 000, 033, 000, 034, 000, 034, 000, 034, 000, 035, 000, 035, 000, 035, 000, 036, 000, 036, 000, 036, 000, 037, 000, 037, 000, 037, 000, 038, 000, 038, 000, 038, 000, 039, 000, 039, 000, 040, 000, 040, 000];

    /// <summary>
    /// Sanity check the languages possible.
    /// </summary>
    /// <param name="language">Language ID of the Transporter application to check.</param>
    /// <returns>True if the language is supported for VC PP table; false otherwise.</returns>
    public static bool IsSupportedLanguage(LanguageID language) => language is (>= Japanese and <= ChineseT) and not UNUSED_6;

    /// <summary>
    /// Gets the VC PP table for the specified Transporter application language.
    /// </summary>
    public static ReadOnlySpan<byte> GetTable(LanguageID language) => language switch
    {
        English => TableENG,
        Japanese => TableJPN,
        Spanish => TableSPA,
        French => TableFRE,
        German => TableDEU,
        Korean => TableKOR,
        Italian => TableITA,
        ChineseS => TableZHS,
        ChineseT => TableZHT,
        _ => throw new NotSupportedException($"Language {language} is not supported for VC PP table."),
    };

    /// <summary>
    /// Fetches the expected PP for the given move from the VC PP table. Returns false if the move is not in the VC range.
    /// </summary>
    /// <param name="move">Move ID.</param>
    /// <param name="arr">Glitched PP table for the Pokémon's language.</param>
    /// <param name="pp">Output expected PP for the move.</param>
    /// <returns><see langword="true"/> if the move is in the VC range and the expected PP was fetched; <see langword="false"/> otherwise.</returns>
    public static bool IsGlitched(ushort move, ReadOnlySpan<byte> arr, out byte pp)
    {
        var index = move - MoveFirst;
        if (index < 0)
        {
            pp = MoveInfo7.PP[move]; // fetch from the normal table for moves below the glitched range
            return true;
        }
        if ((uint)index < arr.Length)
        {
            pp = arr[index]; // fetch from the glitched table for moves in the glitched range
            return true;
        }
        pp = 0; // unexpected move, ignore.
        return false;
    }

    /// <summary>
    /// Checks if all moves are possible from R/B/Y only.
    /// </summary>
    public static bool IsPossibleVC1(ReadOnlySpan<ushort> moves)
        => !moves.ContainsAnyExceptInRange<ushort>(0, Legal.MaxMoveID_1);

    /// <summary>
    /// Checks if all moves are possible from R/B/Y/G/S/C only.
    /// </summary>
    public static bool IsPossibleVC2(ReadOnlySpan<ushort> moves)
        => !moves.ContainsAnyExceptInRange<ushort>(0, Legal.MaxMoveID_2);

    /// <summary>
    /// Checks the moves against the table to see if all match their stored PP value.
    /// </summary>
    public static bool IsMatch(LanguageID language, ReadOnlySpan<ushort> moves, ReadOnlySpan<int> pp)
    {
        var arr = GetTable(language);
        for (int i = 0; i < moves.Length; i++)
        {
            var move = moves[i];
            if (!IsGlitched(move, arr, out var expected))
                return false;
            if (pp[i] != expected)
                return false;
        }
        return true;
    }

    /// <summary>
    /// Checks the moves against all supported Transfer application languages to see if any match their stored PP values.
    /// </summary>
    public static bool IsMatchAnyLanguage(ReadOnlySpan<byte> trash, ReadOnlySpan<ushort> moves, ReadOnlySpan<int> pp, ushort species)
    {
        for (var language = Japanese; language <= ChineseT; language++)
        {
            if (language == UNUSED_6)
                continue;
            if (!IsMatch(language, moves, pp))
                continue;
            if (!IsTrashMatch(trash, species, language))
                continue;
            return true;
        }
        return false;
    }

    private static bool IsTrashMatch(ReadOnlySpan<byte> trash, ushort species, LanguageID language)
    {
        var expectName = SpeciesName.GetSpeciesName(species, (int)language);
        var currentLength = TrashBytesUTF16.GetStringLength(trash) + 1; // terminator
        var match = TrashBytesUTF16.IsUnderlayerPresent(expectName, trash, currentLength);
        return !match.IsInvalid;
    }
}
