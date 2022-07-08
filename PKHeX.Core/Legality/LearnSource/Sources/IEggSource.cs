using System;

namespace PKHeX.Core;

public interface IEggSource
{
    bool GetIsEggMove(int species, int form, int move);
    ReadOnlySpan<int> GetEggMoves(int species, int form);
}
