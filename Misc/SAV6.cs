using System;
using System.Linq;
using System.Text;

namespace PKHeX
{
    public class BlockInfo
    {
        public int Offset;
        public int Length;
        public ushort ID;
        public ushort Checksum;
    }
    public enum GameVersion
    {
        X = 24, Y = 25,
        AS = 26, OR = 27,
        SN = 28, MN = 29,
        Unknown = 0,
        Any = -1,
    }
    public class SAV6 : PKX
    {
        internal const int SIZE_XY = 0x65600;
        internal const int SIZE_ORAS = 0x76000;
        internal const int SIZE_ORASDEMO = 0x5A00;
        internal const int BEEF = 0x42454546;
        internal static bool SizeValid(int size)
        {
            return new[] {SIZE_XY, SIZE_ORAS, SIZE_ORASDEMO}.Contains(size);
        }

        // Global Settings
        internal static bool SetUpdateDex = true;
        internal static bool SetUpdatePK6 = true;
        // Save Data Attributes
        public byte[] Data;
        public bool Edited;
        public readonly bool Exportable;
        public readonly byte[] BAK;
        public string FileName, FilePath;
        public string BAKName => $"{FileName} [{OT} ({Version}) - {LastSavedTime}].bak";
        public SAV6(byte[] data = null)
        {
            Data = (byte[])(data ?? new byte[SIZE_ORAS]).Clone();
            BAK = (byte[])Data.Clone();
            Exportable = !Data.SequenceEqual(new byte[Data.Length]);

            // Load Info
            getBlockInfo();
            getSAVOffsets();
        }
        
