namespace PKHeX.Core;

/// <summary>
/// Stores scalars that range from 0-255.
/// </summary>
public interface IScaledSize : IScaledSizeReadOnly
{
    new byte WeightScalar { get; set; }
    new byte HeightScalar { get; set; }
}

/// <summary>
/// Stores scalars that range from 0-255.
/// </summary>
public interface IScaledSize3
{
    byte Scale { get; set; }
}

/// <summary>
/// Stores scalars that range from 0 to infinity, indicating absolute size.
/// </summary>
public interface IScaledSizeAbsolute
{
    float HeightAbsolute { get; set; }
    float WeightAbsolute { get; set; }
}

/// <summary>
/// Exposes methods to calculate and reset the stored size values.
/// </summary>
public interface IScaledSizeValue : IScaledSize, IScaledSizeAbsolute
{
    void ResetHeight();
    void ResetWeight();
    float CalcHeightAbsolute { get; }
    float CalcWeightAbsolute { get; }
}

/// <summary>
/// Exposes property to indicate the "CP".
/// </summary>
/// <remarks>Used only in LGP/E.</remarks>
public interface ICombatPower
{
    int Stat_CP { get; set; }
    void ResetCP();
}
