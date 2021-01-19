using System.Collections.Generic;
// ReSharper disable UnusedMember.Local
#pragma warning disable IDE0051 // Remove unused private members
#pragma warning disable RCS1213 // Remove unused member declaration.

namespace PKHeX.Core
{
    /// <summary>
    /// Information for Accessing individual blocks within a <see cref="SAV8SWSH"/>.
    /// </summary>
    public sealed class SaveBlockAccessor8SWSH : SCBlockAccessor, ISaveBlock8Main
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
        public RaidSpawnList8 RaidCrown { get; }
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
            RaidCrown = new RaidSpawnList8(sav, GetBlockSafe(KRaidSpawnListR2), RaidSpawnList8.RaidCountLegal_R2);
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
        private const uint KRaidSpawnListR2 = 0x148DA703; // Raid Data for DLC2
        private const uint KFused = 0xc0de5c5f; // Fused PKM (*3)
        public const uint KFusedCalyrex = 0xC37F267B; // Fused Horse
        private const uint KFashionUnlock = 0xd224f9ac; // Fashion unlock bool array (owned for (each apparel type) * 0x80, then another array for "new")
        private const uint KTitleScreenTeam = 0xE9BE28BF; // Title Screen Team details
        public const uint KEnteredHallOfFame = 0xE2F6E456; // U64 Unix Timestamp
        private const uint KMyStatus = 0xf25c070e; // Trainer Details
        private const uint KFriendLeagueCards = 0x28e707f5; // League Cards received from other players
        private const uint KNPCLeagueCards = 0xb1c26fb0; // League Cards received from NPCs
        private const uint KNPCLeagueCardsR1 = 0xb868ee77; // League Cards received from NPCs on The Isle of Armor
        private const uint KNPCLeagueCardsR2 = 0xB968F00A; // League Cards received from NPCs on The Crown Tundra
        private const uint KTrainer1EndlessRecordData = 0x79D787CB; // Trainer 1's Data of Best Endless Dynamax Adventure Record
        private const uint KTrainer2EndlessRecordData = 0x78D78638; // Trainer 2's Data of Best Endless Dynamax Adventure Record
        private const uint KTrainer3EndlessRecordData = 0x7BD78AF1; // Trainer 3's Data of Best Endless Dynamax Adventure Record
        private const uint KTrainer4EndlessRecordData = 0x7AD7895E; // Trainer 4's Data of Best Endless Dynamax Adventure Record

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
        public const uint KCurrentDexEntryR2 = 0x759FF274; // U16 Species ID of last Pokedex entry viewed in Crown Dex
        public const uint KCurrentDex = 0x9CF58395; // U32 Galar=0, Armor=1, Crown=2

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
        public const uint KSparringNormalPartySlot1Species = 0x7BF09DD3; // U16 Species ID of 1st PKM used in party
        public const uint KSparringNormalPartySlot2Species = 0x7AF09C40; // U16 Species ID of 2nd PKM used in party
        public const uint KSparringNormalPartySlot3Species = 0x7DF0A0F9; // U16 Species ID of 3rd PKM used in party
        public const uint KSparringNormalPartySlot1Gender = 0xF8FB2876; // U32 Gender ID of 1st PKM used in party
        public const uint KSparringNormalPartySlot2Gender = 0xF9FB2A09; // U32 Gender ID of 2nd PKM used in party
        public const uint KSparringNormalPartySlot3Gender = 0xF6FB2550; // U32 Gender ID of 3rd PKM used in party
        public const uint KSparringNormalPartySlot1Form = 0xE5181ED2; // U32 Form ID of 1st PKM used in party
        public const uint KSparringNormalPartySlot2Form = 0xE6182065; // U32 Form ID of 2nd PKM used in party
        public const uint KSparringNormalPartySlot3Form = 0xE3181BAC; // U32 Form ID of 3rd PKM used in party
        public const uint KSparringNormalPartySlot1EC = 0xE95199D1; // U32 Encryption Constant of 1st PKM used in party
        public const uint KSparringNormalPartySlot2EC = 0xE851983E; // U32 Encryption Constant of 2nd PKM used in party
        public const uint KSparringNormalPartySlot3EC = 0xE75196AB; // U32 Encryption Constant of 3rd PKM used in party

