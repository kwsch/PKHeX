using System;
using System.Collections.Generic;
using static PKHeX.Core.LegalityCheckStrings;

namespace PKHeX.Core;

/// <summary>
/// Verifies the <see cref="PKM.Ability"/> values.
/// </summary>
public sealed class AbilityVerifier : Verifier
{
    protected override CheckIdentifier Identifier => CheckIdentifier.Ability;

    public override void Verify(LegalityAnalysis data)
    {
        var result = VerifyAbility(data);
        data.AddLine(result);
    }

    private CheckResult VALID => GetValid(LAbilityFlag);
    private CheckResult INVALID => GetInvalid(LAbilityMismatch);

    private enum AbilityState : byte
    {
        CanMismatch,
        MustMatch,
    }

    private CheckResult VerifyAbility(LegalityAnalysis data)
    {
        var pk = data.Entity;
        var pi = data.PersonalInfo;

        // Check ability is possible (within bounds)
        int ability = pk.Ability;
        int abilIndex = pi.GetAbilityIndex(ability);
        if (abilIndex < 0)
            return GetInvalid(LAbilityUnexpected);

        var abilities = pi.Abilities;
        int format = pk.Format;
        if (format >= 6)
        {
            var num = pk.AbilityNumber;
            if (!IsValidAbilityBits(num))
                return INVALID;

            // Check AbilityNumber points to ability
            int an = num >> 1;
            if (an >= abilities.Count || abilities[an] != ability)
                return INVALID;

            // Check AbilityNumber for transfers without unique abilities
            int gen = data.Info.Generation;
            if (gen is 3 or 4 or 5 && num != 4)
            {
                // To determine AbilityNumber [PK5->PK6], check if the first ability in Personal matches the ability.
                // It is not possible to flip it to the other index as capsule requires unique abilities.
                if (abilities[0] == abilities[1] && num != 1)
                {
                    // Check if any pre-evolution could have it flipped.
                    var evos = data.Info.EvoChainsAllGens.Gen6;
                    var pt = GameData.GetPersonal(pk.Context.GetSingleGameVersion());
                    if (!GetWasDual(evos, pt, pk))
                        return INVALID;
                }
            }
        }

        if (format >= 8) // Ability Patch
        {
            var evos = data.Info.EvoChainsAllGens;
            if (pk.AbilityNumber == 4 && IsAccessibleAbilityPatch(evos))
            {
                if (CanAbilityPatch(format, abilities, pk.Species))
                    return GetValid(LAbilityPatchUsed);

                var e = data.EncounterOriginal;
                if (e.Species != pk.Species && CanAbilityPatch(format, PKX.Personal.GetFormEntry(e.Species, e.Form).Abilities, e.Species))
                    return GetValid(LAbilityPatchUsed);

                // Verify later, it may be encountered with its hidden ability without using an ability patch.
            }
        }

        var enc = data.EncounterMatch;
        if (enc is MysteryGift {Generation: >= 4} g)
            return VerifyAbilityMG(data, g, abilities);

        if (format < 6)
            return VerifyAbility345(data, enc, abilities, abilIndex);

        return VerifyAbility(data, abilities, abilIndex);
    }

    public static bool IsValidAbilityBits(int num) => num is 1 or 2 or 4;

    private static bool GetWasDual(ReadOnlySpan<EvoCriteria> evos, IPersonalTable pt, ISpeciesForm pk)
    {
        foreach (var evo in evos)
        {
            if (evo.Species == pk.Species)
                continue;

            var pe = pt.GetFormEntry(evo.Species, evo.Form);
            var abils = pe.Abilities;
            if (CanAbilityCapsule(6, abils))
                return true;
        }

        return false;
    }

