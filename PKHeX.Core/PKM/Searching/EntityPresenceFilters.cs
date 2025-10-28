using System;
using static PKHeX.Core.EntityContext;

namespace PKHeX.Core.Searching;

/// <summary>
/// Logic to filter a large amount of entities depending on if they exist in a specific context.
/// </summary>
/// <remarks>
/// Pre-filtering should be done to disallow any species above the maximum for that context.
/// </remarks>
public static class EntityPresenceFilters
{
    /// <summary>
    /// Returns a filter that checks if the <see cref="PKM"/> can be present in a specified context.
    /// </summary>
    /// <param name="context">Destination context.</param>
    /// <returns>Null if no filter is applicable (allow all)</returns>
    public static Func<PKM, bool>? GetFilterEntity(EntityContext context) => context switch
    {
        // Allow if it already is that format (eager check)
        Gen9a => static pk => pk is PA9 || PersonalTable.ZA.IsPresentInGame(pk.Species, pk.Form),
        Gen9  => static pk => pk is PK9 || PersonalTable.SV.IsPresentInGame(pk.Species, pk.Form),
        Gen8a => static pk => pk is PA8 || PersonalTable.LA.IsPresentInGame(pk.Species, pk.Form),
        Gen8b => static pk => pk is PB8 || PersonalTable.BDSP.IsPresentInGame(pk.Species, pk.Form),
        Gen8  => static pk => pk is PK8 || PersonalTable.SWSH.IsPresentInGame(pk.Species, pk.Form),
        _ => null,
    };

    /// <summary>
    /// Returns a filter that checks if the <see cref="ISpeciesForm"/> can be present in a specified context.
    /// </summary>
    /// <param name="context">Destination context.</param>
    /// <returns>Null if no filter is applicable (allow all)</returns>
    public static Func<T, bool>? GetFilterGeneric<T>(EntityContext context) where T : ISpeciesForm => context switch
    {
        Gen9a => IsPresent<T, PersonalTable9ZA>(PersonalTable.ZA),
        Gen9  => IsPresent<T, PersonalTable9SV>(PersonalTable.SV),
        Gen8a => IsPresent<T, PersonalTable8LA>(PersonalTable.LA),
        Gen8b => IsPresent<T, PersonalTable8BDSP>(PersonalTable.BDSP),
        Gen8  => IsPresent<T, PersonalTable8SWSH>(PersonalTable.SWSH),
        _ => null,
    };

    /// <summary>
    /// Simple filter for Mystery Gift templates; will exclude any species not present in game, and cross-check the generation if it can be transferred in.
    /// </summary>
    public static Func<T, bool>? GetFilterGift<T>(EntityContext context, byte generation) where T : ISpeciesForm, IGeneration => context switch
    {
        Gen7b => z => z is WB7,
        Gen7  => z => z is WC7 || z.Generation < 7,
        _ when generation < 7 => z => z.Generation <= generation,
        _ => GetFilterGeneric<T>(context),
    };

    private static Func<TSpecies, bool> IsPresent<TSpecies, TTable>(TTable pt)
        where TSpecies : ISpeciesForm
        where TTable : IPersonalTable
        => z =>
    {
        if (pt.IsPresentInGame(z.Species, z.Form))
            return true;
        return z is IEncounterFormRandom { IsRandomUnspecificForm: true } && pt.IsSpeciesInGame(z.Species);
    };
}
