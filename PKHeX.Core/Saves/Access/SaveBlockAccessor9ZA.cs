using System.Collections.Generic;
// ReSharper disable UnusedMember.Local
#pragma warning disable IDE0051 // Remove unused private members
#pragma warning disable RCS1213 // Remove unused member declaration.

namespace PKHeX.Core;

/// <summary>
/// Information for Accessing individual blocks within a <see cref="SAV9ZA"/>.
/// </summary>
public sealed class SaveBlockAccessor9ZA(SAV9ZA sav) : SCBlockAccessor
{
    public override IReadOnlyList<SCBlock> BlockInfo { get; } = sav.AllBlocks;
    public Box8 BoxInfo { get; } = new(sav, Block(sav, KBox));
    public Party9a PartyInfo { get; } = new(sav, Block(sav, KParty));
    public MyItem9a Items { get; } = new(sav, Block(sav, KItem));
    public MyStatus9a MyStatus { get; } = new(sav, Block(sav, KMyStatus));
    public BoxLayout9a BoxLayout { get; } = new(sav, Block(sav, KBoxLayout));
    public PlayTime9a Played { get; } = new(sav, Block(sav, KPlayTime)); // not actually used
    public Zukan9a Zukan { get; } = new(sav, Block(sav, KPokedex));
    public Epoch1900DateTimeValue LastSaved { get; } = new(Block(sav, KLastSaved));
    public TeamIndexes8 TeamIndexes { get; } = new(sav, Block(sav, KTeamIndexes), Block(sav, KTeamLocks));
    public Epoch1900DateTimeValue EnrollmentDate { get; } = new(Block(sav, KEnrollmentDate)); // not actually used
    public Coordinates9a Coordinates { get; } = new(sav, Block(sav, KCoordinates));
    public InfiniteRoyale9a InfiniteRoyale { get; } = new(sav, Block(sav, KInfiniteRoyale));
    public PlayerAppearance9a PlayerAppearance { get; } = new(sav, Block(sav, KCurrentAppearance));
    public PlayerFashion9a PlayerFashion { get; } = new(sav, Block(sav, KCurrentClothing));
    public ConfigSave9a Config { get; } = new(sav, Block(sav, KConfig));

    public EventWorkFlagStorage Event { get; } = new(sav, Block(sav, KEventFlag));
    public EventWorkValueStorage Work { get; } = new(sav, Block(sav, KEventWork));
    public EventWorkValueStorage Quest { get; } = new(sav, Block(sav, KEventWorkQuest));
    public EventWorkValueStorage WorkMable { get; } = new(sav, Block(sav, KEventWorkMable));
    public EventWorkValueStorage CountMable { get; } = new(sav, Block(sav, KEventCountMable));
    public EventWorkValueStorage CountTitle { get; } = new(sav, Block(sav, KEventCountTitle));
    public EventWorkValueStorage WorkSpawn { get; } = new(sav, Block(sav, KEventWorkSpawn));
    public EventWorkFlagStorage Flags { get; } = new(sav, Block(sav, KEventFlagsOther));

    public EventWorkValueStorageKey128 Report { get; } = new(sav, Block(sav, KEventReport));
    public EventWorkValueStorageKey128 Obstruction { get; } = new(sav, Block(sav, KObstruction));
    public EventWorkFlagStorage FieldItems { get; } = new(sav, Block(sav, KFieldItems));
    public EventWorkValueStorageKey192 FieldObjectInteractable { get; } = new(sav, Block(sav, KFieldObjectInteractable));

    public EventWorkValueStorageKey128 Spawner2 { get; } = new(sav, Block(sav, KEventSpawner2)); // (u64-key, u64-bool, u64-struct)
    public EventWorkValueStorage InfiniteRank { get; } = new(sav, Block(sav, KEventInfiniteRank)); // (u64-key, u64-struct)
    public EventWorkValueStorageKey128 Spawner4 { get; } = new(sav, Block(sav, KEventSpawner4)); // (u64-key, u64-hash, u64-struct)

    public MableStatus9a Mable { get; } = new(sav, Block(sav, KStatusMable));
    public DonutPocket9a Donuts { get; } = new(sav, BlockSafe(sav, KDonuts));

