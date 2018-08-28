using System.Linq;
using static PKHeX.Core.LegalityCheckStrings;

namespace PKHeX.Core
{
    /// <summary>
    /// Verifies miscellaneous data including <see cref="PKM.FatefulEncounter"/> and minor values.
    /// </summary>
    public sealed class MiscVerifier : Verifier
    {
        protected override CheckIdentifier Identifier => CheckIdentifier.Misc;

        public override void Verify(LegalityAnalysis data)
        {
            var pkm = data.pkm;
            if (pkm.IsEgg)
            {
                VerifyMiscEggCommon(data);

                if (pkm is IContestStats s && s.HasContestStats())
                    data.AddLine(GetInvalid(V320, CheckIdentifier.Egg));

                switch (pkm)
                {
                    case PK4 pk4 when pk4.ShinyLeaf != 0:
                        data.AddLine(GetInvalid(V414, CheckIdentifier.Egg));
                        break;
                    case PK4 pk4 when pk4.PokéathlonStat != 0:
                        data.AddLine(GetInvalid(V415, CheckIdentifier.Egg));
                        break;
                    case PK3 _ when pkm.Language != 1:  // All Eggs are Japanese and flagged specially for localized string
                        data.AddLine(GetInvalid(string.Format(V5, LanguageID.Japanese, (LanguageID)pkm.Language), CheckIdentifier.Egg));
                        break;
                }
            }

            if (pkm is PK7 pk7 && pk7.ResortEventStatus >= 20)
                data.AddLine(GetInvalid(V614));

            VerifyMiscFatefulEncounter(data);
        }

        public void VerifyMiscG1(LegalityAnalysis data)
        {
            var pkm = data.pkm;
            if (pkm.IsEgg)
            {
                VerifyMiscEggCommon(data);
                if (pkm.PKRS_Cured || pkm.PKRS_Infected)
                    data.AddLine(GetInvalid(V368, CheckIdentifier.Egg));
            }

            if (!(pkm is PK1 pk1))
                return;

            VerifyMiscG1Types(data, pk1);
            VerifyMiscG1CatchRate(data, pk1);
        }

        private void VerifyMiscG1Types(LegalityAnalysis data, PK1 pk1)
        {
            var Type_A = pk1.Type_A;
            var Type_B = pk1.Type_B;
            if (pk1.Species == 137) // Porygon
            {
                // Can have any type combination of any species by using Conversion.
                if (!Legal.Types_Gen1.Contains(Type_A))
                {
                    data.AddLine(GetInvalid(V386));
                }
                else if (!Legal.Types_Gen1.Contains(Type_B))
                {
                    data.AddLine(GetInvalid(V387));
                }
                else // Both match a type, ensure a gen1 species has this combo
                {
                    var TypesAB_Match = PersonalTable.RB.IsValidTypeCombination(Type_A, Type_B);
                    var result = TypesAB_Match ? GetValid(V391) : GetInvalid(V388);
                    data.AddLine(result);
                }
            }
            else // Types must match species types
            {
                var Type_A_Match = Type_A == PersonalTable.RB[pk1.Species].Type1;
                var Type_B_Match = Type_B == PersonalTable.RB[pk1.Species].Type2;

                var first = Type_A_Match ? GetValid(V392) : GetInvalid(V389);
                var second = Type_B_Match ? GetValid(V393) : GetInvalid(V390);
                data.AddLine(first);
                data.AddLine(second);
            }
        }

