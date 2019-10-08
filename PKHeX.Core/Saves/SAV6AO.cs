using System;
using System.Linq;
using System.Text;

namespace PKHeX.Core
{
    /// <summary>
    /// Generation 6 <see cref="SaveFile"/> object for <see cref="GameVersion.ORAS"/>.
    /// </summary>
    /// <inheritdoc cref="SAV6" />
    public sealed class SAV6AO : SAV6, IPokePuff, IOPower, ILink
    {
        public SAV6AO(byte[] data) : base(data, BlocksAO, boAO) => Initialize();

        public SAV6AO() : base(SaveUtil.SIZE_G6ORAS, BlocksAO, boAO)
        {
            Initialize();
            ClearBoxes();
        }

        public override SaveFile Clone() => new SAV6AO((byte[])Data.Clone());
        public override int MaxMoveID => Legal.MaxMoveID_6_AO;
        public override int MaxItemID => Legal.MaxItemID_6_AO;
        public override int MaxAbilityID => Legal.MaxAbilityID_6_AO;

        private const int boAO = SaveUtil.SIZE_G6ORAS - 0x200;

        public static readonly BlockInfo[] BlocksAO =
        {
            new BlockInfo6 (boAO, 00, 0x00000, 0x002C8),
            new BlockInfo6 (boAO, 01, 0x00400, 0x00B90),
            new BlockInfo6 (boAO, 02, 0x01000, 0x0002C),
            new BlockInfo6 (boAO, 03, 0x01200, 0x00038),
            new BlockInfo6 (boAO, 04, 0x01400, 0x00150),
            new BlockInfo6 (boAO, 05, 0x01600, 0x00004),
            new BlockInfo6 (boAO, 06, 0x01800, 0x00008),
            new BlockInfo6 (boAO, 07, 0x01A00, 0x001C0),
            new BlockInfo6 (boAO, 08, 0x01C00, 0x000BE),
            new BlockInfo6 (boAO, 09, 0x01E00, 0x00024),
            new BlockInfo6 (boAO, 10, 0x02000, 0x02100),
            new BlockInfo6 (boAO, 11, 0x04200, 0x00130),
            new BlockInfo6 (boAO, 12, 0x04400, 0x00440),
            new BlockInfo6 (boAO, 13, 0x04A00, 0x00574),
            new BlockInfo6 (boAO, 14, 0x05000, 0x04E28),
            new BlockInfo6 (boAO, 15, 0x0A000, 0x04E28),
            new BlockInfo6 (boAO, 16, 0x0F000, 0x04E28),
            new BlockInfo6 (boAO, 17, 0x14000, 0x00170),
            new BlockInfo6 (boAO, 18, 0x14200, 0x0061C),
            new BlockInfo6 (boAO, 19, 0x14A00, 0x00504),
            new BlockInfo6 (boAO, 20, 0x15000, 0x011CC),
            new BlockInfo6 (boAO, 21, 0x16200, 0x00644),
            new BlockInfo6 (boAO, 22, 0x16A00, 0x00104),
            new BlockInfo6 (boAO, 23, 0x16C00, 0x00004),
            new BlockInfo6 (boAO, 24, 0x16E00, 0x00420),
            new BlockInfo6 (boAO, 25, 0x17400, 0x00064),
            new BlockInfo6 (boAO, 26, 0x17600, 0x003F0),
            new BlockInfo6 (boAO, 27, 0x17A00, 0x0070C),
            new BlockInfo6 (boAO, 28, 0x18200, 0x00180),
            new BlockInfo6 (boAO, 29, 0x18400, 0x00004),
            new BlockInfo6 (boAO, 30, 0x18600, 0x0000C),
            new BlockInfo6 (boAO, 31, 0x18800, 0x00048),
            new BlockInfo6 (boAO, 32, 0x18A00, 0x00054),
            new BlockInfo6 (boAO, 33, 0x18C00, 0x00644),
            new BlockInfo6 (boAO, 34, 0x19400, 0x005C8),
            new BlockInfo6 (boAO, 35, 0x19A00, 0x002F8),
            new BlockInfo6 (boAO, 36, 0x19E00, 0x01B40),
            new BlockInfo6 (boAO, 37, 0x1BA00, 0x001F4),
            new BlockInfo6 (boAO, 38, 0x1BC00, 0x003E0),
            new BlockInfo6 (boAO, 39, 0x1C000, 0x00216),
            new BlockInfo6 (boAO, 40, 0x1C400, 0x00640),
            new BlockInfo6 (boAO, 41, 0x1CC00, 0x01A90),
            new BlockInfo6 (boAO, 42, 0x1E800, 0x00400),
            new BlockInfo6 (boAO, 43, 0x1EC00, 0x00618),
            new BlockInfo6 (boAO, 44, 0x1F400, 0x0025C),
            new BlockInfo6 (boAO, 45, 0x1F800, 0x00834),
            new BlockInfo6 (boAO, 46, 0x20200, 0x00318),
            new BlockInfo6 (boAO, 47, 0x20600, 0x007D0),
            new BlockInfo6 (boAO, 48, 0x20E00, 0x00C48),
            new BlockInfo6 (boAO, 49, 0x21C00, 0x00078),
            new BlockInfo6 (boAO, 50, 0x21E00, 0x00200),
            new BlockInfo6 (boAO, 51, 0x22000, 0x00C84),
            new BlockInfo6 (boAO, 52, 0x22E00, 0x00628),
            new BlockInfo6 (boAO, 53, 0x23600, 0x00400),
            new BlockInfo6 (boAO, 54, 0x23A00, 0x07AD0),
            new BlockInfo6 (boAO, 55, 0x2B600, 0x078B0),
            new BlockInfo6 (boAO, 56, 0x33000, 0x34AD0),
            new BlockInfo6 (boAO, 57, 0x67C00, 0x0E058),
        };

