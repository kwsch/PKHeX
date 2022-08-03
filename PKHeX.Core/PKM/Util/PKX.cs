namespace PKHeX.Core;

/// <summary>
/// Common logic for <see cref="PKM"/> data providing and manipulation.
/// </summary>
public static class PKX
{
    internal static readonly IPersonalTable Personal = PersonalTable.LA;
    public const int Generation = 8;
    public const EntityContext Context = EntityContext.Gen8a;
}
