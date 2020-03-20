using System;
using System.Linq;
using static PKHeX.Core.LegalityCheckStrings;
using static PKHeX.Core.CheckIdentifier;

namespace PKHeX.Core
{
    /// <summary>
    /// Verifies miscellaneous data including <see cref="PKM.FatefulEncounter"/> and minor values.
    /// </summary>
    public sealed class MiscVerifier : Verifier
    {
        protected override CheckIdentifier Identifier => Misc;

        public override void Verify(LegalityAnalysis data)
        {
            var pkm = data.pkm;
            if (pkm.IsEgg)
            {
                VerifyMiscEggCommon(data);

                if (pkm is IContestStats s && s.HasContestStats())
                    data.AddLine(GetInvalid(LEggContest, Egg));

                switch (pkm)
                {
                    case PK5 pk5 when pk5.PokeStarFame != 0 && pk5.IsEgg:
                        data.AddLine(GetInvalid(LEggShinyPokeStar, Egg));
                        break;
                    case PK4 pk4 when pk4.ShinyLeaf != 0:
                        data.AddLine(GetInvalid(LEggShinyLeaf, Egg));
                        break;
                    case PK4 pk4 when pk4.PokéathlonStat != 0:
                        data.AddLine(GetInvalid(LEggPokeathlon, Egg));
                        break;
                    case PK3 _ when pkm.Language != 1:  // All Eggs are Japanese and flagged specially for localized string
                        data.AddLine(GetInvalid(string.Format(LOTLanguage, LanguageID.Japanese, (LanguageID)pkm.Language), Egg));
                        break;
                }
            }
            else
            {
                VerifyMiscMovePP(data);
            }

            if (pkm is PK7 pk7 && pk7.ResortEventStatus >= 20)
                data.AddLine(GetInvalid(LTransferBad));
            if (pkm is PB7 pb7)
                VerifyBelugaStats(data, pb7);
            if (pkm is PK8 pk8)
                VerifySWSHStats(data, pk8);

            VerifyMiscFatefulEncounter(data);
        }

