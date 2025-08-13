// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

using System.Text.Json.Serialization;

namespace PKHeX.Core;

/// <summary>
/// Legality Check Message Strings to indicate why certain <see cref="PKM"/> <see cref="LegalInfo"/> values are flagged.
/// </summary>
public sealed class LegalityCheckLocalization
{
    private static readonly LegalityCheckLocalizationContext Context = new(LocalizationStorage<LegalityCheckLocalization>.Options);
    public static readonly LocalizationStorage<LegalityCheckLocalization> Cache = new("legality", Context.LegalityCheckLocalization);
    public static LegalityCheckLocalization Get(string language = GameLanguage.DefaultLanguage) => Cache.Get(language);
    public static LegalityCheckLocalization Get(LanguageID language) => Cache.Get(language.GetLanguageCode());

    // Message String Name format: L/F[Category][Summary]
    #region General Strings

    /// <summary>Default text for indicating validity.</summary>
    public string Valid { get; set; } = "Valid.";

    /// <summary>Default text for indicating legality.</summary>
    public string Legal { get; set; } = "Legal!";

    /// <summary>Default text for indicating an error has occurred.</summary>
    public string Error { get; set; } = "Internal error.";

    /// <summary>Analysis not available for the <see cref="PKM"/></summary>
    public string AnalysisUnavailable { get; set; } = "Analysis not available for this Pokémon.";

    /// <summary>Format text for exporting a legality check result.</summary>
    public string F0_1 { get; set; } = "{0}: {1}";

    /// <summary>Severity string for <see cref="Severity.Invalid"/></summary>
    public string SInvalid { get; set; } = "Invalid";

    /// <summary>Severity string for <see cref="Severity.Fishy"/></summary>
    public string SFishy { get; set; } = "Fishy";

    /// <summary>Severity string for <see cref="Severity.Valid"/></summary>
    public string SValid { get; set; } = "Valid";

    /// <summary>Severity string for anything not implemented.</summary>
    public string NotImplemented { get; set; } = "Not Implemented";

    public string AbilityCapsuleUsed { get; set; } = "Ability available with Ability Capsule.";
    public string AbilityPatchUsed { get; set; } = "Ability available with Ability Patch.";
    public string AbilityPatchRevertUsed { get; set; } = "Ability available with Ability Patch Revert.";
    public string AbilityFlag { get; set; } = "Ability matches ability number.";
    public string AbilityHiddenFail { get; set; } = "Hidden Ability mismatch for encounter type.";
    public string AbilityHiddenUnavailable { get; set; } = "Hidden Ability not available.";
    public string AbilityMismatch { get; set; } = "Ability mismatch for encounter.";
    public string AbilityMismatch3 { get; set; } = "Ability does not match Generation 3 species ability.";
    public string AbilityMismatchFlag { get; set; } = "Ability does not match ability number.";
    public string AbilityMismatchGift { get; set; } = "Ability does not match Mystery Gift.";
    public string AbilityMismatchPID { get; set; } = "Ability does not match PID.";
    public string AbilityUnexpected { get; set; } = "Ability is not valid for species/form.";

    public string AwakenedCap { get; set; } = "Individual AV cannot be greater than {0}.";
    public string AwakenedShouldBeValue { get; set; } = "{1} AV should be greater than {0}.";

    public string BallAbility { get; set; } = "Can't obtain Hidden Ability with Ball.";
    public string BallEggCherish { get; set; } = "Can't have Cherish Ball for regular Egg.";
    public string BallEggMaster { get; set; } = "Can't have Master Ball for regular Egg.";
    public string BallEnc { get; set; } = "Correct ball for encounter type.";
    public string BallEncMismatch { get; set; } = "Can't have ball for encounter type.";
    public string BallHeavy { get; set; } = "Can't have Heavy Ball for light, low-catch rate species (Gen VII).";
    public string BallSpecies { get; set; } = "Can't obtain species in Ball.";
    public string BallSpeciesPass { get; set; } = "Ball possible for species.";
    public string BallUnavailable { get; set; } = "Ball unobtainable in origin Generation.";

    public string ContestZero { get; set; } = "Contest Stats should be 0.";
    public string ContestZeroSheen { get; set; } = "Contest Stat Sheen should be 0.";
    public string ContestSheenGEQ_0 { get; set; } = "Contest Stat Sheen should be >= {0}.";
    public string ContestSheenLEQ_0 { get; set; } = "Contest Stat Sheen should be <= {0}.";

