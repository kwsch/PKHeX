using System;
using static PKHeX.Core.EntityContext;

namespace PKHeX.Core;

/// <summary>
/// Logic for fetching a specific <see cref="ILearnGroup"/>.
/// </summary>
public static class LearnGroupUtil
{
    /// <summary>
    /// Get the <see cref="ILearnGroup"/> for the given <see cref="PKM"/>.
    /// </summary>
    public static ILearnGroup GetCurrentGroup(PKM pk) => GetCurrentGroup(pk.Context);

    /// <summary>
    /// Get the <see cref="ILearnGroup"/> for the given <see cref="EntityContext"/>.
    /// </summary>
    /// <param name="context">Context to get the <see cref="ILearnGroup"/> for.</param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static ILearnGroup GetCurrentGroup(EntityContext context) => context switch
    {
        Gen1 => LearnGroup1.Instance,
        Gen2 => LearnGroup2.Instance,
        Gen3 => LearnGroup3.Instance,
        Gen4 => LearnGroup4.Instance,
        Gen5 => LearnGroup5.Instance,
        Gen6 => LearnGroup6.Instance,
        Gen7 => LearnGroup7.Instance,
        Gen8 => LearnGroup8.Instance,
        Gen9 => LearnGroup9.Instance,

        Gen7b => LearnGroup7b.Instance,
        Gen8a => LearnGroup8a.Instance,
        Gen8b => LearnGroup8b.Instance,

        _ => throw new ArgumentOutOfRangeException(nameof(context), context, null),
    };
}
