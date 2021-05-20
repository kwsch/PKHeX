using static PKHeX.Core.PIDType;

namespace PKHeX.Core
{
    /// <summary>
    /// PID + IV correlation.
    /// </summary>
    /// <remarks>This is just a catch-all enumeration to describe the different correlations.</remarks>
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
        /// <remarks><see cref="RNG.LCRNG"/> seed is clamped to 16bits.</remarks>
        BACD_R,

        /// <summary>
        /// Event Reversed Order PID without Origin Seed restrictions
        /// </summary>
        /// <remarks><see cref="RNG.LCRNG"/></remarks>
        BACD_U,

        /// <summary>
        /// Event Reversed Order PID restricted to 16bit Origin Seed, antishiny.
        /// </summary>
        /// <remarks><see cref="RNG.LCRNG"/> seed is clamped to 16bits.</remarks>
        BACD_R_A,

        /// <summary>
        /// Event Reversed Order PID without Origin Seed restrictions, antishiny.
        /// </summary>
        /// <remarks><see cref="RNG.LCRNG"/></remarks>
        BACD_U_A,

        /// <summary>
        /// Event Reversed Order PID restricted to 8bit Origin Seed, shiny
        /// </summary>
        /// <remarks><see cref="RNG.LCRNG"/> seed is clamped to 16bits.</remarks>
        BACD_R_S,

        /// <summary>
        /// Event Reversed Order PID without Origin Seed restrictions, shiny
        /// </summary>
        /// <remarks><see cref="RNG.LCRNG"/></remarks>
        BACD_U_S,

        /// <summary>
        /// Event Reversed Order PID restricted to 16bit Origin Seed, antishiny (nyx)
        /// </summary>
        /// <remarks><see cref="RNG.LCRNG"/> seed is clamped to 16bits.</remarks>
        BACD_R_AX,

        /// <summary>
        /// Event Reversed Order PID without Origin Seed restrictions, antishiny (nyx)
        /// </summary>
        /// <remarks><see cref="RNG.LCRNG"/></remarks>
        BACD_U_AX,

        /// <summary>
        /// Generation 4 Cute Charm PID, which is forced to an 8 bit PID value based on the gender &amp; gender ratio value.
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
        /// Generation 3 <see cref="GameVersion.CXD"/> PID+IV correlation.
        /// </summary>
        /// <remarks><see cref="RNG.XDRNG"/></remarks>
        CXD,

        /// <summary>
        /// Generation 3 <see cref="GameVersion.CXD"/> PID+IV correlation that was rerolled because it was shiny.
        /// </summary>
        /// <remarks><see cref="RNG.XDRNG"/></remarks>
        CXDAnti,

        /// <summary>
        /// Generation 3 <see cref="GameVersion.CXD"/> PID+IV which is created immediately after the TID and SID RNG calls.
        /// </summary>
        /// <remarks><see cref="RNG.XDRNG"/>. The second starter is created after the first starter, with the same TID and SID.</remarks>
        CXD_ColoStarter,

        /// <summary>
        /// Generation 3 Pokémon Channel Jirachi
        /// </summary>
        /// <remarks><see cref="RNG.XDRNG"/></remarks>
        Channel,

        /// <summary>
        /// Generation 3 <see cref="GameVersion.CXD"/> PokeSpot PID
        /// </summary>
        /// <remarks><see cref="RNG.XDRNG"/></remarks>
        PokeSpot,

        #endregion

        #region ARNG

        /// <summary>
        /// Generation 4 Mystery Gift Anti-Shiny
        /// </summary>
        /// <remarks><see cref="RNG.ARNG"/></remarks>
        G4MGAntiShiny,

        #endregion

        #region Formulaic

        /// <summary>
        /// Generation 5 Mystery Gift Shiny
        /// </summary>
        /// <remarks>Formulaic based on TID, SID, and Gender bytes.</remarks>
        /// <remarks>Unrelated to IVs</remarks>
        G5MGShiny,

        /// <summary>
        /// Generation 4 Pokewalker PID, never Shiny.
        /// </summary>
        /// <remarks>Formulaic based on TID, SID, and Gender bytes.</remarks>
        /// <remarks>Unrelated to IVs</remarks>
        Pokewalker,

        /// <summary>
        /// Generation 8 Raid PID
        /// </summary>
        /// <remarks>Formulaic based on PID &amp; EC values from a 64bit-seed.</remarks>
        Raid8,

        /// <summary>
        /// Generation 8 Overworld Spawn PID
        /// </summary>
        /// <remarks>Formulaic based on PID &amp; EC values from a 32bit-seed.</remarks>
        Overworld8,

        #endregion
    }

#if DEBUG
    public static class PIDTypeExtensions
    {
        public static RNGType GetRNGType(this PIDType type) => type switch
        {
            0 => RNGType.None,
            <= ChainShiny => RNGType.LCRNG,
            <= PokeSpot => RNGType.XDRNG,
            G4MGAntiShiny => RNGType.ARNG,
            _ => RNGType.None,
        };

        public static bool IsReversedPID(this PIDType type) => type switch
        {
            CXD or CXDAnti => true,
            BACD_R or BACD_R_A or BACD_R_S => true,
            BACD_U or BACD_U_A or BACD_U_S => true,
            Method_1_Unown or Method_2_Unown or Method_3_Unown or Method_4_Unown => true,
            _ => false
        };
    }
#endif
}
