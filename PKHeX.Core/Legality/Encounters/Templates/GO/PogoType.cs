namespace PKHeX.Core;

/// <summary>
/// Encounter Type for various <see cref="GameVersion.GO"/> encounters.
/// </summary>
public enum PogoType : byte
{
    None, // Don't use this.

    /// <summary> Pokémon captured in the wild. </summary>
    Wild,

    /// <summary> Pokémon hatched from 2km, 5km, 7km, or 10km Eggs. </summary>
    Egg,
    /// <summary> Pokémon hatched from Strange Eggs received from the Leaders of Team GO Rocket. </summary>
    EggS,

    /// <summary> Pokémon captured after completing Raid Battles. </summary>
    Raid = 10,
    /// <summary> Mythical Pokémon captured after completing Raid Battles. </summary>
    RaidM,
    /// <summary> Ultra Beasts captured after completing Raid Battles. Only Beast Balls can be used. </summary>
    RaidUB,
    /// <summary> Shadow Pokémon captured after completing Shadow Raid Battles. Must be Purified before transferring to Pokémon HOME. </summary>
    /// <remarks> Pokémon with this <see cref="PogoType"/> can not be moved to <see cref="GameVersion.GG"/>. </remarks>
    RaidS,

    /// <summary> Pokémon captured after completing Field Research. </summary>
    Research = 20,
    /// <summary> Mythical Pokémon captured after completing Field Research. </summary>
    ResearchM,
    /// <summary> Mythical Pokémon captured after completing Field Research. Only Poké Balls can be used. </summary>
    ResearchMP,
    /// <summary> Ultra Beasts captured after completing Field Research. Only Beast Balls can be used. </summary>
    ResearchUB,
    /// <summary> Mythical Pokémon captured after completing Field Research. No HUD is visible during these encounters. </summary>
    /// <remarks>
    /// Under normal circumstances, only Poké Balls can be used, but Great Balls and Ultra Balls can be used with the Remember Last-Used Poké Ball setting.
    /// This was rendered unusable as of version 0.277.3.
    /// </remarks>
    ResearchMH,
    /// <summary> Pokémon captured after completing Field Research. No HUD is visible during these encounters. </summary>
    /// <remarks>
    /// The encounter defaults to the player's stock of Poké Balls. If they have none, it falls back to Great Balls, and then to Ultra Balls.
    /// If the player has no Poké Balls, Great Balls, or Ultra Balls, the HUD fails to load in any Poké Ball at all, even if they have a Master Ball.
    /// </remarks>
    ResearchNH,

    /// <summary> Pokémon captured from the GO Battle League. </summary>
    GBL = 30,
    /// <summary> Mythical Pokémon captured from the GO Battle League. </summary>
    GBLM,
    /// <summary> Pokémon captured from the GO Battle League during GO Battle Day events. Excludes Legendary Pokémon, Mythical Pokémon, and Ultra Beasts. </summary>
    GBLD,

    /// <summary> Pokémon captured after defeating members of Team GO Rocket. Must be Purified before transferring to Pokémon HOME. </summary>
    /// <remarks> Pokémon with this <see cref="PogoType"/> can not be moved to <see cref="GameVersion.GG"/>. </remarks>
    Shadow = 40,

    /// <summary> Pokémon captured from Special Research or Timed Research with a Premier Ball. </summary>
    /// <remarks>
    /// Niantic released version 0.269.0 on April 22, 2023, which contained an issue with the Remember Last-Used Poké Ball setting.
    /// This allowed for Premier Balls obtained from Raid Battles to be remembered on all future encounters.
    /// The moment the Premier Ball touched the floor or a wild Pokémon, the encounter would end, except if it was from a Special Research, Timed Research, or Collection Challenge encounter.
    /// This made it possible for over 300 species of Pokémon to be obtainable in a Poké Ball they were never meant to be captured in.
    /// This bug was fixed with the release of version 0.269.2.
    /// </remarks>
    Research269 = 200,
    Research269M,
}