        public void getSAVOffsets()
        {
            if (ORASDEMO)
            {
                /* 00: */   Item = 0x00000; Items = new Inventory(Item, 1);
                /* 01: */   // = 0x00C00; // Select Bound Items
                /* 02: */   AdventureInfo = 0x00E00;
                /* 03: */   Trainer1 = 0x01000;
                /* 04: */   // = 0x01200; [00004] // ???
                /* 05: */   PlayTime = 0x01400;
                /* 06: */   // = 0x01600; [00024] // FFFFFFFF
                /* 07: */   // = 0x01800; [02100] // Overworld Data
                /* 08: */   Trainer2 = 0x03A00;
                /* 09: */   TrainerCard = 0x03C00;
                /* 10: */   Party = 0x03E00;
                /* 11: */   EventConst = 0x04600; EventAsh = EventConst + 0x78; EventFlag = EventConst + 0x2FC;
                /* 12: */   // = 0x04C00; [00004] // 87B1A23F const
                /* 13: */   // = 0x04E00; [00048] // Repel Info, (Swarm?) and other overworld info
                /* 14: */   SUBE = 0x05000;
                /* 15: */   PSSStats = 0x05400;

                Box = BattleBox = GTS = Daycare = EonTicket = Fused = Puff = 
                    SuperTrain = SecretBase = BoxWallpapers = 
                    LastViewedBox = PCLayout = PCBackgrounds = PCFlags = WondercardFlags = WondercardData = 
                    BerryField = OPower = Accessories = 
                    PokeDex = PokeDexLanguageFlags = Spinda = EncounterCount = HoF = PSS = JPEG = Contest -1;
                MaisonStats = -1;
            }
            else if (XY)
            {
                Box = 0x22600;
                TrainerCard = 0x14000;
                Party = 0x14200;
                BattleBox = 0x04A00;
                Daycare = 0x1B200;
                GTS = 0x17800;
                Fused = 0x16000;
                SUBE = 0x1D890;
                Puff = 0x00000;
                Item = 0x00400;
                Items = new Inventory(Item, 0);
                AdventureInfo = 0x01200;
                Trainer1 = 0x1400;
                Trainer2 = 0x4200;
                PCLayout = 0x4400;
                PCBackgrounds = PCLayout + 0x41E;
                PCFlags = PCLayout + 0x43D;
                WondercardFlags = 0x1BC00;
                WondercardData = WondercardFlags + 0x100;
                BerryField = 0x1B800;
                OPower = 0x16A00;
                EventConst = 0x14A00;
                EventAsh = -1;
                EventFlag = EventConst + 0x2FC;
                PokeDex = 0x15000;
                PokeDexLanguageFlags = PokeDex + 0x3C8;
                Spinda = PokeDex + 0x648;
                EncounterCount = -1;
                HoF = 0x19400;
                SuperTrain = 0x1F200;
                JPEG = 0x57200;
                MaisonStats = 0x1B1C0;
                PSS = 0x05000;
                PSSStats = 0x1E400;
                BoxWallpapers = 0x481E;
                SecretBase = -1;
                EonTicket = -1;
                Contest = -1;
                PlayTime = 0x1800;
                Accessories = 0x1A00;
                LastViewedBox = PCLayout + 0x43F;
            }
            else if (ORAS)
            {
                Box = 0x33000; // Confirmed
                TrainerCard = 0x14000; // Confirmed
                Party = 0x14200; // Confirmed
                BattleBox = 0x04A00; // Confirmed
                Daycare = 0x1BC00; // Confirmed (thanks Rei)
                GTS = 0x18200; // Confirmed
                Fused = 0x16A00; // Confirmed
                SUBE = 0x1D890; // ****not in use, not updating?****
                Puff = 0x00000; // Confirmed
                Item = 0x00400; // Confirmed
                Items = new Inventory(Item, 1);
                AdventureInfo = 0x01200;
                Trainer1 = 0x01400; // Confirmed
                Trainer2 = 0x04200; // Confirmed
                PCLayout = 0x04400; // Confirmed
                PCBackgrounds = PCLayout + 0x41E;
                PCFlags = PCLayout + 0x43D;
                WondercardFlags = 0x1CC00; // Confirmed
                WondercardData = WondercardFlags + 0x100;
                BerryField = 0x1C400; // ****changed****
                OPower = 0x17400; // ****changed****
                EventConst = 0x14A00;
                EventAsh = EventConst + 0x78;
                EventFlag = EventConst + 0x2FC;
                PokeDex = 0x15000;
                Spinda = PokeDex + 0x680;
                EncounterCount = PokeDex + 0x686;
                PokeDexLanguageFlags = PokeDex + 0x400;
                HoF = 0x19E00; // Confirmed
                SuperTrain = 0x20200;
                Contest = 0x23600; // Confirmed
                JPEG = 0x67C00; // Confirmed
                MaisonStats = 0x1BBC0;
                PSS = 0x05000; // Confirmed (thanks Rei)
                PSSStats = 0x1F400;
                BoxWallpapers = 0x481E;
                SecretBase = 0x23A00;
                EonTicket = 0x319B8;
                PlayTime = 0x1800;
                Accessories = 0x1A00;
                LastViewedBox = PCLayout + 0x43F;
            }
            DaycareSlot = new[] { Daycare, Daycare + 0x1F0 };
        }
        public class Inventory
        {
            public readonly int HeldItem, KeyItem, Medicine, TMHM, Berry;
            public Inventory(int Offset, int Game)
            {
                switch (Game)
                {
                    case 0:
                        HeldItem = Offset + 0;
                        KeyItem = Offset + 0x640;
                        TMHM = Offset + 0x7C0;
                        Medicine = Offset + 0x968;
                        Berry = Offset + 0xA68;
                        break;
                    case 1: 
                        HeldItem = Offset + 0;
                        KeyItem = Offset + 0x640;
                        TMHM = Offset + 0x7C0;
                        Medicine = Offset + 0x970;
                        Berry = Offset + 0xA70;
                        break;
                }
            }
        }
        public Inventory Items = new Inventory(0, -1);
        public int BattleBox, GTS, Daycare, EonTicket,
            Fused, SUBE, Puff, Item, AdventureInfo, Trainer1, Trainer2, SuperTrain, PSSStats, MaisonStats, SecretBase, BoxWallpapers, LastViewedBox,
            PCLayout, PCBackgrounds, PCFlags, WondercardFlags, WondercardData, BerryField, OPower, EventConst, EventFlag, EventAsh, PlayTime, Accessories,
            PokeDex, PokeDexLanguageFlags, Spinda, EncounterCount, HoF, PSS, JPEG, Contest;
        public int TrainerCard = 0x14000;
        public int Box = 0x33000, Party = 0x14200;
        public int[] DaycareSlot;
        public int DaycareIndex = 0;

