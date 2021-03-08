using System;
using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core
{
    /// <summary>
    /// Generation 3 <see cref="SaveFile"/> object.
    /// </summary>
    public sealed class SAV3 : SaveFile, ILangDeviantSave
    {
        protected internal override string ShortSummary => $"{OT} ({Version}) - {PlayTimeString}";
        public override string Extension => ".sav";

        public int SaveRevision => Japanese ? 0 : 1;
        public string SaveRevisionString => Japanese ? "J" : "U";
        public bool Japanese { get; }
        public bool Korean => false;
        public bool IndeterminateGame => Version == GameVersion.Unknown;

        // Similar to future games, the Generation 3 Mainline save files are comprised of two separate objects:
        // Object 1 - Small Block, containing misc configuration data & the Pokédex.
        // Object 2 - Large Block, containing everything else that isn't PC Storage system data.
        // Object 3 - Storage Block, containing all the data for the PC storage system.

        // When the objects are serialized to the savedata, the game breaks up each object into chunks < 0x1000 bytes.
        // Each serialized save occupies 14 chunks; there are a total of two serialized saves.
        // After the serialized save data, there is "extra data", for stuff like Hall of Fame and battle videos.

        private const int SIZE_BLOCK = 0x1000;
        private const int BLOCK_COUNT = 14;
        public const int SIZE_BLOCK_USED = 0xF80;

        private const int COUNT_BOX = 14;
        private const int COUNT_SLOTSPERBOX = 30;
        // Use the largest of structure sizes, as zeroes being fed into checksum function don't change the value.
        private const int SIZE_SMALL = 0xF2C; // maximum size for R/S/E/FR/LG structures
        private const int SIZE_LARGE = (3 * 0xF80) + 0xF08; // maximum size for R/S/E/FR/LG structures

        public readonly byte[] Small = new byte[SIZE_SMALL];
        public readonly byte[] Large = new byte[SIZE_LARGE];
        public readonly byte[] Storage = new byte[SIZE_PC];
        protected override byte[] BoxBuffer => Storage;
        protected override byte[] PartyBuffer => Large;

        // 0x83D0
        private const int SIZE_PC = sizeof(int) // Current Box
                                    + (COUNT_BOX * (COUNT_SLOTSPERBOX * PokeCrypto.SIZE_3STORED)) // Slots
                                    + (COUNT_BOX * (8 + 1)) // Box Names
                                    + (COUNT_BOX * 1); // Box Wallpapers

        private static readonly ushort[] chunkLength =
        {
            0xf2c, // 0 | Small Block (Trainer Info) [0x890 RS, 0xf24 FR/LG]
            0xf80, // 1 | Large Block Part 1
            0xf80, // 2 | Large Block Part 2
            0xf80, // 3 | Large Block Part 3
            0xf08, // 4 | Large Block Part 4 [0xc40 RS, 0xee8 FR/LG]
            0xf80, // 5 | PC Block 0
            0xf80, // 6 | PC Block 1
            0xf80, // 7 | PC Block 2
            0xf80, // 8 | PC Block 3
            0xf80, // 9 | PC Block 4
            0xf80, // A | PC Block 5
            0xf80, // B | PC Block 6
            0xf80, // C | PC Block 7
            0x7d0  // D | PC Block 8
        };

        private PersonalTable _personal;
        public override PersonalTable Personal => _personal;
        public override IReadOnlyList<ushort> HeldItems => Legal.HeldItems_RS;

        public SAV3(GameVersion version = GameVersion.FRLG, bool japanese = false)
        {
            Version = version switch
            {
                GameVersion.FR or GameVersion.LG => GameVersion.FRLG,
                GameVersion.R or GameVersion.S => GameVersion.RS,
                _ => version
            };
            _personal = SaveUtil.GetG3Personal(Version);
            Japanese = japanese;

            BlockOrder = Array.Empty<short>();

            LegalKeyItems = Version switch
            {
                GameVersion.RS => Legal.Pouch_Key_RS,
                GameVersion.E => Legal.Pouch_Key_E,
                _ => Legal.Pouch_Key_FRLG
            };
            PokeDex = 0x18;
            SeenFlagOffsets = Array.Empty<int>();

            Initialize();
            ClearBoxes();
        }

        public SAV3(byte[] data, GameVersion versionOverride = GameVersion.Any) : base(data)
        {
            LoadBlocks(out BlockOrder);

            // Copy chunk to the allocated location
            LoadBlocks(Small, 0, 1);
            LoadBlocks(Large, 1, 5);
            LoadBlocks(Storage, 5, BLOCK_COUNT);

            Version = versionOverride != GameVersion.Any ? versionOverride : GetVersion(Small);
            _personal = SaveUtil.GetG3Personal(Version);

            // Japanese games are limited to 5 character OT names; any unused characters are 0xFF.
            // 5 for JP, 7 for INT. There's always 1 terminator, thus we can check 0x6-0x7 being 0xFFFF = INT
            // OT name is stored at the top of the first block.
            Japanese = BitConverter.ToInt16(Small, 0x6) == 0;

            LegalKeyItems = Version switch
            {
                GameVersion.RS => Legal.Pouch_Key_RS,
                GameVersion.E => Legal.Pouch_Key_E,
                _ => Legal.Pouch_Key_FRLG
            };

            PokeDex = 0x18;
            SeenFlagOffsets = Version switch
            {
                GameVersion.RS => new[] { 0x938, 0x3A8C },
                GameVersion.E => new[] { 0x988, 0x3B24 },
                _ => new[] { 0x5F8, 0x3A18 }
            };

            Initialize();
        }

        private void Initialize()
        {
            Box = 0;

            switch (Version)
            {
                case GameVersion.RS:
                    OFS_PCItem = 0x0498;
                    OFS_PouchHeldItem = 0x0560;
                    OFS_PouchKeyItem = 0x05B0;
                    OFS_PouchBalls = 0x0600;
                    OFS_PouchTMHM = 0x0640;
                    OFS_PouchBerry = 0x0740;
                    EventFlag = 0x1220;
                    EventConst = 0x1340;
                    OFS_Decorations = 0x26A0;
                    DaycareOffset = 0x2F9C;
                    break;
                case GameVersion.E:
                    OFS_PCItem = 0x0498;
                    OFS_PouchHeldItem = 0x0560;
                    OFS_PouchKeyItem = 0x05D8;
                    OFS_PouchBalls = 0x0650;
                    OFS_PouchTMHM = 0x0690;
                    OFS_PouchBerry = 0x0790;
                    EventFlag = 0x1270;
                    EventConst = 0x139C;
                    OFS_Decorations = 0x2734;
                    DaycareOffset = 0x3030;
                    break;
                case GameVersion.FRLG:
                    OFS_PCItem = 0x0298;
                    OFS_PouchHeldItem = 0x0310;
                    OFS_PouchKeyItem = 0x03B8;
                    OFS_PouchBalls = 0x0430;
                    OFS_PouchTMHM = 0x0464;
                    OFS_PouchBerry = 0x054C;
                    EventFlag = 0xEE0;
                    EventConst = 0x1000;
                    DaycareOffset = 0x2F80;
                    break;
                default:
                    throw new ArgumentException(nameof(Version));
            }

            LoadEReaderBerryData();

            // Sanity Check SeenFlagOffsets -- early saves may not have block 4 initialized yet
            SeenFlagOffsets = SeenFlagOffsets.Where(z => z >= 0).ToArray();
        }

        private void LoadBlocks(byte[] dest, short start, short end)
        {
            for (short i = start; i < end; i++)
            {
                int blockIndex = Array.IndexOf(BlockOrder, i);
                if (blockIndex == -1) // block empty
                    continue;

                var sOfs = (blockIndex * SIZE_BLOCK) + ABO;
                var dOfs = (i - start) * SIZE_BLOCK_USED;
                var count = chunkLength[i];
                Buffer.BlockCopy(Data, sOfs, dest, dOfs, count);
            }
        }

        private void SaveBlocks(byte[] dest, short start, short end)
        {
            for (short i = start; i < end; i++)
            {
                int blockIndex = Array.IndexOf(BlockOrder, i);
                if (blockIndex == -1) // block empty
                    continue;

                var sOfs = (blockIndex * SIZE_BLOCK) + ABO;
                var dOfs = (i - start) * SIZE_BLOCK_USED;
                var count = chunkLength[i];
                Buffer.BlockCopy(dest, dOfs, Data, sOfs, count);
            }
        }

        private void LoadBlocks(out short[] blockOrder)
        {
            var o1 = GetBlockOrder(0);
            if (Data.Length > SaveUtil.SIZE_G3RAWHALF)
            {
                var o2 = GetBlockOrder(0xE000);
                ActiveSAV = GetActiveSaveIndex(o1, o2);
                blockOrder = ActiveSAV == 0 ? o1 : o2;
            }
            else
            {
                ActiveSAV = 0;
                blockOrder = o1;
            }
        }

        private short[] GetBlockOrder(int ofs)
        {
            short[] order = new short[BLOCK_COUNT];
            for (int i = 0; i < BLOCK_COUNT; i++)
                order[i] = BitConverter.ToInt16(Data, ofs + (i * SIZE_BLOCK) + 0xFF4);
            return order;
        }

        private int GetActiveSaveIndex(short[] BlockOrder1, short[] BlockOrder2)
        {
            int zeroBlock1 = Array.IndexOf(BlockOrder1, (short)0);
            int zeroBlock2 = Array.IndexOf(BlockOrder2, (short)0);
            if (zeroBlock2 < 0)
                return 0;
            if (zeroBlock1 < 0)
                return 1;
            var count1 = BitConverter.ToUInt32(Data, (zeroBlock1 * SIZE_BLOCK) + 0x0FFC);
            var count2 = BitConverter.ToUInt32(Data, (zeroBlock2 * SIZE_BLOCK) + 0xEFFC);
            return count1 > count2 ? 0 : 1;
        }

        public static GameVersion GetVersion(byte[] data, int offset = 0)
        {
            uint GameCode = BitConverter.ToUInt32(data, offset + 0xAC);
            switch (GameCode)
            {
                case 1: return GameVersion.FRLG; // fixed value
                case 0: return GameVersion.RS; // no battle tower record data
                case uint.MaxValue: return GameVersion.Unknown; // what a hack
                default:
                    // Ruby doesn't set data as far down as Emerald.
                    // 00 FF 00 00 00 00 00 00 00 FF 00 00 00 00 00 00
                    // ^ byte pattern in Emerald saves, is all zero in Ruby/Sapphire as far as I can tell.
                    // Some saves have had data @ 0x550
                    if (BitConverter.ToUInt64(data, offset + 0xEE0) != 0)
                        return GameVersion.E;
                    if (BitConverter.ToUInt64(data, offset + 0xEE8) != 0)
                        return GameVersion.E;
                    return GameVersion.RS;
            }
        }

        protected override byte[] GetFinalData()
        {
            // Copy Box data back
            SaveBlocks(Small, 0, 1);
            SaveBlocks(Large, 1, 5);
            SaveBlocks(Storage, 5, BLOCK_COUNT);
            return base.GetFinalData();
        }

        private int ActiveSAV;
        private int ABO => ActiveSAV*SIZE_BLOCK*0xE;
        private readonly short[] BlockOrder;

        // Configuration
        protected override SaveFile CloneInternal() => new SAV3(Write(), Version);

        protected override int SIZE_STORED => PokeCrypto.SIZE_3STORED;
        protected override int SIZE_PARTY => PokeCrypto.SIZE_3PARTY;
        public override PKM BlankPKM => new PK3();
        public override Type PKMType => typeof(PK3);

        public override int MaxMoveID => Legal.MaxMoveID_3;
        public override int MaxSpeciesID => Legal.MaxSpeciesID_3;
        public override int MaxAbilityID => Legal.MaxAbilityID_3;
        public override int MaxItemID => Legal.MaxItemID_3;
        public override int MaxBallID => Legal.MaxBallID_3;
        public override int MaxGameID => Legal.MaxGameID_3;

        public override int BoxCount => 14;
        public override int MaxEV => 255;
        public override int Generation => 3;
        protected override int GiftCountMax => 1;
        public override int OTLength => 7;
        public override int NickLength => 10;
        public override int MaxMoney => 999999;
        protected override int EventFlagMax => 8 * (E ? 300 : 288); // 0x960 E, else 0x900
        protected override int EventConstMax => 0x100;

        public bool E => Version == GameVersion.E;
        public bool FRLG => Version == GameVersion.FRLG;
        public bool RS => Version == GameVersion.RS;

        public override bool HasParty => true;

        public override bool IsPKMPresent(byte[] data, int offset) => PKX.IsPKMPresentGBA(data, offset);

        // Checksums
        protected override void SetChecksums()
        {
            for (int i = 0; i < BLOCK_COUNT; i++)
            {
                int ofs = ABO + (i * SIZE_BLOCK);
                var index = BlockOrder[i];
                if (index == -1)
                    continue;
                int len = chunkLength[index];
                ushort chk = Checksums.CheckSum32(Data, ofs, len);
                BitConverter.GetBytes(chk).CopyTo(Data, ofs + 0xFF6);
            }

            if (State.BAK.Length < SaveUtil.SIZE_G3RAW) // don't update HoF for half-sizes
                return;

            // Hall of Fame Checksums
            {
                ushort chk = Checksums.CheckSum32(Data, 0x1C000, SIZE_BLOCK_USED);
                BitConverter.GetBytes(chk).CopyTo(Data, 0x1CFF4);
            }
            {
                ushort chk = Checksums.CheckSum32(Data, 0x1D000, SIZE_BLOCK_USED);
                BitConverter.GetBytes(chk).CopyTo(Data, 0x1DFF4);
            }
        }

        public override bool ChecksumsValid
        {
            get
            {
                for (int i = 0; i < BLOCK_COUNT; i++)
                {
                    if (!IsChunkValid(i))
                        return false;
                }

                if (State.BAK.Length < SaveUtil.SIZE_G3RAW) // don't check HoF for half-sizes
                    return true;

                if (!IsChunkValidHoF(0x1C000))
                    return false;
                if (!IsChunkValidHoF(0x1D000))
                    return false;
                return true;
            }
        }

        private bool IsChunkValidHoF(int ofs)
        {
            ushort chk = Checksums.CheckSum32(Data, ofs, SIZE_BLOCK_USED);
            return chk == BitConverter.ToUInt16(Data, ofs + 0xFF4);
        }

        private bool IsChunkValid(int i)
        {
            int ofs = ABO + (i * SIZE_BLOCK);
            int len = chunkLength[BlockOrder[i]];
            ushort chk = Checksums.CheckSum32(Data, ofs, len);
            return chk == BitConverter.ToUInt16(Data, ofs + 0xFF6);
        }

        public override string ChecksumInfo
        {
            get
            {
                var list = new List<string>();
                for (int i = 0; i < BLOCK_COUNT; i++)
                {
                    if (!IsChunkValid(i))
                        list.Add($"Block {BlockOrder[i]:00} @ {i*SIZE_BLOCK:X5} invalid.");
                }

                if (State.BAK.Length > SaveUtil.SIZE_G3RAW) // don't check HoF for half-sizes
                {
                    if (!IsChunkValidHoF(0x1C000))
                        list.Add("HoF Block 1 invalid.");
                    if (!IsChunkValidHoF(0x1D000))
                        list.Add("HoF Block 2 invalid.");
                }
                return list.Count != 0 ? string.Join(Environment.NewLine, list) : "Checksums are valid.";
            }
        }

        // Trainer Info
        public override GameVersion Version { get; protected set; }

        public uint SecurityKey => Version switch
        {
            GameVersion.E => BitConverter.ToUInt32(Small, 0xAC),
            GameVersion.FRLG => BitConverter.ToUInt32(Small, 0xF20),
            _ => 0u
        };

        public override string OT
        {
            get => GetString(Small, 0, 0x10);
            set
            {
                int len = Japanese ? 5 : OTLength;
                SetString(value, len, PadToSize: len, PadWith: 0xFF).CopyTo(Small, 0);
            }
        }

        public override int Gender
        {
            get => Small[8];
            set => Small[8] = (byte)value;
        }

        public override int TID
        {
            get => BitConverter.ToUInt16(Small, 0xA);
            set => BitConverter.GetBytes((ushort)value).CopyTo(Small, 0xA);
        }

        public override int SID
        {
            get => BitConverter.ToUInt16(Small, 0xC);
            set => BitConverter.GetBytes((ushort)value).CopyTo(Small, 0xC);
        }

        public override int PlayedHours
        {
            get => BitConverter.ToUInt16(Small, 0xE);
            set => BitConverter.GetBytes((ushort)value).CopyTo(Small, 0xE);
        }

        public override int PlayedMinutes
        {
            get => Small[0x10];
            set => Small[0x10] = (byte)value;
        }

        public override int PlayedSeconds
        {
            get => Small[0x11];
            set => Small[0x11] = (byte)value;
        }

        public int PlayedFrames
        {
            get => Small[0x12];
            set => Small[0x12] = (byte)value;
        }

        public override bool GetEventFlag(int flagNumber)
        {
            if (flagNumber >= EventFlagMax)
                throw new ArgumentException($"Event Flag to get ({flagNumber}) is greater than max ({EventFlagMax}).");

            var start = EventFlag;
            return GetFlag(start + (flagNumber >> 3), flagNumber & 7);
        }

        public override void SetEventFlag(int flagNumber, bool value)
        {
            if (flagNumber >= EventFlagMax)
                throw new ArgumentException($"Event Flag to set ({flagNumber}) is greater than max ({EventFlagMax}).");

            var start = EventFlag;
            SetFlag(start + (flagNumber >> 3), flagNumber & 7, value);
        }

        public override bool GetFlag(int offset, int bitIndex) => FlagUtil.GetFlag(Large, offset, bitIndex);
        public override void SetFlag(int offset, int bitIndex, bool value) => FlagUtil.SetFlag(Large, offset, bitIndex, value);

        public ushort GetEventConst(int index) => BitConverter.ToUInt16(Large, EventConst + (index * 2));
        public void SetEventConst(int index, ushort value) => BitConverter.GetBytes(value).CopyTo(Large, EventConst + (index * 2));

        public override ushort[] GetEventConsts()
        {
            ushort[] Constants = new ushort[EventConstMax];
            for (int i = 0; i < Constants.Length; i++)
                Constants[i] = GetEventConst(i);
            return Constants;
        }

        public override void SetEventConsts(ushort[] value)
        {
            if (value.Length != EventConstMax)
                return;

            for (int i = 0; i < value.Length; i++)
                SetEventConst(i, value[i]);
        }

        public int Badges
        {
            get
            {
                int startFlag = BadgeFlagStart;
                int val = 0;
                for (int i = 0; i < 8; i++)
                {
                    if (GetEventFlag(startFlag + i))
                        val |= 1 << i;
                }

                return val;
            }
            set
            {
                int startFlag = BadgeFlagStart;
                for (int i = 0; i < 8; i++)
                    SetEventFlag(startFlag + i, (value & (1 << i)) != 0);
            }
        }

        private int BadgeFlagStart
        {
            get
            {
                if (Version == GameVersion.FRLG)
                    return 0x820;
                if (Version == GameVersion.RS)
                    return 0x807;
                return 0x867; // emerald
            }
        }

        public override uint Money
        {
            get
            {
                switch (Version)
                {
                    case GameVersion.RS:
                    case GameVersion.E: return BitConverter.ToUInt32(Large, 0x0490) ^ SecurityKey;
                    case GameVersion.FRLG: return BitConverter.ToUInt32(Large, 0x0290) ^ SecurityKey;
                    default: return 0;
                }
            }
            set
            {
                switch (Version)
                {
                    case GameVersion.RS:
                    case GameVersion.E: BitConverter.GetBytes(value ^ SecurityKey).CopyTo(Large, 0x0490); break;
                    case GameVersion.FRLG: BitConverter.GetBytes(value ^ SecurityKey).CopyTo(Large, 0x0290); break;
                }
            }
        }

        public uint Coin
        {
            get
            {
                switch (Version)
                {
                    case GameVersion.RS:
                    case GameVersion.E: return (ushort)(BitConverter.ToUInt16(Large, 0x0494) ^ SecurityKey);
                    case GameVersion.FRLG: return (ushort)(BitConverter.ToUInt16(Large, 0x0294) ^ SecurityKey);
                    default: return 0;
                }
            }
            set
            {
                if (value > 9999)
                    value = 9999;
                switch (Version)
                {
                    case GameVersion.RS:
                    case GameVersion.E: BitConverter.GetBytes((ushort)(value ^ SecurityKey)).CopyTo(Large, 0x0494); break;
                    case GameVersion.FRLG: BitConverter.GetBytes((ushort)(value ^ SecurityKey)).CopyTo(Large, 0x0294); break;
                }
            }
        }

        public uint BP
        {
            get => BitConverter.ToUInt16(Small, 0xEB8);
            set
            {
                if (value > 9999)
                    value = 9999;
                BitConverter.GetBytes((ushort)value).CopyTo(Small, 0xEB8);
            }
        }

        public uint BPEarned
        {
            get => BitConverter.ToUInt16(Small, 0xEBA);
            set
            {
                if (value > 65535)
                    value = 65535;
                BitConverter.GetBytes((ushort)value).CopyTo(Small, 0xEBA);
            }
        }

        public uint BerryPowder
        {
            get
            {
                if (Version != GameVersion.FRLG)
                    return 0;
                return BitConverter.ToUInt32(Small, 0xAF8) ^ SecurityKey;
            }
            set
            {
                if (Version != GameVersion.FRLG)
                    return;
                SetData(Small, BitConverter.GetBytes(value ^ SecurityKey), 0xAF8);
            }
        }

        private readonly ushort[] LegalKeyItems;
        private static ushort[] LegalItems => Legal.Pouch_Items_RS;
        private static ushort[] LegalBalls => Legal.Pouch_Ball_RS;
        private static ushort[] LegalTMHMs => Legal.Pouch_TMHM_RS;
        private static ushort[] LegalBerries => Legal.Pouch_Berries_RS;

        private int OFS_PCItem, OFS_PouchHeldItem, OFS_PouchKeyItem, OFS_PouchBalls, OFS_PouchTMHM, OFS_PouchBerry, OFS_Decorations;

        public override IReadOnlyList<InventoryPouch> Inventory
        {
            get
            {
                int max = Version == GameVersion.FRLG ? 999 : 99;
                var PCItems = new [] {LegalItems, LegalKeyItems, LegalBalls, LegalTMHMs, LegalBerries}.SelectMany(a => a).ToArray();
                InventoryPouch[] pouch =
                {
                    new InventoryPouch3(InventoryType.Items, LegalItems, max, OFS_PouchHeldItem, (OFS_PouchKeyItem - OFS_PouchHeldItem)/4),
                    new InventoryPouch3(InventoryType.KeyItems, LegalKeyItems, 1, OFS_PouchKeyItem, (OFS_PouchBalls - OFS_PouchKeyItem)/4),
                    new InventoryPouch3(InventoryType.Balls, LegalBalls, max, OFS_PouchBalls, (OFS_PouchTMHM - OFS_PouchBalls)/4),
                    new InventoryPouch3(InventoryType.TMHMs, LegalTMHMs, max, OFS_PouchTMHM, (OFS_PouchBerry - OFS_PouchTMHM)/4),
                    new InventoryPouch3(InventoryType.Berries, LegalBerries, 999, OFS_PouchBerry, Version == GameVersion.FRLG ? 43 : 46),
                    new InventoryPouch3(InventoryType.PCItems, PCItems, 999, OFS_PCItem, (OFS_PouchHeldItem - OFS_PCItem)/4),
                };
                foreach (var p in pouch)
                {
                    if (p.Type != InventoryType.PCItems)
                        ((InventoryPouch3)p).SecurityKey = SecurityKey;
                }
                return pouch.LoadAll(Large);
            }
            set => value.SaveAll(Large);
        }

        private int DaycareSlotSize => RS ? SIZE_STORED : SIZE_STORED + 0x3C; // 0x38 mail + 4 exp
        public override int DaycareSeedSize => E ? 8 : 4; // 32bit, 16bit
        public override uint? GetDaycareEXP(int loc, int slot) => BitConverter.ToUInt32(Large, GetDaycareEXPOffset(slot));
        public override void SetDaycareEXP(int loc, int slot, uint EXP) => BitConverter.GetBytes(EXP).CopyTo(Large, GetDaycareEXPOffset(slot));
        public override bool? IsDaycareOccupied(int loc, int slot) => IsPKMPresent(Large, GetDaycareSlotOffset(loc, slot));
        public override void SetDaycareOccupied(int loc, int slot, bool occupied) { /* todo */ }
        public override int GetDaycareSlotOffset(int loc, int slot) => DaycareOffset + (slot * DaycareSlotSize);

        private int EggEventFlag => GameVersion.FRLG.Contains(Version) ? 0x266 : 0x86;
        public override bool? IsDaycareHasEgg(int loc) => GetEventFlag(EggEventFlag);
        public override void SetDaycareHasEgg(int loc, bool hasEgg) => SetEventFlag(EggEventFlag, hasEgg);

        private int GetDaycareEXPOffset(int slot)
        {
            if (Version == GameVersion.RS)
                return GetDaycareSlotOffset(0, 2) + (2 * 0x38) + (4 * slot); // consecutive vals, after both consecutive slots & 2 mail
            return GetDaycareSlotOffset(0, slot + 1) - 4; // @ end of each pkm slot
        }

        public override string GetDaycareRNGSeed(int loc)
        {
            if (Version == GameVersion.E)
                return BitConverter.ToUInt32(Large, GetDaycareSlotOffset(0, 2)).ToString("X8"); // after the 2 slots, before the step counter
            return BitConverter.ToUInt16(Large, GetDaycareEXPOffset(2)).ToString("X4"); // after the 2nd slot EXP, before the step counter
        }

        public override void SetDaycareRNGSeed(int loc, string seed)
        {
            if (Version == GameVersion.E) // egg pid
            {
                var val = Util.GetHexValue(seed);
                BitConverter.GetBytes(val).CopyTo(Large, GetDaycareSlotOffset(0, 2));
            }
            // egg pid half
            {
                var val = (ushort)Util.GetHexValue(seed);
                BitConverter.GetBytes(val).CopyTo(Large, GetDaycareEXPOffset(2));
            }
        }

        // Storage
        public override int PartyCount
        {
            get
            {
                int ofs = 0x34;
                if (GameVersion.FRLG != Version)
                    ofs += 0x200;
                return Large[ofs];
            }
            protected set
            {
                int ofs = 0x34;
                if (GameVersion.FRLG != Version)
                    ofs += 0x200;
                Large[ofs] = (byte)value;
            }
        }

        public override int GetBoxOffset(int box)
        {
            return Box + 4 + (SIZE_STORED * box * 30);
        }

        public override int GetPartyOffset(int slot)
        {
            int ofs = 0x38;
            if (GameVersion.FRLG != Version)
                ofs += 0x200;
            return ofs + (SIZE_PARTY * slot);
        }

        public override int CurrentBox
        {
            get => Storage[0];
            set => Storage[0] = (byte)value;
        }

        public override int GetBoxWallpaper(int box)
        {
            if (box > COUNT_BOX)
                return box;
            int offset = GetBoxWallpaperOffset(box);
            return Storage[offset];
        }

        private const int COUNT_BOXNAME = 8 + 1;

        public override void SetBoxWallpaper(int box, int value)
        {
            if (box > COUNT_BOX)
                return;
            int offset = GetBoxWallpaperOffset(box);
            Storage[offset] = (byte)value;
        }

        protected override int GetBoxWallpaperOffset(int box)
        {
            int offset = GetBoxOffset(COUNT_BOX);
            offset += (COUNT_BOX * COUNT_BOXNAME) + box;
            return offset;
        }

        public override string GetBoxName(int box)
        {
            int offset = GetBoxOffset(COUNT_BOX);
            return StringConverter3.GetString3(Storage, offset + (box * COUNT_BOXNAME), COUNT_BOXNAME, Japanese);
        }

        public override void SetBoxName(int box, string value)
        {
            int offset = GetBoxOffset(COUNT_BOX);
            SetString(value, COUNT_BOXNAME - 1).CopyTo(Storage, offset + (box * COUNT_BOXNAME));
        }

        protected override PKM GetPKM(byte[] data)
        {
            return new PK3(data);
        }

        protected override byte[] DecryptPKM(byte[] data)
        {
            return PokeCrypto.DecryptArray3(data);
        }

        /// <summary> Mirrors of the Seen Flags (inside the Large block) </summary>
        private int[] SeenFlagOffsets;

        protected override void SetDex(PKM pkm)
        {
            int species = pkm.Species;
            if (!CanSetDex(species))
                return;

            switch (species)
            {
                case (int)Species.Unown when !GetSeen(species): // Unown
                    DexPIDUnown = pkm.PID;
                    break;
                case (int)Species.Spinda when !GetSeen(species): // Spinda
                    DexPIDSpinda = pkm.PID;
                    break;
            }
            SetCaught(species, true);
            SetSeen(species, true);
        }

        private bool CanSetDex(int species)
        {
            if (species <= 0)
                return false;
            if (species > MaxSpeciesID)
                return false;
            if (Version == GameVersion.Invalid)
                return false;
            return true;
        }

        public uint DexPIDUnown { get => BitConverter.ToUInt32(Small, PokeDex + 0x4); set => BitConverter.GetBytes(value).CopyTo(Small, PokeDex + 0x4); }
        public uint DexPIDSpinda { get => BitConverter.ToUInt32(Small, PokeDex + 0x8); set => BitConverter.GetBytes(value).CopyTo(Small, PokeDex + 0x8); }
        public int DexUnownForm => PKX.GetUnownForm(DexPIDUnown);

        public override bool GetCaught(int species)
        {
            int bit = species - 1;
            int ofs = bit >> 3;
            int caughtOffset = PokeDex + 0x10;
            return FlagUtil.GetFlag(Small, caughtOffset + ofs, bit & 7);
        }

        public override void SetCaught(int species, bool caught)
        {
            int bit = species - 1;
            int ofs = bit >> 3;
            int caughtOffset = PokeDex + 0x10;
            FlagUtil.SetFlag(Small, caughtOffset + ofs, bit & 7, caught);
        }

        public override bool GetSeen(int species)
        {
            int bit = species - 1;
            int ofs = bit >> 3;
            int seenOffset = PokeDex + 0x44;
            return FlagUtil.GetFlag(Small, seenOffset + ofs, bit & 7);
        }

        public override void SetSeen(int species, bool seen)
        {
            int bit = species - 1;
            int ofs = bit >> 3;

            int seenOffset = PokeDex + 0x44;
            FlagUtil.SetFlag(Small, seenOffset + ofs, bit & 7, seen);
            foreach (int o in SeenFlagOffsets)
                FlagUtil.SetFlag(Large, o + ofs, bit & 7, seen);
        }

        public byte PokedexSort
        {
            get => Small[PokeDex + 0x01];
            set => Small[PokeDex + 0x01] = value;
        }

        public byte PokedexMode
        {
            get => Small[PokeDex + 0x01];
            set => Small[PokeDex + 0x01] = value;
        }

        public byte PokedexNationalMagicRSE
        {
            get => Small[PokeDex + 0x02];
            set => Small[PokeDex + 0x02] = value;
        }

        public byte PokedexNationalMagicFRLG
        {
            get => Small[PokeDex + 0x03];
            set => Small[PokeDex + 0x03] = value;
        }

        private const int PokedexNationalUnlockRSE = 0xDA;
        private const int PokedexNationalUnlockFRLG = 0xDA;
        private const ushort PokedexNationalUnlockWorkRSE = 0x0302;
        private const ushort PokedexNationalUnlockWorkFRLG = 0x6258;

        public bool NationalDex
        {
            get
            {
                return Version switch // only check natdex status in Block0
                {
                    // enable nat dex option magic value
                    GameVersion.RS or GameVersion.E => PokedexNationalMagicRSE == PokedexNationalUnlockRSE,
                    GameVersion.FRLG                => PokedexNationalMagicFRLG == PokedexNationalUnlockFRLG,
                    _ => false
                };
            }
            set
            {
                PokedexMode = value ? 1 : 0; // mode
                switch (Version)
                {
                    case GameVersion.RS:
                        PokedexNationalMagicRSE = value ? PokedexNationalUnlockRSE : 0; // magic
                        SetEventFlag(0x836, value);
                        SetEventConst(0x46, PokedexNationalUnlockWorkRSE);
                        break;
                    case GameVersion.E:
                        PokedexNationalMagicRSE = value ? PokedexNationalUnlockRSE : 0; // magic
                        SetEventFlag(0x896, value);
                        SetEventConst(0x46, PokedexNationalUnlockWorkRSE);
                        break;
                    case GameVersion.FRLG:
                        //PokedexNationalMagicRSE = value ? PokedexNationalUnlockRSE : 0; // magic
                        //SetEventFlag(0x838, value);
                        //SetEventConst(0x3C, PokedexNationalUnlockWorkRSE);
                        PokedexNationalMagicFRLG = value ? PokedexNationalUnlockFRLG : 0; // magic
                        SetEventFlag(0x840, value);
                        SetEventConst(0x4E, PokedexNationalUnlockWorkFRLG);
                        break;
                }
            }
        }

        public override string GetString(byte[] data, int offset, int length) => StringConverter3.GetString3(data, offset, length, Japanese);

        public override byte[] SetString(string value, int maxLength, int PadToSize = 0, ushort PadWith = 0)
        {
            if (PadToSize == 0)
                PadToSize = maxLength + 1;
            return StringConverter3.SetString3(value, maxLength, Japanese, PadToSize, PadWith);
        }

        #region eBerry
        // Offset and checksum code based from
        // https://github.com/suloku/wc-tool by Suloku
        private const int SIZE_EBERRY = 0x530;
        private const int OFFSET_EBERRY = 0x2E80 + 0x2E0;

        private uint EBerryChecksum => BitConverter.ToUInt32(Large, OFFSET_EBERRY + SIZE_EBERRY - 4);
        private bool IsEBerryChecksumValid { get; set; }

        public string EBerryName
        {
            get
            {
                if (!GameVersion.RS.Contains(Version) || !IsEBerryChecksumValid)
                    return string.Empty;
                return StringConverter3.GetString3(Large, OFFSET_EBERRY, 7, Japanese).Trim();
            }
        }

        public bool IsEBerryIsEnigma => string.IsNullOrEmpty(EBerryName.Trim());

        private void LoadEReaderBerryData()
        {
            if (!GameVersion.RS.Contains(Version))
                return;

            byte[] data = GetData(Large, OFFSET_EBERRY, SIZE_EBERRY - 4);

            // 8 bytes are 0x00 for chk calculation
            for (int i = 0; i < 8; i++)
                data[0xC + i] = 0x00;
            uint chk = (uint)data.Sum(z => z);
            IsEBerryChecksumValid = EBerryChecksum == chk;
        }
        #endregion

        public RTC3 ClockInitial
        {
            get
            {
                if (FRLG)
                    throw new ArgumentException(nameof(ClockInitial));
                return new RTC3(GetData(Small, 0x98, RTC3.Size));
            }
            set
            {
                if (FRLG)
                    return;
                SetData(Small, value.Data, 0x98);
            }
        }

        public RTC3 ClockElapsed
        {
            get
            {
                if (FRLG)
                    throw new ArgumentException(nameof(ClockElapsed));
                return new RTC3(GetData(Small, 0xA0, RTC3.Size));
            }
            set
            {
                if (FRLG)
                    return;
                SetData(Small, value.Data, 0xA0);
            }
        }

        public PokeBlock3Case PokeBlocks
        {
            get
            {
                var ofs = PokeBlockOffset;
                if (ofs < 0)
                    throw new ArgumentException($"Game does not support {nameof(PokeBlocks)}.");
                return new PokeBlock3Case(Large, ofs);
            }
            set => SetData(Large, value.Write(), PokeBlockOffset);
        }

        private int PokeBlockOffset
        {
            get
            {
                if (Version == GameVersion.E)
                    return 0x848;
                if (Version == GameVersion.RS)
                    return 0x7F8;
                return -1;
            }
        }

        public int GetMailOffset(int index)
        {
            var offset = Version switch
            {
                GameVersion.RS => 0x2B4C,
                GameVersion.E => 0x2BE0,
                _ => 0x2CD0, // FRLG
            };
            return (index * Mail3.SIZE) + offset;
        }

        private int ExternalEventFlags => Version switch
        {
            GameVersion.RS => 0x312F,
            GameVersion.E => 0x31C7,
            _ => throw new IndexOutOfRangeException(),
        };

        public bool HasReceivedWishmkrJirachi
        {
            get => !GameVersion.FRLG.Contains(Version) && GetFlag(ExternalEventFlags + 2, 0);
            set
            {
                if (!GameVersion.FRLG.Contains(Version))
                    SetFlag(ExternalEventFlags + 2, 0, value);
            }
        }

        public bool ResetPersonal(GameVersion g)
        {
            if (g.GetGeneration() != 3)
                return false;
            _personal = SaveUtil.GetG3Personal(g);
            return true;
        }

        public DecorationInventory3 Decorations
        {
            get
            {
                if (Version == GameVersion.FRLG)
                    throw new Exception();
                return Large.Slice(OFS_Decorations, DecorationInventory3.SIZE).ToStructure<DecorationInventory3>();
            }
            set
            {
                if (Version == GameVersion.FRLG)
                    throw new Exception();
                SetData(Large, value.ToBytes(), OFS_Decorations);
            }
        }
    }
}
