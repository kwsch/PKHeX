namespace PKHeX.Core
{
    public enum PIDType
    {
        /// <summary> No relationship between the PID and IVs </summary>
        None,

        #region LCRNG

        /// <summary> Method 1 Variants (H1/J/K) </summary>
        /// <remarks><see cref="RNG.LCRNG"/></remarks>
        Method_1,

        /// <summary> Method H2 </summary>
        /// <remarks><see cref="RNG.LCRNG"/></remarks>
        Method_2,

        /// <summary> Method H3 </summary>
        /// <remarks><see cref="RNG.LCRNG"/></remarks>
        Method_3,

        /// <summary> Method H4 </summary>
        /// <remarks><see cref="RNG.LCRNG"/></remarks>
        Method_4,

        /// <summary> Method H1_Unown (FRLG) </summary>
        /// <remarks><see cref="RNG.LCRNG"/></remarks>
        Method_1_Unown,

        /// <summary> Method H2_Unown (FRLG) </summary>
        /// <remarks><see cref="RNG.LCRNG"/></remarks>
        Method_2_Unown,

        /// <summary> Method H3_Unown (FRLG) </summary>
        /// <remarks><see cref="RNG.LCRNG"/></remarks>
        Method_3_Unown,

        /// <summary> Method H4_Unown (FRLG) </summary>
        /// <remarks><see cref="RNG.LCRNG"/></remarks>
        Method_4_Unown,

        /// <summary>  Method 1 Roamer (Gen3) </summary>
        /// <remarks><see cref="RNG.LCRNG"/></remarks>
        Method_1_Roamer,

        /// <summary>
        /// Event Reversed Order PID restricted to 16bit Origin Seed
        /// </summary>
        /// <remarks><see cref="RNG.LCRNG"/></remarks>
        BACD_R,

        /// <summary>
        /// Event Reversed Order PID without Origin Seed restrictions
        /// </summary>
        /// <remarks><see cref="RNG.LCRNG"/></remarks>
        BACD_U,

        /// <summary>
        /// Event Reversed Order PID restricted to 16bit Origin Seed, antishiny.
        /// </summary>
        /// <remarks><see cref="RNG.LCRNG"/></remarks>
        BACD_R_A,

        /// <summary>
        /// Event Reversed Order PID without Origin Seed restrictions, antishiny.
        /// </summary>
        /// <remarks><see cref="RNG.LCRNG"/></remarks>
        BACD_U_A,

        /// <summary>
        /// Event Reversed Order PID restricted to 8bit Origin Seed, shiny
        /// </summary>
        /// <remarks><see cref="RNG.LCRNG"/></remarks>
        BACD_R_S,

        /// <summary>
        /// Event Reversed Order PID without Origin Seed restrictions, shiny
        /// </summary>
        /// <remarks><see cref="RNG.LCRNG"/></remarks>
        BACD_U_S,

        /// <summary>
        /// Event Reversed Order PID restricted to 16bit Origin Seed, antishiny (nyx)
        /// </summary>
        /// <remarks><see cref="RNG.LCRNG"/></remarks>
        BACD_R_AX,

        /// <summary>
        /// Event Reversed Order PID without Origin Seed restrictions, antishiny (nyx)
        /// </summary>
        /// <remarks><see cref="RNG.LCRNG"/></remarks>
        BACD_U_AX,

        /// <summary>
        /// Generation 4 Cute Charm forced to an 8 bit buffered PID
        /// </summary>
        /// <remarks><see cref="RNG.LCRNG"/></remarks>
        CuteCharm,

        /// <summary>
        /// Generation 4 Chained Shiny
        /// </summary>
        /// <remarks><see cref="RNG.LCRNG"/></remarks>
        ChainShiny,

        #endregion

        #region XDRNG

        /// <summary>
        /// Standard <see cref="GameVersion.CXD"/> PIDIV
        /// </summary>
        /// <remarks><see cref="RNG.XDRNG"/></remarks>
        CXD,

        /// <summary>
        /// Antishiny Rerolled <see cref="GameVersion.CXD"/> PIDIV
        /// </summary>
        /// <remarks><see cref="RNG.XDRNG"/></remarks>
        CXDAnti,

        /// <summary>
        /// Standard <see cref="GameVersion.CXD"/> PIDIV which is immediately after the RNG calls that create the TID and SID.
        /// </summary>
        /// <remarks><see cref="RNG.XDRNG"/></remarks>
        CXD_ColoStarter,

        /// <summary>
        /// Pokémon Channel Jirachi
        /// </summary>
        /// <remarks><see cref="RNG.XDRNG"/></remarks>
        Channel,

        /// <summary>
        /// XD PokeSpot PID
        /// </summary>
        /// <remarks><see cref="RNG.XDRNG"/></remarks>
        PokeSpot,

        #endregion

        #region ARNG

        /// <summary>
        /// 4th Generation Mystery Gift Anti-Shiny
        /// </summary>
        /// <remarks><see cref="RNG.ARNG"/></remarks>
        G4MGAntiShiny,

        #endregion

        #region Formulaic

        /// <summary>
        /// 5th Generation Mystery Gift Shiny
        /// </summary>
        /// <remarks>Formulaic based on TID, SID, and Gender bytes.</remarks>
        /// <remarks>Unrelated to IVs</remarks>
        G5MGShiny,

        /// <summary>
        /// 4th Generation Pokewalker PID, never Shiny.
        /// </summary>
        /// <remarks>Formulaic based on TID, SID, and Gender bytes.</remarks>
        /// <remarks>Unrelated to IVs</remarks>
        Pokewalker,

        #endregion
    }

    public static class PIDTypeExtensions
    {
        public static RNGType GetRNGType(this PIDType type) => type switch
        {
            0 => RNGType.None,
            <= PIDType.ChainShiny => RNGType.LCRNG,
            <= PIDType.PokeSpot => RNGType.XDRNG,
            PIDType.G4MGAntiShiny => RNGType.ARNG,
            _ => RNGType.None,
        };
    }
}
