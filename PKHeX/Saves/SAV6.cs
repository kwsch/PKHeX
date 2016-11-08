using System;
using System.Linq;
using System.Text;

namespace PKHeX
{
    public sealed class SAV6 : SaveFile
    {
        // Save Data Attributes
        public override string BAKName => $"{FileName} [{OT} ({Version}) - {LastSavedTime}].bak";
        public override string Filter => "Main SAV|*.*";
        public override string Extension => "";
        public SAV6(byte[] data = null)
        {
            Data = data == null ? new byte[SaveUtil.SIZE_G6ORAS] : (byte[])data.Clone();
            BAK = (byte[])Data.Clone();
            Exportable = !Data.SequenceEqual(new byte[Data.Length]);

            // Load Info
            getBlockInfo();
            getSAVOffsets();

            HeldItems = ORAS ? Legal.HeldItem_AO : Legal.HeldItem_XY;
            Personal = ORAS ? PersonalTable.AO : PersonalTable.XY;
            if (!Exportable)
                resetBoxes();
        }

        // Configuration
        public override SaveFile Clone() { return new SAV6(Data); }
        
        public override int SIZE_STORED => PKX.SIZE_6STORED;
        public override int SIZE_PARTY => PKX.SIZE_6PARTY;
        public override PKM BlankPKM => new PK6();
        public override Type PKMType => typeof(PK6);

        public override int BoxCount => 31;
        public override int MaxEV => 252;
        public override int Generation => 6;
        protected override int GiftCountMax => 24;
        protected override int GiftFlagMax => 0x100 * 8;
        protected override int EventFlagMax => 8 * 0x180;
        protected override int EventConstMax => (EventFlag - EventConst) / 2;
        public override int OTLength => 12;
        public override int NickLength => 12;

        public override int MaxMoveID => XY ? 617 : 621;
        public override int MaxSpeciesID => Legal.MaxSpeciesID_6;
        public override int MaxItemID => XY ? 717 : 775;
        public override int MaxAbilityID => XY ? 188 : 191;
        public override int MaxBallID => 0x19;
        public override int MaxGameID => 27; // OR

        // Feature Overrides
        public override bool HasGeolocation => true;

        // Blocks & Offsets
        private int BlockInfoOffset;
        private BlockInfo[] Blocks;
        private void getBlockInfo()
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
        protected override void setChecksums()
        {
            // Check for invalid block lengths
            if (Blocks.Length < 3) // arbitrary...
            {
                Console.WriteLine("Not enough blocks ({0}), aborting setChecksums", Blocks.Length);
                return;
            }
            // Apply checksums
            for (int i = 0; i < Blocks.Length; i++)
            {
                byte[] array = new byte[Blocks[i].Length];
                Array.Copy(Data, Blocks[i].Offset, array, 0, array.Length);
                BitConverter.GetBytes(SaveUtil.ccitt16(array)).CopyTo(Data, BlockInfoOffset + 6 + i * 8);
            }
        }
        public override bool ChecksumsValid
        {
            get
            {
                for (int i = 0; i < Blocks.Length; i++)
                {
                    byte[] array = new byte[Blocks[i].Length];
                    Array.Copy(Data, Blocks[i].Offset, array, 0, array.Length);
                    if (SaveUtil.ccitt16(array) != BitConverter.ToUInt16(Data, BlockInfoOffset + 6 + i * 8))
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
                    byte[] array = new byte[Blocks[i].Length];
                    Array.Copy(Data, Blocks[i].Offset, array, 0, array.Length);
                    if (SaveUtil.ccitt16(array) == BitConverter.ToUInt16(Data, BlockInfoOffset + 6 + i * 8))
                        continue;

                    invalid++;
                    rv += $"Invalid: {i.ToString("X2")} @ Region {Blocks[i].Offset.ToString("X5") + Environment.NewLine}";
                }
                // Return Outputs
                rv += $"SAV: {Blocks.Length - invalid}/{Blocks.Length + Environment.NewLine}";
                return rv;
            }   
        }
        public override ulong? Secure1
        {
            get { return BitConverter.ToUInt64(Data, BlockInfoOffset - 0x14); }
            set { BitConverter.GetBytes(value ?? 0).CopyTo(Data, BlockInfoOffset - 0x14); }
        }
        public override ulong? Secure2
        {
            get { return BitConverter.ToUInt64(Data, BlockInfoOffset - 0xC); }
            set { BitConverter.GetBytes(value ?? 0).CopyTo(Data, BlockInfoOffset - 0xC); }
        }
        
