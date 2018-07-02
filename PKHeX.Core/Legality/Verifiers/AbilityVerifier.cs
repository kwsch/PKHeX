using System;
using System.Collections.Generic;
using System.Linq;
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
            var pkm = data.pkm;
            var Info = data.Info;
            var EncounterMatch = data.EncounterMatch;

            int[] abilities = pkm.PersonalInfo.Abilities;
            int abilval = Array.IndexOf(abilities, pkm.Ability);
            if (abilval < 0)
            {
                data.AddLine(GetInvalid(V107));
                return;
            }

            bool? AbilityUnchanged = true;
            // 3 states flag: true for unchanged, false for changed, null for uncertain/allowing PID mismatch
            // if true, check encounter ability
            // if true or false, check PID/AbilityNumber
            if (3 <= pkm.Format && pkm.Format <= 5 && abilities[0] != abilities[1]) // 3-5 and have 2 distinct ability now
                AbilityUnchanged = VerifyAbilityPreCapsule(data, abilities, abilval);

            if (EncounterMatch is PCD d)
            {
                if (VerifyAbilityPCD(data, d, AbilityUnchanged, abilities))
                    return;
            }
            else // Check Ability Mismatches
            {
                int? EncounterAbility = (EncounterMatch as EncounterStatic)?.Ability ??
                                        (EncounterMatch as EncounterTrade)?.Ability ??
                                        (EncounterMatch as EncounterLink)?.Ability;

                if (EncounterAbility != null && VerifySetAbility(data, EncounterAbility, AbilityUnchanged, abilities, abilval))
                    return; // result added via VerifySetAbility
            }
            switch (Info.Generation)
            {
                case 5: VerifyAbility5(data, abilities); break;
                case 6: VerifyAbility6(data, abilities); break;
                case 7: VerifyAbility7(data, abilities); break;
            }

            if (3 <= Info.Generation && Info.Generation <= 4 && pkm.AbilityNumber == 4)
                data.AddLine(GetInvalid(V112));
            else if (AbilityUnchanged != null && abilities[pkm.AbilityNumber >> 1] != pkm.Ability)
                data.AddLine(GetInvalid(pkm.Format < 6 ? V113 : V114));
            else
                data.AddLine(GetValid(V115));
        }
        private bool VerifyAbilityPCD(LegalityAnalysis data, PCD pcd, bool? abilityUnchanged, int[] abilities)
        {
            var pkm = data.pkm;
            if (pcd.Species == pkm.Species && pkm.Ability == pcd.Gift.PK.Ability) // Edge case (PID ability gift mismatch)
                data.AddLine(GetValid(V115));
            else if (pkm.Format >= 6 && abilities[0] == abilities[1] && pkm.AbilityNumber == 1)
                data.AddLine(GetValid(V115)); // gen3-5 transfer with same ability -> 1st ability that matches
            else if (pkm.Format >= 6 && abilities[0] != abilities[1] && pkm.AbilityNumber < 4) // Ability Capsule can change between 1/2
                data.AddLine(GetValid(V109));
            else
            {
                if (!(abilityUnchanged ?? false))
                    return false;
                data.AddLine(GetInvalid(V223));
            }
            return true;
        }
        private bool VerifySetAbility(LegalityAnalysis data, int? EncounterAbility, bool? AbilityUnchanged, int[] abilities, int abilval)
        {
            var pkm = data.pkm;
            if (pkm.AbilityNumber == 4 && EncounterAbility != 4)
            {
                data.AddLine(GetInvalid(V108));
                return true;
            }

            if (data.EncounterMatch is EncounterTradePID z)
            {
                if (z.Species != pkm.Species)
                    return false; // Must match PID ability, handle via default check path
                if (EncounterAbility == 1 << abilval)
                {
                    data.AddLine(GetValid(V115));
                    return true;
                }
            }

            if (!(AbilityUnchanged ?? false) || EncounterAbility == 0 || pkm.AbilityNumber == EncounterAbility)
                return false;
            if (IsAbilityCapsuleModified(pkm, abilities, EncounterAbility))
                data.AddLine(GetValid(V109));
            else
                data.AddLine(GetInvalid(V223));
            return true;
        }

        // Ability Capsule can change between 1/2
        private static bool IsAbilityCapsuleModified(PKM pkm, IReadOnlyList<int> abilities, int? EncounterAbility)
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
        private bool? VerifyAbilityPreCapsule(LegalityAnalysis data, int[] abilities, int abilval)
        {
            var pkm = data.pkm;
            // CXD pokemon could have any ability without maching PID
            if (pkm.Version == (int)GameVersion.CXD && pkm.Format == 3)
                return null;

            // gen3 native or gen4/5 origin
            if (pkm.Format == 3 || !pkm.InhabitedGeneration(3))
                return true;

            // Evovled in gen4/5
            if (pkm.Species > Legal.MaxSpeciesID_3)
                return false;

            // gen3Species will be zero for pokemon with illegal gen 3 encounters, like Infernape with gen 3 "origin"
            var gen3Species = data.Info.EvoChainsAllGens[3].FirstOrDefault()?.Species ?? 0;
            if (gen3Species == 0)
                return true;

            // Fall through when gen3 pkm transferred to gen4/5
            return VerifyAbilityGen3Transfer(data, abilities, abilval, gen3Species);
        }
        private bool? VerifyAbilityGen3Transfer(LegalityAnalysis data, int[] abilities, int abilval, int Species_g3)
        {
            var pkm = data.pkm;
            var Info = data.Info;
            var pers = (PersonalInfoG3)PersonalTable.E[Species_g3];
            if (pers.Ability1 != pers.Ability2) // Excluding Colosseum/XD, a gen3 pkm must match PID if it has 2 unique abilities
                return pkm.Version != (int)GameVersion.CXD;

            int Species_g4 = Info.EvoChainsAllGens[4].FirstOrDefault()?.Species ?? 0;
            int Species_g5 = pkm.Format == 5 ? Info.EvoChainsAllGens[5].FirstOrDefault()?.Species ?? 0 : 0;
            if (Math.Max(Species_g5, Species_g4) > Species_g3) // it has evolved in either gen 4 or gen 5; the ability must match PID
                return false;

            var Evolutions_g45 = Math.Max(Info.EvoChainsAllGens[4].Count, pkm.Format == 5 ? Info.EvoChainsAllGens[5].Count : 0);
            if (Evolutions_g45 > 1)
            {
                // Evolutions_g45 > 1 and Species_g45 = Species_g3 with means both options, evolve in gen 4-5 or not evolve, are possible
                if (pkm.Ability == pers.Ability1)
                    // It could evolve in gen 4-5 an have generation 3 only ability
                    // that means it have not actually evolved in gen 4-5, ability do not need to match PID
                    return null;
                if (pkm.Ability == abilities[1])
                    // It could evolve in gen4-5 an have generation 4 second ability
                    // that means it have actually evolved in gen 4-5, ability must match PID
                    return false;
            }
            // Evolutions_g45 == 1 means it have not evolved in gen 4-5 games,
            // ability do not need to match PID, but only generation 3 ability is allowed
            if (pkm.Ability != pers.Ability1)
                // Not evolved in gen4-5 but do not have generation 3 only ability
                data.AddLine(GetInvalid(V373));
            return null;
        }
        private void VerifyAbility5(LegalityAnalysis data, int[] abilities)
        {
            var pkm = data.pkm;
            switch (data.EncounterMatch)
            {
                case PGF g:
                    VerifyAbilityMG456(data, abilities, g.AbilityType);
                    break;

                case EncounterSlot w:
                    // Hidden Abilities for Wild Encounters are only available at a Hidden Grotto
                    bool grotto = w.Type == SlotType.HiddenGrotto;
                    if (pkm.AbilityNumber == 4 ^ grotto)
                        data.AddLine(GetInvalid(grotto ? V217 : V108));
                    break;

                case EncounterEgg e when pkm.AbilityNumber == 4:
                    // Hidden Abilities for some are unbreedable (male only distribution)
                    if (Legal.MixedGenderBreeding.Contains(e.Species) || Legal.FixedGenderFromBiGender.Contains(e.Species))
                        break; // from female
                    if ((pkm.PersonalInfo.Gender & 0xFF) == 0 || Legal.Ban_BreedHidden.Contains(e.Species))
                        data.AddLine(GetInvalid(V112));
                    break;
            }
        }
        private void VerifyAbility6(LegalityAnalysis data, int[] abilities)
        {
            var pkm = data.pkm;
            var EncounterMatch = data.EncounterMatch;
            if (EncounterMatch is EncounterSlot slot && pkm.AbilityNumber == 4)
            {
                bool valid = slot.Permissions.DexNav || slot.Type == SlotType.FriendSafari || slot.Type == SlotType.Horde;
                if (!valid)
                    data.AddLine(GetInvalid(V300));
            }
            else if (EncounterMatch is WC6 g)
                VerifyAbilityMG456(data, abilities, g.AbilityType);
            else if (Legal.Ban_NoHidden6.Contains(pkm.SpecForm) && pkm.AbilityNumber == 4)
                data.AddLine(GetInvalid(V112));
        }
        private void VerifyAbility7(LegalityAnalysis data, int[] abilities)
        {
            var pkm = data.pkm;
            var EncounterMatch = data.EncounterMatch;
            if (EncounterMatch is EncounterSlot slot && pkm.AbilityNumber == 4)
            {
                bool valid = slot.Type == SlotType.SOS;
                if (!valid)
                    data.AddLine(GetInvalid(V111));
            }
            else if (EncounterMatch is WC7 g)
                VerifyAbilityMG456(data, abilities, g.AbilityType);
            else if (Legal.Ban_NoHidden7.Contains(pkm.SpecForm) && pkm.AbilityNumber == 4)
                data.AddLine(GetInvalid(V112));
        }
        private void VerifyAbilityMG456(LegalityAnalysis data, int[] abilities, int cardtype)
        {
            var pkm = data.pkm;
            int abilNumber = pkm.AbilityNumber;
            if (cardtype < 3 && abilNumber != 1 << cardtype) // set number
            {
                // Ability can be flipped 0/1 if Ability Capsule is available, is not Hidden Ability, and Abilities are different.
                if (pkm.Format >= 6 && cardtype < 2 && abilNumber < 3 && abilities[0] != abilities[1])
                    data.AddLine(GetValid(V109));
                else
                    data.AddLine(GetInvalid(V110));
            }
            else if (cardtype == 3 && abilNumber == 4) // 1/2 only
                data.AddLine(GetInvalid(V110));
        }
    }
}
