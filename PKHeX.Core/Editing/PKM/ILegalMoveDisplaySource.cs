using System.Collections.Generic;

namespace PKHeX.Core;

public interface ILegalMoveDisplaySource<T>
{
    void ReloadMoves(LegalMoveInfo info);
    void ReloadMoves(IReadOnlyList<T> moves);
    bool GetIsMoveBoxOrdered(int index);
    void SetIsMoveBoxOrdered(int index, bool value);

    /// <summary> Creates a shallow copy of the array reference for use in binding. </summary>
    IReadOnlyList<T> DataSource { get; }
}
