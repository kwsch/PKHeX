using System;
using System.Linq;
using System.Text;

namespace PKHeX.Core
{
    /// <summary>
    /// Generation 6 <see cref="SaveFile"/> object.
    /// </summary>
    public sealed class SAV6 : SaveFile, ITrainerStatRecord, IPokePuff, ISecureValueStorage
    {
        // Save Data Attributes
        protected override string BAKText => $"{OT} ({Version}) - {LastSavedTime}";
        public override string Filter => "Main SAV|*.*";
        public override string Extension => string.Empty;

        public SAV6(byte[] data = null)
        {
            Data = data ?? new byte[SaveUtil.SIZE_G6ORAS];
            BAK = (byte[])Data.Clone();
            Exportable = !IsRangeEmpty(0, Data.Length);

            // Load Info
            Blocks = BlockInfo3DS.GetBlockInfoData(Data, out BlockInfoOffset, Checksums.CRC16_CCITT);
            GetSAVOffsets();

            HeldItems = ORAS ? Legal.HeldItem_AO : Legal.HeldItem_XY;
            Personal = ORAS ? PersonalTable.AO : PersonalTable.XY;
            if (!Exportable)
                ClearBoxes();
        }

        // Configuration
        public override SaveFile Clone() => new SAV6((byte[])Data.Clone());

        public override int SIZE_STORED => PKX.SIZE_6STORED;
        protected override int SIZE_PARTY => PKX.SIZE_6PARTY;
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

        public override int MaxMoveID => XY ? Legal.MaxMoveID_6_XY : Legal.MaxMoveID_6_AO;
        public override int MaxSpeciesID => Legal.MaxSpeciesID_6;
        public override int MaxItemID => XY ? Legal.MaxItemID_6_XY : Legal.MaxItemID_6_AO;
        public override int MaxAbilityID => XY ? Legal.MaxAbilityID_6_XY : Legal.MaxAbilityID_6_AO;
        public override int MaxBallID => Legal.MaxBallID_6;
        public override int MaxGameID => Legal.MaxGameID_6; // OR

        // Feature Overrides
        public int Puff { get; set; } = -1;
        public bool HasPuffData => Puff >= 0;