    private const uint KBox = 0x0d66012c; // Box Data
    private const uint KParty = 0x3AA1A9AD; // Party Data
    private const uint KItem = 0x21C9BD44; // Items
    private const uint KMyStatus = 0xE3E89BD1; // Trainer Details
    private const uint KBoxLayout = 0x19722c89; // Box Names
    private const uint KPlayTime = 0xEDAFF794; // Time Played
    public const uint KPlayTimeOverworld = 0x34597EF7;
    private const uint KPokedex = 0x2D87BE5C;
    private const uint KLastSaved = 0x1522C79C; // Epoch 1900 DateTime
    private const uint KTeamIndexes = 0x33F39467; // Team Indexes for competition
    private const uint KTeamLocks = 0x605EBC30;
    private const uint KEnrollmentDate = 0xC7409C89; // Epoch 1900 Date
    public const uint KBoxesUnlocked = 0x71825204;
    public const uint KBoxWallpapers = 0x2EB1B190; // Box Wallpapers
    public const uint KMoney = 0x4F35D0DD; // u32
    public const uint KCurrentBox = 0x017C3CBB; // U32 Box Index
    public const uint KSaveRevision = 0x0926555A; // u64 revision
    private const uint KCoordinates = 0x910D381F; // Player Coordinates/Rotation
    private const uint KInfiniteRoyale = 0x8929BFB6; // object
    private const uint KConfig = 0xAC6DD22F; // u64 object

    private const uint KEventFlag = 0x58505C5E; // event_flag (u64,bool)[2048]
    private const uint KEventFlagsOther = 0xED6F46E7; // system_flag (u64,bool)[2048]
    private const uint KEventWork = 0xFADA7742; // system_work (u64,u64)[256]

    private const uint KEventWorkQuest = 0xB9B223B9; // quest_work (u64,u64)[1024] - Story Quest Status 
    private const uint KEventWorkMable = 0x03913534; // momiji_work (u64,u64)[1024] - Mable Tasks Status
    private const uint KEventCountMable = 0x8D80EC0F; // momiji_count (u64,u64)[64] - Mable Tasks Counts

    private const uint KEventCountTitle = 0x2C2C6964; // title_count (u64,u64)[64] - Player earned display titles
    private const uint KEventWorkSpawn = 0x53FD0223; // Overworld Spawner 0x46500 small values (u64,u64)[18000]
    private const uint KEventSpawner2 = 0x79ABCB0B; // (u64-key, u64-bool, u64-struct)
    private const uint KEventInfiniteRank = 0x7C896A83; // (u64-key, u64-struct)
    private const uint KEventSpawner4 = 0xD1A3FF7B; // (u64-key, u64-hash, u64-struct)
                                                     // B25E7EE5 0x400 unused

    private const uint KEventReport = 0xAF2165F0; // 0x3000 (u64,(s64,u64) value)
    private const uint KObstruction = 0x4C26C29B; // (u64, u64-state, u64-unused)[2000]
    private const uint KFieldObjectInteractable = 0x7147C953; // (u64,u64,u64,value)[5000] (mega crystal, prize medals)

    public const uint KTicketPointsZARoyale = 0x9A730DE1; // u32
    public const uint KTicketPointsZARoyaleInfinite = 0x1D7EE369; // u32

    public const uint KPlayedSeconds = 0xCE3AF8F2; // d64
    private const uint KCurrentAppearance = 0x812FC3E3; // u32[18] (conveniently named `PLAYER_SAVE_DATA`, same as PlayerData8b's official name!
    private const uint KCurrentClothing = 0x64235B3D; // u64[13], see PlayerFashion9 (size is bigger)
    public const uint KFusedCalyrex = 0x916BCA9E; // unused
    private const uint KFieldItems = 0x2482AD60; // Stores grabbed status for each existing field item
    private const uint KOverworld = 0x5E8E1711; // object 0x28488
    private const uint KLostBalls = 0xDD90CEEC; // (uint ID, uint count)[50] items
    public const uint KTimeSecondsPastMidnight = 0x7B70DA66; // f32

    public const uint KStoredEventEntity = 0x654FCD1E; // 128 * (u64-hash + 0x158+48-entity) = 0x1A8
    public const uint KStoredShinyEntity = 0xF3A8569D; // 10 * (u64-hash + 0x158+48+40-entity + u64-hash) = 0x1F0

