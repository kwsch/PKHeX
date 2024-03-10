using System.Collections.Generic;

namespace PKHeX.Core;

// ReSharper disable UnusedMember.Local
#pragma warning disable IDE0051, RCS1213 // Remove unused private members
public sealed class SaveBlockAccessor8LA(SAV8LA sav) : SCBlockAccessor, ISaveBlock8LA
{
    public override IReadOnlyList<SCBlock> BlockInfo { get; } = sav.AllBlocks;
    public Party8a PartyInfo { get; } = new(sav, Block(sav, KParty));
    public Box8 BoxInfo { get; } = new(sav, Block(sav, KBox));
    public MyStatus8a MyStatus { get; } = new(sav, Block(sav, KMyStatus));
    public PokedexSave8a PokedexSave { get; } = new(sav, Block(sav, KZukan));
    public BoxLayout8a BoxLayout { get; } = new(sav, Block(sav, KBoxLayout));
    public MyItem8a Items { get; } = new(sav, Block(sav, KItemRegular));
    public Epoch1970Value AdventureStart { get; } = new(Block(sav, KAdventureStart));
    public Coordinates8a Coordinates { get; } = new(sav, Block(sav, KCoordinates));
    public Epoch1900DateTimeValue LastSaved { get; } = new(Block(sav, KLastSaved));
    public PlayerFashion8a FashionPlayer { get; } = new(sav, Block(sav, KFashionPlayer));
    public PlayTime8b Played { get; } = new(sav, Block(sav, KPlayTime));

    public int DetectRevision() => HasBlock(0x8184EFB4) ? 1 : 0;

    // Arrays (Blocks)
    private const uint KBoxLayout = 0x19722c89; // Box Names
    public const uint KBoxWallpapersUnused = 0x2EB1B190; // Box Wallpapers
    public const uint KItemFavorite = 0x00EF4BAE; // Favorite Item ID bitflags

    // Objects (Blocks)
    private const uint KBox = 0x47E1CEAB; // Box Data
    public const uint KItemRegular = 0x9FE2790A;
    public const uint KItemKey = 0x59A4D0C3;
    public const uint KItemStored = 0x8E434F0D;
    public const uint KItemRecipe = 0xF5D9F4A5;
    private const uint KMysteryGift = 0x99E1625E;
    private const uint KZukan = 0x02168706;
    private const uint KAdventureStart = 0xAEE903A2; // Save File Started
    private const uint KParty = 0x2985fe5d; // Party Data
    private const uint KPlayTime = 0xC4FA7C8C; // Time Played
    private const uint KMyStatus = 0xf25c070e; // Trainer Details
    private const uint KLastSaved = 0x1B1E3D8B; // Last Saved
    private const uint KCoordinates = 0x267DD9DA; // Coordinates
    internal const uint KSpawners = 0x511622B3; // Spawner data
    private const uint KFashionPlayer = 0x6B35BADB; // Player's Current Fashion
    private const uint KFashionUnlockedHat = 0x3ADB8A98;
    private const uint KFashionUnlockedTop = 0x82D57F17;
    private const uint KFashionUnlockedBottoms = 0x11B37EC9;
    private const uint KFashionUnlockedOverall = 0x45851092;
    private const uint KFashionUnlockedShoes = 0x636A5ABD;
    private const uint KFashionUnlockedGlasses = 0x58AB6233;
    public const uint KMassOutbreak = 0x1E0F1BA3;
    public const uint KMassiveMassOutbreak = 0x7799EB86;
    private const uint KCaptureRecords = 0x6506EE96; // 1000 entries, 0x1C each
    private const uint KPlayRecords = 0x549B6033; // 0x18 per entry, first 8 bytes always 01, u64 fnv hash of entry, last 8 bytes value.
    private const uint KOtherPlayerLostSatchels = 0x05E7EBEB;
    private const uint KMyLostSatchels = 0xC5D7112B;
    private const uint KNobleRematchRecords = 0xB9252862; // Best times of Noble rematches

    // Values
    public const uint KCurrentBox = 0x017C3CBB; // U8 Box Index
    public const uint KBoxesUnlocked = 0x71825204; // U8

    private const uint KVolumeBGM = 0xF8154AC9; // U32 Background Music volume control (0-10)
    private const uint KVolumeSFX = 0x62F05895; // U32 Sound Effects volume control (0-10)
    private const uint KVolumeCry = 0x1D482A63; // U32 Pokémon Cries volume control (0-10)