        private void VerifyMiscG1CatchRate(LegalityAnalysis data, PK1 pk1)
        {
            var e = data.EncounterMatch;
            var catch_rate = pk1.Catch_Rate;
            var result = pk1.TradebackStatus == TradebackType.Gen1_NotTradeback
                ? GetWasNotTradeback()
                : GetWasTradeback();
            data.AddLine(result);

            CheckResult GetWasTradeback()
            {
                if (catch_rate == 0 || Legal.HeldItems_GSC.Contains((ushort)catch_rate))
                    return GetValid(V394);
                if (pk1.TradebackStatus == TradebackType.WasTradeback)
                    return GetInvalid(V395);

                return GetWasNotTradeback();
            }

            CheckResult GetWasNotTradeback()
            {
                if ((e as EncounterStatic)?.Version == GameVersion.Stadium || e is EncounterTradeCatchRate)
                    return GetValid(V398); // Encounters detected by the catch rate, cant be invalid if match this encounters
                if ((pk1.Species == 149 && catch_rate == PersonalTable.Y[149].CatchRate) || (Legal.Species_NotAvailable_CatchRate.Contains(pk1.Species) && catch_rate == PersonalTable.RB[pk1.Species].CatchRate))
                    return GetInvalid(V396);
                if (!data.Info.EvoChainsAllGens[1].Any(c => RateMatchesEncounter(c.Species)))
                    return GetInvalid(pk1.Gen1_NotTradeback ? V397 : V399);
                return GetValid(V398);
            }

            bool RateMatchesEncounter(int species)
            {
                if (catch_rate == PersonalTable.RB[species].CatchRate)
                    return true;
                if (catch_rate == PersonalTable.Y[species].CatchRate)
                    return true;
                return false;
            }
        }

        private static void VerifyMiscFatefulEncounter(LegalityAnalysis data)
        {
            var pkm = data.pkm;
            var EncounterMatch = data.EncounterMatch;
            switch (EncounterMatch)
            {
                case WC3 w when w.Fateful:
                    if (w.IsEgg)
                    {
                        // Eggs hatched in RS clear the obedience flag!
                        if (pkm.Format != 3)
                            return; // possible hatched in either game, don't bother checking
                        if (pkm.Met_Location <= 087) // hatched in RS
                            break; // ensure fateful is not active
                        // else, ensure fateful is active (via below)
                    }
                    VerifyFatefulIngameActive(data);
                    VerifyWC3Shiny(data, w);
                    return;
                case WC3 w:
                    if (w.Version == GameVersion.XD)
                        return; // Can have either state
                    VerifyWC3Shiny(data, w);
                    break;
                case MysteryGift g when g.Format != 3: // WC3
                    VerifyReceivability(data, g);
                    VerifyFatefulMysteryGift(data, g);
                    return;
                case EncounterStatic s when s.Fateful: // ingame fateful
                case EncounterSlot _ when pkm.Version == 15: // ingame pokespot
                case EncounterTrade t when t.Fateful:
                    VerifyFatefulIngameActive(data);
                    return;
            }
            if (pkm.FatefulEncounter)
                data.AddLine(GetInvalid(V325, CheckIdentifier.Fateful));
        }

        private static void VerifyMiscEggCommon(LegalityAnalysis data)
        {
            var pkm = data.pkm;
            if (pkm.Move1_PPUps > 0 || pkm.Move2_PPUps > 0 || pkm.Move3_PPUps > 0 || pkm.Move4_PPUps > 0)
                data.AddLine(GetInvalid(V319, CheckIdentifier.Egg));
            if (pkm.Move1_PP != pkm.GetMovePP(pkm.Move1, 0) || pkm.Move2_PP != pkm.GetMovePP(pkm.Move2, 0) || pkm.Move3_PP != pkm.GetMovePP(pkm.Move3, 0) || pkm.Move4_PP != pkm.GetMovePP(pkm.Move4, 0))
                data.AddLine(GetInvalid(V420, CheckIdentifier.Egg));

            var EncounterMatch = data.EncounterOriginal;
            var HatchCycles = (EncounterMatch as EncounterStatic)?.EggCycles;
            if (HatchCycles == 0 || HatchCycles == null)
                HatchCycles = pkm.PersonalInfo.HatchCycles;
            if (pkm.CurrentFriendship > HatchCycles)
                data.AddLine(GetInvalid(V374, CheckIdentifier.Egg));

            if (pkm.Format >= 6 && EncounterMatch is EncounterEgg && !pkm.Moves.SequenceEqual(pkm.RelearnMoves))
            {
                var moves = string.Join(", ", LegalityAnalysis.GetMoveNames(pkm.Moves));
                var msg = string.Format(V343, moves);
                data.AddLine(GetInvalid(msg, CheckIdentifier.Egg));
            }
        }

