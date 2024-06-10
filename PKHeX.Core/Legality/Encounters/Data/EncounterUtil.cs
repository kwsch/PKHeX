using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace PKHeX.Core;

/// <summary>
/// Miscellaneous setup utility for legality checking <see cref="IEncounterTemplate"/> data sources.
/// </summary>
public static class EncounterUtil
{
    /// <summary> Magic value to indicate the form is dynamic and should not be used literally. </summary>
    public const byte FormDynamic = FormVivillon;
    /// <summary> Magic value to indicate the form is set by the save file and can be one of many within the permitted range. </summary>
    public const byte FormVivillon = 30;
    /// <summary> Magic value to indicate the form is random and can be one of many within the permitted range. </summary>
    public const byte FormRandom = 31;

    /// <summary>
    /// Gets a raw chunk of data from the specified resource.
    /// </summary>
    public static ReadOnlySpan<byte> Get([Length(2, 2), ConstantExpected] string resource)
        => Util.GetBinaryResource($"encounter_{resource}.pkl");

    /// <summary>
    /// Gets an index-able accessor for the specified resource.
    /// </summary>
    public static BinLinkerAccessor Get([Length(2, 2), ConstantExpected] string resource, [Length(2, 2)] ReadOnlySpan<byte> ident)
        => BinLinkerAccessor.Get(Get(resource), ident);

    /// <summary>
    /// Grabs the localized names for individual templates for all languages from the specified <see cref="index"/> of the <see cref="names"/> list.
    /// </summary>
    /// <param name="names">Arrays of strings grouped by language</param>
    /// <param name="index">Index to grab from the language arrays</param>
    /// <returns>Row of localized strings for the template.</returns>
    public static string[] GetNamesForLanguage(ReadOnlySpan<string[]> names, uint index)
    {
        var result = new string[names.Length];
        for (int i = 0; i < result.Length; i++)
        {
            var arr = names[i];
            result[i] = index < arr.Length ? arr[index] : string.Empty;
        }
        return result;
    }

    /// <summary>
    /// Applies the encounter moves for the given level and version to the provided PKM.
    /// </summary>
    /// <typeparam name="T">Entity type</typeparam>
    /// <param name="pk">Entity to apply the moves to</param>
    /// <param name="version">Version to apply the moves from</param>
    /// <param name="level">Level to apply the moves at</param>
    public static void SetEncounterMoves<T>(T pk, GameVersion version, byte level) where T : PKM
    {
        Span<ushort> moves = stackalloc ushort[4];
        var source = GameData.GetLearnSource(version);
        source.SetEncounterMoves(pk.Species, pk.Form, level, moves);
        pk.SetMoves(moves);
    }

    /// <summary>
    /// Gets a Generation 1-3 trainer name ensuring each character is present in the game's available character set.
    /// </summary>
    /// <remarks>Only falls back to a valid Name if the language is incompatible with the trainer's language. Doesn't sanity check otherwise.</remarks>
    /// <param name="tr">Trainer to apply the name from</param>
    /// <param name="lang">Language to apply the name in</param>
    public static string GetTrainerName(ITrainerInfo tr, int lang) => lang switch
    {
        (int)LanguageID.Japanese => tr.Language == 1 ? tr.OT : "ゲーフリ",
        _ => tr.Language == 1 ? "GF" : tr.OT,
    };

    /// <summary>
    /// Gets a random DV16 value.
    /// </summary>
    /// <param name="rand">Random number generator to use</param>
    /// <returns>Value between 0 and 65535 (inclusive)</returns>
    public static ushort GetRandomDVs(Random rand) => (ushort)rand.Next(ushort.MaxValue + 1);

    /// <summary>
    /// Mashes the IVs into a DV16 value.
    /// </summary>
    public static ushort GetDV16(in IndividualValueSet actual)
    {
        ushort result = 0;
        result |= (ushort)(actual.SPA << 0);
        result |= (ushort)(actual.SPE << 4);
        result |= (ushort)(actual.DEF << 8);
        result |= (ushort)(actual.ATK << 12);
        return result;
    }

    /// <summary>
    /// Gets the Generation 1 personal info for a Species.
    /// </summary>
    /// <param name="version">Pivot version</param>
    /// <param name="species">Species to get the personal info for</param>
    public static PersonalInfo1 GetPersonal1(GameVersion version, ushort species)
    {
        var pt = version == GameVersion.YW ? PersonalTable.Y : PersonalTable.RB;
        return pt[species];
    }
}
