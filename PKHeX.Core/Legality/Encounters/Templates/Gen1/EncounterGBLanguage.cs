namespace PKHeX.Core;

/// <summary>
/// Generations 1 &amp; 2 cannot communicate between Japanese &amp; International versions.
/// </summary>
public enum EncounterGBLanguage
{
    /// <summary> Can only be obtained in Japanese games. </summary>
    Japanese,

    /// <summary> Can only be obtained in International (not Japanese) games. </summary>
    International,

    /// <summary> Can be obtained in any localization. </summary>
    Any,
}
