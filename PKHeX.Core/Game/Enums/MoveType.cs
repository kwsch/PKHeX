namespace PKHeX.Core
{
    /// <summary>
    /// Elemental type a move has; additionally, types a <see cref="PKM"/> can have.
    /// </summary>
    public enum MoveType : sbyte
    {
        Any = -1,
        Normal,
        Fighting,
        Flying,
        Poison,
        Ground,
        Rock,
        Bug,
        Ghost,
        Steel,
        Fire,
        Water,
        Grass,
        Electric,
        Psychic,
        Ice,
        Dragon,
        Dark,
        Fairy,
    }

    public static class MoveTypeExtensions
    {
        public static MoveType GetMoveTypeGeneration(this MoveType type, int generation)
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
}
