namespace PKHeX.Core;

/// <summary>
/// Interface for a random <see cref="PIDType"/> correlation.
/// </summary>
public interface IRandomCorrelation
{
    bool IsCompatible(PIDType val, PKM pk);
    PIDType GetSuggestedCorrelation();
}
