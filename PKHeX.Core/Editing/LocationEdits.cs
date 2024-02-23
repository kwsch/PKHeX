namespace PKHeX.Core;

/// <summary>
/// Logic for locations of a <see cref="PKM"/>.
/// </summary>
public static class LocationEdits
{
    /// <summary>
    /// Gets the "None" location index for a specific <see cref="PKM"/> context.
    /// </summary>
    public static ushort GetNoneLocation(PKM pk) => GetNoneLocation(pk.Context);

    /// <summary>
    /// Gets the "None" location index for a specific <see cref="PKM"/> context.
    /// </summary>
    public static ushort GetNoneLocation(EntityContext context) => context switch
    {
        EntityContext.Gen8b => Locations.Default8bNone,
        _ => 0,
    };
}