        public const uint KSparringStreakFire = 0xD25E08A0; // U32 Best Fire-Type Restricted Sparring Streak
        public const uint KSparringFirePartySlot1Species = 0x455C523A; // U16 Species ID of 1st PKM used in party
        public const uint KSparringFirePartySlot2Species = 0x465C53CD; // U16 Species ID of 2nd PKM used in party
        public const uint KSparringFirePartySlot3Species = 0x435C4F14; // U16 Species ID of 3rd PKM used in party
        public const uint KSparringFirePartySlot1Gender = 0x4E5271EF; // U32 Gender ID of 1st PKM used in party
        public const uint KSparringFirePartySlot2Gender = 0x4D52705C; // U32 Gender ID of 2nd PKM used in party
        public const uint KSparringFirePartySlot3Gender = 0x50527515; // U32 Gender ID of 3rd PKM used in party
        public const uint KSparringFirePartySlot1Form = 0x41E9A3FB; // U32 Form ID of 1st PKM used in party
        public const uint KSparringFirePartySlot2Form = 0x40E9A268; // U32 Form ID of 2nd PKM used in party
        public const uint KSparringFirePartySlot3Form = 0x43E9A721; // U32 Form ID of 3rd PKM used in party
        public const uint KSparringFirePartySlot1EC = 0x1F637658; // U32 Encryption Constant of 1st PKM used in party
        public const uint KSparringFirePartySlot2EC = 0x206377EB; // U32 Encryption Constant of 2nd PKM used in party
        public const uint KSparringFirePartySlot3EC = 0x2163797E; // U32 Encryption Constant of 3rd PKM used in party

        public const uint KSparringStreakWater = 0xD55BCEC2; // U32 Best Water-Type Restricted Sparring Streak
        public const uint KSparringWaterPartySlot1Species = 0x30396510; // U16 Species ID of 1st PKM used in party
        public const uint KSparringWaterPartySlot2Species = 0x313966A3; // U16 Species ID of 2nd PKM used in party
        public const uint KSparringWaterPartySlot3Species = 0x32396836; // U16 Species ID of 3rd PKM used in party
        public const uint KSparringWaterPartySlot1Gender = 0xC3264459; // U32 Gender ID of 1st PKM used in party
        public const uint KSparringWaterPartySlot2Gender = 0xC22642C6; // U32 Gender ID of 2nd PKM used in party
        public const uint KSparringWaterPartySlot3Gender = 0xC1264133; // U32 Gender ID of 3rd PKM used in party
        public const uint KSparringWaterPartySlot1Form = 0x9AB09895; // U32 Form ID of 1st PKM used in party
        public const uint KSparringWaterPartySlot2Form = 0x99B09702; // U32 Form ID of 2nd PKM used in party
        public const uint KSparringWaterPartySlot3Form = 0x98B0956F; // U32 Form ID of 3rd PKM used in party
        public const uint KSparringWaterPartySlot1EC = 0x1DE9496E; // U32 Encryption Constant of 1st PKM used in party
        public const uint KSparringWaterPartySlot2EC = 0x1EE94B01; // U32 Encryption Constant of 2nd PKM used in party
        public const uint KSparringWaterPartySlot3EC = 0x1BE94648; // U32 Encryption Constant of 3rd PKM used in party

        public const uint KSparringStreakElectric = 0xD35BCB9C; // U32 Best Electric-Type Restricted Sparring Streak
        public const uint KSparringElectricPartySlot1Species = 0x1E5FB12E; // U16 Species ID of 1st PKM used in party
        public const uint KSparringElectricPartySlot2Species = 0x1F5FB2C1; // U16 Species ID of 2nd PKM used in party
        public const uint KSparringElectricPartySlot3Species = 0x1C5FAE08; // U16 Species ID of 3rd PKM used in party
        public const uint KSparringElectricPartySlot1Gender = 0x3DF2EAF7; // U32 Gender ID of 1st PKM used in party
        public const uint KSparringElectricPartySlot2Gender = 0x3CF2E964; // U32 Gender ID of 2nd PKM used in party
        public const uint KSparringElectricPartySlot3Gender = 0x3FF2EE1D; // U32 Gender ID of 3rd PKM used in party
        public const uint KSparringElectricPartySlot1Form = 0xE74A9573; // U32 Form ID of 1st PKM used in party
        public const uint KSparringElectricPartySlot2Form = 0xE64A93E0; // U32 Form ID of 2nd PKM used in party
        public const uint KSparringElectricPartySlot3Form = 0xE94A9899; // U32 Form ID of 3rd PKM used in party
        public const uint KSparringElectricPartySlot1EC = 0x2FC2FD50; // U32 Encryption Constant of 1st PKM used in party
        public const uint KSparringElectricPartySlot2EC = 0x30C2FEE3; // U32 Encryption Constant of 2nd PKM used in party
        public const uint KSparringElectricPartySlot3EC = 0x31C30076; // U32 Encryption Constant of 3rd PKM used in party

