namespace PKHeX.Core;

/// <summary>
/// Enforces a correlation between the randomly generated stats of the PKM.
/// </summary>
public interface IStaticCorrelation8b
{
    StaticCorrelation8bRequirement GetRequirement(PKM pk);
    bool IsStaticCorrelationCorrect(PKM pk);
}
