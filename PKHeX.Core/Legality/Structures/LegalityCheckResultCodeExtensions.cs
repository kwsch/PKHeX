using System;
using static PKHeX.Core.LegalityCheckResultCode;
using static PKHeX.Core.LegalityCheckStrings;

namespace PKHeX.Core;

/// <summary>
/// Provides extension methods for <see cref="LegalityCheckResultCode"/> to convert to human-readable strings.
/// </summary>
public static class LegalityCheckResultCodeExtensions
{
    /// <summary>
    /// Converts a <see cref="LegalityCheckResultCode"/> to its corresponding localized string,
    /// applying formatting with the provided argument if needed.
    /// </summary>
    /// <param name="code">The enum value to humanize.</param>
    /// <param name="la">The <see cref="LegalityAnalysis"/> context for the legality check.</param>
    /// <param name="chk">Raw check value for formatting the string.</param>
    /// <returns>The localized string from <see cref="LegalityCheckStrings"/>, with formatting applied if needed</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the enum value doesn't have a corresponding string</exception>
    public static string Humanize(this LegalityCheckResultCode code, in LegalityLocalizationContext la, in CheckResult chk)
    {
        var template = GetTemplate(code);
        if (code < FirstWithArgument)
            return template;
        if (code.IsArgument())
            return string.Format(template, chk.Argument);
        if (code.IsMove())
            return string.Format(template, la.GetMoveName(chk.Argument));
        if (code.IsLanguage())
            return string.Format(template, la.GetLanguageName(chk.Argument), la.GetLanguageName(la.Analysis.Entity.Language));

        // Complex codes may require additional context or arguments.
        return code.GetComplex(la, chk, template);
    }

    private static bool IsArgument(this LegalityCheckResultCode code) => code is < FirstWithMove and >= FirstWithArgument;
    private static bool IsMove(this LegalityCheckResultCode code) => code is < FirstWithLanguage and >= FirstWithMove;
    private static bool IsLanguage(this LegalityCheckResultCode code) => code is < FirstComplex and >= FirstWithLanguage;