    private CheckResult VerifyAbility(LegalityAnalysis data, IReadOnlyList<int> abilities, int abilIndex)
    {
        var enc = data.EncounterMatch;
        var eabil = enc.Ability;
        if (eabil >= 0)
        {
            if ((data.Entity.AbilityNumber == 4) != (eabil == AbilityPermission.OnlyHidden))
                return GetInvalid(LAbilityHiddenFail);
            if (eabil > 0)
                return VerifyFixedAbility(data, abilities, AbilityState.CanMismatch, eabil, abilIndex);
        }

        var gen = enc.Generation;
        return gen switch
        {
            5 => VerifyAbility5(data, enc, abilities),
            6 => VerifyAbility6(data, enc),
            7 => VerifyAbility7(data, enc),
            8 when data.Entity.BDSP => VerifyAbility8BDSP(data, enc),
            >=8 => VALID,
            _ => CheckMatch(data.Entity, abilities, gen, AbilityState.CanMismatch, enc),
        };
    }

    private CheckResult VerifyAbility345(LegalityAnalysis data, IEncounterable enc, IReadOnlyList<int> abilities, int abilIndex)
    {
        var pk = data.Entity;
        int format = pk.Format;
        var state = AbilityState.MustMatch;
        if (format is (3 or 4 or 5) && abilities[0] != abilities[1]) // 3-4/5 and have 2 distinct abilities now
            state = VerifyAbilityPreCapsule(data, abilities);

        var encounterAbility = enc.Ability;
        if (encounterAbility >= 0)
        {
            if ((pk.AbilityNumber == 4) != (encounterAbility == AbilityPermission.OnlyHidden))
                return GetInvalid(LAbilityHiddenFail);
            if (encounterAbility > 0)
                return VerifyFixedAbility(data, abilities, state, encounterAbility, abilIndex);
        }

        int gen = enc.Generation;
        if (gen == 5)
            return VerifyAbility5(data, enc, abilities);

        return CheckMatch(pk, abilities, gen, state, enc);
    }

    private CheckResult VerifyFixedAbility(LegalityAnalysis data, IReadOnlyList<int> abilities, AbilityState state, AbilityPermission encounterAbility, int abilIndex)
    {
        var pk = data.Entity;
        var enc = data.Info.EncounterMatch;
        if (enc.Generation >= 6)
        {
            if (IsAbilityCapsuleModified(pk, abilities, encounterAbility, data.Info.EvoChainsAllGens))
                return GetValid(LAbilityCapsuleUsed);
            if (pk.AbilityNumber != 1 << encounterAbility.GetSingleValue())
                return INVALID;
            return VALID;
        }

        if ((pk.AbilityNumber == 4) != (encounterAbility == AbilityPermission.OnlyHidden))
            return GetInvalid(LAbilityHiddenFail);

        bool hasEvolved = enc.Species != pk.Species;
        if (hasEvolved && state != AbilityState.CanMismatch)
        {
            // Evolving in Gen3 does not mutate the ability bit, so any mismatched abilities will stay mismatched.
            if (enc.Generation == 3)
            {
                if (encounterAbility.GetSingleValue() == abilIndex)
                    return VALID;

                // If it is in a future game and does not match the fixed ability, then it must match the PID.
                if (pk.Format != 3)
                    return GetPIDAbilityMatch(pk, abilities);

                // No way to un-mismatch it while existing solely on Gen3 games.
                return INVALID;
            }

            return CheckMatch(pk, abilities, enc.Generation, AbilityState.MustMatch, enc);
        }

        if (encounterAbility.GetSingleValue() == abilIndex)
            return VALID;

        if (pk.AbilityNumber == 1 << encounterAbility.GetSingleValue())
            return VALID;

        if (state == AbilityState.CanMismatch || encounterAbility == 0)
            return CheckMatch(pk, abilities, enc.Generation, AbilityState.MustMatch, enc);

        if (IsAbilityCapsuleModified(pk, abilities, encounterAbility, data.Info.EvoChainsAllGens))
            return GetValid(LAbilityCapsuleUsed);

        return INVALID;
    }