        private void getSAVOffsets()
        {
            if (ORASDEMO)
            {
                /* 00: */ Item = 0x00000;
                /* 01: */ ItemInfo = 0x00C00; // Select Bound Items
                /* 02: */ AdventureInfo = 0x00E00;
                /* 03: */ Trainer1 = 0x01000;
                /* 04: */ // = 0x01200; [00004] // ???
                /* 05: */ PlayTime = 0x01400;
                /* 06: */ // = 0x01600; [00024] // FFFFFFFF
                /* 07: */ // = 0x01800; [02100] // Overworld Data
                /* 08: */ Trainer2 = 0x03A00;
                /* 09: */ TrainerCard = 0x03C00;
                /* 10: */ Party = 0x03E00;
                /* 11: */ EventConst = 0x04600; EventFlag = EventConst + 0x2FC;
                /* 12: */ // = 0x04C00; [00004] // 87B1A23F const
                /* 13: */ // = 0x04E00; [00048] // Repel Info, (Swarm?) and other overworld info
                /* 14: */ SUBE = 0x05000;
                /* 15: */ PSSStats = 0x05400;

                OFS_PouchHeldItem = Item + 0;
                OFS_PouchKeyItem = Item + 0x640;
                OFS_PouchTMHM = Item + 0x7C0;
                OFS_PouchMedicine = Item + 0x970;
                OFS_PouchBerry = Item + 0xA70;
            }
            else if (XY)
            {
                Puff = 0x00000;
                Item = 0x00400;
                ItemInfo = 0x1000;
                AdventureInfo = 0x01200;
                Trainer1 = 0x1400;
                PlayTime = 0x1800;
                Accessories = 0x1A00;
                Trainer2 = 0x4200;
                PCLayout = 0x4400;
                BattleBox = 0x04A00;
                PSS = 0x05000;
                TrainerCard = 0x14000;
                Party = 0x14200;
                EventConst = 0x14A00;
                PSSStats = 0x1E400;
                PokeDex = 0x15000;
                Fused = 0x16000;
                OPower = 0x16A00;
                GTS = 0x17800;
                HoF = 0x19400;
                MaisonStats = 0x1B1C0;
                Daycare = 0x1B200;
                BerryField = 0x1B800;
                WondercardFlags = 0x1BC00;
                SUBE = 0x1D890;
                SuperTrain = 0x1F200;
                LinkInfo = 0x1FE00;
                Box = 0x22600;
                JPEG = 0x57200;

                PCBackgrounds = PCLayout + 0x41E;
                PCFlags = PCLayout + 0x43D;
                LastViewedBox = PCLayout + 0x43F;
                EventFlag = EventConst + 0x2FC;
                PokeDexLanguageFlags = PokeDex + 0x3C8;
                Spinda = PokeDex + 0x648;
                WondercardData = WondercardFlags + 0x100;

                OFS_PouchHeldItem = Item + 0;
                OFS_PouchKeyItem = Item + 0x640;
                OFS_PouchTMHM = Item + 0x7C0;
                OFS_PouchMedicine = Item + 0x968;
                OFS_PouchBerry = Item + 0xA68;
            }
            else if (ORAS)
            {
                Puff = 0x00000;
                Item = 0x00400;
                ItemInfo = 0x1000;
                AdventureInfo = 0x01200;
                Trainer1 = 0x01400;
                PlayTime = 0x1800;
                Accessories = 0x1A00;
                Trainer2 = 0x04200;
                PCLayout = 0x04400;
                BattleBox = 0x04A00;
                PSS = 0x05000;
                TrainerCard = 0x14000;
                Party = 0x14200;
                EventConst = 0x14A00;
                PokeDex = 0x15000;
                Fused = 0x16A00;
                OPower = 0x17400;
                GTS = 0x18200;
                HoF = 0x19E00;
                MaisonStats = 0x1BBC0;
                Daycare = 0x1BC00;
                BerryField = 0x1C400;
                WondercardFlags = 0x1CC00;
                SUBE = 0x1D890;
                PSSStats = 0x1F400;
                SuperTrain = 0x20200;
                LinkInfo = 0x20E00;
                Contest = 0x23600;
                SecretBase = 0x23A00;
                EonTicket = 0x319B8;
                Box = 0x33000;
                JPEG = 0x67C00;

                PCBackgrounds = PCLayout + 0x41E;
                PCFlags = PCLayout + 0x43D;
                LastViewedBox = PCLayout + 0x43F;
                EventFlag = EventConst + 0x2FC;
                PokeDexLanguageFlags = PokeDex + 0x400;
                Spinda = PokeDex + 0x680;
                EncounterCount = PokeDex + 0x686;
                WondercardData = WondercardFlags + 0x100;
                Daycare2 = Daycare + 0x1F0;

                OFS_PouchHeldItem = Item + 0;
                OFS_PouchKeyItem = Item + 0x640;
                OFS_PouchTMHM = Item + 0x7C0;
                OFS_PouchMedicine = Item + 0x970;
                OFS_PouchBerry = Item + 0xA70;
            }
            else // Empty input
            {
                Party = 0x0;
                Box = Party + SIZE_PARTY * 6 + 0x1000;
            }
        }

