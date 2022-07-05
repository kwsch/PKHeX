using System.Collections.Generic;

namespace PKHeX.Core;

internal sealed class LearnInfo
{
    public bool MixedGen12NonTradeback { get; set; }
    public List<int> Gen1Moves { get; } = new();
    public List<int> Gen2PreevoMoves { get; } = new();

    public readonly MoveParseSource Source;
    public readonly bool IsGen2Pkm;

    public LearnInfo(PKM pk, MoveParseSource source)
    {
        IsGen2Pkm = pk.Format == 2 || pk.VC2;
        Source = source;
    }
}
