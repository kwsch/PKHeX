namespace PKHeX.Core
{
    public interface IGameValueLimit
    {
        int MaxMoveID { get; }
        int MaxSpeciesID { get; }
        int MaxItemID { get; }
        int MaxAbilityID { get; }
        int MaxBallID { get; }
        int MaxGameID { get; }
        int MinGameID { get; }
        int MaxIV { get; }
        int MaxEV { get; }
        int OTLength { get; }
        int NickLength { get; }
    }
}
