using System.Collections.Generic;
using System.Linq;
using static PKHeX.Core.LegalityCheckStrings;

namespace PKHeX.Core
{
    public class BallVerifier : Verifier
    {
        protected override CheckIdentifier Identifier => CheckIdentifier.Ball;
        public override void Verify(LegalityAnalysis data)
        {
            var pkm = data.pkm;
            if (pkm.Format < 3)
                return; // no ball info saved

            var EncounterMatch = data.EncounterMatch;
            var Info = data.Info;

            if (EncounterMatch is MysteryGift g)
            {
                if (pkm.Species == 490 && g.Ball == 0)
                    // there is no ball data in Manaphy Mystery Gift
                    VerifyBallEquals(data, 4); // Pokeball
                else
                    VerifyBallEquals(data, g.Ball);
                return;
            }
            if (EncounterMatch is EncounterLink l)
            {
                VerifyBallEquals(data, l.Ball);
                return;
            }
            if (EncounterMatch is EncounterTrade t)
            {
                VerifyBallEquals(data, t.Ball); // Pokeball
                return;
            }

            if (pkm.Species == 292 && Info.Generation > 3) // Shedinja. For gen3, copy the ball from Nincada
            {
                VerifyBallEquals(data, 4); // Pokeball Only
                return;
            }

            if (pkm.Ball == 0x14 && !Info.EncounterMatch.EggEncounter && pkm.SM) // Heavy Ball
            {
                var lineage = EvolutionChain.GetLineage(pkm);
                if (lineage.Any(e => Legal.AlolanCaptureNoHeavyBall.Contains(e)))
                {
                    data.AddLine(GetInvalid(V116));
                    return;
                }
            }

            if (EncounterMatch is EncounterStatic s)
            {
                if (s.Gift)
                    VerifyBallEquals(data, s.Ball);
                else if (s.Location == 75 && s.Generation == 5) // Entree Forest (Dream World)
                    VerifyBallEquals(data, Legal.DreamWorldBalls);
                else
                    VerifyBallEquals(data, Legal.GetWildBalls(pkm));
                return;
            }
            if (EncounterMatch is EncounterSlot w)
            {
                if (w.Location == 30016 && w.Generation == 7) // Poké Pelago
                    VerifyBallEquals(data, 4); // Pokeball
                // For gen3/4 Safari Zones and BCC getValidWildEncounters already filter to not return
                // mixed possible encounters between safari, BCC and other encounters
                // That means is the first encounter is not safari then there is no safari encounter in the array
                else if (3 <= Info.Generation && Info.Generation <= 4 && w.Type.IsSafariType())
                    VerifyBallEquals(data, 5); // Safari Ball
                else if (Info.Generation == 4 && w.Type == SlotType.BugContest)
                    VerifyBallEquals(data, 0x18); // Sport Ball
                else
                    VerifyBallEquals(data, Legal.GetWildBalls(pkm));
                return;
            }

            if (pkm.WasEgg)
            {
                VerifyBallEgg(data);
                return;
            }

            VerifyBallEquals(data, 4); // Pokeball
        }
        private void VerifyBallEgg(LegalityAnalysis data)
        {
            var pkm = data.pkm;
            if (data.Info.Generation < 6) // No inheriting Balls
            {
                VerifyBallEquals(data, 4); // Must be Pokéball -- no ball inheritance.
                return;
            }

            if (pkm.Ball == 0x01) // Master Ball
            { data.AddLine(GetInvalid(V117)); return; }
            if (pkm.Ball == 0x10) // Cherish Ball
            { data.AddLine(GetInvalid(V120)); return; }
            if (pkm.Ball == 0x04) // Poké Ball
            { data.AddLine(GetValid(V119)); return; }

            switch (data.Info.Generation)
            {
                case 6: // Gen6 Inheritance Rules
                    VerifyBallEggGen6(data);
                    return;
                case 7: // Gen7 Inheritance Rules
                    VerifyBallEggGen7(data);
                    return;
            }
        }
        private void VerifyBallEggGen6(LegalityAnalysis data)
        {
            var pkm = data.pkm;
            if (pkm.Gender == 2) // Genderless
            {
                VerifyBallEquals(data, 4); // Must be Pokéball as ball can only pass via mother (not Ditto!)
                return;
            }
            if (Legal.BreedMaleOnly.Contains(pkm.Species))
            {
                VerifyBallEquals(data, 4); // Must be Pokéball as ball can only pass via mother (not Ditto!)
                return;
            }

            int ball = pkm.Ball;

            if (ball >= 26)
            {
                data.AddLine(GetInvalid(V126));
                return;
            }
            if (ball == 0x05) // Safari Ball
            {
                if (EvolutionChain.GetLineage(pkm).All(e => !Legal.Inherit_Safari.Contains(e)))
                    data.AddLine(GetInvalid(V121));
                else if (pkm.AbilityNumber == 4)
                    data.AddLine(GetInvalid(V122));
                else
                    data.AddLine(GetValid(V123));

                return;
            }
            if (0x10 < ball && ball < 0x18) // Apricorn Ball
            {
                if (EvolutionChain.GetLineage(pkm).All(e => !Legal.Inherit_Apricorn6.Contains(e)))
                    data.AddLine(GetInvalid(V121));
                if (pkm.AbilityNumber == 4)
                    data.AddLine(GetInvalid(V122));
                else
                    data.AddLine(GetValid(V123));

                return;
            }
            if (ball == 0x18) // Sport Ball
            {
                if (EvolutionChain.GetLineage(pkm).All(e => !Legal.Inherit_Sport.Contains(e)))
                    data.AddLine(GetInvalid(V121));
                else if (pkm.AbilityNumber == 4)
                    data.AddLine(GetInvalid(V122));
                else
                    data.AddLine(GetValid(V123));

                return;
            }
            if (ball == 0x19) // Dream Ball
            {
                if (EvolutionChain.GetLineage(pkm).Any(e => Legal.Inherit_Dream.Contains(e)))
                    data.AddLine(GetValid(V123));
                else
                    data.AddLine(GetInvalid(V121));

                if (pkm.AbilityNumber == 4 && Legal.Ban_DreamHidden.Contains(pkm.Species))
                    data.AddLine(GetInvalid(V122));

                return;
            }
            if (0x0D <= ball && ball <= 0x0F)
            {
                if (!Legal.Ban_Gen4Ball_6.Contains(pkm.Species))
                    data.AddLine(GetValid(V123));
                else
                    data.AddLine(GetInvalid(V121));

                return;
            }
            if (0x02 <= ball && ball <= 0x0C) // Don't worry, Ball # 0x05 was already checked.
            {
                if (Legal.Ban_Gen3Ball.Contains(pkm.Species))
                    data.AddLine(GetInvalid(V121));
                else if (pkm.AbilityNumber == 4 && Legal.Ban_Gen3BallHidden.Contains(pkm.SpecForm))
                    data.AddLine(GetInvalid(V122));
                else
                    data.AddLine(GetValid(V123));

                return;
            }

            if (pkm.Species > 650 && pkm.Species != 700) // Sylveon
            {
                if (Legal.GetWildBalls(pkm).Contains(pkm.Ball))
                    data.AddLine(GetValid(V123));
                else
                    data.AddLine(GetInvalid(V121));
                return;
            }

            data.AddLine(GetInvalid(V125));
        }
        private void VerifyBallEggGen7(LegalityAnalysis data)
        {
            var pkm = data.pkm;
            var Lineage = EvolutionChain.GetLineage(pkm).ToArray();
            if (722 <= pkm.Species && pkm.Species <= 730) // G7 Starters
            {
                VerifyBallEquals(data, 4);
                return;
            }

            int ball = pkm.Ball;
            if (ball == 0x05) // Safari Ball
            {
                if (!Lineage.Any(e => Legal.Inherit_Safari.Contains(e) || Legal.Inherit_SafariMale.Contains(e)))
                    data.AddLine(GetInvalid(V121));
                else if (pkm.AbilityNumber == 4 && Lineage.Any(e => Legal.Ban_SafariBallHidden_7.Contains(e)))
                    data.AddLine(GetInvalid(V122));
                else
                    data.AddLine(GetValid(V123));

                return;
            }
            if (0x10 < ball && ball < 0x18) // Apricorn Ball
            {
                if (!Lineage.Any(e => Legal.Inherit_Apricorn7.Contains(e)))
                    data.AddLine(GetInvalid(V121));
                else if (pkm.AbilityNumber == 4 && Legal.Ban_NoHidden7Apricorn.Contains(Lineage.Last() | pkm.AltForm << 11)) // lineage is 3->2->origin
                    data.AddLine(GetInvalid(V122));
                else
                    data.AddLine(GetValid(V123));

                return;
            }
            if (ball == 0x18) // Sport Ball
            {
                if (!Lineage.Any(e => Legal.Inherit_Sport.Contains(e)))
                    data.AddLine(GetInvalid(V121));
                else if (pkm.AbilityNumber == 4 && (Lineage.Contains(313) || Lineage.Contains(314))) // Volbeat/Illumise
                    data.AddLine(GetInvalid(V122));
                else
                    data.AddLine(GetValid(V123));

                return;
            }
            if (ball == 0x19) // Dream Ball
            {
                if (Lineage.Any(e => Legal.Inherit_Dream.Contains(e) || Legal.Inherit_DreamMale.Contains(e)))
                    data.AddLine(GetValid(V123));
                else
                    data.AddLine(GetInvalid(V121));

                return;
            }
            if (0x0D <= ball && ball <= 0x0F) // Dusk Heal Quick
            {
                if (!Legal.Ban_Gen4Ball_7.Contains(pkm.Species))
                    data.AddLine(GetValid(V123));
                else
                    data.AddLine(GetInvalid(V121));

                return;
            }
            if (0x02 <= ball && ball <= 0x0C) // Don't worry, Ball # 0x05 was already checked.
            {
                if (!Legal.Ban_Gen3Ball_7.Contains(pkm.Species))
                    data.AddLine(GetValid(V123));
                else
                    data.AddLine(GetInvalid(V121));

                return;
            }

            if (ball == 26)
            {
                if (Lineage[0] == 669 && pkm.AltForm == 3 && pkm.AbilityNumber == 4)
                {
                    // Can't obtain Flabébé-Blue with Hidden Ability in wild
                    data.AddLine(GetInvalid(V122));
                    return;
                }
                if ((pkm.Species > 731 && pkm.Species <= 785) || Lineage.Any(e => Legal.PastGenAlolanNatives.Contains(e) && !Legal.PastGenAlolanNativesUncapturable.Contains(e)))
                {
                    data.AddLine(GetValid(V123));
                    return;
                }
                if (Lineage.Any(e => Legal.PastGenAlolanScans.Contains(e)))
                {
                    data.AddLine(GetValid(V123));
                    return;
                }
                // next statement catches all new alolans
            }

            if (pkm.Species > 721)
            {
                VerifyBallEquals(data, Legal.GetWildBalls(pkm));
                return;
            }

            if (ball >= 27)
            {
                data.AddLine(GetInvalid(V126));
                return;
            }
            data.AddLine(GetInvalid(V125));
        }

        private void VerifyBallEquals(LegalityAnalysis data, int ball) => AddBallLine(data, ball == data.pkm.Ball);
        private void VerifyBallEquals(LegalityAnalysis data, ICollection<int> balls) => AddBallLine(data, balls.Contains(data.pkm.Ball));
        private void AddBallLine(LegalityAnalysis data, bool valid)
        {
            if (valid)
                data.AddLine(GetValid(V119));
            else
                data.AddLine(GetInvalid(V118));
        }
    }
}