        public const uint KSparringStreakGrass = 0xD65BD055; // U32 Best Grass-Type Restricted Sparring Streak
        public const uint KSparringGrassPartySlot1Species = 0x70973021; // U16 Species ID of 1st PKM used in party
        public const uint KSparringGrassPartySlot2Species = 0x6F972E8E; // U16 Species ID of 2nd PKM used in party
        public const uint KSparringGrassPartySlot3Species = 0x6E972CFB; // U16 Species ID of 3rd PKM used in party
        public const uint KSparringGrassPartySlot1Gender = 0x2454C888; // U32 Gender ID of 1st PKM used in party
        public const uint KSparringGrassPartySlot2Gender = 0x2554CA1B; // U32 Gender ID of 2nd PKM used in party
        public const uint KSparringGrassPartySlot3Gender = 0x2654CBAE; // U32 Gender ID of 3rd PKM used in party
        public const uint KSparringGrassPartySlot1Form = 0xB3FF0924; // U32 Form ID of 1st PKM used in party
        public const uint KSparringGrassPartySlot2Form = 0xB4FF0AB7; // U32 Form ID of 2nd PKM used in party
        public const uint KSparringGrassPartySlot3Form = 0xB5FF0C4A; // U32 Form ID of 3rd PKM used in party
        public const uint KSparringGrassPartySlot1EC = 0x044B26FF; // U32 Encryption Constant of 1st PKM used in party
        public const uint KSparringGrassPartySlot2EC = 0x034B256C; // U32 Encryption Constant of 2nd PKM used in party
        public const uint KSparringGrassPartySlot3EC = 0x064B2A25; // U32 Encryption Constant of 3rd PKM used in party

        public const uint KSparringStreakIce = 0xD15BC876; // U32 Best Ice-Type Restricted Sparring Streak
        public const uint KSparringIcePartySlot1Species = 0x892112D4; // U16 Species ID of 1st PKM used in party
        public const uint KSparringIcePartySlot2Species = 0x8A211467; // U16 Species ID of 2nd PKM used in party
        public const uint KSparringIcePartySlot3Species = 0x8B2115FA; // U16 Species ID of 3rd PKM used in party
        public const uint KSparringIcePartySlot1Gender = 0x355AA71D; // U32 Gender ID of 1st PKM used in party
        public const uint KSparringIcePartySlot2Gender = 0x345AA58A; // U32 Gender ID of 2nd PKM used in party
        public const uint KSparringIcePartySlot3Gender = 0x335AA3F7; // U32 Gender ID of 3rd PKM used in party
        public const uint KSparringIcePartySlot1Form = 0xE1C853B1; // U32 Form ID of 1st PKM used in party
        public const uint KSparringIcePartySlot2Form = 0xE0C8521E; // U32 Form ID of 2nd PKM used in party
        public const uint KSparringIcePartySlot3Form = 0xDFC8508B; // U32 Form ID of 3rd PKM used in party
        public const uint KSparringIcePartySlot1EC = 0xEFCE9172; // U32 Encryption Constant of 1st PKM used in party
        public const uint KSparringIcePartySlot2EC = 0xF0CE9305; // U32 Encryption Constant of 2nd PKM used in party
        public const uint KSparringIcePartySlot3EC = 0xEDCE8E4C; // U32 Encryption Constant of 3rd PKM used in party

        public const uint KSparringStreakFighting = 0xDA5E1538; // U32 Best Fighting-Type Restricted Sparring Streak
        public const uint KSparringFightingPartySlot1Species = 0x153FD7E2; // U16 Species ID of 1st PKM used in party
        public const uint KSparringFightingPartySlot2Species = 0x163FD975; // U16 Species ID of 2nd PKM used in party
        public const uint KSparringFightingPartySlot3Species = 0x133FD4BC; // U16 Species ID of 3rd PKM used in party
        public const uint KSparringFightingPartySlot1Gender = 0x7E6EEC47; // U32 Gender ID of 1st PKM used in party
        public const uint KSparringFightingPartySlot2Gender = 0x7D6EEAB4; // U32 Gender ID of 2nd PKM used in party
        public const uint KSparringFightingPartySlot3Gender = 0x806EEF6D; // U32 Gender ID of 3rd PKM used in party
        public const uint KSparringFightingPartySlot1Form = 0x1EC26C83; // U32 Form ID of 1st PKM used in party
        public const uint KSparringFightingPartySlot2Form = 0x1DC26AF0; // U32 Form ID of 2nd PKM used in party
        public const uint KSparringFightingPartySlot3Form = 0x20C26FA9; // U32 Form ID of 3rd PKM used in party
        public const uint KSparringFightingPartySlot1EC = 0x62A0A180; // U32 Encryption Constant of 1st PKM used in party
        public const uint KSparringFightingPartySlot2EC = 0x63A0A313; // U32 Encryption Constant of 2nd PKM used in party
        public const uint KSparringFightingPartySlot3EC = 0x64A0A4A6; // U32 Encryption Constant of 3rd PKM used in party