        private static void VerifyFatefulMysteryGift(LegalityAnalysis data, MysteryGift g)
        {
            var pkm = data.pkm;
            if (g is PGF p && p.IsShiny)
            {
                var Info = data.Info;
                Info.PIDIV = MethodFinder.Analyze(pkm);
                if (Info.PIDIV.Type != PIDType.G5MGShiny && pkm.Egg_Location != 30003)
                    data.AddLine(GetInvalid(V411, CheckIdentifier.PID));
            }

            var result = pkm.FatefulEncounter
                ? GetValid(V321, CheckIdentifier.Fateful)
                : GetInvalid(V322, CheckIdentifier.Fateful);
            data.AddLine(result);
        }

        private static void VerifyReceivability(LegalityAnalysis data, MysteryGift g)
        {
            var pkm = data.pkm;
            switch (g)
            {
                case WC6 wc6 when !wc6.CanBeReceivedByVersion(pkm.Version):
                case WC7 wc7 when !wc7.CanBeReceivedByVersion(pkm.Version):
                    data.AddLine(GetInvalid(V416, CheckIdentifier.GameOrigin));
                    return;
                case WC6 wc6 when wc6.RestrictLanguage != 0 && wc6.Language != wc6.RestrictLanguage:
                    data.AddLine(GetInvalid(string.Format(V5, wc6.RestrictLanguage, pkm.Language), CheckIdentifier.Language));
                    return;
                case WC7 wc7 when wc7.RestrictLanguage != 0 && wc7.Language != wc7.RestrictLanguage:
                    data.AddLine(GetInvalid(string.Format(V5, wc7.RestrictLanguage, pkm.Language), CheckIdentifier.Language));
                    return;
            }
        }

        private static void VerifyWC3Shiny(LegalityAnalysis data, WC3 g3)
        {
            // check for shiny locked gifts
            if (!g3.Shiny.IsValid(data.pkm))
                data.AddLine(GetInvalid(V409, CheckIdentifier.Fateful));
        }

        private static void VerifyFatefulIngameActive(LegalityAnalysis data)
        {
            var pkm = data.pkm;
            if (pkm.Version == 15 && pkm is XK3 xk3 && data.Info.WasXD)
            {
                // can't have fateful until traded away, which clears ShadowID
                if (xk3.FatefulEncounter && xk3.ShadowID != 0 && data.EncounterMatch is EncounterStaticShadow)
                    data.AddLine(GetInvalid(V325, CheckIdentifier.Fateful));

                return; // fateful is set when transferred away
            }

            var result = pkm.FatefulEncounter
                ? GetValid(V323, CheckIdentifier.Fateful)
                : GetInvalid(V324, CheckIdentifier.Fateful);
            data.AddLine(result);
        }

        public void VerifyVersionEvolution(LegalityAnalysis data)
        {
            var pkm = data.pkm;
            if (pkm.Format < 7 || data.EncounterMatch.Species == pkm.Species)
                return;

            // No point using the evolution tree. Just handle certain species.
            switch (pkm.Species)
            {
                case 745 when (pkm.AltForm == 0 && Moon()) || (pkm.AltForm == 1 && Sun()): // Lycanroc
                case 791 when Moon(): // Solgaleo
                case 792 when Sun(): // Lunala
                    bool Sun() => pkm.Version == (int)GameVersion.SN || pkm.Version == (int)GameVersion.US;
                    bool Moon() => pkm.Version == (int)GameVersion.MN || pkm.Version == (int)GameVersion.UM;
                    if (pkm.IsUntraded)
                        data.AddLine(GetInvalid(V328, CheckIdentifier.Evolution));
                    break;
            }
        }
    }
}