    private AbilityState VerifyAbilityPreCapsule(LegalityAnalysis data, IReadOnlyList<int> abilities)
    {
        var info = data.Info;
        // Gen4/5 origin
        if (info.Generation != 3)
            return AbilityState.MustMatch;

        // Gen3 origin... a lot of edge cases to check.
        var pk = data.Entity;
        var format = pk.Format;
        // CXD pokemon can have any ability without matching PID
        if (format == 3)
        {
            if (pk.Version == (int)GameVersion.CXD)
                return AbilityState.CanMismatch;
            return AbilityState.MustMatch;
        }

        // Evovled in Gen4/5
        if (pk.Species > Legal.MaxSpeciesID_3)
            return AbilityState.MustMatch;

        // If the species could not exist in Gen3, must match.
        var g3 = info.EvoChainsAllGens.Gen3;
        if (g3.Length == 0)
            return AbilityState.MustMatch;

        // Fall through when gen3 pk transferred to gen4/5
        var maxGen3Species = g3[0].Species;
        return VerifyAbilityGen3Transfer(data, abilities, maxGen3Species);
    }

    private AbilityState VerifyAbilityGen3Transfer(LegalityAnalysis data, IReadOnlyList<int> abilities, int maxGen3Species)
    {
        var pk = data.Entity;
        var pers = PersonalTable.E[maxGen3Species];
        if (pers.Ability1 != pers.Ability2) // Excluding Colosseum/XD, a Gen3 pk must match PID if it has 2 unique abilities
            return pk.Version == (int) GameVersion.CXD ? AbilityState.CanMismatch : AbilityState.MustMatch;

        if (pk.Species != maxGen3Species) // it has evolved in either gen 4 or gen 5; the ability must match PID
            return AbilityState.MustMatch;

        var chain = data.Info.EvoChainsAllGens;
        bool evolved45 = chain.Gen4.Length > 1 || (pk.Format == 5 && chain.Gen5.Length > 1);
        if (evolved45)
        {
            if (pk.Ability == pers.Ability1) // Could evolve in Gen4/5 and have a Gen3 only ability
                return AbilityState.CanMismatch; // Not evolved in Gen4/5, doesn't need to match PIDAbility

            if (pk.Ability == abilities[1]) // It could evolve in Gen4/5 and have Gen4 second ability
                return AbilityState.MustMatch; // Evolved in Gen4/5, must match PIDAbility
        }

        // If we reach here, it has not evolved in Gen4/5 games or has an invalid ability.
        // The ability does not need to match the PIDAbility, but only Gen3 ability is allowed.
        if (pk.Ability != pers.Ability1) // Not evolved in Gen4/5, but doesn't have Gen3 only ability
            data.AddLine(GetInvalid(LAbilityMismatch3)); // probably bad to do this here

        return AbilityState.CanMismatch;
    }

    private CheckResult VerifyAbilityMG(LegalityAnalysis data, MysteryGift g, IReadOnlyList<int> abilities)
    {
        if (g is PCD d)
            return VerifyAbilityPCD(data, abilities, d);

        var pk = data.Entity;
        if (g is PGT) // Ranger Manaphy
            return (pk.Format >= 6 ? (pk.AbilityNumber == 1) : (pk.AbilityNumber < 4)) ? VALID : GetInvalid(LAbilityMismatchGift);

        var cardType = g.AbilityType;
        if (cardType == 4) // 1/2/H
            return VALID;
        int abilNumber = pk.AbilityNumber;
        if (cardType == 3) // 1/2
            return abilNumber == 4 ? GetInvalid(LAbilityMismatchGift) : VALID;

        // Only remaining matches are fixed index abilities
        int cardAbilIndex = 1 << cardType;
        if (abilNumber == cardAbilIndex)
            return VALID;

        // Can still match if the ability was changed via ability capsule...
        // However, it can't change to/from Hidden Abilities.
        if (abilNumber == 4 || cardType == 2)
            return GetInvalid(LAbilityHiddenFail);

        // Ability can be flipped 0/1 if Ability Capsule is available, is not Hidden Ability, and Abilities are different.
        if (pk.Format >= 6)
        {
            if (CanAbilityCapsule(6, abilities))
                return GetValid(LAbilityCapsuleUsed);

            // Maybe was evolved after using ability capsule.
            var evos = data.Info.EvoChainsAllGens.Get(pk.Context);
            if (GetWasDual(evos, PKX.Personal, pk))
                return GetValid(LAbilityCapsuleUsed);
        }

        return pk.Format < 6 ? GetInvalid(LAbilityMismatchPID) : INVALID;
    }

