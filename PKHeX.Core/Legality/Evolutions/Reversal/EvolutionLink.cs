namespace PKHeX.Core;

/// <summary>
/// Links a <see cref="EvolutionMethod"/> to the source <see cref="Species"/> and <see cref="Form"/> that the method can be triggered from.
/// </summary>
public readonly record struct EvolutionLink(EvolutionMethod Method, ushort Species, byte Form)
{
    /// <summary>
    /// Indicates if this link is empty, meaning it does not represent a valid evolution method.
    /// </summary>
    public bool IsEmpty => Species == 0;
}
