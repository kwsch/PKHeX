namespace PKHeX.Core;

/// <summary>
/// Latest game values for <see cref="PKM"/> data providing and manipulation.
/// </summary>
public static class PKX
{
    internal static IPersonalTable Personal => PersonalTable.SV;
    public const EntityContext Context = EntityContext.Gen9;
    public const GameVersion Version = GameVersion.SL;
    public const byte Generation = 9;
}