        private void Initialize()
        {
            /* 00: 00000-002C8, 002C8 */ // Puff = 0x00000;
            /* 01: 00400-00F90, 00B90 */ // MyItem = 0x00400; // Bag
            /* 02: 01000-0102C, 0002C */ // ItemInfo = 0x1000; // Select Bound Items
            /* 03: 01200-01238, 00038 */ // GameTime = 0x01200;
            /* 04: 01400-01550, 00150 */ Trainer1 = 0x01400; // Situation
            /* 05: 01600-01604, 00004 */ // RandomGroup (rand seeds)
            /* 06: 01800-01808, 00008 */ // PlayTime = 0x1800; // PlayTime
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
            /* 17: 14000-14170, 00170 */ // MyStatus
            /* 18: 14200-1481C, 0061C */ Party = 0x14200; // PokePartySave
            /* 19: 14A00-14F04, 00504 */ EventConst = 0x14A00; // EventWork
            /* 20: 15000-161CC, 011CC */ PokeDex = 0x15000; // ZukanData
            /* 21: 16200-16844, 00644 */ // hologram clips
            /* 22: 16A00-16B04, 00104 */ Fused = 0x16A00; // UnionPokemon
            /* 23: 16C00-16C04, 00004 */ // ConfigSave
            /* 24: 16E00-17220, 00420 */ // Amie decoration stuff
            /* 25: 17400-17464, 00064 */ // OPower = 0x17400;
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
            /* 44: 1F400-1F65C, 0025C */ // Record = 0x1F400;
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

            Items = new MyItem6AO(this, 0x00400);
            PuffBlock = new Puff6(this, 0x0000);
            GameTime = new GameTime6(this, 0x01200);
            Situation = new Situation6(this, 0x01400);
            Played = new PlayTime6(this, 0x01800);
            BoxLayout = new BoxLayout6(this, 0x04400);
            BattleBoxBlock = new BattleBox6(this, 0x04A00);
            Status = new MyStatus6(this, 0x14000);
            Zukan = new Zukan6AO(this, 0x15000, 0x400);
            OPowerBlock = new OPower6(this, 0x17400);
            MysteryBlock = new MysteryBlock6(this, 0x1CC00);
            Records = new Record6(this, 0x1F400, Core.Records.MaxType_AO);
            Sango = new SangoInfoBlock(this, 0x2B600);

            EventFlag = EventConst + 0x2FC;
            WondercardData = WondercardFlags + 0x100;
            Daycare2 = Daycare + 0x1F0;

            HeldItems = Legal.HeldItem_AO;
            Personal = PersonalTable.AO;
        }

        public int EonTicket { get; private set; }
        public int Contest { get; private set; }
        private int Daycare2 { get; set; }
        public int SecretBase { get; private set; }
        public int GTS { get; private set; }
        public int Fused { get; private set; }

        public Zukan6 Zukan { get; private set; }
        public Puff6 PuffBlock { get; private set; }
        public OPower6 OPowerBlock { get; private set; }
        public BoxLayout6 BoxLayout { get; private set; }
        public MysteryBlock6 MysteryBlock { get; private set; }
        public SangoInfoBlock Sango { get; set; }
        public BattleBox6 BattleBoxBlock { get; private set; }

        public override GameVersion Version
        {
            get
            {
                return Game switch
                {
                    (int)GameVersion.AS => GameVersion.AS,
                    (int)GameVersion.OR => GameVersion.OR,
                    _ => GameVersion.Invalid
                };
            }
        }

        public override bool GetCaught(int species) => Zukan.GetCaught(species);
        public override bool GetSeen(int species) => Zukan.GetSeen(species);
        public override void SetSeen(int species, bool seen) => Zukan.SetSeen(species, seen);
        public override void SetCaught(int species, bool caught) => Zukan.SetCaught(species, caught);
        protected override void SetDex(PKM pkm) => Zukan.SetDex(pkm);

        // Daycare
        public override int DaycareSeedSize => 16;
        public override bool HasTwoDaycares => true;

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

            Util.GetBytesFromHexString(seed).CopyTo(Data, Daycare + 0x1E8);
        }

        public override void SetDaycareHasEgg(int loc, bool hasEgg)
        {
            int ofs = loc == 0 ? Daycare : Daycare2;
            if (ofs > -1)
                Data[ofs + 0x1E0] = (byte)(hasEgg ? 1 : 0);
        }

        public override string JPEGTitle => HasJPPEGData ? string.Empty : Util.TrimFromZero(Encoding.Unicode.GetString(Data, JPEG, 0x1A));
        public override byte[] JPEGData => HasJPPEGData ? Array.Empty<byte>() : GetData(JPEG + 0x54, 0xE004);

        private bool HasJPPEGData => Data[JPEG + 0x54] == 0xFF;

        protected override bool[] MysteryGiftReceivedFlags { get => MysteryBlock.MysteryGiftReceivedFlags; set => MysteryBlock.MysteryGiftReceivedFlags = value; }
        protected override MysteryGift[] MysteryGiftCards { get => MysteryBlock.MysteryGiftCards; set => MysteryBlock.MysteryGiftCards = value; }

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

        public byte[] LinkBlock
        {
            get => GetData(LinkInfo, 0xC48);
            set
            {
                if (value.Length != 0xC48)
                    throw new ArgumentException(nameof(value));
                SetData(value, LinkInfo);
            }
        }

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
