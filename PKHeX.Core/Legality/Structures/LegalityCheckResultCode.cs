
namespace PKHeX.Core;

/// <summary>
/// Represents the specific type of legality result that was generated during a legality check.
/// </summary>
/// <remarks>
/// When a result is generated, instead of storing the string directly, an instance of this enum is stored instead.
/// The string is only fetched from <see cref="LegalityCheckStrings"/> when needed for display.
/// </remarks>
public enum LegalityCheckResultCode : ushort
{
    // General Strings
    /// <summary>Default text for indicating validity.</summary>
    Valid,
    /// <summary>Default text for indicating legality.</summary>
    Legal,
    /// <summary>Default text for indicating an error has occurred.</summary>
    Error,
    /// <summary>Analysis not available for the PKM</summary>
    AnalysisUnavailable,

    // Ability
    AbilityCapsuleUsed,
    AbilityPatchUsed,
    AbilityPatchRevertUsed,
    AbilityFlag,
    AbilityHiddenFail,
    AbilityHiddenUnavailable,
    AbilityMismatch,
    AbilityMismatch3,
    AbilityMismatchFlag,
    AbilityMismatchGift,
    AbilityMismatchGrotto,
    AbilityMismatchHordeSafari,
    AbilityMismatchPID,
    AbilityMismatchSOS,
    AbilityUnexpected,

    // Awakened Values
    AwakenedCap,
    Awakened0ShouldBeValue_0,
    Awakened1ShouldBeValue_0,
    Awakened2ShouldBeValue_0,
    Awakened3ShouldBeValue_0,
    Awakened4ShouldBeValue_0,
    Awakened5ShouldBeValue_0,

    // Ball
    BallAbility,
    BallEggCherish,
    BallEggMaster,
    BallEnc,
    BallEncMismatch,
    BallHeavy,
    BallNone,
    BallSpecies,
    BallSpeciesPass,
    BallUnavailable,

    // Contest
    ContestZero,
    ContestZeroSheen,
    ContestSheenTooLowGEQ_0,
    ContestSheenTooHighLEQ_0,

    // Date & Timestamps
    DateOutsideConsoleWindow,
    DateTimeClockInvalid,
    DateOutsideDistributionWindow,

    // Egg
    EggContest,
    EggEXP,
    EggFMetLevel_0,
    EggHatchCycles,
    EggLocation,
    EggLocationInvalid,
    EggLocationNone,
    EggLocationPalPark,
    EggLocationTrade,
    EggLocationTradeFail,
    EggMetLocationFail,
    EggNature,
    EggPokeathlon,
    EggPokerus,
    EggPP,
    EggPPUp,
    EggRelearnFlags,
    EggShinyLeaf,
    EggShinyPokeStar,
    EggSpecies,
    EggUnhatched,

    // Encounter
    EncCondition,
    EncConditionBadRNGFrame,
    EncConditionBadSpecies,
    EncConditionBlack,
    EncConditionBlackLead,
    EncConditionDexNav,
    EncConditionLead,
    EncConditionWhite,
    EncConditionWhiteLead,
    EncGift,
    EncGiftEggEvent,
    EncGiftIVMismatch,
    EncGiftNicknamed,
    EncGiftNotFound,
    EncGiftPIDMismatch,
    EncGiftShinyMismatch,
    EncGiftVersionNotDistributed,
    EncGiftLanguageNotDistributed,
    EncGiftRegionNotDistributed,
    EncInvalid,
    EncMasteryInitial,
    EncTradeChangedNickname,
    EncTradeChangedOT,
    EncTradeIndexBad,
    EncTradeMatch,
    EncTradeUnchanged,
    EncTradeShouldHaveEvolvedToSpecies_0,
    EncStaticMatch,
    EncStaticPIDShiny,
    EncStaticRelearn,
    EncTypeMatch,
    EncTypeMismatch,
    EncUnreleased,
    EncUnreleasedEMewJP,
    EncUnreleasedHoOArceus,
    EncUnreleasedPtDarkrai,
    EncUnreleasedPtShaymin,