        public void VerifyMiscG1(LegalityAnalysis data)
        {
            var pkm = data.pkm;
            if (pkm.IsEgg)
            {
                VerifyMiscEggCommon(data);
                if (pkm.PKRS_Cured || pkm.PKRS_Infected)
                    data.AddLine(GetInvalid(LEggPokerus, Egg));
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
            if (pk1.Species == (int)Species.Porygon)
            {
                // Can have any type combination of any species by using Conversion.
                if (!GBRestrictions.TypeIDExists(Type_A))
                {
                    data.AddLine(GetInvalid(LG1TypePorygonFail1));
                }
                if (!GBRestrictions.TypeIDExists(Type_B))
                {
                    data.AddLine(GetInvalid(LG1TypePorygonFail2));
                }
                else // Both match a type, ensure a gen1 species has this combo
                {
                    var TypesAB_Match = PersonalTable.RB.IsValidTypeCombination(Type_A, Type_B);
                    var result = TypesAB_Match ? GetValid(LG1TypeMatchPorygon) : GetInvalid(LG1TypePorygonFail);
                    data.AddLine(result);
                }
            }
            else // Types must match species types
            {
                var Type_A_Match = Type_A == PersonalTable.RB[pk1.Species].Type1;
                var Type_B_Match = Type_B == PersonalTable.RB[pk1.Species].Type2;

                var first = Type_A_Match ? GetValid(LG1TypeMatch1) : GetInvalid(LG1Type1Fail);
                var second = Type_B_Match ? GetValid(LG1TypeMatch2) : GetInvalid(LG1Type2Fail);
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
                    return GetValid(LG1CatchRateMatchTradeback);
                if (pk1.TradebackStatus == TradebackType.WasTradeback)
                    return GetInvalid(LG1CatchRateItem);

                return GetWasNotTradeback();
            }

            CheckResult GetWasNotTradeback()
            {
                if ((e as EncounterStatic)?.Version == GameVersion.Stadium || e is EncounterTradeCatchRate)
                    return GetValid(LG1CatchRateMatchPrevious); // Encounters detected by the catch rate, cant be invalid if match this encounters
                if ((pk1.Species == 149 && catch_rate == PersonalTable.Y[149].CatchRate) || (GBRestrictions.Species_NotAvailable_CatchRate.Contains(pk1.Species) && catch_rate == PersonalTable.RB[pk1.Species].CatchRate))
                    return GetInvalid(LG1CatchRateEvo);
                if (!data.Info.EvoChainsAllGens[1].Any(c => RateMatchesEncounter(c.Species)))
                    return GetInvalid(pk1.Gen1_NotTradeback ? LG1CatchRateChain : LG1CatchRateNone);
                return GetValid(LG1CatchRateMatchPrevious);
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
                        // Hatching in Gen3 doesn't change the origin version.
                        if (pkm.Format != 3)
                            return; // possible hatched in either game, don't bother checking
                        if (pkm.Met_Location <= 087) // hatched in RS or Emerald
                            return; // possible hatched in either game, don't bother checking
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
                case EncounterSlot x when x.Version == GameVersion.XD: // ingame pokespot
                case EncounterTrade t when t.Fateful:
                    VerifyFatefulIngameActive(data);
                    return;
            }
            if (pkm.FatefulEncounter)
                data.AddLine(GetInvalid(LFatefulInvalid, Fateful));
        }

        private static void VerifyMiscMovePP(LegalityAnalysis data)
        {
            var pkm = data.pkm;
            if (pkm.Move1_PP > pkm.GetMovePP(pkm.Move1, pkm.Move1_PPUps))
                data.AddLine(GetInvalid(string.Format(LMovePPTooHigh_0, 1), Move));
            if (pkm.Move2_PP > pkm.GetMovePP(pkm.Move2, pkm.Move2_PPUps))
                data.AddLine(GetInvalid(string.Format(LMovePPTooHigh_0, 2), Move));
            if (pkm.Move3_PP > pkm.GetMovePP(pkm.Move3, pkm.Move3_PPUps))
                data.AddLine(GetInvalid(string.Format(LMovePPTooHigh_0, 3), Move));
            if (pkm.Move4_PP > pkm.GetMovePP(pkm.Move4, pkm.Move4_PPUps))
                data.AddLine(GetInvalid(string.Format(LMovePPTooHigh_0, 4), Move));
        }

        private static void VerifyMiscEggCommon(LegalityAnalysis data)
        {
            var pkm = data.pkm;
            if (pkm.Move1_PPUps > 0 || pkm.Move2_PPUps > 0 || pkm.Move3_PPUps > 0 || pkm.Move4_PPUps > 0)
                data.AddLine(GetInvalid(LEggPPUp, Egg));
            if (pkm.Move1_PP != pkm.GetMovePP(pkm.Move1, 0) || pkm.Move2_PP != pkm.GetMovePP(pkm.Move2, 0) || pkm.Move3_PP != pkm.GetMovePP(pkm.Move3, 0) || pkm.Move4_PP != pkm.GetMovePP(pkm.Move4, 0))
                data.AddLine(GetInvalid(LEggPP, Egg));

            var EncounterMatch = data.EncounterOriginal;
            var HatchCycles = EncounterMatch is EncounterStatic s ? s.EggCycles : 0;
            if (HatchCycles == 0) // no value set
                HatchCycles = pkm.PersonalInfo.HatchCycles;
            if (pkm.CurrentFriendship > HatchCycles)
                data.AddLine(GetInvalid(LEggHatchCycles, Egg));

            if (pkm.Format >= 6 && EncounterMatch is EncounterEgg && !pkm.Moves.SequenceEqual(pkm.RelearnMoves))
            {
                var moves = string.Join(", ", LegalityAnalysis.GetMoveNames(pkm.Moves));
                var msg = string.Format(LMoveFExpect_0, moves);
                data.AddLine(GetInvalid(msg, Egg));
            }

            if (pkm is PK8 pk8)
            {
                if (pk8.HasAnyMoveRecordFlag())
                    data.AddLine(GetInvalid(LEggRelearnFlags, Egg));
                if (pk8.StatNature != pk8.Nature)
                    data.AddLine(GetInvalid(LEggNature, Egg));
            }
        }

        private static void VerifyFatefulMysteryGift(LegalityAnalysis data, MysteryGift g)
        {
            var pkm = data.pkm;
            if (g is PGF p && p.IsShiny)
            {
                var Info = data.Info;
                Info.PIDIV = MethodFinder.Analyze(pkm);
                if (Info.PIDIV.Type != PIDType.G5MGShiny && pkm.Egg_Location != Locations.LinkTrade5)
                    data.AddLine(GetInvalid(LPIDTypeMismatch, PID));
            }

            var result = pkm.FatefulEncounter != pkm.WasLink
                ? GetValid(LFatefulMystery, Fateful)
                : GetInvalid(LFatefulMysteryMissing, Fateful);
            data.AddLine(result);
        }

        private static void VerifyReceivability(LegalityAnalysis data, MysteryGift g)
        {
            var pkm = data.pkm;
            switch (g)
            {
                case WC6 wc6 when !wc6.CanBeReceivedByVersion(pkm.Version) && !pkm.WasTradedEgg:
                case WC7 wc7 when !wc7.CanBeReceivedByVersion(pkm.Version) && !pkm.WasTradedEgg:
                case WC8 wc8 when !wc8.CanBeReceivedByVersion(pkm.Version) && !pkm.WasTradedEgg:
                    data.AddLine(GetInvalid(LEncGiftVersionNotDistributed, GameOrigin));
                    return;
                case WC6 wc6 when wc6.RestrictLanguage != 0 && wc6.Language != wc6.RestrictLanguage:
                    data.AddLine(GetInvalid(string.Format(LOTLanguage, wc6.RestrictLanguage, pkm.Language), CheckIdentifier.Language));
                    return;
                case WC7 wc7 when wc7.RestrictLanguage != 0 && wc7.Language != wc7.RestrictLanguage:
                    data.AddLine(GetInvalid(string.Format(LOTLanguage, wc7.RestrictLanguage, pkm.Language), CheckIdentifier.Language));
                    return;
            }
        }

        private static void VerifyWC3Shiny(LegalityAnalysis data, WC3 g3)
        {
            // check for shiny locked gifts
            if (!g3.Shiny.IsValid(data.pkm))
                data.AddLine(GetInvalid(LEncGiftShinyMismatch, Fateful));
        }

        private static void VerifyFatefulIngameActive(LegalityAnalysis data)
        {
            var pkm = data.pkm;
            var result = pkm.FatefulEncounter
                ? GetValid(LFateful, Fateful)
                : GetInvalid(LFatefulMissing, Fateful);
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
                case (int)Species.Lycanroc when (pkm.AltForm == 0 && Moon()) || (pkm.AltForm == 1 && Sun()):
                case (int)Species.Solgaleo when Moon():
                case (int)Species.Lunala when Sun():
                    bool Sun() => (pkm.Version & 1) == 0;
                    bool Moon() => (pkm.Version & 1) == 1;
                    if (pkm.IsUntraded)
                        data.AddLine(GetInvalid(LEvoTradeRequired, Evolution));
                    break;
            }
        }

