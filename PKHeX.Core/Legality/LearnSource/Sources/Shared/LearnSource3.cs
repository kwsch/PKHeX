namespace PKHeX.Core;

public abstract class LearnSource3
{
    private protected static readonly EggMoves6[] EggMoves = EggMoves6.GetArray(BinLinkerAccessor.Get(Util.GetBinaryResource("eggmove_rs.pkl"), "rs"u8)); // same for all Gen3 games
}
