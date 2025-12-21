using System;
using static PKHeX.Core.LegalityCheckResultCode;
using static PKHeX.Core.CheckIdentifier;

namespace PKHeX.Core;

/// <summary>
/// Verifies miscellaneous data including <see cref="IFatefulEncounter.FatefulEncounter"/> and minor values.
/// </summary>
public sealed class MiscVerifier : Verifier
{
    private static readonly LegendsArceusVerifier Arceus = new();
    private static readonly LegendsZAVerifier LegendsZA = new();

    protected override CheckIdentifier Identifier => Misc;

    public override void Verify(LegalityAnalysis data)
    {
        var pk = data.Entity;
        if (pk.IsEgg)
        {
            VerifyMiscEggCommon(data);

            // No egg have contest stats from the encounter.
            if (pk is IContestStatsReadOnly s && s.HasContestStats())
                data.AddLine(GetInvalid(Egg, EggContest));

            // Cannot transfer eggs across contexts (must be hatched).
            var e = data.EncounterOriginal;
            if (e.Context != pk.Context)
                data.AddLine(GetInvalid(Egg, TransferEggVersion));

            switch (pk)
            {
                // Side Game: No Eggs
                case SK2 or CK3 or XK3 or BK4 or RK4 when e.Context == pk.Context:
                    data.AddLine(GetInvalid(Egg, TransferEggVersion));
                    break;

                // All Eggs are Japanese and flagged specially for localized string
                case PK3 when pk.Language != 1:
                    data.AddLine(GetInvalid(Egg, OTLanguageShouldBe_0, (byte)LanguageID.Japanese));
                    break;
            }

            if (pk is IHomeTrack { HasTracker: true })
                data.AddLine(GetInvalid(TransferTrackerShouldBeZero));
        }

        switch (pk)
        {
            case SK2 sk2: VerifyIsMovesetAllowed(data, sk2); break;
            case G4PKM pk4: VerifyMisc4(data, pk4); break;
            case PK5 pk5: VerifyStats5(data, pk5); break;
            case PK7 pk7: VerifyStats7(data, pk7); break;
            case PB7 pb7: VerifyStats7b(data, pb7); break;
            case PK8 pk8: VerifyStats8(data, pk8); break;
            case PB8 pb8: VerifyStats8b(data, pb8); break;
            case PA8 pa8: VerifyStats8a(data, pa8); break;
            case PK9 pk9: VerifyStats9(data, pk9); break;
            case PA9 pa9: VerifyStats9a(data, pa9); break;
        }

        if (pk is IFullnessEnjoyment fe) // 6-8
            VerifyFullness(data, pk, fe);

        var enc = data.EncounterMatch;
        if (enc is IEncounterServerDate { IsDateRestricted: true } encounterDate)
            VerifyServerDate2000(data, pk, enc, encounterDate);

        if (enc is IOverworldCorrelation8 z)
        {
            VerifyCorrelation8(data, z, pk);
        }
        else if (enc is IStaticCorrelation8b s8b)
        {
            VerifyCorrelation8b(data, s8b, pk);
        }
        else if (enc is ISeedCorrelation64<PKM> s64)
        {
            var pidiv = s64.TryGetSeed(pk, out var seed);
            if (pidiv == SeedCorrelationResult.Success)
                data.Info.PIDIV = new PIDIV(PIDType.Xoroshiro, seed);
            else if (pidiv is SeedCorrelationResult.Invalid)
                data.AddLine(GetInvalid(PIDTypeMismatch));

            if (enc is IMasteryInitialMoveShop8 m && !m.IsForcedMasteryCorrect(pk))
                data.AddLine(GetInvalid(EncMasteryInitial));
        }

        VerifyMiscFatefulEncounter(data);
        VerifyMiscPokerus(data);
        VerifyMiscScaleValues(data, pk, enc);
        VerifyDateValues(data, pk);
    }

