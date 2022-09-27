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

    /// <summary> Pokémon captured after completing Field Research. </summary>
    Research = 20,
    /// <summary> Mythical Pokémon captured after completing Field Research. </summary>
    ResearchM,
    /// <summary> Mythical Pokémon captured after completing Field Research. Only Poké Balls can be used. </summary>
    ResearchP,
    /// <summary> Ultra Beasts captured after completing Field Research. Only Beast Balls can be used. </summary>
    ResearchUB,

    /// <summary> Pokémon captured from the GO Battle League. </summary>
    GBL = 30,
    /// <summary> Mythical Pokémon captured from the GO Battle League. </summary>
    GBLM,
    /// <summary> Pokémon captured from the GO Battle League during GO Battle Day, excluding Legendary and Mythical Pokémon. </summary>
    GBLD,

    /// <summary> Pokémon captured after defeating members of Team GO Rocket. Must become Purified before transferring to Pokémon HOME. </summary>
    /// <remarks> Pokémon with this <see cref="PogoType"/> can not be moved to <see cref="GameVersion.GG"/>. </remarks>
    Shadow = 40,
}

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
        PogoType.Research => 15,
        PogoType.ResearchM => 15,
        PogoType.ResearchP => 15,
        PogoType.ResearchUB => 15,
        PogoType.GBL => 20,
        PogoType.GBLM => 20,
        PogoType.GBLD => 20,
        PogoType.Shadow => 8,
        _ => 1,
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
        PogoType.ResearchP => 10,
        PogoType.GBLM => 10,
        PogoType.GBLD => 0,
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
        PogoType.ResearchP => Ball.Poke,
        PogoType.ResearchUB => Ball.Beast,
        PogoType.Shadow => Ball.Premier,
        _ => Ball.None, // Poke, Great, Ultra
    };
}