    public string DateCalendarInvalidMet { get; set; } = "Met Date is not a valid calendar date.";
    public string DateCalendarInvalidEgg { get; set; } = "Egg Met Date is not a valid calendar date.";
    public string DateLocalInvalidDate { get; set; } = "Local Date is outside of console's local time window.";
    public string DateLocalInvalidTime { get; set; } = "Local Time is not a valid timestamp.";
    public string DateOutsideDistributionWindow { get; set; } = "Met Date is outside of distribution window.";

    public string EggContest { get; set; } = "Cannot increase Contest Stats of an Egg.";
    public string EggEXP { get; set; } = "Eggs cannot receive experience.";
    public string EggFMetLevel_0 { get; set; } = "Invalid Met Level, expected {0}.";
    public string EggHatchCycles { get; set; } = "Invalid Egg hatch cycles.";
    public string EggLocation { get; set; } = "Able to hatch an Egg at Met Location.";
    public string EggLocationInvalid { get; set; } = "Can't hatch an Egg at Met Location.";
    public string EggLocationNone { get; set; } = "Invalid Egg Location, expected none.";
    public string EggLocationPalPark { get; set; } = "Invalid Met Location, expected Pal Park.";
    public string EggLocationTrade { get; set; } = "Able to hatch a traded Egg at Met Location.";
    public string EggLocationTradeFail { get; set; } = "Invalid Egg Location, shouldn't be 'traded' while an Egg.";
    public string EggMetLocationFail { get; set; } = "Can't obtain Egg from Egg Location.";
    public string EggNature { get; set; } = "Eggs cannot have their Stat Nature changed.";
    public string EggPP { get; set; } = "Eggs cannot have modified move PP counts.";
    public string EggPPUp { get; set; } = "Cannot apply PP Ups to an Egg.";
    public string EggRelearnFlags { get; set; } = "Expected no Relearn Move Flags.";
    public string EggShinyPokeStar { get; set; } = "Eggs cannot be a Pokéstar Studios star.";
    public string EggSpecies { get; set; } = "Can't obtain Egg for this species.";
    public string EggUnhatched { get; set; } = "Valid un-hatched Egg.";

    public string EncCondition { get; set; } = "Valid Wild Encounter at location.";
    public string EncConditionBadRNGFrame { get; set; } = "Unable to match encounter conditions to a possible RNG frame.";
    public string EncConditionBadSpecies { get; set; } = "Species does not exist in origin game.";

    public string EncGift { get; set; } = "Unable to match a gift Egg encounter from origin game.";
    public string EncGiftEggEvent { get; set; } = "Unable to match an event Egg encounter from origin game.";
    public string EncGiftIVMismatch { get; set; } = "IVs do not match Mystery Gift Data.";
    public string EncGiftNicknamed { get; set; } = "Event gift has been nicknamed.";
    public string EncGiftNotFound { get; set; } = "Unable to match to a Mystery Gift in the database.";
    public string EncGiftPIDMismatch { get; set; } = "Mystery Gift fixed PID mismatch.";
    public string EncGiftShinyMismatch { get; set; } = "Mystery Gift shiny mismatch.";
    public string EncGiftVersionNotDistributed { get; set; } = "Mystery Gift cannot be received by this version.";

    public string EncInvalid { get; set; } = "Unable to match an encounter from origin game.";
    public string EncMasteryInitial { get; set; } = "Initial move mastery flags do not match the encounter's expected state.";

    public string EncTradeChangedNickname { get; set; } = "In-game Trade Nickname has been altered.";
    public string EncTradeChangedOT { get; set; } = "In-game Trade OT has been altered.";
    public string EncTradeIndexBad { get; set; } = "In-game Trade invalid index?";
    public string EncTradeMatch { get; set; } = "Valid In-game trade.";
    public string EncTradeUnchanged { get; set; } = "In-game Trade OT and Nickname have not been altered.";

    public string EncStaticPIDShiny { get; set; } = "Encounter shiny mismatch.";
    public string EncTypeMatch { get; set; } = "Encounter Type matches encounter.";
    public string EncTypeMismatch { get; set; } = "Encounter Type does not match encounter.";
    public string EncUnreleased { get; set; } = "Unreleased event.";
    public string EncUnreleasedEMewJP { get; set; } = "Non japanese Mew from Faraway Island. Unreleased event.";

    public string EReaderAmerica { get; set; } = "American E-Reader Berry in Japanese save file.";
    public string EReaderInvalid { get; set; } = "Invalid E-Reader Berry.";
    public string EReaderJapan { get; set; } = "Japanese E-Reader Berry in international save file.";

