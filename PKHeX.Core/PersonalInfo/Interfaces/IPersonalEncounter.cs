namespace PKHeX.Core;

public interface IPersonalEncounter
{
    /// <summary>
    /// Base Experience Yield factor
    /// </summary>
    int BaseEXP { get; set; }

    /// <summary>
    /// Amount of Hatching Step Cycles required to hatch if in an egg.
    /// </summary>
    int HatchCycles { get; set; }

    /// <summary>
    /// Catch Rate
    /// </summary>
    int CatchRate { get; set; }

    /// <summary>
    /// Initial Friendship when captured or received.
    /// </summary>
    int BaseFriendship { get; set; }

    /// <summary>
    /// Escape factor used for fleeing the Safari Zone or calling for help in SOS Battles.
    /// </summary>
    int EscapeRate { get; set; }
}