    private void VerifyMisc4(LegalityAnalysis data, G4PKM pk)
    {
        // Mood:
        // Range is [-127, 127]. Deduplicated unique adjustments are +8, +10, and -20.
        // Increment adjustments of -2 (-20 +8 +10) and +2 (+8*4 +10 -20) are possible.
        // Start from 0 or from clamped edge, with above adjustments, at least one mutation path can arrive at a given [-127, 127] value.
        // There are probably other scripted increments, but the above is just to prove that all values are possible via specific actions.
        // Eggs cannot have a value other than 0.
        // HG/SS resets when promoting to lead of party, or removing from party. Therefore, any party slot can retain a non-zero value.
        // Mood also resets for the lead Pokémon when booting the game, but it can be mutated immediately after (and saved/dumped).
        // Note: Trading to D/P/Pt does not clear (thus unchanged on side games too), so this check is only relevant for HG/SS slots.
        if (pk.WalkingMood != 0)
        {
            if (pk.IsEgg)
                data.AddLine(GetInvalid(Egg, G4PartnerMoodEgg));
            else if (!data.IsStoredSlot(StorageSlotType.Party) && ParseSettings.ActiveTrainer is SAV4HGSS)
                data.AddLine(GetInvalid(Egg, G4PartnerMoodZero));
        }

        // Shiny leaf: cannot have crown (bit 5) without bits 0-4. Bits higher are invalid.
        // Eggs cannot receive a Shiny Leaf.
        var leaf = pk.ShinyLeaf;
        if (leaf is > 0b_11111 and not 0b_1_11111 || (leaf is not 0 && pk.IsEgg))
            data.AddLine(GetInvalid(G4ShinyLeafBitsInvalid));
    }

    private static void VerifyDateValues(LegalityAnalysis data, PKM pk)
    {
        if (pk.Format <= 3)
            return; // Gen1-3 do not have date values.

        var expect0 = pk is PB8 ? Locations.Default8bNone : 0;
        if (pk.MetLocation != expect0)
        {
            if (pk.MetDate is null)
                data.AddLine(GetInvalid(Memory, DateCalendarInvalidMet));
        }
        else if (data.EncounterMatch is not EncounterInvalid)
        {
            if (pk.MetMonth != 0 || pk.MetDay != 0 || pk.MetYear != 0)
                data.AddLine(GetInvalid(Memory, DateCalendarInvalidMet));
        }
        if (pk.EggLocation != expect0)
        {
            if (pk.EggMetDate is null)
                data.AddLine(GetInvalid(Memory, DateCalendarInvalidEgg));
        }
        else
        {
            if (pk.EggMonth != 0 || pk.EggDay != 0 || pk.EggYear != 0)
                data.AddLine(GetInvalid(Memory, DateCalendarInvalidEgg));
        }
    }

