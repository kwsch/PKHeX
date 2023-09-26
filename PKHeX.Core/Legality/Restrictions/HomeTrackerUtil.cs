namespace PKHeX.Core;

/// <summary>
/// Logic for determining if a <see cref="IHomeTrack.Tracker"/> value is required for a given encounter.
/// </summary>
public static class HomeTrackerUtil
{
    /// <summary>
    /// Indicates if a <see cref="IHomeTrack.Tracker"/> value is required for the given encounter.
    /// </summary>
    /// <param name="enc">Encounter source</param>
    /// <param name="pk">Entity to check</param>
    /// <returns>True if the encounter must have a <see cref="IHomeTrack.Tracker"/> value</returns>
    public static bool IsRequired(IEncounterTemplate enc, PKM pk) => IsRequired(enc, pk.Context);

    /// <inheritdoc cref="IsRequired(IEncounterTemplate,PKM)"/>
    public static bool IsRequired(IEncounterTemplate enc, EntityContext current)
    {
        if (IsRequired(current, enc.Context))
            return true;
        if (IsRequired(enc))
            return true;
        return false;
    }

    /// <inheritdoc cref="IsRequired(IEncounterTemplate,PKM)"/>
    public static bool IsRequired(EntityContext current, EntityContext origin) => origin != current;

    /// <inheritdoc cref="IsRequired(IEncounterTemplate,PKM)"/>
    /// <remarks>
    /// Encounters that originate in HOME -> transfer to save data
    /// </remarks>
    public static bool IsRequired(IEncounterTemplate enc) => enc switch
    {
        EncounterSlot8GO => true,
        WC8 { IsHOMEGift: true } => true,
        WB8 { IsHOMEGift: true } => true,
        WA8 { IsHOMEGift: true } => true,
        WC9 { IsHOMEGift: true } => true,
        _ => enc.Generation < 8,
    };
}
