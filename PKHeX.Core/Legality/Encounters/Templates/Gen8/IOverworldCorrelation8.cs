namespace PKHeX.Core;

/// <summary>
/// Enforces a correlation between the randomly generated stats of the PKM.
/// </summary>
public interface IOverworldCorrelation8
{
    OverworldCorrelation8Requirement GetRequirement(PKM pk);
    bool IsOverworldCorrelationCorrect(PKM pk);
}