    // E-Reader
    EReaderAmerica,
    EReaderInvalid,
    EReaderJapan,

    // Effort Values
    Effort2Remaining,
    EffortAbove252,
    EffortAbove510,
    EffortAllEqual,
    EffortCap100,
    EffortEgg,
    EffortShouldBeZero,
    EffortEXPIncreased,
    EffortUntrainedCap,

    // Evolution
    EvoInvalid,
    EvoTradeReq,
    EvoTradeReqOutsider_0,
    EvoTradeRequired,

    // Fateful
    FatefulGiftMissing,
    FatefulInvalid,
    FatefulMissing,
    FatefulMystery,
    FatefulMysteryMissing,

    // Favorite Marking
    FavoriteMarkingUnavailable,

    // Form
    FormArgumentHigh,
    FormArgumentLow,
    FormArgumentNotAllowed,
    FormArgumentValid,
    FormArgumentInvalid,
    FormBattle,
    FormEternal,
    FormEternalInvalid,
    FormInvalidGame,
    FormInvalidNature,
    FormInvalidRange,
    FormItem,
    FormItemInvalid,
    FormParty,
    FormPikachuCosplay,
    FormPikachuCosplayInvalid,
    FormPikachuEventInvalid,
    FormInvalidExpect,
    FormValid,
    FormVivillon,
    FormVivillonEventPre,
    FormVivillonInvalid,
    FormVivillonNonNative,
    FormInvalidRangeLEQ_0,

    // Generation 1 & 2
    G1CatchRateChain,
    G1CatchRateEvo,
    G1CatchRateItem,
    G1CatchRateMatchPrevious,
    G1CatchRateMatchTradeback,
    G1CatchRateNone,
    G1CharNick,
    G1CharOT,
    G1GBEncounter,
    G1MoveExclusive,
    G1MoveLearnSameLevel,
    G1MoveTradeback,
    G1OTEvent,
    G1OTGender,
    G1Stadium,
    G1StadiumInternational,
    G1StadiumJapanese,
    G1TradebackPreEvoMove,
    G1Type1Fail,
    G1Type2Fail,
    G1TypeMatch1,
    G1TypeMatch2,
    G1TypeMatchPorygon,
    G1TypePorygonFail,
    G1TypePorygonFail1,
    G1TypePorygonFail2,
    G2InvalidTilePark,
    G2InvalidTileR14,
    G2InvalidTileSafari,
    G2InvalidTileTreeID,
    G2InvalidTileTreeNotFound,
    G2TreeID,
    G2OTGender,

    // Generation 3+
    G3EReader,
    G3OTGender,
    G4InvalidTileR45Surf,
    G5ID_N,
    G5IVAll30,
    G5OTGenderN,
    G5PIDShinyGrotto,
    G5PIDShinyN,
    G5SparkleInvalid,
    G5SparkleRequired,

    // Ganbaru Values
    GanbaruStatTooHigh,

    // Gender
    GenderInvalidNone,

    // Geography
    GeoBadOrder,
    GeoHardwareInvalid,
    GeoHardwareRange,
    GeoHardwareValid,
    GeoMemoryMissing,
    GeoNoCountryHT,
    GeoNoRegion,

    // Hints
    HintEvolvesToSpecies_0,
    HintEvolvesToRareForm_0,

    // Hyper Training
    HyperTooLow_0,
    HyperPerfectAll,
    HyperPerfectOne,
    HyperPerfectUnavailable,

    // Item
    ItemEgg,
    ItemUnreleased,

    // IVs
    IVAllEqual_0,
    IVNotCorrect,
    IVF_COUNT0_31,

    // Level
    LevelEXPThreshold,
    LevelEXPTooHigh,
    LevelMetBelow,
    LevelMetGift,
    LevelMetGiftFail,
    LevelMetSane,

    // Markings
    MarkValueOutOfRange_0,
    MarkValueShouldBeZero,
    MarkValueUnusedBitsPresent,