    private void VerifyCorrelation8b(LegalityAnalysis data, IStaticCorrelation8b s8b, PKM pk)
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
            data.AddLine(GetInvalid(PIDTypeMismatch));
    }

    private void VerifyCorrelation8(LegalityAnalysis data, IOverworldCorrelation8 z, PKM pk)
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
            data.AddLine(GetInvalid(PIDTypeMismatch));
    }

    private void VerifyServerDate2000(LegalityAnalysis data, PKM pk, IEncounterable enc, IEncounterServerDate date)
    {
        const int epoch = 2000;
        var actualDay = new DateOnly(pk.MetYear + epoch, pk.MetMonth, pk.MetDay);

        // HOME Gifts for Sinnoh/Hisui starters were forced JPN until May 20, 2022 (UTC).
        if (enc is WB8 { IsDateLockJapanese: true } or WA8 { IsDateLockJapanese: true })
        {
            if (actualDay < new DateOnly(2022, 5, 20) && pk.Language != (int)LanguageID.Japanese)
                data.AddLine(GetInvalid(DateOutsideDistributionWindow));
        }

        var result = date.IsWithinDistributionWindow(actualDay);
        if (result == EncounterServerDateCheck.Invalid)
            data.AddLine(GetInvalid(DateOutsideDistributionWindow));
    }

    private void VerifyStats7(LegalityAnalysis data, PK7 pk7)
    {
        if (pk7.ResortEventStatus >= ResortEventState.MAX)
            data.AddLine(GetInvalid(TransferBad));
    }

    private void VerifyMiscScaleValues(LegalityAnalysis data, PKM pk, IEncounterTemplate enc)
    {
        if (pk is not IScaledSize s2)
            return;

        // Check for Height/Weight
        if (enc.Generation < 8 && pk.Format >= 9)
        {
            // Gen1-7 can have 0-0 if kept in PLA before HOME 3.0
            if (s2 is { HeightScalar: 0, WeightScalar: 0 } && !data.Info.EvoChainsAllGens.HasVisitedPLA && enc is not IPogoSlot)
                data.AddLine(Get(Encounter, Severity.Invalid, StatInvalidHeightWeight));
        }
        else if (data.EncounterMatch.Context is EntityContext.Gen9a)
        {
            if (s2.HeightScalar != 0)
                data.AddLine(GetInvalid(Encounter, StatIncorrectHeightValue));
            if (s2.WeightScalar != 0)
                data.AddLine(GetInvalid(Encounter, StatIncorrectWeightValue));
        }
        else if (CheckHeightWeightOdds(data.EncounterMatch))
        {
            if (s2 is { HeightScalar: 0, WeightScalar: 0 })
            {
                if (ParseSettings.Settings.HOMETransfer.ZeroHeightWeight != Severity.Valid)
                    data.AddLine(Get(Encounter, ParseSettings.Settings.HOMETransfer.ZeroHeightWeight, StatInvalidHeightWeight));
            }
        }

        // Check for Scale
        if (pk is IScaledSize3 s3 && IsHeightScaleMatchRequired(pk) && s2.HeightScalar != s3.Scale)
            data.AddLine(GetInvalid(StatIncorrectHeightValue));
    }

    private void VerifyIsMovesetAllowed(LegalityAnalysis data, SK2 sk2)
    {
        Span<ushort> moves = stackalloc ushort[4];
        sk2.GetMoves(moves);
        Span<bool> flags = stackalloc bool[4];

        if (sk2.Species is (ushort)Species.Smeargle)
        {
            if (LearnsetStadium.ValidateSmeargle(moves, flags))
                return;
        }
        else
        {
            var learn = LearnSource2Stadium.Instance.GetLearnsetStadium(sk2.Species, sk2.Form);
            if (learn.Validate(moves, sk2.CurrentLevel, flags))
                return;
        }

        var parse = data.Info.Moves;
        for (int i = 0; i < flags.Length; i++)
        {
            if (!flags[i])
                continue;
            ref var m = ref parse[i];
            if (!m.Valid)
                continue;
            m = m with { Info = m.Info with { Method = LearnMethod.Unobtainable, Environment = LearnEnvironment.Stadium2 } };
        }
    }

    private static void VerifyStats5(LegalityAnalysis data, PK5 pk5)
    {
        var enc = data.EncounterMatch;

        var fame = pk5.PokeStarFame;
        // Cannot participate in Pokestar Studios as Egg
        if (fame != 0)
        {
            if (pk5.IsEgg)
                data.AddLine(GetInvalid(Egg, EggShinyPokeStar));
            else if (enc.Species == (ushort)Species.Ditto) // Having Transform is not allowed; Smeargle can change moves.
                data.AddLine(GetInvalid(Misc, G5PokeStarMustBeZero));
            else if (fame % 25 is not (0 or 5)) // all values via +25, -50 for [0,255]
                data.AddLine(GetInvalid(Misc, G5PokeStarImpossibleValue));
        }

        // Ensure NSparkle is only present on N's encounters.
        if (enc is EncounterStatic5N)
        {
            if (!pk5.NSparkle)
                data.AddLine(GetInvalid(Fateful, G5SparkleRequired));
        }
        else
        {
            if (pk5.NSparkle)
                data.AddLine(GetInvalid(Fateful, G5SparkleInvalid));
        }
    }

    private static bool IsHeightScaleMatchRequired(PKM pk)
    {
        if (pk is IHomeTrack { HasTracker: false })
            return false;
        return true;
    }

    private void VerifyStats9(LegalityAnalysis data, PK9 pk9)
    {
        VerifyStatNature(data, pk9);
        VerifyTechRecordSV(data, pk9);

        if (!pk9.IsBattleVersionValid(data.Info.EvoChainsAllGens))
            data.AddLine(GetInvalid(StatBattleVersionInvalid));
        if (!IsObedienceLevelValid(pk9, pk9.ObedienceLevel, pk9.MetLevel))
            data.AddLine(GetInvalid(TransferObedienceLevel));
        if (pk9.IsEgg)
        {
            if (pk9.TeraTypeOverride != (MoveType)TeraTypeUtil.OverrideNone)
                data.AddLine(GetInvalid(TeraTypeIncorrect));
        }
        else if (pk9.Species == (int)Species.Terapagos)
        {
            if (!TeraTypeUtil.IsValidTerapagos((byte)pk9.TeraTypeOverride))
                data.AddLine(GetInvalid(TeraTypeIncorrect));
        }
        else if (pk9.Species == (int)Species.Ogerpon)
        {
            if (!TeraTypeUtil.IsValidOgerpon((byte)pk9.TeraTypeOverride, pk9.Form))
                data.AddLine(GetInvalid(TeraTypeIncorrect));
        }
        else
        {
            if (!TeraTypeUtil.IsValid((byte)pk9.TeraTypeOriginal))
                data.AddLine(GetInvalid(TeraTypeIncorrect));
        }

        var enc = data.EncounterOriginal;
        if (enc is EncounterEgg9 g)
        {
            if (!Tera9RNG.IsMatchTeraTypePersonalEgg(g.Species, g.Form, (byte)pk9.TeraTypeOriginal))
                data.AddLine(GetInvalid(TeraTypeMismatch));
        }
        else if (enc is ITeraRaid9)
        {
            var seed = Tera9RNG.GetOriginalSeed(pk9);
            data.Info.PIDIV = new PIDIV(PIDType.Tera9, seed);
        }
        else if (enc is not { Context: EntityContext.Gen9 } || pk9 is { GO_HOME: true })
        {
            if (pk9.TeraTypeOverride == (MoveType)TeraTypeUtil.OverrideNone)
                data.AddLine(GetInvalid(TeraTypeIncorrect));
            else if (GetTeraImportMatch(data.Info.EvoChainsAllGens.Gen9, pk9.TeraTypeOriginal, enc) == -1)
                data.AddLine(GetInvalid(TeraTypeIncorrect));
        }
        else if (enc is EncounterStatic9 { StarterBoxLegend: true })
        {
            // Ride legends cannot be traded or transferred.
            if (pk9.CurrentHandler != 0 || pk9.Tracker != 0 || !pk9.IsUntraded)
                data.AddLine(GetInvalid(TransferBad));
        }

        if (!Locations9.IsAccessiblePreDLC(pk9.MetLocation))
        {
            if (enc is { Species: (int)Species.Larvesta, Form: 0 } and not EncounterEgg9)
                DisallowLevelUpMove(24, (ushort)Move.BugBite, pk9, data);
            else if (enc is { Species: (int)Species.Zorua, Form: 1 } and not EncounterEgg9)
                DisallowLevelUpMove(28, (ushort)Move.Spite, pk9, data);
            else
                return;

            // Safari and Sport are not obtainable in the base game.
            // For the learnset restricted cases, we need to check if the ball is available too.
            if (((BallUseLegality.WildPokeballs9PreDLC2 >> pk9.Ball) & 1) != 1)
                data.AddLine(GetInvalid(BallUnavailable));
        }
    }

    private void VerifyStats9a(LegalityAnalysis data, PA9 pa9)
    {
        LegendsZA.Verify(data);
        if (!pa9.IsBattleVersionValid(data.Info.EvoChainsAllGens))
            data.AddLine(GetInvalid(StatBattleVersionInvalid));
        VerifyStatNature(data, pa9);
        if (!IsObedienceLevelValid(pa9, pa9.ObedienceLevel, pa9.MetLevel))
            data.AddLine(GetInvalid(TransferObedienceLevel));
        if (pa9.IsAlpha != data.EncounterMatch is IAlphaReadOnly { IsAlpha: true })
            data.AddLine(GetInvalid(StatAlphaInvalid));
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

    private static bool IsObedienceLevelValid(PKM pk, byte current, byte expectObey)
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
        if (pk.Format == 1) // not stored in Gen1 format
            return;

        var strain = pk.PokerusStrain;
        var days = pk.PokerusDays;
        bool strainValid = Pokerus.IsStrainValid(pk, data.Info.EncounterMatch, strain, days);
        if (!strainValid)
            data.AddLine(GetInvalid(PokerusStrainUnobtainable_0, (ushort)strain));

        bool daysValid = Pokerus.IsDurationValid(strain, days, out var max);
        if (!daysValid)
            data.AddLine(GetInvalid(PokerusDaysLEQ_0, (ushort)max));
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
                    data.AddLine(GetInvalid(Encounter, MetDetailTimeOfDay));
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
                data.AddLine(GetInvalid(G1TypePorygonFail1));
            }
            if (!PersonalTable1.TypeIDExists(pk1.Type2))
            {
                data.AddLine(GetInvalid(G1TypePorygonFail2));
            }
            else // Both types exist, ensure a Gen1 species has this combination
            {
                var matchSpecies = PersonalTable.RB.IsValidTypeCombination(pk1);
                var result = matchSpecies != -1 ? GetValid(G1TypeMatchPorygon) : GetInvalid(G1TypePorygonFail);
                data.AddLine(result);
            }
        }
        else // Types must match species types
        {
            var pi = PersonalTable.RB[species];
            var (match1, match2) = pi.IsMatchType(pk1);
            if (!match2 && ParseSettings.AllowGBStadium2)
                match2 = (species is (int)Species.Magnemite or (int)Species.Magneton) && pk1.Type2 == 9; // Steel Magnemite via Stadium2

            var first = match1 ? GetValid(G1TypeMatch1) : GetInvalid(G1Type1Fail);
            var second = match2 ? GetValid(G1TypeMatch2) : GetInvalid(G1Type2Fail);
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
        if (ItemConverter.IsCatchRateHeldItem(rate))
            return GetValid(G1CatchRateMatchTradeback);
        return GetWasNotTradeback(data, pk1, eval);
    }

    private CheckResult GetWasNotTradeback(LegalityAnalysis data, PK1 pk1, TimeCapsuleEvaluation eval)
    {
        var rate = pk1.CatchRate;
        if (MoveInfo.IsAnyFromGeneration(2, data.Info.Moves))
            return GetInvalid(G1CatchRateItem);
        var e = data.EncounterMatch;
        if (e is EncounterGift1 { Version: GameVersion.Stadium } or EncounterTrade1)
            return GetValid(G1CatchRateMatchPrevious); // Encounters detected by the catch rate, cant be invalid if match this encounters

        ushort species = pk1.Species;
        if (GBRestrictions.IsSpeciesNotAvailableCatchRate((byte)species) && rate == PersonalTable.RB[species].CatchRate)
        {
            if (species != (int)Species.Dragonite || rate != 45 || !(e.Version == GameVersion.BU || e.Version.Contains(GameVersion.YW)))
                return GetInvalid(G1CatchRateEvo);
        }
        if (!GBRestrictions.RateMatchesEncounter(e.Species, e.Version, rate))
            return GetInvalid(eval == TimeCapsuleEvaluation.Transferred12 ? G1CatchRateChain : G1CatchRateNone);
        return GetValid(G1CatchRateMatchPrevious);
    }

    private static void VerifyMiscFatefulEncounter(LegalityAnalysis data)
    {
        var pk = data.Entity;
        switch (data.EncounterMatch)
        {
            case EncounterGift3 {FatefulEncounter: true} w:
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
                VerifyGift3Shiny(data, w);
                return;
            case EncounterGift3 w:
                VerifyGift3Shiny(data, w);
                break;
            case MysteryGift g: // WC3 handled above
                VerifyReceivability(data, g);
                VerifyFatefulMysteryGift(data, g);
                return;
            case IFatefulEncounterReadOnly {FatefulEncounter: true}: // in-game fateful
                VerifyFatefulIngameActive(data);
                return;
        }
        if (pk.FatefulEncounter)
            data.AddLine(GetInvalid(Fateful, FatefulInvalid));
    }

    private static void VerifyMiscEggCommon(LegalityAnalysis data)
    {
        var pk = data.Entity;

        var enc = data.EncounterMatch;
        if (!EggStateLegality.GetIsEggHatchCyclesValid(pk, enc))
            data.AddLine(GetInvalid(Egg, EggHatchCycles));

        if (pk.Format >= 6 && enc is IEncounterEgg && !MovesMatchRelearn(pk))
            data.AddLine(GetInvalid(Egg, MovesShouldMatchRelearnMoves));

        if (pk is ITechRecord record)
        {
            if (record.GetMoveRecordFlagAny())
                data.AddLine(GetInvalid(Egg, EggRelearnFlags));
            if (pk.StatNature != pk.Nature)
                data.AddLine(GetInvalid(Egg, EggNature));
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
            if (Info.PIDIV.Type != PIDType.G5MGShiny)
            {
                var locToCheck = pk.IsEgg ? pk.MetLocation : pk.EggLocation;
                if (locToCheck is not (Locations.LinkTrade5 or Locations.LinkTrade5NPC))
                    data.AddLine(GetInvalid(PID, PIDTypeMismatch));
            }
        }

        bool shouldHave = g.FatefulEncounter;
        var result = pk.FatefulEncounter == shouldHave
            ? GetValid(Fateful, FatefulMystery)
            : GetInvalid(Fateful, shouldHave ? FatefulMysteryMissing : FatefulInvalid);
        data.AddLine(result);
    }

    private static void VerifyReceivability(LegalityAnalysis data, MysteryGift g)
    {
        var pk = data.Entity;
        switch (g)
        {
            case PCD pcd when !pcd.CanBeReceivedByVersion(pk.Version) && pcd.Gift.PK.Version == 0:
            case WC6 wc6 when !wc6.CanBeReceivedByVersion(pk.Version) && !pk.WasTradedEgg:
            case WC7 wc7 when !wc7.CanBeReceivedByVersion(pk.Version) && !pk.WasTradedEgg:
            case WC8 wc8 when !wc8.CanBeReceivedByVersion(pk.Version):
            case WB8 wb8 when !wb8.CanBeReceivedByVersion(pk.Version, pk):
            case WA8 wa8 when !wa8.CanBeReceivedByVersion(pk.Version, pk):
                data.AddLine(GetInvalid(GameOrigin, EncGiftVersionNotDistributed));
                return;
            case PGF pgf when pgf.RestrictLanguage != 0 && pk.Language != pgf.RestrictLanguage:
                data.AddLine(GetInvalid(CheckIdentifier.Language, EncGiftLanguageNotDistributed_0, (ushort)pgf.RestrictLanguage));
                return;
            case WC6 wc6 when wc6.RestrictLanguage != 0 && pk.Language != wc6.RestrictLanguage:
                data.AddLine(GetInvalid(CheckIdentifier.Language, EncGiftLanguageNotDistributed_0, (ushort)wc6.RestrictLanguage));
                return;
            case WC7 wc7 when wc7.RestrictLanguage != 0 && pk.Language != wc7.RestrictLanguage:
                data.AddLine(GetInvalid(CheckIdentifier.Language, EncGiftLanguageNotDistributed_0, (ushort)wc7.RestrictLanguage));
                return;
        }
    }

    private static void VerifyGift3Shiny(LegalityAnalysis data, EncounterGift3 g3)
    {
        // check for shiny locked gifts
        if (!g3.Shiny.IsValid(data.Entity))
            data.AddLine(GetInvalid(Fateful, EncGiftShinyMismatch));
    }

    private static void VerifyFatefulIngameActive(LegalityAnalysis data)
    {
        var pk = data.Entity;
        var result = pk.FatefulEncounter
            ? GetValid(Fateful, Valid)
            : GetInvalid(Fateful, FatefulMissing);
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
                    data.AddLine(GetInvalid(Evolution, EvoTradeRequired));
                break;
        }
    }

    private static void VerifyFullness(LegalityAnalysis data, PKM pk, IFullnessEnjoyment fe)
    {
        if (pk.IsEgg)
        {
            if (fe.Fullness != 0)
                data.AddLine(GetInvalid(Encounter, MemoryStatFullness_0, 0));
            if (fe.Enjoyment != 0)
                data.AddLine(GetInvalid(Encounter, MemoryStatEnjoyment_0, 0));
            return;
        }

        if (pk.Format >= 8)
        {
            if (fe.Fullness > 245) // Exiting camp is -10, so a 255=>245 is max.
                data.AddLine(GetInvalid(Encounter, MemoryStatFullnessLEQ_0, 245));
            else if (fe.Fullness is not 0 && pk is not PK8) // BD/SP and PLA do not set this field, even via HOME.
                data.AddLine(GetInvalid(Encounter, MemoryStatFullness_0, 0));

            if (fe.Enjoyment != 0)
                data.AddLine(GetInvalid(Encounter, MemoryStatEnjoyment_0, 0));
            return;
        }

        if (pk.Format != 6 || !pk.IsUntraded || pk.XY)
            return;

        // OR/AS PK6
        if (fe.Fullness == 0)
            return;
        if (pk.Species != data.EncounterMatch.Species)
            return; // evolved

        if (IsUnfeedable(pk.Species))
            data.AddLine(GetInvalid(Encounter, MemoryStatFullness_0, 0));
    }

    public static bool IsUnfeedable(ushort species) => species is
        (int)Species.Metapod or
        (int)Species.Kakuna or
        (int)Species.Pineco or
        (int)Species.Silcoon or
        (int)Species.Cascoon or
        (int)Species.Shedinja or
        (int)Species.Spewpa;

    private static void VerifyStats7b(LegalityAnalysis data, PB7 pb7)
    {
        VerifyAbsoluteSizes(data, pb7);
        var calc = pb7.CalcCP;
        if (pb7.Stat_CP != pb7.CalcCP && !pb7.IsStarter)
            data.AddLine(GetInvalid(Encounter, StatIncorrectCP_0, (uint)calc));

        if (pb7.ReceivedTime is null)
            data.AddLine(GetInvalid(Misc, DateLocalInvalidTime));

        // HOME moving in and out will retain received date. ensure it matches if no HT data present.
        // Go Park captures will have different dates, as the GO met date is retained as Met Date.
        if (pb7.ReceivedDate is not { } date || !EncounterDate.IsValidDateSwitch(date) || (pb7.IsUntraded && data.EncounterOriginal is not EncounterSlot7GO && date != pb7.MetDate))
            data.AddLine(GetInvalid(Misc, DateLocalInvalidDate));

        if (!data.IsStoredSlot(StorageSlotType.Party) && !pb7.IsStarter)
        {
            if (pb7.Spirit is not PB7.InitialSpiritMood)
                data.AddLine(GetInvalid(Misc, G7BSocialShouldBe100Spirit));
            if (pb7.Mood is not PB7.InitialSpiritMood)
                data.AddLine(GetInvalid(Misc, G7BSocialShouldBe100Mood));
        }
    }

    private static void VerifyAbsoluteSizes<T>(LegalityAnalysis data, T obj) where T : IScaledSizeValue
    {
        if (obj is PB7 pb7 && data.EncounterMatch is WB7 { IsHeightWeightFixed: true } enc)
            VerifyFixedSizes(data, pb7, enc);
        else if (obj is PA8 { Scale: 255 } pa8 && data.EncounterMatch is EncounterStatic8a { IsAlpha: true, HeightScalar: 127, WeightScalar: 127 })
            VerifyFixedSizeMidAlpha(data, pa8);
        else
            VerifyCalculatedSizes(data, obj);
    }

    // ReSharper disable CompareOfFloatsByEqualityOperator -- THESE MUST MATCH EXACTLY
    private static void VerifyFixedSizes<T>(LegalityAnalysis data, T obj, WB7 enc) where T : IScaledSizeValue
    {
        // Unlike PLA, there is no way to force it to recalculate in-game.
        // The only encounter this applies to is Meltan, which cannot reach PLA for recalculation.
        var expectHeight = enc.GetHomeHeightAbsolute();
        if (obj.HeightAbsolute != expectHeight)
            data.AddLine(GetInvalid(Encounter, StatIncorrectHeight, BitConverter.SingleToUInt32Bits(expectHeight)));

        var expectWeight = enc.GetHomeWeightAbsolute();
        if (obj.WeightAbsolute != expectWeight)
            data.AddLine(GetInvalid(Encounter, StatIncorrectWeight, BitConverter.SingleToUInt32Bits(expectWeight)));
    }

    private static void VerifyFixedSizeMidAlpha(LegalityAnalysis data, PA8 pk)
    {
        // HOME 3.0.1+ fixes the Height/Weight to 255, but doesn't update the float calculated sizes.
        // Putting it in party and putting it back in box did trigger them to update, so it can legally be two states:
        // Mutated (255 with 127-based-floats), or Updated (255 with 255-based-floats)
        // Since most players won't be triggering an update, it is more likely that it is only mutated.
        // Check for mutated first. If not matching mutated, must match updated.
        var pi = pk.PersonalInfo;
        var mutHeight = PA8.GetHeightAbsolute(pi, 127);
        if (pk.HeightAbsolute == mutHeight)
        {
            var mutWeight = PA8.GetWeightAbsolute(pi, 127, 127);
            if (pk.WeightAbsolute == mutWeight)
                return;
        }
        // Since it does not match the mutated state, it must be the updated state (255 + matching floats)
        VerifyCalculatedSizes(data, pk);
    }

    private static void VerifyCalculatedSizes<T>(LegalityAnalysis data, T obj) where T : IScaledSizeValue
    {
        var expectHeight = obj.CalcHeightAbsolute;
        if (obj.HeightAbsolute != expectHeight)
            data.AddLine(GetInvalid(Encounter, StatIncorrectHeight, BitConverter.SingleToUInt32Bits(expectHeight)));

        var expectWeight = obj.CalcWeightAbsolute;
        if (obj.WeightAbsolute != obj.CalcWeightAbsolute)
            data.AddLine(GetInvalid(Encounter, StatIncorrectWeight, BitConverter.SingleToUInt32Bits(expectWeight)));
    }
    // ReSharper restore CompareOfFloatsByEqualityOperator

    private void VerifyStats8(LegalityAnalysis data, PK8 pk8)
    {
        var social = pk8.Sociability;
        if (pk8.IsEgg)
        {
            if (social != 0)
                data.AddLine(GetInvalid(Encounter, MemorySocialZero));
        }
        else if (social > byte.MaxValue)
        {
            data.AddLine(GetInvalid(Encounter, MemoryStatSocialLEQ_0, 0));
        }

        VerifyStatNature(data, pk8);

        if (!pk8.IsBattleVersionValid(data.Info.EvoChainsAllGens))
            data.AddLine(GetInvalid(StatBattleVersionInvalid));

        var enc = data.EncounterMatch;
        bool originGMax = enc is IGigantamaxReadOnly {CanGigantamax: true};
        if (originGMax != pk8.CanGigantamax)
        {
            bool ok = !pk8.IsEgg && Gigantamax.CanToggle(pk8.Species, pk8.Form, enc.Species, enc.Form);
            var chk = ok ? GetValid(StatGigantamaxValid) : GetInvalid(StatGigantamaxInvalid);
            data.AddLine(chk);
        }

        if (pk8.DynamaxLevel != 0)
        {
            if (!pk8.CanHaveDynamaxLevel(pk8) || pk8.DynamaxLevel > 10)
                data.AddLine(GetInvalid(StatDynamaxInvalid));
        }

        VerifyTechRecordSWSH(data, pk8);
    }

    private void VerifyStats8a(LegalityAnalysis data, PA8 pa8)
    {
        Arceus.Verify(data);
        VerifyAbsoluteSizes(data, pa8);

        var social = pa8.Sociability;
        if (social != 0)
            data.AddLine(GetInvalid(Encounter, MemorySocialZero));

        VerifyStatNature(data, pa8);

        if (!pa8.IsBattleVersionValid(data.Info.EvoChainsAllGens))
            data.AddLine(GetInvalid(StatBattleVersionInvalid));

        if (pa8.CanGigantamax)
            data.AddLine(GetInvalid(StatGigantamaxInvalid));

        if (pa8.DynamaxLevel != 0)
            data.AddLine(GetInvalid(StatDynamaxInvalid));

        if (pa8.GetMoveRecordFlagAny() && !pa8.IsEgg) // already checked for eggs
            data.AddLine(GetInvalid(EggRelearnFlags));

        VerifyTechRecordSWSH(data, pa8);
    }

    private void VerifyStats8b(LegalityAnalysis data, PB8 pb8)
    {
        var social = pb8.Sociability;
        if (social != 0)
            data.AddLine(GetInvalid(Encounter, MemorySocialZero));

        if (pb8.IsDprIllegal)
            data.AddLine(GetInvalid(TransferFlagIllegal));
        if (pb8.Species is (int)Species.Spinda or (int)Species.Nincada && !pb8.BDSP)
            data.AddLine(GetInvalid(TransferNotPossible));
        if (pb8.Species is (int)Species.Spinda && pb8.Tracker != 0)
            data.AddLine(GetInvalid(TransferTrackerShouldBeZero));

        VerifyStatNature(data, pb8);

        if (!pb8.IsBattleVersionValid(data.Info.EvoChainsAllGens))
            data.AddLine(GetInvalid(StatBattleVersionInvalid));

        if (pb8.CanGigantamax)
            data.AddLine(GetInvalid(StatGigantamaxInvalid));

        if (pb8.DynamaxLevel != 0)
            data.AddLine(GetInvalid(StatDynamaxInvalid));

        if (pb8.GetMoveRecordFlagAny() && !pb8.IsEgg) // already checked for eggs
            data.AddLine(GetInvalid(EggRelearnFlags));

        VerifyTechRecordSWSH(data, pb8);
    }

    private static bool CheckHeightWeightOdds(IEncounterTemplate enc)
    {
        if (enc.Generation < 8)
            return false;
        if (enc.Context is EntityContext.Gen9a)
            return true;
        if (enc is WC8 { IsHOMEGift: true })
            return false;
        if (enc is WC9) // fixed values (usually 0 or 128)
            return false;
        return true;
    }

    private void VerifyStatNature<T>(LegalityAnalysis data, T pk) where T : PKM
    {
        // No encounters innately come with a different Stat Nature...
        // If it matches the Nature, it is valid. If it doesn't, it should be one of the mint natures.
        var statNature = pk.StatNature;
        if (statNature == pk.Nature)
            return;
        if (!statNature.IsMint())
            data.AddLine(GetInvalid(StatNatureInvalid));
    }

    private static ushort GetMoveIndex<T>(T pk, int index) where T : PKM, ITechRecord => pk.Permit.RecordPermitIndexes[index];

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
                data.AddLine(GetInvalid(MoveTechRecordFlagMissing_0, GetMoveIndex(pk, i)));
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

                data.AddLine(GetInvalid(MoveTechRecordFlagMissing_0, GetMoveIndex(pk, i)));
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
                data.AddLine(GetInvalid(MoveTechRecordFlagMissing_0, GetMoveIndex(pk, i)));
            }
        }
        else
        {
            static PersonalInfo9SV GetPersonal(EvoCriteria evo) => PersonalTable.SV.GetFormEntry(evo.Species, evo.Form);
            PersonalInfo9SV? pi = null;
            int count = pk.Permit.RecordCountUsed;
            for (int i = 0; i < count; i++)
            {
                var evo = evos[0];
                if (!pk.GetMoveRecordFlag(i))
                    continue;
                if ((pi ??= GetPersonal(evo)).GetIsLearnTM(i))
                    continue;

                // Deoxys has different TM permissions depending on form.
                // Zoroark-0 cannot learn Encore via TM, but the pre-evolution Zorua-0 can via TM.
                // Double check if any pre-evolutions can learn the TM.

                if (evo.Species is (int)Species.Deoxys)
                {
                    bool anyForm = false;
                    var fc = pi.FormCount;
                    for (int p = 1; p < fc; p++)
                    {
                        evo = evo with { Form = (byte)p };
                        if (!GetPersonal(evo).GetIsLearnTM(i))
                            continue;
                        anyForm = true;
                        break;
                    }
                    if (anyForm)
                        continue;
                }
                else
                {
                    bool preEvoHas = false;
                    for (int p = 1; p < evos.Length; p++)
                    {
                        evo = evos[p];
                        if (!GetPersonal(evo).GetIsLearnTM(i))
                            continue;
                        preEvoHas = true;
                        break;
                    }
                    if (preEvoHas)
                        continue;
                }
                data.AddLine(GetInvalid(MoveTechRecordFlagMissing_0, GetMoveIndex(pk, i)));
            }
        }
    }
}
