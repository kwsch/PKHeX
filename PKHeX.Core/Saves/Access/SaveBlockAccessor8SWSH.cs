using System.Collections.Generic;
// ReSharper disable UnusedMember.Local
#pragma warning disable IDE0051 // Remove unused private members
#pragma warning disable RCS1213 // Remove unused member declaration.

namespace PKHeX.Core
{
    public class SaveBlockAccessor8SWSH : SCBlockAccessor, ISaveBlock8Main
    {
        public override IReadOnlyList<SCBlock> BlockInfo { get; }
        public Box8 BoxInfo { get; }
        public Party8 PartyInfo { get; }
        public MyItem Items { get; }
        public MyStatus8 MyStatus { get; }
        public Misc8 Misc { get; }
        public Zukan8 Zukan { get; }
        public BoxLayout8 BoxLayout { get; }
        public PlayTime8 Played { get; }
        public Fused8 Fused { get; }
        public Daycare8 Daycare { get; }
        public Record8 Records { get; }
        public TrainerCard8 TrainerCard{ get; }
        public FashionUnlock8 Fashion { get; }
        public RaidSpawnList8 Raid { get; }
        public RaidSpawnList8 RaidArmor { get; }
        public TitleScreen8 TitleScreen { get; }
        public TeamIndexes8 TeamIndexes { get; }
        public HallOfFameTime8 FameTime { get; }

        public SaveBlockAccessor8SWSH(SAV8SWSH sav)
        {
            BlockInfo = sav.AllBlocks;
            BoxInfo = new Box8(sav, GetBlock(KBox));
            PartyInfo = new Party8(sav, GetBlock(KParty));
            Items = new MyItem8(sav, GetBlock(KItem));
            Zukan = new Zukan8(sav, GetBlock(KZukan), GetBlockSafe(KZukanR1), GetBlockSafe(KZukanR2));
            MyStatus = new MyStatus8(sav, GetBlock(KMyStatus));
            Misc = new Misc8(sav, GetBlock(KMisc));
            BoxLayout = new BoxLayout8(sav, GetBlock(KBoxLayout));
            TrainerCard = new TrainerCard8(sav, GetBlock(KTrainerCard));
            Played = new PlayTime8(sav, GetBlock(KPlayTime));
            Fused = new Fused8(sav, GetBlock(KFused));
            Daycare = new Daycare8(sav, GetBlock(KDaycare));
            Records = new Record8(sav, GetBlock(KRecord), Core.Records.MaxType_SWSH);
            Fashion = new FashionUnlock8(sav, GetBlock(KFashionUnlock));
            Raid = new RaidSpawnList8(sav, GetBlock(KRaidSpawnList), RaidSpawnList8.RaidCountLegal_O0);
            RaidArmor = new RaidSpawnList8(sav, GetBlockSafe(KRaidSpawnListR1), RaidSpawnList8.RaidCountLegal_R1);
            TitleScreen = new TitleScreen8(sav, GetBlock(KTitleScreenTeam));
            TeamIndexes = new TeamIndexes8(sav, GetBlock(KTeamIndexes));
            FameTime = new HallOfFameTime8(sav, GetBlock(KEnteredHallOfFame));
        }

        // Arrays (Blocks)
        private const uint KTeamNames = 0x1920C1E4; // Team 1, 2...6 ((10 + terminator)*6 char16 strings)
        private const uint KBoxLayout = 0x19722c89; // Box Names
        public const uint KBoxWallpapers = 0x2EB1B190; // Box Wallpapers
        private const uint KMenuButtons = 0xB1DDDCA8; // X Menu Button Order