        private static void VerifyBelugaStats(LegalityAnalysis data, PB7 pb7)
        {
            // ReSharper disable once CompareOfFloatsByEqualityOperator -- THESE MUST MATCH EXACTLY
            if (!IsCloseEnough(pb7.HeightAbsolute, pb7.CalcHeightAbsolute))
                data.AddLine(GetInvalid(LStatIncorrectHeight, Encounter));
            // ReSharper disable once CompareOfFloatsByEqualityOperator -- THESE MUST MATCH EXACTLY
            if (!IsCloseEnough(pb7.WeightAbsolute, pb7.CalcWeightAbsolute))
                data.AddLine(GetInvalid(LStatIncorrectWeight, Encounter));
            if (pb7.Stat_CP != pb7.CalcCP && !IsStarter(pb7))
                data.AddLine(GetInvalid(LStatIncorrectCP, Encounter));
        }

        private static bool IsCloseEnough(float a, float b)
        {
            var ia = BitConverter.ToInt32(BitConverter.GetBytes(a), 0);
            var ib = BitConverter.ToInt32(BitConverter.GetBytes(b), 0);
            return Math.Abs(ia - ib) <= 7;
        }

        private static bool IsStarter(PKM pb7) => (pb7.Species == (int)Species.Pikachu && pb7.AltForm == 8) || (pb7.Species == (int)Species.Eevee && pb7.AltForm == 1);

        private void VerifySWSHStats(LegalityAnalysis data, PK8 pk8)
        {
            if (pk8.Favorite)
                data.AddLine(GetInvalid(LFavoriteMarkingUnavailable, Encounter));

            var gflag = data.EncounterMatch is IGigantamax g && g.CanGigantamax;
            if (gflag != pk8.CanGigantamax)
                data.AddLine(GetInvalid(LStatGigantamaxInvalid));

            if (pk8.DynamaxLevel != 0)
            {
                if (pk8.IsEgg || pk8.DynamaxLevel > 10 || pk8.Species >= (int)Species.Zacian)
                    data.AddLine(GetInvalid(LStatDynamaxInvalid));
            }

            PersonalInfo? pi = null;
            for (int i = 0; i < 100; i++)
            {
                if (!pk8.GetMoveRecordFlag(i))
                    continue;
                if (!(pi ??= pk8.PersonalInfo).TMHM[i + 100])
                    data.AddLine(GetInvalid(string.Format(LMoveSourceTR, LegalityAnalysis.MoveStrings[Legal.TMHM_SWSH[i + 100]])));
            }

            // weight/height scalars can be legally 0 (1:65536) so don't bother checking
        }
    }
}
