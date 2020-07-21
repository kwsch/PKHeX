namespace PKHeX.Core
{
    /// <summary>
    /// Message enumeration indicating why a Write Operation would be blocked for a given <see cref="ISlotInfo"/>
    /// </summary>
    public enum WriteBlockedMessage
    {
        None,
        InvalidPartyConfiguration,
        IncompatibleFormat,
        InvalidDestination,
    }
}