// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace PKHeX.Core
{
    public static class LegalityCheckStrings
    {

        #region General Strings

        /// <summary>Default text for indicating validity.</summary>
        public static string V {get; set;} = "Valid.";
        /// <summary>Default text for indicating legality.</summary>
        public static string V193 {get; set;} = "Legal!";
        /// <summary>Default text for indicating an error has occurred.</summary>
        public static string V190 {get; set;} = "Internal error.";
        /// <summary>Analysis not available for the <see cref="PKM"/></summary>
        public static string V189 {get; set;} = "Analysis not available for this Pokémon.";
        /// <summary>Format text for exporting a legality check result.</summary>
        public static string V196 {get; set;} = "{0}: {1}";
        /// <summary>Format text for exporting a legality check result for an invalid Move.</summary>
        public static string V191 {get; set;} = "{0} Move {1}: {2}";
        /// <summary>Format text for exporting a legality check result for an invalid Relearn Move.</summary>
        public static string V192 {get; set;} = "{0} Relearn Move {1}: {2}";
        /// <summary>Format text for exporting the type of Encounter that was matched for the the <see cref="PKM"/></summary>
        public static string V195 {get; set;} = "Encounter Type: {0}";

        /// <summary>Severity string for <see cref="Severity.Indeterminate"/></summary>
        public static string V500 { get; set; } = "Indeterminate";
        /// <summary>Severity string for <see cref="Severity.Invalid"/></summary>
        public static string V501 { get; set; } = "Invalid";
        /// <summary>Severity string for <see cref="Severity.Fishy"/></summary>
        public static string V502 { get; set; } = "Fishy";
        /// <summary>Severity string for <see cref="Severity.Valid"/></summary>
        public static string V503 { get; set; } = "Valid";
        /// <summary>Severity string for <see cref="Severity.NotImplemented"/></summary>
        public static string V504 { get; set; } = "Not Implemented";

        /// <summary>Original Trainer string used in various string formats such as Memories.</summary>
        public static string V205 { get; set; } = "OT";
        /// <summary>Handling Trainer string used in various string formats such as Memories.</summary>
        public static string V206 { get; set; } = "HT";

        public static string V167 { get; set; } = "Empty Move.";
        public static string V171 { get; set; } = "Egg Move.";
        public static string V172 { get; set; } = "Relearnable Move.";
        public static string V173 { get; set; } = "Learned by TM/HM.";
        public static string V174 { get; set; } = "Learned by Move Tutor.";
        public static string V175 { get; set; } = "Special Non-Relearn Move.";
        public static string V177 { get; set; } = "Learned by Level-up.";
        public static string V330 { get; set; } = "Learned by Level-up in generation {0}.";
        public static string V331 { get; set; } = "Learned by TM/HM in generation {0}.";
        public static string V332 { get; set; } = "Learned by Move Tutor in generation {0}.";
        public static string V333 { get; set; } = "Event Egg Move.";
        public static string V344 { get; set; } = "Inherited egg move.";
        public static string V345 { get; set; } = "Inherited move learned by Level-up.";
        public static string V346 { get; set; } = "Inherited tutor move.";
        public static string V349 { get; set; } = "Inherited TM/HM move.";
        public static string V355 { get; set; } = "Learned by evolving Nincada into Ninjask.";
        public static string V356 { get; set; } = "Learned by evolving Nincada into Ninjask in generation {0}.";
        public static string V361 { get; set; } = "Default move.";
        public static string V362 { get; set; } = "Default move in generation {0}.";
        public static string V372 { get; set; } = "{0} Berry";

        #endregion

        #region Legality Check Result Strings

        public static string V203 {get; set;} = "Genderless Pokémon should not have a gender.";
        public static string V201 {get; set;} = "Encryption Constant is not set.";
        public static string V204 {get; set;} = "Held item is unreleased.";

        public static string V187 {get; set;} = "Species does not exist in origin game.";
        public static string V188 {get; set;} = "Fateful Encounter with no matching Encounter. Has the Mystery Gift data been contributed?";
        public static string V194 { get; set;} = "Ingame Trade for Sun/Moon not implemented."; // Valid

        public static string V250 {get; set;} = "Gender matches PID.";
        public static string V251 {get; set;} = "PID-Gender mismatch.";
        public static string V252 {get; set;} = "Nature matches PID.";
        public static string V253 {get; set;} = "PID-Nature mismatch.";
        public static string V207 {get; set;} = "PID is not set.";
        public static string V208 {get; set;} = "Encryption Constant matches PID.";
        public static string V209 {get; set;} = "Static Encounter shiny mismatch.";
        public static string V210 {get; set;} = "Wurmple evolution Encryption Constant mismatch.";
        public static string V211 {get; set;} = "Encryption Constant matches shinyxored PID.";
        public static string V212 {get; set;} = "Wurmple Evolution: {0}";
        public static string V215 {get; set;} = "PID should be equal to EC [with top bit flipped]!";
        public static string V216 {get; set;} = "PID should be equal to EC!";
        public static string V220 {get; set;} = "N's Pokemon cannot be shiny.";
        public static string V221 {get; set;} = "Hidden Grotto captures cannot be shiny.";

        public static string V14 {get; set;} = "Egg matches language Egg name."; // Valid
        public static string V17 {get; set;} = "Nickname does not match another species name."; // Valid
        public static string V18 {get; set;} = "Nickname matches species name."; // Valid
        public static string V19 {get; set;} = "Nickname matches demo language name."; // Valid
        public static string V11 {get; set;} = "Ingame Trade OT and Nickname have not been altered."; // Valid
        public static string V1 {get; set;} = "Nickname too long."; // Invalid
        public static string V2 {get; set;} = "Nickname is empty."; // Invalid
        public static string V4 {get; set;} = "Language ID > 8."; // Invalid
        public static string V5 {get; set;} = "Language ID > 10."; // Invalid
        public static string V222 { get; set; } = "Cannot be given this Nickname."; // Invalid
        public static string V3 {get; set;} = "Species index invalid for Nickname comparison."; // Invalid
        public static string V20 {get; set;} = "Nickname does not match species name."; // Invalid
        public static string V13 {get; set;} = "Egg name does not match language Egg name."; // Invalid
        public static string V12 {get; set;} = "Eggs must be nicknamed."; // Invalid
        public static string V7 {get; set;} = "Ingame Trade invalid version?"; // Invalid
        public static string V8 {get; set;} = "Ingame Trade invalid index?"; // Invalid
        public static string V10 {get; set;} = "Ingame Trade OT has been altered."; // Invalid
        public static string V9 {get; set;} = "Ingame Trade Nickname has been altered."; // Fishy
        public static string V15 {get; set;} = "Nickname matches another species name (+language)."; // Fishy
        public static string V16 {get; set;} = "Nickname flagged, matches species name."; // Fishy
        public static string V0 {get; set;} = "Event gift has been nicknamed."; // Fishy

        public static string V21 {get; set;} = "Matches: {0} {1}"; // Valid

        public static string V25 {get; set;} = "EV total cannot be above 510."; // Invalid
        public static string V22 {get; set;} = "Eggs cannot receive EVs."; // Invalid
        public static string V23 {get; set;} = "All EVs are zero, but leveled above Met Level."; // Fishy
        public static string V24 {get; set;} = "2 EVs remaining."; // Fishy
        public static string V26 {get; set;} = "EVs cannot go above 252."; // Invalid
        public static string V27 {get; set;} = "EVs are all equal."; // Fishy
        public static string V31 {get; set;} = "All IVs are 0."; // Fishy
        public static string V32 {get; set;} = "All IVs are equal."; // Fishy
        public static string V218 {get; set;} = "All IVs of N's Pokemon should be 30."; // Invalid

        public static string V28 {get; set;} = "Should have at least {0} IVs = 31."; // Invalid
        public static string V29 {get; set;} = "Friend Safari captures should have at least 2 IVs = 31."; // Invalid
        public static string V30 {get; set;} = "IVs do not match Mystery Gift Data."; // Invalid

        public static string V38 {get; set;} = "OT Name too long."; // Invalid
        public static string V39 {get; set;} = "Incorrect RBY event OT Name."; // Invalid
        public static string V34 {get; set;} = "SID should be 0."; // Invalid
        public static string V219 {get; set;} = "The Name/TID/SID of N is incorrect."; // Invalid
        public static string V33 {get; set;} = "TID and SID are 0."; // Fishy
        public static string V35 {get; set;} = "TID and SID are equal."; // Fishy
        public static string V36 {get; set;} = "TID is zero."; // Fishy
        public static string V37 {get; set;} = "SID is zero."; // Fishy

        public static string V40 {get; set;} = "Can't Hyper Train a pokemon that isn't level 100."; // Invalid
        public static string V41 {get; set;} = "Can't Hyper Train a pokemon with perfect IVs."; // Invalid
        public static string V42 {get; set;} = "Can't Hyper Train a perfect IV."; // Invalid

        public static string V49 {get; set;} = "Valid Pokémon Link gift."; // Valid
        public static string V47 {get; set;} = "Pokémon Link gift Shiny mismatch."; // Invalid
        public static string V48 {get; set;} = "Pokémon Link gift should not be Fateful Encounter."; // Invalid
        public static string V43 {get; set;} = "Can't find matching Pokémon Link gift."; // Invalid
        public static string V44 {get; set;} = "Can't obtain this Pokémon Link gift in XY."; // Invalid
        public static string V45 {get; set;} = "Can't obtain this Pokémon Link gift in ORAS."; // Invalid
        public static string V46 {get; set;} = "Can't obtain this Pokémon Link gift in SM."; // Invalid

        public static string V63 {get; set;} = "Valid un-hatched egg."; // Valid
        public static string V53 {get; set;} = "Able to hatch an egg at Met Location."; // Valid
        public static string V56 {get; set;} = "Able to hatch a traded egg at Met Location.";
        public static string V54 {get; set;} = "Can't hatch an egg at Met Location."; // Invalid
        public static string V55 {get; set;} = "Can't obtain egg from Egg Location."; // Invalid
        public static string V57 {get; set;} = "Can't transfer eggs between generations."; // Invalid
        public static string V50 {get; set;} = "Can't obtain egg for this species."; // Invalid
        public static string V51 {get; set;} = "Invalid Met Location for hatched egg."; // Invalid
        public static string V52 {get; set;} = "Invalid Met Level, expected {0}."; // Invalid
        public static string V58 {get; set;} = "Invalid Met Level for transfer."; // Invalid
        public static string V59 {get; set;} = "Invalid Egg Location, expected none."; // Invalid
        public static string V60 {get; set;} = "Invalid Met Location, expected Pal Park."; // Invalid
        public static string V61 {get; set;} = "Invalid Met Location, expected Transporter."; // Invalid
        public static string V62 {get; set;} = "Invalid Egg Location, shouldn't be 'traded' while an egg."; // Invalid

        public static string V66 {get; set;} = "Valid Friend Safari encounter."; // Valid
        public static string V64 {get; set;} = "Friend Safari: Not valid color."; // Florges
        public static string V6 {get; set;} = "Friend Safari: Not average sized."; // Pumpkaboo
        public static string V65 {get; set;} = "Friend Safari: Not Spring form."; // Sawsbuck

        public static string V67 {get; set;} = "Valid Wild Encounter at location (Pressure/Hustle/Vital Spirit).";
        public static string V68 {get; set;} = "Valid Wild Encounter at location.";
        public static string V69 {get; set;} = "Valid Wild Encounter at location (White Flute & Pressure/Hustle/Vital Spirit).";
        public static string V70 {get; set;} = "Valid Wild Encounter at location (White Flute).";
        public static string V71 {get; set;} = "Valid Wild Encounter at location (Black Flute & Pressure/Hustle/Vital Spirit).";
        public static string V72 {get; set;} = "Valid Wild Encounter at location (Black Flute).";
        public static string V73 {get; set;} = "Valid Wild Encounter at location (DexNav).";

        public static string V76 {get; set;} = "Valid ingame trade.";
        public static string V75 {get; set;} = "Valid gift/static encounter."; // Valid
        public static string V74 {get; set;} = "Static encounter relearn move mismatch.";

        public static string V77 {get; set;} = "Can't obtain Species from Virtual Console games."; // Invalid
        public static string V79 {get; set;} = "Can't obtain Special encounter in Virtual Console games."; // Invalid
        public static string V78 {get; set;} = "Unable to match to a Mystery Gift in the database."; // Invalid
        public static string V80 {get; set;} = "Unable to match an encounter from origin game."; // Invalid
        public static string V81 {get; set;} = "Invalid Transfer Met Location."; // Invalid
        public static string V82 {get; set;} = "Mewtwo cannot be transferred while knowing Pay Day."; // Invalid

        public static string V88 {get; set;} = "Current level is not below met level.";
        public static string V83 {get; set;} = "Met Level does not match Mystery Gift level.";
        public static string V84 {get; set;} = "Current Level below Mystery Gift level.";
        public static string V85 {get; set;} = "Current level is below met level.";
        public static string V86 {get; set;} = "Evolution not valid (or level/trade evolution unsatisfied).";
        public static string V87 {get; set;} = "Current experience matches level threshold."; // Fishy

        public static string V89 {get; set;} = "Can't Super Train an egg."; // Invalid
        public static string V90 {get; set;} = "Super Training missions are not available in games visited.";
        public static string V91 {get; set;} = "Can't have active Super Training unlocked flag for origins.";
        public static string V92 {get; set;} = "Can't have active Super Training complete flag for origins.";
        public static string V93 {get; set;} = "Super Training complete flag mismatch.";
        public static string V94 {get; set;} = "Distribution Super Training missions are not released."; // Fishy

        public static string V95 {get; set;} = "Can't receive Ribbon(s) as an egg.";
        public static string V96 {get; set;} = "GBA Champion Ribbon";
        public static string V97 {get; set;} = "Artist Ribbon";
        public static string V98 {get; set;} = "National Ribbon (Purified)";
        public static string V99 {get; set;} = "Sinnoh Champion Ribbon";
        public static string V100 {get; set;} = "Legend Ribbon";
        public static string V104 {get; set;} = "Record Ribbon";
        public static string V101 {get; set;} = "Missing Ribbons: {0}";
        public static string V102 {get; set;} = "Invalid Ribbons: {0}";
        public static string V103 {get; set;} = "All ribbons accounted for.";
        public static string V105 {get; set;} = "Battle Memory Ribbon";
        public static string V106 {get; set;} = "Contest Memory Ribbon";

        public static string V107 {get; set;} = "Ability is not valid for species/form.";
        public static string V108 {get; set;} = "Hidden Ability mismatch for encounter type.";
        public static string V223 {get; set;} = "Ability mismatch for encounter.";
        public static string V109 {get; set;} = "Ability modified with Ability Capsule.";
        public static string V110 {get; set;} = "Ability does not match Mystery Gift.";
        public static string V111 {get; set;} = "Hidden Ability on non-SOS wild encounter.";
        public static string V300 {get; set;} = "Hidden Ability on non-horde/friend safari wild encounter.";
        public static string V112 {get; set;} = "Hidden Ability not available.";
        public static string V217 {get; set;} = "Hidden Grotto captures should have Hidden Ability.";

        public static string V115 {get; set;} = "Ability matches ability number."; // Valid
        public static string V113 {get; set;} = "Ability does not match PID.";
        public static string V114 {get; set;} = "Ability does not match ability number.";

        public static string V119 {get; set;} = "Correct ball for encounter type.";
        public static string V118 {get; set;} = "Can't have ball for encounter type.";
        public static string V116 {get; set;} = "Can't have Heavy Ball for light, low-catch rate species (Gen VII).";
        public static string V117 {get; set;} = "Can't have Master Ball for regular egg.";
        public static string V120 {get; set;} = "Can't have Cherish Ball for regular egg.";
        public static string V121 {get; set;} = "Can't obtain species in Ball.";
        public static string V122 {get; set;} = "Can't obtain Hidden Ability with Ball.";
        public static string V123 {get; set;} = "Ball possible for species.";
        public static string V125 {get; set;} = "No check satisfied, assuming illegal.";
        public static string V126 {get; set;} = "Ball unobtainable in origin generation.";

        public static string V145 {get; set;} = "History block is valid.";
        public static string V155 {get; set;} = "{0} Memory is valid.";

        public static string V127 {get; set;} = "Skipped History check due to other check being invalid.";
        public static string V128 {get; set;} = "No History Block to check.";
        public static string V129 {get; set;} = "OT Affection should be 0.";
        public static string V130 {get; set;} = "Can't have any OT Memory.";
        public static string V124 {get; set;} = "Current handler cannot be past gen OT for transferred specimen.";
        public static string V131 {get; set;} = "HT Gender invalid: {0}";
        public static string V132 {get; set;} = "Event OT Friendship does not match base friendship.";
        public static string V133 {get; set;} = "Event OT Affection should be zero.";
        public static string V134 {get; set;} = "Current handler should not be Event OT.";
        public static string V138 {get; set;} = "Contest Stats should be 0.";
        public static string V301 {get; set;} = "Invalid Console Region.";
        public static string V302 {get; set;} = "Geolocation: Country is not in 3DS region.";
        public static string V303 {get; set;} = "Geolocation: Country is in 3DS region.";
        public static string V137 {get; set;} = "GeoLocation Memory: Memories should be present.";
        public static string V135 {get; set;} = "GeoLocation Memory: Gap/Blank present.";
        public static string V136 {get; set;} = "GeoLocation Memory: Region without Country.";
        public static string V146 {get; set;} = "GeoLocation Memory: HT Name present but has no previous Country.";
        public static string V147 {get; set;} = "GeoLocation Memory: Previous country of residence present with no Handling Trainer.";
        public static string V139 {get; set;} = "Untraded: Current handler should not be the Handling Trainer.";
        public static string V140 {get; set;} = "Untraded: Handling Trainer Friendship should be 0.";
        public static string V141 {get; set;} = "Untraded: Handling Trainer Affection should be 0.";
        public static string V142 {get; set;} = "Untraded: Requires a trade evolution.";
        public static string V143 {get; set;} = "Untraded: Beauty is not high enough for Level-up Evolution.";
        public static string V144 {get; set;} = "Untraded: Beauty is high enough but still Level 1.";
        public static string V148 {get; set;} = "Memory: Handling Trainer Memory present with no Handling Trainer name.";
        public static string V150 {get; set;} = "Memory: Handling Trainer Memory missing.";
        public static string V152 {get; set;} = "Memory: Original Trainer Memory missing.";
        public static string V329 {get; set;} = "Memory: Not cleared properly.";
        public static string V149 {get; set;} = "Memory: Can't have Handling Trainer Memory as egg.";
        public static string V151 {get; set;} = "Memory: Can't have Original Trainer Memory as egg.";
        public static string V164 {get; set;} = "{0} Memory: Species can be captured in game.";
        public static string V153 {get; set;} = "{0} Memory: Species can't learn this move.";
        public static string V154 {get; set;} = "{0} Memory: Location doesn't have a Pokemon Center.";
        public static string V160 {get; set;} = "{0} Memory: {0} did not hatch this.";
        public static string V202 {get; set;} = "{0} Memory: {0} did not catch this.";
        public static string V161 {get; set;} = "{0} Memory: Link Trade is not a valid first memory.";
        public static string V162 {get; set;} = "{0} Memory: Can't obtain Location on {0} Version.";
        public static string V163 {get; set;} = "{0} Memory: Can't obtain Memory on {0} Version.";
        public static string V165 {get; set;} = "{0} Memory: Can't capture species in game.";
        public static string V197 {get; set;} = "{0} Memory: Should be index {1}.";
        public static string V198 {get; set;} = "{0} Memory: Intensity should be index {1}.";
        public static string V199 {get; set;} = "{0} Memory: TextVar should be index {1}.";
        public static string V200 {get; set;} = "{0} Memory: Feeling should be index {1}.";

        public static string V168 {get; set;} = "Duplicate Move.";
        public static string V176 {get; set;} = "Invalid Move.";
        public static string V166 {get; set;} = "Invalid Move (Sketch).";
        public static string V169 {get; set;} = "Keldeo Move/Form mismatch.";
        public static string V181 {get; set;} = "Expected the following Relearn Moves: {0}";
        public static string V170 {get; set;} = "Relearn Moves missing: {0}";
        public static string V178 {get; set;} = "Expected: {0}.";

        public static string V179 {get; set;} = "Base egg move.";
        public static string V180 {get; set;} = "Base egg move missing.";
        public static string V182 {get; set;} = "Not an expected Relearnable move.";
        public static string V183 {get; set;} = "Not an expected DexNav move.";
        public static string V184 {get; set;} = "Expected no Relearn Move in slot.";
        public static string V185 {get; set;} = "Egg Moves Source: {0}.";
        public static string V186 {get; set;} = "Egg Move set check unimplemented.";
        public static string V156 {get; set;} = "Should have a Link Trade HT Memory.";
        public static string V157 {get; set;} = "Should have a HT Memory TextVar value (somewhere).";
        public static string V158 {get; set;} = "Should have a HT Memory Intensity value (1st).";
        public static string V159 {get; set;} = "Should have a HT Memory Feeling value 0-9.";

        public static string V318 {get; set;} = "Form is Valid.";
        public static string V304 {get; set;} = "Form Count is out of range. Expected <= {0}, got {1}.";
        public static string V305 {get; set;} = "Cosplay Pikachu cannot have the default form.";
        public static string V306 {get; set;} = "Only Cosplay Pikachu can have this form.";
        public static string V307 {get; set;} = "Event Pikachu cannot have the default form.";
        public static string V308 {get; set;} = "Held item does not match Form.";
        public static string V309 {get; set;} = "Held item matches Form.";
        public static string V310 {get; set;} = "Form cannot exist outside of a battle.";
        public static string V311 {get; set;} = "Event Vivillon pattern on pre-evolution.";
        public static string V312 {get; set;} = "Invalid Vivillon pattern.";
        public static string V313 {get; set;} = "Valid Vivillon pattern.";
        public static string V314 {get; set;} = "Invalid Eternal Flower encounter.";
        public static string V315 {get; set;} = "Valid Eternal Flower encounter.";
        public static string V316 {get; set;} = "Form cannot exist outside of Party.";
        public static string V317 {get; set;} = "Form cannot be obtained for pre-Alola generation games.";

        public static string V319 {get; set;} = "Cannot apply PP Ups to an Egg.";
        public static string V320 {get; set;} = "Cannot increase Contest Stats of an Egg.";
        public static string V321 {get; set;} = "Mystery Gift Fateful Encounter.";
        public static string V322 {get; set;} = "Mystery Gift Fateful Encounter flag missing.";
        public static string V323 {get; set;} = "Special ingame Fateful Encounter.";
        public static string V324 {get; set;} = "Special ingame Fateful Encounter flag missing.";
        public static string V325 {get; set;} = "Fateful Encounter should not be checked.";
        public static string V326 {get; set;} = "Special ingame N's Sparkle flag missing.";
        public static string V327 {get; set;} = "Special ingame N's Sparkle flag should not be checked.";
        public static string V328 {get; set;} = "Version Specific evolution requires a trade to opposite version. A Handling Trainer is required.";

        public static string V334 {get; set;} = "Non-tradeback egg move. Incompatible with generation 1 exclusive moves.";
        public static string V335 {get; set;} = "Generation 1 exclusive move. Incompatible with Non-tradeback egg moves.";
        public static string V336 {get; set;} = "Egg Move. Incompatible with event egg moves.";
        public static string V337 {get; set;} = "Event Egg Move. Incompatible with normal egg moves.";
        public static string V338 {get; set;} = "Defog and whirpool. One of the two moves should have been removed before transfered to generation 5.";
        public static string V339 {get; set;} = "Generation {0} HM. Should have been removed before transfered to generation {1}.";
        public static string V340 {get; set;} = "Not an expected egg move.";
        public static string V341 {get; set;} = "Egg Move. Not expected in an event egg.";
        public static string V342 {get; set;} = "Event egg move missing.";
        public static string V343 {get; set;} = "Expected the following Moves: {0}";
        public static string V347 {get; set;} = "Inherited move learned by Level-up. Not expected in an event egg.";
        public static string V348 {get; set;} = "Inherited tutor move. Not expected in an event egg.";
        public static string V350 {get; set;} = "Inherited TM/HM move. Not expected in an event egg.";
        public static string V351 {get; set;} = "Invalid Met Location, expected Transporter or Crown."; // Invalid
        public static string V352 {get; set;} = "Arceus from Hall of Origin. Unreleased event.";
        public static string V353 {get; set;} = "Non japanese Mew from Faraway Island. Unreleased event.";
        public static string V354 {get; set;} = "Non Platinum Shaymin from Flower Paradise. Unreleased event.";
        public static string V357 {get; set;} = "Only one Ninjask move allowed.";
        public static string V358 {get; set;} = "Inherited move learned by Level-up. Incompatible with event egg moves.";
        public static string V359 {get; set;} = "Unable to match a gift egg encounter from origin game.";
        public static string V360 {get; set;} = "Unable to match an event egg encounter from origin game.";
        public static string V363 {get; set;} = "Incompatible moves. Learned at the same level in Red/Blue and Yellow.";
        public static string V365 {get; set;} = "Incompatible evolution moves. {0} Move learned at a lower level than other {1} moves.";
        public static string V366 {get; set;} = "Incompatible evolution moves. {1} Move learned at a higher level than other {0} moves.";
        public static string V367 {get; set;} = "Individual EV for a level 100 encounter in generation 4 cannot be greater than 100.";
        public static string V368 {get; set;} = "Eggs can not be infected with Pokérus."; // Invalid
        public static string V369 {get; set;} = "Invalid E-Reader Berry.";
        public static string V370 {get; set;} = "Japanese E-Reader Berry in international savegame.";
        public static string V371 {get; set;} = "American E-Reader Berry in Japanese savegame.";
        #endregion

    }
}
