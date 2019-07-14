using System;
using System.Text;

namespace PKHeX.Core
{
    /// <summary>
    /// Generation 6 <see cref="SaveFile"/> object for <see cref="GameVersion.XY"/>.
    /// </summary>
    /// <inheritdoc cref="SAV6" />
    public sealed class SAV6XY : SAV6, IPokePuff, IOPower, ILink
    {
        public SAV6XY(byte[] data) : base(data, BlocksXY, boXY) => Initialize();

        public SAV6XY() : base(SaveUtil.SIZE_G6XY, BlocksXY, boXY)
        {
            Initialize();
            ClearBoxes();
        }

        public override SaveFile Clone() => new SAV6XY((byte[])Data.Clone());
        public override int MaxMoveID => Legal.MaxMoveID_6_XY;
        public override int MaxItemID => Legal.MaxItemID_6_XY;
        public override int MaxAbilityID => Legal.MaxAbilityID_6_XY;

        private const int boXY = SaveUtil.SIZE_G6XY - 0x200;

        public static readonly BlockInfo[] BlocksXY =
        {
            new BlockInfo6(boXY, 00, 0x00000, 0x002C8),
            new BlockInfo6(boXY, 01, 0x00400, 0x00B88),
            new BlockInfo6(boXY, 02, 0x01000, 0x0002C),
            new BlockInfo6(boXY, 03, 0x01200, 0x00038),
            new BlockInfo6(boXY, 04, 0x01400, 0x00150),
            new BlockInfo6(boXY, 05, 0x01600, 0x00004),
            new BlockInfo6(boXY, 06, 0x01800, 0x00008),
            new BlockInfo6(boXY, 07, 0x01A00, 0x001C0),
            new BlockInfo6(boXY, 08, 0x01C00, 0x000BE),
            new BlockInfo6(boXY, 09, 0x01E00, 0x00024),
            new BlockInfo6(boXY, 10, 0x02000, 0x02100),
            new BlockInfo6(boXY, 11, 0x04200, 0x00140),
            new BlockInfo6(boXY, 12, 0x04400, 0x00440),
            new BlockInfo6(boXY, 13, 0x04A00, 0x00574),
            new BlockInfo6(boXY, 14, 0x05000, 0x04E28),
            new BlockInfo6(boXY, 15, 0x0A000, 0x04E28),
            new BlockInfo6(boXY, 16, 0x0F000, 0x04E28),
            new BlockInfo6(boXY, 17, 0x14000, 0x00170),
            new BlockInfo6(boXY, 18, 0x14200, 0x0061C),
            new BlockInfo6(boXY, 19, 0x14A00, 0x00504),
            new BlockInfo6(boXY, 20, 0x15000, 0x006A0),
            new BlockInfo6(boXY, 21, 0x15800, 0x00644),
            new BlockInfo6(boXY, 22, 0x16000, 0x00104),
            new BlockInfo6(boXY, 23, 0x16200, 0x00004),
            new BlockInfo6(boXY, 24, 0x16400, 0x00420),
            new BlockInfo6(boXY, 25, 0x16A00, 0x00064),
            new BlockInfo6(boXY, 26, 0x16C00, 0x003F0),
            new BlockInfo6(boXY, 27, 0x17000, 0x0070C),
            new BlockInfo6(boXY, 28, 0x17800, 0x00180),
            new BlockInfo6(boXY, 29, 0x17A00, 0x00004),
            new BlockInfo6(boXY, 30, 0x17C00, 0x0000C),
            new BlockInfo6(boXY, 31, 0x17E00, 0x00048),
            new BlockInfo6(boXY, 32, 0x18000, 0x00054),
            new BlockInfo6(boXY, 33, 0x18200, 0x00644),
            new BlockInfo6(boXY, 34, 0x18A00, 0x005C8),
            new BlockInfo6(boXY, 35, 0x19000, 0x002F8),
            new BlockInfo6(boXY, 36, 0x19400, 0x01B40),
            new BlockInfo6(boXY, 37, 0x1B000, 0x001F4),
            new BlockInfo6(boXY, 38, 0x1B200, 0x001F0),
            new BlockInfo6(boXY, 39, 0x1B400, 0x00216),
            new BlockInfo6(boXY, 40, 0x1B800, 0x00390),
            new BlockInfo6(boXY, 41, 0x1BC00, 0x01A90),
            new BlockInfo6(boXY, 42, 0x1D800, 0x00308),
            new BlockInfo6(boXY, 43, 0x1DC00, 0x00618),
            new BlockInfo6(boXY, 44, 0x1E400, 0x0025C),
            new BlockInfo6(boXY, 45, 0x1E800, 0x00834),
            new BlockInfo6(boXY, 46, 0x1F200, 0x00318),
            new BlockInfo6(boXY, 47, 0x1F600, 0x007D0),
            new BlockInfo6(boXY, 48, 0x1FE00, 0x00C48),
            new BlockInfo6(boXY, 49, 0x20C00, 0x00078),
            new BlockInfo6(boXY, 50, 0x20E00, 0x00200),
            new BlockInfo6(boXY, 51, 0x21000, 0x00C84),
            new BlockInfo6(boXY, 52, 0x21E00, 0x00628),
            new BlockInfo6(boXY, 53, 0x22600, 0x34AD0),
            new BlockInfo6(boXY, 54, 0x57200, 0x0E058),
        };