        public GameVersion Version
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
        public bool ORASDEMO => Data.Length == SIZE_ORASDEMO;
        public bool ORAS => Version == GameVersion.OR || Version == GameVersion.AS;
        public bool XY => Version == GameVersion.X || Version == GameVersion.Y;

        // Save Information
        private int BlockInfoOffset;
        private BlockInfo[] Blocks;
        private void getBlockInfo()
        {
            BlockInfoOffset = Data.Length - 0x200 + 0x10;
            if (BitConverter.ToUInt32(Data, BlockInfoOffset) != BEEF)
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
                    Length = BitConverter.ToInt32(Data, BlockInfoOffset + 0 + 8*i),
                    ID = BitConverter.ToUInt16(Data, BlockInfoOffset + 4 + 8*i),
                    Checksum = BitConverter.ToUInt16(Data, BlockInfoOffset + 6 + 8*i)
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
        private void setChecksums()
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
                byte[] array = Data.Skip(Blocks[i].Offset).Take(Blocks[i].Length).ToArray();
                BitConverter.GetBytes(ccitt16(array)).CopyTo(Data, BlockInfoOffset + 6 + i * 8);
            }
        }
        public bool ChecksumsValid => verifyG6SAV(Data);
        public string ChecksumInfo => verifyG6CHK(Data);
        public byte[] Write()
        {
            setChecksums();
            return Data;
        }
        public ulong Secure1
        {
            get { return BitConverter.ToUInt64(Data, BlockInfoOffset - 0x14); }
            set { BitConverter.GetBytes(value).CopyTo(Data, BlockInfoOffset - 0x14); }
        }
        public ulong Secure2
        {
            get { return BitConverter.ToUInt64(Data, BlockInfoOffset - 0xC); }
            set { BitConverter.GetBytes(value).CopyTo(Data, BlockInfoOffset - 0xC); }
        }
        public int CurrentBox { get { return Data[LastViewedBox]; } set { Data[LastViewedBox] = (byte)value; } }

        // Player Information
        public ushort TID
        {
            get { return BitConverter.ToUInt16(Data, TrainerCard + 0); }
            set { BitConverter.GetBytes(value).CopyTo(Data, TrainerCard + 0); }
        }
        public ushort SID
        {
            get { return BitConverter.ToUInt16(Data, TrainerCard + 2); }
            set { BitConverter.GetBytes(value).CopyTo(Data, TrainerCard + 2); }
        }
        public int Game
        {
            get { return Data[TrainerCard + 4]; }
            set { Data[TrainerCard + 4] = (byte)value; }
        }
        public int Gender
        {
            get { return Data[TrainerCard + 5]; }
            set { Data[TrainerCard + 5] = (byte)value; }
        }
        public int Sprite
        {
            get { return Data[TrainerCard + 7]; }
            set { Data[TrainerCard + 7] = (byte)value; }
        }
        public ulong GameSyncID
        {
            get { return BitConverter.ToUInt64(Data, TrainerCard + 8); }
            set { BitConverter.GetBytes(value).CopyTo(Data, TrainerCard + 8); }
        }
        public int SubRegion
        {
            get { return Data[TrainerCard + 0x26]; }
            set { Data[TrainerCard + 0x26] = (byte)value; }
        }
        public int Country
        {
            get { return Data[TrainerCard + 0x27]; }
            set { Data[TrainerCard + 0x27] = (byte)value; }
        }
        public int ConsoleRegion
        {
            get { return Data[TrainerCard + 0x2C]; }
            set { Data[TrainerCard + 0x2C] = (byte)value; }
        }
        public int Language
        {
            get { return Data[TrainerCard + 0x2D]; }
            set { Data[TrainerCard + 0x2D] = (byte)value; }
        }
        public string OT
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
        public uint Money
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

        public int PlayedHours{ 
            get { return BitConverter.ToUInt16(Data, PlayTime); } 
            set { BitConverter.GetBytes((ushort)value).CopyTo(Data, PlayTime); } 
        }
        public int PlayedMinutes
        {
            get { return Data[PlayTime + 2]; }
            set { Data[PlayTime + 2] = (byte)value; } 
        }
        public int PlayedSeconds
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
        public int SecondsToStart { get { return BitConverter.ToInt32(Data, AdventureInfo + 0x18); } set { BitConverter.GetBytes(value).CopyTo(Data, AdventureInfo + 0x18); } }
        public int SecondsToFame { get { return BitConverter.ToInt32(Data, AdventureInfo + 0x20); } set { BitConverter.GetBytes(value).CopyTo(Data, AdventureInfo + 0x20); } }