        // Objects (Blocks)
        private const uint KBox = 0x0d66012c; // Box Data
        private const uint KMysteryGift = 0x112d5141; // Mystery Gift Data
        private const uint KItem = 0x1177c2c4; // Items
        private const uint KCoordinates = 0x16aaa7fa; // Coordinates?
        private const uint KMisc = 0x1b882b09; // Money
        private const uint KParty = 0x2985fe5d; // Party Data
        private const uint KDaycare = 0x2d6fba6a; // Daycare slots (2 daycares)
        private const uint KTeamIndexes = 0x33F39467; // Team Indexes for competition
        private const uint KRecord = 0x37da95a3;
        private const uint KZukan = 0x4716c404; // ZukanData_Pokemon
        private const uint KZukanR1 = 0x3F936BA9; // ZukanData_PokemonR1 (Armor)
        private const uint KZukanR2 = 0x3C9366F0; // ZukanData_PokemonR2 (Crown)
        private const uint KCurryDex = 0x6EB72940; // Curry Dex
        private const uint KTrainerCard = 0x874da6fa; // Trainer Card
        private const uint KPlayTime = 0x8cbbfd90; // Time Played
        private const uint KRaidSpawnList = 0x9033eb7b; // Nest current values (hash, seed, meta)
        private const uint KRaidSpawnListR1 = 0x158DA896; // Raid Data for DLC1
        private const uint KFused = 0xc0de5c5f; // Fused PKM (*3)
        private const uint KFashionUnlock = 0xd224f9ac; // Fashion unlock bool array (owned for (each apparel type) * 0x80, then another array for "new")
        private const uint KTitleScreenTeam = 0xE9BE28BF; // Title Screen Team details
        public const uint KEnteredHallOfFame = 0xE2F6E456; // U64 Unix Timestamp
        private const uint KMyStatus = 0xf25c070e; // Trainer Details
        private const uint KFriendLeagueCards = 0x28e707f5; // League Cards received from other players
        private const uint KNPCLeagueCards = 0xb1c26fb0; // League Cards received from NPCs
        private const uint KNPCLeagueCardsR1 = 0xb868ee77; // League Cards received from NPCs on The Isle of Armor

        // Rental Teams - Objects (Blocks) (Incrementing internal names?) 
        private const uint KRentalTeam1 = 0x149A1DD0;
      //private const uint KRentalTeam2 = 0x159A1F63; // does not exist
        private const uint KRentalTeam3 = 0x169A20F6;
        private const uint KRentalTeam4 = 0x179A2289;
        private const uint KRentalTeam5 = 0x189A241C;
        private const uint KRentalTeam6 = 0x199A25AF;

        // Download Rules
        private const uint KDownloadRules1 = 0xEEF1B186;
        private const uint KDownloadRules2 = 0xEFF1B319;
        private const uint KDownloadRules3 = 0xF0F1B4AC;
        private const uint KDownloadRules4 = 0xF1F1B63F;
        private const uint KDownloadRules5 = 0xF2F1B7D2;
        private const uint KDownloadRules6 = 0xF3F1B965;
      //private const uint KDownloadRulesX = 0xF4F1BAF8; // does not exist
      //private const uint KDownloadRulesX = 0xF5F1BC8B; // does not exist
      //private const uint KDownloadRulesX = 0xF6F1BE1E; // does not exist
      //private const uint KDownloadRulesX = 0xF7F1BFB1; // does not exist
      //private const uint KDownloadRulesX = 0xF8F1C144; // does not exist
      //private const uint KDownloadRulesX = 0xF9F1C2D7; // does not exist
        private const uint KDownloadRulesU1 = 0xFAF1C46A;
        private const uint KDownloadRulesU2 = 0xFBF1C5FD;

        private const uint KOfficialCompetition = 0xEEE5A3F8;

        // Raid DLC Flatbuffer Storage Objects (Blocks)
        private const uint KDropRewards = 0x680EEB85; // drop_rewards
        private const uint KDaiEncount = 0xAD3920F5; // dai_encount
        private const uint KNormalEncount = 0xAD9DFA6A; // normal_encount
        private const uint KBonusRewards = 0xEFCAE04E; // bonus_rewards

        private const uint KNormalEncountRigel1 = 0x0E615A8C; // normal_encount_rigel1

