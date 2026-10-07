using System;

namespace PKHeX.Core;

/// <summary>
/// Flags indicating the era of chain-breeding, used to determine the rules and mechanics applicable to the breeding process.
/// </summary>
[Flags]
public enum ChainBreedEraFlags : byte
{
    None,

    /// <summary>
    /// Indicates that the chain-breeding process is occurring in the Nintendo Switch era for GBA games.
    /// Otherwise, follows hardware era rules.
    /// </summary>
    NintendoSwitchGBA = 1 << 0,
}