        // Blocks & Offsets
        private readonly int BlockInfoOffset;
        private readonly BlockInfo[] Blocks;
        protected override void SetChecksums() => Blocks.SetChecksums(Data);
        public override bool ChecksumsValid => Blocks.GetChecksumsValid(Data);
        public override string ChecksumInfo => Blocks.GetChecksumInfo(Data);

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
            if (ORASDEMO)
            {
                /* 00: */ Bag = 0x00000; // MyItem
                /* 01: */ ItemInfo = 0x00C00; // Select Bound Items
                /* 02: */ AdventureInfo = 0x00E00; // GameTime
                /* 03: */ Trainer1 = 0x01000; // Situation
                /* 04: */ // = 0x01200; [00004] // RandomGroup (rand seeds)
                /* 05: */ PlayTime = 0x01400; // PlayTime
                /* 06: */ // = 0x01600; [00024] // temp variables (u32 id + 32 u8)
                /* 07: */ // = 0x01800; [02100] // FieldMoveModelSave
                /* 08: */ Trainer2 = 0x03A00; // Misc
                /* 09: */ TrainerCard = 0x03C00; // MyStatus
                /* 10: */ Party = 0x03E00; // PokePartySave
                /* 11: */ EventConst = 0x04600; // EventWork
                /* 12: */ // = 0x04C00; [00004] // Packed Menu Bits
                /* 13: */ // = 0x04E00; [00048] // Repel Info, (Swarm?) and other overworld info (roamer)
                /* 14: */ SUBE = 0x05000; // PokeDiarySave
                /* 15: */ Record = 0x05400; // Record

                EventFlag = EventConst + 0x2FC;

                OFS_PouchHeldItem = Bag + 0;
                OFS_PouchKeyItem = Bag + 0x640;
                OFS_PouchTMHM = Bag + 0x7C0;
                OFS_PouchMedicine = Bag + 0x970;
                OFS_PouchBerry = Bag + 0xA70;
            }
            else if (XY)
            {
                /* 00: 00000-002C8, 002C8 */ Puff = 0x00000;
                /* 01: 00400-00F88, 00B88 */ Bag = 0x00400; // MyItem
                /* 02: 01000-0102C, 0002C */ ItemInfo = 0x1000; // Select Bound Items
                /* 03: 01200-01238, 00038 */ AdventureInfo = 0x01200; // GameTime
                /* 04: 01400-01550, 00150 */ Trainer1 = 0x1400; // Situation
                /* 05: 01600-01604, 00004 */ // RandomGroup (rand seeds)
                /* 06: 01800-01808, 00008 */ PlayTime = 0x1800; // PlayTime
                /* 07: 01A00-01BC0, 001C0 */ Accessories = 0x1A00; // Fashion
                /* 08: 01C00-01CBE, 000BE */ // amie minigame records
                /* 09: 01E00-01E24, 00024 */ // temp variables (u32 id + 32 u8)
                /* 10: 02000-04100, 02100 */ // FieldMoveModelSave
                /* 11: 04200-04340, 00140 */ Trainer2 = 0x4200; // Misc
                /* 12: 04400-04840, 00440 */ PCLayout = 0x4400; // BOX
                /* 13: 04A00-04F74, 00574 */ BattleBox = 0x04A00; // BattleBox
                /* 14: 05000-09E28, 04E28 */ PSS = 0x05000;
                /* 15: 0A000-0EE28, 04E28 */ // PSS2
                /* 16: 0F000-13E28, 04E28 */ // PSS3
                /* 17: 14000-14170, 00170 */ TrainerCard = 0x14000; // MyStatus
                /* 18: 14200-1481C, 0061C */ Party = 0x14200; // PokePartySave
                /* 19: 14A00-14F04, 00504 */ EventConst = 0x14A00; // EventWork
                /* 20: 15000-156A0, 006A0 */ PokeDex = 0x15000; // ZukanData
                /* 21: 15800-15E44, 00644 */ // hologram clips
                /* 22: 16000-16104, 00104 */ Fused = 0x16000; // UnionPokemon
                /* 23: 16200-16204, 00004 */ // ConfigSave
                /* 24: 16400-16820, 00420 */ // Amie decoration stuff
                /* 25: 16A00-16A64, 00064 */ OPower = 0x16A00;
                /* 26: 16C00-16FF0, 003F0 */ // Strength Rock position (xyz float: 84 entries, 12bytes/entry)
                /* 27: 17000-1770C, 0070C */ // Trainer PR Video
                /* 28: 17800-17980, 00180 */ GTS = 0x17800; // GtsData
                /* 29: 17A00-17A04, 00004 */ // Packed Menu Bits
                /* 30: 17C00-17C0C, 0000C */ // PSS Profile Q&A (6*questions, 6*answer)
                /* 31: 17E00-17E48, 00048 */ // Repel Info, (Swarm?) and other overworld info (roamer)
                /* 32: 18000-18054, 00054 */ // BOSS data fetch history (serial/mystery gift), 4byte intro & 20*4byte entries
                /* 33: 18200-18844, 00644 */ // Streetpass history (4 byte intro, 20*4byte entries, 20*76 byte entries)
                /* 34: 18A00-18FC8, 005C8 */ // LiveMatchData/BattleSpotData
                /* 35: 19000-192F8, 002F8 */ // MAC Address & Network Connection Logging (0x98 per entry, 5 entries)
                /* 36: 19400-1AF40, 01B40 */ HoF = 0x19400; // Dendou
                /* 37: 1B000-1B1F4, 001F4 */ MaisonStats = 0x1B1C0; // BattleInstSave
                /* 38: 1B200-1B3F0, 001F0 */ Daycare = 0x1B200; // Sodateya
                /* 39: 1B400-1B616, 00216 */ // BattleInstSave
                /* 40: 1B800-1BB90, 00390 */ BerryField = 0x1B800;
                /* 41: 1BC00-1D690, 01A90 */ WondercardFlags = 0x1BC00; // MysteryGiftSave
                /* 42: 1D800-1DB08, 00308 */ SUBE = 0x1D890; // PokeDiarySave
                /* 43: 1DC00-1E218, 00618 */ // Storyline Records
                /* 44: 1E400-1E65C, 0025C */ Record = 0x1E400; // Record
                /* 45: 1E800-1F034, 00834 */ // Friend Safari (0x15 per entry, 100 entries)
                /* 46: 1F200-1F518, 00318 */ SuperTrain = 0x1F200;
                /* 47: 1F600-1FDD0, 007D0 */ // Unused (lmao)
                /* 48: 1FE00-20A48, 00C48 */ LinkInfo = 0x1FE00;
                /* 49: 20C00-20C78, 00078 */ // PSS usage info
                /* 50: 20E00-21000, 00200 */ // GameSyncSave
                /* 51: 21000-21C84, 00C84 */ // PSS Icon (bool32 data present, 40x40 u16 pic, unused)
                /* 52: 21E00-22428, 00628 */ // ValidationSave (updatabale Public Key for legal check api calls)
                /* 53: 22600-570D0, 34AD0 */ Box = 0x22600;
                /* 54: 57200-65258, 0E058 */ JPEG = 0x57200;

                PCBackgrounds = PCLayout + 0x41E;
                PCFlags = PCLayout + 0x43D;
                LastViewedBox = PCLayout + 0x43F;
                EventFlag = EventConst + 0x2FC;
                PokeDexLanguageFlags = PokeDex + 0x3C8;
                Spinda = PokeDex + 0x648;
                WondercardData = WondercardFlags + 0x100;

                OFS_PouchHeldItem = Bag + 0;
                OFS_PouchKeyItem = Bag + 0x640;
                OFS_PouchTMHM = Bag + 0x7C0;
                OFS_PouchMedicine = Bag + 0x968;
                OFS_PouchBerry = Bag + 0xA68;
            }
            else if (ORAS)
            {
                /* 00: 00000-002C8, 002C8 */ Puff = 0x00000;
                /* 01: 00400-00F90, 00B90 */ Bag = 0x00400; // MyItem
                /* 02: 01000-0102C, 0002C */ ItemInfo = 0x1000; // Select Bound Items
                /* 03: 01200-01238, 00038 */ AdventureInfo = 0x01200; // GameTime
                /* 04: 01400-01550, 00150 */ Trainer1 = 0x01400; // Situation
                /* 05: 01600-01604, 00004 */ // RandomGroup (rand seeds)
                /* 06: 01800-01808, 00008 */ PlayTime = 0x1800; // PlayTime
                /* 07: 01A00-01BC0, 001C0 */ Accessories = 0x1A00; // Fashion
                /* 08: 01C00-01CBE, 000BE */ // amie minigame records
                /* 09: 01E00-01E24, 00024 */ // temp variables (u32 id + 32 u8)
                /* 10: 02000-04100, 02100 */ // FieldMoveModelSave
                /* 11: 04200-04330, 00130  */ Trainer2 = 0x04200; // Misc
                /* 12: 04400-04840, 00440  */ PCLayout = 0x04400; // BOX
                /* 13: 04A00-04F74, 00574  */ BattleBox = 0x04A00; // BattleBox
                /* 14: 05000-09E28, 04E28 */ PSS = 0x05000;
                /* 15: 0A000-0EE28, 04E28 */ // PSS2
                /* 16: 0F000-13E28, 04E28 */ // PSS3
                /* 17: 14000-14170, 00170 */ TrainerCard = 0x14000; // MyStatus
                /* 18: 14200-1481C, 0061C */ Party = 0x14200; // PokePartySave
                /* 19: 14A00-14F04, 00504 */ EventConst = 0x14A00; // EventWork
                /* 20: 15000-161CC, 011CC */ PokeDex = 0x15000; // ZukanData
                /* 21: 16200-16844, 00644 */ // hologram clips
                /* 22: 16A00-16B04, 00104 */ Fused = 0x16A00; // UnionPokemon
                /* 23: 16C00-16C04, 00004 */ // ConfigSave
                /* 24: 16E00-17220, 00420 */ // Amie decoration stuff
                /* 25: 17400-17464, 00064 */ OPower = 0x17400;
                /* 26: 17600-179F0, 003F0 */ // Strength Rock position (xyz float: 84 entries, 12bytes/entry)
                /* 27: 17A00-1810C, 0070C */ // Trainer PR Video
                /* 28: 18200-18380, 00180 */ GTS = 0x18200; // GtsData
                /* 29: 18400-18404, 00004 */ // Packed Menu Bits
                /* 30: 18600-1860C, 0000C */ // PSS Profile Q&A (6*questions, 6*answer)
                /* 31: 18800-18848, 00048 */ // Repel Info, (Swarm?) and other overworld info (roamer)
                /* 32: 18A00-18A54, 00054 */ // BOSS data fetch history (serial/mystery gift), 4byte intro & 20*4byte entries
                /* 33: 18C00-19244, 00644 */ // Streetpass history
                /* 34: 19400-199C8, 005C8 */ // LiveMatchData/BattleSpotData
                /* 35: 19A00-19CF8, 002F8 */ // MAC Address & Network Connection Logging (0x98 per entry, 5 entries)
                /* 36: 19E00-1B940, 01B40 */ HoF = 0x19E00; // Dendou
                /* 37: 1BA00-1BBF4, 001F4 */ MaisonStats = 0x1BBC0; // BattleInstSave
                /* 38: 1BC00-1BFE0, 003E0 */ Daycare = 0x1BC00; // Sodateya
                /* 39: 1C000-1C216, 00216 */ // BattleInstSave
                /* 40: 1C400-1CA40, 00640 */ BerryField = 0x1C400;
                /* 41: 1CC00-1E690, 01A90 */ WondercardFlags = 0x1CC00; // MysteryGiftSave
                /* 42: 1E800-1EC00, 00400 */ // Storyline Records
                /* 43: 1EC00-1F218, 00618 */ SUBE = 0x1D890; // PokeDiarySave
                /* 44: 1F400-1F65C, 0025C */ Record = 0x1F400; // Record
                /* 45: 1F800-20034, 00834 */ // Friend Safari (0x15 per entry, 100 entries)
                /* 46: 20200-20518, 00318 */ SuperTrain = 0x20200;
                /* 47: 20600-20DD0, 007D0 */ // Unused (lmao)
                /* 48: 20E00-21A48, 00C48 */ LinkInfo = 0x20E00;
                /* 49: 21C00-21C78, 00078 */ // PSS usage info
                /* 50: 21E00-22000, 00200 */ // GameSyncSave
                /* 51: 22000-22C84, 00C84 */ // PSS Icon (bool32 data present, 40x40 u16 pic, unused)
                /* 52: 22E00-23428, 00628 */ // ValidationSave (updatabale Public Key for legal check api calls)
                /* 53: 23600-23A00, 00400 */ Contest = 0x23600;
                /* 54: 23A00-2B4D0, 07AD0 */ SecretBase = 0x23A00;
                /* 55: 2B600-32EB0, 078B0 */ EonTicket = 0x319B8;
                /* 56: 33000-67AD0, 34AD0 */ Box = 0x33000;
                /* 57: 67C00-75C58, 0E058 */ JPEG = 0x67C00;

                PCBackgrounds = PCLayout + 0x41E;
                PCFlags = PCLayout + 0x43D;
                LastViewedBox = PCLayout + 0x43F;
                EventFlag = EventConst + 0x2FC;
                PokeDexLanguageFlags = PokeDex + 0x400;
                Spinda = PokeDex + 0x680;
                EncounterCount = PokeDex + 0x686;
                WondercardData = WondercardFlags + 0x100;
                Daycare2 = Daycare + 0x1F0;

                OFS_PouchHeldItem = Bag + 0;
                OFS_PouchKeyItem = Bag + 0x640;
                OFS_PouchTMHM = Bag + 0x7C0;
                OFS_PouchMedicine = Bag + 0x970;
                OFS_PouchBerry = Bag + 0xA70;
            }
            else // Empty input
            {
                Party = 0x0;
                Box = Party + (SIZE_PARTY * 6) + 0x1000;
            }
        }