        // Values
        public const uint KCurrentBox = 0x017C3CBB; // U32 Box Index
        public const uint KGameLanguage = 0x0BFDEBA1; // U32 Game Language
        public const uint KRepel = 0x9ec079da; // U16 Repel Steps remaining
        public const uint KRotoRally = 0x38548020; // U32 Roto Rally Score (99,999 cap)
        public const uint KBattleTowerSinglesVictory = 0x436CAF2B; // U32 Singles victories (9,999,999 cap)
        public const uint KBattleTowerDoublesVictory = 0x0D477836; // U32 Doubles victories (9,999,999 cap)
        public const uint KBattleTowerSinglesStreak = 0x6226F5AD; // U16 Singles Streak (300 cap)
        public const uint KBattleTowerDoublesStreak = 0x5F74FCEE; // U16 Doubles Streak (300 cap)
        public const uint KStarterChoice = 0x3677602D; // U32 Grookey=0, Scorbunny=1, Sobble=2
        public const uint KDiggingDuoStreakSkill = 0xA0F49CFB; // U32
        public const uint KDiggingDuoStreakStamina = 0x066F38F5; // U32
        public const uint KBirthMonth = 0x0D987D50; // U32
        public const uint KBirthDay = 0x355C8314; // U32
        public const uint KCurrentDexEntry = 0x62743428; // U16 Species ID of last Pokedex entry viewed in Galar Dex
        public const uint KCurrentDexEntryR1 = 0x789FF72D; // U16 Species ID of last Pokedex entry viewed in Armor Dex

        public const uint KVolumeBackgroundMusic = 0xF8154AC9; // U32 0-10
        public const uint KVolumeSoundEffects = 0x62F05895; // U32 0-10
        public const uint KVolumePokémonCries = 0x1D482A63; // U32 0-10

        public const uint KRecordCramorantRobo = 0xB9C0ECFC; // cormorant_robo (Cram-o-matic uses)
        public const uint KRecordBattleVersion = 0x7A9EF7D9; // battle_rom_mark (Past-gen Pokémon reset for battling in Ranked)
        public const uint KRecordSparringTypesCleared = 0xBB1DE8EF; // Number of Types cleared in Restricted Sparring

