namespace PKHeX.Core
{
    public enum PIDType
    {
        /// <summary> No match </summary>
        None,

        /// <summary> Method 1 Variants (H1/J/K) </summary>
        Method_1,
        /// <summary> Method H2 </summary>
        Method_2,
        /// <summary> Method H4 </summary>
        Method_4,
        /// <summary> Method H1_Unown (FRLG) </summary>
        Method_1_Unown,
        /// <summary> Method H2_Unown (FRLG) </summary>
        Method_2_Unown,
        /// <summary> Method H4_Unown (FRLG) </summary>
        Method_4_Unown,
        /// <summary>  Method 1 Roamer (Gen3) </summary>
        Method_1_Roamer,

        /// <summary>
        /// Event Reversed Order PID restricted to 16bit Origin Seed
        /// </summary>
        BACD_R,
        /// <summary>
        /// Event Reversed Order PID without Origin Seed restrictions
        /// </summary>
        BACD_U,
        /// <summary>
        /// Event Reversed Order PID restricted to 16bit Origin Seed, antishiny.
        /// </summary>
        BACD_R_A,
        /// <summary>
        /// Event Reversed Order PID without Origin Seed restrictions, antishiny.
        /// </summary>
        BACD_U_A,
        /// <summary>
        /// Event Reversed Order PID restricted to 16bit Origin Seed, shiny
        /// </summary>
        BACD_R_S,
        /// <summary>
        /// Event Reversed Order PID without Origin Seed restrictions, shiny
        /// </summary>
        BACD_U_S,
        /// <summary>
        /// Event Reversed Order PID restricted to 16bit Origin Seed, antishiny (nyx)
        /// </summary>
        BACD_R_AX,
        /// <summary>
        /// Event Reversed Order PID without Origin Seed restrictions, antishiny (nyx)
        /// </summary>
        BACD_U_AX,

        /// <summary>
        /// Generation 4 Cute Charm forced 8 bit
        /// </summary>
        CuteCharm,
        /// <summary>
        /// Generation 4 Chained Shiny
        /// </summary>
        ChainShiny,

        // XDRNG Based
        CXD,
        Channel,
        PokeSpot,

        // ARNG Based
        G4MGAntiShiny,

        // Formulaic
        G5MGShiny,
        Pokewalker,

        // Specified
        Static,
    }
}
