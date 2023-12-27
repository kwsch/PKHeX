namespace PKHeX.Core;

/// <summary>
/// Criteria for manipulating box data.
/// </summary>
/// <param name="Start">Box to start at (inclusive)</param>
/// <param name="Stop">Box to stop after (inclusive)</param>
/// <param name="Reverse">Iterate in reverse</param>
public readonly record struct BoxManipParam(int Start, int Stop, bool Reverse = false);
