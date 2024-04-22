namespace PKHeX.Core;

/// <summary>
/// Exposes info about encounter details that an entity has when encountered.
/// </summary>
public interface IPersonalEncounter
{
    /// <summary>
    /// Base Experience Yield factor
    /// </summary>
    int BaseEXP { get; set; }

    /// <summary>
    /// Amount of Hatching Step Cycles required to hatch if in an egg.
    /// </summary>
    byte HatchCycles { get; set; }

    /// <summary>
    /// Catch Rate
    /// </summary>
    byte CatchRate { get; set; }

    /// <summary>
    /// Initial Friendship when captured or received.
    /// </summary>
    byte BaseFriendship { get; set; }

    /// <summary>
    /// Escape factor used for fleeing the Safari Zone or calling for help in SOS Battles.
    /// </summary>
    int EscapeRate { get; set; }
}
