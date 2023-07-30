namespace PKHeX.Core;

public interface IOverworldCorrelation8
{
    OverworldCorrelation8Requirement GetRequirement(PKM pk);
    bool IsOverworldCorrelationCorrect(PKM pk);
}
