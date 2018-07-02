using System.Collections.Generic;
using static PKHeX.Core.LegalityCheckStrings;

namespace PKHeX.Core
{
    /// <summary>
    /// Verifies the <see cref="PKM.Ball"/> value.
    /// </summary>
    public sealed class BallVerifier : Verifier
    {
        protected override CheckIdentifier Identifier => CheckIdentifier.Ball;
        private CheckResult NONE => GetInvalid(V125);

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
            if (EncounterMatch is MysteryGift g)
                return VerifyBallMysteryGift(data, g);
            if (EncounterMatch is EncounterLink l)
                return VerifyBallEquals(data, l.Ball);
            if (EncounterMatch is EncounterTrade t)
                return VerifyBallEquals(data, t.Ball);
            if (EncounterMatch is EncounterStatic s && s.Gift)
                return VerifyBallEquals(data, s.Ball);

            // Capture / Inherit cases -- can be one of many balls
            var pkm = data.pkm;
            if (pkm.Species == 292 && Info.Generation > 3) // Shedinja. For gen3, copy the ball from Nincada
                return VerifyBallEquals(data, 4); // Pokeball Only

            if (pkm.Ball == 0x14 && Legal.AlolanCaptureNoHeavyBall.Contains(EncounterMatch.Species)
                && !EncounterMatch.EggEncounter && pkm.SM) // Heavy Ball, can inherit if from egg (USUM fixed catch rate calc)
                return GetInvalid(V116);

            if (EncounterMatch is EncounterStatic e)
                return VerifyBallStatic(data, e);
            if (EncounterMatch is EncounterSlot w)
                return VerifyBallWild(data, w);
            if (EncounterMatch is EncounterEgg)
                return VerifyBallEgg(data);

            return VerifyBallEquals(data, 4); // Pokeball
        }

        private CheckResult VerifyBallMysteryGift(LegalityAnalysis data, MysteryGift g)
        {
            if (g.Gen4 && g.Species == 490 && g.Ball == 0) // there is no ball data in Manaphy Mystery Gift from Gen4
                return VerifyBallEquals(data, 4); // Pokeball
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
                return VerifyBallEquals(data, 4); // Pokeball

            var Info = data.Info;

            // For gen3/4 Safari Zones and BCC getValidWildEncounters already filter to not return
            // mixed possible encounters between safari, BCC and other encounters
            // That means is the first encounter is not safari then there is no safari encounter in the array
            if (3 <= Info.Generation && Info.Generation <= 4 && w.Type.IsSafariType())
                return VerifyBallEquals(data, 5); // Safari Ball
            if (Info.Generation == 4 && w.Type == SlotType.BugContest)
                return VerifyBallEquals(data, 0x18); // Sport Ball
            return VerifyBallEquals(data, Legal.GetWildBalls(data.pkm));
        }
        private CheckResult VerifyBallEgg(LegalityAnalysis data)
        {
            var pkm = data.pkm;
            if (data.Info.Generation < 6) // No inheriting Balls
                return VerifyBallEquals(data, 4); // Must be Pokéball -- no ball inheritance.

            switch (pkm.Ball)
            {
                case 0x04: return GetValid(V119); // Poké Ball
                case 0x01: return GetInvalid(V117); // Master Ball
                case 0x10: return GetInvalid(V120); // Cherish Ball
                default: return VerifyBallInherited(data);
            }
        }
        private CheckResult VerifyBallInherited(LegalityAnalysis data)
        {
            switch (data.Info.Generation)
            {
                case 6: return VerifyBallEggGen6(data); // Gen6 Inheritance Rules
                case 7: return VerifyBallEggGen7(data); // Gen7 Inheritance Rules
                default: return NONE;
            }
        }

