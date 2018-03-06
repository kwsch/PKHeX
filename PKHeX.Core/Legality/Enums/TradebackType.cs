namespace PKHeX.Core
{
    /// <summary>
    /// Indicates if the <see cref="PKM"/> is required to be traded between Generation 1/2 saves.
    /// </summary>
    /// <remarks>Used for only Generation 1/2 data.</remarks>
    public enum TradebackType
    {
        Any,
        Gen1_NotTradeback,
        Gen2_NotTradeback,
        WasTradeback
    }
}