        // Private Only
        private int Bag { get; set; } = int.MinValue;
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
        public int Record { get; private set; } = int.MinValue;
        public int MaisonStats { get; private set; } = int.MinValue;
        public int EonTicket { get; private set; } = int.MinValue;
        public int PCBackgrounds { get; private set; } = int.MinValue;
        public int Contest { get; private set; } = int.MinValue;
        public int Accessories { get; private set; } = int.MinValue;
        public int PokeDexLanguageFlags { get; private set; } = int.MinValue;
        public int Spinda { get; private set; } = int.MinValue;
        public int EncounterCount { get; private set; } = int.MinValue;

        private const int LongStringLength = 0x22; // bytes, not characters
        private const int ShortStringLength = 0x1A; // bytes, not characters

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

        public override int MultiplayerSpriteID
        {
            get => Data[TrainerCard + 7];
            set => Data[TrainerCard + 7] = (byte)value;
        }

        public override int GameSyncIDSize => 16; // 64 bits

        public override string GameSyncID
        {
            get
            {
                var data = Data.Skip(TrainerCard + 8).Take(GameSyncIDSize / 2).Reverse().ToArray();
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
                     .ToArray().CopyTo(Data, TrainerCard + 8);
            }
        }

        public override int SubRegion
        {
            get => Data[TrainerCard + 0x26];
            set => Data[TrainerCard + 0x26] = (byte)value;
        }