    public string Effort2Remaining { get; set; } = "2 EVs remaining.";
    public string EffortAbove252 { get; set; } = "EVs cannot go above 252.";
    public string EffortAbove510 { get; set; } = "EV total cannot be above 510.";
    public string EffortAllEqual { get; set; } = "EVs are all equal.";
    public string EffortCap100 { get; set; } = "Individual EV for a level 100 encounter in Generation 4 cannot be greater than 100.";
    public string EffortEgg { get; set; } = "Eggs cannot receive EVs.";
    public string EffortShouldBeZero { get; set; } = "Cannot receive EVs.";
    public string EffortEXPIncreased { get; set; } = "All EVs are zero, but leveled above Met Level.";
    public string EffortUntrainedCap { get; set; } = "Individual EV without changing EXP cannot be greater than {0}.";

    public string EvoInvalid { get; set; } = "Evolution not valid (or level/trade evolution unsatisfied).";
    public string EvoTradeReqOutsider { get; set; } = "Outsider {0} should have evolved into {1}.";
    public string EvoTradeRequired { get; set; } = "Version Specific evolution requires a trade to opposite version. A Handling Trainer is required.";

    public string FatefulGiftMissing { get; set; } = "Fateful Encounter with no matching Encounter. Has the Mystery Gift data been contributed?";
    public string FatefulInvalid { get; set; } = "Fateful Encounter should not be checked.";
    public string FatefulMissing { get; set; } = "Special In-game Fateful Encounter flag missing.";
    public string FatefulMystery { get; set; } = "Mystery Gift Fateful Encounter.";
    public string FatefulMysteryMissing { get; set; } = "Mystery Gift Fateful Encounter flag missing.";

    public string FavoriteMarkingUnavailable { get; set; } = "Favorite Marking is not available.";

    public string FormArgumentLEQ_0 { get; set; } = "Form argument is too high for current form.";
    public string FormArgumentGEQ_0 { get; set; } = "Form argument is too low for current form.";
    public string FormArgumentNotAllowed { get; set; } = "Form argument is not allowed for this encounter.";
    public string FormArgumentValid { get; set; } = "Form argument is valid.";
    public string FormArgumentInvalid { get; set; } = "Form argument is not valid.";
    public string FormBattle { get; set; } = "Form cannot exist outside of a battle.";
    public string FormEternal { get; set; } = "Valid Eternal Flower encounter.";
    public string FormEternalInvalid { get; set; } = "Invalid Eternal Flower encounter.";
    public string FormInvalidGame { get; set; } = "Form cannot be obtained in origin game.";
    public string FormInvalidNature { get; set; } = "Form cannot have this nature.";
    public string FormItem { get; set; } = "Held item matches Form.";
    public string FormItemInvalid { get; set; } = "Held item does not match Form.";
    public string FormParty { get; set; } = "Form cannot exist outside of Party.";
    public string FormPikachuCosplay { get; set; } = "Only Cosplay Pikachu can have this form.";
    public string FormPikachuCosplayInvalid { get; set; } = "Cosplay Pikachu cannot have the default form.";
    public string FormPikachuEventInvalid { get; set; } = "Event Pikachu cannot have the default form.";
    public string FormInvalidExpect_0 { get; set; } = "Form is invalid, expected form index {0}.";
    public string FormValid { get; set; } = "Form is Valid.";
    public string FormVivillon { get; set; } = "Valid Vivillon pattern.";
    public string FormVivillonEventPre { get; set; } = "Event Vivillon pattern on pre-evolution.";
    public string FormVivillonInvalid { get; set; } = "Invalid Vivillon pattern.";
    public string FormVivillonNonNative { get; set; } = "Non-native Vivillon pattern.";