        private void Initialize()
        {
            /* 00: 00000-002C8, 002C8 */ // Puff = 0x00000;
            /* 01: 00400-00F88, 00B88 */ // MyItem = 0x00400; // Bag
            /* 02: 01000-0102C, 0002C */ // ItemInfo = 0x1000; // Select Bound Items
            /* 03: 01200-01238, 00038 */ // GameTime = 0x01200;
            /* 04: 01400-01550, 00150 */ Trainer1 = 0x1400; // Situation
            /* 05: 01600-01604, 00004 */ // RandomGroup (rand seeds)
            /* 06: 01800-01808, 00008 */ // PlayTime
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
            /* 17: 14000-14170, 00170 */ // MyStatus
            /* 18: 14200-1481C, 0061C */ Party = 0x14200; // PokePartySave
            /* 19: 14A00-14F04, 00504 */ EventConst = 0x14A00; // EventWork
            /* 20: 15000-156A0, 006A0 */ PokeDex = 0x15000; // ZukanData
            /* 21: 15800-15E44, 00644 */ // hologram clips
            /* 22: 16000-16104, 00104 */ Fused = 0x16000; // UnionPokemon
            /* 23: 16200-16204, 00004 */ // ConfigSave
            /* 24: 16400-16820, 00420 */ // Amie decoration stuff
            /* 25: 16A00-16A64, 00064 */ // OPower = 0x16A00;
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
            /* 44: 1E400-1E65C, 0025C */ // Record = 0x1E400;
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

            Items = new MyItem6XY(this, 0x00400);
            PuffBlock = new Puff6(this, 0x00000);
            GameTime = new GameTime6(this, 0x01200);
            Situation = new Situation6(this, 0x01400);
            Played = new PlayTime6(this, 0x01800);
            BoxLayout = new BoxLayout6(this, 0x4400);
            BattleBoxBlock = new BattleBox6(this, 0x04A00);
            Status = new MyStatus6XY(this, 0x14000);
            Zukan = new Zukan6XY(this, 0x15000, 0x3C8);
            OPowerBlock = new OPower6(this, 0x16A00);
            MysteryBlock = new MysteryBlock6(this, 0x1BC00);
            Records = new Record6(this, 0x1E400, Core.Records.MaxType_XY);

            EventFlag = EventConst + 0x2FC;
            WondercardData = WondercardFlags + 0x100;

            HeldItems = Legal.HeldItem_XY;
            Personal = PersonalTable.XY;
        }

        public Zukan6 Zukan { get; private set; }
        public Puff6 PuffBlock { get; private set; }
        public OPower6 OPowerBlock { get; private set; }
        public BoxLayout6 BoxLayout { get; private set; }
        public MysteryBlock6 MysteryBlock { get; private set; }
        public BattleBox6 BattleBoxBlock { get; private set; }

        protected override void SetDex(PKM pkm) => Zukan.SetDex(pkm);

        // Daycare
        public override int DaycareSeedSize => 16;
        public override bool HasTwoDaycares => false;
        public override bool? IsDaycareOccupied(int loc, int slot) => Data[Daycare + 0 + ((SIZE_STORED + 8) * slot)] == 1;
        public override uint? GetDaycareEXP(int loc, int slot) => BitConverter.ToUInt32(Data, Daycare + 4 + ((SIZE_STORED + 8) * slot));

