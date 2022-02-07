using System;
using System.Collections.Generic;
using System.Linq;
using static PKHeX.Core.LegalityCheckStrings;
using static PKHeX.Core.CheckIdentifier;
using static System.Buffers.Binary.BinaryPrimitives;

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
                    case PK3 when pkm.Language != 1:  // All Eggs are Japanese and flagged specially for localized string
                        data.AddLine(GetInvalid(string.Format(LOTLanguage, LanguageID.Japanese, (LanguageID)pkm.Language), Egg));
                        break;
                }

                if (pkm is IHomeTrack {Tracker: not 0})
                    data.AddLine(GetInvalid(LTransferTrackerShouldBeZero));
            }
            else
            {
                VerifyMiscMovePP(data);
            }

            switch (pkm)
            {
                case PK7 {ResortEventStatus: >= ResortEventState.MAX}:
                    data.AddLine(GetInvalid(LTransferBad));
                    break;
                case PB7 pb7:
                    VerifyBelugaStats(data, pb7);
                    break;
                case PK8 pk8:
                    VerifySWSHStats(data, pk8);
                    break;
                case PB8 pb8:
                    VerifyBDSPStats(data, pb8);
                    break;
                case PA8 pa8:
                    VerifyPLAStats(data, pa8);
                    break;
            }

            if (pkm.Format >= 6)
                VerifyFullness(data, pkm);

            var enc = data.EncounterMatch;
            if (enc is WC8 { IsHOMEGift: true } w)
            {
                var date = new DateTime(pkm.Met_Year + 2000, pkm.Met_Month, pkm.Met_Day);
                if (!EncountersHOME.IsValidDateWC8(w.CardID, date))
                    data.AddLine(GetInvalid(LDateOutsideDistributionWindow));
            }
            else if (enc is IOverworldCorrelation8 z)
            {
                var match = z.IsOverworldCorrelationCorrect(pkm);
                var req = z.GetRequirement(pkm);
                if (match)
                {
                    var seed = Overworld8RNG.GetOriginalSeed(pkm);
                    data.Info.PIDIV = new PIDIV(PIDType.Overworld8, seed);
                }

                bool valid = req switch
                {
                    OverworldCorrelation8Requirement.MustHave => match,
                    OverworldCorrelation8Requirement.MustNotHave => !match,
                    _ => true,
                };

                if (!valid)
                    data.AddLine(GetInvalid(LPIDTypeMismatch));
            }
            else if (enc is IStaticCorrelation8b s8b)
            {
                var match = s8b.IsStaticCorrelationCorrect(pkm);
                var req = s8b.GetRequirement(pkm);
                if (match)
                    data.Info.PIDIV = new PIDIV(PIDType.Roaming8b, pkm.EncryptionConstant);

                bool valid = req switch
                {
                    StaticCorrelation8bRequirement.MustHave => match,
                    StaticCorrelation8bRequirement.MustNotHave => !match,
                    _ => true,
                };

                if (!valid)
                    data.AddLine(GetInvalid(LPIDTypeMismatch));
            }

            VerifyMiscFatefulEncounter(data);
            VerifyMiscPokerus(data);
        }

        private void VerifyMiscPokerus(LegalityAnalysis data)
        {
            var pkm = data.pkm;
            if (pkm.Format == 1)
                return;

            var strain = pkm.PKRS_Strain;
            var days = pkm.PKRS_Days;
            bool strainValid = IsPokerusStrainValid(pkm, strain, days);
            if (!strainValid)
                data.AddLine(GetInvalid(string.Format(LPokerusStrainUnobtainable_0, strain)));

            var expect = (strain % 4) + 1;
            if (days > expect)
                data.AddLine(GetInvalid(string.Format(LPokerusDaysTooHigh_0, expect)));
        }

        private static bool IsPokerusStrainValid(PKM pkm, int strain, int days) => strain switch
        {
            0 when days is not 0 => false,
            8 => false,
            not 0 when pkm is PA8 => false,
            _ => true,
        };

        public void VerifyMiscG1(LegalityAnalysis data)
        {
            var pkm = data.pkm;
            if (pkm.IsEgg)
                VerifyMiscEggCommon(data);

            if (pkm is not PK1 pk1)
                return;

            VerifyMiscG1Types(data, pk1);
            VerifyMiscG1CatchRate(data, pk1);
        }

        private void VerifyMiscG1Types(LegalityAnalysis data, PK1 pk1)
        {
            var Type_A = pk1.Type_A;
            var Type_B = pk1.Type_B;
            var species = pk1.Species;
            if (species == (int)Species.Porygon)
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
                else // Both types exist, ensure a Gen1 species has this combination
                {
                    var TypesAB_Match = PersonalTable.RB.IsValidTypeCombination(Type_A, Type_B);
                    var result = TypesAB_Match ? GetValid(LG1TypeMatchPorygon) : GetInvalid(LG1TypePorygonFail);
                    data.AddLine(result);
                }
            }
            else // Types must match species types
            {
                var pi = PersonalTable.RB[species];
                var Type_A_Match = Type_A == pi.Type1;
                var Type_B_Match = Type_B == pi.Type2;

                var first = Type_A_Match ? GetValid(LG1TypeMatch1) : GetInvalid(LG1Type1Fail);
                var second = Type_B_Match || (ParseSettings.AllowGBCartEra && ((species is (int)Species.Magnemite or (int)Species.Magneton) && Type_B == 9)) // Steel Magnemite via Stadium2
                    ? GetValid(LG1TypeMatch2) : GetInvalid(LG1Type2Fail);
                data.AddLine(first);
                data.AddLine(second);
            }
        }

        private void VerifyMiscG1CatchRate(LegalityAnalysis data, PK1 pk1)
        {
            var catch_rate = pk1.Catch_Rate;
            var tradeback = GBRestrictions.IsTimeCapsuleTransferred(pk1, data.Info.Moves, data.EncounterMatch);
            var result = tradeback is TimeCapsuleEvaluation.NotTransferred or TimeCapsuleEvaluation.BadCatchRate
                ? GetWasNotTradeback(tradeback)
                : GetWasTradeback(tradeback);
            data.AddLine(result);

            CheckResult GetWasTradeback(TimeCapsuleEvaluation timeCapsuleEvalution)
            {
                if (catch_rate == 0 || Legal.HeldItems_GSC.Contains((ushort)catch_rate))
                    return GetValid(LG1CatchRateMatchTradeback);
                if (timeCapsuleEvalution == TimeCapsuleEvaluation.BadCatchRate)
                    return GetInvalid(LG1CatchRateItem);

                return GetWasNotTradeback(timeCapsuleEvalution);
            }

            CheckResult GetWasNotTradeback(TimeCapsuleEvaluation timeCapsuleEvalution)
            {
                if (data.Info.Moves.Any(z => z.Generation == 2))
                    return GetInvalid(LG1CatchRateItem);
                var e = data.EncounterMatch;
                if (e is EncounterStatic1E {Version: GameVersion.Stadium} or EncounterTrade1)
                    return GetValid(LG1CatchRateMatchPrevious); // Encounters detected by the catch rate, cant be invalid if match this encounters

                int species = pk1.Species;
                if (GBRestrictions.Species_NotAvailable_CatchRate.Contains(species) && catch_rate == PersonalTable.RB[species].CatchRate)
                {
                    if (species != (int) Species.Dragonite || catch_rate != 45 || !e.Version.Contains(GameVersion.YW))
                        return GetInvalid(LG1CatchRateEvo);
                }
                if (!GBRestrictions.RateMatchesEncounter(e.Species, e.Version, catch_rate))
                    return GetInvalid(timeCapsuleEvalution == TimeCapsuleEvaluation.Transferred12 ? LG1CatchRateChain : LG1CatchRateNone);
                return GetValid(LG1CatchRateMatchPrevious);
            }
        }

        private static void VerifyMiscFatefulEncounter(LegalityAnalysis data)
        {
            var pkm = data.pkm;
            var enc = data.EncounterMatch;
            switch (enc)
            {
                case WC3 {Fateful: true} w:
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
                case MysteryGift g: // WC3 handled above
                    VerifyReceivability(data, g);
                    VerifyFatefulMysteryGift(data, g);
                    return;
                case EncounterStatic {Fateful: true}: // ingame fateful
                case EncounterSlot3PokeSpot: // ingame pokespot
                case EncounterTrade4RanchSpecial: // ranch varied PID
                    VerifyFatefulIngameActive(data);
                    return;
            }
            if (pkm.FatefulEncounter)
                data.AddLine(GetInvalid(LFatefulInvalid, Fateful));
        }

        private static void VerifyMiscMovePP(LegalityAnalysis data)
        {
            var pkm = data.pkm;

            if (!Legal.IsPPUpAvailable(pkm)) // No PP Ups
            {
                if (pkm.Move1_PPUps is not 0)
                    data.AddLine(GetInvalid(string.Format(LMovePPUpsTooHigh_0, 1), CurrentMove));
                if (pkm.Move2_PPUps is not 0)
                    data.AddLine(GetInvalid(string.Format(LMovePPUpsTooHigh_0, 2), CurrentMove));
                if (pkm.Move3_PPUps is not 0)
                    data.AddLine(GetInvalid(string.Format(LMovePPUpsTooHigh_0, 3), CurrentMove));
                if (pkm.Move4_PPUps is not 0)
                    data.AddLine(GetInvalid(string.Format(LMovePPUpsTooHigh_0, 4), CurrentMove));
            }

            if (pkm.Move1_PP > pkm.GetMovePP(pkm.Move1, pkm.Move1_PPUps))
                data.AddLine(GetInvalid(string.Format(LMovePPTooHigh_0, 1), CurrentMove));
            if (pkm.Move2_PP > pkm.GetMovePP(pkm.Move2, pkm.Move2_PPUps))
                data.AddLine(GetInvalid(string.Format(LMovePPTooHigh_0, 2), CurrentMove));
            if (pkm.Move3_PP > pkm.GetMovePP(pkm.Move3, pkm.Move3_PPUps))
                data.AddLine(GetInvalid(string.Format(LMovePPTooHigh_0, 3), CurrentMove));
            if (pkm.Move4_PP > pkm.GetMovePP(pkm.Move4, pkm.Move4_PPUps))
                data.AddLine(GetInvalid(string.Format(LMovePPTooHigh_0, 4), CurrentMove));
        }

        private static void VerifyMiscEggCommon(LegalityAnalysis data)
        {
            var pkm = data.pkm;
            if (pkm.Move1_PPUps > 0 || pkm.Move2_PPUps > 0 || pkm.Move3_PPUps > 0 || pkm.Move4_PPUps > 0)
                data.AddLine(GetInvalid(LEggPPUp, Egg));
            if (pkm.Move1_PP != pkm.GetMovePP(pkm.Move1, 0) || pkm.Move2_PP != pkm.GetMovePP(pkm.Move2, 0) || pkm.Move3_PP != pkm.GetMovePP(pkm.Move3, 0) || pkm.Move4_PP != pkm.GetMovePP(pkm.Move4, 0))
                data.AddLine(GetInvalid(LEggPP, Egg));

            var enc = data.EncounterMatch;
            if (!EggStateLegality.GetIsEggHatchCyclesValid(pkm, enc))
                data.AddLine(GetInvalid(LEggHatchCycles, Egg));

            if (pkm.Format >= 6 && enc is EncounterEgg && !MovesMatchRelearn(pkm))
            {
                var moves = string.Join(", ", ParseSettings.GetMoveNames(pkm.Moves));
                var msg = string.Format(LMoveFExpect_0, moves);
                data.AddLine(GetInvalid(msg, Egg));
            }

            if (pkm is ITechRecord8 pk8)
            {
                if (pk8.GetMoveRecordFlagAny())
                    data.AddLine(GetInvalid(LEggRelearnFlags, Egg));
                if (pkm.StatNature != pkm.Nature)
                    data.AddLine(GetInvalid(LEggNature, Egg));
            }
        }

        private static bool MovesMatchRelearn(PKM pkm)
        {
            if (pkm.Move1 != pkm.RelearnMove1)
                return false;
            if (pkm.Move2 != pkm.RelearnMove2)
                return false;
            if (pkm.Move3 != pkm.RelearnMove3)
                return false;
            if (pkm.Move4 != pkm.RelearnMove4)
                return false;
            return true;
        }

        private static void VerifyFatefulMysteryGift(LegalityAnalysis data, MysteryGift g)
        {
            var pkm = data.pkm;
            if (g is PGF {IsShiny: true})
            {
                var Info = data.Info;
                Info.PIDIV = MethodFinder.Analyze(pkm);
                if (Info.PIDIV.Type != PIDType.G5MGShiny && pkm.Egg_Location != Locations.LinkTrade5)
                    data.AddLine(GetInvalid(LPIDTypeMismatch, PID));
            }

            bool shouldHave = GetFatefulState(g);
            var result = pkm.FatefulEncounter == shouldHave
                ? GetValid(LFatefulMystery, Fateful)
                : GetInvalid(LFatefulMysteryMissing, Fateful);
            data.AddLine(result);
        }

        private static bool GetFatefulState(MysteryGift g)
        {
            if (g is WC6 {IsLinkGift: true})
                return false; // Pokémon Link fake-gifts do not have Fateful
            return true;
        }

        private static void VerifyReceivability(LegalityAnalysis data, MysteryGift g)
        {
            var pkm = data.pkm;
            switch (g)
            {
                case WC6 wc6 when !wc6.CanBeReceivedByVersion(pkm.Version) && !pkm.WasTradedEgg:
                case WC7 wc7 when !wc7.CanBeReceivedByVersion(pkm.Version) && !pkm.WasTradedEgg:
                case WC8 wc8 when !wc8.CanBeReceivedByVersion(pkm.Version):
                case WB8 wb8 when !wb8.CanBeReceivedByVersion(pkm.Version):
                case WA8 wa8 when !wa8.CanBeReceivedByVersion(pkm.Version):
                    data.AddLine(GetInvalid(LEncGiftVersionNotDistributed, GameOrigin));
                    return;
                case WC6 wc6 when wc6.RestrictLanguage != 0 && pkm.Language != wc6.RestrictLanguage:
                    data.AddLine(GetInvalid(string.Format(LOTLanguage, wc6.RestrictLanguage, pkm.Language), CheckIdentifier.Language));
                    return;
                case WC7 wc7 when wc7.RestrictLanguage != 0 && pkm.Language != wc7.RestrictLanguage:
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
                case (int)Species.Lycanroc when pkm.Format == 7 && ((pkm.Form == 0 && Moon()) || (pkm.Form == 1 && Sun())):
                case (int)Species.Solgaleo when Moon():
                case (int)Species.Lunala when Sun():
                    bool Sun() => (pkm.Version & 1) == 0;
                    bool Moon() => (pkm.Version & 1) == 1;
                    if (pkm.IsUntraded)
                        data.AddLine(GetInvalid(LEvoTradeRequired, Evolution));
                    break;
            }
        }

        private static void VerifyFullness(LegalityAnalysis data, PKM pkm)
        {
            if (pkm.IsEgg)
            {
                if (pkm.Fullness != 0)
                    data.AddLine(GetInvalid(string.Format(LMemoryStatFullness, 0), Encounter));
                if (pkm.Enjoyment != 0)
                    data.AddLine(GetInvalid(string.Format(LMemoryStatEnjoyment, 0), Encounter));
                return;
            }

            if (pkm.Format >= 8)
            {
                if (pkm.Fullness > 245) // Exiting camp is -10
                    data.AddLine(GetInvalid(string.Format(LMemoryStatFullness, "<=245"), Encounter));
                if (pkm.Enjoyment != 0)
                    data.AddLine(GetInvalid(string.Format(LMemoryStatEnjoyment, 0), Encounter));
                return;
            }

            if (pkm.Format != 6 || !pkm.IsUntraded || pkm.XY)
                return;

            // OR/AS PK6
            if (pkm.Fullness == 0)
                return;
            if (pkm.Species != data.EncounterMatch.Species)
                return; // evolved

            if (Unfeedable.Contains(pkm.Species))
                data.AddLine(GetInvalid(string.Format(LMemoryStatFullness, 0), Encounter));
        }

        private static readonly HashSet<int> Unfeedable = new()
        {
            (int)Species.Metapod,
            (int)Species.Kakuna,
            (int)Species.Pineco,
            (int)Species.Silcoon,
            (int)Species.Cascoon,
            (int)Species.Shedinja,
            (int)Species.Spewpa,
        };

        private static void VerifyBelugaStats(LegalityAnalysis data, PB7 pb7)
        {
            VerifyAbsoluteSizes(data, pb7);
            if (pb7.Stat_CP != pb7.CalcCP && !IsStarterLGPE(pb7))
                data.AddLine(GetInvalid(LStatIncorrectCP, Encounter));
        }

        private static void VerifyAbsoluteSizes(LegalityAnalysis data, IScaledSizeValue obj)
        {
            // ReSharper disable once CompareOfFloatsByEqualityOperator -- THESE MUST MATCH EXACTLY
            if (!IsCloseEnough(obj.HeightAbsolute, obj.CalcHeightAbsolute))
                data.AddLine(GetInvalid(LStatIncorrectHeight, Encounter));
            // ReSharper disable once CompareOfFloatsByEqualityOperator -- THESE MUST MATCH EXACTLY
            if (!IsCloseEnough(obj.WeightAbsolute, obj.CalcWeightAbsolute))
                data.AddLine(GetInvalid(LStatIncorrectWeight, Encounter));
        }

        private static bool IsCloseEnough(float a, float b)
        {
            // since we don't have access to SingleToInt32Bits on net46, just do a temp write-read.
            Span<byte> ta = stackalloc byte[4];
            WriteSingleLittleEndian(ta, a);
            var ia = ReadInt32LittleEndian(ta);
            WriteSingleLittleEndian(ta, b);
            var ib = ReadInt32LittleEndian(ta);
            return Math.Abs(ia - ib) <= 7;
        }

        private static bool IsStarterLGPE(ISpeciesForm pk) => pk.Species switch
        {
            (int)Species.Pikachu when pk.Form == 8 => true,
            (int)Species.Eevee   when pk.Form == 1 => true,
            _ => false,
        };

        private void VerifySWSHStats(LegalityAnalysis data, PK8 pk8)
        {
            if (pk8.Favorite)
                data.AddLine(GetInvalid(LFavoriteMarkingUnavailable, Encounter));

            var social = pk8.Sociability;
            if (pk8.IsEgg)
            {
                if (social != 0)
                    data.AddLine(GetInvalid(LMemorySocialZero, Encounter));
            }
            else if (social > byte.MaxValue)
            {
                data.AddLine(GetInvalid(string.Format(LMemorySocialTooHigh_0, byte.MaxValue), Encounter));
            }

            VerifyStatNature(data, pk8);

            var bv = pk8.BattleVersion;
            if (bv != 0)
            {
                if ((bv != (int)GameVersion.SW && bv != (int)GameVersion.SH) || pk8.SWSH)
                    data.AddLine(GetInvalid(LStatBattleVersionInvalid));
            }

            var enc = data.EncounterMatch;
            bool originGMax = enc is IGigantamax {CanGigantamax: true};
            if (originGMax != pk8.CanGigantamax)
            {
                bool ok = !pk8.IsEgg && pk8.CanToggleGigantamax(pk8.Species, pk8.Form, enc.Species, enc.Form);
                var chk = ok ? GetValid(LStatGigantamaxValid) : GetInvalid(LStatGigantamaxInvalid);
                data.AddLine(chk);
            }

            if (pk8.DynamaxLevel != 0)
            {
                if (!pk8.CanHaveDynamaxLevel(pk8) || pk8.DynamaxLevel > 10)
                    data.AddLine(GetInvalid(LStatDynamaxInvalid));
            }

            PersonalInfo? pi = null;
            for (int i = 0; i < PersonalInfoSWSH.CountTR; i++)
            {
                if (!pk8.GetMoveRecordFlag(i))
                    continue;
                if ((pi ??= pk8.PersonalInfo).TMHM[i + 100])
                    continue;

                // Calyrex-0 can have TR flags for Calyrex-1/2 after it has force unlearned them.
                // Re-fusing can be reacquire the move via relearner, rather than needing another TR.
                // Calyrex-0 cannot reacquire the move via relearner, even though the TR is checked off in the TR list.
                if (pk8.Species == (int) Species.Calyrex)
                {
                    var form = pk8.Form;
                    // Check if another alt form can learn the TR
                    if ((form != 1 && CanLearnTR((int) Species.Calyrex, 1, i)) || (form != 2 && CanLearnTR((int) Species.Calyrex, 2, i)))
                        continue;
                }

                data.AddLine(GetInvalid(string.Format(LMoveSourceTR, ParseSettings.MoveStrings[Legal.TMHM_SWSH[i + PersonalInfoSWSH.CountTM]])));
            }

            if (CheckHeightWeightOdds(data.EncounterMatch) && pk8.HeightScalar == 0 && pk8.WeightScalar == 0 && ParseSettings.ZeroHeightWeight != Severity.Valid)
                data.AddLine(Get(LStatInvalidHeightWeight, ParseSettings.ZeroHeightWeight, Encounter));
        }

        private void VerifyPLAStats(LegalityAnalysis data, PA8 pa8)
        {
            VerifyAbsoluteSizes(data, pa8);

            if (pa8.Favorite)
                data.AddLine(GetInvalid(LFavoriteMarkingUnavailable, Encounter));

            var affix = pa8.AffixedRibbon;
            if (affix != -1) // None
                data.AddLine(GetInvalid(string.Format(LRibbonMarkingAffixedF_0, affix)));

            var social = pa8.Sociability;
            if (social != 0)
                data.AddLine(GetInvalid(LMemorySocialZero, Encounter));

            VerifyStatNature(data, pa8);

            var bv = pa8.BattleVersion;
            if (bv != 0)
                data.AddLine(GetInvalid(LStatBattleVersionInvalid));

            if (pa8.CanGigantamax)
                data.AddLine(GetInvalid(LStatGigantamaxInvalid));

            if (pa8.DynamaxLevel != 0)
                data.AddLine(GetInvalid(LStatDynamaxInvalid));

            if (pa8.GetMoveRecordFlagAny() && !pa8.IsEgg) // already checked for eggs
                data.AddLine(GetInvalid(LEggRelearnFlags));

            if (CheckHeightWeightOdds(data.EncounterMatch) && pa8.HeightScalar == 0 && pa8.WeightScalar == 0 && ParseSettings.ZeroHeightWeight != Severity.Valid)
                data.AddLine(Get(LStatInvalidHeightWeight, ParseSettings.ZeroHeightWeight, Encounter));
        }

        private void VerifyBDSPStats(LegalityAnalysis data, PB8 pb8)
        {
            if (pb8.Favorite)
                data.AddLine(GetInvalid(LFavoriteMarkingUnavailable, Encounter));

            var affix = pb8.AffixedRibbon;
            if (affix != -1) // None
                data.AddLine(GetInvalid(string.Format(LRibbonMarkingAffixedF_0, affix)));

            var social = pb8.Sociability;
            if (social != 0)
                data.AddLine(GetInvalid(LMemorySocialZero, Encounter));

            VerifyStatNature(data, pb8);

            var bv = pb8.BattleVersion;
            if (bv != 0)
                data.AddLine(GetInvalid(LStatBattleVersionInvalid));

            if (pb8.CanGigantamax)
                data.AddLine(GetInvalid(LStatGigantamaxInvalid));

            if (pb8.DynamaxLevel != 0)
                data.AddLine(GetInvalid(LStatDynamaxInvalid));

            if (pb8.GetMoveRecordFlagAny() && !pb8.IsEgg) // already checked for eggs
                data.AddLine(GetInvalid(LEggRelearnFlags));

            if (CheckHeightWeightOdds(data.EncounterMatch) && pb8.HeightScalar == 0 && pb8.WeightScalar == 0 && ParseSettings.ZeroHeightWeight != Severity.Valid)
                data.AddLine(Get(LStatInvalidHeightWeight, ParseSettings.ZeroHeightWeight, Encounter));
        }

        private static bool CheckHeightWeightOdds(IEncounterTemplate enc)
        {
            if (enc.Generation < 8)
                return false;

            if (GameVersion.BDSP.Contains(enc.Version) || GameVersion.PLA == enc.Version)
                return true;

            if (enc is WC8 { IsHOMEGift: true })
                return false;
            if (GameVersion.SWSH.Contains(enc.Version))
                return true;
            return false;
        }

        private void VerifyStatNature(LegalityAnalysis data, PKM pk)
        {
            var sn = pk.StatNature;
            if (sn == pk.Nature)
                return;
            // Only allow Serious nature (12); disallow all other neutral natures.
            if (sn != 12 && (sn > 24 || sn % 6 == 0))
                data.AddLine(GetInvalid(LStatNatureInvalid));
        }

        private static bool CanLearnTR(int species, int form, int tr)
        {
            var pi = PersonalTable.SWSH.GetFormEntry(species, form);
            return pi.TMHM[tr + PersonalInfoSWSH.CountTM];
        }
    }
}