    public string G1CatchRateChain { get; set; } = "Catch rate does not match any species from Pokémon evolution chain.";
    public string G1CatchRateEvo { get; set; } = "Catch rate match species without encounters. Expected a preevolution catch rate.";
    public string G1CatchRateItem { get; set; } = "Catch rate does not match a valid held item from Generation 2.";
    public string G1CatchRateMatchPrevious { get; set; } = "Catch Rate matches a species from Pokémon evolution chain.";
    public string G1CatchRateMatchTradeback { get; set; } = "Catch rate matches a valid held item from Generation 2.";
    public string G1CatchRateNone { get; set; } = "Catch rate does not match any species from Pokémon evolution chain or any Generation 2 held items.";
    public string G1CharNick { get; set; } = "Nickname from Generation 1/2 uses unavailable characters.";
    public string G1CharOT { get; set; } = "OT from Generation 1/2 uses unavailable characters.";
    public string G1OTGender { get; set; } = "Female OT from Generation 1/2 is invalid.";
    public string G1Stadium { get; set; } = "Incorrect Stadium OT.";
    public string G1Type1Fail { get; set; } = "Invalid Type A, does not match species type.";
    public string G1Type2Fail { get; set; } = "Invalid Type B, does not match species type.";
    public string G1TypeMatch1 { get; set; } = "Valid Type A, matches species type.";
    public string G1TypeMatch2 { get; set; } = "Valid Type B, matches species type.";
    public string G1TypeMatchPorygon { get; set; } = "Porygon with valid Type A and B values.";
    public string G1TypePorygonFail { get; set; } = "Porygon with invalid Type A and B values. Does not a match a valid type combination.";
    public string G1TypePorygonFail1 { get; set; } = "Porygon with invalid Type A value.";
    public string G1TypePorygonFail2 { get; set; } = "Porygon with invalid Type B value.";
    public string G2InvalidTileTreeNotFound { get; set; } = "Could not find a tree for Crystal headbutt encounter that matches OTID.";
    public string G2TreeID { get; set; } = "Found a tree for Crystal headbutt encounter that matches OTID.";
    public string G2OTGender { get; set; } = "OT from Virtual Console games other than Crystal cannot be female.";

    public string G3EReader { get; set; } = "Non Japanese Shadow E-reader Pokémon. Unreleased encounter.";
    public string G3OTGender { get; set; } = "OT from Colosseum/XD cannot be female.";
    public string G4InvalidTileR45Surf { get; set; } = "Johto Route 45 surfing encounter. Unreachable Water tiles.";
    public string G4PartnerMoodEgg { get; set; } = "Eggs cannot have an Mood stat value.";
    public string G4PartnerMoodZero { get; set; } = "Mood stat value should be zero when not in the player's party.";
    public string G4ShinyLeafBitsInvalid { get; set; } = "Shiny Leaf/Crown bits are not valid.";
    public string G4ShinyLeafBitsEgg { get; set; } = "Eggs cannot have Shiny Leaf/Crown.";
    public string G5IVAll30 { get; set; } = "All IVs of N's Pokémon should be 30.";
    public string G5PIDShinyGrotto { get; set; } = "Hidden Grotto captures cannot be shiny.";
    public string G5SparkleInvalid { get; set; } = "Special In-game N's Sparkle flag should not be checked.";
    public string G5SparkleRequired { get; set; } = "Special In-game N's Sparkle flag missing.";
    public string G7BSocialShouldBe100Spirit { get; set; } = "Spirit should be 100 for Pokémon not in the player's party.";
    public string G7BSocialShouldBe100Mood { get; set; } = "Mood should be 100 for Pokémon not in the player's party.";

    public string GanbaruStatTooHigh { get; set; } = "One or more Ganbaru Value is above the natural limit of (10 - IV bonus).";

    public string GenderInvalidNone { get; set; } = "Genderless Pokémon should not have a gender.";
    public string GeoBadOrder { get; set; } = "GeoLocation Memory: Gap/Blank present.";
    public string GeoHardwareInvalid { get; set; } = "Geolocation: Country is not in 3DS region.";
    public string GeoHardwareRange { get; set; } = "Invalid Console Region.";
    public string GeoHardwareValid { get; set; } = "Geolocation: Country is in 3DS region.";
    public string GeoMemoryMissing { get; set; } = "GeoLocation Memory: Memories should be present.";
    public string GeoNoCountryHT { get; set; } = "GeoLocation Memory: HT Name present but has no previous Country.";
    public string GeoNoRegion { get; set; } = "GeoLocation Memory: Region without Country.";

    public string HyperTrainLevelGEQ_0 { get; set; } = "Can't Hyper Train a Pokémon that isn't level {0}.";
    public string HyperPerfectAll { get; set; } = "Can't Hyper Train a Pokémon with perfect IVs.";
    public string HyperPerfectOne { get; set; } = "Can't Hyper Train a perfect IV.";
    public string HyperPerfectUnavailable { get; set; } = "Can't Hyper Train any IV(s).";

    public string ItemEgg { get; set; } = "Eggs cannot hold items.";
    public string ItemUnreleased { get; set; } = "Held item is unreleased.";

    public string IVAllEqual_0 { get; set; } = "All IVs are {0}.";
    public string IVNotCorrect { get; set; } = "IVs do not match encounter requirements.";
    public string IVFlawlessCountGEQ_0 { get; set; } = "Should have at least {0} IVs = 31.";

