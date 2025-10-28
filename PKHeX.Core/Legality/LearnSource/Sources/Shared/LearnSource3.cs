namespace PKHeX.Core;

public abstract class LearnSource3
{
    private protected static readonly MoveSource[] EggMoves = MoveSource.GetArray(BinLinkerAccessor16.Get(Util.GetBinaryResource("eggmove_rs.pkl"), "rs"u8)); // same for all Gen3 games
}
