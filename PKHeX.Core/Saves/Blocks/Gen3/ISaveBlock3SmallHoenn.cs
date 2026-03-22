namespace PKHeX.Core;

/// <summary>
/// Small-block properties common to RS & Emerald save files.
/// </summary>
public interface ISaveBlock3SmallHoenn : ISaveBlock3Small
{
    /// <summary>
    /// localTimeOffset
    /// </summary>
    RTC3 ClockInitial { get; set; }

    /// <summary>
    /// lastBerryTreeUpdate
    /// </summary>
    RTC3 ClockElapsed { get; set; }
}
