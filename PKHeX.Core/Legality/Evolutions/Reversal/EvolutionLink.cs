using System;

namespace PKHeX.Core;

/// <summary>
/// Links a <see cref="EvolutionMethod"/> to the source <see cref="Species"/> and <see cref="Form"/> that the method can be triggered from.
/// </summary>
public struct EvolutionLink
{
    private Func<PKM, bool>? IsBanned = null;
    public readonly EvolutionMethod Method;
    public readonly ushort Species;
    public readonly byte Form;

    public EvolutionLink(ushort species, byte form, EvolutionMethod method)
    {
        Species = species;
        Form = form;
        Method = method;
    }

    public bool IsEmpty => Species == 0;

    public (ushort Species, byte Form) Tuple => (Species, Form);

    public void Ban(Func<PKM, bool> check) => IsBanned = check;

    /// <summary>
    /// Checks if the <see cref="Method"/> is allowed.
    /// </summary>
    /// <param name="pk">Entity to check</param>
    /// <returns>True if banned, false if allowed.</returns>
    public bool IsEvolutionBanned(PKM pk) => IsBanned != null && IsBanned(pk);
}
