namespace PKHeX.Core;

/// <summary>
/// Contains information about the number of perfect IVs the object originates with.
/// </summary>
public interface IFlawlessIVCountConditional
{
    /// <summary>
    /// Number of perfect IVs the object originates with.
    /// </summary>
    /// <param name="pk">Entity to check</param>
    (byte Min, byte Max) GetFlawlessIVCount(PKM pk);
}
