namespace PKHeX.Core;

public interface IRestrictVersion
{
    bool CanBeReceivedByVersion(int version);
}

public interface IRandomCorrelation
{
    bool IsCompatible(PIDType val, PKM pk);
    PIDType GetSuggestedCorrelation();
}
