namespace PKHeX.Core;

/// <summary>
/// Exposes details about yielding effort values on defeat/capture.
/// </summary>
public interface IEffortValueYield
{
    /// <summary>
    /// Amount of HP Effort Values to yield when defeating this entry.
    /// </summary>
    int EV_HP { get; set; }

    /// <summary>
    /// Amount of Attack Effort Values to yield when defeating this entry.
    /// </summary>
    int EV_ATK { get; set; }

    /// <summary>
    /// Amount of Defense Effort Values to yield when defeating this entry.
    /// </summary>
    int EV_DEF { get; set; }

    /// <summary>
    /// Amount of Speed Effort Values to yield when defeating this entry.
    /// </summary>
    int EV_SPE { get; set; }

    /// <summary>
    /// Amount of Special Attack Effort Values to yield when defeating this entry.
    /// </summary>
    int EV_SPA { get; set; }

    /// <summary>
    /// Amount of Special Defense Effort Values to yield when defeating this entry.
    /// </summary>
    int EV_SPD { get; set; }
}
