using System;

namespace PKHeX.Core;

/// <summary>
/// Interface for encounter objects that conditionally provide a special extra move.
/// </summary>
public interface ISingleMoveBonus
{
    bool IsMoveBonusPossible { get; }
    ReadOnlySpan<ushort> GetMoveBonusPossible();
    bool IsMoveBonus(ushort move);

    bool IsMoveBonusRequired { get; }
    bool TryGetRandomMoveBonus(out ushort move);
}
