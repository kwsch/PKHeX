using System.Collections.Generic;
using static PKHeX.Core.LegalityCheckStrings;
using static PKHeX.Core.Ball;

namespace PKHeX.Core
{
    /// <summary>
    /// Verifies the <see cref="PKM.Ball"/> value.
    /// </summary>
    public sealed class BallVerifier : Verifier
    {
        protected override CheckIdentifier Identifier => CheckIdentifier.Ball;
        private CheckResult NONE => GetInvalid(LBallNone);

        public override void Verify(LegalityAnalysis data)
        {
            if (data.pkm.Format <= 2)
                return; // no ball info saved
            var result = VerifyBall(data);
            data.AddLine(result);
        }

        private CheckResult VerifyBall(LegalityAnalysis data)
        {
            var Info = data.Info;
            var enc = Info.EncounterMatch;

            // Fixed ball cases -- can be only one ball ever
            switch (enc)
            {
                case MysteryGift g:
                    return VerifyBallMysteryGift(data, g);
                case EncounterTrade t:
                    return VerifyBallEquals(data, t.Ball);
                case EncounterStatic {Gift: true} s:
                    return VerifyBallEquals(data, s.Ball);
                case EncounterSlot8GO: // Already a strict match
                    return GetResult(true);
            }

            // Capture / Inherit cases -- can be one of many balls
            var pkm = data.pkm;
            if (pkm.Species == (int)Species.Shedinja && data.EncounterMatch.Species != (int)Species.Shedinja) // Shedinja. For gen3, copy the ball from Nincada
            {
                // Only Gen3 origin Shedinja can copy the wild ball.
                // Evolution chains will indicate if it could have existed as Shedinja in Gen3.
                // The special move verifier has a similar check!
                if (Info.Generation != 3 || Info.EvoChainsAllGens[3].Count != 2)
                    return VerifyBallEquals(data, (int)Poke); // Pokeball Only
            }

            if (pkm.Ball == (int)Heavy && BallBreedLegality.AlolanCaptureNoHeavyBall.Contains(enc.Species) && !enc.EggEncounter && pkm.SM)
                return GetInvalid(LBallHeavy); // Heavy Ball, can inherit if from egg (USUM fixed catch rate calc)

            return enc switch
            {
                EncounterStatic e => VerifyBallStatic(data, e),
                EncounterSlot w => VerifyBallWild(data, w),
                EncounterEgg => VerifyBallEgg(data),
                EncounterInvalid => VerifyBallEquals(data, pkm.Ball), // ignore ball, pass whatever
                _ => VerifyBallEquals(data, (int)Poke)
            };
        }

        private CheckResult VerifyBallMysteryGift(LegalityAnalysis data, MysteryGift g)
        {
            if (g.Generation == 4 && g.Species == (int)Species.Manaphy && g.Ball == 0) // there is no ball data in Manaphy Mystery Gift from Gen4
                return VerifyBallEquals(data, (int)Poke); // Pokeball
            return VerifyBallEquals(data, g.Ball);
        }

        private CheckResult VerifyBallStatic(LegalityAnalysis data, EncounterStatic s)
        {
            if (s.Location == 75 && s.Generation == 5) // Entree Forest (Dream World)
                return VerifyBallEquals(data, BallUseLegality.DreamWorldBalls);
            return VerifyBallEquals(data, BallUseLegality.GetWildBalls(data.Info.Generation, data.Info.Game));
        }

        private CheckResult VerifyBallWild(LegalityAnalysis data, EncounterSlot w)
        {
            var req = w.Area.Type.GetRequiredBallValueWild(w.Generation, w.Location);
            if (req != None)
                return VerifyBallEquals(data, (int) req);

            return VerifyBallEquals(data, BallUseLegality.GetWildBalls(data.Info.Generation, data.Info.Game));
        }

        private CheckResult VerifyBallEgg(LegalityAnalysis data)
        {
            var pkm = data.pkm;
            if (data.Info.Generation < 6) // No inheriting Balls
                return VerifyBallEquals(data, (int)Poke); // Must be Pokéball -- no ball inheritance.

            return pkm.Ball switch
            {
                (int)Poke => GetValid(LBallEnc), // Poké Ball
                (int)Master => GetInvalid(LBallEggMaster), // Master Ball
                (int)Cherish => GetInvalid(LBallEggCherish), // Cherish Ball
                _ => VerifyBallInherited(data)
            };
        }

