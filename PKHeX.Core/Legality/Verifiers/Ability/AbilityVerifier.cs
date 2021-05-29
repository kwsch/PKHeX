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
            var pi = data.PersonalInfo;

            // Check ability is possible (within bounds)
            int ability = pkm.Ability;
            int abilval = pi.GetAbilityIndex(ability);
            if (abilval < 0)
                return GetInvalid(LAbilityUnexpected);

            var abilities = pi.Abilities;
            int format = pkm.Format;
            if (format >= 6)
            {
                // Check AbilityNumber is a single set bit
                var num = pkm.AbilityNumber;
                if (!(num != 0 && (num & (num - 1)) == 0)) // not [!zero, and power of 2]
                    return GetInvalid(LAbilityMismatchFlag);

                // Check AbilityNumber points to ability
                int an = num >> 1;
                if (an >= abilities.Count || abilities[an] != ability)
                    return GetInvalid(LAbilityMismatchFlag);

                // Check AbilityNumber for transfers without unique abilities
                int gen = data.Info.Generation;
                if (gen is 3 or 4 or 5 && num != 4)
                {
                    // To determine AbilityNumber [PK5->PK6], check if the first ability in Personal matches the ability.
                    // It is not possible to flip it to the other index as capsule requires unique abilities.
                    if (abilities[0] == abilities[1] && num != 1)
                    {
                        // Check if any pre-evolution could have it flipped.
                        var evos = data.Info.EvoChainsAllGens[6];
                        var pt = GameData.GetPersonal(GameUtil.GetVersion(pkm.Format));
                        if (!GetWasDual(evos, pt, pkm))
                            return GetInvalid(LAbilityMismatchFlag);
                    }
                }
            }

            if (format >= 8) // Ability Patch
            {
                if (pkm.AbilityNumber == 4)
                {
                    if (CanAbilityPatch(format, abilities, pkm.Species))
                        return GetValid(LAbilityPatchUsed);

                    var e = data.EncounterOriginal;
                    if (e.Species != pkm.Species && CanAbilityPatch(format, PKX.Personal.GetFormEntry(e.Species, e.Form).Abilities, e.Species))
                        return GetValid(LAbilityPatchUsed);
                }
            }

            var enc = data.EncounterMatch;
            if (enc is MysteryGift {Generation: >= 4} g)
                return VerifyAbilityMG(data, g, abilities);

            if (format < 6)
                return VerifyAbility345(data, enc, abilities, abilval);

            return VerifyAbility(data, abilities, abilval);
        }

        private static bool GetWasDual(IReadOnlyList<EvoCriteria> evos, PersonalTable pt, ISpeciesForm pk)
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

        private CheckResult VerifyAbility(LegalityAnalysis data, IReadOnlyList<int> abilities, int abilnum)
        {
            var enc = data.EncounterMatch;
            var eabil = GetEncounterFixedAbilityNumber(enc);
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
                5 => VerifyAbility5(data, enc, abilities),
                6 => VerifyAbility6(data, enc),
                7 => VerifyAbility7(data, enc),
              >=8 => VALID,
                _ => CheckMatch(data.pkm, abilities, gen, AbilityState.CanMismatch)
            };
        }

        private CheckResult VerifyAbility345(LegalityAnalysis data, IEncounterable enc, IReadOnlyList<int> abilities, int abilnum)
        {
            var pkm = data.pkm;
            int format = pkm.Format;
            var state = AbilityState.MustMatch;
            if (format is (3 or 4 or 5) && abilities[0] != abilities[1]) // 3-4/5 and have 2 distinct abilities now
                state = VerifyAbilityPreCapsule(data, abilities);

            int eabil = GetEncounterFixedAbilityNumber(enc);
            if (eabil >= 0)
            {
                if ((pkm.AbilityNumber == 4) != (eabil == 4))
                    return GetInvalid(LAbilityHiddenFail);
                if (eabil > 0)
                    return VerifyFixedAbility(data, abilities, state, eabil, abilnum);
            }

            int gen = data.Info.Generation;
            if (gen == 5)
                return VerifyAbility5(data, enc, abilities);

            return CheckMatch(pkm, abilities, gen, state);
        }

        private CheckResult VerifyFixedAbility(LegalityAnalysis data, IReadOnlyList<int> abilities, AbilityState state, int EncounterAbility, int abilval)
        {
            var pkm = data.pkm;
            if (data.Info.EncounterMatch.Generation >= 6)
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
            {
                // Evolving in Gen3 does not mutate the ability bit, so any mismatched abilities will stay mismatched.
                if (pkm.Gen3)
                {
                    if (EncounterAbility == 1 << abilval)
                        return GetValid(LAbilityFlag);

                    // If it is in a future game and does not match the fixed ability, then it must match the PID.
                    if (pkm.Format != 3)
                        return GetPIDAbilityMatch(pkm, abilities);

                    // No way to un-mismatch it while existing solely on Gen3 games.
                    return INVALID;
                }

                return CheckMatch(pkm, abilities, data.Info.Generation, AbilityState.MustMatch);
            }

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
            if (pkm.Format == 3 || data.Info.Generation != 3)
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

        private CheckResult VerifyAbilityMG(LegalityAnalysis data, MysteryGift g, IReadOnlyList<int> abilities)
        {
            if (g is PCD d)
                return VerifyAbilityPCD(data, abilities, d);

            var pkm = data.pkm;
            if (g is PGT) // Ranger Manaphy
                return (pkm.Format >= 6 ? (pkm.AbilityNumber == 1) : (pkm.AbilityNumber < 4)) ? VALID : GetInvalid(LAbilityMismatchGift);

            var cardType = g.AbilityType;
            if (cardType == 4) // 1/2/H
                return VALID;
            int abilNumber = pkm.AbilityNumber;
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
            if (pkm.Format >= 6)
            {
                if (CanAbilityCapsule(6, abilities))
                    return GetValid(LAbilityCapsuleUsed);

                // Maybe was evolved after using ability capsule.
                var evos = data.Info.EvoChainsAllGens[pkm.Format];
                if (GetWasDual(evos, PKX.Personal, pkm))
                    return GetValid(LAbilityCapsuleUsed);
            }

            return GetInvalid(pkm.Format < 6 ? LAbilityMismatchPID : LAbilityMismatchFlag);
        }

        private CheckResult VerifyAbilityPCD(LegalityAnalysis data, IReadOnlyList<int> abilities, PCD pcd)
        {
            var pkm = data.pkm;
            var format = pkm.Format;
            if (format >= 6)
            {
                if (CanAbilityCapsule(format, abilities))
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

        private CheckResult VerifyAbility5(LegalityAnalysis data, IEncounterable enc, IReadOnlyList<int> abilities)
        {
            var pkm = data.pkm;
            switch (enc)
            {
                case EncounterSlot w:
                    // Hidden Abilities for Wild Encounters are only available at a Hidden Grotto
                    bool grotto = w.Area.Type == SlotType.HiddenGrotto;
                    if (pkm.AbilityNumber == 4 ^ grotto)
                        return GetInvalid(grotto ? LAbilityMismatchGrotto : LAbilityHiddenFail);
                    break;

                case EncounterEgg e when pkm.AbilityNumber == 4:
                    // Hidden Abilities for some are unbreedable or unreleased
                    if (AbilityBreedLegality.BanHidden5.Contains(e.Species))
                        return GetInvalid(LAbilityHiddenUnavailable);
                    break;
            }
            var state = pkm.Format == 5 ? AbilityState.MustMatch : AbilityState.CanMismatch;
            return CheckMatch(data.pkm, abilities, 5, state);
        }

        private CheckResult VerifyAbility6(LegalityAnalysis data, IEncounterable enc)
        {
            var pkm = data.pkm;
            if (pkm.AbilityNumber != 4)
                return VALID;

            // hidden abilities
            if (enc is EncounterSlot slot)
            {
                bool valid = slot is EncounterSlot6AO {CanDexNav: true} || slot.Area.Type is SlotType.FriendSafari or SlotType.Horde;
                if (!valid)
                    return GetInvalid(LAbilityMismatchHordeSafari);
            }
            if (AbilityBreedLegality.BanHidden6.Contains(enc.Species | (enc.Form << 11)))
                return GetInvalid(LAbilityHiddenUnavailable);

            return VALID;
        }

        private CheckResult VerifyAbility7(LegalityAnalysis data, IEncounterable enc)
        {
            var pkm = data.pkm;
            if (enc is EncounterSlot slot && pkm.AbilityNumber == 4)
            {
                bool valid = slot.Area.Type == SlotType.SOS;
                if (!valid)
                    return GetInvalid(LAbilityMismatchSOS);
            }
            if (AbilityBreedLegality.BanHidden7.Contains(enc.Species | (enc.Form << 11)) && pkm.AbilityNumber == 4)
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
            if (gen is (3 or 4) && pkm.AbilityNumber == 4)
                return GetInvalid(LAbilityHiddenUnavailable);

            // other cases of hidden ability already flagged, all that is left is 1/2 mismatching
            if (state != AbilityState.MustMatch)
                return VALID;

            // Check that the ability bit is correct.
            if (pkm is G3PKM g3)
            {
                var abit = g3.AbilityBit;
                if (abilities[0] == abilities[1]) // Not a dual ability
                {
                    // Must not have the Ability bit flag set.
                    // Some shadow stuff with single-ability might have the flag set anyways?
                    if (abit && !(pkm is IShadowPKM {ShadowID: not 0}))
                        return GetInvalid(LAbilityMismatchFlag, CheckIdentifier.PID);
                }
                else
                {
                    // Gen3 mainline origin sets the Ability index based on the PID, but only if it has two abilities.
                    // Version value check isn't factually correct, but there are no C/XD gifts with (Version!=15) that have two abilities.
                    // Pikachu, Celebi, Ho-Oh
                    if (pkm.Version != (int)GameVersion.CXD && abit != ((pkm.PID & 1) == 1))
                        return GetInvalid(LAbilityMismatchPID, CheckIdentifier.PID);
                }
            }
            else if (pkm.Format >= 6)
            {
                // 6+ already checked at the top of the verifier call stack
                return VALID;
            }

            // 3-5
            return GetPIDAbilityMatch(pkm, abilities);
        }

        private CheckResult GetPIDAbilityMatch(PKM pkm, IReadOnlyList<int> abilities)
        {
            var abil = abilities[pkm.AbilityNumber >> 1];
            if (abil != pkm.Ability)
                return GetInvalid(LAbilityMismatchPID);

            return VALID;
        }

        // Ability Capsule can change between 1/2
        private static bool IsAbilityCapsuleModified(PKM pkm, IReadOnlyList<int> abilities, int EncounterAbility)
        {
            if (!CanAbilityCapsule(pkm.Format, abilities))
                return false;
            if (pkm.AbilityNumber == 4)
                return false; // Cannot alter to hidden ability.
            if (EncounterAbility == 4)
                return false; // Cannot alter from hidden ability.
            return true;
        }

        private static bool CanAbilityCapsule(int format, IReadOnlyList<int> abilities)
        {
            if (format < 6) // Ability Capsule does not exist
                return false;
            return abilities[0] != abilities[1]; // Cannot alter ability index if it is the same as the other ability.
        }

        public static bool CanAbilityPatch(int format, IReadOnlyList<int> abilities, int species)
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
                _ => false
            };
        }

        private static int GetEncounterFixedAbilityNumber(IEncounterable enc) => enc switch
        {
            EncounterStatic s => s.Ability,
            EncounterTrade t => t.Ability,
            _ => -1
        };
    }
}