        // Private Only
        private int Item { get; set; } = int.MinValue;
        private int AdventureInfo { get; set; } = int.MinValue;
        private int Trainer2 { get; set; } = int.MinValue;
        private int LastViewedBox { get; set; } = int.MinValue;
        private int WondercardFlags { get; set; } = int.MinValue;
        private int PlayTime { get; set; } = int.MinValue;
        private int JPEG { get; set; } = int.MinValue;
        private int ItemInfo { get; set; } = int.MinValue;
        private int Daycare2 { get; set; } = int.MinValue;
        private int LinkInfo { get; set; } = int.MinValue;

        // Accessible as SAV6
        public int TrainerCard { get; private set; } = 0x14000;
        public int PCFlags { get; private set; } = int.MinValue;
        public int PSSStats { get; private set; } = int.MinValue;
        public int MaisonStats { get; private set; } = int.MinValue;
        public int EonTicket { get; private set; } = int.MinValue;
        public int PCBackgrounds { get; private set; } = int.MinValue;
        public int Contest { get; private set; } = int.MinValue;
        public int Accessories { get; private set; } = int.MinValue;
        public int PokeDexLanguageFlags { get; private set; } = int.MinValue;
        public int Spinda { get; private set; } = int.MinValue;
        public int EncounterCount { get; private set; } = int.MinValue;

        public override GameVersion Version
        {
            get
            {
                switch (Game)
                {
                    case 24: return GameVersion.X;
                    case 25: return GameVersion.Y;
                    case 26: return GameVersion.AS;
                    case 27: return GameVersion.OR;
                }
                return GameVersion.Unknown;
            }
        }
        
        // Player Information
        public override ushort TID
        {
            get { return BitConverter.ToUInt16(Data, TrainerCard + 0); }
            set { BitConverter.GetBytes(value).CopyTo(Data, TrainerCard + 0); }
        }
        public override ushort SID
        {
            get { return BitConverter.ToUInt16(Data, TrainerCard + 2); }
            set { BitConverter.GetBytes(value).CopyTo(Data, TrainerCard + 2); }
        }
        public override int Game
        {
            get { return Data[TrainerCard + 4]; }
            set { Data[TrainerCard + 4] = (byte)value; }
        }
        public override int Gender
        {
            get { return Data[TrainerCard + 5]; }
            set { Data[TrainerCard + 5] = (byte)value; }
        }
        public int Sprite
        {
            get { return Data[TrainerCard + 7]; }
            set { Data[TrainerCard + 7] = (byte)value; }
        }
        public override ulong? GameSyncID
        {
            get { return BitConverter.ToUInt64(Data, TrainerCard + 8); }
            set { BitConverter.GetBytes(value ?? 0).CopyTo(Data, TrainerCard + 8); }
        }
        public override int SubRegion
        {
            get { return Data[TrainerCard + 0x26]; }
            set { Data[TrainerCard + 0x26] = (byte)value; }
        }
        public override int Country
        {
            get { return Data[TrainerCard + 0x27]; }
            set { Data[TrainerCard + 0x27] = (byte)value; }
        }
        public override int ConsoleRegion
        {
            get { return Data[TrainerCard + 0x2C]; }
            set { Data[TrainerCard + 0x2C] = (byte)value; }
        }
        public override int Language
        {
            get { return Data[TrainerCard + 0x2D]; }
            set { Data[TrainerCard + 0x2D] = (byte)value; }
        }
        public override string OT
        {
            get { return Util.TrimFromZero(Encoding.Unicode.GetString(Data, TrainerCard + 0x48, 0x1A)); }
            set { Encoding.Unicode.GetBytes(value.PadRight(13, '\0')).CopyTo(Data, TrainerCard + 0x48); }
        }
        public string Saying1
        {
            get { return Util.TrimFromZero(Encoding.Unicode.GetString(Data, TrainerCard + 0x7C + 0x22 * 0, 0x22)); }
            set { Encoding.Unicode.GetBytes(value.PadRight(value.Length + 1, '\0')).CopyTo(Data, TrainerCard + 0x7C + 0x22 * 0); }
        }
        public string Saying2
        {
            get { return Util.TrimFromZero(Encoding.Unicode.GetString(Data, TrainerCard + 0x7C + 0x22 * 1, 0x22)); }
            set { Encoding.Unicode.GetBytes(value.PadRight(value.Length + 1, '\0')).CopyTo(Data, TrainerCard + 0x7C + 0x22 * 1); }
        }
        public string Saying3
        {
            get { return Util.TrimFromZero(Encoding.Unicode.GetString(Data, TrainerCard + 0x7C + 0x22 * 2, 0x22)); }
            set { Encoding.Unicode.GetBytes(value.PadRight(value.Length + 1, '\0')).CopyTo(Data, TrainerCard + 0x7C + 0x22 * 2); }
        }
        public string Saying4
        {
            get { return Util.TrimFromZero(Encoding.Unicode.GetString(Data, TrainerCard + 0x7C + 0x22 * 3, 0x22)); }
            set { Encoding.Unicode.GetBytes(value.PadRight(value.Length + 1, '\0')).CopyTo(Data, TrainerCard + 0x7C + 0x22 * 3); }
        }
        public string Saying5
        {
            get { return Util.TrimFromZero(Encoding.Unicode.GetString(Data, TrainerCard + 0x7C + 0x22 * 4, 0x22)); }
            set { Encoding.Unicode.GetBytes(value.PadRight(value.Length + 1, '\0')).CopyTo(Data, TrainerCard + 0x7C + 0x22 * 4); }
        }

