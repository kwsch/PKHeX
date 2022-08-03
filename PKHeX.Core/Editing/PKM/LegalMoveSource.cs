using System.Collections.Generic;

namespace PKHeX.Core;

/// <summary>
/// Legal Move information for a single <see cref="PKM"/>, for indicating if a move is legal or not.
/// </summary>
public sealed class LegalMoveSource<T>
{
    public LegalMoveInfo Info { get; } = new();
    public readonly ILegalMoveDisplaySource<T> Display;

    public LegalMoveSource(ILegalMoveDisplaySource<T> display) => Display = display;

    public void ReloadMoves(LegalityAnalysis source)
    {
        if (!Info.ReloadMoves(source))
            return;
        Display.ReloadMoves(Info);
    }

    public void ChangeMoveSource(IReadOnlyList<T> moves)
    {
        Display.ReloadMoves(moves);
    }
}