        public const uint KSparringStreakPoison = 0xDC5E185E; // U32 Best Poison-Type Restricted Sparring Streak
        public const uint KSparringPoisonPartySlot1Species = 0x3BFF8084; // U16 Species ID of 1st PKM used in party
        public const uint KSparringPoisonPartySlot2Species = 0x3CFF8217; // U16 Species ID of 2nd PKM used in party
        public const uint KSparringPoisonPartySlot3Species = 0x3DFF83AA; // U16 Species ID of 3rd PKM used in party
        public const uint KSparringPoisonPartySlot1Gender = 0x11850B29; // U32 Gender ID of 1st PKM used in party
        public const uint KSparringPoisonPartySlot2Gender = 0x10850996; // U32 Gender ID of 2nd PKM used in party
        public const uint KSparringPoisonPartySlot3Gender = 0x0F850803; // U32 Gender ID of 3rd PKM used in party
        public const uint KSparringPoisonPartySlot1Form = 0xD0EB3B25; // U32 Form ID of 1st PKM used in party
        public const uint KSparringPoisonPartySlot2Form = 0xCFEB3992; // U32 Form ID of 2nd PKM used in party
        public const uint KSparringPoisonPartySlot3Form = 0xCEEB37FF; // U32 Form ID of 3rd PKM used in party
        public const uint KSparringPoisonPartySlot1EC = 0x171AE45E; // U32 Encryption Constant of 1st PKM used in party
        public const uint KSparringPoisonPartySlot2EC = 0x181AE5F1; // U32 Encryption Constant of 2nd PKM used in party
        public const uint KSparringPoisonPartySlot3EC = 0x151AE138; // U32 Encryption Constant of 3rd PKM used in party

        public const uint KSparringStreakGround = 0xDF5E1D17; // U32 Best Ground-Type Restricted Sparring Streak
        public const uint KSparringGroundPartySlot1Species = 0x29BC6D6F; // U16 Species ID of 1st PKM used in party
        public const uint KSparringGroundPartySlot2Species = 0x28BC6BDC; // U16 Species ID of 2nd PKM used in party
        public const uint KSparringGroundPartySlot3Species = 0x2BBC7095; // U16 Species ID of 3rd PKM used in party
        public const uint KSparringGroundPartySlot1Gender = 0x69F256BA; // U32 Gender ID of 1st PKM used in party
        public const uint KSparringGroundPartySlot2Gender = 0x6AF2584D; // U32 Gender ID of 2nd PKM used in party
        public const uint KSparringGroundPartySlot3Gender = 0x67F25394; // U32 Gender ID of 3rd PKM used in party
        public const uint KSparringGroundPartySlot1Form = 0x2C7F8BCE; // U32 Form ID of 1st PKM used in party
        public const uint KSparringGroundPartySlot2Form = 0x2D7F8D61; // U32 Form ID of 2nd PKM used in party
        public const uint KSparringGroundPartySlot3Form = 0x2A7F88A8; // U32 Form ID of 3rd PKM used in party
        public const uint KSparringGroundPartySlot1EC = 0xBA495F35; // U32 Encryption Constant of 1st PKM used in party
        public const uint KSparringGroundPartySlot2EC = 0xB9495DA2; // U32 Encryption Constant of 2nd PKM used in party
        public const uint KSparringGroundPartySlot3EC = 0xB8495C0F; // U32 Encryption Constant of 3rd PKM used in party

        public const uint KSparringStreakFlying = 0xDD5E19F1; // U32 Best Flying-Type Restricted Sparring Streak
        public const uint KSparringFlyingPartySlot1Species = 0xA17311F5; // U16 Species ID of 1st PKM used in party
        public const uint KSparringFlyingPartySlot2Species = 0xA0731062; // U16 Species ID of 2nd PKM used in party
        public const uint KSparringFlyingPartySlot3Species = 0x9F730ECF; // U16 Species ID of 3rd PKM used in party
        public const uint KSparringFlyingPartySlot1Gender = 0x2B232D98; // U32 Gender ID of 1st PKM used in party
        public const uint KSparringFlyingPartySlot2Gender = 0x2C232F2B; // U32 Gender ID of 2nd PKM used in party
        public const uint KSparringFlyingPartySlot3Gender = 0x2D2330BE; // U32 Gender ID of 3rd PKM used in party
        public const uint KSparringFlyingPartySlot1Form = 0x23E747F4; // U32 Form ID of 1st PKM used in party
        public const uint KSparringFlyingPartySlot2Form = 0x24E74987; // U32 Form ID of 2nd PKM used in party
        public const uint KSparringFlyingPartySlot3Form = 0x25E74B1A; // U32 Form ID of 3rd PKM used in party
        public const uint KSparringFlyingPartySlot1EC = 0x4292BAAF; // U32 Encryption Constant of 1st PKM used in party
        public const uint KSparringFlyingPartySlot2EC = 0x4192B91C; // U32 Encryption Constant of 2nd PKM used in party
        public const uint KSparringFlyingPartySlot3EC = 0x4492BDD5; // U32 Encryption Constant of 3rd PKM used in party

