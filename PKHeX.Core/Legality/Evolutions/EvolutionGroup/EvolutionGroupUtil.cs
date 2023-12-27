using static PKHeX.Core.EntityContext;

namespace PKHeX.Core;

/// <summary>
/// Utility logic for getting the <see cref="IEvolutionGroup"/> based on the input.
/// </summary>
public static class EvolutionGroupUtil
{
    /// <summary>
    /// Gets the <see cref="IEvolutionGroup"/> for the <see cref="EntityContext"/>.
    /// </summary>
    public static IEvolutionGroup GetGroup(EntityContext context) => context switch
    {
        Gen1 => EvolutionGroup1.Instance,
        Gen2 => EvolutionGroup2.Instance,
        Gen3 => EvolutionGroup3.Instance,
        Gen4 => EvolutionGroup4.Instance,
        Gen5 => EvolutionGroup5.Instance,
        Gen6 => EvolutionGroup6.Instance,
        Gen7 => EvolutionGroup7.Instance,
        Gen7b => EvolutionGroup7b.Instance,

        _ => EvolutionGroupHOME.Instance,
    };
}
