namespace PKHeX.Core;

/// <summary>
/// Exposes details about the entity's Nature
/// </summary>
public interface INature : INatureReadOnly
{
    /// <summary>
    /// Nature the entity has.
    /// </summary>
    new Nature Nature { get; set; }
}
