namespace PKHeX.Core;

/// <summary>
/// Logic pertaining to Pokérus -- the virus that doubles EVs gained from battle.
/// </summary>
public static class Pokerus
{
    /// <summary>
    /// Gets the max duration (in days) that a strain can be infectious.
    /// </summary>
    /// <param name="strain">Strain number</param>
    /// <returns>Initial duration (in days). When the value decrements to zero, the Pokémon is no longer infectious.</returns>
    public static int GetMaxDuration(int strain) => (strain & 3) + 1;

    /// <summary>
    /// Checks if any Pokérus values are possible to have on the input entity.
    /// </summary>
    /// <param name="pk">Entity to check</param>
    /// <param name="enc">Encounter template matched to</param>
    /// <returns>True if Pokérus exists in the game format, or can be transmitted to the entity via another game.</returns>
    public static bool IsObtainable(PKM pk, ISpeciesForm enc) => pk switch
    {
        PA8 pa8 => HasVisitedAnother(pa8, enc),
        PB7 => false, // Does not exist in game.
        PK9 => false, // Does not exist in game, does not get copied over via HOME.
        _ => true,
    };

    private static bool HasVisitedAnother(PA8 pk, ISpeciesForm enc)
    {
        if (pk.IsUntraded)
            return false;
        if (pk.Tracker == 0)
            return false;

        // Ideally we can just check an evolution history to see if it has visited an infectious game, but this method has limited input.
        // Use encounter as the lowest evolved species, as Hisuian forms/evolutions are not available on BD/SP or SW/SH.
        if (PersonalTable.BDSP.IsPresentInGame(enc.Species, enc.Form))
            return true;
        if (PersonalTable.SWSH.IsPresentInGame(enc.Species, enc.Form))
            return true;
        // S/V, BD/SP, and SW/SH have the ability to receive every Pokémon
        // However, S/V does not have Pokérus!

        // Species that cannot originate from PLA/SV and have Pokérus.
        // Any Hisuian form
        // Oshawott, Dewott, Samurott, Enamorus
        // Species that cannot originate as-species (can via pre-evo!!!):
        // Wyrdeer, Kleavor, Ursaluna, Sneasler, Overqwil, Basculegion

        return pk.Generation is (< 8 and >= 1); // Transferred from prior game
    }

    /// <summary>
    /// Indicates if the original <see cref="context"/> can randomly infect with Pokérus.
    /// </summary>
    public static bool CanOriginatePokerus(this EntityContext context) => context switch
    {
        EntityContext.Gen1 => false,
        EntityContext.Gen7b => false,
        EntityContext.Gen8a => false,
        EntityContext.Gen9 => false,
        _ => true,
    };

    /// <summary>
    /// Checks if the Pokérus value for Strain is possible to have on the input entity.
    /// </summary>
    /// <param name="pk">Entity to check</param>
    /// <param name="enc">Encounter template matched to</param>
    /// <param name="strain">Strain number</param>
    /// <param name="days">Duration remaining</param>
    /// <returns>True if valid</returns>
    public static bool IsStrainValid(PKM pk, ISpeciesForm enc, int strain, int days)
    {
        if (!IsObtainable(pk, enc))
            return IsSusceptible(strain, days);
        if (pk.Format <= 2)
            return IsStrainValid2(strain);
        return IsStrainValid(strain);
    }

    /// <inheritdoc cref="IsStrainValid(PKM,ISpeciesForm,int,int)"/>
    /// <remarks>
    /// Strains 9+ are not obtainable due to game programming error (jmp label too early).
    /// </remarks>
    public static bool IsStrainValid2(int strain) => strain <= 8;

    /// <inheritdoc cref="IsStrainValid(PKM,ISpeciesForm,int,int)"/>
    /// <remarks>
    /// Gen3 R/S have a 30/255 chance of giving strain 0, and a 1/255 chance of giving strain 8.
    /// Transfers will retain strain 0/8, and they're still able to infect others.
    /// </remarks>
    public static bool IsStrainValid(int strain) => strain <= 0xF;

    /// <summary>
    /// Checks if the Pokérus value for Duration is possible to have on the input entity.
    /// </summary>
    /// <param name="strain">Strain number</param>
    /// <param name="days">Duration remaining</param>
    /// <param name="max">Maximum value permitted</param>
    /// <returns>True if valid</returns>
    public static bool IsDurationValid(int strain, int days, out int max)
    {
        max = GetMaxDuration(strain);
        return (uint)days <= max;
    }

    /// <summary>
    /// Checks if the Pokémon is immune to the Pokérus.
    /// </summary>
    /// <param name="strain">Strain number</param>
    /// <param name="days">Duration remaining</param>
    /// <returns>True if immune (cannot be infected).</returns>
    /// <remarks>An immune Pokémon must have been infected and cured prior to being "immune".</remarks>
    public static bool IsImmune(int strain, int days) => strain != 0 && days == 0;

    /// <summary>
    /// Checks if the Pokémon is currently infected with the Pokérus.
    /// </summary>
    /// <param name="strain">Strain number</param>
    /// <param name="days">Duration remaining</param>
    /// <returns>True if currently infected, and infectious to others.</returns>
    public static bool IsInfectious(int strain, int days) => days != 0;

    /// <summary>
    /// Checks if the Pokémon can be infected with the Pokérus.
    /// </summary>
    /// <param name="strain">Strain number</param>
    /// <param name="days">Duration remaining</param>
    /// <returns>True if it can be infected by another infectious individual.</returns>
    public static bool IsSusceptible(int strain, int days) => strain == 0 && days == 0;

    /// <summary>
    /// Vaccinates the Pokémon, so it will never be infectious in the format it exists in.
    /// </summary>
    /// <param name="pk">Entity to modify.</param>
    /// <param name="enc">Encounter template matched to</param>
    /// <remarks>Overwrites all Pokérus values even if already legal.</remarks>
    public static void Vaccinate(this PKM pk, ISpeciesForm enc)
    {
        pk.PokerusStrain = IsObtainable(pk, enc) ? 1 : 0;
        pk.PokerusDays = 0;
    }
}