    private const uint KOptionTextSpeed = 0x92EB0306; // U32 text speed (0 = Slow, 1 = Normal, 2 = Fast, 3 = Instant)
    private const uint KOptionCameraVertical = 0x2846B7DB; // U32 vertical camera controls (0 = Normal, 1 = Inverted)
    private const uint KOptionCameraHorizontal = 0x7D249649; // U32 horizontal camera controls (0 = Normal, 1 = Inverted)
    private const uint KOptionCameraSensitivity = 0x22DEF108; // U32 camera sensitivity (0-4)
    private const uint KOptionMotionSensitivity = 0x82AD5F84; // U32 motion sensitivity (0-3)
    private const uint KOptionAutosave = 0xB027F396; // U32 Autosave (0 = Enabled, 1 = Disabled)
    private const uint KOptionToggleHUD = 0xF62D79D3; // U32 HUD Toggling (0 = Enabled, 1 = Disabled)
    private const uint KOptionZRButtonConfirmation = 0x4D7EADDD; // U32 ZR Button confirmation (0 = Enabled, 1 = Disabled)
    private const uint KOptionDynamicRange = 0xA4317061; // U32 Dynamic Range (0 = Wide, 1 = Narrow)

    public const uint KGameLanguage = 0x0BFDEBA1; // U32 Game Language
    public const uint KMoney = 0x3279D927; // U32 Money
    public const uint KMeritCurrent = 0x9D5D1CA5; // U32 Current Merit Points
    public const uint KMeritEarnedTotal = 0xC25B0D5A; // U32 Merit Points Earned
    public const uint KSatchelUpgrades = 0x75CE2CF6; // U32 Satchel Upgrades (0-39)
    public const uint KExpeditionTeamRank = 0x50FE632A; // U32 Galaxy Expedition Team Rank (0-10)
    private const uint KTotalUnownCaptured = 0x3EBEE1A7; // U32 Unown Captured (0-28)
    private const uint KStealthSpray = 0x385F9860; // U32 time remaining on active Stealth Spray (0-60000 in milliseconds)

    private const uint KRepelUnused = 0x9ec079da; // U16 Repel Steps remaining

    private const uint KWispsFoundArea00 = 0x8B18ADE5; // U32 Wisps obtained in Jubilife Village (0-7)
    private const uint KWispsFoundArea01 = 0x8B18AC32; // U32 Wisps obtained in Obsidian Fieldlands (0-20)
    private const uint KWispsFoundArea02 = 0x8B18AA7F; // U32 Wisps obtained in Crimson Mirelands (0-20)
    private const uint KWispsFoundArea03 = 0x8B18A8CC; // U32 Wisps obtained in Cobalt Coastlands (0-20)
    private const uint KWispsFoundArea04 = 0x8B18A719; // U32 Wisps obtained in Coronet Highlands (0-20)
    private const uint KWispsFoundArea05 = 0x8B18A566; // U32 Wisps obtained in Alabaster Icelands (0-20)
    private const uint KWispsFoundTotal = 0xB79EF1FE; // U32 total Wisps obtained (0-107)
    private const uint KWispsReported = 0x8F0D8720; // U32 Wisps reported to Vessa (0-107)
    private const uint KRecordTargetPractice = 0xA69E079B; // U32 High score for Target Practice minigame (Practice Field)
    private const uint KRecordLostSatchelsFound = 0x4AAF7FBE; // U32 Satchels retrieved for NPCs and other players
    private const uint KRecordOwnSatchelsRetrieved = 0x8C46768E; // U32 Satchels other players retrieved for you
    private const uint KStarterChoice = 0x6960C6EF; // U32 0=Rowlet, 1=Cyndaquil, 2=Oshawott

    private const uint KRecordEternalBattleReverie = 0xEB550C12; // U32 Highest streak for Eternal Battle Reverie

