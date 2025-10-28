namespace PKHeX.Core;

/// <summary>
/// Elemental type a move has; additionally, types a <see cref="PKM"/> can have.
/// </summary>
public enum MoveType : sbyte
{
    Any = -1,
    Normal = 0,
    Fighting = 1,
    Flying = 2,
    Poison = 3,
    Ground = 4,
    Rock = 5,
    Bug = 6,
    Ghost = 7,
    Steel = 8,
    Fire = 9,
    Water = 10,
    Grass = 11,
    Electric = 12,
    Psychic = 13,
    Ice = 14,
    Dragon = 15,
    Dark = 16,
    Fairy = 17,
}

/// <summary>
/// Extension methods for <see cref="MoveType"/>.
/// </summary>
public static class MoveTypeExtensions
{
    public static MoveType GetMoveTypeGeneration(this MoveType type, byte generation)
    {
        if (generation <= 2)
            return GetMoveTypeFromG12(type);
        return type;
    }

    private static MoveType GetMoveTypeFromG12(this MoveType type)
    {
        if (type <= MoveType.Rock)
            return type;
        type--; // Skip unused Bird type
        if (type <= MoveType.Steel)
            return type;
        type -= 10; // 10 Normal duplicates
        return type;
    }
}
