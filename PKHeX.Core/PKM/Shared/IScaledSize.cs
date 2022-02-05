namespace PKHeX.Core;

public interface IScaledSize
{
    int WeightScalar { get; set; }
    int HeightScalar { get; set; }
}

public interface IScaledSizeAbsolute
{
    float HeightAbsolute { get; set; }
    float WeightAbsolute { get; set; }
}

public interface IScaledSizeValue : IScaledSize, IScaledSizeAbsolute
{
    void ResetHeight();
    void ResetWeight();
    float CalcHeightAbsolute { get; }
    float CalcWeightAbsolute { get; }
}

public interface ICombatPower
{
    int Stat_CP { get; set; }
    void ResetCP();
}
