namespace PKHeX.Core;

public abstract class LearnSource4
{
    private protected static readonly MoveSource[] EggMoves = MoveSource.GetArray(BinLinkerAccessor16.Get(Util.GetBinaryResource("eggmove_dppt.pkl"), "dp"u8));
}
