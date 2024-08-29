using System;

namespace PKHeX.Core;

/// <summary>
/// Generation 5 C-Gear Background Image, tile positions formatted for <see cref="GameVersion.B2W2"/>.
/// </summary>
public sealed class CGearBackgroundB2W2(Memory<byte> Raw) : CGearBackground(Raw)
{
    /// <summary>
    /// Standard format, no oddities compared to the format for B/W.
    /// </summary>
    public const string Extension = "cgb";
}