    // Memory
    MemoryArgBadCatch,
    MemoryArgBadHatch,
    MemoryArgBadHT,
    MemoryArgBadID,
    MemoryArgBadItem,
    MemoryArgBadLocation,
    MemoryArgBadMove,
    MemoryArgBadOTEgg,
    MemoryArgBadSpecies,
    MemoryArgSpecies,
    MemoryCleared,
    MemoryF_0_Valid,
    MemoryFeelInvalid_H,
    MemoryHTFlagInvalid,
    MemoryHTGender_0,
    MemoryHTLanguage,
    MemoryIndexArgHT,
    MemoryIndexFeel,
    MemoryIndexFeelHT09,
    MemoryIndexID,
    MemoryIndexIntensity,
    MemoryIndexIntensityHT1,
    MemoryIndexIntensityMin_01,
    MemoryIndexLinkHT,
    MemoryIndexVar,
    MemoryMissingHT,
    MemoryMissingOT,
    MemorySocialZero,
    MemorySocialTooHighLEQ_0,
    MemoryStatAffectionHT0,
    MemoryStatAffectionOT0,
    MemoryStatFriendshipHT0,
    MemoryStatFriendshipOTBaseEvent,
    MemoryStatFullness_0,
    MemoryStatFullnessLEQ_0,
    MemoryStatEnjoyment_0,

    // Met Detail
    MetDetailTimeOfDay,

    // Moves - Eggs
    MoveEggFIncompatible0_1,
    MoveEggIncompatible,
    MoveEggIncompatibleEvent,
    MoveEggInherited,
    MoveEggInheritedTutor,
    MoveEggInvalid,
    MoveEggInvalidEvent,
    MoveEggInvalidEventLevelUp,
    MoveEggInvalidEventLevelUpGift,
    MoveEggInvalidEventTMHM,
    MoveEggInvalidEventTutor,
    MoveEggLevelUp,
    MoveEggMissing,
    MoveEggMoveGift,
    MoveEggTMHM,

    // Moves - General
    MoveEventEggLevelUp,
    MoveEvoFCombination_0,
    MoveEvoFHigher,
    MoveEvoFLower,
    MoveFDefault_0,
    MoveFExpect_0,
    MoveFExpectSingle_0,
    MoveFLevelUp_0,
    MoveFTMHM_0,
    MoveFTutor_0,
    MoveKeldeoMismatch,
    MoveNincada,
    MoveNincadaEvo,
    MoveNincadaEvoF_0,
    MovePPExpectHealed_0,
    MovePPTooHigh_0,
    MovePPUpsTooHigh_0,
    MoveSourceShared,
    MoveSourceSharedF,
    MovesShouldMatchRelearnMoves,

    // Moves - Relearn
    MoveRelearnDexNav,
    MoveRelearnUnderground,
    MoveRelearnEgg,
    MoveRelearnEggMissing,
    MoveRelearnFExpect_0,
    MoveRelearnFMiss_0,
    MoveRelearnInvalid,
    MoveRelearnNone,

    // Moves - Shop & Alpha
    MoveShopAlphaMoveShouldBeMastered,
    MoveShopAlphaMoveShouldBeOther,
    MoveShopAlphaMoveShouldBeZero,
    MoveShopMasterInvalid_0,
    MoveShopMasterNotLearned_0,
    MoveShopPurchaseInvalid_0,

    // Moves - Sources
    MoveSourceDefault,
    MoveSourceDuplicate,
    MoveSourceEgg,
    MoveSourceEggEvent,
    MoveSourceEmpty,
    MoveSourceInvalid,
    MoveSourceInvalidSketch,
    MoveSourceLevelUp,
    MoveSourceRelearn,
    MoveSourceSpecial,
    MoveSourceTMHM,
    MoveSourceTutor,
    MoveSourceTR,

    // Nickname
    NickFlagEggNo,
    NickFlagEggYes,
    NickInvalidChar,
    NickLengthLong,
    NickLengthShort,
    NickMatchLanguage,
    NickMatchLanguageEgg,
    NickMatchLanguageEggFail,
    NickMatchLanguageFail,
    NickMatchLanguageFlag,
    NickMatchNoOthers,
    NickMatchNoOthersFail,

