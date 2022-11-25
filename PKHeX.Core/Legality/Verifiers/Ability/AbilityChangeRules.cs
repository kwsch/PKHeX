using System;
using static PKHeX.Core.AbilityPermission;

namespace PKHeX.Core;

/// <summary>
/// Rules for changing an entity's current ability index.
/// </summary>
public static class AbilityChangeRules
{
    public static bool IsAbilityStateValid(this IEncounterTemplate enc, EvolutionHistory evosAll, int abilityFlag) => (enc switch
    {
        IFixedAbilityNumber f => f.Ability,
        _ => Any12,
    }).IsAbilityStateValid(evosAll, abilityFlag);

    public static bool IsAbilityStateValid(this AbilityPermission ability, EvolutionHistory evosAll, int abilityFlag) => ability switch
    {
        Any12H     => true,
        Any12      => abilityFlag != 4 || IsAbilityPatchPossible(evosAll),
        OnlyHidden => abilityFlag == 4 || IsAbilityPatchRevertPossible(evosAll, abilityFlag),
        OnlyFirst  => abilityFlag == 1 || (abilityFlag == 4 && IsAbilityPatchPossible(evosAll)) || (abilityFlag != 4 && IsAbilityCapsulePossible(evosAll)),
        OnlySecond => abilityFlag == 2 || (abilityFlag == 4 && IsAbilityPatchPossible(evosAll)) || (abilityFlag != 4 && IsAbilityCapsulePossible(evosAll)),
        _ => throw new ArgumentOutOfRangeException(nameof(ability), ability, null),
    };

    public static bool IsAbilityChangeAvailable(this IEncounterTemplate enc, EvolutionHistory evosAll) => (enc switch
    {
        IFixedAbilityNumber f => f.Ability,
        _ => Any12,
    }).IsAbilityChangeAvailable(evosAll);

    public static bool IsAbilityChangeAvailable(this AbilityPermission ability, EvolutionHistory evosAll) => ability switch
    {
        Any12H => true,
        Any12 => IsAbilityPatchAvailable(evosAll),
        OnlyHidden => IsAbilityPatchRevertAvailable(evosAll),
        OnlyFirst or OnlySecond => IsAbilityPatchAvailable(evosAll) || IsAbilityCapsuleAvailable(evosAll),
        _ => throw new ArgumentOutOfRangeException(nameof(ability), ability, null),
    };

    public static bool IsAbilityCapsuleAvailable(EvolutionHistory evosAll)
    {
        if (evosAll.HasVisitedGen9)
            return true;
        if (evosAll.HasVisitedSWSH)
            return true;
        if (evosAll.HasVisitedBDSP)
            return true;
        if (evosAll.HasVisitedGen7)
            return true;
        if (evosAll.HasVisitedGen6)
            return true;
        return false;
    }

    public static bool IsAbilityCapsulePossible(EvolutionHistory evosAll)
    {
        if (evosAll.HasVisitedGen9 && IsCapsulePossible<PersonalTable9SV, PersonalInfo9SV>(evosAll.Gen9, PersonalTable.SV))
            return true;
        if (evosAll.HasVisitedSWSH && IsCapsulePossible<PersonalTable8SWSH, PersonalInfo8SWSH>(evosAll.Gen8, PersonalTable.SWSH))
            return true;
        if (evosAll.HasVisitedBDSP && IsCapsulePossible<PersonalTable8BDSP, PersonalInfo8BDSP>(evosAll.Gen8b, PersonalTable.BDSP))
            return true;
        if (evosAll.HasVisitedGen7 && IsCapsulePossible<PersonalTable7, PersonalInfo7>(evosAll.Gen7, PersonalTable.USUM))
            return true;
        if (evosAll.HasVisitedGen6 && IsCapsulePossible<PersonalTable6AO, PersonalInfo6AO>(evosAll.Gen6, PersonalTable.AO))
            return true;
        return false;
    }

    public static bool IsAbilityPatchAvailable(EvolutionHistory evosAll)
    {
        if (evosAll.HasVisitedGen9)
            return true;
        if (evosAll.HasVisitedSWSH || evosAll.HasVisitedBDSP)
            return true;
        return false;
    }

    public static bool IsAbilityPatchPossible(EvolutionHistory evosAll)
    {
        if (evosAll.HasVisitedGen9 && IsPatchPossible<PersonalTable9SV, PersonalInfo9SV>(evosAll.Gen9, PersonalTable.SV))
            return true;
        if (evosAll.HasVisitedSWSH && IsPatchPossible<PersonalTable8SWSH, PersonalInfo8SWSH>(evosAll.Gen8, PersonalTable.SWSH))
            return true;
        if (evosAll.HasVisitedBDSP && IsPatchPossible<PersonalTable8BDSP, PersonalInfo8BDSP>(evosAll.Gen8b, PersonalTable.BDSP))
            return true;
        return false;
    }

    public static bool IsAbilityPatchRevertAvailable(EvolutionHistory evosAll)
    {
        if (evosAll.HasVisitedGen9)
            return true;
        return false;
    }

    public static bool IsAbilityPatchRevertPossible(EvolutionHistory evosAll, int abilityIndex)
    {
        if (evosAll.HasVisitedGen9 && IsRevertPossible<PersonalTable9SV, PersonalInfo9SV>(evosAll.Gen9, PersonalTable.SV, abilityIndex))
            return true;
        return false;
    }

    private static bool IsCapsulePossible<TTable, TInfo>(EvoCriteria[] evos, TTable table)
        where TTable : IPersonalTable<TInfo>
        where TInfo : IPersonalInfo, IPersonalAbility12
    {
        for (int i = evos.Length - 1; i >= 0; i--)
        {
            var evo = evos[i];
            var pi = table[evo.Species, evo.Form];
            if (!pi.GetIsAbility12Same())
                return true;
        }
        return false;
    }

    private static bool IsPatchPossible<TTable, TInfo>(EvoCriteria[] evos, TTable table)
        where TTable : IPersonalTable<TInfo>
        where TInfo : IPersonalInfo, IPersonalAbility12H
    {
        for (int i = evos.Length - 1; i >= 0; i--)
        {
            var evo = evos[i];
            var pi = table[evo.Species, evo.Form];
            if (pi.GetIsAbilityHiddenUnique())
                return true;
        }

        // Some species have a distinct hidden ability only on another form, and can change between that form and its current form.
        var first = evos[0];
        return first.Form != 0 && first.Species switch
        {
            (int)Species.Giratina => true, // Form-0 is a/a/h
            (int)Species.Tornadus => true, // Form-0 is a/a/h
            (int)Species.Thundurus => true, // Form-0 is a/a/h
            (int)Species.Landorus => true, // Form-0 is a/a/h
            (int)Species.Enamorus => true, // Form-0 is a/a/h
            _ => false,
        };
    }

    private static bool IsRevertPossible<TTable, TInfo>(EvoCriteria[] evos, TTable table, int abilityIndex)
        where TTable : IPersonalTable<TInfo>
        where TInfo : IPersonalInfo, IPersonalAbility12H
    {
        bool revert = false;
        for (var i = evos.Length - 1; i >= 0; i--)
        {
            var evo = evos[i];
            var pi = table[evo.Species, evo.Form];
            if (revert && abilityIndex == 2 && !pi.GetIsAbility12Same())
                return true;
            if (pi.GetIsAbilityHiddenUnique())
                continue;
            revert = true;
            if (pi.GetIsAbility12Same())
                continue;
            return true;
        }
        return false;
    }
}