/// <summary>
/// Extension methods for <see cref="PogoType"/>.
/// </summary>
public static class PogoTypeExtensions
{
    /// <summary>
    /// Gets the minimum level (relative to GO's 1-<see cref="EncountersGO.MAX_LEVEL"/>) the <see cref="encounterType"/> must have.
    /// </summary>
    /// <param name="encounterType">Descriptor indicating how the Pokémon was encountered in GO.</param>
    public static byte GetMinLevel(this PogoType encounterType) => encounterType switch
    {
        PogoType.EggS => 8,
        PogoType.Raid => 20,
        PogoType.RaidM => 20,
        PogoType.RaidUB => 20,
        PogoType.RaidS => 20,
        PogoType.Research => 15,
        PogoType.ResearchM => 15,
        PogoType.ResearchMP => 15,
        PogoType.ResearchUB => 15,
        PogoType.ResearchMH => 15,
        PogoType.ResearchNH => 15,
        PogoType.GBL => 20,
        PogoType.GBLM => 20,
        PogoType.GBLD => 20,
        PogoType.Shadow => 8,
        PogoType.Research269 => 15,
        PogoType.Research269M => 15,
        _ => 1, // Wild, Egg
    };

    /// <summary>
    /// Gets the minimum IVs (relative to GO's 0-15) the <see cref="encounterType"/> must have.
    /// </summary>
    /// <param name="encounterType">Descriptor indicating how the Pokémon was encountered in GO.</param>
    /// <returns>Required minimum IV (0-15)</returns>
    public static int GetMinIV(this PogoType encounterType) => encounterType switch
    {
        PogoType.Wild => 0,
        PogoType.RaidM => 10,
        PogoType.ResearchM => 10,
        PogoType.ResearchMP => 10,
        PogoType.ResearchMH => 10,
        PogoType.GBLM => 10,
        PogoType.GBLD => 0,
        PogoType.Research269M => 10,
        _ => 1,
    };

    /// <summary>
    /// Checks if the <see cref="ball"/> is valid for the <see cref="encounterType"/>.
    /// </summary>
    /// <param name="encounterType">Descriptor indicating how the Pokémon was encountered in GO.</param>
    /// <param name="ball">Current <see cref="Ball"/> the Pokémon is in.</param>
    /// <returns>True if valid, false if invalid.</returns>
    public static bool IsBallValid(this PogoType encounterType, Ball ball)
    {
        var req = encounterType.GetValidBall();
        if (req == Ball.None)
            return (uint)(ball - 2) <= 2; // Poke, Great, Ultra
        return ball == req;
    }

    /// <summary>
    /// Checks if <see cref="Ball.Master"/> can be used for the <see cref="encounterType"/>.
    /// </summary>
    /// <param name="encounterType">Descriptor indicating how the Pokémon was encountered in GO.</param>
    /// <returns>True if valid, false if invalid.</returns>
    public static bool IsMasterBallUsable(this PogoType encounterType) => encounterType switch
    {
        PogoType.Egg or PogoType.EggS  => false,
        PogoType.ResearchMP or PogoType.ResearchUB or PogoType.ResearchMH or PogoType.ResearchNH  => false,
        _ => true,
    };

    /// <summary>
    /// Gets a valid ball that the <see cref="encounterType"/> can have based on the type of capture in Pokémon GO.
    /// </summary>
    /// <param name="encounterType">Descriptor indicating how the Pokémon was encountered in GO.</param>
    /// <returns><see cref="Ball.None"/> if no specific ball is required, otherwise returns the required ball.</returns>
    public static Ball GetValidBall(this PogoType encounterType) => encounterType switch
    {
        PogoType.Egg => Ball.Poke,
        PogoType.EggS => Ball.Poke,
        PogoType.Raid => Ball.Premier,
        PogoType.RaidM => Ball.Premier,
        PogoType.RaidUB => Ball.Beast,
        PogoType.RaidS => Ball.Premier,
        PogoType.ResearchMP => Ball.Poke,
        PogoType.ResearchUB => Ball.Beast,
        PogoType.Shadow => Ball.Premier,
        PogoType.Research269 => Ball.Premier,
        PogoType.Research269M => Ball.Premier,
        _ => Ball.None, // Poke, Great, Ultra
    };
}
