using System.Collections.Generic;

namespace PKHeX.Core;

/// <summary>
/// Legal Move information for a single <see cref="PKM"/>, for indicating if a move is legal or not.
/// </summary>
public sealed class LegalMoveSource<T>(ILegalMoveDisplaySource<T> Display)
{
    public LegalMoveInfo Info { get; } = new();
    public readonly ILegalMoveDisplaySource<T> Display = Display;

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