    // Flags
    private const uint KEnableSpawnerSpiritomb = 0x2DC7E4CC; // FSYS_MKRG_100_SPAWN
    private const uint KEnableSpawnerUxie = 0x9EC1F2C4; // FEVE_YUKUSII_ENCOUNT_ENABLE
    private const uint KEnableSpawnerMesprit = 0xEF5C95D8; // FEVE_EMURITTO_ENCOUNT_ENABLE
    private const uint KEnableSpawnerAzelf = 0xD038BD89; // FEVE_AGUNOMU_ENCOUNT_ENABLE
    private const uint KEnableSpawnerHeatran = 0x3F6301AC; // FEVE_HIIDORAN__ENCOUNT_ENABLE
    private const uint KEnableSpawnerCresselia = 0x85134D02; // FEVE_KURESERIA_ENCOUNT_ENABLE
    private const uint KEnableSpawnerDarkrai = 0xEE027506; // FSYS_SPAWN_START_DARKRAI
    private const uint KEnableSpawnerShaymin = 0x0DCE6659; // FSYS_SPAWN_START_SHAYMIN
    private const uint KEnableSpawnerTornadus = 0x07D8EC38; // FSYS_SPAWN_START_TORNELOS
    private const uint KEnableSpawnerThundurus = 0x136D3D88; // FSYS_SPAWN_START_VOLTOLOS
    private const uint KEnableSpawnerLandorus = 0xE079071B; // FSYS_SPAWN_START_LANDLOS
    private const uint KEnableSpawnerEnamorus = 0x3AA64045; // FSYS_SPAWN_START_FAIRTOLOS

    private const uint KDisableSpawnerSpiritomb = 0x0AB16F69; // FSYS_MKRG_VALID_SPAWN
    private const uint KDisableSpawnerGiratina = 0x40B908EC; // FMAP_CANNOT_RESPAWN_GIRATINA
    private const uint KDisableSpawnerPhione01 = 0x3C4DB3BE; // FMAP_CANNOT_RESPAWN_PHIONE
    private const uint KDisableSpawnerPhione02 = 0xF6B469D3; // FMAP_CANNOT_RESPAWN_PHIONE_2
    private const uint KDisableSpawnerPhione03 = 0xF6B46820; // FMAP_CANNOT_RESPAWN_PHIONE_3
    private const uint KDisableSpawnerManaphy = 0xBBE677C7; // FMAP_CANNOT_RESPAWN_MANAPHY
    private const uint KDisableSpawnerDarkrai = 0x8AE49E85; // FMAP_CANNOT_RESPAWN_DARKRAI
    private const uint KDisableSpawnerShaymin = 0xF873BBFA; // FMAP_CANNOT_RESPAWN_SHAYMIN
    private const uint KDisableSpawnerTornadus = 0xC8AA3D69; // FMAP_CANNOT_RESPAWN_TORNELOS
    private const uint KDisableSpawnerThundurus = 0x79E259CD; // FMAP_CANNOT_RESPAWN_VOLTOLOS
    private const uint KDisableSpawnerLandorus = 0xD613F320; // FMAP_CANNOT_RESPAWN_LANDLOS
    private const uint KDisableSpawnerEnamorus = 0xE50F4B4E; // FMAP_CANNOT_RESPAWN_FAIRTOLOS

    private const uint KReceivedAlolanVulpix = 0xAC90C782; // FEVE_POKE_SUB092_GET

    private const uint KCanRideWyrdeer = 0x47365FE8; // FSYS_RIDE_OPEN_01
  //private const uint KCanRideUnused02 = 0x47366501; // FSYS_RIDE_OPEN_02
    private const uint KCanRideUrsaluna = 0x4736634E; // FSYS_RIDE_OPEN_03
  //private const uint KCanRideUnused04 = 0x47366867; // FSYS_RIDE_OPEN_04
    private const uint KCanRideBasculegion = 0x473666B4; // FSYS_RIDE_OPEN_05
  //private const uint KCanRideUnused06 = 0x47366BCD; // FSYS_RIDE_OPEN_06
    private const uint KCanRideSneasler = 0x47366A1A; // FSYS_RIDE_OPEN_07
  //private const uint KCanRideUnused08 = 0x47365403; // FSYS_RIDE_OPEN_08
  //private const uint KCanRideUnused09 = 0x47365250; // FSYS_RIDE_OPEN_09
    private const uint KCanRideBraviary = 0x47334812; // FSYS_RIDE_OPEN_10

