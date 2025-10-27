using System;

namespace PKHeX.Core;

/// <summary>
/// Exposes info about the Plus Move compatibility and history of use.
/// </summary>
public interface IPlusRecord
{
    /// <summary>
    /// Indicates if the move at the requested index has been flagged.
    /// </summary>
    /// <param name="index">Move shop offering</param>
    /// <returns>True if flagged</returns>
    bool GetMovePlusFlag(int index);

    /// <summary>
    /// Sets the flag indicating that the move at the requested index has been flagged.
    /// </summary>
    /// <param name="index">Move index</param>
    /// <param name="value">True if flagged</param>
    void SetMovePlusFlag(int index, bool value = true);

    /// <summary>
    /// Indicates if any move has been flagged.
    /// </summary>
    bool GetMovePlusFlagAny();

    /// <summary>
    /// Indicates if any move has been flagged that is outside the permitted range.
    /// </summary>
    bool GetMovePlusFlagAnyImpossible();
}

public interface IPermitPlus : IPermitRecord
{
    /// <summary>
    /// Individual accessed move IDs that a given record remembers.
    /// </summary>
    ReadOnlySpan<ushort> PlusMoveIndexes { get; }

    /// <summary>
    /// Maximum count of record flags that are available.
    /// </summary>
    int PlusCountTotal { get; }

    /// <summary>
    /// Maximum count of record flags that are used.
    /// </summary>
    int PlusCountUsed { get; }
}