    private CheckResult VerifyAbilityPCD(LegalityAnalysis data, IReadOnlyList<int> abilities, PCD pcd)
    {
        var pk = data.Entity;
        var format = pk.Format;
        if (format >= 6)
        {
            if (!CanAbilityCapsule(format, abilities))
            {
                // Gen3-5 transfer with same ability -> 1st ability that matches
                if (pk.AbilityNumber == 1)
                    return VALID;
                return CheckMatch(pk, abilities, 4, AbilityState.MustMatch, pcd); // evolved, must match
            }
            if (pk.AbilityNumber < 4) // Ability Capsule can change between 1/2
                return GetValid(LAbilityCapsuleUsed);
        }

        if (pcd.Species != pk.Species)
            return CheckMatch(pk, abilities, 4, AbilityState.MustMatch, pcd); // evolved, must match

        // Edge case (PID ability gift mismatch) -- must match gift ability.
        return pk.Ability == pcd.Gift.PK.Ability ? VALID : INVALID;
    }

    private CheckResult VerifyAbility5(LegalityAnalysis data, IEncounterTemplate enc, IReadOnlyList<int> abilities)
    {
        var pk = data.Entity;

        // Eggs and Encounter Slots are not yet checked for Hidden Ability potential.
        return enc switch
        {
            EncounterEgg e when pk.AbilityNumber == 4 && AbilityBreedLegality.BanHidden5.Contains(e.Species) => GetInvalid(LAbilityHiddenUnavailable),
            _ => CheckMatch(data.Entity, abilities, 5, pk.Format == 5 ? AbilityState.MustMatch : AbilityState.CanMismatch, enc),
        };
    }

    private CheckResult VerifyAbility6(LegalityAnalysis data, IEncounterTemplate enc)
    {
        var pk = data.Entity;
        if (pk.AbilityNumber != 4)
            return VALID;

        // Eggs and Encounter Slots are not yet checked for Hidden Ability potential.
        return enc switch
        {
            EncounterEgg egg when AbilityBreedLegality.BanHidden6.Contains((ushort)(egg.Species | (egg.Form << 11))) => GetInvalid(LAbilityHiddenUnavailable),
            _ => VALID,
        };
    }

    private CheckResult VerifyAbility7(LegalityAnalysis data, IEncounterTemplate enc)
    {
        var pk = data.Entity;
        if (pk.AbilityNumber != 4)
            return VALID;

        return enc switch
        {
            EncounterEgg egg when AbilityBreedLegality.BanHidden7.Contains((ushort)(egg.Species | (egg.Form << 11))) => GetInvalid(LAbilityHiddenUnavailable),
            _ => VALID,
        };
    }

    private CheckResult VerifyAbility8BDSP(LegalityAnalysis data, IEncounterable enc)
    {
        var pk = data.Entity;
        if (pk.AbilityNumber != 4)
            return VALID;

        return enc switch
        {
            EncounterEgg egg when AbilityBreedLegality.BanHidden8b.Contains((ushort)(egg.Species | (egg.Form << 11))) => GetInvalid(LAbilityHiddenUnavailable),
            _ => VALID,
        };
    }