    // Original Trainer
    OTLanguage,
    OTLanguageShouldBe_0,
    OTLanguageShouldBe_0or1,
    OTLanguageShouldBeLeq_0,
    OTLanguageCannotPlayOnVersion_0,
    OTLanguageCannotTransferFromConsoleRegion_0,
    OTLong,
    OTShort,
    OTSuspicious,
    OT_IDEqual,
    OT_IDs0,
    OT_SID0,
    OT_SID0Invalid,
    OT_TID0,
    OT_IDInvalid,

    // PID & Encryption Constant
    PIDEncryptWurmple,
    PIDEncryptZero,
    PIDEqualsEC,
    PIDGenderMatch,
    PIDGenderMismatch,
    PIDNatureMatch,
    PIDNatureMismatch,
    PIDTypeMismatch,
    PIDZero,

    // Pokerus
    PokerusDaysTooHigh,
    PokerusStrainUnobtainable_0,

    // Ribbons
    RibbonAllValid,
    RibbonEgg,
    RibbonFInvalid_0,
    RibbonMissing_0,
    RibbonMarkingInvalid_0,
    RibbonMarkingAffixed_0,

    // Stats
    StatDynamaxInvalid,
    StatIncorrectHeight,
    StatIncorrectHeightCopy,
    StatIncorrectHeightValue,
    StatIncorrectWeight,
    StatIncorrectWeightValue,
    StatInvalidHeightWeight,
    StatIncorrectCP,
    StatGigantamaxInvalid,
    StatGigantamaxValid,
    StatNatureInvalid,
    StatBattleVersionInvalid,
    StatNobleInvalid,
    StatAlphaInvalid,

    // Storage
    StoredSourceEgg,
    StoredSourceInvalid_0,

    // Super Training
    SuperComplete,
    SuperDistro,
    SuperEgg,
    SuperNoComplete,
    SuperNoUnlocked,
    SuperUnavailable,
    SuperUnused,

    // Tera Type
    TeraTypeIncorrect,
    TeraTypeMismatch,

    // Trading
    TradeNotAvailable,

    // Trainer IDs
    TrainerIDNoSeed,

    // Transfer
    TransferBad,
    TransferCurrentHandlerInvalid,
    TransferEgg,
    TransferEggLocationTransporter,
    TransferEggMetLevel,
    TransferEggVersion,
    TransferFlagIllegal,
    TransferHTFlagRequired,
    TransferHTMismatchName,
    TransferHTMismatchGender,
    TransferHTMismatchLanguage,
    TransferMet,
    TransferNotPossible,
    TransferMetLocation,
    TransferMove,
    TransferMoveG4HM,
    TransferMoveHM,
    TransferNature,
    TransferObedienceLevel,
    TransferOriginFInvalid0_1,
    TransferKoreanGen4,
    TransferPIDECBitFlip,
    TransferPIDECEquals,
    TransferPIDECXor,
    TransferTrackerMissing,
    TransferTrackerShouldBeZero,

    // Trash Bytes
    TrashBytesExpected_0,
    TrashBytesExpected,
    TrashBytesMismatchInitial,
    TrashBytesMissingTerminator,
    TrashBytesShouldBeEmpty,
    TrashBytesUnexpected,

    // Word Filter
    WordFilterInvalidCharacter_0,
    WordFilterFlaggedPattern_01,
    WordFilterTooManyNumbers_0,

    // Bulk Cross-Comparison
    BulkCloneDetectedDetails,
    BulkCloneDetectedTracker,
    BulkSharingEncryptionConstantGenerationSame,
    BulkSharingEncryptionConstantGenerationDifferent,
    BulkSharingEncryptionConstantRNGType,
    BulkSharingPIDGenerationDifferent,
    BulkSharingPIDGenerationSame,
    BulkSharingPIDRNGType,
    BulkDuplicateMysteryGiftEggReceived,
    BulkSharingTrainerID,
    BulkSharingTrainerVersion,

    MAX,
}
