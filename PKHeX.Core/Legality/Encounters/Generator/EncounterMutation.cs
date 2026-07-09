using System;
using static PKHeX.Core.AbilityPermission;
using static PKHeX.Core.EncounterMutation;
using static PKHeX.Core.EntityContext;

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
    public static bool IsComplexNature(this EncounterMutation m) => m.HasFlag(AllowOnlyNeutralNature);

    /// <summary>
    /// Gets the suggested post-generation mutations allowed for the given target context and level.
    /// </summary>
    /// <param name="targetContext">Destination context</param>
    /// <param name="level">Destination level</param>
    public static EncounterMutation GetSuggested(EntityContext targetContext, byte level)
    {
        if (targetContext.IsEraPre3DS)
            return EncounterMutation.None;
        if (targetContext.IsEraPreSwitch)
        {
            if (targetContext is Gen7b)
                return level != 100 ? EncounterMutation.None : CanMaxIndividualStat;
            return CanAbilityCapsule;
        }

        var result = AllCanChange;

        // In PLA, Hyper Training is equivalent to using Grit, which can be done at any level.
        // If bottle caps can't be used, don't allow maxing IVs.
        var minBottleLevel = targetContext.GetHyperTrainMinLevel();
        if (level < minBottleLevel && targetContext is not Gen8a)
            result &= ~CanMaxIndividualStat;
        return result;
    }

    /// <summary>
    /// Determines if the given mutation allows for the encounter to eventually arrive at the specified ability permissions.
    /// </summary>
    /// <param name="mutation">The encounter mutation allowed after capture.</param>
    /// <param name="start">The encounter's ability permission.</param>
    /// <param name="end">The end-state ability permission.</param>
    /// <returns>True if the mutation allows for the encounter to eventually arrive at the specified ability permissions; otherwise, false.</returns>
    public static bool CanGetAbility(this EncounterMutation mutation, AbilityPermission start, AbilityPermission end) => start switch
    {
        Any12 => end switch
        {
            OnlyHidden => mutation.HasFlag(CanAbilityPatch),
            _ => false,
        },
        OnlyFirst => end switch
        {
            OnlySecond => mutation.HasFlag(CanAbilityCapsule),
            OnlyHidden => mutation.HasFlag(CanAbilityPatch),
            _ => false,
        },
        OnlySecond => end switch
        {
            OnlyFirst => mutation.HasFlag(CanAbilityCapsule),
            OnlyHidden => mutation.HasFlag(CanAbilityPatch),
            _ => false,
        },
        OnlyHidden => end.CanBeHidden() || mutation.HasFlag(CanAbilityPatch),
        _ => true,
    };
}