    /// <summary>
    /// Final checks assuming nothing else has flagged the ability.
    /// </summary>
    /// <param name="pk">Pokémon</param>
    /// <param name="abilities">Current abilities</param>
    /// <param name="gen">Generation</param>
    /// <param name="state">Permissive to allow ability to deviate under special circumstances</param>
    /// <param name="enc">Encounter template the <see cref="pk"/> was matched to.</param>
    private CheckResult CheckMatch(PKM pk, IReadOnlyList<int> abilities, int gen, AbilityState state, IEncounterTemplate enc)
    {
        if (gen is (3 or 4) && pk.AbilityNumber == 4)
            return GetInvalid(LAbilityHiddenUnavailable);

        // other cases of hidden ability already flagged, all that is left is 1/2 mismatching
        if (state != AbilityState.MustMatch)
            return VALID;

        // Check that the ability bit is correct.
        if (pk is G3PKM g3)
        {
            var abit = g3.AbilityBit;
            // We've sanitized our personal data to replace "None" abilities with the first ability.
            // Granbull, Vibrava, and Flygon have dual abilities being the same.
            if (abilities[0] == abilities[1] && g3.Species is not ((int)Species.Granbull or (int)Species.Vibrava or (int)Species.Flygon)) // Not a dual ability
            {
                // Must not have the Ability bit flag set.
                // Shadow encounters set a random ability index; don't bother checking if it's a re-battle for ability bit flipping.
                if (abit && enc is not EncounterStaticShadow)
                    return GetInvalid(LAbilityMismatchFlag, CheckIdentifier.PID);
            }
            else
            {
                // Gen3 mainline origin sets the Ability index based on the PID, but only if it has two abilities.
                // Version value check isn't factually correct, but there are no C/XD gifts with (Version!=15) that have two abilities.
                // Pikachu, Celebi, Ho-Oh
                if (pk.Version != (int)GameVersion.CXD && abit != ((pk.PID & 1) == 1))
                    return GetInvalid(LAbilityMismatchPID, CheckIdentifier.PID);
            }
        }
        else if (pk.Format >= 6)
        {
            // 6+ already checked at the top of the verifier call stack
            return VALID;
        }

        // 3-5
        return GetPIDAbilityMatch(pk, abilities);
    }

    private CheckResult GetPIDAbilityMatch(PKM pk, IReadOnlyList<int> abilities)
    {
        // Ability Number bits are already verified as clean.
        var abil = abilities[pk.AbilityNumber >> 1];
        if (abil != pk.Ability)
            return GetInvalid(LAbilityMismatchPID);

        return VALID;
    }

    private static bool IsAccessibleAbilityPatch(EvolutionHistory evosAll)
    {
        return evosAll.HasVisitedSWSH || evosAll.HasVisitedBDSP;
    }

    private static bool IsAccessibleAbilityCapsule(EvolutionHistory evosAll)
    {
        if (evosAll.HasVisitedGen6 || evosAll.HasVisitedGen7)
            return true;
        return evosAll.HasVisitedSWSH || evosAll.HasVisitedBDSP;
    }

    // Ability Capsule can change between 1/2
    private static bool IsAbilityCapsuleModified(PKM pk, IReadOnlyList<int> abilities, AbilityPermission encounterAbility, EvolutionHistory evos)
    {
        if (!IsAccessibleAbilityCapsule(evos))
            return false; // Not available.
        if (!CanAbilityCapsule(pk.Format, abilities))
            return false;
        if (pk.AbilityNumber == 4)
            return false; // Cannot alter to hidden ability.
        if (encounterAbility == AbilityPermission.OnlyHidden)
            return false; // Cannot alter from hidden ability.
        return true;
    }

    public static bool CanAbilityCapsule(int format, IReadOnlyList<int> abilities)
    {
        if (format < 6) // Ability Capsule does not exist
            return false;
        return abilities[0] != abilities[1]; // Cannot alter ability index if it is the same as the other ability.
    }

    public static bool CanAbilityPatch(int format, IReadOnlyList<int> abilities, ushort species)
    {
        if (format < 8) // Ability Patch does not exist
            return false;

        // Can alter ability index if it is different from the other abilities.
        var h = abilities[2];
        if (h != abilities[0] || h != abilities[1])
            return true;

        // Some species have a distinct hidden ability only on another form, and can change between that form and its current form.
        return species switch
        {
            (int)Species.Giratina => true, // Form-0 is a/a/h
            (int)Species.Tornadus => true, // Form-0 is a/a/h
            (int)Species.Thundurus => true, // Form-0 is a/a/h
            (int)Species.Landorus => true, // Form-0 is a/a/h
            (int)Species.Enamorus => true, // Form-0 is a/a/h
            _ => false,
        };
    }
}
