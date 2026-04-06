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
    public string Valid { get; init; } = "Valid.";

    /// <summary>Default text for indicating legality.</summary>
    public string Legal { get; init; } = "Legal!";

    /// <summary>Default text for indicating an error has occurred.</summary>
    public string Error { get; init; } = "Internal error.";

    /// <summary>Analysis not available for the <see cref="PKM"/></summary>
    public string AnalysisUnavailable { get; init; } = "Analysis not available for this Pokémon.";

    /// <summary>Format text for exporting a legality check result.</summary>
    public string F0_1 { get; init; } = "{0}: {1}";

    /// <summary>Severity string for <see cref="Severity.Invalid"/></summary>
    public string SInvalid { get; init; } = "Invalid";

    /// <summary>Severity string for <see cref="Severity.Fishy"/></summary>
    public string SFishy { get; init; } = "Fishy";

    /// <summary>Severity string for <see cref="Severity.Valid"/></summary>
    public string SValid { get; init; } = "Valid";

    /// <summary>Severity string for anything not implemented.</summary>
    public string NotImplemented { get; init; } = "Not Implemented";

    public string AbilityCapsuleUsed { get; init; } = "Ability available with Ability Capsule.";
    public string AbilityPatchUsed { get; init; } = "Ability available with Ability Patch.";
    public string AbilityPatchRevertUsed { get; init; } = "Ability available with Ability Patch Revert.";
    public string AbilityFlag { get; init; } = "Ability matches ability number.";
    public string AbilityHiddenFail { get; init; } = "Hidden Ability mismatch for encounter type.";
    public string AbilityHiddenUnavailable { get; init; } = "Hidden Ability not available.";
    public string AbilityMismatch { get; init; } = "Ability mismatch for encounter.";
    public string AbilityMismatch3 { get; init; } = "Ability does not match Generation 3 species ability.";
    public string AbilityMismatchFlag { get; init; } = "Ability does not match ability number.";
    public string AbilityMismatchGift { get; init; } = "Ability does not match Mystery Gift.";
    public string AbilityMismatchPID { get; init; } = "Ability does not match PID.";
    public string AbilityUnexpected { get; init; } = "Ability is not valid for species/form.";

    public string AwakenedCap { get; init; } = "Individual AV cannot be greater than {0}.";
    public string AwakenedShouldBeValue { get; init; } = "{1} AV should be greater than {0}.";

    public string BallAbility { get; init; } = "Can't obtain Hidden Ability with Ball.";
    public string BallEggCherish { get; init; } = "Can't have Cherish Ball for regular Egg.";
    public string BallEggMaster { get; init; } = "Can't have Master Ball for regular Egg.";
    public string BallEnc { get; init; } = "Correct ball for encounter type.";
    public string BallEncMismatch { get; init; } = "Can't have ball for encounter type.";
    public string BallHeavy { get; init; } = "Can't have Heavy Ball for light, low-catch rate species (Gen VII).";
    public string BallSpecies { get; init; } = "Can't obtain species in Ball.";
    public string BallSpeciesPass { get; init; } = "Ball possible for species.";
    public string BallUnavailable { get; init; } = "Ball unobtainable in origin Generation.";
    public string BallG4Sinnoh { get; init; } = "Ball value for D/P/Pt (0x83) is not within range.";
    public string BallG4Johto { get; init; } = "Extended Ball value for HG/SS (0x86) is not within range.";

    public string ContestZero { get; init; } = "Contest Stats should be 0.";
    public string ContestZeroSheen { get; init; } = "Contest Stat Sheen should be 0.";
    public string ContestSheenGEQ_0 { get; init; } = "Contest Stat Sheen should be >= {0}.";
    public string ContestSheenLEQ_0 { get; init; } = "Contest Stat Sheen should be <= {0}.";

    public string DateCalendarInvalidMet { get; init; } = "Met Date is not a valid calendar date.";
    public string DateCalendarInvalidEgg { get; init; } = "Egg Met Date is not a valid calendar date.";
    public string DateLocalInvalidDate { get; init; } = "Local Date is outside of console's local time window.";
    public string DateLocalInvalidTime { get; init; } = "Local Time is not a valid timestamp.";
    public string DateOutsideDistributionWindow { get; init; } = "Met Date is outside of distribution window.";

    public string EggContest { get; init; } = "Cannot increase Contest Stats of an Egg.";
    public string EggEXP { get; init; } = "Eggs cannot receive experience.";
    public string EggFMetLevel_0 { get; init; } = "Invalid Met Level, expected {0}.";
    public string EggHatchCycles { get; init; } = "Invalid Egg hatch cycles.";
    public string EggLocation { get; init; } = "Able to hatch an Egg at Met Location.";
    public string EggLocationInvalid { get; init; } = "Can't hatch an Egg at Met Location.";
    public string EggLocationNone { get; init; } = "Invalid Egg Location, expected none.";
    public string EggLocationPalPark { get; init; } = "Invalid Met Location, expected Pal Park.";
    public string EggLocationTrade { get; init; } = "Able to hatch a traded Egg at Met Location.";
    public string EggLocationTradeFail { get; init; } = "Invalid Egg Location, shouldn't be 'traded' while an Egg.";
    public string EggMetLocationFail { get; init; } = "Can't obtain Egg from Egg Location.";
    public string EggNature { get; init; } = "Eggs cannot have their Stat Nature changed.";
    public string EggPP { get; init; } = "Eggs cannot have modified move PP counts.";
    public string EggPPUp { get; init; } = "Cannot apply PP Ups to an Egg.";
    public string EggRelearnFlags { get; init; } = "Expected no Relearn Move Flags.";
    public string EggShinyPokeStar { get; init; } = "Eggs cannot be a Pokéstar Studios star.";
    public string EggSpecies { get; init; } = "Can't obtain Egg for this species.";
    public string EggUnhatched { get; init; } = "Valid un-hatched Egg.";

    public string EncCondition { get; init; } = "Valid Wild Encounter at location.";
    public string EncConditionBadRNGFrame { get; init; } = "Unable to match encounter conditions to a possible RNG frame.";
    public string EncConditionBadSpecies { get; init; } = "Species does not exist in origin game.";

    public string EncGift { get; init; } = "Unable to match a gift Egg encounter from origin game.";
    public string EncGiftEggEvent { get; init; } = "Unable to match an event Egg encounter from origin game.";
    public string EncGiftIVMismatch { get; init; } = "IVs do not match Mystery Gift Data.";
    public string EncGiftNicknamed { get; init; } = "Event gift has been nicknamed.";
    public string EncGiftNotFound { get; init; } = "Unable to match to a Mystery Gift in the database.";
    public string EncGiftPIDMismatch { get; init; } = "Mystery Gift fixed PID mismatch.";
    public string EncGiftShinyMismatch { get; init; } = "Mystery Gift shiny mismatch.";
    public string EncGiftVersionNotDistributed { get; init; } = "Mystery Gift cannot be received by this version.";

    public string EncInvalid { get; init; } = "Unable to match an encounter from origin game.";
    public string EncMasteryInitial { get; init; } = "Initial move mastery flags do not match the encounter's expected state.";

    public string EncTradeChangedNickname { get; init; } = "In-game Trade Nickname has been altered.";
    public string EncTradeChangedOT { get; init; } = "In-game Trade OT has been altered.";
    public string EncTradeIndexBad { get; init; } = "In-game Trade invalid index?";
    public string EncTradeMatch { get; init; } = "Valid In-game trade.";
    public string EncTradeUnchanged { get; init; } = "In-game Trade OT and Nickname have not been altered.";

    public string EncStaticPIDShiny { get; init; } = "Encounter shiny mismatch.";
    public string EncTypeMatch { get; init; } = "Encounter Type matches encounter.";
    public string EncTypeMismatch { get; init; } = "Encounter Type does not match encounter.";
    public string EncUnreleased { get; init; } = "Unreleased event.";
    public string EncUnreleasedEMewJP { get; init; } = "Non japanese Mew from Faraway Island. Unreleased event.";

    public string EReaderAmerica { get; init; } = "American E-Reader Berry in Japanese save file.";
    public string EReaderInvalid { get; init; } = "Invalid E-Reader Berry.";
    public string EReaderJapan { get; init; } = "Japanese E-Reader Berry in international save file.";

    public string Effort2Remaining { get; init; } = "2 EVs remaining.";
    public string EffortAbove252 { get; init; } = "EVs cannot go above 252.";
    public string EffortAbove510 { get; init; } = "EV total cannot be above 510.";
    public string EffortAllEqual { get; init; } = "EVs are all equal.";
    public string EffortCap100 { get; init; } = "Individual EV for a level 100 encounter in Generation 4 cannot be greater than 100.";
    public string EffortEgg { get; init; } = "Eggs cannot receive EVs.";
    public string EffortShouldBeZero { get; init; } = "Cannot receive EVs.";
    public string EffortEXPIncreased { get; init; } = "All EVs are zero, but leveled above Met Level.";
    public string EffortUntrainedCap { get; init; } = "Individual EV without changing EXP cannot be greater than {0}.";

    public string EvoInvalid { get; init; } = "Evolution not valid (or level/trade evolution unsatisfied).";
    public string EvoTradeReqOutsider { get; init; } = "Outsider {0} should have evolved into {1}.";
    public string EvoTradeRequired { get; init; } = "Version Specific evolution requires a trade to opposite version. A Handling Trainer is required.";

    public string FatefulGiftMissing { get; init; } = "Fateful Encounter with no matching Encounter. Has the Mystery Gift data been contributed?";
    public string FatefulInvalid { get; init; } = "Fateful Encounter should not be checked.";
    public string FatefulMissing { get; init; } = "Special In-game Fateful Encounter flag missing.";
    public string FatefulMystery { get; init; } = "Mystery Gift Fateful Encounter.";
    public string FatefulMysteryMissing { get; init; } = "Mystery Gift Fateful Encounter flag missing.";

    public string FavoriteMarkingUnavailable { get; init; } = "Favorite Marking is not available.";

    public string FormArgumentLEQ_0 { get; init; } = "Form argument is too high for current form.";
    public string FormArgumentGEQ_0 { get; init; } = "Form argument is too low for current form.";
    public string FormArgumentNotAllowed { get; init; } = "Form argument is not allowed for this encounter.";
    public string FormArgumentValid { get; init; } = "Form argument is valid.";
    public string FormArgumentInvalid { get; init; } = "Form argument is not valid.";
    public string FormBattle { get; init; } = "Form cannot exist outside of a battle.";
    public string FormInvalidGame { get; init; } = "Form cannot be obtained in origin game.";
    public string FormInvalidNature { get; init; } = "Form cannot have this nature.";
    public string FormItem { get; init; } = "Held item matches Form.";
    public string FormItemInvalid { get; init; } = "Held item does not match Form.";
    public string FormParty { get; init; } = "Form cannot exist outside of Party.";
    public string FormInvalidExpect_0 { get; init; } = "Form is invalid, expected form index {0}.";
    public string FormValid { get; init; } = "Form is Valid.";
    public string FormVivillon { get; init; } = "Valid Vivillon pattern.";
    public string FormVivillonEventPre { get; init; } = "Event Vivillon pattern on pre-evolution.";
    public string FormVivillonInvalid { get; init; } = "Invalid Vivillon pattern.";
    public string FormVivillonNonNative { get; init; } = "Non-native Vivillon pattern.";

    public string G1CatchRateChain { get; init; } = "Catch rate does not match any species from Pokémon evolution chain.";
    public string G1CatchRateEvo { get; init; } = "Catch rate match species without encounters. Expected a pre-evolution catch rate.";
    public string G1CatchRateItem { get; init; } = "Catch rate does not match a valid held item from Generation 2.";
    public string G1CatchRateMatchPrevious { get; init; } = "Catch Rate matches a species from Pokémon evolution chain.";
    public string G1CatchRateMatchTradeback { get; init; } = "Catch rate matches a valid held item from Generation 2.";
    public string G1CatchRateNone { get; init; } = "Catch rate does not match any species from Pokémon evolution chain or any Generation 2 held items.";
    public string G1CharNick { get; init; } = "Nickname from Generation 1/2 uses unavailable characters.";
    public string G1CharOT { get; init; } = "OT from Generation 1/2 uses unavailable characters.";
    public string G1OTGender { get; init; } = "Female OT from Generation 1/2 is invalid.";
    public string G1Stadium { get; init; } = "Incorrect Stadium OT.";
    public string G1Type1Fail { get; init; } = "Invalid Type A, does not match species type.";
    public string G1Type2Fail { get; init; } = "Invalid Type B, does not match species type.";
    public string G1TypeMatch1 { get; init; } = "Valid Type A, matches species type.";
    public string G1TypeMatch2 { get; init; } = "Valid Type B, matches species type.";
    public string G1TypeMatchPorygon { get; init; } = "Porygon with valid Type A and B values.";
    public string G1TypePorygonFail { get; init; } = "Porygon with invalid Type A and B values. Does not a match a valid type combination.";
    public string G1TypePorygonFail1 { get; init; } = "Porygon with invalid Type A value.";
    public string G1TypePorygonFail2 { get; init; } = "Porygon with invalid Type B value.";
    public string G2InvalidTileTreeNotFound { get; init; } = "Could not find a tree for Crystal headbutt encounter that matches OTID.";
    public string G2TreeID { get; init; } = "Found a tree for Crystal headbutt encounter that matches OTID.";
    public string G2OTGender { get; init; } = "OT from Virtual Console games other than Crystal cannot be female.";

    public string G3EReader { get; init; } = "Non Japanese Shadow E-reader Pokémon. Unreleased encounter.";
    public string G3OTGender { get; init; } = "OT from Colosseum/XD cannot be female.";
    public string G4InvalidTileR45Surf { get; init; } = "Johto Route 45 surfing encounter. Unreachable Water tiles.";
    public string G4PartnerMoodEgg { get; init; } = "Eggs cannot have an Mood stat value.";
    public string G4PartnerMoodZero { get; init; } = "Mood stat value should be zero when not in the player's party.";
    public string G4ShinyLeafBitsInvalid { get; init; } = "Shiny Leaf/Crown bits are not valid.";
    public string G4ShinyLeafBitsEgg { get; init; } = "Eggs cannot have Shiny Leaf/Crown.";
    public string G5IVAll30 { get; init; } = "All IVs of N's Pokémon should be 30.";
    public string G5PIDShinyGrotto { get; init; } = "Hidden Grotto captures cannot be shiny.";
    public string G5SparkleInvalid { get; init; } = "Special In-game N's Sparkle flag should not be checked.";
    public string G5SparkleRequired { get; init; } = "Special In-game N's Sparkle flag missing.";
    public string G5PokeStarMustBeZero { get; init; } = "Pokéstar Studios fame must be zero, cannot participate.";
    public string G5PokeStarImpossibleValue { get; init; } = "Pokéstar Studios fame value is unreachable.";
    public string G7BSocialShouldBe100Spirit { get; init; } = "Spirit should be 100 for Pokémon not in the player's party.";
    public string G7BSocialShouldBe100Mood { get; init; } = "Mood should be 100 for Pokémon not in the player's party.";

    public string GanbaruStatTooHigh { get; init; } = "One or more Ganbaru Value is above the natural limit of (10 - IV bonus).";

    public string GenderInvalidNone { get; init; } = "Genderless Pokémon should not have a gender.";
    public string GeoBadOrder { get; init; } = "GeoLocation Memory: Gap/Blank present.";
    public string GeoHardwareInvalid { get; init; } = "Geolocation: Country is not in 3DS region.";
    public string GeoHardwareRange { get; init; } = "Invalid Console Region.";
    public string GeoHardwareValid { get; init; } = "Geolocation: Country is in 3DS region.";
    public string GeoMemoryMissing { get; init; } = "GeoLocation Memory: Memories should be present.";
    public string GeoNoCountryHT { get; init; } = "GeoLocation Memory: HT Name present but has no previous Country.";
    public string GeoNoRegion { get; init; } = "GeoLocation Memory: Region without Country.";

    public string HyperTrainLevelGEQ_0 { get; init; } = "Can't Hyper Train a Pokémon that isn't level {0}.";
    public string HyperPerfectAll { get; init; } = "Can't Hyper Train a Pokémon with perfect IVs.";
    public string HyperPerfectOne { get; init; } = "Can't Hyper Train a perfect IV.";
    public string HyperPerfectUnavailable { get; init; } = "Can't Hyper Train any IV(s).";

    public string ItemEgg { get; init; } = "Eggs cannot hold items.";
    public string ItemUnreleased { get; init; } = "Held item is unreleased.";

    public string IVAllEqual_0 { get; init; } = "All IVs are {0}.";
    public string IVNotCorrect { get; init; } = "IVs do not match encounter requirements.";
    public string IVFlawlessCountGEQ_0 { get; init; } = "Should have at least {0} IVs = 31.";

    public string LevelBoostNotZero { get; init; } = "Level Boost should be zero.";
    public string LevelEXPThreshold { get; init; } = "Current experience matches level threshold.";
    public string LevelEXPTooHigh { get; init; } = "Current experience exceeds maximum amount for level 100.";
    public string LevelMetBelow { get; init; } = "Current level is below met level.";
    public string LevelMetGift { get; init; } = "Met Level does not match Mystery Gift level.";
    public string LevelMetGiftFail { get; init; } = "Current Level below Mystery Gift level.";
    public string LevelMetSane { get; init; } = "Current level is not below met level.";

    public string MarkValueOutOfRange_0 { get; init; } = "Individual marking at index {0} is not within the allowed value range.";
    public string MarkValueShouldBeZero { get; init; } = "Marking flags cannot be set.";
    public string MarkValueUnusedBitsPresent { get; init; } = "Marking flags uses bits beyond the accessible range.";

    public string MemoryArgBadCatch_H { get; init; } = "{0} Memory: {0} did not catch this.";
    public string MemoryArgBadHatch_H { get; init; } = "{0} Memory: {0} did not hatch this.";
    public string MemoryArgBadHT { get; init; } = "Memory: Can't have Handling Trainer Memory as Egg.";
    public string MemoryArgBadID_H { get; init; } = "{0} Memory: Can't obtain Memory on {0} Version.";
    public string MemoryArgBadItem_H1 { get; init; } = "{0} Memory: Species can't hold this item.";
    public string MemoryArgBadLocation_H { get; init; } = "{0} Memory: Can't obtain Location on {0} Version.";
    public string MemoryArgBadMove_H1 { get; init; } = "{0} Memory: Species can't learn {1}.";
    public string MemoryArgBadOTEgg_H { get; init; } = "{0} Memory: Link Trade is not a valid first memory.";
    public string MemoryArgBadSpecies_H1 { get; init; } = "{0} Memory: Can't capture species in game.";
    public string MemoryArgSpecies_H { get; init; } = "{0} Memory: Species can be captured in game.";
    public string MemoryCleared_H { get; init; } = "Memory: Not cleared properly.";
    public string MemoryValid_H { get; init; } = "{0} Memory is valid.";
    public string MemoryFeelInvalid_H { get; init; } = "{0} Memory: Invalid Feeling.";
    public string MemoryHTFlagInvalid { get; init; } = "Untraded: Current handler should not be the Handling Trainer.";
    public string MemoryHTGender_0 { get; init; } = "HT Gender invalid: {0}";
    public string MemoryHTLanguage { get; init; } = "HT Language is missing.";

    public string MemoryIndexArgHT { get; init; } = "Should have a HT Memory TextVar value (somewhere).";
    public string MemoryIndexFeel_H1 { get; init; } = "{0} Memory: Feeling should be index {1}.";
    public string MemoryIndexFeelHTLEQ9 { get; init; } = "Should have a HT Memory Feeling value 0-9.";
    public string MemoryIndexID_H1 { get; init; } = "{0} Memory: Should be index {1}.";
    public string MemoryIndexIntensity_H1 { get; init; } = "{0} Memory: Intensity should be index {1}.";
    public string MemoryIndexIntensityHT1 { get; init; } = "Should have a HT Memory Intensity value (1st).";
    public string MemoryIndexIntensityMin_H1 { get; init; } = "{0} Memory: Intensity should be at least {1}.";
    public string MemoryIndexLinkHT { get; init; } = "Should have a Link Trade HT Memory.";
    public string MemoryIndexVar { get; init; } = "{0} Memory: TextVar should be index {1}.";
    public string MemoryMissingHT { get; init; } = "Memory: Handling Trainer Memory missing.";
    public string MemoryMissingOT { get; init; } = "Memory: Original Trainer Memory missing.";

    public string MemorySocialZero { get; init; } = "Social Stat should be zero.";
    public string MemoryStatSocialLEQ_0 { get; init; } = "Social Stat should be <= {0}";

    public string MemoryStatAffectionHT0 { get; init; } = "Untraded: Handling Trainer Affection should be 0.";
    public string MemoryStatAffectionOT0 { get; init; } = "OT Affection should be 0.";
    public string MemoryStatFriendshipHT0 { get; init; } = "Untraded: Handling Trainer Friendship should be 0.";
    public string MemoryStatFriendshipOTBaseEvent_0 { get; init; } = "Event OT Friendship does not match base friendship ({0}).";

    public string MetDetailTimeOfDay { get; init; } = "Met Time of Day value is not within the expected range.";
    public string MoveEvoFCombination_0 { get; init; } = "Moves combinations is not compatible with {0} evolution.";
    public string MoveFExpectSingle_0 { get; init; } = "Expected: {0}";
    public string MoveKeldeoMismatch { get; init; } = "Keldeo Move/Form mismatch.";
    public string MovePPExpectHealed_01 { get; init; } = "Move {0} PP is below the amount expected ({1}).";
    public string MovePPTooHigh_01 { get; init; } = "Move {0} PP is above the amount allowed ({1}).";
    public string MovePPUpsTooHigh_01 { get; init; } = "Move {0} PP Ups is above the amount allowed ({1}).";

    public string MoveShopAlphaMoveShouldBeMastered_0 { get; init; } = "Alpha Move should be marked as mastered.";
    public string MoveShopAlphaMoveShouldBeOther { get; init; } = "Alpha encounter cannot be found with this Alpha Move.";
    public string MoveShopAlphaMoveShouldBeZero { get; init; } = "Only Alphas may have an Alpha Move set.";
    public string MoveShopMasterInvalid_0 { get; init; } = "Cannot manually master {0}: not permitted to master.";
    public string MoveShopMasterNotLearned_0 { get; init; } = "Cannot manually master {0}: not in possible learned level up moves.";
    public string MoveShopPurchaseInvalid_0 { get; init; } = "Cannot purchase {0} from the move shop.";

    public string MoveTechRecordFlagMissing_0 { get; init; } = "Unexpected Technical Record Learned flag: {0}";

    public string NickFlagEggNo { get; init; } = "Egg must be not nicknamed.";
    public string NickFlagEggYes { get; init; } = "Egg must be nicknamed.";
    public string NickInvalidChar { get; init; } = "Cannot be given this Nickname.";
    public string NickLengthLong { get; init; } = "Nickname too long.";
    public string NickLengthShort { get; init; } = "Nickname is empty.";
    public string NickMatchLanguage { get; init; } = "Nickname matches species name.";
    public string NickMatchLanguageEgg { get; init; } = "Egg matches language Egg name.";
    public string NickMatchLanguageEggFail { get; init; } = "Egg name does not match language Egg name.";
    public string NickMatchLanguageFail { get; init; } = "Nickname does not match species name.";
    public string NickMatchLanguageFlag { get; init; } = "Nickname flagged, matches species name.";
    public string NickMatchNoOthers { get; init; } = "Nickname does not match another species name.";
    public string NickMatchNoOthersFail { get; init; } = "Nickname matches another species name (+language).";

    public string OTLanguage { get; init; } = "Language ID should be {0}, not {1}.";
    public string OTLong { get; init; } = "OT Name too long.";
    public string OTShort { get; init; } = "OT Name too short.";
    public string OTSuspicious { get; init; } = "Suspicious Original Trainer details.";

    public string OT_IDEqual { get; init; } = "TID16 and SID16 are equal.";
    public string OT_IDs0 { get; init; } = "TID16 and SID16 are 0.";
    public string OT_SID0 { get; init; } = "SID16 is zero.";
    public string OT_SID0Invalid { get; init; } = "SID16 should be 0.";
    public string OT_TID0 { get; init; } = "TID16 is zero.";
    public string OT_IDInvalid { get; init; } = "TID16 and SID16 combination is not possible.";

    public string PIDEncryptWurmple { get; init; } = "Wurmple evolution Encryption Constant mismatch.";
    public string PIDEncryptZero { get; init; } = "Encryption Constant is not set.";
    public string PIDEqualsEC { get; init; } = "Encryption Constant matches PID.";
    public string PIDGenderMatch { get; init; } = "Gender matches PID.";
    public string PIDGenderMismatch { get; init; } = "PID-Gender mismatch.";
    public string PIDNatureMatch { get; init; } = "Nature matches PID.";
    public string PIDNatureMismatch { get; init; } = "PID-Nature mismatch.";
    public string PIDTypeMismatch { get; init; } = "PID+ correlation does not match what was expected for the Encounter's type.";
    public string PIDZero { get; init; } = "PID is not set.";

    public string PlusMoveAlphaMissing_0 { get; init; } = "Expected to have mastered the move {0} when encountered as an alpha.";
    public string PlusMoveMultipleInvalid { get; init; } = "Multiple Plus Move flags are invalid.";
    public string PlusMoveInvalid_0 { get; init; } = "{0} cannot be learned and set as a Plus Move.";
    public string PlusMoveSufficientLevelMissing_0 { get; init; } = "Plus Move flag for {0} must be set."; // as the Pokémon's current level is above the level it becomes available for use as a Plus Move.
    public string PlusMoveCountInvalid { get; init; } = "Out of range Plus Move flag index is set.";

    public string PokerusDaysTooHigh_0 { get; init; } = "Pokérus Days Remaining value is too high; expected <= {0}.";
    public string PokerusStrainUnobtainable_0 { get; init; } = "Pokérus Strain {0} cannot be obtained.";

    public string RibbonAllValid { get; init; } = "All ribbons accounted for.";
    public string RibbonEgg { get; init; } = "Can't receive Ribbon(s) as an Egg.";
    public string RibbonsInvalid_0 { get; init; } = "Invalid Ribbons: {0}";
    public string RibbonsMissing_0 { get; init; } = "Missing Ribbons: {0}";
    public string RibbonMarkingInvalid_0 { get; init; } = "Invalid Marking: {0}";
    public string RibbonMarkingMissing_0 { get; init; } = "Missing Marking: {0}";
    public string RibbonMarkingAffixed_0 { get; init; } = "Invalid Affixed Ribbon/Marking: {0}";

    public string StatDynamaxInvalid { get; init; } = "Dynamax Level is not within the expected range.";
    public string StatIncorrectHeight { get; init; } = "Calculated Height does not match stored value.";
    public string StatIncorrectWeight { get; init; } = "Calculated Weight does not match stored value.";
    public string StatIncorrectHeightValue_0 { get; init; } = "Height should be {0}.";
    public string StatIncorrectWeightValue_0 { get; init; } = "Weight should be {0}.";
    public string StatIncorrectScaleValue_0 { get; init; } = "Scale should be {0}.";
    public string StatInvalidHeightWeight { get; init; } = "Height / Weight values are statistically improbable.";
    public string StatIncorrectCP { get; init; } = "Calculated CP does not match stored value.";
    public string StatGigantamaxInvalid { get; init; } = "Gigantamax Flag mismatch.";
    public string StatGigantamaxValid { get; init; } = "Gigantamax Flag was changed via Max Soup.";
    public string StatNatureInvalid { get; init; } = "Stat Nature is not within the expected range.";
    public string StatBattleVersionInvalid { get; init; } = "Battle Version is not within the expected range.";
    public string StatNobleInvalid { get; init; } = "Noble Flag mismatch.";
    public string StatAlphaInvalid { get; init; } = "Alpha Flag mismatch.";

    public string StoredSourceEgg { get; init; } = "Egg must be in Box or Party.";
    public string StoredSlotSourceInvalid_0 { get; init; } = "Invalid Stored Source: {0}";

    public string SuperComplete { get; init; } = "Super Training complete flag mismatch.";
    public string SuperDistro { get; init; } = "Distribution Super Training missions are not released.";
    public string SuperEgg { get; init; } = "Can't Super Train an Egg.";
    public string SuperNoComplete { get; init; } = "Can't have active Super Training complete flag for origins.";
    public string SuperNoUnlocked { get; init; } = "Can't have active Super Training unlocked flag for origins.";
    public string SuperUnavailable { get; init; } = "Super Training missions are not available in games visited.";
    public string SuperUnused { get; init; } = "Unused Super Training Flag is flagged.";
    public string G6SuperTrainEggBag { get; init; } = "Egg cannot use a Training Bag.";
    public string G6SuperTrainEggHits { get; init; } = "Eggs cannot hit Training Bags.";
    public string G6SuperTrainBagInvalid_0 { get; init; } = "Unrecognized Training Bag ID: {0}";
    public string G6SuperTrainBagHitsInvalid_012 { get; init; } = "Training bag cannot have {0} hits; expected value within [{1},{2}].";

    public string TeraTypeIncorrect { get; init; } = "Tera Type does not match the expected value.";
    public string TeraTypeMismatch { get; init; } = "Tera Type does not match either of the default types.";

    public string TradeNotAvailable { get; init; } = "Encounter cannot be traded to the active trainer.";

    public string TrainerIDNoSeed { get; init; } = "Trainer ID is not obtainable from any RNG seed.";

    public string TransferBad { get; init; } = "Incorrectly transferred from previous generation.";

    public string TransferCurrentHandlerInvalid { get; init; } = "Invalid Current handler value, trainer details for save file expected another value.";
    public string TransferEgg { get; init; } = "Can't transfer Eggs between Generations.";
    public string TransferEggLocationTransporter { get; init; } = "Invalid Met Location, expected Poké Transfer.";
    public string TransferEggMetLevel { get; init; } = "Invalid Met Level for transfer.";
    public string TransferEggVersion { get; init; } = "Can't transfer Eggs to this game.";
    public string TransferFlagIllegal { get; init; } = "Flagged as illegal by the game (glitch abuse).";
    public string TransferHTFlagRequired { get; init; } = "Current handler cannot be the OT.";
    public string TransferHTMismatchName { get; init; } = "Handling trainer does not match the expected trainer name.";
    public string TransferHTMismatchGender { get; init; } = "Handling trainer does not match the expected trainer gender.";
    public string TransferHTMismatchLanguage { get; init; } = "Handling trainer does not match the expected trainer language.";
    public string TransferKoreanGen4 { get; init; } = "Korean Generation 4 games cannot interact with International Generation 4 games.";
    public string TransferMet { get; init; } = "Invalid Met Location, expected Poké Transfer or Crown.";
    public string TransferNotPossible { get; init; } = "Unable to transfer into current format from origin format.";
    public string TransferMetLocation { get; init; } = "Invalid Transfer Met Location.";
    public string TransferNature { get; init; } = "Invalid Nature for transfer Experience.";
    public string TransferObedienceLevel { get; init; } = "Invalid Obedience Level.";
    public string TransferPIDECBitFlip { get; init; } = "PID should be equal to EC [with top bit flipped]!";
    public string TransferPIDECEquals { get; init; } = "PID should be equal to EC!";
    public string TransferPIDECXor { get; init; } = "Encryption Constant matches shiny-xor'd PID.";
    public string TransferTrackerMissing { get; init; } = "Pokémon HOME Transfer Tracker is missing.";
    public string TransferTrackerShouldBeZero { get; init; } = "Pokémon HOME Transfer Tracker should be 0.";

    public string TrashBytesExpected { get; init; } = "Expected Trash Bytes.";
    public string TrashBytesMismatchInitial { get; init; } = "Expected initial trash bytes to match the encounter.";
    public string TrashBytesMissingTerminatorFinal { get; init; } = "Final terminator missing.";
    public string TrashBytesShouldBeEmpty { get; init; } = "Trash Bytes should be cleared.";
    public string TrashBytesResetViaTransfer { get; init; } = "Trash Bytes were reset via transfer.";

    #endregion

    public string EncTradeShouldHaveEvolvedToSpecies_0 { get; init; } = "Trade Encounter should have evolved to species: {0}.";
    public string EncGiftLanguageNotDistributed { get; init; } = "Gift Encounter was never distributed with this language.";
    public string EncGiftRegionNotDistributed { get; init; } = "Gift Encounter was never distributed to this Console Region.";
    public string FormInvalidRangeLEQ_0F { get; init; } = "Form Count is out of range. Expected <= {0}, got {1}.";
    public string MovesShouldMatchRelearnMoves { get; init; } = "Moves should exactly match Relearn Moves.";
    public string MemoryStatEnjoyment_0 { get; init; } = "Enjoyment should be {0}.";
    public string MemoryStatFullness_0 { get; init; } = "Fullness should be {0}.";
    public string MemoryStatFullnessLEQ_0 { get; init; } = "Fullness should be <= {0}.";
    public string OTLanguageShouldBe_0 { get; init; } = "Language ID should be {0}, not {1}.";
    public string OTLanguageShouldBe_0or1 { get; init; } = "Language ID should be {0} or {1}, not {2}.";
    public string OTLanguageShouldBeLeq_0 { get; init; } = "Language ID should be <= {0}, not {1}.";
    public string OTLanguageCannotPlayOnVersion_0 { get; init; } = "Language ID {0} cannot be played on this version.";
    public string OTLanguageCannotTransferToConsoleRegion_0 { get; init; } = "Language ID {0} cannot be transferred to this Console Region.";

    public string WordFilterInvalidCharacter_0 { get; init; } = "Word Filter: Invalid character '{0}' (0x{1}).";
    public string WordFilterFlaggedPattern_01 { get; init; } = "Word Filter ({1}): Flagged pattern '{0}'.";
    public string WordFilterTooManyNumbers_0 { get; init; } = "Word Filter: Too many numbers (>{0}).";
    public string BulkCloneDetectedDetails { get; init; } = "Clone detected (Details).";
    public string BulkCloneDetectedTracker { get; init; } = "Clone detected (Duplicate Tracker).";
    public string HintEvolvesToSpecies_0 { get; init; } = "Evolves to species: {0}.";
    public string HintEvolvesToRareForm_0 { get; init; } = "Evolves to rare form: {0}.";
    public string BulkSharingEncryptionConstantGenerationDifferent { get; init; } = "Detected sharing of Encryption Constant across generations.";
    public string BulkSharingEncryptionConstantGenerationSame { get; init; } = "Detected sharing of Encryption Constant.";
    public string BulkSharingEncryptionConstantRNGType { get; init; } = "Detected sharing of Encryption Constant sharing for different RNG encounters.";
    public string BulkSharingPIDGenerationDifferent { get; init; } = "Detected sharing of PID across generations.";
    public string BulkSharingPIDGenerationSame { get; init; } = "Detected sharing of PID.";
    public string BulkSharingPIDRNGType { get; init; } = "Detected sharing of PID for different RNG encounters.";
    public string BulkDuplicateMysteryGiftEggReceived { get; init; } = "Detected multiple redemptions of the same non-repeatable Mystery Gift Egg.";
    public string BulkSharingTrainerID { get; init; } = "Detected sharing of Trainer ID across multiple trainer names.";
    public string BulkSharingTrainerVersion { get; init; } = "Detected sharing of Trainer ID across multiple versions.";
    public string BulkDuplicateFusionSlot { get; init; } = "Detected multiple fusions of the same fusion stored slot species.";
    public string BulkHeldItemInventoryAssignedNoneHeld_0 { get; init; } = "{0} is marked as held player inventory, but no Pokémon found in slots checked.";
    public string BulkHeldItemInventoryMultipleSlots_0 { get; init; } = "{0} is a unique item and cannot be held by multiple Pokémon.";
    public string BulkHeldItemInventoryNotAcquired_0 { get; init; } = "{0} has not been acquired in player inventory.";
    public string BulkHeldItemInventoryUnassigned_0 { get; init; } = "{0} is not marked as assigned in player inventory.";
    public string BulkFusionSourceInvalid { get; init; } = "The subsumed Species-Form stored in the save file does not match the expected Species-Form of the fused slot.";
}

[JsonSerializable(typeof(LegalityCheckLocalization))]
internal sealed partial class LegalityCheckLocalizationContext : JsonSerializerContext;
