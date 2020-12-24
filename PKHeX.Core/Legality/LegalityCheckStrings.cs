// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace PKHeX.Core
{
    /// <summary>
    /// Legality Check Message Strings to indicate why certain <see cref="PKM"/> <see cref="LegalInfo"/> values are flagged.
    /// </summary>
    public static class LegalityCheckStrings
    {
        // Message String Name format: L/F[Category][Summary]
        #region General Strings

        /// <summary>Default text for indicating validity.</summary>
        public static string L_AValid { get; set; } = "Valid.";

        /// <summary>Default text for indicating legality.</summary>
        public static string L_ALegal { get; set; } = "Legal!";

        /// <summary>Default text for indicating an error has occurred.</summary>
        public static string L_AError { get; set; } = "Internal error.";

        /// <summary>Analysis not available for the <see cref="PKM"/></summary>
        public static string L_AnalysisUnavailable { get; set; } = "Analysis not available for this Pokémon.";

        /// <summary>Format text for exporting a legality check result.</summary>
        public static string L_F0_1 { get; set; } = "{0}: {1}";

        /// <summary>Format text for exporting a legality check result for a Move.</summary>
        public static string L_F0_M_1_2 { get; set; } = "{0} Move {1}: {2}";

        /// <summary>Format text for exporting a legality check result for a Relearn Move.</summary>
        public static string L_F0_RM_1_2 { get; set; } = "{0} Relearn Move {1}: {2}";

        /// <summary>Format text for exporting the type of Encounter that was matched for the <see cref="PKM"/></summary>
        public static string L_FEncounterType_0 { get; set; } = "Encounter Type: {0}";

        /// <summary>Format text for exporting the <see cref="PIDIV.OriginSeed"/> that was matched for the <see cref="PKM"/></summary>
        public static string L_FOriginSeed_0 { get; set; } = "Origin Seed: {0}";

        /// <summary>Format text for exporting the <see cref="PIDIV.Type"/> that was matched for the <see cref="PKM"/></summary>
        public static string L_FPIDType_0 { get; set; } = "PID Type: {0}";

        /// <summary>Severity string for <see cref="Severity.Indeterminate"/></summary>
        public static string L_SIndeterminate { get; set; } = "Indeterminate";

        /// <summary>Severity string for <see cref="Severity.Invalid"/></summary>
        public static string L_SInvalid { get; set; } = "Invalid";

        /// <summary>Severity string for <see cref="Severity.Fishy"/></summary>
        public static string L_SFishy { get; set; } = "Fishy";

        /// <summary>Severity string for <see cref="Severity.Valid"/></summary>
        public static string L_SValid { get; set; } = "Valid";

        /// <summary>Severity string for anything not implemented.</summary>
        public static string L_SNotImplemented { get; set; } = "Not Implemented";

        public static string L_XOT { get; set; } = "OT";
        public static string L_XHT { get; set; } = "HT";
        public static string L_XKorean { get; set; } = "Korean";
        public static string L_XKoreanNon { get; set; } = "Non-Korean";
        public static string L_XEnigmaBerry_0 { get; set; } = "{0} Berry";
        public static string L_XMatches0_1 { get; set; } = "Matches: {0} {1}";
        public static string L_XWurmpleEvo_0 { get; set; } = "Wurmple Evolution: {0}";

        public static string LAbilityCapsuleUsed { get; set; } = "Ability modified with Ability Capsule.";
        public static string LAbilityPatchUsed { get; set; } = "Ability modified with Ability Patch.";
        public static string LAbilityFlag { get; set; } = "Ability matches ability number.";
        public static string LAbilityHiddenFail { get; set; } = "Hidden Ability mismatch for encounter type.";
        public static string LAbilityHiddenUnavailable { get; set; } = "Hidden Ability not available.";
        public static string LAbilityMismatch { get; set; } = "Ability mismatch for encounter.";
        public static string LAbilityMismatch3 { get; set; } = "Ability does not match Generation 3 species ability.";
        public static string LAbilityMismatchFlag { get; set; } = "Ability does not match ability number.";
        public static string LAbilityMismatchGift { get; set; } = "Ability does not match Mystery Gift.";
        public static string LAbilityMismatchGrotto { get; set; } = "Hidden Grotto captures should have Hidden Ability.";
        public static string LAbilityMismatchHordeSafari { get; set; } = "Hidden Ability on non-horde/friend safari wild encounter.";
        public static string LAbilityMismatchPID { get; set; } = "Ability does not match PID.";
        public static string LAbilityMismatchSOS { get; set; } = "Hidden Ability on non-SOS wild encounter.";
        public static string LAbilityUnexpected { get; set; } = "Ability is not valid for species/form.";

        public static string LAwakenedCap { get; set; } = "Individual AV cannot be greater than {0}.";
        public static string LAwakenedShouldBeValue { get; set; } = "Individual AV should be greater than {0}.";
        public static string LAwakenedEXPIncreased { get; set; } = "All AVs are zero, but leveled above Met Level.";

        public static string LBallAbility { get; set; } = "Can't obtain Hidden Ability with Ball.";
        public static string LBallEggCherish { get; set; } = "Can't have Cherish Ball for regular Egg.";
        public static string LBallEggMaster { get; set; } = "Can't have Master Ball for regular Egg.";
        public static string LBallEnc { get; set; } = "Correct ball for encounter type.";
        public static string LBallEncMismatch { get; set; } = "Can't have ball for encounter type.";
        public static string LBallHeavy { get; set; } = "Can't have Heavy Ball for light, low-catch rate species (Gen VII).";
        public static string LBallNone { get; set; } = "No check satisfied, assuming illegal.";
        public static string LBallSpecies { get; set; } = "Can't obtain species in Ball.";
        public static string LBallSpeciesPass { get; set; } = "Ball possible for species.";
        public static string LBallUnavailable { get; set; } = "Ball unobtainable in origin Generation.";

        public static string LContestZero { get; set; } = "Contest Stats should be 0.";

        public static string LEggContest { get; set; } = "Cannot increase Contest Stats of an Egg.";
        public static string LEggEXP { get; set; } = "Eggs cannot receive experience.";
        public static string LEggFMetLevel_0 { get; set; } = "Invalid Met Level, expected {0}.";
        public static string LEggHatchCycles { get; set; } = "Invalid Egg hatch cycles.";
        public static string LEggLocation { get; set; } = "Able to hatch an Egg at Met Location.";
        public static string LEggLocationInvalid { get; set; } = "Can't hatch an Egg at Met Location.";
        public static string LEggLocationNone { get; set; } = "Invalid Egg Location, expected none.";
        public static string LEggLocationPalPark { get; set; } = "Invalid Met Location, expected Pal Park.";
        public static string LEggLocationTrade { get; set; } = "Able to hatch a traded Egg at Met Location.";
        public static string LEggLocationTradeFail { get; set; } = "Invalid Egg Location, shouldn't be 'traded' while an Egg.";
        public static string LEggMetLocationFail { get; set; } = "Can't obtain Egg from Egg Location.";
        public static string LEggNature { get; set; } = "Eggs cannot have their Stat Nature changed.";
        public static string LEggPokeathlon { get; set; } = "Eggs cannot have Pokéathlon stats.";
        public static string LEggPokerus { get; set; } = "Eggs cannot be infected with Pokérus.";
        public static string LEggPP { get; set; } = "Eggs cannot have modified move PP counts.";
        public static string LEggPPUp { get; set; } = "Cannot apply PP Ups to an Egg.";
        public static string LEggRelearnFlags { get; set; } = "Expected no Relearn Move Flags.";
        public static string LEggShinyLeaf { get; set; } = "Eggs cannot have Shiny Leaf/Crown.";
        public static string LEggShinyPokeStar { get; set; } = "Eggs cannot be a Pokéstar Studios star.";
        public static string LEggSpecies { get; set; } = "Can't obtain Egg for this species.";
        public static string LEggUnhatched { get; set; } = "Valid un-hatched Egg.";

        public static string LEncCondition { get; set; } = "Valid Wild Encounter at location.";
        public static string LEncConditionBadRNGFrame { get; set; } = "Unable to match encounter conditions to a possible RNG frame.";
        public static string LEncConditionBadSpecies { get; set; } = "Species does not exist in origin game.";
        public static string LEncConditionBlack { get; set; } = "Valid Wild Encounter at location (Black Flute).";
        public static string LEncConditionBlackLead { get; set; } = "Valid Wild Encounter at location (Black Flute & Pressure/Hustle/Vital Spirit).";
        public static string LEncConditionDexNav { get; set; } = "Valid Wild Encounter at location (DexNav).";
        public static string LEncConditionLead { get; set; } = "Valid Wild Encounter at location (Pressure/Hustle/Vital Spirit).";
        public static string LEncConditionWhite { get; set; } = "Valid Wild Encounter at location (White Flute).";
        public static string LEncConditionWhiteLead { get; set; } = "Valid Wild Encounter at location (White Flute & Pressure/Hustle/Vital Spirit).";

        public static string LEncGift { get; set; } = "Unable to match a gift Egg encounter from origin game.";
        public static string LEncGiftEggEvent { get; set; } = "Unable to match an event Egg encounter from origin game.";
        public static string LEncGiftIVMismatch { get; set; } = "IVs do not match Mystery Gift Data.";
        public static string LEncGiftNicknamed { get; set; } = "Event gift has been nicknamed.";
        public static string LEncGiftNotFound { get; set; } = "Unable to match to a Mystery Gift in the database.";
        public static string LEncGiftPIDMismatch { get; set; } = "Mystery Gift fixed PID mismatch.";
        public static string LEncGiftShinyMismatch { get; set; } = "Mystery Gift shiny mismatch.";
        public static string LEncGiftVersionNotDistributed { get; set; } = "Mystery Gift cannot be received by this version.";

        public static string LEncInvalid { get; set; } = "Unable to match an encounter from origin game.";

        public static string LEncTradeChangedNickname { get; set; } = "Ingame Trade Nickname has been altered.";
        public static string LEncTradeChangedOT { get; set; } = "Ingame Trade OT has been altered.";
        public static string LEncTradeIndexBad { get; set; } = "Ingame Trade invalid index?";
        public static string LEncTradeMatch { get; set; } = "Valid ingame trade.";
        public static string LEncTradeUnchanged { get; set; } = "Ingame Trade OT and Nickname have not been altered.";

        public static string LEncStaticMatch { get; set; } = "Valid gift/static encounter.";
        public static string LEncStaticPIDShiny { get; set; } = "Static Encounter shiny mismatch.";
        public static string LEncStaticRelearn { get; set; } = "Static encounter relearn move mismatch.";

        public static string LEncTypeMatch { get; set; } = "Encounter Type matches encounter.";
        public static string LEncTypeMismatch { get; set; } = "Encounter Type does not match encounter.";
        public static string LEncUnreleased { get; set; } = "Unreleased event.";
        public static string LEncUnreleasedEMewJP { get; set; } = "Non japanese Mew from Faraway Island. Unreleased event.";
        public static string LEncUnreleasedHoOArceus { get; set; } = "Arceus from Hall of Origin. Unreleased event.";
        public static string LEncUnreleasedPtDarkrai { get; set; } = "Non Platinum Darkrai from Newmoon Island. Unreleased event.";
        public static string LEncUnreleasedPtShaymin { get; set; } = "Non Platinum Shaymin from Flower Paradise. Unreleased event.";

        public static string LEReaderAmerica { get; set; } = "American E-Reader Berry in Japanese savegame.";
        public static string LEReaderInvalid { get; set; } = "Invalid E-Reader Berry.";
        public static string LEReaderJapan { get; set; } = "Japanese E-Reader Berry in international savegame.";

        public static string LEffort2Remaining { get; set; } = "2 EVs remaining.";
        public static string LEffortAbove252 { get; set; } = "EVs cannot go above 252.";
        public static string LEffortAbove510 { get; set; } = "EV total cannot be above 510.";
        public static string LEffortAllEqual { get; set; } = "EVs are all equal.";
        public static string LEffortCap100 { get; set; } = "Individual EV for a level 100 encounter in Generation 4 cannot be greater than 100.";
        public static string LEffortEgg { get; set; } = "Eggs cannot receive EVs.";
        public static string LEffortShouldBeZero { get; set; } = "Cannot receive EVs.";
        public static string LEffortEXPIncreased { get; set; } = "All EVs are zero, but leveled above Met Level.";
        public static string LEffortUntrainedCap { get; set; } = "Individual EV without changing EXP cannot be greater than {0}.";

        public static string LEvoInvalid { get; set; } = "Evolution not valid (or level/trade evolution unsatisfied).";
        public static string LEvoTradeReq { get; set; } = "In-game trade {0} should have evolved into {1}.";
        public static string LEvoTradeReqOutsider { get; set; } = "Outsider {0} should have evolved into {1}.";
        public static string LEvoTradeRequired { get; set; } = "Version Specific evolution requires a trade to opposite version. A Handling Trainer is required.";

        public static string LFateful { get; set; } = "Special ingame Fateful Encounter.";
        public static string LFatefulGiftMissing { get; set; } = "Fateful Encounter with no matching Encounter. Has the Mystery Gift data been contributed?";
        public static string LFatefulInvalid { get; set; } = "Fateful Encounter should not be checked.";
        public static string LFatefulMissing { get; set; } = "Special ingame Fateful Encounter flag missing.";
        public static string LFatefulMystery { get; set; } = "Mystery Gift Fateful Encounter.";
        public static string LFatefulMysteryMissing { get; set; } = "Mystery Gift Fateful Encounter flag missing.";

        public static string LFavoriteMarkingUnavailable { get; set; } = "Favorite Marking is not available.";

        public static string LFormArgumentHigh { get; set; } = "Form argument is too high for current form.";
        public static string LFormArgumentLow { get; set; } = "Form argument is too low for current form.";
        public static string LFormArgumentNotAllowed { get; set; } = "Form argument is not allowed for this encounter.";
        public static string LFormArgumentValid { get; set; } = "Form argument is valid.";
        public static string LFormBattle { get; set; } = "Form cannot exist outside of a battle.";
        public static string LFormEternal { get; set; } = "Valid Eternal Flower encounter.";
        public static string LFormEternalInvalid { get; set; } = "Invalid Eternal Flower encounter.";
        public static string LFormInvalidGame { get; set; } = "Form cannot be obtained in origin game.";
        public static string LFormInvalidNature { get; set; } = "Form cannot have this nature.";
        public static string LFormInvalidRange { get; set; } = "Form Count is out of range. Expected <= {0}, got {1}.";
        public static string LFormItem { get; set; } = "Held item matches Form.";
        public static string LFormItemInvalid { get; set; } = "Held item does not match Form.";
        public static string LFormParty { get; set; } = "Form cannot exist outside of Party.";
        public static string LFormPikachuCosplay { get; set; } = "Only Cosplay Pikachu can have this form.";
        public static string LFormPikachuCosplayInvalid { get; set; } = "Cosplay Pikachu cannot have the default form.";
        public static string LFormPikachuEventInvalid { get; set; } = "Event Pikachu cannot have the default form.";
        public static string LFormSafariFlorgesColor { get; set; } = "Friend Safari: Not valid color.";
        public static string LFormSafariPumpkabooAverage { get; set; } = "Friend Safari: Not average sized.";
        public static string LFormSafariSawsbuckSpring { get; set; } = "Friend Safari: Not Spring form.";
        public static string LFormValid { get; set; } = "Form is Valid.";
        public static string LFormVivillon { get; set; } = "Valid Vivillon pattern.";
        public static string LFormVivillonEventPre { get; set; } = "Event Vivillon pattern on pre-evolution.";
        public static string LFormVivillonInvalid { get; set; } = "Invalid Vivillon pattern.";

        public static string LG1CatchRateChain { get; set; } = "Catch rate does not match any species from Pokémon evolution chain.";
        public static string LG1CatchRateEvo { get; set; } = "Catch rate match species without encounters. Expected a preevolution catch rate.";
        public static string LG1CatchRateItem { get; set; } = "Catch rate does not match a valid held item from Generation 2.";
        public static string LG1CatchRateMatchPrevious { get; set; } = "Catch Rate matches a species from Pokémon evolution chain.";
        public static string LG1CatchRateMatchTradeback { get; set; } = "Catch rate matches a valid held item from Generation 2.";
        public static string LG1CatchRateNone { get; set; } = "Catch rate does not match any species from Pokémon evolution chain or any Generation 2 held items.";
        public static string LG1CharNick { get; set; } = "Nickname from Generation 1/2 uses unavailable characters.";
        public static string LG1CharOT { get; set; } = "OT from Generation 1/2 uses unavailable characters.";
        public static string LG1GBEncounter { get; set; } = "Can't obtain Special encounter in Virtual Console games.";
        public static string LG1MoveExclusive { get; set; } = "Generation 1 exclusive move. Incompatible with Non-tradeback moves.";
        public static string LG1MoveLearnSameLevel { get; set; } = "Incompatible moves. Learned at the same level in Red/Blue and Yellow.";
        public static string LG1MoveTradeback { get; set; } = "Non-tradeback Egg move. Incompatible with Generation 1 exclusive moves.";
        public static string LG1OTEvent { get; set; } = "Incorrect RBY event OT Name.";
        public static string LG1OTGender { get; set; } = "Female OT from Generation 1/2 is invalid.";
        public static string LG1Stadium { get; set; } = "Incorrect Stadium OT.";
        public static string LG1StadiumInternational { get; set; } = "Valid International Stadium OT.";
        public static string LG1StadiumJapanese { get; set; } = "Valid Japanese Stadium OT.";
        public static string LG1TradebackPreEvoMove { get; set; } = "Non-tradeback pre evolution move. Incompatible with Generation 1 exclusive moves.";
        public static string LG1Type1Fail { get; set; } = "Invalid Type A, does not match species type.";
        public static string LG1Type2Fail { get; set; } = "Invalid Type B, does not match species type.";
        public static string LG1TypeMatch1 { get; set; } = "Valid Type A, matches species type.";
        public static string LG1TypeMatch2 { get; set; } = "Valid Type B, matches species type.";
        public static string LG1TypeMatchPorygon { get; set; } = "Porygon with valid Type A and B values.";
        public static string LG1TypePorygonFail { get; set; } = "Porygon with invalid Type A and B values. Does not a match a valid type combination.";
        public static string LG1TypePorygonFail1 { get; set; } = "Porygon with invalid Type A value.";
        public static string LG1TypePorygonFail2 { get; set; } = "Porygon with invalid Type B value.";
        public static string LG2InvalidTilePark { get; set; } = "National Park fishing encounter. Unreachable Water tiles.";
        public static string LG2InvalidTileR14 { get; set; } = "Kanto Route 14 fishing encounter. Unreachable Water tiles.";
        public static string LG2InvalidTileSafari { get; set; } = "Generation 2 Safari Zone fishing encounter. Unreachable zone.";
        public static string LG2InvalidTileTreeID { get; set; } = "Found an unreacheable tree for Crystal headbutt encounter that matches OTID.";
        public static string LG2InvalidTileTreeNotFound { get; set; } = "Could not find a tree for Crystal headbutt encounter that matches OTID.";
        public static string LG2TreeID { get; set; } = "Found a tree for Crystal headbutt encounter that matches OTID.";
        public static string LG2OTGender { get; set; } = "OT from Virtual Console games other than Crystal cannot be female.";

        public static string LG3EReader { get; set; } = "Non Japanese Shadow E-reader Pokémon. Unreleased encounter.";
        public static string LG3OTGender { get; set; } = "OT from Colosseum/XD cannot be female.";
        public static string LG4InvalidTileR45Surf { get; set; } = "Johto Route 45 surfing encounter. Unreachable Water tiles.";
        public static string LG5ID_N { get; set; } = "The Name/TID/SID of N is incorrect.";
        public static string LG5IVAll30 { get; set; } = "All IVs of N's Pokémon should be 30.";
        public static string LG5OTGenderN { get; set; } = "N's Pokémon must have a male OT gender.";
        public static string LG5PIDShinyGrotto { get; set; } = "Hidden Grotto captures cannot be shiny.";
        public static string LG5PIDShinyN { get; set; } = "N's Pokémon cannot be shiny.";
        public static string LG5SparkleInvalid { get; set; } = "Special ingame N's Sparkle flag should not be checked.";
        public static string LG5SparkleRequired { get; set; } = "Special ingame N's Sparkle flag missing.";

        public static string LGenderInvalidNone { get; set; } = "Genderless Pokémon should not have a gender.";
        public static string LGeoBadOrder { get; set; } = "GeoLocation Memory: Gap/Blank present.";
        public static string LGeoHardwareInvalid { get; set; } = "Geolocation: Country is not in 3DS region.";
        public static string LGeoHardwareRange { get; set; } = "Invalid Console Region.";
        public static string LGeoHardwareValid { get; set; } = "Geolocation: Country is in 3DS region.";
        public static string LGeoMemoryMissing { get; set; } = "GeoLocation Memory: Memories should be present.";
        public static string LGeoNoCountryHT { get; set; } = "GeoLocation Memory: HT Name present but has no previous Country.";
        public static string LGeoNoRegion { get; set; } = "GeoLocation Memory: Region without Country.";

        public static string LHyperBelow100 { get; set; } = "Can't Hyper Train a Pokémon that isn't level 100.";
        public static string LHyperPerfectAll { get; set; } = "Can't Hyper Train a Pokémon with perfect IVs.";
        public static string LHyperPerfectOne { get; set; } = "Can't Hyper Train a perfect IV.";

        public static string LItemEgg { get; set; } = "Eggs cannot hold items.";
        public static string LItemUnreleased { get; set; } = "Held item is unreleased.";

        public static string LIVAllEqual_0 { get; set; } = "All IVs are {0}.";
        public static string LIVNotCorrect { get; set; } = "IVs do not match encounter requirements.";
        public static string LIVF_COUNT0_31 { get; set; } = "Should have at least {0} IVs = 31.";

        public static string LLevelEXPThreshold { get; set; } = "Current experience matches level threshold.";
        public static string LLevelMetBelow { get; set; } = "Current level is below met level.";
        public static string LLevelMetGift { get; set; } = "Met Level does not match Mystery Gift level.";
        public static string LLevelMetGiftFail { get; set; } = "Current Level below Mystery Gift level.";
        public static string LLevelMetSane { get; set; } = "Current level is not below met level.";

        public static string LMemoryArgBadCatch { get; set; } = "{0} Memory: {0} did not catch this.";
        public static string LMemoryArgBadHatch { get; set; } = "{0} Memory: {0} did not hatch this.";
        public static string LMemoryArgBadHT { get; set; } = "Memory: Can't have Handling Trainer Memory as Egg.";
        public static string LMemoryArgBadID { get; set; } = "{0} Memory: Can't obtain Memory on {0} Version.";
        public static string LMemoryArgBadItem { get; set; } = "{0} Memory: Species can't hold this item.";
        public static string LMemoryArgBadLocation { get; set; } = "{0} Memory: Can't obtain Location on {0} Version.";
        public static string LMemoryArgBadMove { get; set; } = "{0} Memory: Species can't learn this move.";
        public static string LMemoryArgBadOTEgg { get; set; } = "{0} Memory: Link Trade is not a valid first memory.";
        public static string LMemoryArgBadSpecies { get; set; } = "{0} Memory: Can't capture species in game.";
        public static string LMemoryArgSpecies { get; set; } = "{0} Memory: Species can be captured in game.";
        public static string LMemoryCleared { get; set; } = "Memory: Not cleared properly.";
        public static string LMemoryF_0_Valid { get; set; } = "{0} Memory is valid.";
        public static string LMemoryFeelInvalid { get; set; } = "{0} Memory: Invalid Feeling.";
        public static string LMemoryHTFlagInvalid { get; set; } = "Untraded: Current handler should not be the Handling Trainer.";
        public static string LMemoryHTGender { get; set; } = "HT Gender invalid: {0}";
        public static string LMemoryHTLanguage { get; set; } = "HT Language is missing.";

        public static string LMemoryIndexArgHT { get; set; } = "Should have a HT Memory TextVar value (somewhere).";
        public static string LMemoryIndexFeel { get; set; } = "{0} Memory: Feeling should be index {1}.";
        public static string LMemoryIndexFeelHT09 { get; set; } = "Should have a HT Memory Feeling value 0-9.";
        public static string LMemoryIndexID { get; set; } = "{0} Memory: Should be index {1}.";
        public static string LMemoryIndexIntensity { get; set; } = "{0} Memory: Intensity should be index {1}.";
        public static string LMemoryIndexIntensityHT1 { get; set; } = "Should have a HT Memory Intensity value (1st).";
        public static string LMemoryIndexIntensityMin { get; set; } = "{0} Memory: Intensity should be at least {1}.";
        public static string LMemoryIndexLinkHT { get; set; } = "Should have a Link Trade HT Memory.";
        public static string LMemoryIndexVar { get; set; } = "{0} Memory: TextVar should be index {1}.";
        public static string LMemoryMissingHT { get; set; } = "Memory: Handling Trainer Memory missing.";
        public static string LMemoryMissingOT { get; set; } = "Memory: Original Trainer Memory missing.";

        public static string LMemoryStatAffectionHT0 { get; set; } = "Untraded: Handling Trainer Affection should be 0.";
        public static string LMemoryStatAffectionOT0 { get; set; } = "OT Affection should be 0.";
        public static string LMemoryStatFriendshipHT0 { get; set; } = "Untraded: Handling Trainer Friendship should be 0.";
        public static string LMemoryStatFriendshipOTBaseEvent { get; set; } = "Event OT Friendship does not match base friendship.";

        public static string LMemoryStatFullness { get; set; } = "Fullness should be {0}.";
        public static string LMemoryStatEnjoyment { get; set; } = "Enjoyment should be {0}.";

        public static string LMoveEggFIncompatible0_1 { get; set; } = "{0} Inherited Move. Incompatible with {1} inherited moves.";
        public static string LMoveEggIncompatible { get; set; } = "Egg Move. Incompatible with event Egg moves.";
        public static string LMoveEggIncompatibleEvent { get; set; } = "Event Egg Move. Incompatible with normal Egg moves.";
        public static string LMoveEggInherited { get; set; } = "Inherited Egg move.";
        public static string LMoveEggInheritedTutor { get; set; } = "Inherited tutor move.";
        public static string LMoveEggInvalid { get; set; } = "Not an expected Egg move.";
        public static string LMoveEggInvalidEvent { get; set; } = "Egg Move. Not expected in an event Egg.";
        public static string LMoveEggInvalidEventLevelUp { get; set; } = "Inherited move learned by Level-up. Not expected in an event Egg.";
        public static string LMoveEggInvalidEventLevelUpGift { get; set; } = "Inherited move learned by Level-up. Not expected in a gift Egg.";
        public static string LMoveEggInvalidEventTMHM { get; set; } = "Inherited TM/HM move. Not expected in an event Egg.";
        public static string LMoveEggInvalidEventTutor { get; set; } = "Inherited tutor move. Not expected in an event Egg.";
        public static string LMoveEggLevelUp { get; set; } = "Inherited move learned by Level-up.";
        public static string LMoveEggMissing { get; set; } = "Event Egg move missing.";
        public static string LMoveEggMoveGift { get; set; } = "Egg Move. Not expected in a gift Egg.";
        public static string LMoveEggTMHM { get; set; } = "Inherited TM/HM move.";

        public static string LMoveEventEggLevelUp { get; set; } = "Inherited move learned by Level-up. Incompatible with event Egg moves.";
        public static string LMoveEvoFCombination_0 { get; set; } = "Moves combinations is not compatible with {0} evolution.";
        public static string LMoveEvoFHigher { get; set; } = "Incompatible evolution moves. {1} Move learned at a higher level than other {0} moves.";
        public static string LMoveEvoFLower { get; set; } = "Incompatible evolution moves. {0} Move learned at a lower level than other {1} moves.";
        public static string LMoveFDefault_0 { get; set; } = "Default move in Generation {0}.";
        public static string LMoveFExpect_0 { get; set; } = "Expected the following Moves: {0}";
        public static string LMoveFLevelUp_0 { get; set; } = "Learned by Level-up in Generation {0}.";
        public static string LMoveFTMHM_0 { get; set; } = "Learned by TM/HM in Generation {0}.";
        public static string LMoveFTutor_0 { get; set; } = "Learned by Move Tutor in Generation {0}.";
        public static string LMoveKeldeoMismatch { get; set; } = "Keldeo Move/Form mismatch.";
        public static string LMoveNincada { get; set; } = "Only one Ninjask move allowed.";
        public static string LMoveNincadaEvo { get; set; } = "Learned by evolving Nincada into Ninjask.";
        public static string LMoveNincadaEvoF_0 { get; set; } = "Learned by evolving Nincada into Ninjask in Generation {0}.";
        public static string LMovePPTooHigh_0 { get; set; } = "Move {0} PP is above the amount allowed.";
        public static string LMoveSourceShared { get; set; } = "Shared Non-Relearn Move.";
        public static string LMoveSourceSharedF { get; set; } = "Shared Non-Relearn Move in Generation {0}.";

        public static string LMoveRelearnDexNav { get; set; } = "Not an expected DexNav move.";
        public static string LMoveRelearnEgg { get; set; } = "Base Egg move.";
        public static string LMoveRelearnEggMissing { get; set; } = "Base Egg move missing.";
        public static string LMoveRelearnFExpect_0 { get; set; } = "Expected the following Relearn Moves: {0}";
        public static string LMoveRelearnFMiss_0 { get; set; } = "Relearn Moves missing: {0}";
        public static string LMoveRelearnInvalid { get; set; } = "Not an expected Relearnable move.";
        public static string LMoveRelearnNone { get; set; } = "Expected no Relearn Move in slot.";

        public static string LMoveSourceDefault { get; set; } = "Default move.";
        public static string LMoveSourceDuplicate { get; set; } = "Duplicate Move.";
        public static string LMoveSourceEgg { get; set; } = "Egg Move.";
        public static string LMoveSourceEggEvent { get; set; } = "Event Egg Move.";
        public static string LMoveSourceEmpty { get; set; } = "Empty Move.";
        public static string LMoveSourceInvalid { get; set; } = "Invalid Move.";
        public static string LMoveSourceInvalidSketch { get; set; } = "Invalid Move (Sketch).";
        public static string LMoveSourceLevelUp { get; set; } = "Learned by Level-up.";
        public static string LMoveSourceRelearn { get; set; } = "Relearnable Move.";
        public static string LMoveSourceSpecial { get; set; } = "Special Non-Relearn Move.";
        public static string LMoveSourceTMHM { get; set; } = "Learned by TM/HM.";
        public static string LMoveSourceTutor { get; set; } = "Learned by Move Tutor.";
        public static string LMoveSourceTR { get; set; } = "Unexpected Technical Record Learned flag: {0}";

        public static string LNickFlagEggNo { get; set; } = "Egg must be not nicknamed.";
        public static string LNickFlagEggYes { get; set; } = "Egg must be nicknamed.";
        public static string LNickInvalidChar { get; set; } = "Cannot be given this Nickname.";
        public static string LNickLengthLong { get; set; } = "Nickname too long.";
        public static string LNickLengthShort { get; set; } = "Nickname is empty.";
        public static string LNickMatchLanguage { get; set; } = "Nickname matches species name.";
        public static string LNickMatchLanguageEgg { get; set; } = "Egg matches language Egg name.";
        public static string LNickMatchLanguageEggFail { get; set; } = "Egg name does not match language Egg name.";
        public static string LNickMatchLanguageFail { get; set; } = "Nickname does not match species name.";
        public static string LNickMatchLanguageFlag { get; set; } = "Nickname flagged, matches species name.";
        public static string LNickMatchNoOthers { get; set; } = "Nickname does not match another species name.";
        public static string LNickMatchNoOthersFail { get; set; } = "Nickname matches another species name (+language).";

        public static string LOTLanguage { get; set; } = "Language ID should be {0}, not {1}.";
        public static string LOTLong { get; set; } = "OT Name too long.";
        public static string LOTShort { get; set; } = "OT Name too short.";
        public static string LOTSuspicious { get; set; } = "Suspicious Original Trainer details.";

        public static string LOT_IDEqual { get; set; } = "TID and SID are equal.";
        public static string LOT_IDs0 { get; set; } = "TID and SID are 0.";
        public static string LOT_SID0 { get; set; } = "SID is zero.";
        public static string LOT_SID0Invalid { get; set; } = "SID should be 0.";
        public static string LOT_TID0 { get; set; } = "TID is zero.";

        public static string LPIDEncryptWurmple { get; set; } = "Wurmple evolution Encryption Constant mismatch.";
        public static string LPIDEncryptZero { get; set; } = "Encryption Constant is not set.";
        public static string LPIDEqualsEC { get; set; } = "Encryption Constant matches PID.";
        public static string LPIDGenderMatch { get; set; } = "Gender matches PID.";
        public static string LPIDGenderMismatch { get; set; } = "PID-Gender mismatch.";
        public static string LPIDNatureMatch { get; set; } = "Nature matches PID.";
        public static string LPIDNatureMismatch { get; set; } = "PID-Nature mismatch.";
        public static string LPIDTypeMismatch { get; set; } = "Encounter Type PID mismatch.";
        public static string LPIDZero { get; set; } = "PID is not set.";

        public static string LRibbonAllValid { get; set; } = "All ribbons accounted for.";
        public static string LRibbonEgg { get; set; } = "Can't receive Ribbon(s) as an Egg.";
        public static string LRibbonFInvalid_0 { get; set; } = "Invalid Ribbons: {0}";
        public static string LRibbonFMissing_0 { get; set; } = "Missing Ribbons: {0}";
        public static string LRibbonMarkingFInvalid_0 { get; set; } = "Invalid Marking: {0}";
        public static string LRibbonMarkingAffixedF_0 { get; set; } = "Invalid Affixed Ribbon/Marking: {0}";

        public static string LStatDynamaxInvalid { get; set; } = "Dynamax Level is not within the expected range.";
        public static string LStatIncorrectHeight { get; set; } = "Calculated Height does not match stored value.";
        public static string LStatIncorrectWeight { get; set; } = "Calculated Weight does not match stored value.";
        public static string LStatIncorrectCP { get; set; } = "Calculated CP does not match stored value.";
        public static string LStatGigantamaxInvalid { get; set; } = "Gigantamax Flag mismatch.";
        public static string LStatGigantamaxValid { get; set; } = "Gigantamax Flag was changed via Max Soup.";
        public static string LStatNatureInvalid { get; set; } = "Stat Nature is not within the expected range.";
        public static string LStatBattleVersionInvalid { get; set; } = "Battle Version is not within the expected range.";

        public static string LSuperComplete { get; set; } = "Super Training complete flag mismatch.";
        public static string LSuperDistro { get; set; } = "Distribution Super Training missions are not released.";
        public static string LSuperEgg { get; set; } = "Can't Super Train an Egg.";
        public static string LSuperNoComplete { get; set; } = "Can't have active Super Training complete flag for origins.";
        public static string LSuperNoUnlocked { get; set; } = "Can't have active Super Training unlocked flag for origins.";
        public static string LSuperUnavailable { get; set; } = "Super Training missions are not available in games visited.";
        public static string LSuperUnused { get; set; } = "Unused Super Training Flag is flagged.";

        public static string LTransferBad { get; set; } = "Incorrectly transferred from previous generation.";
        public static string LTransferEgg { get; set; } = "Can't transfer Eggs between Generations.";
        public static string LTransferEggLocationTransporter { get; set; } = "Invalid Met Location, expected Poké Transfer.";
        public static string LTransferEggMetLevel { get; set; } = "Invalid Met Level for transfer.";
        public static string LTransferHTFlagRequired { get; set; } = "Current handler cannot be past gen OT for transferred specimen.";
        public static string LTransferMet { get; set; } = "Invalid Met Location, expected Poké Transfer or Crown.";
        public static string LTransferMetLocation { get; set; } = "Invalid Transfer Met Location.";
        public static string LTransferMove { get; set; } = "Incompatible transfer move.";
        public static string LTransferMoveG4HM { get; set; } = "Defog and Whirlpool. One of the two moves should have been removed before transferred to Generation 5.";
        public static string LTransferMoveHM { get; set; } = "Generation {0} HM. Should have been removed before transferred to Generation {1}.";
        public static string LTransferNature { get; set; } = "Invalid Nature for transfer Experience.";
        public static string LTransferOriginFInvalid0_1 { get; set; } = "{0} origin cannot exist in the currently loaded ({1}) savegame.";
        public static string LTransferPIDECBitFlip { get; set; } = "PID should be equal to EC [with top bit flipped]!";
        public static string LTransferPIDECEquals { get; set; } = "PID should be equal to EC!";
        public static string LTransferPIDECXor { get; set; } = "Encryption Constant matches shinyxored PID.";
        public static string LTransferTrackerMissing { get; set; } = "Pokémon HOME Transfer Tracker is missing.";
        public static string LTransferTrackerShouldBeZero { get; set; } = "Pokémon HOME Transfer Tracker should be 0.";
        #endregion

    }
}
