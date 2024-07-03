using System;
using System.Text;
using static PKHeX.Core.LegalityCheckStrings;
using static PKHeX.Core.CheckIdentifier;

namespace PKHeX.Core;

/// <summary>
/// Verifies miscellaneous data including <see cref="IFatefulEncounter.FatefulEncounter"/> and minor values.
/// </summary>
public sealed class MiscVerifier : Verifier
{
    protected override CheckIdentifier Identifier => Misc;

    public override void Verify(LegalityAnalysis data)
    {
        var pk = data.Entity;
        if (pk.IsEgg)
        {
            VerifyMiscEggCommon(data);

            if (pk is IContestStatsReadOnly s && s.HasContestStats())
                data.AddLine(GetInvalid(LEggContest, Egg));

            switch (pk)
            {
                case PK5 pk5 when pk5.PokeStarFame != 0 && pk5.IsEgg:
                    data.AddLine(GetInvalid(LEggShinyPokeStar, Egg));
                    break;
                case PK4 pk4 when pk4.ShinyLeaf != 0:
                    data.AddLine(GetInvalid(LEggShinyLeaf, Egg));
                    break;
                case PK4 pk4 when pk4.PokeathlonStat != 0:
                    data.AddLine(GetInvalid(LEggPokeathlon, Egg));
                    break;
                case PK3 when pk.Language != 1:  // All Eggs are Japanese and flagged specially for localized string
                    data.AddLine(GetInvalid(string.Format(LOTLanguage, LanguageID.Japanese, (LanguageID)pk.Language), Egg));
                    break;
            }

            if (pk is IHomeTrack { HasTracker: true })
                data.AddLine(GetInvalid(LTransferTrackerShouldBeZero));
        }

        switch (pk)
        {
            case PK5 pk5:
                VerifyGen5Stats(data, pk5);
                break;
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
            case PK9 pk9:
                VerifySVStats(data, pk9);
                break;
        }

        if (pk.Format >= 6)
            VerifyFullness(data, pk);

        var enc = data.EncounterMatch;
        if (enc is IEncounterServerDate { IsDateRestricted: true } serverGift)
        {
            var date = new DateOnly(pk.MetYear + 2000, pk.MetMonth, pk.MetDay);

            // HOME Gifts for Sinnoh/Hisui starters were forced JPN until May 20, 2022 (UTC).
            if (enc is WB8 { CardID: 9015 or 9016 or 9017 } or WA8 { CardID: 9018 or 9019 or 9020 })
            {
                if (date < new DateOnly(2022, 5, 20) && pk.Language != (int)LanguageID.Japanese)
                    data.AddLine(GetInvalid(LDateOutsideDistributionWindow));
            }

            var result = serverGift.IsValidDate(date);
            if (result == EncounterServerDateCheck.Invalid)
                data.AddLine(GetInvalid(LDateOutsideDistributionWindow));
        }
        else if (enc is IOverworldCorrelation8 z)
        {
            var match = z.IsOverworldCorrelationCorrect(pk);
            var req = z.GetRequirement(pk);
            if (match)
            {
                var seed = Overworld8RNG.GetOriginalSeed(pk);
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
            var match = s8b.IsStaticCorrelationCorrect(pk);
            var req = s8b.GetRequirement(pk);
            if (match)
                data.Info.PIDIV = new PIDIV(PIDType.Roaming8b, pk.EncryptionConstant);

            bool valid = req switch
            {
                StaticCorrelation8bRequirement.MustHave => match,
                StaticCorrelation8bRequirement.MustNotHave => !match,
                _ => true,
            };

            if (!valid)
                data.AddLine(GetInvalid(LPIDTypeMismatch));
        }
        else if (enc is ISeedCorrelation64<PKM> s64)
        {
            if (s64.TryGetSeed(pk, out var seed))
                data.Info.PIDIV = new PIDIV(PIDType.Xoroshiro, seed);
            if (enc is IMasteryInitialMoveShop8 m && !m.IsForcedMasteryCorrect(pk))
                data.AddLine(GetInvalid(LEncMasteryInitial));
        }

        VerifyMiscFatefulEncounter(data);
        VerifyMiscPokerus(data);
        if (pk is IScaledSize3 s3 and IScaledSize s2 && IsHeightScaleMatchRequired(pk) && s2.HeightScalar != s3.Scale)
            data.AddLine(GetInvalid(LStatIncorrectHeightValue));
    }

    private static void VerifyGen5Stats(LegalityAnalysis data, PK5 pk5)
    {
        var enc = data.EncounterMatch;
        if (enc is EncounterStatic5N)
        {
            if (!pk5.NSparkle)
                data.AddLine(GetInvalid(LG5SparkleRequired, Fateful));
        }
        else
        {
            if (pk5.NSparkle)
                data.AddLine(GetInvalid(LG5SparkleInvalid, Fateful));
        }
    }

    private static bool IsHeightScaleMatchRequired(PKM pk)
    {
        if (pk is IHomeTrack { HasTracker: false })
            return false;
        return true;
    }

    private void VerifySVStats(LegalityAnalysis data, PK9 pk9)
    {
        VerifyStatNature(data, pk9);
        VerifyTechRecordSV(data, pk9);

        if (!pk9.IsBattleVersionValid(data.Info.EvoChainsAllGens))
            data.AddLine(GetInvalid(LStatBattleVersionInvalid));
        if (!IsObedienceLevelValid(pk9, pk9.ObedienceLevel, pk9.MetLevel))
            data.AddLine(GetInvalid(LTransferObedienceLevel));
        if (pk9.IsEgg)
        {
            if (pk9.TeraTypeOverride != (MoveType)TeraTypeUtil.OverrideNone)
                data.AddLine(GetInvalid(LTeraTypeIncorrect));
        }
        else if (pk9.Species == (int)Species.Terapagos)
        {
            if (!TeraTypeUtil.IsValidTerapagos((byte)pk9.TeraTypeOverride))
                data.AddLine(GetInvalid(LTeraTypeIncorrect));
        }
        else if (pk9.Species == (int)Species.Ogerpon)
        {
            if (!TeraTypeUtil.IsValidOgerpon((byte)pk9.TeraTypeOverride, pk9.Form))
                data.AddLine(GetInvalid(LTeraTypeIncorrect));
        }
        else
        {
            if (!TeraTypeUtil.IsValid((byte)pk9.TeraTypeOriginal))
                data.AddLine(GetInvalid(LTeraTypeIncorrect));
        }

        var enc = data.EncounterOriginal;
        if (pk9 is { HeightScalar: 0, WeightScalar: 0 })
        {
            if (enc.Generation < 8 && !data.Info.EvoChainsAllGens.HasVisitedPLA && enc is not IPogoSlot) // <=Gen8 rerolls height/weight, never zero.
                data.AddLine(Get(LStatInvalidHeightWeight, Severity.Invalid, Encounter));
            else if (CheckHeightWeightOdds(enc) && ParseSettings.Settings.HOMETransfer.ZeroHeightWeight != Severity.Valid)
                data.AddLine(Get(LStatInvalidHeightWeight, ParseSettings.Settings.HOMETransfer.ZeroHeightWeight, Encounter));
        }

        if (enc is EncounterEgg { Context: EntityContext.Gen9 } g)
        {
            if (!Tera9RNG.IsMatchTeraTypePersonalEgg(g.Species, g.Form, (byte)pk9.TeraTypeOriginal))
                data.AddLine(GetInvalid(LTeraTypeMismatch));
        }
        else if (enc is ITeraRaid9)
        {
            var seed = Tera9RNG.GetOriginalSeed(pk9);
            data.Info.PIDIV = new PIDIV(PIDType.Tera9, seed);
        }
        else if (enc is not { Context: EntityContext.Gen9 } || pk9 is { GO_HOME: true })
        {
            if (pk9.TeraTypeOverride == (MoveType)TeraTypeUtil.OverrideNone)
                data.AddLine(GetInvalid(LTeraTypeIncorrect));
            else if (GetTeraImportMatch(data.Info.EvoChainsAllGens.Gen9, pk9.TeraTypeOriginal, enc) == -1)
                data.AddLine(GetInvalid(LTeraTypeIncorrect));
        }
        else if (enc is EncounterStatic9 { StarterBoxLegend: true })
        {
            // Ride legends cannot be traded or transferred.
            if (pk9.CurrentHandler != 0 || pk9.Tracker != 0 || !pk9.IsUntraded)
                data.AddLine(GetInvalid(LTransferBad));
        }

        if (!Locations9.IsAccessiblePreDLC(pk9.MetLocation))
        {
            if (enc is { Species: (int)Species.Larvesta, Form: 0 } and not EncounterEgg)
                DisallowLevelUpMove(24, (ushort)Move.BugBite, pk9, data);
            else if (enc is { Species: (int)Species.Zorua, Form: 1 } and not EncounterEgg)
                DisallowLevelUpMove(28, (ushort)Move.Spite, pk9, data);
            else
                return;

            // Safari and Sport are not obtainable in the base game.
            // For the learnset restricted cases, we need to check if the ball is available too.
            if (((BallUseLegality.WildPokeballs9PreDLC2 >> pk9.Ball) & 1) != 1)
                data.AddLine(GetInvalid(LBallUnavailable));
        }
    }

    private static void DisallowLevelUpMove(byte level, ushort move, PK9 pk, LegalityAnalysis data)
    {
        if (pk.Tracker != 0)
            return;
        int index = pk.GetMoveIndex(move);
        if (index == -1)
            return;

        ref var m = ref data.Info.Moves[index];
        if (m.Info.Method != LearnMethod.LevelUp || m.Info.Argument != level)
            return;
        var flagIndex = pk.Permit.RecordPermitIndexes.IndexOf(move);
        ArgumentOutOfRangeException.ThrowIfNegative(flagIndex, nameof(move)); // Always expect it to match.
        if (pk.GetMoveRecordFlag(flagIndex))
            return;
        m = m with { Info = m.Info with { Method = LearnMethod.None} };
    }

    public static int GetTeraImportMatch(ReadOnlySpan<EvoCriteria> evos, MoveType actual, IEncounterTemplate enc)
    {
        // Sanitize out Form here for Arceus
        if (evos.Length == 0 || evos[0].Species is (int)Species.Arceus)
            return actual == MoveType.Normal ? 0 : -1;
        for (int i = evos.Length - 1; i >= 0; i--)
        {
            var evo = evos[i];
            if (FormInfo.IsFormChangeable(evo.Species, enc.Form, evo.Form, enc.Context, EntityContext.Gen9))
            {
                if (Tera9RNG.IsMatchTeraTypePersonalAnyFormImport(evo.Species, (byte)actual))
                    return i;
            }
            else
            {
                if (Tera9RNG.IsMatchTeraTypePersonalImport(evo.Species, evo.Form, (byte)actual))
                    return i;
            }
        }
        return -1;
    }

    private static bool IsObedienceLevelValid(PKM pk, byte current, int expectObey)
    {
        if (current > pk.CurrentLevel)
            return false;
        if (!pk.IsUntraded)
            return current >= expectObey;
        return current == expectObey;
    }

    private void VerifyMiscPokerus(LegalityAnalysis data)
    {
        var pk = data.Entity;
        if (pk.Format == 1)
            return;

        var strain = pk.PokerusStrain;
        var days = pk.PokerusDays;
        bool strainValid = Pokerus.IsStrainValid(pk, strain, days);
        if (!strainValid)
            data.AddLine(GetInvalid(string.Format(LPokerusStrainUnobtainable_0, strain)));

        bool daysValid = Pokerus.IsDurationValid(strain, days, out var max);
        if (!daysValid)
            data.AddLine(GetInvalid(string.Format(LPokerusDaysTooHigh_0, max)));
    }

    public void VerifyMiscG1(LegalityAnalysis data)
    {
        var pk = data.Entity;
        if (pk.IsEgg)
            VerifyMiscEggCommon(data);

        if (pk is not PK1 pk1)
        {
            if (pk is ICaughtData2 { CaughtData: not 0 } t)
            {
                var time = t.MetTimeOfDay;
                bool valid = data.EncounterOriginal switch
                {
                    EncounterGift2 g2 when (!g2.IsEgg || pk.IsEgg) => time == 0,
                    EncounterTrade2 => time == 0,
                    _ => time is 1 or 2 or 3,
                };
                if (!valid)
                    data.AddLine(new CheckResult(Severity.Invalid, Encounter, LMetDetailTimeOfDay));
            }
            return;
        }

        VerifyMiscG1Types(data, pk1);
        VerifyMiscG1CatchRate(data, pk1);
    }

    private void VerifyMiscG1Types(LegalityAnalysis data, PK1 pk1)
    {
        var species = pk1.Species;
        if (species == (int)Species.Porygon)
        {
            // Can have any type combination of any species by using Conversion.
            if (!PersonalTable1.TypeIDExists(pk1.Type1))
            {
                data.AddLine(GetInvalid(LG1TypePorygonFail1));
            }
            if (!PersonalTable1.TypeIDExists(pk1.Type2))
            {
                data.AddLine(GetInvalid(LG1TypePorygonFail2));
            }
            else // Both types exist, ensure a Gen1 species has this combination
            {
                var matchSpecies = PersonalTable.RB.IsValidTypeCombination(pk1);
                var result = matchSpecies != -1 ? GetValid(LG1TypeMatchPorygon) : GetInvalid(LG1TypePorygonFail);
                data.AddLine(result);
            }
        }
        else // Types must match species types
        {
            var pi = PersonalTable.RB[species];
            var (match1, match2) = pi.IsMatchType(pk1);
            if (!match2 && ParseSettings.AllowGBStadium2)
                match2 = (species is (int)Species.Magnemite or (int)Species.Magneton) && pk1.Type2 == 9; // Steel Magnemite via Stadium2

            var first = match1 ? GetValid(LG1TypeMatch1) : GetInvalid(LG1Type1Fail);
            var second = match2 ? GetValid(LG1TypeMatch2) : GetInvalid(LG1Type2Fail);
            data.AddLine(first);
            data.AddLine(second);
        }
    }

    private void VerifyMiscG1CatchRate(LegalityAnalysis data, PK1 pk1)
    {
        var tradeback = GBRestrictions.IsTimeCapsuleTransferred(pk1, data.Info.Moves, data.EncounterMatch);
        var result = tradeback is TimeCapsuleEvaluation.NotTransferred or TimeCapsuleEvaluation.BadCatchRate
            ? GetWasNotTradeback(data, pk1, tradeback)
            : GetWasTradeback(data, pk1, tradeback);
        data.AddLine(result);
    }

    private CheckResult GetWasTradeback(LegalityAnalysis data, PK1 pk1, TimeCapsuleEvaluation eval)
    {
        var rate = pk1.CatchRate;
        if (PK1.IsCatchRateHeldItem(rate))
            return GetValid(LG1CatchRateMatchTradeback);
        return GetWasNotTradeback(data, pk1, eval);
    }

    private CheckResult GetWasNotTradeback(LegalityAnalysis data, PK1 pk1, TimeCapsuleEvaluation eval)
    {
        var rate = pk1.CatchRate;
        if (MoveInfo.IsAnyFromGeneration(2, data.Info.Moves))
            return GetInvalid(LG1CatchRateItem);
        var e = data.EncounterMatch;
        if (e is EncounterGift1 { Version: GameVersion.Stadium } or EncounterTrade1)
            return GetValid(LG1CatchRateMatchPrevious); // Encounters detected by the catch rate, cant be invalid if match this encounters

        ushort species = pk1.Species;
        if (GBRestrictions.IsSpeciesNotAvailableCatchRate((byte)species) && rate == PersonalTable.RB[species].CatchRate)
        {
            if (species != (int)Species.Dragonite || rate != 45 || !(e.Version == GameVersion.BU || e.Version.Contains(GameVersion.YW)))
                return GetInvalid(LG1CatchRateEvo);
        }
        if (!GBRestrictions.RateMatchesEncounter(e.Species, e.Version, rate))
            return GetInvalid(eval == TimeCapsuleEvaluation.Transferred12 ? LG1CatchRateChain : LG1CatchRateNone);
        return GetValid(LG1CatchRateMatchPrevious);
    }

    private static void VerifyMiscFatefulEncounter(LegalityAnalysis data)
    {
        var pk = data.Entity;
        switch (data.EncounterMatch)
        {
            case WC3 {FatefulEncounter: true} w:
                if (w.IsEgg)
                {
                    // Eggs hatched in RS clear the obedience flag!
                    // Hatching in Gen3 doesn't change the origin version.
                    if (pk.Format != 3)
                        return; // possible hatched in either game, don't bother checking
                    if (Locations.IsMetLocation3RS(pk.MetLocation)) // hatched in RS or Emerald
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
            case IFatefulEncounterReadOnly {FatefulEncounter: true}: // ingame fateful
                VerifyFatefulIngameActive(data);
                return;
        }
        if (pk.FatefulEncounter)
            data.AddLine(GetInvalid(LFatefulInvalid, Fateful));
    }

    private static void VerifyMiscEggCommon(LegalityAnalysis data)
    {
        var pk = data.Entity;

        var enc = data.EncounterMatch;
        if (!EggStateLegality.GetIsEggHatchCyclesValid(pk, enc))
            data.AddLine(GetInvalid(LEggHatchCycles, Egg));

        if (pk.Format >= 6 && enc is EncounterEgg && !MovesMatchRelearn(pk))
        {
            const int moveCount = 4;
            var sb = new StringBuilder(64);
            for (int i = 0; i < moveCount; i++)
            {
                var move = pk.GetRelearnMove(i);
                var name = ParseSettings.GetMoveName(move);
                sb.Append(name);
                if (i != moveCount - 1)
                    sb.Append(", ");
            }
            var msg = string.Format(LMoveFExpect_0, sb);
            data.AddLine(GetInvalid(msg, Egg));
        }

        if (pk is ITechRecord record)
        {
            if (record.GetMoveRecordFlagAny())
                data.AddLine(GetInvalid(LEggRelearnFlags, Egg));
            if (pk.StatNature != pk.Nature)
                data.AddLine(GetInvalid(LEggNature, Egg));
        }
    }

    private static bool MovesMatchRelearn(PKM pk)
    {
        if (pk.Move1 != pk.RelearnMove1)
            return false;
        if (pk.Move2 != pk.RelearnMove2)
            return false;
        if (pk.Move3 != pk.RelearnMove3)
            return false;
        if (pk.Move4 != pk.RelearnMove4)
            return false;
        return true;
    }

    private static void VerifyFatefulMysteryGift(LegalityAnalysis data, MysteryGift g)
    {
        var pk = data.Entity;
        if (g is PGF {IsShiny: true})
        {
            var Info = data.Info;
            Info.PIDIV = MethodFinder.Analyze(pk);
            if (Info.PIDIV.Type != PIDType.G5MGShiny && pk.EggLocation != Locations.LinkTrade5)
                data.AddLine(GetInvalid(LPIDTypeMismatch, PID));
        }

        bool shouldHave = g.FatefulEncounter;
        var result = pk.FatefulEncounter == shouldHave
            ? GetValid(LFatefulMystery, Fateful)
            : GetInvalid(LFatefulMysteryMissing, Fateful);
        data.AddLine(result);
    }

    private static void VerifyReceivability(LegalityAnalysis data, MysteryGift g)
    {
        var pk = data.Entity;
        switch (g)
        {
            case WC6 wc6 when !wc6.CanBeReceivedByVersion(pk.Version) && !pk.WasTradedEgg:
            case WC7 wc7 when !wc7.CanBeReceivedByVersion(pk.Version) && !pk.WasTradedEgg:
            case WC8 wc8 when !wc8.CanBeReceivedByVersion(pk.Version):
            case WB8 wb8 when !wb8.CanBeReceivedByVersion(pk.Version, pk):
            case WA8 wa8 when !wa8.CanBeReceivedByVersion(pk.Version, pk):
                data.AddLine(GetInvalid(LEncGiftVersionNotDistributed, GameOrigin));
                return;
            case PGF pgf when pgf.RestrictLanguage != 0 && pk.Language != pgf.RestrictLanguage:
                data.AddLine(GetInvalid(string.Format(LOTLanguage, pgf.RestrictLanguage, pk.Language), CheckIdentifier.Language));
                return;
            case WC6 wc6 when wc6.RestrictLanguage != 0 && pk.Language != wc6.RestrictLanguage:
                data.AddLine(GetInvalid(string.Format(LOTLanguage, wc6.RestrictLanguage, pk.Language), CheckIdentifier.Language));
                return;
            case WC7 wc7 when wc7.RestrictLanguage != 0 && pk.Language != wc7.RestrictLanguage:
                data.AddLine(GetInvalid(string.Format(LOTLanguage, wc7.RestrictLanguage, pk.Language), CheckIdentifier.Language));
                return;
        }
    }

    private static void VerifyWC3Shiny(LegalityAnalysis data, WC3 g3)
    {
        // check for shiny locked gifts
        if (!g3.Shiny.IsValid(data.Entity))
            data.AddLine(GetInvalid(LEncGiftShinyMismatch, Fateful));
    }

    private static void VerifyFatefulIngameActive(LegalityAnalysis data)
    {
        var pk = data.Entity;
        var result = pk.FatefulEncounter
            ? GetValid(LFateful, Fateful)
            : GetInvalid(LFatefulMissing, Fateful);
        data.AddLine(result);
    }

    public void VerifyVersionEvolution(LegalityAnalysis data)
    {
        var pk = data.Entity;
        if (pk.Format < 7 || data.EncounterMatch.Species == pk.Species)
            return;

        // No point using the evolution tree. Just handle certain species.
        switch (pk.Species)
        {
            case (int)Species.Lycanroc when pk.Format == 7 && ((pk.Form == 0 && Moon()) || (pk.Form == 1 && Sun())):
            case (int)Species.Solgaleo when Moon():
            case (int)Species.Lunala when Sun():
                bool Sun()  => ((uint)pk.Version & 1) == 0;
                bool Moon() => ((uint)pk.Version & 1) == 1;
                if (pk.IsUntraded)
                    data.AddLine(GetInvalid(LEvoTradeRequired, Evolution));
                break;
        }
    }

    private static void VerifyFullness(LegalityAnalysis data, PKM pk)
    {
        if (pk.IsEgg)
        {
            if (pk.Fullness != 0)
                data.AddLine(GetInvalid(string.Format(LMemoryStatFullness, "0"), Encounter));
            if (pk.Enjoyment != 0)
                data.AddLine(GetInvalid(string.Format(LMemoryStatEnjoyment, "0"), Encounter));
            return;
        }

        if (pk.Format >= 8)
        {
            if (pk.Fullness > 245) // Exiting camp is -10
                data.AddLine(GetInvalid(string.Format(LMemoryStatFullness, "<=245"), Encounter));
            else if (pk.Fullness is not 0 && pk is not PK8)
                data.AddLine(GetInvalid(string.Format(LMemoryStatFullness, "0"), Encounter));

            if (pk.Enjoyment != 0)
                data.AddLine(GetInvalid(string.Format(LMemoryStatEnjoyment, "0"), Encounter));
            return;
        }

        if (pk.Format != 6 || !pk.IsUntraded || pk.XY)
            return;

        // OR/AS PK6
        if (pk.Fullness == 0)
            return;
        if (pk.Species != data.EncounterMatch.Species)
            return; // evolved

        if (IsUnfeedable(pk.Species))
            data.AddLine(GetInvalid(string.Format(LMemoryStatFullness, "0"), Encounter));
    }

    public static bool IsUnfeedable(ushort species) => species is
        (int)Species.Metapod or
        (int)Species.Kakuna or
        (int)Species.Pineco or
        (int)Species.Silcoon or
        (int)Species.Cascoon or
        (int)Species.Shedinja or
        (int)Species.Spewpa;

    private static void VerifyBelugaStats(LegalityAnalysis data, PB7 pb7)
    {
        VerifyAbsoluteSizes(data, pb7);
        if (pb7.Stat_CP != pb7.CalcCP && !IsStarterLGPE(pb7))
            data.AddLine(GetInvalid(LStatIncorrectCP, Encounter));
    }

    private static void VerifyAbsoluteSizes(LegalityAnalysis data, IScaledSizeValue obj)
    {
        // ReSharper disable once CompareOfFloatsByEqualityOperator -- THESE MUST MATCH EXACTLY
        if (obj.HeightAbsolute != obj.CalcHeightAbsolute)
            data.AddLine(GetInvalid(LStatIncorrectHeight, Encounter));
        // ReSharper disable once CompareOfFloatsByEqualityOperator -- THESE MUST MATCH EXACTLY
        if (obj.WeightAbsolute != obj.CalcWeightAbsolute)
            data.AddLine(GetInvalid(LStatIncorrectWeight, Encounter));
    }

    private static bool IsStarterLGPE(ISpeciesForm pk) => pk switch
    {
        { Species: (int)Species.Pikachu, Form: 8 } => true,
        { Species: (int)Species.Eevee, Form: 1 } => true,
        _ => false,
    };

    private void VerifySWSHStats(LegalityAnalysis data, PK8 pk8)
    {
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

        if (!pk8.IsBattleVersionValid(data.Info.EvoChainsAllGens))
            data.AddLine(GetInvalid(LStatBattleVersionInvalid));

        var enc = data.EncounterMatch;
        bool originGMax = enc is IGigantamaxReadOnly {CanGigantamax: true};
        if (originGMax != pk8.CanGigantamax)
        {
            bool ok = !pk8.IsEgg && Gigantamax.CanToggle(pk8.Species, pk8.Form, enc.Species, enc.Form);
            var chk = ok ? GetValid(LStatGigantamaxValid) : GetInvalid(LStatGigantamaxInvalid);
            data.AddLine(chk);
        }

        if (pk8.DynamaxLevel != 0)
        {
            if (!pk8.CanHaveDynamaxLevel(pk8) || pk8.DynamaxLevel > 10)
                data.AddLine(GetInvalid(LStatDynamaxInvalid));
        }

        if (CheckHeightWeightOdds(data.EncounterMatch) && pk8 is { HeightScalar: 0, WeightScalar: 0 } && ParseSettings.Settings.HOMETransfer.ZeroHeightWeight != Severity.Valid)
            data.AddLine(Get(LStatInvalidHeightWeight, ParseSettings.Settings.HOMETransfer.ZeroHeightWeight, Encounter));

        VerifyTechRecordSWSH(data, pk8);
    }

    private void VerifyPLAStats(LegalityAnalysis data, PA8 pa8)
    {
        VerifyAbsoluteSizes(data, pa8);

        var social = pa8.Sociability;
        if (social != 0)
            data.AddLine(GetInvalid(LMemorySocialZero, Encounter));

        VerifyStatNature(data, pa8);

        if (!pa8.IsBattleVersionValid(data.Info.EvoChainsAllGens))
            data.AddLine(GetInvalid(LStatBattleVersionInvalid));

        if (pa8.CanGigantamax)
            data.AddLine(GetInvalid(LStatGigantamaxInvalid));

        if (pa8.DynamaxLevel != 0)
            data.AddLine(GetInvalid(LStatDynamaxInvalid));

        if (pa8.GetMoveRecordFlagAny() && !pa8.IsEgg) // already checked for eggs
            data.AddLine(GetInvalid(LEggRelearnFlags));

        if (CheckHeightWeightOdds(data.EncounterMatch) && pa8 is { HeightScalar: 0, WeightScalar: 0 } && ParseSettings.Settings.HOMETransfer.ZeroHeightWeight != Severity.Valid)
            data.AddLine(Get(LStatInvalidHeightWeight, ParseSettings.Settings.HOMETransfer.ZeroHeightWeight, Encounter));

        VerifyTechRecordSWSH(data, pa8);
    }

    private void VerifyBDSPStats(LegalityAnalysis data, PB8 pb8)
    {
        var social = pb8.Sociability;
        if (social != 0)
            data.AddLine(GetInvalid(LMemorySocialZero, Encounter));

        if (pb8.IsDprIllegal)
            data.AddLine(GetInvalid(LTransferFlagIllegal));
        if (pb8.Species is (int)Species.Spinda or (int)Species.Nincada && !pb8.BDSP)
            data.AddLine(GetInvalid(LTransferNotPossible));
        if (pb8.Species is (int)Species.Spinda && pb8.Tracker != 0)
            data.AddLine(GetInvalid(LTransferTrackerShouldBeZero));

        VerifyStatNature(data, pb8);

        if (!pb8.IsBattleVersionValid(data.Info.EvoChainsAllGens))
            data.AddLine(GetInvalid(LStatBattleVersionInvalid));

        if (pb8.CanGigantamax)
            data.AddLine(GetInvalid(LStatGigantamaxInvalid));

        if (pb8.DynamaxLevel != 0)
            data.AddLine(GetInvalid(LStatDynamaxInvalid));

        if (pb8.GetMoveRecordFlagAny() && !pb8.IsEgg) // already checked for eggs
            data.AddLine(GetInvalid(LEggRelearnFlags));

        if (CheckHeightWeightOdds(data.EncounterMatch) && pb8 is { HeightScalar: 0, WeightScalar: 0 } && ParseSettings.Settings.HOMETransfer.ZeroHeightWeight != Severity.Valid)
            data.AddLine(Get(LStatInvalidHeightWeight, ParseSettings.Settings.HOMETransfer.ZeroHeightWeight, Encounter));

        VerifyTechRecordSWSH(data, pb8);
    }

    private static bool CheckHeightWeightOdds(IEncounterTemplate enc)
    {
        if (enc.Generation < 8)
            return false;
        if (enc is WC8 { IsHOMEGift: true })
            return false;
        return true;
    }

    private void VerifyStatNature(LegalityAnalysis data, PKM pk)
    {
        var sn = (byte)pk.StatNature;
        if (sn == (byte)pk.Nature)
            return;
        // Only allow Serious nature (12); disallow all other neutral natures.
        if (sn != 12 && (sn > 24 || sn % 6 == 0))
            data.AddLine(GetInvalid(LStatNatureInvalid));
    }

    private static string GetMoveName<T>(T pk, int index) where T : PKM, ITechRecord => ParseSettings.MoveStrings[pk.Permit.RecordPermitIndexes[index]];

    private void VerifyTechRecordSWSH<T>(LegalityAnalysis data, T pk) where T : PKM, ITechRecord
    {
        var evos = data.Info.EvoChainsAllGens.Gen8;
        if (evos.Length == 0)
        {
            var count = pk.Permit.RecordCountUsed;
            for (int i = 0; i < count; i++)
            {
                if (!pk.GetMoveRecordFlag(i))
                    continue;
                data.AddLine(GetInvalid(string.Format(LMoveSourceTR, GetMoveName(pk, i))));
            }
        }
        else
        {
            static PersonalInfo8SWSH GetPersonal(EvoCriteria evo) => PersonalTable.SWSH.GetFormEntry(evo.Species, evo.Form);
            PersonalInfo8SWSH? pi = null;
            var count = pk.Permit.RecordCountUsed;
            for (int i = 0; i < count; i++)
            {
                if (!pk.GetMoveRecordFlag(i))
                    continue;
                if ((pi ??= GetPersonal(evos[0])).GetIsLearnTR(i))
                    continue;

                // Calyrex-0 can have TR flags for Calyrex-1/2 after it has force unlearned them.
                // Re-fusing can reacquire the move via relearner, rather than needing another TR.
                // Calyrex-0 cannot reacquire the move via relearner, even though the TR is checked off in the TR list.
                if (pk.Species == (int)Species.Calyrex)
                {
                    var form = pk.Form;
                    // Check if another alt form can learn the TR
                    if ((form != 1 && CanLearnTR((int)Species.Calyrex, 1, i)) || (form != 2 && CanLearnTR((int)Species.Calyrex, 2, i)))
                        continue;
                }

                data.AddLine(GetInvalid(string.Format(LMoveSourceTR, GetMoveName(pk, i))));
            }
        }
    }

    private static bool CanLearnTR(ushort species, byte form, int tr)
    {
        var pi = PersonalTable.SWSH.GetFormEntry(species, form);
        return pi.GetIsLearnTR(tr);
    }

    private void VerifyTechRecordSV(LegalityAnalysis data, PK9 pk)
    {
        var evos = data.Info.EvoChainsAllGens.Gen9;
        if (evos.Length == 0)
        {
            int count = pk.Permit.RecordCountUsed;
            for (int i = 0; i < count; i++)
            {
                if (!pk.GetMoveRecordFlag(i))
                    continue;
                data.AddLine(GetInvalid(string.Format(LMoveSourceTR, GetMoveName(pk, i))));
            }
        }
        else
        {
            static PersonalInfo9SV GetPersonal(EvoCriteria evo) => PersonalTable.SV.GetFormEntry(evo.Species, evo.Form);
            PersonalInfo9SV? pi = null;
            int count = pk.Permit.RecordCountUsed;
            for (int i = 0; i < count; i++)
            {
                if (!pk.GetMoveRecordFlag(i))
                    continue;
                if ((pi ??= GetPersonal(evos[0])).GetIsLearnTM(i))
                    continue;

                // Zoroark-0 cannot learn Encore via TM, but the pre-evolution Zorua-0 can via TM.
                // Double check if any pre-evolutions can learn the TM.
                bool preEvoHas = false;
                for (int p = 1; p < evos.Length; p++)
                {
                    if (!GetPersonal(evos[p]).GetIsLearnTM(i))
                        continue;
                    preEvoHas = true;
                    break;
                }
                if (!preEvoHas)
                    data.AddLine(GetInvalid(string.Format(LMoveSourceTR, GetMoveName(pk, i))));
            }
        }
    }
}
