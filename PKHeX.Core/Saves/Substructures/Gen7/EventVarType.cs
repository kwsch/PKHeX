namespace PKHeX.Core;

public enum EventVarType
{
    /// <summary>
    /// Toggles data in the map.
    /// </summary>
    Zone,

    /// <summary>
    /// Toggles certain game features on and off.
    /// </summary>
    System,

    /// <summary>
    /// Hides overworld entities if flag is set.
    /// </summary>
    /// <remarks>Flag only</remarks>
    Vanish,

    /// <summary>
    /// Tweaks overworld entities depending on the work value.
    /// <remarks>
    /// Work Value only
    /// </remarks>
    /// </summary>
    Scene = Vanish,

    /// <summary>
    /// Tracks game progress.
    /// </summary>
    Event,
}
