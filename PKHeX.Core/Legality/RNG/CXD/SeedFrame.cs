namespace PKHeX.Core;

/// <summary>
/// Message Passing for frame results.
/// </summary>
internal readonly record struct SeedFrame(uint PID, int FrameID);