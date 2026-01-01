using System;

namespace PKHeX.Core;

[Flags]
public enum EncounterMutation : byte
{
    None = 0,
    CanMintNature = 1 << 0,
    CanMaxIndividualStat = 1 << 1,

    CanAbilityCapsule = 1 << 2,
    CanAbilityPatch = 1 << 3,

    CanChangeAbility = CanAbilityCapsule | CanAbilityPatch,

    AllCanChange = CanMintNature | CanMaxIndividualStat | CanChangeAbility,

    /// <summary>
    /// Causes the generated encounter to only have a neutral nature.
    /// </summary>
    /// <remarks>Useful for when holding berries that can cause confusion for specific natures.</remarks>
    AllowOnlyNeutralNature = 1 << 7,
}

public static class EncounterMutationUtil
{
    public static bool IsComplexNature(this EncounterMutation m) => m.HasFlag(EncounterMutation.AllowOnlyNeutralNature);

    /// <summary>
    /// Gets the suggested post-generation mutations allowed for the given target context and level.
    /// </summary>
    /// <param name="targetContext">Destination context</param>
    /// <param name="level">Destination level</param>
    public static EncounterMutation GetSuggested(EntityContext targetContext, byte level)
    {
        var gen = targetContext.Generation;
        if (gen < 6)
            return EncounterMutation.None;
        if (gen < 8)
        {
            if (targetContext is EntityContext.Gen7b)
                return level != 100 ? EncounterMutation.None : EncounterMutation.CanMaxIndividualStat;
            return EncounterMutation.CanAbilityCapsule;
        }

        var result = EncounterMutation.AllCanChange;

        // In PLA, Hyper Training is equivalent to using Grit, which can be done at any level.
        // If bottle caps can't be used, don't allow maxing IVs.
        var minBottleLevel = targetContext.GetHyperTrainMinLevel();
        if (level < minBottleLevel && targetContext is not EntityContext.Gen8a)
            result &= ~EncounterMutation.CanMaxIndividualStat;
        return result;
    }
}
