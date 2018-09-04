namespace PKHeX.Core
{
    /// <summary>
    /// Indicates if the <see cref="PKM"/> is required to be traded between Generation 1/2 saves.
    /// </summary>
    /// <remarks>Used for only Generation 1/2 data.</remarks>
    public enum TradebackType
    {
        /// <summary>
        /// Information can originate from either generation without restrictions.
        /// </summary>
        Any,

        /// <summary>
        /// Information can only originate from Gen1.
        /// </summary>
        Gen1_NotTradeback,

        /// <summary>
        /// Information can only originate from Gen2.
        /// </summary>
        Gen2_NotTradeback,

        /// <summary>
        /// Information can only exist if has visited both Gen1 and Gen2.
        /// </summary>
        WasTradeback
    }
}
