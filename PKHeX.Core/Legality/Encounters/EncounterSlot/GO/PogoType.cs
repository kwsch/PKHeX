namespace PKHeX.Core
{
    /// <summary>
    /// Encounter Type for various <see cref="GameVersion.GO"/> encounters.
    /// </summary>
    public enum PogoType : byte
    {
        None, // Don't use this.

        Wild,
        Egg,

        /// <summary> Raid Boss, requires Lv. 15 and IV=1 </summary>
        Raid15 = 10,
        /// <summary> Raid Boss, requires Lv. 20 and IV=10 </summary>
        Raid20,

        /// <summary> Field Research, requires Lv. 15 and IV=1 </summary>
        Field15 = 20,
        /// <summary> Field Research, requires Lv. 15 and IV=10 (Mythicals) </summary>
        FieldM,
        /// <summary> Field Research, requires Lv. 15 and IV=10 (Mythicals, Poké Ball only) </summary>
        FieldP,
        /// <summary> Field Research, requires Lv. 20 and IV=10 (GBL Mythicals) </summary>
        Field20,

        /// <summary> Purified, requires Lv. 8 and IV=1 (Premier Ball) </summary>
        Shadow = 30,
        /// <summary> Purified, requires Lv. 8 and IV=1 (No Premier Ball) </summary>
        ShadowPGU,
    }

    public static class PogoTypeExtensions
    {
        /// <summary>
        /// Gets the minimum level (relative to GO's 1-<see cref="EncountersGO.MAX_LEVEL"/>) the <see cref="encounterType"/> must have.
        /// </summary>
        /// <param name="encounterType">Descriptor indicating how the Pokémon was encountered in GO.</param>
        public static int GetMinLevel(this PogoType encounterType) => encounterType switch
        {
            PogoType.Raid15 => 15,
            PogoType.Raid20 => 20,
            PogoType.Field15 => 15,
            PogoType.FieldM => 15,
            PogoType.Field20 => 20,
            PogoType.Shadow => 8,
            PogoType.ShadowPGU => 8,
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
            PogoType.Raid20 => 10,
            PogoType.FieldM => 10,
            PogoType.FieldP => 10,
            PogoType.Field20 => 10,
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
            PogoType.FieldP => Ball.Poke,
            PogoType.Raid15 => Ball.Premier,
            PogoType.Raid20 => Ball.Premier,
            _ => Ball.None, // Poke, Great, Ultra
        };
    }
}