        public const uint KSparringStreakPsychic = 0xD45BCD2F; // U32 Best Psychic-Type Restricted Sparring Streak
        public const uint KSparringPsychicPartySlot1Species = 0x04C18EBF; // U16 Species ID of 1st PKM used in party
        public const uint KSparringPsychicPartySlot2Species = 0x03C18D2C; // U16 Species ID of 2nd PKM used in party
        public const uint KSparringPsychicPartySlot3Species = 0x06C191E5; // U16 Species ID of 3rd PKM used in party
        public const uint KSparringPsychicPartySlot1Gender = 0x70EEC566; // U32 Gender ID of 1st PKM used in party
        public const uint KSparringPsychicPartySlot2Gender = 0x71EEC6F9; // U32 Gender ID of 2nd PKM used in party
        public const uint KSparringPsychicPartySlot3Gender = 0x6EEEC240; // U32 Gender ID of 3rd PKM used in party
        public const uint KSparringPsychicPartySlot1Form = 0x81D70402; // U32 Form ID of 1st PKM used in party
        public const uint KSparringPsychicPartySlot2Form = 0x82D70595; // U32 Form ID of 2nd PKM used in party
        public const uint KSparringPsychicPartySlot3Form = 0x7FD700DC; // U32 Form ID of 3rd PKM used in party
        public const uint KSparringPsychicPartySlot1EC = 0x896D7D61; // U32 Encryption Constant of 1st PKM used in party
        public const uint KSparringPsychicPartySlot2EC = 0x886D7BCE; // U32 Encryption Constant of 2nd PKM used in party
        public const uint KSparringPsychicPartySlot3EC = 0x876D7A3B; // U32 Encryption Constant of 3rd PKM used in party

        public const uint KSparringStreakBug = 0xE15E203D; // U32 Best Bug-Type Restricted Sparring Streak
        public const uint KSparringBugPartySlot1Species = 0xE9C80191; // U16 Species ID of 1st PKM used in party
        public const uint KSparringBugPartySlot2Species = 0xE8C7FFFE; // U16 Species ID of 2nd PKM used in party
        public const uint KSparringBugPartySlot3Species = 0xE7C7FE6B; // U16 Species ID of 3rd PKM used in party
        public const uint KSparringBugPartySlot1Gender = 0xFC1AF2FC; // U32 Gender ID of 1st PKM used in party
        public const uint KSparringBugPartySlot2Gender = 0xFD1AF48F; // U32 Gender ID of 2nd PKM used in party
        public const uint KSparringBugPartySlot3Gender = 0xFE1AF622; // U32 Gender ID of 3rd PKM used in party
        public const uint KSparringBugPartySlot1Form = 0x32EF5030; // U32 Form ID of 1st PKM used in party
        public const uint KSparringBugPartySlot2Form = 0x33EF51C3; // U32 Form ID of 2nd PKM used in party
        public const uint KSparringBugPartySlot3Form = 0x34EF5356; // U32 Form ID of 3rd PKM used in party
        public const uint KSparringBugPartySlot1EC = 0x7B7A3613; // U32 Encryption Constant of 1st PKM used in party
        public const uint KSparringBugPartySlot2EC = 0x7A7A3480; // U32 Encryption Constant of 2nd PKM used in party
        public const uint KSparringBugPartySlot3EC = 0x7D7A3939; // U32 Encryption Constant of 3rd PKM used in party