        public uint getPSSStat(int index) { return BitConverter.ToUInt32(Data, PSSStats + 4*index); }
        public void setPSSStat(int index, uint value) { BitConverter.GetBytes(value).CopyTo(Data, PSSStats + 4*index); }
        public ushort getMaisonStat(int index) { return BitConverter.ToUInt16(Data, MaisonStats + 2 * index); }
        public void setMaisonStat(int index, ushort value) { BitConverter.GetBytes(value).CopyTo(Data, MaisonStats + 2*index); }
        public uint getEncounterCount(int index) { return BitConverter.ToUInt16(Data, EncounterCount + 2*index); }
        public void setEncounterCount(int index, ushort value) { BitConverter.GetBytes(value).CopyTo(Data, EncounterCount + 2*index); }

        // Misc Properties
        public int PartyCount
        {
            get { return Data[Party + 6 * PK6.SIZE_PARTY]; }
            set { Data[Party + 6 * PK6.SIZE_PARTY] = (byte)value; }
        }
        public bool BattleBoxLocked
        {
            get { return Data[BattleBox + 6 * PK6.SIZE_STORED] != 0; }
            set { Data[BattleBox + 6 * PK6.SIZE_STORED] = (byte)(value ? 1 : 0); }
        }
        public ulong DaycareRNGSeed
        {
            get { return BitConverter.ToUInt64(Data, DaycareSlot[DaycareIndex] + 0x1E8); }
            set { BitConverter.GetBytes(value).CopyTo(Data, DaycareSlot[DaycareIndex] + 0x1E8); }
        }
        public bool DaycareHasEgg
        {
            get { return Data[DaycareSlot[DaycareIndex] + 0x1E0] == 1; }
            set { Data[DaycareSlot[DaycareIndex] + 0x1E0] = (byte)(value ? 1 : 0); }
        }
        public uint DaycareEXP1
        {
            get { return BitConverter.ToUInt32(Data, DaycareSlot[DaycareIndex] + (PK6.SIZE_STORED + 8)*0 + 4); }
            set { BitConverter.GetBytes(value).CopyTo(Data, DaycareSlot[DaycareIndex] + (PK6.SIZE_STORED + 8)*0 + 4); }
        }
        public uint DaycareEXP2
        {
            get { return BitConverter.ToUInt32(Data, DaycareSlot[DaycareIndex] + (PK6.SIZE_STORED + 8)*1 + 4); }
            set { BitConverter.GetBytes(value).CopyTo(Data, DaycareSlot[DaycareIndex] + (PK6.SIZE_STORED + 8)*1 + 4); }
        }
        public bool DaycareOccupied1
        {
            get { return Data[DaycareSlot[DaycareIndex] + (PK6.SIZE_STORED + 8)*0] != 0; }
            set { Data[DaycareSlot[DaycareIndex] + (PK6.SIZE_STORED + 8)*0] = (byte)(value ? 1 : 0); }
        }
        public bool DaycareOccupied2
        {
            get { return Data[DaycareSlot[DaycareIndex] + (PK6.SIZE_STORED + 8) * 1] != 0; }
            set { Data[DaycareSlot[DaycareIndex] + (PK6.SIZE_STORED + 8) * 1] = (byte)(value ? 1 : 0); }
        }

        public byte[] Puffs { get { return Data.Skip(Puff).Take(100).ToArray(); } set { value.CopyTo(Data, Puff); } }
        public int PuffCount { get { return BitConverter.ToInt32(Data, Puff + 100); } set { BitConverter.GetBytes(value).CopyTo(Data, Puff + 100); } }
        
        public string JPEGTitle => JPEG > -1 ? null : Util.TrimFromZero(Encoding.Unicode.GetString(Data, JPEG, 0x1A));
        public byte[] JPEGData => JPEG > -1 || Data[JPEG + 0x54] != 0xFF ? null : Data.Skip(JPEG + 0x54).Take(0xE004).ToArray();

        // Data Accessing
        public byte[] getData(int Offset, int Length)
        {
            return Data.Skip(Offset).Take(Length).ToArray();
        }
        public void setData(byte[] input, int Offset)
        {
            input.CopyTo(Data, Offset);
            Edited = true;
        }