        private CheckResult VerifyBallInherited(LegalityAnalysis data) => data.Info.Generation switch
        {
            6 => VerifyBallEggGen6(data), // Gen6 Inheritance Rules
            7 => VerifyBallEggGen7(data), // Gen7 Inheritance Rules
            8 => VerifyBallEggGen8(data),
            _ => NONE
        };

        private CheckResult VerifyBallEggGen6(LegalityAnalysis data)
        {
            var pkm = data.pkm;
            var enc = data.EncounterMatch;
            int species = enc.Species;
            if (pkm.Gender == 2 || BallBreedLegality.BreedMaleOnly6.Contains(species)) // Genderless
                return VerifyBallEquals(data, (int)Poke); // Must be Pokéball as ball can only pass via mother (not Ditto!)

            Ball ball = (Ball)pkm.Ball;

            if (ball == Safari) // Safari Ball
            {
                if (!BallBreedLegality.Inherit_Safari.Contains(species))
                    return GetInvalid(LBallSpecies);
                if (IsHiddenAndNotPossible(pkm))
                    return GetInvalid(LBallAbility);
                return GetValid(LBallSpeciesPass);
            }
            if (ball.IsApricornBall()) // Apricorn Ball
            {
                if (!BallBreedLegality.Inherit_Apricorn6.Contains(species))
                    return GetInvalid(LBallSpecies);
                if (IsHiddenAndNotPossible(pkm))
                    return GetInvalid(LBallAbility);
                return GetValid(LBallSpeciesPass);
            }
            if (ball == Sport) // Sport Ball
            {
                if (!BallBreedLegality.Inherit_Sport.Contains(species))
                    return GetInvalid(LBallSpecies);
                if (IsHiddenAndNotPossible(pkm))
                    return GetInvalid(LBallAbility);
                return GetValid(LBallSpeciesPass);
            }
            if (ball == Dream) // Dream Ball
            {
                if (BallBreedLegality.Ban_DreamHidden.Contains(species) && IsHiddenAndNotPossible(pkm))
                    return GetInvalid(LBallAbility);
                if (BallBreedLegality.Inherit_Dream.Contains(species))
                    return GetValid(LBallSpeciesPass);
                return GetInvalid(LBallSpecies);
            }
            if (Dusk <= ball && ball <= Quick) // Dusk Heal Quick
            {
                if (!BallBreedLegality.Ban_Gen4Ball_6.Contains(species))
                    return GetValid(LBallSpeciesPass);
                return GetInvalid(LBallSpecies);
            }
            if (Ultra <= ball && ball <= Premier) // Don't worry, Safari was already checked.
            {
                if (BallBreedLegality.Ban_Gen3Ball.Contains(species))
                    return GetInvalid(LBallSpecies);
                if (BallBreedLegality.Ban_Gen3BallHidden.Contains(species | (enc.Form << 11)) && IsHiddenAndNotPossible(pkm))
                    return GetInvalid(LBallAbility);
                return GetValid(LBallSpeciesPass);
            }

            if (species > 650 && species != 700) // Sylveon
            {
                if (BallUseLegality.WildPokeballs6.Contains(pkm.Ball))
                    return GetValid(LBallSpeciesPass);
                return GetInvalid(LBallSpecies);
            }

            if (ball >= Dream)
                return GetInvalid(LBallUnavailable);

            return NONE;
        }