        public const uint KSparringStreakRock = 0xDE5E1B84; // U32 Best Rock-Type Restricted Sparring Streak
        public const uint KSparringRockPartySlot1Species = 0xFE44971E; // U16 Species ID of 1st PKM used in party
        public const uint KSparringRockPartySlot2Species = 0xFF4498B1; // U16 Species ID of 2nd PKM used in party
        public const uint KSparringRockPartySlot3Species = 0xFC4493F8; // U16 Species ID of 3rd PKM used in party
        public const uint KSparringRockPartySlot1Gender = 0x96A7618B; // U32 Gender ID of 1st PKM used in party
        public const uint KSparringRockPartySlot2Gender = 0x95A75FF8; // U32 Gender ID of 2nd PKM used in party
        public const uint KSparringRockPartySlot3Gender = 0x98A764B1; // U32 Gender ID of 3rd PKM used in party
        public const uint KSparringRockPartySlot1Form = 0x92E09FDF; // U32 Form ID of 1st PKM used in party
        public const uint KSparringRockPartySlot2Form = 0x91E09E4C; // U32 Form ID of 2nd PKM used in party
        public const uint KSparringRockPartySlot3Form = 0x94E0A305; // U32 Form ID of 3rd PKM used in party
        public const uint KSparringRockPartySlot1EC = 0x54D5CDC4; // U32 Encryption Constant of 1st PKM used in party
        public const uint KSparringRockPartySlot2EC = 0x55D5CF57; // U32 Encryption Constant of 2nd PKM used in party
        public const uint KSparringRockPartySlot3EC = 0x56D5D0EA; // U32 Encryption Constant of 3rd PKM used in party

        public const uint KSparringStreakGhost = 0xE05E1EAA; // U32 Best Ghost-Type Restricted Sparring Streak
        public const uint KSparringGhostPartySlot1Species = 0x63170940; // U16 Species ID of 1st PKM used in party
        public const uint KSparringGhostPartySlot2Species = 0x64170AD3; // U16 Species ID of 2nd PKM used in party
        public const uint KSparringGhostPartySlot3Species = 0x65170C66; // U16 Species ID of 3rd PKM used in party
        public const uint KSparringGhostPartySlot1Gender = 0xBC29D5AD; // U32 Gender ID of 1st PKM used in party
        public const uint KSparringGhostPartySlot2Gender = 0xBB29D41A; // U32 Gender ID of 2nd PKM used in party
        public const uint KSparringGhostPartySlot3Gender = 0xBA29D287; // U32 Gender ID of 3rd PKM used in party
        public const uint KSparringGhostPartySlot1Form = 0xFEB64141; // U32 Form ID of 1st PKM used in party
        public const uint KSparringGhostPartySlot2Form = 0xFDB63FAE; // U32 Form ID of 2nd PKM used in party
        public const uint KSparringGhostPartySlot3Form = 0xFCB63E1B; // U32 Form ID of 3rd PKM used in party
        public const uint KSparringGhostPartySlot1EC = 0x14C97022; // U32 Encryption Constant of 1st PKM used in party
        public const uint KSparringGhostPartySlot2EC = 0x15C971B5; // U32 Encryption Constant of 2nd PKM used in party
        public const uint KSparringGhostPartySlot3EC = 0x12C96CFC; // U32 Encryption Constant of 3rd PKM used in party

        public const uint KSparringStreakDragon = 0xD25BCA09; // U32 Best Dragon-Type Restricted Sparring Streak
        public const uint KSparringDragonPartySlot1Species = 0xC18E2C05; // U16 Species ID of 1st PKM used in party
        public const uint KSparringDragonPartySlot2Species = 0xC08E2A72; // U16 Species ID of 2nd PKM used in party
        public const uint KSparringDragonPartySlot3Species = 0xBF8E28DF; // U16 Species ID of 3rd PKM used in party
        public const uint KSparringDragonPartySlot1Gender = 0xAEF960AC; // U32 Gender ID of 1st PKM used in party
        public const uint KSparringDragonPartySlot2Gender = 0xAFF9623F; // U32 Gender ID of 2nd PKM used in party
        public const uint KSparringDragonPartySlot3Gender = 0xB0F963D2; // U32 Gender ID of 3rd PKM used in party
        public const uint KSparringDragonPartySlot1Form = 0x5C548FE0; // U32 Form ID of 1st PKM used in party
        public const uint KSparringDragonPartySlot2Form = 0x5D549173; // U32 Form ID of 2nd PKM used in party
        public const uint KSparringDragonPartySlot3Form = 0x5E549306; // U32 Form ID of 3rd PKM used in party
        public const uint KSparringDragonPartySlot1EC = 0x1B9619A3; // U32 Encryption Constant of 1st PKM used in party
        public const uint KSparringDragonPartySlot2EC = 0x1A961810; // U32 Encryption Constant of 2nd PKM used in party
        public const uint KSparringDragonPartySlot3EC = 0x1D961CC9; // U32 Encryption Constant of 3rd PKM used in party