        public int M
        {
            get { return BitConverter.ToUInt16(Data, Trainer1 + 0x02); }
            set { BitConverter.GetBytes((ushort)value).CopyTo(Data, Trainer1 + 0x02); }
        }
        public float X
        {
            get { return BitConverter.ToSingle(Data, Trainer1 + 0x10) / 18; }
            set { BitConverter.GetBytes(value * 18).CopyTo(Data, Trainer1 + 0x10); }
        }
        public float Z
        {
            get { return BitConverter.ToSingle(Data, Trainer1 + 0x14); }
            set { BitConverter.GetBytes(value).CopyTo(Data, Trainer1 + 0x14); }
        }
        public float Y
        {
            get { return BitConverter.ToSingle(Data, Trainer1 + 0x18) / 18; }
            set { BitConverter.GetBytes(value * 18).CopyTo(Data, Trainer1 + 0x18); }
        }
        public int Style
        {
            get { return Data[Trainer1 + 0x14D]; }
            set { Data[Trainer1 + 0x14D] = (byte)value; }
        }
        public override uint Money
        {
            get { return BitConverter.ToUInt32(Data, Trainer2 + 0x8); }
            set { BitConverter.GetBytes(value).CopyTo(Data, Trainer2 + 0x8); }
        }
        public int Badges
        {
            get { return Data[Trainer2 + 0xC]; }
            set { Data[Trainer2 + 0xC] = (byte)value; }
        }
        public int BP
        {
            get
            {
                int offset = Trainer2 + 0x3C;
                if (ORAS) offset -= 0xC; // 0x30
                return BitConverter.ToUInt16(Data, offset);
            }
            set
            {
                int offset = Trainer2 + 0x3C;
                if (ORAS) offset -= 0xC; // 0x30
                BitConverter.GetBytes((ushort)value).CopyTo(Data, offset);
            }
        }
        public int Vivillon
        {
            get
            {
                int offset = Trainer2 + 0x50;
                if (ORAS) offset -= 0xC; // 0x44
                return Data[offset];
            }
            set
            {
                int offset = Trainer2 + 0x50;
                if (ORAS) offset -= 0xC; // 0x44
                Data[offset] = (byte)value;
            }
        }

        public override int PlayedHours
        { 
            get { return BitConverter.ToUInt16(Data, PlayTime); } 
            set { BitConverter.GetBytes((ushort)value).CopyTo(Data, PlayTime); } 
        }
        public override int PlayedMinutes
        {
            get { return Data[PlayTime + 2]; }
            set { Data[PlayTime + 2] = (byte)value; } 
        }
        public override int PlayedSeconds
        {
            get { return Data[PlayTime + 3]; }
            set { Data[PlayTime + 3] = (byte)value; }
        }
        public uint LastSaved { get { return BitConverter.ToUInt32(Data, PlayTime + 0x4); } set { BitConverter.GetBytes(value).CopyTo(Data, PlayTime + 0x4); } }
        public int LastSavedYear { get { return (int)(LastSaved & 0xFFF); } set { LastSaved = LastSaved & 0xFFFFF000 | (uint)value; } }
        public int LastSavedMonth { get { return (int)(LastSaved >> 12 & 0xF); } set { LastSaved = LastSaved & 0xFFFF0FFF | ((uint)value & 0xF) << 12; } }
        public int LastSavedDay { get { return (int)(LastSaved >> 16 & 0x1F); } set { LastSaved = LastSaved & 0xFFE0FFFF | ((uint)value & 0x1F) << 16; } }
        public int LastSavedHour { get { return (int)(LastSaved >> 21 & 0x1F); } set { LastSaved = LastSaved & 0xFC1FFFFF | ((uint)value & 0x1F) << 21; } }
        public int LastSavedMinute { get { return (int)(LastSaved >> 26 & 0x3F); } set { LastSaved = LastSaved & 0x03FFFFFF | ((uint)value & 0x3F) << 26; } }
        public string LastSavedTime => $"{LastSavedYear.ToString("0000")}{LastSavedMonth.ToString("00")}{LastSavedDay.ToString("00")}{LastSavedHour.ToString("00")}{LastSavedMinute.ToString("00")}";