        private CheckResult VerifyBallEggGen7(LegalityAnalysis data)
        {
            var pkm = data.pkm;
            int species = data.EncounterMatch.Species;
            if (722 <= species && species <= 730) // G7 Starters
                return VerifyBallEquals(data, (int)Poke);

            Ball ball = (Ball)pkm.Ball;

            if (ball == Safari)
            {
                if (!(BallBreedLegality.Inherit_Safari.Contains(species) || BallBreedLegality.Inherit_SafariMale.Contains(species)))
                    return GetInvalid(LBallSpecies);
                if (BallBreedLegality.Ban_SafariBallHidden_7.Contains(species) && IsHiddenAndNotPossible(pkm))
                    return GetInvalid(LBallAbility);
                return GetValid(LBallSpeciesPass);
            }
            if (ball.IsApricornBall()) // Apricorn Ball
            {
                if (!BallBreedLegality.Inherit_Apricorn7.Contains(species))
                    return GetInvalid(LBallSpecies);
                if (BallBreedLegality.Ban_NoHidden7Apricorn.Contains(species | pkm.Form << 11) && IsHiddenAndNotPossible(pkm))
                    return GetInvalid(LBallAbility);
                return GetValid(LBallSpeciesPass);
            }
            if (ball == Sport) // Sport Ball
            {
                if (!BallBreedLegality.Inherit_Sport.Contains(species))
                    return GetInvalid(LBallSpecies);
                if ((species is (int)Species.Volbeat or (int)Species.Illumise) && IsHiddenAndNotPossible(pkm)) // Volbeat/Illumise
                    return GetInvalid(LBallAbility);
                return GetValid(LBallSpeciesPass);
            }
            if (ball == Dream) // Dream Ball
            {
                if (BallBreedLegality.Inherit_Dream.Contains(species) || BallBreedLegality.Inherit_DreamMale.Contains(species))
                    return GetValid(LBallSpeciesPass);
                return GetInvalid(LBallSpecies);
            }
            if (Dusk <= ball && ball <= Quick) // Dusk Heal Quick
            {
                if (!BallBreedLegality.Ban_Gen4Ball_7.Contains(species))
                    return GetValid(LBallSpeciesPass);
                return GetInvalid(LBallSpecies);
            }
            if (Ultra <= ball && ball <= Premier) // Don't worry, Safari was already checked.
            {
                if (!BallBreedLegality.Ban_Gen3Ball_7.Contains(species))
                    return GetValid(LBallSpeciesPass);
                return GetInvalid(LBallSpecies);
            }

            if (ball == Beast)
            {
                if (species == (int)Species.Flabébé && pkm.Form == 3 && IsHiddenAndNotPossible(pkm))
                    return GetInvalid(LBallAbility); // Can't obtain Flabébé-Blue with Hidden Ability in wild
                if (species == (int)Species.Voltorb && IsHiddenAndNotPossible(pkm))
                    return GetInvalid(LBallAbility); // Can't obtain with Hidden Ability in wild (can only breed with Ditto)
                if (((int)Species.Pikipek <= species && species <= (int)Species.Kommoo) || (BallBreedLegality.AlolanCaptureOffspring.Contains(species) && !BallBreedLegality.PastGenAlolanNativesUncapturable.Contains(species)))
                    return GetValid(LBallSpeciesPass);
                if (BallBreedLegality.PastGenAlolanScans.Contains(species))
                    return GetValid(LBallSpeciesPass);
                // next statement catches all new alolans
            }

            if (species > (int)Species.Volcanion)
                return VerifyBallEquals(data, BallUseLegality.WildPokeballs7);

            if (ball > Beast)
                return GetInvalid(LBallUnavailable);

            return NONE;
        }

