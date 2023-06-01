namespace PKHeX.Core;

/// <summary>
/// Ball IDs for the corresponding English ball name.
/// </summary>
public enum Ball : byte
{
    None = 0,

    Master = 1,
    Ultra = 2,
    Great = 3,
    Poke = 4,

    Safari = 5,

    Net = 6,
    Dive = 7,
    Nest = 8,
    Repeat = 9,
    Timer = 10,
    Luxury = 11,
    Premier = 12,
    Dusk = 13,
    Heal = 14,
    Quick = 15,

    Cherish = 16,

    Fast = 17,
    Level = 18,
    Lure = 19,
    Heavy = 20,
    Love = 21,
    Friend = 22,
    Moon = 23,

    Sport = 24,
    Dream = 25,
    Beast = 26,

    // Legends: Arceus
    Strange = 27,
    LAPoke = 28,
    LAGreat = 29,
    LAUltra = 30,
    LAFeather = 31,
    LAWing = 32,
    LAJet = 33,
    LAHeavy = 34,
    LALeaden = 35,
    LAGigaton = 36,
    LAOrigin = 37,
}

/// <summary>
/// Extension methods for <see cref="Ball"/>.
/// </summary>
public static class BallExtensions
{
    /// <summary>
    /// Checks if the <see cref="ball"/> is an Apricorn Ball (HG/SS)
    /// </summary>
    /// <param name="ball">Ball ID</param>
    /// <returns>True if Apricorn, false if not.</returns>
    public static bool IsApricornBall(this Ball ball) => ball is >= Ball.Fast and <= Ball.Moon;

    public static bool IsLegendBall(this Ball ball) => ball is >= Ball.LAPoke and <= Ball.LAOrigin;
}
