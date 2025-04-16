namespace PKHeX.Core;

public abstract class LearnSource5
{
    private protected static readonly EggMoves6[] EggMoves = EggMoves6.GetArray(BinLinkerAccessor.Get(Util.GetBinaryResource("eggmove_bw.pkl"), "bw"u8));
}