    private const uint KDefeatedLordKleavor = 0x96774421; // FSYS_NS_01_CLEARED
    private const uint KDefeatedLadyLilligant = 0x3A50000C; // FSYS_NS_02_CLEARED
    private const uint KDefeatedLordArcanine = 0xA5981A37; // FSYS_NS_03_CLEARED
    private const uint KDefeatedLordElectrode = 0x6EF3C712; // FSYS_NS_04_CLEARED
    private const uint KDefeatedLordAvalugg = 0x424E9F0D; // FSYS_NS_05_CLEARED
    private const uint KDefeatedOriginDialga = 0x5185ADC0; // FSYS_NS_D_CLEARED (Only applicable if Pearl Clan was chosen)
    private const uint KDefeatedOriginPalkia = 0x5E5BFD94; // FSYS_NS_P_CLEARED (Only applicable if Diamond Clan was chosen)
    private const uint KDefeatedArceus = 0x2F91EFD3; // FSYS_SCENARIO_CLEARED_URA
    private const uint KCompletedPokedex = 0xD985E1C2; // FEVE_EV110100_END (Enables using Azure Flute to reach Arceus)
    private const uint KPerfectedPokedex = 0x98ED661E; //  FSYS_POKEDEX_COMPLETE_WITHOUT_EXCEPTION
    private const uint KUnlockedUnownNotes = 0xC9127B4E; // FSYS_UNNN_ENABLE_PLACEMENT
    private const uint KUnlockedLostAndFound = 0xFE837926; // FSYS_LOSTBAG_SEARCH_REQUEST_ENABLE
    private const uint KUnlockedMassRelease = 0x0C16BEF4; // FSYS_APP_BOX_SUMFREE_ENABLE
    private const uint KUnlockedDistortions = 0x7611BFC3; // FSYS_WORMHOLE_OPEN
    private const uint KCanFastTravel = 0xFE98F73F; // FSYS_CAN_USE_FAST_TRAVEL
    private const uint KUnlockedArea01 = 0x24C0252D; // FSYS_AREA_01_OPEN
    private const uint KUnlockedArea02 = 0x1599C206; // FSYS_AREA_02_OPEN
    private const uint KUnlockedArea03 = 0x408DE1D3; // FSYS_AREA_03_OPEN
    private const uint KUnlockedArea04 = 0x8C062C9C; // FSYS_AREA_04_OPEN
    private const uint KUnlockedArea05 = 0xC08D4C69; // FSYS_AREA_05_OPEN
    private const uint KUnlockedArea06 = 0x76350E52; // FSYS_AREA_06_OPEN
    private const uint KAutoConnectInternet = 0xAFA034A5;
    private const uint KCompletedEternalBattleReverie = 0xD6BD5654; // Mark displayed on save card; achieved by a 50 streak

    private const uint KHasPlayRecordsBDSP = 0x52CE2052; // FSYS_SAVEDATA_LINKAGE_DEL_01
    private const uint KHasPlayRecordsSWSH = 0x530EF0B9; // FSYS_SAVEDATA_LINKAGE_ORI_01
    private const uint KHasPlayRecordsLGPE = 0x6CFA9468; // FSYS_SAVEDATA_LINKAGE_BEL_01

    private const uint KChoseDiamondClanLeader = 0x669B325F; // Choice of Adaman over Irida
    private const uint KHasApplianceElectricFan = 0xC734C80F; // Access to Fan-Rotom
    private const uint KHasApplianceWashingMachine = 0x62872639; // Access to Wash-Rotom
    private const uint KHasApplianceLawnMower = 0xFB87E941; // Access to Mow-Rotom
    private const uint KHasApplianceMicrowaveOven = 0xD9A315A0; // Access to Heat-Rotom
    private const uint KHasApplianceRefrigerator = 0xAE868040; // Access to Frost-Rotom
    private const uint KReceivedRemainderStarters = 0x8D45EB90; // Got Starters that weren't chosen
    private const uint KReceivedRemainderStarterRowlet = 0x4570A16B; // Combine with other 2 and KReceivedRemainderStarers to re-activate
    private const uint KReceivedRemainderStarterCyndaquil = 0x602C2CD6; // Combine with other 2 and KReceivedRemainderStarers to re-activate
    private const uint KReceivedRemainderStarterOshawott = 0xAFCE7320; // Combine with other 2 and KReceivedRemainderStarers to re-activate

    public const uint KUnlockedSecretBox01 = 0xF224CA8E; // FSYS_SECRET_BOX_01_OPEN
    public const uint KUnlockedSecretBox02 = 0x06924515; // FSYS_SECRET_BOX_02_OPEN
    public const uint KUnlockedSecretBox03 = 0xF67C6DC8; // FSYS_SECRET_BOX_03_OPEN
}
