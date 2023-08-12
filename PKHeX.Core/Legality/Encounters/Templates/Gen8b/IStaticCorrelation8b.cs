namespace PKHeX.Core;

public interface IStaticCorrelation8b
{
    StaticCorrelation8bRequirement GetRequirement(PKM pk);
    bool IsStaticCorrelationCorrect(PKM pk);
}