        public int ResumeYear { get { return BitConverter.ToInt32(Data, AdventureInfo + 0x4); } set { BitConverter.GetBytes(value).CopyTo(Data,AdventureInfo + 0x4); } }
        public int ResumeMonth { get { return Data[AdventureInfo + 0x8]; } set { Data[AdventureInfo + 0x8] = (byte)value; } }
        public int ResumeDay { get { return Data[AdventureInfo + 0x9]; } set { Data[AdventureInfo + 0x9] = (byte)value; } }
        public int ResumeHour { get { return Data[AdventureInfo + 0xB]; } set { Data[AdventureInfo + 0xB] = (byte)value; } }
        public int ResumeMinute { get { return Data[AdventureInfo + 0xC]; } set { Data[AdventureInfo + 0xC] = (byte)value; } }
        public int ResumeSeconds { get { return Data[AdventureInfo + 0xD]; } set { Data[AdventureInfo + 0xD] = (byte)value; } }
        public override int SecondsToStart { get { return BitConverter.ToInt32(Data, AdventureInfo + 0x18); } set { BitConverter.GetBytes(value).CopyTo(Data, AdventureInfo + 0x18); } }
        public override int SecondsToFame { get { return BitConverter.ToInt32(Data, AdventureInfo + 0x20); } set { BitConverter.GetBytes(value).CopyTo(Data, AdventureInfo + 0x20); } }

        public uint getPSSStat(int index) { return BitConverter.ToUInt32(Data, PSSStats + 4*index); }
        public void setPSSStat(int index, uint value) { BitConverter.GetBytes(value).CopyTo(Data, PSSStats + 4*index); }
        public ushort getMaisonStat(int index) { return BitConverter.ToUInt16(Data, MaisonStats + 2 * index); }
        public void setMaisonStat(int index, ushort value) { BitConverter.GetBytes(value).CopyTo(Data, MaisonStats + 2*index); }
        public uint getEncounterCount(int index) { return BitConverter.ToUInt16(Data, EncounterCount + 2*index); }
        public void setEncounterCount(int index, ushort value) { BitConverter.GetBytes(value).CopyTo(Data, EncounterCount + 2*index); }
        
        // Daycare
        public override int DaycareSeedSize => 16;
        public override bool HasTwoDaycares => ORAS;
        public override int getDaycareSlotOffset(int loc, int slot)
        {
            int ofs = loc == 0 ? Daycare : Daycare2;
            if (ofs < 0)
                return -1;
            return ofs + 8 + slot*(SIZE_STORED + 8);
        }
        public override uint? getDaycareEXP(int loc, int slot)
        {
            int ofs = loc == 0 ? Daycare : Daycare2;
            if (ofs > -1)
                return BitConverter.ToUInt32(Data, ofs + (SIZE_STORED + 8)*slot + 4);
            return null;
        }
        public override bool? getDaycareOccupied(int loc, int slot)
        {
            int ofs = loc == 0 ? Daycare : Daycare2;
            if (ofs > -1)
                return Data[ofs + (SIZE_STORED + 8) * slot] == 1;
            return null;
        }
        public override string getDaycareRNGSeed(int loc)
        {
            int ofs = loc == 0 ? Daycare : Daycare2;
            if (ofs <= 0)
                return null;
            
            var data = Data.Skip(Daycare + 0x1E8).Take(DaycareSeedSize/2).Reverse().ToArray();
            return BitConverter.ToString(data).Replace("-", "");
        }
        public override bool? getDaycareHasEgg(int loc)
        {
            int ofs = loc == 0 ? Daycare : Daycare2;
            if (ofs > -1)
                return Data[ofs + 0x1E0] == 1;
            return null;
        }
        public override void setDaycareEXP(int loc, int slot, uint EXP)
        {
            int ofs = loc == 0 ? Daycare : Daycare2;
            if (ofs > -1)
                BitConverter.GetBytes(EXP).CopyTo(Data, ofs + (SIZE_STORED + 8)*slot + 4);
        }
        public override void setDaycareOccupied(int loc, int slot, bool occupied)
        {
            int ofs = loc == 0 ? Daycare : Daycare2;
            if (ofs > -1)
                Data[ofs + (SIZE_STORED + 8)*slot] = (byte) (occupied ? 1 : 0);
        }
        public override void setDaycareRNGSeed(int loc, string seed)
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
                 .Select(x => Convert.ToByte(seed.Substring(x, 2), 16))
                 .Reverse().ToArray().CopyTo(Data, Daycare + 0x1E8);
        }
        public override void setDaycareHasEgg(int loc, bool hasEgg)
        {
            int ofs = loc == 0 ? Daycare : Daycare2;
            if (ofs > -1)
                Data[ofs + 0x1E0] = (byte)(hasEgg ? 1 : 0);
        }