        public override int Country
        {
            get => Data[TrainerCard + 0x27];
            set => Data[TrainerCard + 0x27] = (byte)value;
        }

        public override int ConsoleRegion
        {
            get => Data[TrainerCard + 0x2C];
            set => Data[TrainerCard + 0x2C] = (byte)value;
        }

        public override int Language
        {
            get => Data[TrainerCard + 0x2D];
            set => Data[TrainerCard + 0x2D] = (byte)value;
        }

        public override string OT
        {
            get => GetString(TrainerCard + 0x48, 0x1A);
            set => SetData(SetString(value, OTLength), TrainerCard + 0x48);
        }

        public string OT_Nick
        {
            get => GetString(TrainerCard + 0x62, ShortStringLength / 2);
            set => SetData(SetString(value, ShortStringLength/2), TrainerCard + 0x62);
        }

        private int GetSayingOffset(int say) => TrainerCard + 0x7C + (LongStringLength * say);
        private string GetSaying(int say) => GetString(GetSayingOffset(say), LongStringLength);
        private void SetSaying(int say, string value) => SetData(SetString(value, LongStringLength / 2), GetSayingOffset(say));

        public string Saying1 { get => GetSaying(0); set => SetSaying(0, value); }
        public string Saying2 { get => GetSaying(1); set => SetSaying(1, value); }
        public string Saying3 { get => GetSaying(2); set => SetSaying(2, value); }
        public string Saying4 { get => GetSaying(3); set => SetSaying(3, value); }
        public string Saying5 { get => GetSaying(4); set => SetSaying(4, value); }

        public short EyeColor
        {
            get => BitConverter.ToInt16(Data, TrainerCard + 0x148);
            set => BitConverter.GetBytes(value).CopyTo(Data, TrainerCard + 0x148);
        }

        public bool IsMegaEvolutionUnlocked
        {
            get => (Data[TrainerCard + 0x14A] & 0x01) != 0;
            set => Data[TrainerCard + 0x14A] = (byte)((Data[TrainerCard + 0x14A] & 0xFE) | (value ? 1 : 0)); // in battle
        }

        public bool IsMegaRayquazaUnlocked
        {
            get => (Data[TrainerCard + 0x14A] & 0x02) != 0;
            set => Data[TrainerCard + 0x14A] = (byte)((Data[TrainerCard + 0x14A] & ~2) | (value ? 2 : 0)); // in battle
        }

        public int M
        {
            get => BitConverter.ToUInt16(Data, Trainer1 + 0x02);
            set
            {
                var val = BitConverter.GetBytes((ushort)value);
                val.CopyTo(Data, Trainer1 + 0x02);
                val.CopyTo(Data, Trainer1 + 0x02 + 0xF4);
            }
        }

