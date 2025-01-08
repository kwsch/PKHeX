namespace PKHeX.Core;

/// <summary>
/// Latest game values for <see cref="PKM"/> data providing and manipulation.
/// </summary>
public static class PKX
{
    public const EntityContext Context = EntityContext.Gen9;
    public const GameVersion Version = GameVersion.SL;
    public const byte Generation = 9;

    // We need to expose gender info for backwards compatibility / arbitrary game logic.

    /// <summary>
    /// Gets the gender details for the requested species (base form).
    /// </summary>
    /// <returns>Gender details for the personal info.</returns>
    internal static IGenderDetail GetGenderDetail(ushort species)
        => PersonalTable.SV[species];

    /// <summary>
    /// Gets the magic gender value used in past generations (3-8) for logic checks.
    /// </summary>
    internal static byte GetGenderRatio(ushort species)
        => PersonalTable.SV[species].Gender;
}
