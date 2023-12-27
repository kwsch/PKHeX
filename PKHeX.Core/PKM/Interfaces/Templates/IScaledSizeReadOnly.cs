namespace PKHeX.Core;

/// <summary>
/// Stores scalars that range from 0-255.
/// </summary>
public interface IScaledSizeReadOnly
{
    byte WeightScalar { get; }
    byte HeightScalar { get; }
}