        // Pokémon Requests
        public PK6 getPK6Party(int offset)
        {
            return new PK6(decryptArray(getData(offset , PK6.SIZE_PARTY)));
        }
        public PK6 getPK6Stored(int offset)
        {
            return new PK6(decryptArray(getData(offset, PK6.SIZE_STORED)));
        }
        public void setPK6Party(PK6 pk6, int offset, bool? trade = null, bool? dex = null)
        {
            if (pk6 == null) return;
            if (trade ?? SetUpdatePK6)
                setPK6(pk6);
            if (dex ?? SetUpdateDex)
                setDex(pk6);
            
            setData(pk6.EncryptedPartyData, offset);
            Edited = true;
        }
        public void setPK6Stored(PK6 pk6, int offset, bool? trade = null, bool? dex = null)
        {
            if (pk6 == null) return;
            if (trade ?? SetUpdatePK6)
                setPK6(pk6);
            if (dex ?? SetUpdateDex)
                setDex(pk6);

            setData(pk6.EncryptedBoxData, offset);
            Edited = true;
        }
        public void setEK6Stored(byte[] ek6, int offset, bool? trade = null, bool? dex = null)
        {
            if (ek6 == null) return;
            PK6 pk6 = new PK6(decryptArray(ek6));
            if (trade ?? SetUpdatePK6)
                setPK6(pk6);
            if (dex ?? SetUpdateDex)
                setDex(pk6);
            
            setData(pk6.EncryptedBoxData, offset);
            Edited = true;
        }
        public void setEK6Party(byte[] ek6, int offset, bool? trade = null, bool? dex = null)
        {
            if (ek6 == null) return;
            PK6 pk6 = new PK6(decryptArray(ek6));
            if (trade ?? SetUpdatePK6)
                setPK6(pk6);
            if (dex ?? SetUpdateDex)
                setDex(pk6);
            
            setData(pk6.EncryptedPartyData, offset);
            Edited = true;
        }

