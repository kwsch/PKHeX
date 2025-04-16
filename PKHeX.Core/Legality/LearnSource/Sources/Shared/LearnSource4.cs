namespace PKHeX.Core;

public abstract class LearnSource4
{
    private protected static readonly EggMoves6[] EggMoves = EggMoves6.GetArray(BinLinkerAccessor.Get(Util.GetBinaryResource("eggmove_dppt.pkl"), "dp"u8));
}