        public float X
        {
            get => BitConverter.ToSingle(Data, Trainer1 + 0x10) / 18;
            set
            {
                var val = BitConverter.GetBytes(value * 18);
                val.CopyTo(Data, Trainer1 + 0x10);
                val.CopyTo(Data, Trainer1 + 0x10 + 0xF4);
            }
        }

        public float Z
        {
            get => BitConverter.ToSingle(Data, Trainer1 + 0x14);
            set
            {
                var val = BitConverter.GetBytes(value);
                val.CopyTo(Data, Trainer1 + 0x14);
                val.CopyTo(Data, Trainer1 + 0x14 + 0xF4);
            }
        }

        public float Y
        {
            get => BitConverter.ToSingle(Data, Trainer1 + 0x18) / 18;
            set
            {
                var val = BitConverter.GetBytes(value * 18);
                val.CopyTo(Data, Trainer1 + 0x18);
                val.CopyTo(Data, Trainer1 + 0x18 + 0xF4);
            }
        }

        public int Style
        {
            get => Data[Trainer1 + 0x14D];
            set => Data[Trainer1 + 0x14D] = (byte)value;
        }

        public override uint Money
        {
            get => BitConverter.ToUInt32(Data, Trainer2 + 0x8);
            set => BitConverter.GetBytes(value).CopyTo(Data, Trainer2 + 0x8);
        }

