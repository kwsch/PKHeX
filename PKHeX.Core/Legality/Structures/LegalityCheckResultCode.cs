namespace PKHeX.Core;

/// <summary>
/// Represents the specific type of legality result that was generated during a legality check.
/// </summary>
/// <remarks>
/// When a result is generated, instead of storing the string directly, an instance of this enum is stored instead.
/// The string is only fetched from <see cref="LegalityCheckLocalization"/> when needed for display.
/// </remarks>
public enum LegalityCheckResultCode : ushort
{
    /// <summary>Originated from / Defer to an external validator.</summary>
    External,
    /// <summary>Default text for indicating validity.</summary>
    Valid,
    /// <summary>Default text for indicating an error has occurred.</summary>
    Error,

    // The order of the enum is important only for humanization; keep sorted by handling, with groups by category and functionality.

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
    AbilityMismatchPID,
    AbilityUnexpected,

    // Awakened Values
    AwakenedCap,

    // Ball
    BallAbility,
    BallEggCherish,
    BallEggMaster,
    BallEnc,
    BallEncMismatch,
    BallHeavy,
    BallSpecies,
    BallSpeciesPass,
    BallUnavailable,

    // Contest
    ContestZero,
    ContestZeroSheen,

    // Date & Timestamps
    DateCalendarInvalidMet,
    DateCalendarInvalidEgg,
    DateLocalInvalidDate,
    DateLocalInvalidTime,
    DateOutsideDistributionWindow,

    // Egg
    EggContest,
    EggEXP,
    EggHatchCycles,
    EggLocation,
    EggLocationInvalid,
    EggLocationNone,
    EggLocationPalPark,
    EggLocationTrade,
    EggLocationTradeFail,
    EggMetLocationFail,
    EggNature,
    EggPP,
    EggPPUp,
    EggRelearnFlags,
    EggShinyPokeStar,
    EggSpecies,
    EggUnhatched,

    // Encounter
    EncCondition,
    EncConditionBadRNGFrame,
    EncConditionBadSpecies,
    EncGift,
    EncGiftEggEvent,
    EncGiftIVMismatch,
    EncGiftNicknamed,
    EncGiftNotFound,
    EncGiftPIDMismatch,
    EncGiftShinyMismatch,
    EncGiftVersionNotDistributed,
    EncGiftRegionNotDistributed,
    EncInvalid,
    EncMasteryInitial,
    EncTradeChangedNickname,
    EncTradeChangedOT,
    EncTradeIndexBad,
    EncTradeMatch,
    EncTradeUnchanged,
    EncStaticPIDShiny,
    EncTypeMatch,
    EncTypeMismatch,
    EncUnreleased,
    EncUnreleasedEMewJP,

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

    // Evolution
    EvoInvalid,
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
    FormArgumentNotAllowed,
    FormArgumentValid,
    FormArgumentInvalid,
    FormBattle,
    FormEternal,
    FormEternalInvalid,
    FormInvalidGame,
    FormInvalidNature,
    FormItemMatches,
    FormItemInvalid,
    FormParty,
    FormPikachuCosplay,
    FormPikachuCosplayInvalid,
    FormPikachuEventInvalid,
    FormInvalidExpect_0,
    FormValid,
    FormVivillon,
    FormVivillonEventPre,
    FormVivillonInvalid,
    FormVivillonNonNative,

    // Generation 1 & 2
    G1CatchRateChain,
    G1CatchRateEvo,
    G1CatchRateItem,
    G1CatchRateMatchPrevious,
    G1CatchRateMatchTradeback,
    G1CatchRateNone,
    G1CharNick,
    G1CharOT,
    G1OTGender,
    G1Stadium,
    G1Type1Fail,
    G1Type2Fail,
    G1TypeMatch1,
    G1TypeMatch2,
    G1TypeMatchPorygon,
    G1TypePorygonFail,
    G1TypePorygonFail1,
    G1TypePorygonFail2,
    G2InvalidTileTreeNotFound,
    G2TreeID,
    G2OTGender,

    // Generation 3+
    G3EReader,
    G3OTGender,
    G4InvalidTileR45Surf,
    G4PartnerMoodEgg,
    G4PartnerMoodZero,
    G4ShinyLeafBitsEgg,
    G4ShinyLeafBitsInvalid,
    G5IVAll30,
    G5PIDShinyGrotto,
    G5SparkleInvalid,
    G5SparkleRequired,
    G7BSocialShouldBe100Spirit,
    G7BSocialShouldBe100Mood,

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

    // Hyper Training
    HyperPerfectAll,
    HyperPerfectOne,
    HyperPerfectUnavailable,

    // Item
    ItemEgg,
    ItemUnreleased,

    // IVs
    IVNotCorrect,

    // Level
    LevelEXPThreshold,
    LevelEXPTooHigh,
    LevelMetBelow,
    LevelMetGift,
    LevelMetGiftFail,
    LevelMetSane,

    // Markings
    MarkValueShouldBeZero,
    MarkValueUnusedBitsPresent,

    // Memory
    MemoryArgBadHT,
    MemoryHTFlagInvalid,
    MemoryHTLanguage,
    MemoryIndexArgHT,
    MemoryIndexFeelHTLEQ9,
    MemoryIndexIntensityHT1,
    MemoryIndexLinkHT,
    MemoryIndexVar_H1,
    MemoryMissingHT,
    MemoryMissingOT,
    MemorySocialZero,
    MemoryStatAffectionHT0,
    MemoryStatAffectionOT0,
    MemoryStatFriendshipHT0,

    // Met Detail
    MetDetailTimeOfDay,

    // Moves - General
    MoveKeldeoMismatch,
    MovesShouldMatchRelearnMoves,

