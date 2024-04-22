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
    /// <returns>True if Pokérus exists in the game format, or can be transmitted to the entity via another game.</returns>
    public static bool IsObtainable(PKM pk) => pk switch
    {
        PA8 pa8 => HasVisitedAnother(pa8),
        PB7 => false,
        PK9 => false,
        _ => true,
    };

    private static bool HasVisitedAnother(PA8 pk)
    {
        if (pk.IsUntraded)
            return false;
        if (pk.Tracker == 0)
            return false;
        if (PersonalTable.BDSP.IsPresentInGame(pk.Species, pk.Form))
            return true;
        if (PersonalTable.SWSH.IsPresentInGame(pk.Species, pk.Form))
            return true;
        return pk.Generation is (< 8 and >= 1); // Transferred from prior game
    }

    /// <summary>
    /// Checks if the Pokérus value for Strain is possible to have on the input entity.
    /// </summary>
    /// <param name="pk">Entity to check</param>
    /// <param name="strain">Strain number</param>
    /// <param name="days">Duration remaining</param>
    /// <returns>True if valid</returns>
    public static bool IsStrainValid(PKM pk, int strain, int days)
    {
        if (!IsObtainable(pk))
            return IsSusceptible(strain, days);
        if (pk.Format <= 2)
            return IsStrainValid2(strain);
        return IsStrainValid(strain);
    }

    /// <inheritdoc cref="IsStrainValid(PKM,int,int)"/>
    /// <remarks>
    /// Strains 9+ are not obtainable due to game programming error (jmp label too early).
    /// </remarks>
    public static bool IsStrainValid2(int strain) => strain <= 8;

    /// <inheritdoc cref="IsStrainValid(PKM,int,int)"/>
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
    /// <remarks>Overwrites all Pokérus values even if already legal.</remarks>
    public static void Vaccinate(this PKM pk)
    {
        pk.PokerusStrain = IsObtainable(pk) ? 1 : 0;
        pk.PokerusDays = 0;
    }
}