        public const uint KOptionTextSpeed = 0x92EB0306; // U32 TextSpeedOption
        public const uint KOptionBattleEffects = 0xCCC153CD; // U32 OptOut (Show effects by default)
        public const uint KOptionBattleStyle = 0x765468C3; // U32 OptOut (Allow Switch by default)
        public const uint KOptionSendToBoxes = 0xB1C7C436; // U32 OptIn
        public const uint KOptionGiveNicknames = 0x26A1BEDE; // U32 OptOut
        public const uint KOptionUseGyroscope = 0x79C56A5C; // U32 OptOut
        public const uint KOptionCameraVertical = 0x2846B7DB; // U32 OptOut Invert=1
        public const uint KOptionCameraHorizontal = 0x7D249649; // U32 OptOut Invert=1
        public const uint KOptionCasualControls = 0x3B23B1E2; // U32 OptOut Casual=0
        public const uint KOptionAutoSave = 0xB027F396; // U32 OptOut AutoSave=0
        public const uint KOptionShowNicknames = 0xCA8A8CEE; // U32 OptOut Show=0
        public const uint KOptionShowMoves = 0x9C781AE2; // U32 OptOut Show=0
        public const uint KDojoWattDonationTotal = 0xC7161487; // U32 Amount of Watts donated to Master Dojo
        public const uint KDiggingPaWattStreak = 0x68BBA8B1; // U32 Most Watts dug up by the Digging Pa
        public const uint KAlolanDiglettFound = 0x4AEA5A7E; // U32 Amount of Alolan Diglett found on Isle of Armor
        public const uint KSparringStreakNormal = 0xDB5E16CB; // U32 Best Normal-Type Restricted Sparring Streak
        public const uint KSparringNormalPartySlot1 = 0x7BF09DD3; // U16 Species ID of 1st PKM in party
        public const uint KSparringNormalPartySlot2 = 0x7AF09C40; // U16 Species ID of 2nd PKM in party
        public const uint KSparringNormalPartySlot3 = 0x7DF0A0F9; // U16 Species ID of 3rd PKM in party
        public const uint KSparringStreakFire = 0xD25E08A0; // U32 Best Fire-Type Restricted Sparring Streak
        public const uint KSparringFirePartySlot1 = 0x455C523A; // U16 Species ID of 1st PKM in party
        public const uint KSparringFirePartySlot2 = 0x465C53CD; // U16 Species ID of 2nd PKM in party
        public const uint KSparringFirePartySlot3 = 0x435C4F14; // U16 Species ID of 3rd PKM in party
        public const uint KSparringStreakWater = 0xD55BCEC2; // U32 Best Water-Type Restricted Sparring Streak
        public const uint KSparringWaterPartySlot1 = 0x30396510; // U16 Species ID of 1st PKM in party
        public const uint KSparringWaterPartySlot2 = 0x313966A3; // U16 Species ID of 2nd PKM in party
        public const uint KSparringWaterPartySlot3 = 0x32396836; // U16 Species ID of 3rd PKM in party
        public const uint KSparringStreakElectric = 0xD35BCB9C; // U32 Best Electric-Type Restricted Sparring Streak
        public const uint KSparringElectricPartySlot1 = 0x1E5FB12E; // U16 Species ID of 1st PKM in party
        public const uint KSparringElectricPartySlot2 = 0x1F5FB2C1; // U16 Species ID of 2nd PKM in party
        public const uint KSparringElectricPartySlot3 = 0x1C5FAE08; // U16 Species ID of 3rd PKM in party
        public const uint KSparringStreakGrass = 0xD65BD055; // U32 Best Grass-Type Restricted Sparring Streak
        public const uint KSparringGrassPartySlot1 = 0x70973021; // U16 Species ID of 1st PKM in party
        public const uint KSparringGrassPartySlot2 = 0x6F972E8E; // U16 Species ID of 2nd PKM in party
        public const uint KSparringGrassPartySlot3 = 0x6E972CFB; // U16 Species ID of 3rd PKM in party
        public const uint KSparringStreakIce = 0xD15BC876; // U32 Best Ice-Type Restricted Sparring Streak
        public const uint KSparringIcePartySlot1 = 0x892112D4; // U16 Species ID of 1st PKM in party
        public const uint KSparringIcePartySlot2 = 0x8A211467; // U16 Species ID of 2nd PKM in party
        public const uint KSparringIcePartySlot3 = 0x8B2115FA; // U16 Species ID of 3rd PKM in party
        public const uint KSparringStreakFighting = 0xDA5E1538; // U32 Best Fighting-Type Restricted Sparring Streak
        public const uint KSparringFightingPartySlot1 = 0x153FD7E2; // U16 Species ID of 1st PKM in party
        public const uint KSparringFightingPartySlot2 = 0x163FD975; // U16 Species ID of 2nd PKM in party
        public const uint KSparringFightingPartySlot3 = 0x133FD4BC; // U16 Species ID of 3rd PKM in party
        public const uint KSparringStreakPoison = 0xDC5E185E; // U32 Best Poison-Type Restricted Sparring Streak
        public const uint KSparringPoisonPartySlot1 = 0x3BFF8084; // U16 Species ID of 1st PKM in party
        public const uint KSparringPoisonPartySlot2 = 0x3CFF8217; // U16 Species ID of 2nd PKM in party
        public const uint KSparringPoisonPartySlot3 = 0x3DFF83AA; // U16 Species ID of 3rd PKM in party
        public const uint KSparringStreakGround = 0xDF5E1D17; // U32 Best Ground-Type Restricted Sparring Streak
        public const uint KSparringGroundPartySlot1 = 0x29BC6D6F; // U16 Species ID of 1st PKM in party
        public const uint KSparringGroundPartySlot2 = 0x28BC6BDC; // U16 Species ID of 2nd PKM in party
        public const uint KSparringGroundPartySlot3 = 0x2BBC7095; // U16 Species ID of 3rd PKM in party
        public const uint KSparringStreakFlying = 0xDD5E19F1; // U32 Best Flying-Type Restricted Sparring Streak
        public const uint KSparringFlyingPartySlot1 = 0xA17311F5; // U16 Species ID of 1st PKM in party
        public const uint KSparringFlyingPartySlot2 = 0xA0731062; // U16 Species ID of 2nd PKM in party
        public const uint KSparringFlyingPartySlot3 = 0x9F730ECF; // U16 Species ID of 3rd PKM in party
        public const uint KSparringStreakPsychic = 0xD45BCD2F; // U32 Best Psychic-Type Restricted Sparring Streak
        public const uint KSparringPsychicPartySlot1 = 0x04C18EBF; // U16 Species ID of 1st PKM in party
        public const uint KSparringPsychicPartySlot2 = 0x03C18D2C; // U16 Species ID of 2nd PKM in party
        public const uint KSparringPsychicPartySlot3 = 0x06C191E5; // U16 Species ID of 3rd PKM in party
        public const uint KSparringStreakBug = 0xE15E203D; // U32 Best Bug-Type Restricted Sparring Streak
        public const uint KSparringBugPartySlot1 = 0xE9C80191; // U16 Species ID of 1st PKM in party
        public const uint KSparringBugPartySlot2 = 0xE8C7FFFE; // U16 Species ID of 2nd PKM in party
        public const uint KSparringBugPartySlot3 = 0xE7C7FE6B; // U16 Species ID of 3rd PKM in party
        public const uint KSparringStreakRock = 0xDE5E1B84; // U32 Best Rock-Type Restricted Sparring Streak
        public const uint KSparringRockPartySlot1 = 0xFE44971E; // U16 Species ID of 1st PKM in party
        public const uint KSparringRockPartySlot2 = 0xFF4498B1; // U16 Species ID of 2nd PKM in party
        public const uint KSparringRockPartySlot3 = 0xFC4493F8; // U16 Species ID of 3rd PKM in party
        public const uint KSparringStreakGhost = 0xE05E1EAA; // U32 Best Ghost-Type Restricted Sparring Streak
        public const uint KSparringGhostPartySlot1 = 0x63170940; // U16 Species ID of 1st PKM in party
        public const uint KSparringGhostPartySlot2 = 0x64170AD3; // U16 Species ID of 2nd PKM in party
        public const uint KSparringGhostPartySlot3 = 0x65170C66; // U16 Species ID of 3rd PKM in party
        public const uint KSparringStreakDragon = 0xD25BCA09; // U32 Best Dragon-Type Restricted Sparring Streak
        public const uint KSparringDragonPartySlot1 = 0xC18E2C05; // U16 Species ID of 1st PKM in party
        public const uint KSparringDragonPartySlot2 = 0xC08E2A72; // U16 Species ID of 2nd PKM in party
        public const uint KSparringDragonPartySlot3 = 0xBF8E28DF; // U16 Species ID of 3rd PKM in party
        public const uint KSparringStreakDark = 0xCF5BC550; // U32 Best Dark-Type Restricted Sparring Streak
        public const uint KSparringDarkPartySlot1 = 0xD6F84432; // U16 Species ID of 1st PKM in party
        public const uint KSparringDarkPartySlot2 = 0xD7F845C5; // U16 Species ID of 2nd PKM in party
        public const uint KSparringDarkPartySlot3 = 0xD4F8410C; // U16 Species ID of 3rd PKM in party
        public const uint KSparringStreakSteel = 0xD35E0A33; // U32 Best Steel-Type Restricted Sparring Streak
        public const uint KSparringSteelPartySlot1 = 0x72115D0B; // U16 Species ID of 1st PKM in party
        public const uint KSparringSteelPartySlot2 = 0x71115B78; // U16 Species ID of 2nd PKM in party
        public const uint KSparringSteelPartySlot3 = 0x74116031; // U16 Species ID of 3rd PKM in party
        public const uint KSparringStreakFairy = 0xD05BC6E3; // U32 Best Fairy-Type Restricted Sparring Streak
        public const uint KSparringFairyPartySlot1 = 0x02BFCC63; // U16 Species ID of 1st PKM in party
        public const uint KSparringFairyPartySlot2 = 0x01BFCAD0; // U16 Species ID of 2nd PKM in party
        public const uint KSparringFairyPartySlot3 = 0x04BFCF89; // U16 Species ID of 3rd PKM in party
    }
}
#pragma warning restore IDE0051 // Remove unused private members
#pragma warning restore RCS1213 // Remove unused member declaration.
