using System;

namespace PKHeX.Core;

public interface IEvolutionContext
{
    int ExploreDevolve(Span<EvoCriteria> evos, PKM pk, IEvolutionContext last);
    int ExploreEvolve(Span<EvoCriteria> evos, PKM pk, IEvolutionContext last);

    int Forward(Span<EvoCriteria> evos, PKM pk);
    int Reverse(Span<EvoCriteria> evos, PKM pk);

    bool Meld(Span<EvoCriteria> reference, Span<EvoCriteria> local);
    bool Drop(Span<EvoCriteria> reference);
}