        public const uint KSparringStreakDark = 0xCF5BC550; // U32 Best Dark-Type Restricted Sparring Streak
        public const uint KSparringDarkPartySlot1Species = 0xD6F84432; // U16 Species ID of 1st PKM used in party
        public const uint KSparringDarkPartySlot2Species = 0xD7F845C5; // U16 Species ID of 2nd PKM used in party
        public const uint KSparringDarkPartySlot3Species = 0xD4F8410C; // U16 Species ID of 3rd PKM used in party
        public const uint KSparringDarkPartySlot1Gender = 0x768C477B; // U32 Gender ID of 1st PKM used in party
        public const uint KSparringDarkPartySlot2Gender = 0x758C45E8; // U32 Gender ID of 2nd PKM used in party
        public const uint KSparringDarkPartySlot3Gender = 0x788C4AA1; // U32 Gender ID of 3rd PKM used in party
        public const uint KSparringDarkPartySlot1Form = 0x2F9F850F; // U32 Form ID of 1st PKM used in party
        public const uint KSparringDarkPartySlot2Form = 0x2E9F837C; // U32 Form ID of 2nd PKM used in party
        public const uint KSparringDarkPartySlot3Form = 0x319F8835; // U32 Form ID of 3rd PKM used in party
        public const uint KSparringDarkPartySlot1EC = 0xA1F76014; // U32 Encryption Constant of 1st PKM used in party
        public const uint KSparringDarkPartySlot2EC = 0xA2F761A7; // U32 Encryption Constant of 2nd PKM used in party
        public const uint KSparringDarkPartySlot3EC = 0xA3F7633A; // U32 Encryption Constant of 3rd PKM used in party

        public const uint KSparringStreakSteel = 0xD35E0A33; // U32 Best Steel-Type Restricted Sparring Streak
        public const uint KSparringSteelPartySlot1Species = 0x72115D0B; // U16 Species ID of 1st PKM used in party
        public const uint KSparringSteelPartySlot2Species = 0x71115B78; // U16 Species ID of 2nd PKM used in party
        public const uint KSparringSteelPartySlot3Species = 0x74116031; // U16 Species ID of 3rd PKM used in party
        public const uint KSparringSteelPartySlot1Gender = 0x22DA9B9E; // U32 Gender ID of 1st PKM used in party
        public const uint KSparringSteelPartySlot2Gender = 0x23DA9D31; // U32 Gender ID of 2nd PKM used in party
        public const uint KSparringSteelPartySlot3Gender = 0x20DA9878; // U32 Gender ID of 3rd PKM used in party
        public const uint KSparringSteelPartySlot1Form = 0x1534992A; // U32 Form ID of 1st PKM used in party
        public const uint KSparringSteelPartySlot2Form = 0x16349ABD; // U32 Form ID of 2nd PKM used in party
        public const uint KSparringSteelPartySlot3Form = 0x13349604; // U32 Form ID of 3rd PKM used in party
        public const uint KSparringSteelPartySlot1EC = 0x05C553E9; // U32 Encryption Constant of 1st PKM used in party
        public const uint KSparringSteelPartySlot2EC = 0x04C55256; // U32 Encryption Constant of 2nd PKM used in party
        public const uint KSparringSteelPartySlot3EC = 0x03C550C3; // U32 Encryption Constant of 3rd PKM used in party

        public const uint KSparringStreakFairy = 0xD05BC6E3; // U32 Best Fairy-Type Restricted Sparring Streak
        public const uint KSparringFairyPartySlot1Species = 0x02BFCC63; // U16 Species ID of 1st PKM used in party
        public const uint KSparringFairyPartySlot2Species = 0x01BFCAD0; // U16 Species ID of 2nd PKM used in party
        public const uint KSparringFairyPartySlot3Species = 0x04BFCF89; // U16 Species ID of 3rd PKM used in party
        public const uint KSparringFairyPartySlot1Gender = 0x49D73CAA; // U32 Gender ID of 1st PKM used in party
        public const uint KSparringFairyPartySlot2Gender = 0x4AD73E3D; // U32 Gender ID of 2nd PKM used in party
        public const uint KSparringFairyPartySlot3Gender = 0x47D73984; // U32 Gender ID of 3rd PKM used in party
        public const uint KSparringFairyPartySlot1Form = 0x77442151; // U32 Form ID of 1st PKM used in party
        public const uint KSparringFairyPartySlot2Form = 0x76441FBE; // U32 Form ID of 2nd PKM used in party
        public const uint KSparringFairyPartySlot3Form = 0x74441C98; // U32 Form ID of 3rd PKM used in party
        public const uint KSparringFairyPartySlot1EC = 0xC117C445; // U32 Encryption Constant of 1st PKM used in party
        public const uint KSparringFairyPartySlot2EC = 0xC017C2B2; // U32 Encryption Constant of 2nd PKM used in party
        public const uint KSparringFairyPartySlot3EC = 0xBF17C11F; // U32 Encryption Constant of 3rd PKM used in party
        public const uint KSparringFairyPartySlot1Sweet = 0xB14624FF; // U32 Alcremie Sweet ID if 1st PKM used in party, otherwise -1
        public const uint KSparringFairyPartySlot2Sweet = 0xB046236C; // U32 Alcremie Sweet ID if 2nd PKM used in party, otherwise -1
        public const uint KSparringFairyPartySlot3Sweet = 0xB3462825; // U32 Alcremie Sweet ID if 3rd PKM used in party, otherwise -1