    /// <summary>
    /// Returns the template string for the given result code.
    /// </summary>
    private static string GetTemplate(LegalityCheckResultCode code) => code switch
    {
        // General Strings
        Valid => L_AValid,
        Error => L_AError,

        // Ability
        AbilityCapsuleUsed => LAbilityCapsuleUsed,
        AbilityPatchUsed => LAbilityPatchUsed,
        AbilityPatchRevertUsed => LAbilityPatchRevertUsed,
        AbilityFlag => LAbilityFlag,
        AbilityHiddenFail => LAbilityHiddenFail,
        AbilityHiddenUnavailable => LAbilityHiddenUnavailable,
        AbilityMismatch => LAbilityMismatch,
        AbilityMismatch3 => LAbilityMismatch3,
        AbilityMismatchFlag => LAbilityMismatchFlag,
        AbilityMismatchGift => LAbilityMismatchGift,
        AbilityMismatchPID => LAbilityMismatchPID,
        AbilityUnexpected => LAbilityUnexpected,

        // Awakened Values
        AwakenedCap => LAwakenedCap,
        AwakenedStatGEQ_01 => LAwakenedShouldBeValue,

        // Ball
        BallAbility => LBallAbility,
        BallEggCherish => LBallEggCherish,
        BallEggMaster => LBallEggMaster,
        BallEnc => LBallEnc,
        BallEncMismatch => LBallEncMismatch,
        BallHeavy => LBallHeavy,
        BallSpecies => LBallSpecies,
        BallSpeciesPass => LBallSpeciesPass,
        BallUnavailable => LBallUnavailable,

        // Contest
        ContestZero => LContestZero,
        ContestZeroSheen => LContestZeroSheen,
        ContestSheenGEQ_0 => LContestSheenTooLow_0,
        ContestSheenLEQ_0 => LContestSheenTooHigh_0,

        // Date & Timestamps
        DateOutsideConsoleWindow => LDateOutsideConsoleWindow,
        DateTimeClockInvalid => LDateTimeClockInvalid,
        DateOutsideDistributionWindow => LDateOutsideDistributionWindow,

        // Egg
        EggContest => LEggContest,
        EggEXP => LEggEXP,
        EggFMetLevel_0 => LEggFMetLevel_0,
        EggHatchCycles => LEggHatchCycles,
        EggLocation => LEggLocation,
        EggLocationInvalid => LEggLocationInvalid,
        EggLocationNone => LEggLocationNone,
        EggLocationPalPark => LEggLocationPalPark,
        EggLocationTrade => LEggLocationTrade,
        EggLocationTradeFail => LEggLocationTradeFail,
        EggMetLocationFail => LEggMetLocationFail,
        EggNature => LEggNature,
        EggPokeathlon => LEggPokeathlon,
        EggPP => LEggPP,
        EggPPUp => LEggPPUp,
        EggRelearnFlags => LEggRelearnFlags,
        EggShinyLeaf => LEggShinyLeaf,
        EggShinyPokeStar => LEggShinyPokeStar,
        EggSpecies => LEggSpecies,
        EggUnhatched => LEggUnhatched,

        // Encounter
        EncCondition => LEncCondition,
        EncConditionBadRNGFrame => LEncConditionBadRNGFrame,
        EncConditionBadSpecies => LEncConditionBadSpecies,
        EncGift => LEncGift,
        EncGiftEggEvent => LEncGiftEggEvent,
        EncGiftIVMismatch => LEncGiftIVMismatch,
        EncGiftNicknamed => LEncGiftNicknamed,
        EncGiftNotFound => LEncGiftNotFound,
        EncGiftPIDMismatch => LEncGiftPIDMismatch,
        EncGiftShinyMismatch => LEncGiftShinyMismatch,
        EncGiftVersionNotDistributed => LEncGiftVersionNotDistributed,
        EncInvalid => LEncInvalid,
        EncMasteryInitial => LEncMasteryInitial,
        EncTradeChangedNickname => LEncTradeChangedNickname,
        EncTradeChangedOT => LEncTradeChangedOT,
        EncTradeIndexBad => LEncTradeIndexBad,
        EncTradeMatch => LEncTradeMatch,
        EncTradeUnchanged => LEncTradeUnchanged,
        EncStaticPIDShiny => LEncStaticPIDShiny,
        EncTypeMatch => LEncTypeMatch,
        EncTypeMismatch => LEncTypeMismatch,
        EncUnreleased => LEncUnreleased,
        EncUnreleasedEMewJP => LEncUnreleasedEMewJP,

        // E-Reader
        EReaderAmerica => LEReaderAmerica,
        EReaderInvalid => LEReaderInvalid,
        EReaderJapan => LEReaderJapan,

        // Effort Values
        Effort2Remaining => LEffort2Remaining,
        EffortAbove252 => LEffortAbove252,
        EffortAbove510 => LEffortAbove510,
        EffortAllEqual => LEffortAllEqual,
        EffortCap100 => LEffortCap100,
        EffortEgg => LEffortEgg,
        EffortShouldBeZero => LEffortShouldBeZero,
        EffortEXPIncreased => LEffortEXPIncreased,
        EffortUntrainedCap_0 => LEffortUntrainedCap,

        // Evolution
        EvoInvalid => LEvoInvalid,
        EvoTradeReqOutsider_0 => LEvoTradeReqOutsider,
        EvoTradeRequired => LEvoTradeRequired,

        // Form
        FormArgumentLEQ_0 => LFormArgumentHigh,
        FormArgumentGEQ_0 => LFormArgumentLow,
        FormArgumentNotAllowed => LFormArgumentNotAllowed,
        FormArgumentValid => LFormArgumentValid,
        FormArgumentInvalid => LFormArgumentInvalid,
        FormBattle => LFormBattle,
        FormEternal => LFormEternal,
        FormEternalInvalid => LFormEternalInvalid,
        FormInvalidGame => LFormInvalidGame,
        FormInvalidNature => LFormInvalidNature,
        FormInvalidRange_0 => LFormInvalidRange,
        FormItemMatches => LFormItem,
        FormItemInvalid => LFormItemInvalid,
        FormParty => LFormParty,
        FormPikachuCosplay => LFormPikachuCosplay,
        FormPikachuCosplayInvalid => LFormPikachuCosplayInvalid,
        FormPikachuEventInvalid => LFormPikachuEventInvalid,
        FormInvalidExpect => LFormInvalidExpect_0,
        FormValid => LFormValid,
        FormVivillon => LFormVivillon,
        FormVivillonEventPre => LFormVivillonEventPre,
        FormVivillonInvalid => LFormVivillonInvalid,
        FormVivillonNonNative => LFormVivillonNonNative,

        // Hyper Training
        HyperTrainLevelGEQ_0 => LHyperTooLow_0,
        HyperPerfectAll => LHyperPerfectAll,
        HyperPerfectOne => LHyperPerfectOne,
        HyperPerfectUnavailable => LHyperPerfectUnavailable,

        // IVs
        IVAllEqual_0 => LIVAllEqual_0,
        IVNotCorrect => LIVNotCorrect,
        IVFlawlessCountGEQ_0 => LIVF_COUNT0_31,

        // Markings
        MarkValueOutOfRange_0 => LMarkValueOutOfRange_0,
        MarkValueShouldBeZero => LMarkValueShouldBeZero,
        MarkValueUnusedBitsPresent => LMarkValueUnusedBitsPresent,

        // Moves
        MoveEvoFCombination_0 => LMoveEvoFCombination_0,
        MovePPExpectHealed_0 => LMovePPExpectHealed_0,
        MovePPTooHigh_0 => LMovePPTooHigh_0,
        MovePPUpsTooHigh_0 => LMovePPUpsTooHigh_0,
        MoveShopMasterInvalid_0 => LMoveShopMasterInvalid_0,
        MoveShopMasterNotLearned_0 => LMoveShopMasterNotLearned_0,
        MoveShopPurchaseInvalid_0 => LMoveShopPurchaseInvalid_0,
        MoveTechRecordFlagMissing_0 => LMoveSourceTR,

        // Memory
        MemoryStatSocialLEQ_0 => LMemorySocialTooHigh_0,

        // Pokerus
        PokerusDaysLEQ_0 => LPokerusDaysTooHigh_0,
        PokerusStrainUnobtainable_0 => LPokerusStrainUnobtainable_0,

        // Ribbons
        RibbonFInvalid_0 => LRibbonFInvalid_0,
        RibbonMissing_0 => LRibbonFMissing_0,
        RibbonMarkingInvalid_0 => LRibbonMarkingFInvalid_0,
        RibbonMarkingAffixed_0 => LRibbonMarkingAffixedF_0,

        // Storage
        StoredSlotSourceInvalid_0 => LStoredSourceInvalid_0,

        EncGiftLanguageNotDistributed_0 => LEncGiftLanguageNotDistributed,
        EncGiftRegionNotDistributed => LEncGiftRegionNotDistributed,
        EncTradeShouldHaveEvolvedToSpecies_0 => LEncTradeShouldHaveEvolvedToSpecies_0,
        FatefulGiftMissing => LFatefulGiftMissing,
        FatefulInvalid => LFatefulInvalid,
        FatefulMissing => LFatefulMissing,
        FatefulMystery => LFatefulMystery,
        FatefulMysteryMissing => LFatefulMysteryMissing,
        FavoriteMarkingUnavailable => LFavoriteMarkingUnavailable,
        FormInvalidRangeLEQ_0 => LFormInvalidRangeLEQ_0,
        G1CatchRateChain => LG1CatchRateChain,
        G1CatchRateEvo => LG1CatchRateEvo,
        G1CatchRateItem => LG1CatchRateItem,
        G1CatchRateMatchPrevious => LG1CatchRateMatchPrevious,
        G1CatchRateMatchTradeback => LG1CatchRateMatchTradeback,
        G1CatchRateNone => LG1CatchRateNone,
        G1CharNick => LG1CharNick,
        G1CharOT => LG1CharOT,
        G1OTGender => LG1OTGender,
        G1Stadium => LG1Stadium,
        G1Type1Fail => LG1Type1Fail,
        G1Type2Fail => LG1Type2Fail,
        G1TypeMatch1 => LG1TypeMatch1,
        G1TypeMatch2 => LG1TypeMatch2,
        G1TypeMatchPorygon => LG1TypeMatchPorygon,
        G1TypePorygonFail => LG1TypePorygonFail,
        G1TypePorygonFail1 => LG1TypePorygonFail1,
        G1TypePorygonFail2 => LG1TypePorygonFail2,
        G2InvalidTileTreeNotFound => LG2InvalidTileTreeNotFound,
        G2TreeID => LG2TreeID,
        G2OTGender => LG2OTGender,
        G3EReader => LG3EReader,
        G3OTGender => LG3OTGender,
        G4InvalidTileR45Surf => LG4InvalidTileR45Surf,
        G5IVAll30 => LG5IVAll30,
        G5PIDShinyGrotto => LG5PIDShinyGrotto,
        G5SparkleInvalid => LG5SparkleInvalid,
        G5SparkleRequired => LG5SparkleRequired,
        GanbaruStatLEQ_01 => LGanbaruStatTooHigh,
        GenderInvalidNone => LGenderInvalidNone,
        GeoBadOrder => LGeoBadOrder,
        GeoHardwareInvalid => LGeoHardwareInvalid,
        GeoHardwareRange => LGeoHardwareRange,
        GeoHardwareValid => LGeoHardwareValid,
        GeoMemoryMissing => LGeoMemoryMissing,
        GeoNoCountryHT => LGeoNoCountryHT,
        GeoNoRegion => LGeoNoRegion,
        HintEvolvesToSpecies_0 => LHintEvolvesToSpecies_0,
        HintEvolvesToRareForm_0 => LHintEvolvesToRareForm_0,
        ItemEgg => LItemEgg,
        ItemUnreleased => LItemUnreleased,
        LevelEXPThreshold => LLevelEXPThreshold,
        LevelEXPTooHigh => LLevelEXPTooHigh,
        LevelMetBelow => LLevelMetBelow,
        LevelMetGift => LLevelMetGift,
        LevelMetGiftFail => LLevelMetGiftFail,
        LevelMetSane => LLevelMetSane,
        MemoryArgBadCatch => LMemoryArgBadCatch,
        MemoryArgBadHatch => LMemoryArgBadHatch,
        MemoryArgBadHT => LMemoryArgBadHT,
        MemoryArgBadID => LMemoryArgBadID,
        MemoryArgBadItem => LMemoryArgBadItem,
        MemoryArgBadLocation => LMemoryArgBadLocation,
        MemoryArgBadMove => LMemoryArgBadMove,
        MemoryArgBadOTEgg => LMemoryArgBadOTEgg,
        MemoryArgBadSpecies => LMemoryArgBadSpecies,
        MemoryArgSpecies => LMemoryArgSpecies,
        MemoryCleared => LMemoryCleared,
        MemoryF_0_Valid => LMemoryF_0_Valid,
        MemoryFeelInvalid_H => LMemoryFeelInvalid_H,
        MemoryHTFlagInvalid => LMemoryHTFlagInvalid,
        MemoryHTGender_0 => LMemoryHTGender,
        MemoryHTLanguage => LMemoryHTLanguage,
        MemoryIndexArgHT => LMemoryIndexArgHT,
        MemoryIndexFeel => LMemoryIndexFeel,
        MemoryIndexFeelHT09 => LMemoryIndexFeelHT09,
        MemoryIndexID => LMemoryIndexID,
        MemoryIndexIntensity => LMemoryIndexIntensity,
        MemoryIndexIntensityHT1 => LMemoryIndexIntensityHT1,
        MemoryIndexIntensityMin_01 => LMemoryIndexIntensityMin,
        MemoryIndexLinkHT => LMemoryIndexLinkHT,
        MemoryIndexVar => LMemoryIndexVar,
        MemoryMissingHT => LMemoryMissingHT,
        MemoryMissingOT => LMemoryMissingOT,
        MemorySocialZero => LMemorySocialZero,
        MemoryStatAffectionHT0 => LMemoryStatAffectionHT0,
        MemoryStatAffectionOT0 => LMemoryStatAffectionOT0,
        MemoryStatFriendshipHT0 => LMemoryStatFriendshipHT0,
        MemoryStatFriendshipOTBaseEvent => LMemoryStatFriendshipOTBaseEvent,
        MemoryStatFullness_0 => LMemoryStatFullness_0,
        MemoryStatFullnessLEQ_0 => LMemoryStatFullnessLEQ_0,
        MemoryStatEnjoyment_0 => LMemoryStatEnjoyment_0,
        MetDetailTimeOfDay => LMetDetailTimeOfDay,
        MoveKeldeoMismatch => LMoveKeldeoMismatch,
        MovesShouldMatchRelearnMoves => LMovesShouldMatchRelearnMoves,
        MoveShopAlphaMoveShouldBeMastered_0 => LMoveShopAlphaMoveShouldBeMastered,
        MoveShopAlphaMoveShouldBeOther => LMoveShopAlphaMoveShouldBeOther,
        MoveShopAlphaMoveShouldBeZero => LMoveShopAlphaMoveShouldBeZero,
        NickFlagEggNo => LNickFlagEggNo,
        NickFlagEggYes => LNickFlagEggYes,
        NickInvalidChar => LNickInvalidChar,
        NickLengthLong => LNickLengthLong,
        NickLengthShort => LNickLengthShort,
        NickMatchLanguage => LNickMatchLanguage,
        NickMatchLanguageEgg => LNickMatchLanguageEgg,
        NickMatchLanguageEggFail => LNickMatchLanguageEggFail,
        NickMatchLanguageFail => LNickMatchLanguageFail,
        NickMatchLanguageFlag => LNickMatchLanguageFlag,
        NickMatchNoOthers => LNickMatchNoOthers,
        NickMatchNoOthersFail => LNickMatchNoOthersFail,
        OTLanguage => LOTLanguage,
        OTLanguageShouldBe_0 => LOTLanguageShouldBe_0,
        OTLanguageShouldBe_0or1 => LOTLanguageShouldBe_0or1,
        OTLanguageShouldBeLeq_0 => LOTLanguageShouldBeLeq_0,
        OTLanguageCannotPlayOnVersion_0 => LOTLanguageCannotPlayOnVersion_0,
        OTLanguageCannotTransferToConsoleRegion_0 => LOTLanguageCannotTransferToConsoleRegion_0,
        OTLong => LOTLong,
        OTShort => LOTShort,
        OTSuspicious => LOTSuspicious,
        OT_IDEqual => LOT_IDEqual,
        OT_IDs0 => LOT_IDs0,
        OT_SID0 => LOT_SID0,
        OT_SID0Invalid => LOT_SID0Invalid,
        OT_TID0 => LOT_TID0,
        OT_IDInvalid => LOT_IDInvalid,
        PIDEncryptWurmple => LPIDEncryptWurmple,
        PIDEncryptZero => LPIDEncryptZero,
        PIDEqualsEC => LPIDEqualsEC,
        PIDGenderMatch => LPIDGenderMatch,
        PIDGenderMismatch => LPIDGenderMismatch,
        PIDNatureMatch => LPIDNatureMatch,
        PIDNatureMismatch => LPIDNatureMismatch,
        PIDTypeMismatch => LPIDTypeMismatch,
        PIDZero => LPIDZero,
        RibbonAllValid => LRibbonAllValid,
        RibbonEgg => LRibbonEgg,
        StatDynamaxInvalid => LStatDynamaxInvalid,
        StatIncorrectHeight => LStatIncorrectHeight,
        StatIncorrectHeightCopy => LStatIncorrectHeightCopy,
        StatIncorrectHeightValue => LStatIncorrectHeightValue,
        StatIncorrectWeight => LStatIncorrectWeight,
        StatIncorrectWeightValue => LStatIncorrectWeightValue,
        StatInvalidHeightWeight => LStatInvalidHeightWeight,
        StatIncorrectCP_0 => LStatIncorrectCP,
        StatGigantamaxInvalid => LStatGigantamaxInvalid,
        StatGigantamaxValid => LStatGigantamaxValid,
        StatNatureInvalid => LStatNatureInvalid,
        StatBattleVersionInvalid => LStatBattleVersionInvalid,
        StatNobleInvalid => LStatNobleInvalid,
        StatAlphaInvalid => LStatAlphaInvalid,
        StoredSourceEgg => LStoredSourceEgg,
        SuperComplete => LSuperComplete,
        SuperDistro => LSuperDistro,
        SuperEgg => LSuperEgg,
        SuperNoComplete => LSuperNoComplete,
        SuperNoUnlocked => LSuperNoUnlocked,
        SuperUnavailable => LSuperUnavailable,
        SuperUnused => LSuperUnused,
        TeraTypeIncorrect => LTeraTypeIncorrect,
        TeraTypeMismatch => LTeraTypeMismatch,
        TradeNotAvailable => LTradeNotAvailable,
        TrainerIDNoSeed => LTrainerIDNoSeed,
        TransferBad => LTransferBad,
        TransferCurrentHandlerInvalid => LTransferCurrentHandlerInvalid,
        TransferEgg => LTransferEgg,
        TransferEggLocationTransporter => LTransferEggLocationTransporter,
        TransferEggMetLevel => LTransferEggMetLevel,
        TransferEggVersion => LTransferEggVersion,
        TransferFlagIllegal => LTransferFlagIllegal,
        TransferHandlerFlagRequired => LTransferHTFlagRequired,
        TransferHandlerMismatchName => LTransferHTMismatchName,
        TransferHandlerMismatchGender => LTransferHTMismatchGender,
        TransferHandlerMismatchLanguage => LTransferHTMismatchLanguage,
        TransferMet => LTransferMet,
        TransferNotPossible => LTransferNotPossible,
        TransferMetLocation => LTransferMetLocation,
        TransferNature => LTransferNature,
        TransferObedienceLevel => LTransferObedienceLevel,
        TransferKoreanGen4 => LTransferKoreanGen4,
        TransferEncryptGen6BitFlip => LTransferPIDECBitFlip,
        TransferEncryptGen6Equals => LTransferPIDECEquals,
        TransferEncryptGen6Xor => LTransferPIDECXor,
        TransferTrackerMissing => LTransferTrackerMissing,
        TransferTrackerShouldBeZero => LTransferTrackerShouldBeZero,
        TrashBytesExpected => LTrashBytesExpected,
        TrashBytesMismatchInitial => LTrashBytesMismatchInitial,
        TrashBytesMissingTerminator => LTrashBytesMissingTerminator,
        TrashBytesShouldBeEmpty => LTrashBytesShouldBeEmpty,
        WordFilterInvalidCharacter_0 => LWordFilterInvalidCharacter_0,
        WordFilterFlaggedPattern_01 => LWordFilterFlaggedPattern_01,
        WordFilterTooManyNumbers_0 => LWordFilterTooManyNumbers_0,
        BulkCloneDetectedDetails => LBulkCloneDetectedDetails,
        BulkCloneDetectedTracker => LBulkCloneDetectedTracker,
        BulkSharingEncryptionConstantGenerationSame => LBulkSharingEncryptionConstantGenerationSame,
        BulkSharingEncryptionConstantGenerationDifferent => LBulkSharingEncryptionConstantGenerationDifferent,
        BulkSharingEncryptionConstantEncounterType => LBulkSharingEncryptionConstantRNGType,
        BulkSharingPIDGenerationDifferent => LBulkSharingPIDGenerationDifferent,
        BulkSharingPIDGenerationSame => LBulkSharingPIDGenerationSame,
        BulkSharingPIDEncounterType => LBulkSharingPIDRNGType,
        BulkDuplicateMysteryGiftEggReceived => LBulkDuplicateMysteryGiftEggReceived,
        BulkSharingTrainerIDs => LBulkSharingTrainerID,
        BulkSharingTrainerVersion => LBulkSharingTrainerVersion,

        >= MAX => throw new ArgumentOutOfRangeException(nameof(code), code, null),
    };

