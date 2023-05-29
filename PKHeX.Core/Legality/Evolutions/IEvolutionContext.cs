using System;

namespace PKHeX.Core;

public interface IEvolutionContext
{
    int Forward(Span<EvoCriteria> evos, PKM pk, bool skipChecks = false);
    int Reverse(Span<EvoCriteria> evos, Span<EvoCriteria> result, PKM pk, ref EvolutionOrigin origin);

    bool Meld(Span<EvoCriteria> reference, Span<EvoCriteria> local);
    bool Drop(Span<EvoCriteria> reference);

    IEvolutionContext? GetPrevious(EvolutionOrigin origin);
    IEvolutionContext? GetNext(EvolutionOrigin origin);
}