        public const uint KRegielekiOrRegidragoPattern = 0xCF90B39A; // U32 Chosen Pattern for Split-Decision Ruins (0 = not chosen, 1 = electric, 2 = dragon)
        public const uint KFootprintPercentageCobalion = 0x4D50B655; // U32 Footprints of Cobalion collected on Crown Tundra; values go from 0-100
        public const uint KFootprintPercentageTerrakion = 0x771E4C88; // U32 Footprints of Terrakion collected on Crown Tundra; values from 0-100
        public const uint KFootprintPercentageVirizion = 0xAD67A297; // U32 Footprints of Virizion collected on Crown Tundra; values go from 0-100
        public const uint KPlayersInteractedOnline = 0x31A13425; // U32 Number of Players interacted with online
        public const uint KMaxLairSpeciesID1Noted = 0x6F669A35; // U32 Max Lair Species 1 Noted
        public const uint KMaxLairSpeciesID2Noted = 0x6F66951C; // U32 Max Lair Species 2 Noted
        public const uint KMaxLairSpeciesID3Noted = 0x6F6696CF; // U32 Max Lair Species 3 Noted
        public const uint KMaxLairEndlessStreak = 0x7F4B4B10; // U32 Endless Dynamax Adventure Best Streak
        public const uint KMaxLairDisconnectStreak = 0x8EAEB8FF; // U32 Value of 3 will have you pay a Dynite Ore fee upon entry
        public const uint KMaxLairPeoniaSpeciesHint = 0xF26B9151; // U32 Species ID for Peonia to hint
        public const uint KMaxLairRentalChoiceSeed = 0x0D74AA40; // U64 seed used to pick Dynamax Adventure rental and encounter templates

        public const uint KGSTVictoriesTotal = 0x9D6727F6; // U32 Total Galarian Star Tournament victories
        public const uint KGSTVictoriesAvery = 0x3934BEC0; // U32 Galarian Star Tournament victories with Avery
        public const uint KGSTVictoriesKlara = 0xE9131991; // U32 Galarian Star Tournament victories with Klara
        public const uint KGSTVictoriesMustard = 0x7742B542; // U32 Galarian Star Tournament victories with Mustard
        public const uint KGSTVictoriesPeony = 0x9A535FA0; // U32 Galarian Star Tournament victories with Peony
        public const uint KGSTVictoriesLeon = 0xFEE68CE1; // U32 Galarian Star Tournament victories with Leon
        public const uint KGSTVictoriesBede = 0x2D0520CD; // U32 Galarian Star Tournament victories with Bede
        public const uint KGSTVictoriesMarnie = 0xFB5133BC; // U32 Galarian Star Tournament victories with Marnie
        public const uint KGSTVictoriesMilo = 0x24B9E3CC; // U32 Galarian Star Tournament victories with Milo
        public const uint KGSTVictoriesNessa = 0xB19386DE; // U32 Galarian Star Tournament victories with Nessa
        public const uint KGSTVictoriesKabu = 0x3576EE34; // U32 Galarian Star Tournament victories with Kabu
        public const uint KGSTVictoriesPiers = 0xB95B1BB9; // U32 Galarian Star Tournament victories with Piers
        public const uint KGSTVictoriesRaihan = 0x343E6FC1; // U32 Galarian Star Tournament victories with Raihan
        public const uint KGSTVictoriesBea = 0x6371183B; // U32 Galarian Star Tournament victories with Bea
        public const uint KGSTVictoriesGordie = 0xA2C094C7; // U32 Galarian Star Tournament victories with Gordie
        public const uint KGSTVictoriesAllister = 0x9E32AE34; // U32 Galarian Star Tournament victories with Allister
        public const uint KGSTVictoriesMelony = 0x06C0FBC8; // U32 Galarian Star Tournament victories with Melony
        public const uint KGSTVictoriesSordward = 0xE3ED8F16; // U32 Galarian Star Tournament victories with Sordward
        public const uint KGSTVictoriesShielbert = 0xC0D49E2D; // U32 Galarian Star Tournament victories with Shielbert
        public const uint KGSTVictoriesHop = 0xEB07C276; // U32 Galarian Star Tournament victories with Hop
        public const uint KGSTVictoriesOpal = 0xDBE374D7; // U32 Galarian Star Tournament victories with Opal
    }
}
#pragma warning restore IDE0051 // Remove unused private members
#pragma warning restore RCS1213 // Remove unused member declaration.
