using System;

namespace PKHeX.Core;

public interface ILearnChecker
{
    ILearnChecker? GetPrevious(EntityContext currentContext);
    ILearnChecker? GetPrevious(Span<MoveResult> result, PKM pk, EvolutionHistory history);
    bool Check(Span<MoveResult> result, ReadOnlySpan<int> current, PKM pk, EvolutionHistory history);
}