        public byte[] Puffs { get { return Data.Skip(Puff).Take(100).ToArray(); } set { value.CopyTo(Data, Puff); } }
        public int PuffCount { get { return BitConverter.ToInt32(Data, Puff + 100); } set { BitConverter.GetBytes(value).CopyTo(Data, Puff + 100); } }

        public int[] SelectItems
        {
            // UP,RIGHT,DOWN,LEFT
            get
            {
                int[] list = new int[4];
                for (int i = 0; i < list.Length; i++)
                    list[i] = BitConverter.ToUInt16(Data, ItemInfo + 10 + 2 * i);
                return list;
            }
            set
            {
                if (value == null || value.Length > 4)
                    return;
                for (int i = 0; i < value.Length; i++)
                    BitConverter.GetBytes((ushort)value[i]).CopyTo(Data, ItemInfo + 10 + 2 * i);
            }
        }
        public int[] RecentItems
        {
            // Items recently interacted with (Give, Use)
            get
            {
                int[] list = new int[12];
                for (int i = 0; i < list.Length; i++)
                    list[i] = BitConverter.ToUInt16(Data, ItemInfo + 20 + 2 * i);
                return list;
            }
            set
            {
                if (value == null || value.Length > 12)
                    return;
                for (int i = 0; i < value.Length; i++)
                    BitConverter.GetBytes((ushort)value[i]).CopyTo(Data, ItemInfo + 20 + 2 * i);
            }
        }

        public override string JPEGTitle => JPEG < 0 ? null : Util.TrimFromZero(Encoding.Unicode.GetString(Data, JPEG, 0x1A));
        public override byte[] JPEGData => JPEG < 0 || Data[JPEG + 0x54] != 0xFF ? null : Data.Skip(JPEG + 0x54).Take(0xE004).ToArray();

        // Inventory
        public override InventoryPouch[] Inventory
        {
            get
            {
                ushort[] legalItems = ORAS ? Legal.Pouch_Items_AO : Legal.Pouch_Items_XY;
                ushort[] legalKey = ORAS ? Legal.Pouch_Key_AO : Legal.Pouch_Key_XY;
                ushort[] legalTMHM = ORAS ? Legal.Pouch_TMHM_AO : Legal.Pouch_TMHM_XY;
                ushort[] legalMedicine = ORAS ? Legal.Pouch_Medicine_AO : Legal.Pouch_Medicine_XY;
                InventoryPouch[] pouch =
                {
                    new InventoryPouch(InventoryType.Items, legalItems, 995, OFS_PouchHeldItem),
                    new InventoryPouch(InventoryType.KeyItems, legalKey, 1, OFS_PouchKeyItem),
                    new InventoryPouch(InventoryType.TMHMs, legalTMHM, 1, OFS_PouchTMHM),
                    new InventoryPouch(InventoryType.Medicine, legalMedicine, 995, OFS_PouchMedicine),
                    new InventoryPouch(InventoryType.Berries, Legal.Pouch_Berry_XY, 995, OFS_PouchBerry),
                };
                foreach (var p in pouch)
                    p.getPouch(ref Data);
                return pouch;
            }
            set
            {
                foreach (var p in value)
                    p.setPouch(ref Data);
            }
        }

