namespace PKHeX.Core;

/// <summary>
/// Logic for determining the hidden potential (overall IV grade) of a Pokémon.
/// </summary>
public static class PowerPotential
{
    /// <summary>
    /// Gets the Potential evaluation of the input <see cref="ivTotal"/>.
    /// </summary>
    public static int GetPotential(int ivTotal) => ivTotal switch
    {
        <= 90 => 0,
        <= 120 => 1,
        <= 150 => 2,
        _ => 3,
    };

    private static string GetPotentialUnicode(int rating) => rating switch
    {
        0 => "★☆☆☆",
        1 => "★★☆☆",
        2 => "★★★☆",
        _ => "★★★★",
    };

    private static string GetPotentialASCII(int rating) => rating switch
    {
        0 => "+",
        1 => "++",
        2 => "+++",
        _ => "++++",
    };

    /// <summary>
    /// Gets the Potential evaluation of the input <see cref="pk"/>.
    /// </summary>
    /// <param name="pk">Pokémon to analyze.</param>
    /// <param name="unicode">Returned value is unicode or not</param>
    /// <returns>Potential string</returns>
    public static string GetPotentialString(this PKM pk, bool unicode = true)
    {
        var rating = pk.PotentialRating;
        if (unicode)
            return GetPotentialUnicode(rating);
        return GetPotentialASCII(rating);
    }
}
