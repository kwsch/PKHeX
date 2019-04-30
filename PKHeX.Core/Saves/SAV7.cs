using System;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace PKHeX.Core
{
    /// <summary>
    /// Generation 7 <see cref="SaveFile"/> object.
    /// </summary>
    public sealed class SAV7 : SaveFile, ITrainerStatRecord, ISecureValueStorage
    {
        // Save Data Attributes
        protected override string BAKText => $"{OT} ({Version}) - {LastSavedTime}";
        public override string Filter => "Main SAV|*.*";
        public override string Extension => string.Empty;

        public override string[] PKMExtensions => PKM.Extensions.Where(f =>
        {
            int gen = f.Last() - 0x30;
            return gen <= 7 && f[1] != 'b'; // ignore PB7
        }).ToArray();

        public SAV7(byte[] data = null)
        {
            Data = data ?? new byte[SaveUtil.SIZE_G7SM];
            BAK = (byte[])Data.Clone();
            Exportable = !IsRangeEmpty(0, Data.Length);

            // Load Info
            Blocks = BlockInfo3DS.GetBlockInfoData(Data, out BlockInfoOffset, Checksums.CRC16);
            if (Exportable)
                CanReadChecksums();
            GetSAVOffsets();

            HeldItems = USUM ? Legal.HeldItems_USUM : Legal.HeldItems_SM;
            Personal = USUM ? PersonalTable.USUM : PersonalTable.SM;
            if (!Exportable)
                ClearBoxes();

            var demo = !USUM && Data.Skip(PCLayout).Take(0x4C4).All(z => z == 0); // up to Battle Box values
            if (demo || !Exportable)
            {
                PokeDex = -1; // Disabled
                TeamSlots = Array.Empty<int>();
            }
            else // Valid slot locking info present
            {
                LoadBattleTeams();
            }
        }

        // Configuration
        public override SaveFile Clone() => new SAV7((byte[])Data.Clone());

        public override int SIZE_STORED => PKX.SIZE_6STORED;
        protected override int SIZE_PARTY => PKX.SIZE_6PARTY;
        public override PKM BlankPKM => new PK7();
        public override Type PKMType => typeof(PK7);

        public override int BoxCount => 32;
        public override int MaxEV => 252;
        public override int Generation => 7;
        protected override int GiftCountMax => 48;
        protected override int GiftFlagMax => 0x100 * 8;
        protected override int EventFlagMax => USUM ? 4928 : 3968;
        protected override int EventConstMax => 1000;
        public override int OTLength => 12;
        public override int NickLength => 12;

        public override int MaxMoveID => SM ? Legal.MaxMoveID_7 : Legal.MaxMoveID_7_USUM;
        public override int MaxSpeciesID => SM ? Legal.MaxSpeciesID_7 : Legal.MaxSpeciesID_7_USUM;
        public override int MaxItemID => SM ? Legal.MaxItemID_7 : Legal.MaxItemID_7_USUM;
        public override int MaxAbilityID => SM ? Legal.MaxAbilityID_7 : Legal.MaxAbilityID_7_USUM;
        public override int MaxBallID => Legal.MaxBallID_7; // 26
        public override int MaxGameID => Legal.MaxGameID_7;

        public int QRSaveData;

        // Feature Overrides

        // Blocks & Offsets
        private readonly int BlockInfoOffset;
        private readonly BlockInfo[] Blocks;
        private bool IsMemeCryptoApplied = true;
        private const int MemeCryptoBlock = 36;
        public override bool ChecksumsValid => CanReadChecksums() && Blocks.GetChecksumsValid(Data);
        public override string ChecksumInfo => CanReadChecksums() ? Blocks.GetChecksumInfo(Data) : string.Empty;

        private bool CanReadChecksums()
        {
            if (Blocks.Length <= MemeCryptoBlock)
            { Debug.WriteLine($"Not enough blocks ({Blocks.Length}), aborting {nameof(CanReadChecksums)}"); return false; }
            if (!IsMemeCryptoApplied)
                return true;
            // clear memecrypto sig
            new byte[0x80].CopyTo(Data, Blocks[MemeCryptoBlock].Offset + 0x100);
            IsMemeCryptoApplied = false;
            return true;
        }

        protected override void SetChecksums()
        {
            if (!CanReadChecksums())
                return;
            Blocks.SetChecksums(Data);
            SaveBattleTeams();
            Data = MemeCrypto.Resign7(Data);
            IsMemeCryptoApplied = true;
        }

        public ulong TimeStampCurrent
        {
            get => BitConverter.ToUInt64(Data, BlockInfoOffset - 0x14);
            set => BitConverter.GetBytes(value).CopyTo(Data, BlockInfoOffset - 0x14);
        }

        public ulong TimeStampPrevious
        {
            get => BitConverter.ToUInt64(Data, BlockInfoOffset - 0xC);
            set => BitConverter.GetBytes(value).CopyTo(Data, BlockInfoOffset - 0xC);
        }

        private void GetSAVOffsets()
        {
            if (!Exportable) // Empty input
            {
                // Set bare minimum values for empty sav compatibility (based on S/M offsets)
                Trainer1 = 0x00E00;
                TrainerCard = 0x01200;
                Party = 0x01400;
                Box = 0x04E00;
                PCLayout = 0x04800;
                BattleBoxFlags = PCLayout + 0x4C4;
                PCBackgrounds = PCLayout + 0x5C0;
                LastViewedBox = PCLayout + 0x5E3;
                PCFlags = PCLayout + 0x5E0;
                return;
            }

            /* 00 */ Bag            = Blocks[00].Offset; // 0x00000  // [DE0]    MyItem
            /* 01 */ Trainer1       = Blocks[01].Offset; // 0x00E00  // [07C]    Situation
            /* 02 */            //  = Blocks[02].Offset; // 0x01000  // [014]    RandomGroup
            /* 03 */ TrainerCard    = Blocks[03].Offset; // 0x01200  // [0C0]    MyStatus
            /* 04 */ Party          = Blocks[04].Offset; // 0x01400  // [61C]    PokePartySave
            /* 05 */ EventConst     = Blocks[05].Offset; // 0x01C00  // [E00]    EventWork
            /* 06 */ PokeDex        = Blocks[06].Offset; // 0x02A00  // [F78]    ZukanData
            /* 07 */ GTS            = Blocks[07].Offset; // 0x03A00  // [228]    GtsData
            /* 08 */ Fused          = Blocks[08].Offset; // 0x03E00  // [104]    UnionPokemon
            /* 09 */ Misc           = Blocks[09].Offset; // 0x04000  // [200]    Misc
            /* 10 */ Trainer2       = Blocks[10].Offset; // 0x04200  // [020]    FieldMenu
            /* 11 */ ConfigSave     = Blocks[11].Offset; // 0x04400  // [004]    ConfigSave
            /* 12 */ AdventureInfo  = Blocks[12].Offset; // 0x04600  // [058]    GameTime
            /* 13 */ PCLayout       = Blocks[13].Offset; // 0x04800  // [5E6]    BOX
            /* 14 */ Box            = Blocks[14].Offset; // 0x04E00  // [36600]  BoxPokemon
            /* 15 */ Resort         = Blocks[15].Offset; // 0x3B400  // [572C]   ResortSave
            /* 16 */ PlayTime       = Blocks[16].Offset; // 0x40C00  // [008]    PlayTime
            /* 17 */ Overworld      = Blocks[17].Offset; // 0x40E00  // [1080]   FieldMoveModelSave
            /* 18 */ Fashion        = Blocks[18].Offset; // 0x42000  // [1A08]   Fashion
            /* 19 */            //  = Blocks[19].Offset; // 0x43C00  // [6408]   JoinFestaPersonalSave
            /* 20 */            //  = Blocks[20].Offset; // 0x4A200  // [6408]   JoinFestaPersonalSave
            /* 21 */ JoinFestaData  = Blocks[21].Offset; // 0x50800  // [3998]   JoinFestaDataSave
            /* 22 */            //  = Blocks[22].Offset; // 0x54200  // [100]    BerrySpot
            /* 23 */            //  = Blocks[23].Offset; // 0x54400  // [100]    FishingSpot
            /* 24 */            //  = Blocks[24].Offset; // 0x54600  // [10528]  LiveMatchData
            /* 25 */            //  = Blocks[25].Offset; // 0x64C00  // [204]    BattleSpotData
            /* 26 */ PokeFinderSave = Blocks[26].Offset; // 0x65000  // [B60]    PokeFinderSave
            /* 27 */ WondercardFlags= Blocks[27].Offset; // 0x65C00  // [3F50]   MysteryGiftSave
            /* 28 */ Record         = Blocks[28].Offset; // 0x69C00  // [358]    Record
            /* 29 */            //  = Blocks[29].Offset; // 0x6A000  // [728]    ValidationSave
            /* 30 */            //  = Blocks[30].Offset; // 0x6A800  // [200]    GameSyncSave
            /* 31 */            //  = Blocks[31].Offset; // 0x6AA00  // [718]    PokeDiarySave
            /* 32 */ BattleTree     = Blocks[32].Offset; // 0x6B200  // [1FC]    BattleInstSave
            /* 33 */ Daycare        = Blocks[33].Offset; // 0x6B400  // [200]    Sodateya
            /* 34 */            //  = Blocks[34].Offset; // 0x6B600  // [120]    WeatherSave
            /* 35 */ QRSaveData     = Blocks[35].Offset; // 0x6B800  // [1C8]    QRReaderSaveData
            /* 36 */            //  = Blocks[36].Offset; // 0x6BA00  // [200]    TurtleSalmonSave

            EventFlag = EventConst + (EventConstMax * 2); // After Event Const (u16)*n
            HoF = EventFlag + (EventFlagMax / 8); // After Event Flags (1b)*(1u8/8b)*n

            OFS_PouchHeldItem =     Bag + 0; // 430 (Case 0)
            OFS_PouchKeyItem =      Bag + 0x6B8; // 184 (Case 4)
            OFS_PouchTMHM =         Bag + 0x998; // 108 (Case 2)
            OFS_PouchMedicine =     Bag + 0xB48; // 64 (Case 1)
            OFS_PouchBerry =        Bag + 0xC48; // 72 (Case 3)
            OFS_PouchZCrystals =    Bag + 0xD68; // 30 (Case 5)

            PokeDexLanguageFlags =  PokeDex + 0x550;
            WondercardData = WondercardFlags + 0x100;

            BattleBoxFlags =        PCLayout + 0x4C4;
            PCBackgrounds =         PCLayout + 0x5C0;
            LastViewedBox =         PCLayout + 0x5E3;
            PCFlags =               PCLayout + 0x5E0;

            FashionLength = 0x1A08;

            TeamCount = 6;
            TeamSlots = new int[6*TeamCount];
            if (USUM)
            {
                // slight differences
                /* 37 */            //  = Blocks[37].Offset;   BattleFesSave
                /* 38 */            //  = Blocks[38].Offset;   FinderStudioSave

                OFS_PouchHeldItem =     Bag + 0; // 427 (Case 0)
                OFS_PouchKeyItem = OFS_PouchHeldItem + (4 * 427); // 198 (Case 4)
                OFS_PouchTMHM = OFS_PouchKeyItem + (4 * 198); // 108 (Case 2)
                OFS_PouchMedicine = OFS_PouchTMHM + (4 * 108); // 60 (Case 1)
                OFS_PouchBerry = OFS_PouchMedicine + (4 * 60); // 67 (Case 3)
                OFS_PouchZCrystals = OFS_PouchBerry + (4 * 67); // 35 (Case 5)
                OFS_BattleItems = OFS_PouchZCrystals + (4 * 35); // 11 (Case 6)
            }
        }

        // Private Only
        private int Bag { get; set; } = int.MinValue;
        private int AdventureInfo { get; set; } = int.MinValue;
        private int Trainer2 { get; set; } = int.MinValue;
        public int Misc { get; private set; } = int.MinValue;
        private int LastViewedBox { get; set; } = int.MinValue;
        private int WondercardFlags { get; set; } = int.MinValue;
        private int PlayTime { get; set; } = int.MinValue;
        private int Overworld { get; set; } = int.MinValue;
        public int JoinFestaData { get; private set; } = int.MinValue;
        private int PokeFinderSave { get; set; } = int.MinValue;
        private int BattleTree { get; set; } = int.MinValue;
        private int BattleBoxFlags { get; set; } = int.MinValue;
        private int TeamCount { get; set; } = int.MinValue;
        private int ConfigSave { get; set; } = int.MinValue;

        // Accessible as SAV7
        private int TrainerCard { get; set; } = 0x14000;
        private int Resort { get; set; }
        private int PCFlags { get; set; } = int.MinValue;
        private int PCBackgrounds { get; set; } = int.MinValue;
        public int PokeDexLanguageFlags { get; private set; } = int.MinValue;
        public int Fashion { get; set; } = int.MinValue;
        public int FashionLength { get; set; } = int.MinValue;
        private int Record { get; set; } = int.MinValue;

        public const int ResortCount = 93;
        public int GetResortSlotOffset(int slot) => Resort + 0x16 + (slot * SIZE_STORED);

        public PKM[] ResortPKM
        {
            get
            {
                PKM[] data = new PKM[ResortCount];
                for (int i = 0; i < data.Length; i++)
                {
                    data[i] = GetPKM(GetData(GetResortSlotOffset(i), SIZE_STORED));
                    data[i].Identifier = $"Resort Slot {i}";
                }
                return data;
            }
            set
            {
                if (value?.Length != ResortCount)
                    throw new ArgumentException(nameof(ResortCount));

                for (int i = 0; i < value.Length; i++)
                    SetStoredSlot(value[i], GetResortSlotOffset(i));
            }
        }

        public override GameVersion Version
        {
            get
            {
                switch (Game)
                {
                    case 30: return GameVersion.SN;
                    case 31: return GameVersion.MN;
                    case 32: return GameVersion.US;
                    case 33: return GameVersion.UM;
                }
                return GameVersion.Invalid;
            }
        }

        // Player Information
        public override int TID
        {
            get => BitConverter.ToUInt16(Data, TrainerCard + 0);
            set => BitConverter.GetBytes((ushort)value).CopyTo(Data, TrainerCard + 0);
        }

        public override int SID
        {
            get => BitConverter.ToUInt16(Data, TrainerCard + 2);
            set => BitConverter.GetBytes((ushort)value).CopyTo(Data, TrainerCard + 2);
        }

        public override int Game
        {
            get => Data[TrainerCard + 4];
            set => Data[TrainerCard + 4] = (byte)value;
        }

        public override int Gender
        {
            get => Data[TrainerCard + 5];
            set => Data[TrainerCard + 5] = (byte)value;
        }

        public override int GameSyncIDSize => 16; // 64 bits

        public override string GameSyncID
        {
            get
            {
                var data = Data.Skip(TrainerCard + 0x10).Take(GameSyncIDSize/2).Reverse().ToArray();
                return BitConverter.ToString(data).Replace("-", string.Empty);
            }
            set
            {
                if (value == null)
                    return;
                if (value.Length > GameSyncIDSize)
                    return;

                Enumerable.Range(0, value.Length)
                     .Where(x => x % 2 == 0)
                     .Reverse()
                     .Select(x => Convert.ToByte(value.Substring(x, 2), 16))
                     .ToArray().CopyTo(Data, TrainerCard + 0x10);
            }
        }

        private const int NexUniqueIDSize = 32; // 128 bits

        public string NexUniqueID
        {
            get
            {
                var data = Data.Skip(TrainerCard + 0x18).Take(NexUniqueIDSize/2).Reverse().ToArray();
                return BitConverter.ToString(data).Replace("-", string.Empty);
            }
            set
            {
                if (value == null)
                    return;
                if (value.Length > NexUniqueIDSize)
                    return;

                Enumerable.Range(0, value.Length)
                     .Where(x => x % 2 == 0)
                     .Reverse()
                     .Select(x => Convert.ToByte(value.Substring(x, 2), 16))
                     .ToArray().CopyTo(Data, TrainerCard + 0x18);
            }
        }

        public byte[] FestaID // 12byte
        {
            get => Data.Skip(TrainerCard + 0x28).Take(4).Concat(Data.Skip(TrainerCard + 0x18).Take(8)).ToArray();
            set
            {
                if (value?.Length != 12)
                    return;
                Array.Copy(value, 0, Data, TrainerCard + 0x28, 4);
                Array.Copy(value, 4, Data, TrainerCard + 0x18, 8);
            }
        }

        public override int SubRegion
        {
            get => Data[TrainerCard + 0x2E];
            set => Data[TrainerCard + 0x2E] = (byte)value;
        }

        public override int Country
        {
            get => Data[TrainerCard + 0x2F];
            set => Data[TrainerCard + 0x2F] = (byte)value;
        }

        public override int ConsoleRegion
        {
            get => Data[TrainerCard + 0x34];
            set => Data[TrainerCard + 0x34] = (byte)value;
        }

        public override int Language
        {
            get => Data[TrainerCard + 0x35];
            set => Data[TrainerCard + 0x35] = (byte)value;
        }

        public override string OT
        {
            get => GetString(TrainerCard + 0x38, 0x1A);
            set => SetString(value, OTLength).CopyTo(Data, TrainerCard + 0x38);
        }

        public int DressUpSkinColor
        {
            get => (Data[TrainerCard + 0x54] >> 2) & 7;
            set => Data[TrainerCard + 0x54] = (byte)((Data[TrainerCard + 0x54] & ~(7 << 2)) | (value << 2));
        }

        public override int MultiplayerSpriteID
        {
            get => Data[TrainerCard + 0x58];
            set => Data[TrainerCard + 0x58] = (byte)value;
        }

        public int BallThrowType
        {
            get => Data[TrainerCard + 0x7A];
            set => Data[TrainerCard + 0x7A] = (byte)(value > 8 ? 0 : value);
        }

        public int M // "StartLocation"
        {
            get => BitConverter.ToUInt16(Data, Trainer1 + 0x00);
            set => BitConverter.GetBytes((ushort)value).CopyTo(Data, Trainer1 + 0x00);
        }

        public float X
        {
            get => BitConverter.ToSingle(Data, Trainer1 + 0x08);
            set
            {
                BitConverter.GetBytes(value).CopyTo(Data, Trainer1 + 0x08);
                BitConverter.GetBytes(value).CopyTo(Data, Overworld + 0x08);
            }
        }

        public float Z
        {
            get => BitConverter.ToSingle(Data, Trainer1 + 0x10);
            set
            {
                BitConverter.GetBytes(value).CopyTo(Data, Trainer1 + 0x10);
                BitConverter.GetBytes(value).CopyTo(Data, Overworld + 0x10);
            }
        }

        public float Y
        {
            get => (int)BitConverter.ToSingle(Data, Trainer1 + 0x18);
            set
            {
                BitConverter.GetBytes(value).CopyTo(Data, Trainer1 + 0x18);
                BitConverter.GetBytes(value).CopyTo(Data, Overworld + 0x18);
            }
        }

        public float R
        {
            get => (int)BitConverter.ToSingle(Data, Trainer1 + 0x20);
            set
            {
                BitConverter.GetBytes(value).CopyTo(Data, Trainer1 + 0x20);
                BitConverter.GetBytes(value).CopyTo(Data, Overworld + 0x20);
            }
        }

        public int SpecialLocation
        {
            get => Data[Trainer1 + 0x24];
            set => Data[Trainer1 + 0x24] = (byte)value;
        }

        public int WarpContinueRequest
        {
            get => Data[Trainer1 + 0x6E];
            set => Data[Trainer1 + 0x6E] = (byte)value;
        }

        public int StepCountEgg
        {
            get => BitConverter.ToInt32(Data, Trainer1 + 0x70);
            set => BitConverter.GetBytes(value).CopyTo(Data, Trainer1 + 0x70);
        }

        public int LastZoneID
        {
            get => BitConverter.ToUInt16(Data, Trainer1 + 0x74);
            set => BitConverter.GetBytes((ushort)value).CopyTo(Data, Trainer1 + 0x74);
        }

        public int StepCountFriendship
        {
            get => BitConverter.ToUInt16(Data, Trainer1 + 0x76);
            set => BitConverter.GetBytes((ushort)value).CopyTo(Data, Trainer1 + 0x76);
        }

        public int StepCountAffection // Kawaigari
        {
            get => BitConverter.ToUInt16(Data, Trainer1 + 0x78);
            set => BitConverter.GetBytes((ushort)value).CopyTo(Data, Trainer1 + 0x78);
        }

        public int ConfigValue
        {
            get => BitConverter.ToInt32(Data, ConfigSave);
            set => BitConverter.GetBytes(value).CopyTo(Data, ConfigSave);
        }

        public int TalkingSpeed
        {
            get => ConfigValue & 3;
            set => ConfigValue = (ConfigValue & ~3) | (value & 3);
        }

        public int BattleAnimation
        {
            // Effects OFF = 1, Effects ON = 0
            get => (ConfigValue >> 2) & 1;
            set => ConfigValue = (ConfigValue & ~(1<<2)) | (value << 2);
        }

        public int BattleStyle
        {
            // SET = 1, SWITCH = 0
            get => (ConfigValue >> 3) & 1;
            set => ConfigValue = (ConfigValue & ~(1 << 3)) | (value << 3);
        }

        public int ButtonMode
        {
            get => (ConfigValue >> 13) & 3;
            set => ConfigValue = (ConfigValue & ~(1 << 13)) | (value << 13);
        }

        public int BoxStatus
        {
            // MANUAL = 1, AUTOMATIC = 0
            get => (ConfigValue >> 15) & 1;
            set => ConfigValue = (ConfigValue & ~(1 << 15)) | (value << 15);
        }

        public override uint Money
        {
            get => BitConverter.ToUInt32(Data, Misc + 0x4);
            set
            {
                if (value > 9999999) value = 9999999;
                BitConverter.GetBytes(value).CopyTo(Data, Misc + 0x4);
            }
        }

        public uint Stamps
        {
            get => (BitConverter.ToUInt32(Data, Misc + 0x08) << 13) >> 17;  // 15 stamps; discard top13, lowest4
            set
            {
                uint flags = BitConverter.ToUInt32(Data, Misc + 0x08) & 0xFFF8000F;
                flags |= (value & 0x7FFF) << 4;
                BitConverter.GetBytes(flags).CopyTo(Data, Misc + 0x08);
            }
        }

        public uint BP
        {
            get => BitConverter.ToUInt32(Data, Misc + 0x11C);
            set
            {
                if (value > 9999) value = 9999;
                BitConverter.GetBytes(value).CopyTo(Data, Misc + 0x11C);
            }
        }

        public int Vivillon
        {
            get => Data[Misc + 0x130] & 0x1F;
            set => Data[Misc + 0x130] = (byte)((Data[Misc + 0x130] & ~0x1F) | (value & 0x1F));
        }

        public uint StarterEncryptionConstant
        {
            get => BitConverter.ToUInt32(Data, Misc + 0x148);
            set => SetData(BitConverter.GetBytes(value), Misc + 0x148);
        }

        public int DaysFromRefreshed
        {
            get => Data[Misc + 0x123];
            set => Data[Misc + 0x123] = (byte)value;
        }

        public uint UsedFestaCoins
        {
            get => BitConverter.ToUInt32(Data, Record + (38 << 2)); //Record[038]
            set
            {
                if (value > 9999999) value = 9999999;
                BitConverter.GetBytes(value).CopyTo(Data, Record + (38 << 2));
            }
        }

        public uint FestaCoins
        {
            get => BitConverter.ToUInt32(Data, JoinFestaData + 0x508);
            set
            {
                if (value > 9999999) value = 9999999;
                BitConverter.GetBytes(value).CopyTo(Data, JoinFestaData + 0x508);

                TotalFestaCoins = UsedFestaCoins + value;
            }
        }

        private uint TotalFestaCoins
        {
            get => BitConverter.ToUInt32(Data, JoinFestaData + 0x50C);
            set
            {
                if (value > 9999999) value = 9999999;
                BitConverter.GetBytes(value).CopyTo(Data, JoinFestaData + 0x50C);
            }
        }

        public string FestivalPlazaName
        {
            get => Util.TrimFromZero(Encoding.Unicode.GetString(Data, JoinFestaData + 0x510, 0x2A));
            set
            {
                const int max = 20;
                if (value.Length > max) value = value.Substring(0, max);
                Encoding.Unicode.GetBytes(value.PadRight(value.Length + 1, '\0')).CopyTo(Data, JoinFestaData + 0x510);
            }
        }

        public ushort FestaRank { get => BitConverter.ToUInt16(Data, JoinFestaData + 0x53A); set => BitConverter.GetBytes(value).CopyTo(Data, JoinFestaData + 0x53A); }
        public ushort GetFestaMessage(int index) => BitConverter.ToUInt16(Data, JoinFestaData + (index * 2));
        public void SetFestaMessage(int index, ushort value) => BitConverter.GetBytes(value).CopyTo(Data, JoinFestaData + (index * 2));
        public bool GetFestaPhraseUnlocked(int index) => Data[JoinFestaData + 0x2A50 + index] != 0; //index: 0 to 105:commonPhrases, 106:Lv100!

        public void SetFestaPhraseUnlocked(int index, bool value)
        {
            if (GetFestaPhraseUnlocked(index) != value) Data[JoinFestaData + 0x2A50 + index] = (byte)(value ? 1 : 0);
        }

        public byte GetFestPrizeReceived(int index) => Data[JoinFestaData + 0x53C + index];
        public void SetFestaPrizeReceived(int index, byte value) => Data[JoinFestaData + 0x53C + index] = value;
        private int FestaYear { get => BitConverter.ToInt32(Data, JoinFestaData + 0x2F0); set => BitConverter.GetBytes(value).CopyTo(Data, JoinFestaData + 0x2F0); }
        private int FestaMonth { get => BitConverter.ToInt32(Data, JoinFestaData + 0x2F4); set => BitConverter.GetBytes(value).CopyTo(Data, JoinFestaData + 0x2F4); }
        private int FestaDay { get => BitConverter.ToInt32(Data, JoinFestaData + 0x2F8); set => BitConverter.GetBytes(value).CopyTo(Data, JoinFestaData + 0x2F8); }
        private int FestaHour { get => BitConverter.ToInt32(Data, JoinFestaData + 0x300); set => BitConverter.GetBytes(value).CopyTo(Data, JoinFestaData + 0x300); }
        private int FestaMinute { get => BitConverter.ToInt32(Data, JoinFestaData + 0x304); set => BitConverter.GetBytes(value).CopyTo(Data, JoinFestaData + 0x304); }
        private int FestaSecond { get => BitConverter.ToInt32(Data, JoinFestaData + 0x308); set => BitConverter.GetBytes(value).CopyTo(Data, JoinFestaData + 0x308); }

        public DateTime? FestaDate
        {
            get => FestaYear >= 0 && FestaMonth > 0 && FestaDay > 0 && FestaHour >= 0 && FestaMinute >= 0 && FestaSecond >= 0 && Util.IsDateValid(FestaYear, FestaMonth, FestaDay)
                ? new DateTime(FestaYear, FestaMonth, FestaDay, FestaHour, FestaMinute, FestaSecond)
                : (DateTime?)null;
            set
            {
                if (value.HasValue)
                {
                    DateTime dt = value.Value;
                    FestaYear = dt.Year;
                    FestaMonth = dt.Month;
                    FestaDay = dt.Day;
                    FestaHour = dt.Hour;
                    FestaMinute = dt.Minute;
                    FestaSecond = dt.Second;
                }
            }
        }

        public sealed class FashionItem
        {
            public bool IsOwned { get; set; }
            public bool IsNew { get; set; }
        }

        public FashionItem[] Wardrobe
        {
            get
            {
                var data = GetData(Fashion, 0x5A8);
                return data.Select(b => new FashionItem {IsOwned = (b & 1) != 0, IsNew = (b & 2) != 0}).ToArray();
            }
            set
            {
                if (value.Length != 0x5A8)
                    throw new ArgumentOutOfRangeException($"Unexpected size: 0x{value.Length:X}");
                SetData(value.Select(t => (byte) ((t.IsOwned ? 1 : 0) | (t.IsNew ? 2 : 0))).ToArray(), Fashion);
            }
        }

        public override int PlayedHours
        {
            get => BitConverter.ToUInt16(Data, PlayTime);
            set => BitConverter.GetBytes((ushort)value).CopyTo(Data, PlayTime);
        }

        public override int PlayedMinutes
        {
            get => Data[PlayTime + 2];
            set => Data[PlayTime + 2] = (byte)value;
        }

        public override int PlayedSeconds
        {
            get => Data[PlayTime + 3];
            set => Data[PlayTime + 3] = (byte)value;
        }

        private uint LastSaved { get => BitConverter.ToUInt32(Data, PlayTime + 0x4); set => BitConverter.GetBytes(value).CopyTo(Data, PlayTime + 0x4); }
        private int LastSavedYear { get => (int)(LastSaved & 0xFFF); set => LastSaved = (LastSaved & 0xFFFFF000) | (uint)value; }
        private int LastSavedMonth { get => (int)(LastSaved >> 12 & 0xF); set => LastSaved = (LastSaved & 0xFFFF0FFF) | ((uint)value & 0xF) << 12; }
        private int LastSavedDay { get => (int)(LastSaved >> 16 & 0x1F); set => LastSaved = (LastSaved & 0xFFE0FFFF) | ((uint)value & 0x1F) << 16; }
        private int LastSavedHour { get => (int)(LastSaved >> 21 & 0x1F); set => LastSaved = (LastSaved & 0xFC1FFFFF) | ((uint)value & 0x1F) << 21; }
        private int LastSavedMinute { get => (int)(LastSaved >> 26 & 0x3F); set => LastSaved = (LastSaved & 0x03FFFFFF) | ((uint)value & 0x3F) << 26; }
        private string LastSavedTime => $"{LastSavedYear:0000}{LastSavedMonth:00}{LastSavedDay:00}{LastSavedHour:00}{LastSavedMinute:00}";

        public DateTime? LastSavedDate
        {
            get => !Util.IsDateValid(LastSavedYear, LastSavedMonth, LastSavedDay)
                    ? (DateTime?)null
                    : new DateTime(LastSavedYear, LastSavedMonth, LastSavedDay, LastSavedHour, LastSavedMinute, 0);
            set
            {
                // Only update the properties if a value is provided.
                if (value.HasValue)
                {
                    var dt = value.Value;
                    LastSavedYear = dt.Year;
                    LastSavedMonth = dt.Month;
                    LastSavedDay = dt.Day;
                    LastSavedHour = dt.Hour;
                    LastSavedMinute = dt.Minute;
                }
                else // Clear the date.
                {
                    // If code tries to access MetDate again, null will be returned.
                    LastSavedYear = 0;
                    LastSavedMonth = 0;
                    LastSavedDay = 0;
                    LastSavedHour = 0;
                    LastSavedMinute = 0;
                }
            }
        }

        public int ResumeYear { get => BitConverter.ToInt32(Data, AdventureInfo + 0x4); set => BitConverter.GetBytes(value).CopyTo(Data, AdventureInfo + 0x4); }
        public int ResumeMonth { get => Data[AdventureInfo + 0x8]; set => Data[AdventureInfo + 0x8] = (byte)value; }
        public int ResumeDay { get => Data[AdventureInfo + 0x9]; set => Data[AdventureInfo + 0x9] = (byte)value; }
        public int ResumeHour { get => Data[AdventureInfo + 0xB]; set => Data[AdventureInfo + 0xB] = (byte)value; }
        public int ResumeMinute { get => Data[AdventureInfo + 0xC]; set => Data[AdventureInfo + 0xC] = (byte)value; }
        public int ResumeSeconds { get => Data[AdventureInfo + 0xD]; set => Data[AdventureInfo + 0xD] = (byte)value; }
        public override uint SecondsToStart { get => BitConverter.ToUInt32(Data, AdventureInfo + 0x28); set => BitConverter.GetBytes(value).CopyTo(Data, AdventureInfo + 0x28); }
        public override uint SecondsToFame { get => BitConverter.ToUInt32(Data, AdventureInfo + 0x30); set => BitConverter.GetBytes(value).CopyTo(Data, AdventureInfo + 0x30); }

        public ulong AlolaTime { get => BitConverter.ToUInt64(Data, AdventureInfo + 0x48); set => BitConverter.GetBytes(value).CopyTo(Data, AdventureInfo + 0x48); }

        // Stat Records
        public int RecordCount => 200;

        public int GetRecord(int recordID)
        {
            int ofs = Records.GetOffset(Record, recordID);
            if (recordID < 100)
                return BitConverter.ToInt32(Data, ofs);
            if (recordID < 200)
                return BitConverter.ToInt16(Data, ofs);
            return 0;
        }

        public void SetRecord(int recordID, int value)
        {
            int ofs = Records.GetOffset(Record, recordID);
            var maxes = USUM ? Records.MaxType_USUM: Records.MaxType_SM;
            int max = Records.GetMax(recordID, maxes);
            if (value > max)
                value = max;
            if (recordID < 100)
                BitConverter.GetBytes(value).CopyTo(Data, ofs);
            if (recordID < 200)
                BitConverter.GetBytes((ushort)value).CopyTo(Data, ofs);
        }

        public int GetRecordMax(int recordID) => Records.GetMax(recordID, USUM ? Records.MaxType_USUM : Records.MaxType_SM);
        public int GetRecordOffset(int recordID) => Records.GetOffset(Record, recordID);
        public void AddRecord(int recordID) => SetRecord(recordID, GetRecord(recordID) + 1);

        public ushort PokeFinderCameraVersion
        {
            get => BitConverter.ToUInt16(Data, PokeFinderSave + 0x00);
            set => BitConverter.GetBytes(value).CopyTo(Data, PokeFinderSave + 0x00);
        }

        public bool PokeFinderGyroFlag
        {
            get => BitConverter.ToUInt16(Data, PokeFinderSave + 0x02) == 1;
            set => BitConverter.GetBytes((ushort)(value ? 1 : 0)).CopyTo(Data, PokeFinderSave + 0x02);
        }

        public uint PokeFinderSnapCount
        {
            get => BitConverter.ToUInt32(Data, PokeFinderSave + 0x04);
            set
            {
                if (value > 9999999) // Top bound is unchecked, check anyway
                    value = 9999999;
                BitConverter.GetBytes(value).CopyTo(Data, PokeFinderSave + 0x04);
            }
        }

        public uint PokeFinderThumbsTotalValue
        {
            get => BitConverter.ToUInt32(Data, PokeFinderSave + 0x0C);
            set => BitConverter.GetBytes(value).CopyTo(Data, PokeFinderSave + 0x0C);
        }

        public uint PokeFinderThumbsHighValue
        {
            get => BitConverter.ToUInt32(Data, PokeFinderSave + 0x10);
            set
            {
                if (value > 9_999_999)
                    value = 9_999_999;
                BitConverter.GetBytes(value).CopyTo(Data, PokeFinderSave + 0x10);

                if (value > PokeFinderThumbsTotalValue)
                    PokeFinderThumbsTotalValue = value;
            }
        }

        public ushort PokeFinderTutorialFlags
        {
            get => BitConverter.ToUInt16(Data, PokeFinderSave + 0x14);
            set => BitConverter.GetBytes(value).CopyTo(Data, PokeFinderSave + 0x14);
        }

        // Inventory
        public override InventoryPouch[] Inventory
        {
            get
            {
                var bag = GetPouches();
                return bag.LoadAll(Data);
            }
            set => value.SaveAll(Data);
        }

        private InventoryPouch[] GetPouches()
        {
            if (SM)
            {
                return new InventoryPouch[]
                {
                    new InventoryPouch7(InventoryType.Medicine, Legal.Pouch_Medicine_SM, 999, OFS_PouchMedicine),
                    new InventoryPouch7(InventoryType.Items, Legal.Pouch_Items_SM, 999, OFS_PouchHeldItem),
                    new InventoryPouch7(InventoryType.TMHMs, Legal.Pouch_TMHM_SM, 1, OFS_PouchTMHM),
                    new InventoryPouch7(InventoryType.Berries, Legal.Pouch_Berries_SM, 999, OFS_PouchBerry),
                    new InventoryPouch7(InventoryType.KeyItems, Legal.Pouch_Key_SM, 1, OFS_PouchKeyItem),
                    new InventoryPouch7(InventoryType.ZCrystals, Legal.Pouch_ZCrystal_SM, 1, OFS_PouchZCrystals),
                };
            }
            return new InventoryPouch[] // USUM
            {
                new InventoryPouch7(InventoryType.Medicine, Legal.Pouch_Medicine_SM, 999, OFS_PouchMedicine),
                new InventoryPouch7(InventoryType.Items, Legal.Pouch_Items_SM, 999, OFS_PouchHeldItem),
                new InventoryPouch7(InventoryType.TMHMs, Legal.Pouch_TMHM_SM, 1, OFS_PouchTMHM),
                new InventoryPouch7(InventoryType.Berries, Legal.Pouch_Berries_SM, 999, OFS_PouchBerry),
                new InventoryPouch7(InventoryType.KeyItems, Legal.Pouch_Key_USUM, 1, OFS_PouchKeyItem),
                new InventoryPouch7(InventoryType.ZCrystals, Legal.Pouch_ZCrystal_USUM, 1, OFS_PouchZCrystals),
                new InventoryPouch7(InventoryType.BattleItems, Legal.Pouch_Roto_USUM, 999, OFS_BattleItems),
            };
        }

        // Battle Tree
        public int GetTreeStreak(int battletype, bool super, bool max)
        {
            if (battletype > 3)
                throw new ArgumentException(nameof(battletype));

            int offset = 8*battletype;
            if (super)
                offset += 2;
            if (max)
                offset += 4;

            return BitConverter.ToUInt16(Data, BattleTree + offset);
        }

        public void SetTreeStreak(int value, int battletype, bool super, bool max)
        {
            if (battletype > 3)
                throw new ArgumentException(nameof(battletype));

            if (value > ushort.MaxValue)
                value = ushort.MaxValue;

            int offset = 8 * battletype;
            if (super)
                offset += 2;
            if (max)
                offset += 4;

            BitConverter.GetBytes((ushort)value).CopyTo(Data, BattleTree + offset);
        }

        // Resort Save
        public int GetPokebeanCount(int bean_id)
        {
            if (bean_id < 0 || bean_id > 14)
                throw new ArgumentException("Invalid bean id!");
            return Data[Resort + 0x564C + bean_id];
        }

        public void SetPokebeanCount(int bean_id, int count)
        {
            if (bean_id < 0 || bean_id > 14)
                throw new ArgumentException("Invalid bean id!");
            if (count < 0)
                count = 0;
            if (count > 255)
                count = 255;
            Data[Resort + 0x564C + bean_id] = (byte) count;
        }

        // Storage
        public override int CurrentBox { get => Data[LastViewedBox]; set => Data[LastViewedBox] = (byte)value; }

        public override int GetPartyOffset(int slot)
        {
            return Party + (SIZE_PARTY * slot);
        }

        public override int GetBoxOffset(int box)
        {
            return Box + (SIZE_STORED *box*30);
        }

        protected override int GetBoxWallpaperOffset(int box)
        {
            int ofs = PCBackgrounds > 0 && PCBackgrounds < Data.Length ? PCBackgrounds : -1;
            if (ofs > -1)
                return ofs + box;
            return ofs;
        }

        public override void SetBoxWallpaper(int box, int value)
        {
            if (PCBackgrounds < 0)
                return;
            int ofs = PCBackgrounds > 0 && PCBackgrounds < Data.Length ? PCBackgrounds : 0;
            Data[ofs + box] = (byte)value;
        }

        public override string GetBoxName(int box)
        {
            if (PCLayout < 0)
                return $"B{box + 1}";
            return Util.TrimFromZero(Encoding.Unicode.GetString(Data, PCLayout + (0x22 * box), 0x22));
        }

        public override void SetBoxName(int box, string value)
        {
            var data = Encoding.Unicode.GetBytes(value.PadRight(0x11, '\0'));
            SetData(data, PCLayout + (0x22 * box));
        }

        protected override PKM GetPKM(byte[] data)
        {
            return new PK7(data);
        }

        protected override void SetPKM(PKM pkm)
        {
            PK7 pk7 = (PK7)pkm;
            // Apply to this Save File
            int CT = pk7.CurrentHandler;
            DateTime Date = DateTime.Now;
            pk7.Trade(this, Date.Day, Date.Month, Date.Year);
            if (CT != pk7.CurrentHandler) // Logic updated Friendship
            {
                // Copy over the Friendship Value only under certain circumstances
                if (pk7.Moves.Contains(216)) // Return
                    pk7.CurrentFriendship = pk7.OppositeFriendship;
                else if (pk7.Moves.Contains(218)) // Frustration
                    pkm.CurrentFriendship = pk7.OppositeFriendship;
            }
            pkm.RefreshChecksum();
            if (Record > 0)
                AddCountAcquired(pkm);
        }

        private void AddCountAcquired(PKM pkm)
        {
            AddRecord(pkm.WasEgg ? 008 : 006); // egg, capture
            if (pkm.CurrentHandler == 1)
                AddRecord(011); // trade
            if (!pkm.WasEgg)
                AddRecord(004); // wild encounters
        }

        protected override void SetDex(PKM pkm)
        {
            if (PokeDex < 0 || Version == GameVersion.Invalid) // sanity
                return;
            if (pkm.Species == 0 || pkm.Species > MaxSpeciesID) // out of range
                return;
            if (pkm.IsEgg) // do not add
                return;

            int bit = pkm.Species - 1;
            int bd = bit >> 3; // div8
            int bm = bit & 7; // mod8
            int gender = pkm.Gender % 2; // genderless -> male
            int shiny = pkm.IsShiny ? 1 : 0;
            if (pkm.Species == 351) // castform
                shiny = 0;
            int shift = gender | (shiny << 1);
            if (pkm.Species == 327) // Spinda
            {
                if ((Data[PokeDex + 0x84] & (1 << (shift + 4))) != 0) // Already 2
                {
                    BitConverter.GetBytes(pkm.EncryptionConstant).CopyTo(Data, PokeDex + 0x8E8 + (shift * 4));
                    // Data[PokeDex + 0x84] |= (byte)(1 << (shift + 4)); // 2 -- pointless
                    Data[PokeDex + 0x84] |= (byte)(1 << shift); // 1
                }
                else if ((Data[PokeDex + 0x84] & (1 << shift)) == 0) // Not yet 1
                {
                    Data[PokeDex + 0x84] |= (byte)(1 << shift); // 1
                }
            }
            int ofs = PokeDex // Raw Offset
                      + 0x08 // Magic + Flags
                      + 0x80; // Misc Data (1024 bits)
            // Set the Owned Flag
            Data[ofs + bd] |= (byte)(1 << bm);

            // Starting with Gen7, form bits are stored in the same region as the species flags.

            int formstart = pkm.AltForm;
            int formend = pkm.AltForm;
            bool reset = SanitizeFormsToIterate(pkm.Species, out int fs, out int fe, formstart, USUM);
            if (reset)
            {
                formstart = fs;
                formend = fe;
            }

            for (int form = formstart; form <= formend; form++)
            {
                int bitIndex = bit;
                if (form > 0) // Override the bit to overwrite
                {
                    int fc = Personal[pkm.Species].FormeCount;
                    if (fc > 1) // actually has forms
                    {
                        int f = USUM
                            ? DexFormUtil.GetDexFormIndexUSUM(pkm.Species, fc, MaxSpeciesID - 1)
                            : DexFormUtil.GetDexFormIndexSM(pkm.Species, fc, MaxSpeciesID - 1);
                        if (f >= 0) // bit index valid
                            bitIndex = f + form;
                    }
                }
                SetDexFlags(bitIndex, gender, shiny, pkm.Species - 1);
            }

            // Set the Language
            int lang = pkm.Language;
            const int langCount = 9;
            if (lang <= 10 && lang != 6 && lang != 0) // valid language
            {
                if (lang >= 7)
                    lang--;
                lang--; // 0-8 languages
                if (lang < 0) lang = 1;
                int lbit = (bit * langCount) + lang;
                if (lbit >> 3 < 920) // Sanity check for max length of region
                    Data[PokeDexLanguageFlags + (lbit >> 3)] |= (byte)(1 << (lbit & 7));
            }
        }

        protected override void SetPartyValues(PKM pkm, bool isParty)
        {
            base.SetPartyValues(pkm, isParty);
            ((PK7)pkm).FormDuration = GetFormDuration(pkm, isParty);
        }

        private static uint GetFormDuration(PKM pkm, bool isParty)
        {
            if (!isParty || pkm.AltForm == 0)
                return 0;
            switch (pkm.Species)
            {
                case 676: return 5; // Furfrou
                case 720: return 3; // Hoopa
                default: return 0;
            }
        }

        public static bool SanitizeFormsToIterate(int species, out int formStart, out int formEnd, int formIn, bool USUM)
        {
            // 004AA370 in Moon
            // Simplified in terms of usage -- only overrides to give all the battle forms for a pkm
            switch (species)
            {
                case 351: // Castform
                    formStart = 0;
                    formEnd = 3;
                    return true;


                case 421: // Cherrim
                case 555: // Darmanitan
                case 648: // Meloetta
                case 746: // Wishiwashi
                case 778: // Mimikyu
                    // Alolans
                case 020: // Raticate
                case 105: // Marowak
                    formStart = 0;
                    formEnd = 1;
                    return true;

                case 735: // Gumshoos
                case 758: // Salazzle
                case 754: // Lurantis
                case 738: // Vikavolt
                case 784: // Kommo-o
                case 752: // Araquanid
                case 777: // Togedemaru
                case 743: // Ribombee
                case 744: // Rockruff
                    break;

                case 774 when formIn <= 6: // Minior
                    break; // don't give meteor forms except the first

                case 718 when formIn > 1:
                    break;
                default:
                    int count = USUM ? DexFormUtil.GetDexFormCountUSUM(species) : DexFormUtil.GetDexFormCountSM(species);
                    formStart = formEnd = 0;
                    return count < formIn;
            }
            formStart = 0;
            formEnd = 0;
            return true;
        }

        private void SetDexFlags(int index, int gender, int shiny, int baseSpecies)
        {
            const int brSize = 0x8C;
            int shift = gender | (shiny << 1);
            int ofs = PokeDex // Raw Offset
                      + 0x08 // Magic + Flags
                      + 0x80 // Misc Data (1024 bits)
                      + 0x68; // Owned Flags

            int bd = index >> 3; // div8
            int bm = index & 7; // mod8
            int bd1 = baseSpecies >> 3;
            int bm1 = baseSpecies & 7;
            // Set the [Species/Gender/Shiny] Seen Flag
            int brSeen = shift * brSize;
            Data[ofs + brSeen + bd] |= (byte)(1 << bm);

            // Check Displayed Status for base form
            bool Displayed = false;
            for (int i = 0; i < 4; i++)
            {
                int brDisplayed = (4 + i) * brSize;
                Displayed |= (Data[ofs + brDisplayed + bd1] & (byte)(1 << bm1)) != 0;
            }

            // If form is not base form, check form too
            if (!Displayed && baseSpecies != index)
            {
                for (int i = 0; i < 4; i++)
                {
                    int brDisplayed = (4 + i) * brSize;
                    Displayed |= (Data[ofs + brDisplayed + bd] & (byte)(1 << bm)) != 0;
                }
            }
            if (Displayed)
                return;

            // Set the Display flag if none are set
            Data[ofs + ((4 + shift) * brSize) + bd] |= (byte)(1 << bm);
        }

        public bool NationalDex
        {
            get => (Data[PokeDex + 4] & 1) == 1;
            set => Data[PokeDex + 4] = (byte)((Data[PokeDex + 4] & 0xFE) | (value ? 1 : 0));
        }

        /// <summary>
        /// Gets the last viewed dex entry in the Pokedex (by National Dex ID), internally called DefaultMons
        /// </summary>
        public uint CurrentViewedDex => BitConverter.ToUInt32(Data, PokeDex + 4) >> 9 & 0x3FF;

        public override bool GetCaught(int species)
        {
            int bit = species - 1;
            int bd = bit >> 3; // div8
            int bm = bit & 7; // mod8
            int ofs = PokeDex // Raw Offset
                      + 0x08 // Magic + Flags
                      + 0x80; // Misc Data (1024 bits)
            return (1 << bm & Data[ofs + bd]) != 0;
        }

        public override bool GetSeen(int species)
        {
            const int brSize = 0x8C;

            int bit = species - 1;
            int bd = bit >> 3; // div8
            int bm = bit & 7; // mod8
            byte mask = (byte)(1 << bm);
            int ofs = PokeDex // Raw Offset
                      + 0x08 // Magic + Flags
                      + 0x80; // Misc Data (1024 bits)

            for (int i = 1; i <= 4; i++) // check all 4 seen flags (gender/shiny)
            {
                if ((Data[ofs + bd + (i * brSize)] & mask) != 0)
                    return true;
            }

            return false;
        }

        protected override byte[] DecryptPKM(byte[] data)
        {
            return PKX.DecryptArray(data);
        }

        public override int PartyCount
        {
            get => Data[Party + (6 * SIZE_PARTY)];
            protected set => Data[Party + (6 * SIZE_PARTY)] = (byte)value;
        }

        public override int BoxesUnlocked { get => Data[PCFlags + 1]; set => Data[PCFlags + 1] = (byte)value; }

        private void LoadBattleTeams()
        {
            for (int i = 0; i < TeamCount*6; i++)
            {
                short val = BitConverter.ToInt16(Data, BattleBoxFlags + (i * 2));
                if (val < 0)
                {
                    TeamSlots[i] = -1;
                    continue;
                }

                int box = val >> 8;
                int slot = val & 0xFF;
                int index = (BoxSlotCount * box) + slot;
                TeamSlots[i] = index & 0xFFFF;
            }
        }

        private void SaveBattleTeams()
        {
            for (int i = 0; i < TeamCount * 6; i++)
            {
                int index = TeamSlots[i];
                if (index < 0)
                {
                    BitConverter.GetBytes((short)index).CopyTo(Data, BattleBoxFlags + (i * 2));
                    continue;
                }

                int box = index / BoxSlotCount;
                int slot = index % BoxSlotCount;
                int val = (box << 8) | slot;
                BitConverter.GetBytes((short)val).CopyTo(Data, BattleBoxFlags + (i * 2));
            }
        }

        private bool IsTeamLocked(int team) => Data[PCBackgrounds - TeamCount - team] == 1;

        public override StorageSlotFlag GetSlotFlags(int index)
        {
            int team = Array.IndexOf(TeamSlots, index);
            if (team < 0)
                return StorageSlotFlag.None;

            team /= 6;
            var val = (StorageSlotFlag)((int)StorageSlotFlag.BattleTeam1 << team);
            if (IsTeamLocked(team))
                val |= StorageSlotFlag.Locked;
            return val;
        }

        private int FusedCount => USUM ? 3 : 1;

        public int GetFusedSlotOffset(int slot)
        {
            if (Fused < 0 || slot < 0 || slot >= FusedCount)
                return -1;
            return Fused + (SIZE_PARTY * slot); // 0x104*slot
        }

        public int GetSurfScore(int recordID)
        {
            if (recordID < 0 || recordID > 4)
                recordID = 0;
            return BitConverter.ToInt32(Data, Misc + 0x138 + (4 * recordID));
        }

        public void SetSurfScore(int recordID, int score)
        {
            if (recordID < 0 || recordID > 4)
                recordID = 0;
            SetData(BitConverter.GetBytes(score), Misc + 0x138 + (4 * recordID));
        }

        public string RotomOT
        {
            get => GetString(Trainer2 + 0x30, 0x1A);
            set => SetString(value, OTLength).CopyTo(Data, Trainer2 + 0x30);
        }

        public override int DaycareSeedSize => 32; // 128 bits

        public override int GetDaycareSlotOffset(int loc, int slot)
        {
            if (loc != 0)
                return -1;
            if (Daycare < 0)
                return -1;
            return Daycare + 1 + (slot * (SIZE_STORED + 1));
        }

        public override bool? IsDaycareOccupied(int loc, int slot)
        {
            if (loc != 0)
                return null;
            if (Daycare < 0)
                return null;

            return Data[Daycare + ((SIZE_STORED + 1) * slot)] != 0;
        }

        public override string GetDaycareRNGSeed(int loc)
        {
            if (loc != 0)
                return null;
            if (Daycare < 0)
                return null;

            var data = Data.Skip(Daycare + 0x1DC).Take(DaycareSeedSize / 2).Reverse().ToArray();
            return BitConverter.ToString(data).Replace("-", string.Empty);
        }

        public override bool? IsDaycareHasEgg(int loc)
        {
            if (loc != 0)
                return null;
            if (Daycare < 0)
                return null;

            return Data[Daycare + 0x1D8] == 1;
        }

        public override void SetDaycareOccupied(int loc, int slot, bool occupied)
        {
            if (loc != 0)
                return;
            if (Daycare < 0)
                return;

            Data[Daycare + ((SIZE_STORED + 1) * slot)] = (byte)(occupied ? 1 : 0);
        }

        public override void SetDaycareRNGSeed(int loc, string seed)
        {
            if (loc != 0)
                return;
            if (Daycare < 0)
                return;
            if (seed == null)
                return;
            if (seed.Length > DaycareSeedSize)
                return;

            Enumerable.Range(0, seed.Length)
                 .Where(x => x % 2 == 0)
                 .Reverse()
                 .Select(x => Convert.ToByte(seed.Substring(x, 2), 16))
                 .ToArray().CopyTo(Data, Daycare + 0x1DC);
        }

        public override void SetDaycareHasEgg(int loc, bool hasEgg)
        {
            if (loc != 0)
                return;
            if (Daycare < 0)
                return;

            Data[Daycare + 0x1D8] = (byte)(hasEgg ? 1 : 0);
        }

        // Mystery Gift
        protected override bool[] MysteryGiftReceivedFlags
        {
            get
            {
                if (WondercardData < 0 || WondercardFlags < 0)
                    return Array.Empty<bool>();

                bool[] result = new bool[(WondercardData-WondercardFlags)*8];
                for (int i = 0; i < result.Length; i++)
                    result[i] = (Data[WondercardFlags + (i>>3)] >> (i&7) & 0x1) == 1;
                return result;
            }
            set
            {
                if (WondercardData < 0 || WondercardFlags < 0)
                    return;
                if (value == null || (WondercardData - WondercardFlags)*8 != value.Length)
                    return;

                byte[] data = new byte[value.Length/8];
                for (int i = 0; i < value.Length; i++)
                {
                    if (value[i])
                        data[i>>3] |= (byte)(1 << (i&7));
                }

                SetData(data, WondercardFlags);
            }
        }

        protected override MysteryGift[] MysteryGiftCards
        {
            get
            {
                if (WondercardData < 0)
                    return Array.Empty<MysteryGift>();
                MysteryGift[] cards = new MysteryGift[GiftCountMax];
                for (int i = 0; i < cards.Length; i++)
                    cards[i] = GetWC7(i);

                return cards;
            }
            set
            {
                if (value == null)
                    return;
                if (value.Length > GiftCountMax)
                    Array.Resize(ref value, GiftCountMax);

                for (int i = 0; i < value.Length; i++)
                    SetWC7(value[i], i);
                for (int i = value.Length; i < GiftCountMax; i++)
                    SetWC7(new WC7(), i);
            }
        }

        private WC7 GetWC7(int index)
        {
            if (WondercardData < 0)
                throw new ArgumentException(nameof(WondercardData));
            if (index < 0 || index > GiftCountMax)
                throw new ArgumentException(nameof(index));

            return new WC7(GetData(WondercardData + (index * WC7.Size), WC7.Size));
        }

        private void SetWC7(MysteryGift wc7, int index)
        {
            if (WondercardData < 0)
                return;
            if (index < 0 || index > GiftCountMax)
                return;

            SetData(wc7.Data, WondercardData + (index * WC7.Size));
        }

        // Writeback Validity
        public override string MiscSaveChecks()
        {
            var sb = new StringBuilder();

            // FFFF checks
            for (int i = 0; i < Data.Length / 0x200; i++)
            {
                if (Data.Skip(i * 0x200).Take(0x200).Any(z => z != 0xFF))
                    continue;
                sb.Append("0x200 chunk @ 0x").AppendFormat("{0:X5}", i * 0x200).AppendLine(" is FF'd.");
                sb.AppendLine("Cyber will screw up (as of August 31st 2014).");
                sb.AppendLine();

                // Check to see if it is in the Pokedex
                if (i * 0x200 > PokeDex && i * 0x200 < PokeDex + 0x900)
                {
                    sb.Append("Problem lies in the Pokedex. ");
                    if (i * 0x200 == PokeDex + 0x400)
                        sb.Append("Remove a language flag for a species < 585, ie Petilil");
                }
                break;
            }
            return sb.ToString();
        }

        public override string MiscSaveInfo() => string.Join(Environment.NewLine, Blocks.Select(b => b.Summary));

        public bool MegaUnlocked
        {
            get => (Data[TrainerCard + 0x78] & 0x01) != 0;
            set => Data[TrainerCard + 0x78] = (byte)((Data[TrainerCard + 0x78] & 0xFE) | (value ? 1 : 0)); // in battle
            // Data[0x1F22] = (byte)((Data[0x1F22] & 0xFE) | (value ? 1 : 0)); // event
        }

        public bool ZMoveUnlocked
        {
            get => (Data[TrainerCard + 0x78] & 2) != 0;
            set => Data[TrainerCard + 0x78] = (byte)((Data[TrainerCard + 0x78] & ~2) | (value ? 2 : 0)); // in battle
        }

        public override string GetString(byte[] data, int offset, int length) => StringConverter.GetString7(data, offset, length);

        public override byte[] SetString(string value, int maxLength, int PadToSize = 0, ushort PadWith = 0)
        {
            if (PadToSize == 0)
                PadToSize = maxLength + 1;
            return StringConverter.SetString7(value, maxLength, Language, PadToSize, PadWith);
        }
    }
}
