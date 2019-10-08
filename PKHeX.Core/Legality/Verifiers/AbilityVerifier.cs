using System;
using System.Collections.Generic;
using static PKHeX.Core.LegalityCheckStrings;

namespace PKHeX.Core
{
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
            var pkm = data.pkm;
            var abilities = pkm.PersonalInfo.Abilities;

            // Check ability is possible (within bounds)
            int ability = pkm.Ability;
            int abilval = Array.IndexOf(abilities, ability);
            if (abilval < 0)
                return GetInvalid(LAbilityUnexpected);

            if (data.EncounterMatch is MysteryGift g && g.Format >= 4)
                return VerifyAbilityMG(data, abilities, g.AbilityType);

            if (pkm.Format < 6)
                return VerifyAbility345(data, abilities, abilval);

            // Check AbilityNumber points to ability
            int an = pkm.AbilityNumber >> 1;
            if (an >= abilities.Length || abilities[an] != ability)
                return GetInvalid(LAbilityMismatchFlag);

            return VerifyAbility(data, abilities, abilval);
        }

        private CheckResult VerifyAbility(LegalityAnalysis data, IReadOnlyList<int> abilities, int abilnum)
        {
            var EncounterMatch = data.EncounterMatch;
            var eabil = GetEncounterFixedAbilityNumber(EncounterMatch);
            if (eabil >= 0)
            {
                if ((data.pkm.AbilityNumber == 4) != (eabil == 4))
                    return GetInvalid(LAbilityHiddenFail);
                if (eabil > 0)
                    return VerifyFixedAbility(data, abilities, AbilityState.CanMismatch, eabil, abilnum);
            }

            var gen = data.Info.Generation;
            return gen switch
            {
                5 => VerifyAbility5(data, abilities),
                6 => VerifyAbility6(data),
                7 => VerifyAbility7(data),
                8 => VerifyAbility8(data),
                _ => CheckMatch(data.pkm, abilities, gen, AbilityState.CanMismatch)
            };
        }

        private CheckResult VerifyAbility345(LegalityAnalysis data, IReadOnlyList<int> abilities, int abilnum)
        {
            var pkm = data.pkm;
            var state = AbilityState.MustMatch;
            if (3 <= pkm.Format && pkm.Format <= 5 && abilities[0] != abilities[1]) // 3-4/5 and have 2 distinct abilities now
                state = VerifyAbilityPreCapsule(data, abilities);

            var EncounterMatch = data.EncounterMatch;
            int eabil = GetEncounterFixedAbilityNumber(EncounterMatch);
            if (eabil >= 0)
            {
                if ((data.pkm.AbilityNumber == 4) != (eabil == 4))
                    return GetInvalid(LAbilityHiddenFail);
                if (eabil > 0)
                    return VerifyFixedAbility(data, abilities, state, eabil, abilnum);
            }

            int gen = data.Info.Generation;
            if (gen == 5)
                return VerifyAbility5(data, abilities);

            return CheckMatch(pkm, abilities, gen, state);
        }

        private CheckResult VerifyFixedAbility(LegalityAnalysis data, IReadOnlyList<int> abilities, AbilityState state, int EncounterAbility, int abilval)
        {
            var pkm = data.pkm;
            if (data.Info.EncounterMatch is IGeneration g && g.Generation >= 6)
            {
                if (IsAbilityCapsuleModified(pkm, abilities, EncounterAbility))
                    return GetValid(LAbilityCapsuleUsed);
                if (pkm.AbilityNumber != EncounterAbility)
                    return INVALID;
                return VALID;
            }

            if ((pkm.AbilityNumber == 4) != (EncounterAbility == 4))
                return GetInvalid(LAbilityHiddenFail);

            if (data.EncounterMatch.Species != pkm.Species && state != AbilityState.CanMismatch) // evolved
                return CheckMatch(pkm, abilities, data.Info.Generation, AbilityState.MustMatch);

            if (EncounterAbility == 1 << abilval)
                return GetValid(LAbilityFlag);

            if (pkm.AbilityNumber == EncounterAbility)
                return VALID;

            if (state == AbilityState.CanMismatch || EncounterAbility == 0)
                return CheckMatch(pkm, abilities, data.Info.Generation, AbilityState.MustMatch);

            if (IsAbilityCapsuleModified(pkm, abilities, EncounterAbility))
                return GetValid(LAbilityCapsuleUsed);

            return INVALID;
        }

        private AbilityState VerifyAbilityPreCapsule(LegalityAnalysis data, IReadOnlyList<int> abilities)
        {
            var pkm = data.pkm;
            // CXD pokemon can have any ability without matching PID
            if (pkm.Version == (int)GameVersion.CXD && pkm.Format == 3)
                return AbilityState.CanMismatch;

            // Gen3 native or Gen4/5 origin
            if (pkm.Format == 3 || !pkm.InhabitedGeneration(3))
                return AbilityState.MustMatch;

            // Evovled in Gen4/5
            if (pkm.Species > Legal.MaxSpeciesID_3)
                return AbilityState.MustMatch;

            // If the species could not exist in Gen3, must match.
            if (data.Info.EvoChainsAllGens[3].Count == 0)
                return AbilityState.MustMatch;

            // Fall through when gen3 pkm transferred to gen4/5
            return VerifyAbilityGen3Transfer(data, abilities, data.Info.EvoChainsAllGens[3][0].Species);
        }

        private AbilityState VerifyAbilityGen3Transfer(LegalityAnalysis data, IReadOnlyList<int> abilities, int maxGen3Species)
        {
            var pkm = data.pkm;
            var pers = (PersonalInfoG3)PersonalTable.E[maxGen3Species];
            if (pers.Ability1 != pers.Ability2) // Excluding Colosseum/XD, a Gen3 pkm must match PID if it has 2 unique abilities
                return pkm.Version == (int) GameVersion.CXD ? AbilityState.CanMismatch : AbilityState.MustMatch;

            if (pkm.Species != maxGen3Species) // it has evolved in either gen 4 or gen 5; the ability must match PID
                return AbilityState.MustMatch;

            var chain = data.Info.EvoChainsAllGens;
            bool evolved45 = chain[4].Count > 1 || (pkm.Format == 5 && chain[5].Count > 1);
            if (evolved45)
            {
                if (pkm.Ability == pers.Ability1) // Could evolve in Gen4/5 and have a Gen3 only ability
                    return AbilityState.CanMismatch; // Not evolved in Gen4/5, doesn't need to match PIDAbility

                if (pkm.Ability == abilities[1]) // It could evolve in Gen4/5 and have Gen4 second ability
                    return AbilityState.MustMatch; // Evolved in Gen4/5, must match PIDAbility
            }

            // If we reach here, it has not evolved in Gen4/5 games or has an invalid ability.
            // The ability does not need to match the PIDAbility, but only Gen3 ability is allowed.
            if (pkm.Ability != pers.Ability1) // Not evolved in Gen4/5, but doesn't have Gen3 only ability
                data.AddLine(GetInvalid(LAbilityMismatch3)); // probably bad to do this here

            return AbilityState.CanMismatch;
        }

        private CheckResult VerifyAbilityMG(LegalityAnalysis data, IReadOnlyList<int> abilities, int cardtype)
        {
            if (data.EncounterMatch is PCD d)
                return VerifyAbilityPCD(data, abilities, d);

            var pkm = data.pkm;
            if (data.EncounterMatch is PGT) // Ranger Manaphy
                return (pkm.Format >= 6 ? (pkm.AbilityNumber == 1) : (pkm.AbilityNumber < 4)) ? VALID : GetInvalid(LAbilityMismatchGift);

            int abilNumber = pkm.AbilityNumber;
            if (cardtype == 4) // 1/2/H
                return VALID;
            if (cardtype == 3) // 1/2
                return abilNumber == 4 ? GetInvalid(LAbilityMismatchGift) : VALID;

            // Only remaining matches are fixed index abilities
            int cardAbilIndex = 1 << cardtype;
            if (abilNumber == cardAbilIndex)
                return VALID;

            // Can still match if the ability was changed via ability capsule...
            // However, it can't change to/from Hidden Abilities.
            if (abilNumber == 4 || cardtype == 2)
                return GetInvalid(LAbilityHiddenFail);

            // Ability can be flipped 0/1 if Ability Capsule is available, is not Hidden Ability, and Abilities are different.
            if (pkm.Format >= 6 && abilities[0] != abilities[1])
                return GetValid(LAbilityCapsuleUsed);

            return GetInvalid(pkm.Format < 6 ? LAbilityMismatchPID : LAbilityMismatchFlag);
        }

        private CheckResult VerifyAbilityPCD(LegalityAnalysis data, IReadOnlyList<int> abilities, PCD pcd)
        {
            var pkm = data.pkm;
            if (pkm.Format >= 6)
            {
                if (abilities[0] == abilities[1])
                {
                    // Gen3-5 transfer with same ability -> 1st ability that matches
                    if (pkm.AbilityNumber == 1)
                        return GetValid(LAbilityFlag);
                    return CheckMatch(pkm, abilities, 4, AbilityState.MustMatch); // evolved, must match
                }
                if (pkm.AbilityNumber < 4) // Ability Capsule can change between 1/2
                    return GetValid(LAbilityCapsuleUsed);
            }

            if (pcd.Species != pkm.Species)
                return CheckMatch(pkm, abilities, 4, AbilityState.MustMatch); // evolved, must match

            // Edge case (PID ability gift mismatch) -- must match gift ability.
            return pkm.Ability == pcd.Gift.PK.Ability ? VALID : INVALID;
        }

        private CheckResult VerifyAbility5(LegalityAnalysis data, IReadOnlyList<int> abilities)
        {
            var pkm = data.pkm;
            switch (data.EncounterMatch)
            {
                case EncounterSlot w:
                    // Hidden Abilities for Wild Encounters are only available at a Hidden Grotto
                    bool grotto = w.Type == SlotType.HiddenGrotto;
                    if (pkm.AbilityNumber == 4 ^ grotto)
                        return GetInvalid(grotto ? LAbilityMismatchGrotto : LAbilityHiddenFail);
                    break;

                case EncounterEgg e when pkm.AbilityNumber == 4:
                    // Hidden Abilities for some are unbreedable or unreleased
                    if (Legal.Ban_BreedHidden5.Contains(e.Species))
                        return GetInvalid(LAbilityHiddenUnavailable);
                    break;
            }
            var state = pkm.Format == 5 ? AbilityState.MustMatch : AbilityState.CanMismatch;
            return CheckMatch(data.pkm, abilities, 5, state);
        }

        private CheckResult VerifyAbility6(LegalityAnalysis data)
        {
            var pkm = data.pkm;
            if (pkm.AbilityNumber != 4)
                return VALID;

            // hidden abilities
            var EncounterMatch = data.EncounterMatch;
            if (EncounterMatch is EncounterSlot slot)
            {
                bool valid = slot.Permissions.DexNav || slot.Type == SlotType.FriendSafari || slot.Type == SlotType.Horde;
                if (!valid)
                    return GetInvalid(LAbilityMismatchHordeSafari);
            }
            if (Legal.Ban_NoHidden6.Contains(pkm.SpecForm))
                return GetInvalid(LAbilityHiddenUnavailable);

            return VALID;
        }

        private CheckResult VerifyAbility7(LegalityAnalysis data)
        {
            var pkm = data.pkm;
            var EncounterMatch = data.EncounterMatch;
            if (EncounterMatch is EncounterSlot slot && pkm.AbilityNumber == 4)
            {
                bool valid = slot.Type == SlotType.SOS;
                if (!valid)
                    return GetInvalid(LAbilityMismatchSOS);
            }
            if (Legal.Ban_NoHidden7.Contains(pkm.SpecForm) && pkm.AbilityNumber == 4)
                return GetInvalid(LAbilityHiddenUnavailable);

            return VALID;
        }

        private CheckResult VerifyAbility8(LegalityAnalysis data)
        {
            var pkm = data.pkm;
            var EncounterMatch = data.EncounterMatch;
            if (EncounterMatch is EncounterSlot && pkm.AbilityNumber == 4)
                return GetInvalid(LAbilityHiddenUnavailable);
            if (Legal.Ban_NoHidden8.Contains(pkm.SpecForm) && pkm.AbilityNumber == 4)
                return GetInvalid(LAbilityHiddenUnavailable);

            return VALID;
        }

        /// <summary>
        /// Final checks assuming nothing else has flagged the ability.
        /// </summary>
        /// <param name="pkm">Pokémon</param>
        /// <param name="abilities">Current abilities</param>
        /// <param name="gen">Generation</param>
        /// <param name="state">Permissive to allow ability to deviate under special circumstances</param>
        private CheckResult CheckMatch(PKM pkm, IReadOnlyList<int> abilities, int gen, AbilityState state)
        {
            if (3 <= gen && gen <= 4 && pkm.AbilityNumber == 4)
                return GetInvalid(LAbilityHiddenUnavailable);

            // other cases of hidden ability already flagged, all that is left is 1/2 mismatching
            if (state == AbilityState.MustMatch && abilities[pkm.AbilityNumber >> 1] != pkm.Ability)
                return GetInvalid(pkm.Format < 6 ? LAbilityMismatchPID : LAbilityMismatchFlag);

            return VALID;
        }

        // Ability Capsule can change between 1/2
        private static bool IsAbilityCapsuleModified(PKM pkm, IReadOnlyList<int> abilities, int EncounterAbility)
        {
            if (pkm.Format < 6)
                return false; // Ability Capsule does not exist
            if (abilities[0] == abilities[1])
                return false; // Cannot alter ability index if it is the same as the other ability.
            if (pkm.AbilityNumber == 4)
                return false; // Cannot alter to hidden ability.
            if (EncounterAbility == 4)
                return false; // Cannot alter from hidden ability.
            return true;
        }

        private static int GetEncounterFixedAbilityNumber(IEncounterable enc)
        {
            return enc switch
            {
                EncounterStatic s => s.Ability,
                EncounterTrade t => t.Ability,
                _ => -1
            };
        }
    }
}