    public string LevelEXPThreshold { get; set; } = "Current experience matches level threshold.";
    public string LevelEXPTooHigh { get; set; } = "Current experience exceeds maximum amount for level 100.";
    public string LevelMetBelow { get; set; } = "Current level is below met level.";
    public string LevelMetGift { get; set; } = "Met Level does not match Mystery Gift level.";
    public string LevelMetGiftFail { get; set; } = "Current Level below Mystery Gift level.";
    public string LevelMetSane { get; set; } = "Current level is not below met level.";

    public string MarkValueOutOfRange_0 { get; set; } = "Individual marking at index {0} is not within the allowed value range.";
    public string MarkValueShouldBeZero { get; set; } = "Marking flags cannot be set.";
    public string MarkValueUnusedBitsPresent { get; set; } = "Marking flags uses bits beyond the accessible range.";

    public string MemoryArgBadCatch_H { get; set; } = "{0} Memory: {0} did not catch this.";
    public string MemoryArgBadHatch_H { get; set; } = "{0} Memory: {0} did not hatch this.";
    public string MemoryArgBadHT { get; set; } = "Memory: Can't have Handling Trainer Memory as Egg.";
    public string MemoryArgBadID_H { get; set; } = "{0} Memory: Can't obtain Memory on {0} Version.";
    public string MemoryArgBadItem_H1 { get; set; } = "{0} Memory: Species can't hold this item.";
    public string MemoryArgBadLocation_H { get; set; } = "{0} Memory: Can't obtain Location on {0} Version.";
    public string MemoryArgBadMove_H1 { get; set; } = "{0} Memory: Species can't learn {1}.";
    public string MemoryArgBadOTEgg_H { get; set; } = "{0} Memory: Link Trade is not a valid first memory.";
    public string MemoryArgBadSpecies_H1 { get; set; } = "{0} Memory: Can't capture species in game.";
    public string MemoryArgSpecies_H { get; set; } = "{0} Memory: Species can be captured in game.";
    public string MemoryCleared_H { get; set; } = "Memory: Not cleared properly.";
    public string MemoryValid_H { get; set; } = "{0} Memory is valid.";
    public string MemoryFeelInvalid_H { get; set; } = "{0} Memory: Invalid Feeling.";
    public string MemoryHTFlagInvalid { get; set; } = "Untraded: Current handler should not be the Handling Trainer.";
    public string MemoryHTGender_0 { get; set; } = "HT Gender invalid: {0}";
    public string MemoryHTLanguage { get; set; } = "HT Language is missing.";

    public string MemoryIndexArgHT { get; set; } = "Should have a HT Memory TextVar value (somewhere).";
    public string MemoryIndexFeel_H1 { get; set; } = "{0} Memory: Feeling should be index {1}.";
    public string MemoryIndexFeelHTLEQ9 { get; set; } = "Should have a HT Memory Feeling value 0-9.";
    public string MemoryIndexID_H1 { get; set; } = "{0} Memory: Should be index {1}.";
    public string MemoryIndexIntensity_H1 { get; set; } = "{0} Memory: Intensity should be index {1}.";
    public string MemoryIndexIntensityHT1 { get; set; } = "Should have a HT Memory Intensity value (1st).";
    public string MemoryIndexIntensityMin_H1 { get; set; } = "{0} Memory: Intensity should be at least {1}.";
    public string MemoryIndexLinkHT { get; set; } = "Should have a Link Trade HT Memory.";
    public string MemoryIndexVar { get; set; } = "{0} Memory: TextVar should be index {1}.";
    public string MemoryMissingHT { get; set; } = "Memory: Handling Trainer Memory missing.";
    public string MemoryMissingOT { get; set; } = "Memory: Original Trainer Memory missing.";

    public string MemorySocialZero { get; set; } = "Social Stat should be zero.";
    public string MemoryStatSocialLEQ_0 { get; set; } = "Social Stat should be <= {0}";

    public string MemoryStatAffectionHT0 { get; set; } = "Untraded: Handling Trainer Affection should be 0.";
    public string MemoryStatAffectionOT0 { get; set; } = "OT Affection should be 0.";
    public string MemoryStatFriendshipHT0 { get; set; } = "Untraded: Handling Trainer Friendship should be 0.";
    public string MemoryStatFriendshipOTBaseEvent_0 { get; set; } = "Event OT Friendship does not match base friendship ({0}).";