        // Meta
        public void setPK6(PK6 pk6)
        {
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
                    pk6.CurrentFriendship = pk6.OppositeFriendship;
                else if (pk6.CurrentHandler == 1) // OT->HT, needs new Friendship/Affection
                    pk6.TradeFriendshipAffection(OT);
            }
            pk6.RefreshChecksum();
        }
        public void setDex(PK6 pk6)
        {
            if (PokeDex < 0) 
                return;
            if (pk6.Species == 0)
                return;
            if (Version == GameVersion.Unknown)
                return;

            int bit = pk6.Species - 1;
            int lang = pk6.Language - 1; if (lang > 5) lang--; // 0-6 language vals
            int origin = pk6.Version;
            int gender = pk6.Gender % 2; // genderless -> male
            int shiny = pk6.IsShiny ? 1 : 0;
            int shiftoff = shiny * 0x60 * 2 + gender * 0x60 + 0x60;

            // Set the [Species/Gender/Shiny] Owned Flag
            Data[PokeDex + shiftoff + bit / 8 + 0x8] |= (byte)(1 << (bit % 8));

            // Owned quality flag
            if (origin < 0x18 && bit < 649 && !ORAS) // Species: 1-649 for X/Y, and not for ORAS; Set the Foreign Owned Flag
                Data[PokeDex + 0x64C + bit / 8] |= (byte)(1 << (bit % 8));
            else if (origin >= 0x18 || ORAS) // Set Native Owned Flag (should always happen)
                Data[PokeDex + bit / 8 + 0x8] |= (byte)(1 << (bit % 8));

            // Set the Display flag if none are set
            bool Displayed = false;
            Displayed |= (Data[PokeDex + 0x60*5 + bit/8 + 0x8] & (byte) (1 << (bit%8))) != 0;
            Displayed |= (Data[PokeDex + 0x60*6 + bit/8 + 0x8] & (byte) (1 << (bit%8))) != 0;
            Displayed |= (Data[PokeDex + 0x60*7 + bit/8 + 0x8] & (byte) (1 << (bit%8))) != 0;
            Displayed |= (Data[PokeDex + 0x60*8 + bit/8 + 0x8] & (byte) (1 << (bit%8))) != 0;
            if (!Displayed) // offset is already biased by 0x60, reuse shiftoff but for the display flags.
                Data[PokeDex + shiftoff + 0x60 * 4 + bit / 8 + 0x8] |= (byte)(1 << (bit % 8));

            // Set the Language
            if (lang < 0) lang = 1;
            Data[PokeDexLanguageFlags + (bit * 7 + lang) / 8] |= (byte)(1 << ((bit * 7 + lang) % 8));

            // Set Form flags
            int fc = Personal[pk6.Species].FormeCount;
            int f = ORAS ? getDexFormIndexORAS(pk6.Species, fc) : getDexFormIndexXY(pk6.Species, fc);
            if (f >= 0)
            {
                int FormLen = ORAS ? 0x26 : 0x18;
                int FormDex = PokeDex + 0x368;
                bit = f + pk6.AltForm;
                // Set Seen Flag
                Data[FormDex + FormLen*shiny + bit/8] |= (byte)(1 << (bit%8));

                // Set Displayed Flag if necessary, check all flags
                bool FormDisplayed = false;
                for (int i = 0; i < fc; i++)
                {
                    bit = f + i;
                    FormDisplayed |= (Data[FormDex + FormLen*2 + bit/8] & (byte)(1 << (bit%8))) != 0; // Nonshiny
                    FormDisplayed |= (Data[FormDex + FormLen*3 + bit/8] & (byte)(1 << (bit%8))) != 0; // Shiny
                }
                if (!FormDisplayed)
                {
                    bit = f + pk6.AltForm;
                    Data[FormDex + FormLen*(2+shiny) + bit/8] |= (byte)(1 << (bit%8));
                }
            }

            // Set DexNav count (only if not encountered previously)
            if (ORAS && getEncounterCount(pk6.Species - 1) == 0)
                setEncounterCount(pk6.Species - 1, 1);
        }
        public int getBoxWallpaper(int box)
        {
            return 1 + Data[BoxWallpapers + box];
        }
        public string getBoxName(int box)
        {
            return Util.TrimFromZero(Encoding.Unicode.GetString(Data, PCLayout + 0x22*box, 0x22));
        }
        public void setBoxName(int box, string val)
        {
            Encoding.Unicode.GetBytes(val.PadRight(0x11, '\0')).CopyTo(Data, PCLayout + 0x22*box);
            Edited = true;
        }
        public void setParty()
        {
            byte partymembers = 0; // start off with a ctr of 0
            for (int i = 0; i < 6; i++)
            {
                // Gather all the species
                byte[] data = new byte[PK6.SIZE_PARTY];
                Array.Copy(Data, Party + i * PK6.SIZE_PARTY, data, 0, PK6.SIZE_PARTY);
                byte[] decdata = decryptArray(data);
                int species = BitConverter.ToInt16(decdata, 8);
                if ((species != 0) && (species < 722))
                    Array.Copy(data, 0, Data, Party + partymembers++ * PK6.SIZE_PARTY, PK6.SIZE_PARTY);
            }

            // Write in the current party count
            PartyCount = partymembers;
            // Zero out the party slots that are empty.
            for (int i = 0; i < 6; i++)
                if (i >= partymembers)
                    Array.Copy(encryptArray(new byte[PK6.SIZE_PARTY]), 0, Data, Party + i * PK6.SIZE_PARTY, PK6.SIZE_PARTY);

            if (BattleBox < 0)
                return;
            // Repeat for Battle Box.
            byte battlemem = 0;
            for (int i = 0; i < 6; i++)
            {
                // Gather all the species
                byte[] data = new byte[PK6.SIZE_PARTY];
                Array.Copy(Data, BattleBox + i * PK6.SIZE_STORED, data, 0, PK6.SIZE_STORED);
                byte[] decdata = decryptArray(data);
                int species = BitConverter.ToInt16(decdata, 8);
                if ((species != 0) && (species < 722))
                    Array.Copy(data, 0, Data, BattleBox + battlemem++ * PK6.SIZE_STORED, PK6.SIZE_STORED);
            }

            // Zero out the party slots that are empty.
            for (int i = 0; i < 6; i++)
                if (i >= battlemem)
                    Array.Copy(encryptArray(new byte[PK6.SIZE_PARTY]), 0, Data, BattleBox + i * PK6.SIZE_STORED, PK6.SIZE_STORED);

            BattleBoxLocked &= battlemem != 0;
        }
        public void sortBoxes()
        {
            const int len = 31 * 30; // amount of pk6's in boxes

            // Fetch encrypted box data
            byte[][] bdata = new byte[len][];
            for (int i = 0; i < len; i++)
                bdata[i] = getData(Box + i * PK6.SIZE_STORED, PK6.SIZE_STORED);

            // Sorting Method: Data will sort empty slots to the end, then from the filled slots eggs will be last, then species will be sorted.
            var query = from i in bdata
                        let p = new PK6(decryptArray(i))
                        orderby
                            p.Species == 0 ascending,
                            p.IsEgg ascending,
                            p.Species ascending,
                            p.IsNicknamed ascending
                        select i;
            byte[][] sorted = query.ToArray();

            // Write data back
            for (int i = 0; i < len; i++)
                setData(sorted[i], Box + i * PK6.SIZE_STORED);
        }

        // Informational
        public PK6[] BoxData
        {
            get
            {
                PK6[] data = new PK6[31*30];
                for (int i = 0; i < data.Length; i++)
                {
                    data[i] = getPK6Stored(Box + PK6.SIZE_STORED * i);
                    data[i].Identifier = $"B{(i/30 + 1).ToString("00")}:{(i%30 + 1).ToString("00")}";
                }
                return data;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException();
                if (value.Length != 31*30)
                    throw new ArgumentException("Expected 930, got " + value.Length);

                for (int i = 0; i < value.Length; i++)
                    setPK6Stored(value[i], Box + PK6.SIZE_STORED * i);
            }
        }
        public PK6[] PartyData
        {
            get
            {
                PK6[] data = new PK6[PartyCount];
                for (int i = 0; i < data.Length; i++)
                    data[i] = getPK6Party(Party + PK6.SIZE_PARTY * i);
                return data;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException();
                if (value.Length == 0 || value.Length > 6)
                    throw new ArgumentException("Expected 1-6, got " + value.Length);
                if (value[0].Species == 0)
                    throw new ArgumentException("Can't have an empty first slot." + value.Length);

                PK6[] newParty = value.Where(pk => pk.Species != 0).ToArray();
                
                PartyCount = newParty.Length;
                Array.Resize(ref newParty, 6);

                for (int i = PartyCount; i < newParty.Length; i++)
                    newParty[i] = new PK6();
                for (int i = 0; i < newParty.Length; i++)
                    setPK6Party(newParty[i], Party + PK6.SIZE_PARTY*i);
            }
        }
        public PK6[] BattleBoxData
        {
            get
            {
                PK6[] data = new PK6[6];
                for (int i = 0; i < data.Length; i++)
                {
                    data[i] = getPK6Stored(BattleBox + PK6.SIZE_STORED * i);
                    if (data[i].Species == 0)
                        return data.Take(i).ToArray();
                }
                return data;
            }
        }

        public bool[] WC6Flags
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
        public WC6 getWC6(int index)
        {
            if (WondercardData < 0)
                return null;
            if (index < 0 || index > 24)
                return null;

            return new WC6(Data.Skip(WondercardData + index * WC6.Size).Take(WC6.Size).ToArray());
        }
        public void setWC6(WC6 wc6, int index)
        {
            if (WondercardData < 0)
                return;
            if (index < 0 || index > 24)
                return;

            wc6.Data.CopyTo(Data, WondercardData + index * WC6.Size);

            for (int i = 0; i < 24; i++)
                if (BitConverter.ToUInt16(Data, WondercardData + i * WC6.Size) == 0)
                    for (int j = i + 1; j < 24 - i; j++) // Shift everything down
                        Array.Copy(Data, WondercardData + j * WC6.Size, Data, WondercardData + (j - 1) * WC6.Size, WC6.Size);

            Edited = true;
        }

        // Writeback Validity
        public string checkChunkFF()
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

        // Debug
        public string getBlockInfoString()
        {
            return Blocks.Aggregate("", (current, b) => current +
                $"{b.ID.ToString("00")}: {b.Offset.ToString("X5")}-{(b.Offset + b.Length).ToString("X5")}, {b.Length.ToString("X5")}{Environment.NewLine}");
        }
    }
}
