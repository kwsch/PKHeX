namespace PKHeX.Core;

/// <summary>
/// Exposes details about the entity's Nature
/// </summary>
public interface INatureReadOnly
{
    /// <summary>
    /// Nature the entity has.
    /// </summary>
    Nature Nature { get; }
}