    public string MetDetailTimeOfDay { get; set; } = "Met Time of Day value is not within the expected range.";
    public string MoveEvoFCombination_0 { get; set; } = "Moves combinations is not compatible with {0} evolution.";
    public string MoveFExpectSingle_0 { get; set; } = "Expected: {0}";
    public string MoveKeldeoMismatch { get; set; } = "Keldeo Move/Form mismatch.";
    public string MovePPExpectHealed_0 { get; set; } = "Move {0} PP is below the amount expected.";
    public string MovePPTooHigh_0 { get; set; } = "Move {0} PP is above the amount allowed.";
    public string MovePPUpsTooHigh_0 { get; set; } = "Move {0} PP Ups is above the amount allowed.";

    public string MoveShopAlphaMoveShouldBeMastered_0 { get; set; } = "Alpha Move should be marked as mastered.";
    public string MoveShopAlphaMoveShouldBeOther { get; set; } = "Alpha encounter cannot be found with this Alpha Move.";
    public string MoveShopAlphaMoveShouldBeZero { get; set; } = "Only Alphas may have an Alpha Move set.";
    public string MoveShopMasterInvalid_0 { get; set; } = "Cannot manually master {0}: not permitted to master.";
    public string MoveShopMasterNotLearned_0 { get; set; } = "Cannot manually master {0}: not in possible learned level up moves.";
    public string MoveShopPurchaseInvalid_0 { get; set; } = "Cannot purchase {0} from the move shop.";

    public string MoveTechRecordFlagMissing_0 { get; set; } = "Unexpected Technical Record Learned flag: {0}";

    public string NickFlagEggNo { get; set; } = "Egg must be not nicknamed.";
    public string NickFlagEggYes { get; set; } = "Egg must be nicknamed.";
    public string NickInvalidChar { get; set; } = "Cannot be given this Nickname.";
    public string NickLengthLong { get; set; } = "Nickname too long.";
    public string NickLengthShort { get; set; } = "Nickname is empty.";
    public string NickMatchLanguage { get; set; } = "Nickname matches species name.";
    public string NickMatchLanguageEgg { get; set; } = "Egg matches language Egg name.";
    public string NickMatchLanguageEggFail { get; set; } = "Egg name does not match language Egg name.";
    public string NickMatchLanguageFail { get; set; } = "Nickname does not match species name.";
    public string NickMatchLanguageFlag { get; set; } = "Nickname flagged, matches species name.";
    public string NickMatchNoOthers { get; set; } = "Nickname does not match another species name.";
    public string NickMatchNoOthersFail { get; set; } = "Nickname matches another species name (+language).";

    public string OTLanguage { get; set; } = "Language ID should be {0}, not {1}.";
    public string OTLong { get; set; } = "OT Name too long.";
    public string OTShort { get; set; } = "OT Name too short.";
    public string OTSuspicious { get; set; } = "Suspicious Original Trainer details.";

    public string OT_IDEqual { get; set; } = "TID16 and SID16 are equal.";
    public string OT_IDs0 { get; set; } = "TID16 and SID16 are 0.";
    public string OT_SID0 { get; set; } = "SID16 is zero.";
    public string OT_SID0Invalid { get; set; } = "SID16 should be 0.";
    public string OT_TID0 { get; set; } = "TID16 is zero.";
    public string OT_IDInvalid { get; set; } = "TID16 and SID16 combination is not possible.";

    public string PIDEncryptWurmple { get; set; } = "Wurmple evolution Encryption Constant mismatch.";
    public string PIDEncryptZero { get; set; } = "Encryption Constant is not set.";
    public string PIDEqualsEC { get; set; } = "Encryption Constant matches PID.";
    public string PIDGenderMatch { get; set; } = "Gender matches PID.";
    public string PIDGenderMismatch { get; set; } = "PID-Gender mismatch.";
    public string PIDNatureMatch { get; set; } = "Nature matches PID.";
    public string PIDNatureMismatch { get; set; } = "PID-Nature mismatch.";
    public string PIDTypeMismatch { get; set; } = "PID+ correlation does not match what was expected for the Encounter's type.";
    public string PIDZero { get; set; } = "PID is not set.";

    public string PokerusDaysTooHigh_0 { get; set; } = "Pokérus Days Remaining value is too high; expected <= {0}.";
    public string PokerusStrainUnobtainable_0 { get; set; } = "Pokérus Strain {0} cannot be obtained.";