        public int Badges
        {
            get => Data[Trainer2 + 0xC];
            set => Data[Trainer2 + 0xC] = (byte)value;
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
        public override uint SecondsToStart { get => BitConverter.ToUInt32(Data, AdventureInfo + 0x18); set => BitConverter.GetBytes(value).CopyTo(Data, AdventureInfo + 0x18); }
        public override uint SecondsToFame { get => BitConverter.ToUInt32(Data, AdventureInfo + 0x20); set => BitConverter.GetBytes(value).CopyTo(Data, AdventureInfo + 0x20); }

        public ushort GetMaisonStat(int index) { return BitConverter.ToUInt16(Data, MaisonStats + (2 * index)); }
        public void SetMaisonStat(int index, ushort value) { BitConverter.GetBytes(value).CopyTo(Data, MaisonStats + (2 * index)); }
        public uint GetEncounterCount(int index) { return BitConverter.ToUInt16(Data, EncounterCount + (2 * index)); }
        public void SetEncounterCount(int index, ushort value) { BitConverter.GetBytes(value).CopyTo(Data, EncounterCount + (2 * index)); }

        // Daycare
        public override int DaycareSeedSize => 16;
        public override bool HasTwoDaycares => ORAS;

        public override int GetDaycareSlotOffset(int loc, int slot)
        {
            int ofs = loc == 0 ? Daycare : Daycare2;
            if (ofs < 0)
                return -1;
            return ofs + 8 + (slot * (SIZE_STORED + 8));
        }

        public override uint? GetDaycareEXP(int loc, int slot)
        {
            int ofs = loc == 0 ? Daycare : Daycare2;
            if (ofs > -1)
                return BitConverter.ToUInt32(Data, ofs + ((SIZE_STORED + 8) * slot) + 4);
            return null;
        }

        public override bool? IsDaycareOccupied(int loc, int slot)
        {
            int ofs = loc == 0 ? Daycare : Daycare2;
            if (ofs > -1)
                return Data[ofs + ((SIZE_STORED + 8) * slot)] == 1;
            return null;
        }

        public override string GetDaycareRNGSeed(int loc)
        {
            int ofs = loc == 0 ? Daycare : Daycare2;
            if (ofs <= 0)
                return null;

            var data = Data.Skip(ofs + 0x1E8).Take(DaycareSeedSize / 2).Reverse().ToArray();
            return BitConverter.ToString(data).Replace("-", string.Empty);
        }

        public override bool? IsDaycareHasEgg(int loc)
        {
            int ofs = loc == 0 ? Daycare : Daycare2;
            if (ofs > -1)
                return Data[ofs + 0x1E0] == 1;
            return null;
        }

        public override void SetDaycareEXP(int loc, int slot, uint EXP)
        {
            int ofs = loc == 0 ? Daycare : Daycare2;
            if (ofs > -1)
                BitConverter.GetBytes(EXP).CopyTo(Data, ofs + ((SIZE_STORED + 8) * slot) + 4);
        }

        public override void SetDaycareOccupied(int loc, int slot, bool occupied)
        {
            int ofs = loc == 0 ? Daycare : Daycare2;
            if (ofs > -1)
                Data[ofs + ((SIZE_STORED + 8) * slot)] = (byte)(occupied ? 1 : 0);
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
                 .Select(x => Convert.ToByte(seed.Substring(x, 2), 16))
                 .Reverse().ToArray().CopyTo(Data, Daycare + 0x1E8);
        }

        public override void SetDaycareHasEgg(int loc, bool hasEgg)
        {
            int ofs = loc == 0 ? Daycare : Daycare2;
            if (ofs > -1)
                Data[ofs + 0x1E0] = (byte)(hasEgg ? 1 : 0);
        }

        public byte[] Puffs { get => GetData(Puff, 100); set => SetData(value, Puff); }
        public int PuffCount { get => BitConverter.ToInt32(Data, Puff + 100); set => BitConverter.GetBytes(value).CopyTo(Data, Puff + 100); }

        public int[] SelectItems
        {
            // UP,RIGHT,DOWN,LEFT
            get
            {
                int[] list = new int[4];
                for (int i = 0; i < list.Length; i++)
                    list[i] = BitConverter.ToUInt16(Data, ItemInfo + 10 + (2 * i));
                return list;
            }
            set
            {
                if (value == null || value.Length > 4)
                    return;
                for (int i = 0; i < value.Length; i++)
                    BitConverter.GetBytes((ushort)value[i]).CopyTo(Data, ItemInfo + 10 + (2 * i));
            }
        }

        public int[] RecentItems
        {
            // Items recently interacted with (Give, Use)
            get
            {
                int[] list = new int[12];
                for (int i = 0; i < list.Length; i++)
                    list[i] = BitConverter.ToUInt16(Data, ItemInfo + 20 + (2 * i));
                return list;
            }
            set
            {
                if (value == null || value.Length > 12)
                    return;
                for (int i = 0; i < value.Length; i++)
                    BitConverter.GetBytes((ushort)value[i]).CopyTo(Data, ItemInfo + 20 + (2 * i));
            }
        }

        public override string JPEGTitle => JPEG < 0 ? string.Empty : Util.TrimFromZero(Encoding.Unicode.GetString(Data, JPEG, 0x1A));
        public override byte[] JPEGData => JPEG < 0 || Data[JPEG + 0x54] != 0xFF ? Array.Empty<byte>() : GetData(JPEG + 0x54, 0xE004);

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
                    new InventoryPouch4(InventoryType.Items, legalItems, 999, OFS_PouchHeldItem),
                    new InventoryPouch4(InventoryType.KeyItems, legalKey, 1, OFS_PouchKeyItem),
                    new InventoryPouch4(InventoryType.TMHMs, legalTMHM, 1, OFS_PouchTMHM),
                    new InventoryPouch4(InventoryType.Medicine, legalMedicine, 999, OFS_PouchMedicine),
                    new InventoryPouch4(InventoryType.Berries, Legal.Pouch_Berry_XY, 999, OFS_PouchBerry),
                };
                return pouch.LoadAll(Data);
            }
            set => value.SaveAll(Data);
        }

        // Storage
        public override int CurrentBox { get => Data[LastViewedBox]; set => Data[LastViewedBox] = (byte)value; }

        public override int GetPartyOffset(int slot)
        {
            return Party + (SIZE_PARTY * slot);
        }

        public override int GetBoxOffset(int box)
        {
            return Box + (SIZE_STORED * box * 30);
        }

        protected override int GetBoxWallpaperOffset(int box)
        {
            int ofs = PCBackgrounds > 0 && PCBackgrounds < Data.Length ? PCBackgrounds : -1;
            if (ofs > -1)
                return ofs + box;
            return ofs;
        }

        private int GetBoxNameOffset(int box) => PCLayout + (LongStringLength * box);

        public override string GetBoxName(int box)
        {
            if (PCLayout < 0)
                return "B" + (box + 1);
            return Util.TrimFromZero(Encoding.Unicode.GetString(Data, GetBoxNameOffset(box), LongStringLength));
        }

        public override void SetBoxName(int box, string value)
        {
            SetData(Encoding.Unicode.GetBytes(value.PadRight(LongStringLength / 2, '\0')), PCLayout + (LongStringLength * box));
        }

        protected override PKM GetPKM(byte[] data)
        {
            return new PK6(data);
        }

        protected override void SetPKM(PKM pkm)
        {
            PK6 pk6 = (PK6)pkm;
            // Apply to this Save File
            int CT = pk6.CurrentHandler;
            DateTime Date = DateTime.Now;
            pk6.Trade(this, Date.Day, Date.Month, Date.Year);
            if (CT != pk6.CurrentHandler) // Logic updated Friendship
            {
                // Copy over the Friendship Value only under certain circumstances
                if (pk6.Moves.Contains(216)) // Return
                    pk6.CurrentFriendship = pk6.OppositeFriendship;
                else if (pk6.Moves.Contains(218)) // Frustration
                    pkm.CurrentFriendship = pk6.OppositeFriendship;
            }
            pkm.RefreshChecksum();
            if (Record > 0)
                AddCountAcquired(pkm);
        }

        private void AddCountAcquired(PKM pkm)
        {
            AddRecord(pkm.WasEgg ? 009 : 007); // egg, capture
            if (pkm.CurrentHandler == 1)
                AddRecord(012); // trade
            if (!pkm.WasEgg)
                AddRecord(005); // wild encounters
        }

        protected override void SetDex(PKM pkm)
        {
            if (PokeDex < 0)
                return;
            if (pkm.Species == 0)
                return;
            if (pkm.Species > MaxSpeciesID)
                return;
            if (Version == GameVersion.Invalid)
                return;

            const int brSize = 0x60;
            int bit = pkm.Species - 1;
            int lang = pkm.Language - 1; if (lang > 5) lang--; // 0-6 language vals
            int origin = pkm.Version;
            int gender = pkm.Gender % 2; // genderless -> male
            int shiny = pkm.IsShiny ? 1 : 0;
            int shiftoff = brSize * (1 + gender + (2 * shiny)); // after the Owned region
            int bd = bit >> 3; // div8
            int bm = bit & 7; // mod8
            byte mask = (byte)(1 << bm);
            int ofs = PokeDex + 0x8 + bd;

            // Owned quality flag
            if (origin < 0x18 && bit < 649 && !ORAS) // Species: 1-649 for X/Y, and not for ORAS; Set the Foreign Owned Flag
                Data[ofs + 0x644] |= mask;
            else if (origin >= 0x18 || ORAS) // Set Native Owned Flag (should always happen)
                Data[ofs + (brSize * 0)] |= mask;

            // Set the [Species/Gender/Shiny] Seen Flag
            Data[ofs + shiftoff] |= mask;

            // Set the Display flag if none are set
            bool Displayed = false;
            Displayed |= (Data[ofs + (brSize * 5)] & mask) != 0;
            Displayed |= (Data[ofs + (brSize * 6)] & mask) != 0;
            Displayed |= (Data[ofs + (brSize * 7)] & mask) != 0;
            Displayed |= (Data[ofs + (brSize * 8)] & mask) != 0;
            if (!Displayed) // offset is already biased by brSize, reuse shiftoff but for the display flags.
                Data[ofs + (brSize * 4) + shiftoff] |= mask;

            // Set the Language
            if (lang < 0) lang = 1;
            Data[PokeDexLanguageFlags + (((bit * 7) + lang) / 8)] |= (byte)(1 << (((bit * 7) + lang) % 8));

            // Set DexNav count (only if not encountered previously)
            if (ORAS && GetEncounterCount(pkm.Species - 1) == 0)
                SetEncounterCount(pkm.Species - 1, 1);

            // Set Form flags
            int fc = Personal[pkm.Species].FormeCount;
            int f = ORAS ? DexFormUtil.GetDexFormIndexORAS(pkm.Species, fc) : DexFormUtil.GetDexFormIndexXY(pkm.Species, fc);
            if (f < 0) return;

            int FormLen = ORAS ? 0x26 : 0x18;
            int FormDex = PokeDex + 0x8 + (brSize * 9);
            bit = f + pkm.AltForm;

            // Set Form Seen Flag
            Data[FormDex + (FormLen * shiny) + (bit / 8)] |= (byte)(1 << (bit % 8));

            // Set Displayed Flag if necessary, check all flags
            for (int i = 0; i < fc; i++)
            {
                bit = f + i;
                if ((Data[FormDex + (FormLen * 2) + (bit / 8)] & (byte)(1 << (bit % 8))) != 0) // Nonshiny
                    return; // already set
                if ((Data[FormDex + (FormLen * 3) + (bit / 8)] & (byte)(1 << (bit % 8))) != 0) // Shiny
                    return; // already set
            }
            bit = f + pkm.AltForm;
            Data[FormDex + (FormLen * (2 + shiny)) + (bit / 8)] |= (byte)(1 << (bit % 8));
        }

        protected override void SetPartyValues(PKM pkm, bool isParty)
        {
            base.SetPartyValues(pkm, isParty);
            ((PK6)pkm).FormDuration = GetFormDuration(pkm, isParty);
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

        public override bool GetCaught(int species)
        {
            int bit = species - 1;
            int bd = bit >> 3; // div8
            int bm = bit & 7; // mod8
            int ofs = PokeDex // Raw Offset
                      + 0x08; // Magic + Flags

            if ((1 << bm & Data[ofs + bd]) != 0)
                return true; // Owned Native

            if (ORAS || bit >= 649) // no Foreign flag
                return false;
            return (1 << bm & Data[ofs + bd + 0x644]) != 0;
        }

        public override bool GetSeen(int species)
        {
            const int brSize = 0x60;

            int bit = species - 1;
            int bd = bit >> 3; // div8
            int bm = bit & 7; // mod8
            byte mask = (byte)(1 << bm);
            int ofs = PokeDex // Raw Offset
                      + 0x08; // Magic + Flags

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

        public override bool BattleBoxLocked
        {
            get => Data[BattleBox + (6 * SIZE_STORED)] != 0;
            set => Data[BattleBox + (6 * SIZE_STORED)] = (byte)(value ? 1 : 0);
        }

        public override int BoxesUnlocked { get => Data[PCFlags + 1] - 1; set => Data[PCFlags + 1] = (byte)(value + 1); }

        public override byte[] BoxFlags
        {
            get => new[] { Data[PCFlags] }; // 7 bits for wallpaper unlocks, top bit to unlock final box (delta episode)
            set
            {
                if (value.Length != 1)
                    return;
                Data[PCFlags] = value[0];
            }
        }

        // Mystery Gift
        protected override bool[] MysteryGiftReceivedFlags
        {
            get
            {
                if (WondercardData < 0 || WondercardFlags < 0)
                    return Array.Empty<bool>();

                bool[] result = new bool[(WondercardData - WondercardFlags) * 8];
                for (int i = 0; i < result.Length; i++)
                    result[i] = (Data[WondercardFlags + (i >> 3)] >> (i & 7) & 0x1) == 1;
                return result;
            }
            set
            {
                if (WondercardData < 0 || WondercardFlags < 0)
                    return;
                if (value == null || (WondercardData - WondercardFlags) * 8 != value.Length)
                    return;

                byte[] data = new byte[value.Length / 8];
                for (int i = 0; i < value.Length; i++)
                {
                    if (value[i])
                        data[i >> 3] |= (byte)(1 << (i & 7));
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
                    cards[i] = GetWC6(i);

                return cards;
            }
            set
            {
                if (value == null)
                    return;
                if (value.Length > GiftCountMax)
                    Array.Resize(ref value, GiftCountMax);

                for (int i = 0; i < value.Length; i++)
                    SetWC6(value[i], i);
                for (int i = value.Length; i < GiftCountMax; i++)
                    SetWC6(new WC6(), i);
            }
        }

        public byte[] LinkBlock
        {
            get
            {
                if (LinkInfo < 0)
                    return Array.Empty<byte>();
                return GetData(LinkInfo, 0xC48);
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

        private MysteryGift GetWC6(int index)
        {
            if (WondercardData < 0)
                return null;
            if (index < 0 || index > GiftCountMax)
                return null;

            return new WC6(GetData(WondercardData + (index * WC6.Size), WC6.Size));
        }

        private void SetWC6(MysteryGift wc6, int index)
        {
            if (WondercardData < 0)
                return;
            if (index < 0 || index > GiftCountMax)
                return;

            SetData(wc6.Data, WondercardData + (index * WC6.Size));
        }

        // Gym History
        public ushort[][] GymTeams
        {
            get
            {
                if (SUBE < 0 || ORASDEMO)
                    return Array.Empty<ushort[]>(); // no gym data

                const int teamsize = 2 * 6; // 2byte/species, 6species/team
                const int size = teamsize * 8; // 8 gyms
                int ofs = SUBE - size - 4;

                var data = GetData(ofs, size);
                ushort[][] teams = new ushort[8][];
                for (int i = 0; i < teams.Length; i++)
                    Buffer.BlockCopy(data, teamsize * i, teams[i] = new ushort[6], 0, teamsize);
                return teams;
            }
            set
            {
                if (SUBE < 0 || ORASDEMO)
                    return; // no gym data

                const int teamsize = 2 * 6; // 2byte/species, 6species/team
                const int size = teamsize * 8; // 8 gyms
                int ofs = SUBE - size - 4;

                byte[] data = new byte[size];
                for (int i = 0; i < value.Length; i++)
                    Buffer.BlockCopy(value[i], 0, data, teamsize * i, teamsize);
                SetData(data, ofs);
            }
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

        public override string GetString(byte[] data, int offset, int length) => StringConverter.GetString6(data, offset, length);

        public override byte[] SetString(string value, int maxLength, int PadToSize = 0, ushort PadWith = 0)
        {
            if (PadToSize == 0)
                PadToSize = maxLength + 1;
            return StringConverter.SetString6(value, maxLength, PadToSize, PadWith);
        }

        public OPower6 OPowerData
        {
            get => new OPower6(Data, OPower);
            set => SetData(value.Write(), 0);
        }

        public void UnlockAllFriendSafariSlots()
        {
            if (!XY)
                return;

            // Unlock + reveal all safari slots if friend data is present
            const int start = 0x1E7FF;
            const int size = 0x15;
            for (int i = 1; i < 101; i++)
            {
                int ofs = start + (i * size);
                if (Data[ofs] != 0) // no friend data == 0x00
                    Data[ofs] = 0x3D;
            }
            Edited = true;
        }

        public void UnlockAllAccessories()
        {
            if (!XY)
                return;

            new byte[]
            {
                0xFE,0xFF,0xFF,0x7E,0xFF,0xFD,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,
                0xFF,0xEF,0xFF,0xFF,0xFF,0xF9,0xFF,0xFB,0xFF,0xF7,0xFF,0xFF,0x0F,0x00,0x00,0x00,
                0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0xFE,0xFF,
                0xFF,0x7E,0xFF,0xFD,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xEF,
                0xFF,0xFF,0xFF,0xF9,0xFF,0xFB,0xFF,0xF7,0xFF,0xFF,0x0F,0x00,0x00,0x00,0x00,0x00,
                0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x01,0x00,0x00,0x00,
                0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00
            }.CopyTo(Data, Accessories);
        }

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
            var maxes = XY ? Records.MaxType_XY : Records.MaxType_AO;
            int max = Records.GetMax(recordID, maxes);
            if (value > max)
                return; // out of range, don't set value
            if (recordID < 100)
                BitConverter.GetBytes(value).CopyTo(Data, ofs);
            if (recordID < 200)
                BitConverter.GetBytes((ushort)value).CopyTo(Data, ofs);
        }

        public int GetRecordMax(int recordID) => Records.GetMax(recordID, XY ? Records.MaxType_XY : Records.MaxType_AO);
        public int GetRecordOffset(int recordID) => Records.GetOffset(Record, recordID);
        public void AddRecord(int recordID) => SetRecord(recordID, GetRecord(recordID) + 1);
    }
}
