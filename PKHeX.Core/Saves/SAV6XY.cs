using System;
using System.Collections.Generic;
using System.Text;

namespace PKHeX.Core
{
    /// <summary>
    /// Generation 6 <see cref="SaveFile"/> object for <see cref="GameVersion.XY"/>.
    /// </summary>
    /// <inheritdoc cref="SAV6" />
    public sealed class SAV6XY : SAV6, ISaveBlock6Main
    {
        public SAV6XY(byte[] data) : base(data, SaveBlockAccessorXY.boXY)
        {
            Blocks = new SaveBlockAccessorXY(this);
            Initialize();
        }

        public SAV6XY() : base(SaveUtil.SIZE_G6XY, SaveBlockAccessorXY.boXY)
        {
            Blocks = new SaveBlockAccessorXY(this);
            Initialize();
            ClearBoxes();
        }

        public override PersonalTable Personal => PersonalTable.XY;
        public override IReadOnlyList<ushort> HeldItems => Legal.HeldItem_XY;
        public SaveBlockAccessorXY Blocks { get; }
        public override SaveFile Clone() => new SAV6XY((byte[])Data.Clone());
        public override int MaxMoveID => Legal.MaxMoveID_6_XY;
        public override int MaxItemID => Legal.MaxItemID_6_XY;
        public override int MaxAbilityID => Legal.MaxAbilityID_6_XY;

        private void Initialize()
        {
            // Enable Features
            Party = 0x14200;
            PCLayout = 0x4400;
            BattleBox = 0x04A00;
            PSS = 0x05000;
            EventConst = 0x14A00;
            PokeDex = 0x15000;
            HoF = 0x19400;
            MaisonStats = 0x1B1C0;
            Daycare = 0x1B200;
            BerryField = 0x1B800;
            WondercardFlags = 0x1BC00;
            Box = 0x22600;
            JPEG = 0x57200;

            EventFlag = EventConst + 0x2FC;
            WondercardData = WondercardFlags + 0x100;

            // Extra Viewable Slots
            Fused = 0x16000;
            GTS = 0x17800;
            SUBE = 0x1D890;
        }

        public int GTS { get; private set; } = int.MinValue;
        public int Fused { get; private set; } = int.MinValue;

        #region Blocks
        public override IReadOnlyList<BlockInfo> AllBlocks => Blocks.BlockInfo;
        public override MyItem Items => Blocks.Items;
        public override ItemInfo6 ItemInfo => Blocks.ItemInfo;
        public override GameTime6 GameTime => Blocks.GameTime;
        public override Situation6 Situation => Blocks.Situation;
        public override PlayTime6 Played => Blocks.Played;
        public override MyStatus6 Status => Blocks.Status;
        public override Record6 Records => Blocks.Records;
        public Puff6 PuffBlock => Blocks.PuffBlock;
        public OPower6 OPowerBlock => Blocks.OPowerBlock;
        public Link6 LinkBlock => Blocks.LinkBlock;
        public BoxLayout6 BoxLayout => Blocks.BoxLayout;
        public BattleBox6 BattleBoxBlock => Blocks.BattleBoxBlock;
        public MysteryBlock6 MysteryBlock => Blocks.MysteryBlock;
        public SuperTrainBlock SuperTrain => Blocks.SuperTrain;
        #endregion

        protected override void SetDex(PKM pkm) => Blocks.Zukan.SetDex(pkm);

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
        
        public override GameVersion Version
        {
            get
            {
                return Game switch
                {
                    (int)GameVersion.X => GameVersion.X,
                    (int)GameVersion.Y => GameVersion.Y,
                    _ => GameVersion.Invalid
                };
            }
        }

        protected override bool[] MysteryGiftReceivedFlags { get => Blocks.MysteryBlock.MysteryGiftReceivedFlags; set => Blocks.MysteryBlock.MysteryGiftReceivedFlags = value; }
        protected override DataMysteryGift[] MysteryGiftCards { get => Blocks.MysteryBlock.MysteryGiftCards; set => Blocks.MysteryBlock.MysteryGiftCards = value; }
        
        public override bool GetCaught(int species) => Blocks.Zukan.GetCaught(species);
        public override bool GetSeen(int species) => Blocks.Zukan.GetSeen(species);
        public override void SetSeen(int species, bool seen) => Blocks.Zukan.SetSeen(species, seen);
        public override void SetCaught(int species, bool caught) => Blocks.Zukan.SetCaught(species, caught);

        public override int CurrentBox { get => Blocks.BoxLayout.CurrentBox; set => Blocks.BoxLayout.CurrentBox = value; }
        protected override int GetBoxWallpaperOffset(int box) => Blocks.BoxLayout.GetBoxWallpaperOffset(box);
        public override int BoxesUnlocked { get => Blocks.BoxLayout.BoxesUnlocked; set => Blocks.BoxLayout.BoxesUnlocked = value; }
        public override byte[] BoxFlags { get => Blocks.BoxLayout.BoxFlags; set => Blocks.BoxLayout.BoxFlags = value; }

        public override bool BattleBoxLocked
        {
            get => Blocks.BattleBoxBlock.Locked;
            set => Blocks.BattleBoxBlock.Locked = value;
        }

        public override uint Money { get => Blocks.Misc.Money; set => Blocks.Misc.Money = value; }
        public override int Vivillon { get => Blocks.Misc.Vivillon; set => Blocks.Misc.Vivillon = value; }
        public override int Badges { get => Blocks.Misc.Badges; set => Blocks.Misc.Badges = value; }
        public override int BP { get => Blocks.Misc.BP; set => Blocks.Misc.BP = value; }
    }
}