    public string RibbonAllValid { get; set; } = "All ribbons accounted for.";
    public string RibbonEgg { get; set; } = "Can't receive Ribbon(s) as an Egg.";
    public string RibbonsInvalid_0 { get; set; } = "Invalid Ribbons: {0}";
    public string RibbonsMissing_0 { get; set; } = "Missing Ribbons: {0}";
    public string RibbonMarkingInvalid_0 { get; set; } = "Invalid Marking: {0}";
    public string RibbonMarkingMissing_0 { get; set; } = "Missing Marking: {0}";
    public string RibbonMarkingAffixed_0 { get; set; } = "Invalid Affixed Ribbon/Marking: {0}";

    public string StatDynamaxInvalid { get; set; } = "Dynamax Level is not within the expected range.";
    public string StatIncorrectHeight { get; set; } = "Calculated Height does not match stored value.";
    public string StatIncorrectHeightCopy { get; set; } = "Copy Height does not match the original value.";
    public string StatIncorrectHeightValue { get; set; } = "Height does not match the expected value.";
    public string StatIncorrectWeight { get; set; } = "Calculated Weight does not match stored value.";
    public string StatIncorrectWeightValue { get; set; } = "Weight does not match the expected value.";
    public string StatInvalidHeightWeight { get; set; } = "Height / Weight values are statistically improbable.";
    public string StatIncorrectCP { get; set; } = "Calculated CP does not match stored value.";
    public string StatGigantamaxInvalid { get; set; } = "Gigantamax Flag mismatch.";
    public string StatGigantamaxValid { get; set; } = "Gigantamax Flag was changed via Max Soup.";
    public string StatNatureInvalid { get; set; } = "Stat Nature is not within the expected range.";
    public string StatBattleVersionInvalid { get; set; } = "Battle Version is not within the expected range.";
    public string StatNobleInvalid { get; set; } = "Noble Flag mismatch.";
    public string StatAlphaInvalid { get; set; } = "Alpha Flag mismatch.";

    public string StoredSourceEgg { get; set; } = "Egg must be in Box or Party.";
    public string StoredSlotSourceInvalid_0 { get; set; } = "Invalid Stored Source: {0}";

    public string SuperComplete { get; set; } = "Super Training complete flag mismatch.";
    public string SuperDistro { get; set; } = "Distribution Super Training missions are not released.";
    public string SuperEgg { get; set; } = "Can't Super Train an Egg.";
    public string SuperNoComplete { get; set; } = "Can't have active Super Training complete flag for origins.";
    public string SuperNoUnlocked { get; set; } = "Can't have active Super Training unlocked flag for origins.";
    public string SuperUnavailable { get; set; } = "Super Training missions are not available in games visited.";
    public string SuperUnused { get; set; } = "Unused Super Training Flag is flagged.";

    public string TeraTypeIncorrect { get; set; } = "Tera Type does not match the expected value.";
    public string TeraTypeMismatch { get; set; } = "Tera Type does not match either of the default types.";

    public string TradeNotAvailable { get; set; } = "Encounter cannot be traded to the active trainer.";

    public string TrainerIDNoSeed { get; set; } = "Trainer ID is not obtainable from any RNG seed.";

    public string TransferBad { get; set; } = "Incorrectly transferred from previous generation.";

    public string TransferCurrentHandlerInvalid { get; set; } = "Invalid Current handler value, trainer details for save file expected another value.";
    public string TransferEgg { get; set; } = "Can't transfer Eggs between Generations.";
    public string TransferEggLocationTransporter { get; set; } = "Invalid Met Location, expected Poké Transfer.";
    public string TransferEggMetLevel { get; set; } = "Invalid Met Level for transfer.";
    public string TransferEggVersion { get; set; } = "Can't transfer Eggs to this game.";
    public string TransferFlagIllegal { get; set; } = "Flagged as illegal by the game (glitch abuse).";
    public string TransferHTFlagRequired { get; set; } = "Current handler cannot be the OT.";
    public string TransferHTMismatchName { get; set; } = "Handling trainer does not match the expected trainer name.";
    public string TransferHTMismatchGender { get; set; } = "Handling trainer does not match the expected trainer gender.";
    public string TransferHTMismatchLanguage { get; set; } = "Handling trainer does not match the expected trainer language.";
    public string TransferKoreanGen4 { get; set; } = "Korean Generation 4 games cannot interact with International Generation 4 games.";
    public string TransferMet { get; set; } = "Invalid Met Location, expected Poké Transfer or Crown.";
    public string TransferNotPossible { get; set; } = "Unable to transfer into current format from origin format.";
    public string TransferMetLocation { get; set; } = "Invalid Transfer Met Location.";
    public string TransferNature { get; set; } = "Invalid Nature for transfer Experience.";
    public string TransferObedienceLevel { get; set; } = "Invalid Obedience Level.";
    public string TransferPIDECBitFlip { get; set; } = "PID should be equal to EC [with top bit flipped]!";
    public string TransferPIDECEquals { get; set; } = "PID should be equal to EC!";
    public string TransferPIDECXor { get; set; } = "Encryption Constant matches shinyxored PID.";
    public string TransferTrackerMissing { get; set; } = "Pokémon HOME Transfer Tracker is missing.";
    public string TransferTrackerShouldBeZero { get; set; } = "Pokémon HOME Transfer Tracker should be 0.";

