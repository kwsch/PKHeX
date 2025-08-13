namespace PKHeX.Core;

public abstract class LearnSource5
{
    private protected static readonly MoveSource[] EggMoves = MoveSource.GetArray(BinLinkerAccessor16.Get(Util.GetBinaryResource("eggmove_bw.pkl"), "bw"u8));
}