    // Moves - Shop & Alpha
    MoveShopAlphaMoveShouldBeOther,
    MoveShopAlphaMoveShouldBeZero,

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

    // Ribbons
    RibbonAllValid,
    RibbonEgg,

    // Stats
    StatDynamaxInvalid,
    StatIncorrectHeight,
    StatIncorrectHeightCopy,
    StatIncorrectHeightValue,
    StatIncorrectWeight,
    StatIncorrectWeightValue,
    StatInvalidHeightWeight,
    StatGigantamaxInvalid,
    StatGigantamaxValid,
    StatNatureInvalid,
    StatBattleVersionInvalid,
    StatNobleInvalid,
    StatAlphaInvalid,

    // Storage
    StoredSourceEgg,

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
    TransferHandlerFlagRequired,
    TransferHandlerMismatchName,
    TransferHandlerMismatchGender,
    TransferHandlerMismatchLanguage,
    TransferMet,
    TransferNotPossible,
    TransferMetLocation,
    TransferNature,
    TransferObedienceLevel,
    TransferKoreanGen4,
    TransferEncryptGen6BitFlip,
    TransferEncryptGen6Equals,
    TransferEncryptGen6Xor,
    TransferTrackerMissing,
    TransferTrackerShouldBeZero,
    TrashBytesExpected,
    TrashBytesMismatchInitial,
    TrashBytesMissingTerminator,
    TrashBytesShouldBeEmpty,

    // Bulk Cross-Comparison
    BulkCloneDetectedDetails,
    BulkCloneDetectedTracker,
    BulkSharingEncryptionConstantGenerationSame,
    BulkSharingEncryptionConstantGenerationDifferent,
    BulkSharingEncryptionConstantEncounterType,
    BulkSharingPIDGenerationDifferent,
    BulkSharingPIDGenerationSame,
    BulkSharingPIDEncounterType,
    BulkDuplicateMysteryGiftEggReceived,
    BulkSharingTrainerIDs,
    BulkSharingTrainerVersion,

    // Formattable Argument Present: 1 Number
    FirstWithArgument,
    ContestSheenGEQ_0 = FirstWithArgument,
    MemoryStatFriendshipOTBaseEvent_0,
    ContestSheenLEQ_0,
    EggFMetLevel_0,
    EffortUntrainedCap_0,
    EvoTradeReqOutsider_0,
    FormArgumentLEQ_0,
    FormArgumentGEQ_0,
    HyperTrainLevelGEQ_0, // level
    IVAllEqual_0,
    IVFlawlessCountGEQ_0, // count
    MarkValueOutOfRange_0, // unknown value
    MemoryStatSocialLEQ_0,
    MemoryStatFullness_0,
    MemoryStatFullnessLEQ_0,
    MemoryStatEnjoyment_0,
    StatIncorrectCP_0, // value
    WordFilterTooManyNumbers_0, // count
    PokerusDaysLEQ_0, // days
    PokerusStrainUnobtainable_0, // strain
    MovePPExpectHealed_0, // move slot
    MovePPTooHigh_0, // move slot
    MovePPUpsTooHigh_0, // move slot
    MemoryHTGender_0, // gender value

    // Single Argument: Move ID
    FirstWithMove,
    MoveTechRecordFlagMissing_0 = FirstWithMove, // move ID
    MoveShopAlphaMoveShouldBeMastered_0, // move
    MoveShopMasterInvalid_0, // move ID
    MoveShopMasterNotLearned_0, // move ID
    MoveShopPurchaseInvalid_0, // move ID

    // One Argument: Language
    FirstWithLanguage,
    OTLanguageShouldBe_0 = FirstWithLanguage, // language
    OTLanguageShouldBeLeq_0, // language
    EncGiftLanguageNotDistributed_0, // language
    OTLanguageCannotPlayOnVersion_0, // language

    // Multiple Arguments: Memories
    FirstWithMemory,
    MemoryValid_H = FirstWithMemory,
    MemoryArgBadCatch_H,
    MemoryArgBadHatch_H,
    MemoryArgBadID_H,
    MemoryArgBadLocation_H,
    MemoryArgBadOTEgg_H,
    MemoryArgSpecies_H,
    MemoryCleared_H,
    MemoryFeelInvalid_H,
    FirstMemoryWithValue,
    MemoryArgBadSpecies_H1 = FirstMemoryWithValue,
    MemoryArgBadMove_H1,
    MemoryArgBadItem_H1,
    MemoryIndexID_H1,
    MemoryIndexFeel_H1,
    MemoryIndexIntensity_H1,
    MemoryIndexIntensityMin_H1,

    // One/Two Arguments: Special
    FirstComplex,
    RibbonsInvalid_0 = FirstComplex, // generated string
    WordFilterFlaggedPattern_01, // filter, pattern
    WordFilterInvalidCharacter_0, // filter, pattern

    AwakenedStatGEQ_01,// value, statName
    GanbaruStatLEQ_01, // value, statName
    OTLanguageCannotTransferToConsoleRegion_0, // ConsoleRegion
    EncTradeShouldHaveEvolvedToSpecies_0, // species
    MoveEvoFCombination_0, // species
    HintEvolvesToSpecies_0, // species

    RibbonMarkingInvalid_0, // ribbon
    RibbonMarkingMissing_0, // ribbon
    RibbonMarkingAffixed_0, // ribbon
    RibbonsMissing_0, // ribbon

    StoredSlotSourceInvalid_0, // StorageSlotType
    HintEvolvesToRareForm_0, // bool
    FormInvalidRangeLEQ_0F,

    OTLanguageShouldBe_0or1, // language,language

    MAX,
}
