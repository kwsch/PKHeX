namespace PKHeX.Core;

/// <summary>
/// Holds the ability and ability number for a <see cref="PKM"/> that has side-game specific data.
/// </summary>
public interface IGameDataSplitAbility
{
    /// <summary>
    /// Ability ID for the <see cref="PKM"/>.
    /// </summary>
    ushort Ability { get; set; }

    /// <summary>
    /// Ability number for the <see cref="PKM"/>.
    /// </summary>
    byte AbilityNumber { get; set; }
}