        private CheckResult VerifyBallEggGen6(LegalityAnalysis data)
        {
            var pkm = data.pkm;
            int species = data.EncounterMatch.Species;
            if (pkm.Gender == 2 || Legal.BreedMaleOnly.Contains(species)) // Genderless
                return VerifyBallEquals(data, 4); // Must be Pokéball as ball can only pass via mother (not Ditto!)

            int ball = pkm.Ball;

            if (ball >= 26)
                return GetInvalid(V126);

            if (ball == 0x05) // Safari Ball
            {
                if (!Legal.Inherit_Safari.Contains(species))
                    return GetInvalid(V121);
                if (pkm.AbilityNumber == 4)
                    return GetInvalid(V122);
                return GetValid(V123);
            }
            if (0x10 < ball && ball < 0x18) // Apricorn Ball
            {
                if (!Legal.Inherit_Apricorn6.Contains(species))
                    return GetInvalid(V121);
                if (pkm.AbilityNumber == 4)
                    return GetInvalid(V122);
                return GetValid(V123);
            }
            if (ball == 0x18) // Sport Ball
            {
                if (!Legal.Inherit_Sport.Contains(species))
                    return GetInvalid(V121);
                if (pkm.AbilityNumber == 4)
                    return GetInvalid(V122);
                return GetValid(V123);
            }
            if (ball == 0x19) // Dream Ball
            {
                if (pkm.AbilityNumber == 4 && Legal.Ban_DreamHidden.Contains(species))
                    return GetInvalid(V122);
                if (Legal.Inherit_Dream.Contains(species))
                    return GetValid(V123);
                return GetInvalid(V121);
            }
            if (0x0D <= ball && ball <= 0x0F) // Dusk Heal Quick
            {
                if (!Legal.Ban_Gen4Ball_6.Contains(species))
                    return GetValid(V123);
                return GetInvalid(V121);
            }
            if (0x02 <= ball && ball <= 0x0C) // Don't worry, Ball # 0x05 was already checked.
            {
                if (Legal.Ban_Gen3Ball.Contains(species))
                    return GetInvalid(V121);
                if (pkm.AbilityNumber == 4 && Legal.Ban_Gen3BallHidden.Contains(pkm.SpecForm))
                    return GetInvalid(V122);
                return GetValid(V123);

            }

            if (species > 650 && species != 700) // Sylveon
            {
                if (Legal.WildPokeballs6.Contains(pkm.Ball))
                    return GetValid(V123);
                return GetInvalid(V121);
            }

            return NONE;
        }
        private CheckResult VerifyBallEggGen7(LegalityAnalysis data)
        {
            var pkm = data.pkm;
            int species = data.EncounterMatch.Species;
            if (722 <= species && species <= 730) // G7 Starters
                return VerifyBallEquals(data, 4);

            int ball = pkm.Ball;

            if (ball == 0x05) // Safari Ball
            {
                if (!(Legal.Inherit_Safari.Contains(species) || Legal.Inherit_SafariMale.Contains(species)))
                    return GetInvalid(V121);
                if (pkm.AbilityNumber == 4 && Legal.Ban_SafariBallHidden_7.Contains(species))
                    return GetInvalid(V122);
                return GetValid(V123);
            }
            if (0x10 < ball && ball < 0x18) // Apricorn Ball
            {
                if (!Legal.Inherit_Apricorn7.Contains(species))
                    return GetInvalid(V121);
                if (pkm.AbilityNumber == 4 && Legal.Ban_NoHidden7Apricorn.Contains(species | pkm.AltForm << 11)) // lineage is 3->2->origin
                    return GetInvalid(V122);
                return GetValid(V123);
            }
            if (ball == 0x18) // Sport Ball
            {
                if (!Legal.Inherit_Sport.Contains(species))
                    return GetInvalid(V121);
                if (pkm.AbilityNumber == 4 && (species == 313) || (species == 314)) // Volbeat/Illumise
                    return GetInvalid(V122);
                return GetValid(V123);
            }
            if (ball == 0x19) // Dream Ball
            {
                if (Legal.Inherit_Dream.Contains(species) || Legal.Inherit_DreamMale.Contains(species))
                    return GetValid(V123);
                return GetInvalid(V121);
            }
            if (0x0D <= ball && ball <= 0x0F) // Dusk Heal Quick
            {
                if (!Legal.Ban_Gen4Ball_7.Contains(species))
                    return GetValid(V123);
                return GetInvalid(V121);
            }
            if (0x02 <= ball && ball <= 0x0C) // Don't worry, Ball # 0x05 was already checked.
            {
                if (!Legal.Ban_Gen3Ball_7.Contains(species))
                    return GetValid(V123);
                return GetInvalid(V121);
            }

            if (ball == 26)
            {
                if (species == 669 && pkm.AltForm == 3 && pkm.AbilityNumber == 4)
                    return GetInvalid(V122); // Can't obtain Flabébé-Blue with Hidden Ability in wild
                if ((species > 731 && species <= 785) || Legal.PastGenAlolanNatives.Contains(species) && !Legal.PastGenAlolanNativesUncapturable.Contains(species))
                    return GetValid(V123);
                if (Legal.PastGenAlolanScans.Contains(species))
                    return GetValid(V123);
                // next statement catches all new alolans
            }

            if (species > 721)
                return VerifyBallEquals(data, Legal.WildPokeballs7);

            if (ball >= 27)
                return GetInvalid(V126);

            return NONE;
        }

        private CheckResult VerifyBallEquals(LegalityAnalysis data, int ball) => GetResult(ball == data.pkm.Ball);
        private CheckResult VerifyBallEquals(LegalityAnalysis data, HashSet<int> balls) => GetResult(balls.Contains(data.pkm.Ball));
        private CheckResult VerifyBallEquals(LegalityAnalysis data, ICollection<int> balls) => GetResult(balls.Contains(data.pkm.Ball));
        private CheckResult GetResult(bool valid)
        {
            return valid ? GetValid(V119) : GetInvalid(V118);
        }
    }
}