    private static string GetComplex(this LegalityCheckResultCode code, LegalityLocalizationContext ctx, CheckResult chk, string format) => code switch
    {
        < FirstComplex => format, // why are you even here?
        RibbonFInvalid_0 => string.Format(format, ctx.GetRibbonMessage()),
        WordFilterFlaggedPattern_01 => string.Format(format, (WordFilterType)chk.Argument, WordFilter.GetPattern((WordFilterType)chk.Argument, chk.Argument2)),
        WordFilterInvalidCharacter_0 => string.Format(format, chk.Argument.ToString("X4")),

        AwakenedStatGEQ_01 => string.Format(format, chk.Argument, ctx.GetStatName(chk.Argument2)),
        GanbaruStatLEQ_01 => string.Format(format, chk.Argument, ctx.GetStatName(chk.Argument2)),

        OTLanguageCannotTransferToConsoleRegion_0 => string.Format(format, ctx.GetConsoleRegion3DS(chk.Argument)),
        EncTradeShouldHaveEvolvedToSpecies_0 => string.Format(format, ctx.GetSpeciesName(chk.Argument)),
        MoveEvoFCombination_0 => string.Format(format, ctx.GetSpeciesName(chk.Argument)),
        HintEvolvesToSpecies_0 => string.Format(format, ctx.GetSpeciesName(chk.Argument)),

        RibbonMarkingInvalid_0 => string.Format(format, ctx.GetRibbonName(chk.Argument)),
        RibbonMarkingAffixed_0 => string.Format(format, ctx.GetRibbonName(chk.Argument)),
        RibbonMissing_0 => string.Format(format, ctx.GetRibbonName(chk.Argument)),

        StoredSlotSourceInvalid_0 => string.Format(format, (StorageSlotSource)chk.Argument),
        HintEvolvesToRareForm_0 => string.Format(format, chk.Argument == 1),

        OTLanguageShouldBe_0or1 => string.Format(format, ctx.GetLanguageName(chk.Argument), ctx.GetLanguageName(chk.Argument2), ctx.GetLanguageName(ctx.Analysis.Entity.Language)),

        >= MAX => throw new ArgumentOutOfRangeException(nameof(code), code, null),
    };
}