    public string TrashBytesExpected { get; set; } = "Expected Trash Bytes.";
    public string TrashBytesMismatchInitial { get; set; } = "Expected initial trash bytes to match the encounter.";
    public string TrashBytesMissingTerminator { get; set; } = "Final terminator missing.";
    public string TrashBytesShouldBeEmpty { get; set; } = "Trash Bytes should be cleared.";

    #endregion

    public string EncTradeShouldHaveEvolvedToSpecies_0 { get; set; } = "Trade Encounter should have evolved to species: {0}.";
    public string EncGiftLanguageNotDistributed { get; set; } = "Gift Encounter was never distributed with this language.";
    public string EncGiftRegionNotDistributed { get; set; } = "Gift Encounter was never distributed to this Console Region.";
    public string FormInvalidRangeLEQ_0F { get; set; } = "Form Count is out of range. Expected <= {0}, got {1}.";
    public string MovesShouldMatchRelearnMoves { get; set; } = "Moves should exactly match Relearn Moves.";
    public string MemoryStatEnjoyment_0 { get; set; } = "Enjoyment should be {0}.";
    public string MemoryStatFullness_0 { get; set; } = "Fullness should be {0}.";
    public string MemoryStatFullnessLEQ_0 { get; set; } = "Fullness should be <= {0}.";
    public string OTLanguageShouldBe_0 { get; set; } = "Language ID should be {0}, not {1}.";
    public string OTLanguageShouldBe_0or1 { get; set; } = "Language ID should be {0} or {1}, not {2}.";
    public string OTLanguageShouldBeLeq_0 { get; set; } = "Language ID should be <= {0}, not {1}.";
    public string OTLanguageCannotPlayOnVersion_0 { get; set; } = "Language ID {0} cannot be played on this version.";
    public string OTLanguageCannotTransferToConsoleRegion_0 { get; set; } = "Language ID {0} cannot be transferred to this Console Region.";

    public string WordFilterInvalidCharacter_0 { get; set; } = "Word Filter: Invalid character '{0}' (0x{1}).";
    public string WordFilterFlaggedPattern_01 { get; set; } = "Word Filter ({1}): Flagged pattern '{0}'.";
    public string WordFilterTooManyNumbers_0 { get; set; } = "Word Filter: Too many numbers (>{0}).";
    public string BulkCloneDetectedDetails { get; set; } = "Clone detected (Details).";
    public string BulkCloneDetectedTracker { get; set; } = "Clone detected (Duplicate Tracker).";
    public string HintEvolvesToSpecies_0 { get; set; } = "Evolves to species: {0}.";
    public string HintEvolvesToRareForm_0 { get; set; } = "Evolves to rare form: {0}.";
    public string BulkSharingEncryptionConstantGenerationDifferent { get; set; } = "Detected sharing of Encryption Constant across generations.";
    public string BulkSharingEncryptionConstantGenerationSame { get; set; } = "Detected sharing of Encryption Constant.";
    public string BulkSharingEncryptionConstantRNGType { get; set; } = "Detected sharing of Encryption Constant sharing for different RNG encounters.";
    public string BulkSharingPIDGenerationDifferent { get; set; } = "Detected sharing of PID across generations.";
    public string BulkSharingPIDGenerationSame { get; set; } = "Detected sharing of PID.";
    public string BulkSharingPIDRNGType { get; set; } = "Detected sharing of PID for different RNG encounters.";
    public string BulkDuplicateMysteryGiftEggReceived { get; set; } = "Detected multiple redemptions of the same non-repeatable Mystery Gift Egg.";
    public string BulkSharingTrainerID { get; set; } = "Detected sharing of Trainer ID across multiple trainer names.";
    public string BulkSharingTrainerVersion { get; set; } = "Detected sharing of Trainer ID across multiple versions.";
}

[JsonSerializable(typeof(LegalityCheckLocalization))]
internal sealed partial class LegalityCheckLocalizationContext : JsonSerializerContext;