    public const uint KFashionTops     = 0xD4BCE690; // 01_230_101 - unlock[4800]
    public const uint KFashionBottoms  = 0x4DCB6EBC; // 02_011_010 - unlock[5300]
    public const uint KFashionAllInOne = 0x324F26DD; // 03_021_212 - unlock[3000]
    public const uint KFashionHeadwear = 0x293B1722; // 04_010_108 - unlock[600]
    public const uint KFashionEyewear  = 0xF4439BD4; // 05_060_101 - unlock[300]
    public const uint KFashionGloves   = 0x502EA6C8; // 06_010_101 - unlock[200]
    public const uint KFashionLegwear  = 0x6019F261; // 07_020_115 - unlock[700]
    public const uint KFashionFootwear = 0x954C8BAB; // 08_010_105 - unlock[400]
    public const uint KFashionSatchels = 0x08638763; // 09_010_101 - unlock[200]
    public const uint KFashionEarrings = 0xFA089054; // 10_030_116 - unlock[300]

    public const uint KHairMake00StyleHair    = 0x8E466865; // unlock[30]
    public const uint KHairMake01StyleBangs   = 0x7D78DBBB; // unlock[10]
    public const uint KHairMake02ColorHair    = 0xA9BF43AF; // unlock[64] - Base Color
    public const uint KHairMake03ColorHair    = 0x0AABCD8F; // unlock[64] - Blocking
    public const uint KHairMake04ColorHair    = 0x9D48303D; // unlock[64] - Balayage
    public const uint KHairMake05StyleEyebrow = 0x1354939F; // unlock[30]
    public const uint KHairMake06ColorEyebrow = 0xF428AFF2; // unlock[64]
    public const uint KHairMake07StyleEyes    = 0x267FB825; // unlock[8]
    public const uint KHairMake08ColorEyes    = 0x4856C1F4; // unlock[64]
    public const uint KHairMake09StyleEyelash = 0xDE67CD9D; // unlock[16]
    public const uint KHairMake10ColorEyelash = 0x53106AFC; // unlock[64]
    public const uint KHairMake11Lips         = 0xDACF515D; // unlock[20]
    public const uint KHairMake12BeautyMark   = 0x0B758BF5; // unlock[18]
    public const uint KHairMake13Freckles     = 0xFC08F055; // unlock[8]
    public const uint KHairMake14DarkCircles  = 0x550EC7E1; // unlock[8]

    // Taken during the story after the Society for Battle Connoisseurs complete
    public const uint KPictureSBCWidth = 0x70CDE2DF; // 340
    public const uint KPictureSBCHeight = 0x08B59D02; // 340
    public const uint KPictureSBCData = 0x89816ADE; // byte[]

    // Taken at the start of the game inside Hotel Z
    public const uint KPictureInitialWidth = 0x1C140AA0; // 564
    public const uint KPictureInitialHeight = 0x72A45DD7; // 292
    public const uint KPictureInitialData = 0x71392A77; // byte[]

    // Taken by the player at any time
    public const uint KPictureCurrentWidth = 0x580634CD; // 564
    public const uint KPictureCurrentHeight = 0xF286ADA4; // 444
    public const uint KPictureCurrentData = 0x48064544; // byte[]

    private const uint KMysteryGiftRecords = 0xBEC26CD5; // object, stores the majority of received card?
    private const uint KNPLNUserID = 0x6CF6C183; // NEX no longer being used, now using NPLN

    private const uint KNightRoyalePostBattleRewards = 0x356087AD; // object
    private const uint KNightRoyaleTrainerStatus = 0x718B8CB1; // object
    private const uint KNightRoyaleBonusCards = 0x2A07F494; // object

    private const uint KStatusMable = 0x85DBDCE9; // Mable Overall Status

    private const uint KDistortionTimeRemain = 0x84D0F3CA;
    private const uint KDonuts = 0xBE007476; // object: donut[999], sizeof = 0x48
    private const uint KDonutDistortionInUse = 0x25335B2A; // Inside distortion, this is the currently used donut
    public const uint KHyperspaceSurveyPoints = 0x0235471C;
    public const uint KStreetName = 0xBCCE00D6; // 0x26 string = 18 chars + \0
}
