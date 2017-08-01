using System;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace PKHeX.Core
{
    public sealed class SAV7 : SaveFile
    {
        // Save Data Attributes
        public override string BAKName => $"{FileName} [{OT} ({Version}) - {LastSavedTime}].bak";
        public override string Filter => "Main SAV|*.*";
        public override string Extension => "";
        public override string[] PKMExtensions => PKM.Extensions.Where(f =>
        {
            int gen = f.Last() - 0x30;
            return gen == 1 || (3 <= gen && gen <= 7);
        }).ToArray();

        public SAV7(byte[] data = null)
        {
            Data = data == null ? new byte[SaveUtil.SIZE_G7SM] : (byte[])data.Clone();
            BAK = (byte[])Data.Clone();
            Exportable = !Data.SequenceEqual(new byte[Data.Length]);

            // Load Info
            GetBlockInfo();
            GetSAVOffsets();

            HeldItems = Legal.HeldItems_SM;
            Personal = PersonalTable.SM;
            if (!Exportable)
                ClearBoxes();

            var demo = new byte[0x4C4].SequenceEqual(Data.Skip(PCLayout).Take(0x4C4)); // up to Battle Box values
            if (demo || !Exportable)
            {
                PokeDex = -1; // Disabled
                LockedSlots = new int[0];
                TeamSlots = new int[0];
            }
            else // Valid slot locking info present
            {
                int lockedCount = 0, teamCount = 0;
                for (int i = 0; i < TeamCount; i++)
                {
                    bool locked = Data[PCBackgrounds - TeamCount - i] == 1;
                    for (int j = 0; j < 6; j++)
                    {
                        short val = BitConverter.ToInt16(Data, BattleBoxFlags + (i*6 + j) * 2);
                        if (val < 0)
                            continue;

                        var slotVal = (BoxSlotCount*(val >> 8) + (val & 0xFF)) & 0xFFFF;

                        if (locked)
                            LockedSlots[lockedCount++] = slotVal;
                        else TeamSlots[teamCount++] = slotVal;
                    }
                }
                Array.Resize(ref LockedSlots, lockedCount);
                Array.Resize(ref TeamSlots, teamCount);
            }
        }

        // Configuration
        public override SaveFile Clone() { return new SAV7(Data); }
        
        public override int SIZE_STORED => PKX.SIZE_6STORED;
        protected override int SIZE_PARTY => PKX.SIZE_6PARTY;
        public override PKM BlankPKM => new PK7();
        public override Type PKMType => typeof(PK7);

        public override int BoxCount => 32;
        public override int MaxEV => 252;
        public override int Generation => 7;
        protected override int GiftCountMax => 48;
        protected override int GiftFlagMax => 0x100 * 8;
        protected override int EventFlagMax => 3968;
        protected override int EventConstMax => (EventFlag - EventConst) / 2;
        public override int OTLength => 12;
        public override int NickLength => 12;

        public override int MaxMoveID => Legal.MaxMoveID_7;
        public override int MaxSpeciesID => Legal.MaxSpeciesID_7;
        public override int MaxItemID => Legal.MaxItemID_7;
        public override int MaxAbilityID => Legal.MaxAbilityID_7;
        public override int MaxBallID => Legal.MaxBallID_7; // 26
        public override int MaxGameID => Legal.MaxGameID_7;

        public int QRSaveData;

        // Feature Overrides
        public override bool HasGeolocation => true;

        // Blocks & Offsets
        private int BlockInfoOffset;
        private BlockInfo[] Blocks;
        private void GetBlockInfo()
        {
            BlockInfoOffset = Data.Length - 0x200 + 0x10;
            if (BitConverter.ToUInt32(Data, BlockInfoOffset) != SaveUtil.BEEF)
                BlockInfoOffset -= 0x200; // No savegames have more than 0x3D blocks, maybe in the future?
            int count = (Data.Length - BlockInfoOffset - 0x8) / 8;
            BlockInfoOffset += 4;

            Blocks = new BlockInfo[count];
            int CurrentPosition = 0;
            for (int i = 0; i < Blocks.Length; i++)
            {
                Blocks[i] = new BlockInfo
                {
                    Offset = CurrentPosition,
                    Length = BitConverter.ToInt32(Data, BlockInfoOffset + 0 + 8 * i),
                    ID = BitConverter.ToUInt16(Data, BlockInfoOffset + 4 + 8 * i),
                    Checksum = BitConverter.ToUInt16(Data, BlockInfoOffset + 6 + 8 * i)
                };

                // Expand out to nearest 0x200
                CurrentPosition += Blocks[i].Length % 0x200 == 0 ? Blocks[i].Length : 0x200 - Blocks[i].Length % 0x200 + Blocks[i].Length;

                if ((Blocks[i].ID != 0) || i == 0) continue;
                count = i;
                break;
            }
            // Fix Final Array Lengths
            Array.Resize(ref Blocks, count);
        }
        protected override void SetChecksums()
        {
            // Check for invalid block lengths
            if (Blocks.Length < 3) // arbitrary...
            {
                Debug.WriteLine("Not enough blocks ({0}), aborting SetChecksums", Blocks.Length);
                return;
            }
            // Apply checksums
            for (int i = 0; i < Blocks.Length; i++)
            {
                if (Blocks[i].Length + Blocks[i].Offset > Data.Length)
                { Debug.WriteLine("Block {0} has invalid offset/length value.", i); return; }
                byte[] array = new byte[Blocks[i].Length];
                Array.Copy(Data, Blocks[i].Offset, array, 0, array.Length);
                BitConverter.GetBytes(SaveUtil.CRC16_7(array, Blocks[i].ID)).CopyTo(Data, BlockInfoOffset + 6 + i * 8);
            }
            
            Data = SaveUtil.Resign7(Data);
        }
        public override bool ChecksumsValid
        {
            get
            {
                for (int i = 0; i < Blocks.Length; i++)
                {
                    if (Blocks[i].Length + Blocks[i].Offset > Data.Length)
                        return false;
                    byte[] array = new byte[Blocks[i].Length];
                    Array.Copy(Data, Blocks[i].Offset, array, 0, array.Length);
                    if (SaveUtil.CRC16_7(array, Blocks[i].ID) != BitConverter.ToUInt16(Data, BlockInfoOffset + 6 + i * 8))
                        return false;
                }
                return true;
            }
        }
        public override string ChecksumInfo
        {
            get
            {
                int invalid = 0;
                string rv = "";
                for (int i = 0; i < Blocks.Length; i++)
                {
                    if (Blocks[i].Length + Blocks[i].Offset > Data.Length)
                        return $"Block {i} Invalid Offset/Length.";
                    byte[] array = new byte[Blocks[i].Length];
                    Array.Copy(Data, Blocks[i].Offset, array, 0, array.Length);
                    if (SaveUtil.CRC16_7(array, Blocks[i].ID) == BitConverter.ToUInt16(Data, BlockInfoOffset + 6 + i * 8))
                        continue;

                    invalid++;
                    rv += $"Invalid: {i:X2} @ Region {Blocks[i].Offset:X5}" + Environment.NewLine;
                }
                // Return Outputs
                rv += $"SAV: {Blocks.Length - invalid}/{Blocks.Length + Environment.NewLine}";
                return rv;
            }
        }
        public override ulong? Secure1
        {
            get => BitConverter.ToUInt64(Data, BlockInfoOffset - 0x14);
            set => BitConverter.GetBytes(value ?? 0).CopyTo(Data, BlockInfoOffset - 0x14);
        }
        public override ulong? Secure2
        {
            get => BitConverter.ToUInt64(Data, BlockInfoOffset - 0xC);
            set => BitConverter.GetBytes(value ?? 0).CopyTo(Data, BlockInfoOffset - 0xC);
        }

        private void GetSAVOffsets()
        {
            if (SM)
            {
                /* 00 */ Bag           = 0x00000;  // [DE0]    MyItem
                /* 01 */ Trainer1       = 0x00E00;  // [07C]    Situation
                /* 02 */            //  = 0x01000;  // [014]    RandomGroup
                /* 03 */ TrainerCard    = 0x01200;  // [0C0]    MyStatus
                /* 04 */ Party          = 0x01400;  // [61C]    PokePartySave
                /* 05 */ EventConst     = 0x01C00;  // [E00]    EventWork
                /* 06 */ PokeDex        = 0x02A00;  // [F78]    ZukanData
                /* 07 */ GTS            = 0x03A00;  // [228]    GtsData
                /* 08 */ Fused          = 0x03E00;  // [104]    UnionPokemon 
                /* 09 */ Misc           = 0x04000;  // [200]    Misc
                /* 10 */ Trainer2       = 0x04200;  // [020]    FieldMenu
                /* 11 */            //  = 0x04400;  // [004]    ConfigSave
                /* 12 */ AdventureInfo  = 0x04600;  // [058]    GameTime
                /* 13 */ PCLayout       = 0x04800;  // [5E6]    BOX
                /* 14 */ Box            = 0x04E00;  // [36600]  BoxPokemon
                /* 15 */ Resort         = 0x3B400;  // [572C]   ResortSave
                /* 16 */ PlayTime       = 0x40C00;  // [008]    PlayTime
                /* 17 */ Overworld      = 0x40E00;  // [1080]   FieldMoveModelSave
                /* 18 */ Fashion        = 0x42000;  // [1A08]   Fashion
                /* 19 */            //  = 0x43C00;  // [6408]   JoinFestaPersonalSave
                /* 20 */            //  = 0x4A200;  // [6408]   JoinFestaPersonalSave
                /* 21 */ JoinFestaData  = 0x50800;  // [3998]   JoinFestaDataSave
                /* 22 */            //  = 0x54200;  // [100]    BerrySpot
                /* 23 */            //  = 0x54400;  // [100]    FishingSpot
                /* 24 */            //  = 0x54600;  // [10528]  LiveMatchData
                /* 25 */            //  = 0x64C00;  // [204]    BattleSpotData
                /* 26 */ PokeFinderSave = 0x65000;  // [B60]    PokeFinderSave
                /* 27 */ WondercardFlags = 0x65C00; // [3F50]   MysteryGiftSave
                /* 28 */ Record         = 0x69C00;  // [358]    Record
                /* 29 */            //  = 0x6A000;  // [728]    Data Block
                /* 30 */            //  = 0x6A800;  // [200]    GameSyncSave
                /* 31 */            //  = 0x6AA00;  // [718]    PokeDiarySave
                /* 32 */ BattleTree     = 0x6B200;  // [1FC]    BattleInstSave
                /* 33 */ Daycare        = 0x6B400;  // [200]    Sodateya
                /* 34 */            //  = 0x6B600;  // [120]    WeatherSave
                /* 35 */ QRSaveData     = 0x6B800;  // [1C8]    QRReaderSaveData
                /* 36 */            //  = 0x6BA00;  // [200]    TurtleSalmonSave

                EventFlag = EventConst + 0x7D0;

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

                HoF = 0x25C0; // Inside EventWork (const/flag) block

                FashionLength = 0x1A08;

                TeamCount = 6;
                LockedSlots = new int[6*TeamCount];
                TeamSlots = new int[6*TeamCount];
            }
            else // Empty input
            {
                Party = 0x0;
                Box = Party + SIZE_PARTY * 6 + 0x1000;
            }
        }

        // Private Only
        private int Bag { get; set; } = int.MinValue;
        private int AdventureInfo { get; set; } = int.MinValue;
        private int Trainer2 { get; set; } = int.MinValue;
        private int Misc { get; set; } = int.MinValue;
        private int LastViewedBox { get; set; } = int.MinValue;
        private int WondercardFlags { get; set; } = int.MinValue;
        private int PlayTime { get; set; } = int.MinValue;
        private int ItemInfo { get; set; } = int.MinValue;
        private int Overworld { get; set; } = int.MinValue;
        private int JoinFestaData { get; set; } = int.MinValue;
        private int PokeFinderSave { get; set; } = int.MinValue;
        private int BattleTree { get; set; } = int.MinValue;
        private int BattleBoxFlags { get; set; } = int.MinValue;
        private int TeamCount { get; set; } = int.MinValue;

        // Accessible as SAV7
        private int TrainerCard { get; set; } = 0x14000;
        private int Resort { get; set; }
        private int PCFlags { get; set; } = int.MinValue;
        public int PSSStats { get; private set; } = int.MinValue;
        public int MaisonStats { get; private set; } = int.MinValue;
        private int PCBackgrounds { get; set; } = int.MinValue;
        public int Contest { get; private set; } = int.MinValue;
        public int Accessories { get; private set; } = int.MinValue;
        public int PokeDexLanguageFlags { get; private set; } = int.MinValue;
        public int Fashion { get; set; } = int.MinValue;
        public int FashionLength { get; set; } = int.MinValue;
        private int Record { get; set; } = int.MinValue;

        private const int ResortCount = 93;
        public PKM[] ResortPKM
        {
            get
            {
                PKM[] data = new PKM[ResortCount];
                for (int i = 0; i < data.Length; i++)
                {
                    data[i] = GetPKM(GetData(Resort + 0x12 + i * SIZE_STORED, SIZE_STORED));
                    data[i].Identifier = $"Resort Slot {i}";
                }
                return data;
            }
            set
            {
                if (value?.Length != ResortCount)
                    throw new ArgumentException();

                for (int i = 0; i < value.Length; i++)
                    SetStoredSlot(value[i], Resort + 0x12 + i*SIZE_STORED);
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
                }
                return GameVersion.Unknown;
            }
        }
        
        // Player Information
        public override ushort TID
        {
            get => BitConverter.ToUInt16(Data, TrainerCard + 0);
            set => BitConverter.GetBytes(value).CopyTo(Data, TrainerCard + 0);
        }
        public override ushort SID
        {
            get => BitConverter.ToUInt16(Data, TrainerCard + 2);
            set => BitConverter.GetBytes(value).CopyTo(Data, TrainerCard + 2);
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
                return BitConverter.ToString(data).Replace("-", "");
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
                return BitConverter.ToString(data).Replace("-", "");
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
        public int BallThrowType
        {
            get => Data[TrainerCard + 0x7A];
            set => Data[TrainerCard + 0x7A] = (byte)(value > 8 ? 0 : value);
        }
        public int M
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
        public int DaysFromRefreshed
        {
            get => Data[Misc + 0x123];
            set => Data[Misc + 0x123] = (byte)value;
        }
        public uint UsedFestaCoins
        {
            get => BitConverter.ToUInt32(Data, 0x69C98);
            set
            {
                if (value > 9999999) value = 9999999;
                BitConverter.GetBytes(value).CopyTo(Data, 0x69C98);
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
        public sealed class FashionItem
        {
            public bool IsOwned;
            public bool IsNew;
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
        private int LastSavedYear { get => (int)(LastSaved & 0xFFF); set => LastSaved = LastSaved & 0xFFFFF000 | (uint)value; }
        private int LastSavedMonth { get => (int)(LastSaved >> 12 & 0xF); set => LastSaved = LastSaved & 0xFFFF0FFF | ((uint)value & 0xF) << 12; }
        private int LastSavedDay { get => (int)(LastSaved >> 16 & 0x1F); set => LastSaved = LastSaved & 0xFFE0FFFF | ((uint)value & 0x1F) << 16; }
        private int LastSavedHour { get => (int)(LastSaved >> 21 & 0x1F); set => LastSaved = LastSaved & 0xFC1FFFFF | ((uint)value & 0x1F) << 21; }
        private int LastSavedMinute { get => (int)(LastSaved >> 26 & 0x3F); set => LastSaved = LastSaved & 0x03FFFFFF | ((uint)value & 0x3F) << 26; }
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
        public override int SecondsToStart { get => BitConverter.ToInt32(Data, AdventureInfo + 0x28); set => BitConverter.GetBytes(value).CopyTo(Data, AdventureInfo + 0x28); }
        public override int SecondsToFame { get => BitConverter.ToInt32(Data, AdventureInfo + 0x30); set => BitConverter.GetBytes(value).CopyTo(Data, AdventureInfo + 0x30); }
        
        public ulong AlolaTime { get => BitConverter.ToUInt64(Data, AdventureInfo + 0x48); set => BitConverter.GetBytes(value).CopyTo(Data, AdventureInfo + 0x48); }

        // Stat Records
        public int GetRecord(int recordID)
        {
            int ofs = GetRecordOffset(recordID);
            if (recordID < 100)
                return BitConverter.ToInt32(Data, ofs);
            if (recordID < 200)
                return BitConverter.ToInt16(Data, ofs);
            return 0;
        }
        public void SetRecord(int recordID, int value)
        {
            int ofs = GetRecordOffset(recordID);
            int max = GetRecordMax(recordID);
            if (value > max)
                value = max;
            if (recordID < 100)
                BitConverter.GetBytes(value).CopyTo(Data, ofs);
            if (recordID < 200)
                BitConverter.GetBytes((ushort)value).CopyTo(Data, ofs);
        }
        public int GetRecordOffset(int recordID)
        {
            if (recordID < 100)
                return Record + recordID*4;
            if (recordID < 200)
                return Record + recordID*2 + 200; // first 100 are 4bytes, so bias the difference
            return -1;
        }

        public static int GetRecordMax(int recordID) => recordID < 200 ? RecordMax[RecordMaxType[recordID]] : 0;
        private static readonly int[] RecordMax = {999999999, 9999999, 999999, 99999, 65535, 9999, 999};
        private static readonly int[] RecordMaxType =
        {
            0, 0, 0, 0, 0, 0, 2, 2, 2, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 2, 2, 2, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 2, 2, 2, 0, 0, 0, 2, 2, 0,
            0, 2, 2, 2, 2, 2, 2, 2, 2, 2,
            2, 2, 2, 2, 2, 2, 1, 2, 2, 2,
            2, 2, 2, 2, 2, 2, 2, 2, 2, 2,
            2, 2, 2, 2, 2, 2, 2, 2, 2, 2,
            2, 2, 2, 2, 2, 2, 2, 2, 2, 2,

            5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
            5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
            5, 5, 5, 5, 5, 5, 6, 5, 5, 5,
            5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
            5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
            5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
            5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
            5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
            5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
            5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
        };

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
                if (value > 9999999) // 9mil;
                    value = 9999999;
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
                InventoryPouch[] pouch =
                {
                    new InventoryPouch(InventoryType.Medicine, Legal.Pouch_Medicine_SM, 999, OFS_PouchMedicine),
                    new InventoryPouch(InventoryType.Items, Legal.Pouch_Items_SM, 999, OFS_PouchHeldItem),
                    new InventoryPouch(InventoryType.TMHMs, Legal.Pouch_TMHM_SM, 1, OFS_PouchTMHM),
                    new InventoryPouch(InventoryType.Berries, Legal.Pouch_Berries_SM, 999, OFS_PouchBerry),
                    new InventoryPouch(InventoryType.KeyItems, Legal.Pouch_Key_SM, 1, OFS_PouchKeyItem),
                    new InventoryPouch(InventoryType.ZCrystals, Legal.Pouch_ZCrystal_SM, 1, OFS_PouchZCrystals),
                };
                foreach (var p in pouch)
                    p.GetPouch7(ref Data);
                return pouch;
            }
            set
            {
                foreach (var p in value)
                    p.SetPouch7(ref Data);
            }
        }

        // Battle Tree
        public int GetTreeStreak(int battletype, bool super, bool max)
        {
            if (battletype > 3)
                throw new ArgumentException();

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
                throw new ArgumentException();

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
            return Party + SIZE_PARTY * slot;
        }
        public override int GetBoxOffset(int box)
        {
            return Box + SIZE_STORED*box*30;
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
                return "B" + (box + 1);
            return Util.TrimFromZero(Encoding.Unicode.GetString(Data, PCLayout + 0x22*box, 0x22));
        }
        public override void SetBoxName(int box, string val)
        {
            Encoding.Unicode.GetBytes(val.PadRight(0x11, '\0')).CopyTo(Data, PCLayout + 0x22*box);
            Edited = true;
        }
        public override PKM GetPKM(byte[] data)
        {
            return new PK7(data);
        }
        protected override void SetPKM(PKM pkm)
        {
            PK7 pk7 = pkm as PK7;
            // Apply to this Save File
            int CT = pk7.CurrentHandler;
            DateTime Date = DateTime.Now;
            pk7.Trade(OT, TID, SID, Country, SubRegion, Gender, false, Date.Day, Date.Month, Date.Year);
            if (CT != pk7.CurrentHandler) // Logic updated Friendship
            {
                // Copy over the Friendship Value only under certain circumstances
                if (pk7.Moves.Contains(216)) // Return
                    pk7.CurrentFriendship = pk7.OppositeFriendship;
                else if (pk7.Moves.Contains(218)) // Frustration
                    pkm.CurrentFriendship = pk7.OppositeFriendship;
            }
            pkm.RefreshChecksum();
        }
        protected override void SetDex(PKM pkm)
        {
            if (PokeDex < 0 || Version == GameVersion.Unknown) // sanity
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
                    BitConverter.GetBytes(pkm.EncryptionConstant).CopyTo(Data, PokeDex + 0x8E8 + shift * 4);
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
            bool reset = SanitizeFormsToIterate(pkm.Species, out int fs, out int fe, formstart);
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
                        int f = SaveUtil.GetDexFormIndexSM(pkm.Species, fc, MaxSpeciesID - 1);
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
                int lbit = bit * langCount + lang;
                if (lbit >> 3 < 920) // Sanity check for max length of region
                    Data[PokeDexLanguageFlags + (lbit >> 3)] |= (byte)(1 << (lbit & 7));
            }
        }
        protected override void SetPartyValues(PKM pkm, bool isParty)
        {
            uint duration = 0;
            if (isParty && pkm.AltForm != 0)
                switch (pkm.Species)
                {
                    case 676:
                        duration = 5;
                        break;
                    case 720: // Hoopa
                        duration = 3;
                        break;
                }

            ((PK7)pkm).FormDuration = duration;
        }
        private static bool SanitizeFormsToIterate(int species, out int formStart, out int formEnd, int formIn)
        {
            // 004AA370 in Moon
            // Simplified in terms of usage -- only overrides to give all the battle forms for a pkm
            formStart = 0;
            formEnd = 0;
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
                    formStart = 0;
                    formEnd = 1;
                    return true;

                case 774: // Minior
                    // Cores forms are after Meteor forms, so the game iterator would give all meteor forms (NO!)
                    // So the game so the game chooses to only award entries for Core forms after they appear in battle.
                    return formIn > 6; // resets to 0/0 if an invalid request is made (non-form entry)
                    
                case 718:
                    if (formIn == 3) // complete
                        return true; // 0/0
                    if (formIn != 2) // give
                        return false;

                    // Apparently form 2 is invalid (50% core-ability), set to 10%'s form
                    formStart = 1; 
                    formEnd = 1;
                    return true;
                default:
                    return false;
            }
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
            Data[ofs + (4 + shift) * brSize + bd] |= (byte)(1 << bm);
        }

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
                if ((Data[ofs + bd + i * brSize] & mask) != 0)
                    return true;
            return false;
        }
        public override byte[] DecryptPKM(byte[] data)
        {
            return PKX.DecryptArray(data);
        }
        public override int PartyCount
        {
            get => Data[Party + 6 * SIZE_PARTY];
            protected set => Data[Party + 6 * SIZE_PARTY] = (byte)value;
        }
        public override int BoxesUnlocked { get => Data[PCFlags + 1]; set => Data[PCFlags + 1] = (byte)value; }
        public override bool IsSlotLocked(int box, int slot)
        {
            if (slot >= 30 || box >= BoxCount)
                return false;

            int slotIndex = slot + BoxSlotCount*box;
            return LockedSlots.Any(s => s == slotIndex);
        }
        public override bool IsSlotInBattleTeam(int box, int slot)
        {
            if (slot >= 30 || box >= BoxCount)
                return false;

            int slotIndex = slot + BoxSlotCount * box;
            return TeamSlots.Any(s => s == slotIndex);
        }

        public override int DaycareSeedSize => 32; // 128 bits
        public override int GetDaycareSlotOffset(int loc, int slot)
        {
            if (loc != 0)
                return -1;
            if (Daycare < 0)
                return -1;
            return Daycare + 1 + slot * (SIZE_STORED + 1);
        }
        public override bool? IsDaycareOccupied(int loc, int slot)
        {
            if (loc != 0)
                return null;
            if (Daycare < 0)
                return null;

            return Data[Daycare + (SIZE_STORED + 1) * slot] != 0;
        }
        public override string GetDaycareRNGSeed(int loc)
        {
            if (loc != 0)
                return null;
            if (Daycare < 0)
                return null;

            var data = Data.Skip(Daycare + 0x1DC).Take(DaycareSeedSize / 2).Reverse().ToArray();
            return BitConverter.ToString(data).Replace("-", "");
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
            
            Data[Daycare + (SIZE_STORED + 1) * slot] = (byte)(occupied ? 1 : 0);
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
                    return null;

                bool[] r = new bool[(WondercardData-WondercardFlags)*8];
                for (int i = 0; i < r.Length; i++)
                    r[i] = (Data[WondercardFlags + (i>>3)] >> (i&7) & 0x1) == 1;
                return r;
            }
            set
            {
                if (WondercardData < 0 || WondercardFlags < 0)
                    return;
                if ((WondercardData - WondercardFlags)*8 != value?.Length)
                    return;

                byte[] data = new byte[value.Length/8];
                for (int i = 0; i < value.Length; i++)
                    if (value[i])
                        data[i>>3] |= (byte)(1 << (i&7));

                data.CopyTo(Data, WondercardFlags);
                Edited = true;
            }
        }
        protected override MysteryGift[] MysteryGiftCards
        {
            get
            {
                if (WondercardData < 0)
                    return null;
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
                return null;
            if (index < 0 || index > GiftCountMax)
                return null;

            return new WC7(GetData(WondercardData + index * WC7.Size, WC7.Size));
        }
        private void SetWC7(MysteryGift wc7, int index)
        {
            if (WondercardData < 0)
                return;
            if (index < 0 || index > GiftCountMax)
                return;

            wc7.Data.CopyTo(Data, WondercardData + index * WC7.Size);

            Edited = true;
        }

        // Writeback Validity
        public override string MiscSaveChecks()
        {
            var r = new StringBuilder();

            // MemeCrypto check
            if (RequiresMemeCrypto && !MemeCrypto.CanUseMemeCrypto())
            {
                r.AppendLine("Platform does not support required cryptography providers.");
                r.AppendLine("Checksum will be broken until the file is saved using an OS without FIPS compliance enabled or a newer OS.");
                r.AppendLine();
            }

            // FFFF checks
            byte[] FFFF = Enumerable.Repeat((byte)0xFF, 0x200).ToArray();
            for (int i = 0; i < Data.Length / 0x200; i++)
            {
                if (!FFFF.SequenceEqual(Data.Skip(i * 0x200).Take(0x200))) continue;
                r.AppendLine($"0x200 chunk @ 0x{i*0x200:X5} is FF'd.");
                r.AppendLine("Cyber will screw up (as of August 31st 2014).");
                r.AppendLine();

                // Check to see if it is in the Pokedex
                if (i * 0x200 > PokeDex && i * 0x200 < PokeDex + 0x900)
                {
                    r.Append("Problem lies in the Pokedex. ");
                    if (i * 0x200 == PokeDex + 0x400)
                        r.Append("Remove a language flag for a species < 585, ie Petilil");
                }
                break;
            }
            return r.ToString();
        }
        public override string MiscSaveInfo()
        {
            return Blocks.Aggregate("", (current, b) => current +
                $"{b.ID:00}: {b.Offset:X5}-{b.Offset + b.Length:X5}, {b.Length:X5}{Environment.NewLine}");
        }
        public byte BallThrowTypeUnlocked
        {
            get => (byte)(((BitConverter.ToUInt16(Data, 0x23F4) << 4) >> 10) << 2);
            set
            {
                ushort flags = (ushort)(BitConverter.ToUInt16(Data, 0x23F4) & 0xF03F);
                flags |= (ushort)((value & 0xFC) << 4);
                BitConverter.GetBytes(flags).CopyTo(Data, 0x23F4);
            }
        }
        public byte BallThrowTypeLearned
        {
            get => (byte)((Data[0x2583] & 0x7F) << 1);
            set => Data[0x2583] = (byte)((Data[0x2583] & 0x80) | ((value & 0xFE) >> 1));
        }
        public byte BattleTreeSuperUnlocked
        {
            get => (byte)(Data[0x23F9] >> 5);
            set => Data[0x23F9] = (byte)((Data[0x23F9] & 0x1F) | ((value & 0x07) << 5));
        }
        public bool MegaUnlocked
        {
            get => (Data[0x1278] & 0x01) != 0;
            set => Data[0x1278] = (byte)((Data[0x1278] & 0xFE) | (value ? 1 : 0)); // in battle
            // Data[0x1F22] = (byte)((Data[0x1F22] & 0xFE) | (value ? 1 : 0)); // event
        }

        public override bool RequiresMemeCrypto => true;

        public override string GetString(int Offset, int Count) => StringConverter.GetString7(Data, Offset, Count);
        public override byte[] SetString(string value, int maxLength, int PadToSize = 0, ushort PadWith = 0)
        {
            if (PadToSize == 0)
                PadToSize = maxLength + 1;
            return StringConverter.SetString7(value, maxLength, PadToSize, PadWith);
        }
    }
}