        // Storage
        public override int CurrentBox { get { return Data[LastViewedBox]; } set { Data[LastViewedBox] = (byte)value; } }
        public override int getPartyOffset(int slot)
        {
            return Party + SIZE_PARTY * slot;
        }
        public override int getBoxOffset(int box)
        {
            return Box + SIZE_STORED*box*30;
        }
        protected override int getBoxWallpaperOffset(int box)
        {
            int ofs = PCBackgrounds > 0 && PCBackgrounds < Data.Length ? PCBackgrounds : -1;
            if (ofs > -1)
                return ofs + box;
            return ofs;
        }
        public override string getBoxName(int box)
        {
            if (PCLayout < 0)
                return "B" + (box + 1);
            return Util.TrimFromZero(Encoding.Unicode.GetString(Data, PCLayout + 0x22*box, 0x22));
        }
        public override void setBoxName(int box, string val)
        {
            Encoding.Unicode.GetBytes(val.PadRight(0x11, '\0')).CopyTo(Data, PCLayout + 0x22*box);
            Edited = true;
        }
        public override PKM getPKM(byte[] data)
        {
            return new PK6(data);
        }
        protected override void setPKM(PKM pkm)
        {
            PK6 pk6 = pkm as PK6;
            // Apply to this Save File
            int CT = pk6.CurrentHandler;
            DateTime Date = DateTime.Now;
            pk6.Trade(OT, TID, SID, Country, SubRegion, Gender, false, Date.Day, Date.Month, Date.Year);
            if (CT != pk6.CurrentHandler) // Logic updated Friendship
            {
                // Copy over the Friendship Value only under certain circumstances
                if (pk6.Moves.Contains(216)) // Return
                    pk6.CurrentFriendship = pk6.OppositeFriendship;
                else if (pk6.Moves.Contains(218)) // Frustration
                    pkm.CurrentFriendship = pk6.OppositeFriendship;
                else if (pk6.CurrentHandler == 1) // OT->HT, needs new Friendship/Affection
                    pk6.TradeFriendshipAffection(OT);
            }
            pkm.RefreshChecksum();
        }
        protected override void setDex(PKM pkm)
        {
            if (PokeDex < 0)
                return;
            if (pkm.Species == 0)
                return;
            if (pkm.Species > MaxSpeciesID)
                return;
            if (Version == GameVersion.Unknown)
                return;

            const int brSize = 0x60;
            int bit = pkm.Species - 1;
            int lang = pkm.Language - 1; if (lang > 5) lang--; // 0-6 language vals
            int origin = pkm.Version;
            int gender = pkm.Gender % 2; // genderless -> male
            int shiny = pkm.IsShiny ? 1 : 0;
            int shiftoff = shiny * brSize * 2 + gender * brSize + brSize;

            // Set the [Species/Gender/Shiny] Owned Flag
            Data[PokeDex + shiftoff + bit / 8 + 0x8] |= (byte)(1 << (bit % 8));

            // Owned quality flag
            if (origin < 0x18 && bit < 649 && !ORAS) // Species: 1-649 for X/Y, and not for ORAS; Set the Foreign Owned Flag
                Data[PokeDex + 0x64C + bit / 8] |= (byte)(1 << (bit % 8));
            else if (origin >= 0x18 || ORAS) // Set Native Owned Flag (should always happen)
                Data[PokeDex + bit / 8 + 0x8] |= (byte)(1 << (bit % 8));

            // Set the Display flag if none are set
            bool Displayed = false;
            Displayed |= (Data[PokeDex + brSize * 5 + bit / 8 + 0x8] & (byte)(1 << (bit % 8))) != 0;
            Displayed |= (Data[PokeDex + brSize * 6 + bit / 8 + 0x8] & (byte)(1 << (bit % 8))) != 0;
            Displayed |= (Data[PokeDex + brSize * 7 + bit / 8 + 0x8] & (byte)(1 << (bit % 8))) != 0;
            Displayed |= (Data[PokeDex + brSize * 8 + bit / 8 + 0x8] & (byte)(1 << (bit % 8))) != 0;
            if (!Displayed) // offset is already biased by brSize, reuse shiftoff but for the display flags.
                Data[PokeDex + shiftoff + brSize * 4 + bit / 8 + 0x8] |= (byte)(1 << (bit % 8));

            // Set the Language
            if (lang < 0) lang = 1;
            Data[PokeDexLanguageFlags + (bit * 7 + lang) / 8] |= (byte)(1 << ((bit * 7 + lang) % 8));

            // Set DexNav count (only if not encountered previously)
            if (ORAS && getEncounterCount(pkm.Species - 1) == 0)
                setEncounterCount(pkm.Species - 1, 1);

            // Set Form flags
            int fc = Personal[pkm.Species].FormeCount;
            int f = ORAS ? SaveUtil.getDexFormIndexORAS(pkm.Species, fc) : SaveUtil.getDexFormIndexXY(pkm.Species, fc);
            if (f < 0) return;

            int FormLen = ORAS ? 0x26 : 0x18;
            int FormDex = PokeDex + 0x8 + brSize*9;
            bit = f + pkm.AltForm;

            // Set Form Seen Flag
            Data[FormDex + FormLen*shiny + bit/8] |= (byte)(1 << (bit%8));

            // Set Displayed Flag if necessary, check all flags
            for (int i = 0; i < fc; i++)
            {
                bit = f + i;
                if ((Data[FormDex + FormLen*2 + bit/8] & (byte) (1 << (bit%8))) != 0) // Nonshiny
                    return; // already set
                if ((Data[FormDex + FormLen*3 + bit/8] & (byte) (1 << (bit%8))) != 0) // Shiny
                    return; // already set
            }
            bit = f + pkm.AltForm;
            Data[FormDex + FormLen * (2 + shiny) + bit / 8] |= (byte)(1 << (bit % 8));
        }
        public override byte[] decryptPKM(byte[] data)
        {
            return PKX.decryptArray(data);
        }
        public override int PartyCount
        {
            get { return Data[Party + 6 * SIZE_PARTY]; }
            protected set { Data[Party + 6 * SIZE_PARTY] = (byte)value; }
        }
        public override bool BattleBoxLocked
        {
            get { return Data[BattleBox + 6 * SIZE_STORED] != 0; }
            set { Data[BattleBox + 6 * SIZE_STORED] = (byte)(value ? 1 : 0); }
        }
        public override int BoxesUnlocked { get { return Data[PCFlags + 1] - 1; } set { Data[PCFlags + 1] = (byte)(value + 1); } }
        public override byte[] BoxFlags
        {
            get { return new[] { Data[PCFlags], Data[PCFlags + 2] }; }
            set
            {
                if (value.Length != 2) return;
                Data[PCFlags] = value[0];
                Data[PCFlags + 2] = value[1];
            }
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
                    cards[i] = getWC6(i);

                return cards;
            }
            set
            {
                if (value == null)
                    return;
                if (value.Length > GiftCountMax)
                    Array.Resize(ref value, GiftCountMax);
                
                for (int i = 0; i < value.Length; i++)
                    setWC6(value[i], i);
                for (int i = value.Length; i < GiftCountMax; i++)
                    setWC6(new WC6(), i);
            }
        }

        public byte[] LinkBlock
        {
            get
            {
                if (LinkInfo < 0)
                    return null;
                return Data.Skip(LinkInfo).Take(0xC48).ToArray();
            }
            set
            {
                if (LinkInfo < 0)
                    return;
                if (value.Length != 0xC48)
                    return;
                value.CopyTo(Data, LinkInfo);
            }
        }

        private MysteryGift getWC6(int index)
        {
            if (WondercardData < 0)
                return null;
            if (index < 0 || index > GiftCountMax)
                return null;

            return new WC6(Data.Skip(WondercardData + index * WC6.Size).Take(WC6.Size).ToArray());
        }
        private void setWC6(MysteryGift wc6, int index)
        {
            if (WondercardData < 0)
                return;
            if (index < 0 || index > GiftCountMax)
                return;

            wc6.Data.CopyTo(Data, WondercardData + index * WC6.Size);

            for (int i = 0; i < GiftCountMax; i++)
                if (BitConverter.ToUInt16(Data, WondercardData + i * WC6.Size) == 0)
                    for (int j = i + 1; j < GiftCountMax - i; j++) // Shift everything down
                        Array.Copy(Data, WondercardData + j * WC6.Size, Data, WondercardData + (j - 1) * WC6.Size, WC6.Size);

            Edited = true;
        }

        // Writeback Validity
        public override string MiscSaveChecks()
        {
            string r = "";
            byte[] FFFF = Enumerable.Repeat((byte)0xFF, 0x200).ToArray();
            for (int i = 0; i < Data.Length / 0x200; i++)
            {
                if (!FFFF.SequenceEqual(Data.Skip(i * 0x200).Take(0x200))) continue;
                r = $"0x200 chunk @ 0x{(i*0x200).ToString("X5")} is FF'd."
                    + Environment.NewLine + "Cyber will screw up (as of August 31st 2014)." + Environment.NewLine + Environment.NewLine;

                // Check to see if it is in the Pokedex
                if (i * 0x200 > PokeDex && i * 0x200 < PokeDex + 0x900)
                {
                    r += "Problem lies in the Pokedex. ";
                    if (i * 0x200 == PokeDex + 0x400)
                        r += "Remove a language flag for a species < 585, ie Petilil";
                }
                break;
            }
            return r;
        }
        public override string MiscSaveInfo()
        {
            return Blocks.Aggregate("", (current, b) => current +
                $"{b.ID.ToString("00")}: {b.Offset.ToString("X5")}-{(b.Offset + b.Length).ToString("X5")}, {b.Length.ToString("X5")}{Environment.NewLine}");
        }
    }
}
