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
            var EncounterMatch = data.EncounterMatch;
            var Info = data.Info;

            // Fixed ball cases -- can be only one ball ever
            switch (EncounterMatch)
            {
                case MysteryGift g:
                    return VerifyBallMysteryGift(data, g);
                case EncounterTrade t:
                    return VerifyBallEquals(data, t.Ball);
                case EncounterStatic s when s.Gift:
                    return VerifyBallEquals(data, s.Ball);
            }

            // Capture / Inherit cases -- can be one of many balls
            var pkm = data.pkm;
            if (pkm.Species == (int)Species.Shedinja && Info.Generation != 3 && data.EncounterMatch.Species != (int)Species.Shedinja) // Shedinja. For gen3, copy the ball from Nincada
                return VerifyBallEquals(data, (int)Poke); // Pokeball Only

            if (pkm.Ball == (int)Heavy && Legal.AlolanCaptureNoHeavyBall.Contains(EncounterMatch.Species) && !EncounterMatch.EggEncounter && pkm.SM)
                return GetInvalid(LBallHeavy); // Heavy Ball, can inherit if from egg (USUM fixed catch rate calc)

            return EncounterMatch switch
            {
                EncounterStatic e => VerifyBallStatic(data, e),
                EncounterSlot w => VerifyBallWild(data, w),
                EncounterEgg _ => VerifyBallEgg(data),
                EncounterInvalid _ => VerifyBallEquals(data, pkm.Ball), // ignore ball, pass whatever
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
                return VerifyBallEquals(data, Legal.DreamWorldBalls);
            return VerifyBallEquals(data, Legal.GetWildBalls(data.pkm));
        }

        private CheckResult VerifyBallWild(LegalityAnalysis data, EncounterSlot w)
        {
            if (w.Location == 30016 && w.Generation == 7) // Poké Pelago
                return VerifyBallEquals(data, (int)Poke); // Pokeball

            var Info = data.Info;

            // For gen3/4 Safari Zones and BCC getValidWildEncounters already filter to not return
            // mixed possible encounters between safari, BCC and other encounters
            // That means is the first encounter is not safari then there is no safari encounter in the array
            if (3 <= Info.Generation && Info.Generation <= 4 && w.Type.IsSafariType())
                return VerifyBallEquals(data, (int)Safari); // Safari Ball
            if (Info.Generation == 4 && w.Type == SlotType.BugContest)
                return VerifyBallEquals(data, (int)Sport); // Sport Ball
            return VerifyBallEquals(data, Legal.GetWildBalls(data.pkm));
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

        private CheckResult VerifyBallInherited(LegalityAnalysis data)
        {
            return data.Info.Generation switch
            {
                6 => VerifyBallEggGen6(data), // Gen6 Inheritance Rules
                7 => VerifyBallEggGen7(data), // Gen7 Inheritance Rules
                8 => VerifyBallEggGen8(data),
                _ => NONE
            };
        }

        private CheckResult VerifyBallEggGen6(LegalityAnalysis data)
        {
            var pkm = data.pkm;
            int species = data.EncounterMatch.Species;
            if (pkm.Gender == 2 || Legal.BreedMaleOnly.Contains(species)) // Genderless
                return VerifyBallEquals(data, (int)Poke); // Must be Pokéball as ball can only pass via mother (not Ditto!)

            Ball ball = (Ball)pkm.Ball;

            if (ball == Safari) // Safari Ball
            {
                if (!Legal.Inherit_Safari.Contains(species))
                    return GetInvalid(LBallSpecies);
                if (pkm.AbilityNumber == 4)
                    return GetInvalid(LBallAbility);
                return GetValid(LBallSpeciesPass);
            }
            if (ball.IsApricornBall()) // Apricorn Ball
            {
                if (!Legal.Inherit_Apricorn6.Contains(species))
                    return GetInvalid(LBallSpecies);
                if (pkm.AbilityNumber == 4)
                    return GetInvalid(LBallAbility);
                return GetValid(LBallSpeciesPass);
            }
            if (ball == Sport) // Sport Ball
            {
                if (!Legal.Inherit_Sport.Contains(species))
                    return GetInvalid(LBallSpecies);
                if (pkm.AbilityNumber == 4)
                    return GetInvalid(LBallAbility);
                return GetValid(LBallSpeciesPass);
            }
            if (ball == Dream) // Dream Ball
            {
                if (pkm.AbilityNumber == 4 && Legal.Ban_DreamHidden.Contains(species))
                    return GetInvalid(LBallAbility);
                if (Legal.Inherit_Dream.Contains(species))
                    return GetValid(LBallSpeciesPass);
                return GetInvalid(LBallSpecies);
            }
            if (Dusk <= ball && ball <= Quick) // Dusk Heal Quick
            {
                if (!Legal.Ban_Gen4Ball_6.Contains(species))
                    return GetValid(LBallSpeciesPass);
                return GetInvalid(LBallSpecies);
            }
            if (Ultra <= ball && ball <= Premier) // Don't worry, Safari was already checked.
            {
                if (Legal.Ban_Gen3Ball.Contains(species))
                    return GetInvalid(LBallSpecies);
                if (pkm.AbilityNumber == 4 && Legal.Ban_Gen3BallHidden.Contains(pkm.SpecForm))
                    return GetInvalid(LBallAbility);
                return GetValid(LBallSpeciesPass);
            }

            if (species > 650 && species != 700) // Sylveon
            {
                if (Legal.WildPokeballs6.Contains(pkm.Ball))
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
                if (!(Legal.Inherit_Safari.Contains(species) || Legal.Inherit_SafariMale.Contains(species)))
                    return GetInvalid(LBallSpecies);
                if (pkm.AbilityNumber == 4 && Legal.Ban_SafariBallHidden_7.Contains(species))
                    return GetInvalid(LBallAbility);
                return GetValid(LBallSpeciesPass);
            }
            if (ball.IsApricornBall()) // Apricorn Ball
            {
                if (!Legal.Inherit_Apricorn7.Contains(species))
                    return GetInvalid(LBallSpecies);
                if (pkm.AbilityNumber == 4 && Legal.Ban_NoHidden7Apricorn.Contains(species | pkm.AltForm << 11)) // lineage is 3->2->origin
                    return GetInvalid(LBallAbility);
                return GetValid(LBallSpeciesPass);
            }
            if (ball == Sport) // Sport Ball
            {
                if (!Legal.Inherit_Sport.Contains(species))
                    return GetInvalid(LBallSpecies);
                if (pkm.AbilityNumber == 4 && (species == (int)Species.Volbeat || species == (int)Species.Illumise)) // Volbeat/Illumise
                    return GetInvalid(LBallAbility);
                return GetValid(LBallSpeciesPass);
            }
            if (ball == Dream) // Dream Ball
            {
                if (Legal.Inherit_Dream.Contains(species) || Legal.Inherit_DreamMale.Contains(species))
                    return GetValid(LBallSpeciesPass);
                return GetInvalid(LBallSpecies);
            }
            if (Dusk <= ball && ball <= Quick) // Dusk Heal Quick
            {
                if (!Legal.Ban_Gen4Ball_7.Contains(species))
                    return GetValid(LBallSpeciesPass);
                return GetInvalid(LBallSpecies);
            }
            if (Ultra <= ball && ball <= Premier) // Don't worry, Safari was already checked.
            {
                if (!Legal.Ban_Gen3Ball_7.Contains(species))
                    return GetValid(LBallSpeciesPass);
                return GetInvalid(LBallSpecies);
            }

            if (ball == Beast)
            {
                if (species == (int)Species.Flabébé && pkm.AltForm == 3 && pkm.AbilityNumber == 4)
                    return GetInvalid(LBallAbility); // Can't obtain Flabébé-Blue with Hidden Ability in wild
                if (species == (int)Species.Voltorb && pkm.AbilityNumber == 4)
                    return GetInvalid(LBallAbility); // Can't obtain with Hidden Ability in wild (can only breed with Ditto)
                if (((int)Species.Pikipek <= species && species <= (int)Species.Kommoo) || (Legal.AlolanCaptureOffspring.Contains(species) && !Legal.PastGenAlolanNativesUncapturable.Contains(species)))
                    return GetValid(LBallSpeciesPass);
                if (Legal.PastGenAlolanScans.Contains(species))
                    return GetValid(LBallSpeciesPass);
                // next statement catches all new alolans
            }

            if (species > (int)Species.Volcanion)
                return VerifyBallEquals(data, Legal.WildPokeballs7);

            if (ball > Beast)
                return GetInvalid(LBallUnavailable);

            return NONE;
        }

        private CheckResult VerifyBallEggGen8(LegalityAnalysis data)
        {
            var pkm = data.pkm;
            int species = data.EncounterMatch.Species;
            if ((int)Species.Grookey <= species && species <= (int)Species.Inteleon) // G8 Starters
                return VerifyBallEquals(data, (int)Poke);

            if (IsGalarBreed(species))
            {
                if (Legal.WildPokeballs8.Contains(pkm.Ball))
                    return GetValid(LBallSpeciesPass);
                if (species >= (int)Species.Grookey)
                    return GetInvalid(LBallSpecies);
            }

            if (722 <= species && species <= 730) // G7 Starters
                return VerifyBallEquals(data, (int)Poke);

            Ball ball = (Ball)pkm.Ball;

            if (ball == Safari)
            {
                if (!(Legal.Inherit_Safari.Contains(species) || Legal.Inherit_SafariMale.Contains(species)))
                    return GetInvalid(LBallSpecies);
                if (pkm.AbilityNumber == 4 && Legal.Ban_SafariBallHidden_7.Contains(species))
                    return GetInvalid(LBallAbility);
                return GetValid(LBallSpeciesPass);
            }
            if (ball.IsApricornBall()) // Apricorn Ball
            {
                if (!Legal.Inherit_Apricorn7.Contains(species))
                    return GetInvalid(LBallSpecies);
                if (pkm.AbilityNumber == 4 && Legal.Ban_NoHidden8Apricorn.Contains(species | pkm.AltForm << 11)) // lineage is 3->2->origin
                    return GetInvalid(LBallAbility);
                return GetValid(LBallSpeciesPass);
            }
            if (ball == Sport) // Sport Ball
            {
                if (!Legal.Inherit_Sport.Contains(species))
                    return GetInvalid(LBallSpecies);
                if (pkm.AbilityNumber == 4 && (species == (int)Species.Volbeat || species == (int)Species.Illumise)) // Volbeat/Illumise
                    return GetInvalid(LBallAbility);
                return GetValid(LBallSpeciesPass);
            }
            if (ball == Dream) // Dream Ball
            {
                if (Legal.Inherit_Dream.Contains(species) || Legal.Inherit_DreamMale.Contains(species))
                    return GetValid(LBallSpeciesPass);
                return GetInvalid(LBallSpecies);
            }
            if (Dusk <= ball && ball <= Quick) // Dusk Heal Quick
            {
                if (!Legal.Ban_Gen4Ball_7.Contains(species))
                    return GetValid(LBallSpeciesPass);
                return GetInvalid(LBallSpecies);
            }
            if (Ultra <= ball && ball <= Premier) // Don't worry, Safari was already checked.
            {
                if (!Legal.Ban_Gen3Ball_7.Contains(species))
                    return GetValid(LBallSpeciesPass);
                return GetInvalid(LBallSpecies);
            }

            if (ball == Beast)
            {
                if (species == (int)Species.Flabébé && pkm.AltForm == 3 && pkm.AbilityNumber == 4)
                    return GetInvalid(LBallAbility); // Can't obtain Flabébé-Blue with Hidden Ability in wild
                if (((int)Species.Pikipek <= species && species <= (int)Species.Kommoo) || (Legal.AlolanCaptureOffspring.Contains(species) && !Legal.PastGenAlolanNativesUncapturable.Contains(species)))
                    return GetValid(LBallSpeciesPass);
                if (Legal.PastGenAlolanScans.Contains(species))
                    return GetValid(LBallSpeciesPass);
                // next statement catches all new alolans
            }

            if (species > Legal.MaxSpeciesID_7_USUM)
                return VerifyBallEquals(data, Legal.WildPokeballs8);

            if (species > (int)Species.Volcanion)
                return VerifyBallEquals(data, Legal.WildPokeballs7);

            if (ball > Beast)
                return GetInvalid(LBallUnavailable);

            return NONE;
        }

        public static bool IsGalarBreed(int species)
        {
            if ((int)Species.Grookey <= species && species <= (int)Species.Inteleon) // starter
                return false;
            var pt = PersonalTable.SWSH;
            return ((PersonalInfoSWSH)pt.GetFormeEntry(species, 0)).PokeDexIndex != 0;
        }

        private CheckResult VerifyBallEquals(LegalityAnalysis data, int ball) => GetResult(ball == data.pkm.Ball);
        private CheckResult VerifyBallEquals(LegalityAnalysis data, HashSet<int> balls) => GetResult(balls.Contains(data.pkm.Ball));
        private CheckResult VerifyBallEquals(LegalityAnalysis data, ICollection<int> balls) => GetResult(balls.Contains(data.pkm.Ball));
        private CheckResult GetResult(bool valid) => valid ? GetValid(LBallEnc) : GetInvalid(LBallEncMismatch);
    }
}