        public override int GetDaycareSlotOffset(int loc, int slot) => Daycare + 8 + (slot * (SIZE_STORED + 8));
        public override bool? IsDaycareHasEgg(int loc) => Data[Daycare + 0x1E0] == 1;
        public override void SetDaycareHasEgg(int loc, bool hasEgg) => Data[Daycare + 0x1E0] = (byte)(hasEgg ? 1 : 0);
        public override void SetDaycareOccupied(int loc, int slot, bool occupied) => Data[Daycare + ((SIZE_STORED + 8) * slot)] = (byte)(occupied ? 1 : 0);
        public override void SetDaycareEXP(int loc, int slot, uint EXP) => BitConverter.GetBytes(EXP).CopyTo(Data, Daycare + 4 + ((SIZE_STORED + 8) * slot));

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

            Util.GetBytesFromHexString(seed).CopyTo(Data, Daycare + 0x1E8);
        }

        public override string JPEGTitle => HasJPPEGData ? string.Empty : Util.TrimFromZero(Encoding.Unicode.GetString(Data, JPEG, 0x1A));
        public override byte[] JPEGData => HasJPPEGData ? Array.Empty<byte>() : GetData(JPEG + 0x54, 0xE004);

        private bool HasJPPEGData => Data[JPEG + 0x54] == 0xFF;

        // Gym History
        public ushort[][] GymTeams
        {
            get
            {
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
                const int teamsize = 2 * 6; // 2byte/species, 6species/team
                const int size = teamsize * 8; // 8 gyms
                int ofs = SUBE - size - 4;

                byte[] data = new byte[size];
                for (int i = 0; i < value.Length; i++)
                    Buffer.BlockCopy(value[i], 0, data, teamsize * i, teamsize);
                SetData(data, ofs);
            }
        }

        public void UnlockAllFriendSafariSlots()
        {
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
            SetData(AllAccessories, Accessories);
        }

        private static readonly byte[] AllAccessories =
        {
            0xFE,0xFF,0xFF,0x7E,0xFF,0xFD,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,
            0xFF,0xEF,0xFF,0xFF,0xFF,0xF9,0xFF,0xFB,0xFF,0xF7,0xFF,0xFF,0x0F,0x00,0x00,0x00,
            0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0xFE,0xFF,
            0xFF,0x7E,0xFF,0xFD,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xEF,
            0xFF,0xFF,0xFF,0xF9,0xFF,0xFB,0xFF,0xF7,0xFF,0xFF,0x0F,0x00,0x00,0x00,0x00,0x00,
            0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x01,0x00,0x00,0x00,
            0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00
        };

        public override GameVersion Version
        {
            get
            {
                switch (Game)
                {
                    case (int)GameVersion.X: return GameVersion.X;
                    case (int)GameVersion.Y: return GameVersion.Y;
                }
                return GameVersion.Invalid;
            }
        }

        protected override bool[] MysteryGiftReceivedFlags { get => MysteryBlock.MysteryGiftReceivedFlags; set => MysteryBlock.MysteryGiftReceivedFlags = value; }
        protected override MysteryGift[] MysteryGiftCards { get => MysteryBlock.MysteryGiftCards; set => MysteryBlock.MysteryGiftCards = value; }

        public byte[] LinkBlock
        {
            get => GetData(LinkInfo, 0xC48);
            set
            {
                if (value.Length != 0xC48)
                    throw new ArgumentException(nameof(value));
                value.CopyTo(Data, LinkInfo);
            }
        }

        public override bool GetCaught(int species) => Zukan.GetCaught(species);
        public override bool GetSeen(int species) => Zukan.GetSeen(species);
        public override void SetSeen(int species, bool seen) => Zukan.SetSeen(species, seen);
        public override void SetCaught(int species, bool caught) => Zukan.SetCaught(species, caught);

        public override int CurrentBox { get => BoxLayout.CurrentBox; set => BoxLayout.CurrentBox = value; }
        protected override int GetBoxWallpaperOffset(int box) => BoxLayout.GetBoxWallpaperOffset(box);
        public override int BoxesUnlocked { get => BoxLayout.BoxesUnlocked; set => BoxLayout.BoxesUnlocked = value; }
        public override byte[] BoxFlags { get => BoxLayout.BoxFlags; set => BoxLayout.BoxFlags = value; }

        public override bool BattleBoxLocked
        {
            get => BattleBoxBlock.Locked;
            set => BattleBoxBlock.Locked = value;
        }
    }
}