        private CheckResult VerifyBallEggGen8(LegalityAnalysis data)
        {
            var pkm = data.pkm;
            int species = data.EncounterMatch.Species;
            if (722 <= species && species <= 730) // G7 Starters
                return VerifyBallEquals(data, (int)Poke);
            if ((int)Species.Grookey <= species && species <= (int)Species.Inteleon) // G8 Starters
                return VerifyBallEquals(data, (int)Poke);

            if (IsGalarCatchAndBreed(species))
            {
                if (BallUseLegality.WildPokeballs8.Contains(pkm.Ball))
                    return GetValid(LBallSpeciesPass);
                if (species >= (int)Species.Grookey)
                    return GetInvalid(LBallSpecies);
            }

            Ball ball = (Ball)pkm.Ball;

            if (ball == Safari)
            {
                if (!(BallBreedLegality.Inherit_Safari.Contains(species) || BallBreedLegality.Inherit_SafariMale.Contains(species)))
                    return GetInvalid(LBallSpecies);
                if (BallBreedLegality.Ban_SafariBallHidden_7.Contains(species) && IsHiddenAndNotPossible(pkm))
                    return GetInvalid(LBallAbility);
                return GetValid(LBallSpeciesPass);
            }
            if (ball.IsApricornBall()) // Apricorn Ball
            {
                if (!BallBreedLegality.Inherit_Apricorn7.Contains(species))
                    return GetInvalid(LBallSpecies);
                if (BallBreedLegality.Ban_NoHidden8Apricorn.Contains(species | pkm.Form << 11) && IsHiddenAndNotPossible(pkm)) // lineage is 3->2->origin
                    return GetInvalid(LBallAbility);
                return GetValid(LBallSpeciesPass);
            }
            if (ball == Sport) // Sport Ball
            {
                if (!BallBreedLegality.Inherit_Sport.Contains(species))
                    return GetInvalid(LBallSpecies);
                if ((species is (int)Species.Volbeat or (int)Species.Illumise) && IsHiddenAndNotPossible(pkm)) // Volbeat/Illumise
                    return GetInvalid(LBallAbility);
                return GetValid(LBallSpeciesPass);
            }
            if (ball == Dream) // Dream Ball
            {
                if (BallBreedLegality.Inherit_Dream.Contains(species) || BallBreedLegality.Inherit_DreamMale.Contains(species))
                    return GetValid(LBallSpeciesPass);
                return GetInvalid(LBallSpecies);
            }
            if (Dusk <= ball && ball <= Quick) // Dusk Heal Quick
            {
                if (!BallBreedLegality.Ban_Gen4Ball_7.Contains(species))
                    return GetValid(LBallSpeciesPass);
                return GetInvalid(LBallSpecies);
            }
            if (Ultra <= ball && ball <= Premier) // Don't worry, Safari was already checked.
            {
                if (!BallBreedLegality.Ban_Gen3Ball_7.Contains(species))
                    return GetValid(LBallSpeciesPass);
                return GetInvalid(LBallSpecies);
            }

            if (ball == Beast)
            {
                if (species == (int)Species.Flabébé && pkm.Form == 3 && IsHiddenAndNotPossible(pkm))
                    return GetInvalid(LBallAbility); // Can't obtain Flabébé-Blue with Hidden Ability in wild
                if (((int)Species.Pikipek <= species && species <= (int)Species.Kommoo) || (BallBreedLegality.AlolanCaptureOffspring.Contains(species) && !BallBreedLegality.PastGenAlolanNativesUncapturable.Contains(species)))
                    return GetValid(LBallSpeciesPass);
                if (BallBreedLegality.PastGenAlolanScans.Contains(species))
                    return GetValid(LBallSpeciesPass);
                // next statement catches all new alolans
            }

            if (species > Legal.MaxSpeciesID_7_USUM)
                return VerifyBallEquals(data, BallUseLegality.WildPokeballs8);

            if (species > (int)Species.Volcanion)
                return VerifyBallEquals(data, BallUseLegality.WildPokeballs7);

            if (ball > Beast)
                return GetInvalid(LBallUnavailable);

            return NONE;
        }

        private static bool IsHiddenAndNotPossible(PKM pkm)
        {
            if (pkm.AbilityNumber != 4)
                return false;
            return !AbilityVerifier.CanAbilityPatch(pkm.Format, pkm.PersonalInfo.Abilities, pkm.Species);
        }

        private static bool IsGalarCatchAndBreed(int species)
        {
            if ((int)Species.Grookey <= species && species <= (int)Species.Inteleon) // starter
                return false;

            // Everything breed-able that is in the Galar Dex can be captured in-game.
            var pt = PersonalTable.SWSH;
            var pi = (PersonalInfoSWSH) pt.GetFormEntry(species, 0);
            if (pi.IsInDex)
                return true;

            // Foreign Captures
            if ((int)Species.Treecko <= species && species <= (int)Species.Swampert) // Dynamax Adventures
                return true;

            return false;
        }

        private CheckResult VerifyBallEquals(LegalityAnalysis data, int ball) => GetResult(ball == data.pkm.Ball);
        private CheckResult VerifyBallEquals(LegalityAnalysis data, HashSet<int> balls) => GetResult(balls.Contains(data.pkm.Ball));
        private CheckResult VerifyBallEquals(LegalityAnalysis data, ICollection<int> balls) => GetResult(balls.Contains(data.pkm.Ball));
        private CheckResult GetResult(bool valid) => valid ? GetValid(LBallEnc) : GetInvalid(LBallEncMismatch);
    }
}
